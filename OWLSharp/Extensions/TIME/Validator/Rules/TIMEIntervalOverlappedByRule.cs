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

using System.Collections.Generic;
using System.Linq;
using RDFSharp.Model;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// OWL-TIME validator rule checking for consistency of time:intervalOverlappedBy relations
    /// </summary>
    internal class TIMEIntervalOverlappedByRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology, List<RDFResource> timeIntervals)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            foreach (RDFResource currentInterval in timeIntervals)
                foreach (RDFResource overlappingInterval in ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.OverlappedBy))
                {
                    //time:intervalBefore
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Before).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_BEFORE(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalAfter
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.After).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_AFTER(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalContains
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Contains).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_CONTAINS(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalDisjoint
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Disjoint).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_DISJOINT(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalDuring
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.During).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_DURING(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalFinishedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.FinishedBy).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_FINISHED_BY(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalFinishes
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Finishes).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_FINISHES(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));
                
                    //time:intervalHasInside
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.HasInside).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_HAS_INSIDE(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalIn
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.In).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_IN(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalMeets
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Meets).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_MEETS(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalMetBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.MetBy).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_MET_BY(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));
                
                    //time:intervalOverlaps
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Overlaps).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_OVERLAPS(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalStartedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.StartedBy).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_STARTED_BY(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));

                    //time:intervalStarts
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Starts).Any(intv => intv.Equals(overlappingInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalOverlappedByRule),
                            $"Violation of 'time:intervalOverlappedBy' relation for interval '{currentInterval}' against interval '{overlappingInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_OVERLAPPED_BY(I1,I2) ^ INTERVAL_STARTS(I1,I2)': this violates the integrity of 'time:intervalOverlappedBy' relations."));
                }

            return validatorRuleReport;
        }
    }
}