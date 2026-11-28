/*
  Copyright 2014-2026 Marco De Salvo
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
using OWLSharp.Reasoner;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Reasoner;

[TestClass]
public class OWLFactClassAssertionEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailClassAssertionOnSimpleSubClassOfCase()
    {
        OWLOntology ontology = new OWLOntology
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
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1
                                          && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Feline")
                                          && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
    }

    [TestMethod]
    public void ShouldEntailClassAssertionOnSimpleEquivalentClassesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))),
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
            ],
            ClassAxioms = [
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))]),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))])
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
            ]
        };
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MewAnimal")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1
                                          && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                                          && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
    }

    [TestMethod]
    public void ShouldEntailClassAssertionOnSubClassOfHavingEquivalentClassCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))),
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))])
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
            ]
        };
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MewAnimal")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1
                                          && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                                          && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
    }

    [TestMethod]
    public void ShouldEntailClassAssertionOnEquivalentClassHavingSubClassOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal"))),
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline"))),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MewAnimal")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat"))])
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")))
            ]
        };
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MewAnimal")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf1
                                          && string.Equals(inf1.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                                          && string.Equals(inf1.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
    }

    [TestMethod]
    public void ShouldEntailClassAssertionOnComplexTBOXCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Parrot")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Paco"))),
                new OWLSameIndividual([
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Paco")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/PaquitoTheParrot")) ])
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Parrot")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Animal")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/LivingEntity"))),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Cat")),
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/DomesticFeline")) ]),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/MyAnimals")),
                    new OWLObjectOneOf([
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Felix")),
                        new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Paco")) ]) ])
            ]
        };
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/DomesticFeline")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MyAnimals")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Felix")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Animal")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Paco")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/LivingEntity")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Paco")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && string.Equals(inf.ClassExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/MyAnimals")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Paco")));
    }

    [TestMethod]
    public void ShouldEntailClassAssertionOnIntersectionWithDataSomeValuesFromOperandCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Wine")), new OWLNamedIndividual(new RDFResource("ex:vinsanto"))),
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Wine")), new OWLNamedIndividual(new RDFResource("ex:rosso"))),
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Wine")), new OWLNamedIndividual(new RDFResource("ex:bianco"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:alcohol")),
                    new OWLNamedIndividual(new RDFResource("ex:vinsanto")),
                    new OWLLiteral(new RDFTypedLiteral("16.0", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:alcohol")),
                    new OWLNamedIndividual(new RDFResource("ex:bianco")),
                    new OWLLiteral(new RDFTypedLiteral("13.5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))
            ],
            ClassAxioms = [
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:StrongWine")),
                    new OWLObjectIntersectionOf([
                        new OWLClass(new RDFResource("ex:Wine")),
                        new OWLDataSomeValuesFrom(
                            new OWLDataProperty(new RDFResource("ex:alcohol")),
                            new OWLDatatypeRestriction(
                                new OWLDatatype(RDFVocabulary.XSD.DECIMAL),
                                [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("15", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MIN_EXCLUSIVE) ])) ]) ])
            ]
        };
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        List<OWLClassAssertion> strongWineAssertions = inferences.Select(inf => inf.Axiom).OfType<OWLClassAssertion>()
                                                                 .Where(asn => string.Equals(asn.ClassExpression.GetIRI().ToString(), "ex:StrongWine"))
                                                                 .ToList();
        Assert.HasCount(1, strongWineAssertions);
        Assert.AreEqual("ex:vinsanto", strongWineAssertions[0].IndividualExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldEntailClassAssertionOnAnonymousSubClassIntersectionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Wine"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:SweetWine"))) ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Wine")), new OWLNamedIndividual(new RDFResource("ex:vinsanto"))),
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Wine")), new OWLNamedIndividual(new RDFResource("ex:amarone"))),
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Wine")), new OWLNamedIndividual(new RDFResource("ex:rosso"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:alcohol")),
                    new OWLNamedIndividual(new RDFResource("ex:vinsanto")),
                    new OWLLiteral(new RDFTypedLiteral("16.0", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:alcohol")),
                    new OWLNamedIndividual(new RDFResource("ex:amarone")),
                    new OWLLiteral(new RDFTypedLiteral("15.0", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:alcohol")),
                    new OWLNamedIndividual(new RDFResource("ex:rosso")),
                    new OWLLiteral(new RDFTypedLiteral("16.5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:method")),
                    new OWLNamedIndividual(new RDFResource("ex:vinsanto")),
                    new OWLNamedIndividual(new RDFResource("ex:appassimento"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:method")),
                    new OWLNamedIndividual(new RDFResource("ex:amarone")),
                    new OWLNamedIndividual(new RDFResource("ex:appassimento")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLObjectIntersectionOf([
                        new OWLClass(new RDFResource("ex:Wine")),
                        new OWLDataSomeValuesFrom(
                            new OWLDataProperty(new RDFResource("ex:alcohol")),
                            new OWLDatatypeRestriction(
                                new OWLDatatype(RDFVocabulary.XSD.DECIMAL),
                                [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("15", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MIN_EXCLUSIVE) ])),
                        new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:method")), new OWLNamedIndividual(new RDFResource("ex:appassimento"))) ]),
                    new OWLClass(new RDFResource("ex:SweetWine")))
            ]
        };
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        List<OWLClassAssertion> sweetWineAssertions = inferences.Select(inf => inf.Axiom).OfType<OWLClassAssertion>()
                                                                .Where(asn => string.Equals(asn.ClassExpression.GetIRI().ToString(), "ex:SweetWine"))
                                                                .ToList();
        Assert.HasCount(1, sweetWineAssertions);
        Assert.AreEqual("ex:vinsanto", sweetWineAssertions[0].IndividualExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldEntailClassAssertionOnUndeclaredSuperClassOfAnonymousSubClassCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Wine")), new OWLNamedIndividual(new RDFResource("ex:vinsanto"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:method")),
                    new OWLNamedIndividual(new RDFResource("ex:vinsanto")),
                    new OWLNamedIndividual(new RDFResource("ex:appassimento")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLObjectIntersectionOf([
                        new OWLClass(new RDFResource("ex:Wine")),
                        new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:method")), new OWLNamedIndividual(new RDFResource("ex:appassimento"))) ]),
                    new OWLClass(new RDFResource("ex:SweetWine")))
            ]
        };
        List<OWLInference> inferences = OWLFactClassAssertionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        List<OWLClassAssertion> sweetWineAssertions = inferences.Select(inf => inf.Axiom).OfType<OWLClassAssertion>()
                                                                .Where(asn => string.Equals(asn.ClassExpression.GetIRI().ToString(), "ex:SweetWine"))
                                                                .ToList();
        Assert.HasCount(1, sweetWineAssertions);
        Assert.AreEqual("ex:vinsanto", sweetWineAssertions[0].IndividualExpression.GetIRI().ToString());
    }
    #endregion
}