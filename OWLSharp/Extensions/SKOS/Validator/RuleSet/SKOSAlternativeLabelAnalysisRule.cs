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

namespace OWLSharp.Extensions.SKOS
{
    internal class SKOSAlternativeLabelAnalysisRule
    {
        internal static readonly string rulename = SKOSEnums.SKOSValidatorRules.AlternativeLabelAnalysis.ToString();
        internal static readonly string rulesugg1 = "There should not be SKOS concepts having the same value for skos:altLabel and skos:prefLabel data annotations.";
        internal static readonly string rulesugg2 = "There should not be SKOS concepts having the same value for skos:altLabel and skos:hiddenLabel data annotations.";
        internal static readonly string rulesugg3 = "There should not be SKOS-XL concepts having the same value for skosxl:altLabel and skosxl:prefLabel data relations.";
        internal static readonly string rulesugg4 = "There should not be SKOS-XL concepts having the same value for skosxl:altLabel and skosxl:hiddenLabel data relations.";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            //SKOS

            //skos:altLabel VS skos:prefLabel
            SWRLRule altprefRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSAlternativeLabelAnalysisRule)),
                new RDFPlainLiteral("This rule checks for collisions of values assumed by a skos:Concept in its skos:altLabel and skos:prefLabel data annotations"),
                new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>() 
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C"))),
                        new SWRLAnnotationPropertyAtom(
                            new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL"))),
                        new SWRLAnnotationPropertyAtom(
                            new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL")))
                    }
                },
                new SWRLConsequent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLLiteralArgument(RDFTypedLiteral.True))
                    }
                });
            violations.AddRange(altprefRule.ApplyToOntologyAsync(ontology).GetAwaiter().GetResult());
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg1,
                    $"SKOS concept '{((OWLDataPropertyAssertion)violation.Axiom).IndividualExpression.GetIRI()}' should be adjusted to not clash on skos:altLabel and skos:prefLabel values"
                )));
            violations.Clear();

            //skos:altLabel VS skos:hiddenLabel
            SWRLRule althiddenRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSAlternativeLabelAnalysisRule)),
                new RDFPlainLiteral("This rule checks for collisions of values assumed by a skos:Concept in its skos:altLabel and skos:hiddenLabel data annotations"),
                new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C"))),
                        new SWRLAnnotationPropertyAtom(
                            new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL"))),
                        new SWRLAnnotationPropertyAtom(
                            new OWLAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?HIDDEN_LABEL")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL")),
                            new SWRLVariableArgument(new RDFVariable("?HIDDEN_LABEL")))
                    }
                },
                new SWRLConsequent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLLiteralArgument(RDFTypedLiteral.True))
                    }
                });
            violations.AddRange(althiddenRule.ApplyToOntologyAsync(ontology).GetAwaiter().GetResult());
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg2,
                    $"SKOS concept '{((OWLDataPropertyAssertion)violation.Axiom).IndividualExpression.GetIRI()}' should be adjusted to not clash on skos:altLabel and skos:hiddenLabel values"
                )));
            violations.Clear();

            //SKOS-XL

            //skosxl:altLabel VS skosxl:prefLabel
            SWRLRule altprefXLRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSAlternativeLabelAnalysisRule)),
                new RDFPlainLiteral("This rule checks for collisions of values assumed by a skos:Concept in its skosxl:altLabel and skosxl:prefLabel data relations"),
                new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?AL"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?AL")),
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?PL"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?PL")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL")))
                    }
                },
                new SWRLConsequent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLLiteralArgument(RDFTypedLiteral.True))
                    }
                });
            violations.AddRange(altprefXLRule.ApplyToOntologyAsync(ontology).GetAwaiter().GetResult());
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg3,
                    $"SKOS concept '{((OWLDataPropertyAssertion)violation.Axiom).IndividualExpression.GetIRI()}' should be adjusted to not clash on skosxl:altLabel and skosxl:prefLabel values"
                )));
            violations.Clear();

            //skosxl:altLabel VS skosxl:hiddenLabel
            SWRLRule althiddenXLRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSAlternativeLabelAnalysisRule)),
                new RDFPlainLiteral("This rule checks for collisions of values assumed by a skos:Concept in its skosxl:altLabel and skosxl:hiddenLabel data relations"),
                new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?AL"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?AL")),
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?HL"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?HL")),
                            new SWRLVariableArgument(new RDFVariable("?HIDDEN_LABEL")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?ALT_LABEL")),
                            new SWRLVariableArgument(new RDFVariable("?HIDDEN_LABEL")))
                    }
                },
                new SWRLConsequent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLLiteralArgument(RDFTypedLiteral.True))
                    }
                });
            violations.AddRange(althiddenXLRule.ApplyToOntologyAsync(ontology).GetAwaiter().GetResult());
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg4,
                    $"SKOS concept '{((OWLDataPropertyAssertion)violation.Axiom).IndividualExpression.GetIRI()}' should be adjusted to not clash on skosxl:altLabel and skosxl:hiddenLabel values"
                )));
            violations.Clear();

            return issues;
        }
    }
}