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

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLOntologyHelperTest
{
    #region Tests
    [TestMethod]
    public async Task ShouldApplyAskQueryWithReasonerToOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls3"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls3")),
                    new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLClass(new RDFResource("ex:cls2"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLClass(new RDFResource("ex:cls3")))
            ],
            Rules = [
                new SWRLRule(
                    new RDFPlainLiteral("Example"),
                    new RDFPlainLiteral("Example"),
                    new SWRLAntecedent
                    {
                        Atoms = [
                            new SWRLClassAtom(
                                new OWLClass(new RDFResource("ex:cls3")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")))
                        ],
                        BuiltIns = [
                            SWRLBuiltIn.NotEqual(
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLIndividualArgument(new RDFResource("ex:idv3")))
                        ]
                    },
                    new SWRLConsequent
                    {
                        Atoms = [
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:owlsharp:hasInference")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLLiteralArgument(RDFTypedLiteral.True))
                        ]
                    })
            ]
        };
        OWLReasoner reasoner = new OWLReasoner
        {
            Rules = [
                OWLEnums.OWLReasonerRules.SubClassOfEntailment,
                OWLEnums.OWLReasonerRules.ClassAssertionEntailment
            ]
        };
        RDFAskQuery query = new RDFAskQuery()
            .AddPatternGroup(new RDFPatternGroup()
                .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:cls3"))));
        RDFAskQueryResult result = await query.ApplyToOntologyAsync(ontology, reasoner);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.AskResult);
    }

    [TestMethod]
    public async Task ShouldApplyConstructQueryWithReasonerToOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls3"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls3")),
                    new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLClass(new RDFResource("ex:cls2"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLClass(new RDFResource("ex:cls3")))
            ],
            Rules = [
                new SWRLRule(
                    new RDFPlainLiteral("Example"),
                    new RDFPlainLiteral("Example"),
                    new SWRLAntecedent
                    {
                        Atoms = [
                            new SWRLClassAtom(
                                new OWLClass(new RDFResource("ex:cls3")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")))
                        ],
                        BuiltIns = [
                            SWRLBuiltIn.NotEqual(
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLIndividualArgument(new RDFResource("ex:idv3")))
                        ]
                    },
                    new SWRLConsequent
                    {
                        Atoms = [
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:owlsharp:hasInference")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLLiteralArgument(RDFTypedLiteral.True))
                        ]
                    })
            ]
        };
        OWLReasoner reasoner = new OWLReasoner
        {
            Rules = [
                OWLEnums.OWLReasonerRules.SubClassOfEntailment,
                OWLEnums.OWLReasonerRules.ClassAssertionEntailment
            ]
        };
        RDFConstructQuery query = new RDFConstructQuery()
            .AddPatternGroup(new RDFPatternGroup()
                .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:cls3"))))
            .AddTemplate(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:Individual")));
        RDFConstructQueryResult result = await query.ApplyToOntologyAsync(ontology, reasoner);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.ConstructResultsCount);
    }

    [TestMethod]
    public async Task ShouldApplyDescribeQueryWithReasonerToOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls3"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls3")),
                    new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLClass(new RDFResource("ex:cls2"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLClass(new RDFResource("ex:cls3")))
            ],
            Rules = [
                new SWRLRule(
                    new RDFPlainLiteral("Example"),
                    new RDFPlainLiteral("Example"),
                    new SWRLAntecedent
                    {
                        Atoms = [
                            new SWRLClassAtom(
                                new OWLClass(new RDFResource("ex:cls3")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")))
                        ],
                        BuiltIns = [
                            SWRLBuiltIn.NotEqual(
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLIndividualArgument(new RDFResource("ex:idv3")))
                        ]
                    },
                    new SWRLConsequent
                    {
                        Atoms = [
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:owlsharp:hasInference")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLLiteralArgument(RDFTypedLiteral.True))
                        ]
                    })
            ]
        };
        OWLReasoner reasoner = new OWLReasoner
        {
            Rules = [
                OWLEnums.OWLReasonerRules.SubClassOfEntailment,
                OWLEnums.OWLReasonerRules.ClassAssertionEntailment
            ]
        };
        RDFDescribeQuery query = new RDFDescribeQuery()
            .AddPatternGroup(new RDFPatternGroup()
                .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:cls3"))))
            .AddDescribeTerm(new RDFResource("ex:idv3"));
        RDFDescribeQueryResult result = await query.ApplyToOntologyAsync(ontology, reasoner);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.DescribeResultsCount);
    }

    [TestMethod]
    public async Task ShouldApplySelectQueryWithReasonerToOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls3"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls3")),
                    new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLClass(new RDFResource("ex:cls2"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLClass(new RDFResource("ex:cls3")))
            ],
            Rules = [
                new SWRLRule(
                    new RDFPlainLiteral("Example"),
                    new RDFPlainLiteral("Example"),
                    new SWRLAntecedent
                    {
                        Atoms = [
                            new SWRLClassAtom(
                                new OWLClass(new RDFResource("ex:cls3")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")))
                        ],
                        BuiltIns = [
                            SWRLBuiltIn.NotEqual(
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLIndividualArgument(new RDFResource("ex:idv3")))
                        ]
                    },
                    new SWRLConsequent
                    {
                        Atoms = [
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:owlsharp:hasInference")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLLiteralArgument(RDFTypedLiteral.True))
                        ]
                    })
            ]
        };
        OWLReasoner reasoner = new OWLReasoner
        {
            Rules = [
                OWLEnums.OWLReasonerRules.SubClassOfEntailment,
                OWLEnums.OWLReasonerRules.ClassAssertionEntailment
            ]
        };
        RDFSelectQuery query = new RDFSelectQuery()
            .AddPatternGroup(new RDFPatternGroup()
                .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:cls3"))));
        RDFSelectQueryResult result = await query.ApplyToOntologyAsync(ontology, reasoner);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.SelectResultsCount);
    }

    [TestMethod]
    public async Task ShouldApplySelectQueryWithoutReasonerToOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:cls3"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLNamedIndividual(new RDFResource("ex:idv1"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLNamedIndividual(new RDFResource("ex:idv2"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:cls3")),
                    new OWLNamedIndividual(new RDFResource("ex:idv3")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls1")),
                    new OWLClass(new RDFResource("ex:cls2"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:cls2")),
                    new OWLClass(new RDFResource("ex:cls3")))
            ],
            Rules = [
                new SWRLRule(
                    new RDFPlainLiteral("Example"),
                    new RDFPlainLiteral("Example"),
                    new SWRLAntecedent
                    {
                        Atoms = [
                            new SWRLClassAtom(
                                new OWLClass(new RDFResource("ex:cls3")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")))
                        ],
                        BuiltIns = [
                            SWRLBuiltIn.NotEqual(
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLIndividualArgument(new RDFResource("ex:idv3")))
                        ]
                    },
                    new SWRLConsequent
                    {
                        Atoms = [
                            new SWRLDataPropertyAtom(
                                new OWLDataProperty(new RDFResource("urn:owlsharp:hasInference")),
                                new SWRLVariableArgument(new RDFVariable("?IDV")),
                                new SWRLLiteralArgument(RDFTypedLiteral.True))
                        ]
                    })
            ]
        };
        RDFSelectQuery query = new RDFSelectQuery()
            .AddPatternGroup(new RDFPatternGroup()
                .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:cls3"))));
        RDFSelectQueryResult result = await query.ApplyToOntologyAsync(ontology);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.SelectResultsCount);
    }

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingAskQueryToOntologyBecauseNullQueryAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => (null as RDFAskQuery).ApplyToOntologyAsync(null));

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingAskQueryToOntologyBecauseNullOntologyAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => new RDFAskQuery().ApplyToOntologyAsync(null));

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingConstructQueryToOntologyBecauseNullQueryAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => (null as RDFConstructQuery).ApplyToOntologyAsync(null));

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingConstructQueryToOntologyBecauseNullOntologyAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => new RDFConstructQuery().ApplyToOntologyAsync(null));

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingDescribeQueryToOntologyBecauseNullQueryAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => (null as RDFDescribeQuery).ApplyToOntologyAsync(null));

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingDescribeQueryToOntologyBecauseNullOntologyAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => new RDFDescribeQuery().ApplyToOntologyAsync(null));

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingSelectQueryToOntologyBecauseNullQueryAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => (null as RDFSelectQuery).ApplyToOntologyAsync(null));

    [TestMethod]
    public async Task ShouldThrowExceptionOnApplyingSelectQueryToOntologyBecauseNullOntologyAsync()
        => await Assert.ThrowsExactlyAsync<OWLException>(() => new RDFSelectQuery().ApplyToOntologyAsync(null));
    #endregion
}