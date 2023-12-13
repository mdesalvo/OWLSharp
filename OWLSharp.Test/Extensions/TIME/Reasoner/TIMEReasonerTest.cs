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
using OWLSharp.Extensions.SWRL;
using System.Collections.Generic;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEReasonerTest
    {
        [TestMethod]
        public void ShouldAddTIMERule()
        {
            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddTIMERule(TIMEEnums.TIMEReasonerRules.TIME_EqualsEntailment);
            reasoner.AddTIMERule(TIMEEnums.TIMEReasonerRules.TIME_EqualsEntailment); //Will be discarded, since duplicate standard rules are not allowed

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.Rules);
            Assert.IsTrue(reasoner.Rules.Count == 3);
            Assert.IsTrue(reasoner.Rules.ContainsKey("STD"));
            Assert.IsTrue(reasoner.Rules["STD"] is List<OWLEnums.OWLReasonerRules> stdRules && stdRules.Count == 0);
            Assert.IsTrue(reasoner.Rules.ContainsKey("SWRL"));
            Assert.IsTrue(reasoner.Rules["SWRL"] is List<SWRLRule> swrlRules && swrlRules.Count == 0);
            Assert.IsTrue(reasoner.Rules.ContainsKey("TIME"));
            Assert.IsTrue(reasoner.Rules["TIME"] is List<TIMEEnums.TIMEReasonerRules> timeRules && timeRules.Count == 1);
        }
    }
}