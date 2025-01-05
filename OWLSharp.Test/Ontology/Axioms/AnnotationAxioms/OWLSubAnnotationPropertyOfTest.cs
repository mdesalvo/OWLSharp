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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Ontology
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
            string serializedXML = OWLSerializer.SerializeObject(subAnnotationPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubAnnotationPropertyOf><AnnotationProperty IRI=""http://purl.org/dc/terms/title"" /><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" /></SubAnnotationPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubAnnotationPropertyOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AnnotationAxioms.Add(
                new OWLSubAnnotationPropertyOf(
                    new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    new OWLAnnotationProperty(RDFVocabulary.DC.TITLE)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><SubAnnotationPropertyOf><AnnotationProperty IRI=""http://purl.org/dc/terms/title"" /><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" /></SubAnnotationPropertyOf></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeSubAnnotationPropertyOf()
        {
            OWLSubAnnotationPropertyOf subAnnotationPropertyOf = OWLSerializer.DeserializeObject<OWLSubAnnotationPropertyOf>(
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
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
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

        [TestMethod]
        public void ShouldSerializeMultipleAndNestedSubAnnotationPropertyOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.AnnotationAxioms.Add(
                new OWLSubAnnotationPropertyOf(
                    new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    new OWLAnnotationProperty(RDFVocabulary.DC.TITLE))
                {
                    Annotations = new List<OWLAnnotation>()
                    {
                        new OWLAnnotation(
                            new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION),
                            new RDFResource("ex:AnnValue"))
                        {
                            Annotation = new OWLAnnotation(
                                new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR),
                                new OWLLiteral(new RDFPlainLiteral("contributor", "en-us--rtl")))
                        }
                    }
                });
            ontology.AnnotationAxioms.Add(
                new OWLSubAnnotationPropertyOf(
                    new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.CREATOR),
                    new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" /><SubAnnotationPropertyOf><Annotation><Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" /><Literal xml:lang=""EN-US--RTL"">contributor</Literal></Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" /><IRI>ex:AnnValue</IRI></Annotation><AnnotationProperty IRI=""http://purl.org/dc/terms/title"" /><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" /></SubAnnotationPropertyOf><SubAnnotationPropertyOf><AnnotationProperty IRI=""http://purl.org/dc/terms/creator"" /><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/creator"" /></SubAnnotationPropertyOf></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeMultipleAndNestedAnnotationAssertionsViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:foaf=""http://xmlns.com/foaf/0.1/"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
  <SubAnnotationPropertyOf>
    <Annotation>
      <Annotation>
        <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
        <Literal xml:lang=""EN-US--RTL"">contributor</Literal>
      </Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
      <IRI>ex:AnnValue</IRI>
    </Annotation>
    <AnnotationProperty IRI=""http://purl.org/dc/terms/title"" />
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
  </SubAnnotationPropertyOf>
  <SubAnnotationPropertyOf>
    <AnnotationProperty IRI=""http://purl.org/dc/terms/creator"" />
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  </SubAnnotationPropertyOf>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 2);
            Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLSubAnnotationPropertyOf subannOf
                            && string.Equals(subannOf.SubAnnotationProperty.IRI.ToString(), "http://purl.org/dc/terms/title")
                            && string.Equals(subannOf.SuperAnnotationProperty.IRI.ToLower(), "http://purl.org/dc/elements/1.1/title")
                                && string.Equals(subannOf.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/description")
                                && string.Equals(subannOf.Annotations.Single().ValueIRI, "ex:AnnValue")
                                    && string.Equals(subannOf.Annotations.Single().Annotation.AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                                    && string.Equals(subannOf.Annotations.Single().Annotation.ValueLiteral.Value, "contributor")
                                    && string.Equals(subannOf.Annotations.Single().Annotation.ValueLiteral.Language, "EN-US--RTL")));
            Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLSubAnnotationPropertyOf subannProp
                            && string.Equals(subannProp.SubAnnotationProperty.IRI.ToString(), "http://purl.org/dc/terms/creator")
                            && string.Equals(subannProp.SuperAnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/creator")));
        }

        [TestMethod]
        public void ShouldConvertSubAnnotationPropertyOfToGraph()
        {
            OWLSubAnnotationPropertyOf subAnnotationPropertyOf = new OWLSubAnnotationPropertyOf(
                new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLAnnotationProperty(RDFVocabulary.DC.TITLE));
            RDFGraph graph = subAnnotationPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.DC.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertSubAnnotationPropertyOfWithAnnotationToGraph()
        {
            OWLSubAnnotationPropertyOf subAnnotationPropertyOf = new OWLSubAnnotationPropertyOf(
                new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLAnnotationProperty(RDFVocabulary.DC.TITLE))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = subAnnotationPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 8);
            Assert.IsTrue(graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.DC.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.DC.DCTERMS.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.DC.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}