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
    public class OWLInverseFunctionalObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateInverseFunctionalObjectProperty()
        {
			OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

			Assert.IsNotNull(inverseFunctionalObjectProperty);
			Assert.IsNotNull(inverseFunctionalObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(inverseFunctionalObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
		}

        [TestMethod]
        public void ShouldCreateInverseFunctionalObjectInverseOf()
        {
            OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

            Assert.IsNotNull(inverseFunctionalObjectProperty);
            Assert.IsNotNull(inverseFunctionalObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(inverseFunctionalObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingInverseFunctionalObjectPropertyBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLInverseFunctionalObjectProperty(null as OWLObjectProperty));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingInverseFunctionalObjectPropertyBecauseNullObjectInverseOf()
            => Assert.ThrowsException<OWLException>(() => new OWLInverseFunctionalObjectProperty(null as OWLObjectInverseOf));

        [TestMethod]
        public void ShouldSerializeInverseFunctionalObjectProperty()
        {
            OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty(
				new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLTestSerializer<OWLInverseFunctionalObjectProperty>.Serialize(inverseFunctionalObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<InverseFunctionalObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></InverseFunctionalObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeInverseFunctionalObjectInverseOf()
        {
            OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = new OWLInverseFunctionalObjectProperty(
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLTestSerializer<OWLInverseFunctionalObjectProperty>.Serialize(inverseFunctionalObjectProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<InverseFunctionalObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></InverseFunctionalObjectProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeInverseFunctionalObjectPropertyViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseFunctionalObjectProperty(
					new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><InverseFunctionalObjectProperty><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></InverseFunctionalObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldSerializeInverseFunctionalObjectInverseOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseFunctionalObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><InverseFunctionalObjectProperty><ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf></InverseFunctionalObjectProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeInverseFunctionalObjectProperty()
        {
            OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = OWLTestSerializer<OWLInverseFunctionalObjectProperty>.Deserialize(
@"<InverseFunctionalObjectProperty>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</InverseFunctionalObjectProperty>");

            Assert.IsNotNull(inverseFunctionalObjectProperty);
			Assert.IsNotNull(inverseFunctionalObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(inverseFunctionalObjectProperty.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeInverseFunctionalObjectInverseOf()
        {
            OWLInverseFunctionalObjectProperty inverseFunctionalObjectProperty = OWLTestSerializer<OWLInverseFunctionalObjectProperty>.Deserialize(
@"<InverseFunctionalObjectProperty>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
</InverseFunctionalObjectProperty>");

            Assert.IsNotNull(inverseFunctionalObjectProperty);
            Assert.IsNotNull(inverseFunctionalObjectProperty.ObjectPropertyExpression);
            Assert.IsTrue(inverseFunctionalObjectProperty.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeInverseFunctionalObjectPropertyViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <InverseFunctionalObjectProperty>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </InverseFunctionalObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLInverseFunctionalObjectProperty invfuncObjProp
                            && invfuncObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLInverseFunctionalObjectProperty invfuncObjProp1
							&& string.Equals(invfuncObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(invfuncObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(invfuncObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldDeserializeInverseFunctionalObjectInverseOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <InverseFunctionalObjectProperty>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectInverseOf>
      <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    </ObjectInverseOf>
  </InverseFunctionalObjectProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLInverseFunctionalObjectProperty invfuncObjProp
                            && invfuncObjProp.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLInverseFunctionalObjectProperty invfuncObjProp1
                            && string.Equals(invfuncObjProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(invfuncObjProp1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(invfuncObjProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }
        #endregion
    }
}