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
    public class OWLTermsDeprecationAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeDeprecatedTermsClassCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                        new RDFResource("http://xmlns.com/foaf/0.1/Person"),
                        new OWLLiteral(RDFTypedLiteral.True))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Person"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("http://xmlns.com/foaf/0.1/Organization")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDeprecationAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDeprecationAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected presence of deprecated class with IRI: 'http://xmlns.com/foaf/0.1/Person'")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDeprecationAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDeprecatedTermsDatatypeCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                        RDFVocabulary.XSD.INTEGER,
                        new OWLLiteral(RDFTypedLiteral.True))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.INTEGER)),
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.DATETIME))
                ]
            };
            List<OWLIssue> issues = OWLTermsDeprecationAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDeprecationAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected presence of deprecated datatype with IRI: 'http://www.w3.org/2001/XMLSchema#integer'")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDeprecationAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDeprecatedTermsDataPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                        new RDFResource("http://xmlns.com/foaf/0.1/age"),
                        new OWLLiteral(RDFTypedLiteral.True))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/age"))),
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("http://xmlns.com/foaf/0.1/name")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDeprecationAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDeprecationAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected presence of deprecated data property with IRI: 'http://xmlns.com/foaf/0.1/age'")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDeprecationAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDeprecatedTermsObjectPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                        new RDFResource("http://xmlns.com/foaf/0.1/knows"),
                        new OWLLiteral(RDFTypedLiteral.True))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/knows"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("http://xmlns.com/foaf/0.1/depicts")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDeprecationAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDeprecationAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected presence of deprecated object property with IRI: 'http://xmlns.com/foaf/0.1/knows'")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDeprecationAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeDeprecatedTermsAnnotationPropertyCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                        new RDFResource("http://xmlns.com/foaf/0.1/author"),
                        new OWLLiteral(RDFTypedLiteral.True))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLAnnotationProperty(new RDFResource("http://xmlns.com/foaf/0.1/author"))),
                    new OWLDeclaration(new OWLAnnotationProperty(new RDFResource("http://xmlns.com/foaf/0.1/lastCommit")))
                ]
            };
            List<OWLIssue> issues = OWLTermsDeprecationAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLTermsDeprecationAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected presence of deprecated annotation property with IRI: 'http://xmlns.com/foaf/0.1/author'")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLTermsDeprecationAnalysisRule.rulesugg)));
        }
        #endregion
    }
}