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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.TIME;
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEInstanBeforeAnalysisRuleTest : TIMETestOntology
    {
        #region Tests
        [TestMethod]
        public async Task ShouldAnalyzeInstantBeforeAndViolateRule1()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantC")))
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC")))
            ]);
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
            {
                { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
                { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
            };
            List<OWLIssue> issues = await TIMEInstantBeforeAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(2, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEInstantBeforeAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, TIMEInstantBeforeAnalysisRule.rulesugg1));
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[1].Severity);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEInstantBeforeAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, TIMEInstantBeforeAnalysisRule.rulesugg1));
        }

        [TestMethod]
        public async Task ShouldAnalyzeInstantBeforeAndViolateRule2()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantC")))
            ]);
            ontology.AssertionAxioms.AddRange([
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.INSTANT),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.BEFORE),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC")))
            ]);
            Dictionary<string, List<OWLIndividualExpression>> cacheRegistry = new Dictionary<string, List<OWLIndividualExpression>>
            {
                { "INSTANTS",  ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INSTANT)) },
                { "INTERVALS", ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.INTERVAL)) }
            };
            List<OWLIssue> issues = await TIMEInstantBeforeAnalysisRule.ExecuteRuleAsync(ontology, cacheRegistry);

            Assert.IsNotNull(issues);
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual(OWLEnums.OWLIssueSeverity.Error, issues[0].Severity);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEInstantBeforeAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, TIMEInstantBeforeAnalysisRule.rulesugg2));
        }
        #endregion
    }
}