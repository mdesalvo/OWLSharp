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
public class OWLDisjointDataPropertiesAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeDisjointDataProperties()
    {
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFPlainLiteral("Mark"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                    new OWLNamedIndividual(new RDFResource("ex:John")),
                    new OWLLiteral(new RDFPlainLiteral("John"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:age")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))) //conflicts with foaf:age
            ],
            DataPropertyAxioms = [
                new OWLDisjointDataProperties([
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME), 
                    new OWLDataProperty(new RDFResource("ex:age")) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME)),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
            ]
        };
        Dictionary<string, object> validatorCache = new Dictionary<string, object>()
        {
            { "OPASN",  OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)}
        };
        List<OWLIssue> issues = OWLDisjointDataPropertiesAnalysisRule.ExecuteRule(ontology, validatorCache);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointDataPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointDataPropertiesAnalysisRule.rulesugg)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointDataPropertiesSubDataPropertyOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLDisjointDataProperties([
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME) ]),
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME))
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME))
            ]
        };
        Dictionary<string, object> validatorCache = new Dictionary<string, object>()
        {
            { "OPASN",  OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)}
        };
        List<OWLIssue> issues = OWLDisjointDataPropertiesAnalysisRule.ExecuteRule(ontology, validatorCache);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointDataPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointDataPropertiesAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointDataPropertiesSuperDataPropertyOfCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLDisjointDataProperties([
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME) ]),
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME), 
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE))
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME))
            ]
        };
        Dictionary<string, object> validatorCache = new Dictionary<string, object>()
        {
            { "OPASN",  OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)}
        };
        List<OWLIssue> issues = OWLDisjointDataPropertiesAnalysisRule.ExecuteRule(ontology, validatorCache);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointDataPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointDataPropertiesAnalysisRule.rulesugg2)));
    }

    [TestMethod]
    public void ShouldAnalyzeDisjointDataPropertiesEquivalentDataPropertiesCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLDisjointDataProperties([
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME) ]),
                new OWLEquivalentDataProperties([
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE), 
                    new OWLDataProperty(RDFVocabulary.FOAF.NAME) ])
            ],
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME))
            ]
        };
        Dictionary<string, object> validatorCache = new Dictionary<string, object>()
        {
            { "OPASN",  OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)}
        };
        List<OWLIssue> issues = OWLDisjointDataPropertiesAnalysisRule.ExecuteRule(ontology, validatorCache);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointDataPropertiesAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointDataPropertiesAnalysisRule.rulesugg2)));
    }
    #endregion
}