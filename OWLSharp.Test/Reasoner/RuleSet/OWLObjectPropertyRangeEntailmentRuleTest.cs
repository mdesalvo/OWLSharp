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
    public class OWLObjectPropertyRangeEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailSimpleObjectPropertyRangeCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLObjectPropertyRange(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine"))
                    )
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyRangeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Janine")));
        }

        [TestMethod]
        public void ShouldEntailInverseObjectPropertyRangeCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLObjectPropertyRange(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine"))
                    )
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyRangeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Kelly")));
        }

        [TestMethod]
        public void ShouldEntailInverseObjectPropertyAssertionRangeCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLObjectPropertyRange(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine"))
                    )
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyRangeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Kelly")));
        }

        [TestMethod]
        public void ShouldEntailInverseObjectPropertyAssertionInverseRangeCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine")))
                ],
                ObjectPropertyAxioms = [ 
                    new OWLObjectPropertyRange(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine"))
                    )
                ]
            };
            List<OWLInference> inferences = OWLObjectPropertyRangeEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Janine")));
        }
        #endregion
    }
}