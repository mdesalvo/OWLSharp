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
using System.Xml;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLAnnotationPropertyRangeTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIAnnotationPropertyRange()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON);

            Assert.IsNotNull(annotationPropertyRange);
            Assert.IsNotNull(annotationPropertyRange.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyRange.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(annotationPropertyRange.IRI);
            Assert.IsTrue(string.Equals(annotationPropertyRange.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(annotationPropertyRange.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAnnotationPropertyRangeBecauseNullAnnotationProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationPropertyRange(
                null,
                RDFVocabulary.FOAF.PERSON));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAnnotationPropertyRangeBecauseNullIRI()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as RDFResource));

        [TestMethod]
        public void ShouldSerializeIRIAnnotationPropertyRange()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON);
            string serializedXML = OWLSerializer.SerializeObject(annotationPropertyRange);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationPropertyRange><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><IRI>http://xmlns.com/foaf/0.1/Person</IRI></AnnotationPropertyRange>"));
        }

        [TestMethod]
        public void ShouldSerializeIRIAnnotationPropertyRangeViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyRange(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                    RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><AnnotationPropertyRange><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><IRI>http://xmlns.com/foaf/0.1/Person</IRI></AnnotationPropertyRange></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationPropertyRange()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = OWLSerializer.DeserializeObject<OWLAnnotationPropertyRange>(
@"<AnnotationPropertyRange>
  <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  <IRI>http://xmlns.com/foaf/0.1/Person</IRI>
</AnnotationPropertyRange>");

            Assert.IsNotNull(annotationPropertyRange);
            Assert.IsNotNull(annotationPropertyRange.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyRange.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(annotationPropertyRange.IRI);
            Assert.IsTrue(string.Equals(annotationPropertyRange.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(annotationPropertyRange.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationPropertyRangeViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <AnnotationPropertyRange>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <IRI>http://xmlns.com/foaf/0.1/Person</IRI>
  </AnnotationPropertyRange>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AnnotationAxioms.Count);
            Assert.IsTrue(ontology.AnnotationAxioms.Single() is OWLAnnotationPropertyRange annPropDom
                            && string.Equals(annPropDom.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString())
                            && string.Equals(annPropDom.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldCreateAbbreviatedIRIAnnotationPropertyRange()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(annotationPropertyRange);
            Assert.IsNotNull(annotationPropertyRange.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyRange.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNull(annotationPropertyRange.IRI);
            Assert.IsNotNull(annotationPropertyRange.AbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationPropertyRange.AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:Person"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAnnotationPropertyRangeBecauseNullAnnotationProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationPropertyRange(
                null,
                new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAnnotationPropertyRangeBecauseNullQualifiedName()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeAbbreviatedIRIAnnotationPropertyRange()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLSerializer.SerializeObject(annotationPropertyRange);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationPropertyRange><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Person</AbbreviatedIRI></AnnotationPropertyRange>"));
        }

        [TestMethod]
        public void ShouldSerializeAbbreviatedIRIAnnotationPropertyRangeViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyRange(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                    new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" /><AnnotationPropertyRange><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Person</AbbreviatedIRI></AnnotationPropertyRange></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeAbbreviatedIRIAnnotationPropertyRange()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = OWLSerializer.DeserializeObject<OWLAnnotationPropertyRange>(
@"<AnnotationPropertyRange>
  <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  <AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Person</AbbreviatedIRI>
</AnnotationPropertyRange>");

            Assert.IsNotNull(annotationPropertyRange);
            Assert.IsNotNull(annotationPropertyRange.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyRange.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNull(annotationPropertyRange.IRI);
            Assert.IsNotNull(annotationPropertyRange.AbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationPropertyRange.AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:Person"));
        }

        [TestMethod]
        public void ShouldDeserializeAbbreviatedIRIAnnotationPropertyRangeViaOntology()
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
  <AnnotationPropertyRange>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <AbbreviatedIRI>foaf:Person</AbbreviatedIRI>
  </AnnotationPropertyRange>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(1, ontology.AnnotationAxioms.Count);
            Assert.IsTrue(ontology.AnnotationAxioms.Single() is OWLAnnotationPropertyRange annPropRng
                            && string.Equals(annPropRng.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString())
                            && string.Equals(annPropRng.AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:Person"));
        }

        [TestMethod]
        public void ShouldSerializeMultipleAndNestedAnnotationPropertyRangesViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyRange(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                    RDFVocabulary.FOAF.PERSON)
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
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyRange(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.TITLE),
                    new XmlQualifiedName("Agent", RDFVocabulary.FOAF.BASE_URI)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" /><AnnotationPropertyRange><Annotation><Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" /><Literal xml:lang=""EN-US--RTL"">contributor</Literal></Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" /><IRI>ex:AnnValue</IRI></Annotation><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><IRI>http://xmlns.com/foaf/0.1/Person</IRI></AnnotationPropertyRange><AnnotationPropertyRange><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/title"" /><AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Agent</AbbreviatedIRI></AnnotationPropertyRange></Ontology>"));
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
  <AnnotationPropertyRange>
    <Annotation>
      <Annotation>
        <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
        <Literal xml:lang=""EN-US--RTL"">contributor</Literal>
      </Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
      <IRI>ex:AnnValue</IRI>
    </Annotation>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <IRI>http://xmlns.com/foaf/0.1/Person</IRI>
  </AnnotationPropertyRange>
  <AnnotationPropertyRange>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/title"" />
    <AbbreviatedIRI>foaf:Agent</AbbreviatedIRI>
  </AnnotationPropertyRange>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.AreEqual(2, ontology.AnnotationAxioms.Count);
            Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLAnnotationPropertyRange annPropRng
                            && string.Equals(annPropRng.AnnotationProperty.IRI.ToString(), "http://xmlns.com/foaf/0.1/Agent")
                            && string.Equals(annPropRng.IRI, "http://xmlns.com/foaf/0.1/Person")
                                && string.Equals(annPropRng.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/description")
                                && string.Equals(annPropRng.Annotations.Single().ValueIRI, "ex:AnnValue")
                                    && string.Equals(annPropRng.Annotations.Single().Annotation.AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                                    && string.Equals(annPropRng.Annotations.Single().Annotation.ValueLiteral.Value, "contributor")
                                    && string.Equals(annPropRng.Annotations.Single().Annotation.ValueLiteral.Language, "EN-US--RTL")));
            Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLAnnotationPropertyRange annPropRng
                            && string.Equals(annPropRng.AbbreviatedIRI?.ToString(), "http://xmlns.com/foaf/0.1/:Agent")
                            && string.Equals(annPropRng.AnnotationProperty.IRI, "http://xmlns.com/foaf/0.1/title")));
        }

        [TestMethod]
        public void ShouldConvertIRIAnnotationPropertyRangeToGraph()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON);
            RDFGraph graph = annotationPropertyRange.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(2, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDFS.RANGE, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertIRIAnnotationPropertyRangeWithAnnotationToGraph()
        {
            OWLAnnotationPropertyRange annotationPropertyRange = new OWLAnnotationPropertyRange(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON)
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = annotationPropertyRange.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(8, graph.TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDFS.RANGE, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            //Annotations
            Assert.AreEqual(1, graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.RANGE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount);
        }
        #endregion
    }
}