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
    internal class TIMEIntervalOverlappedByAnalysisRule
    {
        internal static readonly string rulename = TIMEEnums.TIMEValidatorRules.IntervalOverlappedByAnalysis.ToString();
        internal const string rulesugg = "There should not be OWL-TIME intervals having a clash in temporal relations (time:intervalOverlappedBy VS {0}";

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
                            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY),
                            new SWRLVariableArgument(new RDFVariable("?I1")),
                            new SWRLVariableArgument(new RDFVariable("?I2"))),
                        clashingAtom
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
                        $"TIME intervals '{((OWLObjectPropertyAssertion)violation.Axiom).SourceIndividualExpression.GetIRI()}' and '{((OWLObjectPropertyAssertion)violation.Axiom).TargetIndividualExpression.GetIRI()}' should be adjusted to not clash on temporal relations (time:intervalOverlappedBy VS {shortClashingProperty})"
                    )));
                violations.Clear();
            }
            #endregion

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_BEFORE(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalBefore"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalBefore");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_AFTER(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalAfter"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalAfter");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_CONTAINS(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalContains"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalContains");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_DISJOINT(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalDisjoint"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalDisjoint");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_DURING(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalDuring"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalDuring");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_EQUALS(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalEquals"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalEquals");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_FINISHED_BY(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalFinishedBy"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHED_BY),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalFinishedBy");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_FINISHES(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalFinishes"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalFinishes");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ HAS_INSIDE(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:hasInside"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_INSIDE),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:hasInside");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_IN(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalIn"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_IN),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalIn");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_MEETS(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalMeets"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalMeets");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_MET_BY(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalMetBy"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalMetBy");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_OVERLAPS(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalOverlaps"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPS),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalOverlaps");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_STARTED_BY(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalStartedBy"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalStartedBy");

            await ExecuteRuleBodyAsync(
                "INTERVAL_OVERLAPPED_BY(?I1,?I2) ^ INTERVAL_STARTS(?I1,?I2) -> ERROR",
                string.Format(rulesugg, "time:intervalStarts"),
                new SWRLObjectPropertyAtom(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS),
                    new SWRLVariableArgument(new RDFVariable("?I1")),
                    new SWRLVariableArgument(new RDFVariable("?I2"))),
                "time:intervalStarts");

            return issues;
        }
    }
}