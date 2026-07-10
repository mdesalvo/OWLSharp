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
public class OWLFactObjectOneOfEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailClassAssertionForEachMemberViaEquivalentClassesCase()
    {
        //EquivalentClasses(Weekday,ObjectOneOf(Monday,Tuesday)) -> ClassAssertion(Weekday,Monday) ^ ClassAssertion(Weekday,Tuesday)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Weekday"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Monday"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Tuesday")))
            ],
            ClassAxioms = [
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:Weekday")),
                    new OWLObjectOneOf([
                        new OWLNamedIndividual(new RDFResource("ex:Monday")),
                        new OWLNamedIndividual(new RDFResource("ex:Tuesday"))
                    ])
                ])
            ]
        };
        List<OWLInference> inferences = OWLFactObjectOneOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion clsAsn
                                          && clsAsn.ClassExpression is OWLObjectOneOf
                                          && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:Monday")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion clsAsn1
                                          && clsAsn1.ClassExpression is OWLObjectOneOf
                                          && string.Equals(clsAsn1.IndividualExpression.GetIRI().ToString(), "ex:Tuesday")));
    }

    [TestMethod]
    public void ShouldEntailClassAssertionForEachMemberViaAnonymousRestrictionFillerCase()
    {
        //SubClassOf(C, ObjectSomeValuesFrom(hasDay, ObjectOneOf(Monday,Tuesday))) [anonymous oneOf nested in a restriction filler]
        //  -> ClassAssertion(ObjectOneOf(Monday,Tuesday),Monday) ^ ClassAssertion(ObjectOneOf(Monday,Tuesday),Tuesday)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasDay"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Monday"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Tuesday")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLObjectSomeValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:hasDay")),
                        new OWLObjectOneOf([
                            new OWLNamedIndividual(new RDFResource("ex:Monday")),
                            new OWLNamedIndividual(new RDFResource("ex:Tuesday"))
                        ])))
            ]
        };
        List<OWLInference> inferences = OWLFactObjectOneOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.AreEqual(2, inferences.Count);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion clsAsn
                                          && string.Equals(clsAsn.IndividualExpression.GetIRI().ToString(), "ex:Monday")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLClassAssertion clsAsn1
                                          && string.Equals(clsAsn1.IndividualExpression.GetIRI().ToString(), "ex:Tuesday")));
    }

    [TestMethod]
    public void ShouldNotEntailClassAssertionBecauseNoObjectOneOfReferenced()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Mammal")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Dog")), new OWLClass(new RDFResource("ex:Mammal")))
            ]
        };
        List<OWLInference> inferences = OWLFactObjectOneOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
