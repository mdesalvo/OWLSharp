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
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator;

[TestClass]
public class OWLDataPropertyDomainAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeDataPropertyDomain()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cls1"))), //ex:Mark is explicitly incompatible with domain of ex:dp1
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Cls1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:dp1")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFPlainLiteral("lit2"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:dp2")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFPlainLiteral("lit"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:dp1")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLLiteral(new RDFPlainLiteral("lit"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:dp2")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLLiteral(new RDFPlainLiteral("lit"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:dp1")),
                    new OWLNamedIndividual(new RDFResource("ex:Helen")),
                    new OWLLiteral(new RDFPlainLiteral("lit")))
            ],
            DataPropertyAxioms = [
                new OWLDataPropertyDomain(
                    new OWLDataProperty(new RDFResource("ex:dp1")),
                    new OWLClass(new RDFResource("ex:Cls1")))
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls1"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls2"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLDataPropertyDomainAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyDomainAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyDomainAnalysisRule.rulesugg)));
    }
    #endregion
}