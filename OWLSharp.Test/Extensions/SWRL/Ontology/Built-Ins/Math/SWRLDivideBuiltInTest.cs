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
using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;

namespace OWLSharp.Extensions.SWRL.Test
{
    [TestClass]
    public class SWRLDivideBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDivideBuiltIn()
        {
            SWRLDivideBuiltIn builtin = new SWRLDivideBuiltIn(new RDFVariable("?L"), new RDFVariable("?R"), 2);

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.Equals(new RDFResource("swrlb:divide")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(builtin.IsBuiltIn);
            Assert.IsTrue(string.Equals("swrlb:divide(?L,?R,\"2\"^^xsd:double)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDivideBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLDivideBuiltIn(null, new RDFVariable("?R"), 2));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDivideBuiltInBecauseNullRightArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLDivideBuiltIn(new RDFVariable("?L"), null, 2));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDivideBuiltInBecauseZeroDivide()
           => Assert.ThrowsException<OWLException>(() => new SWRLDivideBuiltIn(new RDFVariable("?L"), new RDFVariable("?R"), 0));

        [TestMethod]
        public void ShouldEvaluateDivideBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?X");
            antecedentTable.Columns.Add("?Y");
            antecedentTable.Rows.Add($"3.00^^{RDFVocabulary.XSD.FLOAT}", $"6.00^^{RDFVocabulary.XSD.DOUBLE}");
            antecedentTable.Rows.Add($"2^^{RDFVocabulary.XSD.INTEGER}", $"3.57^^{RDFVocabulary.XSD.DOUBLE}");

            SWRLDivideBuiltIn builtin = new SWRLDivideBuiltIn(new RDFVariable("?X"), new RDFVariable("?Y"), 2);
            DataTable result = builtin.Evaluate(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 1);
        }

        #endregion
    }
}