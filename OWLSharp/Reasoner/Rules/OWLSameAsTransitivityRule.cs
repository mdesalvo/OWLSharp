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
using System.Collections.Generic;

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL reasoner rule targeting data knowledge (A-BOX) to reason over owl:sameAs relations
    /// </summary>
    internal static class OWLSameAsTransitivityRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferSameIndividuals(RDFResource currentIndividual, OWLReasonerReport report)
            {
                List<RDFResource> sameIndividuals = ontology.Data.GetSameIndividuals(currentIndividual);
                foreach (RDFResource sameIndividual in sameIndividuals)
                {
                    //Create the inferences
                    OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                        nameof(OWLSameAsTransitivityRule), new RDFTriple(currentIndividual, RDFVocabulary.OWL.SAME_AS, sameIndividual).SetInference());
                    OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                        nameof(OWLSameAsTransitivityRule), new RDFTriple(sameIndividual, RDFVocabulary.OWL.SAME_AS, currentIndividual).SetInference());

                    //Add the inferences to the report
                    if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceA.EvidenceContent))
                        report.AddEvidence(evidenceA);
                    if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceB.EvidenceContent))
                        report.AddEvidence(evidenceB);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:NamedIndividual
            IEnumerator<RDFResource> individualsEnumerator = ontology.Data.IndividualsEnumerator;
            while (individualsEnumerator.MoveNext())
                InferSameIndividuals(individualsEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}