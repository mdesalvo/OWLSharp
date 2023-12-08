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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL reasoner rule targeting data knowledge (A-BOX) to reason over transitive object assertions
    /// </summary>
    internal static class OWLTransitivePropertyEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferTransitiveObjectAssertions(RDFResource currentProperty, OWLReasonerReport report)
            {
                foreach (IGrouping<RDFResource, RDFTriple> propertyObjectAssertionsBySubject in ontology.Data.ABoxGraph[null, currentProperty, null, null]
                                                                                                             .Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                                                                                                             .GroupBy(t => (RDFResource)t.Subject))
                {
                    //Get individuals transitively related with the current subject individual
                    List<RDFResource> transitiveRelatedIndividuals = ontology.Data.GetTransitiveRelatedIndividuals(propertyObjectAssertionsBySubject.Key, currentProperty);
                    foreach (RDFResource transitiveRelatedIndividual in transitiveRelatedIndividuals)
                    {
                        //Create the inferences
                        OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                            nameof(OWLTransitivePropertyEntailmentRule), new RDFTriple(propertyObjectAssertionsBySubject.Key, currentProperty, transitiveRelatedIndividual).SetInference());

                        //Add the inferences to the report
                        if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                            report.AddEvidence(evidence);
                    }
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:TransitiveProperty
            IEnumerator<RDFResource> transitivePropertiesEnumerator = ontology.Model.PropertyModel.TransitivePropertiesEnumerator;
            while (transitivePropertiesEnumerator.MoveNext())
                InferTransitiveObjectAssertions(transitivePropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}