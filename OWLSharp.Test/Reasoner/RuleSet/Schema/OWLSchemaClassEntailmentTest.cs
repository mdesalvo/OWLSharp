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
public class OWLSchemaClassEntailmentTest
{
    #region Tests
    [TestMethod]
    public void ShouldEntailSchemaClassCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Animal")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaClassEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.IsTrue(inferences.TrueForAll(inf => inf.Axiom.IsInference));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco
                                          && string.Equals(sco.SubClassExpression.GetIRI().ToString(), "ex:Animal")
                                          && string.Equals(sco.SuperClassExpression.GetIRI().ToString(), "ex:Animal")));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLEquivalentClasses eqc
                                          && eqc.ClassExpressions.TrueForAll(cex => string.Equals(cex.GetIRI().ToString(), "ex:Animal"))));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco1
                                          && string.Equals(sco1.SubClassExpression.GetIRI().ToString(), "ex:Animal")
                                          && string.Equals(sco1.SuperClassExpression.GetIRI().ToString(), RDFVocabulary.OWL.THING.ToString())));
        Assert.IsTrue(inferences.Any(i => i.Axiom is OWLSubClassOf sco2
                                          && string.Equals(sco2.SubClassExpression.GetIRI().ToString(), RDFVocabulary.OWL.NOTHING.ToString())
                                          && string.Equals(sco2.SuperClassExpression.GetIRI().ToString(), "ex:Animal")));
    }

    [TestMethod]
    public void ShouldNotEntailSchemaClassBecauseNoDeclaredClasses()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Alice")))
            ]
        };
        List<OWLInference> inferences = OWLSchemaClassEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }

    [TestMethod]
    public void ShouldNotEntailSchemaClassSelfLoopOnOwlThingOrOwlNothing()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.OWL.THING)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.OWL.NOTHING))
            ]
        };
        List<OWLInference> inferences = OWLSchemaClassEntailment.ExecuteRule(ontology);

        Assert.IsNotNull(inferences);
        Assert.AreEqual(0, inferences.Count);
    }
    #endregion
}
