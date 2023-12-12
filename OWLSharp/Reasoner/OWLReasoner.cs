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
using OWLSharp.Extensions.TIME;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp
{
    /// <summary>
    /// OWLReasoner analyzes an ontology in order to infer knowledge from its model and data
    /// </summary>
    public class OWLReasoner
    {
        #region Properties
        /// <summary>
        /// List of standard rules applied by the reasoner (RDFS, OWL, OWL2)
        /// </summary>
        internal List<OWLEnums.OWLReasonerStandardRules> StandardRules { get; set; }

        /// <summary>
        /// List of OWL-TIME extension rules applied by the reasoner
        /// </summary>
        internal List<TIMEEnums.TIMEReasonerExtensionRules> TIMEExtensionRules { get; set; }

        /// <summary>
        /// List of custom rules applied by the reasoner (SWRL)
        /// </summary>
        internal List<OWLReasonerRule> CustomRules { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty reasoner
        /// </summary>
        public OWLReasoner()
        {
            StandardRules = new List<OWLEnums.OWLReasonerStandardRules>();
            CustomRules = new List<OWLReasonerRule>();
            //Extensions (OWL-TIME)
            TIMEExtensionRules = new List<TIMEEnums.TIMEReasonerExtensionRules>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given standard rule (RDFS, OWL, OWL2) to the reasoner
        /// </summary>
        public OWLReasoner AddStandardRule(OWLEnums.OWLReasonerStandardRules standardRule)
        {
            if (!StandardRules.Contains(standardRule))
                StandardRules.Add(standardRule);
            return this;
        }

        /// <summary>
        /// Adds the given custom rule (SWRL) to the reasoner
        /// </summary>
        public OWLReasoner AddCustomRule(OWLReasonerRule swrlRule)
        {
            #region Guards
            if (swrlRule == null)
                throw new OWLException("Cannot add SWRL rule to reasoner because given \"swrlRule\" parameter is null");
            #endregion

            CustomRules.Add(swrlRule);
            return this;
        }

        /// <summary>
        /// Applies the reasoner on the given ontology
        /// </summary>
        public OWLReasonerReport ApplyToOntology(OWLOntology ontology)
        {
            OWLReasonerReport reasonerReport = new OWLReasonerReport();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Reasoner is going to be applied on Ontology '{ontology.URI}': this may require intensive processing, depending on size and complexity of domain knowledge and rules");

                //Initialize inference registry
                Dictionary<string, OWLReasonerReport> inferenceRegistry = new Dictionary<string, OWLReasonerReport>();
                foreach (OWLEnums.OWLReasonerStandardRules standardRule in StandardRules)
                    inferenceRegistry.Add(standardRule.ToString(), null);
                foreach (OWLReasonerRule customRule in CustomRules)
                    inferenceRegistry.Add(customRule.RuleName, null);

                //Execute standard rules (RDFS, OWL, OWL2)
                Parallel.ForEach(StandardRules, 
                    standardRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching standard reasoner rule '{standardRule}'");

                        switch (standardRule)
                        {
                            case OWLEnums.OWLReasonerStandardRules.SubClassTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.SubClassTransitivity.ToString()] = OWLSubClassTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.SubPropertyTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.SubPropertyTransitivity.ToString()] = OWLSubPropertyTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.EquivalentClassTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.EquivalentClassTransitivity.ToString()] = OWLEquivalentClassTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.EquivalentPropertyTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.EquivalentPropertyTransitivity.ToString()] = OWLEquivalentPropertyTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.DisjointClassEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.DisjointClassEntailment.ToString()] = OWLDisjointClassEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.DisjointPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.DisjointPropertyEntailment.ToString()] = OWLDisjointPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.DomainEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.DomainEntailment.ToString()] = OWLDomainEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.RangeEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.RangeEntailment.ToString()] = OWLRangeEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.SameAsTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.SameAsTransitivity.ToString()] = OWLSameAsTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.DifferentFromEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.DifferentFromEntailment.ToString()] = OWLDifferentFromEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.IndividualTypeEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.IndividualTypeEntailment.ToString()] = OWLIndividualTypeEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.SymmetricPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.SymmetricPropertyEntailment.ToString()] = OWLSymmetricPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.TransitivePropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.TransitivePropertyEntailment.ToString()] = OWLTransitivePropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.ReflexivePropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.ReflexivePropertyEntailment.ToString()] = OWLReflexivePropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.InverseOfEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.InverseOfEntailment.ToString()] = OWLInverseOfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.PropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.PropertyEntailment.ToString()] = OWLPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.SameAsEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.SameAsEntailment.ToString()] = OWLSameAsEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.HasValueEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.HasValueEntailment.ToString()] = OWLHasValueEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.HasSelfEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.HasSelfEntailment.ToString()] = OWLHasSelfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.HasKeyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.HasKeyEntailment.ToString()] = OWLHasKeyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.PropertyChainEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.PropertyChainEntailment.ToString()] = OWLPropertyChainEntailmentRule.ExecuteRule(ontology);
                                break;
                             case OWLEnums.OWLReasonerStandardRules.FunctionalEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.FunctionalEntailment.ToString()] = OWLFunctionalEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerStandardRules.InverseFunctionalEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerStandardRules.InverseFunctionalEntailment.ToString()] = OWLInverseFunctionalEntailmentRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed standard reasoner rule '{standardRule}': found {inferenceRegistry[standardRule.ToString()].EvidencesCount} evidences");
                    });

                //Execute extension rules (OWL-TIME)
                TIMEReasoner.ApplyToOntology(this, ontology, inferenceRegistry);

                //Execute custom rules (SWRL)
                Parallel.ForEach(CustomRules, 
                    customRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching custom (SWRL) reasoner rule '{customRule.RuleName}'");

                        inferenceRegistry[customRule.RuleName] = customRule.ApplyToOntology(ontology);

                        OWLEvents.RaiseInfo($"Completed custom (SWRL) reasoner rule '{customRule.RuleName}': found {inferenceRegistry[customRule.RuleName].EvidencesCount} evidences");
                    });

                //Process inference registry
                foreach (OWLReasonerReport inferenceRegistryReport in inferenceRegistry.Values)
                    reasonerReport.MergeEvidences(inferenceRegistryReport);

                OWLEvents.RaiseInfo($"Reasoner has been applied on Ontology '{ontology.URI}': found {reasonerReport.EvidencesCount} evidences");
            }

            return reasonerReport;
        }

        /// <summary>
        /// Asynchronously applies the reasoner on the given ontology
        /// </summary>
        public Task<OWLReasonerReport> ApplyToOntologyAsync(OWLOntology ontology)
            => Task.Run(() => ApplyToOntology(ontology));
        #endregion
    }
}