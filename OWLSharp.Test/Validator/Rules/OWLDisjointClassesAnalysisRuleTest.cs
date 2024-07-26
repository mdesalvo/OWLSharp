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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Validator;
using OWLSharp.Validator.Rules;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Validator.Rules
{
    [TestClass]
    public class OWLDisjointClassesAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeDisjointClassesSubClassOfCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				ClassAxioms = [
                    new OWLDisjointClasses([
                        new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]),
					new OWLSubClassOf(
						new OWLClass(RDFVocabulary.FOAF.PERSON), 
						new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
					new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION))
                ]
            };
            List<OWLIssue> issues = OWLDisjointClassesAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysisRule.rulesugg)));
        }

		[TestMethod]
        public void ShouldAnalyzeDisjointClassesEquivalentClassesCase()
        {
            OWLOntology ontology = new OWLOntology()
            {
				ClassAxioms = [
                    new OWLDisjointClasses([
                        new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION), new OWLClass(RDFVocabulary.FOAF.AGENT) ]),
					new OWLEquivalentClasses([
                        new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
					new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
					new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT))
                ]
            };
            List<OWLIssue> issues = OWLDisjointClassesAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDisjointClassesAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDisjointClassesAnalysisRule.rulesugg)));
        }
        #endregion
    }
}