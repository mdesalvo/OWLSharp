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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.SKOS;
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.SKOS
{
    [TestClass]
    public class SKOSRelatedConceptAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule1A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg1A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS hierarchical relations (skos:related VS skos:broader)"));
        }

        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule1B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROADER)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg1B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS hierarchical relations (skos:relatedMatch VS skos:broader)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule2A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg2A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS hierarchical relations (skos:related VS skos:narrower)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule2B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg2B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:narrower)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule3A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg3A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:broadMatch)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule3B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg3B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:broadMatch)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule4A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg4A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:narrowMatch)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule4B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg4B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:narrowMatch)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule5A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg5A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:closeMatch)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule5B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg5B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:closeMatch)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule6A()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg6A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:related VS skos:exactMatch)"));
        }
        
        [TestMethod]
        public void ShouldAnalyzeRelatedConceptAndViolateRule6B()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH)),
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            List<OWLIssue> issues = SKOSRelatedConceptAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSRelatedConceptAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, SKOSRelatedConceptAnalysisRule.rulesugg6B));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' should be adjusted to not clash on associative VS mapping relations (skos:relatedMatch VS skos:exactMatch)"));
        }
        #endregion
    }
}