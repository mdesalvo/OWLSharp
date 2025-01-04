/*
  Copyright 2014-2025 Marco De Salvo
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at
	http://www.apache.org/licenses/LICENSE-2.0
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific languKNOWS governing permissions and
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
    public class OWLSubObjectPropertyOfAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeSubObjectPropertyOfSubObjectPropertyOfCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(new RDFResource("ex:knows")) ),
					new OWLSubObjectPropertyOf(
						new OWLObjectProperty(new RDFResource("ex:knows")), 
						new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows")))
                ]
            };
            List<OWLIssue> issues = OWLSubObjectPropertyOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubObjectPropertyOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubObjectPropertyOfAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public void ShouldAnalyzeSubObjectPropertyOfEquivalentObjectPropertiesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(new RDFResource("ex:knows")) ),
					new OWLEquivalentObjectProperties([
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(new RDFResource("ex:knows")) ])
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows")))
                ]
            };
            List<OWLIssue> issues = OWLSubObjectPropertyOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubObjectPropertyOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubObjectPropertyOfAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public void ShouldAnalyzeSubObjectPropertyOfDisjointObjectPropertiesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(new RDFResource("ex:knows")) ),
					new OWLDisjointObjectProperties([
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 
						new OWLObjectProperty(new RDFResource("ex:knows")) ])
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows")))
                ]
            };
            List<OWLIssue> issues = OWLSubObjectPropertyOfAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLSubObjectPropertyOfAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLSubObjectPropertyOfAnalysisRule.rulesugg)));
        }
        #endregion
    }
}