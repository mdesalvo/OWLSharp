﻿/*
  Copyright 2014-2025 Marco De Salvo
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
public class OWLFunctionalObjectPropertyEntailmentRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailSimpleObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
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
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLInference> inferences = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.AreEqual(1, inferences.Count);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                                          && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                                          && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")));
    }

    [TestMethod]
    public void ShouldEntailInverseOfFunctionalObjectPropertyCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLFunctionalObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLInference> inferences = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.AreEqual(1, inferences.Count);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                                          && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                                          && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
    }

    [TestMethod]
    public void ShouldEntailInverseOfObjectPropertyAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLFunctionalObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLInference> inferences = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.AreEqual(1, inferences.Count);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                                          && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                                          && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Stiv")));
    }

    [TestMethod]
    public void ShouldEntailInverseOfFunctionalPropertyAndInverseOfObjectPropertyAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLFunctionalObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ]
        };
        List<OWLInference> inferences = OWLFunctionalObjectPropertyEntailmentRule.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.AreEqual(1, inferences.Count);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                                          && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/John")
                                          && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Mark")));
    }
    #endregion
}