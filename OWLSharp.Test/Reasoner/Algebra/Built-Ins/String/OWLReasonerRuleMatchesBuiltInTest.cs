/*
   Copyright 2012-2024 Marco De Salvo

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
using System.Text.RegularExpressions;

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerRuleMatchesBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateMatchesBuiltIn()
        {
            OWLReasonerRuleMatchesBuiltIn builtin = new OWLReasonerRuleMatchesBuiltIn(new RDFVariable("?L"), new Regex("val"));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.Equals(new RDFResource("swrlb:matches")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFPlainLiteral("val")));
            Assert.IsTrue(builtin.IsBuiltIn);
            Assert.IsTrue(string.Equals("swrlb:matches(?L,\"val\")", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldCreateMatchesBuiltInWithOptions()
        {
            OWLReasonerRuleMatchesBuiltIn builtin = new OWLReasonerRuleMatchesBuiltIn(new RDFVariable("?L"), new Regex("val", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.Equals(new RDFResource("swrlb:matches")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFPlainLiteral("val\",\"ismx")));
            Assert.IsTrue(builtin.IsBuiltIn);
            Assert.IsTrue(string.Equals("swrlb:matches(?L,\"val\",\"ismx\")", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingMatchesBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleMatchesBuiltIn(null, new Regex("val")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingMatchesBuiltInBecauseNullRightArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleMatchesBuiltIn(new RDFVariable("?L"), null));
        
        [TestMethod]
        public void ShouldEvaluateMatchesBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv1");
            antecedentTable.Rows.Add("ex:indiv2");
            antecedentTable.Rows.Add("ex:indiV2");

            OWLReasonerRuleMatchesBuiltIn builtin = new OWLReasonerRuleMatchesBuiltIn(new RDFVariable("?C"), new Regex("iv2$", RegexOptions.IgnoreCase | RegexOptions.Multiline));
            DataTable result = builtin.Evaluate(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 2);
        }
        #endregion
    }
}