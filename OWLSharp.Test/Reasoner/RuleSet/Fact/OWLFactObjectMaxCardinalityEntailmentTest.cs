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

    // ── cls-maxqc3, cls-maxqc4 ────────────────────────────────────────────────

    [TestMethod]
    public void ShouldNotEntailSameIndividualFromQualifiedMaxCardinalityOneWhenFillerIsOutsideQualifyingClass()
    {
        //SubClassOf(D, ObjectMaxCardinality(1, propObj, Q)) ^ ClassAssertion(D, x)
        //^ ObjectPropertyAssertion(propObj, x, y1) ^ ClassAssertion(Q, y1)
        //^ ObjectPropertyAssertion(propObj, x, y2) [y2 is not a Q]
        //  -> no SameIndividual: only y1 is capped by the restriction
        OWLOntology ontology = BuildMaxCardinalityOntology(
            new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1, new OWLClass(new RDFResource("ex:Q"))),
            [("ex:x", "ex:y1"), ("ex:x", "ex:y2")],
            ["ex:y1"]);
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
    }

    [TestMethod]
    public void ShouldEntailSameIndividualFromQualifiedMaxCardinalityOneOnlyAmongQualifyingFillers()
    {
        //SubClassOf(D, ObjectMaxCardinality(1, propObj, Q)) ^ ClassAssertion(D, x)
        //^ ObjectPropertyAssertion(propObj, x, y1) ^ ClassAssertion(Q, y1)
        //^ ObjectPropertyAssertion(propObj, x, y2) ^ ClassAssertion(Q, y2)
        //^ ObjectPropertyAssertion(propObj, x, y3) [y3 is not a Q]
        //  -> SameIndividual(y1, y2), with y3 left out
        OWLOntology ontology = BuildMaxCardinalityOntology(
            new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1, new OWLClass(new RDFResource("ex:Q"))),
            [("ex:x", "ex:y1"), ("ex:x", "ex:y2"), ("ex:x", "ex:y3")],
            ["ex:y1", "ex:y2"]);
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(1, inferences.Count);
        Assert.IsTrue(inferences[0].Axiom is OWLSameIndividual sameIdv
                       && sameIdv.IndividualExpressions.Count == 2
                       && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y1"))
                       && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y2")));
    }

    [TestMethod]
    public void ShouldEntailSameIndividualFromMaxCardinalityOneQualifiedByThing()
    {
        //SubClassOf(D, ObjectMaxCardinality(1, propObj, owl:Thing)) caps every filler, as the unqualified restriction does
        OWLOntology ontology = BuildMaxCardinalityOntology(
            new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1, new OWLClass(RDFVocabulary.OWL.THING)),
            [("ex:x", "ex:y1"), ("ex:x", "ex:y2")],
            []);
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual sameIdv
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y1"))
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y2"))));
    }

    [TestMethod]
    public void ShouldEntailSameIndividualFromExactCardinalityOne()
    {
        //SubClassOf(D, ObjectExactCardinality(1, propObj)) caps the fillers at one, as ObjectMaxCardinality(1, propObj) does
        OWLOntology ontology = BuildMaxCardinalityOntology(
            new OWLObjectExactCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1),
            [("ex:x", "ex:y1"), ("ex:x", "ex:y2")],
            []);
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual sameIdv
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y1"))
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y2"))));
    }

    [TestMethod]
    public void ShouldNotEntailSameIndividualFromExactCardinalityTwo()
    {
        //SubClassOf(D, ObjectExactCardinality(2, propObj)) admits two distinct fillers
        OWLOntology ontology = BuildMaxCardinalityOntology(
            new OWLObjectExactCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 2),
            [("ex:x", "ex:y1"), ("ex:x", "ex:y2")],
            []);
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
    }

    [TestMethod]
    public void ShouldEntailSameIndividualFromQualifiedMaxCardinalityOneOnInverseProperty()
    {
        //SubClassOf(D, ObjectMaxCardinality(1, ObjectInverseOf(propObj), Q)) ^ ClassAssertion(D, x)
        //^ ObjectPropertyAssertion(propObj, y1, x) ^ ObjectPropertyAssertion(propObj, y2, x) [both Q]
        //  -> SameIndividual(y1, y2)
        OWLOntology ontology = BuildMaxCardinalityOntology(
            new OWLObjectMaxCardinality(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:propObj"))), 1, new OWLClass(new RDFResource("ex:Q"))),
            [("ex:y1", "ex:x"), ("ex:y2", "ex:x")],
            ["ex:y1", "ex:y2"]);
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.Any(inf => inf.Axiom is OWLSameIndividual sameIdv
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y1"))
                                            && sameIdv.IndividualExpressions.Any(ie => string.Equals(ie.GetIRI().ToString(), "ex:y2"))));
    }

    [TestMethod]
    public void ShouldNotEntailSameIndividualFromMaxCardinalityOneWhenFillerIsAssertedTwice()
    {
        //The same filler asserted twice is still a single filler: no SameIndividual(y1, y1)
        OWLOntology ontology = BuildMaxCardinalityOntology(
            new OWLObjectMaxCardinality(new OWLObjectProperty(new RDFResource("ex:propObj")), 1),
            [("ex:x", "ex:y1"), ("ex:x", "ex:y1")],
            []);
        List<OWLInference> inferences = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsFalse(inferences.Any(inf => inf.Axiom is OWLSameIndividual));
    }

    #endregion

    #region Utilities

    //Builds SubClassOf(D, restriction) with ex:x as the only D, the given propObj assertions and the given members of ex:Q
    private static OWLOntology BuildMaxCardinalityOntology(OWLClassExpression restriction, (string source, string target)[] propObjAsns, string[] qualifyingMembers)
        => new OWLOntology
        {
            ClassAxioms = [ new OWLSubClassOf(new OWLClass(new RDFResource("ex:D")), restriction) ],
            AssertionAxioms = [
                new OWLClassAssertion(new OWLClass(new RDFResource("ex:D")), new OWLNamedIndividual(new RDFResource("ex:x"))),
                ..propObjAsns.Select(asn => new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:propObj")),
                    new OWLNamedIndividual(new RDFResource(asn.source)),
                    new OWLNamedIndividual(new RDFResource(asn.target)))),
                ..qualifyingMembers.Select(member => new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Q")),
                    new OWLNamedIndividual(new RDFResource(member))))
            ]
        };

    #endregion
}
