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

namespace OWLSharp.Extensions.TIME
{
    internal class TIMEInstantAfterAnalysisRule
    {
        internal static readonly string rulename = nameof(TIMEEnums.TIMEValidatorRules.InstantAfterAnalysis);
        internal const string rulesugg1 = "There should not be OWL-TIME instants having a clash in temporal relations (time:after VS time:after)";
        internal const string rulesugg2 = "There should not be OWL-TIME instants having a clash in temporal relations (time:after VS time:before)";

        internal static async Task<List<OWLIssue>> ExecuteRuleAsync(OWLOntology ontology, Dictionary<string, List<OWLIndividualExpression>> cacheRegistry)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            //time:after vs time:after
            SWRLRule clashRule1 = new SWRLRule(
                new RDFPlainLiteral(nameof(TIMEInstantAfterAnalysisRule)),
                new RDFPlainLiteral("AFTER(?I1,?I2) ^ AFTER(?I2,?I1) -> ERROR"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.TIME.INSTANT),
                            new SWRLVariableArgument(new RDFVariable("?I1"))) { IndividualsCache = cacheRegistry["INSTANTS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.TIME.INSTANT),
                            new SWRLVariableArgument(new RDFVariable("?I2"))) { IndividualsCache = cacheRegistry["INSTANTS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                            new SWRLVariableArgument(new RDFVariable("?I2")),
                            new SWRLVariableArgument(new RDFVariable("?I1")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(TIMEValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2")))
                    }
                });
            violations.AddRange(await clashRule1.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg1,
                    $"TIME instants '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on temporal relations (time:after VS time:after)"
                )));
            violations.Clear();

            //time:after vs time:before
            SWRLRule clashRule2 = new SWRLRule(
                new RDFPlainLiteral(nameof(TIMEInstantAfterAnalysisRule)),
                new RDFPlainLiteral("AFTER(?I1,?I2) ^ BEFORE(?I1,?I2) -> ERROR"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.TIME.INSTANT),
                            new SWRLVariableArgument(new RDFVariable("?I1"))),
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.TIME.INSTANT),
                            new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(TIMEValidator.ViolationIRI),
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2")))
                    }
                });
            violations.AddRange(await clashRule2.ApplyToOntologyAsync(ontology));
            violations.ForEach(violation => issues.Add(
                new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Error,
                    rulename,
                    rulesugg2,
                    $"TIME instants '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on temporal relations (time:after VS time:before)"
                )));
            violations.Clear();

            return issues;
        }
    }
}