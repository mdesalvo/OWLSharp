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
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, "SKOS concept 'ex:ConceptA' should be adjusted to not clash on skos:altlabel and skos:prefLabel values")));
        }
        #endregion
    }
}