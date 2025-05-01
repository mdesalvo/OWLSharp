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
    internal class SKOSBroaderConceptAnalysisRule
    {
        internal static readonly string rulename = nameof(SKOSEnums.SKOSValidatorRules.BroaderConceptAnalysis);
        internal const string rulesugg1A = "There should not be SKOS concepts having a clash in hierarchical relations (skos:broader VS skos:narrower)";
        internal const string rulesugg1B = "There should not be SKOS concepts having a clash in hierarchical relations (skos:broaderTransitive VS skos:narrowerTransitive)";
        internal const string rulesugg2A = "There should not be SKOS concepts having a clash in hierarchical VS associative relations (skos:broader VS skos:related)";
        internal const string rulesugg2B = "There should not be SKOS concepts having a clash in hierarchical VS associative relations (skos:broaderTransitive VS skos:related)";
        internal const string rulesugg3A = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broader VS skos:narrowMatch)";
        internal const string rulesugg3B = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broaderTransitive VS skos:narrowMatch)";
        internal const string rulesugg4A = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broader VS skos:closeMatch)";
        internal const string rulesugg4B = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broaderTransitive VS skos:closeMatch)";
        internal const string rulesugg5A = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broader VS skos:exactMatch)";
        internal const string rulesugg5B = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broaderTransitive VS skos:exactMatch)";
        internal const string rulesugg6A = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broader VS skos:relatedMatch)";
        internal const string rulesugg6B = "There should not be SKOS concepts having a clash in hierarchical VS mapping relations (skos:broaderTransitive VS skos:relatedMatch)";

        internal static async Task<List<OWLIssue>> ExecuteRuleAsync(OWLOntology ontology, Dictionary<string, List<OWLIndividualExpression>> cacheRegistry)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            //skos:broader VS skos:narrower
            SWRLRule clash1ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical relations (skos:broader VS skos:narrower)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
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
            violations.AddRange(await clash1ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg1A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical relations (skos:broader VS skos:narrower)"
                )));
            violations.Clear();

            //skos:broaderTransitive VS skos:narrowerTransitive
            SWRLRule clash1BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical relations (skos:broaderTransitive VS skos:narrowerTransitive)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
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
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical relations (skos:broaderTransitive VS skos:narrowerTransitive)"
                )));
            violations.Clear();

            //skos:broader VS skos:related
            SWRLRule clash2ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/associative relations (skos:broader VS skos:related)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
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
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/associative relations (skos:broader VS skos:related)"
                )));
            violations.Clear();

            //skos:broaderTransitive VS skos:related
            SWRLRule clash2BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/associative relations (skos:broaderTransitive VS skos:related)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
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
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/associative relations (skos:broaderTransitive VS skos:related)"
                )));
            violations.Clear();

            //skos:broader VS skos:narrowMatch
            SWRLRule clash3ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broader VS skos:narrowMatch)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
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
            violations.AddRange(await clash3ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg3A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broader VS skos:narrowMatch)"
                )));
            violations.Clear();

            //skos:broaderTransitive VS skos:narrowMatch
            SWRLRule clash3BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broaderTransitive VS skos:narrowMatch)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
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
            violations.AddRange(await clash3BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg3B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broaderTransitive VS skos:narrowMatch)"
                )));
            violations.Clear();

            //skos:broader VS skos:closeMatch
            SWRLRule clash4ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broader VS skos:closeMatch)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
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
            violations.AddRange(await clash4ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg4A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broader VS skos:closeMatch)"
                )));
            violations.Clear();

            //skos:broaderTransitive VS skos:closeMatch
            SWRLRule clash4BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broaderTransitive VS skos:closeMatch)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
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
            violations.AddRange(await clash4BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg4B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broaderTransitive VS skos:closeMatch)"
                )));
            violations.Clear();

            //skos:broader VS skos:exactMatch
            SWRLRule clash5ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broader VS skos:exactMatch)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
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
            violations.AddRange(await clash5ARule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg5A,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broader VS skos:exactMatch)"
                )));
            violations.Clear();

            //skos:broaderTransitive VS skos:exactMatch
            SWRLRule clash5BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
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
            violations.AddRange(await clash5BRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg5B,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broaderTransitive VS skos:exactMatch)"
                )));
            violations.Clear();

            //skos:broader VS skos:relatedMatch
            SWRLRule clash6ARule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broader VS skos:relatedMatch)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
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
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broader VS skos:relatedMatch)"
                )));
            violations.Clear();

            //skos:broaderTransitive VS skos:relatedMatch
            SWRLRule clash6BRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSBroaderConceptAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skos:Concept instances clashing on their hierarchical/mapping relations (skos:broaderTransitive VS skos:relatedMatch)"),
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
                            new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
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
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on hierarchical/mapping relations (skos:broaderTransitive VS skos:relatedMatch)"
                )));
            violations.Clear();

            return issues;
        }
    }
}