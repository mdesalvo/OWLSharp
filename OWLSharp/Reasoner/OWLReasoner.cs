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
using System;
using OWLSharp.Ontology;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Reasoner
{
    /// <summary>
    /// OWLReasoner is an inference engine that processes an ontology by analyzing its
    /// T-BOX (terminological/schema knowledge about classes and properties),
    /// A-BOX (assertional knowledge about individuals and their relationships), and
    /// R-BOX (rules for additional inference patterns).
    /// The reasoner applies logical deduction to derive implicit knowledge that is entailed
    /// but not explicitly stated, such as inferring class memberships, property relationships,
    /// detecting inconsistencies, and computing class hierarchies, thereby materializing
    /// the full logical consequences of the ontology's axioms and rules.
    /// </summary>
    public sealed class OWLReasoner
    {
        #region Properties
        /// <summary>
        /// A predefined reasoner including all available OWL2 reasoner rules
        /// </summary>
        public static readonly OWLReasoner Default = new OWLReasoner {
            Rules = Enum.GetValues(typeof(OWLEnums.OWLReasonerRules)).Cast<OWLEnums.OWLReasonerRules>().ToList() };

        /// <summary>
        /// The set of rules to be applied by the reasoner
        /// </summary>
        public List<OWLEnums.OWLReasonerRules> Rules { get; internal set; } = new List<OWLEnums.OWLReasonerRules>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given rule to the reasoner
        /// </summary>
        /// <returns>The reasoner itself</returns>
        public OWLReasoner AddRule(OWLEnums.OWLReasonerRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        /// <summary>
        /// Applies the reasoner to the given ontology. If specified, it automatically merges the inferences into the ontology.
        /// </summary>
        /// <returns>The list of discovered inferences</returns>
        public async Task<List<OWLInference>> ApplyToOntologyAsync(OWLOntology ontology, bool shouldMergeInferences=false)
        {
            List<OWLInference> inferences = new List<OWLInference>();
            Rules = Rules.Distinct().ToList();

            if (ontology != null)
            {
                #region Discover inferences
                OWLEvents.RaiseInfo($"Launching OWL2/SWRL reasoner on ontology '{ontology.IRI}'...");

                #region Init registry & context
                //Initialize inference registry
                Dictionary<string, List<OWLInference>> inferenceRegistry = new Dictionary<string, List<OWLInference>>(Rules.Count + ontology.Rules.Count);
                Rules.ForEach(owl2Rule => inferenceRegistry.Add(owl2Rule.ToString(), null));
                ontology.Rules.ForEach(swrlRule => inferenceRegistry.Add(swrlRule.ToString(), null));

                //Initialize axioms XML (required for inference deduplication phase)
                Task<HashSet<string>> clsAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Select(asn => asn.GetXML())));
                Task<HashSet<string>> dtPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Select(asn => asn.GetXML())));
                Task<HashSet<string>> opPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Select(asn => asn.GetXML())));
                Task<HashSet<string>> sameIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Select(asn => asn.GetXML())));
                Task<HashSet<string>> diffIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Select(asn => asn.GetXML())));
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
                Task<HashSet<string>> opDomainAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>().Select(asn => asn.GetXML())));
                Task<HashSet<string>> opRangeAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>().Select(asn => asn.GetXML())));
                #endregion

                #region Process rules
                //Execute OWL2 reasoner rules
                Parallel.ForEach(Rules, rule =>
                {
                    string ruleString = rule.ToString();
                    OWLEvents.RaiseInfo($"Launching OWL2 rule {ruleString}...");

                    switch (rule)
                    {
                        case OWLEnums.OWLReasonerRules.ClassAssertionEntailment:
                            inferenceRegistry[OWLClassAssertionEntailment.rulename] = OWLClassAssertionEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.DataPropertyDomainEntailment:
                            inferenceRegistry[OWLDataPropertyDomainEntailment.rulename] = OWLDataPropertyDomainEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.DifferentIndividualsEntailment:
                            inferenceRegistry[OWLDifferentIndividualsEntailment.rulename] = OWLDifferentIndividualsEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.DisjointClassesEntailment:
                            inferenceRegistry[OWLDisjointClassesEntailment.rulename] = OWLDisjointClassesEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.DisjointDataPropertiesEntailment:
                            inferenceRegistry[OWLDisjointDataPropertiesEntailment.rulename] = OWLDisjointDataPropertiesEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.DisjointObjectPropertiesEntailment:
                            inferenceRegistry[OWLDisjointObjectPropertiesEntailment.rulename] = OWLDisjointObjectPropertiesEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.EquivalentClassesEntailment:
                            inferenceRegistry[OWLEquivalentClassesEntailment.rulename] = OWLEquivalentClassesEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.EquivalentDataPropertiesEntailment:
                            inferenceRegistry[OWLEquivalentDataPropertiesEntailment.rulename] = OWLEquivalentDataPropertiesEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.EquivalentObjectPropertiesEntailment:
                            inferenceRegistry[OWLEquivalentObjectPropertiesEntailment.rulename] = OWLEquivalentObjectPropertiesEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment:
                            inferenceRegistry[OWLFunctionalObjectPropertyEntailment.rulename] = OWLFunctionalObjectPropertyEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.HasKeyEntailment:
                            inferenceRegistry[OWLHasKeyEntailment.rulename] = OWLHasKeyEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.HasSelfEntailment:
                            inferenceRegistry[OWLHasSelfEntailment.rulename] = OWLHasSelfEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.HasValueEntailment:
                            inferenceRegistry[OWLHasValueEntailment.rulename] = OWLHasValueEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.InverseFunctionalObjectPropertyEntailment:
                            inferenceRegistry[OWLInverseFunctionalObjectPropertyEntailment.rulename] = OWLInverseFunctionalObjectPropertyEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment:
                            inferenceRegistry[OWLInverseObjectPropertiesEntailment.rulename] = OWLInverseObjectPropertiesEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.ObjectPropertyChainEntailment:
                            inferenceRegistry[OWLObjectPropertyChainEntailment.rulename] = OWLObjectPropertyChainEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.ObjectPropertyDomainEntailment:
                            inferenceRegistry[OWLObjectPropertyDomainEntailment.rulename] = OWLObjectPropertyDomainEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.ObjectPropertyRangeEntailment:
                            inferenceRegistry[OWLObjectPropertyRangeEntailment.rulename] = OWLObjectPropertyRangeEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.ReflexiveObjectPropertyEntailment:
                            inferenceRegistry[OWLReflexiveObjectPropertyEntailment.rulename] = OWLReflexiveObjectPropertyEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.SameIndividualEntailment:
                            inferenceRegistry[OWLSameIndividualEntailment.rulename] = OWLSameIndividualEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.SubClassOfEntailment:
                            inferenceRegistry[OWLSubClassOfEntailment.rulename] = OWLSubClassOfEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.SubDataPropertyOfEntailment:
                            inferenceRegistry[OWLSubDataPropertyOfEntailment.rulename] = OWLSubDataPropertyOfEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.SubObjectPropertyOfEntailment:
                            inferenceRegistry[OWLSubObjectPropertyOfEntailment.rulename] = OWLSubObjectPropertyOfEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.SymmetricObjectPropertyEntailment:
                            inferenceRegistry[OWLSymmetricObjectPropertyEntailment.rulename] = OWLSymmetricObjectPropertyEntailment.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLReasonerRules.TransitiveObjectPropertyEntailment:
                            inferenceRegistry[OWLTransitiveObjectPropertyEntailment.rulename] = OWLTransitiveObjectPropertyEntailment.ExecuteRule(ontology);
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
                #endregion

                #region Deduplicate & finalize inferences
                //Deduplicate inferences (in order to not state already known knowledge)
                await Task.WhenAll(clsAsnAxiomsTask, dtPropAsnAxiomsTask, opPropAsnAxiomsTask, sameIdvsAxiomsTask, diffIdvsAxiomsTask, dsjClsAxiomsTask, eqvClsAxiomsTask, subClsAxiomsTask,
                    dsjDtPropAxiomsTask, eqvDtPropAxiomsTask, subDtPropAxiomsTask, dsjOpPropAxiomsTask, eqvOpPropAxiomsTask, subOpPropAxiomsTask, invOpPropAxiomsTask, opDomainAxiomsTask, opRangeAxiomsTask);
                foreach (KeyValuePair<string, List<OWLInference>> inferenceRegistryEntry in inferenceRegistry.Where(ir => ir.Value?.Count > 0))
                    inferenceRegistryEntry.Value.RemoveAll(inf =>
                    {
                        switch (inf.Axiom.GetType().Name)
                        {
                            case nameof(OWLClassAssertion):
                                return clsAsnAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLDataPropertyAssertion):
                                return dtPropAsnAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLObjectPropertyAssertion):
                                return opPropAsnAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLSameIndividual):
                                return sameIdvsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLDifferentIndividuals):
                                return diffIdvsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLDisjointClasses):
                                return dsjClsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLEquivalentClasses):
                                return eqvClsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLSubClassOf):
                                return subClsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLDisjointDataProperties):
                                return dsjDtPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLEquivalentDataProperties):
                                return eqvDtPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLSubDataPropertyOf):
                                return subDtPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLDisjointObjectProperties):
                                return dsjOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLEquivalentObjectProperties):
                                return eqvOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLSubObjectPropertyOf):
                                return subOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLInverseObjectProperties):
                                return invOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLObjectPropertyDomain):
                                return opDomainAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            case nameof(OWLObjectPropertyRange):
                                return opRangeAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                            //Not explicitly handled inference type: just keep it
                            default: return true;
                        }
                    });

                //Collect inferences
                IEnumerable<OWLInference> emptyInferenceSet = Enumerable.Empty<OWLInference>();
                inferences.AddRange(inferenceRegistry.SelectMany(ir => ir.Value ?? emptyInferenceSet).Distinct());
                #endregion

                OWLEvents.RaiseInfo($"Completed OWL2/SWRL reasoner on ontology {ontology.IRI} => {inferences.Count} inferences");
                #endregion

                #region Merge inferences
                if (shouldMergeInferences)
                {
                    OWLEvents.RaiseInfo($"Merging OWL2/SWRL inferences into ontology '{ontology.IRI}'...");

                    foreach (OWLInference inference in inferences)
                    {
                        switch (inference.Axiom)
                        {
                            case OWLAssertionAxiom asnAx:
                                ontology.DeclareAssertionAxiom(asnAx);
                                break;
                            case OWLClassAxiom clsAx:
                                ontology.DeclareClassAxiom(clsAx);
                                break;
                            case OWLDataPropertyAxiom dpAx:
                                ontology.DeclareDataPropertyAxiom(dpAx);
                                break;
                            case OWLObjectPropertyAxiom opAx:
                                ontology.DeclareObjectPropertyAxiom(opAx);
                                break;
                            case OWLAnnotationAxiom annAx:
                                ontology.DeclareAnnotationAxiom(annAx);
                                break;
                        }
                    }

                    OWLEvents.RaiseInfo($"Completed merging of OWL2/SWRL inferences into ontology {ontology.IRI}");
                }
                #endregion
            }

            return inferences;
        }
        #endregion
    }
}