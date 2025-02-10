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

namespace OWLSharp.Test.Validator
{
    [TestClass]
    public class OWLTopBottomAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeTopBottomT1Case()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY),
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ]
            };
            List<OWLIssue> issues = OWLTopBottomAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected object property axioms causing reserved owl:topObjectProperty property to not be the root object property of the ontology")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysisRule.rulesuggT1)));
        }

        [TestMethod]
        public void ShouldAnalyzeTopBottomT2Case()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DataPropertyAxioms = [
                    new OWLSubDataPropertyOf(
                        new OWLDataProperty(RDFVocabulary.OWL.TOP_DATA_PROPERTY),
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.OWL.TOP_DATA_PROPERTY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE))
                ]
            };
            List<OWLIssue> issues = OWLTopBottomAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected data property axioms causing reserved owl:topDataProperty property to not be the root data property of the ontology")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysisRule.rulesuggT2)));
        }

        [TestMethod]
        public void ShouldAnalyzeTopBottomB1Case()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLObjectProperty(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY))                        
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ]
            };
            List<OWLIssue> issues = OWLTopBottomAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected object property axioms causing reserved owl:bottomObjectProperty property to not be the bottom object property of the ontology")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysisRule.rulesuggB1)));
        }

        [TestMethod]
        public void ShouldAnalyzeTopBottomB2Case()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DataPropertyAxioms = [
                    new OWLSubDataPropertyOf(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLDataProperty(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE))
                ]
            };
            List<OWLIssue> issues = OWLTopBottomAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTopBottomAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected data property axioms causing reserved owl:bottomDataProperty property to not be the bottom data property of the ontology")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTopBottomAnalysisRule.rulesuggB2)));
        }
        #endregion
    }
}