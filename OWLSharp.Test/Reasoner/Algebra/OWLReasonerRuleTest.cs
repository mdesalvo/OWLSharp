/*
   Copyright 2012-2023 Marco De Salvo

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
using RDFSharp.Model;
using RDFSharp.Query;
using System.Threading.Tasks;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerRuleTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReasonerRule()
        {
            OWLReasonerRule reasonerRule = new OWLReasonerRule("testRule", "this is test rule",
                new OWLReasonerRuleAntecedent().AddAtom(new OWLReasonerRuleClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"))),
                new OWLReasonerRuleConsequent().AddAtom(new OWLReasonerRuleObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL)));

            Assert.IsNotNull(reasonerRule);
            Assert.IsTrue(string.Equals("testRule", reasonerRule.RuleName));
            Assert.IsTrue(string.Equals("this is test rule", reasonerRule.RuleDescription));
            Assert.IsNotNull(reasonerRule.Antecedent);
            Assert.IsNotNull(reasonerRule.Consequent);
            Assert.IsTrue(string.Equals("ex:class(?C) -> type(?C,Individual)", reasonerRule.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingReasonerRuleBecauseNullOrEmptyRuleName()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRule(null, "this is test rule",
                new OWLReasonerRuleAntecedent(), new OWLReasonerRuleConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingReasonerRuleBecauseNullAntecedent()
           => Assert.ThrowsException<OWLException>(() => new OWLReasonerRule("testRule", "this is test rule",
               null, new OWLReasonerRuleConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingReasonerRuleBecauseNullConsequent()
           => Assert.ThrowsException<OWLException>(() => new OWLReasonerRule("testRule", "description",
               new OWLReasonerRuleAntecedent(), null));

        [TestMethod]
        public void ShouldApplyToOntology()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasonerRule reasonerRule = new OWLReasonerRule("testRule", "this is test rule",
                new OWLReasonerRuleAntecedent().AddAtom(new OWLReasonerRuleClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"))),
                new OWLReasonerRuleConsequent().AddAtom(new OWLReasonerRuleObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL)));

            OWLReasonerReport reasonerReport = reasonerRule.ApplyToOntology(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }

        [TestMethod]
        public async Task ShouldApplyToOntologyAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:class"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:indiv"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:indiv"), new RDFResource("ex:class"));

            OWLReasonerRule reasonerRule = new OWLReasonerRule("testRule", "this is test rule",
                new OWLReasonerRuleAntecedent().AddAtom(new OWLReasonerRuleClassAtom(new RDFResource("ex:class"), new RDFVariable("?C"))),
                new OWLReasonerRuleConsequent().AddAtom(new OWLReasonerRuleObjectPropertyAtom(RDFVocabulary.RDF.TYPE, new RDFVariable("?C"), RDFVocabulary.OWL.INDIVIDUAL)));

            OWLReasonerReport reasonerReport = await reasonerRule.ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(reasonerReport);
            Assert.IsTrue(reasonerReport.EvidencesCount == 1);
        }
        #endregion
    }
}