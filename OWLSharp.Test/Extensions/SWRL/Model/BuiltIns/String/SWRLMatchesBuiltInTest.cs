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
using OWLSharp.Extensions.SWRL.Model.BuiltIns;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;
using System.Text.RegularExpressions;

namespace OWLSharp.Test.Extensions.SWRL.Model.BuiltIns
{
    [TestClass]
    public partial class SWRLMatchesBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateMatchesBuiltIn()
        {
            SWRLMatchesBuiltIn builtin = new SWRLMatchesBuiltIn(new RDFVariable("?L"), Regex_val());

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.GetIRI().Equals(new RDFResource("swrlb:matches")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFPlainLiteral("val")));
            Assert.IsTrue(string.Equals("swrlb:matches(?L,\"val\")", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldCreateMatchesBuiltInWithOptions()
        {
            SWRLMatchesBuiltIn builtin = new SWRLMatchesBuiltIn(new RDFVariable("?L"), Regex_val_withoptions());

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.GetIRI().Equals(new RDFResource("swrlb:matches")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFPlainLiteral("val\",\"ismx")));
            Assert.IsTrue(string.Equals("swrlb:matches(?L,\"val\",\"ismx\")", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingMatchesBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLMatchesBuiltIn(null, Regex_val()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingMatchesBuiltInBecauseNullRightArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLMatchesBuiltIn(new RDFVariable("?L"), null));
        
        [TestMethod]
        public void ShouldEvaluateMatchesBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv1");
            antecedentTable.Rows.Add("ex:indiv2");
            antecedentTable.Rows.Add("ex:indiV2");

            SWRLMatchesBuiltIn builtin = new SWRLMatchesBuiltIn(new RDFVariable("?C"), Regex_iv2());
            DataTable result = builtin.EvaluateOnAntecedent(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 2);
        }

        [GeneratedRegex("val")]
        private static partial Regex Regex_val();

        [GeneratedRegex("val", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace, "en-US")]
        private static partial Regex Regex_val_withoptions();

        [GeneratedRegex("iv2$", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-US")]
        private static partial Regex Regex_iv2();
        #endregion
    }
}