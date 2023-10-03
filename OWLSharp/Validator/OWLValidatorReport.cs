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

using System.Collections;
using System.Collections.Generic;

namespace OWLSharp
{
    /// <summary>
    /// OWLValidatorReport represents a detailed report of an ontology validator analysis
    /// </summary>
    public class OWLValidatorReport : IEnumerable<OWLValidatorEvidence>
    {
        #region Properties
        /// <summary>
        /// Counter of the evidences
        /// </summary>
        public int EvidencesCount 
            => Evidences.Count;

        /// <summary>
        /// Gets an enumerator on the evidences for iteration
        /// </summary>
        public IEnumerator<OWLValidatorEvidence> EvidencesEnumerator 
            => Evidences.GetEnumerator();

        /// <summary>
        /// List of evidences
        /// </summary>
        internal List<OWLValidatorEvidence> Evidences { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty validator report
        /// </summary>
        public OWLValidatorReport()
            => Evidences = new List<OWLValidatorEvidence>();
        #endregion

        #region Interfaces
        /// <summary>
        /// Exposes a typed enumerator on the validator report's evidences
        /// </summary>
        IEnumerator<OWLValidatorEvidence> IEnumerable<OWLValidatorEvidence>.GetEnumerator()
            => EvidencesEnumerator;

        /// <summary>
        /// Exposes an untyped enumerator on the validator report's evidences
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => EvidencesEnumerator;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given evidence to the validation report
        /// </summary>
        public OWLValidatorReport AddEvidence(OWLValidatorEvidence evidence)
        {
            if (evidence != null)
                Evidences.Add(evidence);
            return this;
        }

        /// <summary>
        /// Merges the evidences of the given report
        /// </summary>
        internal void MergeEvidences(OWLValidatorReport report)
            => Evidences.AddRange(report.Evidences);

        /// <summary>
        /// Gets the warning evidences from the validation report
        /// </summary>
        public List<OWLValidatorEvidence> SelectWarnings()
            => Evidences.FindAll(e => e.EvidenceCategory == OWLEnums.OWLValidatorEvidenceCategory.Warning);

        /// <summary>
        /// Gets the error evidences from the validation report
        /// </summary>
        public List<OWLValidatorEvidence> SelectErrors()
            => Evidences.FindAll(e => e.EvidenceCategory == OWLEnums.OWLValidatorEvidenceCategory.Error);
        #endregion
    }
}
