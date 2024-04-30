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
using OWLSharp.Ontology.Expressions;
using OWLSharp.Test;
using RDFSharp.Model;
using System.Linq;

namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLDisjointObjectPropertiesTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDisjointObjectProperties()
        {
            OWLDisjointObjectProperties disjointObjectProperties = new OWLDisjointObjectProperties(
                [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]);

            Assert.IsNotNull(disjointObjectProperties);
            Assert.IsNotNull(disjointObjectProperties.ObjectPropertyExpressions);
            Assert.IsTrue(string.Equals(((OWLObjectProperty)disjointObjectProperties.ObjectPropertyExpressions[0]).IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(string.Equals(((OWLObjectProperty)disjointObjectProperties.ObjectPropertyExpressions[1]).IRI, RDFVocabulary.FOAF.MEMBER.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointObjectPropertiesBecauseNullObjectProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointObjectProperties(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointObjectPropertiesBecauseLessThan2ObjectProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointObjectProperties(
                 [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDisjointObjectPropertiesBecauseFoundNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDisjointObjectProperties(
                 [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), null ]));

        [TestMethod]
        public void ShouldSerializeDisjointObjectProperties()
        {
            OWLDisjointObjectProperties disjointObjectProperties = new OWLDisjointObjectProperties(
                [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]);
            string serializedXML = OWLTestSerializer<OWLDisjointObjectProperties>.Serialize(disjointObjectProperties);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DisjointObjectProperties><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" /></DisjointObjectProperties>"));
        }

        [TestMethod]
        public void ShouldSerializeDisjointObjectPropertiesViaOntology()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.ObjectPropertyAxioms.Add(
                new OWLDisjointObjectProperties(
                [ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]));
            string serializedXML = OWLTestSerializer<OWLOntology>.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Ontology><Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" /><Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" /><Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" /><Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" /><Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" /><DisjointObjectProperties><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" /></DisjointObjectProperties></Ontology>"));
        }

        [TestMethod]
        public void ShouldDeserializeDisjointObjectProperties()
        {
            OWLDisjointObjectProperties disjointObjectProperties = OWLTestSerializer<OWLDisjointObjectProperties>.Deserialize(
@"<DisjointObjectProperties>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" />
</DisjointObjectProperties>");

            Assert.IsNotNull(disjointObjectProperties);
            Assert.IsNotNull(disjointObjectProperties.ObjectPropertyExpressions);
            Assert.IsTrue(string.Equals(((OWLObjectProperty)disjointObjectProperties.ObjectPropertyExpressions[0]).IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(string.Equals(((OWLObjectProperty)disjointObjectProperties.ObjectPropertyExpressions[1]).IRI, RDFVocabulary.FOAF.MEMBER.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeDisjointObjectPropertiesViaOntology()
        {
            OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <DisjointObjectProperties>
    <Annotation>
		<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/contributor"" />
		<Literal xml:lang=""EN"">Steve</Literal>
	</Annotation>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" />
  </DisjointObjectProperties>
</Ontology>");

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLDisjointObjectProperties djObjProps
                            && string.Equals(((OWLObjectProperty)djObjProps.ObjectPropertyExpressions[0]).IRI, "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(((OWLObjectProperty)djObjProps.ObjectPropertyExpressions[1]).IRI, "http://xmlns.com/foaf/0.1/member"));
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Single() is OWLDisjointObjectProperties djDtProps1
							&& string.Equals(djDtProps1.Annotations.Single().AnnotationProperty.IRI, "http://purl.org/dc/elements/1.1/contributor")
							&& string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Value, "Steve")
							&& string.Equals(djDtProps1.Annotations.Single().ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldConvert2DisjointObjectPropertiesToGraph()
        {
            OWLDisjointObjectProperties disjointObjectProperties = new OWLDisjointObjectProperties(
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER)]);
            RDFGraph graph = disjointObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, RDFVocabulary.FOAF.MEMBER, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvert3DisjointObjectPropertiesToGraph()
        {
            OWLDisjointObjectProperties disjointObjectProperties = new OWLDisjointObjectProperties(
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                 new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER),
                 new OWLObjectProperty(RDFVocabulary.FOAF.ACCOUNT)]);
            RDFGraph graph = disjointObjectProperties.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 14);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.MEMBERS, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.MEMBER, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.ACCOUNT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 3);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.ACCOUNT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }
        #endregion
    }
}