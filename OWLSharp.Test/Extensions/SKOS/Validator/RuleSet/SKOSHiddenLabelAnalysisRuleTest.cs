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
    public class SKOSHiddenLabelAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeHiddenLabelConflictingWithAltLabel()
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
                        new OWLLiteral(new RDFPlainLiteral("This is concept B (but this is hidden)"))),
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
            List<OWLIssue> issues = SKOSHiddenLabelAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, SKOSHiddenLabelAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, SKOSHiddenLabelAnalysisRule.rulesugg1)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not clash on skos:hiddenLabel and skos:altLabel values")));
        }
        
        [TestMethod]
        public void ShouldAnalyzeHiddenLabelConflictingWithPrefLabel()
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
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        new RDFResource("ex:ConceptA"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL),
                        new RDFResource("ex:ConceptB"),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B"))),
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
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
            List<OWLIssue> issues = SKOSHiddenLabelAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, SKOSHiddenLabelAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, SKOSHiddenLabelAnalysisRule.rulesugg2)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not clash on skos:hiddenLabel and skos:prefLabel values")));
        }

        [TestMethod]
        public void ShouldAnalyzeAlternativeXLabelConflictingWithAltXLabel()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelA2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelB2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelC")))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA2"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB2"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelC"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA2"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA2")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB2"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB2")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B (but this is hidden)"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelC"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelC")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept C")))
                ]
            };
            List<OWLIssue> issues = SKOSHiddenLabelAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, SKOSHiddenLabelAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, SKOSHiddenLabelAnalysisRule.rulesugg3)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not clash on skosxl:hiddenLabel and skosxl:altLabel values")));
        }

        [TestMethod]
        public void ShouldAnalyzeAlternativeXLabelConflictingWithPrefXLabel()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.CONCEPT)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelA2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelB"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelB2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:LabelC")))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA2"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB2"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC"))),
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.SKOSXL.LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:LabelC"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptA")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA2"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelA2")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept A"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptB")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB2"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelB2")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept B (but this is preferred)"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL),
                        new OWLNamedIndividual(new RDFResource("ex:ConceptC")),
                        new OWLNamedIndividual(new RDFResource("ex:LabelC"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM),
                        new OWLNamedIndividual(new RDFResource("ex:LabelC")),
                        new OWLLiteral(new RDFPlainLiteral("This is concept C")))
                ]
            };
            List<OWLIssue> issues = SKOSHiddenLabelAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, SKOSHiddenLabelAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, SKOSHiddenLabelAnalysisRule.rulesugg4)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not clash on skosxl:hiddenLabel and skosxl:prefLabel values")));
        }
        #endregion
    }
}