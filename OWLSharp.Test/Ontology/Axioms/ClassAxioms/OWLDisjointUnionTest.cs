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
    public class OWLDisjointUnionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDisjointUnion()
        {
            OWLDisjointUnion disjointUnion = new OWLDisjointUnion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]);

            Assert.IsNotNull(disjointUnion);
            Assert.IsNotNull(disjointUnion.ClassIRI);
            Assert.IsTrue(string.Equals(disjointUnion.ClassIRI.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(disjointUnion.ClassExpressions);
            Assert.IsTrue(string.Equals(((OWLClass)disjointUnion.ClassExpressions[0]).IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsTrue(string.Equals(((OWLClass)disjointUnion.ClassExpressions[1]).IRI, RDFVocabulary.FOAF.ORGANIZATION.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointUnionBecauseNullClassIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointUnion(
                null,
                [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointUnionBecauseNullClassExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointUnion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointUnionBecauseLessThan2CLassExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointUnion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLClass(RDFVocabulary.FOAF.PERSON)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointUnionBecauseFoundNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointUnion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLClass(RDFVocabulary.FOAF.PERSON), null]));

        [TestMethod]
        public void ShouldSerializeDisjointUnion()
        {
            OWLDisjointUnion disjointUnion = new OWLDisjointUnion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)]);
            string serializedXML = OWLSerializer.Serialize(disjointUnion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DisjointUnion><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Class IRI=""http://xmlns.com/foaf/0.1/Organization"" /></DisjointUnion>"));
        }

        [TestMethod]
        public void ShouldSerializeDisjointUnionViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ClassAxioms.Add(
                new OWLDisjointUnion(
                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                    [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)]));
            string serializedXML = OWLSerializer.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><DisjointUnion><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Class IRI=""http://xmlns.com/foaf/0.1/Organization"" /></DisjointUnion></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeDisjointUnion()
        {
            OWLDisjointUnion disjointUnion = OWLSerializer.Deserialize<OWLDisjointUnion>(
@"<DisjointUnion><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Class IRI=""http://xmlns.com/foaf/0.1/Organization"" /></DisjointUnion>");

            Assert.IsNotNull(disjointUnion);
            Assert.IsNotNull(disjointUnion.ClassIRI);
            Assert.IsTrue(string.Equals(disjointUnion.ClassIRI.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
            Assert.IsNotNull(disjointUnion.ClassExpressions);
            Assert.IsTrue(string.Equals(((OWLClass)disjointUnion.ClassExpressions[0]).IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsTrue(string.Equals(((OWLClass)disjointUnion.ClassExpressions[1]).IRI, RDFVocabulary.FOAF.ORGANIZATION.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeDisjointUnionViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<Ontology xmlns:foaf=""http://xmlns.com/foaf/0.1/"">
	<Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#""/>
	<Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#""/>
	<Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""/>
	<Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#""/>
	<Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace""/>
    <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
	<DisjointUnion>
        <Annotation>
		    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
		    <Literal xml:lang=""EN"">Steve</Literal>
	    </Annotation>
		<Class IRI=""http://xmlns.com/foaf/0.1/Agent""/>
		<Class IRI=""http://xmlns.com/foaf/0.1/Person""/>
		<Class abbreviatedIRI=""foaf:Organization""/>
	</DisjointUnion>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ClassAxioms.Count == 1);
            Assert.IsTrue(ontology.ClassAxioms.Single() is OWLDisjointUnion djUnAsn
                            && string.Equals(djUnAsn.ClassIRI.IRI, "http://xmlns.com/foaf/0.1/Agent")
                            && string.Equals(((OWLClass)djUnAsn.ClassExpressions[0]).IRI, "http://xmlns.com/foaf/0.1/Person")
                            && string.Equals(((OWLClass)djUnAsn.ClassExpressions[1]).AbbreviatedIRI.ToString(), "http://xmlns.com/foaf/0.1/:Organization"));
			Assert.IsTrue(ontology.ClassAxioms.Single() is OWLDisjointUnion djUnAsn1
							&& string.Equals(djUnAsn1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(djUnAsn1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(djUnAsn1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

		[TestMethod]
        public void ShouldConvertDisjointUnionToGraph()
        {
            OWLDisjointUnion disjointUnion = new OWLDisjointUnion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]);
			RDFGraph graph = disjointUnion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 10);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.ORGANIZATION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.ORGANIZATION, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
        }

        [TestMethod]
        public void ShouldConvertDisjointUnionWithAnnotationToGraph()
        {
            OWLDisjointUnion disjointUnion = new OWLDisjointUnion(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)])
            {
                Annotations = [
                    new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                ]
            };
            RDFGraph graph = disjointUnion.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 16);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.ORGANIZATION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.ORGANIZATION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.TITLE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.DISJOINT_UNION_OF, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.TITLE, new RDFResource("ex:title"), null].TriplesCount == 1);
        }
        #endregion
    }
}