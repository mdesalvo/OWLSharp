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
    /// OWL-DL reasoner rule targeting data knowledge (A-BOX) to reason over rdf:type relations
    /// </summary>
    internal static class OWLIndividualTypeEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferClassIndividuals(RDFResource currentClass, OWLReasonerReport report)
            {
                List<RDFResource> classIndividuals = ontology.Data.GetIndividualsOf(ontology.Model, currentClass);
                foreach (RDFResource classIndividual in classIndividuals)
                {
                    //Create the inferences
                    OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                        nameof(OWLIndividualTypeEntailmentRule), new RDFTriple(classIndividual, RDFVocabulary.RDF.TYPE, currentClass));

                    //Add the inferences to the report
                    if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                        report.AddEvidence(evidence);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:Class
            IEnumerator<RDFResource> classesEnumerator = ontology.Model.ClassModel.ClassesEnumerator;
            while (classesEnumerator.MoveNext())
                InferClassIndividuals(classesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}