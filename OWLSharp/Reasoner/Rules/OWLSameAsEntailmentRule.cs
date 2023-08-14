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
    /// OWL-DL reasoner rule targeting data knowledge (A-BOX) to infer assertions from owl:sameAs hierarchy
    /// </summary>
    internal static class OWLSameAsEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferSameIndividualsAssertions(RDFResource currentIndividual, OWLReasonerReport report)
            {
                List<RDFResource> sameIndividuals = ontology.Data.GetSameIndividuals(currentIndividual);
                if (sameIndividuals.Any())
                {
                    //Consider assertions having the current individual as SUBJECT
                    foreach (RDFTriple currentIndividualSubjectTriple in ontology.Data.ABoxGraph[currentIndividual, null, null, null]
                                                                                      .Where(t => ontology.Model.PropertyModel.CheckHasObjectProperty((RDFResource)t.Predicate)
                                                                                                   || ontology.Model.PropertyModel.CheckHasDatatypeProperty((RDFResource)t.Predicate)))
                    {
                        foreach (RDFResource sameIndividual in sameIndividuals)
                        {
                            //Create the inferences
                            OWLReasonerEvidence evidence = currentIndividualSubjectTriple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO
                                ? new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, nameof(OWLSameAsEntailmentRule), new RDFTriple(sameIndividual, (RDFResource)currentIndividualSubjectTriple.Predicate, (RDFResource)currentIndividualSubjectTriple.Object))
                                : new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, nameof(OWLSameAsEntailmentRule), new RDFTriple(sameIndividual, (RDFResource)currentIndividualSubjectTriple.Predicate, (RDFLiteral)currentIndividualSubjectTriple.Object));

                            //Add the inference to the report
                            if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                                report.AddEvidence(evidence);
                        }
                    }

                    //Consider assertions having the current individual as OBJECT
                    foreach (RDFTriple currentIndividualObjectTriple in ontology.Data.ABoxGraph[null, null, currentIndividual, null]
                                                                                     .Where(t => ontology.Model.PropertyModel.CheckHasObjectProperty((RDFResource)t.Predicate)))
                    {
                        foreach (RDFResource sameIndividual in sameIndividuals)
                        {
                            //Create the inferences
                            OWLReasonerEvidence evidence = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, 
                                nameof(OWLSameAsEntailmentRule), new RDFTriple((RDFResource)currentIndividualObjectTriple.Subject, (RDFResource)currentIndividualObjectTriple.Predicate, sameIndividual));

                            //Add the inference to the report
                            if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                                report.AddEvidence(evidence);
                        }
                    }
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:NamedIndividual
            IEnumerator<RDFResource> individualsEnumerator = ontology.Data.IndividualsEnumerator;
            while (individualsEnumerator.MoveNext())
                InferSameIndividualsAssertions(individualsEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}