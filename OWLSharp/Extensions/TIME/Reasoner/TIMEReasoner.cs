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
        /// Adds the given OWL-TIME extension rule to the reasoner
        /// </summary>
        public static OWLReasoner AddTIMEExtensionRule(this OWLReasoner reasoner, TIMEEnums.TIMEReasonerExtensionRules extensionRule)
        {
            if (reasoner != null && !reasoner.TIMEExtensionRules.Contains(extensionRule))
                reasoner.TIMEExtensionRules.Add(extensionRule);
            return reasoner;
        }

        /// <summary>
        /// Applies the OWL-TIME reasoner on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLReasoner reasoner, OWLOntology ontology, Dictionary<string, OWLReasonerReport> inferenceRegistry)
        {
            //Initialize inference registry
            foreach (TIMEEnums.TIMEReasonerExtensionRules extensionRule in reasoner.TIMEExtensionRules)
                inferenceRegistry.Add(extensionRule.ToString(), null);

            //Execute extension rules (OWL-TIME)
            Parallel.ForEach(reasoner.TIMEExtensionRules, 
                extensionRule =>
                {
                    OWLEvents.RaiseInfo($"Launching OWL-TIME extension reasoner rule '{extensionRule}'");

                    switch (extensionRule)
                    {
                        case TIMEEnums.TIMEReasonerExtensionRules.TIME_EqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerExtensionRules.TIME_EqualsEntailment.ToString()] = TIMEEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerExtensionRules.TIME_MeetsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerExtensionRules.TIME_MeetsEntailment.ToString()] = TIMEMeetsEntailmentRule.ExecuteRule(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL-TIME extension reasoner rule '{extensionRule}': found {inferenceRegistry[extensionRule.ToString()].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}