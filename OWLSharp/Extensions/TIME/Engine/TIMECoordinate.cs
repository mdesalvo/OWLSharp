/*
   Copyright 2012-2024 Marco De Salvo

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
    /// Represents the definition of a temporal coordinate encoded in standard 6-dimensions
    /// </summary>
    public class TIMECoordinate : IComparable<TIMECoordinate>, IEquatable<TIMECoordinate>
    {
        #region Built-Ins
        /// <summary>
        /// Gets a temporal coordinate with all components set to zero
        /// </summary>
        public static readonly TIMECoordinate Zero = new TIMECoordinate(0, 0, 0, 0, 0, 0);

        /// <summary>
        /// Gets a temporal coordinate with components set to Unix Time origin (1st January 1970)
        /// </summary>
        public static readonly TIMECoordinate UnixTime = new TIMECoordinate(1970, 1, 1, 0, 0, 0);

        /// <summary>
        /// Gets a temporal coordinate with components set to Geologic Time present (1st January 1950)
        /// </summary>
        public static readonly TIMECoordinate GeologicTime = new TIMECoordinate(1950, 1, 1, 0, 0, 0);

        /// <summary>
        /// Gets a temporal coordinate with components set to GPS Time origin (6st January 1980)
        /// </summary>
        public static readonly TIMECoordinate GPSTime = new TIMECoordinate(1980, 1, 6, 0, 0, 0);
        #endregion

        #region Properties
        /// <summary>
        /// Year component of the temporal coordinate
        /// </summary>
        public double? Year { get; internal set; }

        /// <summary>
        /// Month component of the temporal coordinate (cannot be negative)
        /// </summary>
        public double? Month { get; internal set; }

        /// <summary>
        /// Day component of the temporal coordinate (cannot be negative)
        /// </summary>
        public double? Day { get; internal set; }

        /// <summary>
        /// Hour component of the temporal coordinate (cannot be negative)
        /// </summary>
        public double? Hour { get; internal set; }

        /// <summary>
        /// Minute component of the temporal coordinate (cannot be negative)
        /// </summary>
        public double? Minute { get; internal set; }

        /// <summary>
        /// Second component of the temporal coordinate (cannot be negative)
        /// </summary>
        public double? Second { get; internal set; }

        /// <summary>
        /// Metadata describing the metrics of the temporal coordinate and its textual decorators for calendar placement
        /// </summary>
        public TIMECoordinateMetadata Metadata { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Internal-ctor for initializing an empty temporal coordinate
        /// </summary>
        internal TIMECoordinate() { }

        /// <summary>
        /// Builds a temporal coordinate with the given components and optional metadata
        /// </summary>
        public TIMECoordinate(double? year, double? month, double? day, 
            double? hour, double? minute, double? second, TIMECoordinateMetadata metadata=null)
        {
            #region Guards
            if (month < 0)
                throw new OWLException("Cannot create temporal coordinate because given \"month\" parameter is negative");
            if (day < 0)
                throw new OWLException("Cannot create temporal coordinate because given \"day\" parameter is negative");
            if (hour < 0)
                throw new OWLException("Cannot create temporal coordinate because given \"hour\" parameter is negative");
            if (minute < 0)
                throw new OWLException("Cannot create temporal coordinate because given \"minute\" parameter is negative");
            if (second < 0)
                throw new OWLException("Cannot create temporal coordinate because given \"second\" parameter is negative");
            #endregion

            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            Metadata = metadata ?? new TIMECoordinateMetadata();
        }

        /// <summary>
        /// Builds a temporal coordinate with components extracted from the given UTC DateTime
        /// </summary>
        public TIMECoordinate(DateTime dateTime)
        {
            DateTime utcDateTime = dateTime.ToUniversalTime();

            Year = utcDateTime.Year;
            Month = utcDateTime.Month;
            Day = utcDateTime.Day;
            Hour = utcDateTime.Hour;
            Minute = utcDateTime.Minute;
            Second = utcDateTime.Second;
            Metadata = new TIMECoordinateMetadata(
                TIMECalendarReferenceSystem.Gregorian, 
                RDFVocabulary.TIME.UNIT_SECOND,
                TIMEOntologyHelper.GetMonthOfYear(utcDateTime.Month),
                TIMEOntologyHelper.GetDayOfWeek(utcDateTime.DayOfWeek),
                Convert.ToUInt32(utcDateTime.DayOfYear));
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Compares this temporal coordinate with the given one
        /// </summary>
        public int CompareTo(TIMECoordinate other)
        {
            if (other == null)
                return 1;

            //Compare year
            double thisYear = Year ?? Zero.Year.Value;
            double otherYear = other.Year ?? Zero.Year.Value;
            if (thisYear != otherYear)
                return thisYear.CompareTo(otherYear);

            //Compare month
            double thisMonth = Month ?? Zero.Month.Value;
            double otherMonth = other.Month ?? Zero.Month.Value;
            if (thisMonth != otherMonth)
                return thisMonth.CompareTo(otherMonth);

            //Compare day
            double thisDay = Day ?? Zero.Day.Value;
            double otherDay = other.Day ?? Zero.Day.Value;
            if (thisDay != otherDay)
                return thisDay.CompareTo(otherDay);

            //Compare hour
            double thisHour = Hour ?? Zero.Hour.Value;
            double otherHour = other.Hour ?? Zero.Hour.Value;
            if (thisHour != otherHour)
                return thisHour.CompareTo(otherHour);

            //Compare minute
            double thisMinute = Minute ?? Zero.Minute.Value;
            double otherMinute = other.Minute ?? Zero.Minute.Value;
            if (thisMinute != otherMinute)
                return thisMinute.CompareTo(otherMinute);

            //Compare second
            double thisSecond = Second ?? Zero.Second.Value;
            double otherSecond = other.Second ?? Zero.Second.Value;
            if (thisSecond != otherSecond)
                return thisSecond.CompareTo(otherSecond);

            return 0;
        }

        /// <summary>
        /// Checks if this temporal coordinate is the same as given one
        /// </summary>
        public bool Equals(TIMECoordinate other)
            => CompareTo(other) == 0;
        #endregion

        #region Methods
        /// <summary>
        /// Gets a human-readable representation of this temporal coordinate
        /// </summary>
        public override string ToString()
            => $"{Year ?? 0}_{Month ?? 0}_{Day ?? 0}_{Hour ?? 0}_{Minute ?? 0}_{Second ?? 0}";
        #endregion
    }

    /// <summary>
    /// Extends the definition of a temporal coordinate with some metrics informations and some textual decorators for calendar placement
    /// </summary>
    public class TIMECoordinateMetadata
    {
        #region Properties
        /// <summary>
        /// Indicates the temporal reference system in which the temporal coordinate is encoded (e.g: Gregorian)
        /// </summary>
        public RDFResource TRS { get; internal set; }

        /// <summary>
        /// Indicates the temporal unit which provides the precision of a date-time value or scale of a temporal extent (e.g: Second)
        /// </summary>
        public RDFResource UnitType { get; internal set; }

        /// <summary>
        /// Decorates the coordinate with the month-of-year position of the time instant in a calendar-clock system (e.g: February)
        /// </summary>
        public RDFResource MonthOfYear { get; internal set; }

        /// <summary>
        /// Decorates the coordinate with the day-of-week position of the time instant in a calendar-clock system (e.g: Tuesday)
        /// </summary>
        public RDFResource DayOfWeek { get; internal set; }

        /// <summary>
        /// Decorates the coordinate with the day-of-year position of the time instant in a calendar-clock system (e.g: 181)
        /// </summary>
        public uint? DayOfYear { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Internal-ctor for initializing an empty coordinate metadata
        /// </summary>
        internal TIMECoordinateMetadata() { }

        /// <summary>
        /// Builds a coordinate metadata with the given mandatory metrics (TRS, UnitType) and optional textual decorators for calendar placement
        /// </summary>
        public TIMECoordinateMetadata(RDFResource trsUri, RDFResource unitTypeUri, 
            RDFResource monthOfYear=null, RDFResource dayOfWeek=null, uint? dayOfYear=null)
        {
            #region Guards
            if (trsUri == null)
                throw new OWLException("Cannot create coordinate metadata because given \"trsUri\" parameter is null");
            if (unitTypeUri == null)
                throw new OWLException("Cannot create coordinate metadata because given \"unitTypeUri\" parameter is null");
            #endregion

            TRS = trsUri;
            UnitType = unitTypeUri;
            MonthOfYear = monthOfYear;
            DayOfWeek = dayOfWeek;
            DayOfYear = dayOfYear;
        }
        #endregion
    }
}