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
using OWLSharp.Ontology.Expressions;
using OWLSharp.Test;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Axioms.Test
{
	[TestClass]
    public class OWLObjectPropertyRangeTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectPropertyRange()
        {
			OWLObjectPropertyRange objectPropertyRange = new OWLObjectPropertyRange(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
				new OWLClass(RDFVocabulary.FOAF.PERSON));

			Assert.IsNotNull(objectPropertyRange);
			Assert.IsNotNull(objectPropertyRange.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyRange.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyRange.ClassExpression);
            Assert.IsTrue(objectPropertyRange.ClassExpression is OWLClass cls 
							&& string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
		}

		[TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyRangeBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectPropertyRange(
                null as OWLObjectProperty,
                new OWLClass(RDFVocabulary.FOAF.PERSON)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyRangeBecauseNullObjectInverseOf()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectPropertyRange(
                null as OWLObjectInverseOf,
                new OWLClass(RDFVocabulary.FOAF.PERSON)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyRangeBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectPropertyRange(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                null));

		[TestMethod]
        public void ShouldSerializeObjectPropertyRange()
        {
            OWLObjectPropertyRange ObjectPropertyRange = new OWLObjectPropertyRange(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
				new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLTestSerializer<OWLObjectPropertyRange>.Serialize(ObjectPropertyRange);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectPropertyRange><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectPropertyRange>"));
        }

        [TestMethod]
        public void ShouldSerializeObjectPropertyRangeWithInverseOf()
        {
            OWLObjectPropertyRange ObjectPropertyRange = new OWLObjectPropertyRange(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLTestSerializer<OWLObjectPropertyRange>.Serialize(ObjectPropertyRange);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectPropertyRange><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectPropertyRange>"));
        }

        [TestMethod]
        public void ShouldSerializeObjectPropertyRangeViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyRange(
					new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
					new OWLClass(RDFVocabulary.FOAF.PERSON)));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><ObjectPropertyRange><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectPropertyRange></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectPropertyRange()
        {
            OWLObjectPropertyRange objectPropertyRange = OWLTestSerializer<OWLObjectPropertyRange>.Deserialize(
@"<ObjectPropertyRange>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectPropertyRange>");

            Assert.IsNotNull(objectPropertyRange);
            Assert.IsNotNull(objectPropertyRange.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyRange.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyRange.ClassExpression);
            Assert.IsTrue(objectPropertyRange.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeObjectPropertyRangeWithInverseOf()
        {
            OWLObjectPropertyRange objectPropertyRange = OWLTestSerializer<OWLObjectPropertyRange>.Deserialize(
@"<ObjectPropertyRange>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectPropertyRange>");

            Assert.IsNotNull(objectPropertyRange);
            Assert.IsNotNull(objectPropertyRange.ObjectPropertyExpression);
            Assert.IsTrue(objectPropertyRange.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectPropertyRange.ClassExpression);
            Assert.IsTrue(objectPropertyRange.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeObjectPropertyRangeViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <ObjectPropertyRange>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  </ObjectPropertyRange>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLObjectPropertyRange objPropDom
                            && objPropDom.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString())
                            && string.Equals(((OWLClass)objPropDom.ClassExpression).IRI, RDFVocabulary.FOAF.PERSON.ToString()));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLObjectPropertyRange objPropDom1
							&& string.Equals(objPropDom1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(objPropDom1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(objPropDom1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertObjectPropertyRangeToGraph()
        {
            OWLObjectPropertyRange objectPropertyRange = new OWLObjectPropertyRange(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new OWLClass(RDFVocabulary.FOAF.PERSON));
            RDFGraph graph = objectPropertyRange.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDFS.RANGE, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }
        #endregion
    }
}