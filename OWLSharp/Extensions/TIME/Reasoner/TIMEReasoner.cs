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
using System.Threading.Tasks;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// TIMEReasoner gives OWL-TIME reasoning capabilities to standard reasoners
    /// </summary>
    public static class TIMEReasoner
    {
        #region Methods
        /// <summary>
        /// Adds the given OWL-TIME rule to the reasoner
        /// </summary>
        public static OWLReasoner AddTIMERule(this OWLReasoner reasoner, TIMEEnums.TIMEReasonerRules timeRule)
        {
            if (reasoner != null)
            {
                //Activate OWL-TIME extension on the reasoner
                if (!reasoner.Extensions.ContainsKey("TIME"))
                    reasoner.Extensions.Add("TIME", ApplyToOntology);

                //Add OWL-TIME rule to the reasoner
                if (!reasoner.Rules.ContainsKey("TIME"))
                    reasoner.Rules.Add("TIME", new List<TIMEEnums.TIMEReasonerRules>());
                if (!((List<TIMEEnums.TIMEReasonerRules>)reasoner.Rules["TIME"]).Contains(timeRule))
                    ((List<TIMEEnums.TIMEReasonerRules>)reasoner.Rules["TIME"]).Add(timeRule);
            }
            return reasoner;
        }

        /// <summary>
        /// Applies the OWL-TIME reasoner on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLReasoner reasoner, OWLOntology ontology, Dictionary<string, OWLReasonerReport> inferenceRegistry)
        {
            //Initialize inference registry
            foreach (TIMEEnums.TIMEReasonerRules timeRule in (List<TIMEEnums.TIMEReasonerRules>)reasoner.Rules["TIME"])
                inferenceRegistry.Add(timeRule.ToString(), null);

            //Execute rules
            Parallel.ForEach((List<TIMEEnums.TIMEReasonerRules>)reasoner.Rules["TIME"], 
                timeRule =>
                {
                    OWLEvents.RaiseInfo($"Launching OWL-TIME reasoner rule '{timeRule}'");

                    switch (timeRule)
                    {
                        case TIMEEnums.TIMEReasonerRules.TIME_AfterEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_AfterEqualsEntailment.ToString()] = TIMEAfterEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_AfterFinishesEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_AfterFinishesEntailment.ToString()] = TIMEAfterFinishesEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_AfterMetByEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_AfterMetByEntailment.ToString()] = TIMEAfterMetByEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_AfterTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_AfterTransitiveEntailment.ToString()] = TIMEAfterTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_BeforeEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_BeforeEqualsEntailment.ToString()] = TIMEBeforeEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_BeforeMeetsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_BeforeMeetsEntailment.ToString()] = TIMEBeforeMeetsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_BeforeStartsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_BeforeStartsEntailment.ToString()] = TIMEBeforeStartsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_BeforeTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_BeforeTransitiveEntailment.ToString()] = TIMEBeforeTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_ContainsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_ContainsEqualsEntailment.ToString()] = TIMEContainsEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_ContainsTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_ContainsTransitiveEntailment.ToString()] = TIMEContainsTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_DuringEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_DuringEqualsEntailment.ToString()] = TIMEDuringEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_DuringTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_DuringTransitiveEntailment.ToString()] = TIMEDuringTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_EqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_EqualsEntailment.ToString()] = TIMEEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_EqualsInverseEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_EqualsInverseEntailment.ToString()] = TIMEEqualsInverseEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_EqualsTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_EqualsTransitiveEntailment.ToString()] = TIMEEqualsTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_FinishedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_FinishedByEqualsEntailment.ToString()] = TIMEFinishedByEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_FinishesEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_FinishesEqualsEntailment.ToString()] = TIMEFinishesEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_MeetsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_MeetsEqualsEntailment.ToString()] = TIMEMeetsEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_MeetsStartsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_MeetsStartsEntailment.ToString()] = TIMEMeetsStartsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_StartsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_StartsEqualsEntailment.ToString()] = TIMEStartsEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_StartedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_StartedByEqualsEntailment.ToString()] = TIMEStartedByEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL-TIME reasoner rule '{timeRule}': found {inferenceRegistry[timeRule.ToString()].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}