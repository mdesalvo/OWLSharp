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
public class OWLSchemaHasValueEntailmentTest
{
    #region Tests

    // ── scm-hv (ObjectHasValue) ──────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSubClassOfBetweenObjectHasValueRestrictionsCase()
    {
        //ObjectHasValue(hasCapital,Rome) [referenced] ^ ObjectHasValue(hasCity,Rome) [referenced] ^ SubObjectPropertyOf(hasCapital,hasCity)
        //  -> SubClassOf(ObjectHasValue(hasCapital,Rome),ObjectHasValue(hasCity,Rome))
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:HasRomeAsCapital"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:HasRomeAsCity"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasCapital"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasCity"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Rome")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:hasCapital")), new OWLObjectProperty(new RDFResource("ex:hasCity")))
            ],
            ClassAxioms = [
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:HasRomeAsCapital")),
                    new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:hasCapital")), new OWLNamedIndividual(new RDFResource("ex:Rome")))
                ]),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:HasRomeAsCity")),
                    new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:hasCity")), new OWLNamedIndividual(new RDFResource("ex:Rome")))
                ])
            ]
        };
        List<OWLInference> inferences = OWLSchemaHasValueEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && sco.SubClassExpression is OWLObjectHasValue subHv && string.Equals(subHv.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasCapital")
                                          && sco.SuperClassExpression is OWLObjectHasValue supHv && string.Equals(supHv.ObjectPropertyExpression.GetIRI().ToString(), "ex:hasCity")));
    }

    [TestMethod]
    public void ShouldNotEntailSubClassOfBetweenObjectHasValueRestrictionsBecauseDifferentValues()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:HasRomeAsCapital"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:HasMilanAsCity"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasCapital"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasCity"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Rome"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Milan")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:hasCapital")), new OWLObjectProperty(new RDFResource("ex:hasCity")))
            ],
            ClassAxioms = [
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:HasRomeAsCapital")),
                    new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:hasCapital")), new OWLNamedIndividual(new RDFResource("ex:Rome")))
                ]),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:HasMilanAsCity")),
                    new OWLObjectHasValue(new OWLObjectProperty(new RDFResource("ex:hasCity")), new OWLNamedIndividual(new RDFResource("ex:Milan")))
                ])
            ]
        };
        List<OWLInference> inferences = OWLSchemaHasValueEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }

    // ── scm-hv (DataHasValue) ─────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSubClassOfBetweenDataHasValueRestrictionsCase()
    {
        //DataHasValue(hasCountryCode,"IT") [referenced] ^ DataHasValue(hasCode,"IT") [referenced] ^ SubDataPropertyOf(hasCountryCode,hasCode)
        //  -> SubClassOf(DataHasValue(hasCountryCode,"IT"),DataHasValue(hasCode,"IT"))
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:HasItalyAsCountryCode"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:HasItalyAsCode"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasCountryCode"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasCode")))
            ],
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:hasCountryCode")), new OWLDataProperty(new RDFResource("ex:hasCode")))
            ],
            ClassAxioms = [
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:HasItalyAsCountryCode")),
                    new OWLDataHasValue(new OWLDataProperty(new RDFResource("ex:hasCountryCode")), new OWLLiteral(new RDFPlainLiteral("IT")))
                ]),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:HasItalyAsCode")),
                    new OWLDataHasValue(new OWLDataProperty(new RDFResource("ex:hasCode")), new OWLLiteral(new RDFPlainLiteral("IT")))
                ])
            ]
        };
        List<OWLInference> inferences = OWLSchemaHasValueEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && sco.SubClassExpression is OWLDataHasValue subHv && string.Equals(subHv.DataProperty.GetIRI().ToString(), "ex:hasCountryCode")
                                          && sco.SuperClassExpression is OWLDataHasValue supHv && string.Equals(supHv.DataProperty.GetIRI().ToString(), "ex:hasCode")));
    }
    #endregion
}
