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
    public class OWLObjectPropertyAssertionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateNamedIndividualObjectPropertyAssertion()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));

            Assert.IsNotNull(objectPropertyAssertion);
            Assert.IsNotNull(objectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyAssertion.ObjectPropertyExpression is OWLObjectProperty objProp &&
                            string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.SourceIndividualExpression is OWLNamedIndividual srcIdv &&
                            string.Equals(srcIdv.IRI, "ex:Alice"));
            Assert.IsNotNull(objectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.TargetIndividualExpression is OWLNamedIndividual dstIdv &&
                            string.Equals(dstIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualObjectPropertyAssertionBecauseNullObjectProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectPropertyAssertion(
                null,
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualObjectPropertyAssertionBecauseSourceIndividual()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.AGE),
                null,
                new OWLNamedIndividual(new RDFResource("ex:Bob"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualObjectPropertyAssertionBecauseNullTargetIndividual()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.AGE),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                null));

        [TestMethod]
        public void ShouldSerializeNamedIndividualObjectPropertyAssertion()
        {
             OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(objectPropertyAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
"""<ObjectPropertyAssertion><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /><NamedIndividual IRI="ex:Alice" /><NamedIndividual IRI="ex:Bob" /></ObjectPropertyAssertion>"""));
        }

        [TestMethod]
        public void ShouldSerializeNamedIndividualObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AssertionAxioms.Add(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:Alice")),
                    new OWLNamedIndividual(new RDFResource("ex:Bob")))
                    {
                        Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
                    });
            string serializedXML = OWLSerializer.SerializeObject(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
"""<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><ObjectPropertyAssertion><Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" /><Literal xml:lang="EN">Steve</Literal></Annotation><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /><NamedIndividual IRI="ex:Alice" /><NamedIndividual IRI="ex:Bob" /></ObjectPropertyAssertion></Ontology>"""));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualObjectPropertyAssertion()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = OWLSerializer.DeserializeObject<OWLObjectPropertyAssertion>(
"""<ObjectPropertyAssertion><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /><NamedIndividual IRI="ex:Alice" /><NamedIndividual IRI="ex:Bob" /></ObjectPropertyAssertion>""");
        
            Assert.IsNotNull(objectPropertyAssertion);
            Assert.IsNotNull(objectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyAssertion.ObjectPropertyExpression is OWLObjectProperty objProp &&
                            string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.SourceIndividualExpression is OWLNamedIndividual srcIdv &&
                            string.Equals(srcIdv.IRI, "ex:Alice"));
            Assert.IsNotNull(objectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.TargetIndividualExpression is OWLNamedIndividual dstIdv &&
                            string.Equals(dstIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualObjectPropertyAssertionViaOntology()
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
                    <ObjectPropertyAssertion>
                        <Annotation>
                            <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                            <Literal xml:lang="EN">Steve</Literal>
                        </Annotation>
                        <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                        <NamedIndividual IRI="ex:Alice" />
                        <NamedIndividual IRI="ex:Bob" />
                    </ObjectPropertyAssertion>
                </Ontology>
                """);

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLObjectPropertyAssertion
                          {
                              ObjectPropertyExpression: OWLObjectProperty objProp
                          } opAsn
                          && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows")
                          && opAsn.SourceIndividualExpression is OWLNamedIndividual srcIdv
                          && string.Equals(srcIdv.IRI, "ex:Alice")
                          && opAsn.TargetIndividualExpression is OWLNamedIndividual dstIdv
                          && string.Equals(dstIdv.IRI, "ex:Bob"));
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLObjectPropertyAssertion dpAsn1
                            && string.Equals(dpAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(dpAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(dpAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldSerializeNamedIndividualInverseObjectPropertyAssertion()
        {
             OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(objectPropertyAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
"""<ObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf><NamedIndividual IRI="ex:Alice" /><NamedIndividual IRI="ex:Bob" /></ObjectPropertyAssertion>"""));
        }

        [TestMethod]
        public void ShouldSerializeNamedIndividualInverseObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AssertionAxioms.Add(
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLNamedIndividual(new RDFResource("ex:Alice")),
                    new OWLNamedIndividual(new RDFResource("ex:Bob")))
                    {
                        Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
                    });
            string serializedXML = OWLSerializer.SerializeObject(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
"""<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><ObjectPropertyAssertion><Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" /><Literal xml:lang="EN">Steve</Literal></Annotation><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf><NamedIndividual IRI="ex:Alice" /><NamedIndividual IRI="ex:Bob" /></ObjectPropertyAssertion></Ontology>"""));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualInverseObjectPropertyAssertion()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = OWLSerializer.DeserializeObject<OWLObjectPropertyAssertion>(
"""<ObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf><NamedIndividual IRI="ex:Alice" /><NamedIndividual IRI="ex:Bob" /></ObjectPropertyAssertion>""");
        
            Assert.IsNotNull(objectPropertyAssertion);
            Assert.IsNotNull(objectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyAssertion.ObjectPropertyExpression is OWLObjectInverseOf objInvOf &&
                            string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.SourceIndividualExpression is OWLNamedIndividual srcIdv &&
                            string.Equals(srcIdv.IRI, "ex:Alice"));
            Assert.IsNotNull(objectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.TargetIndividualExpression is OWLNamedIndividual dstIdv &&
                            string.Equals(dstIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeNamedIndividualInverseObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
                """
                <?xml version="1.0" encoding="UTF-8"?>
                <Ontology>
                  <Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" />
                  <Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" />
                  <Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" />
                  <Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" />
                  <Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" />
                  <ObjectPropertyAssertion>
                    <Annotation>
                      <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                      <Literal xml:lang="EN">Steve</Literal>
                    </Annotation>
                    <ObjectInverseOf>
                      <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                    </ObjectInverseOf>
                    <NamedIndividual IRI="ex:Alice" />
                    <NamedIndividual IRI="ex:Bob" />
                  </ObjectPropertyAssertion>
                </Ontology>
                """);

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLObjectPropertyAssertion
                          {
                              ObjectPropertyExpression: OWLObjectInverseOf objInvOf
                          } opAsn
                          && string.Equals(objInvOf.ObjectProperty.IRI, "http://xmlns.com/foaf/0.1/knows")
                          && opAsn.SourceIndividualExpression is OWLNamedIndividual srcIdv
                          && string.Equals(srcIdv.IRI, "ex:Alice")
                          && opAsn.TargetIndividualExpression is OWLNamedIndividual dstIdv
                          && string.Equals(dstIdv.IRI, "ex:Bob"));
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLObjectPropertyAssertion dpAsn1
                            && string.Equals(dpAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(dpAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(dpAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualInverseObjectPropertyAssertion()
        {
             OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLAnonymousIndividual("Alice"),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(objectPropertyAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
"""<ObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf><AnonymousIndividual nodeID="Alice" /><NamedIndividual IRI="ex:Bob" /></ObjectPropertyAssertion>"""));
        }

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualInverseObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AssertionAxioms.Add(
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLNamedIndividual(new RDFResource("ex:Alice")),
                    new OWLAnonymousIndividual("Bob"))
                    {
                        Annotations = [ new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR), new OWLLiteral(new RDFPlainLiteral("Steve","en"))) ]
                    });
            string serializedXML = OWLSerializer.SerializeObject(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
"""<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><ObjectPropertyAssertion><Annotation><AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" /><Literal xml:lang="EN">Steve</Literal></Annotation><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf><NamedIndividual IRI="ex:Alice" /><AnonymousIndividual nodeID="Bob" /></ObjectPropertyAssertion></Ontology>"""));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualInverseObjectPropertyAssertion()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = OWLSerializer.DeserializeObject<OWLObjectPropertyAssertion>(
"""<ObjectPropertyAssertion><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf><AnonymousIndividual nodeID="Alice" /><AnonymousIndividual nodeID="Bob" /></ObjectPropertyAssertion>""");
        
            Assert.IsNotNull(objectPropertyAssertion);
            Assert.IsNotNull(objectPropertyAssertion.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyAssertion.ObjectPropertyExpression is OWLObjectInverseOf objInvOf &&
                            string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyAssertion.SourceIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.SourceIndividualExpression is OWLAnonymousIndividual srcIdv &&
                            string.Equals(srcIdv.NodeID, "Alice"));
            Assert.IsNotNull(objectPropertyAssertion.TargetIndividualExpression);
            Assert.IsTrue(objectPropertyAssertion.TargetIndividualExpression is OWLAnonymousIndividual dstIdv &&
                            string.Equals(dstIdv.NodeID, "Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualInverseObjectPropertyAssertionViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
                """
                <?xml version="1.0" encoding="UTF-8"?>
                <Ontology>
                  <Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" />
                  <Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" />
                  <Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" />
                  <Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" />
                  <Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" />
                  <ObjectPropertyAssertion>
                    <Annotation>
                      <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                      <Literal xml:lang="EN">Steve</Literal>
                    </Annotation>
                    <ObjectInverseOf>
                      <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                    </ObjectInverseOf>
                    <AnonymousIndividual nodeID="Alice" />
                    <NamedIndividual IRI="ex:Bob" />
                  </ObjectPropertyAssertion>
                </Ontology>
                """);

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AssertionAxioms.Count);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLObjectPropertyAssertion
                          {
                              ObjectPropertyExpression: OWLObjectInverseOf objInvOf
                          } opAsn
                          && string.Equals(objInvOf.ObjectProperty.IRI, "http://xmlns.com/foaf/0.1/knows")
                          && opAsn.SourceIndividualExpression is OWLAnonymousIndividual srcIdv
                          && string.Equals(srcIdv.NodeID, "Alice")
                          && opAsn.TargetIndividualExpression is OWLNamedIndividual dstIdv
                          && string.Equals(dstIdv.IRI, "ex:Bob"));
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLObjectPropertyAssertion opAsn1
                            && string.Equals(opAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(opAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertObjectPropertyAssertionToGraph()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            RDFGraph graph = objectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(4, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertObjectPropertyAssertionWithObjectInverseOfToGraph()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            RDFGraph graph = objectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(4, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertObjectPropertyAssertionWithAnonymousIndividualToGraph()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLAnonymousIndividual("Alice"),
                new OWLNamedIndividual(new RDFResource("ex:Bob")));
            RDFGraph graph = objectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(3, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("bnode:Alice"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertObjectPropertyAssertionWithAnnotationToGraph()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = objectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(10, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);

        }

        [TestMethod]
        public void ShouldConvertObjectPropertyAssertionWithObjectInverseOfWithAnnotationToGraph()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLNamedIndividual(new RDFResource("ex:Alice")),
                new OWLNamedIndividual(new RDFResource("ex:Bob")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = objectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(10, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertObjectPropertyAssertionWithAnonymousIndividualWithAnnotationToGraph()
        {
            OWLObjectPropertyAssertion objectPropertyAssertion = new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLAnonymousIndividual("Alice"),
                new OWLNamedIndividual(new RDFResource("ex:Bob")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = objectPropertyAssertion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("bnode:Alice"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("bnode:Alice"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:Bob"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}