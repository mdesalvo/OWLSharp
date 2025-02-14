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
    public class OWLReflexiveObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReflexiveObjectProperty()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

            Assert.IsNotNull(reflexiveObjectProperty);
            Assert.IsNotNull(reflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(reflexiveObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldCreateReflexiveObjectInverseOf()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

            Assert.IsNotNull(reflexiveObjectProperty);
            Assert.IsNotNull(reflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(reflexiveObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingReflexiveObjectPropertyBecauseNullObjectProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLReflexiveObjectProperty(null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingReflexiveObjectPropertyBecauseNullObjectInverseOf()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLReflexiveObjectProperty(null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldSerializeReflexiveObjectProperty()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLSerializer.SerializeObject(reflexiveObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ReflexiveObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ReflexiveObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeReflexiveObjectInverseOf()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.SerializeObject(reflexiveObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ReflexiveObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></ReflexiveObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeReflexiveObjectPropertyViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLReflexiveObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><ReflexiveObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ReflexiveObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldSerializeReflexiveObjectInverseOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLReflexiveObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><ReflexiveObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></ReflexiveObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeReflexiveObjectProperty()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = OWLSerializer.DeserializeObject<OWLReflexiveObjectProperty>(
@"<ReflexiveObjectProperty>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ReflexiveObjectProperty>");

            Assert.IsNotNull(reflexiveObjectProperty);
            Assert.IsNotNull(reflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(reflexiveObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeReflexiveObjectInverseOf()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = OWLSerializer.DeserializeObject<OWLReflexiveObjectProperty>(
@"<ReflexiveObjectProperty>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
</ReflexiveObjectProperty>");

            Assert.IsNotNull(reflexiveObjectProperty);
            Assert.IsNotNull(reflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(reflexiveObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeReflexiveObjectPropertyViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <ReflexiveObjectProperty>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
      <Literal xml:lang=""EN"">Steve</Literal>
    </Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ReflexiveObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLReflexiveObjectProperty
                          {
                              ObjectPropertyExpression: OWLObjectProperty objProp
                          } && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLReflexiveObjectProperty reflObjProp1
                            && string.Equals(reflObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(reflObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(reflObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldDeserializeReflexiveObjectInverseOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <ReflexiveObjectProperty>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
      <Literal xml:lang=""EN"">Steve</Literal>
    </Annotation>
    <ObjectInverseOf>
      <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    </ObjectInverseOf>
  </ReflexiveObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLReflexiveObjectProperty
                          {
                              ObjectPropertyExpression: OWLObjectInverseOf objInvOf
                          } && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLReflexiveObjectProperty reflObjProp1
                            && string.Equals(reflObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(reflObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(reflObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertReflexiveObjectPropertyToGraph()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            RDFGraph graph = reflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(2, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertReflexiveObjectPropertyWithAnnotationToGraph()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = reflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(8, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertReflexiveObjectInverseOfToGraph()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            RDFGraph graph = reflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(3, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertReflexiveObjectInverseOfWithAnnotationToGraph()
        {
            OWLReflexiveObjectProperty reflexiveObjectProperty = new OWLReflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = reflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}