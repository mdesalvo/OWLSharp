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
    public class SWRLEqualBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateEqualBuiltInWithVariableRightArgument()
        {
            SWRLEqualBuiltIn builtin = new SWRLEqualBuiltIn(new RDFVariable("?L"), new RDFVariable("?R"));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.GetIRI().Equals(new RDFResource("swrlb:equal")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFVariable("?R")));
            Assert.IsTrue(string.Equals("swrlb:equal(?L,?R)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldCreateEqualBuiltInWithResourceRightArgument()
        {
            SWRLEqualBuiltIn builtin = new SWRLEqualBuiltIn(new RDFVariable("?L"), new RDFResource("ex:res"));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.GetIRI().Equals(new RDFResource("swrlb:equal")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFResource("ex:res")));
            Assert.IsTrue(string.Equals("swrlb:equal(?L,ex:res)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldCreateEqualBuiltInWithLiteralRightArgument()
        {
            SWRLEqualBuiltIn builtin = new SWRLEqualBuiltIn(new RDFVariable("?L"), new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.BuiltInFilter);
            Assert.IsNotNull(builtin.Predicate);
            Assert.IsTrue(builtin.Predicate.GetIRI().Equals(new RDFResource("swrlb:equal")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument.Equals(new RDFVariable("?L")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument.Equals(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            Assert.IsTrue(string.Equals($"swrlb:equal(?L,\"25\"^^xsd:integer)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEqualBuiltInBecauseNullLeftArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLEqualBuiltIn(null, new RDFVariable("?R")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEqualBuiltInBecauseNullRightArgumentVariable()
            => Assert.ThrowsException<OWLException>(() => new SWRLEqualBuiltIn(new RDFVariable("?L"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEqualBuiltInBecauseNullRightArgumentResource()
            => Assert.ThrowsException<OWLException>(() => new SWRLEqualBuiltIn(new RDFVariable("?L"), null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingEqualBuiltInBecauseNullRightArgumentLiteral()
            => Assert.ThrowsException<OWLException>(() => new SWRLEqualBuiltIn(new RDFVariable("?L"), null as RDFLiteral));

        [TestMethod]
        public void ShouldEvaluateEqualBuiltIn()
        {
            DataTable antecedentTable = new DataTable();
            antecedentTable.Columns.Add("?C");
            antecedentTable.Rows.Add("ex:indiv");
            antecedentTable.Rows.Add("ex:indiv2");

            SWRLEqualBuiltIn builtin = new SWRLEqualBuiltIn(new RDFVariable("?C"), new RDFResource("ex:indiv2"));
            DataTable result = builtin.EvaluateOnAntecedent(antecedentTable, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count == 1);
        }

        #endregion
    }
}