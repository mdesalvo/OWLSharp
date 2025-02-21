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
public class OWLSubDataPropertyOfAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeSubDataPropertyOfSubDataPropertyOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(new RDFResource("ex:age")) ),
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("ex:age")), 
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE))
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLSubDataPropertyOfAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(2, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubDataPropertyOfAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubDataPropertyOfAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubDataPropertyOfEquivalentDataPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(new RDFResource("ex:age")) ),
                new OWLEquivalentDataProperties([
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(new RDFResource("ex:age")) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLSubDataPropertyOfAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubDataPropertyOfAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubDataPropertyOfAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeSubDataPropertyOfDisjointDataPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(new RDFResource("ex:age")) ),
                new OWLDisjointDataProperties([
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(new RDFResource("ex:age")) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age")))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLSubDataPropertyOfAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubDataPropertyOfAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubDataPropertyOfAnalysisRule.rulesugg)));
    }
    #endregion
}