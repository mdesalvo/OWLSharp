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
public class OWLDisjointObjectPropertiesAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeDisjointObjectPropertiesSimpleCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:Lenny"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))) //conflicts with foaf:knows
            ],
            ObjectPropertyAxioms = [
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.AGENT)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Lenny")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
        };
        List<OWLIssue> issues = OWLDisjointObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointObjectPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointObjectPropertiesAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointObjectPropertiesObjectInverseCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:Lenny"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))) //conflicts with foaf:knows
            ],
            ObjectPropertyAxioms = [
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.AGENT) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.AGENT)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Lenny")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
        };
        List<OWLIssue> issues = OWLDisjointObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointObjectPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointObjectPropertiesAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointObjectPropertiesSubObjectPropertyOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.NAME) ]),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.NAME))
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.NAME))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
        };
        List<OWLIssue> issues = OWLDisjointObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointObjectPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointObjectPropertiesAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointObjectPropertiesSuperObjectPropertyOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.NAME) ]),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(RDFVocabulary.FOAF.NAME), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.NAME))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
        };
        List<OWLIssue> issues = OWLDisjointObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointObjectPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointObjectPropertiesAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointObjectPropertiesEquivalentObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.NAME) ]),
                new OWLEquivalentObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
                    new OWLObjectProperty(RDFVocabulary.FOAF.NAME) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.NAME))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
        };
        List<OWLIssue> issues = OWLDisjointObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointObjectPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointObjectPropertiesAnalysisRule.rulesugg2)));
    }
    #endregion
}