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
    /// Represents the duration of a temporal extent expressed as a decimal number scaled by a temporal unit (e.g: 8.25 years, 54 Mars Sols)
    /// </summary>
    public class TIMEIntervalDuration : RDFResource
    {
        #region Properties
        /// <summary>
        /// Indicates the temporal unit which provides the precision of a date-time value or scale of a temporal extent (e.g: Second)
        /// </summary>
        public RDFResource UnitType { get; set; }

        /// <summary>
        /// Indicates the value of a temporal extent expressed as a number scaled by a temporal unit (e.g: 25)
        /// </summary>
        public double Value { get; set; }        
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a time interval duration with the given Uri, whose temporal extent is not expressed
        /// </summary>
        internal TIMEIntervalDuration(RDFResource timeIntervalDurationUri)
            : base(timeIntervalDurationUri?.ToString()) { }

        /// <summary>
        /// Builds a time interval duration with the given Uri, whose temporal extent is a numeric value encoded in the given unit type
        /// </summary>
        public TIMEIntervalDuration(RDFResource timeIntervalDurationUri, RDFResource unitTypeURI, double value)
            : this(timeIntervalDurationUri)
        {
            #region Guards
            if (unitTypeURI == null)
                throw new OWLException("Cannot create duration of time interval because given \"unitTypeURI\" parameter is null");
            #endregion

            UnitType = unitTypeURI;
            Value = value;
        }
        #endregion
    }
}