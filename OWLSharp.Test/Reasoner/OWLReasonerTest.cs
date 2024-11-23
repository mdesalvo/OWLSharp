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
using OWLSharp.Ontology.Rules;
using OWLSharp.Reasoner;
using OWLSharp.Reasoner.RuleSet;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
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
            OWLReasoner reasoner = new OWLReasoner() { Rules = [ OWLEnums.OWLReasonerRules.ClassAssertionEntailment, OWLEnums.OWLReasonerRules.ClassAssertionEntailment ] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 2);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLClassAssertionEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLDataPropertyDomainEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLDifferentIndividualsEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLDisjointClassesEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLDisjointDataPropertiesEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLDisjointObjectPropertiesEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLEquivalentClassesEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLEquivalentDataPropertiesEntailmentRule.rulename)));
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
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLEquivalentObjectPropertiesEntailmentRule.rulename)));
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

        [TestMethod]
        public async Task ShouldEntailFunctionalObjectPropertyAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLFunctionalObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLFunctionalObjectPropertyEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                            && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")));
        }

        [TestMethod]
        public async Task ShouldEntailHasKeyAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
                ],
                KeyAxioms = [
                    new OWLHasKey(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                        null,
                        [ new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")) ]
                    )
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))
                    ),
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))
                    ),
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))
                    ),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen")),
                        new OWLLiteral(new RDFPlainLiteral("GLN1"))
                    ),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                        new OWLLiteral(new RDFPlainLiteral("HNR1"))
                    ),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener")),
                        new OWLLiteral(new RDFPlainLiteral("GLN1"))
                    )
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.HasKeyEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLHasKeyEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                            && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Glen")
                            && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Glener")));
        }

        [TestMethod]
        public async Task ShouldEntailHasSelfAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassSelf"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propHas"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))),
                ],
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://frede.gat/stuff#ClassSelf")),
                        new OWLObjectHasSelf(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propHas"))))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://frede.gat/stuff#ClassSelf")),
                        new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny")))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.HasSelfEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLHasSelfEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://frede.gat/stuff#propHas")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")));
        }

        [TestMethod]
        public async Task ShouldEntailHasValueAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://frede.gat/stuff#propData"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions"))),
                ],
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                        new OWLDataHasValue(
                            new OWLDataProperty(new RDFResource("http://frede.gat/stuff#propData")),
                            new OWLLiteral(new RDFTypedLiteral("44", RDFModelEnums.RDFDatatypes.XSD_INTEGER))))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                        new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions")))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.HasValueEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLHasValueEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf
                            && string.Equals(inf.DataProperty.GetIRI().ToString(), "http://frede.gat/stuff#propData")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemDefinedByClassRestrictions")
                            && string.Equals(inf.Literal.GetLiteral().ToString(), "44^^http://www.w3.org/2001/XMLSchema#integer")));
        }

        [TestMethod]
        public async Task ShouldEntailInverseFunctionalObjectPropertyAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.InverseFunctionalObjectPropertyEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLInverseFunctionalObjectPropertyEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                            && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")
                            && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")));
        }

        [TestMethod]
        public async Task ShouldEntailInverseObjectPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLInverseObjectProperties(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                                                   new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isKnownBy")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 2);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLInverseObjectPropertiesEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/isKnownBy")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public async Task ShouldEntailObjectPropertyChainAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectPropertyChain([
                            new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                            new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")) ]),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Aebe")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jish")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Fritz"))),
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.ObjectPropertyChainEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 2);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLObjectPropertyChainEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Aebe")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasUncle")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jish")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Fritz")));
        }

        [TestMethod]
        public async Task ShouldEntailObjectPropertyDomainAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine")))
                ],
                ObjectPropertyAxioms = [
                    new OWLObjectPropertyDomain(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine"))
                    )
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.ObjectPropertyDomainEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLObjectPropertyDomainEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Kelly")));
        }

        [TestMethod]
        public async Task ShouldEntailObjectPropertyRangeAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine")))
                ],
                ObjectPropertyAxioms = [
                    new OWLObjectPropertyRange(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Kelly")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Janine"))
                    )
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.ObjectPropertyRangeEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLObjectPropertyRangeEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                            && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Janine")));
        }

        [TestMethod]
        public async Task ShouldEntailReflexiveObjectPropertyAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.ReflexiveObjectPropertyEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 2);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLReflexiveObjectPropertyEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public async Task ShouldEntailSameIndividualAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                AssertionAxioms = [
                    new OWLSameIndividual([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Marco"))
                    ]),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.SameIndividualEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 2);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLSameIndividualEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
            Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLObjectPropertyAssertion opAsn
                                                    && string.Equals(opAsn.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                                                    && string.Equals(opAsn.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Marco")
                                                    && string.Equals(opAsn.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public async Task ShouldEntailSubClassOfAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammifero"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")))
                ],
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Human")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal"))),
                    new OWLSubClassOf(
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                    new OWLEquivalentClasses([
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammal")),
                        new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Mammifero"))
                    ])
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.SubClassOfEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 3);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLSubClassOfEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf
                            && string.Equals(inf.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mammifero")
                            && string.Equals(inf.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf1
                            && string.Equals(inf1.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf1.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf inf2
                            && string.Equals(inf2.SubClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Human")
                            && string.Equals(inf2.SuperClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mammifero")));
        }

        [TestMethod]
        public async Task ShouldEntailSubDataPropertyOfAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasCharacteristic"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTemporalCharacteristic"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")))
                ],
                DataPropertyAxioms = [
                    new OWLSubDataPropertyOf(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasCharacteristic"))),
                    new OWLSubDataPropertyOf(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))),
                    new OWLEquivalentDataProperties([
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")),
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTimeCharacteristic"))])
                ],
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasAge")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                        new OWLLiteral(new RDFTypedLiteral("26", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.SubDataPropertyOfEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 6);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLSubDataPropertyOfEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubDataPropertyOf inf
                            && string.Equals(inf.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")
                            && string.Equals(inf.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubDataPropertyOf inf1
                            && string.Equals(inf1.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                            && string.Equals(inf1.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubDataPropertyOf inf2
                            && string.Equals(inf2.SubDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasAge")
                            && string.Equals(inf2.SuperDataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf3
                            && string.Equals(inf3.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTimeCharacteristic")
                            && string.Equals(inf3.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf3.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf4
                            && string.Equals(inf4.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasCharacteristic")
                            && string.Equals(inf4.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf4.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf5
                            && string.Equals(inf5.DataProperty.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTemporalCharacteristic")
                            && string.Equals(inf5.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf5.Literal.GetLiteral().ToString(), "26^^http://www.w3.org/2001/XMLSchema#integer")));
        }

        [TestMethod]
        public async Task ShouldEntailSubObjectPropertyOfAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTopFriend"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ],
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFriend"))),
                    new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasTopFriend"))])
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBestFriend")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jessie")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Olly")))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.SubObjectPropertyOfEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 5);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLSubObjectPropertyOfEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubObjectPropertyOf inf
                            && string.Equals(inf.SubObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasBestFriend")
                            && string.Equals(inf.SuperObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasFriend")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf2
                            && string.Equals(inf2.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf2.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Jessie")
                            && string.Equals(inf2.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Olly")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubObjectPropertyOf inf3
                            && string.Equals(inf3.SubObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTopFriend")
                            && string.Equals(inf3.SuperObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasFriend")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubObjectPropertyOf inf4
                            && string.Equals(inf4.SubObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/hasTopFriend")
                            && string.Equals(inf4.SuperObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")));
        }

        [TestMethod]
        public async Task ShouldEntailSymmetricObjectPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.SymmetricObjectPropertyEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 2);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLSymmetricObjectPropertyEntailmentRule.rulename)));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                            && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                            && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
            Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf1
                            && string.Equals(inf1.ObjectPropertyExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/knows")
                            && string.Equals(inf1.SourceIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")
                            && string.Equals(inf1.TargetIndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")));
        }

        [TestMethod]
        public async Task ShouldEntailTransitiveObjectPropertiesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jenny"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Helen")))
                ],
                ObjectPropertyAxioms = [
                    new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jenny"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Jenny")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Helen")))
                ]
            };
            OWLReasoner reasoner = new OWLReasoner() { Rules = [OWLEnums.OWLReasonerRules.TransitiveObjectPropertyEntailment] };
            List<OWLInference> inferences = await reasoner.ApplyToOntologyAsync(ontology);

            Assert.IsTrue(inferences.Count == 6);
            Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
            Assert.IsTrue(inferences.TrueForAll(inf => string.Equals(inf.RuleName, OWLTransitiveObjectPropertyEntailmentRule.rulename)));
        }

        [TestMethod]
        public async Task ShouldEntailOntologyRulesAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
                Rules = [
                    new SWRLRule(
                        new RDFPlainLiteral("SWRL1"),
                        new RDFPlainLiteral("This is a test SWRL rule"),
                        new SWRLAntecedent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                                    new SWRLVariableArgument(new RDFVariable("?P")))
                            ]
                        },
                        new SWRLConsequent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                                    new SWRLVariableArgument(new RDFVariable("?P")))
                            ]
                        })
                ]
            };
            List<OWLInference> inferences = await new OWLReasoner().ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0].Axiom is OWLClassAssertion clsAsnInf
                            && clsAsnInf.ClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.AGENT)
                            && clsAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark")));
        }

        [TestMethod]
        public async Task ShouldEntailOntologyRulesHavingCustomRegisteredBuiltInsAsync()
        {
            bool Evaluator(DataRow datarow) => string.Equals(datarow["?P"].ToString(), "ex:Mark");
            SWRLBuiltIn builtIn = new SWRLBuiltIn(
                Evaluator,
                new RDFResource("http://www.w3.org/2003/11/swrl#exampleRegistered"),
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit")));
            SWRLBuiltIn builtInExec = new SWRLBuiltIn(
                Evaluator,
                new RDFResource("http://www.w3.org/2003/11/swrl#exampleRegistered"),
                new SWRLVariableArgument(new RDFVariable("?P")));
			SWRLBuiltInRegister.AddBuiltIn(builtIn);

            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
                Rules = [
                    new SWRLRule(
                        new RDFPlainLiteral("SWRL1"),
                        new RDFPlainLiteral("This is a test SWRL rule"),
                        new SWRLAntecedent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                                    new SWRLVariableArgument(new RDFVariable("?P")))
                            ],
                            BuiltIns = [ builtInExec ]
                        },
                        new SWRLConsequent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                                    new SWRLVariableArgument(new RDFVariable("?P")))
                            ]
                        })
                ]
            };
            List<OWLInference> inferences = await new OWLReasoner().ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0].Axiom is OWLClassAssertion clsAsnInf
                            && clsAsnInf.ClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.AGENT)
                            && clsAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark")));
        }
        #endregion
    }
}