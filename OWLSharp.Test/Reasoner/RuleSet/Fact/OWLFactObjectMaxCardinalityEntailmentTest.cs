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
public class OWLFactObjectMaxCardinalityEntailmentTest
{
    #region Tests

    // ── cls-maxc2 ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void ShouldEntailSameIndividualFromMaxCardinalityOneCase()
    {
        //SubClassOf(D, ObjectMaxCardinality(1, propObj)) ^ ClassAssertion(D, x)
        //^ ObjectPropertyAssertion(propObj, x, y1) ^ ObjectPropertyAssertion(propObj, x, y2)
        //  -> SameIndividual(y1, y2)
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:D"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y2")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:D")),
                    new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:D")), new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y1"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y2")))
            ]
        };
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual sameIdv
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y1"))
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y2"))));
    }

    [TestMethod]
    public void ShouldNotEntailSameIndividualFromMaxCardinalityOneWhenOnlyOneFiller()
    {
        //Only one filler -> MaxCardinality(1) is satisfied, no SameIndividual inference
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:D"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:propObj"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:y1")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:D")),
                    new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:D")), new OWLNamedIndividual(new RDFResource("ex:x"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource("ex:x")),
                    new OWLNamedIndividual(new RDFResource("ex:y1")))
            ]
        };
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
    }

    #endregion
}
