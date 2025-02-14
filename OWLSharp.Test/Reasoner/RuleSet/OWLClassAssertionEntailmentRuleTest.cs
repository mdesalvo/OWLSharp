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
    public class OWLClassAssertionEntailmentRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldEntailClassAssertionOnSimpleSubClassOfCase()
        {
            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Feline"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ],
                ClassAxioms = [ 
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Feline"))),
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Feline")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ]
            };
            List<OWLInference> inferences = OWLClassAssertionEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1 
                            && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Feline")
                            && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        }

        [TestMethod]
        public void ShouldEntailClassAssertionOnSimpleEquivalentClassesCase()
        {
            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ],
                ClassAxioms = [ 
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))]),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))])
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ]
            };
            List<OWLInference> inferences = OWLClassAssertionEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MewAnimal")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1 
                            && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                            && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        }

        [TestMethod]
        public void ShouldEntailClassAssertionOnSubClassOfHavingEquivalentClassCase()
        {
            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ],
                ClassAxioms = [ 
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))])
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ]
            };
            List<OWLInference> inferences = OWLClassAssertionEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MewAnimal")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1 
                            && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                            && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        }

        [TestMethod]
        public void ShouldEntailClassAssertionOnEquivalentClassHavingSubClassOfCase()
        {
            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ],
                ClassAxioms = [ 
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))])
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ]
            };
            List<OWLInference> inferences = OWLClassAssertionEntailmentRule.ExecuteRule(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MewAnimal")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1 
                            && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                            && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        }

        [TestMethod]
        public void ShouldEntailClassAssertionOnComplexTBOXCase()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")), 
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix"))),
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Parrot")), 
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Paco"))),
                    new OWLSameIndividual([ 
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Paco")), 
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/PaquitoTheParrot")) ])
                ],
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Parrot")), 
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")), 
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")), 
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/LivingEntity"))),
                    new OWLEquivalentClasses([ 
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")), 
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline")) ]),
                    new OWLEquivalentClasses([ 
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MyAnimals")), 
                        new OWLObjectOneOf([ 
                            new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")), 
                            new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Paco")) ]) ])
                ]
            };
            List<OWLInference> inferences = OWLClassAssertionEntailmentRule.ExecuteRule(ontology);
            
            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MyAnimals")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Paco")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Paco")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf 
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MyAnimals")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Paco")));
        }
        #endregion
    }
}