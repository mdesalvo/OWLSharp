/*
  Copyright 2014-2026 Marco De Salvo
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
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator;

[TestClass]
public class OWLDisjointClassesAnalysisTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeDisjointClassesSubClassOfCase1()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointClassesSubClassOfCase2()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                    new OWLClass(RDFVocabulary.FOAF.PERSON))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointClassesEquivalentClassesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION), new OWLClass(RDFVocabulary.FOAF.AGENT) ]),
                new OWLEquivalentClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointClassesClassAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv")))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointClassesWithBothViolations()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                    new OWLClass(RDFVocabulary.FOAF.ACCOUNT)]),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                    new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLEquivalentClasses([
                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                    new OWLClass(RDFVocabulary.FOAF.ACCOUNT)])
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.ACCOUNT),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ACCOUNT)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv")))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        //The two unsatisfiable subclasses (Agent and Account, both below Person and Organization) add two T-BOX warnings to the overlap one
        Assert.HasCount(4, issues);
        Assert.HasCount(2, issues.FindAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg3)));
        //T-BOX overlap is a modeling smell (forces classes equivalent to owl:Nothing), not an ontology-level inconsistency
        Assert.IsTrue(issues.Exists(iss => iss.Description.Contains("(T-BOX)") && iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        //A-BOX shared ClassAssertion on disjoint classes is a genuine ontology-level inconsistency
        Assert.IsTrue(issues.Exists(iss => iss.Description.Contains("(A-BOX)") && iss.Severity == OWLEnums.OWLIssueSeverity.Error));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointClassesWithUnsatisfiableDirectSubClassCase()
    {
        //SubClassOf(C,A) ^ SubClassOf(C,B) ^ DisjointClasses(A,B) -> WARNING
        //C can have no individuals: it is equivalent to owl:Nothing
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:A")), new OWLClass(new RDFResource("ex:B")) ]),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLClass(new RDFResource("ex:A"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLClass(new RDFResource("ex:B")))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg3)));
        Assert.IsTrue(issues.TrueForAll(iss => iss.Description.Contains("ex:C")));
    }

    [TestMethod]
    public void ShouldNotAnalyzeDisjointClassesWithNothingAsCommonSubClassCase()
    {
        //SubClassOf(owl:Nothing,A) ^ SubClassOf(owl:Nothing,B) ^ DisjointClasses(A,B) -> no issue
        //owl:Nothing sits below every class once the schema is reasoned (scm-cls) and is empty by definition
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:A")), new OWLClass(new RDFResource("ex:B")) ]),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.OWL.NOTHING),
                    new OWLClass(new RDFResource("ex:A"))),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.OWL.NOTHING),
                    new OWLClass(new RDFResource("ex:B")))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointClassesWithUnsatisfiableIndirectSubClassCase()
    {
        //SubClassOf(C,A) ^ SubClassOf(C,B2) ^ SubClassOf(B2,B) ^ DisjointClasses(A,B) -> WARNING
        //C is an indirect subclass of B through B2: it is still equivalent to owl:Nothing (B2 is not, since it is not below A)
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:A")), new OWLClass(new RDFResource("ex:B")) ]),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLClass(new RDFResource("ex:A"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLClass(new RDFResource("ex:B2"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:B2")),
                    new OWLClass(new RDFResource("ex:B")))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg3)));
        Assert.IsTrue(issues.TrueForAll(iss => iss.Description.Contains("ex:C")));
    }

    [TestMethod]
    public void ShouldNotAnalyzeDisjointClassesWithSubClassOfOnlyOneMemberCase()
    {
        //SubClassOf(C,A) ^ DisjointClasses(A,B) -> NO ISSUE
        //C is below one member only of the disjoint pair: it stays satisfiable
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:A")), new OWLClass(new RDFResource("ex:B")) ]),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLClass(new RDFResource("ex:A")))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointClassesWithUnsatisfiableSubClassHavingIndividualCase()
    {
        //SubClassOf(C,A) ^ SubClassOf(C,B) ^ DisjointClasses(A,B) ^ ClassAssertion(C,I) -> WARNING (C) ^ ERROR (cax-dw on I)
        //An individual of the unsatisfiable class C is already reported as Error by the A-BOX check, since I is a member of both A and B
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:A")), new OWLClass(new RDFResource("ex:B")) ]),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLClass(new RDFResource("ex:A"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLClass(new RDFResource("ex:B")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLNamedIndividual(new RDFResource("ex:I")))
            ]
        };
        List<OWLIssue> issues = OWLDisjointClassesAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(2, issues);
        Assert.IsTrue(issues.Exists(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning && string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg3)));
        Assert.IsTrue(issues.Exists(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error && string.Equals(iss.Suggestion, OWLDisjointClassesAnalysis.rulesugg2)));
    }
    #endregion
}