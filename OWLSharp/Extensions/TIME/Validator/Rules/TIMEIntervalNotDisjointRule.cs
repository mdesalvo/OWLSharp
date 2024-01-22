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
    /// OWL-TIME validator rule checking for consistency of time:intervalNotDisjoint relations
    /// </summary>
    internal class TIMEIntervalNotDisjointRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology, List<RDFResource> timeIntervals)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            foreach (RDFResource currentInterval in timeIntervals)
                foreach (RDFResource notDisjointInterval in ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.NotDisjoint))
                {
                    //time:intervalBefore
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Before).Any(intv => intv.Equals(notDisjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalNotDisjointRule),
                            $"Violation of 'time:intervalNotDisjoint' relation for interval '{currentInterval}' against interval '{notDisjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_NOT_DISJOINT(I1,I2) ^ INTERVAL_BEFORE(I1,I2)': this violates the integrity of 'time:intervalNotDisjoint' relations."));

                    //time:intervalAfter
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.After).Any(intv => intv.Equals(notDisjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalNotDisjointRule),
                            $"Violation of 'time:intervalNotDisjoint' relation for interval '{currentInterval}' against interval '{notDisjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_NOT_DISJOINT(I1,I2) ^ INTERVAL_AFTER(I1,I2)': this violates the integrity of 'time:intervalNotDisjoint' relations."));

                    //time:intervalDisjoint
                    if (ontology.GetRelatedIntervals(currentInterval, TIMEEnums.TIMEIntervalRelation.Disjoint).Any(intv => intv.Equals(notDisjointInterval)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEIntervalNotDisjointRule),
                            $"Violation of 'time:intervalNotDisjoint' relation for interval '{currentInterval}' against interval '{notDisjointInterval}'",
                            "These intervals clash with temporal paradox 'INTERVAL_NOT_DISJOINT(I1,I2) ^ INTERVAL_DISJOINT(I1,I2)': this violates the integrity of 'time:intervalNotDisjoint' relations."));
                }

            return validatorRuleReport;
        }
    }
}