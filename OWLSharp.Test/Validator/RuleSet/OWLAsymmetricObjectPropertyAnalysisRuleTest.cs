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
public class OWLAsymmetricObjectPropertyAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeAsymmetricSymmetricObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                new OWLSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysisRule.rulesugg1)));
    }

    [TestMethod]
    public void ShouldAnalyzeSimpleAsymmetricObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertiesWithInverseObjectAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertiesWithInverseObjectPropertyCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeAsymmetricObjectPropertiesWithInverseObjectPropertyAndInverseObjectAssertionCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John")))
            ],
            ObjectPropertyAxioms = [
                new OWLAsymmetricObjectProperty(new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks"))),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/kicks")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLAsymmetricObjectPropertyAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLAsymmetricObjectPropertyAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLAsymmetricObjectPropertyAnalysisRule.rulesugg2)));
    }
    #endregion
}