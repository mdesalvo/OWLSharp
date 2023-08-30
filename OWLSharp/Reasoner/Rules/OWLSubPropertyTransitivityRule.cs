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
    /// OWL-DL reasoner rule targeting property model knowledge (T-BOX) to reason over rdfs:subPropertyOf hierarchy
    /// </summary>
    internal static class OWLSubPropertyTransitivityRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferSuperProperties(RDFResource currentProperty, OWLReasonerReport report)
            {
                List<RDFResource> superProperties = ontology.Model.PropertyModel.GetSuperPropertiesOf(currentProperty);
                foreach (RDFResource superProperty in superProperties)
                {
                    //Create the inference
                    OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel,
                        nameof(OWLSubPropertyTransitivityRule), new RDFTriple(currentProperty, RDFVocabulary.RDFS.SUB_PROPERTY_OF, superProperty).SetInference());

                    //Add the inference to the report
                    if (!ontology.Model.PropertyModel.TBoxGraph.ContainsTriple(evidence.EvidenceContent))
                        report.AddEvidence(evidence);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
                InferSuperProperties(objectPropertiesEnumerator.Current, reasonerRuleReport);

            //owl:DatatypeProperty
            IEnumerator<RDFResource> datatypePropertiesEnumerator = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypePropertiesEnumerator.MoveNext())
                InferSuperProperties(datatypePropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}