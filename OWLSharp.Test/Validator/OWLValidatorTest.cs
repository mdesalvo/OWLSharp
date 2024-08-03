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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Validator;
using OWLSharp.Validator.Rules;
using RDFSharp.Model;

namespace OWLSharp.Test.Validator
{
    [TestClass]
    public class OWLValidatorTest
    {
        #region Tests
        [TestMethod]
        public async Task ShouldAnalyzeAsymmetricObjectPropertyAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
					new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [ OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis, OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysisRule.rulesugg2)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeIrreflexiveObjectPropertyAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
					new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/friendOf")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [ OWLEnums.OWLValidatorRules.IrreflexiveObjectPropertyAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLIrreflexiveObjectPropertyAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysisRule.rulesugg2)));
        }
        
		[TestMethod]
        public async Task ShouldAnalyzeDeprecatedTermsAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				AnnotationAxioms = [
					new OWLAnnotationAssertion(
						new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
						new RDFResource("http://xmlns.com/foaf/0.1/Person"),
						new OWLLiteral(RDFTypedLiteral.True))
				],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person"))),
					new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Organization")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [ OWLEnums.OWLValidatorRules.TermsDeprecationAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDeprecationAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected presence of deprecated class with IRI: 'http://xmlns.com/foaf/0.1/Person'")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDeprecationAnalysisRule.rulesugg)));
        }
		
		[TestMethod]
        public async Task ShouldAnalyzeDisjointTermsAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person"))),
					new OWLDeclaration(new OWLDatatype(new RDFResource("http://xmlns.com/foaf/0.1/Person")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [ OWLEnums.OWLValidatorRules.TermsDisjointnessAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDisjointnessAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected clash on terms disjointness for class with IRI: 'http://xmlns.com/foaf/0.1/Person'")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDisjointnessAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public async Task ShouldAnalyzeThingNothingAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(RDFVocabulary.OWL.THING),
                        new OWLClass(RDFVocabulary.FOAF.PERSON))
                ],
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.OWL.THING)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.ThingNothingAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLThingNothingAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected class axioms causing reserved owl:Thing class to not be the root entity of the ontology")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLThingNothingAnalysisRule.rulesuggT1)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeTopBottomAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY),
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY)),
					new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.TopBottomAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected object property axioms causing reserved owl:topObjectProperty property to not be the root object property of the ontology")));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysisRule.rulesuggT1)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeDifferentIndividualsAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))),
                ],
                AssertionAxioms = [
                    new OWLDifferentIndividuals([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))]),
					new OWLDifferentIndividuals([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))]),
					new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))]),
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.DifferentIndividualsAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDifferentIndividualsAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDifferentIndividualsAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeNegativeDataAssertionsAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
                ],
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                        new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLNegativeDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.NegativeDataAssertionsAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLNegativeDataAssertionsAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLNegativeDataAssertionsAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeNegativeObjectAssertionsAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
					new OWLNegativeObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.NegativeObjectAssertionsAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLNegativeObjectAssertionsAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLNegativeObjectAssertionsAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeDisjointUnionAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				AssertionAxioms = [
					new OWLClassAssertion(
						new OWLClass(RDFVocabulary.FOAF.PERSON),
						new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLClassAssertion(
						new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
						new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLClassAssertion(
						new OWLClass(RDFVocabulary.FOAF.PERSON),
						new OWLNamedIndividual(new RDFResource("ex:John"))),
				],
                ClassAxioms = [
                    new OWLDisjointUnion(
                        new OWLClass(RDFVocabulary.FOAF.AGENT),
                         [ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
					new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.DisjointUnionAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointUnionAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointUnionAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeDisjointClassesAsync()
        {
            OWLOntology ontology = new OWLOntology()
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
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.DisjointClassesAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeObjectPropertyChainAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
				],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectPropertyChain([
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
					new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
				]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.ObjectPropertyChainAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeEquivalentClassesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				ClassAxioms = [
                    new OWLEquivalentClasses([
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
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.EquivalentClassesAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLEquivalentClassesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLEquivalentClassesAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeSubClassOfSubClassAsync()
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
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.SubClassOfAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubClassOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubClassOfAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeDisjointDataPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				AssertionAxioms = [
					new OWLDataPropertyAssertion(
						new OWLDataProperty(RDFVocabulary.FOAF.AGE),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(RDFVocabulary.FOAF.AGE),
						new OWLNamedIndividual(new RDFResource("ex:John")),
						new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(RDFVocabulary.FOAF.AGE),
						new OWLNamedIndividual(new RDFResource("ex:Stiv")),
						new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(RDFVocabulary.FOAF.NAME),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLLiteral(new RDFPlainLiteral("Mark"))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(RDFVocabulary.FOAF.NAME),
						new OWLNamedIndividual(new RDFResource("ex:John")),
						new OWLLiteral(new RDFPlainLiteral("John"))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:age")),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))) //conflicts with foaf:age
				],
				DataPropertyAxioms = [
                    new OWLDisjointDataProperties([
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
						new OWLDataProperty(RDFVocabulary.FOAF.NAME), 
						new OWLDataProperty(new RDFResource("ex:age")) ]),
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
					new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME)),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.DisjointDataPropertiesAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);


            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointDataPropertiesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointDataPropertiesAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeDisjointObjectPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				AssertionAxioms = [
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:John"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
						new OWLNamedIndividual(new RDFResource("ex:John")),
						new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
						new OWLNamedIndividual(new RDFResource("ex:Stiv")),
						new OWLNamedIndividual(new RDFResource("ex:Lenny"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:John"))), //conflicts with foaf:knows
				],
				ObjectPropertyAxioms = [
                    new OWLDisjointObjectProperties([
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(RDFVocabulary.FOAF.AGENT) ]),
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.AGENT)),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Lenny")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.DisjointObjectPropertiesAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointObjectPropertiesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointObjectPropertiesAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeEquivalentDataPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				DataPropertyAxioms = [
                    new OWLEquivalentDataProperties([
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
						new OWLDataProperty(new RDFResource("ex:age")) ]),
					new OWLSubDataPropertyOf(
						new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
						new OWLDataProperty(new RDFResource("ex:age")))
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.EquivalentDataPropertiesAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLEquivalentDataPropertiesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLEquivalentDataPropertiesAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public async Task ShouldAnalyzeEquivalentObjectPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
				ObjectPropertyAxioms = [
                    new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(new RDFResource("ex:knows")) ]),
					new OWLSubObjectPropertyOf(
						new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(new RDFResource("ex:knows")))
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows")))
                ]
            };
            OWLValidator validator = new OWLValidator() { Rules = [OWLEnums.OWLValidatorRules.EquivalentObjectPropertiesAnalysis] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLEquivalentObjectPropertiesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLEquivalentObjectPropertiesAnalysisRule.rulesugg)));
        }
        #endregion
    }
}