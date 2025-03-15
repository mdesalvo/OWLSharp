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
public class OWLEquivalentObjectPropertiesAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeEquivalentObjectPropertiesSubObjectPropertyOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLEquivalentObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectProperty(new RDFResource("ex:knows")) ]),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectProperty(new RDFResource("ex:knows")))
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLEquivalentObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLEquivalentObjectPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLEquivalentObjectPropertiesAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeEquivalentObjectPropertiesDisjointObjectPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [
                new OWLEquivalentObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectProperty(new RDFResource("ex:knows")),
                    new OWLObjectProperty(new RDFResource("ex:knows2")) ]),
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                    new OWLObjectProperty(new RDFResource("ex:knows")) ])
            ],
            DeclarationAxioms = [
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows2")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLEquivalentObjectPropertiesAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLEquivalentObjectPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLEquivalentObjectPropertiesAnalysisRule.rulesugg)));
    }
    #endregion
}