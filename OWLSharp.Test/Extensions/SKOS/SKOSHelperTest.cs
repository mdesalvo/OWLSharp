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
using OWLSharp.Extensions.SKOS;
using RDFSharp.Model;
using System;
using System.Collections.Generic;

namespace OWLSharp.Test.Extensions.SKOS
{
    [TestClass]
    public class SKOSHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldDeclareConceptScheme()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareConceptScheme(new RDFResource("ex:ConceptScheme"), [new RDFResource("ex:ConceptA")]);

            Assert.AreEqual(2, ontology.DeclarationAxioms.Count);
            Assert.AreEqual(2, ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count);
            Assert.AreEqual(1, ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareConceptScheme(null, [new RDFResource("ex:ConceptScheme")]));
        }

        [TestMethod]
        public void ShouldDeclareConcept()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareConcept(new RDFResource("ex:Concept"), [
                new RDFPlainLiteral("This is a concept"),
                new RDFPlainLiteral("This is a concept", "en-US")]);

            Assert.AreEqual(1, ontology.DeclarationAxioms.Count);
            Assert.AreEqual(1, ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count);
            Assert.AreEqual(2, ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareConcept(null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareConcept(new RDFResource("ex:Concept"), [
                new RDFPlainLiteral("This is a concept"), new RDFPlainLiteral("This is the same concept")]));
        }

        [TestMethod]
        public void ShouldDeclareConceptInScheme()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareConcept(new RDFResource("ex:Concept"), [
                new RDFPlainLiteral("This is a concept"),
                new RDFPlainLiteral("This is a concept", "en-US")], new RDFResource("ex:ConceptScheme"));

            Assert.AreEqual(2, ontology.DeclarationAxioms.Count);
            Assert.AreEqual(2, ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count);
            Assert.AreEqual(2, ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count);
            Assert.AreEqual(1, ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count);
        }

        [TestMethod]
        public void ShouldDeclareCollection()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareCollection(new RDFResource("ex:Collection"),
                [new RDFResource("ex:ConceptA"), new RDFResource("ex:ConceptB")],
                [new RDFPlainLiteral("This is a collection"), new RDFPlainLiteral("This is a collection", "en-US")]);

            Assert.AreEqual(3, ontology.DeclarationAxioms.Count);
            Assert.AreEqual(3, ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count);
            Assert.AreEqual(2, ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareCollection(null, [new RDFResource("ex:ConceptA")]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareCollection(new RDFResource("ex:Collection"), null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareCollection(new RDFResource("ex:Collection"), []));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareCollection(new RDFResource("ex:Collection"),
                [new RDFResource("ex:ConceptA"), new RDFResource("ex:ConceptB")],
                [new RDFPlainLiteral("This is a collection", "en-US"), new RDFPlainLiteral("This is the same collection", "en-US")]));
        }

        [TestMethod]
        public void ShouldDeclareCollectionInScheme()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareCollection(new RDFResource("ex:Collection"),
                [new RDFResource("ex:ConceptA"), new RDFResource("ex:ConceptB")],
                [new RDFPlainLiteral("This is a collection"), new RDFPlainLiteral("This is a collection", "en-US")], new RDFResource("ex:ConceptScheme"));

            Assert.AreEqual(4, ontology.DeclarationAxioms.Count);
            Assert.AreEqual(4, ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count);
            Assert.AreEqual(2, ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count);
            Assert.AreEqual(3, ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count);
        }

        [TestMethod]
        public void ShouldCheckConceptScheme()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:conceptScheme1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:conceptScheme2")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme2"))));

            Assert.IsTrue(ontology.CheckHasConceptScheme(new RDFResource("ex:conceptScheme1")));
            Assert.IsFalse(ontology.CheckHasConceptScheme(new RDFResource("ex:conceptScheme3")));
            Assert.IsFalse((null as OWLOntology).CheckHasConceptScheme(new RDFResource("ex:conceptScheme3")));
            Assert.IsFalse(ontology.CheckHasConceptScheme(null));
        }

        [TestMethod]
        public void ShouldCheckAndGetConceptsInScheme()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.HAS_TOP_CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.TOP_CONCEPT_OF));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:conceptScheme1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:conceptScheme2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
               new OWLClass(RDFVocabulary.SKOS.CONCEPT),
               new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
               new OWLClass(RDFVocabulary.SKOS.CONCEPT),
               new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.TOP_CONCEPT_OF),
                new OWLNamedIndividual(new RDFResource("ex:concept4")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.HAS_TOP_CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme2"))));

            List<RDFResource> cs1Concepts = ontology.GetConceptsInScheme(new RDFResource("ex:conceptScheme1"));

            Assert.AreEqual(4, cs1Concepts.Count);
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept4"))); //via skos:topConceptOf
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept5"))); //via skos:hasTopConcept

            Assert.AreEqual(0, (null as OWLOntology).GetConceptsInScheme(new RDFResource("ex:concept1")).Count);
            Assert.AreEqual(0, ontology.GetConceptsInScheme(null).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetCollectionsInScheme()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.COLLECTION));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.ORDERED_COLLECTION));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:conceptScheme1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:collection1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:collection2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.COLLECTION),
                new OWLNamedIndividual(new RDFResource("ex:collection1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.ORDERED_COLLECTION),
                new OWLNamedIndividual(new RDFResource("ex:collection2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
               new OWLClass(RDFVocabulary.SKOS.CONCEPT),
               new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
               new OWLClass(RDFVocabulary.SKOS.CONCEPT),
               new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept4")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept5")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:collection1")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:collection2")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));

            List<RDFResource> cs1Collections = ontology.GetCollectionsInScheme(new RDFResource("ex:conceptScheme1"));

            Assert.AreEqual(2, cs1Collections.Count);
            Assert.IsTrue(ontology.CheckHasCollection(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:collection1")));
            Assert.IsTrue(ontology.CheckHasCollection(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:collection2")));

            Assert.AreEqual(0, (null as OWLOntology).GetCollectionsInScheme(new RDFResource("ex:conceptScheme1")).Count);
            Assert.AreEqual(0, ontology.GetCollectionsInScheme(null).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasCollection(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasCollection(null, new RDFResource("ex:conceptScheme1")));
            Assert.IsFalse(ontology.CheckHasCollection(new RDFResource("ex:conceptScheme1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetBroaderConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));

            List<RDFResource> c1BroaderConcepts = ontology.GetBroaderConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(3, c1BroaderConcepts.Count);
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"))); //skos:broader is not transitive
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //inference

            Assert.AreEqual(0, ontology.GetBroaderConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetBroaderConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasBroaderConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetNarrowerConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));

            List<RDFResource> c1NarrowerConcepts = ontology.GetNarrowerConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(3, c1NarrowerConcepts.Count);
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"))); //skos:narrower is not transitive
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //inference

            Assert.AreEqual(0, ontology.GetNarrowerConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetNarrowerConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasNarrowerConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1RelatedConcepts = ontology.GetRelatedConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(2, c1RelatedConcepts.Count);
            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via simmetry)

            Assert.AreEqual(0, ontology.GetRelatedConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetRelatedConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasRelatedConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetBroadMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1BroadMatchConcepts = ontology.GetBroadMatchConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(2, c1BroadMatchConcepts.Count);
            Assert.IsTrue(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via inverse)

            Assert.AreEqual(0, ontology.GetBroadMatchConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetBroadMatchConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasBroadMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetNarrowMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1NarrowMatchConcepts = ontology.GetNarrowMatchConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(2, c1NarrowMatchConcepts.Count);
            Assert.IsTrue(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via inverse)

            Assert.AreEqual(0, ontology.GetNarrowMatchConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetNarrowMatchConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasNarrowMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetCloseMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1CloseMatchConcepts = ontology.GetCloseMatchConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(2, c1CloseMatchConcepts.Count);
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via simmetry)

            Assert.AreEqual(0, ontology.GetCloseMatchConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetCloseMatchConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasCloseMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetExactMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept5")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));

            List<RDFResource> c1ExactMatchConcepts = ontology.GetExactMatchConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(4, c1ExactMatchConcepts.Count);
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"))); //inference (via simmetry)

            Assert.AreEqual(0, ontology.GetExactMatchConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetExactMatchConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasExactMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetRelatedMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1RelatedMatchConcepts = ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept1"));

            Assert.AreEqual(2, c1RelatedMatchConcepts.Count);
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via simmetry)

            Assert.AreEqual(0, ontology.GetRelatedMatchConcepts(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetRelatedMatchConcepts(new RDFResource("ex:concept1")).Count);
            Assert.IsFalse((null as OWLOntology).CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasRelatedMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), null));
        }
        #endregion
    }
}