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
        public void ShouldReadAsymmetricObjectPropertyAxiomFromGraph()
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
        public void ShouldReadSymmetricObjectPropertyAxiomFromGraph()
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
        public void ShouldReadIrreflexiveObjectPropertyAxiomFromGraph()
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
        public void ShouldReadReflexiveObjectPropertyAxiomFromGraph()
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
        public void ShouldReadTransitiveObjectPropertyAxiomFromGraph()
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
        public void ShouldReadInverseFunctionalObjectPropertyAxiomFromGraph()
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
        public void ShouldReadFunctionalObjectPropertyAxiomFromGraph()
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
        public void ShouldReadInverseObjectPropertyAxiomFromGraph()
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
        public void ShouldReadEquivalentObjectPropertyAxiomFromGraph()
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
        public void ShouldReadDisjointObjectPropertyAxiomFromGraph()
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
                    new OWLObjectProperty(new RDFResource("ex:objPropA2")),
					new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:objPropB2"))),])
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