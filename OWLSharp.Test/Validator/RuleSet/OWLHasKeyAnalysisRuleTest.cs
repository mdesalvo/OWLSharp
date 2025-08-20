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
public class OWLHasKeyAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeObjectHasKeyCase()
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
                    [ new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/isFatherOf")) ],
                    null
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
                ),
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Henry")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener")) ]) //Will cause the validation error, since they conflict on computed key
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLHasKeyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLHasKeyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLHasKeyAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeDataHasKeyCase()
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
                    null,
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
                ),
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glen")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Glener")) ]) //Will cause the validation error, since they conflict on computed key
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLHasKeyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLHasKeyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLHasKeyAnalysisRule.rulesugg)));
    }
    #endregion
}