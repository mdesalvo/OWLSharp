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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerReport represents a detailed report of an ontology reasoner analysis
    /// </summary>
    public class OWLReasonerReport : IEnumerable<OWLReasonerEvidence>
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
        public IEnumerator<OWLReasonerEvidence> EvidencesEnumerator 
            => Evidences.Values.GetEnumerator();

        /// <summary>
        /// Dictionary of evidences
        /// </summary>
        internal Dictionary<long, OWLReasonerEvidence> Evidences { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty reasoner report
        /// </summary>
        internal OWLReasonerReport()
            => Evidences = new Dictionary<long, OWLReasonerEvidence>();
        #endregion

        #region Interfaces
        /// <summary>
        /// Exposes a typed enumerator on the reasoner report's evidences
        /// </summary>
        IEnumerator<OWLReasonerEvidence> IEnumerable<OWLReasonerEvidence>.GetEnumerator()
            => EvidencesEnumerator;

        /// <summary>
        /// Exposes an untyped enumerator on the reasoner report's evidences
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => EvidencesEnumerator;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given evidence to the reasoner report
        /// </summary>
        public void AddEvidence(OWLReasonerEvidence evidence)
        {
            if (evidence != null && !Evidences.ContainsKey(evidence.EvidenceContent.TripleID))
                Evidences.Add(evidence.EvidenceContent.TripleID, evidence);
        }

        /// <summary>
        /// Merges the evidences of the given report into the reasoner report
        /// </summary>
        internal void MergeEvidences(OWLReasonerReport report)
        {
            foreach (OWLReasonerEvidence evidence in report)
                AddEvidence(evidence);
        }

        /// <summary>
        /// Gets a graph representation of the reasoner report
        /// </summary>
        public RDFGraph ToRDFGraph()
        {
            RDFGraph evidenceGraph = new RDFGraph();
            foreach (OWLReasonerEvidence evidence in this)
                evidenceGraph.AddTriple(evidence.EvidenceContent);
            return evidenceGraph;
        }

        /// <summary>
        /// Asynchronously gets a graph representation of the reasoner report
        /// </summary>
        public Task<RDFGraph> ToRDFGraphAsync()
            => Task.Run(() => ToRDFGraph());

        /// <summary>
        /// Joins the reasoner evidences of this report into the given ontology
        /// </summary>
        public void JoinEvidences(OWLOntology ontology)
        {
            foreach (OWLReasonerEvidence evidence in this)
                switch (evidence.EvidenceCategory)
                {
                    case OWLEnums.OWLReasonerEvidenceCategory.ClassModel:
                        ontology.Model.ClassModel.TBoxGraph.AddTriple(evidence.EvidenceContent);
                        break;
                    case OWLEnums.OWLReasonerEvidenceCategory.PropertyModel:
                        ontology.Model.PropertyModel.TBoxGraph.AddTriple(evidence.EvidenceContent);
                        break;
                    case OWLEnums.OWLReasonerEvidenceCategory.Data:
                        ontology.Data.ABoxGraph.AddTriple(evidence.EvidenceContent);
                        break;
                }
        }

        /// <summary>
        /// Asynchronously joins the reasoner evidences of this report into the given ontology
        /// </summary>
        public Task JoinEvidencesAsync(OWLOntology ontology)
            => Task.Run(() => JoinEvidences(ontology));
        #endregion
    }
}