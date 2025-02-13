﻿/*
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
    public class OWLObjectPropertyRangeAnalysisRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnalyzeObjectPropertyRange()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cls1"))), //ex:Mark is explicitly incompatible with range of ex:op1
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:Cls1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op1")),
                        new OWLNamedIndividual(new RDFResource("ex:John")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLObjectPropertyRange(
                        new OWLObjectProperty(new RDFResource("ex:op1")),
                        new OWLClass(new RDFResource("ex:Cls1")))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLObjectPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyRangeAnalysisRule.rulesugg)));
        }

        [TestMethod]
        public void ShouldAnalyzeObjectPropertyRangeInverseOfCase()
        {
            OWLOntology ontology = new OWLOntology
            {
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cls1"))), //ex:Mark is explicitly incompatible with range of ex:op1
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLClassAssertion(
                        new OWLClass(new RDFResource("ex:Cls1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op1")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op2")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op1")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op2")),
                        new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                        new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(new RDFResource("ex:op1")),
                        new OWLNamedIndividual(new RDFResource("ex:Helen")),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                ObjectPropertyAxioms = [
                    new OWLObjectPropertyRange(
                        new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("ex:op1"))),
                        new OWLClass(new RDFResource("ex:Cls1")))
                ],
                DeclarationAxioms = [ 
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLDeclaration(new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op1"))),
                    new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:op2"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Helen")))
                ]
            };
            List<OWLIssue> issues = OWLObjectPropertyRangeAnalysisRule.ExecuteRule(ontology);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.IsTrue(issues.TrueForAll(iss => iss.Severity == OWLEnums.OWLIssueSeverity.Error));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.RuleName, OWLObjectPropertyRangeAnalysisRule.rulename)));
            Assert.IsTrue(issues.TrueForAll(iss => string.Equals(iss.Suggestion, OWLObjectPropertyRangeAnalysisRule.rulesugg)));
        }
        #endregion
    }
}