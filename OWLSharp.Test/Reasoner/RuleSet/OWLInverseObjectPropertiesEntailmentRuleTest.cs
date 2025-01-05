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
using OWLSharp.Reasoner;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner
{
    [TestClass]
    public class OWLInverseObjectPropertiesEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailSimpleInverseObjectPropertiesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLInverseObjectProperties(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                                                   new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            List<OWLInference> inferences = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public void ShouldEntailInverseObjectPropertiesWithInverseObjectAssertionCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [ 
                    new OWLInverseObjectProperties(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                                                   new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            List<OWLInference> inferences = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf 
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1 
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2 
                            && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")
                            && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public void ShouldEntailInverseObjectPropertiesWithInverseObjectPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLInverseObjectProperties(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                                                   new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            List<OWLInference> inferences = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")));
        }

        [TestMethod]
        public void ShouldEntailInverseObjectPropertiesWithInverseObjectPropertyAndInverseObjectAssertionCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLInverseObjectProperties(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                                                   new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            List<OWLInference> inferences = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2
                            && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")));
        }
        
        [TestMethod]
        public void ShouldEntailInverseObjectPropertiesViaEquivalentObjectPropertiesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFriendOf")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLInverseObjectProperties(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                                                   new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy"))),
                    new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFriendOf"))
                    ])
                ]
            };
            List<OWLInference> inferences = OWLInverseObjectPropertiesEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLInverseObjectProperties inf 
                            && string.Equals(inf.LeftObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf.RightObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isFriendOf")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLInverseObjectProperties inf1 
                            && string.Equals(inf1.LeftObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isFriendOf")
                            && string.Equals(inf1.RightObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
        }
        #endregion
    }
}