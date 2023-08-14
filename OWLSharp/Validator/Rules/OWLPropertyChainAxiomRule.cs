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
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL2 validator rule checking for consistency of owl:propertyChainAxiom usage [OWL2]
    /// </summary>
    internal static class OWLPropertyChainAxiomRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:Restriction
            IEnumerator<RDFResource> restrictionsEnumerator = ontology.Model.ClassModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                //There should not be cardinality or self restrictions working on a property chain axiom
                if (ontology.Model.ClassModel.CheckHasCardinalityRestrictionClass(restrictionsEnumerator.Current)
                     || ontology.Model.ClassModel.CheckHasMinCardinalityRestrictionClass(restrictionsEnumerator.Current)
                      || ontology.Model.ClassModel.CheckHasMaxCardinalityRestrictionClass(restrictionsEnumerator.Current)
                       || ontology.Model.ClassModel.CheckHasMinMaxCardinalityRestrictionClass(restrictionsEnumerator.Current)
                        || ontology.Model.ClassModel.CheckHasQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current)
                         || ontology.Model.ClassModel.CheckHasMinQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current)
                          || ontology.Model.ClassModel.CheckHasMaxQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current)
                           || ontology.Model.ClassModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current)
                            || ontology.Model.ClassModel.CheckHasSelfRestrictionClass(restrictionsEnumerator.Current))
                {
                    //Grab owl:onProperty value to check if it is a property chain axiom
                    RDFResource onProperty = ontology.Model.ClassModel.TBoxGraph[restrictionsEnumerator.Current, RDFVocabulary.OWL.ON_PROPERTY, null, null]
                                                .FirstOrDefault()?.Object as RDFResource;
                    if (ontology.Model.PropertyModel.CheckHasPropertyChainAxiom(onProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLPropertyChainAxiomRule),
                            $"Violation of OWL2-DL integrity caused by 'owl:propertyChainAxiom' used as 'owl:onProperty' of restriction '{restrictionsEnumerator.Current}'",
                            "Revise your class model: it is not allowed the use of property chain axioms on cardinality or self restrictions"));
                }
            }

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
            {
                //There should not be functional, inverse functional, irreflexive, asymmetric object properties acting as property chain axioms
                if (ontology.Model.PropertyModel.CheckHasPropertyChainAxiom(objectPropertiesEnumerator.Current) && 
                     (ontology.Model.PropertyModel.CheckHasFunctionalProperty(objectPropertiesEnumerator.Current)
                      || ontology.Model.PropertyModel.CheckHasInverseFunctionalProperty(objectPropertiesEnumerator.Current)
                       || ontology.Model.PropertyModel.CheckHasIrreflexiveProperty(objectPropertiesEnumerator.Current)
                        || ontology.Model.PropertyModel.CheckHasAsymmetricProperty(objectPropertiesEnumerator.Current)))
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(OWLPropertyChainAxiomRule),
                        $"Violation of OWL2-DL integrity caused by 'owl:propertyChainAxiom' used as object property '{objectPropertiesEnumerator.Current}'",
                        "Revise your property model: it is not allowed the use of property chain axioms on object properties being functional, or inverse functional, or irreflexive, or asymmetric"));
            }

            return validatorRuleReport;
        }
    }
}