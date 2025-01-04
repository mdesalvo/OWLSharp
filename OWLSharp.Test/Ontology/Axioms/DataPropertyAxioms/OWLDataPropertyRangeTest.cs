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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLDataPropertyRangeTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataPropertyRange()
        {
			OWLDataPropertyRange dataPropertyRange = new OWLDataPropertyRange(
				new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
				new OWLDatatype(RDFVocabulary.XSD.STRING));

			Assert.IsNotNull(dataPropertyRange);
			Assert.IsNotNull(dataPropertyRange.DataProperty);
            Assert.IsTrue(string.Equals(dataPropertyRange.DataProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(dataPropertyRange.DataRangeExpression);
            Assert.IsTrue(dataPropertyRange.DataRangeExpression is OWLDatatype dt 
							&& string.Equals(dt.IRI, RDFVocabulary.XSD.STRING.ToString()));
		}

		[TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyRangeBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataPropertyRange(
                null,
                new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyRangeBecauseNullDataRangeExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLDataPropertyRange(
                new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
                null));

		[TestMethod]
        public void ShouldSerializeDataPropertyRange()
        {
            OWLDataPropertyRange dataPropertyRange = new OWLDataPropertyRange(
				new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
				new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLSerializer.SerializeObject(dataPropertyRange);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataPropertyRange><DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" /><Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" /></DataPropertyRange>"));
        }

        [TestMethod]
        public void ShouldSerializeDataPropertyRangeViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DataPropertyAxioms.Add(
                new OWLDataPropertyRange(
					new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
					new OWLDatatype(RDFVocabulary.XSD.STRING)));
            string serializedXML = OWLSerializer.SerializeObject<OWLOntology>(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><DataPropertyRange><DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" /><Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" /></DataPropertyRange></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataPropertyRange()
        {
            OWLDataPropertyRange dataPropertyRange = OWLSerializer.DeserializeObject<OWLDataPropertyRange>(
@"<DataPropertyRange>
  <DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataPropertyRange>");

            Assert.IsNotNull(dataPropertyRange);
			Assert.IsNotNull(dataPropertyRange.DataProperty);
            Assert.IsTrue(string.Equals(dataPropertyRange.DataProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(dataPropertyRange.DataRangeExpression);
            Assert.IsTrue(dataPropertyRange.DataRangeExpression is OWLDatatype dt 
							&& string.Equals(dt.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeDataPropertyRangeViaOntology()
        {
            OWLOntology ontology = OWLSerializer.DeserializeOntology(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <DataPropertyRange>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
    <Datatype IRI=""http://www.w3.org/2001/XMLSchema#integer"" />
  </DataPropertyRange>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLDataPropertyRange dtPropRng
                            && string.Equals(dtPropRng.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString())
                            && string.Equals(((OWLDatatype)dtPropRng.DataRangeExpression).IRI, RDFVocabulary.XSD.INTEGER.ToString()));
			Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLDataPropertyRange dtPropRng1
							&& string.Equals(dtPropRng1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(dtPropRng1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(dtPropRng1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertDataPropertyRangeToGraph()
        {
            OWLDataPropertyRange dataPropertyRange = new OWLDataPropertyRange(
                new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            RDFGraph graph = dataPropertyRange.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDFS.RANGE, RDFVocabulary.XSD.STRING, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertDataPropertyRangeWithAnnotationToGraph()
        {
            OWLDataPropertyRange dataPropertyRange = new OWLDataPropertyRange(
                new OWLDataProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLDatatype(RDFVocabulary.XSD.STRING))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = dataPropertyRange.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 9);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDFS.RANGE, RDFVocabulary.XSD.STRING, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.RDFS.COMMENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDFS.RANGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.XSD.STRING, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}