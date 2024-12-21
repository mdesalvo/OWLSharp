﻿/*
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
    public class TIMEPositionReferenceSystem : TIMEReferenceSystem
    {
        #region Built-Ins
        public static readonly TIMEPositionReferenceSystem UnixTRS = new TIMEPositionReferenceSystem(
            new RDFResource("https://en.wikipedia.org/wiki/Unix_time"), TIMECoordinate.UnixTime, TIMEUnit.Second, false);

        public static readonly TIMEPositionReferenceSystem GeologicTRS = new TIMEPositionReferenceSystem(
            new RDFResource("https://en.wikipedia.org/wiki/Geologic_time_scale"), TIMECoordinate.GeologicTime, TIMEUnit.MillionYearsAgo, true);

        public static readonly TIMEPositionReferenceSystem GlobalPositioningSystemTRS = new TIMEPositionReferenceSystem(
            new RDFResource("https://en.wikipedia.org/wiki/Global_Positioning_System"), TIMECoordinate.GPSTime, TIMEUnit.Second, false);
        #endregion

        #region Properties
        public TIMECoordinate Origin { get; internal set; }

        public TIMEUnit Unit { get; internal set; }

        public bool IsLargeScaleTRS { get; internal set; }
        #endregion

        #region Ctors
        public TIMEPositionReferenceSystem(RDFResource trsUri, TIMECoordinate trsOrigin, TIMEUnit trsUnit, bool isLargeScaleTRS)
            : base(trsUri)
        {
            Origin = trsOrigin ?? throw new OWLException("Cannot create TimeReferenceSystem because given \"trsOrigin\" parameter is null");
            Unit = trsUnit ?? throw new OWLException("Cannot create TimeReferenceSystem because given \"trsUnit\" parameter is null");
            IsLargeScaleTRS = isLargeScaleTRS;
        }
        #endregion
    }
}