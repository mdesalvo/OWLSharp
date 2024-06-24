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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Linq;

namespace OWLSharp.Test.Ontology.Axioms
{
    [TestClass]
    public class OWLEquivalentDataPropertiesTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateEquivalentDataProperties()
        {
            OWLEquivalentDataProperties EquivalentDataProperties = new OWLEquivalentDataProperties(
                [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]);

            Assert.IsNotNull(EquivalentDataProperties);
            Assert.IsNotNull(EquivalentDataProperties.DataProperties);
            Assert.IsTrue(string.Equals(EquivalentDataProperties.DataProperties[0].IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsTrue(string.Equals(EquivalentDataProperties.DataProperties[1].IRI, RDFVocabulary.FOAF.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEquivalentDataPropertiesBecauseNullDataProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLEquivalentDataProperties(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEquivalentDataPropertiesBecauseLessThan2DataProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLEquivalentDataProperties(
                 [ new OWLDataProperty(RDFVocabulary.FOAF.AGE) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEquivalentDataPropertiesBecauseFoundNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLEquivalentDataProperties(
                 [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), null ]));

        [TestMethod]
        public void ShouldSerializeEquivalentDataProperties()
        {
            OWLEquivalentDataProperties EquivalentDataProperties = new OWLEquivalentDataProperties(
                [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]);
            string serializedXML = OWLSerializer.Serialize(EquivalentDataProperties);

            Assert.IsTrue(string.Equals(serializedXML,
@"<EquivalentDataProperties><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/title"" /></EquivalentDataProperties>"));
        }

        [TestMethod]
        public void ShouldSerializeEquivalentDataPropertiesViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DataPropertyAxioms.Add(
                new OWLEquivalentDataProperties(
                [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]));
            string serializedXML = OWLSerializer.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><EquivalentDataProperties><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/title"" /></EquivalentDataProperties></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeEquivalentDataProperties()
        {
            OWLEquivalentDataProperties EquivalentDataProperties = OWLSerializer.Deserialize<OWLEquivalentDataProperties>(
@"<EquivalentDataProperties>
  <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
  <DataProperty IRI=""http://xmlns.com/foaf/0.1/title"" />
</EquivalentDataProperties>");

            Assert.IsNotNull(EquivalentDataProperties);
            Assert.IsNotNull(EquivalentDataProperties.DataProperties);
            Assert.IsTrue(string.Equals(EquivalentDataProperties.DataProperties[0].IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsTrue(string.Equals(EquivalentDataProperties.DataProperties[1].IRI, RDFVocabulary.FOAF.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeEquivalentDataPropertiesViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:foaf=""http://xmlns.com/foaf/0.1/"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
  <EquivalentDataProperties>
    <Annotation>
		<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
		<Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
    <DataProperty abbreviatedIRI=""foaf:title"" />
  </EquivalentDataProperties>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLEquivalentDataProperties djDtProps
                            && string.Equals(djDtProps.DataProperties[0].IRI, "http://xmlns.com/foaf/0.1/age")
                            && string.Equals(djDtProps.DataProperties[1].AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:title"));
			Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLEquivalentDataProperties djDtProps1
							&& string.Equals(djDtProps1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvert2EquivalentDataPropertiesToGraph()
        {
            OWLEquivalentDataProperties equivalentObjectProperties = new OWLEquivalentDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.NAME)]);
            RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert3EquivalentDataPropertiesToGraph()
        {
            OWLEquivalentDataProperties equivalentObjectProperties = new OWLEquivalentDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                 new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                 new OWLDataProperty(RDFVocabulary.FOAF.TITLE)]);
            RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 6);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.TITLE, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert2EquivalentDataPropertiesWithAnnotationToGraph()
        {
            OWLEquivalentDataProperties equivalentObjectProperties = new OWLEquivalentDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.NAME)])
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 9);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert3EquivalentDataPropertiesWithAnnotationToGraph()
        {
            OWLEquivalentDataProperties equivalentObjectProperties = new OWLEquivalentDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                 new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                 new OWLDataProperty(RDFVocabulary.FOAF.TITLE)])
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = equivalentObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 22);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, RDFVocabulary.FOAF.TITLE, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGE, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.TITLE, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 3);
        }
        #endregion
    }
}