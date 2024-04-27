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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Test;
using RDFSharp.Model;
using System.Linq;

namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLEquivalentObjectPropertiesTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateEquivalentObjectProperties()
        {
            OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
                [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]);

            Assert.IsNotNull(equivalentObjectProperties);
            Assert.IsNotNull(equivalentObjectProperties.ObjectPropertyExpressions);
            Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[0]).IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[1]).IRI, RDFVocabulary.FOAF.MEMBER.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEquivalentObjectPropertiesBecauseNullObjectProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLEquivalentObjectProperties(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEquivalentObjectPropertiesBecauseLessThan2ObjectProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLEquivalentObjectProperties(
                 [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEquivalentObjectPropertiesBecauseFoundNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLEquivalentObjectProperties(
                 [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), null ]));

        [TestMethod]
        public void ShouldSerializeEquivalentObjectProperties()
        {
            OWLEquivalentObjectProperties equivalentObjectProperties = new OWLEquivalentObjectProperties(
                [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]);
            string serializedXML = OWLTestSerializer<OWLEquivalentObjectProperties>.Serialize(equivalentObjectProperties);

            Assert.IsTrue(string.Equals(serializedXML,
@"<EquivalentObjectProperties><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" /></EquivalentObjectProperties>"));
        }

        [TestMethod]
        public void ShouldSerializeEquivalentObjectPropertiesViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLEquivalentObjectProperties(
                [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><EquivalentObjectProperties><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" /></EquivalentObjectProperties></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeEquivalentObjectProperties()
        {
            OWLEquivalentObjectProperties equivalentObjectProperties = OWLTestSerializer<OWLEquivalentObjectProperties>.Deserialize(
@"<EquivalentObjectProperties>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" />
</EquivalentObjectProperties>");

            Assert.IsNotNull(equivalentObjectProperties);
            Assert.IsNotNull(equivalentObjectProperties.ObjectPropertyExpressions);
            Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[0]).IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(string.Equals(((OWLObjectProperty)equivalentObjectProperties.ObjectPropertyExpressions[1]).IRI, RDFVocabulary.FOAF.MEMBER.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeEquivalentObjectPropertiesViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <EquivalentObjectProperties>
    <Annotation>
		<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
		<Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" />
  </EquivalentObjectProperties>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLEquivalentObjectProperties djObjProps
                            && string.Equals(((OWLObjectProperty)djObjProps.ObjectPropertyExpressions[0]).IRI, "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(((OWLObjectProperty)djObjProps.ObjectPropertyExpressions[1]).IRI, "http://xmlns.com/foaf/0.1/member"));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLEquivalentObjectProperties djDtProps1
							&& string.Equals(djDtProps1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Language, "EN"));
        }
        #endregion
    }
}