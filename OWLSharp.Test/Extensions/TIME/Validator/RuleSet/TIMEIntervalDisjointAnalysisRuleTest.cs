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
public class TIMEIntervalDisjointAnalysisRuleTest : TIMETestOntology
{
    #region Tests
    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule1()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalContains";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule2()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalDuring";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule3()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalEquals";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule4()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalFinishedBy";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule5()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalFinishes";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule6()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_INSIDE),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:hasInside";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule7()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_IN),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalIn";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule8()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalMeets";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule9()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalMetBy";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule10()
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
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:notDisjoint";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule11()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalOverlappedBy";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule12()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalOverlaps";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule13()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalStartedBy";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeIntervalDisjointAndViolateRule14()
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
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))),
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalB"))), //clash
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT),
                new OWLNamedIndividual(new RDFResource("ex:IntervalA")),
                new OWLNamedIndividual(new RDFResource("ex:IntervalC")))
        ]);
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
            { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
        };
        List<OWLIssue> issues = await TIMEIntervalDisjointAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);
        const string clashingRelation = "time:intervalStarts";

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEIntervalDisjointAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, string.Format(TIMEIntervalDisjointAnalysisRule.rulesugg, clashingRelation)));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, $"TIME intervals 'ex:IntervalA' and 'ex:IntervalB' should be adjusted to not clash on temporal relations (time:intervalDisjoint VS {clashingRelation})"));
    }
    #endregion
}