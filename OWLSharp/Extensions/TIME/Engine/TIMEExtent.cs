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
    /// Represents the definition of a temporal extent encoded in standard 7-dimensions
    /// </summary>
    public class TIMEExtent : IComparable<TIMEExtent>, IEquatable<TIMEExtent>
    {
        #region Built-Ins
        /// <summary>
        /// Gets a temporal extent with all components set to zero
        /// </summary>
        public static readonly TIMEExtent Zero = new TIMEExtent(0, 0, 0, 0, 0, 0, 0);
        #endregion

        #region Properties
        /// <summary>
        /// Years component of the temporal extent (cannot be negative)
        /// </summary>
        public double? Years { get; internal set; }

        /// <summary>
        /// Months component of the temporal extent (cannot be negative)
        /// </summary>
        public double? Months { get; internal set; }

        /// <summary>
        /// Weeks component of the temporal extent (cannot be negative)
        /// </summary>
        public double? Weeks { get; internal set; }

        /// <summary>
        /// Days component of the temporal extent (cannot be negative)
        /// </summary>
        public double? Days { get; internal set; }

        /// <summary>
        /// Hours component of the temporal extent (cannot be negative)
        /// </summary>
        public double? Hours { get; internal set; }

        /// <summary>
        /// Minutes component of the temporal extent (cannot be negative)
        /// </summary>
        public double? Minutes { get; internal set; }

        /// <summary>
        /// Seconds component of the temporal extent (cannot be negative)
        /// </summary>
        public double? Seconds { get; internal set; }

        /// <summary>
        /// Metadata describing the metrics of the temporal extent
        /// </summary>
        public TIMEExtentMetadata Metadata { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Internal-ctor for initializing an empty temporal extent
        /// </summary>
        internal TIMEExtent() { }

        /// <summary>
        /// Builds a temporal extent with the given components and optional metadata
        /// </summary>
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

        /// <summary>
        /// Builds a temporal extent with components extracted from the given TimeSpan
        /// </summary>
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
        /// <summary>
        /// Compares this temporal extent with the given one
        /// </summary>
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

        /// <summary>
        /// Checks if this temporal extent is the same as given one
        /// </summary>
        public bool Equals(TIMEExtent other)
            => CompareTo(other) == 0;
        #endregion

        #region Methods
        /// <summary>
        /// Gets a human-readable representation of this temporal extent
        /// </summary>
        public override string ToString()
            => $"{Years ?? 0}_{Months ?? 0}_{Weeks ?? 0}_{Days ?? 0}_{Hours ?? 0}_{Minutes ?? 0}_{Seconds ?? 0}";
        #endregion
    }

    /// <summary>
    /// Extends the definition of a temporal extent with some metrics informations
    /// </summary>
    public class TIMEExtentMetadata
    {
        #region Properties
        /// <summary>
        /// Indicates the temporal reference system in which the temporal extent is encoded (e.g: Gregorian)
        /// </summary>
        public RDFResource TRS { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Internal-ctor for initializing an empty extent metadata
        /// </summary>
        internal TIMEExtentMetadata() { }

        /// <summary>
        /// Builds a extent metadata with the given TRS
        /// </summary>
        public TIMEExtentMetadata(RDFResource trsUri)
            => TRS = trsUri ?? throw new OWLException("Cannot create extent metadata because given \"trsUri\" parameter is null");
        #endregion
    }
}