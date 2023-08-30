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
    /// OWL-DL reasoner rule targeting class model knowledge (T-BOX) to reason over rdfs:subClassOf hierarchy
    /// </summary>
    internal static class OWLSubClassTransitivityRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferSuperClasses(RDFResource currentClass, OWLReasonerReport report)
            {
                List<RDFResource> superClasses = ontology.Model.ClassModel.GetSuperClassesOf(currentClass);
                foreach (RDFResource superClass in superClasses)
                {
                    //Create the inference
                    OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                        nameof(OWLSubClassTransitivityRule), new RDFTriple(currentClass, RDFVocabulary.RDFS.SUB_CLASS_OF, superClass).SetInference());

                    //Add the inference to the report
                    if (!ontology.Model.ClassModel.TBoxGraph.ContainsTriple(evidence.EvidenceContent))
                        report.AddEvidence(evidence);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:Class
            IEnumerator<RDFResource> classesEnumerator = ontology.Model.ClassModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
                InferSuperClasses(classesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}