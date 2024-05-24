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
    public class OWLDifferentIndividualsTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDifferentIndividuals()
        {
            OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLNamedIndividual(new RDFResource("ex:Carl")) ]);

            Assert.IsNotNull(differentIndividuals);
            Assert.IsNotNull(differentIndividuals.IndividualExpressions);
			Assert.IsTrue(differentIndividuals.IndividualExpressions.Count == 3);
            Assert.IsTrue(differentIndividuals.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Alice")));
			Assert.IsTrue(differentIndividuals.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Bob")));
			Assert.IsTrue(differentIndividuals.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Carl")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDIfferentIndividualsBecauseNullIndividualExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDifferentIndividuals(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDIfferentIndividualsBecauseLessThan2IndividualExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDifferentIndividuals([new OWLNamedIndividual(new RDFResource("ex:Carl"))]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDIfferentIndividualsBecauseFoundNullIndividualExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLDifferentIndividuals([new OWLNamedIndividual(new RDFResource("ex:Carl")), null]));

        [TestMethod]
        public void ShouldSerializeDifferentIndividuals()
        {
            OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLNamedIndividual(new RDFResource("ex:Carl")) ]);
            string serializedXML = OWLTestSerializer<OWLDifferentIndividuals>.Serialize(differentIndividuals);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DifferentIndividuals><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /><NamedIndividual IRI=""ex:Carl"" /></DifferentIndividuals>"));
        }

		[TestMethod]
        public void ShouldSerializeDifferentIndividualsViaOntology()
        {
			OWLOntology ontology = new OWLOntology();
			ontology.AssertionAxioms.Add(
				new OWLDifferentIndividuals(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLAnonymousIndividual("AnonIdv") ]));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><DifferentIndividuals><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /><AnonymousIndividual nodeID=""AnonIdv"" /></DifferentIndividuals></Ontology>"));
        }

		[TestMethod]
        public void ShouldDeserializeNamedIndividuals()
        {
            OWLDifferentIndividuals differentIndividuals = OWLTestSerializer<OWLDifferentIndividuals>.Deserialize(
@"<DifferentIndividuals><NamedIndividual IRI=""ex:Alice"" /><NamedIndividual IRI=""ex:Bob"" /><NamedIndividual IRI=""ex:Carl"" /></DifferentIndividuals>");
        
			Assert.IsNotNull(differentIndividuals);
            Assert.IsNotNull(differentIndividuals.IndividualExpressions);
			Assert.IsTrue(differentIndividuals.IndividualExpressions.Count == 3);
            Assert.IsTrue(differentIndividuals.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Alice")));
			Assert.IsTrue(differentIndividuals.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Bob")));
			Assert.IsTrue(differentIndividuals.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv
							&& string.Equals(nidv.IRI, "ex:Carl")));
		}

		[TestMethod]
        public void ShouldDeserializeNamedIndividualsViaOntology()
        {
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Ontology>
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <DifferentIndividuals>
    <Annotation>
		<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
		<Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <NamedIndividual IRI=""ex:Alice"" />
    <NamedIndividual IRI=""ex:Bob"" />
    <AnonymousIndividual nodeID=""AnonIdv"" />
  </DifferentIndividuals>
</Ontology>
");

			Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.AssertionAxioms.Count == 1);
            Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLDifferentIndividuals diffAsn
                            && diffAsn.IndividualExpressions.Count == 3
							&& diffAsn.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv 
								&& string.Equals(nidv.IRI, "ex:Alice"))
							&& diffAsn.IndividualExpressions.Any(iex => iex is OWLNamedIndividual nidv 
								&& string.Equals(nidv.IRI, "ex:Bob"))
							&& diffAsn.IndividualExpressions.Any(iex => iex is OWLAnonymousIndividual anonidv 
								&& string.Equals(anonidv.NodeID, "AnonIdv")));
			Assert.IsTrue(ontology.AssertionAxioms.Single() is OWLDifferentIndividuals diffAsn1
							&& string.Equals(diffAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(diffAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(diffAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

		[TestMethod]
        public void ShouldConvert2DifferentIndividualsToGraph()
        {
            OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")) ]);
            RDFGraph graph = differentIndividuals.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[new RDFResource("ex:Alice"), RDFVocabulary.OWL.DIFFERENT_FROM, new RDFResource("ex:Bob"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert3DifferentIndividualsToGraph()
        {
            OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLNamedIndividual(new RDFResource("ex:Carl")) ]);
            RDFGraph graph = differentIndividuals.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 14);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.DISTINCT_MEMBERS, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:Alice"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:Bob"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:Carl"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 3);
            Assert.IsTrue(graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Carl"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvert2DifferentIndividualsWithAnnotationToGraph()
        {
            OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")) ])
			{
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = differentIndividuals.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 9);
            Assert.IsTrue(graph[new RDFResource("ex:Alice"), RDFVocabulary.OWL.DIFFERENT_FROM, new RDFResource("ex:Bob"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
			//Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:Alice"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.DIFFERENT_FROM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, new RDFResource("ex:Bob"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert3DifferentIndividualsWithAnnotationToGraph()
        {
            OWLDifferentIndividuals differentIndividuals = new OWLDifferentIndividuals(
                [ new OWLNamedIndividual(new RDFResource("ex:Alice")),
				  new OWLNamedIndividual(new RDFResource("ex:Bob")),
				  new OWLNamedIndividual(new RDFResource("ex:Carl")) ])
			{
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = differentIndividuals.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 16);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.DISTINCT_MEMBERS, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:Alice"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:Bob"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:Carl"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 3);
            Assert.IsTrue(graph[new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Carl"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
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