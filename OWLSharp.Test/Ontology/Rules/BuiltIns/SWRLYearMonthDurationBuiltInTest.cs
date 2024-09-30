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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Rules;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLYearMonthDurationBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateYearMonthDurationBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.YearMonthDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#yearMonthDuration", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 3);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2 
                            && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
            Assert.IsTrue(string.Equals("swrlb:yearMonthDuration(?X,?Y,?Z)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.YearMonthDuration(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.YearMonthDuration(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeYearMonthDurationBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.YearMonthDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#yearMonthDuration\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeYearMonthDurationBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#yearMonthDuration""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Variable IRI=""urn:swrl:var#Z"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#yearMonthDuration", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 3);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                            && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
            Assert.IsTrue(string.Equals("swrlb:yearMonthDuration(?X,?Y,?Z)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#yearMonthDuration\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#yearMonthDuration""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#yearMonthDuration\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldEvaluateYearMonthDurationBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Rows.Add(
                "P2Y7M^^http://www.w3.org/2001/XMLSchema#duration", 
                "2^^http://www.w3.org/2001/XMLSchema#int", 
                "7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P0Y0M^^http://www.w3.org/2001/XMLSchema#duration", 
                "0^^http://www.w3.org/2001/XMLSchema#int", 
                "0^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P2Y7M^^http://www.w3.org/2001/XMLSchema#duration", 
                "2^^http://www.w3.org/2001/XMLSchema#int", 
                "9^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.YearMonthDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 3);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P2Y7M^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "P0Y0M^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "0^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "0^^http://www.w3.org/2001/XMLSchema#int"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.YearMonthDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Z"))); 
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 3);
            Assert.IsTrue(builtinResults2.Rows.Count == 3);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.YearMonthDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),  
                new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 3);
            Assert.IsTrue(builtinResults3.Rows.Count == 3);

            //Test exception on unknown builtIn
            Assert.ThrowsException<OWLException>(() =>
                new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#example",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?X")),
                        new SWRLVariableArgument(new RDFVariable("?Y"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));

            //Test exception on bad-formed builtIn
            Assert.ThrowsException<OWLException>(() => 
                new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#yearMonthDuration",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateYearMonthDurationBuiltInWithLeftLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Rows.Add(
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "0^^http://www.w3.org/2001/XMLSchema#int",
                "19^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.YearMonthDuration(
                new SWRLLiteralArgument(new RDFTypedLiteral("P1Y7M", RDFModelEnums.RDFDatatypes.XSD_DURATION)),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "0^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "19^^http://www.w3.org/2001/XMLSchema#int"));
        }

        [TestMethod]
        public void ShouldEvaluateYearMonthDurationBuiltInWithRightYearLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Rows.Add(
                "P1Y7M^^http://www.w3.org/2001/XMLSchema#duration",
                "7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P1Y7M^^http://www.w3.org/2001/XMLSchema#duration",
                "6^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P1Y6M^^http://www.w3.org/2001/XMLSchema#duration",
                "7^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.YearMonthDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P1Y7M^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
        }

        [TestMethod]
        public void ShouldEvaluateYearMonthBuiltInWithRightMonthLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add(
                "P1Y7M^^http://www.w3.org/2001/XMLSchema#duration",
                "1^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P1Y7M^^http://www.w3.org/2001/XMLSchema#duration",
                "2^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.YearMonthDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLLiteralArgument(new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P1Y7M^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));
        }
        #endregion
    }
}