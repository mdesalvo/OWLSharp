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
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysisRule.rulesugg)));
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
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLIrreflexiveObjectPropertyAnalysisRule.rulesugg)));
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
        #endregion
    }
}