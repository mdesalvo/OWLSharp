/*
   Copyright 2014-2025 Marco De Salvo

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
    public sealed class TIMEExtent : IComparable<TIMEExtent>, IEquatable<TIMEExtent>
    {
        #region Built-Ins
        public static readonly TIMEExtent Zero = new TIMEExtent(0, 0, 0, 0, 0, 0, 0);
        #endregion

        #region Properties
        public double? Years { get; internal set; }
        public double? Months { get; internal set; }
        public double? Weeks { get; internal set; }
        public double? Days { get; internal set; }
        public double? Hours { get; internal set; }
        public double? Minutes { get; internal set; }
        public double? Seconds { get; internal set; }
        public TIMEExtentMetadata Metadata { get; internal set; }
        #endregion

        #region Ctors
        internal TIMEExtent() { }

        public TIMEExtent(double? years, double? months, double? weeks, double? days,
            double? hours, double? minutes, double? seconds, TIMEExtentMetadata metadata = null)
        {
            #region Guards
            if (years < 0)
                throw new OWLException("Cannot create temporal extent because given \"years\" parameter is negative");
            if (months < 0)
                throw new OWLException("Cannot create temporal extent because given \"months\" parameter is negative");
            if (weeks < 0)
                throw new OWLException("Cannot create temporal extent because given \"weeks\" parameter is negative");
            if (days < 0)
                throw new OWLException("Cannot create temporal extent because given \"days\" parameter is negative");
            if (hours < 0)
                throw new OWLException("Cannot create temporal extent because given \"hours\" parameter is negative");
            if (minutes < 0)
                throw new OWLException("Cannot create temporal extent because given \"minutes\" parameter is negative");
            if (seconds < 0)
                throw new OWLException("Cannot create temporal extent because given \"seconds\" parameter is negative");
            #endregion

            Years = years;
            Months = months;
            Weeks = weeks;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Metadata = metadata ?? new TIMEExtentMetadata();
        }

        public TIMEExtent(TimeSpan timeSpan)
        {
            Days = timeSpan.Days;
            Hours = timeSpan.Hours;
            Minutes = timeSpan.Minutes;
            Seconds = timeSpan.Seconds;
            Metadata = new TIMEExtentMetadata(TIMECalendarReferenceSystem.Gregorian);
        }
        #endregion

        #region Interfaces
        public int CompareTo(TIMEExtent other)
        {
            if (other == null)
                return 1;

            //Compare years
            double thisYears = Years ?? Zero.Years.Value;
            double otherYears = other.Years ?? Zero.Years.Value;
            if (thisYears != otherYears)
                return thisYears.CompareTo(otherYears);

            //Compare months
            double thisMonths = Months ?? Zero.Months.Value;
            double otherMonths = other.Months ?? Zero.Months.Value;
            if (thisMonths != otherMonths)
                return thisMonths.CompareTo(otherMonths);

            //Compare weeks
            double thisWeeks = Weeks ?? Zero.Weeks.Value;
            double otherWeeks = other.Weeks ?? Zero.Weeks.Value;
            if (thisWeeks != otherWeeks)
                return thisWeeks.CompareTo(otherWeeks);

            //Compare days
            double thisDays = Days ?? Zero.Days.Value;
            double otherDays = other.Days ?? Zero.Days.Value;
            if (thisDays != otherDays)
                return thisDays.CompareTo(otherDays);

            //Compare hours
            double thisHours = Hours ?? Zero.Hours.Value;
            double otherHours = other.Hours ?? Zero.Hours.Value;
            if (thisHours != otherHours)
                return thisHours.CompareTo(otherHours);

            //Compare minutes
            double thisMinutes = Minutes ?? Zero.Minutes.Value;
            double otherMinutes = other.Minutes ?? Zero.Minutes.Value;
            if (thisMinutes != otherMinutes)
                return thisMinutes.CompareTo(otherMinutes);

            //Compare seconds
            double thisSeconds = Seconds ?? Zero.Seconds.Value;
            double otherSeconds = other.Seconds ?? Zero.Seconds.Value;
            if (thisSeconds != otherSeconds)
                return thisSeconds.CompareTo(otherSeconds);

            return 0;
        }

        public bool Equals(TIMEExtent other)
            => CompareTo(other) == 0;
        #endregion

        #region Methods
        public override string ToString()
            => $"{Years ?? 0}_{Months ?? 0}_{Weeks ?? 0}_{Days ?? 0}_{Hours ?? 0}_{Minutes ?? 0}_{Seconds ?? 0}";
        #endregion
    }

    public sealed class TIMEExtentMetadata
    {
        #region Properties
        public RDFResource TRS { get; }
        #endregion

        #region Ctors
        internal TIMEExtentMetadata() { }

        public TIMEExtentMetadata(RDFResource trsUri)
            => TRS = trsUri ?? throw new OWLException("Cannot create extent metadata because given \"trsUri\" parameter is null");
        #endregion
    }
}