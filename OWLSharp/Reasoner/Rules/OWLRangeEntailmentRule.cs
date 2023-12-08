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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// RDFS reasoner rule targeting data knowledge (A-BOX) to infer individual types from rdfs:range relations
    /// </summary>
    internal static class OWLRangeEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferIndividualTypesFromRangeClass(RDFResource currentProperty, OWLReasonerReport report)
            {
                List<RDFResource> propertyRanges = ontology.Model.PropertyModel.GetRangeOf(currentProperty);
                foreach (RDFResource propertyRange in propertyRanges)
                {
                    //Get assertions of the current property
                    RDFGraph propertyAssertions = ontology.Data.ABoxGraph[null, currentProperty, null, null];
                    foreach (RDFResource propertyAssertionIndividual in propertyAssertions.GroupBy(propAsn => propAsn.Object)
                                                                                          .Select(grpAsn => grpAsn.Key)
                                                                                          .OfType<RDFResource>())
                    {
                        //Create the inferences
                        OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                            nameof(OWLRangeEntailmentRule), new RDFTriple(propertyAssertionIndividual, RDFVocabulary.RDF.TYPE, propertyRange).SetInference());

                        //Add the inferences to the report
                        if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                            report.AddEvidence(evidence);
                    }
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
                InferIndividualTypesFromRangeClass(objectPropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}