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

namespace OWLSharp
{
    /// <summary>
    /// OWL2 rule checking for consistency of owl:TopObjectProperty (as root) and owl:BottomObjectProperty (as bottom) 
    /// </summary>
    internal static class OWLTopBottomRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:TopObjectProperty
            foreach (RDFResource topObjPropSuperProperty in ontology.Model.PropertyModel.GetSuperPropertiesOf(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY))
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(OWLTopBottomRule),
                    $"Violation of OWL2 integrity caused by '{topObjPropSuperProperty}' being superProperty of 'owl:TopObjectProperty'",
                    "Revise your property model: it is not allowed the use of superProperties of reserved top object property 'owl:TopObjectProperty'"));
            //owl:TopDataProperty
            foreach (RDFResource topDtPropSuperProperty in ontology.Model.PropertyModel.GetSuperPropertiesOf(RDFVocabulary.OWL.TOP_DATA_PROPERTY))
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(OWLTopBottomRule),
                    $"Violation of OWL2 integrity caused by '{topDtPropSuperProperty}' being superProperty of 'owl:TopDataProperty'",
                    "Revise your property model: it is not allowed the use of superProperties of reserved top datatype property 'owl:TopDataProperty'"));

            //owl:BottomObjectProperty
            foreach (RDFResource bottomObjPropSubProperty in ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY))
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(OWLTopBottomRule),
                    $"Violation of OWL2 integrity caused by '{bottomObjPropSubProperty}' being subProperty of 'owl:BottomObjectProperty'",
                    "Revise your property model: it is not allowed the use of subProperties of reserved bottom object property 'owl:BottomObjectProperty'"));
            //owl:BottomDataProperty
            foreach (RDFResource bottomDtPropSubProperty in ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY))
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(OWLTopBottomRule),
                    $"Violation of OWL2 integrity caused by '{bottomDtPropSubProperty}' being subProperty of 'owl:BottomDataProperty'",
                    "Revise your property model: it is not allowed the use of subProperties of reserved bottom data property 'owl:BottomDataProperty'"));

            return validatorRuleReport;
        }
    }
}