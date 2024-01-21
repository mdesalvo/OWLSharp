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
    /// OWL-TIME validator rule checking for consistency of time:intervalDisjoint relations
    /// </summary>
    internal class TIMEIntervalDisjointRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology, List<RDFResource> timeIntervals)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            foreach (RDFResource currentInterval in timeIntervals)
                foreach (RDFResource disjointInterval in ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Disjoint))
                {
                    //time:intervalContains
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Contains).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_CONTAINS(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalDuring
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.During).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_DURING(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalEquals
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Equals).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_EQUALS(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));
                
                    //time:intervalFinishedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.FinishedBy).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_FINISHED_BY(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalFinishes
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Finishes).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_FINISHES(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));
                
                    //time:intervalIn
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.In).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_IN(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalMeets
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Meets).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_MEETS(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalMetBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.MetBy).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_MET_BY(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));
                
                    //time:intervalNotDisjoint
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.NotDisjoint).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_NOT_DISJOINT(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalOverlappedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.OverlappedBy).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_OVERLAPPED_BY(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalOverlaps
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Overlaps).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_OVERLAPS(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));
                
                    //time:intervalStartedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.StartedBy).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_STARTED_BY(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));

                    //time:intervalStarts
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Starts).Any(intv => intv.Equals(disjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalDisjointRule),
                            $"Violation of 'time:intervalDisjoint' relation for interval '{currentInterval}' against interval '{disjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_DISJOINT(I1,I2) ^ INTERVAL_STARTS(I1,I2)': this violates the integrity of 'time:intervalDisjoint' relations."));
                }

            return validatorRuleReport;
        }
    }
}