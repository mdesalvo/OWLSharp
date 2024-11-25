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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Validator;
using OWLSharp.Validator.RuleSet;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator.RuleSet
{
    [TestClass]
    public class OWLSubClassOfAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeSubClassOfSubClassOfCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg1)));
        }

		[TestMethod]
        public void ShouldAnalyzeSubClassOfEquivalentClassesCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg1)));
        }

		[TestMethod]
        public void ShouldAnalyzeSubClassOfDisjointClassesCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg1)));
        }
        
        [TestMethod]
        public void ShouldAnalyzeSubClassOfExactObjectCardinalityCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg2)));
        }

        [TestMethod]
        public void ShouldAnalyzeSubClassOfQUalifiedExactObjectCardinalityCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg2)));
        }

        [TestMethod]
        public void ShouldAnalyzeSubClassOfMaxObjectCardinalityCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg2)));
        }

        [TestMethod]
        public void ShouldAnalyzeSubClassOfQUalifiedMaxObjectCardinalityCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg2)));
        }

        [TestMethod]
        public void ShouldAnalyzeSubClassOfExactDataCardinalityCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg3)));
        }

        [TestMethod]
        public void ShouldAnalyzeSubClassOfQualifiedExactDataCardinalityCase()
        {
            OWLDatatypeRestriction length6to10Facet = new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]);

            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg3)));
        }

        [TestMethod]
        public void ShouldAnalyzeSubClassOfMaxDataCardinalityCase()
        {
            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg3)));
        }

        [TestMethod]
        public void ShouldAnalyzeSubClassOfQualifiedMaxDataCardinalityCase()
        {
            OWLDatatypeRestriction length6to10Facet = new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]);

            OWLOntology ontology = new OWLOntology()
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
            List<OWLIssue> issues = OWLSubClassOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg3)));
        }
        #endregion
    }
}