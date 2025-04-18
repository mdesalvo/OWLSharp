﻿/*
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
public class OWLObjectPropertyChainAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainAsymmetricCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainFunctionalCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainInverseFunctionalCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainIrreflexiveCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBroher"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeObjectPropertyChainLoopCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasBrother")))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectPropertyChain([
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasFather")),
                        new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle"))]),
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/hasUncle")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLObjectPropertyChainAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyChainAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyChainAnalysisRule.rulesugg2)));
    }
    #endregion
}