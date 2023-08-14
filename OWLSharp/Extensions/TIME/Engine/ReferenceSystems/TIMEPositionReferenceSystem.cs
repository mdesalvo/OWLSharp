/*
   Copyright 2012-2023 Marco De Salvo

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
    /// Represents the definition of a reference system for expressing position-based temporal extents (e.g: UnixTime)
    /// </summary>
    public class TIMEPositionReferenceSystem : TIMEReferenceSystem
    {
        #region Built-Ins
        /// <summary>
        /// TRS for expressing temporal extents encoded in Unix Time (seconds since midnight of 1st January 1970)
        /// </summary>
        public static readonly TIMEPositionReferenceSystem UnixTRS = new TIMEPositionReferenceSystem(
            RDFVocabulary.TIME.TRS_UNIX, TIMECoordinate.UnixTime, TIMEUnit.Second, false);

        /// <summary>
        /// TRS for expressing temporal extents encoded in Geologic Time (million years backward from midnight of 1st January 1950)
        /// </summary>
        public static readonly TIMEPositionReferenceSystem GeologicTRS = new TIMEPositionReferenceSystem(
            RDFVocabulary.TIME.TRS_GEOLOGIC, TIMECoordinate.GeologicTime, TIMEUnit.MYA, true);

        /// <summary>
        /// TRS for expressing temporal extents encoded in GPS Time (seconds since midnight of 6st January 1980)
        /// </summary>
        public static readonly TIMEPositionReferenceSystem GlobalPositioningSystemTRS = new TIMEPositionReferenceSystem(
            RDFVocabulary.TIME.TRS_GLOBAL_POSITIONING_SYSTEM, TIMECoordinate.GPSTime, TIMEUnit.Second, false);
        #endregion

        #region Properties
        /// <summary>
        /// Origin from which this positional TRS measures time
        /// </summary>
        public TIMECoordinate Origin { get; internal set; }

        /// <summary>
        /// Unit in which this positional TRS measures time
        /// </summary>
        public TIMEUnit Unit { get; internal set; }

        /// <summary>
        /// Indicates that this positional TRS works on large scale temporal extents (so that only the Year component is valuable)
        /// </summary>
        public bool IsLargeScaleTRS { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a positional TRS measuring offset from the given origin in the given temporal unit
        /// </summary>
        public TIMEPositionReferenceSystem(RDFResource trsUri, TIMECoordinate trsOrigin, TIMEUnit trsUnit, bool isLargeScaleTRS)
            : base(trsUri)
        {
            #region Guards
            if (trsOrigin == null)
                throw new OWLException("Cannot create TimeReferenceSystem because given \"trsOrigin\" parameter is null");
            if (trsUnit == null)
                throw new OWLException("Cannot create TimeReferenceSystem because given \"trsUnit\" parameter is null");
            #endregion

            Origin = trsOrigin;
            Unit = trsUnit;
            IsLargeScaleTRS = isLargeScaleTRS;
        }
        #endregion
    }
}