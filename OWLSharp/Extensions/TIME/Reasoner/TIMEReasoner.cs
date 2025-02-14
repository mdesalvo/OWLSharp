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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dasync.Collections;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;

namespace OWLSharp.Extensions.TIME
{
    public class TIMEReasoner
    {
        #region Properties
        public List<TIMEEnums.TIMEReasonerRules> Rules { get; internal set; } = new List<TIMEEnums.TIMEReasonerRules>();
        #endregion

        #region Methods
        public TIMEReasoner AddRule(TIMEEnums.TIMEReasonerRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        public async Task<List<OWLInference>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching OWL-TIME reasoner on ontology '{ontology.IRI}'...");
                Rules = Rules.Distinct().ToList();

                //Initialize inference registry
                Dictionary<string, List<OWLInference>> inferenceRegistry = new Dictionary<string, List<OWLInference>>();
                Rules.ForEach(timeRule => inferenceRegistry.Add(timeRule.ToString(), null));

                //Initialize cache registry
                Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
                {
                    { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
                    { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
                };

                //Execute OWL-TIME reasoner rules
                await Rules.ParallelForEachAsync(async (rule) =>
                {
                    OWLEvents.RaiseInfo($"Launching OWL-TIME rule {rule}...");

                    switch (rule)
                    {
                        case TIMEEnums.TIMEReasonerRules.AfterEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterEqualsEntailment.ToString()] = await TIMEAfterEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.AfterFinishesEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterFinishesEntailment.ToString()] = await TIMEAfterFinishesEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.AfterMetByEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterMetByEntailment.ToString()] = await TIMEAfterMetByEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.AfterTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterTransitiveEntailment.ToString()] = await TIMEAfterTransitiveEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeEqualsEntailment.ToString()] = await TIMEBeforeEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeMeetsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeMeetsEntailment.ToString()] = await TIMEBeforeMeetsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeStartsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeStartsEntailment.ToString()] = await TIMEBeforeStartsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeTransitiveEntailment.ToString()] = await TIMEBeforeTransitiveEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.ContainsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.ContainsEqualsEntailment.ToString()] = await TIMEContainsEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.ContainsTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.ContainsTransitiveEntailment.ToString()] = await TIMEContainsTransitiveEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.DuringEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.DuringEqualsEntailment.ToString()] = await TIMEDuringEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.DuringTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.DuringTransitiveEntailment.ToString()] = await TIMEDuringTransitiveEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.EqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.EqualsEntailment.ToString()] = await TIMEEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.EqualsInverseEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.EqualsInverseEntailment.ToString()] = await TIMEEqualsInverseEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.EqualsTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.EqualsTransitiveEntailment.ToString()] = await TIMEEqualsTransitiveEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.FinishedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.FinishedByEqualsEntailment.ToString()] = await TIMEFinishedByEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.FinishesEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.FinishesEqualsEntailment.ToString()] = await TIMEFinishesEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.MeetsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.MeetsEqualsEntailment.ToString()] = await TIMEMeetsEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.MeetsStartsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.MeetsStartsEntailment.ToString()] = await TIMEMeetsStartsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.MetByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.MetByEqualsEntailment.ToString()] = await TIMEMetByEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.OverlappedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.OverlappedByEqualsEntailment.ToString()] = await TIMEOverlappedByEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.OverlapsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.OverlapsEqualsEntailment.ToString()] = await TIMEOverlapsEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.StartsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.StartsEqualsEntailment.ToString()] = await TIMEStartsEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEReasonerRules.StartedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.StartedByEqualsEntailment.ToString()] = await TIMEStartedByEqualsEntailmentRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL-TIME rule {rule} => {inferenceRegistry[rule.ToString()].Count} candidate inferences");
                });

                //Process inference registry
                await Task.Run(async () =>
                {
                    //Fetch axioms commonly targeted by OWL-TIME rules
                    Task<HashSet<string>> dtPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Select(asn => asn.GetXML())));
                    Task<HashSet<string>> opPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Select(asn => asn.GetXML())));
                    await Task.WhenAll(dtPropAsnAxiomsTask, opPropAsnAxiomsTask);

                    //Deduplicate inferences by analyzing explicit knowledge
                    foreach (KeyValuePair<string, List<OWLInference>> inferenceRegistryEntry in inferenceRegistry.Where(ir => ir.Value?.Count > 0))
                        inferenceRegistryEntry.Value.RemoveAll(inf =>
                        {
                            string infXML = inf.Axiom.GetXML();
                            return dtPropAsnAxiomsTask.Result.Contains(infXML)
                                   || opPropAsnAxiomsTask.Result.Contains(infXML);
                        });

                    //Collect inferences and perform final cleanup
                    inferences.AddRange(inferenceRegistry.SelectMany(ir => ir.Value ?? Enumerable.Empty<OWLInference>()).Distinct());
                    inferenceRegistry.Clear();
                });

                OWLEvents.RaiseInfo($"Completed OWL-TIME reasoner on ontology {ontology.IRI} => {inferences.Count} unique inferences");
            }

            return inferences;
        }
        #endregion
    }
}