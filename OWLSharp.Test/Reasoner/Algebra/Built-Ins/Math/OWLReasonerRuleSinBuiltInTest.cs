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
using System.Data;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerRuleSinBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSinBuiltIn()
        {
            OWLReasonerRuleSinBuiltIn builtin = new OWLReasonerRuleSinBuiltIn(new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.Equals(new RDFResource("swrlb:sin")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(builtin.IsBuiltIn);
            Assert.IsTrue(string.Equals("swrlb:sin(?L,?R)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSinBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleSinBuiltIn(null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSinBuiltInBecauseNullRightArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleSinBuiltIn(new RDFVariable("?L"), null));

        [TestMethod]
        public void ShouldEvaluateSinBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?X");
            antecedentTable.Columns.Add("?Y");
            antecedentTable.Rows.Add($"0.00^^{RDFVocabulary.XSD.FLOAT}", $"0^^{RDFVocabulary.XSD.DOUBLE}");
            antecedentTable.Rows.Add($"0.22^^{RDFVocabulary.XSD.INTEGER}", $"35^^{RDFVocabulary.XSD.DOUBLE}");

            OWLReasonerRuleSinBuiltIn builtin = new OWLReasonerRuleSinBuiltIn(new RDFVariable("?X"), new RDFVariable("?Y"));
            DataTable result = builtin.Evaluate(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 1);
        }

        #endregion
    }
}