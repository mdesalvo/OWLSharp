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
public class OWLFactHasSelfEntailmentTest
{
    #region Tests

    // ── Forward: SubClassOf(C,ObjectHasSelf(OP)) ^ ClassAssertion(C,I) -> ObjectPropertyAssertion(OP,I,I) ──────

    [TestMethod]
    public void ShouldEntailObjectHasSelfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassSelf"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propHas"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny")))
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
        List<OWLInference> inferences = OWLFactHasSelfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                                          && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://frede.gat/stuff#propHas")
                                          && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")
                                          && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")));
    }

    [TestMethod]
    public void ShouldEntailObjectHasSelfWithInverseObjectHasSelfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://frede.gat/stuff#ClassSelf"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propHas"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassSelf")),
                    new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://frede.gat/stuff#propHas")))))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://frede.gat/stuff#ClassSelf")),
                    new OWLNamedIndividual(new RDFResource("http://frede.gat/stuff#ItemAny")))
            ]
        };
        List<OWLInference> inferences = OWLFactHasSelfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyAssertion inf
                                          && string.Equals(inf.ObjectPropertyExpression.GetIRI().ToString(), "http://frede.gat/stuff#propHas")
                                          && string.Equals(inf.SourceIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")
                                          && string.Equals(inf.TargetIndividualExpression.GetIRI().ToString(), "http://frede.gat/stuff#ItemAny")));
    }

    // ── Reverse: ObjectPropertyAssertion(OP,I,I) ^ [ObjectHasSelf(OP) referenced] -> ClassAssertion(ObjectHasSelf(OP),I) ──────

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
        List<OWLInference> inferences = OWLFactHasSelfEntailment.ExecuteRule(ontology);

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
        List<OWLInference> inferences = OWLFactHasSelfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLClassAssertion clsAsn
                                            && clsAsn.ClassExpression is OWLObjectHasSelf ohs
                                            && ohs.ObjectPropertyExpression is OWLObjectInverseOf
                                            && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:x")));
    }

    #endregion
}
