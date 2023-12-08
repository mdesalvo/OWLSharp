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

namespace OWLSharp.Reasoner.Test
{
    [TestClass]
    public class OWLReasonerRuleNotEqualBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateEqualBuiltInWithVariableRightArgument()
        {
            OWLReasonerRuleNotEqualBuiltIn builtin = new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.Equals(new RDFResource("swrlb:notEqual")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(builtin.IsBuiltIn);
            Assert.IsTrue(string.Equals("swrlb:notEqual(?L,?R)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldCreateEqualBuiltInWithResourceRightArgument()
        {
            OWLReasonerRuleNotEqualBuiltIn builtin = new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?L"), new RDFResource("ex:res"));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.Equals(new RDFResource("swrlb:notEqual")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFResource("ex:res")));
            Assert.IsTrue(builtin.IsBuiltIn);
            Assert.IsTrue(string.Equals("swrlb:notEqual(?L,ex:res)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldCreateEqualBuiltInWithLiteralRightArgument()
        {
            OWLReasonerRuleNotEqualBuiltIn builtin = new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?L"), new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.Equals(new RDFResource("swrlb:notEqual")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            Assert.IsTrue(builtin.IsBuiltIn);
            Assert.IsTrue(string.Equals($"swrlb:notEqual(?L,\"25\"^^xsd:integer)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNotEqualBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleNotEqualBuiltIn(null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNotEqualBuiltInBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNotEqualBuiltInBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNotEqualBuiltInBecauseNullRightArgumentLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?L"), null as RDFLiteral));

        [TestMethod]
        public void ShouldEvaluateNotEqualBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv");
            antecedentTable.Rows.Add("ex:indiv2");

            OWLReasonerRuleNotEqualBuiltIn builtin = new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?C"), new RDFResource("ex:indiv2"));
            DataTable result = builtin.Evaluate(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 1);
        }

        #endregion
    }
}