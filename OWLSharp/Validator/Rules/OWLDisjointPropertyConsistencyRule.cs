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

namespace OWLSharp
{
    /// <summary>
    /// OWL2 validator rule checking for consistency of owl:propertyDisjointWith relations (useful after import/merge scenarios)
    /// </summary>
    internal static class OWLDisjointPropertyConsistencyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //Iterate the properties of the model
            IEnumerator<RDFResource> propertiesEnumerator = ontology.Model.PropertyModel.PropertiesEnumerator;
            while (propertiesEnumerator.MoveNext())
            {
                //Get the disjoint properties of the current property
                foreach (RDFResource disjointProperty in ontology.Model.PropertyModel.GetDisjointPropertiesWith(propertiesEnumerator.Current))
                {
                    //Clash on rdfs:subPropertyOf
                    if (ontology.Model.PropertyModel.CheckIsSubPropertyOf(propertiesEnumerator.Current, disjointProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDisjointPropertyConsistencyRule),
                            $"Violation of 'owl:propertyDisjointWith' hierarchy of property '{propertiesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{propertiesEnumerator.Current}' at the same time propertyDisjointWith and subProperty of '{disjointProperty}'"));
                    if (ontology.Model.PropertyModel.CheckIsSubPropertyOf(disjointProperty, propertiesEnumerator.Current))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDisjointPropertyConsistencyRule),
                            $"Violation of 'owl:propertyDisjointWith' hierarchy of property '{propertiesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{disjointProperty}' at the same time propertyDisjointWith and subProperty of '{propertiesEnumerator.Current}'"));

                    //Clash on owl:equivalentProperty
                    if (ontology.Model.PropertyModel.CheckIsEquivalentPropertyOf(propertiesEnumerator.Current, disjointProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDisjointPropertyConsistencyRule),
                            $"Violation of 'owl:propertyDisjointWith' hierarchy of property '{propertiesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{disjointProperty}' at the same time propertyDisjointWith and equivalentProperty of '{propertiesEnumerator.Current}'"));
                }
            }

            return validatorRuleReport;
        }
    }
}