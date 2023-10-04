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
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL rule targeting data knowledge (A-BOX) to infer owl:sameAs relations from assertions having inverse functional properties
    /// </summary>
    internal static class OWLInverseFunctionalEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferSameAsRelations(RDFResource currentIFProperty, OWLReasonerReport report)
            {
                IEnumerable<IGrouping<RDFPatternMember,RDFTriple>> groupedMultiInverseFunctionalAssertions = ontology.Data.ABoxGraph[null, currentIFProperty, null, null]
                                                                                                                          .GroupBy(t => t.Object)
                                                                                                                          .Where(grp => grp.Count() > 1);
                foreach (IGrouping<RDFPatternMember,RDFTriple> groupedMultiInverseFunctionalAssertion in groupedMultiInverseFunctionalAssertions)
                {
                    List<RDFResource> sourceIndividuals = groupedMultiInverseFunctionalAssertion.Select(t => t.Subject)
                                                                                                .OfType<RDFResource>()
                                                                                                .ToList();
                    for (int i = 0; i < sourceIndividuals.Count; i++)
                        for (int j = i + 1; j < sourceIndividuals.Count; j++)
                            if (ontology.Data.CheckSameAsCompatibility(sourceIndividuals[i],sourceIndividuals[j]))
                            {
                                //Create the inferences
                                OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                                    nameof(OWLFunctionalEntailmentRule), new RDFTriple(sourceIndividuals[i], RDFVocabulary.OWL.SAME_AS, sourceIndividuals[j]).SetInference());
                                OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                                    nameof(OWLFunctionalEntailmentRule), new RDFTriple(sourceIndividuals[j], RDFVocabulary.OWL.SAME_AS, sourceIndividuals[i]).SetInference());

                                //Add the inferences to the report
                                if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceA.EvidenceContent))
                                    report.AddEvidence(evidenceA);
                                if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceB.EvidenceContent))
                                    report.AddEvidence(evidenceB);
                            }
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:InverseFunctionalProperty
            IEnumerator<RDFResource> inverseFunctionalPropertiesEnumerator = ontology.Model.PropertyModel.InverseFunctionalPropertiesEnumerator;
            while (inverseFunctionalPropertiesEnumerator.MoveNext())
                InferSameAsRelations(inverseFunctionalPropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}
