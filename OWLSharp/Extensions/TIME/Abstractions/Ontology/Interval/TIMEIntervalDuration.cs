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
    public class TIMEIntervalDuration : RDFResource
    {
        #region Properties
        public RDFResource UnitType { get; set; }

        public double Value { get; set; }
        #endregion

        #region Ctors
        internal TIMEIntervalDuration(RDFResource timeIntervalDurationUri) : base(timeIntervalDurationUri?.ToString()) { }

        public TIMEIntervalDuration(RDFResource timeIntervalDurationUri, RDFResource unitTypeURI, double value) : this(timeIntervalDurationUri)
        {
            UnitType = unitTypeURI ?? throw new OWLException("Cannot create duration of time interval because given \"unitTypeURI\" parameter is null");
            Value = value;
        }
        #endregion
    }
}