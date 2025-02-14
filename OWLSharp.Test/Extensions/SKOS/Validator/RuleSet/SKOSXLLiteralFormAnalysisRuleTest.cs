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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.SKOS;
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.SKOS;

[TestClass]
public class SKOSXLLiteralFormAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public async Task ShouldAnalyzeLiteralFormAndViolateRule()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelB")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                    new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                    new OWLNamedIndividual(new RDFResource("ex:LabelB"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                    new OWLNamedIndividual(new RDFResource("ex:LabelA")),
                    new OWLLiteral(new RDFPlainLiteral("labelA1"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                    new OWLNamedIndividual(new RDFResource("ex:LabelA")),
                    new OWLLiteral(new RDFPlainLiteral("labelA2"))), //clash
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                    new OWLNamedIndividual(new RDFResource("ex:LabelB")),
                    new OWLLiteral(new RDFPlainLiteral("labelB", "en")))
            ]
        };
        List<OWLIssue> issues = await SKOSXLLiteralFormAnalysisRule.ExecuteRuleAsync(ontology);

        Assert.IsNotNull(issues);
        Assert.AreEqual(2, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSXLLiteralFormAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, SKOSXLLiteralFormAnalysisRule.rulesugg));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS-XL label 'ex:LabelA' should be adjusted to not have more than one occurrence of skosxl:literalForm values"));
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[1].Severity);
        Assert.IsTrue(string.Equals(issues[1].RuleName, SKOSXLLiteralFormAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[1].Description, SKOSXLLiteralFormAnalysisRule.rulesugg));
        Assert.IsTrue(string.Equals(issues[1].Suggestion, "SKOS-XL label 'ex:LabelA' should be adjusted to not have more than one occurrence of skosxl:literalForm values"));
    }
    #endregion
}