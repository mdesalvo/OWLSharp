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
public class OWLSchemaObjectUnionOfEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailSubClassOfForEachUnionMemberCase()
    {
        //EquivalentClasses(Pet,ObjectUnionOf(Dog,Cat)) -> SubClassOf(Dog,Pet) ^ SubClassOf(Cat,Pet)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Pet"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Cat")))
            ],
            ClassAxioms = [
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:Pet")),
                    new OWLObjectUnionOf([
                        new OWLClass(new RDFResource("ex:Dog")),
                        new OWLClass(new RDFResource("ex:Cat"))
                    ])
                ])
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectUnionOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && string.Equals(sco.SubClassExpression.GetIRI().ToString(), "ex:Dog")
                                          && sco.SuperClassExpression is OWLObjectUnionOf));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco1
                                          && string.Equals(sco1.SubClassExpression.GetIRI().ToString(), "ex:Cat")
                                          && sco1.SuperClassExpression is OWLObjectUnionOf));
    }

    [TestMethod]
    public void ShouldNotEntailSubClassOfForUnionNestedInsideRestrictionFillerCase()
    {
        //SubClassOf(C, ObjectSomeValuesFrom(hasPet, ObjectUnionOf(Dog,Cat))) -- the union here is nested one level inside the SVF filler,
        //not directly a SubClassOf/EquivalentClasses operand, so this rule (unlike ObjectOneOfEntailment) does NOT look one level deep
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Cat"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C")),
                    new OWLObjectSomeValuesFrom(
                        new OWLObjectProperty(new RDFResource("ex:hasPet")),
                        new OWLObjectUnionOf([
                            new OWLClass(new RDFResource("ex:Dog")),
                            new OWLClass(new RDFResource("ex:Cat"))
                        ])))
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectUnionOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count); //the union here is nested inside an ObjectSomeValuesFrom, not directly a SubClassOf super/sub expression
    }

    [TestMethod]
    public void ShouldNotEntailSubClassOfBecauseNoUnionOfReferenced()
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
        List<OWLInference> inferences = OWLSchemaObjectUnionOfEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
