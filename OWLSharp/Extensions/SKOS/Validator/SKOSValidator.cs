﻿/*
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

namespace OWLSharp.Extensions.SKOS
{
    public sealed class SKOSValidator
    {
        #region Properties
        internal static readonly RDFResource ViolationIRI = new RDFResource("urn:owlsharp:swrl:hasViolations");

        public List<SKOSEnums.SKOSValidatorRules> Rules { get; internal set; } = new List<SKOSEnums.SKOSValidatorRules>();
        #endregion

        #region Methods
        public SKOSValidator AddRule(SKOSEnums.SKOSValidatorRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        public async Task<List<OWLIssue>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching SKOS validator on ontology '{ontology.IRI}'...");
                Rules = Rules.Distinct().ToList();

                //Initialize issue registry
                Dictionary<string, List<OWLIssue>> issueRegistry = new Dictionary<string, List<OWLIssue>>();
                Rules.ForEach(rule => issueRegistry.Add(rule.ToString(), null));

                //Initialize cache registry
                Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
                {
                    { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
                };

                //Execute validator rules
                await Rules.ParallelForEachAsync(async rule =>
                {
                    OWLEvents.RaiseInfo($"Launching SKOS rule {rule}...");

                    switch (rule)
                    {
                        case SKOSEnums.SKOSValidatorRules.AlternativeLabelAnalysis:
                            issueRegistry[SKOSAlternativeLabelAnalysisRule.rulename] = await SKOSAlternativeLabelAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.HiddenLabelAnalysis:
                            issueRegistry[SKOSHiddenLabelAnalysisRule.rulename] = await SKOSHiddenLabelAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.PreferredLabelAnalysis:
                            issueRegistry[SKOSPreferredLabelAnalysisRule.rulename] = await SKOSPreferredLabelAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.NotationAnalysis:
                            issueRegistry[SKOSNotationAnalysisRule.rulename] = await SKOSNotationAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.BroaderConceptAnalysis:
                            issueRegistry[SKOSBroaderConceptAnalysisRule.rulename] = await SKOSBroaderConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.NarrowerConceptAnalysis:
                            issueRegistry[SKOSNarrowerConceptAnalysisRule.rulename] = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.CloseOrExactMatchConceptAnalysis:
                            issueRegistry[SKOSCloseOrExactMatchConceptAnalysisRule.rulename] = await SKOSCloseOrExactMatchConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.RelatedConceptAnalysis:
                            issueRegistry[SKOSRelatedConceptAnalysisRule.rulename] = await SKOSRelatedConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
                            break;
                        case SKOSEnums.SKOSValidatorRules.LiteralFormAnalysis:
                            issueRegistry[SKOSXLLiteralFormAnalysisRule.rulename] = await SKOSXLLiteralFormAnalysisRule.ExecuteRuleAsync(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed SKOS rule {rule} => {issueRegistry[rule.ToString()].Count} issues");
                });

                //Process issues registry
                await Task.Run(() =>
                {
                    issues.AddRange(issueRegistry.SelectMany(ir => ir.Value ?? Enumerable.Empty<OWLIssue>()));
                    issueRegistry.Clear();
                });

                OWLEvents.RaiseInfo($"Completed SKOS validator on ontology {ontology.IRI} => {issues.Count} issues");
            }

            return issues;
        }
        #endregion
    }
}