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
    public class OWLSubDataPropertyOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSubDataPropertyOf()
        {
            OWLSubDataPropertyOf subDataPropertyOf = new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLDataProperty(RDFVocabulary.DC.TITLE));

            Assert.IsNotNull(subDataPropertyOf);
            Assert.IsNotNull(subDataPropertyOf.SubDataProperty);
            Assert.IsTrue(string.Equals(subDataPropertyOf.SubDataProperty.IRI, RDFVocabulary.DC.DCTERMS.TITLE.ToString()));
            Assert.IsNotNull(subDataPropertyOf.SuperDataProperty);
            Assert.IsTrue(string.Equals(subDataPropertyOf.SuperDataProperty.IRI, RDFVocabulary.DC.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubDataPropertyOfBecauseNullSubDataProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLSubDataPropertyOf(
                null,
                new OWLDataProperty(RDFVocabulary.DC.TITLE)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSubDataPropertyOfBecauseNullSuperDataProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                null));

        [TestMethod]
        public void ShouldSerializeSubDataPropertyOf()
        {
            OWLSubDataPropertyOf subDataPropertyOf = new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLDataProperty(RDFVocabulary.DC.TITLE));
            string serializedXML = OWLSerializer.SerializeObject(subDataPropertyOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<SubDataPropertyOf><DataProperty IRI=""http://purl.org/dc/terms/title"" /><DataProperty IRI=""http://purl.org/dc/elements/1.1/title"" /></SubDataPropertyOf>"));
        }

        [TestMethod]
        public void ShouldSerializeSubDataPropertyOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DataPropertyAxioms.Add(
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    new OWLDataProperty(RDFVocabulary.DC.TITLE)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><SubDataPropertyOf><DataProperty IRI=""http://purl.org/dc/terms/title"" /><DataProperty IRI=""http://purl.org/dc/elements/1.1/title"" /></SubDataPropertyOf></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeSubDataPropertyOf()
        {
            OWLSubDataPropertyOf subDataPropertyOf = OWLSerializer.DeserializeObject<OWLSubDataPropertyOf>(
@"<SubDataPropertyOf>
  <DataProperty IRI=""http://purl.org/dc/terms/title"" />
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
</SubDataPropertyOf>");

            Assert.IsNotNull(subDataPropertyOf);
            Assert.IsNotNull(subDataPropertyOf.SubDataProperty);
            Assert.IsTrue(string.Equals(subDataPropertyOf.SubDataProperty.IRI, RDFVocabulary.DC.DCTERMS.TITLE.ToString()));
            Assert.IsNotNull(subDataPropertyOf.SuperDataProperty);
            Assert.IsTrue(string.Equals(subDataPropertyOf.SuperDataProperty.IRI, RDFVocabulary.DC.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeSubDataPropertyOfViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <SubDataPropertyOf>
    <DataProperty IRI=""http://purl.org/dc/terms/title"" />
    <DataProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
  </SubDataPropertyOf>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.DataPropertyAxioms.Count);
            Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLSubDataPropertyOf annPropDom
                            && string.Equals(annPropDom.SubDataProperty.IRI, RDFVocabulary.DC.DCTERMS.TITLE.ToString())
                            && string.Equals(annPropDom.SuperDataProperty.IRI, RDFVocabulary.DC.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldSerializeMultipleAndNestedSubDataPropertyOfViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.DataPropertyAxioms.Add(
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    new OWLDataProperty(RDFVocabulary.DC.TITLE))
                {
                    Annotations =
                    [
                        new OWLAnnotation(
                            new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION),
                            new RDFResource("ex:AnnValue"))
                        {
                            Annotation = new OWLAnnotation(
                                new OWLAnnotationProperty(RDFVocabulary.DC.CONTRIBUTOR),
                                new OWLLiteral(new RDFPlainLiteral("contributor", "en-us--rtl")))
                        }
                    ]
                });
            ontology.DataPropertyAxioms.Add(
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.DC.DCTERMS.CREATOR),
                    new OWLDataProperty(RDFVocabulary.DC.CREATOR)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" /><SubDataPropertyOf><Annotation><Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" /><Literal xml:lang=""EN-US--RTL"">contributor</Literal></Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" /><IRI>ex:AnnValue</IRI></Annotation><DataProperty IRI=""http://purl.org/dc/terms/title"" /><DataProperty IRI=""http://purl.org/dc/elements/1.1/title"" /></SubDataPropertyOf><SubDataPropertyOf><DataProperty IRI=""http://purl.org/dc/terms/creator"" /><DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" /></SubDataPropertyOf></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeMultipleAndNestedDataAssertionsViaOntology()
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
  <SubDataPropertyOf>
    <Annotation>
      <Annotation>
        <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
        <Literal xml:lang=""EN-US--RTL"">contributor</Literal>
      </Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
      <IRI>ex:AnnValue</IRI>
    </Annotation>
    <DataProperty IRI=""http://purl.org/dc/terms/title"" />
    <DataProperty IRI=""http://purl.org/dc/elements/1.1/title"" />
  </SubDataPropertyOf>
  <SubDataPropertyOf>
    <DataProperty IRI=""http://purl.org/dc/terms/creator"" />
    <DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  </SubDataPropertyOf>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(2, ontology.DataPropertyAxioms.Count);
            Assert.IsTrue(ontology.DataPropertyAxioms.Any(annAxm => annAxm is OWLSubDataPropertyOf subannOf
                            && string.Equals(subannOf.SubDataProperty.IRI.ToString(), "http://purl.org/dc/terms/title")
                            && string.Equals(subannOf.SuperDataProperty.IRI.ToLower(), "http://purl.org/dc/elements/1.1/title")
                                && string.Equals(subannOf.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/description")
                                && string.Equals(subannOf.Annotations.Single().ValueIRI, "ex:AnnValue")
                                    && string.Equals(subannOf.Annotations.Single().Annotation.AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                                    && string.Equals(subannOf.Annotations.Single().Annotation.ValueLiteral.Value, "contributor")
                                    && string.Equals(subannOf.Annotations.Single().Annotation.ValueLiteral.Language, "EN-US--RTL")));
            Assert.IsTrue(ontology.DataPropertyAxioms.Any(annAxm => annAxm is OWLSubDataPropertyOf subannProp
                            && string.Equals(subannProp.SubDataProperty.IRI.ToString(), "http://purl.org/dc/terms/creator")
                            && string.Equals(subannProp.SuperDataProperty.IRI, "http://purl.org/dc/elements/1.1/creator")));
        }

        [TestMethod]
        public void ShouldConvertSubDataPropertyOfToGraph()
        {
            OWLSubDataPropertyOf subDataPropertyOf = new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLDataProperty(RDFVocabulary.DC.TITLE));
            RDFGraph graph = subDataPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(3, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.DC.TITLE, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertSubDataPropertyOfWithAnnotationToGraph()
        {
            OWLSubDataPropertyOf subDataPropertyOf = new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                new OWLDataProperty(RDFVocabulary.DC.TITLE))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = subDataPropertyOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(9, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.DC.TITLE, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.DCTERMS.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.DC.DCTERMS.TITLE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.DC.TITLE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}