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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Axioms
{
    [TestClass]
    public class OWLFunctionalDataPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateFunctionalDataProperty()
        {
			OWLFunctionalDataProperty functionalDataProperty = new OWLFunctionalDataProperty(
				new OWLDataProperty(RDFVocabulary.RDFS.LABEL));

			Assert.IsNotNull(functionalDataProperty);
			Assert.IsNotNull(functionalDataProperty.DataProperty);
            Assert.IsTrue(string.Equals(functionalDataProperty.DataProperty.IRI, RDFVocabulary.RDFS.LABEL.ToString()));
		}

		[TestMethod]
        public void ShouldThrowExceptionOnCreatingFunctionalDataPropertyBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLFunctionalDataProperty(null));

		[TestMethod]
        public void ShouldSerializeFunctionalDataProperty()
        {
            OWLFunctionalDataProperty functionalDataProperty = new OWLFunctionalDataProperty(
				new OWLDataProperty(RDFVocabulary.RDFS.LABEL));
            string serializedXML = OWLTestSerializer<OWLFunctionalDataProperty>.Serialize(functionalDataProperty);

            Assert.IsTrue(string.Equals(serializedXML,
@"<FunctionalDataProperty><DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" /></FunctionalDataProperty>"));
        }

        [TestMethod]
        public void ShouldSerializeFunctionalDataPropertyViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DataPropertyAxioms.Add(
                new OWLFunctionalDataProperty(
					new OWLDataProperty(RDFVocabulary.RDFS.LABEL)));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><FunctionalDataProperty><DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" /></FunctionalDataProperty></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeFunctionalDataProperty()
        {
            OWLFunctionalDataProperty functionalDataProperty = OWLTestSerializer<OWLFunctionalDataProperty>.Deserialize(
@"<FunctionalDataProperty>
  <DataProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
</FunctionalDataProperty>");

            Assert.IsNotNull(functionalDataProperty);
			Assert.IsNotNull(functionalDataProperty.DataProperty);
            Assert.IsTrue(string.Equals(functionalDataProperty.DataProperty.IRI, RDFVocabulary.RDFS.LABEL.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeFunctionalDataPropertyViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <FunctionalDataProperty>
    <Annotation>
	  <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
	  <Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
  </FunctionalDataProperty>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLFunctionalDataProperty funcDtProp
                            && string.Equals(funcDtProp.DataProperty.IRI, RDFVocabulary.FOAF.AGE.ToString()));
			Assert.IsTrue(ontology.DataPropertyAxioms.Single() is OWLFunctionalDataProperty funcDtProp1
							&& string.Equals(funcDtProp1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(funcDtProp1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(funcDtProp1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvertFunctionalDataPropertyToGraph()
        {
            OWLFunctionalDataProperty functionalDataProperty = new OWLFunctionalDataProperty(
                new OWLDataProperty(RDFVocabulary.RDFS.LABEL));
            RDFGraph graph = functionalDataProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 2);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.LABEL, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.LABEL, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertFunctionalDataPropertyWithAnnotationToGraph()
        {
            OWLFunctionalDataProperty functionalDataProperty = new OWLFunctionalDataProperty(
                new OWLDataProperty(RDFVocabulary.RDFS.LABEL))
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = functionalDataProperty.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 8);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.LABEL, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.LABEL, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.RDFS.LABEL, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.RDF.TYPE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}