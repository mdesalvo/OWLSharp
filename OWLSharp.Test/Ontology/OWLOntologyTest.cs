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

using System;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Test
{
    [TestClass]
    public class OWLOntologyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateOntology()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));

            Assert.IsNotNull(ontology);
            Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont"));
			Assert.IsTrue(string.Equals(ontology.VersionIRI, "ex:ont/v1"));
			Assert.IsNotNull(ontology.Prefixes);
			Assert.IsTrue(ontology.Prefixes.Count == 5);
			Assert.IsNotNull(ontology.Imports);
			Assert.IsTrue(ontology.Imports.Count == 0);
			Assert.IsNotNull(ontology.Annotations);
			Assert.IsTrue(ontology.Annotations.Count == 0);
			Assert.IsNotNull(ontology.DeclarationAxioms);
			Assert.IsTrue(ontology.DeclarationAxioms.Count == 0);
			Assert.IsNotNull(ontology.ClassAxioms);
			Assert.IsTrue(ontology.ClassAxioms.Count == 0);
			Assert.IsNotNull(ontology.ObjectPropertyAxioms);
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 0);
			Assert.IsNotNull(ontology.DataPropertyAxioms);
			Assert.IsTrue(ontology.DataPropertyAxioms.Count == 0);
			Assert.IsNotNull(ontology.DatatypeDefinitionAxioms);
			Assert.IsTrue(ontology.DatatypeDefinitionAxioms.Count == 0);
			Assert.IsNotNull(ontology.KeyAxioms);
			Assert.IsTrue(ontology.KeyAxioms.Count == 0);
			Assert.IsNotNull(ontology.AssertionAxioms);
			Assert.IsTrue(ontology.AssertionAxioms.Count == 0);
			Assert.IsNotNull(ontology.AnnotationAxioms);
			Assert.IsTrue(ontology.AnnotationAxioms.Count == 0);
        }

		[TestMethod]
		public void ShouldSerializeOntology()
		{
			OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
			ontology.Prefixes.Add(
				new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
			ontology.Imports.Add(
				new OWLImport(new RDFResource("ex:ont2")));
			ontology.Annotations.Add(
				new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("annotation")))
				{ 
					Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("nested annotation")))
				});
			ontology.DeclarationAxioms.AddRange(
				[ new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)), 
				  new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)), 
				  new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
				  new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Steve"))) ]);
			ontology.ClassAxioms.Add(
				new OWLDisjointClasses([ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]));
			ontology.ObjectPropertyAxioms.Add(
				new OWLAsymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
			ontology.DataPropertyAxioms.Add(
				new OWLDataPropertyDomain(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLClass(RDFVocabulary.FOAF.PERSON)));
			ontology.DatatypeDefinitionAxioms.Add(
				new OWLDatatypeDefinition(
					new OWLDatatype(new RDFResource("ex:length6to10")),
					new OWLDatatypeRestriction(
						new OWLDatatype(RDFVocabulary.XSD.STRING),
						[ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
						  new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH) ])));
			ontology.KeyAxioms.Add(
				new OWLHasKey(
					new OWLClass(RDFVocabulary.FOAF.AGENT),
					[ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ],
					[ new OWLDataProperty(RDFVocabulary.FOAF.AGE) ]));
			ontology.AssertionAxioms.Add(
				new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Mark")), new OWLNamedIndividual(new RDFResource("ex:Steve"))));
			ontology.AnnotationAxioms.Add(
				new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),new RDFResource("ex:Mark"),new OWLLiteral(new RDFPlainLiteral("This is Mark"))));
			string serializedXML = OWLSerializer.Serialize(ontology);

            Assert.IsTrue(string.Equals(serializedXML,
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:foaf=""http://xmlns.com/foaf/0.1/"" ontologyIRI=""ex:ont"" ontologyVersion=""ex:ont/v1"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
  <Import>ex:ont2</Import>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
      <Literal>nested annotation</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
    <Literal>annotation</Literal>
  </Annotation>
  <Declaration>
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://xmlns.com/foaf/0.1/Organization"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
  </Declaration>
  <Declaration>
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  </Declaration>
  <Declaration>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  </Declaration>
  <Declaration>
    <NamedIndividual IRI=""ex:Mark"" />
  </Declaration>
  <Declaration>
    <NamedIndividual IRI=""ex:Steve"" />
  </Declaration>
  <DisjointClasses>
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
    <Class IRI=""http://xmlns.com/foaf/0.1/Organization"" />
  </DisjointClasses>
  <AsymmetricObjectProperty>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </AsymmetricObjectProperty>
  <DataPropertyDomain>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  </DataPropertyDomain>
  <DatatypeDefinition>
    <Datatype IRI=""ex:length6to10"" />
    <DatatypeRestriction>
      <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
      <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#minLength"">
        <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">6</Literal>
      </FacetRestriction>
      <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#maxLength"">
        <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">10</Literal>
      </FacetRestriction>
    </DatatypeRestriction>
  </DatatypeDefinition>
  <HasKey>
    <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
  </HasKey>
  <ObjectPropertyAssertion>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <NamedIndividual IRI=""ex:Mark"" />
    <NamedIndividual IRI=""ex:Steve"" />
  </ObjectPropertyAssertion>
  <AnnotationAssertion>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <IRI>ex:Mark</IRI>
    <Literal>This is Mark</Literal>
  </AnnotationAssertion>
</Ontology>")); 
		}

		[TestMethod]
		public void ShouldDeserializeOntology()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:foaf=""http://xmlns.com/foaf/0.1/"" ontologyIRI=""ex:ont"" ontologyVersion=""ex:ont/v1"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
  <Import>ex:ont2</Import>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
      <Literal>nested annotation</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
    <Literal>annotation</Literal>
  </Annotation>
  <Declaration>
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://xmlns.com/foaf/0.1/Organization"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
  </Declaration>
  <Declaration>
    <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  </Declaration>
  <Declaration>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  </Declaration>
  <Declaration>
    <NamedIndividual IRI=""ex:Mark"" />
  </Declaration>
  <Declaration>
    <NamedIndividual IRI=""ex:Steve"" />
  </Declaration>
  <DisjointClasses>
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
    <Class IRI=""http://xmlns.com/foaf/0.1/Organization"" />
  </DisjointClasses>
  <AsymmetricObjectProperty>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </AsymmetricObjectProperty>
  <DataPropertyDomain>
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
    <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  </DataPropertyDomain>
  <DatatypeDefinition>
    <Datatype IRI=""ex:length6to10"" />
    <DatatypeRestriction>
      <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
      <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#minLength"">
        <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">6</Literal>
      </FacetRestriction>
      <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#maxLength"">
        <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">10</Literal>
      </FacetRestriction>
    </DatatypeRestriction>
  </DatatypeDefinition>
  <HasKey>
    <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
  </HasKey>
  <ObjectPropertyAssertion>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
    <NamedIndividual IRI=""ex:Mark"" />
    <NamedIndividual IRI=""ex:Steve"" />
  </ObjectPropertyAssertion>
  <AnnotationAssertion>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <IRI>ex:Mark</IRI>
    <Literal>This is Mark</Literal>
  </AnnotationAssertion>
</Ontology>");

			Assert.IsNotNull(ontology);
			Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont"));
			Assert.IsTrue(string.Equals(ontology.VersionIRI, "ex:ont/v1"));
			Assert.IsNotNull(ontology.Prefixes);
			Assert.IsTrue(ontology.Prefixes.Count == 6);
			Assert.IsNotNull(ontology.Imports);
			Assert.IsTrue(ontology.Imports.Count == 1);
			Assert.IsNotNull(ontology.Annotations);
			Assert.IsTrue(ontology.Annotations.Count == 1);
			Assert.IsNotNull(ontology.DeclarationAxioms);
			Assert.IsTrue(ontology.DeclarationAxioms.Count == 8);
			Assert.IsNotNull(ontology.ClassAxioms);
			Assert.IsTrue(ontology.ClassAxioms.Count == 1);
			Assert.IsNotNull(ontology.ObjectPropertyAxioms);
			Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology.DataPropertyAxioms);
			Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology.DatatypeDefinitionAxioms);
			Assert.IsTrue(ontology.DatatypeDefinitionAxioms.Count == 1);
			Assert.IsNotNull(ontology.KeyAxioms);
			Assert.IsTrue(ontology.KeyAxioms.Count == 1);
			Assert.IsNotNull(ontology.AssertionAxioms);
			Assert.IsTrue(ontology.AssertionAxioms.Count == 1);
			Assert.IsNotNull(ontology.AnnotationAxioms);
			Assert.IsTrue(ontology.AnnotationAxioms.Count == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithPrefixAndImportAndAnnotationToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" ontologyIRI=""ex:ont"" ontologyVersion=""ex:ont/v1"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Import>ex:ont2</Import>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
      <Literal>nested annotation</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2002/07/owl#versionInfo"" />
    <Literal>v1.0</Literal>
  </Annotation>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.Context.Equals(new Uri("ex:ont")));
			Assert.IsTrue(graph.TriplesCount == 11);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.OWL.VERSION_IRI, new RDFResource("ex:ont/v1"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.OWL.IMPORTS, new RDFResource("ex:ont2"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.OWL.VERSION_INFO, null, new RDFPlainLiteral("v1.0")].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.OWL.VERSION_INFO, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
            //Annotations
            Assert.IsTrue(graph[RDFVocabulary.DC.DESCRIPTION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.AXIOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_SOURCE, new RDFResource("ex:ont"), null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_PROPERTY, RDFVocabulary.OWL.VERSION_INFO, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ANNOTATED_TARGET, null, new RDFPlainLiteral("v1.0")].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.DC.DESCRIPTION, null, new RDFPlainLiteral("nested annotation")].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithDeclarationToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" ontologyIRI=""ex:ont"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Declaration>
	<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  </Declaration>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 2);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.DC.DESCRIPTION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithClassAxiomToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" ontologyIRI=""ex:ont"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <SubClassOf>
	<Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
	<Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
  </SubClassOf>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 4);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDFS.SUB_CLASS_OF, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithObjectPropertyAxiomToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" ontologyIRI=""ex:ont"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <SubObjectPropertyOf>
	<ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
	<ObjectProperty IRI=""http://xmlns.com/foaf/0.1/member"" />
  </SubObjectPropertyOf>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 4);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.MEMBER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.FOAF.MEMBER, null].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithDataPropertyAxiomToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" ontologyIRI=""ex:ont"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <SubDataPropertyOf>
	<DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />
	<DataProperty IRI=""http://xmlns.com/foaf/0.1/name"" />
  </SubDataPropertyOf>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 4);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithDatatypeDefinitionAxiomToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<Ontology>
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <DatatypeDefinition>
    <Datatype IRI=""ex:length6to10"" />
    <DatatypeRestriction>
      <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
      <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#minLength"">
        <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">6</Literal>
      </FacetRestriction>
      <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#maxLength"">
        <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">10</Literal>
      </FacetRestriction>
    </DatatypeRestriction>
  </DatatypeDefinition>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.Context.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
			Assert.IsTrue(graph.TriplesCount == 15);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:length6to10"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 1);
			Assert.IsTrue(graph[new RDFResource("ex:length6to10"), RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 3);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_DATATYPE, RDFVocabulary.XSD.STRING, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.OWL.WITH_RESTRICTIONS, null, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, null, null].TriplesCount == 2);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
			Assert.IsTrue(graph[null, OWLFacetRestriction.MIN_LENGTH, null, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)].TriplesCount == 1);
			Assert.IsTrue(graph[null, OWLFacetRestriction.MAX_LENGTH, null, new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithHasKeyAxiomToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:foaf=""http://xmlns.com/foaf/0.1/"" ontologyIRI=""ex:ont"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
  <HasKey>
    <Class abbreviatedIRI=""foaf:Person"" />
	<ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
	<DataProperty abbreviatedIRI=""foaf:name"" />
  </HasKey>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 11);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.OWL.HAS_KEY, null, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.NAME, null].TriplesCount == 1);
			Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithAssertionAxiomToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:foaf=""http://xmlns.com/foaf/0.1/"" ontologyIRI=""ex:ont"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
  <ObjectPropertyAssertion>
    <ObjectInverseOf>
      <ObjectProperty abbreviatedIRI=""foaf:knows"" />
    </ObjectInverseOf>
    <AnonymousIndividual nodeID=""Alice"" />
    <NamedIndividual IRI=""ex:Bob"" />
  </ObjectPropertyAssertion>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 4);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.FOAF.KNOWS, new RDFResource("bnode:Alice"), null].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldConvertOntologyWithAnnotationAxiomToGraph()
		{
			OWLOntology ontology = OWLSerializer.Deserialize(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:foaf=""http://xmlns.com/foaf/0.1/"" ontologyIRI=""ex:ont"">
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />
  <AnnotationAssertion>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <IRI>http://xmlns.com/foaf/0.1/age</IRI>
    <Literal xml:lang=""en-US"">States the age of a person</Literal>    
  </AnnotationAssertion>
</Ontology>");
			RDFGraph graph = ontology.ToRDFGraph();

			Assert.IsNotNull(graph);
			Assert.IsTrue(graph.TriplesCount == 3);
			Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.RDFS.COMMENT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("States the age of a person", "en-US")].TriplesCount == 1);
		}

		[TestMethod]
		public void ShouldWriteOntologyToFile()
		{
			OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
			ontology.Prefixes.Add(
				new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
			ontology.Imports.Add(
				new OWLImport(new RDFResource("ex:ont2")));
			ontology.Annotations.Add(
				new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("annotation")))
				{ 
					Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("nested annotation")))
				});
			ontology.DeclarationAxioms.AddRange(
				[ new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)), 
				  new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)), 
				  new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
				  new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Steve"))) ]);
			ontology.ClassAxioms.Add(
				new OWLDisjointClasses([ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]));
			ontology.ObjectPropertyAxioms.Add(
				new OWLAsymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
			ontology.DataPropertyAxioms.Add(
				new OWLDataPropertyDomain(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLClass(RDFVocabulary.FOAF.PERSON)));
			ontology.DatatypeDefinitionAxioms.Add(
				new OWLDatatypeDefinition(
					new OWLDatatype(new RDFResource("ex:length6to10")),
					new OWLDatatypeRestriction(
						new OWLDatatype(RDFVocabulary.XSD.STRING),
						[ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
						  new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH) ])));
			ontology.KeyAxioms.Add(
				new OWLHasKey(
					new OWLClass(RDFVocabulary.FOAF.AGENT),
					[ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ],
					[ new OWLDataProperty(RDFVocabulary.FOAF.AGE) ]));
			ontology.AssertionAxioms.Add(
				new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Mark")), new OWLNamedIndividual(new RDFResource("ex:Steve"))));
			ontology.AnnotationAxioms.Add(
				new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),new RDFResource("ex:Mark"),new OWLLiteral(new RDFPlainLiteral("This is Mark"))));
			
			//Write to file
			ontology.ToFile(OWLEnums.OWLFormats.Owl2Xml, Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldWriteOntologyToFile.owx"));
			Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldWriteOntologyToFile.owx")));

			//Read from file and deserialize to test content
			OWLOntology ontology2 = OWLSerializer.Deserialize(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldWriteOntologyToFile.owx")));

			Assert.IsNotNull(ontology2);
			Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
			Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
			Assert.IsNotNull(ontology2.Prefixes);
			Assert.IsTrue(ontology2.Prefixes.Count == 6);
			Assert.IsNotNull(ontology2.Imports);
			Assert.IsTrue(ontology2.Imports.Count == 1);
			Assert.IsNotNull(ontology2.Annotations);
			Assert.IsTrue(ontology2.Annotations.Count == 1);
			Assert.IsNotNull(ontology2.DeclarationAxioms);
			Assert.IsTrue(ontology2.DeclarationAxioms.Count == 8);
			Assert.IsNotNull(ontology2.ClassAxioms);
			Assert.IsTrue(ontology2.ClassAxioms.Count == 1);
			Assert.IsNotNull(ontology2.ObjectPropertyAxioms);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DataPropertyAxioms);
			Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DatatypeDefinitionAxioms);
			Assert.IsTrue(ontology2.DatatypeDefinitionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.KeyAxioms);
			Assert.IsTrue(ontology2.KeyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AssertionAxioms);
			Assert.IsTrue(ontology2.AssertionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AnnotationAxioms);
			Assert.IsTrue(ontology2.AnnotationAxioms.Count == 1);			
		}

		[TestMethod]
		public void ShouldThrowExceptionOnWritingOntologyToFileBecauseNullPath()
			=> Assert.ThrowsException<OWLException>(() => new OWLOntology().ToFile(OWLEnums.OWLFormats.Owl2Xml, null));

		[TestMethod]
		public void ShouldWriteOntologyToStream()
		{
			OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
			ontology.Prefixes.Add(
				new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
			ontology.Imports.Add(
				new OWLImport(new RDFResource("ex:ont2")));
			ontology.Annotations.Add(
				new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("annotation")))
				{ 
					Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("nested annotation")))
				});
			ontology.DeclarationAxioms.AddRange(
				[ new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)), 
				  new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)), 
				  new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
				  new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Steve"))) ]);
			ontology.ClassAxioms.Add(
				new OWLDisjointClasses([ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]));
			ontology.ObjectPropertyAxioms.Add(
				new OWLAsymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
			ontology.DataPropertyAxioms.Add(
				new OWLDataPropertyDomain(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLClass(RDFVocabulary.FOAF.PERSON)));
			ontology.DatatypeDefinitionAxioms.Add(
				new OWLDatatypeDefinition(
					new OWLDatatype(new RDFResource("ex:length6to10")),
					new OWLDatatypeRestriction(
						new OWLDatatype(RDFVocabulary.XSD.STRING),
						[ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
						  new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH) ])));
			ontology.KeyAxioms.Add(
				new OWLHasKey(
					new OWLClass(RDFVocabulary.FOAF.AGENT),
					[ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ],
					[ new OWLDataProperty(RDFVocabulary.FOAF.AGE) ]));
			ontology.AssertionAxioms.Add(
				new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Mark")), new OWLNamedIndividual(new RDFResource("ex:Steve"))));
			ontology.AnnotationAxioms.Add(
				new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),new RDFResource("ex:Mark"),new OWLLiteral(new RDFPlainLiteral("This is Mark"))));
			
			//Write to stream
			MemoryStream stream = new MemoryStream();
			ontology.ToStream(OWLEnums.OWLFormats.Owl2Xml, stream);

			//Read from stream and deserialize to test content
			string fileContent;
            using (StreamReader reader = new StreamReader(new MemoryStream(stream.ToArray())))
                fileContent = reader.ReadToEnd();
			OWLOntology ontology2 = OWLSerializer.Deserialize(fileContent);

			Assert.IsNotNull(ontology2);
			Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
			Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
			Assert.IsNotNull(ontology2.Prefixes);
			Assert.IsTrue(ontology2.Prefixes.Count == 6);
			Assert.IsNotNull(ontology2.Imports);
			Assert.IsTrue(ontology2.Imports.Count == 1);
			Assert.IsNotNull(ontology2.Annotations);
			Assert.IsTrue(ontology2.Annotations.Count == 1);
			Assert.IsNotNull(ontology2.DeclarationAxioms);
			Assert.IsTrue(ontology2.DeclarationAxioms.Count == 8);
			Assert.IsNotNull(ontology2.ClassAxioms);
			Assert.IsTrue(ontology2.ClassAxioms.Count == 1);
			Assert.IsNotNull(ontology2.ObjectPropertyAxioms);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DataPropertyAxioms);
			Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DatatypeDefinitionAxioms);
			Assert.IsTrue(ontology2.DatatypeDefinitionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.KeyAxioms);
			Assert.IsTrue(ontology2.KeyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AssertionAxioms);
			Assert.IsTrue(ontology2.AssertionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AnnotationAxioms);
			Assert.IsTrue(ontology2.AnnotationAxioms.Count == 1);			
		}

		[TestMethod]
		public void ShouldThrowExceptionOnWritingOntologyToStreamBecauseNullStream()
			=> Assert.ThrowsException<OWLException>(() => new OWLOntology().ToStream(OWLEnums.OWLFormats.Owl2Xml, null));

		[TestMethod]
		public void ShouldReadOntologyFromFile()
		{
			OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
			ontology.Prefixes.Add(
				new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
			ontology.Imports.Add(
				new OWLImport(new RDFResource("ex:ont2")));
			ontology.Annotations.Add(
				new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("annotation")))
				{ 
					Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("nested annotation")))
				});
			ontology.DeclarationAxioms.AddRange(
				[ new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)), 
				  new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)), 
				  new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
				  new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Steve"))) ]);
			ontology.ClassAxioms.Add(
				new OWLDisjointClasses([ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]));
			ontology.ObjectPropertyAxioms.Add(
				new OWLAsymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
			ontology.DataPropertyAxioms.Add(
				new OWLDataPropertyDomain(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLClass(RDFVocabulary.FOAF.PERSON)));
			ontology.DatatypeDefinitionAxioms.Add(
				new OWLDatatypeDefinition(
					new OWLDatatype(new RDFResource("ex:length6to10")),
					new OWLDatatypeRestriction(
						new OWLDatatype(RDFVocabulary.XSD.STRING),
						[ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
						  new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH) ])));
			ontology.KeyAxioms.Add(
				new OWLHasKey(
					new OWLClass(RDFVocabulary.FOAF.AGENT),
					[ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ],
					[ new OWLDataProperty(RDFVocabulary.FOAF.AGE) ]));
			ontology.AssertionAxioms.Add(
				new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Mark")), new OWLNamedIndividual(new RDFResource("ex:Steve"))));
			ontology.AnnotationAxioms.Add(
				new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),new RDFResource("ex:Mark"),new OWLLiteral(new RDFPlainLiteral("This is Mark"))));
			
			//Write to file
			ontology.ToFile(OWLEnums.OWLFormats.Owl2Xml, Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldReadOntologyFromFile.owx"));
			
			//Read from file
			OWLOntology ontology2 = OWLOntology.FromFile(OWLEnums.OWLFormats.Owl2Xml, Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldReadOntologyFromFile.owx"));

			Assert.IsNotNull(ontology2);
			Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
			Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
			Assert.IsNotNull(ontology2.Prefixes);
			Assert.IsTrue(ontology2.Prefixes.Count == 6);
			Assert.IsNotNull(ontology2.Imports);
			Assert.IsTrue(ontology2.Imports.Count == 1);
			Assert.IsNotNull(ontology2.Annotations);
			Assert.IsTrue(ontology2.Annotations.Count == 1);
			Assert.IsNotNull(ontology2.DeclarationAxioms);
			Assert.IsTrue(ontology2.DeclarationAxioms.Count == 8);
			Assert.IsNotNull(ontology2.ClassAxioms);
			Assert.IsTrue(ontology2.ClassAxioms.Count == 1);
			Assert.IsNotNull(ontology2.ObjectPropertyAxioms);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DataPropertyAxioms);
			Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DatatypeDefinitionAxioms);
			Assert.IsTrue(ontology2.DatatypeDefinitionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.KeyAxioms);
			Assert.IsTrue(ontology2.KeyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AssertionAxioms);
			Assert.IsTrue(ontology2.AssertionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AnnotationAxioms);
			Assert.IsTrue(ontology2.AnnotationAxioms.Count == 1);			
		}

		[TestMethod]
		public void ShouldThrowExceptionOnReadingOntologyFromFileBecauseNullPath()
			=> Assert.ThrowsException<OWLException>(() => OWLOntology.FromFile(OWLEnums.OWLFormats.Owl2Xml, null));

		[TestMethod]
		public void ShouldThrowExceptionOnReadingOntologyFromFileBecauseUnexistingPath()
			=> Assert.ThrowsException<OWLException>(() => OWLOntology.FromFile(OWLEnums.OWLFormats.Owl2Xml, "test/test"));

		[TestMethod]
		public void ShouldReadOntologyFromStream()
		{
			OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
			ontology.Prefixes.Add(
				new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
			ontology.Imports.Add(
				new OWLImport(new RDFResource("ex:ont2")));
			ontology.Annotations.Add(
				new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("annotation")))
				{ 
					Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("nested annotation")))
				});
			ontology.DeclarationAxioms.AddRange(
				[ new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)), 
				  new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)), 
				  new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
				  new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION)),
				  new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
				  new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Steve"))) ]);
			ontology.ClassAxioms.Add(
				new OWLDisjointClasses([ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]));
			ontology.ObjectPropertyAxioms.Add(
				new OWLAsymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
			ontology.DataPropertyAxioms.Add(
				new OWLDataPropertyDomain(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLClass(RDFVocabulary.FOAF.PERSON)));
			ontology.DatatypeDefinitionAxioms.Add(
				new OWLDatatypeDefinition(
					new OWLDatatype(new RDFResource("ex:length6to10")),
					new OWLDatatypeRestriction(
						new OWLDatatype(RDFVocabulary.XSD.STRING),
						[ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
						  new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH) ])));
			ontology.KeyAxioms.Add(
				new OWLHasKey(
					new OWLClass(RDFVocabulary.FOAF.AGENT),
					[ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS) ],
					[ new OWLDataProperty(RDFVocabulary.FOAF.AGE) ]));
			ontology.AssertionAxioms.Add(
				new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Mark")), new OWLNamedIndividual(new RDFResource("ex:Steve"))));
			ontology.AnnotationAxioms.Add(
				new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),new RDFResource("ex:Mark"),new OWLLiteral(new RDFPlainLiteral("This is Mark"))));
			
			//Write to stream
			MemoryStream stream = new MemoryStream();
			ontology.ToStream(OWLEnums.OWLFormats.Owl2Xml, stream);

			//Read from stream
			OWLOntology ontology2 = OWLOntology.FromStream(OWLEnums.OWLFormats.Owl2Xml, new MemoryStream(stream.ToArray()));

			Assert.IsNotNull(ontology2);
			Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
			Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
			Assert.IsNotNull(ontology2.Prefixes);
			Assert.IsTrue(ontology2.Prefixes.Count == 6);
			Assert.IsNotNull(ontology2.Imports);
			Assert.IsTrue(ontology2.Imports.Count == 1);
			Assert.IsNotNull(ontology2.Annotations);
			Assert.IsTrue(ontology2.Annotations.Count == 1);
			Assert.IsNotNull(ontology2.DeclarationAxioms);
			Assert.IsTrue(ontology2.DeclarationAxioms.Count == 8);
			Assert.IsNotNull(ontology2.ClassAxioms);
			Assert.IsTrue(ontology2.ClassAxioms.Count == 1);
			Assert.IsNotNull(ontology2.ObjectPropertyAxioms);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DataPropertyAxioms);
			Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.DatatypeDefinitionAxioms);
			Assert.IsTrue(ontology2.DatatypeDefinitionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.KeyAxioms);
			Assert.IsTrue(ontology2.KeyAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AssertionAxioms);
			Assert.IsTrue(ontology2.AssertionAxioms.Count == 1);
			Assert.IsNotNull(ontology2.AnnotationAxioms);
			Assert.IsTrue(ontology2.AnnotationAxioms.Count == 1);			
		}

		[TestMethod]
		public void ShouldThrowExceptionOnReadingOntologyFromStreamBecauseNullStream()
			=> Assert.ThrowsException<OWLException>(() => OWLOntology.FromStream(OWLEnums.OWLFormats.Owl2Xml, null));

        [TestMethod]
        public void ShouldReadOntologyHeaderFromGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.OWL.VERSION_IRI, new RDFResource("ex:ont/v1")));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology.VersionIRI, "ex:ont/v1"));
        }

        [TestMethod]
        public void ShouldReadOntologyHeaderWithInvalidVersionIRIFromGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.OWL.VERSION_IRI, new RDFPlainLiteral("ex:ont/v1")));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont"));
            Assert.IsNull(ontology.VersionIRI);
        }

        [TestMethod]
        public void ShouldCrashOnReadingOntologyHeaderFromGraphBecauseNoOntologyDeclaration()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.OWL.VERSION_IRI, new RDFResource("ex:ont/v1")));
            Assert.ThrowsException<OWLException>(() => OWLOntology.FromRDFGraph(graph));
        }

        [TestMethod]
        public void ShouldReadImportsFromGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.OWL.IMPORTS, new RDFResource("ex:ont2")));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont"));
            Assert.IsNull(ontology.VersionIRI);
            Assert.IsTrue(ontology.Imports.Count == 1);
            Assert.IsTrue(ontology.Imports.Count(imp => string.Equals(imp.IRI, "ex:ont2")) == 1);
        }

        [TestMethod]
        public void ShouldReadPrefixesFromGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.FOAF.MAKER, new RDFResource("ex:Mark")));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont"));
            Assert.IsNull(ontology.VersionIRI);
            Assert.IsTrue(ontology.Prefixes.Count == 6);
            Assert.IsTrue(ontology.Prefixes.Count(pfx => string.Equals(pfx.Name, RDFVocabulary.OWL.PREFIX)) == 1);
            Assert.IsTrue(ontology.Prefixes.Count(pfx => string.Equals(pfx.Name, RDFVocabulary.RDFS.PREFIX)) == 1);
            Assert.IsTrue(ontology.Prefixes.Count(pfx => string.Equals(pfx.Name, RDFVocabulary.RDF.PREFIX)) == 1);
            Assert.IsTrue(ontology.Prefixes.Count(pfx => string.Equals(pfx.Name, RDFVocabulary.XSD.PREFIX)) == 1);
            Assert.IsTrue(ontology.Prefixes.Count(pfx => string.Equals(pfx.Name, RDFVocabulary.XML.PREFIX)) == 1);
            Assert.IsTrue(ontology.Prefixes.Count(pfx => string.Equals(pfx.Name, RDFVocabulary.FOAF.PREFIX)) == 1);
        }

        [TestMethod]
        public void ShouldReadDeclarationsFromGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(RDFVocabulary.FOAF.ORGANIZATION, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS));
            graph.AddTriple(new RDFTriple(RDFVocabulary.XSD.INTEGER, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE));
            graph.AddTriple(new RDFTriple(RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(RDFVocabulary.FOAF.NAME, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(RDFVocabulary.FOAF.MAKER, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:Alice"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont"));
            Assert.IsNull(ontology.VersionIRI);
            Assert.IsTrue(ontology.DeclarationAxioms.Count == 7);
            Assert.IsTrue(ontology.DeclarationAxioms.Count(dax => dax.Expression is OWLClass daxCls
                                                                    && daxCls.GetIRI().Equals(RDFVocabulary.FOAF.PERSON)) == 1);
            Assert.IsTrue(ontology.DeclarationAxioms.Count(dax => dax.Expression is OWLClass daxCls
                                                                    && daxCls.GetIRI().Equals(RDFVocabulary.FOAF.ORGANIZATION)) == 1);
            Assert.IsTrue(ontology.DeclarationAxioms.Count(dax => dax.Expression is OWLDatatype daxDtt
                                                                    && daxDtt.GetIRI().Equals(RDFVocabulary.XSD.INTEGER)) == 1);
            Assert.IsTrue(ontology.DeclarationAxioms.Count(dax => dax.Expression is OWLObjectProperty daxObp
                                                                    && daxObp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)) == 1);
            Assert.IsTrue(ontology.DeclarationAxioms.Count(dax => dax.Expression is OWLDataProperty daxDtp
                                                                    && daxDtp.GetIRI().Equals(RDFVocabulary.FOAF.NAME)) == 1);
            Assert.IsTrue(ontology.DeclarationAxioms.Count(dax => dax.Expression is OWLAnnotationProperty daxAnp
                                                                    && daxAnp.GetIRI().Equals(RDFVocabulary.FOAF.MAKER)) == 1);
            Assert.IsTrue(ontology.DeclarationAxioms.Count(dax => dax.Expression is OWLNamedIndividual daxIdv
                                                                    && daxIdv.GetIRI().Equals(new RDFResource("ex:Alice"))) == 1);
        }

        [TestMethod]
        public void ShouldReadOntologyAnnotationsFromGraph()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.OWL.VERSION_IRI, new RDFResource("ex:ont/v2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.OWL.BACKWARD_COMPATIBLE_WITH, new RDFResource("ex:ont1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.OWL.INCOMPATIBLE_WITH, new RDFResource("ex:ont0")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.OWL.PRIOR_VERSION, new RDFResource("ex:ont1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.OWL.VERSION_INFO, new RDFPlainLiteral("v2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.OWL.DEPRECATED, RDFTypedLiteral.True));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label", "en-US")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:ont2/seeAlso")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.RDFS.IS_DEFINED_BY, new RDFResource("ex:ont2")));
            graph.AddTriple(new RDFTriple(RDFVocabulary.DC.CREATOR, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont2"), RDFVocabulary.DC.CREATOR, new RDFResource("ex:Test")));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(string.Equals(ontology.IRI, "ex:ont2"));
            Assert.IsTrue(string.Equals(ontology.VersionIRI, "ex:ont/v2"));
            Assert.IsTrue(ontology.Annotations.Count == 10);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.BACKWARD_COMPATIBLE_WITH)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.INCOMPATIBLE_WITH)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.PRIOR_VERSION)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.VERSION_INFO)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.LABEL)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.SEE_ALSO)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.IS_DEFINED_BY)) == 1);
            Assert.IsTrue(ontology.Annotations.Count(ann => ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.CREATOR)) == 1);
        }

        [TestMethod]
        public void ShouldReadOntologyNestedAnnotationsFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.Annotations.Add(
                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("annotation")))
                {
                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("nested annotation")))
                    {
                        Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new RDFResource("ex:ann"))
                    }
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.Annotations.Count == 1);
            Assert.IsTrue(ontology2.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DESCRIPTION)
                           && ontology2.Annotations.Single().ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("annotation")));
            Assert.IsTrue(ontology2.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DESCRIPTION)
                           && ontology2.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("nested annotation")));
            Assert.IsTrue(ontology2.Annotations.Single().Annotation.Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DESCRIPTION)
                           && ontology2.Annotations.Single().Annotation.Annotation.ValueIRI.Equals("ex:ann"));
        }

        [TestMethod]
        public void ShouldReadAsymmetricObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLAsymmetricObjectProperty(
					new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
					{
						Annotations = [
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
							}
						]
					});
            ontology.ObjectPropertyAxioms.Add(
                new OWLAsymmetricObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
					{
						Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                            },
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                            }
                        ]
					});
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLAsymmetricObjectProperty asymObjProp
							&& asymObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
							&& objProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
							 && asymObjProp.Annotations.Count == 1
							 && asymObjProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
							 && string.Equals(asymObjProp.Annotations.Single().ValueIRI, "ex:title")
							  && asymObjProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && asymObjProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLAsymmetricObjectProperty asymObjProp1
                            && asymObjProp1.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && objInvOf.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
							&& asymObjProp1.Annotations.Count == 2
							 && asymObjProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(asymObjProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && asymObjProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && asymObjProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && asymObjProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(asymObjProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && asymObjProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && asymObjProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadSymmetricObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLSymmetricObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
					{
						Annotations = [
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
							}
						]
					});
            ontology.ObjectPropertyAxioms.Add(
                new OWLSymmetricObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
					{
						Annotations = [
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
							},
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
							}
						]
					});
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLSymmetricObjectProperty symObjProp
                            && symObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && objProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                             && symObjProp.Annotations.Count == 1
                             && symObjProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(symObjProp.Annotations.Single().ValueIRI, "ex:title")
                              && symObjProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && symObjProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLSymmetricObjectProperty symObjProp1
                            && symObjProp1.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && objInvOf.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && symObjProp1.Annotations.Count == 2
                             && symObjProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(symObjProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && symObjProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && symObjProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && symObjProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(symObjProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && symObjProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && symObjProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadIrreflexiveObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLIrreflexiveObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
					{
						Annotations = [
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
							}
						]
					});
            ontology.ObjectPropertyAxioms.Add(
                new OWLIrreflexiveObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
					{
						Annotations = [
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
							},
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
							}
						]
					});
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLIrreflexiveObjectProperty irrefObjProp
                            && irrefObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && objProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                             && irrefObjProp.Annotations.Count == 1
                             && irrefObjProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(irrefObjProp.Annotations.Single().ValueIRI, "ex:title")
                              && irrefObjProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && irrefObjProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLIrreflexiveObjectProperty irrefObjProp1
                            && irrefObjProp1.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && objInvOf.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && irrefObjProp1.Annotations.Count == 2
                             && irrefObjProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(irrefObjProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && irrefObjProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && irrefObjProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && irrefObjProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(irrefObjProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && irrefObjProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && irrefObjProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadReflexiveObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLReflexiveObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLReflexiveObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLReflexiveObjectProperty refObjProp
                            && refObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && objProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                             && refObjProp.Annotations.Count == 1
                             && refObjProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(refObjProp.Annotations.Single().ValueIRI, "ex:title")
                              && refObjProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && refObjProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLReflexiveObjectProperty refObjProp1
                            && refObjProp1.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && objInvOf.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && refObjProp1.Annotations.Count == 2
                             && refObjProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(refObjProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && refObjProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && refObjProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && refObjProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(refObjProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && refObjProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && refObjProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadTransitiveObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLTransitiveObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLTransitiveObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLTransitiveObjectProperty transObjProp
                            && transObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && objProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                             && transObjProp.Annotations.Count == 1
                             && transObjProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(transObjProp.Annotations.Single().ValueIRI, "ex:title")
                              && transObjProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && transObjProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLTransitiveObjectProperty transObjProp1
                            && transObjProp1.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && objInvOf.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && transObjProp1.Annotations.Count == 2
                             && transObjProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(transObjProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && transObjProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && transObjProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && transObjProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(transObjProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && transObjProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && transObjProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadInverseFunctionalObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseFunctionalObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseFunctionalObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLInverseFunctionalObjectProperty invfuncObjProp
                            && invfuncObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && objProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                             && invfuncObjProp.Annotations.Count == 1
                             && invfuncObjProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(invfuncObjProp.Annotations.Single().ValueIRI, "ex:title")
                              && invfuncObjProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && invfuncObjProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLInverseFunctionalObjectProperty invfuncObjProp1
                            && invfuncObjProp1.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && objInvOf.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && invfuncObjProp1.Annotations.Count == 2
                             && invfuncObjProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(invfuncObjProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && invfuncObjProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && invfuncObjProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && invfuncObjProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(invfuncObjProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && invfuncObjProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && invfuncObjProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadFunctionalObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLFunctionalObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                            }
                        ]
                    });
            ontology.ObjectPropertyAxioms.Add(
                new OWLFunctionalObjectProperty(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                            },
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                            }
                        ]
                    });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLFunctionalObjectProperty funcObjProp
                            && funcObjProp.ObjectPropertyExpression is OWLObjectProperty objProp
                            && objProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                             && funcObjProp.Annotations.Count == 1
                             && funcObjProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(funcObjProp.Annotations.Single().ValueIRI, "ex:title")
                              && funcObjProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && funcObjProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLFunctionalObjectProperty funcObjProp1
                            && funcObjProp1.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && objInvOf.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && funcObjProp1.Annotations.Count == 2
                             && funcObjProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(funcObjProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && funcObjProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && funcObjProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && funcObjProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(funcObjProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && funcObjProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && funcObjProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

		[TestMethod]
        public void ShouldReadInverseObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseObjectProperties(
                    new OWLObjectProperty(new RDFResource("ex:objPropA1")), 
                    new OWLObjectProperty(new RDFResource("ex:objPropB1")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseObjectProperties(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA2"))), 
                    new OWLObjectProperty(new RDFResource("ex:objPropB2")))
                         {
                             Annotations = [
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                                },
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                                }
                            ]
                         });
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseObjectProperties(
                    new OWLObjectProperty(new RDFResource("ex:objPropA3")),
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB3"))))
                {
                    Annotations = []
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLInverseObjectProperties(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA4"))),
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB4"))))
                {
                    Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                                   {
                                       Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                                   },
                                   new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                                   {
                                       Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                                   }
                        ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 4);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLInverseObjectProperties invObjProps0
                            && invObjProps0.LeftObjectPropertyExpression is OWLObjectProperty objPropA1
                            && objPropA1.GetIRI().Equals(new RDFResource("ex:objPropA1"))
							&& invObjProps0.RightObjectPropertyExpression is OWLObjectProperty objPropB1
                            && objPropB1.GetIRI().Equals(new RDFResource("ex:objPropB1"))
                             && invObjProps0.Annotations.Count == 1
                             && invObjProps0.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(invObjProps0.Annotations.Single().ValueIRI, "ex:title")
                              && invObjProps0.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && invObjProps0.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLInverseObjectProperties invObjProps1
                            && invObjProps1.LeftObjectPropertyExpression is OWLObjectProperty objPropA3
                            && objPropA3.GetIRI().Equals(new RDFResource("ex:objPropA3"))
                            && invObjProps1.RightObjectPropertyExpression is OWLObjectInverseOf objInvOfObjPropB3
                            && objInvOfObjPropB3.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB3"))
                             && invObjProps1.Annotations.Count == 0);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[2] is OWLInverseObjectProperties invObjProps2
                            && invObjProps2.LeftObjectPropertyExpression is OWLObjectInverseOf objInvOfObjPropA2
                            && objInvOfObjPropA2.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA2"))
							&& invObjProps2.RightObjectPropertyExpression is OWLObjectProperty objPropB2
                            && objPropB2.GetIRI().Equals(new RDFResource("ex:objPropB2"))
                            && invObjProps2.Annotations.Count == 2
                             && invObjProps2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(invObjProps2.Annotations[0].ValueIRI, "ex:comment1")
                              && invObjProps2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && invObjProps2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && invObjProps2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(invObjProps2.Annotations[1].ValueIRI, "ex:comment2")
                              && invObjProps2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && invObjProps2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[3] is OWLInverseObjectProperties invObjProps3
                           && invObjProps3.LeftObjectPropertyExpression is OWLObjectInverseOf objInvOfObjPropA4
                           && objInvOfObjPropA4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA4"))
                           && invObjProps3.RightObjectPropertyExpression is OWLObjectInverseOf objInvOfObjPropB4
                           && objInvOfObjPropB4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB4"))
                           && invObjProps3.Annotations.Count == 2
                            && invObjProps3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                            && string.Equals(invObjProps3.Annotations[0].ValueIRI, "ex:comment1")
                             && invObjProps3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && invObjProps3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                            && invObjProps3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                            && string.Equals(invObjProps3.Annotations[1].ValueIRI, "ex:comment2")
                             && invObjProps3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && invObjProps3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadEquivalentObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLEquivalentObjectProperties(
					[ new OWLObjectProperty(new RDFResource("ex:objPropA1")), new OWLObjectProperty(new RDFResource("ex:objPropB1")) ])
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                            }
                        ]
                    });
			ontology.ObjectPropertyAxioms.Add(
                new OWLEquivalentObjectProperties(
					[ new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA2"))), new OWLObjectProperty(new RDFResource("ex:objPropB2")) ])
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                            },
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                            }
                        ]
                    });
			ontology.ObjectPropertyAxioms.Add(
                new OWLEquivalentObjectProperties(
					[ new OWLObjectProperty(new RDFResource("ex:objPropA3")), new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB3"))) ])
                    {
                        Annotations = []
                    });
			ontology.ObjectPropertyAxioms.Add(
                new OWLEquivalentObjectProperties(
					[ new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA4"))), new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB4"))) ])
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                            },
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                            }
                        ]
                    });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 4);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLEquivalentObjectProperties equivObjProps
                            && equivObjProps.ObjectPropertyExpressions[0] is OWLObjectProperty objPropA1
                            && objPropA1.GetIRI().Equals(new RDFResource("ex:objPropA1"))
							&& equivObjProps.ObjectPropertyExpressions[1] is OWLObjectProperty objPropB1
                            && objPropB1.GetIRI().Equals(new RDFResource("ex:objPropB1"))
                             && equivObjProps.Annotations.Count == 1
                             && equivObjProps.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(equivObjProps.Annotations.Single().ValueIRI, "ex:title")
                              && equivObjProps.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && equivObjProps.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLEquivalentObjectProperties equivObjProps1
                            && equivObjProps1.ObjectPropertyExpressions[0] is OWLObjectInverseOf objInvOfA2
                            && objInvOfA2.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA2"))
							&& equivObjProps1.ObjectPropertyExpressions[1] is OWLObjectProperty objPropB2
                            && objPropB2.GetIRI().Equals(new RDFResource("ex:objPropB2"))
                            && equivObjProps1.Annotations.Count == 2
                             && equivObjProps1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(equivObjProps1.Annotations[0].ValueIRI, "ex:comment1")
                              && equivObjProps1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && equivObjProps1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && equivObjProps1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(equivObjProps1.Annotations[1].ValueIRI, "ex:comment2")
                              && equivObjProps1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && equivObjProps1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[2] is OWLEquivalentObjectProperties equivObjProps2
                            && equivObjProps2.ObjectPropertyExpressions[0] is OWLObjectProperty objPropA3
                            && objPropA3.GetIRI().Equals(new RDFResource("ex:objPropA3"))
							&& equivObjProps2.ObjectPropertyExpressions[1] is OWLObjectInverseOf objInvOfB3
                            && objInvOfB3.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB3"))
                            && equivObjProps2.Annotations.Count == 0);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[3] is OWLEquivalentObjectProperties equivObjProps3
                            && equivObjProps3.ObjectPropertyExpressions[0] is OWLObjectInverseOf objInvOfA4
                            && objInvOfA4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA4"))
							&& equivObjProps3.ObjectPropertyExpressions[1] is OWLObjectInverseOf objInvOfB4
                            && objInvOfB4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB4"))
                            && equivObjProps3.Annotations.Count == 2
                             && equivObjProps3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(equivObjProps3.Annotations[0].ValueIRI, "ex:comment1")
                              && equivObjProps3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && equivObjProps3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && equivObjProps3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(equivObjProps3.Annotations[1].ValueIRI, "ex:comment2")
                              && equivObjProps3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && equivObjProps3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

		[TestMethod]
        public void ShouldReadDisjointObjectPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(new RDFResource("ex:objPropA1")), 
                    new OWLObjectProperty(new RDFResource("ex:objPropB1"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLDisjointObjectProperties([
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA2"))), 
                    new OWLObjectProperty(new RDFResource("ex:objPropB2"))])
                         {
                             Annotations = [
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                                },
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                                }
                            ]
                         });
            ontology.ObjectPropertyAxioms.Add(
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(new RDFResource("ex:objPropA3")),
					new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB3"))),])
                         {
                             Annotations = [
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                                },
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                                }
                            ]
                         });
            ontology.ObjectPropertyAxioms.Add(
                new OWLDisjointObjectProperties([
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA4"))),
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB4")))])
                {
                    Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                                   {
                                       Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                                   },
                                   new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                                   {
                                       Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                                   }
                        ]
                });
			ontology.ObjectPropertyAxioms.Add(
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(new RDFResource("ex:objPropA5")), 
                    new OWLObjectProperty(new RDFResource("ex:objPropB5")),
					new OWLObjectProperty(new RDFResource("ex:objPropC5")),
					new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropD5")))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
						}
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 5);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLDisjointObjectProperties disjObjProps
                            && disjObjProps.ObjectPropertyExpressions[0] is OWLObjectProperty objPropA1
                            && objPropA1.GetIRI().Equals(new RDFResource("ex:objPropA1"))
							&& disjObjProps.ObjectPropertyExpressions[1] is OWLObjectProperty objPropB1
                            && objPropB1.GetIRI().Equals(new RDFResource("ex:objPropB1"))
                             && disjObjProps.Annotations.Count == 1
                             && disjObjProps.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(disjObjProps.Annotations.Single().ValueIRI, "ex:title")
                              && disjObjProps.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && disjObjProps.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLDisjointObjectProperties disjObjProps1
                            && disjObjProps1.ObjectPropertyExpressions[0] is OWLObjectInverseOf objInvOfA2
                            && objInvOfA2.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA2"))
							&& disjObjProps1.ObjectPropertyExpressions[1] is OWLObjectProperty objPropB2
                            && objPropB2.GetIRI().Equals(new RDFResource("ex:objPropB2"))
                            && disjObjProps1.Annotations.Count == 2
                             && disjObjProps1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjObjProps1.Annotations[0].ValueIRI, "ex:comment1")
                              && disjObjProps1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjObjProps1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && disjObjProps1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjObjProps1.Annotations[1].ValueIRI, "ex:comment2")
                              && disjObjProps1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjObjProps1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[2] is OWLDisjointObjectProperties disjObjProps2
                            && disjObjProps2.ObjectPropertyExpressions[0] is OWLObjectProperty objPropA3
                            && objPropA3.GetIRI().Equals(new RDFResource("ex:objPropA3"))
							&& disjObjProps2.ObjectPropertyExpressions[1] is OWLObjectInverseOf objInvOfB3
                            && objInvOfB3.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB3"))
                            && disjObjProps2.Annotations.Count == 2
                             && disjObjProps2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjObjProps2.Annotations[0].ValueIRI, "ex:comment1")
                              && disjObjProps2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjObjProps2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && disjObjProps2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjObjProps2.Annotations[1].ValueIRI, "ex:comment2")
                              && disjObjProps2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjObjProps2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[3] is OWLDisjointObjectProperties disjObjProps3
                            && disjObjProps3.ObjectPropertyExpressions[0] is OWLObjectInverseOf objInvOfA4
                            && objInvOfA4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA4"))
							&& disjObjProps3.ObjectPropertyExpressions[1] is OWLObjectInverseOf objInvOfB4
                            && objInvOfB4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB4"))
                            && disjObjProps3.Annotations.Count == 2
                             && disjObjProps3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjObjProps3.Annotations[0].ValueIRI, "ex:comment1")
                              && disjObjProps3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjObjProps3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && disjObjProps3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjObjProps3.Annotations[1].ValueIRI, "ex:comment2")
                              && disjObjProps3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjObjProps3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[4] is OWLDisjointObjectProperties disjObjProps4
                            && disjObjProps4.ObjectPropertyExpressions[0] is OWLObjectProperty objPropA5
                            && objPropA5.GetIRI().Equals(new RDFResource("ex:objPropA5"))
							&& disjObjProps4.ObjectPropertyExpressions[1] is OWLObjectProperty objPropB5
                            && objPropB5.GetIRI().Equals(new RDFResource("ex:objPropB5"))
							&& disjObjProps4.ObjectPropertyExpressions[2] is OWLObjectProperty objPropC5
                            && objPropC5.GetIRI().Equals(new RDFResource("ex:objPropC5"))
							&& disjObjProps4.ObjectPropertyExpressions[3] is OWLObjectInverseOf objInvOfD5
                            && objInvOfD5.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropD5"))
                             && disjObjProps4.Annotations.Count == 1
                             && disjObjProps4.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(disjObjProps4.Annotations.Single().ValueIRI, "ex:title")
                              && disjObjProps4.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && disjObjProps4.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
        }

		[TestMethod]
        public void ShouldReadSubObjectPropertyOfFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("ex:objPropA1")), 
                    new OWLObjectProperty(new RDFResource("ex:objPropB1")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLSubObjectPropertyOf(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA2"))), 
                    new OWLObjectProperty(new RDFResource("ex:objPropB2")))
                         {
                             Annotations = [
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                                },
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                                }
                            ]
                         });
            ontology.ObjectPropertyAxioms.Add(
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("ex:objPropA3")),
					new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB3"))))
                         {
                             Annotations = [
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                                },
                                new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                                {
                                    Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                                }
                            ]
                         });
            ontology.ObjectPropertyAxioms.Add(
                new OWLSubObjectPropertyOf(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropA4"))),
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB4"))))
                {
                    Annotations = [
						new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
								{
									Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
								},
								new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
								{
									Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
								}
					]
                });
			ontology.ObjectPropertyAxioms.Add(
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("ex:objPropA5")), 
                        new OWLObjectProperty(new RDFResource("ex:objPropB5")),
                        new OWLObjectProperty(new RDFResource("ex:objPropC5"))]),
					new OWLObjectProperty(new RDFResource("ex:objPropD5")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
						}
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("ex:objPropA6")), 
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB6"))),
                        new OWLObjectProperty(new RDFResource("ex:objPropC6"))]),
					new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropD6"))))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT--ltr")))
						}
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 6);
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLSubObjectPropertyOf subObjProps
                            && subObjProps.SubObjectPropertyChain.ObjectPropertyExpressions[0] is OWLObjectProperty objPropA5
                            && objPropA5.GetIRI().Equals(new RDFResource("ex:objPropA5"))
							&& subObjProps.SubObjectPropertyChain.ObjectPropertyExpressions[1] is OWLObjectProperty objPropB5
                            && objPropB5.GetIRI().Equals(new RDFResource("ex:objPropB5"))
							&& subObjProps.SubObjectPropertyChain.ObjectPropertyExpressions[2] is OWLObjectProperty objPropC5
                            && objPropC5.GetIRI().Equals(new RDFResource("ex:objPropC5"))
							&& subObjProps.SuperObjectPropertyExpression is OWLObjectProperty objPropD5
                            && objPropD5.GetIRI().Equals(new RDFResource("ex:objPropD5"))
                             && subObjProps.Annotations.Count == 1
                             && subObjProps.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(subObjProps.Annotations.Single().ValueIRI, "ex:title")
                              && subObjProps.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && subObjProps.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLSubObjectPropertyOf subObjProps1
                            && subObjProps1.SubObjectPropertyChain.ObjectPropertyExpressions[0] is OWLObjectProperty objPropA6
                            && objPropA6.GetIRI().Equals(new RDFResource("ex:objPropA6"))
							&& subObjProps1.SubObjectPropertyChain.ObjectPropertyExpressions[1] is OWLObjectInverseOf objInvOfB6
                            && objInvOfB6.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB6"))
							&& subObjProps1.SubObjectPropertyChain.ObjectPropertyExpressions[2] is OWLObjectProperty objPropC6
                            && objPropC6.GetIRI().Equals(new RDFResource("ex:objPropC6"))
							&& subObjProps1.SuperObjectPropertyExpression is OWLObjectInverseOf objInvOfD6
                            && objInvOfD6.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropD6"))
                             && subObjProps1.Annotations.Count == 1
                             && subObjProps1.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(subObjProps1.Annotations.Single().ValueIRI, "ex:title")
                              && subObjProps1.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && subObjProps1.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT--ltr")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[2] is OWLSubObjectPropertyOf subObjProps2
                            && subObjProps2.SubObjectPropertyExpression is OWLObjectProperty objPropA1
                            && objPropA1.GetIRI().Equals(new RDFResource("ex:objPropA1"))
							&& subObjProps2.SuperObjectPropertyExpression is OWLObjectProperty objPropB1
                            && objPropB1.GetIRI().Equals(new RDFResource("ex:objPropB1"))
                             && subObjProps2.Annotations.Count == 1
                             && subObjProps2.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(subObjProps2.Annotations.Single().ValueIRI, "ex:title")
                              && subObjProps2.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && subObjProps2.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[3] is OWLSubObjectPropertyOf subObjProps3
                            && subObjProps3.SubObjectPropertyExpression is OWLObjectInverseOf objInvOfA2
                            && objInvOfA2.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA2"))
							&& subObjProps3.SuperObjectPropertyExpression is OWLObjectProperty objPropB2
                            && objPropB2.GetIRI().Equals(new RDFResource("ex:objPropB2"))
                            && subObjProps3.Annotations.Count == 2
                             && subObjProps3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subObjProps3.Annotations[0].ValueIRI, "ex:comment1")
                              && subObjProps3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subObjProps3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && subObjProps3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subObjProps3.Annotations[1].ValueIRI, "ex:comment2")
                              && subObjProps3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subObjProps3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[4] is OWLSubObjectPropertyOf subObjProps4
                            && subObjProps4.SubObjectPropertyExpression is OWLObjectProperty objPropA3
                            && objPropA3.GetIRI().Equals(new RDFResource("ex:objPropA3"))
							&& subObjProps4.SuperObjectPropertyExpression is OWLObjectInverseOf objInvOfB3
                            && objInvOfB3.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB3"))
                            && subObjProps4.Annotations.Count == 2
                             && subObjProps4.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subObjProps4.Annotations[0].ValueIRI, "ex:comment1")
                              && subObjProps4.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subObjProps4.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && subObjProps4.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subObjProps4.Annotations[1].ValueIRI, "ex:comment2")
                              && subObjProps4.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subObjProps4.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[5] is OWLSubObjectPropertyOf subObjProps5
                            && subObjProps5.SubObjectPropertyExpression is OWLObjectInverseOf objInvOfA4
                            && objInvOfA4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropA4"))
							&& subObjProps5.SuperObjectPropertyExpression is OWLObjectInverseOf objInvOfB4
                            && objInvOfB4.ObjectProperty.GetIRI().Equals(new RDFResource("ex:objPropB4"))
                            && subObjProps5.Annotations.Count == 2
                             && subObjProps5.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subObjProps5.Annotations[0].ValueIRI, "ex:comment1")
                              && subObjProps5.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subObjProps5.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && subObjProps5.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subObjProps5.Annotations[1].ValueIRI, "ex:comment2")
                              && subObjProps5.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subObjProps5.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadObjectPropertyDomainGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyDomain(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLClass(RDFVocabulary.FOAF.PERSON))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyDomain(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.TITLE),
                        2, new OWLObjectSomeValuesFrom(new OWLObjectProperty(RDFVocabulary.FOAF.PROJECT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyDomain(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectOneOf([
                        new OWLNamedIndividual(new RDFResource("ex:IdvA")),
                        new OWLAnonymousIndividual("AnonIdv1")]))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyDomain(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectIntersectionOf([
                        new OWLClass(RDFVocabulary.FOAF.MEMBERSHIP_CLASS),
                        new OWLObjectComplementOf(new OWLClass(RDFVocabulary.FOAF.MBOX))]))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
			ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyDomain(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.TITLE),
                        2, new OWLObjectSomeValuesFrom(new OWLObjectProperty(RDFVocabulary.FOAF.PROJECT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 5);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLObjectPropertyDomain objPropDom
                            && objPropDom.ObjectPropertyExpression is OWLObjectProperty foafKnows
                            && foafKnows.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropDom.ClassExpression is OWLClass foafPerson
                            && foafPerson.GetIRI().Equals(RDFVocabulary.FOAF.PERSON)
                             && objPropDom.Annotations.Count == 1
                             && objPropDom.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(objPropDom.Annotations.Single().ValueIRI, "ex:title")
                              && objPropDom.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && objPropDom.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLObjectPropertyDomain objPropDom1
                            && objPropDom1.ObjectPropertyExpression is OWLObjectInverseOf invOfFoafKnows
                            && invOfFoafKnows.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropDom1.ClassExpression is OWLObjectMaxCardinality max2Title
                            && max2Title.ObjectPropertyExpression is OWLObjectProperty max2TitleObjProp
                            && max2TitleObjProp.GetIRI().Equals(RDFVocabulary.FOAF.TITLE)
                            && max2Title.Cardinality == "2"
                            && max2Title.ClassExpression is OWLObjectSomeValuesFrom max2TitleClsExp
                            && max2TitleClsExp.ObjectPropertyExpression is OWLObjectProperty foafProject
                            && foafProject.GetIRI().Equals(RDFVocabulary.FOAF.PROJECT)
                            && max2TitleClsExp.ClassExpression is OWLClass foafOrganization
                            && foafOrganization.GetIRI().Equals(RDFVocabulary.FOAF.ORGANIZATION)
                             && objPropDom1.Annotations.Count == 2
                             && objPropDom1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom1.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropDom1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropDom1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom1.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropDom1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[2] is OWLObjectPropertyDomain objPropDom2
                            && objPropDom2.ObjectPropertyExpression is OWLObjectProperty foafKnows2
                            && foafKnows2.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropDom2.ClassExpression is OWLObjectOneOf objOneOf
                            && objOneOf.IndividualExpressions.Count == 2
                            && objOneOf.IndividualExpressions[0] is OWLNamedIndividual idvA
                            && idvA.GetIRI().Equals(new RDFResource("ex:IdvA"))
                            && objOneOf.IndividualExpressions[1] is OWLAnonymousIndividual anonIdv1
                            && anonIdv1.GetIRI().Equals(new RDFResource("bnode:AnonIdv1"))
                             && objPropDom2.Annotations.Count == 2
                             && objPropDom2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom2.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropDom2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropDom2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom2.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropDom2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[3] is OWLObjectPropertyDomain objPropDom3
                            && objPropDom3.ObjectPropertyExpression is OWLObjectProperty foafKnows3
                            && foafKnows3.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropDom3.ClassExpression is OWLObjectIntersectionOf objIntOf
                            && objIntOf.ClassExpressions.Count == 2
                            && objIntOf.ClassExpressions[0] is OWLClass foafMembershipClass
                            && foafMembershipClass.GetIRI().Equals(RDFVocabulary.FOAF.MEMBERSHIP_CLASS)
                            && objIntOf.ClassExpressions[1] is OWLObjectComplementOf objComplOf
                            && objComplOf.ClassExpression is OWLClass foafMBox
                            && foafMBox.GetIRI().Equals(RDFVocabulary.FOAF.MBOX)
                             && objPropDom3.Annotations.Count == 2
                             && objPropDom3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom3.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropDom3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropDom3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom3.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropDom3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[4] is OWLObjectPropertyDomain objPropDom4
                            && objPropDom4.ObjectPropertyExpression is OWLObjectInverseOf invOfFoafKnows2
                            && invOfFoafKnows2.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropDom4.ClassExpression is OWLObjectExactCardinality exact2Title
                            && exact2Title.ObjectPropertyExpression is OWLObjectProperty exact2TitleObjProp
                            && exact2TitleObjProp.GetIRI().Equals(RDFVocabulary.FOAF.TITLE)
                            && exact2Title.Cardinality == "2"
                            && exact2Title.ClassExpression is OWLObjectSomeValuesFrom exact2TitleClsExp
                            && exact2TitleClsExp.ObjectPropertyExpression is OWLObjectProperty foafProject2
                            && foafProject2.GetIRI().Equals(RDFVocabulary.FOAF.PROJECT)
                            && exact2TitleClsExp.ClassExpression is OWLClass foafOrganization2
                            && foafOrganization2.GetIRI().Equals(RDFVocabulary.FOAF.ORGANIZATION)
                             && objPropDom4.Annotations.Count == 2
                             && objPropDom4.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom4.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropDom4.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom4.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropDom4.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropDom4.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropDom4.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropDom4.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

		[TestMethod]
        public void ShouldReadObjectPropertyRangeGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyRange(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLClass(RDFVocabulary.FOAF.PERSON))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyRange(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.TITLE),
                        2, new OWLObjectAllValuesFrom(new OWLObjectProperty(RDFVocabulary.FOAF.PROJECT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyRange(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectOneOf([
                        new OWLNamedIndividual(new RDFResource("ex:IdvA")),
                        new OWLAnonymousIndividual("AnonIdv1")]))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyRange(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectUnionOf([
                        new OWLClass(RDFVocabulary.FOAF.MEMBERSHIP_CLASS),
                        new OWLObjectComplementOf(new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.MBOX), new OWLNamedIndividual(new RDFResource("ex:IdvA"))))]))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
			ontology.ObjectPropertyAxioms.Add(
                new OWLObjectPropertyRange(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectHasSelf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms.Count == 5);
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[0] is OWLObjectPropertyRange objPropRng
                            && objPropRng.ObjectPropertyExpression is OWLObjectProperty foafKnows
                            && foafKnows.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropRng.ClassExpression is OWLClass foafPerson
                            && foafPerson.GetIRI().Equals(RDFVocabulary.FOAF.PERSON)
                             && objPropRng.Annotations.Count == 1
                             && objPropRng.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(objPropRng.Annotations.Single().ValueIRI, "ex:title")
                              && objPropRng.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && objPropRng.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[1] is OWLObjectPropertyRange objPropRng1
                            && objPropRng1.ObjectPropertyExpression is OWLObjectInverseOf invOfFoafKnows
                            && invOfFoafKnows.ObjectProperty.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropRng1.ClassExpression is OWLObjectMinCardinality min2Title
                            && min2Title.ObjectPropertyExpression is OWLObjectProperty min2TitleObjProp
                            && min2TitleObjProp.GetIRI().Equals(RDFVocabulary.FOAF.TITLE)
                            && min2Title.Cardinality == "2"
                            && min2Title.ClassExpression is OWLObjectAllValuesFrom min2TitleClsExp
                            && min2TitleClsExp.ObjectPropertyExpression is OWLObjectProperty foafProject
                            && foafProject.GetIRI().Equals(RDFVocabulary.FOAF.PROJECT)
                            && min2TitleClsExp.ClassExpression is OWLClass foafOrganization
                            && foafOrganization.GetIRI().Equals(RDFVocabulary.FOAF.ORGANIZATION)
                             && objPropRng1.Annotations.Count == 2
                             && objPropRng1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng1.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropRng1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropRng1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng1.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropRng1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[2] is OWLObjectPropertyRange objPropRng2
                            && objPropRng2.ObjectPropertyExpression is OWLObjectProperty foafKnows2
                            && foafKnows2.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropRng2.ClassExpression is OWLObjectOneOf objOneOf
                            && objOneOf.IndividualExpressions.Count == 2
                            && objOneOf.IndividualExpressions[0] is OWLNamedIndividual idvA
                            && idvA.GetIRI().Equals(new RDFResource("ex:IdvA"))
                            && objOneOf.IndividualExpressions[1] is OWLAnonymousIndividual anonIdv1
                            && anonIdv1.GetIRI().Equals(new RDFResource("bnode:AnonIdv1"))
                             && objPropRng2.Annotations.Count == 2
                             && objPropRng2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng2.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropRng2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropRng2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng2.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropRng2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.ObjectPropertyAxioms[3] is OWLObjectPropertyRange objPropRng3
                            && objPropRng3.ObjectPropertyExpression is OWLObjectProperty foafKnows3
                            && foafKnows3.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropRng3.ClassExpression is OWLObjectUnionOf objUnOf
                            && objUnOf.ClassExpressions.Count == 2
                            && objUnOf.ClassExpressions[0] is OWLClass foafMembershipClass
                            && foafMembershipClass.GetIRI().Equals(RDFVocabulary.FOAF.MEMBERSHIP_CLASS)
                            && objUnOf.ClassExpressions[1] is OWLObjectComplementOf objComplOf
                            && objComplOf.ClassExpression is OWLObjectHasValue objHV
                            && objHV.ObjectPropertyExpression is OWLObjectProperty objHVObjProp
							&& objHVObjProp.GetIRI().Equals(RDFVocabulary.FOAF.MBOX)
							&& objHV.IndividualExpression is OWLNamedIndividual objHVIdv
							&& objHVIdv.GetIRI().Equals(new RDFResource("ex:IdvA"))
                             && objPropRng3.Annotations.Count == 2
                             && objPropRng3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng3.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropRng3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropRng3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng3.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropRng3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.ObjectPropertyAxioms[4] is OWLObjectPropertyRange objPropRng4
                            && objPropRng4.ObjectPropertyExpression is OWLObjectProperty foafKnows4
                            && foafKnows4.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && objPropRng4.ClassExpression is OWLObjectHasSelf objHS
                            && objHS.ObjectPropertyExpression is OWLObjectProperty objHSProp
                            && objHSProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                             && objPropRng4.Annotations.Count == 2
                             && objPropRng4.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng4.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropRng4.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng4.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropRng4.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropRng4.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropRng4.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropRng4.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadFunctionalDataPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.DataPropertyAxioms.Add(
                new OWLFunctionalDataProperty(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE))
                {
                    Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                            }
                        ]
                });
            ontology.DataPropertyAxioms.Add(
                new OWLFunctionalDataProperty(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME))
                {
                    Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                            },
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                            }
                        ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.DataPropertyAxioms[0] is OWLFunctionalDataProperty funcDtProp
                            && funcDtProp.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                             && funcDtProp.Annotations.Count == 1
                             && funcDtProp.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(funcDtProp.Annotations.Single().ValueIRI, "ex:title")
                              && funcDtProp.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && funcDtProp.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.DataPropertyAxioms[1] is OWLFunctionalDataProperty funcDtProp1
                            && funcDtProp1.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.NAME)
                            && funcDtProp1.Annotations.Count == 2
                             && funcDtProp1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(funcDtProp1.Annotations[0].ValueIRI, "ex:comment1")
                              && funcDtProp1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && funcDtProp1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && funcDtProp1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(funcDtProp1.Annotations[1].ValueIRI, "ex:comment2")
                              && funcDtProp1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && funcDtProp1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadEquivalentDataPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.DataPropertyAxioms.Add(
                new OWLEquivalentDataProperties(
                    [new OWLDataProperty(new RDFResource("ex:dtPropA1")), new OWLDataProperty(new RDFResource("ex:dtPropB1"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.DataPropertyAxioms.Add(
                new OWLEquivalentDataProperties(
                    [new OWLDataProperty(new RDFResource("ex:dtPropA2")), new OWLDataProperty(new RDFResource("ex:dtPropB2"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.DataPropertyAxioms[0] is OWLEquivalentDataProperties equivDtProps
                            && equivDtProps.DataProperties[0] is OWLDataProperty dtPropA1
                            && dtPropA1.GetIRI().Equals(new RDFResource("ex:dtPropA1"))
                            && equivDtProps.DataProperties[1] is OWLDataProperty dtPropB1
                            && dtPropB1.GetIRI().Equals(new RDFResource("ex:dtPropB1"))
                             && equivDtProps.Annotations.Count == 1
                             && equivDtProps.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(equivDtProps.Annotations.Single().ValueIRI, "ex:title")
                              && equivDtProps.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && equivDtProps.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.DataPropertyAxioms[1] is OWLEquivalentDataProperties equivDtProps1
                            && equivDtProps1.DataProperties[0] is OWLDataProperty dtPropA2
                            && dtPropA2.GetIRI().Equals(new RDFResource("ex:dtPropA2"))
                            && equivDtProps1.DataProperties[1] is OWLDataProperty dtPropB2
                            && dtPropB2.GetIRI().Equals(new RDFResource("ex:dtPropB2"))
                            && equivDtProps1.Annotations.Count == 2
                             && equivDtProps1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(equivDtProps1.Annotations[0].ValueIRI, "ex:comment1")
                              && equivDtProps1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && equivDtProps1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && equivDtProps1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(equivDtProps1.Annotations[1].ValueIRI, "ex:comment2")
                              && equivDtProps1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && equivDtProps1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadDisjointDataPropertyFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.DataPropertyAxioms.Add(
                new OWLDisjointDataProperties([
                    new OWLDataProperty(new RDFResource("ex:dtPropA1")),
                    new OWLDataProperty(new RDFResource("ex:dtPropB1"))])
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                            }
                        ]
                    });
            ontology.DataPropertyAxioms.Add(
                new OWLDisjointDataProperties([
                    new OWLDataProperty(new RDFResource("ex:dtPropA2")),
                    new OWLDataProperty(new RDFResource("ex:dtPropB2")),
                    new OWLDataProperty(new RDFResource("ex:dtPropC2"))])
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                            },
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                            }
                        ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.DataPropertyAxioms[0] is OWLDisjointDataProperties disjDtProps
                            && disjDtProps.DataProperties[0].GetIRI().Equals(new RDFResource("ex:dtPropA1"))
                            && disjDtProps.DataProperties[1].GetIRI().Equals(new RDFResource("ex:dtPropB1"))
                             && disjDtProps.Annotations.Count == 1
                             && disjDtProps.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(disjDtProps.Annotations.Single().ValueIRI, "ex:title")
                              && disjDtProps.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && disjDtProps.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.DataPropertyAxioms[1] is OWLDisjointDataProperties disjDtProps1
                            && disjDtProps1.DataProperties[0].GetIRI().Equals(new RDFResource("ex:dtPropA2"))
                            && disjDtProps1.DataProperties[1].GetIRI().Equals(new RDFResource("ex:dtPropB2"))
                            && disjDtProps1.DataProperties[2].GetIRI().Equals(new RDFResource("ex:dtPropC2"))
                            && disjDtProps1.Annotations.Count == 2
                             && disjDtProps1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjDtProps1.Annotations[0].ValueIRI, "ex:comment1")
                              && disjDtProps1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjDtProps1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && disjDtProps1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(disjDtProps1.Annotations[1].ValueIRI, "ex:comment2")
                              && disjDtProps1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && disjDtProps1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadSubDataPropertyOfFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.DataPropertyAxioms.Add(
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("ex:dtPropA1")),
                    new OWLDataProperty(new RDFResource("ex:dtPropB1")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.DataPropertyAxioms.Add(
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("ex:dtPropA2")),
                    new OWLDataProperty(new RDFResource("ex:dtPropB2")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 2);
            Assert.IsTrue(ontology2.DataPropertyAxioms[0] is OWLSubDataPropertyOf subdtProps
                            && subdtProps.SubDataProperty.GetIRI().Equals(new RDFResource("ex:dtPropA1"))
                            && subdtProps.SuperDataProperty.GetIRI().Equals(new RDFResource("ex:dtPropB1"))
                             && subdtProps.Annotations.Count == 1
                             && subdtProps.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(subdtProps.Annotations.Single().ValueIRI, "ex:title")
                              && subdtProps.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && subdtProps.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.DataPropertyAxioms[1] is OWLSubDataPropertyOf subdtProps1
                            && subdtProps1.SubDataProperty.GetIRI().Equals(new RDFResource("ex:dtPropA2"))
                            && subdtProps1.SuperDataProperty.GetIRI().Equals(new RDFResource("ex:dtPropB2"))
                             && subdtProps1.Annotations.Count == 2
                             && subdtProps1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subdtProps1.Annotations[0].ValueIRI, "ex:comment1")
                              && subdtProps1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subdtProps1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && subdtProps1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(subdtProps1.Annotations[1].ValueIRI, "ex:comment2")
                              && subdtProps1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && subdtProps1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadSubClassOfFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ClassAxioms.Add(
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.AGENT))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ClassAxioms.Count == 1);
            Assert.IsTrue(ontology2.ClassAxioms[0] is OWLSubClassOf subClsOf
                            && subClsOf.SubClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.PERSON)
                            && subClsOf.SuperClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.AGENT)
                             && subClsOf.Annotations.Count == 1
                             && subClsOf.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(subClsOf.Annotations.Single().ValueIRI, "ex:title")
                              && subClsOf.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && subClsOf.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public void ShouldReadEquivalentClassesGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ClassAxioms.Add(
                new OWLEquivalentClasses([
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.AGENT)])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ClassAxioms.Count == 1);
            Assert.IsTrue(ontology2.ClassAxioms[0] is OWLEquivalentClasses equivCls
                            && equivCls.ClassExpressions[0].GetIRI().Equals(RDFVocabulary.FOAF.PERSON)
                            && equivCls.ClassExpressions[1].GetIRI().Equals(RDFVocabulary.FOAF.AGENT)
                             && equivCls.Annotations.Count == 1
                             && equivCls.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(equivCls.Annotations.Single().ValueIRI, "ex:title")
                              && equivCls.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && equivCls.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public void ShouldReadDisjointClassesFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ClassAxioms.Add(
                new OWLDisjointClasses([
                    new OWLClass(new RDFResource("ex:clsA1")),
                    new OWLClass(new RDFResource("ex:clsB1"))])
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                            }
                        ]
                    });
            ontology.ClassAxioms.Add(
                new OWLDisjointClasses([
                    new OWLClass(new RDFResource("ex:clsA5")),
                    new OWLClass(new RDFResource("ex:clsB5")),
                    new OWLClass(new RDFResource("ex:clsC5"))])
                    {
                        Annotations = [
                            new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                            {
                                Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                            }
                        ]
                    });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ClassAxioms.Count == 2);
            Assert.IsTrue(ontology2.ClassAxioms[0] is OWLDisjointClasses disjCls
                            && disjCls.ClassExpressions[0] is OWLClass clsA1
                            && clsA1.GetIRI().Equals(new RDFResource("ex:clsA1"))
                            && disjCls.ClassExpressions[1] is OWLClass clsB1
                            && clsB1.GetIRI().Equals(new RDFResource("ex:clsB1"))
                             && disjCls.Annotations.Count == 1
                             && disjCls.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(disjCls.Annotations.Single().ValueIRI, "ex:title")
                              && disjCls.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && disjCls.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.ClassAxioms[1] is OWLDisjointClasses disjCls1
                            && disjCls1.ClassExpressions[0] is OWLClass clsA5
                            && clsA5.GetIRI().Equals(new RDFResource("ex:clsA5"))
                            && disjCls1.ClassExpressions[1] is OWLClass clsB5
                            && clsB5.GetIRI().Equals(new RDFResource("ex:clsB5"))
                            && disjCls1.ClassExpressions[2] is OWLClass clsC5
                            && clsC5.GetIRI().Equals(new RDFResource("ex:clsC5"))
                             && disjCls1.Annotations.Count == 1
                             && disjCls1.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(disjCls1.Annotations.Single().ValueIRI, "ex:title")
                              && disjCls1.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && disjCls1.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
        }

        [TestMethod]
        public void ShouldReadDisjointUnionFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.ClassAxioms.Add(
                new OWLDisjointUnion(
                    new OWLClass(new RDFResource("ex:clsA")),
                    [ new OWLClass(new RDFResource("ex:clsB")), new OWLClass(new RDFResource("ex:clsC"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.ClassAxioms.Count == 1);
            Assert.IsTrue(ontology2.ClassAxioms[0] is OWLDisjointUnion disjUnion
                            && disjUnion.ClassIRI.GetIRI().Equals(new RDFResource("ex:clsA"))
                            && disjUnion.ClassExpressions[0] is OWLClass exClsB 
                            && exClsB.GetIRI().Equals(new RDFResource("ex:clsB"))
                            && disjUnion.ClassExpressions[1] is OWLClass exClsC 
                            && exClsC.GetIRI().Equals(new RDFResource("ex:clsC"))
                             && disjUnion.Annotations.Count == 1
                             && disjUnion.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(disjUnion.Annotations.Single().ValueIRI, "ex:title")
                              && disjUnion.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && disjUnion.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
        }

        [TestMethod]
        public void ShouldReadSameIndividualFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AssertionAxioms.Add(
                new OWLSameIndividual([
                    new OWLNamedIndividual(new RDFResource("ex:IDVA")),
                    new OWLNamedIndividual(new RDFResource("ex:IDVB"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLSameIndividual([
                    new OWLNamedIndividual(new RDFResource("ex:IDVC")),
                    new OWLAnonymousIndividual("AnonIDV1")])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLSameIndividual([
                    new OWLAnonymousIndividual("AnonIDV2"),
                    new OWLNamedIndividual(new RDFResource("ex:IDVD"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLSameIndividual([
                    new OWLAnonymousIndividual(),
                    new OWLAnonymousIndividual("AnonIDV3")])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AssertionAxioms.Count == 4);
            Assert.IsTrue(ontology2.AssertionAxioms[0] is OWLSameIndividual sameIdvs
                            && sameIdvs.IndividualExpressions[0].GetIRI().Equals(new RDFResource("ex:IDVA"))
                            && sameIdvs.IndividualExpressions[1].GetIRI().Equals(new RDFResource("ex:IDVB"))
                             && sameIdvs.Annotations.Count == 1
                             && sameIdvs.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(sameIdvs.Annotations.Single().ValueIRI, "ex:title")
                              && sameIdvs.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && sameIdvs.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.AssertionAxioms[1] is OWLSameIndividual sameIdvs1
                            && sameIdvs1.IndividualExpressions[0].GetIRI().Equals(new RDFResource("ex:IDVC"))
                            && sameIdvs1.IndividualExpressions[1].GetIRI().Equals(new RDFResource("bnode:AnonIDV1"))
                             && sameIdvs1.Annotations.Count == 2
                             && sameIdvs1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(sameIdvs1.Annotations[0].ValueIRI, "ex:comment1")
                              && sameIdvs1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && sameIdvs1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && sameIdvs1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(sameIdvs1.Annotations[1].ValueIRI, "ex:comment2")
                              && sameIdvs1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && sameIdvs1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.AssertionAxioms[2] is OWLSameIndividual sameIdvs2
                            && sameIdvs2.IndividualExpressions[0].GetIRI().Equals(new RDFResource("bnode:AnonIDV2"))
                            && sameIdvs2.IndividualExpressions[1].GetIRI().Equals(new RDFResource("ex:IDVD"))
                             && sameIdvs2.Annotations.Count == 2
                             && sameIdvs2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(sameIdvs2.Annotations[0].ValueIRI, "ex:comment1")
                              && sameIdvs2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && sameIdvs2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && sameIdvs2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(sameIdvs2.Annotations[1].ValueIRI, "ex:comment2")
                              && sameIdvs2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && sameIdvs2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.AssertionAxioms[3] is OWLSameIndividual sameIdvs3
                            && sameIdvs3.IndividualExpressions[0].GetIRI().IsBlank //Fully anonymous
                            && sameIdvs3.IndividualExpressions[1].GetIRI().Equals(new RDFResource("bnode:AnonIDV3"))
                             && sameIdvs3.Annotations.Count == 2
                             && sameIdvs3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(sameIdvs3.Annotations[0].ValueIRI, "ex:comment1")
                              && sameIdvs3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && sameIdvs3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && sameIdvs3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(sameIdvs3.Annotations[1].ValueIRI, "ex:comment2")
                              && sameIdvs3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && sameIdvs3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadDifferentIndividualsFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AssertionAxioms.Add(
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:IDVA")),
                    new OWLNamedIndividual(new RDFResource("ex:IDVB"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLDifferentIndividuals([
                   new OWLAnonymousIndividual("ANONIDV1"),
                   new OWLNamedIndividual(new RDFResource("ex:IDVC"))])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:IDVD")),
                    new OWLAnonymousIndividual("ANONIDV2")])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLDifferentIndividuals([
                    new OWLAnonymousIndividual(),
                    new OWLAnonymousIndividual("ANONIDV3")])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:IDVF")),
                    new OWLNamedIndividual(new RDFResource("ex:IDVG")),
                    new OWLNamedIndividual(new RDFResource("ex:IDVH")),
                    new OWLAnonymousIndividual("ANONIDV4")])
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFPlainLiteral("titolo", "it-IT")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AssertionAxioms.Count == 5);
            Assert.IsTrue(ontology2.AssertionAxioms[0] is OWLDifferentIndividuals diffIdvs
                            && diffIdvs.IndividualExpressions[0] is OWLNamedIndividual idvA
                            && idvA.GetIRI().Equals(new RDFResource("ex:IDVA"))
                            && diffIdvs.IndividualExpressions[1] is OWLNamedIndividual idvB
                            && idvB.GetIRI().Equals(new RDFResource("ex:IDVB"))
                             && diffIdvs.Annotations.Count == 1
                             && diffIdvs.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(diffIdvs.Annotations.Single().ValueIRI, "ex:title")
                              && diffIdvs.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && diffIdvs.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
            Assert.IsTrue(ontology2.AssertionAxioms[1] is OWLDifferentIndividuals diffIdvs1
                            && diffIdvs1.IndividualExpressions[0] is OWLAnonymousIndividual anonIdv1
                            && anonIdv1.GetIRI().Equals(new RDFResource("bnode:ANONIDV1"))
                            && diffIdvs1.IndividualExpressions[1] is OWLNamedIndividual idvC
                            && idvC.GetIRI().Equals(new RDFResource("ex:IDVC"))
                            && diffIdvs1.Annotations.Count == 2
                             && diffIdvs1.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(diffIdvs1.Annotations[0].ValueIRI, "ex:comment1")
                              && diffIdvs1.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && diffIdvs1.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && diffIdvs1.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(diffIdvs1.Annotations[1].ValueIRI, "ex:comment2")
                              && diffIdvs1.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && diffIdvs1.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.AssertionAxioms[2] is OWLDifferentIndividuals diffIdvs2
                            && diffIdvs2.IndividualExpressions[0] is OWLNamedIndividual idvD
                            && idvD.GetIRI().Equals(new RDFResource("ex:IDVD"))
                            && diffIdvs2.IndividualExpressions[1] is OWLAnonymousIndividual anonIdv2
                            && anonIdv2.GetIRI().Equals(new RDFResource("bnode:ANONIDV2"))
                            && diffIdvs2.Annotations.Count == 2
                             && diffIdvs2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(diffIdvs2.Annotations[0].ValueIRI, "ex:comment1")
                              && diffIdvs2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && diffIdvs2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && diffIdvs2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(diffIdvs2.Annotations[1].ValueIRI, "ex:comment2")
                              && diffIdvs2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && diffIdvs2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.AssertionAxioms[3] is OWLDifferentIndividuals diffIdvs3
                            && diffIdvs3.IndividualExpressions[0] is OWLAnonymousIndividual fullAnonIdv
                            && fullAnonIdv.GetIRI().IsBlank //Full anonymous
                            && diffIdvs3.IndividualExpressions[1] is OWLAnonymousIndividual anonIdv3
                            && anonIdv3.GetIRI().Equals(new RDFResource("bnode:ANONIDV3"))
                            && diffIdvs3.Annotations.Count == 2
                             && diffIdvs3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(diffIdvs3.Annotations[0].ValueIRI, "ex:comment1")
                              && diffIdvs3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && diffIdvs3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && diffIdvs3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(diffIdvs3.Annotations[1].ValueIRI, "ex:comment2")
                              && diffIdvs3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && diffIdvs3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.AssertionAxioms[4] is OWLDifferentIndividuals diffIdvs4
                            && diffIdvs4.IndividualExpressions[0] is OWLNamedIndividual idvF
                            && idvF.GetIRI().Equals(new RDFResource("ex:IDVF"))
                            && diffIdvs4.IndividualExpressions[1] is OWLNamedIndividual idvG
                            && idvG.GetIRI().Equals(new RDFResource("ex:IDVG"))
                            && diffIdvs4.IndividualExpressions[2] is OWLNamedIndividual idvH
                            && idvH.GetIRI().Equals(new RDFResource("ex:IDVH"))
                            && diffIdvs4.IndividualExpressions[3] is OWLAnonymousIndividual anonIdv4
                            && anonIdv4.GetIRI().Equals(new RDFResource("bnode:ANONIDV4"))
                             && diffIdvs4.Annotations.Count == 1
                             && diffIdvs4.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(diffIdvs4.Annotations.Single().ValueIRI, "ex:title")
                              && diffIdvs4.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && diffIdvs4.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("titolo", "it-IT")));
        }
        
		[TestMethod]
        public void ShouldReadObjectPropertyAssertionFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AssertionAxioms.Add(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")), 
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.DEPICTS),
                    new OWLAnonymousIndividual("ANONIDV1"), 
                    new OWLNamedIndividual(new RDFResource("ex:IDV3")))
				{
					Annotations = [
						new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
						{
							Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
						},
						new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
						{
							Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
						}
					]
				});
            ontology.AssertionAxioms.Add(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:IDV4")),
					new OWLAnonymousIndividual("ANONIDV2"))
					{
						Annotations = [
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
							},
							new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
							{
								Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
							}
						]
					});
            ontology.AssertionAxioms.Add(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLAnonymousIndividual(),
                    new OWLAnonymousIndividual("ANONIDV3"))
                {
                    Annotations = [
						new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
						{
							Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
						},
						new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
						{
							Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
						}
					]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AssertionAxioms.Count == 4);
			Assert.IsTrue(ontology2.AssertionAxioms[0] is OWLObjectPropertyAssertion objPropAsn
                            && objPropAsn.ObjectPropertyExpression is OWLObjectProperty foafKnows
                            && foafKnows.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
							&& objPropAsn.SourceIndividualExpression is OWLNamedIndividual exIdv1
                            && exIdv1.GetIRI().Equals(new RDFResource("ex:IDV1"))
							&& objPropAsn.TargetIndividualExpression is OWLNamedIndividual exIdv2
                            && exIdv2.GetIRI().Equals(new RDFResource("ex:IDV2"))
                             && objPropAsn.Annotations.Count == 1
                             && objPropAsn.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(objPropAsn.Annotations.Single().ValueIRI, "ex:title")
                              && objPropAsn.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && objPropAsn.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
			Assert.IsTrue(ontology2.AssertionAxioms[1] is OWLObjectPropertyAssertion objPropAsn2
                            && objPropAsn2.ObjectPropertyExpression is OWLObjectProperty foafKnows2
                            && foafKnows2.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
							&& objPropAsn2.SourceIndividualExpression is OWLNamedIndividual exIdv4
                            && exIdv4.GetIRI().Equals(new RDFResource("ex:IDV4"))
							&& objPropAsn2.TargetIndividualExpression is OWLAnonymousIndividual anonIdv2
                            && anonIdv2.GetIRI().Equals(new RDFResource("bnode:ANONIDV2"))
                             && objPropAsn2.Annotations.Count == 2
                             && objPropAsn2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropAsn2.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropAsn2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropAsn2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropAsn2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropAsn2.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropAsn2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropAsn2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.AssertionAxioms[2] is OWLObjectPropertyAssertion objPropAsn3
                            && objPropAsn3.ObjectPropertyExpression is OWLObjectProperty foafKnows3
                            && foafKnows3.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
							&& objPropAsn3.SourceIndividualExpression is OWLAnonymousIndividual fullAnonIdv
                            && fullAnonIdv.GetIRI().IsBlank //Full anonymous
							&& objPropAsn3.TargetIndividualExpression is OWLAnonymousIndividual anonIdv3
                            && anonIdv3.GetIRI().Equals(new RDFResource("bnode:ANONIDV3"))
                             && objPropAsn3.Annotations.Count == 2
                             && objPropAsn3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropAsn3.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropAsn3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropAsn3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropAsn3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropAsn3.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropAsn3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropAsn3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
			Assert.IsTrue(ontology2.AssertionAxioms[3] is OWLObjectPropertyAssertion objPropAsn4
                            && objPropAsn4.ObjectPropertyExpression is OWLObjectProperty foafDepicts
                            && foafDepicts.GetIRI().Equals(RDFVocabulary.FOAF.DEPICTS)
							&& objPropAsn4.SourceIndividualExpression is OWLAnonymousIndividual anonIdv1
                            && anonIdv1.GetIRI().Equals(new RDFResource("bnode:ANONIDV1"))
							&& objPropAsn4.TargetIndividualExpression is OWLNamedIndividual exIdv3
                            && exIdv3.GetIRI().Equals(new RDFResource("ex:IDV3"))
                             && objPropAsn4.Annotations.Count == 2
                             && objPropAsn4.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropAsn4.Annotations[0].ValueIRI, "ex:comment1")
                              && objPropAsn4.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropAsn4.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && objPropAsn4.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(objPropAsn4.Annotations[1].ValueIRI, "ex:comment2")
                              && objPropAsn4.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && objPropAsn4.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
		}

        [TestMethod]
        public void ShouldReadNegativeObjectPropertyAssertionFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AssertionAxioms.Add(
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLNamedIndividual(new RDFResource("ex:IDV2")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.DEPICTS),
                    new OWLAnonymousIndividual("ANONIDV1"),
                    new OWLNamedIndividual(new RDFResource("ex:IDV3")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:IDV4")),
                    new OWLAnonymousIndividual("ANONIDV2"))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLAnonymousIndividual(),
                    new OWLAnonymousIndividual("ANONIDV3"))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AssertionAxioms.Count == 4);
            Assert.IsTrue(ontology2.AssertionAxioms[0] is OWLNegativeObjectPropertyAssertion negObjPropAsn
                            && negObjPropAsn.ObjectPropertyExpression is OWLObjectProperty foafKnows
                            && foafKnows.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && negObjPropAsn.SourceIndividualExpression is OWLNamedIndividual exIdv1
                            && exIdv1.GetIRI().Equals(new RDFResource("ex:IDV1"))
                            && negObjPropAsn.TargetIndividualExpression is OWLNamedIndividual exIdv2
                            && exIdv2.GetIRI().Equals(new RDFResource("ex:IDV2"))
                             && negObjPropAsn.Annotations.Count == 1
                             && negObjPropAsn.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(negObjPropAsn.Annotations.Single().ValueIRI, "ex:title")
                              && negObjPropAsn.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && negObjPropAsn.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.AssertionAxioms[1] is OWLNegativeObjectPropertyAssertion negObjPropAsn2
                            && negObjPropAsn2.ObjectPropertyExpression is OWLObjectProperty foafDepicts
                            && foafDepicts.GetIRI().Equals(RDFVocabulary.FOAF.DEPICTS)
                            && negObjPropAsn2.SourceIndividualExpression is OWLAnonymousIndividual anonIdv1
                            && anonIdv1.GetIRI().Equals(new RDFResource("bnode:ANONIDV1"))
                            && negObjPropAsn2.TargetIndividualExpression is OWLNamedIndividual exIdv3
                            && exIdv3.GetIRI().Equals(new RDFResource("ex:IDV3"))
                             && negObjPropAsn2.Annotations.Count == 2
                             && negObjPropAsn2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(negObjPropAsn2.Annotations[0].ValueIRI, "ex:comment1")
                              && negObjPropAsn2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && negObjPropAsn2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && negObjPropAsn2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(negObjPropAsn2.Annotations[1].ValueIRI, "ex:comment2")
                              && negObjPropAsn2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && negObjPropAsn2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.AssertionAxioms[2] is OWLNegativeObjectPropertyAssertion negObjPropAsn3
                            && negObjPropAsn3.ObjectPropertyExpression is OWLObjectProperty foafKnows2
                            && foafKnows2.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && negObjPropAsn3.SourceIndividualExpression is OWLNamedIndividual exIdv4
                            && exIdv4.GetIRI().Equals(new RDFResource("ex:IDV4"))
                            && negObjPropAsn3.TargetIndividualExpression is OWLAnonymousIndividual anonIdv2
                            && anonIdv2.GetIRI().Equals(new RDFResource("bnode:ANONIDV2"))
                             && negObjPropAsn3.Annotations.Count == 2
                             && negObjPropAsn3.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(negObjPropAsn3.Annotations[0].ValueIRI, "ex:comment1")
                              && negObjPropAsn3.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && negObjPropAsn3.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && negObjPropAsn3.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(negObjPropAsn3.Annotations[1].ValueIRI, "ex:comment2")
                              && negObjPropAsn3.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && negObjPropAsn3.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
            Assert.IsTrue(ontology2.AssertionAxioms[3] is OWLNegativeObjectPropertyAssertion negObjPropAsn4
                            && negObjPropAsn4.ObjectPropertyExpression is OWLObjectProperty foafKnows3
                            && foafKnows3.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && negObjPropAsn4.SourceIndividualExpression is OWLAnonymousIndividual fullAnonIdv
                            && fullAnonIdv.GetIRI().IsBlank //Full anonymous
                            && negObjPropAsn4.TargetIndividualExpression is OWLAnonymousIndividual anonIdv3
                            && anonIdv3.GetIRI().Equals(new RDFResource("bnode:ANONIDV3"))
                             && negObjPropAsn4.Annotations.Count == 2
                             && negObjPropAsn4.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(negObjPropAsn4.Annotations[0].ValueIRI, "ex:comment1")
                              && negObjPropAsn4.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && negObjPropAsn4.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && negObjPropAsn4.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(negObjPropAsn4.Annotations[1].ValueIRI, "ex:comment2")
                              && negObjPropAsn4.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && negObjPropAsn4.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadDataPropertyAssertionFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AssertionAxioms.Add(
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFTypedLiteral("41", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                    new OWLAnonymousIndividual("ANONIDV1"),
                    new OWLLiteral(new RDFPlainLiteral("anonidv1")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AssertionAxioms.Count == 2);
            Assert.IsTrue(ontology2.AssertionAxioms[0] is OWLDataPropertyAssertion dtPropAsn
                            && dtPropAsn.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                            && dtPropAsn.IndividualExpression is OWLNamedIndividual exIdv1
                            && exIdv1.GetIRI().Equals(new RDFResource("ex:IDV1"))
                            && dtPropAsn.Literal.GetLiteral().Equals(new RDFTypedLiteral("41", RDFModelEnums.RDFDatatypes.XSD_INTEGER))
                             && dtPropAsn.Annotations.Count == 1
                             && dtPropAsn.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(dtPropAsn.Annotations.Single().ValueIRI, "ex:title")
                              && dtPropAsn.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && dtPropAsn.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.AssertionAxioms[1] is OWLDataPropertyAssertion dtPropAsn2
                            && dtPropAsn2.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.NAME)
                            && dtPropAsn2.IndividualExpression is OWLAnonymousIndividual anonIdv1
                            && anonIdv1.GetIRI().Equals(new RDFResource("bnode:ANONIDV1"))
                            && dtPropAsn2.Literal.GetLiteral().Equals(new RDFPlainLiteral("anonidv1"))
                             && dtPropAsn2.Annotations.Count == 2
                             && dtPropAsn2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(dtPropAsn2.Annotations[0].ValueIRI, "ex:comment1")
                              && dtPropAsn2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && dtPropAsn2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && dtPropAsn2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(dtPropAsn2.Annotations[1].ValueIRI, "ex:comment2")
                              && dtPropAsn2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && dtPropAsn2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

        [TestMethod]
        public void ShouldReadNegativeDataPropertyAssertionFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AssertionAxioms.Add(
                new OWLNegativeDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")),
                    new OWLLiteral(new RDFTypedLiteral("41", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLNegativeDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                    new OWLAnonymousIndividual("ANONIDV1"),
                    new OWLLiteral(new RDFPlainLiteral("anonidv1")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment1"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("commento", "it-IT")))
                        },
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:comment2"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("comment", "en-US")))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AssertionAxioms.Count == 2);
            Assert.IsTrue(ontology2.AssertionAxioms[0] is OWLNegativeDataPropertyAssertion dtPropAsn
                            && dtPropAsn.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                            && dtPropAsn.IndividualExpression is OWLNamedIndividual exIdv1
                            && exIdv1.GetIRI().Equals(new RDFResource("ex:IDV1"))
                            && dtPropAsn.Literal.GetLiteral().Equals(new RDFTypedLiteral("41", RDFModelEnums.RDFDatatypes.XSD_INTEGER))
                             && dtPropAsn.Annotations.Count == 1
                             && dtPropAsn.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(dtPropAsn.Annotations.Single().ValueIRI, "ex:title")
                              && dtPropAsn.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && dtPropAsn.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.AssertionAxioms[1] is OWLNegativeDataPropertyAssertion dtPropAsn2
                            && dtPropAsn2.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.NAME)
                            && dtPropAsn2.IndividualExpression is OWLAnonymousIndividual anonIdv1
                            && anonIdv1.GetIRI().Equals(new RDFResource("bnode:ANONIDV1"))
                            && dtPropAsn2.Literal.GetLiteral().Equals(new RDFPlainLiteral("anonidv1"))
                             && dtPropAsn2.Annotations.Count == 2
                             && dtPropAsn2.Annotations[0].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(dtPropAsn2.Annotations[0].ValueIRI, "ex:comment1")
                              && dtPropAsn2.Annotations[0].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && dtPropAsn2.Annotations[0].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("commento", "it-IT"))
                             && dtPropAsn2.Annotations[1].AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                             && string.Equals(dtPropAsn2.Annotations[1].ValueIRI, "ex:comment2")
                              && dtPropAsn2.Annotations[1].Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.RDFS.COMMENT)
                              && dtPropAsn2.Annotations[1].Annotation.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("comment", "en-US")));
        }

		[TestMethod]
        public void ShouldReadDataPropertyDomainGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.DataPropertyAxioms.Add(
                new OWLDataPropertyDomain(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLClass(RDFVocabulary.FOAF.PERSON))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
			RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology2.DataPropertyAxioms[0] is OWLDataPropertyDomain dtPropDom
                            && dtPropDom.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                            && dtPropDom.ClassExpression is OWLClass foafPerson
                            && foafPerson.GetIRI().Equals(RDFVocabulary.FOAF.PERSON)
                             && dtPropDom.Annotations.Count == 1
                             && dtPropDom.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(dtPropDom.Annotations.Single().ValueIRI, "ex:title")
                              && dtPropDom.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && dtPropDom.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

		[TestMethod]
        public void ShouldReadDataPropertyRangeGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.DataPropertyAxioms.Add(
                new OWLDataPropertyRange(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLDatatype(RDFVocabulary.XSD.INTEGER))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology2.DataPropertyAxioms[0] is OWLDataPropertyRange dtPropRng
                            && dtPropRng.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                            && dtPropRng.DataRangeExpression is OWLDatatype xsdInteger
                            && xsdInteger.GetIRI().Equals(RDFVocabulary.XSD.INTEGER)
                             && dtPropRng.Annotations.Count == 1
                             && dtPropRng.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(dtPropRng.Annotations.Single().ValueIRI, "ex:title")
                              && dtPropRng.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && dtPropRng.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

		[TestMethod]
        public void ShouldReadClassAssertionFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AssertionAxioms.Add(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLNamedIndividual(new RDFResource("ex:IDV1")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            ontology.AssertionAxioms.Add(
                new OWLClassAssertion(
                    new OWLObjectUnionOf([
						new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLAnonymousIndividual("AnonIDV1")),
						new OWLObjectOneOf([new OWLNamedIndividual(new RDFResource("ex:IDV2")), new OWLNamedIndividual(new RDFResource("ex:IDV3"))])
					]),
                    new OWLNamedIndividual(new RDFResource("ex:IDV4")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
			ontology.AssertionAxioms.Add(
                new OWLClassAssertion(
                    new OWLDataExactCardinality(
						new OWLDataProperty(RDFVocabulary.FOAF.AGE), 1,  new OWLDatatypeRestriction(
							new OWLDatatype(RDFVocabulary.XSD.STRING),
							[new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
							new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH)])),
                    new OWLNamedIndividual(new RDFResource("ex:IDV5")))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AssertionAxioms.Count == 3);
            Assert.IsTrue(ontology2.AssertionAxioms[0] is OWLClassAssertion clsAsn
                            && clsAsn.ClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.PERSON)
                            && clsAsn.IndividualExpression is OWLNamedIndividual exIdv1
                            && exIdv1.GetIRI().Equals(new RDFResource("ex:IDV1"))
                             && clsAsn.Annotations.Count == 1
                             && clsAsn.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(clsAsn.Annotations.Single().ValueIRI, "ex:title")
                              && clsAsn.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && clsAsn.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
            Assert.IsTrue(ontology2.AssertionAxioms[1] is OWLClassAssertion clsAsn1
                            && clsAsn1.ClassExpression is OWLObjectUnionOf objUnOf
							&& objUnOf.ClassExpressions.Count == 2
							&& objUnOf.ClassExpressions[0] is OWLObjectHasValue objHV
							&& objHV.ObjectPropertyExpression is OWLObjectProperty objHVObjProp
							&& objHVObjProp.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
							&& objHV.IndividualExpression is OWLAnonymousIndividual anonIDV1
							&& anonIDV1.GetIRI().Equals(new RDFResource("bnode:AnonIDV1"))
							&& objUnOf.ClassExpressions[1] is OWLObjectOneOf objOneOf
							&& objOneOf.IndividualExpressions.Count == 2
							&& objOneOf.IndividualExpressions[0] is OWLNamedIndividual exIdv2
							&& exIdv2.GetIRI().Equals(new RDFResource("ex:IDV2"))
							&& objOneOf.IndividualExpressions[1] is OWLNamedIndividual exIdv3
							&& exIdv3.GetIRI().Equals(new RDFResource("ex:IDV3"))
                            && clsAsn1.IndividualExpression is OWLNamedIndividual exIdv4
                            && exIdv4.GetIRI().Equals(new RDFResource("ex:IDV4"))
                             && clsAsn1.Annotations.Count == 1
                             && clsAsn1.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(clsAsn1.Annotations.Single().ValueIRI, "ex:title")
                              && clsAsn1.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && clsAsn1.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
			Assert.IsTrue(ontology2.AssertionAxioms[2] is OWLClassAssertion clsAsn2
                            && clsAsn2.ClassExpression is OWLDataExactCardinality dtExCard
							&& dtExCard.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
							&& dtExCard.Cardinality == "1"
							&& dtExCard.DataRangeExpression is OWLDatatypeRestriction dtRestr
							&& dtRestr.Datatype.GetIRI().Equals(RDFVocabulary.XSD.STRING)
							&& dtRestr.FacetRestrictions.Count == 2
							&& string.Equals(dtRestr.FacetRestrictions[0].FacetIRI, OWLFacetRestriction.MIN_LENGTH.ToString())
							&& dtRestr.FacetRestrictions[0].Literal.GetLiteral().Equals(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT))
							&& string.Equals(dtRestr.FacetRestrictions[1].FacetIRI, OWLFacetRestriction.MAX_LENGTH.ToString())
							&& dtRestr.FacetRestrictions[1].Literal.GetLiteral().Equals(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT))
                            && clsAsn2.IndividualExpression is OWLNamedIndividual exIdv5
                            && exIdv5.GetIRI().Equals(new RDFResource("ex:IDV5"))
                            && clsAsn2.Annotations.Count == 1
                             && clsAsn2.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(clsAsn2.Annotations.Single().ValueIRI, "ex:title")
                              && clsAsn2.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && clsAsn2.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public void ShouldReadSubAnnotationPropertyOfFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AnnotationAxioms.Add(
                new OWLSubAnnotationPropertyOf(
                    new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    new OWLAnnotationProperty(RDFVocabulary.DC.TITLE))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AnnotationAxioms.Count == 1);
            Assert.IsTrue(ontology2.AnnotationAxioms[0] is OWLSubAnnotationPropertyOf subanProps
                            && subanProps.SubAnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                            && subanProps.SuperAnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && subanProps.Annotations.Count == 1
                             && subanProps.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(subanProps.Annotations.Single().ValueIRI, "ex:title")
                              && subanProps.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && subanProps.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

		[TestMethod]
        public void ShouldReadAnnotationPropertyDomainFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyDomain(
                    new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    RDFVocabulary.FOAF.PERSON)
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
			ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyDomain(
                    new OWLAnnotationProperty(RDFVocabulary.DC.TITLE),
                    new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AnnotationAxioms.Count == 2);
            Assert.IsTrue(ontology2.AnnotationAxioms[0] is OWLAnnotationPropertyDomain anPropDom
                            && anPropDom.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                            && string.Equals(anPropDom.IRI, RDFVocabulary.FOAF.PERSON.ToString())
                             && anPropDom.Annotations.Count == 1
                             && anPropDom.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(anPropDom.Annotations.Single().ValueIRI, "ex:title")
                              && anPropDom.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && anPropDom.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
			Assert.IsTrue(ontology2.AnnotationAxioms[1] is OWLAnnotationPropertyDomain anPropDom1
                            && anPropDom1.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                            && string.Equals(anPropDom1.IRI,RDFVocabulary.FOAF.PERSON.ToString())
                             && anPropDom1.Annotations.Count == 1
                             && anPropDom1.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(anPropDom1.Annotations.Single().ValueIRI, "ex:title")
                              && anPropDom1.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && anPropDom1.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
		}
        
		[TestMethod]
        public void ShouldReadAnnotationPropertyRangeFromGraph()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"), new Uri("ex:ont/v1"));
            ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyRange(
                    new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE),
                    RDFVocabulary.FOAF.PERSON)
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
			ontology.AnnotationAxioms.Add(
                new OWLAnnotationPropertyRange(
                    new OWLAnnotationProperty(RDFVocabulary.DC.TITLE),
                    new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI))
                {
                    Annotations = [
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.TITLE), new RDFResource("ex:title"))
                        {
                            Annotation = new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLLiteral(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                        }
                    ]
                });
            RDFGraph graph = ontology.ToRDFGraph();
            OWLOntology ontology2 = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.IRI, "ex:ont"));
            Assert.IsTrue(string.Equals(ontology2.VersionIRI, "ex:ont/v1"));
            Assert.IsTrue(ontology2.AnnotationAxioms.Count == 2);
            Assert.IsTrue(ontology2.AnnotationAxioms[0] is OWLAnnotationPropertyRange anPropRng
                            && anPropRng.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                            && string.Equals(anPropRng.IRI, RDFVocabulary.FOAF.PERSON.ToString())
                             && anPropRng.Annotations.Count == 1
                             && anPropRng.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(anPropRng.Annotations.Single().ValueIRI, "ex:title")
                              && anPropRng.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && anPropRng.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
			Assert.IsTrue(ontology2.AnnotationAxioms[1] is OWLAnnotationPropertyRange anPropRng1
                            && anPropRng1.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                            && string.Equals(anPropRng1.IRI,RDFVocabulary.FOAF.PERSON.ToString())
                             && anPropRng1.Annotations.Count == 1
                             && anPropRng1.Annotations.Single().AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.TITLE)
                             && string.Equals(anPropRng1.Annotations.Single().ValueIRI, "ex:title")
                              && anPropRng1.Annotations.Single().Annotation.AnnotationProperty.GetIRI().Equals(RDFVocabulary.DC.DCTERMS.TITLE)
                              && anPropRng1.Annotations.Single().Annotation.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("titolo", RDFModelEnums.RDFDatatypes.XSD_STRING)));
		}
		#endregion

        [TestCleanup]
        public void Cleanup()
        {
            foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory, "OWLOntologyTest_Should*"))
                File.Delete(file);
        }
    }
}