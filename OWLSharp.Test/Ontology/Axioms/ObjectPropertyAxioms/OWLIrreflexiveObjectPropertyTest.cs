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
    public class OWLIrreflexiveObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIrreflexiveObjectProperty()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

            Assert.IsNotNull(irreflexiveObjectProperty);
            Assert.IsNotNull(irreflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(irreflexiveObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldCreateIrreflexiveObjectInverseOf()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

            Assert.IsNotNull(irreflexiveObjectProperty);
            Assert.IsNotNull(irreflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(irreflexiveObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIrreflexiveObjectPropertyBecauseNullObjectProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLIrreflexiveObjectProperty(null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIrreflexiveObjectPropertyBecauseNullObjectInverseOf()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLIrreflexiveObjectProperty(null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldSerializeIrreflexiveObjectProperty()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLSerializer.SerializeObject(irreflexiveObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<IrreflexiveObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></IrreflexiveObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeIrreflexiveObjectInverseOf()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.SerializeObject(irreflexiveObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<IrreflexiveObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></IrreflexiveObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeIrreflexiveObjectPropertyViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLIrreflexiveObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><IrreflexiveObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></IrreflexiveObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldSerializeIrreflexiveObjectInverseOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLIrreflexiveObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><IrreflexiveObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></IrreflexiveObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeIrreflexiveObjectProperty()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = OWLSerializer.DeserializeObject<OWLIrreflexiveObjectProperty>(
@"<IrreflexiveObjectProperty>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</IrreflexiveObjectProperty>");

            Assert.IsNotNull(irreflexiveObjectProperty);
            Assert.IsNotNull(irreflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(irreflexiveObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeIrreflexiveObjectInverseOf()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = OWLSerializer.DeserializeObject<OWLIrreflexiveObjectProperty>(
@"<IrreflexiveObjectProperty>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
</IrreflexiveObjectProperty>");

            Assert.IsNotNull(irreflexiveObjectProperty);
            Assert.IsNotNull(irreflexiveObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(irreflexiveObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeIrreflexiveObjectPropertyViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <IrreflexiveObjectProperty>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
      <Literal xml:lang=""EN"">Steve</Literal>
    </Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </IrreflexiveObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLIrreflexiveObjectProperty irreflObjProp
                            && irreflObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLIrreflexiveObjectProperty irreflObjProp1
                            && string.Equals(irreflObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(irreflObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(irreflObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldDeserializeIrreflexiveObjectInverseOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <IrreflexiveObjectProperty>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
      <Literal xml:lang=""EN"">Steve</Literal>
    </Annotation>
    <ObjectInverseOf>
      <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    </ObjectInverseOf>
  </IrreflexiveObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLIrreflexiveObjectProperty irreflObjProp
                            && irreflObjProp.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLIrreflexiveObjectProperty irreflObjProp1
                            && string.Equals(irreflObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(irreflObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(irreflObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertIrreflexiveObjectPropertyToGraph()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            RDFGraph graph = irreflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(2, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertIrreflexiveObjectPropertyWithAnnotationToGraph()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = irreflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(8, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertIrreflexiveObjectInverseOfToGraph()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            RDFGraph graph = irreflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(3, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertIrreflexiveObjectInverseOfWithAnnotationToGraph()
        {
            OWLIrreflexiveObjectProperty irreflexiveObjectProperty = new OWLIrreflexiveObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = irreflexiveObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}