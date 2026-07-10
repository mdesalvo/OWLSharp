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
public class OWLFactObjectSomeValuesFromEntailmentTest
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
        List<OWLInference> inferences = OWLFactObjectSomeValuesFromEntailment.ExecuteRule(ontology);

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
        List<OWLInference> inferences = OWLFactObjectSomeValuesFromEntailment.ExecuteRule(ontology);

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
        List<OWLInference> inferences = OWLFactObjectSomeValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                             && clsAsn.ClassExpression is OWLObjectSomeValuesFrom
                                             && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:x")));
    }

    // ── cls-svf2 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectSomeValuesFromMembershipOnUnqualifiedOwlThingCase()
    {
        //ObjectSomeValuesFrom(propObj, owl:Thing) ^ ObjectPropertyAssertion(propObj, x, t) [t untyped]
        //  -> ClassAssertion(ObjectSomeValuesFrom(propObj, owl:Thing), x)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Source"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:t")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Source")),
                    new OWLObjectSomeValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:propObj")),
                        new OWLClass(RDFVocabulary.OWL.THING)))
            ],
            AssertionAxioms = [
                //t is NOT typed at all: cls-svf2 must not require any filler typing, unlike cls-svf1
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:t")))
            ]
        };
        List<OWLInference> inferences = OWLFactObjectSomeValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && clsAsn.ClassExpression is OWLObjectSomeValuesFrom svf
                                            && string.Equals(svf.ClassExpression.GetIRI().ToString(), RDFVocabulary.OWL.THING.ToString())
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:x")));
    }

    [TestMethod]
    public void ShouldNotEntailObjectSomeValuesFromMembershipOnUnqualifiedOwlThingBecauseNoPropertyAssertion()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Source"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Source")),
                    new OWLObjectSomeValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:propObj")),
                        new OWLClass(RDFVocabulary.OWL.THING)))
            ]
        };
        List<OWLInference> inferences = OWLFactObjectSomeValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn && clsAsn.ClassExpression is OWLObjectSomeValuesFrom));
    }

    #endregion
}
