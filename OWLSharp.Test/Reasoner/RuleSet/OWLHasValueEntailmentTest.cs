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
public class OWLHasValueEntailmentTest
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
        List<OWLInference> inferences = OWLHasValueEntailment.ExecuteRule(ontology);

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
        List<OWLInference> inferences = OWLHasValueEntailment.ExecuteRule(ontology);

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
        List<OWLInference> inferences = OWLHasValueEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyAssertion inf
                                          && string.Equals(inf.DataProperty.GetIRI().ToString(), "http://frede.gat/stuff#propData")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemDefinedByClassRestrictions")
                                          && string.Equals(inf.Literal.GetLiteral().ToString(), "44^^http://www.w3.org/2001/XMLSchema#integer")));
    }
    [TestMethod]
    public void ShouldEntailObjectHasValueReverseCase()
    {
        //cls-hv2: ObjectPropertyAssertion(P,x,v) ^ ObjectHasValue(P,v) [referenced] -> ClassAssertion(ObjectHasValue(P,v), x)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemUser")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                    new OWLObjectHasValue(
                        new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj")),
                        new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))))
            ],
            AssertionAxioms = [
                //ItemUser already has the property pointing to ItemAny: it must be classified as ObjectHasValue(propObj, ItemAny)
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemUser")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny")))
            ]
        };
        List<OWLInference> inferences = OWLHasValueEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && inf.ClassExpression is OWLObjectHasValue ohv
                                          && string.Equals(ohv.ObjectPropertyExpression.GetIRI().ToString(), "http://frede.gat/stuff#propObj")
                                          && string.Equals(ohv.IndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemUser")));
    }

    [TestMethod]
    public void ShouldEntailObjectHasValueReverseWithInverseObjectPropertyCase()
    {
        //cls-hv2 inverse: ObjectPropertyAssertion(P,v,x) ^ ObjectHasValue(inverse(P),v) [referenced] -> ClassAssertion(ObjectHasValue(inverse(P),v), x)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemUser")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                    new OWLObjectHasValue(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj"))),
                        new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny"))))
            ],
            AssertionAxioms = [
                //inverse(propObj)(ItemUser, ItemAny) means propObj(ItemAny, ItemUser): ItemAny is the source
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propObj")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemUser")))
            ]
        };
        List<OWLInference> inferences = OWLHasValueEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && inf.ClassExpression is OWLObjectHasValue ohv
                                          && ohv.ObjectPropertyExpression is OWLObjectInverseOf
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemUser")));
    }

    [TestMethod]
    public void ShouldEntailDataHasValueReverseCase()
    {
        //cls-hv2 data: DataPropertyAssertion(DP,x,lit) ^ DataHasValue(DP,lit) [referenced] -> ClassAssertion(DataHasValue(DP,lit), x)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://frede.gat/stuff#propData"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemUser")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassWithValueRestriction")),
                    new OWLDataHasValue(
                        new OWLDataProperty(new RDFResource("http://frede.gat/stuff#propData")),
                        new OWLLiteral(new RDFTypedLiteral("44", RDFModelEnums.RDFDatatypes.XSD_INTEGER))))
            ],
            AssertionAxioms = [
                //ItemUser already has the data property set to 44: it must be classified as DataHasValue(propData, 44)
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("http://frede.gat/stuff#propData")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemUser")),
                    new OWLLiteral(new RDFTypedLiteral("44", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
            ]
        };
        List<OWLInference> inferences = OWLHasValueEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion inf
                                          && inf.ClassExpression is OWLDataHasValue dhv
                                          && string.Equals(dhv.DataProperty.GetIRI().ToString(), "http://frede.gat/stuff#propData")
                                          && string.Equals(dhv.Literal.GetLiteral().ToString(), "44^^http://www.w3.org/2001/XMLSchema#integer")
                                          && string.Equals(inf.IndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemUser")));
    }
    #endregion
}