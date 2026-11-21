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
public class OWLSubClassOfAnalysisTest
{
    #region Tests
    [TestMethod]
    public void ShouldNotAnalyzeSubClassOfMutualSubClassOfCase()
    {
        //Mutual SubClassOf is a common, deliberate idiom for expressing class equivalence without an explicit
        //EquivalentClasses axiom: it is redundant, not contradictory, so it must not be flagged at all
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                    new OWLClass(RDFVocabulary.FOAF.PERSON))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldNotAnalyzeSubClassOfEquivalentClassesCase()
    {
        //SubClassOf restating one direction of an already-declared EquivalentClasses is redundant, not contradictory
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLEquivalentClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfDisjointClassesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLDisjointClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg1)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfExactObjectCardinalityCase()
    {
        //With an upper bound of 0 a single filler already is N+1 (1) pairwise different fillers: it stays an Error even without DifferentIndividuals
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectExactCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 0))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfQUalifiedExactObjectCardinalityCase()
    {
        //With an upper bound of 0 a single filler already is N+1 (1) pairwise different fillers: it stays an Error even without DifferentIndividuals
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectExactCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 0, new OWLClass(new RDFResource("ex:QCLS"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion (IDV2 isA QCLS)
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:QCLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:QCLS"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxObjectCardinalityCase()
    {
        //With an upper bound of 0 a single filler already is N+1 (1) pairwise different fillers: it stays an Error even without DifferentIndividuals
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 0))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfQUalifiedMaxObjectCardinalityCase()
    {
        //With an upper bound of 0 a single filler already is N+1 (1) pairwise different fillers: it stays an Error even without DifferentIndividuals
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 0, new OWLClass(new RDFResource("ex:QCLS"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion (IDV2 isA QCLS)
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:QCLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:QCLS"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:OP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfExactDataCardinalityCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLDataExactCardinality(
                        new OWLDataProperty(new RDFResource("ex:DP")), 0))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFPlainLiteral("value")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:DP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfQualifiedExactDataCardinalityCase()
    {
        OWLDatatypeRestriction length6to10Facet = new OWLDatatypeRestriction(
            new OWLDatatype(RDFVocabulary.XSD.STRING),
            [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]);

        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLDataExactCardinality(
                        new OWLDataProperty(new RDFResource("ex:DP")), 0, length6to10Facet))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFTypedLiteral("literal", RDFModelEnums.RDFDatatypes.XSD_STRING)))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:DP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxDataCardinalityCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLDataMaxCardinality(
                        new OWLDataProperty(new RDFResource("ex:DP")), 0))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFPlainLiteral("value")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:DP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfQualifiedMaxDataCardinalityCase()
    {
        OWLDatatypeRestriction length6to10Facet = new OWLDatatypeRestriction(
            new OWLDatatype(RDFVocabulary.XSD.STRING),
            [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]);

        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLDataMaxCardinality(
                        new OWLDataProperty(new RDFResource("ex:DP")), 0, length6to10Facet))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))), //violated by the assertion
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFTypedLiteral("literal", RDFModelEnums.RDFDatatypes.XSD_STRING)))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:CLS"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:DP"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxObjectCardinalityWithDifferentFillersCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV3) ^ DifferentIndividuals(IDV2,IDV3) -> ERROR
        //N+1 (2) pairwise different fillers can never be unified down to N (1): genuine inconsistency
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3"))),
                new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:IDV2")), new OWLNamedIndividual(new RDFResource("ex:IDV3")) ])
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxObjectCardinalityWithoutDifferentFillersCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV3) -> WARNING
        //No unique name assumption: the Reasoner unifies IDV2 and IDV3 unless they are declared different, so it is not an inconsistency
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldNotAnalyzeSubClassOfMaxObjectCardinalityWithSameIndividualFillersCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV3) ^ SameIndividual(IDV2,IDV3) -> NO ISSUE
        //Fillers related by SameIndividual denote one individual only, so they count once
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3"))),
                new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:IDV2")), new OWLNamedIndividual(new RDFResource("ex:IDV3")) ])
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldNotAnalyzeSubClassOfMaxObjectCardinalityWithSameFillerAssertedTwiceCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV2) -> NO ISSUE
        //The same filler asserted twice is one filler only
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldNotAnalyzeSubClassOfQualifiedMaxObjectCardinalityWithFillerOutsideClassCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP,QCLS)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV3) ^ ClassAssertion(QCLS,IDV2) -> NO ISSUE
        //IDV3 does not belong to the qualifying class, so only one filler is bounded by the restriction
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1, new OWLClass(new RDFResource("ex:QCLS"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:QCLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxObjectCardinalityWithOWLThingQualifierCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP,owl:Thing)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV3) -> WARNING
        //An owl:Thing qualifier excludes no filler: it is handled as the unqualified restriction even if IDV2 and IDV3 are not asserted in any class
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1, new OWLClass(RDFVocabulary.OWL.THING)))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxObjectCardinalityWithInverseOfCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,ObjectInverseOf(OP))) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV2,IDV1) ^ OP(IDV3,IDV1) ^ DifferentIndividuals(IDV2,IDV3) -> ERROR
        //Under ObjectInverseOf the constrained individual is the target of the assertions and the fillers are their sources
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:OP"))), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:IDV2")), new OWLNamedIndividual(new RDFResource("ex:IDV3")) ])
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxObjectCardinalityWithInverseOfAssertionsCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,ObjectInverseOf(OP))) ^ ClassAssertion(CLS,IDV1) ^ ObjectInverseOf(OP)(IDV1,IDV2) ^ ObjectInverseOf(OP)(IDV1,IDV3) -> WARNING
        //Assertions stated through ObjectInverseOf(OP) have IDV1 as the constrained individual and IDV2/IDV3 as fillers: not declared different, so the Reasoner unifies them
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:OP"))), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:OP"))),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:OP"))),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfExactObjectCardinalityWithPairwiseDifferentFillersCase()
    {
        //SubClassOf(CLS,ObjectExactCardinality(2,OP)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV3) ^ OP(IDV1,IDV4) ^ DifferentIndividuals(IDV2,IDV3,IDV4) -> ERROR
        //N+1 (3) pairwise different fillers can never be unified down to N (2)
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectExactCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 2))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV4"))),
                new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:IDV2")), new OWLNamedIndividual(new RDFResource("ex:IDV3")), new OWLNamedIndividual(new RDFResource("ex:IDV4")) ])
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfExactObjectCardinalityWithNotPairwiseDifferentFillersCase()
    {
        //SubClassOf(CLS,ObjectExactCardinality(2,OP)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2) ^ OP(IDV1,IDV3) ^ OP(IDV1,IDV4) ^ DifferentIndividuals(IDV2,IDV3) -> WARNING
        //Only IDV2 and IDV3 are declared different: the Reasoner can still unify IDV4 with one of them, so there are no N+1 (3) pairwise different fillers
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectExactCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 2))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV4"))),
                new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:IDV2")), new OWLNamedIndividual(new RDFResource("ex:IDV3")) ])
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxObjectCardinalityBeyondExactSearchLimitCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(4,OP)) ^ ClassAssertion(CLS,IDV1) ^ OP(IDV1,IDV2..IDV6) ^ DifferentIndividuals(IDV2..IDV6) -> WARNING
        //The exact search of N+1 pairwise different fillers is limited to N up to 3: beyond it only the Warning is emitted
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 4))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV4"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV5"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:OP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV6"))),
                new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:IDV2")), new OWLNamedIndividual(new RDFResource("ex:IDV3")), new OWLNamedIndividual(new RDFResource("ex:IDV4")), new OWLNamedIndividual(new RDFResource("ex:IDV5")), new OWLNamedIndividual(new RDFResource("ex:IDV6")) ])
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg4)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfMaxDataCardinalityWithDifferentLiteralsCase()
    {
        //SubClassOf(CLS,DataMaxCardinality(1,DP)) ^ ClassAssertion(CLS,IDV1) ^ DP(IDV1,"value1") ^ DP(IDV1,"value2") -> ERROR
        //Distinct literals are distinct values (no unique name assumption applies to data): they can never be unified
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLDataMaxCardinality(
                        new OWLDataProperty(new RDFResource("ex:DP")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFPlainLiteral("value1"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFPlainLiteral("value2")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg3)));
    }

    [TestMethod]
    public void ShouldNotAnalyzeSubClassOfMaxDataCardinalityWithSameLiteralAssertedTwiceCase()
    {
        //SubClassOf(CLS,DataMaxCardinality(1,DP)) ^ ClassAssertion(CLS,IDV1) ^ DP(IDV1,"value") ^ DP(IDV1,"value") -> NO ISSUE
        //The same literal asserted twice is one value only
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLDataMaxCardinality(
                        new OWLDataProperty(new RDFResource("ex:DP")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFPlainLiteral("value"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:DP")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFPlainLiteral("value")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfObjectCardinalityOnTransitivePropertyCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP)) ^ TransitiveObjectProperty(OP) -> ERROR
        //The OWL2 DL simple-role restriction does not depend on the assertions: CLS has no individuals here
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            ObjectPropertyAxioms = [
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg5)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfObjectCardinalityOnPropertyWithTransitiveSubPropertyCase()
    {
        //SubClassOf(CLS,ObjectMinCardinality(1,OP)) ^ SubObjectPropertyOf(SUBOP,OP) ^ TransitiveObjectProperty(SUBOP) -> ERROR
        //A property having a transitive subproperty is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMinCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            ObjectPropertyAxioms = [
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:SUBOP"))),
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:SUBOP")), new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg5)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubClassOfObjectCardinalityOnPropertyDefinedByChainCase()
    {
        //SubClassOf(CLS,ObjectExactCardinality(1,OP)) ^ SubObjectPropertyOf(ObjectPropertyChain(OP1,OP2),OP) -> ERROR
        //A property defined as the superproperty of a chain is non-simple
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectExactCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:OP1")), new OWLObjectProperty(new RDFResource("ex:OP2")) ]),
                    new OWLObjectProperty(new RDFResource("ex:OP")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysis.rulesugg5)));
    }

    [TestMethod]
    public void ShouldNotAnalyzeSubClassOfObjectCardinalityOnPropertyUnderTransitiveCase()
    {
        //SubClassOf(CLS,ObjectMaxCardinality(1,OP)) ^ SubObjectPropertyOf(OP,SUPEROP) ^ TransitiveObjectProperty(SUPEROP) -> NO ISSUE
        //A subproperty of a transitive property stays simple (e.g. hasMother under hasAncestor)
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:CLS")),
                    new OWLObjectMaxCardinality(
                        new OWLObjectProperty(new RDFResource("ex:OP")), 1))
            ],
            ObjectPropertyAxioms = [
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:SUPEROP"))),
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:OP")), new OWLObjectProperty(new RDFResource("ex:SUPEROP")))
            ]
        };
        List<OWLIssue> issues = OWLSubClassOfAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.IsEmpty(issues);
    }
    #endregion
}