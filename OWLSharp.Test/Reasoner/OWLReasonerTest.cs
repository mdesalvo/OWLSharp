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
using OWLSharp.Reasoner;
using OWLSharp.Reasoner.Rules;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Test.Reasoner
{
    [TestClass]
    public class OWLReasonerTest
    {
        #region Tests
        [TestMethod]
        public async Task ShouldEntailClassAssertionAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Feline"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ],
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Feline"))),
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Feline")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [ OWLEnums.OWLReasonerRules.ClassAssertionEntailment ] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 2);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLClassAssertionEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1
                            && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Feline")
                            && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        }

        [TestMethod]
        public async Task ShouldEntailDataPropertyDomainAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")))
                ],
                DataPropertyAxioms = [
                    new OWLDataPropertyDomain(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")),
                        new OWLLiteral(new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER))
                    )
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.DataPropertyDomainEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLDataPropertyDomainEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Kelly")));
        }

        [TestMethod]
        public async Task ShouldEntailDifferentIndividualsAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ],
                AssertionAxioms = [
                    new OWLDifferentIndividuals([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))
                    ])
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.DifferentIndividualsEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 6);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLDifferentIndividualsEntailmentRule.rulename)));
        }

        [TestMethod]
        public async Task ShouldEntailDisjointClassesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Fungine")))
                ],
                ClassAxioms = [
                    new OWLDisjointClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal"))]),
                    new OWLDisjointClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom"))]),
                    new OWLDisjointClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Vegetal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom"))]),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mushroom")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Fungine"))
                    ])
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.DisjointClassesEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 5);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLDisjointClassesEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointClasses inf
                            && string.Equals(inf.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal")
                            && string.Equals(inf.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointClasses inf1
                            && string.Equals(inf1.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mushroom")
                            && string.Equals(inf1.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointClasses inf2
                            && string.Equals(inf2.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mushroom")
                            && string.Equals(inf2.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointClasses inf3
                            && string.Equals(inf3.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fungine")
                            && string.Equals(inf3.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointClasses inf4
                            && string.Equals(inf4.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fungine")
                            && string.Equals(inf4.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Vegetal")));
        }

        [TestMethod]
        public async Task ShouldEntailDisjointDataPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasName"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")))
                ],
                DataPropertyAxioms = [
                    new OWLDisjointDataProperties([
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasName"))])
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.DisjointDataPropertiesEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLDisjointDataPropertiesEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointDataProperties inf
                            && string.Equals(inf.DataProperties[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasName")
                            && string.Equals(inf.DataProperties[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")));
        }

        [TestMethod]
        public async Task ShouldEntailDisjointObjectPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/avoids"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")))
                ],
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDisjointObjectProperties([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/avoids"))])
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.DisjointObjectPropertiesEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLDisjointObjectPropertiesEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDisjointObjectProperties inf
                            && string.Equals(inf.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/avoids")
                            && string.Equals(inf.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
        }

        [TestMethod]
        public async Task ShouldEntailEquivalentClassesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mankind"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Humans"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/EarthMan")))
                ],
                ClassAxioms = [
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mankind")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Humans"))]),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Humans")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/EarthMan"))])
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.EquivalentClassesEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 4);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLEquivalentClassesEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentClasses inf
                            && string.Equals(inf.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mankind")
                            && string.Equals(inf.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/EarthMan")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentClasses inf1
                            && string.Equals(inf1.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Humans")
                            && string.Equals(inf1.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mankind")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentClasses inf2
                            && string.Equals(inf2.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/EarthMan")
                            && string.Equals(inf2.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Humans")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentClasses inf3
                            && string.Equals(inf3.ClassExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/EarthMan")
                            && string.Equals(inf3.ClassExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mankind")));
        }

        [TestMethod]
        public async Task ShouldEntailEquivalentDataPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/isOld"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/wasBornNYearsAgo"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")))
                ],
                DataPropertyAxioms = [
                    new OWLEquivalentDataProperties([
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/wasBornNYearsAgo"))]),
                    new OWLEquivalentDataProperties([
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/wasBornNYearsAgo")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/isOld"))])
                ],
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                        new OWLLiteral(new RDFTypedLiteral("26", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.EquivalentDataPropertiesEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 6);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLEquivalentDataPropertiesEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentDataProperties inf
                            && string.Equals(inf.DataProperties[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isOld")
                            && string.Equals(inf.DataProperties[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/wasBornNYearsAgo")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentDataProperties inf1
                            && string.Equals(inf1.DataProperties[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isOld")
                            && string.Equals(inf1.DataProperties[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentDataProperties inf2
                            && string.Equals(inf2.DataProperties[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/wasBornNYearsAgo")
                            && string.Equals(inf2.DataProperties[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentDataProperties inf3
                            && string.Equals(inf3.DataProperties[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                            && string.Equals(inf3.DataProperties[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isOld")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf4
                            && string.Equals(inf4.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/wasBornNYearsAgo")
                            && string.Equals(inf4.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf4.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf5
                            && string.Equals(inf5.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isOld")
                            && string.Equals(inf5.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf5.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer")));
        }

        [TestMethod]
        public async Task ShouldEntailEquivalentObjectPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/supports"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ],
                ObjectPropertyAxioms = [
                    new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf"))]),
                    new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/takesCareOf")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/supports"))])
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/helps")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.EquivalentObjectPropertiesEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 6);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.Rule, OWLEquivalentObjectPropertiesEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf
                            && string.Equals(inf.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")
                            && string.Equals(inf.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2
                            && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf3
                            && string.Equals(inf3.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")
                            && string.Equals(inf3.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf4
                            && string.Equals(inf4.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf4.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/takesCareOf")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentObjectProperties inf5
                            && string.Equals(inf5.ObjectPropertyExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/supports")
                            && string.Equals(inf5.ObjectPropertyExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/helps")));
        }
        #endregion
    }
}