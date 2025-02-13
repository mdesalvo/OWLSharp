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
    internal class TIMEIntervalNotDisjointAnalysisRule
    {
        internal static readonly string rulename = TIMEEnums.TIMEValidatorRules.IntervalNotDisjointAnalysis.ToString();
        internal static readonly string rulesugg = "There should not be OWL-TIME intervals having a clash in temporal relations (time:notDisjoint VS {0}";

        internal static async Task<List<OWLIssue>> ExecuteRuleAsync(OWLOntology ontology, Dictionary<string, List<OWLIndividualExpression>> cacheRegistry)
        {
            List<OWLIssue> issues = new List<OWLIssue>();
            List<OWLInference> violations = new List<OWLInference>();

            #region Utilities
            async Task ExecuteRuleBodyAsync(string ruleDescription, string ruleSugg, SWRLObjectPropertyAtom clashingAtom, string shortClashingProperty)
            {
                SWRLRule clashRule = new SWRLRule(
                new RDFPlainLiteral(nameof(TIMEIntervalContainsAnalysisRule)),
                new RDFPlainLiteral(ruleDescription),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.TIME.INTERVAL),
                            new SWRLVariableArgument(new RDFVariable("?I1"))) { IndividualsCache = cacheRegistry["INTERVALS"] },
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.TIME.INTERVAL),
                            new SWRLVariableArgument(new RDFVariable("?I2"))) { IndividualsCache = cacheRegistry["INTERVALS"] },
                        new SWRLObjectPropertyAtom(
                            new OWLObjectProperty(RDFVocabulary.TIME.NOT_DISJOINT),
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2"))),
                        clashingAtom,
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
                violations.AddRange(await clashRule.ApplyToOntologyAsync(ontology));
                violations.ForEach(violation => issues.Add(
                    new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        ruleSugg,
                        $"TIME intervals '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on temporal relations (time:notDisjoint VS {shortClashingProperty})"
                    )));
                violations.Clear();
            }
            #endregion

            await ExecuteRuleBodyAsync(
                "NOT_DISJOINT(?I1,?I2) ^ INTERVAL_BEFORE(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalBefore"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalBefore");

            await ExecuteRuleBodyAsync(
                "NOT_DISJOINT(?I1,?I2) ^ INTERVAL_AFTER(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalAfter"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalAfter");

            await ExecuteRuleBodyAsync(
                "NOT_DISJOINT(?I1,?I2) ^ INTERVAL_DISJOINT(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalDisjoint"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalDisjoint");

            return issues;
        }
    }
}