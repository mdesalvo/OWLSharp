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
public class OWLEquivalentObjectPropertiesTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateEquivalentObjectProperties()
    {
        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
            [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]);

        Assert.IsNotNull(equivalentObjectProperties);
        Assert.IsNotNull(equivalentObjectProperties.ObjectPropertyExpressions);
        Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[0]).IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[1]).IRI, RDFVocabulary.FOAF.MEMBER.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingEquivalentObjectPropertiesBecauseNullObjectProperties()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLEquivalentObjectProperties(null));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingEquivalentObjectPropertiesBecauseLessThan2ObjectProperties()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLEquivalentObjectProperties(
            [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ]));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingEquivalentObjectPropertiesBecauseFoundNullObjectProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLEquivalentObjectProperties(
            [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), null ]));

    [TestMethod]
    public void ShouldSerializeEquivalentObjectProperties()
    {
        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
            [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]);
        string serializedXML = OWLSerializer.SerializeObject(equivalentObjectProperties);

        Assert.IsTrue(string.Equals(serializedXML,
            """<EquivalentObjectProperties><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /><ObjectProperty IRI="http://xmlns.com/foaf/0.1/member" /></EquivalentObjectProperties>"""));
    }

    [TestMethod]
    public void ShouldSerializeEquivalentObjectPropertiesViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.ObjectPropertyAxioms.Add(
            new OWLEquivalentObjectProperties(
                [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><EquivalentObjectProperties><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /><ObjectProperty IRI="http://xmlns.com/foaf/0.1/member" /></EquivalentObjectProperties></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeEquivalentObjectProperties()
    {
        OWLEquivalentObjectProperties equivalentObjectProperties = OWLSerializer.DeserializeObject<OWLEquivalentObjectProperties>(
            """
            <EquivalentObjectProperties>
              <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
              <ObjectProperty IRI="http://xmlns.com/foaf/0.1/member" />
            </EquivalentObjectProperties>
            """);

        Assert.IsNotNull(equivalentObjectProperties);
        Assert.IsNotNull(equivalentObjectProperties.ObjectPropertyExpressions);
        Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[0]).IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[1]).IRI, RDFVocabulary.FOAF.MEMBER.ToString()));
    }

    [TestMethod]
    public void ShouldDeserializeEquivalentObjectPropertiesViaOntology()
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
              <EquivalentObjectProperties>
                <Annotation>
                    <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                    <Literal xml:lang="EN">Steve</Literal>
                </Annotation>
                <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                <ObjectProperty IRI="http://xmlns.com/foaf/0.1/member" />
              </EquivalentObjectProperties>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
        Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLEquivalentObjectProperties djObjProps
                      && string.Equals(((OWLObjectProperty)djObjProps.ObjectPropertyExpressions[0]).IRI, "http://xmlns.com/foaf/0.1/knows")
                      && string.Equals(((OWLObjectProperty)djObjProps.ObjectPropertyExpressions[1]).IRI, "http://xmlns.com/foaf/0.1/member"));
        Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLEquivalentObjectProperties djDtProps1
                      && string.Equals(djDtProps1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                      && string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Value, "Steve")
                      && string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldConvert2EquivalentObjectPropertiesToGraph()
    {
        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
            [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER)]);
        RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(3, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.MEMBER, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvert3EquivalentObjectPropertiesToGraph()
    {
        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
        [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
            new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER),
            new OWLObjectProperty(RDFVocabulary.FOAF.ACCOUNT)]);
        RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(6, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.ACCOUNT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.MEMBER, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.ACCOUNT, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.ACCOUNT, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvert2EquivalentObjectPropertiesWithAnnotationToGraph()
    {
        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
            [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER)])
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(9, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.MEMBER, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.MEMBER, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvert3EquivalentObjectPropertiesWithAnnotationToGraph()
    {
        OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
        [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
            new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER),
            new OWLObjectProperty(RDFVocabulary.FOAF.ACCOUNT)])
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(22, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.ACCOUNT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.MEMBER, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.ACCOUNT, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.ACCOUNT, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.MEMBER, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.MEMBER, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.ACCOUNT, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}