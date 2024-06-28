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
                OWLEvents.RaiseInfo($"Reasoner is going to be applied to ontology '{ontology.IRI}': this may require intensive processing, depending on size and complexity of domain knowledge and rules");

                //Initialize inference registry
                Dictionary<string, List<OWLAxiom>> inferenceRegistry = new Dictionary<string, List<OWLAxiom>>();
				StandardRules.ForEach(standardRule => inferenceRegistry.Add(standardRule.ToString(), null));

                //Execute standard rules
                Parallel.ForEach(StandardRules, 
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
                            case OWLEnums.OWLReasonerRules.FunctionalDataPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.FunctionalDataPropertyEntailment.ToString()] = OWLFunctionalDataPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment:
                                inferenceRegistry[OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment.ToString()] = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed standard reasoner rule '{standardRule}': got {inferenceRegistry[standardRule.ToString()].Count} inferences");
                    });

                //Process inference registry
                foreach (KeyValuePair<string, List<OWLAxiom>> inferenceRegistryEntries in inferenceRegistry)
                    inferences.AddRange(inferenceRegistryEntries.Value);

                OWLEvents.RaiseInfo($"Reasoner has been applied to ontology '{ontology.IRI}': got {inferences.Count} inferences");
            }

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }
        #endregion
    }
}