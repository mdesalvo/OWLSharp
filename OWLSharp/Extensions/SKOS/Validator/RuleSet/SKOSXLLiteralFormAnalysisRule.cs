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
    internal class SKOSXLLiteralFormAnalysisRule
    {
        internal static readonly string rulename = SKOSEnums.SKOSValidatorRules.LiteralFormAnalysis.ToString();
        internal static readonly string rulesugg = "There should not be SKOS-XL labels having more than one occurrence of skosxl:literalForm relation";

        internal static async Task<List<OWLIssue>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            SWRLRule literalFormRule = new SWRLRule(
                new RDFPlainLiteral(nameof(SKOSXLLiteralFormAnalysisRule)),
                new RDFPlainLiteral("This rule checks for skosxl:Label instances having more than one occurrence of skosxl:literalForm relation"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                            new SWRLVariableArgument(new RDFVariable("?L"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?L")),
                            new SWRLVariableArgument(new RDFVariable("?LF1"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                            new SWRLVariableArgument(new RDFVariable("?L")),
                            new SWRLVariableArgument(new RDFVariable("?LF2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?LF1")),
                            new SWRLVariableArgument(new RDFVariable("?LF2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(SKOSValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?L")),
                            new SWRLLiteralArgument(RDFTypedLiteral.True))
                    }
                });
            violations.AddRange(await literalFormRule.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg,
                    $"SKOS-XL label '{((OWLDataPropertyAssertion)violation.Axiom).IndividualExpression.GetIRI()}' should be adjusted to not have more than one occurrence of skosxl:literalForm values"
                )));
            violations.Clear();

            return issues;
        }
    }
}