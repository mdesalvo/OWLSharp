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

namespace OWLSharp.Extensions.TIME
{
    public class TIMEReasoner
    {
        #region Properties
        public List<TIMEEnums.TIMEReasonerRules> Rules { get; internal set; }
        #endregion

        #region Ctors
        public TIMEReasoner()
            => Rules = new List<TIMEEnums.TIMEReasonerRules>();
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

                //Execute OWL-TIME reasoner rules
                await Rules.ParallelForEachAsync(async (rule) =>
                {
                    OWLEvents.RaiseInfo($"Launching OWL-TIME rule {rule}...");

                    switch (rule)
                    {
                        case TIMEEnums.TIMEReasonerRules.AfterEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterEqualsEntailment.ToString()] = await TIMEAfterEqualsEntailmentRule.ExecuteRuleAsync(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.AfterFinishesEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterFinishesEntailment.ToString()] = await TIMEAfterFinishesEntailmentRule.ExecuteRuleAsync(ontology);
                            break;
                        /*case TIMEEnums.TIMEReasonerRules.AfterMetByEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterMetByEntailment.ToString()] = TIMEAfterMetByEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.AfterTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.AfterTransitiveEntailment.ToString()] = TIMEAfterTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeEqualsEntailment.ToString()] = TIMEBeforeEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeMeetsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeMeetsEntailment.ToString()] = TIMEBeforeMeetsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeStartsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeStartsEntailment.ToString()] = TIMEBeforeStartsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.BeforeTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.BeforeTransitiveEntailment.ToString()] = TIMEBeforeTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.ContainsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.ContainsEqualsEntailment.ToString()] = TIMEContainsEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.ContainsTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.ContainsTransitiveEntailment.ToString()] = TIMEContainsTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.DuringEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.DuringEqualsEntailment.ToString()] = TIMEDuringEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.DuringTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.DuringTransitiveEntailment.ToString()] = TIMEDuringTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.EqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.EqualsEntailment.ToString()] = TIMEEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.EqualsInverseEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.EqualsInverseEntailment.ToString()] = TIMEEqualsInverseEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.EqualsTransitiveEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.EqualsTransitiveEntailment.ToString()] = TIMEEqualsTransitiveEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.FinishedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.FinishedByEqualsEntailment.ToString()] = TIMEFinishedByEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.FinishesEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.FinishesEqualsEntailment.ToString()] = TIMEFinishesEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.MeetsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.MeetsEqualsEntailment.ToString()] = TIMEMeetsEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.MeetsStartsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.MeetsStartsEntailment.ToString()] = TIMEMeetsStartsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.MetByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.MetByEqualsEntailment.ToString()] = TIMEMetByEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.OverlappedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.OverlappedByEqualsEntailment.ToString()] = TIMEOverlappedByEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.OverlapsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.OverlapsEqualsEntailment.ToString()] = TIMEOverlapsEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.StartsEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.StartsEqualsEntailment.ToString()] = TIMEStartsEqualsEntailmentRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEReasonerRules.StartedByEqualsEntailment:
                            inferenceRegistry[TIMEEnums.TIMEReasonerRules.StartedByEqualsEntailment.ToString()] = TIMEStartedByEqualsEntailmentRule.ExecuteRule(ontology);
                            break;*/
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