﻿/*
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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLAnnotationPropertyDomainTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON);

            Assert.IsNotNull(annotationPropertyDomain);
            Assert.IsNotNull(annotationPropertyDomain.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(annotationPropertyDomain.IRI);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(annotationPropertyDomain.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAnnotationPropertyDomainBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationPropertyDomain(
                null,
                RDFVocabulary.FOAF.PERSON));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAnnotationPropertyDomainBecauseIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as RDFResource));

        [TestMethod]
        public void ShouldSerializeIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON);
            string serializedXML = OWLTestSerializer<OWLAnnotationPropertyDomain>.Serialize(annotationPropertyDomain);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationPropertyDomain>
  <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  <IRI>http://xmlns.com/foaf/0.1/Person</IRI>
</AnnotationPropertyDomain>"));
        }

        [TestMethod]
        public void ShouldSerializeIRIAnnotationPropertyDomainViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyDomain(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                    RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLSerializer.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <AnnotationPropertyDomain>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <IRI>http://xmlns.com/foaf/0.1/Person</IRI>
  </AnnotationPropertyDomain>
</Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = OWLTestSerializer<OWLAnnotationPropertyDomain>.Deserialize(
@"<AnnotationPropertyDomain>
  <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  <IRI>http://xmlns.com/foaf/0.1/Person</IRI>
</AnnotationPropertyDomain>");

            Assert.IsNotNull(annotationPropertyDomain);
            Assert.IsNotNull(annotationPropertyDomain.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(annotationPropertyDomain.IRI);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(annotationPropertyDomain.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationPropertyDomainViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <AnnotationPropertyDomain>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <IRI>http://xmlns.com/foaf/0.1/Person</IRI>
  </AnnotationPropertyDomain>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 1);
            Assert.IsTrue(ontology.AnnotationAxioms.Single() is OWLAnnotationPropertyDomain annPropDom
                            && string.Equals(annPropDom.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString())
                            && string.Equals(annPropDom.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }
        #endregion
    }
}