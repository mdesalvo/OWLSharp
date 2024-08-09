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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Views;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Views
{
	[TestClass]
	public class OWLOntologyViewTest
	{
		[TestMethod]
		public async Task ShouldCountAnnotationsAndImportsAndPrefixesAsync()
		{
			OWLOntology ont = new OWLOntology() 
			{
				Imports = [
					new OWLImport(new RDFResource("ex:org"))
				],
				Annotations = [
					new OWLAnnotation(
						new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
						new OWLLiteral(new RDFPlainLiteral("comment")))
				]
			};
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.AnnotationsCountAsync() == 1);
			Assert.IsTrue(await ontView.ImportsCountAsync() == 1);
			Assert.IsTrue(await ontView.PrefixesCountAsync() == 5);
		}

		[TestMethod]
		public async Task ShouldCountEntitiesAndDeclarationsAsync()
		{
			OWLOntology ont = new OWLOntology() 
			{
				DeclarationAxioms = [
					new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
					new OWLDeclaration(new OWLDataProperty(RDFVocabulary.RDFS.LABEL)),
					new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.RDFS.DOMAIN)),
					new OWLDeclaration(new OWLClass(RDFVocabulary.RDFS.RESOURCE)),
					new OWLDeclaration(new OWLDatatype(RDFVocabulary.RDFS.LITERAL)),
					new OWLDeclaration(new OWLNamedIndividual(RDFVocabulary.RDF.NIL)),
				]
			};
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.AnnotationPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.DataPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.ObjectPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.ClassCountAsync() == 1);
			Assert.IsTrue(await ontView.DatatypeCountAsync() == 1);
			Assert.IsTrue(await ontView.NamedIndividualCountAsync() == 1);
			Assert.IsTrue(await ontView.DeclarationCountAsync() == 6);
		}

		[TestMethod]
		public async Task ShouldCountAnnotationAxiomsAsync()
		{
			OWLOntology ont = new OWLOntology() 
			{
				AnnotationAxioms = [
					new OWLAnnotationAssertion(
						new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
						RDFVocabulary.RDFS.COMMENT,
						new OWLLiteral(new RDFPlainLiteral("comment"))),
					new OWLAnnotationPropertyDomain(
						new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
						RDFVocabulary.RDFS.RESOURCE),
					new OWLAnnotationPropertyRange(
						new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
						RDFVocabulary.RDFS.RESOURCE),
					new OWLSubAnnotationPropertyOf(
						new OWLAnnotationProperty(new RDFResource("ex:comment")),
						new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT))
				],
				DeclarationAxioms = [
					new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT)),
					new OWLDeclaration(new OWLAnnotationProperty(new RDFResource("ex:comment"))),
					new OWLDeclaration(new OWLClass(RDFVocabulary.RDFS.RESOURCE))
				]
			};
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.AnnotationAssertionCountAsync() == 1);
			Assert.IsTrue(await ontView.AnnotationPropertyDomainCountAsync() == 1);
			Assert.IsTrue(await ontView.AnnotationPropertyRangeCountAsync() == 1);
			Assert.IsTrue(await ontView.SubAnnotationPropertyOfCountAsync() == 1);
		}

		[TestMethod]
		public async Task ShouldCountAssertionAxiomsAsync()
		{
			OWLOntology ont = new OWLOntology() 
			{
				DeclarationAxioms = [
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Human"))),
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Developer"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Markus"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv")))
				],
				AssertionAxioms = [
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:age")),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLNamedIndividual(new RDFResource("ex:Stiv")),
						new OWLNamedIndividual(new RDFResource("ex:John"))),
					new OWLNegativeDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:age")),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLLiteral(new RDFTypedLiteral("33", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER))),
					new OWLNegativeObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:John"))),
					new OWLSameIndividual([
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:Markus"))]),
					new OWLDifferentIndividuals([
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:John"))]),
					new OWLDifferentIndividuals([
						new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLNamedIndividual(new RDFResource("ex:Stiv"))]),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("ex:Developer")),
						new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("ex:Human")),
						new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("ex:Human")),
						new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("ex:Human")),
						new OWLNamedIndividual(new RDFResource("ex:John")))
				]
			};	
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.ClassAssertionCountAsync() == 4);
			Assert.IsTrue(await ontView.DataPropertyAssertionCountAsync() == 1);
			Assert.IsTrue(await ontView.DifferentIndividualsCountAsync() == 2);
			Assert.IsTrue(await ontView.NegativeDataPropertyAssertionCountAsync() == 1);
			Assert.IsTrue(await ontView.NegativeObjectPropertyAssertionCountAsync() == 1);
			Assert.IsTrue(await ontView.ObjectPropertyAssertionCountAsync() == 2);
			Assert.IsTrue(await ontView.SameIndividualCountAsync() == 1);
		}

		[TestMethod]
		public async Task ShouldCountClassAxiomsAsync()
		{
			OWLOntology ont = new OWLOntology() 
			{
				DeclarationAxioms = [
					new OWLDeclaration(new OWLClass(new RDFResource("ex:LivingEntity"))),
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Animal"))),
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Cat"))),
					new OWLDeclaration(new OWLClass(new RDFResource("ex:DomesticFeline"))),
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog")))
				],
				ClassAxioms = [
					new OWLSubClassOf(
						new OWLClass(new RDFResource("ex:Animal")),
						new OWLClass(new RDFResource("ex:LivingEntity"))),
					new OWLSubClassOf(
						new OWLClass(new RDFResource("ex:Cat")),
						new OWLClass(new RDFResource("ex:Animal"))),
					new OWLSubClassOf(
						new OWLClass(new RDFResource("ex:Dog")),
						new OWLClass(new RDFResource("ex:Animal"))),
					new OWLDisjointClasses([
						new OWLClass(new RDFResource("ex:Cat")),
						new OWLClass(new RDFResource("ex:Dog"))]),
					new OWLDisjointUnion(
						new OWLClass(new RDFResource("ex:Animal")),
						[ new OWLClass(new RDFResource("ex:Cat")), new OWLClass(new RDFResource("ex:Dog")) ]),
					new OWLEquivalentClasses([
						new OWLClass(new RDFResource("ex:DomesticFeline")),
						new OWLClass(new RDFResource("ex:Cat"))]),
				]
			};
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.DisjointClassesCountAsync() == 1);
			Assert.IsTrue(await ontView.DisjointUnionCountAsync() == 1);
			Assert.IsTrue(await ontView.EquivalentClassesCountAsync() == 1);
			Assert.IsTrue(await ontView.SubClassOfCountAsync() == 3);
		}
	
		[TestMethod]
		public async Task ShouldCountDataPropertyAxioms()
		{
			OWLOntology ont = new OWLOntology() 
			{
				DeclarationAxioms = [
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Human"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasData"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasPersonalData"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasAge"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:isNYearsOld"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasName"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
				],
				DataPropertyAxioms = [
					new OWLSubDataPropertyOf(
						new OWLDataProperty(new RDFResource("ex:hasAge")),
						new OWLDataProperty(new RDFResource("ex:hasPersonalData"))),
					new OWLSubDataPropertyOf(
						new OWLDataProperty(new RDFResource("ex:hasPersonalData")),
						new OWLDataProperty(new RDFResource("ex:hasData"))),
					new OWLSubDataPropertyOf(
						new OWLDataProperty(new RDFResource("ex:hasName")),
						new OWLDataProperty(new RDFResource("ex:hasPersonalData"))),
					new OWLEquivalentDataProperties([
						new OWLDataProperty(new RDFResource("ex:hasAge")),
						new OWLDataProperty(new RDFResource("ex:isNYearsOld"))]),
					new OWLDisjointDataProperties([
						new OWLDataProperty(new RDFResource("ex:hasAge")),
						new OWLDataProperty(new RDFResource("ex:hasName"))]),
					new OWLDataPropertyDomain(
						new OWLDataProperty(new RDFResource("ex:hasAge")),
						new OWLClass(new RDFResource("ex:Human"))),
					new OWLDataPropertyRange(
						new OWLDataProperty(new RDFResource("ex:hasAge")),
						new OWLDatatype(RDFVocabulary.XSD.INTEGER)),
					new OWLFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:hasAge")))
				]
			};
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.DataPropertyDomainCountAsync() == 1);
			Assert.IsTrue(await ontView.DataPropertyRangeCountAsync() == 1);
			Assert.IsTrue(await ontView.DisjointDataPropertiesCountAsync() == 1);
			Assert.IsTrue(await ontView.EquivalentDataPropertiesCountAsync() == 1);
			Assert.IsTrue(await ontView.FunctionalDataPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.SubDataPropertyOfCountAsync() == 3);
		}
	
		[TestMethod]
		public async Task ShouldCountObjectPropertyAxioms()
		{
			OWLOntology ont = new OWLOntology() 
			{
				DeclarationAxioms = [
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Human"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:bestFriendOf"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:isFavoriteFriendOf"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:loves")))
				],
				ObjectPropertyAxioms = [
					new OWLSubObjectPropertyOf(
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLObjectProperty(new RDFResource("ex:knows"))),
					new OWLSubObjectPropertyOf(
						new OWLObjectProperty(new RDFResource("ex:bestFriendOf")),
						new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLEquivalentObjectProperties([
						new OWLObjectProperty(new RDFResource("ex:bestFriendOf")),
						new OWLObjectProperty(new RDFResource("ex:isFavoriteFriendOf"))]),
					new OWLDisjointObjectProperties([
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLObjectProperty(new RDFResource("ex:loves"))]),
					new OWLObjectPropertyDomain(
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLClass(new RDFResource("ex:Human"))),
					new OWLObjectPropertyRange(
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLClass(new RDFResource("ex:Human"))),
					new OWLInverseObjectProperties(
						new OWLObjectProperty(new RDFResource("ex:friendOf")),
						new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
					//Just for test purposes...
					new OWLFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
					new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf")))
				]
			};
			OWLOntologyView ontView = new OWLOntologyView(ont);

			Assert.IsTrue(await ontView.AsymmetricObjectPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.DisjointObjectPropertiesCountAsync() == 1);
			Assert.IsTrue(await ontView.EquivalentObjectPropertiesCountAsync() == 1);
			Assert.IsTrue(await ontView.FunctionalObjectPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.InverseFunctionalObjectPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.InverseObjectPropertiesCountAsync() == 1);
			Assert.IsTrue(await ontView.IrreflexiveObjectPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.ObjectPropertyDomainCountAsync() == 1);
			Assert.IsTrue(await ontView.ObjectPropertyRangeCountAsync() == 1);
			Assert.IsTrue(await ontView.ReflexiveObjectPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.SubObjectPropertyOfCountAsync() == 2);
			Assert.IsTrue(await ontView.SymmetricObjectPropertyCountAsync() == 1);
			Assert.IsTrue(await ontView.TransitiveObjectPropertyCountAsync() == 1);
		}
	}
}