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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLFunctionalObjectPropertyTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateFunctionalObjectProperty()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

        Assert.IsNotNull(functionalObjectProperty);
        Assert.IsNotNull(functionalObjectProperty.ObjectPropertyExpression);
        Assert.IsTrue(functionalObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                      && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
    }

    [TestMethod]
    public void ShouldCreateFunctionalObjectInverseOf()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

        Assert.IsNotNull(functionalObjectProperty);
        Assert.IsNotNull(functionalObjectProperty.ObjectPropertyExpression);
        Assert.IsTrue(functionalObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                      && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingFunctionalObjectPropertyBecauseNullObjectProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLFunctionalObjectProperty(null as OWLObjectProperty));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingFunctionalObjectPropertyBecauseNullObjectInverseOf()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLFunctionalObjectProperty(null as OWLObjectInverseOf));

    [TestMethod]
    public void ShouldSerializeFunctionalObjectProperty()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
        string serializedXML = OWLSerializer.SerializeObject(functionalObjectProperty);

        Assert.IsTrue(string.Equals(serializedXML,
            """<FunctionalObjectProperty><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></FunctionalObjectProperty>"""));
    }

    [TestMethod]
    public void ShouldSerializeFunctionalObjectInverseOf()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
        string serializedXML = OWLSerializer.SerializeObject(functionalObjectProperty);

        Assert.IsTrue(string.Equals(serializedXML,
            """<FunctionalObjectProperty><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf></FunctionalObjectProperty>"""));
    }

    [TestMethod]
    public void ShouldSerializeFunctionalObjectPropertyViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.ObjectPropertyAxioms.Add(
            new OWLFunctionalObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><FunctionalObjectProperty><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></FunctionalObjectProperty></Ontology>"""));
    }

    [TestMethod]
    public void ShouldSerializeFunctionalObjectInverseOfViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.ObjectPropertyAxioms.Add(
            new OWLFunctionalObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><FunctionalObjectProperty><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf></FunctionalObjectProperty></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeFunctionalObjectProperty()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = OWLSerializer.DeserializeObject<OWLFunctionalObjectProperty>(
            """
            <FunctionalObjectProperty>
              <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
            </FunctionalObjectProperty>
            """);

        Assert.IsNotNull(functionalObjectProperty);
        Assert.IsNotNull(functionalObjectProperty.ObjectPropertyExpression);
        Assert.IsTrue(functionalObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                      && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
    }

    [TestMethod]
    public void ShouldDeserializeFunctionalObjectInverseOf()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = OWLSerializer.DeserializeObject<OWLFunctionalObjectProperty>(
            """
            <FunctionalObjectProperty>
              <ObjectInverseOf>
                <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
              </ObjectInverseOf>
            </FunctionalObjectProperty>
            """);

        Assert.IsNotNull(functionalObjectProperty);
        Assert.IsNotNull(functionalObjectProperty.ObjectPropertyExpression);
        Assert.IsTrue(functionalObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                      && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
    }

    [TestMethod]
    public void ShouldDeserializeFunctionalObjectPropertyViaOntology()
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
              <FunctionalObjectProperty>
                <Annotation>
                  <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                  <Literal xml:lang="EN">Steve</Literal>
                </Annotation>
                <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
              </FunctionalObjectProperty>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
        Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLFunctionalObjectProperty
        {
            ObjectPropertyExpression: OWLObjectProperty objProp
        } && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLFunctionalObjectProperty funcObjProp1
                      && string.Equals(funcObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                      && string.Equals(funcObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                      && string.Equals(funcObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldDeserializeFunctionalObjectInverseOfViaOntology()
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
              <FunctionalObjectProperty>
                <Annotation>
                  <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                  <Literal xml:lang="EN">Steve</Literal>
                </Annotation>
                <ObjectInverseOf>
                  <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                </ObjectInverseOf>
              </FunctionalObjectProperty>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
        Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLFunctionalObjectProperty
        {
            ObjectPropertyExpression: OWLObjectInverseOf objInvOf
        } && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLFunctionalObjectProperty funcObjProp1
                      && string.Equals(funcObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                      && string.Equals(funcObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                      && string.Equals(funcObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldConvertFunctionalObjectPropertyToGraph()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
        RDFGraph graph = functionalObjectProperty.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(2, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertFunctionalObjectPropertyWithAnnotationToGraph()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = functionalObjectProperty.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(8, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertFunctionalObjectInverseOfToGraph()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
        RDFGraph graph = functionalObjectProperty.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(3, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertFunctionalObjectInverseOfWithAnnotationToGraph()
    {
        OWLFunctionalObjectProperty functionalObjectProperty = new OWLFunctionalObjectProperty(
            new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = functionalObjectProperty.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(9, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}