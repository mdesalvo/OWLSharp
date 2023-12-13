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
        /// Adds the given SWRL rule to the reasoner
        /// </summary>
        public static OWLReasoner AddSWRLRule(this OWLReasoner reasoner, SWRLRule swrlRule)
        {
            if (reasoner != null && swrlRule != null)
            {
                //Activate SWRL extension on the reasoner
                if (!reasoner.Extensions.ContainsKey("SWRL"))
                    reasoner.Extensions.Add("SWRL", ApplyToOntology);

                //Add SWRL rule to the reasoner
                if (!reasoner.Rules.ContainsKey("SWRL"))
                    reasoner.Rules.Add("SWRL", new List<SWRLRule>());
                ((List<SWRLRule>)reasoner.Rules["SWRL"]).Add(swrlRule);
            }
            return reasoner;
        }

        /// <summary>
        /// Applies the SWRL reasoner on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLReasoner reasoner, OWLOntology ontology, Dictionary<string, OWLReasonerReport> inferenceRegistry)
        {
            //Initialize inference registry
            foreach (SWRLRule swrlRule in (List<SWRLRule>)reasoner.Rules["SWRL"])
                inferenceRegistry.Add(swrlRule.RuleName, null);

            //Execute rules
            Parallel.ForEach((List<SWRLRule>)reasoner.Rules["SWRL"], 
                swrlRule =>
                {
                    OWLEvents.RaiseInfo($"Launching SWRL reasoner rule '{swrlRule.RuleName}'");

                    inferenceRegistry[swrlRule.RuleName] = swrlRule.ApplyToOntology(ontology);

                    OWLEvents.RaiseInfo($"Completed SWRL reasoner rule '{swrlRule.RuleName}': found {inferenceRegistry[swrlRule.RuleName].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}