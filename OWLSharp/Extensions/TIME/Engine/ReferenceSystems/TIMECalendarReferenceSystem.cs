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
using System.Linq;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// Represents the definition of a reference system for expressing calendar-based temporal extents (e.g: Gregorian)
    /// </summary>
    public class TIMECalendarReferenceSystem : TIMEReferenceSystem
    {
        #region Built-Ins
        /// <summary>
        /// TRS for expressing temporal extents encoded in Gregorian Calendar
        /// </summary>
        public static readonly TIMECalendarReferenceSystem Gregorian = new TIMECalendarReferenceSystem(
            RDFVocabulary.TIME.TRS_GREGORIAN,
            new TIMECalendarReferenceSystemMetrics(60, 60, 24, new uint[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 })
                .SetLeapYearRule(year => {
                    return (year >= 1582 && ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0)))
                        ? new uint[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }
                        : new uint[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                }));
        #endregion

        #region Properties
        /// <summary>
        /// Metrics of this calendar-based TRS
        /// </summary>
        public TIMECalendarReferenceSystemMetrics Metrics { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a calendar TRS measuring time with the given metrics
        /// </summary>
        public TIMECalendarReferenceSystem(RDFResource trsUri, TIMECalendarReferenceSystemMetrics trsMetrics) : base(trsUri)
            => Metrics = trsMetrics ?? throw new OWLException("Cannot create calendar-based TRS because given \"trsMetrics\" parameter is null");
        #endregion
    }

    /// <summary>
    /// Represents the measuring metrics of a calendar-based temporal reference system
    /// </summary>
    public class TIMECalendarReferenceSystemMetrics
    {
        #region Properties
        /// <summary>
        /// Represents the quantity of seconds found in a minute of this calendar TRS
        /// </summary>
        public uint SecondsInMinute { get; internal set; }

        /// <summary>
        /// Represents the quantity of minutes found in an hour of this calendar TRS
        /// </summary>
        public uint MinutesInHour { get; internal set; }

        /// <summary>
        /// Represents the quantity of hours found in a day of this calendar TRS
        /// </summary>
        public uint HoursInDay { get; internal set; }

        /// <summary>
        /// Represents the months found in this calendar TRS, each defined by its quantity of days
        /// </summary>
        public uint[] Months { get; internal set; }

        //Derived

        /// <summary>
        /// Represents the quantity of days found in an year of this calendar TRS
        /// </summary>
        public uint DaysInYear { get; internal set; }

        /// <summary>
        /// Represents the quantity of months found in an year of this calendar TRS
        /// </summary>
        public uint MonthsInYear { get; internal set; }

        /// <summary>
        /// Indicates that this calendar TRS has months of the same length, resulting in exact temporal coordinates
        /// </summary>
        public bool HasExactMetric { get; internal set; }

        /// <summary>
        /// Function altering the definition of the regular months of this calendar TRS in case the working year is to be considered leap
        /// </summary>
        public Func<double,uint[]> LeapYearRule { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds the given metrics for fine-tuning the behavior of a calendar-based TRS
        /// </summary>
        public TIMECalendarReferenceSystemMetrics(uint secondsInMinute, uint minutesInHour, uint hoursInDay, uint[] months)
        {
            #region Guards
            if (secondsInMinute == 0)
                throw new OWLException("Cannot build calendar metrics because given \"secondsInMinute\" parameter must be greater than zero");
            if (minutesInHour == 0)
                throw new OWLException("Cannot build calendar metrics because given \"minutesInHour\" parameter must be greater than zero");
            if (hoursInDay == 0)
                throw new OWLException("Cannot build calendar metrics because given \"hoursInDay\" parameter must be greater than zero");
            if (months == null)
                throw new OWLException("Cannot build calendar metrics because given \"months\" parameter is null");
            if (months.Length == 0)
                throw new OWLException("Cannot build calendar metrics because given \"months\" parameter must contain at least one element");
            if (months.Contains<uint>(0))
                throw new OWLException("Cannot build calendar metrics because given \"months\" parameter must contain all elements greater than zerp");
            #endregion

            SecondsInMinute = secondsInMinute;
            MinutesInHour = minutesInHour;
            HoursInDay = hoursInDay;
            Months = months;

            //Derived
            DaysInYear = Convert.ToUInt32(months.Sum(m => m));
            MonthsInYear = Convert.ToUInt32(months.Length);
            HasExactMetric = months.Distinct().Count() == 1;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the given function as the Leap Year detector for this calendar TRS.<br/><br/>
        /// This function should check if the working year is leap and, if so, return an altered months array for this calendar TRS.<br/>
        /// For example, Gregorian calendar TRS has been configured to return [31,29,31,30,31,30,31,31,30,31,30,31] in case of leap year.
        /// </summary>
        public TIMECalendarReferenceSystemMetrics SetLeapYearRule(Func<double,uint[]> leapYearRule)
        {
            LeapYearRule = leapYearRule;
            return this;
        }
        #endregion
    }
}