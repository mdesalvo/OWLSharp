/*
   Copyright 2012-2024 Marco De Salvo
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
    /// Represents a temporal position described using a scaled numeric numericValue in a temporal coordinate system (e.g: UnixTime 1679477734)<br/>
    /// or a nominal numericValue in a temporal hierarchical-ordinal reference system (e.g: GeologicTime ArcheanEra)
    /// </summary>
    public class TIMEInstantPosition : RDFResource
    {
        #region Properties
        /// <summary>
        /// Indicates the temporal reference system in which the position of the time instant is encoded
        /// </summary>
        public RDFResource TRS { get; set; }

        /// <summary>
        /// Indicates the numeric numericValue of the position in the given TRS
        /// </summary>
        public double NumericValue { get; set; }

        /// <summary>
        /// Indicates the nominal numericValue of the position in the given TRS
        /// </summary>
        public RDFResource NominalValue { get; set; }

        /// <summary>
        /// Indicates a temporal interval of uncertainty about the position of the time instant
        /// </summary>
        public TIMEIntervalDuration PositionalUncertainty { get; set; }

        /// <summary>
        /// Checks if this instant position is expressed though a nominal value in the given TRS
        /// </summary>
        internal bool IsNominal => NominalValue != null;
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a time instant position with given Uri, whose temporal extent is not expressed
        /// </summary>
        internal TIMEInstantPosition(RDFResource timeInstantPositionUri)
            : base(timeInstantPositionUri?.ToString()) { }

        /// <summary>
        /// Builds a time instant position with given Uri, whose temporal extent is a numeric value encoded in the given TRS
        /// </summary>
        public TIMEInstantPosition(RDFResource timeInstantPositionUri, RDFResource trsUri, double numericValue)
            : this(timeInstantPositionUri)
        {
            #region Guards
            if (trsUri == null)
                throw new OWLException("Cannot create position of time instant because given \"trsUri\" parameter is null");
            #endregion

            TRS = trsUri;
            NumericValue = numericValue;
        }

        /// <summary>
        /// Builds a time instant position with given Uri, whose temporal extent is a nominal value described in the given TRS
        /// </summary>
        public TIMEInstantPosition(RDFResource timeInstantPositionUri, RDFResource trsUri, RDFResource nominalValue)
            : this(timeInstantPositionUri)
        {
            #region Guards
            if (trsUri == null)
                throw new OWLException("Cannot create position of time instant because given \"trsUri\" parameter is null");
            if (nominalValue == null)
                throw new OWLException("Cannot create position of time instant because given \"nominalValue\" parameter is null");
            #endregion

            TRS = trsUri;
            NominalValue = nominalValue;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the given interval duration as positional uncertainty of this time instant
        /// </summary>
        public TIMEInstantPosition SetPositionalUncertainty(TIMEIntervalDuration positionalUncertainty)
        {
            PositionalUncertainty = positionalUncertainty;
            return this;
        }
        #endregion
    }
}