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
    public class OWLAsymmetricObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateAsymmetricObjectProperty()
        {
			OWLAsymmetricObjectProperty asymmetricObjectProperty = new OWLAsymmetricObjectProperty(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

			Assert.IsNotNull(asymmetricObjectProperty);
			Assert.IsNotNull(asymmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(asymmetricObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
		}

        [TestMethod]
        public void ShouldCreateAsymmetricObjectInverseOf()
        {
            OWLAsymmetricObjectProperty asymmetricObjectProperty = new OWLAsymmetricObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

            Assert.IsNotNull(asymmetricObjectProperty);
            Assert.IsNotNull(asymmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(asymmetricObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAsymmetricObjectPropertyBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAsymmetricObjectProperty(null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAsymmetricObjectPropertyBecauseNullObjectInverseOf()
            => Assert.ThrowsException<OWLException>(() => new OWLAsymmetricObjectProperty(null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldSerializeAsymmetricObjectProperty()
        {
            OWLAsymmetricObjectProperty asymmetricObjectProperty = new OWLAsymmetricObjectProperty(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLTestSerializer<OWLAsymmetricObjectProperty>.Serialize(asymmetricObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AsymmetricObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></AsymmetricObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeAsymmetricObjectInverseOf()
        {
            OWLAsymmetricObjectProperty asymmetricObjectProperty = new OWLAsymmetricObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLTestSerializer<OWLAsymmetricObjectProperty>.Serialize(asymmetricObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AsymmetricObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></AsymmetricObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeAsymmetricObjectPropertyViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLAsymmetricObjectProperty(
					new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><AsymmetricObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></AsymmetricObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldSerializeAsymmetricObjectInverseOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLAsymmetricObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><AsymmetricObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></AsymmetricObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeAsymmetricObjectProperty()
        {
            OWLAsymmetricObjectProperty asymmetricObjectProperty = OWLTestSerializer<OWLAsymmetricObjectProperty>.Deserialize(
@"<AsymmetricObjectProperty>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</AsymmetricObjectProperty>");

            Assert.IsNotNull(asymmetricObjectProperty);
			Assert.IsNotNull(asymmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(asymmetricObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeAsymmetricObjectInverseOf()
        {
            OWLAsymmetricObjectProperty asymmetricObjectProperty = OWLTestSerializer<OWLAsymmetricObjectProperty>.Deserialize(
@"<AsymmetricObjectProperty>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
</AsymmetricObjectProperty>");

            Assert.IsNotNull(asymmetricObjectProperty);
            Assert.IsNotNull(asymmetricObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(asymmetricObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeAsymmetricObjectPropertyViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <AsymmetricObjectProperty>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </AsymmetricObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLAsymmetricObjectProperty asymObjProp
                            && asymObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLAsymmetricObjectProperty asymObjProp1
							&& string.Equals(asymObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(asymObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(asymObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldDeserializeAsymmetricObjectInverseOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <AsymmetricObjectProperty>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectInverseOf>
      <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    </ObjectInverseOf>
  </AsymmetricObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLAsymmetricObjectProperty asymObjProp
                            && asymObjProp.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLAsymmetricObjectProperty asymObjProp1
                            && string.Equals(asymObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(asymObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(asymObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }
        #endregion
    }
}