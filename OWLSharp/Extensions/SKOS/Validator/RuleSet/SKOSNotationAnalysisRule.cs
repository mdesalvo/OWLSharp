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
    internal class SKOSNotationAnalysisRule
    {
        internal static readonly string rulename = SKOSEnums.SKOSValidatorRules.NotationAnalysis.ToString();
        internal static readonly string rulesugg = "There should not be SKOS concepts sharing the same value for skos:Notation data relation under the same schema.";

        internal static async Task<List<OWLIssue>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            SWRLRule notationRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSNotationAnalysisRule)),
                new RDFPlainLiteral("This rule checks for collisions of values assumed by different skos:Concept instances in their skos:Notation data relations within the same skos:ConceptScheme"),
                new SWRLAntecedent()
                {
                    Atoms = new List<SWRLAtom>() 
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                            new SWRLVariableArgument(new RDFVariable("?S"))),
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C1"))),
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?S"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                            new SWRLVariableArgument(new RDFVariable("?C2")),
                            new SWRLVariableArgument(new RDFVariable("?S"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.NOTATION),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?N1"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.NOTATION),
                            new SWRLVariableArgument(new RDFVariable("?C2")),
                            new SWRLVariableArgument(new RDFVariable("?N2"))),
                    },
                    BuiltIns = new List<SWRLBuiltIn>()
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2"))),
                        SWRLBuiltIn.Equal(
                            new SWRLVariableArgument(new RDFVariable("?N1")),
                            new SWRLVariableArgument(new RDFVariable("?N2")))
                    }
                },
                new SWRLConsequent()
                {
                    Atoms = new List<SWRLAtom>()
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?C1")),
                            new SWRLVariableArgument(new RDFVariable("?C2")))
                    }
                });
            violations.AddRange(await notationRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg,
                    $"SKOS concepts '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on skos:Notation values"
                )));
            violations.Clear();

            return issues;
        }
    }
}