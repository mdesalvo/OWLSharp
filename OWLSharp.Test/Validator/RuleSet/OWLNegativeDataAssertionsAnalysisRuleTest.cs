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
public class OWLNegativeDataAssertionsAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public void ShouldAnalyzeNegativeDataAssertionsCase()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [ 
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")))
            ],
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Stiv")),
                    new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLNegativeDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("http://xmlns.com/foaf/0.1/Mark")),
                    new OWLLiteral(new RDFTypedLiteral("36", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))
            ]
        };
        OWLValidatorContext validatorContext = new OWLValidatorContext()
        {
            ClassAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>(),
            DataPropertyAssertions = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(),
            ObjectPropertyAssertions = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
        };
        List<OWLIssue> issues = OWLNegativeDataAssertionsAnalysisRule.ExecuteRule(ontology, validatorContext);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLNegativeDataAssertionsAnalysisRule.rulename)));
        Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLNegativeDataAssertionsAnalysisRule.rulesugg)));
    }
    #endregion
}