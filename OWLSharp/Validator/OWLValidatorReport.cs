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
using System.Linq;

namespace OWLSharp.Validator
{
    /// <summary>
    /// OWLValidatorReport is the outcome of validating an ontology's T-BOX, A-BOX, and R-BOX against the available
    /// validator rules, collecting the full list of issues (modeling errors, structural inconsistencies, constraint
    /// violations) detected across the ontology.
    /// An ontology is considered consistent if, and only if, the report carries no issues of Error severity
    /// (Warning-level issues do not compromise consistency, but should still be addressed for ontology quality).
    /// </summary>
    public sealed class OWLValidatorReport
    {
        #region Properties
        /// <summary>
        /// The set of issues detected by the validator rules (empty if the ontology is fully compliant)
        /// </summary>
        public List<OWLIssue> Issues { get; internal set; } = new List<OWLIssue>();

        /// <summary>
        /// Indicates that the ontology has no issues of Error severity
        /// </summary>
        public bool IsConsistent
            => Issues.Count == 0 || Issues.All(issue => issue.Severity == OWLEnums.OWLIssueSeverity.Warning);
        #endregion

        #region Ctors
        internal OWLValidatorReport() { }
        #endregion
    }
}