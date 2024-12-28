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
    public class TIMECoordinate : IComparable<TIMECoordinate>, IEquatable<TIMECoordinate>
    {
        #region Built-Ins
        public static readonly TIMECoordinate Zero = new TIMECoordinate(0, 0, 0, 0, 0, 0);
        public static readonly TIMECoordinate UnixTime = new TIMECoordinate(1970, 1, 1, 0, 0, 0);
        public static readonly TIMECoordinate GPSTime = new TIMECoordinate(1980, 1, 6, 0, 0, 0);
        public static readonly TIMECoordinate ChronometricGeologicTime = new TIMECoordinate(1950, 1, 1, 0, 0, 0);
        #endregion

        #region Properties
        public double? Year { get; internal set; }
        public double? Month { get; internal set; }
        public double? Day { get; internal set; }
        public double? Hour { get; internal set; }
        public double? Minute { get; internal set; }
        public double? Second { get; internal set; }
        public TIMECoordinateMetadata Metadata { get; internal set; }
        #endregion

        #region Ctors
        internal TIMECoordinate() { }

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
                TIMEHelper.GetMonthOfYear(utcDateTime.Month),
                TIMEHelper.GetDayOfWeek(utcDateTime.DayOfWeek),
                Convert.ToUInt32(utcDateTime.DayOfYear));
        }
        #endregion

        #region Interfaces
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

        public bool Equals(TIMECoordinate other)
            => CompareTo(other) == 0;
        #endregion

        #region Methods
        public override string ToString()
            => $"{Year ?? 0}_{Month ?? 0}_{Day ?? 0}_{Hour ?? 0}_{Minute ?? 0}_{Second ?? 0}";
        #endregion
    }

    public class TIMECoordinateMetadata
    {
        #region Properties
        public RDFResource TRS { get; internal set; }

        public RDFResource UnitType { get; internal set; }

        public RDFResource MonthOfYear { get; internal set; }

        public RDFResource DayOfWeek { get; internal set; }

        public uint? DayOfYear { get; internal set; }
        #endregion

        #region Ctors
        internal TIMECoordinateMetadata() { }

        public TIMECoordinateMetadata(RDFResource trsUri, RDFResource unitTypeUri, 
            RDFResource monthOfYear=null, RDFResource dayOfWeek=null, uint? dayOfYear=null)
        {
            TRS = trsUri ?? throw new OWLException("Cannot create coordinate metadata because given \"trsUri\" parameter is null");
            UnitType = unitTypeUri ?? throw new OWLException("Cannot create coordinate metadata because given \"unitTypeUri\" parameter is null");
            MonthOfYear = monthOfYear;
            DayOfWeek = dayOfWeek;
            DayOfYear = dayOfYear;
        }
        #endregion
    }
}