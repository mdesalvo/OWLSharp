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
    public class OWLNegativeNegativeObjectPropertyAssertionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateNamedIndividualNegativeObjectPropertyAssertion()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));

            Assert.IsNotNull(negativeObjectPropertyAssertion);
            Assert.IsNotNull(negativeObjectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.ObjectPropertyExpression is OWLObjectProperty objProp &&
                            string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(negativeObjectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.SourceIndividualExpression is OWLNamedIndividual srcIdv &&
                            string.Equals(srcIdv.IRI, "ex:Alice"));
            Assert.IsNotNull(negativeObjectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.TargetIndividualExpression is OWLNamedIndividual dstIdv &&
                            string.Equals(dstIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualNegativeObjectPropertyAssertionBecauseNullObjectProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeObjectPropertyAssertion(
                null,
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualNegativeObjectPropertyAssertionBecauseSourceIndividual()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.AGE),
                null,
                new OWLNamedIndividual(new RDFResource("ex:Bob"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualNegativeObjectPropertyAssertionBecauseNullTargetIndividual()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.AGE),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                null));

        [TestMethod]
        public void ShouldSerializeNamedIndividualNegativeObjectPropertyAssertion()
        {
             OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(negativeObjectPropertyAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<NegativeObjectPropertyAssertion><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /></NegativeObjectPropertyAssertion>"));
        }

        [TestMethod]
        public void ShouldSerializeNamedIndividualNegativeObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AssertionAxioms.Add(
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:Alice")),
                    new OWLNamedIndividual(new RDFResource("ex:Bob")))
                    {
                        Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
                    });
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><NegativeObjectPropertyAssertion><Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" /><Literal xml:lang=""EN"">Steve</Literal></Annotation><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /></NegativeObjectPropertyAssertion></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualNegativeObjectPropertyAssertion()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = OWLSerializer.DeserializeObject<OWLNegativeObjectPropertyAssertion>(
@"<NegativeObjectPropertyAssertion><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /></NegativeObjectPropertyAssertion>");
        
            Assert.IsNotNull(negativeObjectPropertyAssertion);
            Assert.IsNotNull(negativeObjectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.ObjectPropertyExpression is OWLObjectProperty objProp &&
                            string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(negativeObjectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.SourceIndividualExpression is OWLNamedIndividual srcIdv &&
                            string.Equals(srcIdv.IRI, "ex:Alice"));
            Assert.IsNotNull(negativeObjectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.TargetIndividualExpression is OWLNamedIndividual dstIdv &&
                            string.Equals(dstIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualNegativeObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
    <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
    <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
    <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
    <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
    <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
    <NegativeObjectPropertyAssertion>
        <Annotation>
            <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
            <Literal xml:lang=""EN"">Steve</Literal>
        </Annotation>
        <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
        <NamedIndividual IRI=""ex:Alice"" />
        <NamedIndividual IRI=""ex:Bob"" />
    </NegativeObjectPropertyAssertion>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeObjectPropertyAssertion opAsn
                            && opAsn.ObjectPropertyExpression is OWLObjectProperty objProp 
                            && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows")
                            && opAsn.SourceIndividualExpression is OWLNamedIndividual srcIdv
                            && string.Equals(srcIdv.IRI, "ex:Alice")
                            && opAsn.TargetIndividualExpression is OWLNamedIndividual dstIdv
                            && string.Equals(dstIdv.IRI, "ex:Bob"));
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeObjectPropertyAssertion opAsn1
                            && string.Equals(opAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldSerializeNamedIndividualInverseNegativeObjectPropertyAssertion()
        {
             OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(negativeObjectPropertyAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<NegativeObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /></NegativeObjectPropertyAssertion>"));
        }

        [TestMethod]
        public void ShouldSerializeNamedIndividualInverseNegativeObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AssertionAxioms.Add(
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLNamedIndividual(new RDFResource("ex:Alice")),
                    new OWLNamedIndividual(new RDFResource("ex:Bob")))
                    {
                        Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
                    });
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><NegativeObjectPropertyAssertion><Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" /><Literal xml:lang=""EN"">Steve</Literal></Annotation><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /></NegativeObjectPropertyAssertion></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualInverseNegativeObjectPropertyAssertion()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = OWLSerializer.DeserializeObject<OWLNegativeObjectPropertyAssertion>(
@"<NegativeObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /></NegativeObjectPropertyAssertion>");
        
            Assert.IsNotNull(negativeObjectPropertyAssertion);
            Assert.IsNotNull(negativeObjectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.ObjectPropertyExpression is OWLObjectInverseOf objInvOf &&
                            string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(negativeObjectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.SourceIndividualExpression is OWLNamedIndividual srcIdv &&
                            string.Equals(srcIdv.IRI, "ex:Alice"));
            Assert.IsNotNull(negativeObjectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.TargetIndividualExpression is OWLNamedIndividual dstIdv &&
                            string.Equals(dstIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualInverseNegativeObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Ontology>
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <NegativeObjectPropertyAssertion>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
      <Literal xml:lang=""EN"">Steve</Literal>
    </Annotation>
    <ObjectInverseOf>
      <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    </ObjectInverseOf>
    <NamedIndividual IRI=""ex:Alice"" />
    <NamedIndividual IRI=""ex:Bob"" />
  </NegativeObjectPropertyAssertion>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeObjectPropertyAssertion opAsn
                            && opAsn.ObjectPropertyExpression is OWLObjectInverseOf objInvOf 
                            && string.Equals(objInvOf.ObjectProperty.IRI, "http://xmlns.com/foaf/0.1/knows")
                            && opAsn.SourceIndividualExpression is OWLNamedIndividual srcIdv
                            && string.Equals(srcIdv.IRI, "ex:Alice")
                            && opAsn.TargetIndividualExpression is OWLNamedIndividual dstIdv
                            && string.Equals(dstIdv.IRI, "ex:Bob"));
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeObjectPropertyAssertion opAsn1
                            && string.Equals(opAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualInverseNegativeObjectPropertyAssertion()
        {
             OWLNegativeObjectPropertyAssertion NegativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLAnonymousIndividual("Alice"),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(NegativeObjectPropertyAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<NegativeObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><AnonymousIndividual nodeID=""Alice"" /><NamedIndividual IRI=""ex:Bob"" /></NegativeObjectPropertyAssertion>"));
        }

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualInverseNegativeObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AssertionAxioms.Add(
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLNamedIndividual(new RDFResource("ex:Alice")),
                    new OWLAnonymousIndividual("Bob"))
                    {
                        Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
                    });
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><NegativeObjectPropertyAssertion><Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" /><Literal xml:lang=""EN"">Steve</Literal></Annotation><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><NamedIndividual IRI=""ex:Alice"" /><AnonymousIndividual nodeID=""Bob"" /></NegativeObjectPropertyAssertion></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualInverseNegativeObjectPropertyAssertion()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = OWLSerializer.DeserializeObject<OWLNegativeObjectPropertyAssertion>(
@"<NegativeObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><AnonymousIndividual nodeID=""Alice"" /><AnonymousIndividual nodeID=""Bob"" /></NegativeObjectPropertyAssertion>");
        
            Assert.IsNotNull(negativeObjectPropertyAssertion);
            Assert.IsNotNull(negativeObjectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.ObjectPropertyExpression is OWLObjectInverseOf objInvOf &&
                            string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(negativeObjectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.SourceIndividualExpression is OWLAnonymousIndividual srcIdv &&
                            string.Equals(srcIdv.NodeID, "Alice"));
            Assert.IsNotNull(negativeObjectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(negativeObjectPropertyAssertion.TargetIndividualExpression is OWLAnonymousIndividual dstIdv &&
                            string.Equals(dstIdv.NodeID, "Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualInverseNegativeObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Ontology>
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <NegativeObjectPropertyAssertion>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
      <Literal xml:lang=""EN"">Steve</Literal>
    </Annotation>
    <ObjectInverseOf>
      <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    </ObjectInverseOf>
    <AnonymousIndividual nodeID=""Alice"" />
    <NamedIndividual IRI=""ex:Bob"" />
  </NegativeObjectPropertyAssertion>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeObjectPropertyAssertion opAsn
                            && opAsn.ObjectPropertyExpression is OWLObjectInverseOf objInvOf 
                            && string.Equals(objInvOf.ObjectProperty.IRI, "http://xmlns.com/foaf/0.1/knows")
                            && opAsn.SourceIndividualExpression is OWLAnonymousIndividual srcIdv
                            && string.Equals(srcIdv.NodeID, "Alice")
                            && opAsn.TargetIndividualExpression is OWLNamedIndividual dstIdv
                            && string.Equals(dstIdv.IRI, "ex:Bob"));
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeObjectPropertyAssertion opAsn1
                            && string.Equals(opAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertNegativeObjectPropertyAssertionToGraph()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            RDFGraph graph = negativeObjectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(7, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertNegativeObjectPropertyAssertionWithObjectInverseOfToGraph()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            RDFGraph graph = negativeObjectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(7, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertNegativeObjectPropertyAssertionWithAnonymousIndividualToGraph()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLAnonymousIndividual("Alice"),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            RDFGraph graph = negativeObjectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(6, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("bnode:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertNegativeObjectPropertyAssertionWithAnnotationToGraph()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = negativeObjectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertNegativeObjectPropertyAssertionWithObjectInverseOfWithAnnotationToGraph()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = negativeObjectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertNegativeObjectPropertyAssertionWithAnonymousIndividualWithAnnotationToGraph()
        {
            OWLNegativeObjectPropertyAssertion negativeObjectPropertyAssertion = new OWLNegativeObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLAnonymousIndividual("Alice"),
                new OWLNamedIndividual(new RDFResource("ex:Bob")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = negativeObjectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(8, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("bnode:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}