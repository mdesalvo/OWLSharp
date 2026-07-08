/*
   Copyright 2014-2026 Marco De Salvo
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

using System.Collections.Generic;

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLProfileReport is the outcome of checking an ontology against a given OWL2 profile (EL, QL, RL),
    /// collecting the full list of syntactic violations detected against that profile's grammar.
    /// An ontology is compliant with the profile if, and only if, the report carries no violations.
    /// </summary>
    public sealed class OWLProfileReport
    {
        #region Properties
        /// <summary>
        /// The profile this report has been computed against
        /// </summary>
        public OWLEnums.OWLProfiles Profile { get; }

        /// <summary>
        /// The set of violations detected against the profile's grammar (empty if the ontology is compliant)
        /// </summary>
        public List<OWLProfileViolation> Violations { get; internal set; } = new List<OWLProfileViolation>();

        /// <summary>
        /// Indicates that the ontology has no violations against the profile's grammar
        /// </summary>
        public bool IsCompliant
            => Violations.Count == 0;
        #endregion

        #region Ctors
        internal OWLProfileReport(OWLEnums.OWLProfiles profile)
            => Profile = profile;
        #endregion
    }
}