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
    /// OWL-DL validator rule checking for consistency of local cardinality constraints
    /// </summary>
    internal static class OWLLocalCardinalityRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:Restriction
            IEnumerator<RDFResource> restrictionsEnumerator = ontology.Model.ClassModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                //There should not be cardinality restrictions working on a transitive property, or any super properties or inverse properties being transitive
                if (ontology.Model.ClassModel.CheckHasCardinalityRestrictionClass(restrictionsEnumerator.Current)
                     || ontology.Model.ClassModel.CheckHasMinCardinalityRestrictionClass(restrictionsEnumerator.Current)
                      || ontology.Model.ClassModel.CheckHasMaxCardinalityRestrictionClass(restrictionsEnumerator.Current)
                       || ontology.Model.ClassModel.CheckHasMinMaxCardinalityRestrictionClass(restrictionsEnumerator.Current)
                        || ontology.Model.ClassModel.CheckHasQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current)
                         || ontology.Model.ClassModel.CheckHasMinQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current)
                          || ontology.Model.ClassModel.CheckHasMaxQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current)
                           || ontology.Model.ClassModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current))
                {
                    //Grab owl:onProperty value to check if it is a transitive property
                    RDFResource onProperty = ontology.Model.ClassModel.TBoxGraph[restrictionsEnumerator.Current, RDFVocabulary.OWL.ON_PROPERTY, null, null]
                                                .FirstOrDefault()?.Object as RDFResource;
                    if (ontology.Model.PropertyModel.CheckHasTransitiveProperty(onProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLLocalCardinalityRule),
                            $"Violation of OWL-DL integrity caused by 'owl:TransitiveProperty' used as 'owl:onProperty' of cardinality restriction '{restrictionsEnumerator.Current}'",
                            "Revise your class model: it is not allowed the direct use of transitive properties on cardinality restrictions"));
                    else
                    {
                        //Grab its super properties to check for presence of any transitive properties
                        List<RDFResource> superProperties = ontology.Model.PropertyModel.GetSuperPropertiesOf(onProperty);
                        if (superProperties.Any(sp => ontology.Model.PropertyModel.CheckHasTransitiveProperty(sp)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLLocalCardinalityRule),
                                $"Violation of OWL-DL integrity caused by 'owl:TransitiveProperty' being super property of 'owl:onProperty' of cardinality restriction '{restrictionsEnumerator.Current}'",
                                "Revise your class model: it is not allowed the indirect use of transitive properties on cardinality restrictions"));

                        //Grab its inverse properties to check for presence of any transitive properties
                        List<RDFResource> inverseProperties = ontology.Model.PropertyModel.GetInversePropertiesOf(onProperty);
                        if (inverseProperties.Any(ip => ontology.Model.PropertyModel.CheckHasTransitiveProperty(ip)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLLocalCardinalityRule),
                                $"Violation of OWL-DL integrity caused by 'owl:TransitiveProperty' being inverse property of 'owl:onProperty' of cardinality restriction '{restrictionsEnumerator.Current}'",
                                "Revise your class model: it is not allowed the indirect use of transitive properties on cardinality restrictions"));
                    }
                }   
            }

            return validatorRuleReport;
        }
    }
}