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
public class OWLSchemaObjectSomeValuesFromEntailmentTest
{
    #region Tests

    // ── scm-svf1 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSubClassOfViaSameFillerPropertyCase()
    {
        //ObjectSomeValuesFrom(hasPet,Dog) [referenced] ^ ObjectSomeValuesFrom(hasPet,Mammal) [referenced] ^ SubClassOf(Dog,Mammal)
        //  -> SubClassOf(ObjectSomeValuesFrom(hasPet,Dog),ObjectSomeValuesFrom(hasPet,Mammal))
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:DogOwner"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:MammalOwner"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Mammal"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Dog")), new OWLClass(new RDFResource("ex:Mammal"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:DogOwner")),
                    new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Dog")))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:MammalOwner")),
                    new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Mammal"))))
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectSomeValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && sco.SubClassExpression is OWLObjectSomeValuesFrom subSvf && string.Equals(subSvf.ClassExpression.GetIRI().ToString(), "ex:Dog")
                                          && sco.SuperClassExpression is OWLObjectSomeValuesFrom supSvf && string.Equals(supSvf.ClassExpression.GetIRI().ToString(), "ex:Mammal")));
    }

    // ── scm-svf2 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSubClassOfViaSameFillerSubPropertyCase()
    {
        //ObjectSomeValuesFrom(hasDog,Animal) [referenced] ^ ObjectSomeValuesFrom(hasPet,Animal) [referenced] ^ SubObjectPropertyOf(hasDog,hasPet)
        //  -> SubClassOf(ObjectSomeValuesFrom(hasDog,Animal),ObjectSomeValuesFrom(hasPet,Animal))
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Animal"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasDog"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:hasDog")), new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C1")),
                    new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasDog")), new OWLClass(new RDFResource("ex:Animal")))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C2")),
                    new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Animal"))))
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectSomeValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && sco.SubClassExpression is OWLObjectSomeValuesFrom subSvf && string.Equals(subSvf.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasDog")
                                          && sco.SuperClassExpression is OWLObjectSomeValuesFrom supSvf && string.Equals(supSvf.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasPet")));
    }

    [TestMethod]
    public void ShouldNotEntailSubClassOfBecauseUnrelatedFillersAndProperties()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Cat"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hatesPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C1")),
                    new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Dog")))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C2")),
                    new OWLObjectSomeValuesFrom(new OWLObjectProperty(new RDFResource("ex:hatesPet")), new OWLClass(new RDFResource("ex:Cat"))))
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectSomeValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
