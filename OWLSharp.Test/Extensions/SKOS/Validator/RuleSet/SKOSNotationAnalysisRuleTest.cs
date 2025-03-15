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
public class SKOSNotationAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public async Task ShouldAnalyzeNotation()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.SKOS.NOTATION)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptC")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.SKOS.NOTATION),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLLiteral(new RDFTypedLiteral("C1N", RDFModelEnums.RDFDatatypes.XSD_STRING))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.SKOS.NOTATION),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB")),
                    new OWLLiteral(new RDFTypedLiteral("C1N", RDFModelEnums.RDFDatatypes.XSD_STRING))), //clash
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.SKOS.NOTATION),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptC")),
                    new OWLLiteral(new RDFTypedLiteral("C2N", RDFModelEnums.RDFDatatypes.XSD_STRING)))
            ]
        };
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
        };
        List<OWLIssue> issues = await SKOSNotationAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

        Assert.IsNotNull(issues);
        Assert.AreEqual(2, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNotationAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, SKOSNotationAnalysisRule.rulesugg));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on skos:Notation values"));
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[1].Severity);
        Assert.IsTrue(string.Equals(issues[1].RuleName, SKOSNotationAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[1].Description, SKOSNotationAnalysisRule.rulesugg));
        Assert.IsTrue(string.Equals(issues[1].Suggestion, "SKOS concepts 'ex:ConceptB' and 'ex:ConceptA' should be adjusted to not clash on skos:Notation values"));
    }
    #endregion
}