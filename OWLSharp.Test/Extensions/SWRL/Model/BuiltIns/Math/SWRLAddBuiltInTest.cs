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

namespace OWLSharp.Test.Extensions.SWRL.Model.BuiltIns
{
    [TestClass]
    public class SWRLAddBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateAddBuiltIn()
        {
            SWRLAddBuiltIn builtin = new SWRLAddBuiltIn(new RDFVariable("?L"), new RDFVariable("?R"), 2);

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.GetIRI().Equals(new RDFResource("swrlb:add")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(string.Equals("swrlb:add(?L,?R,\"2\"^^xsd:double)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAddBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLAddBuiltIn(null, new RDFVariable("?R"), 2));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAddBuiltInBecauseNullRightArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLAddBuiltIn(new RDFVariable("?L"), null, 2));

        [TestMethod]
        public void ShouldEvaluateAddBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?X");
            antecedentTable.Columns.Add("?Y");
            antecedentTable.Rows.Add($"4.00^^{RDFVocabulary.XSD.FLOAT}", $"2.00^^{RDFVocabulary.XSD.DOUBLE}");
            antecedentTable.Rows.Add($"2^^{RDFVocabulary.XSD.INTEGER}", $"3.57^^{RDFVocabulary.XSD.DOUBLE}");

            SWRLAddBuiltIn builtin = new SWRLAddBuiltIn(new RDFVariable("?X"), new RDFVariable("?Y"), 2);
            DataTable result = builtin.EvaluateOnAntecedent(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 1);
        }

        #endregion
    }
}