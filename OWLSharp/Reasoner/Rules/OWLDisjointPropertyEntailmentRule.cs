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
    /// OWL2 reasoner rule targeting property model knowledge (T-BOX) to reason over owl:propertyDisjointWith relations
    /// </summary>
    internal static class OWLDisjointPropertyEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferDisjointProperties(RDFResource currentProperty, OWLReasonerReport report)
            {
                List<RDFResource> disjointProperties = ontology.Model.PropertyModel.GetDisjointPropertiesWith(currentProperty);
                foreach (RDFResource disjointProperty in disjointProperties)
                {
                    //Create the inferences
                    OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel,
                        nameof(OWLDisjointPropertyEntailmentRule), new RDFTriple(currentProperty, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, disjointProperty));
                    OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel,
                        nameof(OWLDisjointPropertyEntailmentRule), new RDFTriple(disjointProperty, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, currentProperty));

                    //Add the inferences to the report
                    if (!ontology.Model.PropertyModel.TBoxGraph.ContainsTriple(evidenceA.EvidenceContent))
                        report.AddEvidence(evidenceA);
                    if (!ontology.Model.PropertyModel.TBoxGraph.ContainsTriple(evidenceB.EvidenceContent))
                        report.AddEvidence(evidenceB);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
                InferDisjointProperties(objectPropertiesEnumerator.Current, reasonerRuleReport);

            //owl:DatatypeProperty
            IEnumerator<RDFResource> datatypePropertiesEnumerator = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypePropertiesEnumerator.MoveNext())
                InferDisjointProperties(datatypePropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}