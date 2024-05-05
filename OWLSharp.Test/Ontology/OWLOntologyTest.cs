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
			Assert.IsTrue(ontology.Prefixes.Count == 11); //TODO: since we inject 5 default prefixes, there may be duplicates after deserialization
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
        #endregion
    }
}