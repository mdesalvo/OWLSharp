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
    public class OWLInverseObjectPropertiesTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateInverseObjectProperties()
        {
            OWLInverseObjectProperties inverseObjectProperties = new OWLInverseObjectProperties(
                new OWLObjectProperty(new RDFResource("ex:hasWife")),
                new OWLObjectProperty(new RDFResource("ex:isWifeOf")));

            Assert.IsNotNull(inverseObjectProperties);
            Assert.IsNotNull(inverseObjectProperties.LeftObjectPropertyExpression);
            Assert.IsTrue(inverseObjectProperties.LeftObjectPropertyExpression is OWLObjectProperty objLProp
                            && string.Equals(objLProp.IRI, "ex:hasWife"));
            Assert.IsNotNull(inverseObjectProperties.RightObjectPropertyExpression);
            Assert.IsTrue(inverseObjectProperties.RightObjectPropertyExpression is OWLObjectProperty objRProp
                            && string.Equals(objRProp.IRI, "ex:isWifeOf"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingInverseObjectPropertiesBecauseNullLeftObjectProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLInverseObjectProperties(
                null,
                new OWLObjectProperty(new RDFResource("ex:isWifeOf"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingInverseObjectPropertiesBecauseNullRightObjectProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLInverseObjectProperties(
                new OWLObjectProperty(new RDFResource("ex:hasWife")),
                null));

        [TestMethod]
        public void ShouldSerializeInverseObjectProperties()
        {
            OWLInverseObjectProperties InverseObjectProperties = new OWLInverseObjectProperties(
                new OWLObjectProperty(new RDFResource("ex:hasWife")),
                new OWLObjectProperty(new RDFResource("ex:isWifeOf")));
            string serializedXML = OWLSerializer.SerializeObject(InverseObjectProperties);

            Assert.IsTrue(string.Equals(serializedXML,
@"<InverseObjectProperties><ObjectProperty IRI=""ex:hasWife"" /><ObjectProperty IRI=""ex:isWifeOf"" /></InverseObjectProperties>"));
        }

        [TestMethod]
        public void ShouldSerializeInverseObjectPropertiesWithFirstInverse()
        {
            OWLInverseObjectProperties InverseObjectProperties = new OWLInverseObjectProperties(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:isWifeOf"))),
                new OWLObjectProperty(new RDFResource("ex:isWifeOf")));
            string serializedXML = OWLSerializer.SerializeObject(InverseObjectProperties);

            Assert.IsTrue(string.Equals(serializedXML,
@"<InverseObjectProperties><ObjectInverseOf><ObjectProperty IRI=""ex:isWifeOf"" /></ObjectInverseOf><ObjectProperty IRI=""ex:isWifeOf"" /></InverseObjectProperties>"));
        }

        [TestMethod]
        public void ShouldSerializeInverseObjectPropertiesViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseObjectProperties(
                    new OWLObjectProperty(new RDFResource("ex:hasWife")),
                    new OWLObjectProperty(new RDFResource("ex:isWifeOf"))));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><InverseObjectProperties><ObjectProperty IRI=""ex:hasWife"" /><ObjectProperty IRI=""ex:isWifeOf"" /></InverseObjectProperties></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeInverseObjectProperties()
        {
            OWLInverseObjectProperties inverseObjectProperties = OWLSerializer.DeserializeObject<OWLInverseObjectProperties>(
@"<InverseObjectProperties>
  <ObjectProperty IRI=""ex:hasWife"" />
  <ObjectProperty IRI=""ex:isWifeOf"" />
</InverseObjectProperties>");

            Assert.IsNotNull(inverseObjectProperties);
            Assert.IsNotNull(inverseObjectProperties.LeftObjectPropertyExpression);
            Assert.IsTrue(inverseObjectProperties.LeftObjectPropertyExpression is OWLObjectProperty objLProp
                            && string.Equals(objLProp.IRI, "ex:hasWife"));
            Assert.IsNotNull(inverseObjectProperties.RightObjectPropertyExpression);
            Assert.IsTrue(inverseObjectProperties.RightObjectPropertyExpression is OWLObjectProperty objRProp
                            && string.Equals(objRProp.IRI, "ex:isWifeOf"));
        }

        [TestMethod]
        public void ShouldDeserializeInverseObjectPropertiesWithFirstInverse()
        {
            OWLInverseObjectProperties inverseObjectProperties = OWLSerializer.DeserializeObject<OWLInverseObjectProperties>(
@"<InverseObjectProperties>
  <ObjectInverseOf>
    <ObjectProperty IRI=""ex:isWifeOf"" />
  </ObjectInverseOf>
  <ObjectProperty IRI=""ex:isWifeOf"" />
</InverseObjectProperties>");

            Assert.IsNotNull(inverseObjectProperties);
            Assert.IsNotNull(inverseObjectProperties.LeftObjectPropertyExpression);
            Assert.IsTrue(inverseObjectProperties.LeftObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, "ex:isWifeOf"));
            Assert.IsNotNull(inverseObjectProperties.RightObjectPropertyExpression);
            Assert.IsTrue(inverseObjectProperties.RightObjectPropertyExpression is OWLObjectProperty objRProp
                            && string.Equals(objRProp.IRI, "ex:isWifeOf"));
        }

        [TestMethod]
        public void ShouldDeserializeInverseObjectPropertiesViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <InverseObjectProperties>
    <ObjectProperty IRI=""ex:hasWife"" />
    <ObjectProperty IRI=""ex:isWifeOf"" />
  </InverseObjectProperties>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLInverseObjectProperties invObjProp
                            && string.Equals(((OWLObjectProperty)invObjProp.LeftObjectPropertyExpression).IRI, "ex:hasWife")
                            && string.Equals(((OWLObjectProperty)invObjProp.RightObjectPropertyExpression).IRI, "ex:isWifeOf"));
        }

        [TestMethod]
        public void ShouldConvertInverseObjectPropertiesToGraph()
        {
            OWLInverseObjectProperties inverseObjectProperties = new OWLInverseObjectProperties(
                new OWLObjectProperty(new RDFResource("ex:hasWife")),
                new OWLObjectProperty(new RDFResource("ex:isWifeOf")));
            RDFGraph graph = inverseObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(3, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:hasWife"), RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:isWifeOf"), null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:hasWife"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:isWifeOf"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertInverseObjectPropertiesWithObjectInverseOfAsLeftToGraph()
        {
            OWLInverseObjectProperties inverseObjectProperties = new OWLInverseObjectProperties(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasWife"))),
                new OWLObjectProperty(new RDFResource("ex:isWifeOf")));
            RDFGraph graph = inverseObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(4, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:isWifeOf"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:hasWife"), null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:hasWife"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:isWifeOf"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertInverseObjectPropertiesWithAnnotationToGraph()
        {
            OWLInverseObjectProperties inverseObjectProperties = new OWLInverseObjectProperties(
                new OWLObjectProperty(new RDFResource("ex:hasWife")),
                new OWLObjectProperty(new RDFResource("ex:isWifeOf")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = inverseObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:hasWife"), RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:isWifeOf"), null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:hasWife"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:isWifeOf"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:hasWife"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.INVERSE_OF, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:isWifeOf"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertInverseObjectPropertiesWithObjectInverseOfAsLeftWithAnnotationToGraph()
        {
            OWLInverseObjectProperties inverseObjectProperties = new OWLInverseObjectProperties(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:hasWife"))),
                new OWLObjectProperty(new RDFResource("ex:isWifeOf")))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = inverseObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(10, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:isWifeOf"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:hasWife"), null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:hasWife"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("ex:isWifeOf"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.INVERSE_OF, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:isWifeOf"), null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}