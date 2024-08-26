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
using OWLSharp.Extensions.SWRL.Model.Atoms;
using OWLSharp.Extensions.SWRL.Model;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWLSharp.Extensions.SWRL.Reasoner;
using System;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Extensions.SWRL.Model.BuiltIns;

namespace OWLSharp.Test.Extensions.SWRL.Reasoner
{
    [TestClass]
    public class SWRLReasonerTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReasoner()
        {
            SWRLReasoner reasoner = new SWRLReasoner()
            {
                Rules = [
                    new SWRLRule(new Uri("ex:testRule"), "description",
                        new SWRLAntecedent(), new SWRLConsequent())
                ]
            };

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.Rules);
            Assert.IsTrue(reasoner.Rules.Count == 1);
        }

        [TestMethod]
        public async Task ShouldApplyToOntologyAsync()
        {
            string infoMsg = null;
            OWLEvents.OnInfo += (string msg) => { infoMsg += msg; };

            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:class"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:indiv")))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:class")),
                        new OWLNamedIndividual(new RDFResource("ex:indiv")))
                ]
            };

            SWRLReasoner reasoner = new SWRLReasoner()
            {
                Rules = [ 
                    new SWRLRule(new Uri("ex:testRule"), "this is test rule",
                        new SWRLAntecedent()
                        {
                            Atoms = [
                                new SWRLClassAtom(new OWLClass(new RDFResource("ex:class")), new RDFVariable("?C"))
                            ]
                        },
                        new SWRLConsequent()
                        {
                            Atoms = [
                                new SWRLClassAtom(new OWLClass(RDFVocabulary.OWL.NAMED_INDIVIDUAL), new RDFVariable("?C"))
                            ]
                        }) 
                ]
            };

            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsNotNull(infoMsg);
            Assert.IsTrue(infoMsg.IndexOf("1 unique inferences") > -1);
        }

		[TestMethod]
        public async Task HarryPotterReasoningExample()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://harrypotter.com/ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://harrypotter.com/Wizard"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://harrypotter.com/loves"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("http://harrypotter.com/hasRedHair"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HarryPotter"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers")))
                ],
                AssertionAxioms = [
					new OWLClassAssertion(
						new OWLClass(new RDFResource("http://harrypotter.com/Wizard")),
						new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HarryPotter"))),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("http://harrypotter.com/Wizard")),
						new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley"))),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("http://harrypotter.com/Wizard")),
						new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers"))),
                    new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HarryPotter")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HarryPotter")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HarryPotter"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HarryPotter"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("http://harrypotter.com/loves")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley"))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("http://harrypotter.com/hasRedHair")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/RonWesley")),
                        new OWLLiteral(RDFTypedLiteral.True)),
					new OWLNegativeDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("http://harrypotter.com/hasRedHair")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HarryPotter")),
                        new OWLLiteral(RDFTypedLiteral.True)),
					new OWLNegativeDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("http://harrypotter.com/hasRedHair")),
                        new OWLNamedIndividual(new RDFResource("http://harrypotter.com/HermioneGrangers")),
                        new OWLLiteral(RDFTypedLiteral.True)),
                ]
            };
            string ontString = OWLSerializer.SerializeOntology(ontology);
            SWRLReasoner reasoner = new SWRLReasoner()
			{
				Rules = [
					new SWRLRule(
						new Uri("http://harrypotter.com/HogwardsRule1"), 
						"Wizards knowing Harry Potter and having red hair are aliases of Ron Wesley",
						new SWRLAntecedent() { 
							Atoms = [
								new SWRLClassAtom(new OWLClass(new RDFResource("http://harrypotter.com/Wizard")), new RDFVariable("?W")),
								new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")), new RDFVariable("?W"), new RDFResource("http://harrypotter.com/HarryPotter")),
								new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("http://harrypotter.com/hasRedHair")), new RDFVariable("?W"), RDFTypedLiteral.True)
							] },
						new SWRLConsequent() { 
							Atoms = [
								new SWRLSameAsAtom(new RDFVariable("?W"), new RDFResource("http://harrypotter.com/RonWesley"))
							] }),
					new SWRLRule(
						new Uri("ex:HogwardsRule2"), 
						"Wizards knowing Harry Potter and not having red hair and loving Ron Wesley and having 'Hermione' in their IRIs are aliases of Hermione Grangers",
						new SWRLAntecedent() { 
							Atoms = [
								new SWRLClassAtom(new OWLClass(new RDFResource("http://harrypotter.com/Wizard")), new RDFVariable("?WIZ")),
								new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("http://harrypotter.com/knows")), new RDFVariable("?WIZ"), new RDFResource("http://harrypotter.com/HarryPotter")),
								new SWRLNegativeDataPropertyAtom(new OWLDataProperty(new RDFResource("http://harrypotter.com/hasRedHair")), new RDFVariable("?WIZ"), RDFTypedLiteral.True),
								new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("http://harrypotter.com/loves")), new RDFVariable("?WIZ"), new RDFResource("http://harrypotter.com/RonWesley")),
								new SWRLContainsIgnoreCaseBuiltIn(new RDFVariable("?WIZ"), "hermione")
							] },
						new SWRLConsequent() { 
							Atoms = [
								new SWRLSameAsAtom(new RDFVariable("?WIZ"), new RDFResource("http://harrypotter.com/HermioneGrangers"))
							] })
				]
			};
            string reasonerString = reasoner.ToString();

            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 2);
        }
        #endregion
    }
}