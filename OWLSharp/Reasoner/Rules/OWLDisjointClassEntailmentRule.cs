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
    /// OWL-DL reasoner rule targeting class model knowledge (T-BOX) to reason over owl:disjointWith relations
    /// </summary>
    internal static class OWLDisjointClassEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferDisjointClasses(RDFResource currentClass, OWLReasonerReport report)
            {
                List<RDFResource> disjointClasses = ontology.Model.ClassModel.GetDisjointClassesWith(currentClass);
                foreach (RDFResource disjointClass in disjointClasses)
                {
                    //Create the inferences
                    OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                        nameof(OWLDisjointClassEntailmentRule), new RDFTriple(currentClass, RDFVocabulary.OWL.DISJOINT_WITH, disjointClass).SetInference());
                    OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.ClassModel,
                        nameof(OWLDisjointClassEntailmentRule), new RDFTriple(disjointClass, RDFVocabulary.OWL.DISJOINT_WITH, currentClass).SetInference());

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
                InferDisjointClasses(classesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}