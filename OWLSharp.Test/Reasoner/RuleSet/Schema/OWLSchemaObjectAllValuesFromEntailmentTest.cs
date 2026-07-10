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
public class OWLSchemaObjectAllValuesFromEntailmentTest
{
    #region Tests

    // ── scm-avf1 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSubClassOfViaSameFillerPropertyCase()
    {
        //ObjectAllValuesFrom(hasPet,Dog) [referenced] ^ ObjectAllValuesFrom(hasPet,Mammal) [referenced] ^ SubClassOf(Dog,Mammal)
        //  -> SubClassOf(ObjectAllValuesFrom(hasPet,Dog),ObjectAllValuesFrom(hasPet,Mammal))
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:C2"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Mammal"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Dog")), new OWLClass(new RDFResource("ex:Mammal"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C1")),
                    new OWLObjectAllValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Dog")))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C2")),
                    new OWLObjectAllValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Mammal"))))
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectAllValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && sco.SubClassExpression is OWLObjectAllValuesFrom subAvf && string.Equals(subAvf.ClassExpression.GetIRI().ToString(), "ex:Dog")
                                          && sco.SuperClassExpression is OWLObjectAllValuesFrom supAvf && string.Equals(supAvf.ClassExpression.GetIRI().ToString(), "ex:Mammal")));
    }

    // ── scm-avf2 (inverted polarity) ─────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSubClassOfWithInvertedPolarityViaSubPropertyCase()
    {
        //ObjectAllValuesFrom(hasDog,Animal) [referenced, OP1=narrower] ^ ObjectAllValuesFrom(hasPet,Animal) [referenced, OP2=wider] ^ SubObjectPropertyOf(hasDog,hasPet)
        //  -> SubClassOf(ObjectAllValuesFrom(hasPet,Animal),ObjectAllValuesFrom(hasDog,Animal))  <-- note: wider-property restriction is the SUBCLASS
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
                    new OWLObjectAllValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasDog")), new OWLClass(new RDFResource("ex:Animal")))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C2")),
                    new OWLObjectAllValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Animal"))))
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectAllValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && sco.SubClassExpression is OWLObjectAllValuesFrom subAvf && string.Equals(subAvf.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasPet")
                                          && sco.SuperClassExpression is OWLObjectAllValuesFrom supAvf && string.Equals(supAvf.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasDog")));
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
                    new OWLObjectAllValuesFrom(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Dog")))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:C2")),
                    new OWLObjectAllValuesFrom(new OWLObjectProperty(new RDFResource("ex:hatesPet")), new OWLClass(new RDFResource("ex:Cat"))))
            ]
        };
        List<OWLInference> inferences = OWLSchemaObjectAllValuesFromEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
