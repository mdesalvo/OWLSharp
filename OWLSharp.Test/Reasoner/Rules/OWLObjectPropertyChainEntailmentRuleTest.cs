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
using OWLSharp.Reasoner;
using OWLSharp.Reasoner.Rules;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner.Rules
{
    [TestClass]
    public class OWLObjectPropertyChainEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailSimpleObjectPropertyChainCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectPropertyChain([
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")) ]),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Aebe")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jish")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fritz")));
        }

		[TestMethod]
        public void ShouldEntailSimpleObjectPropertyChainWithAnonymousIndividualsCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectPropertyChain([
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")) ]),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLAnonymousIndividual("Aebe"),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLAnonymousIndividual("John")),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "bnode:Aebe")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "bnode:John")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jish")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fritz")));
        }

		[TestMethod]
        public void ShouldEntailObjectPropertyChainWithInverseStepCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectPropertyChain([
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
							new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother"))) ]),
						new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Aebe")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jish")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fritz")));
        }

		[TestMethod]
        public void ShouldEntailObjectPropertyChainWithInverseSuperObjectPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectPropertyChain([
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")) ]),
						new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Aebe")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fritz")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jish")));
        }

		[TestMethod]
        public void ShouldEntailObjectPropertyChainWithInverseSTepCaseAndInverseSuperObjectPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLSubObjectPropertyOf(
						new OWLObjectPropertyChain([
							new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
							new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother"))) ]),
						new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
					new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyChainEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Aebe")));
			Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
							&& string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fritz")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jish")));
        }
        #endregion
    }
}