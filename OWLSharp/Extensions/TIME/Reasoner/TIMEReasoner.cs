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

using OWLSharp.Extensions.SWRL;
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

            //Execute extension rules (OWL-TIME)
            Parallel.ForEach((List<TIMEEnums.TIMEReasonerRules>)reasoner.Rules["TIME"], 
                timeRule =>
                {
                    OWLEvents.RaiseInfo($"Launching OWL-TIME reasoner rule '{timeRule}'");

                    switch (timeRule)
                    {
                        case TIMEEnums.TIMEReasonerRules.TIME_EqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_EqualsEntailment.ToString()] = TIMEEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.TIME_MeetsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.TIME_MeetsEntailment.ToString()] = TIMEMeetsEntailmentRule.ExecuteRule(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL-TIME reasoner rule '{timeRule}': found {inferenceRegistry[timeRule.ToString()].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}