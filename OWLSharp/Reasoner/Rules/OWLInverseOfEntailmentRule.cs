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
    /// OWL-DL reasoner rule targeting data knowledge (A-BOX) to reason over inverse object assertions
    /// </summary>
    internal static class OWLInverseOfEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferInverseObjectAssertions(RDFResource currentProperty, OWLReasonerReport report)
            {
                List<RDFResource> inverseProperties = ontology.Model.PropertyModel.GetInversePropertiesOf(currentProperty);
                if (inverseProperties.Any())
                {
                    foreach (RDFTriple propertyObjectAssertion in ontology.Data.ABoxGraph[null, currentProperty, null, null]
                                                                           .Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
                    {
                        foreach (RDFResource inverseProperty in inverseProperties)
                        {
                            //Create the inferences
                            OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                                nameof(OWLInverseOfEntailmentRule), new RDFTriple((RDFResource)propertyObjectAssertion.Object, inverseProperty, (RDFResource)propertyObjectAssertion.Subject).SetInference());

                            //Add the inferences to the report
                            if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                                report.AddEvidence(evidence);
                        }
                    }
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
                InferInverseObjectAssertions(objectPropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}