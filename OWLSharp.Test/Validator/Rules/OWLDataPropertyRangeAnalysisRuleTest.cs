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
    public class OWLDataPropertyRangeAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeDataPropertyRange()
        {
            OWLOntology ontology = new OWLOntology()
            {
				AssertionAxioms = [
                    new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))), //clashes with range of ex:dp1
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
						new OWLLiteral(new RDFPlainLiteral("lit"))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
						new OWLLiteral(new RDFTypedLiteral("32", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:dp2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
						new OWLLiteral(new RDFPlainLiteral("lit"))),
					new OWLDataPropertyAssertion(
						new OWLDataProperty(new RDFResource("ex:dp1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
						new OWLLiteral(new RDFTypedLiteral("43", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                ],
				DataPropertyAxioms = [
					new OWLDataPropertyRange(
						new OWLDataProperty(new RDFResource("ex:dp1")),
						new OWLDatatype(RDFVocabulary.XSD.INTEGER))
				],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp1"))),
					new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:dp2"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLDataPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLDataPropertyRangeAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLDataPropertyRangeAnalysisRule.rulesugg)));
        }
        #endregion
    }
}