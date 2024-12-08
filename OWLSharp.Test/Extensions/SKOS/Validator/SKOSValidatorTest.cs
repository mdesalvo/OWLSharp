/*
  Copyright 2014-2024 Marco De Salvo
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
using OWLSharp.Extensions.SKOS.Validator;
using OWLSharp.Extensions.SKOS.Validator.RuleSet;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Validator;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.SKOS.Validator
{
    [TestClass]
    public class SKOSValidatorTest
    {
        #region Tests
        [TestMethod]
        public async Task ShouldAnalyzeAlternativeLabelAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ],
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL),
                        new RDFResource("ex:ConceptB"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptB"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B (but this is preferred)"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL),
                        new RDFResource("ex:ConceptC"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept C")))
                ],
                AssertionAxioms = [
                     new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
                ]
            };
            SKOSValidator validator = new SKOSValidator() { Rules = [ SKOSEnums.SKOSValidatorRules.AlternativeLabelAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, SKOSAlternativeLabelAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, SKOSAlternativeLabelAnalysisRule.rulesugg1)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not clash on skos:altLabel and skos:prefLabel values")));
        }

        [TestMethod]
        public async Task ShouldAnalyzeHiddenLabelAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ],
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL),
                        new RDFResource("ex:ConceptB"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL),
                        new RDFResource("ex:ConceptB"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B (but this is preferred)"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL),
                        new RDFResource("ex:ConceptC"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept C")))
                ],
                AssertionAxioms = [
                     new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
                ]
            };
            SKOSValidator validator = new SKOSValidator() { Rules = [ SKOSEnums.SKOSValidatorRules.HiddenLabelAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, SKOSHiddenLabelAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, SKOSHiddenLabelAnalysisRule.rulesugg1)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not clash on skos:hiddenLabel and skos:altLabel values")));
        }

        [TestMethod]
        public async Task ShouldAnalyzePreferredLabelAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ],
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A", "en-US"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A again", "en-US"))), //clash on EN-US
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptB"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B", "en-US"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptB"),
                        new OWLLiteral(new RDFPlainLiteral("Questo e' il concetto B", "it-IT"))),
                ],
                AssertionAxioms = [
                     new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC")))
                ]
            };
            SKOSValidator validator = new SKOSValidator() { Rules = [ SKOSEnums.SKOSValidatorRules.PreferredLabelAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, SKOSPreferredLabelAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, SKOSPreferredLabelAnalysisRule.rulesugg1)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not have more than one occurrence of the same language tag in skos:prefLabel values")));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNotationAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.SKOS.NOTATION)),
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
                        new OWLLiteral(new RDFTypedLiteral("C2N", RDFModelEnums.RDFDatatypes.XSD_STRING))),
                ]
            };
            SKOSValidator validator = new SKOSValidator() { Rules = [ SKOSEnums.SKOSValidatorRules.NotationAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
			Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNotationAnalysisRule.rulename));
			Assert.IsTrue(string.Equals(issues[0].Description, SKOSNotationAnalysisRule.rulesugg));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' belonging to the same schema should be adjusted to not clash on skos:Notation values"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
			Assert.IsTrue(string.Equals(issues[1].RuleName, SKOSNotationAnalysisRule.rulename));
			Assert.IsTrue(string.Equals(issues[1].Description, SKOSNotationAnalysisRule.rulesugg));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, "SKOS concepts 'ex:ConceptB' and 'ex:ConceptA' belonging to the same schema should be adjusted to not clash on skos:Notation values"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeBroaderConceptAsync()
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))), //clash
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.BROADER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            SKOSValidator validator = new SKOSValidator() { Rules = [ SKOSEnums.SKOSValidatorRules.BroaderConceptAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
			Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSBroaderConceptAnalysisRule.rulename));
			Assert.IsTrue(string.Equals(issues[0].Description, SKOSBroaderConceptAnalysisRule.rulesugg1A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' belonging to the same schema should be adjusted to not clash on hierarchical relations (skos:broader VS skos:narrower)"));
        }
        
        [TestMethod]
        public async Task ShouldAnalyzeNarrowerConceptAsync()
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
                        new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            SKOSValidator validator = new SKOSValidator() { Rules = [ SKOSEnums.SKOSValidatorRules.NarrowerConceptAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
			Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSNarrowerConceptAnalysisRule.rulename));
			Assert.IsTrue(string.Equals(issues[0].Description, SKOSNarrowerConceptAnalysisRule.rulesugg1A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' belonging to the same schema should be adjusted to not clash on hierarchical relations (skos:narrower VS skos:broader)"));
        }

        [TestMethod]
        public async Task ShouldAnalyzeCloseOrExactMatchConceptAsync()
        {
            OWLOntology ontology = new OWLOntology()
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
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                ]
            };
            SKOSValidator validator = new SKOSValidator() { Rules = [ SKOSEnums.SKOSValidatorRules.CloseOrExactMatchConceptAnalysis ] };
            List<OWLIssue> issues = await validator.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
			Assert.IsTrue(string.Equals(issues[0].RuleName, SKOSCloseOrExactMatchConceptAnalysisRule.rulename));
			Assert.IsTrue(string.Equals(issues[0].Description, SKOSCloseOrExactMatchConceptAnalysisRule.rulesugg1A));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "SKOS concepts 'ex:ConceptA' and 'ex:ConceptB' belonging to the same schema should be adjusted to not clash on associative/mapping relations (skos:closeMatch VS skos:related)"));
        }
        #endregion
    }
}