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
    /// OWL-TIME validator rule checking for consistency of time:after relations
    /// </summary>
    internal class TIMEInstantAfterRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology, List<RDFResource> timeInstants)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            foreach (RDFResource currentInstant in timeInstants)
                foreach (RDFResource precedingInstant in ontology.GetRelatedInstants(currentInstant, TIMEEnums.TIMEInstantRelation.After))
                {
                    //time:after
                    if (ontology.GetRelatedInstants(precedingInstant, TIMEEnums.TIMEInstantRelation.After).Any(intv => intv.Equals(currentInstant)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEInstantAfterRule),
                            $"Violation of 'time:after' relation for instant '{currentInstant}' against instant '{precedingInstant}'",
                            "These instants clash with temporal paradox 'AFTER(I1,I2) ^ AFTER(I2,I1)': this violates the integrity of 'time:after' relations."));

                    //time:before
                    if (ontology.GetRelatedInstants(currentInstant, TIMEEnums.TIMEInstantRelation.Before).Any(intv => intv.Equals(precedingInstant)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEInstantAfterRule),
                            $"Violation of 'time:after' relation for instant '{currentInstant}' against instant '{precedingInstant}'",
                            "These instants clash with temporal paradox 'AFTER(I1,I2) ^ BEFORE(I1,I2)': this violates the integrity of 'time:after' relations."));
                }

            return validatorRuleReport;
        }
    }
}