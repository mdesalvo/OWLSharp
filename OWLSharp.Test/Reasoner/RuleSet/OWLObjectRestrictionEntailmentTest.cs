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
public class OWLObjectRestrictionEntailmentTest
{
    #region Tests

    // ── cls-svf1 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectSomeValuesFromMembershipCase()
    {
        //ObjectSomeValuesFrom(propObj, Target) ^ ObjectPropertyAssertion(propObj, x, t) ^ ClassAssertion(Target, t)
        //  -> ClassAssertion(ObjectSomeValuesFrom(propObj, Target), x)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Source"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Target"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:t")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Source")),
                    new OWLObjectSomeValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:propObj")),
                        new OWLClass(new RDFResource("ex:Target"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Target")), new OWLNamedIndividual(new RDFResource("ex:t"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:t")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && clsAsn.ClassExpression is OWLObjectSomeValuesFrom svf
                                            && string.Equals(svf.ObjectPropertyExpression.GetIRI().ToString(), "ex:propObj")
                                            && string.Equals(svf.ClassExpression.GetIRI().ToString(), "ex:Target")
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:x")));
    }

    [TestMethod]
    public void ShouldEntailObjectSomeValuesFromMembershipWithInversePropertyCase()
    {
        //ObjectSomeValuesFrom(inverse(propObj), Target) ^ ObjectPropertyAssertion(propObj, t, x) ^ ClassAssertion(Target, t)
        //  -> ClassAssertion(ObjectSomeValuesFrom(inverse(propObj), Target), x)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Source"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Target"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:t")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Source")),
                    new OWLObjectSomeValuesFrom(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                        new OWLClass(new RDFResource("ex:Target"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:Target")), new OWLNamedIndividual(new RDFResource("ex:t"))),
                //inverse(propObj)(x,t) = propObj(t,x): t is the source
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:t")),
                    new OWLNamedIndividual(new RDFResource("ex:x")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && clsAsn.ClassExpression is OWLObjectSomeValuesFrom svf
                                            && svf.ObjectPropertyExpression is OWLObjectInverseOf
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:x")));
    }

    [TestMethod]
    public void ShouldNotEntailObjectSomeValuesFromMembershipWhenFillerNotMatchCase()
    {
        //t is NOT an instance of Target -> no inference for x
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Source"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Target"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:t")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Source")),
                    new OWLObjectSomeValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:propObj")),
                        new OWLClass(new RDFResource("ex:Target"))))
            ],
            AssertionAxioms = [
                //t is NOT classified as Target
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:t")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                             && clsAsn.ClassExpression is OWLObjectSomeValuesFrom
                                             && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:x")));
    }

    // ── cls-avf ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectAllValuesFromPropagationCase()
    {
        //ClassAssertion(ObjectAllValuesFrom(propObj, Target), x) ^ ObjectPropertyAssertion(propObj, x, y)
        //  -> ClassAssertion(Target, y)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Restricted"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Target"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Restricted")),
                    new OWLObjectAllValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:propObj")),
                        new OWLClass(new RDFResource("ex:Target"))))
            ],
            AssertionAxioms = [
                //x is classified as ObjectAllValuesFrom(propObj, Target)
                new OWLClassAssertion(
                    new OWLObjectAllValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:propObj")),
                        new OWLClass(new RDFResource("ex:Target"))),
                    new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && string.Equals(clsAsn.ClassExpression.GetIRI().ToString(), "ex:Target")
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:y")));
    }

    [TestMethod]
    public void ShouldEntailObjectAllValuesFromPropagationWithInversePropertyCase()
    {
        //ClassAssertion(ObjectAllValuesFrom(inverse(propObj), Target), x) ^ ObjectPropertyAssertion(propObj, y, x)
        //  -> ClassAssertion(Target, y)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Restricted"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Target"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Restricted")),
                    new OWLObjectAllValuesFrom(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                        new OWLClass(new RDFResource("ex:Target"))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLObjectAllValuesFrom(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                        new OWLClass(new RDFResource("ex:Target"))),
                    new OWLNamedIndividual(new RDFResource("ex:x"))),
                //inverse(propObj)(x,y) = propObj(y,x): y is source, x is target
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:y")),
                    new OWLNamedIndividual(new RDFResource("ex:x")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && string.Equals(clsAsn.ClassExpression.GetIRI().ToString(), "ex:Target")
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:y")));
    }

    [TestMethod]
    public void ShouldNotEntailObjectAllValuesFromWhenIndividualNotClassifiedCase()
    {
        //x is NOT classified as ObjectAllValuesFrom -> no propagation to y
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Restricted"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Target"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Restricted")),
                    new OWLObjectAllValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:propObj")),
                        new OWLClass(new RDFResource("ex:Target"))))
            ],
            AssertionAxioms = [
                //x is NOT classified as ObjectAllValuesFrom(propObj, Target)
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                             && string.Equals(clsAsn.ClassExpression.GetIRI().ToString(), "ex:Target")
                                             && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:y")));
    }

    // ── cls-maxc2 ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSameIndividualFromMaxCardinalityOneCase()
    {
        //SubClassOf(D, ObjectMaxCardinality(1, propObj)) ^ ClassAssertion(D, x)
        //^ ObjectPropertyAssertion(propObj, x, y1) ^ ObjectPropertyAssertion(propObj, x, y2)
        //  -> SameIndividual(y1, y2)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:D"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y2")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:D")),
                    new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:D")), new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y2")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual sameIdv
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y1"))
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y2"))));
    }

    [TestMethod]
    public void ShouldNotEntailSameIndividualFromMaxCardinalityOneWhenOnlyOneFiller()
    {
        //Only one filler -> MaxCardinality(1) is satisfied, no SameIndividual inference
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:D"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y1")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:D")),
                    new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:D")), new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y1")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
    }

    // ── ObjectHasSelf reverse ──────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectHasSelfReverseCase()
    {
        //ObjectPropertyAssertion(propObj, x, x) ^ ObjectHasSelf(propObj) [referenced]
        //  -> ClassAssertion(ObjectHasSelf(propObj), x)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Narcissist"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:loves"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Narcissus"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Normal")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Narcissist")),
                    new OWLObjectHasSelf(new OWLObjectProperty(new RDFResource("ex:loves"))))
            ],
            AssertionAxioms = [
                //Narcissus loves himself -> should be classified as ObjectHasSelf(loves)
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:loves")),
                    new OWLNamedIndividual(new RDFResource("ex:Narcissus")),
                    new OWLNamedIndividual(new RDFResource("ex:Narcissus"))),
                //Normal loves Narcissus (not reflexive) -> no hasSelf inference for Normal
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:loves")),
                    new OWLNamedIndividual(new RDFResource("ex:Normal")),
                    new OWLNamedIndividual(new RDFResource("ex:Narcissus")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        //Narcissus classified as ObjectHasSelf(loves)
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && clsAsn.ClassExpression is OWLObjectHasSelf ohs
                                            && string.Equals(ohs.ObjectPropertyExpression.GetIRI().ToString(), "ex:loves")
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:Narcissus")));
        //Normal is NOT classified as ObjectHasSelf
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                             && clsAsn.ClassExpression is OWLObjectHasSelf
                                             && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:Normal")));
    }

    [TestMethod]
    public void ShouldEntailObjectHasSelfReverseWithInversePropertyCase()
    {
        //ObjectHasSelf(inverse(propObj)) is semantically P(x,x): same reflexive check
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:SelfReferential"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:SelfReferential")),
                    new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:knows")))))
            ],
            AssertionAxioms = [
                //knows(x,x) -> inverse(knows)(x,x) -> x in ObjectHasSelf(inverse(knows))
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:knows")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:x")))
            ]
        };
        List<OWLInference> inferences = OWLObjectRestrictionEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && clsAsn.ClassExpression is OWLObjectHasSelf ohs
                                            && ohs.ObjectPropertyExpression is OWLObjectInverseOf
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:x")));
    }

    #endregion
}
