/*
   Copyright 2014-2024 Marco De Salvo

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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Axioms
{
    [TestClass]
    public class OWLSymmetricObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSymmetricObjectProperty()
        {
			OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

			Assert.IsNotNull(symmetricObjectProperty);
			Assert.IsNotNull(symmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(symmetricObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
		}

        [TestMethod]
        public void ShouldCreateSymmetricObjectInverseOf()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

            Assert.IsNotNull(symmetricObjectProperty);
            Assert.IsNotNull(symmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(symmetricObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSymmetricObjectPropertyBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLSymmetricObjectProperty(null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSymmetricObjectPropertyBecauseNullObjectInverseOf()
            => Assert.ThrowsException<OWLException>(() => new OWLSymmetricObjectProperty(null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldSerializeSymmetricObjectProperty()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLSerializer.Serialize(symmetricObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SymmetricObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></SymmetricObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeSymmetricObjectInverseOf()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.Serialize(symmetricObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SymmetricObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></SymmetricObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeSymmetricObjectPropertyViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLSymmetricObjectProperty(
					new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.Serialize<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><SymmetricObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></SymmetricObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldSerializeSymmetricObjectInverseOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLSymmetricObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))));
            string serializedXML = OWLSerializer.Serialize<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><SymmetricObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></SymmetricObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeSymmetricObjectProperty()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = OWLSerializer.Deserialize<OWLSymmetricObjectProperty>(
@"<SymmetricObjectProperty>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</SymmetricObjectProperty>");

            Assert.IsNotNull(symmetricObjectProperty);
			Assert.IsNotNull(symmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(symmetricObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeSymmetricObjectInverseOf()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = OWLSerializer.Deserialize<OWLSymmetricObjectProperty>(
@"<SymmetricObjectProperty>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
</SymmetricObjectProperty>");

            Assert.IsNotNull(symmetricObjectProperty);
            Assert.IsNotNull(symmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(symmetricObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeSymmetricObjectPropertyViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SymmetricObjectProperty>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </SymmetricObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSymmetricObjectProperty symObjProp
                            && symObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSymmetricObjectProperty symObjProp1
							&& string.Equals(symObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(symObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(symObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldDeserializeSymmetricObjectInverseOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SymmetricObjectProperty>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectInverseOf>
      <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    </ObjectInverseOf>
  </SymmetricObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSymmetricObjectProperty symObjProp
                            && symObjProp.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLSymmetricObjectProperty symObjProp1
                            && string.Equals(symObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(symObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(symObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertSymmetricObjectPropertyToGraph()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            RDFGraph graph = symmetricObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 2);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertSymmetricObjectPropertyWithAnnotationToGraph()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = symmetricObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 8);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertSymmetricObjectInverseOfToGraph()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            RDFGraph graph = symmetricObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertSymmetricObjectInverseOfWithAnnotationToGraph()
        {
            OWLSymmetricObjectProperty symmetricObjectProperty = new OWLSymmetricObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = symmetricObjectProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 9);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}