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

namespace OWLSharp.Extensions.TIME
{
    public class TIMEUnit : RDFResource
    {
        #region Built-ins
        public static readonly TIMEUnit Millennium = new TIMEUnit(RDFVocabulary.TIME.UNIT_MILLENIUM, TIMEEnums.TIMEUnitType.Year, 1000);
        public static readonly TIMEUnit Century = new TIMEUnit(RDFVocabulary.TIME.UNIT_CENTURY, TIMEEnums.TIMEUnitType.Year, 100);
        public static readonly TIMEUnit Decade = new TIMEUnit(RDFVocabulary.TIME.UNIT_DECADE, TIMEEnums.TIMEUnitType.Year, 10);
        public static readonly TIMEUnit Year = new TIMEUnit(RDFVocabulary.TIME.UNIT_YEAR, TIMEEnums.TIMEUnitType.Year, 1);
        public static readonly TIMEUnit Month = new TIMEUnit(RDFVocabulary.TIME.UNIT_MONTH, TIMEEnums.TIMEUnitType.Month, 1);
        public static readonly TIMEUnit Week = new TIMEUnit(RDFVocabulary.TIME.UNIT_WEEK, TIMEEnums.TIMEUnitType.Day, 7);
        public static readonly TIMEUnit Day = new TIMEUnit(RDFVocabulary.TIME.UNIT_DAY, TIMEEnums.TIMEUnitType.Day, 1);
        public static readonly TIMEUnit Hour = new TIMEUnit(RDFVocabulary.TIME.UNIT_HOUR, TIMEEnums.TIMEUnitType.Hour, 1);
        public static readonly TIMEUnit Minute = new TIMEUnit(RDFVocabulary.TIME.UNIT_MINUTE, TIMEEnums.TIMEUnitType.Minute, 1);
        public static readonly TIMEUnit Second = new TIMEUnit(RDFVocabulary.TIME.UNIT_SECOND, TIMEEnums.TIMEUnitType.Second, 1);

        //Derived

        public static readonly TIMEUnit BillionYearsAgo = new TIMEUnit(new RDFResource("https://en.wikipedia.org/wiki/Bya"), TIMEEnums.TIMEUnitType.Year, -1_000_000_000);
        public static readonly TIMEUnit MillionYearsAgo = new TIMEUnit(new RDFResource("https://en.wikipedia.org/wiki/Million_years_ago"), TIMEEnums.TIMEUnitType.Year, -1_000_000);
        public static readonly TIMEUnit MarsSol = new TIMEUnit(new RDFResource("https://en.wikipedia.org/wiki/Mars_sol"), TIMEEnums.TIMEUnitType.Day, 1.02749125);
        #endregion

        #region Properties
        public TIMEEnums.TIMEUnitType UnitType { get; internal set; }

        public double ScaleFactor { get; internal set; }
        #endregion

        #region Ctors
        public TIMEUnit(RDFResource timeUnitUri, TIMEEnums.TIMEUnitType unitType, double scaleFactor) : base(timeUnitUri?.ToString())
        {
            UnitType = unitType;
            ScaleFactor = scaleFactor;
        }
        #endregion
    }
}