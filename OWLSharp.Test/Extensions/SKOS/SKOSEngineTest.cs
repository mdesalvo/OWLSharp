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
using OWLSharp.Extensions.SKOS;
using RDFSharp.Model;
using System;
using System.Collections.Generic;

namespace OWLSharp.Test.Extensions.SKOS
{
    [TestClass]
    public class SKOSEngineTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCheckAndGetConceptsInScheme()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.HAS_TOP_CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.TOP_CONCEPT_OF));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:conceptScheme1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:conceptScheme2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
               new OWLClass(RDFVocabulary.SKOS.CONCEPT),
               new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
               new OWLClass(RDFVocabulary.SKOS.CONCEPT),
               new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.TOP_CONCEPT_OF),
                new OWLNamedIndividual(new RDFResource("ex:concept4")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.HAS_TOP_CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme1")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:conceptScheme2"))));

            List<RDFResource> cs1Concepts = ontology.GetConceptsInScheme(new RDFResource("ex:conceptScheme1"));

            Assert.IsTrue(cs1Concepts.Count == 4);
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept1")));
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept4"))); //via skos:topConceptOf
            Assert.IsTrue(ontology.CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept5"))); //via skos:hasTopConcept

            Assert.IsTrue((null as OWLOntology).GetConceptsInScheme(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsTrue(ontology.GetConceptsInScheme(null).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasConcept(new RDFResource("ex:conceptScheme1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetBroaderConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));

            List<RDFResource> c1BroaderConcepts = ontology.GetBroaderConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1BroaderConcepts.Count == 3);
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"))); //skos:broader is not transitive
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //inference

            Assert.IsTrue(ontology.GetBroaderConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetBroaderConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasBroaderConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasBroaderConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasBroaderConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetNarrowerConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));

            List<RDFResource> c1NarrowerConcepts = ontology.GetNarrowerConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1NarrowerConcepts.Count == 3);
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"))); //skos:narrower is not transitive
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3")));
            Assert.IsTrue(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //inference

            Assert.IsTrue(ontology.GetNarrowerConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetNarrowerConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasNarrowerConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasNarrowerConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasNarrowerConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetRelatedConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1RelatedConcepts = ontology.GetRelatedConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1RelatedConcepts.Count == 2);
            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via simmetry)

            Assert.IsTrue(ontology.GetRelatedConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetRelatedConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasRelatedConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasRelatedConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasRelatedConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetBroadMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1BroadMatchConcepts = ontology.GetBroadMatchConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1BroadMatchConcepts.Count == 2);
            Assert.IsTrue(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via inverse)

            Assert.IsTrue(ontology.GetBroadMatchConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetBroadMatchConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasBroadMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasBroadMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetNarrowMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1NarrowMatchConcepts = ontology.GetNarrowMatchConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1NarrowMatchConcepts.Count == 2);
            Assert.IsTrue(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via inverse)

            Assert.IsTrue(ontology.GetNarrowMatchConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetNarrowMatchConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasNarrowMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasNarrowMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetCloseMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1CloseMatchConcepts = ontology.GetCloseMatchConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1CloseMatchConcepts.Count == 2);
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via simmetry)

            Assert.IsTrue(ontology.GetCloseMatchConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetCloseMatchConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasCloseMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasCloseMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetExactMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept5")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));

            List<RDFResource> c1ExactMatchConcepts = ontology.GetExactMatchConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1ExactMatchConcepts.Count == 4);
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept4"))); //inference
            Assert.IsTrue(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept5"))); //inference (via simmetry)

            Assert.IsTrue(ontology.GetExactMatchConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetExactMatchConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasExactMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasExactMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasExactMatchConcept(new RDFResource("ex:concept1"), null));
        }

        [TestMethod]
        public void ShouldCheckAndGetRelatedMatchConcepts()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ontology"));
            ontology.AddEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.AddEntity(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept1")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept2")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept3")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept4")));
            ontology.AddEntity(new OWLNamedIndividual(new RDFResource("ex:concept5")));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept3"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept1")),
                new OWLNamedIndividual(new RDFResource("ex:concept2"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept1"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept2")),
                new OWLNamedIndividual(new RDFResource("ex:concept4"))));
            ontology.AddAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                new OWLNamedIndividual(new RDFResource("ex:concept3")),
                new OWLNamedIndividual(new RDFResource("ex:concept5"))));

            List<RDFResource> c1RelatedMatchConcepts = ontology.GetRelatedMatchConcepts(new RDFResource("ex:concept1"));

            Assert.IsTrue(c1RelatedMatchConcepts.Count == 2);
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsTrue(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept3"))); //inference (via simmetry)

            Assert.IsTrue(ontology.GetRelatedMatchConcepts(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetRelatedMatchConcepts(new RDFResource("ex:concept1")).Count == 0);
            Assert.IsFalse((null as OWLOntology).CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), new RDFResource("ex:concept2")));
            Assert.IsFalse(ontology.CheckHasRelatedMatchConcept(null, new RDFResource("ex:concept5")));
            Assert.IsFalse(ontology.CheckHasRelatedMatchConcept(new RDFResource("ex:concept1"), null));
        }
        #endregion
    }
}