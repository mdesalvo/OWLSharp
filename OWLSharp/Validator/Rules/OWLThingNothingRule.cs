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
    /// OWL-DL rule checking for consistency of owl:Thing (as root) and owl:Nothing (as bottom) 
    /// </summary>
    internal static class OWLThingNothingRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:Thing
            foreach (RDFResource thingSuperClass in ontology.Model.ClassModel.GetSuperClassesOf(RDFVocabulary.OWL.THING))
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(OWLThingNothingRule),
                    $"Violation of OWL-DL integrity caused by '{thingSuperClass}' being superClass of 'owl:Thing'",
                    "Revise your class model: it is not allowed the use of superClasses of reserved top class 'owl:Thing'"));

            //owl:Nothing
            foreach (RDFResource nothingSubClass in ontology.Model.ClassModel.GetSubClassesOf(RDFVocabulary.OWL.NOTHING))
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(OWLThingNothingRule),
                    $"Violation of OWL-DL integrity caused by '{nothingSubClass}' being subClass of 'owl:Nothing'",
                    "Revise your class model: it is not allowed the use of subClasses of reserved bottom class 'owl:Nothing'"));
            foreach (RDFResource nothingIndividual in ontology.Data.GetIndividualsOf(ontology.Model, RDFVocabulary.OWL.NOTHING))
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(OWLThingNothingRule),
                    $"Violation of OWL-DL integrity caused by '{nothingIndividual}' being individual of 'owl:Nothing'",
                    "Revise your data: it is not allowed the use of individuals of reserved bottom class 'owl:Nothing'"));

            return validatorRuleReport;
        }
    }
}