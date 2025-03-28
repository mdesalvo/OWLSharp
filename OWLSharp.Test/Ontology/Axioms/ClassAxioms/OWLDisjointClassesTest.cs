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
public class OWLDisjointClassesTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateDisjointClasses()
    {
        OWLDisjointClasses disjointClasses = new OWLDisjointClasses(
            [ new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]);

        Assert.IsNotNull(disjointClasses);
        Assert.IsNotNull(disjointClasses.ClassExpressions);
        Assert.IsTrue(string.Equals(((OWLClass)disjointClasses.ClassExpressions[0]).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
        Assert.IsTrue(string.Equals(((OWLClass)disjointClasses.ClassExpressions[1]).IRI, RDFVocabulary.FOAF.ORGANIZATION.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDisjointClassesBecauseNullClassExpressions()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDisjointClasses(null));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDisjointClassesBecauseLessThan2CLassExpressions()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDisjointClasses(
            [ new OWLClass(RDFVocabulary.FOAF.AGENT) ]));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDisjointClassesBecauseFoundNullClassExpression()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDisjointClasses(
            [ new OWLClass(RDFVocabulary.FOAF.AGENT), null ]));

    [TestMethod]
    public void ShouldSerializeDisjointClasses()
    {
        OWLDisjointClasses disjointClasses = new OWLDisjointClasses(
            [ new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]);
        string serializedXML = OWLSerializer.SerializeObject(disjointClasses);

        Assert.IsTrue(string.Equals(serializedXML,
            """<DisjointClasses><Class IRI="http://xmlns.com/foaf/0.1/Agent" /><Class IRI="http://xmlns.com/foaf/0.1/Organization" /></DisjointClasses>"""));
    }

    [TestMethod]
    public void ShouldSerializeDisjointClassesViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.ClassAxioms.Add(
            new OWLDisjointClasses(
                [ new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><DisjointClasses><Class IRI="http://xmlns.com/foaf/0.1/Agent" /><Class IRI="http://xmlns.com/foaf/0.1/Organization" /></DisjointClasses></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeDisjointClasses()
    {
        OWLDisjointClasses disjointClasses = OWLSerializer.DeserializeObject<OWLDisjointClasses>(
            """
            <DisjointClasses>
              <Class IRI="http://xmlns.com/foaf/0.1/Agent" />
              <Class IRI="http://xmlns.com/foaf/0.1/Organization" />
            </DisjointClasses>
            """);

        Assert.IsNotNull(disjointClasses);
        Assert.IsNotNull(disjointClasses.ClassExpressions);
        Assert.IsTrue(string.Equals(((OWLClass)disjointClasses.ClassExpressions[0]).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
        Assert.IsTrue(string.Equals(((OWLClass)disjointClasses.ClassExpressions[1]).IRI, RDFVocabulary.FOAF.ORGANIZATION.ToString()));
    }

    [TestMethod]
    public void ShouldDeserializeDisjointClassesViaOntology()
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
              <DisjointClasses>
                <Annotation>
                    <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                    <Literal xml:lang="EN">Steve</Literal>
                </Annotation>
                <Class IRI="http://xmlns.com/foaf/0.1/Agent" />
                <Class IRI="http://xmlns.com/foaf/0.1/Organization" />
              </DisjointClasses>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.AreEqual(1, ontology.ClassAxioms.Count);
        Assert.IsTrue(ontology.ClassAxioms.Single() is OWLDisjointClasses djclsAsn
                      && string.Equals(((OWLClass)djclsAsn.ClassExpressions[0]).IRI, "http://xmlns.com/foaf/0.1/Agent")
                      && string.Equals(((OWLClass)djclsAsn.ClassExpressions[1]).IRI, "http://xmlns.com/foaf/0.1/Organization"));
        Assert.IsTrue(ontology.ClassAxioms.Single() is OWLDisjointClasses djclsAsn1
                      && string.Equals(djclsAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                      && string.Equals(djclsAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                      && string.Equals(djclsAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
    }

    [TestMethod]
    public void ShouldConvert2DisjointClassesToGraph()
    {
        OWLDisjointClasses disjointClasses = new OWLDisjointClasses(
            [ new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]);
        RDFGraph graph = disjointClasses.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(3, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.OWL.DISJOINT_WITH, RDFVocabulary.FOAF.ORGANIZATION, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.ORGANIZATION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvert3DisjointClassesToGraph()
    {
        OWLDisjointClasses disjointClasses = new OWLDisjointClasses(
        [ new OWLClass(RDFVocabulary.FOAF.AGENT),
            new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
            new OWLClass(RDFVocabulary.FOAF.PERSON) ]);
        RDFGraph graph = disjointClasses.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(14, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.MEMBERS, null, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.ORGANIZATION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.ORGANIZATION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvert2DisjointClassesWithAnnotationToGraph()
    {
        OWLDisjointClasses disjointClasses = new OWLDisjointClasses(
            [new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)])
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = disjointClasses.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(9, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.OWL.DISJOINT_WITH, RDFVocabulary.FOAF.ORGANIZATION, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.ORGANIZATION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.DISJOINT_WITH, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.ORGANIZATION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvert3DisjointClassesWithAnnotationToGraph()
    {
        OWLDisjointClasses disjointClasses = new OWLDisjointClasses(
        [ new OWLClass(RDFVocabulary.FOAF.AGENT),
            new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
            new OWLClass(RDFVocabulary.FOAF.PERSON) ])
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = disjointClasses.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(16, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.MEMBERS, null, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.ORGANIZATION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.ORGANIZATION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}