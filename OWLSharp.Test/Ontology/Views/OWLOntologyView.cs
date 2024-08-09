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
		public async Task ShouldCountClassAssertionAxiomsAsync()
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
	}
}