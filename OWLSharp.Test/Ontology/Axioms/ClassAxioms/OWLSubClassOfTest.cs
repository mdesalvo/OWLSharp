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
public class OWLSubClassOfTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateSubClassOf()
    {
        OWLSubClassOf subClassOf = new OWLSubClassOf(
            new OWLClass(RDFVocabulary.FOAF.PERSON),
            new OWLClass(RDFVocabulary.FOAF.AGENT));

        Assert.IsNotNull(subClassOf);
        Assert.IsNotNull(subClassOf.SubClassExpression);
        Assert.IsTrue(string.Equals(((OWLClass)subClassOf.SubClassExpression).IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        Assert.IsNotNull(subClassOf.SuperClassExpression);
        Assert.IsTrue(string.Equals(((OWLClass)subClassOf.SuperClassExpression).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingSubClassOfBecauseNullSubClassExpression()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLSubClassOf(
            null,
            new OWLClass(RDFVocabulary.FOAF.AGENT)));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingSubClassOfBecauseNullSuperClassExpression()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLSubClassOf(
            new OWLClass(RDFVocabulary.FOAF.PERSON),
            null));

    [TestMethod]
    public void ShouldSerializeSubClassOf()
    {
        OWLSubClassOf SubClassOf = new OWLSubClassOf(
            new OWLClass(RDFVocabulary.FOAF.PERSON),
            new OWLClass(RDFVocabulary.FOAF.AGENT));
        string serializedXML = OWLSerializer.SerializeObject(SubClassOf);

        Assert.IsTrue(string.Equals(serializedXML,
            """<SubClassOf><Class IRI="http://xmlns.com/foaf/0.1/Person" /><Class IRI="http://xmlns.com/foaf/0.1/Agent" /></SubClassOf>"""));
    }

    [TestMethod]
    public void ShouldSerializeSubClassOfViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.ClassAxioms.Add(
            new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT)));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><SubClassOf><Class IRI="http://xmlns.com/foaf/0.1/Person" /><Class IRI="http://xmlns.com/foaf/0.1/Agent" /></SubClassOf></Ontology>"""));
    }

    [TestMethod]
    public void ShouldDeserializeSubClassOf()
    {
        OWLSubClassOf subClassOf = OWLSerializer.DeserializeObject<OWLSubClassOf>(
            """
            <SubClassOf>
              <Class IRI="http://xmlns.com/foaf/0.1/Person" />
              <Class IRI="http://xmlns.com/foaf/0.1/Agent" />
            </SubClassOf>
            """);

        Assert.IsNotNull(subClassOf);
        Assert.IsNotNull(subClassOf.SubClassExpression);
        Assert.IsTrue(string.Equals(((OWLClass)subClassOf.SubClassExpression).IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        Assert.IsNotNull(subClassOf.SuperClassExpression);
        Assert.IsTrue(string.Equals(((OWLClass)subClassOf.SuperClassExpression).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
    }

    [TestMethod]
    public void ShouldDeserializeSubClassOfViaOntology()
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
              <SubClassOf>
                <Class IRI="http://xmlns.com/foaf/0.1/Person" />
                <Class IRI="http://xmlns.com/foaf/0.1/Agent" />
              </SubClassOf>
            </Ontology>
            """);

        Assert.IsNotNull(ontology);
        Assert.HasCount(1, ontology.ClassAxioms);
        Assert.IsTrue(ontology.ClassAxioms.Single() is OWLSubClassOf subcAsn
                      && string.Equals(((OWLClass)subcAsn.SubClassExpression).IRI, RDFVocabulary.FOAF.PERSON.ToString())
                      && string.Equals(((OWLClass)subcAsn.SuperClassExpression).IRI, RDFVocabulary.FOAF.AGENT.ToString()));
    }

    [TestMethod]
    public void ShouldSerializeMultipleAndNestedSubClassOfViaOntology()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
        ontology.ClassAxioms.Add(
            new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT))
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
        ontology.ClassAxioms.Add(
            new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new OWLClass(RDFVocabulary.OWL.THING)));
        string serializedXML = OWLSerializer.SerializeObject(ontology);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><Prefix name="foaf" IRI="http://xmlns.com/foaf/0.1/" /><SubClassOf><Annotation><Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" /><Literal xml:lang="EN-US--RTL">contributor</Literal></Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/description" /><IRI>ex:AnnValue</IRI></Annotation><Class IRI="http://xmlns.com/foaf/0.1/Person" /><Class IRI="http://xmlns.com/foaf/0.1/Agent" /></SubClassOf><SubClassOf><Class IRI="http://xmlns.com/foaf/0.1/Agent" /><Class IRI="http://www.w3.org/2002/07/owl#Thing" /></SubClassOf></Ontology>"""));
    }

    [TestMethod]
    public void ShouldConvertSubClassOfToGraph()
    {
        OWLSubClassOf subClassOf = new OWLSubClassOf(
            new OWLClass(RDFVocabulary.FOAF.PERSON),
            new OWLClass(RDFVocabulary.FOAF.AGENT));
        RDFGraph graph = subClassOf.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(3, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDFS.SUB_CLASS_OF, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertSubClassOfWithAnnotationToGraph()
    {
        OWLSubClassOf subClassOf = new OWLSubClassOf(
            new OWLClass(RDFVocabulary.FOAF.PERSON),
            new OWLClass(RDFVocabulary.FOAF.AGENT))
        {
            Annotations = [
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
            ]
        };
        RDFGraph graph = subClassOf.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(9, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDFS.SUB_CLASS_OF, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        //Annotations
        Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.SUB_CLASS_OF, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
    }
    #endregion
}