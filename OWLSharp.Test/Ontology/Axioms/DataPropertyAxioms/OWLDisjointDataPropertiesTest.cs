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
using System.Linq;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLDisjointDataPropertiesTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDisjointDataProperties()
        {
            OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties(
                [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]);

            Assert.IsNotNull(disjointDataProperties);
            Assert.IsNotNull(disjointDataProperties.DataProperties);
            Assert.IsTrue(string.Equals(disjointDataProperties.DataProperties[0].IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsTrue(string.Equals(disjointDataProperties.DataProperties[1].IRI, RDFVocabulary.FOAF.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointDataPropertiesBecauseNullDataProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointDataProperties(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointDataPropertiesBecauseLessThan2DataProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointDataProperties(
                 [ new OWLDataProperty(RDFVocabulary.FOAF.AGE) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointDataPropertiesBecauseFoundNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointDataProperties(
                 [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), null ]));

        [TestMethod]
        public void ShouldSerializeDisjointDataProperties()
        {
            OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties(
                [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]);
            string serializedXML = OWLSerializer.SerializeObject(disjointDataProperties);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DisjointDataProperties><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/title"" /></DisjointDataProperties>"));
        }

        [TestMethod]
        public void ShouldSerializeDisjointDataPropertiesViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DataPropertyAxioms.Add(
                new OWLDisjointDataProperties(
                [ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><DisjointDataProperties><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/title"" /></DisjointDataProperties></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeDisjointDataProperties()
        {
            OWLDisjointDataProperties disjointDataProperties = OWLSerializer.DeserializeObject<OWLDisjointDataProperties>(
@"<DisjointDataProperties>
  <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
  <DataProperty IRI=""http://xmlns.com/foaf/0.1/title"" />
</DisjointDataProperties>");

            Assert.IsNotNull(disjointDataProperties);
            Assert.IsNotNull(disjointDataProperties.DataProperties);
            Assert.IsTrue(string.Equals(disjointDataProperties.DataProperties[0].IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsTrue(string.Equals(disjointDataProperties.DataProperties[1].IRI, RDFVocabulary.FOAF.TITLE.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeDisjointDataPropertiesViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <DisjointDataProperties>
    <Annotation>
        <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
        <Literal xml:lang=""EN"">Steve</Literal>
    </Annotation>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/title"" />
  </DisjointDataProperties>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLDisjointDataProperties djDtProps
                            && string.Equals(djDtProps.DataProperties[0].IRI, "http://xmlns.com/foaf/0.1/age")
                            && string.Equals(djDtProps.DataProperties[1].IRI, "http://xmlns.com/foaf/0.1/title"));
            Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLDisjointDataProperties djDtProps1
                            && string.Equals(djDtProps1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
                            && string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Value, "Steve")
                            && string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvert2DisjointDataPropertiesToGraph()
        {
            OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.NAME)]);
            RDFGraph graph = disjointDataProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert3DisjointDataPropertiesToGraph()
        {
            OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                 new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                 new OWLDataProperty(RDFVocabulary.FOAF.TITLE)]);
            RDFGraph graph = disjointDataProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 14);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.MEMBERS, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert2DisjointDataPropertiesWithAnnotationToGraph()
        {
            OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.NAME)])
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = disjointDataProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 9);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert3DisjointDataPropertiesWithAnnotationToGraph()
        {
            OWLDisjointDataProperties disjointDataProperties = new OWLDisjointDataProperties(
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                 new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                 new OWLDataProperty(RDFVocabulary.FOAF.TITLE)])
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = disjointDataProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 16);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.MEMBERS, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.TITLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}