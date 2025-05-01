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
using OWLSharp.Validator;
using RDFSharp.Model;

namespace OWLSharp.Extensions.TIME
{
    public sealed class TIMEValidator
    {
        #region Properties
        internal static readonly RDFResource ViolationIRI = new RDFResource("urn:owlsharp:swrl:hasViolations");

        public List<TIMEEnums.TIMEValidatorRules> Rules { get; internal set; } = new List<TIMEEnums.TIMEValidatorRules>();
        #endregion

        #region Methods
        public TIMEValidator AddRule(TIMEEnums.TIMEValidatorRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        public async Task<List<OWLIssue>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching OWL-TIME validator on ontology '{ontology.IRI}'...");
                Rules = Rules.Distinct().ToList();

                //Initialize issue registry
                Dictionary<string, List<OWLIssue>> issueRegistry = new Dictionary<string, List<OWLIssue>>();
                Rules.ForEach(rule => issueRegistry.Add(rule.ToString(), null));

                //Initialize cache registry
                Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
                {
                    { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
                    { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
                };

                //Execute validator rules
                await Rules.ParallelForEachAsync(async rule =>
                {
                    OWLEvents.RaiseInfo($"Launching OWL-TIME rule {rule}...");

                    switch (rule)
                    {
                        case TIMEEnums.TIMEValidatorRules.InstantAfterAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.InstantAfterAnalysis)] = await TIMEInstantAfterAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.InstantBeforeAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.InstantBeforeAnalysis)] = await TIMEInstantBeforeAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalAfterAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalAfterAnalysis)] = await TIMEIntervalAfterAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalBeforeAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalBeforeAnalysis)] = await TIMEIntervalBeforeAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalContainsAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalContainsAnalysis)] = await TIMEIntervalContainsAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalDisjointAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalDisjointAnalysis)] = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalDuringAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalDuringAnalysis)] = await TIMEIntervalDuringAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalEqualsAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalEqualsAnalysis)] = await TIMEIntervalEqualsAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalFinishesAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalFinishesAnalysis)] = await TIMEIntervalFinishesAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalFinishedByAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalFinishedByAnalysis)] = await TIMEIntervalFinishedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalHasInsideAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalHasInsideAnalysis)] = await TIMEIntervalHasInsideAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalInAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalInAnalysis)] = await TIMEIntervalInAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalMeetsAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalMeetsAnalysis)] = await TIMEIntervalMeetsAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalMetByAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalMetByAnalysis)] = await TIMEIntervalMetByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalNotDisjointAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalNotDisjointAnalysis)] = await TIMEIntervalNotDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalOverlapsAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalOverlapsAnalysis)] = await TIMEIntervalOverlapsAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalOverlappedByAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalOverlappedByAnalysis)] = await TIMEIntervalOverlappedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalStartsAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalStartsAnalysis)] = await TIMEIntervalStartsAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case TIMEEnums.TIMEValidatorRules.IntervalStartedByAnalysis:
                            issueRegistry[nameof(TIMEEnums.TIMEValidatorRules.IntervalStartedByAnalysis)] = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL-TIME rule {rule} => {issueRegistry[rule.ToString()].Count} issues");
                });

                //Process issues registry
                await Task.Run(() =>
                {
                    issues.AddRange(issueRegistry.SelectMany(ir => ir.Value ?? Enumerable.Empty<OWLIssue>()));
                    issueRegistry.Clear();
                });

                OWLEvents.RaiseInfo($"Completed OWL-TIME validator on ontology {ontology.IRI} => {issues.Count} issues");
            }

            return issues;
        }
        #endregion
    }
}