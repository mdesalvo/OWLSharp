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

namespace OWLSharp.Extensions.SWRL
{
    /// <summary>
    /// SWRLReasoner gives SWRL modeling/reasoning capabilities to standard reasoners
    /// </summary>
    public static class SWRLReasoner
    {
        #region Methods
        /// <summary>
        /// Adds the given SWRL extension rule to the reasoner
        /// </summary>
        public static OWLReasoner AddSWRLExtensionRule(this OWLReasoner reasoner, SWRLRule swrlRule)
        {
            if (reasoner != null && swrlRule != null)
                reasoner.SWRLExtensionRules.Add(swrlRule);
            return reasoner;
        }

        /// <summary>
        /// Applies the SWRL reasoner on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLReasoner reasoner, OWLOntology ontology, Dictionary<string, OWLReasonerReport> inferenceRegistry)
        {
            //Initialize inference registry
            foreach (SWRLRule extensionRule in reasoner.SWRLExtensionRules)
                inferenceRegistry.Add(extensionRule.RuleName, null);

            //Execute custom rules (SWRL)
            Parallel.ForEach(reasoner.SWRLExtensionRules, 
                extensionRule =>
                {
                    OWLEvents.RaiseInfo($"Launching SWRL extension reasoner rule '{extensionRule.RuleName}'");

                    inferenceRegistry[extensionRule.RuleName] = extensionRule.ApplyToOntology(ontology);

                    OWLEvents.RaiseInfo($"Completed SWRL extension reasoner rule '{extensionRule.RuleName}': found {inferenceRegistry[extensionRule.RuleName].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}