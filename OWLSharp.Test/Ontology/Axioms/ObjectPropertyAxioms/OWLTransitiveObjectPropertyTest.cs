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

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLTransitiveObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateTransitiveObjectProperty()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

            Assert.IsNotNull(transitiveObjectProperty);
            Assert.IsNotNull(transitiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(transitiveObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldCreateTransitiveObjectInverseOf()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

            Assert.IsNotNull(transitiveObjectProperty);
            Assert.IsNotNull(transitiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(transitiveObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingTransitiveObjectPropertyBecauseNullObjectProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLTransitiveObjectProperty(null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingTransitiveObjectPropertyBecauseNullObjectInverseOf()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLTransitiveObjectProperty(null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldSerializeTransitiveObjectProperty()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLSerializer.SerializeObject(transitiveObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
"""<TransitiveObjectProperty><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></TransitiveObjectProperty>"""));
        }

        [TestMethod]
        public void ShouldSerializeTransitiveObjectInverseOf()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.SerializeObject(transitiveObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
"""<TransitiveObjectProperty><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf></TransitiveObjectProperty>"""));
        }

        [TestMethod]
        public void ShouldSerializeTransitiveObjectPropertyViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLTransitiveObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.SerializeObject(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
"""<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><TransitiveObjectProperty><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></TransitiveObjectProperty></Ontology>"""));
        }

        [TestMethod]
        public void ShouldSerializeTransitiveObjectInverseOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLTransitiveObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))));
            string serializedXML = OWLSerializer.SerializeObject(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
"""<Ontology><Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" /><Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" /><Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" /><Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" /><Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" /><TransitiveObjectProperty><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf></TransitiveObjectProperty></Ontology>"""));
        }

        [TestMethod]
        public void ShouldDeserializeTransitiveObjectProperty()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = OWLSerializer.DeserializeObject<OWLTransitiveObjectProperty>(
                """
                <TransitiveObjectProperty>
                  <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                </TransitiveObjectProperty>
                """);

            Assert.IsNotNull(transitiveObjectProperty);
            Assert.IsNotNull(transitiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(transitiveObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeTransitiveObjectInverseOf()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = OWLSerializer.DeserializeObject<OWLTransitiveObjectProperty>(
                """
                <TransitiveObjectProperty>
                  <ObjectInverseOf>
                    <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                  </ObjectInverseOf>
                </TransitiveObjectProperty>
                """);

            Assert.IsNotNull(transitiveObjectProperty);
            Assert.IsNotNull(transitiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(transitiveObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeTransitiveObjectPropertyViaOntology()
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
                  <TransitiveObjectProperty>
                    <Annotation>
                      <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                      <Literal xml:lang="EN">Steve</Literal>
                    </Annotation>
                    <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                  </TransitiveObjectProperty>
                </Ontology>
                """);

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLTransitiveObjectProperty
                          {
                              ObjectPropertyExpression: OWLObjectProperty objProp
                          } && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLTransitiveObjectProperty transObjProp1
                            && string.Equals(transObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(transObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(transObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldDeserializeTransitiveObjectInverseOfViaOntology()
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
                  <TransitiveObjectProperty>
                    <Annotation>
                      <AnnotationProperty IRI="http://purl.org/dc/elements/1.1/contributor" />
                      <Literal xml:lang="EN">Steve</Literal>
                    </Annotation>
                    <ObjectInverseOf>
                      <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                    </ObjectInverseOf>
                  </TransitiveObjectProperty>
                </Ontology>
                """);

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLTransitiveObjectProperty
                          {
                              ObjectPropertyExpression: OWLObjectInverseOf objInvOf
                          } && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLTransitiveObjectProperty transObjProp1
                            && string.Equals(transObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(transObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(transObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertTransitiveObjectPropertyToGraph()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            RDFGraph graph = transitiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(2, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertTransitiveObjectPropertyWithAnnotationToGraph()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = transitiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(8, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertTransitiveObjectInverseOfToGraph()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            RDFGraph graph = transitiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(3, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertTransitiveObjectInverseOfWithAnnotationToGraph()
        {
            OWLTransitiveObjectProperty transitiveObjectProperty = new OWLTransitiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = transitiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}