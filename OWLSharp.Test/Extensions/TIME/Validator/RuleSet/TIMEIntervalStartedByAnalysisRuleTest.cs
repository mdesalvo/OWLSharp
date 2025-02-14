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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.TIME;
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.TIME;

[TestClass]
public class TIMEIntervalStartedByAnalysisRuleTest : TIMETestOntology
{
    #region Tests
    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule1()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalBefore";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule2()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalAfter";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule3()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalContains";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule4()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalDisjoint";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule5()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalDuring";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule6()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalEquals";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule7()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalFinishes";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule8()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalFinishedBy";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule9()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_IN),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalIn";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule10()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalOverlaps";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule11()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalMeets";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule12()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalMetBy";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule13()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalOverlappedBy";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }[TestMethod]
    public async Task ShouldAnalyzeIntervalStartedByAndViolateRule14()
    {
        OWLOntology ontology = new OWLOntology(TestOntology);
        ontology.DeclarationAxioms.AddRange([
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalA"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalStartedByAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        string clashingRelation = "time:intervalStarts";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalStartedByAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalStartedByAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalStartedBy VS {clashingRelation})"));
    }
    #endregion
}