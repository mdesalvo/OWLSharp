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
using System.Collections.Generic;

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSReasonerTest
    {
        [TestMethod]
        public void ShouldAddSKOSRule()
        {
            OWLReasoner reasoner = new OWLReasoner();
            reasoner.AddSKOSRule(SKOSEnums.SKOSReasonerRules.SKOS_BroaderTransitiveEntailment);
            reasoner.AddSKOSRule(SKOSEnums.SKOSReasonerRules.SKOS_BroaderTransitiveEntailment); //Will be discarded, since duplicate standard rules are not allowed

            Assert.IsNotNull(reasoner);
            Assert.IsNotNull(reasoner.Rules);
            Assert.IsTrue(reasoner.Rules.Count == 2);
            Assert.IsTrue(reasoner.Rules.ContainsKey("STD"));
            Assert.IsTrue(reasoner.Rules["STD"] is List<OWLEnums.OWLReasonerRules> stdRules && stdRules.Count == 0);
            Assert.IsTrue(reasoner.Rules.ContainsKey("SKOS"));
            Assert.IsTrue(reasoner.Rules["SKOS"] is List<SKOSEnums.SKOSReasonerRules> skosRules && skosRules.Count == 1);
            Assert.IsNotNull(reasoner.Extensions);
            Assert.IsTrue(reasoner.Extensions.Count == 1);
            Assert.IsTrue(reasoner.Extensions.ContainsKey("SKOS"));
        }
    }
}