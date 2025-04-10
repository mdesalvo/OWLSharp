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
using System.Linq;

namespace OWLSharp.Extensions.TIME
{
    public sealed class TIMECalendarReferenceSystem : TIMEReferenceSystem
    {
        #region Built-Ins
        public static readonly TIMECalendarReferenceSystem Gregorian = new TIMECalendarReferenceSystem(
            new RDFResource("https://en.wikipedia.org/wiki/Gregorian_calendar"),
            new TIMECalendarReferenceSystemMetrics(60, 60, 24, new uint[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 })
                .SetLeapYearRule(year => {
                    return year >= 1582 && ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0)
                        ? new uint[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }
                        : new uint[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                }));
        #endregion

        #region Properties
        public TIMECalendarReferenceSystemMetrics Metrics { get; }
        #endregion

        #region Ctors
        public TIMECalendarReferenceSystem(RDFResource trsUri, TIMECalendarReferenceSystemMetrics trsMetrics) : base(trsUri)
            => Metrics = trsMetrics ?? throw new OWLException("Cannot create calendar-based TRS because given \"trsMetrics\" parameter is null");
        #endregion
    }

    public sealed class TIMECalendarReferenceSystemMetrics
    {
        #region Properties
        public uint SecondsInMinute { get; }

        public uint MinutesInHour { get; }

        public uint HoursInDay { get; }

        public uint[] Months { get; }

        //Derived

        public uint DaysInYear { get; }

        public uint MonthsInYear { get; }

        public bool HasExactMetric { get; }

        public Func<double,uint[]> LeapYearRule { get; internal set; }
        #endregion

        #region Ctors
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
        public TIMECalendarReferenceSystemMetrics SetLeapYearRule(Func<double,uint[]> leapYearRule)
        {
            LeapYearRule = leapYearRule;
            return this;
        }
        #endregion
    }
}