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
    public class OWLDataPropertyDomainTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataPropertyDomain()
        {
			OWLDataPropertyDomain dataPropertyDomain = new OWLDataPropertyDomain(
				new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
				new OWLClass(RDFVocabulary.FOAF.PERSON));

			Assert.IsNotNull(dataPropertyDomain);
			Assert.IsNotNull(dataPropertyDomain.DataProperty);
            Assert.IsTrue(string.Equals(dataPropertyDomain.DataProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(dataPropertyDomain.ClassExpression);
            Assert.IsTrue(dataPropertyDomain.ClassExpression is OWLClass cls 
							&& string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
		}

		[TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyDomainBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataPropertyDomain(
                null,
                new OWLClass(RDFVocabulary.FOAF.PERSON)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyDomainBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLDataPropertyDomain(
                new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
                null));

		[TestMethod]
        public void ShouldSerializeDataPropertyDomain()
        {
            OWLDataPropertyDomain dataPropertyDomain = new OWLDataPropertyDomain(
				new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
				new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLTestSerializer<OWLDataPropertyDomain>.Serialize(dataPropertyDomain);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataPropertyDomain><DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></DataPropertyDomain>"));
        }

        [TestMethod]
        public void ShouldSerializeDataPropertyDomainViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DataPropertyAxioms.Add(
                new OWLDataPropertyDomain(
					new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
					new OWLClass(RDFVocabulary.FOAF.PERSON)));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><DataPropertyDomain><DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></DataPropertyDomain></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataPropertyDomain()
        {
            OWLDataPropertyDomain dataPropertyDomain = OWLTestSerializer<OWLDataPropertyDomain>.Deserialize(
@"<DataPropertyDomain>
  <DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</DataPropertyDomain>");

            Assert.IsNotNull(dataPropertyDomain);
			Assert.IsNotNull(dataPropertyDomain.DataProperty);
            Assert.IsTrue(string.Equals(dataPropertyDomain.DataProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(dataPropertyDomain.ClassExpression);
            Assert.IsTrue(dataPropertyDomain.ClassExpression is OWLClass cls 
							&& string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeDataPropertyDomainViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <DataPropertyDomain>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  </DataPropertyDomain>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLDataPropertyDomain dtPropDom
                            && string.Equals(dtPropDom.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString())
                            && string.Equals(((OWLClass)dtPropDom.ClassExpression).IRI, RDFVocabulary.FOAF.PERSON.ToString()));
			Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLDataPropertyDomain dtPropDom1
							&& string.Equals(dtPropDom1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(dtPropDom1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(dtPropDom1.Annotations.Single().ValueLiteral.Language, "EN"));
        }
		#endregion
	}
}