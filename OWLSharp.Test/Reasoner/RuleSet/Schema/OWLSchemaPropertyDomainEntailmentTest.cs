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
public class OWLSchemaPropertyDomainEntailmentTest
{
    #region Tests

    // ── scm-dom1 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectPropertyDomainViaSubClassOfCase()
    {
        //ObjectPropertyDomain(hasPet,Person) ^ SubClassOf(Person,Mammal) -> ObjectPropertyDomain(hasPet,Mammal)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Person"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Mammal"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Person")), new OWLClass(new RDFResource("ex:Mammal")))
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyDomain(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Person")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyDomainEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyDomain opd
                                          && string.Equals(opd.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasPet")
                                          && string.Equals(opd.ClassExpression.GetIRI().ToString(), "ex:Mammal")));
    }

    [TestMethod]
    public void ShouldEntailDataPropertyDomainViaSubClassOfCase()
    {
        //DataPropertyDomain(hasAge,Person) ^ SubClassOf(Person,Mammal) -> DataPropertyDomain(hasAge,Mammal)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Person"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Mammal"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasAge")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Person")), new OWLClass(new RDFResource("ex:Mammal")))
            ],
            DataPropertyAxioms = [
                new OWLDataPropertyDomain(new OWLDataProperty(new RDFResource("ex:hasAge")), new OWLClass(new RDFResource("ex:Person")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyDomainEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyDomain dpd
                                          && string.Equals(dpd.DataProperty.GetIRI().ToString(), "ex:hasAge")
                                          && string.Equals(dpd.ClassExpression.GetIRI().ToString(), "ex:Mammal")));
    }

    // ── scm-dom2 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectPropertyDomainViaSubObjectPropertyOfCase()
    {
        //ObjectPropertyDomain(hasPet,Person) ^ SubObjectPropertyOf(hasDog,hasPet) -> ObjectPropertyDomain(hasDog,Person)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Person"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasDog")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:hasDog")), new OWLObjectProperty(new RDFResource("ex:hasPet"))),
                new OWLObjectPropertyDomain(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Person")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyDomainEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyDomain opd
                                          && string.Equals(opd.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasDog")
                                          && string.Equals(opd.ClassExpression.GetIRI().ToString(), "ex:Person")));
    }

    [TestMethod]
    public void ShouldEntailDataPropertyDomainViaSubDataPropertyOfCase()
    {
        //DataPropertyDomain(hasAge,Person) ^ SubDataPropertyOf(hasYearsOld,hasAge) -> DataPropertyDomain(hasYearsOld,Person)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Person"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasAge"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasYearsOld")))
            ],
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:hasYearsOld")), new OWLDataProperty(new RDFResource("ex:hasAge"))),
                new OWLDataPropertyDomain(new OWLDataProperty(new RDFResource("ex:hasAge")), new OWLClass(new RDFResource("ex:Person")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyDomainEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyDomain dpd
                                          && string.Equals(dpd.DataProperty.GetIRI().ToString(), "ex:hasYearsOld")
                                          && string.Equals(dpd.ClassExpression.GetIRI().ToString(), "ex:Person")));
    }

    [TestMethod]
    public void ShouldNotEntailPropertyDomainBecauseNoDomainAxioms()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Person"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Person")), new OWLClass(new RDFResource("ex:Mammal")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyDomainEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
