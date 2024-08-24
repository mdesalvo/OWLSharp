﻿/*
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

namespace OWLSharp.Test.Extensions.SWRL.Model.BuiltIns
{
    [TestClass]
    public class SWRLContainsBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateContainsBuiltIn()
        {
            SWRLContainsBuiltIn builtin = new SWRLContainsBuiltIn(new RDFVariable("?L"), "val");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.GetIRI().Equals(new RDFResource("swrlb:contains")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFPlainLiteral("val")));
            Assert.IsTrue(string.Equals("swrlb:contains(?L,\"val\")", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingContainsBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLContainsBuiltIn(null, "val"));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingContainsBuiltInBecauseNullRightArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLContainsBuiltIn(new RDFVariable("?L"), null));
        
        [TestMethod]
        public void ShouldEvaluateContainsBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv");
            antecedentTable.Rows.Add("ex:indiv2");
            antecedentTable.Rows.Add("ex:indiV2");

            SWRLContainsBuiltIn builtin = new SWRLContainsBuiltIn(new RDFVariable("?C"), "div2");
            DataTable result = builtin.EvaluateOnAntecedent(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 1);
        }
        #endregion
    }
}