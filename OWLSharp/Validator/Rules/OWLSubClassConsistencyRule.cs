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
    /// OWL-DL validator rule checking for consistency of rdfs:subClassOf relations (useful after import/merge scenarios)
    /// </summary>
    internal static class OWLSubClassConsistencyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //Iterate the classes of the model
            IEnumerator<RDFResource> classesEnumerator = ontology.Model.ClassModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
            {
                //Get the subclasses of the current class
                foreach (RDFResource subClass in ontology.Model.ClassModel.GetSubClassesOf(classesEnumerator.Current))
                {
                    //Clash on rdfs:subClassOf
                    if (ontology.Model.ClassModel.CheckIsSubClassOf(classesEnumerator.Current, subClass))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLSubClassConsistencyRule),
                            $"Violation of 'rdfs:subClassOf' hierarchy of class '{classesEnumerator.Current}'",
                            $"Revise you model: after post/merge actions you have '{subClass}' at the same time subClass and superClass of '{classesEnumerator.Current}'"));

                    //Clash on owl:equivalentClasses
                    if (ontology.Model.ClassModel.CheckIsEquivalentClassOf(classesEnumerator.Current, subClass))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLSubClassConsistencyRule),
                            $"Violation of 'rdfs:subClassOf' hierarchy of class '{classesEnumerator.Current}'",
                            $"Revise you model: after post/merge actions you have '{subClass}' at the same time subClass and equivalentClass of '{classesEnumerator.Current}'"));
           
                    //Clash on owl:disjointWith
                    if (ontology.Model.ClassModel.CheckIsDisjointClassWith(classesEnumerator.Current, subClass))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLSubClassConsistencyRule),
                            $"Violation of 'rdfs:subClassOf' hierarchy of class '{classesEnumerator.Current}'",
                            $"Revise you model: after post/merge actions you have '{subClass}' at the same time subClass and disjointWith of '{classesEnumerator.Current}'"));
                }
            }

            return validatorRuleReport;
        }
    }
}