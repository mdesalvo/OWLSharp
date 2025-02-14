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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLAssertionAxiomHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetAssertionAxioms()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
                    new OWLDataPropertyAssertion(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                    new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLNamedIndividual(new RDFResource("ex:Carl")) ]),
                    new OWLNegativeDataPropertyAssertion(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                    new OWLNegativeObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
                    new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLNamedIndividual(new RDFResource("ex:Carl")) ])
                ]
            };

            List<OWLClassAssertion> classAssertion = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            Assert.AreEqual(1, classAssertion.Count);

            List<OWLDataPropertyAssertion> dataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            Assert.AreEqual(1, dataPropertyAssertion.Count);

            List<OWLDifferentIndividuals> differentIndividuals = ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>();
            Assert.AreEqual(1, differentIndividuals.Count);

            List<OWLNegativeDataPropertyAssertion> negativeDataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>();
            Assert.AreEqual(1, negativeDataPropertyAssertion.Count);

            List<OWLNegativeObjectPropertyAssertion> negativeObjectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>();
            Assert.AreEqual(1, negativeObjectPropertyAssertion.Count);

            List<OWLObjectPropertyAssertion> objectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            Assert.AreEqual(1, objectPropertyAssertion.Count);

            List<OWLSameIndividual> sameIndividual = ontology.GetAssertionAxiomsOfType<OWLSameIndividual>();
            Assert.AreEqual(1, sameIndividual.Count);

            Assert.AreEqual(0, (null as OWLOntology).GetAssertionAxiomsOfType<OWLClassAssertion>().Count);
        }

        [TestMethod]
        public void ShouldDeclareAssertionAxiom()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLNamedIndividual(new RDFResource("ex:Mark"))));

            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
            Assert.IsTrue(ontology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLNamedIndividual(new RDFResource("ex:Mark")))));
            Assert.ThrowsExactly<OWLException>(() => ontology.DeclareAssertionAxiom<OWLAssertionAxiom>(null));

            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLNamedIndividual(new RDFResource("ex:Mark")))); //will be discarded, since duplicates are not allowed
            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
        }

        [TestMethod]
        public void ShouldGetSameIndividuals()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv3")) ]),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4")) ]),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv5")) ])
                ]
            };

            List<OWLIndividualExpression> sameIndividualsOfIdv1 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")));
            Assert.AreEqual(4, sameIndividualsOfIdv1.Count);
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv4")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv4"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv4")), new OWLNamedIndividual(new RDFResource("ex:Idv2"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv5"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv5")), new OWLNamedIndividual(new RDFResource("ex:Idv2"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv4")), new OWLNamedIndividual(new RDFResource("ex:Idv5"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv5")), new OWLNamedIndividual(new RDFResource("ex:Idv4"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv5"))));
            Assert.IsTrue(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv5")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));

            List<OWLIndividualExpression> sameIndividualsOfIdv2 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv2")));
            Assert.AreEqual(4, sameIndividualsOfIdv2.Count);

            List<OWLIndividualExpression> sameIndividualsOfIdv3 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv3")));
            Assert.AreEqual(4, sameIndividualsOfIdv3.Count);

            List<OWLIndividualExpression> sameIndividualsOfIdv4 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv4")));
            Assert.AreEqual(4, sameIndividualsOfIdv4.Count);

            Assert.AreEqual(0, ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv6"))).Count);
            Assert.AreEqual(0, ontology.GetSameIndividuals(null).Count);
            Assert.IsFalse(ontology.CheckIsSameIndividual(null, new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsFalse(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv1")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2"))));
            Assert.AreEqual(0, (null as OWLOntology).GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1"))).Count);
        }

        [TestMethod]
        public void ShouldGetDifferentIndividuals()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv3")) ]),
                    new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4")) ]),
                    new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv5")) ])
                ]
            };

            List<OWLIndividualExpression> differentIndividualsOfIdv1 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")));
            Assert.AreEqual(3, differentIndividualsOfIdv1.Count);
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv3"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv3")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv4")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsFalse(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv5"))));
            Assert.IsFalse(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv5")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));

            List<OWLIndividualExpression> differentIndividualsOfIdv2 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv2")));
            Assert.AreEqual(3, differentIndividualsOfIdv2.Count);

            List<OWLIndividualExpression> differentIndividualsOfIdv3 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv3")));
            Assert.AreEqual(2, differentIndividualsOfIdv3.Count);

            List<OWLIndividualExpression> differentIndividualsOfIdv4 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv4")));
            Assert.AreEqual(1, differentIndividualsOfIdv4.Count);

            Assert.AreEqual(0, ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv6"))).Count);
            Assert.AreEqual(0, ontology.GetDifferentIndividuals(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1"))).Count);
        }

        [TestMethod]
        public void ShouldGetIndividualsOfSimpleClass()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Cat")), new OWLNamedIndividual(new RDFResource("ex:Felix"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Parrot")), new OWLNamedIndividual(new RDFResource("ex:Paco"))),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Paco")), new OWLNamedIndividual(new RDFResource("ex:PaquitoTheParrot")) ])
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Parrot")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Animal")), new OWLClass(new RDFResource("ex:LivingEntity"))),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:DomesticFeline")) ])
                ]
            };

            List<OWLIndividualExpression> cats = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")));
            Assert.AreEqual(1, cats.Count);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(ontology.CheckIsIndividualOf(new OWLClass(new RDFResource("ex:Cat")), new OWLNamedIndividual(new RDFResource("ex:Felix"))));

            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.AreEqual(1, domesticFelines.Count);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.AreEqual(2, parrots.Count);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));
            Assert.IsTrue(ontology.CheckIsIndividualOf(new OWLClass(new RDFResource("ex:Parrot")), new OWLNamedIndividual(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.AreEqual(3, animals.Count);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.AreEqual(3, livingEntities.Count);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfEnumerateClass()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Cat")), new OWLNamedIndividual(new RDFResource("ex:Felix"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Parrot")), new OWLNamedIndividual(new RDFResource("ex:Paco"))),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Paco")), new OWLNamedIndividual(new RDFResource("ex:PaquitoTheParrot")) ])
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Parrot")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Animal")), new OWLClass(new RDFResource("ex:LivingEntity"))),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:DomesticFeline")) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:MyAnimals")), new OWLObjectOneOf([ new OWLNamedIndividual(new RDFResource("ex:Felix")), new OWLNamedIndividual(new RDFResource("ex:Paco")) ]) ])
                ]
            };

            List<OWLIndividualExpression> cats = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")));
            Assert.AreEqual(1, cats.Count);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));

            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.AreEqual(1, domesticFelines.Count);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.AreEqual(2, parrots.Count);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.AreEqual(3, animals.Count);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.AreEqual(3, livingEntities.Count);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.AreEqual(3, myAnimals.Count);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfCompositeUnionClass()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:PersianCat")), new OWLNamedIndividual(new RDFResource("ex:PFelix"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Cat")), new OWLNamedIndividual(new RDFResource("ex:Felix"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Parrot")), new OWLNamedIndividual(new RDFResource("ex:Paco"))),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Paco")), new OWLNamedIndividual(new RDFResource("ex:PaquitoTheParrot")) ])
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Parrot")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:PersianCat")), new OWLClass(new RDFResource("ex:Cat"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Animal")), new OWLClass(new RDFResource("ex:LivingEntity"))),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:DomesticFeline")) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:MyAnimals")), new OWLObjectOneOf([ new OWLNamedIndividual(new RDFResource("ex:Felix")), new OWLNamedIndividual(new RDFResource("ex:Paco")) ]) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:ParrotOrCat")), new OWLObjectUnionOf([new OWLClass(new RDFResource("ex:Parrot")), new OWLClass(new RDFResource("ex:Cat"))]) ])
                ]
            };

            List<OWLIndividualExpression> cats = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")));
            Assert.AreEqual(2, cats.Count);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.AreEqual(2, domesticFelines.Count);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.AreEqual(2, parrots.Count);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.AreEqual(4, animals.Count);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.AreEqual(4, livingEntities.Count);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.AreEqual(3, myAnimals.Count);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> parrotsOrCats = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:ParrotOrCat")));
            Assert.AreEqual(4, parrotsOrCats.Count);
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfCompositeIntersectionClass()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:PersianCat")), new OWLNamedIndividual(new RDFResource("ex:PFelix"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Cat")), new OWLNamedIndividual(new RDFResource("ex:Felix"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Parrot")), new OWLNamedIndividual(new RDFResource("ex:Paco"))),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Paco")), new OWLNamedIndividual(new RDFResource("ex:PaquitoTheParrot")) ])
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Parrot")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:PersianCat")), new OWLClass(new RDFResource("ex:Cat"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Animal")), new OWLClass(new RDFResource("ex:LivingEntity"))),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:DomesticFeline")) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:MyAnimals")), new OWLObjectOneOf([ new OWLNamedIndividual(new RDFResource("ex:Felix")), new OWLNamedIndividual(new RDFResource("ex:Paco")) ]) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:ParrotAndMyAnymal")), new OWLObjectIntersectionOf([ new OWLClass(new RDFResource("ex:Parrot")), new OWLClass(new RDFResource("ex:MyAnimals")) ]) ])
                ]
            };

            List<OWLIndividualExpression> cats = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")));
            Assert.AreEqual(2, cats.Count);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.AreEqual(2, domesticFelines.Count);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.AreEqual(2, parrots.Count);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.AreEqual(4, animals.Count);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.AreEqual(4, livingEntities.Count);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.AreEqual(3, myAnimals.Count);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> parrotsAndMyAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:ParrotAndMyAnymal")));
            Assert.AreEqual(2, parrotsAndMyAnimals.Count);
            Assert.IsTrue(parrotsAndMyAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrotsAndMyAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfCompositeComplementClass()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:NotMyAnymal")), new OWLNamedIndividual(new RDFResource("ex:Fuffy"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Cat")), new OWLNamedIndividual(new RDFResource("ex:Felix"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Parrot")), new OWLNamedIndividual(new RDFResource("ex:Paco"))),
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Paco")), new OWLNamedIndividual(new RDFResource("ex:PaquitoTheParrot")) ])
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Parrot")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:PersianCat")), new OWLClass(new RDFResource("ex:Cat"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Animal")), new OWLClass(new RDFResource("ex:LivingEntity"))),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:DomesticFeline")) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:MyAnimals")), new OWLObjectOneOf([ new OWLNamedIndividual(new RDFResource("ex:Felix")), new OWLNamedIndividual(new RDFResource("ex:Paco")) ]) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:NotMyAnymal")), new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:MyAnimals"))) ])
                ]
            };

            List<OWLIndividualExpression> cats = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")));
            Assert.AreEqual(1, cats.Count);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            
            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.AreEqual(1, domesticFelines.Count);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            
            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.AreEqual(2, parrots.Count);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.AreEqual(3, animals.Count);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.AreEqual(3, livingEntities.Count);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.AreEqual(3, myAnimals.Count);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> notMyAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:NotMyAnymal")));
            Assert.AreEqual(1, notMyAnimals.Count);
            Assert.IsTrue(notMyAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Fuffy"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasValueRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasCat")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Felix")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:OwnersOfFelix")),
                        new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:hasCat")),new OWLNamedIndividual(new RDFResource("ex:Felix"))) ])
                ]
            };

            List<OWLIndividualExpression> ownersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:OwnersOfFelix")));
            Assert.AreEqual(1, ownersOfFelix.Count);
            Assert.IsTrue(ownersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasValueRestrictionWithInverseAssertion()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasCat"))), 
                        new OWLNamedIndividual(new RDFResource("ex:Felix")), 
                        new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:OwnersOfFelix")),
                        new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:hasCat")),new OWLNamedIndividual(new RDFResource("ex:Felix"))) ])
                ]
            };

            List<OWLIndividualExpression> ownersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:OwnersOfFelix")));
            Assert.AreEqual(1, ownersOfFelix.Count);
            Assert.IsTrue(ownersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasValueRestrictionWithInverseRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasCat")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Felix")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:InverseOwnersOfFelix")),
                        new OWLObjectHasValue(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasCat"))),new OWLNamedIndividual(new RDFResource("ex:Mark"))) ])
                ]
            };
            string ont = OWLSerializer.SerializeObject<OWLOntology>(ontology);
            List<OWLIndividualExpression> inverseOwnersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:InverseOwnersOfFelix")));
            Assert.AreEqual(1, inverseOwnersOfFelix.Count);
            Assert.IsTrue(inverseOwnersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasValueRestrictionWithInverseAssertionAndInverseRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasCat"))),
                        new OWLNamedIndividual(new RDFResource("ex:Felix")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:DoubleInverseOwnersOfFelix")),
                        new OWLObjectHasValue(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasCat"))),new OWLNamedIndividual(new RDFResource("ex:Mark"))) ])
                ]
            };
            string ont = OWLSerializer.SerializeObject<OWLOntology>(ontology);
            List<OWLIndividualExpression> doubleinverseOwnersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DoubleInverseOwnersOfFelix")));
            Assert.AreEqual(1, doubleinverseOwnersOfFelix.Count);
            Assert.IsTrue(doubleinverseOwnersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasSelfRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:SelfFriends")),
                        new OWLObjectHasSelf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))) ])
                ]
            };

            List<OWLIndividualExpression> ownersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:SelfFriends")));
            Assert.AreEqual(1, ownersOfFelix.Count);
            Assert.IsTrue(ownersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasSelfRestrictionWithInverseAssertion()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:SelfFriends")),
                        new OWLObjectHasSelf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))) ])
                ]
            };

            List<OWLIndividualExpression> ownersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:SelfFriends")));
            Assert.AreEqual(1, ownersOfFelix.Count);
            Assert.IsTrue(ownersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasSelfRestrictionWithInverseRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:SelfFriends")),
                        new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend")))) ])
                ]
            };

            List<OWLIndividualExpression> ownersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:SelfFriends")));
            Assert.AreEqual(1, ownersOfFelix.Count);
            Assert.IsTrue(ownersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectHasSelfRestrictionWithInverseAssertionAndInverseRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:SelfFriends")),
                        new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend")))) ])
                ]
            };

            List<OWLIndividualExpression> ownersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:SelfFriends")));
            Assert.AreEqual(1, ownersOfFelix.Count);
            Assert.IsTrue(ownersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectMinCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:John")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:HasAtLeast2Friends")),
                        new OWLObjectMinCardinality(new OWLObjectProperty(new RDFResource("ex:hasFriend")), 2) ])
                ]
            };

            List<OWLIndividualExpression> havingAtLeast2Friends = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:HasAtLeast2Friends")));
            Assert.AreEqual(1, havingAtLeast2Friends.Count);
            Assert.IsTrue(havingAtLeast2Friends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectMinCardinalityRestrictionWithInverseAssertion()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:John")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:HasAtLeast2Friends")),
                        new OWLObjectMinCardinality(new OWLObjectProperty(new RDFResource("ex:hasFriend")), 2) ])
                ]
            };

            List<OWLIndividualExpression> havingAtLeast2Friends = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:HasAtLeast2Friends")));
            Assert.AreEqual(1, havingAtLeast2Friends.Count);
            Assert.IsTrue(havingAtLeast2Friends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectMinCardinalityRestrictionWithInverseRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:John")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:John")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:HasAtLeast2Friends")),
                        new OWLObjectMinCardinality(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))), 2) ])
                ]
            };

            List<OWLIndividualExpression> havingAtLeast2Friends = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:HasAtLeast2Friends")));
            Assert.AreEqual(1, havingAtLeast2Friends.Count);
            Assert.IsTrue(havingAtLeast2Friends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectSomeValuesFromRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:John")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Human")), 
                                          new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Human")),
                                          new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Man")),
                                          new OWLNamedIndividual(new RDFResource("ex:John")))
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Man")), new OWLClass(new RDFResource("ex:Human"))),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:HasSomeHumanFriends")),
                        new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasFriend")), new OWLClass(new RDFResource("ex:Human"))) ])
                ]
            };

            List<OWLIndividualExpression> havingSomeHumanFriends = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:HasSomeHumanFriends")));
            Assert.AreEqual(2, havingSomeHumanFriends.Count);
            Assert.IsTrue(havingSomeHumanFriends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
            Assert.IsTrue(havingSomeHumanFriends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:John"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectSomeValuesFromRestrictionWithInverseAssertion()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:John")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Human")),
                                          new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Human")),
                                          new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Man")),
                                          new OWLNamedIndividual(new RDFResource("ex:John")))
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Man")), new OWLClass(new RDFResource("ex:Human"))),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:HasSomeHumanFriends")),
                        new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasFriend")), new OWLClass(new RDFResource("ex:Human"))) ])
                ]
            };

            List<OWLIndividualExpression> havingSomeHumanFriends = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:HasSomeHumanFriends")));
            Assert.AreEqual(2, havingSomeHumanFriends.Count);
            Assert.IsTrue(havingSomeHumanFriends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
            Assert.IsTrue(havingSomeHumanFriends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:John"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfObjectSomeValuesFromRestrictionWithInverseRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:John")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:hasFriend")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Human")),
                                          new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Human")),
                                          new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:Man")),
                                          new OWLNamedIndividual(new RDFResource("ex:John")))
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Man")), new OWLClass(new RDFResource("ex:Human"))),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:HasSomeHumanFriends")),
                        new OWLObjectSomeValuesFrom(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasFriend"))), new OWLClass(new RDFResource("ex:Human"))) ])
                ]
            };

            List<OWLIndividualExpression> havingSomeHumanFriends = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:HasSomeHumanFriends")));
            Assert.AreEqual(2, havingSomeHumanFriends.Count);
            Assert.IsTrue(havingSomeHumanFriends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
            Assert.IsTrue(havingSomeHumanFriends.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:John"))));
        }

        [TestMethod]
        public void ShouldGetIndividualsOfDataHasValueRestriction()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("ex:hasAge")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("ex:Having25Years")),
                        new OWLDataHasValue(new OWLDataProperty(new RDFResource("ex:hasAge")),new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))) ])
                ]
            };

            List<OWLIndividualExpression> ownersOfFelix = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Having25Years")));
            Assert.AreEqual(1, ownersOfFelix.Count);
            Assert.IsTrue(ownersOfFelix.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Mark"))));
        }

        [TestMethod]
        public void ShouldCheckIsNegativeIndividualOf()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(new RDFResource("ex:NotMyAnymal")), new OWLNamedIndividual(new RDFResource("ex:Fuffy"))),
                    new OWLClassAssertion(new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cat"))), new OWLNamedIndividual(new RDFResource("ex:Snoopy")))
                ],
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:Animal"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:PersianCat")), new OWLClass(new RDFResource("ex:Cat"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Animal")), new OWLClass(new RDFResource("ex:LivingEntity"))),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:DomesticFeline")) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:MyAnimals")), new OWLObjectOneOf([ new OWLNamedIndividual(new RDFResource("ex:Fuffy")), new OWLNamedIndividual(new RDFResource("ex:Felix")), new OWLNamedIndividual(new RDFResource("ex:Paco")) ]) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:NotMyAnymal")), new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:MyAnimals"))) ])
                ]
            };

            Assert.IsTrue(ontology.CheckIsNegativeIndividualOf(new OWLClass(new RDFResource("ex:MyAnimals")), new OWLNamedIndividual(new RDFResource("ex:Fuffy")))); //There is a subtle contradiction in this, but it will be detected during ontology validation: Fuffy is ex:NotMyAnimal but is also ex:MyAnimals which is its complement...
            Assert.IsTrue(ontology.CheckIsNegativeIndividualOf(new OWLClass(new RDFResource("ex:Cat")), new OWLNamedIndividual(new RDFResource("ex:Snoopy"))));
            Assert.IsFalse(ontology.CheckIsNegativeIndividualOf(new OWLClass(new RDFResource("ex:Animal")), new OWLNamedIndividual(new RDFResource("ex:Snoopy"))));
        }

        [TestMethod]
        public void ShouldCheckIsLiteralOf()
        {
            Assert.IsFalse((null as OWLOntology).CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDFS.LITERAL), new OWLLiteral(new RDFPlainLiteral("hello"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(null, new OWLLiteral(new RDFPlainLiteral("hello"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDFS.LITERAL), null));

            //Datatype
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDFS.LITERAL), new OWLLiteral(new RDFPlainLiteral("hello"))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDFS.LITERAL), new OWLLiteral(new RDFPlainLiteral("hello","en-US"))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDFS.LITERAL), new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDF.LANG_STRING), new OWLLiteral(new RDFPlainLiteral("hello", "en-US"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDF.LANG_STRING), new OWLLiteral(new RDFPlainLiteral("hello"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDF.LANG_STRING), new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDF.PLAIN_LITERAL), new OWLLiteral(new RDFPlainLiteral("hello"))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDF.PLAIN_LITERAL), new OWLLiteral(new RDFPlainLiteral("hello", "en-US"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.RDF.PLAIN_LITERAL), new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.XSD.DOUBLE), new OWLLiteral(new RDFPlainLiteral("hello"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.XSD.DOUBLE), new OWLLiteral(new RDFPlainLiteral("hello", "en-US"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.XSD.DOUBLE), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDatatype(RDFVocabulary.XSD.DOUBLE), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));
            //Enumerate
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDataOneOf([ 
                new OWLLiteral(new RDFPlainLiteral("hello")), 
                new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]), new OWLLiteral(new RDFPlainLiteral("hello"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDataOneOf([ 
                new OWLLiteral(new RDFPlainLiteral("hello")), 
                new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]), new OWLLiteral(new RDFPlainLiteral("hey"))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDataOneOf([ 
                new OWLLiteral(new RDFPlainLiteral("hello")), 
                new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]), new OWLLiteral(new RDFPlainLiteral("hi", "en-US"))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDataOneOf([ 
                new OWLLiteral(new RDFPlainLiteral("hello")), 
                new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]), new OWLLiteral(new RDFPlainLiteral("hi", "en-UK"))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDataOneOf([ 
                new OWLLiteral(new RDFPlainLiteral("hello")), 
                new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDataOneOf([ 
                new OWLLiteral(new RDFPlainLiteral("hello")), 
                new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INT))));
            //Composite
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDataUnionOf([
                new OWLDataOneOf([ 
                    new OWLLiteral(new RDFPlainLiteral("hello")), 
                    new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                    new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]),
                new OWLDatatype(RDFVocabulary.XSD.BOOLEAN) ]), new OWLLiteral(RDFTypedLiteral.False)));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDataUnionOf([
                new OWLDataOneOf([ 
                    new OWLLiteral(new RDFPlainLiteral("hello")), 
                    new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                    new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]),
                new OWLDatatype(RDFVocabulary.XSD.BOOLEAN) ]), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_FLOAT))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDataIntersectionOf([
                new OWLDataOneOf([ 
                    new OWLLiteral(new RDFPlainLiteral("hello")), 
                    new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                    new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]),
                new OWLDatatype(RDFVocabulary.XSD.INTEGER) ]), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDataIntersectionOf([
                new OWLDataOneOf([ 
                    new OWLLiteral(new RDFPlainLiteral("hello")), 
                    new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                    new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]),
                new OWLDatatype(RDFVocabulary.XSD.INTEGER) ]), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_FLOAT))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDataIntersectionOf([
                new OWLDataOneOf([ 
                    new OWLLiteral(new RDFPlainLiteral("hello")), 
                    new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                    new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ]),
                new OWLDatatype(RDFVocabulary.XSD.INTEGER) ]), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(new OWLDataComplementOf(
                new OWLDataOneOf([ 
                    new OWLLiteral(new RDFPlainLiteral("hello")), 
                    new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                    new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ])), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_FLOAT))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(new OWLDataComplementOf(
                new OWLDataOneOf([ 
                    new OWLLiteral(new RDFPlainLiteral("hello")), 
                    new OWLLiteral(new RDFPlainLiteral("hi", "en-US")), 
                    new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)) ])), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
            //DatatypeRestriction
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_LENGTH),
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MAX_LENGTH)
                    ]),
                new OWLLiteral(new RDFTypedLiteral("abcdef", RDFModelEnums.RDFDatatypes.XSD_STRING))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.LENGTH)
                    ]),
                new OWLLiteral(new RDFTypedLiteral("abc", RDFModelEnums.RDFDatatypes.XSD_STRING))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("^a", RDFModelEnums.RDFDatatypes.XSD_STRING)), RDFVocabulary.XSD.PATTERN)
                    ]),
                new OWLLiteral(new RDFTypedLiteral("abc", RDFModelEnums.RDFDatatypes.XSD_STRING))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING), null),
                new OWLLiteral(new RDFTypedLiteral("abc", RDFModelEnums.RDFDatatypes.XSD_STRING))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.LENGTH)
                    ]),
                new OWLLiteral(new RDFPlainLiteral("abc")))); //Plain literals have no chances against datatype restrictions (even if virtually compatible)
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.DOUBLE),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_EXCLUSIVE),
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MAX_EXCLUSIVE)
                    ]),
                new OWLLiteral(new RDFTypedLiteral("8.35", RDFModelEnums.RDFDatatypes.XSD_FLOAT))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.DOUBLE),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_EXCLUSIVE),
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MAX_EXCLUSIVE)
                    ]),
                new OWLLiteral(new RDFTypedLiteral("6.0", RDFModelEnums.RDFDatatypes.XSD_FLOAT))));
            Assert.IsTrue(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.DOUBLE),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_INCLUSIVE),
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MAX_INCLUSIVE)
                    ]),
                new OWLLiteral(new RDFTypedLiteral("8.35", RDFModelEnums.RDFDatatypes.XSD_FLOAT))));
            Assert.IsFalse(new OWLOntology().CheckIsLiteralOf(
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.FLOAT),
                    [ 
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_INCLUSIVE),
                        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MAX_INCLUSIVE)
                    ]),
                new OWLLiteral(new RDFTypedLiteral("5.99", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));
        }
        #endregion
    }
}