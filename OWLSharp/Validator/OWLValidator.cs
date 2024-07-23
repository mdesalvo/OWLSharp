﻿/*
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
using OWLSharp.Validator.Rules;

namespace OWLSharp.Validator
{
    public class OWLValidator
    {
        #region Properties
        public List<OWLEnums.OWLValidatorRules> Rules { get; internal set; }
        #endregion

        #region Ctors
        public OWLValidator()
			=> Rules = new List<OWLEnums.OWLValidatorRules>();
        #endregion

        #region Methods
        public async Task<List<OWLIssue>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching validator on ontology '{ontology.IRI}'...");
				Rules = Rules.Distinct().ToList();

                //Initialize issue registry
                Dictionary<string, List<OWLIssue>> issueRegistry = new Dictionary<string, List<OWLIssue>>();
				Rules.ForEach(rule => issueRegistry.Add(rule.ToString(), null));

                //Execute validator rules
                await Task.Run(() => 
					Parallel.ForEach(Rules, rule =>
					{
						OWLEvents.RaiseInfo($"Launching rule {rule}...");

						switch (rule)
						{
							case OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis:
								issueRegistry[OWLAsymmetricObjectPropertyAnalysisRule.rulename] = OWLAsymmetricObjectPropertyAnalysisRule.ExecuteRule(ontology);
								break;
							case OWLEnums.OWLValidatorRules.DifferentIndividualsAnalysis:
								issueRegistry[OWLDifferentIndividualsAnalysisRule.rulename] = OWLDifferentIndividualsAnalysisRule.ExecuteRule(ontology);
								break;
							case OWLEnums.OWLValidatorRules.IrreflexiveObjectPropertyAnalysis:
								issueRegistry[OWLIrreflexiveObjectPropertyAnalysisRule.rulename] = OWLIrreflexiveObjectPropertyAnalysisRule.ExecuteRule(ontology);
								break;
							case OWLEnums.OWLValidatorRules.TermsDeprecationAnalysis:
								issueRegistry[OWLTermsDeprecationAnalysisRule.rulename] = OWLTermsDeprecationAnalysisRule.ExecuteRule(ontology);
								break;
							case OWLEnums.OWLValidatorRules.TermsDisjointnessAnalysis:
								issueRegistry[OWLTermsDisjointnessAnalysisRule.rulename] = OWLTermsDisjointnessAnalysisRule.ExecuteRule(ontology);
								break;
                            case OWLEnums.OWLValidatorRules.ThingNothingAnalysis:
                                issueRegistry[OWLThingNothingAnalysisRule.rulename] = OWLThingNothingAnalysisRule.ExecuteRule(ontology);
                                break;
							case OWLEnums.OWLValidatorRules.TopBottomAnalysis:
                                issueRegistry[OWLTopBottomAnalysisRule.rulename] = OWLTopBottomAnalysisRule.ExecuteRule(ontology);
                                break;
                        }

						OWLEvents.RaiseInfo($"Completed rule {rule} => {issueRegistry[rule.ToString()].Count} issues");
					}));

                //Process issues registry
                await Task.Run(() => 
				{
					issues.AddRange(issueRegistry.SelectMany(ir => ir.Value));
					issueRegistry.Clear();
				});           

                OWLEvents.RaiseInfo($"Completed validator on ontology {ontology.IRI} => {issues.Count} issues");
            }

            return issues;
        }
        #endregion
    }
}