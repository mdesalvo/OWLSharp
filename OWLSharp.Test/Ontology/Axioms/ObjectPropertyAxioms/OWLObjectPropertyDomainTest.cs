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
    public class OWLObjectPropertyDomainTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectPropertyDomain()
        {
			OWLObjectPropertyDomain objectPropertyDomain = new OWLObjectPropertyDomain(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
				new OWLClass(RDFVocabulary.FOAF.PERSON));

			Assert.IsNotNull(objectPropertyDomain);
			Assert.IsNotNull(objectPropertyDomain.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyDomain.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyDomain.ClassExpression);
            Assert.IsTrue(objectPropertyDomain.ClassExpression is OWLClass cls 
							&& string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
		}

		[TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyDomainBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectPropertyDomain(
                null as OWLObjectProperty,
                new OWLClass(RDFVocabulary.FOAF.PERSON)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyDomainBecauseNullObjectInverseOf()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectPropertyDomain(
                null as OWLObjectInverseOf,
                new OWLClass(RDFVocabulary.FOAF.PERSON)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyDomainBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectPropertyDomain(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                null));

		[TestMethod]
        public void ShouldSerializeObjectPropertyDomain()
        {
            OWLObjectPropertyDomain ObjectPropertyDomain = new OWLObjectPropertyDomain(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
				new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLSerializer.Serialize(ObjectPropertyDomain);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectPropertyDomain><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectPropertyDomain>"));
        }

        [TestMethod]
        public void ShouldSerializeObjectPropertyDomainWithInverseOf()
        {
            OWLObjectPropertyDomain ObjectPropertyDomain = new OWLObjectPropertyDomain(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLSerializer.Serialize(ObjectPropertyDomain);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectPropertyDomain><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectPropertyDomain>"));
        }

        [TestMethod]
        public void ShouldSerializeObjectPropertyDomainViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyDomain(
					new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
					new OWLClass(RDFVocabulary.FOAF.PERSON)));
            string serializedXML = OWLSerializer.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><ObjectPropertyDomain><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectPropertyDomain></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectPropertyDomain()
        {
            OWLObjectPropertyDomain objectPropertyDomain = OWLSerializer.Deserialize<OWLObjectPropertyDomain>(
@"<ObjectPropertyDomain>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectPropertyDomain>");

            Assert.IsNotNull(objectPropertyDomain);
            Assert.IsNotNull(objectPropertyDomain.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyDomain.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyDomain.ClassExpression);
            Assert.IsTrue(objectPropertyDomain.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeObjectPropertyDomainWithInverseOf()
        {
            OWLObjectPropertyDomain objectPropertyDomain = OWLSerializer.Deserialize<OWLObjectPropertyDomain>(
@"<ObjectPropertyDomain>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectPropertyDomain>");

            Assert.IsNotNull(objectPropertyDomain);
            Assert.IsNotNull(objectPropertyDomain.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyDomain.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyDomain.ClassExpression);
            Assert.IsTrue(objectPropertyDomain.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeObjectPropertyDomainViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <ObjectPropertyDomain>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  </ObjectPropertyDomain>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLObjectPropertyDomain objPropDom
                            && objPropDom.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString())
                            && string.Equals(((OWLClass)objPropDom.ClassExpression).IRI, RDFVocabulary.FOAF.PERSON.ToString()));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLObjectPropertyDomain objPropDom1
							&& string.Equals(objPropDom1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(objPropDom1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(objPropDom1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertObjectPropertyDomainToGraph()
        {
            OWLObjectPropertyDomain objectPropertyDomain = new OWLObjectPropertyDomain(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLClass(RDFVocabulary.FOAF.PERSON));
            RDFGraph graph = objectPropertyDomain.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDFS.DOMAIN, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertObjectPropertyDomainWithAnnotationToGraph()
        {
            OWLObjectPropertyDomain objectPropertyDomain = new OWLObjectPropertyDomain(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLClass(RDFVocabulary.FOAF.PERSON))
			{
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = objectPropertyDomain.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 9);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDFS.DOMAIN, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			//Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.DOMAIN, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}