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

using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Rules;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLRuleTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLRule()
        {
            SWRLRule rule = new SWRLRule(
                new RDFPlainLiteral("SWRL1"),
                new RDFPlainLiteral("This is a test SWRL rule"),
                new SWRLAntecedent(),
                new SWRLConsequent());

            Assert.IsNotNull(rule);
            Assert.IsNotNull(rule.Annotations);
            Assert.IsTrue(rule.Annotations.Count == 2);
            Assert.IsNotNull(rule.Antecedent);
            Assert.IsNotNull(rule.Consequent);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullRuleName()
            => Assert.ThrowsException<OWLException>(() => 
                new SWRLRule(
                    null,
                    new RDFPlainLiteral("This is a test SWRL rule"),
                    new SWRLAntecedent(),
                    new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullRuleDescription()
            => Assert.ThrowsException<OWLException>(() =>
                new SWRLRule(
                    new RDFPlainLiteral("SWRL1"),
                    null,
                    new SWRLAntecedent(),
                    new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullAntecedent()
           => Assert.ThrowsException<OWLException>(() =>
               new SWRLRule(
                   new RDFPlainLiteral("SWRL1"),
                   new RDFPlainLiteral("This is a test SWRL rule"),
                   null,
                   new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullConsequent()
           => Assert.ThrowsException<OWLException>(() =>
               new SWRLRule(
                   new RDFPlainLiteral("SWRL1"),
                   new RDFPlainLiteral("This is a test SWRL rule"),
                   new SWRLAntecedent(),
                   null));
        #endregion
    }
}