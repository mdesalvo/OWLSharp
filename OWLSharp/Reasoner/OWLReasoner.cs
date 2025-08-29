/*
   Copyright 2014-2025 Marco De Salvo
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

#if !NET8_0_OR_GREATER
using Dasync.Collections;
#endif
using OWLSharp.Ontology;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Reasoner
{
    public sealed class OWLReasoner
    {
        #region Properties
        public List<OWLEnums.OWLReasonerRules> Rules { get; internal set; } = new List<OWLEnums.OWLReasonerRules>();
        #endregion

        #region Methods
        public OWLReasoner AddRule(OWLEnums.OWLReasonerRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        public async Task<List<OWLInference>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching OWL2/SWRL reasoner on ontology '{ontology.IRI}'...");
                Rules = Rules.Distinct().ToList();

                //Initialize inference registry
                Dictionary<string, List<OWLInference>> inferenceRegistry = new Dictionary<string, List<OWLInference>>(Rules.Count + ontology.Rules.Count);
                Rules.ForEach(owl2Rule => inferenceRegistry.Add(owl2Rule.ToString(), null));
                ontology.Rules.ForEach(swrlRule => inferenceRegistry.Add(swrlRule.ToString(), null));

                //Initialize reasoner context
                OWLReasonerContext reasonerContext = new OWLReasonerContext
                {
                    ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
                    DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
                    ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
                };

                //Execute OWL2 reasoner rules
#if !NET8_0_OR_GREATER
                await Rules.ParallelForEachAsync(async (rule, _) =>
#else
                await Parallel.ForEachAsync(Rules, async (rule, _) =>
#endif
                {
                    string ruleString = rule.ToString();
                    OWLEvents.RaiseInfo($"Launching OWL2 rule {ruleString}...");

                    switch (rule)
                    {
                        case OWLEnums.OWLReasonerRules.ClassAssertionEntailment:
                            inferenceRegistry[OWLClassAssertionEntailmentRule.rulename] = OWLClassAssertionEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.DataPropertyDomainEntailment:
                            inferenceRegistry[OWLDataPropertyDomainEntailmentRule.rulename] = OWLDataPropertyDomainEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.DifferentIndividualsEntailment:
                            inferenceRegistry[OWLDifferentIndividualsEntailmentRule.rulename] = OWLDifferentIndividualsEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.DisjointClassesEntailment:
                            inferenceRegistry[OWLDisjointClassesEntailmentRule.rulename] = OWLDisjointClassesEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.DisjointDataPropertiesEntailment:
                            inferenceRegistry[OWLDisjointDataPropertiesEntailmentRule.rulename] = OWLDisjointDataPropertiesEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.DisjointObjectPropertiesEntailment:
                            inferenceRegistry[OWLDisjointObjectPropertiesEntailmentRule.rulename] = OWLDisjointObjectPropertiesEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.EquivalentClassesEntailment:
                            inferenceRegistry[OWLEquivalentClassesEntailmentRule.rulename] = OWLEquivalentClassesEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.EquivalentDataPropertiesEntailment:
                            inferenceRegistry[OWLEquivalentDataPropertiesEntailmentRule.rulename] = OWLEquivalentDataPropertiesEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.EquivalentObjectPropertiesEntailment:
                            inferenceRegistry[OWLEquivalentObjectPropertiesEntailmentRule.rulename] = OWLEquivalentObjectPropertiesEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment:
                            inferenceRegistry[OWLFunctionalObjectPropertyEntailmentRule.rulename] = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.HasKeyEntailment:
                            inferenceRegistry[OWLHasKeyEntailmentRule.rulename] = OWLHasKeyEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.HasSelfEntailment:
                            inferenceRegistry[OWLHasSelfEntailmentRule.rulename] = OWLHasSelfEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.HasValueEntailment:
                            inferenceRegistry[OWLHasValueEntailmentRule.rulename] = OWLHasValueEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.InverseFunctionalObjectPropertyEntailment:
                            inferenceRegistry[OWLInverseFunctionalObjectPropertyEntailmentRule.rulename] = OWLInverseFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment:
                            inferenceRegistry[OWLInverseObjectPropertiesEntailmentRule.rulename] = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.ObjectPropertyChainEntailment:
                            inferenceRegistry[OWLObjectPropertyChainEntailmentRule.rulename] = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.ObjectPropertyDomainEntailment:
                            inferenceRegistry[OWLObjectPropertyDomainEntailmentRule.rulename] = OWLObjectPropertyDomainEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.ObjectPropertyRangeEntailment:
                            inferenceRegistry[OWLObjectPropertyRangeEntailmentRule.rulename] = OWLObjectPropertyRangeEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.ReflexiveObjectPropertyEntailment:
                            inferenceRegistry[OWLReflexiveObjectPropertyEntailmentRule.rulename] = OWLReflexiveObjectPropertyEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.SameIndividualEntailment:
                            inferenceRegistry[OWLSameIndividualEntailmentRule.rulename] = OWLSameIndividualEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.SubClassOfEntailment:
                            inferenceRegistry[OWLSubClassOfEntailmentRule.rulename] = OWLSubClassOfEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.SubDataPropertyOfEntailment:
                            inferenceRegistry[OWLSubDataPropertyOfEntailmentRule.rulename] = OWLSubDataPropertyOfEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.SubObjectPropertyOfEntailment:
                            inferenceRegistry[OWLSubObjectPropertyOfEntailmentRule.rulename] = OWLSubObjectPropertyOfEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.SymmetricObjectPropertyEntailment:
                            inferenceRegistry[OWLSymmetricObjectPropertyEntailmentRule.rulename] = OWLSymmetricObjectPropertyEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                        case OWLEnums.OWLReasonerRules.TransitiveObjectPropertyEntailment:
                            inferenceRegistry[OWLTransitiveObjectPropertyEntailmentRule.rulename] = OWLTransitiveObjectPropertyEntailmentRule.ExecuteRule(ontology, reasonerContext);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL2 rule {ruleString} => {inferenceRegistry[ruleString].Count} candidate inferences");
                });

                //Execute SWRL reasoner rules
#if !NET8_0_OR_GREATER
                await ontology.Rules.ParallelForEachAsync(async (swrlRule, _) =>
#else
                await Parallel.ForEachAsync(ontology.Rules, async (swrlRule, _) =>
#endif
                {
                    string swrlRuleString = swrlRule.ToString();
                    OWLEvents.RaiseInfo($"Launching SWRL rule {swrlRuleString}...");

                    inferenceRegistry[swrlRuleString] = await swrlRule.ApplyToOntologyAsync(ontology);

                    OWLEvents.RaiseInfo($"Completed SWRL rule {swrlRuleString} => {inferenceRegistry[swrlRuleString].Count} candidate inferences");
                });

                //Process inferences: fetch axioms commonly targeted by rules
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

                //Process inferences: deduplicate inferences by analyzing explicit knowledge
                foreach (KeyValuePair<string, List<OWLInference>> inferenceRegistryEntry in inferenceRegistry.Where(ir => ir.Value?.Count > 0))
                    inferenceRegistryEntry.Value.RemoveAll(inf =>
                    {
                        string infXML = inf.Axiom.GetXML();
                        return clsAsnAxiomsTask.Result.Contains(infXML)
                               || dtPropAsnAxiomsTask.Result.Contains(infXML)
                               || opPropAsnAxiomsTask.Result.Contains(infXML)
                               || diffIdvsAxiomsTask.Result.Contains(infXML)
                               || sameIdvsAxiomsTask.Result.Contains(infXML)
                               || dsjClsAxiomsTask.Result.Contains(infXML)
                               || eqvClsAxiomsTask.Result.Contains(infXML)
                               || subClsAxiomsTask.Result.Contains(infXML)
                               || dsjDtPropAxiomsTask.Result.Contains(infXML)
                               || eqvDtPropAxiomsTask.Result.Contains(infXML)
                               || subDtPropAxiomsTask.Result.Contains(infXML)
                               || dsjOpPropAxiomsTask.Result.Contains(infXML)
                               || eqvOpPropAxiomsTask.Result.Contains(infXML)
                               || subOpPropAxiomsTask.Result.Contains(infXML)
                               || invOpPropAxiomsTask.Result.Contains(infXML);
                    });

                //Process inferences: collect inferences and perform final cleanup
                inferences.AddRange(inferenceRegistry.SelectMany(ir => ir.Value ?? Enumerable.Empty<OWLInference>()).Distinct());
                inferenceRegistry.Clear();

                OWLEvents.RaiseInfo($"Completed OWL2/SWRL reasoner on ontology {ontology.IRI} => {inferences.Count} unique inferences");
            }

            return inferences;
        }
#endregion
    }

    internal sealed class OWLReasonerContext
    {
        internal List<OWLClassAssertion> ClassAssertions { get; set; }
        internal List<OWLDataPropertyAssertion> DataPropertyAssertions { get; set; }
        internal List<OWLObjectPropertyAssertion> ObjectPropertyAssertions { get; set; }
    }
}