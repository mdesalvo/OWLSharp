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
        /// Dictionary of rules applied by the reasoner, categorized by reserved keys
        /// </summary>
        internal Dictionary<string, object> Rules { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty reasoner
        /// </summary>
        public OWLReasoner()
            => Rules = new Dictionary<string, object>()
               {
                   { "STD", new List<OWLEnums.OWLReasonerRules>() },
                   //Extensions
                   { "SWRL", new List<SWRLRule>() },
                   { "TIME", new List<TIMEEnums.TIMEReasonerRules>() }
               };
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given RDFS/OWL/OWL2 rule to the reasoner
        /// </summary>
        public OWLReasoner AddRule(OWLEnums.OWLReasonerRules standardRule)
        {
            if (!((List<OWLEnums.OWLReasonerRules>)Rules["STD"]).Contains(standardRule))
                ((List<OWLEnums.OWLReasonerRules>)Rules["STD"]).Add(standardRule);
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
                foreach (OWLEnums.OWLReasonerRules stdRule in ((List<OWLEnums.OWLReasonerRules>)Rules["STD"]))
                    inferenceRegistry.Add(stdRule.ToString(), null);

                //Execute standard rules (RDFS, OWL, OWL2)
                Parallel.ForEach(((List<OWLEnums.OWLReasonerRules>)Rules["STD"]), 
                    stdRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching standard reasoner rule '{stdRule}'");

                        switch (stdRule)
                        {
                            case OWLEnums.OWLReasonerRules.SubClassTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SubClassTransitivity.ToString()] = OWLSubClassTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SubPropertyTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SubPropertyTransitivity.ToString()] = OWLSubPropertyTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.EquivalentClassTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.EquivalentClassTransitivity.ToString()] = OWLEquivalentClassTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.EquivalentPropertyTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.EquivalentPropertyTransitivity.ToString()] = OWLEquivalentPropertyTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DisjointClassEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DisjointClassEntailment.ToString()] = OWLDisjointClassEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DisjointPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DisjointPropertyEntailment.ToString()] = OWLDisjointPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DomainEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DomainEntailment.ToString()] = OWLDomainEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.RangeEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.RangeEntailment.ToString()] = OWLRangeEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SameAsTransitivity:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SameAsTransitivity.ToString()] = OWLSameAsTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DifferentFromEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DifferentFromEntailment.ToString()] = OWLDifferentFromEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.IndividualTypeEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.IndividualTypeEntailment.ToString()] = OWLIndividualTypeEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SymmetricPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SymmetricPropertyEntailment.ToString()] = OWLSymmetricPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.TransitivePropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.TransitivePropertyEntailment.ToString()] = OWLTransitivePropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.ReflexivePropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.ReflexivePropertyEntailment.ToString()] = OWLReflexivePropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.InverseOfEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.InverseOfEntailment.ToString()] = OWLInverseOfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.PropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.PropertyEntailment.ToString()] = OWLPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SameAsEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SameAsEntailment.ToString()] = OWLSameAsEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.HasValueEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.HasValueEntailment.ToString()] = OWLHasValueEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.HasSelfEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.HasSelfEntailment.ToString()] = OWLHasSelfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.HasKeyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.HasKeyEntailment.ToString()] = OWLHasKeyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.PropertyChainEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.PropertyChainEntailment.ToString()] = OWLPropertyChainEntailmentRule.ExecuteRule(ontology);
                                break;
                             case OWLEnums.OWLReasonerRules.FunctionalEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.FunctionalEntailment.ToString()] = OWLFunctionalEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.InverseFunctionalEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.InverseFunctionalEntailment.ToString()] = OWLInverseFunctionalEntailmentRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed standard reasoner rule '{stdRule}': found {inferenceRegistry[stdRule.ToString()].EvidencesCount} evidences");
                    });

                //Execute extension rules (SWRL, TIME)
                SWRLReasoner.ApplyToOntology(this, ontology, inferenceRegistry);
                TIMEReasoner.ApplyToOntology(this, ontology, inferenceRegistry);

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