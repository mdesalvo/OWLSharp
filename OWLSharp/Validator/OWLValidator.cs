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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OWLSharp.Ontology;

namespace OWLSharp.Validator
{
    /// <summary>
    /// OWLValidator is an analysis engine that examines an ontology's T-BOX, A-BOX, and R-BOX to identify modeling errors,
    /// structural inconsistencies, and constraint violations without necessarily deriving new implicit knowledge.
    /// Unlike reasoners focused on inference, validators detect issues such as unsatisfiable classes, violated cardinality
    /// restrictions, improper axiom usage, syntactic errors, and logical contradictions, providing diagnostic feedback
    /// to improve ontology quality, correctness, and adherence to best practices or design patterns.
    /// </summary>
    public sealed class OWLValidator
    {
        #region Properties
        /// <summary>
        /// A predefined validator including all available OWL2 validator rules
        /// </summary>

        public static readonly OWLValidator Default = new OWLValidator {
            Rules = Enum.GetValues(typeof(OWLEnums.OWLValidatorRules)).Cast<OWLEnums.OWLValidatorRules>().ToList() };

        /// <summary>
        /// The set of rules to be applied by the validator
        /// </summary>
        public List<OWLEnums.OWLValidatorRules> Rules { get; internal set; } = new List<OWLEnums.OWLValidatorRules>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given rule to the validator
        /// </summary>
        /// <returns>The validator itself</returns>
        public OWLValidator AddRule(OWLEnums.OWLValidatorRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        /// <summary>
        /// Applies the validator on the given ontology
        /// </summary>
        /// <returns>The list of detected issues</returns>
        public Task<List<OWLIssue>> ApplyToOntologyAsync(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            Rules = Rules.Distinct().ToList();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching OWL2 validator on ontology '{ontology.IRI}'...");

                #region Init registry & context
                //Initialize issue registry
                Dictionary<string, List<OWLIssue>> issueRegistry = new Dictionary<string, List<OWLIssue>>(Rules.Count);
                Rules.ForEach(rule => issueRegistry.Add(rule.ToString(), null));
                #endregion

                #region Process rules
                //Execute validator rules
                Parallel.ForEach(Rules, rule =>
                {
                    string ruleString = rule.ToString();
                    OWLEvents.RaiseInfo($"Launching OWL2 rule {ruleString}...");

                    switch (rule)
                    {
                        case OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis:
                            issueRegistry[OWLAsymmetricObjectPropertyAnalysis.rulename] = OWLAsymmetricObjectPropertyAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.ClassAssertionAnalysis:
                            issueRegistry[OWLClassAssertionAnalysis.rulename] = OWLClassAssertionAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.DataPropertyDomainAnalysis:
                            issueRegistry[OWLDataPropertyDomainAnalysis.rulename] = OWLDataPropertyDomainAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.DataPropertyRangeAnalysis:
                            issueRegistry[OWLDataPropertyRangeAnalysis.rulename] = OWLDataPropertyRangeAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.DifferentIndividualsAnalysis:
                            issueRegistry[OWLDifferentIndividualsAnalysis.rulename] = OWLDifferentIndividualsAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointClassesAnalysis:
                            issueRegistry[OWLDisjointClassesAnalysis.rulename] = OWLDisjointClassesAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointDataPropertiesAnalysis:
                            issueRegistry[OWLDisjointDataPropertiesAnalysis.rulename] = OWLDisjointDataPropertiesAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointObjectPropertiesAnalysis:
                            issueRegistry[OWLDisjointObjectPropertiesAnalysis.rulename] = OWLDisjointObjectPropertiesAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.DisjointUnionAnalysis:
                            issueRegistry[OWLDisjointUnionAnalysis.rulename] = OWLDisjointUnionAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.EquivalentClassesAnalysis:
                            issueRegistry[OWLEquivalentClassesAnalysis.rulename] = OWLEquivalentClassesAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.EquivalentDataPropertiesAnalysis:
                            issueRegistry[OWLEquivalentDataPropertiesAnalysis.rulename] = OWLEquivalentDataPropertiesAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.EquivalentObjectPropertiesAnalysis:
                            issueRegistry[OWLEquivalentObjectPropertiesAnalysis.rulename] = OWLEquivalentObjectPropertiesAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.FunctionalDataPropertyAnalysis:
                            issueRegistry[OWLFunctionalDataPropertyAnalysis.rulename] = OWLFunctionalDataPropertyAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.FunctionalObjectPropertyAnalysis:
                            issueRegistry[OWLFunctionalObjectPropertyAnalysis.rulename] = OWLFunctionalObjectPropertyAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.HasKeyAnalysis:
                            issueRegistry[OWLHasKeyAnalysis.rulename] = OWLHasKeyAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.InverseFunctionalObjectPropertyAnalysis:
                            issueRegistry[OWLInverseFunctionalObjectPropertyAnalysis.rulename] = OWLInverseFunctionalObjectPropertyAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.IrreflexiveObjectPropertyAnalysis:
                            issueRegistry[OWLIrreflexiveObjectPropertyAnalysis.rulename] = OWLIrreflexiveObjectPropertyAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.NegativeDataAssertionsAnalysis:
                            issueRegistry[OWLNegativeDataAssertionsAnalysis.rulename] = OWLNegativeDataAssertionsAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.NegativeObjectAssertionsAnalysis:
                            issueRegistry[OWLNegativeObjectAssertionsAnalysis.rulename] = OWLNegativeObjectAssertionsAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.ObjectPropertyChainAnalysis:
                            issueRegistry[OWLObjectPropertyChainAnalysis.rulename] = OWLObjectPropertyChainAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.ObjectPropertyDomainAnalysis:
                            issueRegistry[OWLObjectPropertyDomainAnalysis.rulename] = OWLObjectPropertyDomainAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.ObjectPropertyRangeAnalysis:
                            issueRegistry[OWLObjectPropertyRangeAnalysis.rulename] = OWLObjectPropertyRangeAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.SubClassOfAnalysis:
                            issueRegistry[OWLSubClassOfAnalysis.rulename] = OWLSubClassOfAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.SubDataPropertyOfAnalysis:
                            issueRegistry[OWLSubDataPropertyOfAnalysis.rulename] = OWLSubDataPropertyOfAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.SubObjectPropertyOfAnalysis:
                            issueRegistry[OWLSubObjectPropertyOfAnalysis.rulename] = OWLSubObjectPropertyOfAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.TermsDeprecationAnalysis:
                            issueRegistry[OWLTermsDeprecationAnalysis.rulename] = OWLTermsDeprecationAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.TermsDisjointnessAnalysis:
                            issueRegistry[OWLTermsDisjointnessAnalysis.rulename] = OWLTermsDisjointnessAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.ThingNothingAnalysis:
                            issueRegistry[OWLThingNothingAnalysis.rulename] = OWLThingNothingAnalysis.ExecuteRule(ontology);
                            break;
                        case OWLEnums.OWLValidatorRules.TopBottomAnalysis:
                            issueRegistry[OWLTopBottomAnalysis.rulename] = OWLTopBottomAnalysis.ExecuteRule(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed OWL2 rule {ruleString} => {issueRegistry[ruleString].Count} issues");
                });
                #endregion

                #region Finalize issues
                //Process issues
                IEnumerable<OWLIssue> emptyIssueSet = Enumerable.Empty<OWLIssue>();
                issues.AddRange(issueRegistry.SelectMany(ir => ir.Value ?? emptyIssueSet));
                #endregion

                OWLEvents.RaiseInfo($"Completed OWL2 validator on ontology {ontology.IRI} => {issues.Count} issues");
            }

            return Task.FromResult(issues);
        }
        #endregion
    }
}