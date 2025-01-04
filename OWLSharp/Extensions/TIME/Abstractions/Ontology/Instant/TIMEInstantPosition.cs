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
    public class TIMEInstantPosition : RDFResource
    {
        #region Properties
        public RDFResource TRS { get; set; }

        public double NumericValue { get; set; }

        public RDFResource NominalValue { get; set; }

        public TIMEIntervalDuration PositionalUncertainty { get; set; }

        internal bool IsNominal => NominalValue != null;
        #endregion

        #region Ctors
        internal TIMEInstantPosition(RDFResource timeInstantPositionUri) : base(timeInstantPositionUri?.ToString()) { }

        public TIMEInstantPosition(RDFResource timeInstantPositionUri, RDFResource trsUri, double numericValue)
            : this(timeInstantPositionUri)
        {
            TRS = trsUri ?? throw new OWLException("Cannot create position of time instant because given \"trsUri\" parameter is null");
            NumericValue = numericValue;
        }

        public TIMEInstantPosition(RDFResource timeInstantPositionUri, RDFResource trsUri, RDFResource nominalValue)
            : this(timeInstantPositionUri)
        {
            TRS = trsUri ?? throw new OWLException("Cannot create position of time instant because given \"trsUri\" parameter is null");
            NominalValue = nominalValue ?? throw new OWLException("Cannot create position of time instant because given \"nominalValue\" parameter is null");
        }
        #endregion
    }
}