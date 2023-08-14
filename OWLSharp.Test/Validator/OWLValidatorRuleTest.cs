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

namespace OWLSharp.Validator.Test
{
    [TestClass]
    public class OWLValidatorRuleTest
    {
        #region Test
        [TestMethod]
        public void ShouldCreateValidatorRule()
        {
            #region RuleDelegate
            OWLValidatorReport TestRuleBody(OWLOntology ontology) 
                => new OWLValidatorReport();
            #endregion

            OWLValidatorRule rule = new OWLValidatorRule("rulename", "description", TestRuleBody);

            Assert.IsNotNull(rule);
            Assert.IsTrue(string.Equals(rule.RuleName, "rulename"));
            Assert.IsTrue(string.Equals(rule.RuleDescription, "description"));
            Assert.IsNotNull(rule.ExecuteRule);
            Assert.IsNotNull(rule.ExecuteRule(new OWLOntology("ex:ont")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingvalidatorRuleBecauseNullOrEmptyName()
        {
            #region RuleDelegate
            OWLValidatorReport TestRuleBody(OWLOntology ontology)
                => new OWLValidatorReport();
            #endregion

            Assert.ThrowsException<OWLException>(() => new OWLValidatorRule(null, "description", TestRuleBody));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingvalidatorRuleBecauseNullDelegate()
            => Assert.ThrowsException<OWLException>(() => new OWLValidatorRule("rulename", "description", null));
        #endregion
    }
}
