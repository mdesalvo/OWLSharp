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

using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using OWLSharp.Validator;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS
{
    internal class SKOSRelatedConceptAnalysisRule
    {
        internal static readonly string rulename = SKOSEnums.SKOSValidatorRules.RelatedConceptAnalysis.ToString();
        internal static readonly string rulesugg1A = "There should not be SKOS concepts having a clash in associative VS hierarchical relations (skos:related VS skos:broader)";
        internal static readonly string rulesugg1B = "There should not be SKOS concepts having a clash in associative VS hierarchical relations (skos:relatedMatch VS skos:broader)";
        internal static readonly string rulesugg2A = "There should not be SKOS concepts having a clash in associative VS hierarchical relations (skos:related VS skos:narrower)";
        internal static readonly string rulesugg2B = "There should not be SKOS concepts having a clash in associative VS hierarchical relations (skos:relatedMatch VS skos:narrower)";
        internal static readonly string rulesugg3A = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:related VS skos:broadMatch)";
        internal static readonly string rulesugg3B = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:relatedMatch VS skos:broadMatch)";
        internal static readonly string rulesugg4A = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:related VS skos:narrowMatch)";
        internal static readonly string rulesugg4B = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:relatedMatch VS skos:narrowMatch)";
        internal static readonly string rulesugg5A = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:related VS skos:closeMatch)";
        internal static readonly string rulesugg5B = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:relatedMatch VS skos:closeMatch)";
        internal static readonly string rulesugg6A = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:related VS skos:exactMatch)";
        internal static readonly string rulesugg6B = "There should not be SKOS concepts having a clash in associative VS mapping relations (skos:relatedMatch VS skos:exactMatch)";

        internal static async Task<List<OWLIssue>> ExecuteRuleAsync(OWLOntology ontology, Dictionary<string, List<OWLIndividualExpression>> cacheRegistry)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            //skos:related VS skos:broader
            SWRLRule clash1ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS hierarchical relations (skos:related VS skos:broader)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash1ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg1A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS hierarchical relations (skos:related VS skos:broader)"
                )));
            violations.Clear();

            //skos:relatedMatch VS skos:broader
            SWRLRule clash1BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS hierarchical relations (skos:relatedMatch VS skos:broader)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash1BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg1B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS hierarchical relations (skos:relatedMatch VS skos:broader)"
                )));
            violations.Clear();

            //skos:related VS skos:narrower
            SWRLRule clash2ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS hierarchical relations (skos:related VS skos:narrower)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash2ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg2A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS hierarchical relations (skos:related VS skos:narrower)"
                )));
            violations.Clear();

            //skos:relatedMatch VS skos:narrower
            SWRLRule clash2BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:relatedMatch VS skos:narrower)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash2BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg2B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:narrower)"
                )));
            violations.Clear();

            //skos:related VS skos:broadMatch
            SWRLRule clash3ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:related VS skos:broadMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash3ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg3A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:broadMatch)"
                )));
            violations.Clear();

            //skos:relatedMatch VS skos:broadMatch
            SWRLRule clash3BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:relatedMatch VS skos:broadMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash3BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg3B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:broadMatch)"
                )));
            violations.Clear();

            //skos:related VS skos:narrowMatch
            SWRLRule clash4ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:related VS skos:narrowMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash4ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg4A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:narrowMatch)"
                )));
            violations.Clear();

            //skos:relatedMatch VS skos:narrowMatch
            SWRLRule clash4BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:relatedMatch VS skos:narrowMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash4BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg4B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:narrowMatch)"
                )));
            violations.Clear();

            //skos:related VS skos:closeMatch
            SWRLRule clash5ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:related VS skos:closeMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash5ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg5A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:closeMatch)"
                )));
            violations.Clear();

            //skos:relatedMatch VS skos:closeMatch
            SWRLRule clash5BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broaderTransitive VS skos:exactMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash5BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg5B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:closeMatch)"
                )));
            violations.Clear();

            //skos:related VS skos:exactMatch
            SWRLRule clash6ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:related VS skos:exactMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash6ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg6A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:exactMatch)"
                )));
            violations.Clear();

            //skos:relatedMatch VS skos:exactMatch
            SWRLRule clash6BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSRelatedConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their associative VS mapping relations (skos:relatedMatch VS skos:exactMatch)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))) { IndividualsCache = cacheRegistry["CONCEPTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await clash6BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg6B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:exactMatch)"
                )));
            violations.Clear();

            return issues;
        }
    }
}