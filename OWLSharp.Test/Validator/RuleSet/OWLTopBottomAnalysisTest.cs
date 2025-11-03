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
public class OWLTopBottomAnalysisTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeTopBottomT1Case()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY),
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            ]
        };
        List<OWLIssue> issues = OWLTopBottomAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected object property axioms causing reserved owl:topObjectProperty property to not be the root object property of the ontology")));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysis.rulesuggT1)));
    }

    [TestMethod]
    public void ShouldAnalyzeTopBottomT2Case()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.OWL.TOP_DATA_PROPERTY),
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.OWL.TOP_DATA_PROPERTY)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE))
            ]
        };
        List<OWLIssue> issues = OWLTopBottomAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected data property axioms causing reserved owl:topDataProperty property to not be the root data property of the ontology")));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysis.rulesuggT2)));
    }

    [TestMethod]
    public void ShouldAnalyzeTopBottomB1Case()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectProperty(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
            ]
        };
        List<OWLIssue> issues = OWLTopBottomAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected object property axioms causing reserved owl:bottomObjectProperty property to not be the bottom object property of the ontology")));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysis.rulesuggB1)));
    }

    [TestMethod]
    public void ShouldAnalyzeTopBottomB2Case()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLDataProperty(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE))
            ]
        };
        List<OWLIssue> issues = OWLTopBottomAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected data property axioms causing reserved owl:bottomDataProperty property to not be the bottom data property of the ontology")));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysis.rulesuggB2)));
    }

    [TestMethod]
    public void ShouldAnalyzeTopBottomB3Case()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY),
                    new OWLNamedIndividual(new RDFResource("ex:Idv1")),
                    new OWLNamedIndividual(new RDFResource("ex:Idv2")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Idv1"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Idv2"))),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY))
            ]
        };
        List<OWLIssue> issues = OWLTopBottomAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected object property assertion having owl:bottomObjectProperty as predicate: this is not allowed")));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysis.rulesuggB3)));
    }

    [TestMethod]
    public void ShouldAnalyzeTopBottomB4Case()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY),
                    new OWLNamedIndividual(new RDFResource("ex:Idv1")),
                    new OWLLiteral(new RDFPlainLiteral("hello")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Idv1"))),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY))
            ]
        };
        List<OWLIssue> issues = OWLTopBottomAnalysis.ExecuteRule(ontology);

        Assert.IsNotNull(issues);
        Assert.HasCount(1, issues);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysis.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected data property assertion having owl:bottomDataProperty as predicate: this is not allowed")));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysis.rulesuggB4)));
    }
    #endregion
}