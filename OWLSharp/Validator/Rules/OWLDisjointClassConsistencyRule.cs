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
    /// OWL-DL validator rule checking for consistency of owl:disjointWith relations (useful after import/merge scenarios)
    /// </summary>
    internal static class OWLDisjointClassConsistencyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //Iterate the classes of the model
            IEnumerator<RDFResource> classesEnumerator = ontology.Model.ClassModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
            {
                //Get the disjoint classes of the current class
                foreach (RDFResource disjointClass in ontology.Model.ClassModel.GetDisjointClassesWith(classesEnumerator.Current))
                {
                    //Clash on rdfs:subClassOf
                    if (ontology.Model.ClassModel.CheckIsSubClassOf(classesEnumerator.Current, disjointClass))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDisjointClassConsistencyRule),
                            $"Violation of 'owl:disjointWith' hierarchy of class '{classesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{classesEnumerator.Current}' at the same time disjointWith and subClass of '{disjointClass}'"));
                    if (ontology.Model.ClassModel.CheckIsSubClassOf(disjointClass, classesEnumerator.Current))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDisjointClassConsistencyRule),
                            $"Violation of 'owl:disjointWith' hierarchy of class '{classesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{disjointClass}' at the same time disjointWith and subClass of '{classesEnumerator.Current}'"));

                    //Clash on owl:equivalentClasses
                    if (ontology.Model.ClassModel.CheckIsEquivalentClassOf(classesEnumerator.Current, disjointClass))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDisjointClassConsistencyRule),
                            $"Violation of 'owl:disjointWith' hierarchy of class '{classesEnumerator.Current}'",
                            $"Revise you model: after import/merge actions you have '{disjointClass}' at the same time disjointWith and equivalentClass of '{classesEnumerator.Current}'"));
                }
            }

            return validatorRuleReport;
        }
    }
}