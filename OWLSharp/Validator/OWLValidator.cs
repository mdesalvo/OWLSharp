/*
   Copyright 2012-2023 Marco De Salvo
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
using System.Threading.Tasks;

namespace OWLSharp
{
    /// <summary>
    /// OWLValidator analyzes an ontology in order to discover errors and inconsistencies affecting its model and data
    /// </summary>
    public class OWLValidator
    {
        #region Properties
        /// <summary>
        /// List of standard rules applied by the validator
        /// </summary>
        internal List<OWLEnums.OWLValidatorStandardRules> StandardRules { get; set; }

        /// <summary>
        /// List of custom rules applied by the validator
        /// </summary>
        internal List<OWLValidatorRule> CustomRules { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty validator
        /// </summary>
        public OWLValidator()
        {
            StandardRules = new List<OWLEnums.OWLValidatorStandardRules>();
            CustomRules = new List<OWLValidatorRule>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given standard rule to the validator
        /// </summary>
        public OWLValidator AddStandardRule(OWLEnums.OWLValidatorStandardRules standardRule)
        {
            if (!StandardRules.Contains(standardRule))
                StandardRules.Add(standardRule);
            return this;
        }

        /// <summary>
        /// Adds the given custom rule to the validator
        /// </summary>
        public OWLValidator AddCustomRule(OWLValidatorRule customRule)
        {
            if (customRule == null)
                throw new OWLException("Cannot add custom rule to validator because given \"customRule\" parameter is null");

            CustomRules.Add(customRule);
            return this;
        }

        /// <summary>
        /// Applies the validator on the given ontology
        /// </summary>
        public OWLValidatorReport ApplyToOntology(OWLOntology ontology)
        {
            OWLValidatorReport validatorReport = new OWLValidatorReport();

            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Validator is going to be applied on Ontology '{ontology.URI}': this may require intensive processing, depending on size and complexity of domain knowledge and rules");

                //Initialize validator registry
                Dictionary<string, OWLValidatorReport> validatorRegistry = new Dictionary<string, OWLValidatorReport>();
                foreach (OWLEnums.OWLValidatorStandardRules standardRule in StandardRules)
                    validatorRegistry.Add(standardRule.ToString(), null);
                foreach (OWLValidatorRule customRule in CustomRules)
                    validatorRegistry.Add(customRule.RuleName, null);

                //Execute standard rules
                Parallel.ForEach(StandardRules, 
                    standardRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching standard validator rule '{standardRule}'");

                        switch (standardRule)
                        {
                            case OWLEnums.OWLValidatorStandardRules.TermDisjointness:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.TermDisjointness.ToString()] = OWLTermDisjointnessRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.TermDeclaration:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.TermDeclaration.ToString()] = OWLTermDeclarationRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.TermDeprecation:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.TermDeprecation.ToString()] = OWLTermDeprecationRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.DomainRange:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.DomainRange.ToString()] = OWLDomainRangeRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.InverseOf:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.InverseOf.ToString()] = OWLInverseOfRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.SymmetricProperty:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.SymmetricProperty.ToString()] = OWLSymmetricPropertyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.AsymmetricProperty:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.AsymmetricProperty.ToString()] = OWLAsymmetricPropertyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.IrreflexiveProperty:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.IrreflexiveProperty.ToString()] = OWLIrreflexivePropertyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.PropertyDisjoint:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.PropertyDisjoint.ToString()] = OWLPropertyDisjointRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.ClassKey:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.ClassKey.ToString()] = OWLClassKeyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.PropertyChainAxiom:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.PropertyChainAxiom.ToString()] = OWLPropertyChainAxiomRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.ClassType:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.ClassType.ToString()] = OWLClassTypeRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.NegativeAssertions:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.NegativeAssertions.ToString()] = OWLNegativeAssertionsRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.GlobalCardinality:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.GlobalCardinality.ToString()] = OWLGlobalCardinalityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.LocalCardinality:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.LocalCardinality.ToString()] = OWLLocalCardinalityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.PropertyConsistency:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.PropertyConsistency.ToString()] = OWLPropertyConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorStandardRules.DisjointUnion:
                                validatorRegistry[OWLEnums.OWLValidatorStandardRules.DisjointUnion.ToString()] = OWLDisjointUnionRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed standard validator rule '{standardRule}': found {validatorRegistry[standardRule.ToString()].EvidencesCount} evidences");
                    });

                //Execute custom rules
                Parallel.ForEach(CustomRules, 
                    customRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching custom validator rule '{customRule.RuleName}'");

                        validatorRegistry[customRule.RuleName] = customRule.ExecuteRule(ontology);

                        OWLEvents.RaiseInfo($"Completed custom validator rule '{customRule.RuleName}': found {validatorRegistry[customRule.RuleName].EvidencesCount} evidences");
                    });

                //Process validator registry
                foreach (OWLValidatorReport validatorRegistryReport in validatorRegistry.Values)
                    validatorReport.MergeEvidences(validatorRegistryReport);

                OWLEvents.RaiseInfo($"Validator has been applied on Ontology '{ontology.URI}': found {validatorReport.EvidencesCount} evidences");
            }

            return validatorReport;
        }

        /// <summary>
        /// Asynchronously applies the validator on the given ontology
        /// </summary>
        public Task<OWLValidatorReport> ApplyToOntologyAsync(OWLOntology ontology)
            => Task.Run(() => ApplyToOntology(ontology));
        #endregion
    }
}