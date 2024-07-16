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

                //Fetch inference-targeted ontology axioms
                Task<List<string>> clsAsnAxiomsTask = Task.Run(() => ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> dtPropAsnAxiomsTask = Task.Run(() => ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> opPropAsnAxiomsTask = Task.Run(() => ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> diffIdvsAxiomsTask = Task.Run(() => ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> sameIdvsAxiomsTask = Task.Run(() => ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> dsjClsAxiomsTask = Task.Run(() => ontology.GetClassAxiomsOfType<OWLDisjointClasses>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> eqvClsAxiomsTask = Task.Run(() => ontology.GetClassAxiomsOfType<OWLEquivalentClasses>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> subClsAxiomsTask = Task.Run(() => ontology.GetClassAxiomsOfType<OWLSubClassOf>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> dsjDtPropAxiomsTask = Task.Run(() => ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> eqvDtPropAxiomsTask = Task.Run(() => ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> subDtPropAxiomsTask = Task.Run(() => ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> dsjOpPropAxiomsTask = Task.Run(() => ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> eqvOpPropAxiomsTask = Task.Run(() => ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> subOpPropAxiomsTask = Task.Run(() => ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Select(asn => asn.GetXML()).ToList());
                Task<List<string>> invOpPropAxiomsTask = Task.Run(() => ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>().Select(asn => asn.GetXML()).ToList());
                await Task.WhenAll(clsAsnAxiomsTask, dtPropAsnAxiomsTask, opPropAsnAxiomsTask, diffIdvsAxiomsTask, sameIdvsAxiomsTask, dsjClsAxiomsTask, eqvClsAxiomsTask, subClsAxiomsTask,
                    dsjDtPropAxiomsTask, eqvDtPropAxiomsTask, subDtPropAxiomsTask, dsjOpPropAxiomsTask, eqvOpPropAxiomsTask, subOpPropAxiomsTask, invOpPropAxiomsTask);

                //Process inference registry
                foreach (KeyValuePair<string, List<OWLInference>> inferenceRegistryEntry in inferenceRegistry.Where(ir => ir.Value?.Count > 0))
                    inferenceRegistryEntry.Value.RemoveAll(inf => 
                    {
                        string infXML = inf.Axiom.GetXML();
                        return clsAsnAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || dtPropAsnAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || opPropAsnAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || diffIdvsAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || sameIdvsAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || dsjClsAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || eqvClsAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || subClsAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || dsjDtPropAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || eqvDtPropAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || subDtPropAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || dsjOpPropAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || eqvOpPropAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || subOpPropAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML))
                                || invOpPropAxiomsTask.GetAwaiter().GetResult().Any(axiomXML => string.Equals(infXML, axiomXML));
                    });

                OWLEvents.RaiseInfo($"Completed reasoner on ontology {ontology.IRI} ({inferences.Count} inferences)");
            }

            return inferences;
        }
        #endregion
    }
}