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
    internal class SKOSPreferredLabelAnalysisRule
    {
        internal static readonly string rulename = SKOSEnums.SKOSValidatorRules.PreferredLabelAnalysis.ToString();
        internal static readonly string rulesugg1 = "There should not be SKOS concepts having more than one occurrence of the same language tag in values of skos:prefLabel data annotations.";
        internal static readonly string rulesugg2 = "There should not be SKOS concepts having more than one occurrence of the same language tag in values of skosxl:prefLabel data relations.";
        
        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            //SKOS

            SWRLRule prefRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSPreferredLabelAnalysisRule)),
                new RDFPlainLiteral("This rule checks for duplicate language tags detected for values assumed by a skos:Concept in its skos:prefLabel data annotations"),
                new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>() 
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C"))),
                        new SWRLAnnotationPropertyAtom(
                            new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_1"))),
                        new SWRLAnnotationPropertyAtom(
                            new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_1")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_2"))),
                        SWRLBuiltIn.EXTLangMatches(
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_1")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_2")))
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
            violations.AddRange(prefRule.ApplyToOntologyAsync(ontology).GetAwaiter().GetResult());
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg1,
                    $"SKOS concept '{((OWLDataPropertyAssertion)violation.Axiom).IndividualExpression.GetIRI()}' should be adjusted to not have more than one occurrence of the same language tag in skos:prefLabel values"
                )));
            violations.Clear();

            //SKOS-XL

            SWRLRule prefXLRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSPreferredLabelAnalysisRule)),
                new RDFPlainLiteral("This rule checks for duplicate language tags detected for values assumed by a skos:Concept in its skosxl:prefLabel data relations"),
                new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?PL1"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?PL1")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_1"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL),
                            new SWRLVariableArgument(new RDFVariable("?C")),
                            new SWRLVariableArgument(new RDFVariable("?PL2"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?PL2")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?PL1")),
                            new SWRLVariableArgument(new RDFVariable("?PL2"))),
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_1")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_2"))),
                        SWRLBuiltIn.EXTLangMatches(
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_1")),
                            new SWRLVariableArgument(new RDFVariable("?PREF_LABEL_2")))
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
            violations.AddRange(prefXLRule.ApplyToOntologyAsync(ontology).GetAwaiter().GetResult());
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg2,
                    $"SKOS concept '{((OWLDataPropertyAssertion)violation.Axiom).IndividualExpression.GetIRI()}' should be adjusted to not have more than one occurrence of the same language tag in skosxl:prefLabel values"
                )));
            violations.Clear();

            return issues;
        }
    }
}