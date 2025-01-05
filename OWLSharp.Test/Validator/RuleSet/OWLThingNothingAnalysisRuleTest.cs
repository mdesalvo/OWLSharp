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
    public class OWLThingNothingAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeThingNothingT1Case()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(RDFVocabulary.OWL.THING),
                        new OWLClass(RDFVocabulary.FOAF.PERSON))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(RDFVocabulary.OWL.THING)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON))
                ]
            };
            List<OWLIssue> issues = OWLThingNothingAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLThingNothingAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected class axioms causing reserved owl:Thing class to not be the root entity of the ontology")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLThingNothingAnalysisRule.rulesuggT1)));
        }

        [TestMethod]
        public void ShouldAnalyzeThingNothingN1Case()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new OWLClass(RDFVocabulary.OWL.NOTHING))
                ],
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.OWL.NOTHING)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON))
                ]
            };
            List<OWLIssue> issues = OWLThingNothingAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Warning));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLThingNothingAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Detected class axioms causing reserved owl:Nothing class to not be the bottom entity of the ontology")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLThingNothingAnalysisRule.rulesuggN1)));
        }

        [TestMethod]
        public void ShouldAnalyzeThingNothingN2Case()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.OWL.NOTHING),
                        new OWLNamedIndividual(new RDFResource("ex:Idv")))
                ],
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.OWL.NOTHING)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Idv")))
                ]
            };
            List<OWLIssue> issues = OWLThingNothingAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 1);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLThingNothingAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Description, "Violated owl:Nothing class having individuals")));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLThingNothingAnalysisRule.rulesuggN2)));
        }
        #endregion
    }
}