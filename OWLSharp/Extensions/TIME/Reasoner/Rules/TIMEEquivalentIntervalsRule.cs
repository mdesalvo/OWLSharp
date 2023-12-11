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

using OWLSharp.Extensions.TIME;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL-TIME rule inferring that, given temporal intervals I1 and I2: STARTS(I1,I2) AND FINISHES(I1,I2) => EQUALS(I1,I2)
    /// </summary>
    internal static class TIMEEquivalentIntervalsRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferEquivalentIntervals(RDFResource currentInterval, OWLReasonerReport report)
            {
                List<RDFResource> startedIntervals = TIMEOntologyHelper.GetRelatedIntervals(ontology, currentInterval, TIMEEnums.TIMEIntervalRelation.Starts);
                List<RDFResource> finishedIntervals = TIMEOntologyHelper.GetRelatedIntervals(ontology, currentInterval, TIMEEnums.TIMEIntervalRelation.Finishes);
                foreach (RDFResource equivalentInterval in startedIntervals.Intersect(finishedIntervals))
                {
                    //Create the inferences
                    OWLReasonerEvidence evidenceA = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                        nameof(OWLDifferentFromEntailmentRule), new RDFTriple(currentInterval, RDFVocabulary.TIME.EQUALS, equivalentInterval).SetInference());
                    OWLReasonerEvidence evidenceB = new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data,
                        nameof(OWLDifferentFromEntailmentRule), new RDFTriple(equivalentInterval, RDFVocabulary.TIME.EQUALS, currentInterval).SetInference());

                    //Add the inferences to the report
                    if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceA.EvidenceContent))
                        report.AddEvidence(evidenceA);
                    if (!ontology.Data.ABoxGraph.ContainsTriple(evidenceB.EvidenceContent))
                        report.AddEvidence(evidenceB);
                }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //time:Interval
            List<RDFResource> intervals = ontology.Data.GetIndividualsOf(ontology.Model, RDFVocabulary.TIME.INTERVAL);
            foreach (RDFResource interval in intervals)
                InferEquivalentIntervals(interval, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}