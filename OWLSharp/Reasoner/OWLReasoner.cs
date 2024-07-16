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
using OWLSharp.Ontology.Helpers;
using OWLSharp.Reasoner.Rules;

namespace OWLSharp.Reasoner
{
    public class OWLReasoner
    {
        #region Properties
        public List<OWLEnums.OWLReasonerRules> Rules { get; internal set; }
        #endregion

        #region Ctors
        public OWLReasoner()
			=> Rules = new List<OWLEnums.OWLReasonerRules>();
        #endregion

        #region Methods
        public async Task<List<OWLInference>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching reasoner on ontology '{ontology.IRI}'...");

                //Initialize inference registry
                Dictionary<string, List<OWLInference>> inferenceRegistry = new Dictionary<string, List<OWLInference>>();
				Rules.ForEach(rule => inferenceRegistry.Add(rule.ToString(), null));

                //Execute reasoner rules
                Parallel.ForEach(Rules.Distinct(), 
                    rule =>
                    {
                        OWLEvents.RaiseInfo($"Launching rule {rule}...");

                        switch (rule)
                        {
                            case OWLEnums.OWLReasonerRules.ClassAssertionEntailment:
                                inferenceRegistry[OWLClassAssertionEntailmentRule.rulename] = OWLClassAssertionEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DataPropertyDomainEntailment:
                                inferenceRegistry[OWLDataPropertyDomainEntailmentRule.rulename] = OWLDataPropertyDomainEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DifferentIndividualsEntailment:
                                inferenceRegistry[OWLDifferentIndividualsEntailmentRule.rulename] = OWLDifferentIndividualsEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DisjointClassesEntailment:
                                inferenceRegistry[OWLDisjointClassesEntailmentRule.rulename] = OWLDisjointClassesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DisjointDataPropertiesEntailment:
                                inferenceRegistry[OWLDisjointDataPropertiesEntailmentRule.rulename] = OWLDisjointDataPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.DisjointObjectPropertiesEntailment:
                                inferenceRegistry[OWLDisjointObjectPropertiesEntailmentRule.rulename] = OWLDisjointObjectPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.EquivalentClassesEntailment:
                                inferenceRegistry[OWLEquivalentClassesEntailmentRule.rulename] = OWLEquivalentClassesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.EquivalentDataPropertiesEntailment:
                                inferenceRegistry[OWLEquivalentDataPropertiesEntailmentRule.rulename] = OWLEquivalentDataPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.EquivalentObjectPropertiesEntailment:
                                inferenceRegistry[OWLEquivalentObjectPropertiesEntailmentRule.rulename] = OWLEquivalentObjectPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment:
                                inferenceRegistry[OWLFunctionalObjectPropertyEntailmentRule.rulename] = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.HasKeyEntailment:
                                inferenceRegistry[OWLHasKeyEntailmentRule.rulename] = OWLHasKeyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.HasSelfEntailment:
                                inferenceRegistry[OWLHasSelfEntailmentRule.rulename] = OWLHasSelfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.HasValueEntailment:
                                inferenceRegistry[OWLHasValueEntailmentRule.rulename] = OWLHasValueEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.InverseFunctionalObjectPropertyEntailment:
                                inferenceRegistry[OWLInverseFunctionalObjectPropertyEntailmentRule.rulename] = OWLInverseFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment:
                                inferenceRegistry[OWLInverseObjectPropertiesEntailmentRule.rulename] = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.ObjectPropertyChainEntailment:
                                inferenceRegistry[OWLObjectPropertyChainEntailmentRule.rulename] = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.ObjectPropertyDomainEntailment:
                                inferenceRegistry[OWLObjectPropertyDomainEntailmentRule.rulename] = OWLObjectPropertyDomainEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.ObjectPropertyRangeEntailment:
                                inferenceRegistry[OWLObjectPropertyRangeEntailmentRule.rulename] = OWLObjectPropertyRangeEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.ReflexiveObjectPropertyEntailment:
                                inferenceRegistry[OWLReflexiveObjectPropertyEntailmentRule.rulename] = OWLReflexiveObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SameIndividualEntailment:
                                inferenceRegistry[OWLSameIndividualEntailmentRule.rulename] = OWLSameIndividualEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SubClassOfEntailment:
								inferenceRegistry[OWLSubClassOfEntailmentRule.rulename] = OWLSubClassOfEntailmentRule.ExecuteRule(ontology);
								break;
                            case OWLEnums.OWLReasonerRules.SubDataPropertyOfEntailment:
                                inferenceRegistry[OWLSubDataPropertyOfEntailmentRule.rulename] = OWLSubDataPropertyOfEntailmentRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLReasonerRules.SubObjectPropertyOfEntailment:
                                inferenceRegistry[OWLSubObjectPropertyOfEntailmentRule.rulename] = OWLSubObjectPropertyOfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLReasonerRules.SymmetricObjectPropertyEntailment:
                                inferenceRegistry[OWLSymmetricObjectPropertyEntailmentRule.rulename] = OWLSymmetricObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;							
                            case OWLEnums.OWLReasonerRules.TransitiveObjectPropertyEntailment:
                                inferenceRegistry[OWLTransitiveObjectPropertyEntailmentRule.rulename] = OWLTransitiveObjectPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed rule {rule}");
                    });

                //Process inference registry
                Task processInferenceRegistryTask = Task.Run(async () =>
                {
                    //Fetch axioms commonly targeted by inferences
                    Task<HashSet<string>> clsAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> dtPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> opPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> diffIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> sameIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> dsjClsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetClassAxiomsOfType<OWLDisjointClasses>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> eqvClsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetClassAxiomsOfType<OWLEquivalentClasses>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> subClsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetClassAxiomsOfType<OWLSubClassOf>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> dsjDtPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> eqvDtPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> subDtPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> dsjOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> eqvOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> subOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> invOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>().Select(asn => asn.GetXML())));
                    await Task.WhenAll(clsAsnAxiomsTask, dtPropAsnAxiomsTask, opPropAsnAxiomsTask, diffIdvsAxiomsTask, sameIdvsAxiomsTask, dsjClsAxiomsTask, eqvClsAxiomsTask, subClsAxiomsTask,
                        dsjDtPropAxiomsTask, eqvDtPropAxiomsTask, subDtPropAxiomsTask, dsjOpPropAxiomsTask, eqvOpPropAxiomsTask, subOpPropAxiomsTask, invOpPropAxiomsTask);

                    //Deduplicate inferences by analyzing fetched axioms
                    foreach (KeyValuePair<string, List<OWLInference>> inferenceRegistryEntry in inferenceRegistry.Where(ir => ir.Value?.Count > 0))
                        inferenceRegistryEntry.Value.RemoveAll(inf =>
                        {
                            string infXML = inf.Axiom.GetXML();
                            return clsAsnAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || dtPropAsnAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || opPropAsnAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || diffIdvsAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || sameIdvsAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || dsjClsAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || eqvClsAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || subClsAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || dsjDtPropAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || eqvDtPropAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || subDtPropAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || dsjOpPropAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || eqvOpPropAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || subOpPropAxiomsTask.GetAwaiter().GetResult().Contains(infXML)
                                    || invOpPropAxiomsTask.GetAwaiter().GetResult().Contains(infXML);
                        });
                });
                await processInferenceRegistryTask;

                OWLEvents.RaiseInfo($"Completed reasoner on ontology {ontology.IRI} ({inferences.Count} inferences)");
            }

            return inferences;
        }
        #endregion
    }
}