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
using System.Threading.Tasks;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Reasoner.Rules;

namespace OWLSharp.Reasoner
{
    public class OWLReasoner
    {
        #region Properties
        public List<OWLEnums.OWLReasonerRules> StandardRules { get; internal set; }
        #endregion

        #region Ctors
        public OWLReasoner()
			=> StandardRules = new List<OWLEnums.OWLReasonerRules>();
        #endregion

        #region Methods
        public List<OWLAxiom> ApplyToOntology(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Reasoner is going to be applied to ontology '{ontology.IRI}'...");

                //Initialize inference registry
                Dictionary<string, List<OWLAxiom>> inferenceRegistry = new Dictionary<string, List<OWLAxiom>>();
				StandardRules.ForEach(standardRule => inferenceRegistry.Add(standardRule.ToString(), null));

                //Execute standard rules
                Parallel.ForEach(StandardRules.Distinct(), 
                    standardRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching standard reasoner rule '{standardRule}'");

                        switch (standardRule)
                        {
                            case OWLEnums.OWLReasonerRules.SubClassOfEntailment:
								inferenceRegistry[OWLEnums.OWLReasonerRules.SubClassOfEntailment.ToString()] = OWLSubClassOfEntailmentRule.ExecuteRule(ontology);
								break;
							case OWLEnums.OWLReasonerRules.EquivalentClassesEntailment:
								inferenceRegistry[OWLEnums.OWLReasonerRules.EquivalentClassesEntailment.ToString()] = OWLEquivalentClassesEntailmentRule.ExecuteRule(ontology);
								break;
							case OWLEnums.OWLReasonerRules.DisjointClassesEntailment:
								inferenceRegistry[OWLEnums.OWLReasonerRules.DisjointClassesEntailment.ToString()] = OWLDisjointClassesEntailmentRule.ExecuteRule(ontology);
								break;
                            case OWLEnums.OWLReasonerRules.SubDataPropertyOfEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SubDataPropertyOfEntailment.ToString()] = OWLSubDataPropertyOfEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.SubObjectPropertyOfEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SubObjectPropertyOfEntailment.ToString()] = OWLSubObjectPropertyOfEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.EquivalentDataPropertiesEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.EquivalentDataPropertiesEntailment.ToString()] = OWLEquivalentDataPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.EquivalentObjectPropertiesEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.EquivalentObjectPropertiesEntailment.ToString()] = OWLEquivalentObjectPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DisjointDataPropertiesEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DisjointDataPropertiesEntailment.ToString()] = OWLDisjointDataPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DisjointObjectPropertiesEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DisjointObjectPropertiesEntailment.ToString()] = OWLDisjointObjectPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.SameIndividualEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SameIndividualEntailment.ToString()] = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.DifferentIndividualsEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DifferentIndividualsEntailment.ToString()] = OWLDifferentIndividualsEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment.ToString()] = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.InverseFunctionalObjectPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.InverseFunctionalObjectPropertyEntailment.ToString()] = OWLInverseFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment.ToString()] = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SymmetricObjectPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.SymmetricObjectPropertyEntailment.ToString()] = OWLSymmetricObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.ReflexiveObjectPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.ReflexiveObjectPropertyEntailment.ToString()] = OWLReflexiveObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.TransitiveObjectPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.TransitiveObjectPropertyEntailment.ToString()] = OWLTransitiveObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.ObjectPropertyChainEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.ObjectPropertyChainEntailment.ToString()] = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology);
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
							case OWLEnums.OWLReasonerRules.ClassAssertionEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.ClassAssertionEntailment.ToString()] = OWLClassAssertionEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.DataPropertyDomainEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.DataPropertyDomainEntailment.ToString()] = OWLDataPropertyDomainEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.ObjectPropertyDomainEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.ObjectPropertyDomainEntailment.ToString()] = OWLObjectPropertyDomainEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.ObjectPropertyRangeEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.ObjectPropertyRangeEntailment.ToString()] = OWLObjectPropertyRangeEntailmentRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed standard reasoner rule '{standardRule}': {inferenceRegistry[standardRule.ToString()].Count} inferences");
                    });

                //Process inference registry
                foreach (KeyValuePair<string, List<OWLAxiom>> inferenceRegistryEntries in inferenceRegistry)
                    inferences.AddRange(inferenceRegistryEntries.Value);

                OWLEvents.RaiseInfo($"Reasoner has been applied to ontology '{ontology.IRI}': {inferences.Count} inferences");
            }

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }
        #endregion
    }
}