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
using RDFSharp.Model;
using System.Linq;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLClassAssertionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateClassAssertion()
        {
            OWLClassAssertion classAssertion = new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));

            Assert.IsNotNull(classAssertion);
            Assert.IsNotNull(classAssertion.ClassExpression);
            Assert.IsTrue(string.Equals(((OWLClass)classAssertion.ClassExpression).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(classAssertion.IndividualExpression);
            Assert.IsTrue(string.Equals(((OWLNamedIndividual)classAssertion.IndividualExpression).IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassAssertionBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLClassAssertion(
                null,
                new OWLNamedIndividual(new RDFResource("ex:Bob"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassAssertionBecauseNullNamedIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                null as OWLNamedIndividual));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassAssertionBecauseNullAnonymousIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                null as OWLAnonymousIndividual));

        [TestMethod]
        public void ShouldSerializeClassAssertion()
        {
            OWLClassAssertion classAssertion = new OWLClassAssertion(
               new OWLClass(RDFVocabulary.FOAF.AGENT),
               new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(classAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ClassAssertion><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><NamedIndividual IRI=""ex:Bob"" /></ClassAssertion>"));
        }

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualClassAssertion()
        {
            OWLClassAssertion classAssertion = new OWLClassAssertion(
               new OWLClass(RDFVocabulary.FOAF.AGENT),
               new OWLAnonymousIndividual("AnonIdv"));
            string serializedXML = OWLSerializer.SerializeObject(classAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ClassAssertion><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><AnonymousIndividual nodeID=""AnonIdv"" /></ClassAssertion>"));
        }

        [TestMethod]
        public void ShouldSerializeClassAssertionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AssertionAxioms.Add(
                new OWLClassAssertion(
                   new OWLClass(RDFVocabulary.FOAF.AGENT),
                   new OWLNamedIndividual(new RDFResource("ex:Bob"))));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><ClassAssertion><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><NamedIndividual IRI=""ex:Bob"" /></ClassAssertion></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeClassAssertion()
        {
            OWLClassAssertion classAssertion = OWLSerializer.DeserializeObject<OWLClassAssertion>(
@"<ClassAssertion>
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  <NamedIndividual IRI=""ex:Bob"" />
</ClassAssertion>");

            Assert.IsNotNull(classAssertion);
            Assert.IsNotNull(classAssertion.ClassExpression);
            Assert.IsTrue(string.Equals(((OWLClass)classAssertion.ClassExpression).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(classAssertion.IndividualExpression);
            Assert.IsTrue(string.Equals(((OWLNamedIndividual)classAssertion.IndividualExpression).IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualClassAssertion()
        {
            OWLClassAssertion classAssertion = OWLSerializer.DeserializeObject<OWLClassAssertion>(
@"<ClassAssertion>
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</ClassAssertion>");

            Assert.IsNotNull(classAssertion);
            Assert.IsNotNull(classAssertion.ClassExpression);
            Assert.IsTrue(string.Equals(((OWLClass)classAssertion.ClassExpression).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(classAssertion.IndividualExpression);
            Assert.IsTrue(string.Equals(((OWLAnonymousIndividual)classAssertion.IndividualExpression).NodeID, "AnonIdv"));
        }

        [TestMethod]
        public void ShouldDeserializeClassAssertionViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <ClassAssertion>
    <Annotation>
		<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
		<Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <NamedIndividual IRI=""ex:Bob"" />
  </ClassAssertion>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AssertionAxioms.Count == 1);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLClassAssertion clsAsn
                            && string.Equals(((OWLClass)clsAsn.ClassExpression).IRI, "http://xmlns.com/foaf/0.1/Agent")
                            && string.Equals(((OWLNamedIndividual)clsAsn.IndividualExpression).IRI, "ex:Bob"));
			Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLClassAssertion clsAsn1
							&& string.Equals(clsAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(clsAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(clsAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

		[TestMethod]
        public void ShouldConvertClassAssertionToGraph()
        {
            OWLClassAssertion classAssertion = new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
			RDFGraph graph = classAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
			Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertClassAssertionWithAnnotationToGraph()
        {
            OWLClassAssertion classAssertion = new OWLClassAssertion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new OWLNamedIndividual(new RDFResource("ex:Bob")))
			{
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
			RDFGraph graph = classAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 9);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
			Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			//Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:Bob"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}