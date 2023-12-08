/*
   Copyright 2014-2024 Marco De Salvo

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using RDFSharp.Model;
using System;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// Represents an engine for conversion of coordinates and temporal extents beween different forms of representation
    /// </summary>
    public static class TIMEConverter
    {
        #region Methods
        /// <summary>
        /// From a numeric instant position encoded in the given positional TRS, it calculates a coordinate compatible with the metrics of the given calendar TRS.<br/><br/>
        /// If the given positional TRS works at large-scale, only the Year component of the result coordinate will be valorized.<br/>
        /// If the given calendar TRS ships with a metric configured for supporting the detection of leap years, it will be considered.
        /// </summary>
        public static TIMECoordinate PositionToCoordinate(double timePosition, TIMEPositionReferenceSystem positionTRS, TIMECalendarReferenceSystem calendarTRS)
        {
            #region Guards
            if (positionTRS == null)
                throw new OWLException("Cannot convert position to coordinate because given \"positionTRS\" parameter is null");
            if (calendarTRS == null)
                throw new OWLException("Cannot convert position to coordinate because given \"calendarTRS\" parameter is null");
            #endregion

            //Normalize origin of the given positional TRS according to the metrics of the given calendar TRS
            TIMECoordinate coordinate = NormalizeCoordinate(positionTRS.Origin, calendarTRS);

            //Scale the given time position of the factor specified by the unit of the given position TRS
            double scaledTimePosition = timePosition * positionTRS.Unit.ScaleFactor;

            #region Large-Scale
            if (positionTRS.IsLargeScaleTRS)
            {
                coordinate.Metadata = new TIMECoordinateMetadata(calendarTRS, RDFVocabulary.TIME.UNIT_YEAR);

                //Transform the scaled time position to years (since the large scale works at this level of detail)
                double timePositionYears =
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Second ? scaledTimePosition / calendarTRS.Metrics.SecondsInMinute / calendarTRS.Metrics.MinutesInHour / calendarTRS.Metrics.HoursInDay  / (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear) :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Minute ? scaledTimePosition / calendarTRS.Metrics.MinutesInHour   / calendarTRS.Metrics.HoursInDay    / (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear) :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Hour   ? scaledTimePosition / calendarTRS.Metrics.HoursInDay      / (calendarTRS.Metrics.DaysInYear   / calendarTRS.Metrics.MonthsInYear) :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Day    ? scaledTimePosition / (calendarTRS.Metrics.DaysInYear     / calendarTRS.Metrics.MonthsInYear) :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Month  ? scaledTimePosition / calendarTRS.Metrics.MonthsInYear :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Year   ? scaledTimePosition :
                    0;

                //Set year component and reset the others (unsenseful at large scale level)
                coordinate.Year = Math.Truncate(positionTRS.Origin.Year.Value + timePositionYears);
                coordinate.Month = null;
                coordinate.Day = null;
                coordinate.Hour = null;
                coordinate.Minute = null;
                coordinate.Second = null;
            }
            #endregion

            #region Little-Scale
            else
            {
                coordinate.Metadata = new TIMECoordinateMetadata(calendarTRS, RDFVocabulary.TIME.UNIT_SECOND);

                //Transform the scaled time position to seconds (since the clock emulator works at this level of detail)
                double scaledTimePositionSeconds =
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Second ? scaledTimePosition :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Minute ? scaledTimePosition * calendarTRS.Metrics.SecondsInMinute :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Hour   ? scaledTimePosition * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Day    ? scaledTimePosition * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Month  ? scaledTimePosition * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear) :
                    positionTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Year   ? scaledTimePosition * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * calendarTRS.Metrics.DaysInYear :
                    0;

                //Execute the clock emulator on the transformed time position
                if (scaledTimePositionSeconds > 0)
                    ClockForward(scaledTimePositionSeconds, coordinate, calendarTRS);
                else if (scaledTimePositionSeconds < 0)
                    ClockBackward(scaledTimePositionSeconds, coordinate, calendarTRS);
            }
            #endregion

            return coordinate;
        }

        /// <summary>
        /// Gets a coordinate which is the normalization of the given one according to the metrics of the given calendar TRS.<br/><br/>
        /// If this calendar TRS ships with a metric configured for supporting the detection of leap years, it will be considered.
        /// </summary>
        public static TIMECoordinate NormalizeCoordinate(TIMECoordinate timeCoordinate, TIMECalendarReferenceSystem calendarTRS)
        {
            #region Guards
            if (timeCoordinate == null)
                throw new OWLException("Cannot normalize coordinate because given \"timeExtent\" parameter is null");
            if (calendarTRS == null)
                throw new OWLException("Cannot normalize coordinate because given \"calendarTRS\" parameter is null");
            #endregion

            //Normalize second
            double coordinateSecond = timeCoordinate.Second ?? 0;
            double normalizedSecond = coordinateSecond % calendarTRS.Metrics.SecondsInMinute;
            double remainingMinutes = Math.Truncate(coordinateSecond / calendarTRS.Metrics.SecondsInMinute);
            //Normalize minute
            double coordinateMinute = (timeCoordinate.Minute ?? 0) + remainingMinutes;
            double normalizedMinute = coordinateMinute % calendarTRS.Metrics.MinutesInHour;
            double remainingHours = Math.Truncate(coordinateMinute / calendarTRS.Metrics.MinutesInHour);
            //Normalize hour
            double coordinateHour = (timeCoordinate.Hour ?? 0) + remainingHours;
            double normalizedHour = coordinateHour % calendarTRS.Metrics.HoursInDay;
            double remainingDays = Math.Truncate(coordinateHour / calendarTRS.Metrics.HoursInDay);

            //Pre-Normalize month
            double coordinateMonth = timeCoordinate.Month ?? 0;
            if (coordinateMonth < 1) //Calendar month start at 1
                coordinateMonth = 1;
            double normalizedMonth = coordinateMonth % calendarTRS.Metrics.MonthsInYear;
            if (normalizedMonth == 0)
                normalizedMonth = calendarTRS.Metrics.MonthsInYear;
            double remainingYears = Math.Truncate(coordinateMonth / calendarTRS.Metrics.MonthsInYear);
            if (coordinateMonth % calendarTRS.Metrics.MonthsInYear == 0)
                remainingYears--;

            //Normalize day
            double coordinateDay = timeCoordinate.Day ?? 0;
            if (coordinateDay < 1) //Calendar day start at 1
                coordinateDay = 1;
            coordinateDay += remainingDays;
            uint[] metricsMonths = calendarTRS.Metrics.LeapYearRule?.Invoke((timeCoordinate.Year ?? 0) + remainingYears)
                                     ?? calendarTRS.Metrics.Months;
            double daysOfNormalizedMonth = metricsMonths[Convert.ToUInt32(normalizedMonth) - 1];
            double normalizedDay = coordinateDay % daysOfNormalizedMonth;
            if (normalizedDay == 0)
                normalizedDay = daysOfNormalizedMonth;
            double remainingMonths = Math.Truncate(coordinateDay / daysOfNormalizedMonth);
            if (coordinateDay % daysOfNormalizedMonth == 0)
                remainingMonths--;

            //Post-Normalize month
            while (remainingMonths > 0)
            {
                normalizedMonth += remainingMonths;
                remainingMonths = 0;
                double postNormalizedMonth = normalizedMonth % calendarTRS.Metrics.MonthsInYear;
                if (postNormalizedMonth == 0)
                    postNormalizedMonth = calendarTRS.Metrics.MonthsInYear;
                remainingYears += Math.Truncate(normalizedMonth / calendarTRS.Metrics.MonthsInYear);
                if (normalizedMonth % calendarTRS.Metrics.MonthsInYear == 0)
                    remainingYears--;
                normalizedMonth = postNormalizedMonth;

                //If normalizedDay exceeds metrics configured for normalizedMonth,
                //we need an extra iteration to further normalize the situation 
                daysOfNormalizedMonth = metricsMonths[Convert.ToUInt32(normalizedMonth) - 1];
                if (normalizedDay > daysOfNormalizedMonth)
                {
                    normalizedDay = 1;
                    remainingMonths++;
                }
            }

            //Normalize year
            double normalizedYear = (timeCoordinate.Year ?? 0) + remainingYears;

            return new TIMECoordinate(Math.Truncate(normalizedYear), Math.Truncate(normalizedMonth), Math.Truncate(normalizedDay),
                Math.Truncate(normalizedHour), Math.Truncate(normalizedMinute), normalizedSecond) { 
                Metadata = new TIMECoordinateMetadata(calendarTRS, RDFVocabulary.TIME.UNIT_SECOND) };
        }

        /// <summary>
        /// From a numeric interval duration encoded in the given unit type, it calculates an extent compatible with the metrics of the given calendar TRS.<br/><br/>
        /// If the given calendar TRS ships with inexact metric, calendarized components (Months, Years) will be cumulated in Days component.
        /// </summary>
        public static TIMEExtent DurationToExtent(double timeDuration, TIMEUnit unitType, TIMECalendarReferenceSystem calendarTRS)
        {
            #region Guards
            if (timeDuration < 0)
                throw new OWLException("Cannot convert duration to extent because given \"timeDuration\" parameter must be greater or equal than zero");
            if (unitType == null)
                throw new OWLException("Cannot convert duration to extent because given \"unitType\" parameter is null");
            if (calendarTRS == null)
                throw new OWLException("Cannot convert duration to extent because given \"calendarTRS\" parameter is null");
            #endregion

            TIMEExtent extent = new TIMEExtent() { Metadata = new TIMEExtentMetadata(calendarTRS) };

            //Scale the given time duration of the factor specified by its unit type
            double scaledTimeDuration = timeDuration * unitType.ScaleFactor;

            //Transform the scaled time duration to seconds (since duration works at this level of detail)
            double timeDurationSeconds =
                unitType.UnitType == TIMEEnums.TIMEUnitType.Second ? scaledTimeDuration :
                unitType.UnitType == TIMEEnums.TIMEUnitType.Minute ? scaledTimeDuration * calendarTRS.Metrics.SecondsInMinute :
                unitType.UnitType == TIMEEnums.TIMEUnitType.Hour   ? scaledTimeDuration * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour :
                unitType.UnitType == TIMEEnums.TIMEUnitType.Day    ? scaledTimeDuration * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay :
                unitType.UnitType == TIMEEnums.TIMEUnitType.Month  ? scaledTimeDuration * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear) :
                unitType.UnitType == TIMEEnums.TIMEUnitType.Year   ? scaledTimeDuration * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * calendarTRS.Metrics.DaysInYear :
                0;

            //Extract components of the time extent according to the metrics of the given calendar TRS
            //Calendarization (Months, Years) can be calculated only if the given calendar TRS has exact metric
            extent.Seconds = Math.Truncate(timeDurationSeconds  % calendarTRS.Metrics.SecondsInMinute);
            extent.Minutes = Math.Truncate((timeDurationSeconds / calendarTRS.Metrics.SecondsInMinute) % calendarTRS.Metrics.MinutesInHour);
            extent.Hours   = Math.Truncate((timeDurationSeconds / calendarTRS.Metrics.SecondsInMinute  / calendarTRS.Metrics.MinutesInHour) % calendarTRS.Metrics.HoursInDay);
            extent.Days    = Math.Truncate((timeDurationSeconds / calendarTRS.Metrics.SecondsInMinute  / calendarTRS.Metrics.MinutesInHour  / calendarTRS.Metrics.HoursInDay));
            extent.Months  = 0;
            extent.Weeks   = 0;
            extent.Years   = 0;
            if (calendarTRS.Metrics.HasExactMetric)
            {
                extent.Days %= (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear);
                extent.Months = Math.Truncate((timeDurationSeconds / calendarTRS.Metrics.SecondsInMinute / calendarTRS.Metrics.MinutesInHour / calendarTRS.Metrics.HoursInDay / (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear)) % calendarTRS.Metrics.MonthsInYear);
                extent.Years = Math.Truncate((timeDurationSeconds / calendarTRS.Metrics.SecondsInMinute / calendarTRS.Metrics.MinutesInHour / calendarTRS.Metrics.HoursInDay / calendarTRS.Metrics.DaysInYear) % calendarTRS.Metrics.DaysInYear);
            }

            return extent;
        }

        /// <summary>
        /// Gets an extent which is the normalization of the given one according to the metrics of the given calendar TRS.<br/><br/>
        /// Normalized time extents cumulate their Months, Weeks and Years components into Days component.
        /// </summary>
        public static TIMEExtent NormalizeExtent(TIMEExtent timeExtent, TIMECalendarReferenceSystem calendarTRS)
        {
            #region Guards
            if (timeExtent == null)
                throw new OWLException("Cannot normalize extent because given \"timeExtent\" parameter is null");
            if (calendarTRS == null)
                throw new OWLException("Cannot normalize extent because given \"calendarTRS\" parameter is null");
            #endregion

            //Transform the components to seconds (since duration works at this level of detail)
            double timeExtentSeconds = timeExtent.Seconds ?? 0;
            timeExtentSeconds += ((timeExtent.Minutes ?? 0) * calendarTRS.Metrics.SecondsInMinute);
            timeExtentSeconds += ((timeExtent.Hours ?? 0)   * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour);
            timeExtentSeconds += ((timeExtent.Days ?? 0)    * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay);
            timeExtentSeconds += ((timeExtent.Weeks ?? 0)   * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * TIMEUnit.Week.ScaleFactor);
            timeExtentSeconds += ((timeExtent.Months ?? 0)  * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear));
            timeExtentSeconds += ((timeExtent.Years ?? 0)   * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * calendarTRS.Metrics.DaysInYear);

            //Obtain an equivalent calendar TRS with forced inexact metrics: this is needed to suppress
            //eventual representation of Years, Months and Weeks (which all cumulate into Days)
            TIMECalendarReferenceSystem inexactCalendarTRS = new TIMECalendarReferenceSystem(calendarTRS,
                new TIMECalendarReferenceSystemMetrics(calendarTRS.Metrics.SecondsInMinute, calendarTRS.Metrics.MinutesInHour, calendarTRS.Metrics.HoursInDay, calendarTRS.Metrics.Months));
            inexactCalendarTRS.Metrics.HasExactMetric = false;

            return DurationToExtent(timeExtentSeconds, TIMEUnit.Second, calendarTRS);
        }

        /// <summary>
        /// Calculates the extent between the given coordinates, which is normalized according to the metrics of the given calendar TRS.<br/><br/>
        /// If this calendar TRS ships with a metric configured for supporting the detection of leap years, it will not be considered.
        /// </summary>
        public static TIMEExtent CalculateExtent(TIMECoordinate timeCoordinateStart, TIMECoordinate timeCoordinateEnd, TIMECalendarReferenceSystem calendarTRS)
        {
            #region Guards
            if (timeCoordinateStart == null)
                throw new OWLException("Cannot get extent because given \"timeCoordinateStart\" parameter is null");
            if (timeCoordinateEnd == null)
                throw new OWLException("Cannot get extent because given \"timeCoordinateEnd\" parameter is null");
            if (calendarTRS == null)
                throw new OWLException("Cannot get extent because given \"calendarTRS\" parameter is null");
            #endregion

            //Normalize the given coordinates according to the metrics of the given calendar TRS
            TIMECoordinate normalizedStart = NormalizeCoordinate(timeCoordinateStart, calendarTRS);
            TIMECoordinate normalizedEnd = NormalizeCoordinate(timeCoordinateEnd, calendarTRS);

            //Determine if the extent would be negative: if so, just swap the parameters
            if (normalizedStart.CompareTo(normalizedEnd) > -1)
            {
                TIMECoordinate swapCoordinate = new TIMECoordinate(normalizedStart.Year, normalizedStart.Month, normalizedStart.Day,
                    normalizedStart.Hour, normalizedStart.Minute, normalizedStart.Second, new TIMECoordinateMetadata() { 
                        TRS = normalizedStart.Metadata.TRS, UnitType = normalizedStart.Metadata.UnitType });
                normalizedStart = normalizedEnd;
                normalizedEnd = swapCoordinate;
            }

            //Reduce start coordinate to seconds
            double normalizedStartSeconds = normalizedStart.Second.Value
                                             + (normalizedStart.Minute.Value * calendarTRS.Metrics.SecondsInMinute)
                                             + (normalizedStart.Hour.Value   * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour)
                                             + (normalizedStart.Day.Value    * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay)
                                             + (normalizedStart.Month.Value  * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear))
                                             + (normalizedStart.Year.Value   * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * calendarTRS.Metrics.DaysInYear);

            //Reduce end coordinate to seconds
            double normalizedEndSeconds =  normalizedEnd.Second.Value
                                             + (normalizedEnd.Minute.Value   * calendarTRS.Metrics.SecondsInMinute)
                                             + (normalizedEnd.Hour.Value     * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour)
                                             + (normalizedEnd.Day.Value      * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay)
                                             + (normalizedEnd.Month.Value    * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * (calendarTRS.Metrics.DaysInYear / calendarTRS.Metrics.MonthsInYear))
                                             + (normalizedEnd.Year.Value     * calendarTRS.Metrics.SecondsInMinute * calendarTRS.Metrics.MinutesInHour * calendarTRS.Metrics.HoursInDay * calendarTRS.Metrics.DaysInYear);

            //Calculate the vector distance of start/end coordinates
            double vectorDistance = Math.Sqrt(Math.Pow(normalizedEndSeconds - normalizedStartSeconds, 2));

            //Return extent equivalent to the vector distance
            TIMEExtent vectorDistanceExtent = DurationToExtent(vectorDistance, TIMEUnit.Second, calendarTRS);
            return vectorDistanceExtent;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Consumes the given time position in chunks of [calendarTRS.Metrics.SecondsInMinute] seconds by moving forward from the origin
        /// </summary>
        internal static void ClockForward(double secondsToConsume, TIMECoordinate positionTRSOrigin, TIMECalendarReferenceSystem calendarTRS)
        {
            uint[] metricsMonths = calendarTRS.Metrics.LeapYearRule?.Invoke(positionTRSOrigin.Year ?? 0) 
                                    ?? calendarTRS.Metrics.Months;
            while (secondsToConsume >= calendarTRS.Metrics.SecondsInMinute)
            {
                secondsToConsume -= calendarTRS.Metrics.SecondsInMinute;
                positionTRSOrigin.Second %= calendarTRS.Metrics.SecondsInMinute;
                positionTRSOrigin.Minute++;
                if (positionTRSOrigin.Minute < calendarTRS.Metrics.MinutesInHour)
                    continue;

                //Minute overflow => propagate to hour
                positionTRSOrigin.Minute = 0;
                positionTRSOrigin.Hour++;
                if (positionTRSOrigin.Hour < calendarTRS.Metrics.HoursInDay)
                    continue;

                //Hour overflow => propagate to day
                positionTRSOrigin.Hour = 0;
                positionTRSOrigin.Day++;
                if (positionTRSOrigin.Day <= metricsMonths[Convert.ToInt32(positionTRSOrigin.Month)-1])
                    continue;

                //Day overflow => propagate to month
                positionTRSOrigin.Day = 1;
                positionTRSOrigin.Month++;
                if (positionTRSOrigin.Month <= calendarTRS.Metrics.MonthsInYear)
                    continue;

                //Month overflow => propagate to year, fetch new metrics
                positionTRSOrigin.Month = 1;
                positionTRSOrigin.Year++;
                metricsMonths = calendarTRS.Metrics.LeapYearRule?.Invoke(positionTRSOrigin.Year ?? 0)
                                 ?? calendarTRS.Metrics.Months;
            }
            positionTRSOrigin.Second = Math.Truncate(positionTRSOrigin.Second.Value + secondsToConsume);
        }
        /// <summary>
        /// Consumes the given time position in chunks of [calendarTRS.Metrics.SecondsInMinute] seconds by moving backward from the origin
        /// </summary>
        internal static void ClockBackward(double secondsToConsume, TIMECoordinate positionTRSOrigin, TIMECalendarReferenceSystem calendarTRS)
        {
            uint[] metricsMonths = calendarTRS.Metrics.LeapYearRule?.Invoke(positionTRSOrigin.Year ?? 0)
                                    ?? calendarTRS.Metrics.Months;
            while (secondsToConsume < 0)
            {
                secondsToConsume += calendarTRS.Metrics.SecondsInMinute;
                positionTRSOrigin.Second %= calendarTRS.Metrics.SecondsInMinute;
                positionTRSOrigin.Minute--;
                if (positionTRSOrigin.Minute >= 0)
                    continue;

                //Minute underflow => propagate to hour
                positionTRSOrigin.Minute = calendarTRS.Metrics.MinutesInHour - 1;
                positionTRSOrigin.Hour--;
                if (positionTRSOrigin.Hour >= 0)
                    continue;

                //Hour underflow => propagate to day
                positionTRSOrigin.Hour = calendarTRS.Metrics.HoursInDay - 1;
                positionTRSOrigin.Day--;
                if (positionTRSOrigin.Day >= 1)
                    continue;

                //Day underflow => propagate to month
                positionTRSOrigin.Month--;
                if (positionTRSOrigin.Month == 0)
                {
                    //Month underflow => propagate to year, fetch new metrics
                    positionTRSOrigin.Month = calendarTRS.Metrics.MonthsInYear;
                    positionTRSOrigin.Year--;
                    metricsMonths = calendarTRS.Metrics.LeapYearRule?.Invoke(positionTRSOrigin.Year ?? 0)
                                     ?? calendarTRS.Metrics.Months;
                }
                positionTRSOrigin.Day = metricsMonths[Convert.ToInt32(positionTRSOrigin.Month)-1];
            }
            positionTRSOrigin.Second = Math.Truncate(positionTRSOrigin.Second.Value + secondsToConsume);
        }
        #endregion
    }
}