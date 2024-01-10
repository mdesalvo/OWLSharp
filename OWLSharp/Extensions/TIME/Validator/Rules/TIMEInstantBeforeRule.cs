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
    /// OWL-TIME validator rule checking for consistency of time:before relations
    /// </summary>
    internal class TIMEInstantBeforeRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //Get the individuals of type 'time:Instant' declared in the ontology
            foreach (RDFResource currentInstant in ontology.Data.GetIndividualsOf(ontology.Model, RDFVocabulary.TIME.INSTANT))
                //Get the list of instants against which the currently analyzed instant is related by 'time:before'
                foreach (RDFResource followingInstant in ontology.GetRelatedInstants(currentInstant, TIMEEnums.TIMEInstantRelation.Before))
                {
                    //time:after
                    if (ontology.GetRelatedInstants(followingInstant, TIMEEnums.TIMEInstantRelation.Before).Any(intv => intv.Equals(currentInstant)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEInstantBeforeRule),
                            $"Violation of 'time:before' relation for instant '{currentInstant}' against instant '{followingInstant}'",
                            "These instants clash with temporal paradox 'BEFORE(I1,I2) ^ BEFORE(I2,I1)': this violates the integrity of 'time:before' relations."));

                    //time:before
                    if (ontology.GetRelatedInstants(currentInstant, TIMEEnums.TIMEInstantRelation.After).Any(intv => intv.Equals(followingInstant)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(TIMEInstantBeforeRule),
                            $"Violation of 'time:before' relation for instant '{currentInstant}' against instant '{followingInstant}'",
                            "These instants clash with temporal paradox 'BEFORE(I1,I2) ^ AFTER(I1,I2)': this violates the integrity of 'time:before' relations."));
                }

            return validatorRuleReport;
        }
    }
}