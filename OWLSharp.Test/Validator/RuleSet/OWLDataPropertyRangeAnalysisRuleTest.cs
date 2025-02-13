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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator
{
    [TestClass]
    public class OWLDataPropertyRangeAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeDataPropertyRangeViolatingDatatype()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))), //clashes with range of ex:dp1
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLLiteral(new RDFTypedLiteral("43", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                ],
                DataPropertyAxioms = [
                    new OWLDataPropertyRange(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLDatatype(RDFVocabulary.XSD.INTEGER))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyRangeAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDataPropertyRangeViolatingDataIntersectionOf()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("Z"))), //clashes with range of ex:dp1
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("A"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLLiteral(new RDFPlainLiteral("C")))
                ],
                DataPropertyAxioms = [
                    new OWLDataPropertyRange(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLDataIntersectionOf([
                            new OWLDataOneOf([
                                new OWLLiteral(new RDFPlainLiteral("A")), 
                                new OWLLiteral(new RDFPlainLiteral("B")),
                                new OWLLiteral(new RDFPlainLiteral("C")) ]),
                            new OWLDataOneOf([
                                new OWLLiteral(new RDFPlainLiteral("A")), 
                                new OWLLiteral(new RDFPlainLiteral("C")) ])    ]))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyRangeAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDataPropertyRangeViolatingDataOneOf()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("Z"))), //clashes with range of ex:dp1
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("A"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLLiteral(new RDFPlainLiteral("B")))
                ],
                DataPropertyAxioms = [
                    new OWLDataPropertyRange(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLDataOneOf([
                            new OWLLiteral(new RDFPlainLiteral("A")), 
                            new OWLLiteral(new RDFPlainLiteral("B")) ]))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyRangeAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDataPropertyRangeViolatingDataUnionOf()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("Z"))), //clashes with range of ex:dp1
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("A"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLLiteral(new RDFPlainLiteral("C")))
                ],
                DataPropertyAxioms = [
                    new OWLDataPropertyRange(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLDataUnionOf([
                            new OWLDataOneOf([
                                new OWLLiteral(new RDFPlainLiteral("A")), 
                                new OWLLiteral(new RDFPlainLiteral("B")) ]),
                            new OWLDataOneOf([
                                new OWLLiteral(new RDFPlainLiteral("C")), 
                                new OWLLiteral(new RDFPlainLiteral("D")) ])    ]))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyRangeAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDataPropertyRangeViolatingDataComplementOf()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("A"))), //clashes with range of ex:dp1
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("Z"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLLiteral(new RDFPlainLiteral("Q")))
                ],
                DataPropertyAxioms = [
                    new OWLDataPropertyRange(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLDataComplementOf(
                            new OWLDataOneOf([
                                new OWLLiteral(new RDFPlainLiteral("A")), 
                                new OWLLiteral(new RDFPlainLiteral("B")),
                                new OWLLiteral(new RDFPlainLiteral("C")) ])))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyRangeAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDataPropertyRangeViolatingDatatypeRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFTypedLiteral("Mark1", RDFModelEnums.RDFDatatypes.XSD_STRING))), //clashes with range of ex:dp1
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFTypedLiteral("Stiv17899", RDFModelEnums.RDFDatatypes.XSD_STRING))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLLiteral(new RDFPlainLiteral("lit"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLLiteral(new RDFTypedLiteral("Helen6", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                ],
                DatatypeDefinitionAxioms = [
                    new OWLDatatypeDefinition(
                        new OWLDatatype(new RDFResource("ex:length6to10")),
                        new OWLDatatypeRestriction(
                            new OWLDatatype(RDFVocabulary.XSD.STRING),
                            [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                              new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH) ]))
                ],
                DataPropertyAxioms = [
                    new OWLDataPropertyRange(
                        new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLDatatypeRestriction(
                            new OWLDatatype(RDFVocabulary.XSD.STRING),
                            [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                              new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH) ]))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyRangeAnalysisRule.rulesugg)));
        }
        #endregion
    }
}