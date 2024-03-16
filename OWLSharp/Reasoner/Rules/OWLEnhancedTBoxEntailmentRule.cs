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
    /// OWL-DL rule targets model knowledge (T-BOX) to infer relations from complex hierarchies between classes and between properties
    /// </summary>
    internal static class OWLEnhancedTBoxEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferRelationsFromObjectPropertyHierarchy(RDFResource currentProperty, OWLReasonerReport report)
            {
                //EquivalentProperty(?P1,?P2) ^ InverseOf(?P2,?P3) -> InverseOf(?P1,?P3) 
                foreach (RDFResource eqProperty in ontology.Model.PropertyModel.GetEquivalentPropertiesOf(currentProperty))
                    foreach (RDFResource invOfEqProperty in ontology.Model.PropertyModel.GetInversePropertiesOf(eqProperty))
                    {
                        //Create the inferences
                        OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel, 
                            nameof(OWLEnhancedTBoxEntailmentRule), new RDFTriple(currentProperty, RDFVocabulary.OWL.INVERSE_OF, invOfEqProperty));
                        OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.PropertyModel, 
                            nameof(OWLEnhancedTBoxEntailmentRule), new RDFTriple(invOfEqProperty, RDFVocabulary.OWL.INVERSE_OF, currentProperty));

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
                InferRelationsFromObjectPropertyHierarchy(objectPropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}