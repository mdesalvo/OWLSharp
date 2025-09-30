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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OWLSharp.Ontology;

namespace OWLSharp.Validator
{
    public sealed class OWLValidator
    {
        #region Properties
        public List<OWLEnums.OWLValidatorRules> Rules { get; internal set; } = new List<OWLEnums.OWLValidatorRules>();
        #endregion

        #region Methods
        public OWLValidator AddRule(OWLEnums.OWLValidatorRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        public async Task<List<OWLIssue>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching OWL2 validator on ontology '{ontology.IRI}'...");
                Rules = Rules.Distinct().ToList();

                //Initialize issue registry
                Dictionary<string, List<OWLIssue>> issueRegistry = new Dictionary<string, List<OWLIssue>>(Rules.Count);
                Rules.ForEach(rule => issueRegistry.Add(rule.ToString(), null));

                //Initialize validator context
                OWLValidatorContext validatorContext = new OWLValidatorContext
                {
                    ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
                    DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
                    ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
                };

                //Execute validator rules
                Parallel.ForEach(Rules, rule =>
                {
                    string ruleString = rule.ToString();
                    OWLEvents.RaiseInfo($"Launching OWL2 rule {ruleString}...");

                    switch (rule)
                    {
                        case OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis:
                            issueRegistry[OWLAsymmetricObjectPropertyAnalysisRule.rulename] = OWLAsymmetricObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.ClassAssertionAnalysis:
                            issueRegistry[OWLClassAssertionAnalysisRule.rulename] = OWLClassAssertionAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.DataPropertyDomainAnalysis:
                            issueRegistry[OWLDataPropertyDomainAnalysisRule.rulename] = OWLDataPropertyDomainAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.DataPropertyRangeAnalysis:
                            issueRegistry[OWLDataPropertyRangeAnalysisRule.rulename] = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.DifferentIndividualsAnalysis:
                            issueRegistry[OWLDifferentIndividualsAnalysisRule.rulename] = OWLDifferentIndividualsAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointClassesAnalysis:
                            issueRegistry[OWLDisjointClassesAnalysisRule.rulename] = OWLDisjointClassesAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointDataPropertiesAnalysis:
                            issueRegistry[OWLDisjointDataPropertiesAnalysisRule.rulename] = OWLDisjointDataPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointObjectPropertiesAnalysis:
                            issueRegistry[OWLDisjointObjectPropertiesAnalysisRule.rulename] = OWLDisjointObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointUnionAnalysis:
                            issueRegistry[OWLDisjointUnionAnalysisRule.rulename] = OWLDisjointUnionAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.EquivalentClassesAnalysis:
                            issueRegistry[OWLEquivalentClassesAnalysisRule.rulename] = OWLEquivalentClassesAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.EquivalentDataPropertiesAnalysis:
                            issueRegistry[OWLEquivalentDataPropertiesAnalysisRule.rulename] = OWLEquivalentDataPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.EquivalentObjectPropertiesAnalysis:
                            issueRegistry[OWLEquivalentObjectPropertiesAnalysisRule.rulename] = OWLEquivalentObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.FunctionalDataPropertyAnalysis:
                            issueRegistry[OWLFunctionalDataPropertyAnalysisRule.rulename] = OWLFunctionalDataPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.FunctionalObjectPropertyAnalysis:
                            issueRegistry[OWLFunctionalObjectPropertyAnalysisRule.rulename] = OWLFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.HasKeyAnalysis:
                            issueRegistry[OWLHasKeyAnalysisRule.rulename] = OWLHasKeyAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.InverseFunctionalObjectPropertyAnalysis:
                            issueRegistry[OWLInverseFunctionalObjectPropertyAnalysisRule.rulename] = OWLInverseFunctionalObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.IrreflexiveObjectPropertyAnalysis:
                            issueRegistry[OWLIrreflexiveObjectPropertyAnalysisRule.rulename] = OWLIrreflexiveObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.NegativeDataAssertionsAnalysis:
                            issueRegistry[OWLNegativeDataAssertionsAnalysisRule.rulename] = OWLNegativeDataAssertionsAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.NegativeObjectAssertionsAnalysis:
                            issueRegistry[OWLNegativeObjectAssertionsAnalysisRule.rulename] = OWLNegativeObjectAssertionsAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.ObjectPropertyChainAnalysis:
                            issueRegistry[OWLObjectPropertyChainAnalysisRule.rulename] = OWLObjectPropertyChainAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.ObjectPropertyDomainAnalysis:
                            issueRegistry[OWLObjectPropertyDomainAnalysisRule.rulename] = OWLObjectPropertyDomainAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.ObjectPropertyRangeAnalysis:
                            issueRegistry[OWLObjectPropertyRangeAnalysisRule.rulename] = OWLObjectPropertyRangeAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.SubClassOfAnalysis:
                            issueRegistry[OWLSubClassOfAnalysisRule.rulename] = OWLSubClassOfAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.SubDataPropertyOfAnalysis:
                            issueRegistry[OWLSubDataPropertyOfAnalysisRule.rulename] = OWLSubDataPropertyOfAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.SubObjectPropertyOfAnalysis:
                            issueRegistry[OWLSubObjectPropertyOfAnalysisRule.rulename] = OWLSubObjectPropertyOfAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.TermsDeprecationAnalysis:
                            issueRegistry[OWLTermsDeprecationAnalysisRule.rulename] = OWLTermsDeprecationAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.TermsDisjointnessAnalysis:
                            issueRegistry[OWLTermsDisjointnessAnalysisRule.rulename] = OWLTermsDisjointnessAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.ThingNothingAnalysis:
                            issueRegistry[OWLThingNothingAnalysisRule.rulename] = OWLThingNothingAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                        case OWLEnums.OWLValidatorRules.TopBottomAnalysis:
                            issueRegistry[OWLTopBottomAnalysisRule.rulename] = OWLTopBottomAnalysisRule.ExecuteRule(ontology, validatorContext);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL2 rule {ruleString} => {issueRegistry[ruleString].Count} issues");
                });

                //Process issues
                issues.AddRange(issueRegistry.SelectMany(ir => ir.Value ?? Enumerable.Empty<OWLIssue>()));
                issueRegistry.Clear();

                OWLEvents.RaiseInfo($"Completed OWL2 validator on ontology {ontology.IRI} => {issues.Count} issues");
            }

            return issues;
        }
        #endregion
    }

    internal sealed class OWLValidatorContext
    {
        internal List<OWLClassAssertion> ClassAssertions { get; set; }
        internal List<OWLDataPropertyAssertion> DataPropertyAssertions { get; set; }
        internal List<OWLObjectPropertyAssertion> ObjectPropertyAssertions { get; set; }
    }
}