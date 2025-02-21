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
public class OWLFunctionalDataPropertyAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeFunctionalDataProperty()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:dp1")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFPlainLiteral("lit"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:dp1")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFPlainLiteral("lit2"))), //clash with first data assertion
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
                    new OWLLiteral(new RDFPlainLiteral("lit")))
            ],
            DataPropertyAxioms = [
                new OWLFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:dp1"))),
                new OWLFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:dp2")))
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLFunctionalDataPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLFunctionalDataPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLFunctionalDataPropertyAnalysisRule.rulesugg)));
    }
    #endregion
}