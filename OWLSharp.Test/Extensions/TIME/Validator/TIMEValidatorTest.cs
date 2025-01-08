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
using OWLSharp.Extensions.TIME;
using OWLSharp.Ontology;
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEValidatorTest : TIMETestOntology
    {
        [TestMethod]
        public void ShouldAddRule()
        {
            TIMEValidator validator = new TIMEValidator();
            
            Assert.IsNotNull(validator);
            Assert.IsNotNull(validator.Rules);
            Assert.IsTrue(validator.Rules.Count == 0);

            validator.AddRule(TIMEEnums.TIMEValidatorRules.InstantAfterAnalysis);
            Assert.IsTrue(validator.Rules.Count == 1);
        }

        [TestMethod]
        public async Task ShouldValidateInstantAfter()
        {
            OWLOntology ontology = new OWLOntology(TestOntology);
            ontology.DeclarationAxioms.AddRange([
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantA"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
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
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantB")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA"))), //clash
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.AFTER),
                    new OWLNamedIndividual(new RDFResource("ex:InstantA")),
                    new OWLNamedIndividual(new RDFResource("ex:InstantC"))),
            ]);

            TIMEValidator validator = new TIMEValidator();
            validator.AddRule(TIMEEnums.TIMEValidatorRules.InstantAfterAnalysis);

            List<OWLIssue> issues = await TIMEInstantAfterAnalysisRule.ExecuteRuleAsync(ontology);

            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 2);
            Assert.IsTrue(issues[0].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[0].RuleName, TIMEInstantAfterAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[0].Description, TIMEInstantAfterAnalysisRule.rulesugg1));
            Assert.IsTrue(string.Equals(issues[0].Suggestion, "TIME instants 'ex:InstantA' and 'ex:InstantB' should be adjusted to not clash on temporal relations (time:after VS time:after)"));
            Assert.IsTrue(issues[1].Severity == OWLEnums.OWLIssueSeverity.Error);
            Assert.IsTrue(string.Equals(issues[1].RuleName, TIMEInstantAfterAnalysisRule.rulename));
            Assert.IsTrue(string.Equals(issues[1].Description, TIMEInstantAfterAnalysisRule.rulesugg1));
            Assert.IsTrue(string.Equals(issues[1].Suggestion, "TIME instants 'ex:InstantB' and 'ex:InstantA' should be adjusted to not clash on temporal relations (time:after VS time:after)"));
        }
    }
}