/*
  Copyright 2014-2026 Marco De Salvo

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
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLAnnotationAssertionTest
{
    #region Tests
    [TestMethod]
    public void ShouldSerializeToFunctionalWithIRIValue()
    {
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLClass topping = new OWLClass(new RDFResource("http://example.org/pz#Topping"));
        OWLAnnotationAssertion axiom = new OWLAnnotationAssertion(note, pizza.GetIRI(), topping.GetIRI());

        string functionalString = axiom.ToFunctionalString(CreateContext());

        Assert.AreEqual("AnnotationAssertion( pz:note pz:Pizza pz:Topping )", functionalString);
    }

    [TestMethod]
    public void ShouldSerializeToFunctionalWithLiteralValue()
    {
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLAnnotationAssertion axiom = new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), pizza.GetIRI(), new OWLLiteral(new RDFPlainLiteral("A tasty dish")));

        string functionalString = axiom.ToFunctionalString(CreateContext());

        Assert.AreEqual("AnnotationAssertion( rdfs:comment pz:Pizza \"A tasty dish\" )", functionalString);
    }

    [TestMethod]
    public void ShouldSerializeToManchester()
    {
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLAnnotationAssertion axiom = new OWLAnnotationAssertion(note, pizza.GetIRI(), new OWLLiteral(new RDFPlainLiteral("A tasty dish")));

        OWLManchesterFrameItem frameItem = axiom.ToManchesterFrameItem(CreateManchesterContext());

        Assert.IsNotNull(frameItem);
        Assert.AreEqual(OWLManchesterFrameKind.EntityAnnotation, frameItem.FrameKind);
        Assert.AreEqual("pz:Pizza", frameItem.EntityName);
        Assert.AreEqual("Annotations:", frameItem.SectionKeyword);
        Assert.AreEqual("pz:note \"A tasty dish\"", frameItem.ItemText);
    }
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
        Assert.HasCount(1, ontology.AnnotationAxioms);
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
        Assert.HasCount(1, ontology.AnnotationAxioms);
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
        Assert.HasCount(2, ontology.AnnotationAxioms);
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

    #region Tests (anonymous individuals)
    //Verifies that Functional-Style writes the blank node subject or value of an assertion as the anonymous individual _:x (never as <bnode:x>) in the six combinations
    [TestMethod]
    public void ShouldSerializeToFunctionalWithAnonymousIndividuals()
    {
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        RDFResource pizza = new RDFResource("http://example.org/pz#Pizza");
        RDFResource topping = new RDFResource("http://example.org/pz#Topping");
        RDFResource anonymousSubject = new OWLAnonymousIndividual("a").GetIRI();
        RDFResource anonymousValue = new OWLAnonymousIndividual("b").GetIRI();
        OWLLiteral literal = new OWLLiteral(new RDFPlainLiteral("x"));
        OWLFunctionalContext functionalContext = CreateContext();

        Assert.AreEqual("AnnotationAssertion( pz:note pz:Pizza _:b )", new OWLAnnotationAssertion(note, pizza, anonymousValue).ToFunctionalString(functionalContext));
        Assert.AreEqual("AnnotationAssertion( pz:note _:a pz:Topping )", new OWLAnnotationAssertion(note, anonymousSubject, topping).ToFunctionalString(functionalContext));
        Assert.AreEqual("AnnotationAssertion( pz:note _:a \"x\" )", new OWLAnnotationAssertion(note, anonymousSubject, literal).ToFunctionalString(functionalContext));
        Assert.AreEqual("AnnotationAssertion( pz:note _:a _:b )", new OWLAnnotationAssertion(note, anonymousSubject, anonymousValue).ToFunctionalString(functionalContext));
    }

    //Verifies that Manchester gives a blank node subject the Individual frame of its own and writes a blank node value as _:y
    [TestMethod]
    public void ShouldSerializeToManchesterWithAnonymousIndividuals()
    {
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        OWLManchesterContext manchesterContext = CreateManchesterContext();

        OWLManchesterFrameItem anonymousSubjectItem = new OWLAnnotationAssertion(note, new OWLAnonymousIndividual("a").GetIRI(), new OWLLiteral(new RDFPlainLiteral("x"))).ToManchesterFrameItem(manchesterContext);
        OWLManchesterFrameItem anonymousValueItem = new OWLAnnotationAssertion(note, new RDFResource("http://example.org/pz#Pizza"), new OWLAnonymousIndividual("b").GetIRI()).ToManchesterFrameItem(manchesterContext);

        Assert.AreEqual(OWLManchesterFrameKind.Individual, anonymousSubjectItem.FrameKind);
        Assert.AreEqual("_:a", anonymousSubjectItem.EntityName);
        Assert.AreEqual("Annotations:", anonymousSubjectItem.SectionKeyword);
        Assert.AreEqual("pz:note \"x\"", anonymousSubjectItem.ItemText);
        Assert.AreEqual(OWLManchesterFrameKind.EntityAnnotation, anonymousValueItem.FrameKind);
        Assert.AreEqual("pz:Pizza", anonymousValueItem.EntityName);
        Assert.AreEqual("pz:note _:b", anonymousValueItem.ItemText);
    }

    //Verifies that the RDF export of an assertion with blank node subject or value yields the bnode: resource of the anonymous individual
    [TestMethod]
    public void ShouldConvertAnnotationAssertionWithAnonymousIndividualsToGraph()
    {
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        RDFResource anonymousSubject = new OWLAnonymousIndividual("a").GetIRI();
        RDFResource anonymousValue = new OWLAnonymousIndividual("b").GetIRI();

        RDFGraph anonymousToAnonymousGraph = new OWLAnnotationAssertion(note, anonymousSubject, anonymousValue).ToRDFGraph();
        RDFGraph anonymousToLiteralGraph = new OWLAnnotationAssertion(note, anonymousSubject, new OWLLiteral(new RDFPlainLiteral("x"))).ToRDFGraph();

        Assert.AreEqual("bnode:a", anonymousSubject.ToString());
        Assert.AreEqual(1, anonymousToAnonymousGraph[new RDFResource("bnode:a"), note.GetIRI(), new RDFResource("bnode:b"), null].TriplesCount);
        Assert.AreEqual(1, anonymousToLiteralGraph[new RDFResource("bnode:a"), note.GetIRI(), null, new RDFPlainLiteral("x")].TriplesCount);
    }

    //Verifies that a standard OWL2/XML document with anonymous individuals as subject or value of an annotation assertion is read as bnode: IRIs
    [TestMethod]
    public void ShouldDeserializeAnnotationAssertionWithAnonymousIndividualsViaOntology()
    {
        OWLOntology ontology = OWLSerializer.DeserializeOntology(
            """
            <?xml version="1.0" encoding="utf-8"?>
            <Ontology xmlns="http://www.w3.org/2002/07/owl#" ontologyIRI="http://example.org/pz">
              <AnnotationAssertion>
                <AnnotationProperty IRI="http://example.org/pz#note" />
                <AnonymousIndividual nodeID="a" />
                <AnonymousIndividual nodeID="b" />
              </AnnotationAssertion>
              <AnnotationAssertion>
                <AnnotationProperty IRI="http://example.org/pz#note" />
                <IRI>http://example.org/pz#Pizza</IRI>
                <AnonymousIndividual nodeID="c" />
              </AnnotationAssertion>
              <AnnotationAssertion>
                <Annotation>
                  <AnnotationProperty IRI="http://example.org/pz#meta" />
                  <AnonymousIndividual nodeID="m" />
                </Annotation>
                <AnnotationProperty IRI="http://example.org/pz#note" />
                <AnonymousIndividual nodeID="d" />
                <Literal>x</Literal>
              </AnnotationAssertion>
            </Ontology>
            """);

        Assert.HasCount(3, ontology.AnnotationAxioms);
        OWLAnnotationAssertion anonymousToAnonymous = (OWLAnnotationAssertion)ontology.AnnotationAxioms[0];
        Assert.AreEqual("bnode:a", anonymousToAnonymous.SubjectIRI);
        Assert.AreEqual("bnode:b", anonymousToAnonymous.ValueIRI);
        OWLAnnotationAssertion iriToAnonymous = (OWLAnnotationAssertion)ontology.AnnotationAxioms[1];
        Assert.AreEqual("http://example.org/pz#Pizza", iriToAnonymous.SubjectIRI);
        Assert.AreEqual("bnode:c", iriToAnonymous.ValueIRI);
        OWLAnnotationAssertion anonymousToLiteral = (OWLAnnotationAssertion)ontology.AnnotationAxioms[2];
        Assert.AreEqual("bnode:d", anonymousToLiteral.SubjectIRI);
        Assert.AreEqual("x", anonymousToLiteral.ValueLiteral.Value);
        //The anonymous individual of the annotation on the axiom has its own model field and is not turned into an IRI
        Assert.IsNull(anonymousToLiteral.Annotations.Single().ValueIRI);
        Assert.AreEqual("m", anonymousToLiteral.Annotations.Single().ValueAnonymousIndividual.NodeID);
    }

    //Verifies that the six subject x value combinations (with blank nodes) are written as IRI elements by OWL2/XML and read back identical
    [TestMethod]
    public void ShouldRoundTripAnnotationAssertionsWithAnonymousIndividualsThroughOWL2XML()
    {
        OWLOntology originalOntology = CreateOntologyWithAnonymousIndividualAssertions();

        string serializedXML = OWLSerializer.SerializeOntology(originalOntology);
        OWLOntology reparsedOntology = OWLSerializer.DeserializeOntology(serializedXML);

        Assert.IsTrue(serializedXML.Contains("<IRI>bnode:a</IRI>"));
        Assert.IsFalse(serializedXML.Contains("<AnonymousIndividual"));
        CollectionAssert.AreEqual(SortedXml(originalOntology), SortedXml(reparsedOntology));
    }

    //Verifies that the six subject x value combinations (with blank nodes) are written as _:x by Functional-Style and read back identical
    [TestMethod]
    public void ShouldRoundTripAnnotationAssertionsWithAnonymousIndividualsThroughFunctional()
    {
        OWLOntology originalOntology = CreateOntologyWithAnonymousIndividualAssertions();

        string functionalDocument = OWLFunctionalSerializer.SerializeOntology(originalOntology);
        OWLOntology reparsedOntology = OWLFunctionalParser.DeserializeOntology(functionalDocument);

        Assert.IsTrue(functionalDocument.Contains("AnnotationAssertion( pz:note _:a _:b )"));
        Assert.IsFalse(functionalDocument.Contains("bnode:"));
        CollectionAssert.AreEqual(SortedXml(originalOntology), SortedXml(reparsedOntology));
    }

    //Verifies that the six subject x value combinations (with blank nodes) are written by Manchester under the frame of the subject and read back identical
    [TestMethod]
    public void ShouldRoundTripAnnotationAssertionsWithAnonymousIndividualsThroughManchester()
    {
        OWLOntology originalOntology = CreateOntologyWithAnonymousIndividualAssertions();

        string manchesterDocument = OWLManchesterSerializer.SerializeOntology(originalOntology);
        OWLOntology reparsedOntology = OWLManchesterParser.DeserializeOntology(manchesterDocument);

        Assert.IsTrue(manchesterDocument.Contains("Individual: _:a"));
        Assert.IsTrue(manchesterDocument.Contains("pz:note _:b"));
        Assert.IsFalse(manchesterDocument.Contains("bnode:"));
        CollectionAssert.AreEqual(SortedXml(originalOntology), SortedXml(reparsedOntology));
    }
    #endregion

    #region Utilities
    private static OWLFunctionalContext CreateContext()
    {
        OWLOntology ontology = new OWLOntology(new Uri("http://example.org/pz"));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        return new OWLFunctionalContext(ontology.Prefixes);
    }

    //Builds an ontology with the six subject (named or blank node) x value (IRI, blank node or literal) combinations: the named subject is declared since Manchester resolves it against the declared entities
    private static OWLOntology CreateOntologyWithAnonymousIndividualAssertions()
    {
        OWLOntology ontology = new OWLOntology(new Uri("http://example.org/pz"));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        RDFResource pizza = new RDFResource("http://example.org/pz#Pizza");
        RDFResource topping = new RDFResource("http://example.org/pz#Topping");
        RDFResource anonymousSubject = new OWLAnonymousIndividual("a").GetIRI();
        RDFResource anonymousValue = new OWLAnonymousIndividual("b").GetIRI();
        OWLLiteral literal = new OWLLiteral(new RDFPlainLiteral("x"));

        ontology.DeclarationAxioms.Add(new OWLDeclaration(note));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLClass(pizza)));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(note, pizza, topping));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(note, pizza, literal));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(note, pizza, anonymousValue));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(note, anonymousSubject, topping));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(note, anonymousSubject, literal));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(note, anonymousSubject, anonymousValue));
        return ontology;
    }

    //Gets the annotation axioms of the given ontology as XML texts sorted by ordinal comparison
    private static List<string> SortedXml(OWLOntology ontology)
        => ontology.AnnotationAxioms.Select(axiom => axiom.GetXML()).OrderBy(xml => xml, StringComparer.Ordinal).ToList();

    private static OWLManchesterContext CreateManchesterContext()
    {
        OWLOntology ontology = new OWLOntology(new Uri("http://example.org/pz"));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        return new OWLManchesterContext(ontology.Prefixes);
    }
    #endregion
}