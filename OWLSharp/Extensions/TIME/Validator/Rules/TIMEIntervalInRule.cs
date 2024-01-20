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

using System.Linq;
using RDFSharp.Model;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// OWL-TIME validator rule checking for consistency of time:intervalIn relations
    /// </summary>
    internal class TIMEIntervalInRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //Get the individuals of type 'time:Interval' declared in the ontology
            foreach (RDFResource currentInterval in ontology.Data.GetIndividualsOf(ontology.Model, RDFVocabulary.TIME.INTERVAL))
                //Get the list of intervals against which the currently analyzed interval is related by 'time:intervalIn'
                foreach (RDFResource outerInterval in ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.In))
                {
                    //time:intervalBefore
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Before).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_BEFORE(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));

                    //time:intervalAfter
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.After).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_AFTER(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));

                    //time:intervalDisjoint
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Disjoint).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_DISJOINT(I2,I1)': this violates the integrity of 'time:intervalIn' relations."));

                    //time:intervalEquals
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Equals).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_EQUALS(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));
                
                    //time:intervalFinishedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.FinishedBy).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_FINISHED_BY(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));

                    //time:hasInside
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.HasInside).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_HAS_INSIDE(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));

                    //time:intervalMeets
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Meets).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_MEETS(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));

                    //time:intervalMetBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.MetBy).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_MET_BY(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));
                
                    //time:intervalOverlappedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.OverlappedBy).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_OVERLAPPED_BY(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));

                    //time:intervalOverlaps
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Overlaps).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_OVERLAPS(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));
                
                    //time:intervalStartedBy
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.StartedBy).Any(intv => intv.Equals(outerInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalInRule),
                            $"Violation of 'time:intervalIn' relation for interval '{currentInterval}' against interval '{outerInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_IN(I1,I2) ^ INTERVAL_STARTED_BY(I1,I2)': this violates the integrity of 'time:intervalIn' relations."));
                }

            return validatorRuleReport;
        }
    }
}