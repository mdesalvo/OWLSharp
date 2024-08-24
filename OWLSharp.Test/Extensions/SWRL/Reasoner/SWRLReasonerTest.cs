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
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:Wizard"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows"))),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:loves"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasRedHair"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:HarryPotter"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:RonWesley"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers")))
                ],
                AssertionAxioms = [
					new OWLClassAssertion(
						new OWLClass(new RDFResource("ex:Wizard")),
						new OWLNamedIndividual(new RDFResource("ex:HarryPotter"))),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("ex:Wizard")),
						new OWLNamedIndividual(new RDFResource("ex:RonWesley"))),
					new OWLClassAssertion(
						new OWLClass(new RDFResource("ex:Wizard")),
						new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers"))),
                    new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:knows")),
                        new OWLNamedIndividual(new RDFResource("ex:HarryPotter")),
                        new OWLNamedIndividual(new RDFResource("ex:RonWesley"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:knows")),
                        new OWLNamedIndividual(new RDFResource("ex:HarryPotter")),
                        new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:knows")),
                        new OWLNamedIndividual(new RDFResource("ex:RonWesley")),
                        new OWLNamedIndividual(new RDFResource("ex:HarryPotter"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:knows")),
                        new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers")),
                        new OWLNamedIndividual(new RDFResource("ex:HarryPotter"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:knows")),
                        new OWLNamedIndividual(new RDFResource("ex:RonWesley")),
                        new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:knows")),
                        new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers")),
                        new OWLNamedIndividual(new RDFResource("ex:RonWesley"))),
					new OWLObjectPropertyAssertion(
						new OWLObjectProperty(new RDFResource("ex:loves")),
                        new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers")),
                        new OWLNamedIndividual(new RDFResource("ex:RonWesley"))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:hasRedHair")),
                        new OWLNamedIndividual(new RDFResource("ex:RonWesley")),
                        new OWLLiteral(RDFTypedLiteral.True)),
					new OWLNegativeDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:hasRedHair")),
                        new OWLNamedIndividual(new RDFResource("ex:HarryPotter")),
                        new OWLLiteral(RDFTypedLiteral.True)),
					new OWLNegativeDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:hasRedHair")),
                        new OWLNamedIndividual(new RDFResource("ex:HermioneGrangers")),
                        new OWLLiteral(RDFTypedLiteral.True)),
                ]
            };

            SWRLReasoner reasoner = new SWRLReasoner()
			{
				Rules = [
					new SWRLRule(
						new Uri("ex:HogwardsRule1"), 
						"Wizards knowing Harry Potter and having red hair are aliases of Ron Wesley",
						new SWRLAntecedent() { 
							Atoms = [
								new SWRLClassAtom(new OWLClass(new RDFResource("ex:Wizard")), new RDFVariable("?W")),
								new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("ex:knows")), new RDFVariable("?W"), new RDFResource("ex:HarryPotter")),
								new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:hasRedHair")), new RDFVariable("?W"), RDFTypedLiteral.True)
							] },
						new SWRLConsequent() { 
							Atoms = [
								new SWRLSameAsAtom(new RDFVariable("?W"), new RDFResource("ex:RonWesley"))
							] }),
					new SWRLRule(
						new Uri("ex:HogwardsRule2"), 
						"Wizards knowing Harry Potter and not having red hair and loving Ron Wesley and having 'Hermione' in their IRIs are aliases of Hermione Grangers",
						new SWRLAntecedent() { 
							Atoms = [
								new SWRLClassAtom(new OWLClass(new RDFResource("ex:Wizard")), new RDFVariable("?WIZ")),
								new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("ex:knows")), new RDFVariable("?WIZ"), new RDFResource("ex:HarryPotter")),
								new SWRLNegativeDataPropertyAtom(new OWLDataProperty(new RDFResource("ex:hasRedHair")), new RDFVariable("?WIZ"), RDFTypedLiteral.True),
								new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("ex:loves")), new RDFVariable("?WIZ"), new RDFResource("ex:RonWesley")),
								new SWRLContainsIgnoreCaseBuiltIn(new RDFVariable("?WIZ"), "hermione")
							] },
						new SWRLConsequent() { 
							Atoms = [
								new SWRLSameAsAtom(new RDFVariable("?WIZ"), new RDFResource("ex:HermioneGrangers"))
							] })
				]
			};

            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 2);
        }
        #endregion
    }
}