/*
   Copyright 2012-2024 Marco De Salvo
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
    /// OWL-DL reasoner rule targeting class model knowledge (T-BOX) to reason over owl:equivalentClass relations
    /// </summary>
    internal static class OWLEquivalentClassTransitivityRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferEquivalentClasses(RDFResource currentClass, OWLReasonerReport report)
            {
                List<RDFResource> equivalentClasses = ontology.Model.ClassModel.GetEquivalentClassesOf(currentClass);
                foreach (RDFResource equivalentClass in equivalentClasses)
                {
                    //Create the inferences
                    OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                        nameof(OWLEquivalentClassTransitivityRule), new RDFTriple(currentClass, RDFVocabulary.OWL.EQUIVALENT_CLASS, equivalentClass).SetInference());
                    OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                        nameof(OWLEquivalentClassTransitivityRule), new RDFTriple(equivalentClass, RDFVocabulary.OWL.EQUIVALENT_CLASS, currentClass).SetInference());

                    //Add the inferences to the report
                    if (!ontology.Model.ClassModel.TBoxGraph.ContainsTriple(evidenceA.EvidenceContent))
                        report.AddEvidence(evidenceA);
                    if (!ontology.Model.ClassModel.TBoxGraph.ContainsTriple(evidenceB.EvidenceContent))
                        report.AddEvidence(evidenceB);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:Class
            IEnumerator<RDFResource> classesEnumerator = ontology.Model.ClassModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
                InferEquivalentClasses(classesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}