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
    public class SKOSHelperTest
    {
        #region Tests
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
        #endregion
    }
}