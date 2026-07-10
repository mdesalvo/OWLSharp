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
public class OWLSchemaPropertyRangeEntailmentTest
{
    #region Tests

    // ── scm-rng1 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectPropertyRangeViaSubClassOfCase()
    {
        //ObjectPropertyRange(hasPet,Dog) ^ SubClassOf(Dog,Mammal) -> ObjectPropertyRange(hasPet,Mammal)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Mammal"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Dog")), new OWLClass(new RDFResource("ex:Mammal")))
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyRange(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Dog")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyRangeEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyRange opr
                                          && string.Equals(opr.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasPet")
                                          && string.Equals(opr.ClassExpression.GetIRI().ToString(), "ex:Mammal")));
    }

    // ── scm-rng2 ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailObjectPropertyRangeViaSubObjectPropertyOfCase()
    {
        //ObjectPropertyRange(hasPet,Dog) ^ SubObjectPropertyOf(hasFavouritePet,hasPet) -> ObjectPropertyRange(hasFavouritePet,Dog)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasFavouritePet")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:hasFavouritePet")), new OWLObjectProperty(new RDFResource("ex:hasPet"))),
                new OWLObjectPropertyRange(new OWLObjectProperty(new RDFResource("ex:hasPet")), new OWLClass(new RDFResource("ex:Dog")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyRangeEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLObjectPropertyRange opr
                                          && string.Equals(opr.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasFavouritePet")
                                          && string.Equals(opr.ClassExpression.GetIRI().ToString(), "ex:Dog")));
    }

    [TestMethod]
    public void ShouldEntailDataPropertyRangeViaSubDataPropertyOfCase()
    {
        //DataPropertyRange(hasAge,xsd:nonNegativeInteger) ^ SubDataPropertyOf(hasYearsOld,hasAge) -> DataPropertyRange(hasYearsOld,xsd:nonNegativeInteger)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasAge"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasYearsOld")))
            ],
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:hasYearsOld")), new OWLDataProperty(new RDFResource("ex:hasAge"))),
                new OWLDataPropertyRange(new OWLDataProperty(new RDFResource("ex:hasAge")), new OWLDatatype(RDFVocabulary.XSD.NON_NEGATIVE_INTEGER))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyRangeEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLDataPropertyRange dpr
                                          && string.Equals(dpr.DataProperty.GetIRI().ToString(), "ex:hasYearsOld")
                                          && string.Equals(dpr.DataRangeExpression.GetIRI().ToString(), RDFVocabulary.XSD.NON_NEGATIVE_INTEGER.ToString())));
    }

    [TestMethod]
    public void ShouldNotEntailPropertyRangeBecauseNoRangeAxioms()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasPet")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(new OWLClass(new RDFResource("ex:Dog")), new OWLClass(new RDFResource("ex:Mammal")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaPropertyRangeEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
