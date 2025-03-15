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

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLNegativeDataPropertyAssertionTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateNamedIndividualNegativeDataPropertyAssertion()
    {
        OWLNegativeDataPropertyAssertion NegativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));

        Assert.IsNotNull(NegativeDataPropertyAssertion);
        Assert.IsNotNull(NegativeDataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(NegativeDataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLNamedIndividual)NegativeDataPropertyAssertion.IndividualExpression).IRI, "ex:Bob"));
        Assert.IsNotNull(NegativeDataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualNegativeDataPropertyAssertionBecauseNullDataProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeDataPropertyAssertion(
            null,
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualNegativeDataPropertyAssertionBecauseNullNamedIndividual()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            null as OWLNamedIndividual,
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualNegativeDataPropertyAssertionBecauseNullLiteral()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            null));

    [TestMethod]
    public void ShouldSerializeNamedIndividualNegativeDataPropertyAssertion()
    {
        OWLNegativeDataPropertyAssertion NegativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        string serializedXML = OWLSerializer.SerializeObject(NegativeDataPropertyAssertion);

        Assert.IsTrue(string.Equals(serializedXML,
            """<NegativeDataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><NamedIndividual IRI="ex:Bob" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></NegativeDataPropertyAssertion>"""));
    }

    [TestMethod]
    public void ShouldSerializeNamedIndividualNegativeDataPropertyAssertionViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.AssertionAxioms.Add(
            new OWLNegativeDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                new OWLNamedIndividual(new RDFResource("ex:Bob")),
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
            {
                Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
            });
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><NegativeDataPropertyAssertion><Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" /><Literal xml:lang="EN">Steve</Literal></Annotation><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><NamedIndividual IRI="ex:Bob" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></NegativeDataPropertyAssertion></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeNamedIndividualNegativeDataPropertyAssertion()
    {
        OWLNegativeDataPropertyAssertion NegativeDataPropertyAssertion = OWLSerializer.DeserializeObject<OWLNegativeDataPropertyAssertion>(
            """<NegativeDataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><NamedIndividual IRI="ex:Bob" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></NegativeDataPropertyAssertion>""");

        Assert.IsNotNull(NegativeDataPropertyAssertion);
        Assert.IsNotNull(NegativeDataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(NegativeDataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLNamedIndividual)NegativeDataPropertyAssertion.IndividualExpression).IRI, "ex:Bob"));
        Assert.IsNotNull(NegativeDataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldDeserializeNamedIndividualNegativeDataPropertyAssertionViaOntology()
    {
        OWLOntology ontology = OWLSerializer.DeserializeOntology(
            """
            <?xml version="1.0" encoding="utf-8"?>
            <Ontology xmlns:owl="http://www.w3.org/2002/07/owl#" xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:xsd="http://www.w3.org/2001/XMLSchema#">
                <Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" />
                <Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" />
                <Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" />
                <Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" />
                <Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" />
                <NegativeDataPropertyAssertion>
                    <Annotation>
                        <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                        <Literal xml:lang="EN">Steve</Literal>
                    </Annotation>
                    <DataProperty IRI="http://xmlns.com/foaf/0.1/age" />
                    <NamedIndividual IRI="ex:Bob" />
                    <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal>
                </NegativeDataPropertyAssertion>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.AssertionAxioms.Count);
        Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeDataPropertyAssertion ndpAsn
                      && string.Equals(ndpAsn.DataProperty.IRI, "http://xmlns.com/foaf/0.1/age")
                      && string.Equals(((OWLNamedIndividual)ndpAsn.IndividualExpression).IRI, "ex:Bob")
                      && string.Equals(ndpAsn.Literal.Value, "25")
                      && string.Equals(ndpAsn.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
        Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLNegativeDataPropertyAssertion ndpAsn1
                      && string.Equals(ndpAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                      && string.Equals(ndpAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                      && string.Equals(ndpAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldCreateAnonymousIndividualNegativeDataPropertyAssertion()
    {
        OWLNegativeDataPropertyAssertion NegativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("AnonIdv"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));

        Assert.IsNotNull(NegativeDataPropertyAssertion);
        Assert.IsNotNull(NegativeDataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(NegativeDataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLAnonymousIndividual)NegativeDataPropertyAssertion.IndividualExpression).NodeID, "AnonIdv"));
        Assert.IsNotNull(NegativeDataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnonymousIndividualNegativeDataPropertyAssertionBecauseNullDataProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeDataPropertyAssertion(
            null,
            new OWLAnonymousIndividual("AnonIdv"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnonymousIndividualNegativeDataPropertyAssertionBecauseNullAnonymousIndividual()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            null as OWLAnonymousIndividual,
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnonymousIndividualNegativeDataPropertyAssertionBecauseNullLiteral()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("AnonIdv"),
            null));

    [TestMethod]
    public void ShouldSerializeAnonymousIndividualNegativeDataPropertyAssertion()
    {
        OWLNegativeDataPropertyAssertion NegativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("AnonIdv"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        string serializedXML = OWLSerializer.SerializeObject(NegativeDataPropertyAssertion);

        Assert.IsTrue(string.Equals(serializedXML,
            """<NegativeDataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><AnonymousIndividual nodeID="AnonIdv" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></NegativeDataPropertyAssertion>"""));
    }

    [TestMethod]
    public void ShouldDeserializeAnonymousIndividualNegativeDataPropertyAssertion()
    {
        OWLNegativeDataPropertyAssertion NegativeDataPropertyAssertion = OWLSerializer.DeserializeObject<OWLNegativeDataPropertyAssertion>(
            """<NegativeDataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><AnonymousIndividual nodeID="AnonIdv" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></NegativeDataPropertyAssertion>""");

        Assert.IsNotNull(NegativeDataPropertyAssertion);
        Assert.IsNotNull(NegativeDataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(NegativeDataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLAnonymousIndividual)NegativeDataPropertyAssertion.IndividualExpression).NodeID, "AnonIdv"));
        Assert.IsNotNull(NegativeDataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(NegativeDataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldConvertNegativeDataPropertyAssertionToGraph()
    {
        OWLNegativeDataPropertyAssertion negativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        RDFGraph graph = negativeDataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(6, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.AGE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_VALUE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertNegativeDataPropertyAssertionWithAnonymousIndividualToGraph()
    {
        OWLNegativeDataPropertyAssertion negativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("Bob"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        RDFGraph graph = negativeDataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(5, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("bnode:Bob"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.AGE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_VALUE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertNegativeDataPropertyAssertionWithAnnotationToGraph()
    {
        OWLNegativeDataPropertyAssertion negativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = negativeDataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(8, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:Bob"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.AGE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_VALUE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertNegativeDataPropertyAssertionWithAnonymousIndividualWithAnnotationToGraph()
    {
        OWLNegativeDataPropertyAssertion negativeDataPropertyAssertion = new OWLNegativeDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("Bob"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = negativeDataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(7, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("bnode:Bob"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.AGE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.TARGET_VALUE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}