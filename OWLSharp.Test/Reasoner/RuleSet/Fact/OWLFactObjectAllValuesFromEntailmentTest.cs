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
public class OWLFactObjectAllValuesFromEntailmentTest
{
    #region Tests

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
        List<OWLInference> inferences = OWLFactObjectAllValuesFromEntailment.ExecuteRule(ontology);

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
        List<OWLInference> inferences = OWLFactObjectAllValuesFromEntailment.ExecuteRule(ontology);

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
        List<OWLInference> inferences = OWLFactObjectAllValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                             && string.Equals(clsAsn.ClassExpression.GetIRI().ToString(), "ex:Target")
                                             && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:y")));
    }

    #endregion
}
