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
public class OWLAnnotationAssertionTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateAnnotationIRIAssertion()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new RDFResource("ex:Obj"));

        Assert.IsNotNull(annotationAssertion);
        Assert.IsNotNull(annotationAssertion.AnnotationProperty);
        Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
        Assert.IsNotNull(annotationAssertion.SubjectIRI);
        Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
        Assert.IsNotNull(annotationAssertion.ValueIRI);
        Assert.IsTrue(string.Equals(annotationAssertion.ValueIRI, "ex:Obj"));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnnotationIRIAssertionBecauseNullAnnotationProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationAssertion(
            null,
            new RDFResource("ex:Subj"),
            new RDFResource("ex:Obj")));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnnotationIRIAssertionBecauseNullSubjectIRI()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            null,
            new RDFResource("ex:Obj")));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnnotationIRIAssertionBecauseNullValueIRI()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            null as RDFResource));

    [TestMethod]
    public void ShouldSerializeAnnotationIRIAssertion()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new RDFResource("ex:Obj"));
        string serializedXML = OWLSerializer.SerializeObject(annotationAssertion);

        Assert.IsTrue(string.Equals(serializedXML,
            """<AnnotationAssertion><AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" /><IRI>ex:Subj</IRI><IRI>ex:Obj</IRI></AnnotationAssertion>"""));
    }

    [TestMethod]
    public void ShouldSerializeAnnotationIRIAssertionViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.AnnotationAxioms.Add(
            new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj")));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><AnnotationAssertion><AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" /><IRI>ex:Subj</IRI><IRI>ex:Obj</IRI></AnnotationAssertion></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeAnnotationIRIAssertion()
    {
        OWLAnnotationAssertion annotationAssertion = OWLSerializer.DeserializeObject<OWLAnnotationAssertion>(
            """
            <AnnotationAssertion>
              <AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" />
              <IRI>ex:Subj</IRI>
              <IRI>ex:Obj</IRI>
            </AnnotationAssertion>
            """);

        Assert.IsNotNull(annotationAssertion);
        Assert.IsNotNull(annotationAssertion.AnnotationProperty);
        Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
        Assert.IsNotNull(annotationAssertion.SubjectIRI);
        Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
        Assert.IsNotNull(annotationAssertion.ValueIRI);
        Assert.IsTrue(string.Equals(annotationAssertion.ValueIRI, "ex:Obj"));
    }

    [TestMethod]
    public void ShouldDeserializeAnnotationIRIAssertionViaOntology()
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
              <AnnotationAssertion>
                <AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" />
                <IRI>ex:Subj</IRI>
                <IRI>ex:Obj</IRI>
              </AnnotationAssertion>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.AnnotationAxioms.Count);
        Assert.IsTrue(ontology.AnnotationAxioms.Single() is OWLAnnotationAssertion annAsn
                      && string.Equals(annAsn.SubjectIRI, "ex:Subj")
                      && string.Equals(annAsn.ValueIRI, "ex:Obj")
                      && string.Equals(annAsn.AnnotationProperty.IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
    }

    [TestMethod]
    public void ShouldCreateAnnotationLiteralAssertion()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new OWLLiteral(new RDFPlainLiteral("hello", "en")));

        Assert.IsNotNull(annotationAssertion);
        Assert.IsNotNull(annotationAssertion.AnnotationProperty);
        Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
        Assert.IsNotNull(annotationAssertion.SubjectIRI);
        Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
        Assert.IsNotNull(annotationAssertion.ValueLiteral);
        Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Value, "hello"));
        Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnnotationLiteralAssertionBecauseNullAnnotationProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationAssertion(
            null,
            new RDFResource("ex:Subj"),
            new OWLLiteral(new RDFPlainLiteral("hello", "en"))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnnotationLiteralAssertionBecauseNullSubjectIRI()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            null,
            new OWLLiteral(new RDFPlainLiteral("hello", "en"))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingAnnotationLiteralAssertionBecauseNullValueLiteral()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            null as OWLLiteral));

    [TestMethod]
    public void ShouldSerializeAnnotationLiteralAssertion()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new OWLLiteral(new RDFPlainLiteral("hello", "en")));
        string serializedXML = OWLSerializer.SerializeObject(annotationAssertion);

        Assert.IsTrue(string.Equals(serializedXML,
            """<AnnotationAssertion><AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" /><IRI>ex:Subj</IRI><Literal xml:lang="EN">hello</Literal></AnnotationAssertion>"""));
    }

    [TestMethod]
    public void ShouldSerializeAnnotationLiteralAssertionViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.AnnotationAxioms.Add(
            new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><AnnotationAssertion><AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" /><IRI>ex:Subj</IRI><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#string">hello</Literal></AnnotationAssertion></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeAnnotationLiteralAssertion()
    {
        OWLAnnotationAssertion annotationAssertion = OWLSerializer.DeserializeObject<OWLAnnotationAssertion>(
            """
            <AnnotationAssertion>
              <AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" />
              <IRI>ex:Subj</IRI>
              <Literal xml:lang="EN">hello</Literal>
            </AnnotationAssertion>
            """);

        Assert.IsNotNull(annotationAssertion);
        Assert.IsNotNull(annotationAssertion.AnnotationProperty);
        Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
        Assert.IsNotNull(annotationAssertion.SubjectIRI);
        Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
        Assert.IsNotNull(annotationAssertion.ValueLiteral);
        Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Value, "hello"));
        Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldDeserializeAnnotationLiteralAssertionViaOntology()
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
              <AnnotationAssertion>
                <AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" />
                <IRI>ex:Subj</IRI>
                <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#string">hello</Literal>
              </AnnotationAssertion>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.AnnotationAxioms.Count);
        Assert.IsTrue(ontology.AnnotationAxioms.Single() is OWLAnnotationAssertion annAsn
                      && string.Equals(annAsn.SubjectIRI, "ex:Subj")
                      && string.Equals(annAsn.ValueLiteral.Value, "hello")
                      && string.Equals(annAsn.ValueLiteral.DatatypeIRI, "http://www.w3.org/2001/XMLSchema#string")
                      && string.Equals(annAsn.AnnotationProperty.IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
    }

    [TestMethod]
    public void ShouldSerializeMultipleAndNestedAnnotationAssertionsViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.AnnotationAxioms.Add(
            new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj"))
            {
                Annotations =
                [
                    new OWLAnnotation(
                        new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION),
                        new RDFResource("ex:AnnValue"))
                    {
                        Annotation = new OWLAnnotation(
                            new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR),
                            new OWLLiteral(new RDFPlainLiteral("contributor", "en-us--rtl")))
                    }
                ]
            });
        ontology.AnnotationAxioms.Add(
            new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><AnnotationAssertion><Annotation><Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" /><Literal xml:lang="EN-US--RTL">contributor</Literal></Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/description" /><IRI>ex:AnnValue</IRI></Annotation><AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" /><IRI>ex:Subj</IRI><IRI>ex:Obj</IRI></AnnotationAssertion><AnnotationAssertion><AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" /><IRI>ex:Subj</IRI><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#string">hello</Literal></AnnotationAssertion></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeMultipleAndNestedAnnotationAssertionsViaOntology()
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
              <AnnotationAssertion>
                <Annotation>
                  <Annotation>
                    <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                    <Literal xml:lang="EN-US--RTL">contributor</Literal>
                  </Annotation>
                  <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/description" />
                  <IRI>ex:AnnValue</IRI>
                </Annotation>
                <AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" />
                <IRI>ex:Subj</IRI>
                <IRI>ex:Obj</IRI>
              </AnnotationAssertion>
              <AnnotationAssertion>
                <AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#comment" />
                <IRI>ex:Subj</IRI>
                <IRI>ex:Obj2</IRI>
              </AnnotationAssertion>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(2, ontology.AnnotationAxioms.Count);
        Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLAnnotationAssertion annAsn
                                                              && string.Equals(annAsn.SubjectIRI, "ex:Subj")
                                                              && string.Equals(annAsn.ValueIRI, "ex:Obj")
                                                              && string.Equals(annAsn.AnnotationProperty.IRI, "http://www.w3.org/2000/01/rdf-schema#comment")
                                                              && string.Equals(annAsn.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/description")
                                                              && string.Equals(annAsn.Annotations.Single().ValueIRI, "ex:AnnValue")
                                                              && string.Equals(annAsn.Annotations.Single().Annotation.AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                                                              && string.Equals(annAsn.Annotations.Single().Annotation.ValueLiteral.Value, "contributor")
                                                              && string.Equals(annAsn.Annotations.Single().Annotation.ValueLiteral.Language, "EN-US--RTL")));
        Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLAnnotationAssertion annAsn
                                                              && string.Equals(annAsn.SubjectIRI, "ex:Subj")
                                                              && string.Equals(annAsn.ValueIRI, "ex:Obj2")
                                                              && string.Equals(annAsn.AnnotationProperty.IRI, "http://www.w3.org/2000/01/rdf-schema#comment")));
    }

    [TestMethod]
    public void ShouldConvertAnnotationIRIAssertionToGraph()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new RDFResource("ex:Obj"));
        RDFGraph graph = annotationAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(2, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Subj"), RDFVocabulary.RDFS.COMMENT, new RDFResource("ex:Obj"), null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertAnnotationLiteralAssertionToGraph()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new OWLLiteral(new RDFPlainLiteral("hello", "en")));
        RDFGraph graph = annotationAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(2, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Subj"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("hello", "en")].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertAnnotationIRIAssertionWithAnnotationToGraph()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new RDFResource("ex:Obj"))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = annotationAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(8, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Subj"), RDFVocabulary.RDFS.COMMENT, new RDFResource("ex:Obj"), null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:Subj"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.COMMENT, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:Obj"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertAnnotationLiteralAssertionWithAnnotationToGraph()
    {
        OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
            new RDFResource("ex:Subj"),
            new OWLLiteral(new RDFPlainLiteral("hello", "en--ltr")))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = annotationAssertion.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(8, graph.TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Subj"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("hello", "en--ltr")].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:Subj"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.COMMENT, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, new RDFPlainLiteral("hello", "en--ltr")].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}