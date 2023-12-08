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

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// Represents the definition of a temporal unit, which is modeled as a multiple or a fraction of a standard unit type (depending on the scale factor)
    /// </summary>
    public class TIMEUnit : RDFResource
    {
        #region Built-ins
        /// <summary>
        /// This unit expresses a time of 1000 years
        /// </summary>
        public static readonly TIMEUnit Millennium = new TIMEUnit(RDFVocabulary.TIME.UNIT_MILLENIUM, TIMEEnums.TIMEUnitType.Year, 1000);

        /// <summary>
        /// This unit expresses a time of 100 years
        /// </summary>
        public static readonly TIMEUnit Century = new TIMEUnit(RDFVocabulary.TIME.UNIT_CENTURY, TIMEEnums.TIMEUnitType.Year, 100);

        /// <summary>
        /// This unit expresses a time of 10 years
        /// </summary>
        public static readonly TIMEUnit Decade = new TIMEUnit(RDFVocabulary.TIME.UNIT_DECADE, TIMEEnums.TIMEUnitType.Year, 10);

        /// <summary>
        /// This unit expresses a time of 1 year
        /// </summary>
        public static readonly TIMEUnit Year = new TIMEUnit(RDFVocabulary.TIME.UNIT_YEAR, TIMEEnums.TIMEUnitType.Year, 1);

        /// <summary>
        /// This unit expresses a time of 1 month
        /// </summary>
        public static readonly TIMEUnit Month = new TIMEUnit(RDFVocabulary.TIME.UNIT_MONTH, TIMEEnums.TIMEUnitType.Month, 1);

        /// <summary>
        /// This unit expresses a time of 7 days
        /// </summary>
        public static readonly TIMEUnit Week = new TIMEUnit(RDFVocabulary.TIME.UNIT_WEEK, TIMEEnums.TIMEUnitType.Day, 7);

        /// <summary>
        /// This unit expresses a time of 1 day
        /// </summary>
        public static readonly TIMEUnit Day = new TIMEUnit(RDFVocabulary.TIME.DAY, TIMEEnums.TIMEUnitType.Day, 1);

        /// <summary>
        /// This unit expresses a time of 1 hour
        /// </summary>
        public static readonly TIMEUnit Hour = new TIMEUnit(RDFVocabulary.TIME.HOUR, TIMEEnums.TIMEUnitType.Hour, 1);

        /// <summary>
        /// This unit expresses a time of 1 minute
        /// </summary>
        public static readonly TIMEUnit Minute = new TIMEUnit(RDFVocabulary.TIME.MINUTE, TIMEEnums.TIMEUnitType.Minute, 1);

        /// <summary>
        /// This unit expresses a time of 1 second
        /// </summary>
        public static readonly TIMEUnit Second = new TIMEUnit(RDFVocabulary.TIME.SECOND, TIMEEnums.TIMEUnitType.Second, 1);

        //Derived

        /// <summary>
        /// This unit expresses a time of 1B years backward
        /// </summary>
        public static readonly TIMEUnit BYA = new TIMEUnit(RDFVocabulary.TIME.UNIT_BYA, TIMEEnums.TIMEUnitType.Year, -1_000_000_000);

        /// <summary>
        /// This unit expresses a time of 1M years backward
        /// </summary>
        public static readonly TIMEUnit MYA = new TIMEUnit(RDFVocabulary.TIME.UNIT_MYA, TIMEEnums.TIMEUnitType.Year, -1_000_000);

        /// <summary>
        /// This unit expresses a time of 1 Mars Sol (duration of a solar day on Mars)
        /// </summary>
        public static readonly TIMEUnit MarsSol = new TIMEUnit(RDFVocabulary.TIME.UNIT_MARS_SOL, TIMEEnums.TIMEUnitType.Day, 1.02749125);
        #endregion

        #region Properties
        /// <summary>
        /// Indicates the type of unit in which time is measured
        /// </summary>
        public TIMEEnums.TIMEUnitType UnitType { get; internal set; }

        /// <summary>
        /// Indicates the scale factor applied to the unit type (useful for expressing derived multiple or fraction unit types)
        /// </summary>
        public double ScaleFactor { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a temporal unit of the given type having the given scale factor applied to it 
        /// </summary>
        public TIMEUnit(RDFResource timeUnitUri, TIMEEnums.TIMEUnitType unitType, double scaleFactor)
            : base(timeUnitUri?.ToString())
        {
            UnitType = unitType;
            ScaleFactor = scaleFactor;
        }
        #endregion
    }
}