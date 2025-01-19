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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.TIME;
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEValidatorTest : TIMETestOntology
    {
        [TestMethod]
        public void ShouldAddRule()
        {
            TIMEValidator validator = new TIMEValidator();
            
            Assert.IsNotNull(validator);
            Assert.IsNotNull(validator.Rules);
            Assert.IsTrue(validator.Rules.Count == 0);

            validator.AddRule(TIMEEnums.TIMEValidatorRules.InstantAfterAnalysis);
            Assert.IsTrue(validator.Rules.Count == 1);
        }

        [TestMethod]
        public async Task ShouldValidateInstantAfter()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.InstantAfterAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEInstantAfterAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, TIMEInstantAfterAnalysisRule.rulesugg1));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "TIME instants 'ex:InstantA' and 'ex:InstantB' should be adjusted to not clash on temporal relations (time:after VS time:after)"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEInstantAfterAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, TIMEInstantAfterAnalysisRule.rulesugg1));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, "TIME instants 'ex:InstantB' and 'ex:InstantA' should be adjusted to not clash on temporal relations (time:after VS time:after)"));
        }

        [TestMethod]
        public async Task ShouldValidateInstantBefore()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.InstantBeforeAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEInstantBeforeAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, TIMEInstantBeforeAnalysisRule.rulesugg1));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "TIME instants 'ex:InstantA' and 'ex:InstantB' should be adjusted to not clash on temporal relations (time:before VS time:before)"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEInstantBeforeAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, TIMEInstantBeforeAnalysisRule.rulesugg1));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, "TIME instants 'ex:InstantB' and 'ex:InstantA' should be adjusted to not clash on temporal relations (time:before VS time:before)"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalAfter()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalAfterAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalAfter";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalAfterAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalAfterAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEIntervalAfterAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, string.Format(TIMEIntervalAfterAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, $"TIME intervals 'ex:IntervalB' and 'ex:IntervalA' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalBefore()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalBeforeAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalBefore";
            string clashingRelation = "time:intervalBefore";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalBeforeAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalBeforeAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEIntervalBeforeAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, string.Format(TIMEIntervalBeforeAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, $"TIME intervals 'ex:IntervalB' and 'ex:IntervalA' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalContains()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalContainsAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalContains";
            string clashingRelation = "time:intervalContains";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalContainsAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalContainsAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEIntervalContainsAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, string.Format(TIMEIntervalContainsAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, $"TIME intervals 'ex:IntervalB' and 'ex:IntervalA' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalDisjoint()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.NOT_DISJOINT),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalDisjointAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalDisjoint";
            string clashingRelation = "time:notDisjoint";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalDuring()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalDuringAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalDuring";
            string clashingRelation = "time:intervalDuring";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDuringAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDuringAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEIntervalDuringAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, string.Format(TIMEIntervalDuringAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, $"TIME intervals 'ex:IntervalB' and 'ex:IntervalA' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalEquals()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalEqualsAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalEquals";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalEqualsAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalEqualsAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalFinishes()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalFinishesAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalFinishes";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalFinishesAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalFinishesAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalFinishedBy()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHED_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHED_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalFinishedByAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalFinishedBy";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalFinishedByAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalFinishedByAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalHasInside()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_INSIDE),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_INSIDE),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalHasInsideAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:hasInside";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalHasInsideAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalHasInsideAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalIn()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_IN),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_IN),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalInAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalIn";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalInAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalInAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalMeets()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalMeetsAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalMeets";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalMeetsAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalMeetsAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalMetBy()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalMetByAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalMetBy";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalMetByAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalMetByAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalNotDisjoint()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.NOT_DISJOINT),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.NOT_DISJOINT),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalNotDisjointAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:notDisjoint";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalNotDisjointAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalNotDisjointAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalOverlaps()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalOverlapsAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalOverlaps";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalOverlapsAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalOverlapsAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalOverlappedBy()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalOverlappedByAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalOverlappedBy";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalOverlappedByAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalOverlappedByAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalStarts()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalStartsAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalStarts";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartsAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartsAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }

        [TestMethod]
        public async Task ShouldValidateIntervalStartedBy()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INTERVAL),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                    new OWLNamedIndividual(new RDFResource("ex:IntervalC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.IntervalStartedByAnalysis);

            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);
            string testingRelation = "time:intervalStartedBy";
            string clashingRelation = "time:intervalAfter";

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations ({testingRelation} VS {clashingRelation})"));
        }
    }
}