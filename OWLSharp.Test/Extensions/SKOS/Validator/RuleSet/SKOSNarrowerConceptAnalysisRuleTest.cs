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

namespace OWLSharp.Test.Extensions.SKOS
{
    [TestClass]
    public class SKOSNarrowerConceptAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule1A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg1A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical relations (skos:narrower VS skos:broader)"));
        }

        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule1B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg1B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical relations (skos:narrowerTransitive VS skos:broaderTransitive)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule2A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg2A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/associative relations (skos:narrower VS skos:related)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule2B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg2B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/associative relations (skos:narrowerTransitive VS skos:related)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule3A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg3A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrower VS skos:narrowMatch)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule3B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg3B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrowerTransitive VS skos:narrowMatch)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule4A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg4A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrower VS skos:closeMatch)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule4B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg4B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrowerTransitive VS skos:closeMatch)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule5A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg5A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrower VS skos:exactMatch)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule5B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg5B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrowerTransitive VS skos:exactMatch)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule6A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg6A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrower VS skos:relatedMatch)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAndViolateRule6B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptScheme"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>()
            {
                { "CONCEPTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT)) }
            };
            List<OWLIssue> issues = await SKOSNarrowerConceptAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg6B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on hierarchical/mapping relations (skos:narrowerTransitive VS skos:relatedMatch)"));
        }
        #endregion
    }
}