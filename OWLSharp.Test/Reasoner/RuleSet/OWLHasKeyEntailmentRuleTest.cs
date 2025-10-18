/*
  Copyright 2014-2025 Marco De Salvo
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
public class OWLHasKeyEntailmentRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailObjectHasKeyCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Rose"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Abigail"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")))
            ],
            KeyAxioms = [
                new OWLHasKey(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
                    [ new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")) ]
                )
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))
                ),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))
                ),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Dad")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))
                ),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Rose"))
                ),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Helen"))
                ),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Abigail"))
                ),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Abigail"))
                )
            ]
        };
        OWLReasonerContext reasonerContext = new OWLReasonerContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLInference> inferences = OWLHasKeyEntailmentRule.ExecuteRule(ontology, reasonerContext);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                                          && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Glener")
                                          && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Henry")));
    }

    [TestMethod]
    public void ShouldEntailDataHasKeyCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")))
            ],
            KeyAxioms = [
                new OWLHasKey(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                    [ new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")) ]
                )
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen"))
                ),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener"))
                ),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry"))
                ),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen")),
                    new OWLLiteral(new RDFPlainLiteral("GLN1"))
                ),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                    new OWLLiteral(new RDFPlainLiteral("HNR1"))
                ),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFiscalCode")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener")),
                    new OWLLiteral(new RDFPlainLiteral("GLN1"))
                )
            ]
        };
        OWLReasonerContext reasonerContext = new OWLReasonerContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLInference> inferences = OWLHasKeyEntailmentRule.ExecuteRule(ontology, reasonerContext);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSameIndividual inf
                                          && string.Equals(inf.IndividualExpressions[0].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Glen")
                                          && string.Equals(inf.IndividualExpressions[1].GetIRI().ToString(), "http://xmlns.com/foaf/0.1/Glener")));
    }
    #endregion
}