/*
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

using System;
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
        /// Dictionary of rules applied by the validator, categorized by reserved keys
        /// </summary>
        internal Dictionary<string, object> Rules { get; set; }

        /// <summary>
        /// Dictionary of extensions activated on the validator, categorized by reserved keys
        /// </summary>
        internal Dictionary<string, Action<OWLValidator, OWLOntology, Dictionary<string, OWLValidatorReport>>> Extensions { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty validator
        /// </summary>
        public OWLValidator()
        {
            Rules = new Dictionary<string, object>()
            {
                { "STD", new List<OWLEnums.OWLValidatorRules>() },
                { "CTM", new List<OWLValidatorRule>() }
            };
            Extensions = new Dictionary<string, Action<OWLValidator, OWLOntology, Dictionary<string, OWLValidatorReport>>>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given rule to the validator
        /// </summary>
        public OWLValidator AddRule(OWLEnums.OWLValidatorRules validatorRule)
        {
            if (!((List<OWLEnums.OWLValidatorRules>)Rules["STD"]).Contains(validatorRule))
                ((List<OWLEnums.OWLValidatorRules>)Rules["STD"]).Add(validatorRule);
            return this;
        }

        /// <summary>
        /// Adds the given rule to the validator
        /// </summary>
        public OWLValidator AddRule(OWLValidatorRule validatorRule)
        {
            if (validatorRule != null)
                ((List<OWLValidatorRule>)Rules["CTM"]).Add(validatorRule);
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

                //Initialize evidence registry
                Dictionary<string, OWLValidatorReport> evidenceRegistry = new Dictionary<string, OWLValidatorReport>();
                foreach (OWLEnums.OWLValidatorRules stdRule in (List<OWLEnums.OWLValidatorRules>)Rules["STD"])
                    evidenceRegistry.Add(stdRule.ToString(), null);
                foreach (OWLValidatorRule ctmRule in (List<OWLValidatorRule>)Rules["CTM"])
                    evidenceRegistry.Add(ctmRule.RuleName, null);

                //Execute rules
                Parallel.ForEach((List<OWLEnums.OWLValidatorRules>)Rules["STD"],
                    stdRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching standard validator rule '{stdRule}'");

                        switch (stdRule)
                        {
                            case OWLEnums.OWLValidatorRules.TermDisjointness:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.TermDisjointness.ToString()] = OWLTermDisjointnessRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.TermDeclaration:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.TermDeclaration.ToString()] = OWLTermDeclarationRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.TermDeprecation:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.TermDeprecation.ToString()] = OWLTermDeprecationRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.DomainRange:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.DomainRange.ToString()] = OWLDomainRangeRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.InverseOf:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.InverseOf.ToString()] = OWLInverseOfRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.SymmetricProperty:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.SymmetricProperty.ToString()] = OWLSymmetricPropertyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.AsymmetricProperty:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.AsymmetricProperty.ToString()] = OWLAsymmetricPropertyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.IrreflexiveProperty:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.IrreflexiveProperty.ToString()] = OWLIrreflexivePropertyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.PropertyDisjoint:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.PropertyDisjoint.ToString()] = OWLPropertyDisjointRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.ClassKey:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.ClassKey.ToString()] = OWLClassKeyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.PropertyChainAxiom:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.PropertyChainAxiom.ToString()] = OWLPropertyChainAxiomRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.ClassType:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.ClassType.ToString()] = OWLClassTypeRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.NegativeAssertions:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.NegativeAssertions.ToString()] = OWLNegativeAssertionsRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.GlobalCardinality:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.GlobalCardinality.ToString()] = OWLGlobalCardinalityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.LocalCardinality:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.LocalCardinality.ToString()] = OWLLocalCardinalityRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.PropertyConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.PropertyConsistency.ToString()] = OWLPropertyConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.DisjointUnion:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.DisjointUnion.ToString()] = OWLDisjointUnionRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.ThingNothing:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.ThingNothing.ToString()] = OWLThingNothingRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.TopBottom:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.TopBottom.ToString()] = OWLTopBottomRule.ExecuteRule(ontology);
                                break;
                            //Rules detecting violations in consequence of Import/Merge actions
                            case OWLEnums.OWLValidatorRules.SubClassConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.SubClassConsistency.ToString()] = OWLSubClassConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.EquivalentClassConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.EquivalentClassConsistency.ToString()] = OWLEquivalentClassConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.DisjointClassConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.DisjointClassConsistency.ToString()] = OWLDisjointClassConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.SubPropertyConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.SubPropertyConsistency.ToString()] = OWLSubPropertyConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.EquivalentPropertyConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.EquivalentPropertyConsistency.ToString()] = OWLEquivalentPropertyConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.DisjointPropertyConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.DisjointPropertyConsistency.ToString()] = OWLDisjointPropertyConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.SameIndividualConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.SameIndividualConsistency.ToString()] = OWLSameIndividualConsistencyRule.ExecuteRule(ontology);
                                break;
                            case OWLEnums.OWLValidatorRules.DifferentIndividualConsistency:
                                evidenceRegistry[OWLEnums.OWLValidatorRules.DifferentIndividualConsistency.ToString()] = OWLDifferentIndividualConsistencyRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed standard validator rule '{stdRule}': found {evidenceRegistry[stdRule.ToString()].EvidencesCount} evidences");
                    });
                Parallel.ForEach((List<OWLValidatorRule>)Rules["CTM"],
                    ctmRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching custom validator rule '{ctmRule.RuleName}'");

                        evidenceRegistry[ctmRule.RuleName] = ctmRule.ExecuteRule(ontology);

                        OWLEvents.RaiseInfo($"Completed custom validator rule '{ctmRule.RuleName}': found {evidenceRegistry[ctmRule.RuleName].EvidencesCount} evidences");
                    });

                //Execute extensions
                foreach (var extension in Extensions)
                    extension.Value?.Invoke(this, ontology, evidenceRegistry);

                //Process validator registry
                foreach (OWLValidatorReport validatorRegistryReport in evidenceRegistry.Values)
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

        /// <summary>
        /// Activates the given extension into the validator
        /// </summary>
        internal void ActivateExtension<T>(string extKey, Action<OWLValidator, OWLOntology, Dictionary<string, OWLValidatorReport>> extRuleExecutor)
        {
            if (!Extensions.ContainsKey(extKey))
                Extensions.Add(extKey, extRuleExecutor);
            if (!Rules.ContainsKey(extKey))
                Rules.Add(extKey, new List<T>());
        }
        #endregion
    }
}