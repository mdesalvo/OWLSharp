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
public class SKOSCloseOrExactMatchConceptAnalysisRuleTest
{
    #region Tests
    [TestMethod]
    public async Task ShouldAnalyzeCloseOrExactMatchConceptAndViolateRule1A()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
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
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
            ]
        };
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
        };
        List<OWLIssue> issues = await SKOSCloseOrExactMatchConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSCloseOrExactMatchConceptAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, SKOSCloseOrExactMatchConceptAnalysisRule.rulesugg1A));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on mapping/associative relations (skos:closeMatch VS skos:related)"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeCloseOrExactMatchConceptAndViolateRule1B()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
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
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
            ]
        };
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
        };
        List<OWLIssue> issues = await SKOSCloseOrExactMatchConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSCloseOrExactMatchConceptAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, SKOSCloseOrExactMatchConceptAnalysisRule.rulesugg1B));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on mapping/associative relations (skos:closeMatch VS skos:relatedMatch)"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeCloseOrExactMatchConceptAndViolateRule2A()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
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
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
            ]
        };
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
        };
        List<OWLIssue> issues = await SKOSCloseOrExactMatchConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSCloseOrExactMatchConceptAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, SKOSCloseOrExactMatchConceptAnalysisRule.rulesugg2A));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on mapping/associative relations (skos:exactMatch VS skos:related)"));
    }

    [TestMethod]
    public async Task ShouldAnalyzeCloseOrExactMatchConceptAndViolateRule2B()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH)),
                new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
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
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                    new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
            ]
        };
        Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
        {
            { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
        };
        List<OWLIssue> issues = await SKOSCloseOrExactMatchConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

        Assert.IsNotNull(issues);
        Assert.AreEqual(1, issues.Count);
        Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
        Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSCloseOrExactMatchConceptAnalysisRule.rulename));
        Assert.IsTrue(string.Equals(issues[0].Description, SKOSCloseOrExactMatchConceptAnalysisRule.rulesugg2B));
        Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on mapping/associative relations (skos:exactMatch VS skos:relatedMatch)"));
    }
    #endregion
}