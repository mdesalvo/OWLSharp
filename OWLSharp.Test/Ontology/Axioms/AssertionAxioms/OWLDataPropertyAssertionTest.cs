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
public class OWLDataPropertyAssertionTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateNamedIndividualDataPropertyAssertion()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));

        Assert.IsNotNull(dataPropertyAssertion);
        Assert.IsNotNull(dataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(dataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLNamedIndividual)dataPropertyAssertion.IndividualExpression).IRI, "ex:Bob"));
        Assert.IsNotNull(dataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualDataPropertyAssertionBecauseNullDataProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataPropertyAssertion(
            null,
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualDataPropertyAssertionBecauseNullNamedIndividual()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            null as OWLNamedIndividual,
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualDataPropertyAssertionBecauseNullLiteral()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            null));

    [TestMethod]
    public void ShouldSerializeNamedIndividualDataPropertyAssertion()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        string serializedXML = OWLSerializer.SerializeObject(dataPropertyAssertion);

        Assert.IsTrue(string.Equals(serializedXML,
            """<DataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><NamedIndividual IRI="ex:Bob" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></DataPropertyAssertion>"""));
    }

    [TestMethod]
    public void ShouldSerializeNamedIndividualDataPropertyAssertionViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.AssertionAxioms.Add(
            new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                new OWLNamedIndividual(new RDFResource("ex:Bob")),
                new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
            {
                Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
            });
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><DataPropertyAssertion><Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" /><Literal xml:lang="EN">Steve</Literal></Annotation><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><NamedIndividual IRI="ex:Bob" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></DataPropertyAssertion></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeNamedIndividualDataPropertyAssertion()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = OWLSerializer.DeserializeObject<OWLDataPropertyAssertion>(
            """<DataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><NamedIndividual IRI="ex:Bob" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></DataPropertyAssertion>""");
        
        Assert.IsNotNull(dataPropertyAssertion);
        Assert.IsNotNull(dataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(dataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLNamedIndividual)dataPropertyAssertion.IndividualExpression).IRI, "ex:Bob"));
        Assert.IsNotNull(dataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldDeserializeNamedIndividualDataPropertyAssertionViaOntology()
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
                <DataPropertyAssertion>
                    <Annotation>
                        <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                        <Literal xml:lang="EN">Steve</Literal>
                    </Annotation>
                    <DataProperty IRI="http://xmlns.com/foaf/0.1/age" />
                    <NamedIndividual IRI="ex:Bob" />
                    <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal>
                </DataPropertyAssertion>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.AssertionAxioms.Count);
        Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLDataPropertyAssertion dpAsn
                      && string.Equals(dpAsn.DataProperty.IRI, "http://xmlns.com/foaf/0.1/age")
                      && string.Equals(((OWLNamedIndividual)dpAsn.IndividualExpression).IRI, "ex:Bob")
                      && string.Equals(dpAsn.Literal.Value, "25")
                      && string.Equals(dpAsn.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
        Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLDataPropertyAssertion dpAsn1
                      && string.Equals(dpAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                      && string.Equals(dpAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                      && string.Equals(dpAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldCreateAnonymousIndividualDataPropertyAssertion()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("AnonIdv"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));

        Assert.IsNotNull(dataPropertyAssertion);
        Assert.IsNotNull(dataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(dataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLAnonymousIndividual)dataPropertyAssertion.IndividualExpression).NodeID, "AnonIdv"));
        Assert.IsNotNull(dataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnonymousIndividualDataPropertyAssertionBecauseNullDataProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataPropertyAssertion(
            null,
            new OWLAnonymousIndividual("AnonIdv"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnonymousIndividualDataPropertyAssertionBecauseNullAnonymousIndividual()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            null as OWLAnonymousIndividual,
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnonymousIndividualDataPropertyAssertionBecauseNullLiteral()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("AnonIdv"),
            null));

    [TestMethod]
    public void ShouldSerializeAnonymousIndividualDataPropertyAssertion()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("AnonIdv"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        string serializedXML = OWLSerializer.SerializeObject(dataPropertyAssertion);

        Assert.IsTrue(string.Equals(serializedXML,
            """<DataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><AnonymousIndividual nodeID="AnonIdv" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></DataPropertyAssertion>"""));
    }

    [TestMethod]
    public void ShouldDeserializeAnonymousIndividualDataPropertyAssertion()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = OWLSerializer.DeserializeObject<OWLDataPropertyAssertion>(
            """<DataPropertyAssertion><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><AnonymousIndividual nodeID="AnonIdv" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">25</Literal></DataPropertyAssertion>""");
        
        Assert.IsNotNull(dataPropertyAssertion);
        Assert.IsNotNull(dataPropertyAssertion.DataProperty);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNotNull(dataPropertyAssertion.IndividualExpression);
        Assert.IsTrue(string.Equals(((OWLAnonymousIndividual)dataPropertyAssertion.IndividualExpression).NodeID, "AnonIdv"));
        Assert.IsNotNull(dataPropertyAssertion.Literal);
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.Value, "25"));
        Assert.IsTrue(string.Equals(dataPropertyAssertion.Literal.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#integer"));
    }

    [TestMethod]
    public void ShouldConvertDataPropertyAssertionToGraph()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        RDFGraph graph = dataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(3, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.FOAF.AGE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertDataPropertyAssertionWithAnonymousIndividualToGraph()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("Bob"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        RDFGraph graph = dataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(2, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("bnode:Bob"), RDFVocabulary.FOAF.AGE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertDataPropertyAssertionWithAnnotationToGraph()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLNamedIndividual(new RDFResource("ex:Bob")),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = dataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(9, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.FOAF.AGE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:Bob"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.FOAF.AGE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertDataPropertyAssertionWithAnonymousIndividualWithAnnotationToGraph()
    {
        OWLDataPropertyAssertion dataPropertyAssertion = new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new OWLAnonymousIndividual("Bob"),
            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = dataPropertyAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(8, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("bnode:Bob"), RDFVocabulary.FOAF.AGE, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("bnode:Bob"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.FOAF.AGE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}