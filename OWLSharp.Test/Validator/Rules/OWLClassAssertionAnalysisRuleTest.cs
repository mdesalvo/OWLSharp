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
    public class OWLClassAssertionAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeClassAssertions()
        {
            OWLOntology ontology = new OWLOntology()
            {
				AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:Cls1")),
						new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:Cls2")),
						new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLClassAssertion(
                        new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cls1"))),
						new OWLNamedIndividual(new RDFResource("ex:Mark"))), //clashing with first class assertion
					new OWLClassAssertion(
                        new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cls1"))),
						new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
					new OWLClassAssertion(
                        new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cls2"))),
						new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                ],
				DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls1"))),
					new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls2"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
					new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv")))
                ]
            };
            List<OWLIssue> issues = OWLClassAssertionAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
			Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLClassAssertionAnalysisRule.rulename)));
			Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLClassAssertionAnalysisRule.rulesugg)));
        }
        #endregion
    }
}