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
using System.Xml;

namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLSubAnnotationPropertyOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSubAnnotationPropertyOf()
        {
            OWLSubAnnotationPropertyOf subAnnotationPropertyOf = new OWLSubAnnotationPropertyOf(
                new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLAnnotationProperty(RDFVocabulary.DC.TITLE));

            Assert.IsNotNull(subAnnotationPropertyOf);
            Assert.IsNotNull(subAnnotationPropertyOf.SubAnnotationProperty);
            Assert.IsTrue(string.Equals(subAnnotationPropertyOf.SubAnnotationProperty.IRI, RDFVocabulary.DC.DCTERMS.TITLE.ToString()));
            Assert.IsNotNull(subAnnotationPropertyOf.SuperAnnotationProperty);
            Assert.IsTrue(string.Equals(subAnnotationPropertyOf.SuperAnnotationProperty.IRI, RDFVocabulary.DC.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubAnnotationPropertyOfBecauseNullSubAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLSubAnnotationPropertyOf(
                null,
                new OWLAnnotationProperty(RDFVocabulary.DC.TITLE)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubAnnotationPropertyOfBecauseNullSuperAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLSubAnnotationPropertyOf(
                new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                null));

        [TestMethod]
        public void ShouldSerializeSubAnnotationPropertyOf()
        {
            OWLSubAnnotationPropertyOf subAnnotationPropertyOf = new OWLSubAnnotationPropertyOf(
                new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLAnnotationProperty(RDFVocabulary.DC.TITLE));
            string serializedXML = OWLTestSerializer<OWLSubAnnotationPropertyOf>.Serialize(subAnnotationPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubAnnotationPropertyOf>
  <AnnotationProperty IRI=""http://purl.org/dc/terms/title"" />
  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
</SubAnnotationPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubAnnotationPropertyOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AnnotationAxioms.Add(
                new OWLSubAnnotationPropertyOf(
                    new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    new OWLAnnotationProperty(RDFVocabulary.DC.TITLE)));
            string serializedXML = OWLSerializer.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SubAnnotationPropertyOf>
    <AnnotationProperty IRI=""http://purl.org/dc/terms/title"" />
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
  </SubAnnotationPropertyOf>
</Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeSubAnnotationPropertyOf()
        {
            OWLSubAnnotationPropertyOf subAnnotationPropertyOf = OWLTestSerializer<OWLSubAnnotationPropertyOf>.Deserialize(
@"<SubAnnotationPropertyOf>
  <AnnotationProperty IRI=""http://purl.org/dc/terms/title"" />
  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
</SubAnnotationPropertyOf>");

            Assert.IsNotNull(subAnnotationPropertyOf);
            Assert.IsNotNull(subAnnotationPropertyOf.SubAnnotationProperty);
            Assert.IsTrue(string.Equals(subAnnotationPropertyOf.SubAnnotationProperty.IRI, RDFVocabulary.DC.DCTERMS.TITLE.ToString()));
            Assert.IsNotNull(subAnnotationPropertyOf.SuperAnnotationProperty);
            Assert.IsTrue(string.Equals(subAnnotationPropertyOf.SuperAnnotationProperty.IRI, RDFVocabulary.DC.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeSubAnnotationPropertyOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SubAnnotationPropertyOf>
    <AnnotationProperty IRI=""http://purl.org/dc/terms/title"" />
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
  </SubAnnotationPropertyOf>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 1);
            Assert.IsTrue(ontology.AnnotationAxioms.Single() is OWLSubAnnotationPropertyOf annPropDom
                            && string.Equals(annPropDom.SubAnnotationProperty.IRI, RDFVocabulary.DC.DCTERMS.TITLE.ToString())
                            && string.Equals(annPropDom.SuperAnnotationProperty.IRI, RDFVocabulary.DC.TITLE.ToString()));
        }
        #endregion
    }
}