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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Helpers
{
    [TestClass]
    public class OWLAssertionAxiomHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldGetAssertionAxioms()
		{
			OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(classAssertion.Count == 1);

			List<OWLDataPropertyAssertion> dataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            Assert.IsTrue(dataPropertyAssertion.Count == 1);

			List<OWLDifferentIndividuals> differentIndividuals = ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>();
            Assert.IsTrue(differentIndividuals.Count == 1);

			List<OWLNegativeDataPropertyAssertion> negativeDataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>();
            Assert.IsTrue(negativeDataPropertyAssertion.Count == 1);

			List<OWLNegativeObjectPropertyAssertion> negativeObjectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>();
            Assert.IsTrue(negativeObjectPropertyAssertion.Count == 1);

			List<OWLObjectPropertyAssertion> objectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            Assert.IsTrue(objectPropertyAssertion.Count == 1);

			List<OWLSameIndividual> sameIndividual = ontology.GetAssertionAxiomsOfType<OWLSameIndividual>();
            Assert.IsTrue(sameIndividual.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 0);
        }
		
		[TestMethod]
        public void ShouldGetSameIndividuals()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AssertionAxioms = [
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv3")) ]),
					new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4")) ]),
					new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv5")) ]),
                ]
            };

            List<OWLIndividualExpression> sameIndividualsOfIdv1 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")));
            Assert.IsTrue(sameIndividualsOfIdv1.Count == 4);
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
            Assert.IsTrue(sameIndividualsOfIdv2.Count == 4);

            List<OWLIndividualExpression> sameIndividualsOfIdv3 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv3")));
            Assert.IsTrue(sameIndividualsOfIdv3.Count == 4);

            List<OWLIndividualExpression> sameIndividualsOfIdv4 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv4")));
            Assert.IsTrue(sameIndividualsOfIdv4.Count == 4);

            Assert.IsTrue(ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv6"))).Count == 0);
            Assert.IsTrue(ontology.GetSameIndividuals(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSameIndividual(null, new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsFalse(ontology.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv1")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2"))));
            Assert.IsTrue((null as OWLOntology).GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldGetSameIndividualsDirectOnly()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AssertionAxioms = [
                    new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv3")) ]),
					new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4")) ]),
					new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv5")) ]),
                ]
            };

            List<OWLIndividualExpression> equivalentDataPropertiesOfIdv1 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), true);
            Assert.IsTrue(equivalentDataPropertiesOfIdv1.Count == 3);

            List<OWLIndividualExpression> equivalentDataPropertiesOfIdv2 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv2")), true);
            Assert.IsTrue(equivalentDataPropertiesOfIdv2.Count == 3);

            List<OWLIndividualExpression> equivalentDataPropertiesOfIdv3 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv3")), true);
            Assert.IsTrue(equivalentDataPropertiesOfIdv3.Count == 2);

            List<OWLIndividualExpression> equivalentDataPropertiesOfIdv4 = ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv4")), true);
            Assert.IsTrue(equivalentDataPropertiesOfIdv4.Count == 1);

            Assert.IsTrue(ontology.GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv6")), true).Count == 0);
            Assert.IsTrue(ontology.GetSameIndividuals(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSameIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), true).Count == 0);
        }

        [TestMethod]
        public void ShouldGetDifferentIndividuals()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AssertionAxioms = [
                    new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv3")) ]),
                    new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4")) ]),
                    new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv5")) ]),
                ]
            };

            List<OWLIndividualExpression> differentIndividualsOfIdv1 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), true);
            Assert.IsTrue(differentIndividualsOfIdv1.Count == 3);
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv2"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv2")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv3"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv3")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv4"))));
            Assert.IsTrue(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv4")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));
            Assert.IsFalse(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), new OWLNamedIndividual(new RDFResource("ex:Idv5"))));
            Assert.IsFalse(ontology.CheckAreDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv5")), new OWLNamedIndividual(new RDFResource("ex:Idv1"))));

            List<OWLIndividualExpression> differentIndividualsOfIdv2 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv2")), true);
            Assert.IsTrue(differentIndividualsOfIdv2.Count == 3);

            List<OWLIndividualExpression> differentIndividualsOfIdv3 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv3")), true);
            Assert.IsTrue(differentIndividualsOfIdv3.Count == 2);

            List<OWLIndividualExpression> differentIndividualsOfIdv4 = ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv4")), true);
            Assert.IsTrue(differentIndividualsOfIdv4.Count == 1);

            Assert.IsTrue(ontology.GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv6")), true).Count == 0);
            Assert.IsTrue(ontology.GetDifferentIndividuals(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetDifferentIndividuals(new OWLNamedIndividual(new RDFResource("ex:Idv1")), true).Count == 0);
        }

        [TestMethod]
        public void ShouldGetIndividualsOfSimpleClass()
        {
            OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(cats.Count == 1);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            
            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.IsTrue(domesticFelines.Count == 1);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.IsTrue(parrots.Count == 2);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.IsTrue(animals.Count == 3);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.IsTrue(livingEntities.Count == 3);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            //DirectOnly
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")), true).Count == 0);
        }

        [TestMethod]
        public void ShouldGetIndividualsOfEnumerateClass()
        {
            OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(cats.Count == 1);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));

            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.IsTrue(domesticFelines.Count == 1);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.IsTrue(parrots.Count == 2);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.IsTrue(animals.Count == 3);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.IsTrue(livingEntities.Count == 3);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.IsTrue(myAnimals.Count == 3);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            //DirectOnly
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")), true).Count == 0);
        }

        [TestMethod]
        public void ShouldGetIndividualsOfCompositeUnionClass()
        {
            OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(cats.Count == 2);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.IsTrue(domesticFelines.Count == 2);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.IsTrue(parrots.Count == 2);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.IsTrue(animals.Count == 4);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.IsTrue(livingEntities.Count == 4);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.IsTrue(myAnimals.Count == 3);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> parrotsOrCats = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:ParrotOrCat")));
            Assert.IsTrue(parrotsOrCats.Count == 4);
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrotsOrCats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            //DirectOnly
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:ParrotOrCat")), true).Count == 0);
        }

        [TestMethod]
        public void ShouldGetIndividualsOfCompositeIntersectionClass()
        {
            OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(cats.Count == 2);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.IsTrue(domesticFelines.Count == 2);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));

            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.IsTrue(parrots.Count == 2);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.IsTrue(animals.Count == 4);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.IsTrue(livingEntities.Count == 4);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PFelix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.IsTrue(myAnimals.Count == 3);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> parrotsAndMyAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:ParrotAndMyAnymal")));
            Assert.IsTrue(parrotsAndMyAnimals.Count == 2);
            Assert.IsTrue(parrotsAndMyAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrotsAndMyAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            //DirectOnly
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:ParrotAndMyAnymal")), true).Count == 0);
        }

        [TestMethod]
        public void ShouldGetIndividualsOfCompositeComplementClass()
        {
            OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(cats.Count == 1);
            Assert.IsTrue(cats.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            
            List<OWLIndividualExpression> domesticFelines = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")));
            Assert.IsTrue(domesticFelines.Count == 1);
            Assert.IsTrue(domesticFelines.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            
            List<OWLIndividualExpression> parrots = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")));
            Assert.IsTrue(parrots.Count == 2);
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(parrots.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> animals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")));
            Assert.IsTrue(animals.Count == 3);
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(animals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> livingEntities = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")));
            Assert.IsTrue(livingEntities.Count == 3);
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(livingEntities.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> myAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")));
            Assert.IsTrue(myAnimals.Count == 3);
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Felix"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Paco"))));
            Assert.IsTrue(myAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:PaquitoTheParrot"))));

            List<OWLIndividualExpression> notMyAnimals = ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:NotMyAnymal")));
            Assert.IsTrue(notMyAnimals.Count == 1);
            Assert.IsTrue(notMyAnimals.Any(iex => iex.GetIRI().Equals(new RDFResource("ex:Fuffy"))));

            //DirectOnly
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Cat")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:DomesticFeline")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Parrot")), true).Count == 1);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:Animal")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:LivingEntity")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:MyAnimals")), true).Count == 0);
            Assert.IsTrue(ontology.GetIndividualsOf(new OWLClass(new RDFResource("ex:NotMyAnymal")), true).Count == 1);
        }
        #endregion
    }
}