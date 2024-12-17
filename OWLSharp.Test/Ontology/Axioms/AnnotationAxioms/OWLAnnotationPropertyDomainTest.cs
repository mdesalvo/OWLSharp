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
using OWLSharp.Ontology;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OWLSharp.Test.Ontology
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
        public void ShouldThrowExceptionOnCreatingIRIAnnotationPropertyDomainBecauseNullIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as RDFResource));

        [TestMethod]
        public void ShouldSerializeIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON);
            string serializedXML = OWLSerializer.SerializeObject(annotationPropertyDomain);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationPropertyDomain><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><IRI>http://xmlns.com/foaf/0.1/Person</IRI></AnnotationPropertyDomain>"));
        }

        [TestMethod]
        public void ShouldSerializeIRIAnnotationPropertyDomainViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyDomain(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                    RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><AnnotationPropertyDomain><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><IRI>http://xmlns.com/foaf/0.1/Person</IRI></AnnotationPropertyDomain></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = OWLSerializer.DeserializeObject<OWLAnnotationPropertyDomain>(
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
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
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

        [TestMethod]
        public void ShouldCreateAbbreviatedIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(annotationPropertyDomain);
            Assert.IsNotNull(annotationPropertyDomain.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNull(annotationPropertyDomain.IRI);
            Assert.IsNotNull(annotationPropertyDomain.AbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:Person"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAnnotationPropertyDomainBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationPropertyDomain(
                null,
                new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAnnotationPropertyDomainBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeAbbreviatedIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLSerializer.SerializeObject(annotationPropertyDomain);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationPropertyDomain><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Person</AbbreviatedIRI></AnnotationPropertyDomain>"));
        }

        [TestMethod]
        public void ShouldSerializeAbbreviatedIRIAnnotationPropertyDomainViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyDomain(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                    new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" /><AnnotationPropertyDomain><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Person</AbbreviatedIRI></AnnotationPropertyDomain></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeAbbreviatedIRIAnnotationPropertyDomain()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = OWLSerializer.DeserializeObject<OWLAnnotationPropertyDomain>(
@"<AnnotationPropertyDomain>
  <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  <AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Person</AbbreviatedIRI>
</AnnotationPropertyDomain>");

            Assert.IsNotNull(annotationPropertyDomain);
            Assert.IsNotNull(annotationPropertyDomain.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNull(annotationPropertyDomain.IRI);
            Assert.IsNotNull(annotationPropertyDomain.AbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationPropertyDomain.AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:Person"));
        }

        [TestMethod]
        public void ShouldDeserializeAbbreviatedIRIAnnotationPropertyDomainViaOntology()
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
  <AnnotationPropertyDomain>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <AbbreviatedIRI>foaf:Person</AbbreviatedIRI>
  </AnnotationPropertyDomain>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 1);
            Assert.IsTrue(ontology.AnnotationAxioms.Single() is OWLAnnotationPropertyDomain annPropDom
                            && string.Equals(annPropDom.AnnotationProperty.IRI, RDFVocabulary.FOAF.AGENT.ToString())
                            && string.Equals(annPropDom.AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:Person"));
        }

        [TestMethod]
        public void ShouldSerializeMultipleAndNestedAnnotationPropertyDomainsViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyDomain(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                    RDFVocabulary.FOAF.PERSON)
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
                new OWLAnnotationPropertyDomain(
                    new OWLAnnotationProperty(RDFVocabulary.FOAF.TITLE),
                    new XmlQualifiedName("Agent", RDFVocabulary.FOAF.BASE_URI)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" /><AnnotationPropertyDomain><Annotation><Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" /><Literal xml:lang=""EN-US--RTL"">contributor</Literal></Annotation><AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" /><IRI>ex:AnnValue</IRI></Annotation><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/Agent"" /><IRI>http://xmlns.com/foaf/0.1/Person</IRI></AnnotationPropertyDomain><AnnotationPropertyDomain><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/title"" /><AbbreviatedIRI xmlns:q1=""http://xmlns.com/foaf/0.1/"">q1:Agent</AbbreviatedIRI></AnnotationPropertyDomain></Ontology>"));
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
  <AnnotationPropertyDomain>
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
  </AnnotationPropertyDomain>
  <AnnotationPropertyDomain>
    <AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/title"" />
    <AbbreviatedIRI>foaf:Agent</AbbreviatedIRI>
  </AnnotationPropertyDomain>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 2);
            Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLAnnotationPropertyDomain annPropDom
                            && string.Equals(annPropDom.AnnotationProperty.IRI.ToString(), "http://xmlns.com/foaf/0.1/Agent")
                            && string.Equals(annPropDom.IRI, "http://xmlns.com/foaf/0.1/Person")
                                && string.Equals(annPropDom.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/description")
                                && string.Equals(annPropDom.Annotations.Single().ValueIRI, "ex:AnnValue")
                                    && string.Equals(annPropDom.Annotations.Single().Annotation.AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                                    && string.Equals(annPropDom.Annotations.Single().Annotation.ValueLiteral.Value, "contributor")
                                    && string.Equals(annPropDom.Annotations.Single().Annotation.ValueLiteral.Language, "EN-US--RTL")));
            Assert.IsTrue(ontology.AnnotationAxioms.Any(annAxm => annAxm is OWLAnnotationPropertyDomain annPropDom
                            && string.Equals(annPropDom.AbbreviatedIRI?.ToString(), "http://xmlns.com/foaf/0.1/:Agent")
                            && string.Equals(annPropDom.AnnotationProperty.IRI, "http://xmlns.com/foaf/0.1/title")));
        }

		[TestMethod]
        public void ShouldConvertIRIAnnotationPropertyDomainToGraph()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON);
			RDFGraph graph = annotationPropertyDomain.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 2);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDFS.DOMAIN, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertIRIAnnotationPropertyDomainWithAnnotationToGraph()
        {
            OWLAnnotationPropertyDomain annotationPropertyDomain = new OWLAnnotationPropertyDomain(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT),
                RDFVocabulary.FOAF.PERSON)
			{
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
			RDFGraph graph = annotationPropertyDomain.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 8);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDFS.DOMAIN, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
			//Annotations
			Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.DOMAIN, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}