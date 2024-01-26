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
    /// OWL-DL validator rule checking for consistency of rdfs:subPropertyOf relations (useful after import/merge scenarios)
    /// </summary>
    internal static class OWLSubPropertyConsistencyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //Iterate the properties of the model
            IEnumerator<RDFResource> propertiesEnumerator = ontology.Model.PropertyModel.PropertiesEnumerator;
            while (propertiesEnumerator.MoveNext())
            {
                //Get the subproperties of the current property
                foreach (RDFResource subProperty in ontology.Model.PropertyModel.GetSubPropertiesOf(propertiesEnumerator.Current))
                {
                    //Clash on rdfs:subPropertyOf
                    if (ontology.Model.PropertyModel.CheckIsSubPropertyOf(propertiesEnumerator.Current, subProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLSubPropertyConsistencyRule),
                            $"Violation of 'rdfs:subPropertyOf' hierarchy of property '{propertiesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{subProperty}' at the same time subProperty and superProperty of '{propertiesEnumerator.Current}'"));

                    //Clash on owl:equivalentProperties
                    if (ontology.Model.PropertyModel.CheckIsEquivalentPropertyOf(propertiesEnumerator.Current, subProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLSubPropertyConsistencyRule),
                            $"Violation of 'rdfs:subPropertyOf' hierarchy of property '{propertiesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{subProperty}' at the same time subProperty and equivalentProperty of '{propertiesEnumerator.Current}'"));
           
                    //Clash on owl:propertyDisjointWith
                    if (ontology.Model.PropertyModel.CheckIsDisjointPropertyWith(propertiesEnumerator.Current, subProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLSubPropertyConsistencyRule),
                            $"Violation of 'rdfs:subPropertyOf' hierarchy of property '{propertiesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{subProperty}' at the same time subProperty and propertyDisjointWith of '{propertiesEnumerator.Current}'"));

                    //Clash on owl:propertyChainAxiom [OWL2]
                    if (ontology.Model.PropertyModel.CheckHasPropertyChainAxiom(subProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLSubPropertyConsistencyRule),
                            $"Violation of 'rdfs:subPropertyOf' hierarchy of property '{propertiesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have propertyChainAxiom '{subProperty}' being subProperty of '{propertiesEnumerator.Current}'"));
                }
            }

            return validatorRuleReport;
        }
    }
}