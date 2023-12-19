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
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSReasoner gives SKOS reasoning capabilities to standard reasoners
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SKOSReasoner
    {
        #region Methods
        /// <summary>
        /// Adds the given SKOS rule to the reasoner
        /// </summary>
        public static OWLReasoner AddSKOSRule(this OWLReasoner reasoner, SKOSEnums.SKOSReasonerRules skosRule)
        {
            if (reasoner != null)
            {
                //Activate SKOS extension on the reasoner
                reasoner.ActivateExtension<SKOSEnums.SKOSReasonerRules>("SKOS", ApplyToOntology);

                //Add SKOS rule to the reasoner
                if (!((List<SKOSEnums.SKOSReasonerRules>)reasoner.Rules["SKOS"]).Contains(skosRule))
                    ((List<SKOSEnums.SKOSReasonerRules>)reasoner.Rules["SKOS"]).Add(skosRule);
            }
            return reasoner;
        }

        /// <summary>
        /// Applies the SKOS reasoner on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLReasoner reasoner, OWLOntology ontology, Dictionary<string, OWLReasonerReport> inferenceRegistry)
        {
            //Initialize inference registry
            foreach (SKOSEnums.SKOSReasonerRules skosRule in (List<SKOSEnums.SKOSReasonerRules>)reasoner.Rules["SKOS"])
                inferenceRegistry.Add(skosRule.ToString(), null);

            //Execute rules
            Parallel.ForEach((List<SKOSEnums.SKOSReasonerRules>)reasoner.Rules["SKOS"], 
                skosRule =>
                {
                    OWLEvents.RaiseInfo($"Launching SKOS reasoner rule '{skosRule}'");

                    switch (skosRule)
                    {
                        case SKOSEnums.SKOSReasonerRules.SKOS_BroaderTransitiveEntailment:
                            inferenceRegistry[SKOSEnums.SKOSReasonerRules.SKOS_BroaderTransitiveEntailment.ToString()] = SKOSBroaderTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case SKOSEnums.SKOSReasonerRules.SKOS_NarrowerTransitiveEntailment:
                            inferenceRegistry[SKOSEnums.SKOSReasonerRules.SKOS_NarrowerTransitiveEntailment.ToString()] = SKOSNarrowerTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed SKOS reasoner rule '{skosRule}': found {inferenceRegistry[skosRule.ToString()].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}