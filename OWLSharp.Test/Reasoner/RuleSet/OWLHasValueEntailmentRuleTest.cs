/*
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
public class OWLHasValueEntailmentRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailObjectHasValueCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                    new OWLObjectHasValue(
                        new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj")),
                        new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions")))
            ]
        };
        OWLReasonerContext reasonerContext = new OWLReasonerContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLInference> inferences = OWLHasValueEntailmentRule.ExecuteRule(ontology, reasonerContext);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                                          && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://frede.gat/stuff#propObj")
                                          && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemDefinedByClassRestrictions")
                                          && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")));
    }

    [TestMethod]
    public void ShouldEntailObjectHasValueWithInverseObjectHasValueCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                    new OWLObjectHasValue(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj"))),
                        new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions")))
            ]
        };
        OWLReasonerContext reasonerContext = new OWLReasonerContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLInference> inferences = OWLHasValueEntailmentRule.ExecuteRule(ontology, reasonerContext);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                                          && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://frede.gat/stuff#propObj")
                                          && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")
                                          && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemDefinedByClassRestrictions")));
    }

    [TestMethod]
    public void ShouldEntailDataHasValueCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://frede.gat/stuff#propData"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemDefinedByClassRestrictions")))
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
        OWLReasonerContext reasonerContext = new OWLReasonerContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLInference> inferences = OWLHasValueEntailmentRule.ExecuteRule(ontology, reasonerContext);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf
                                          && string.Equals(inf.DataProperty.GetIRI().ToString(), "http://frede.gat/stuff#propData")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemDefinedByClassRestrictions")
                                          && string.Equals(inf.Literal.GetLiteral().ToString(), "44^^http://www.w3.org/2001/XMLSchema#integer")));
    }
    #endregion
}