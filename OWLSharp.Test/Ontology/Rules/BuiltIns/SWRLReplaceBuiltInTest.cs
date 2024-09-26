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
using System;
using System.Data;

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLReplaceBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateReplaceBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#replace", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 4);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg1
                            && rlarg1.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2 
                            && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
            Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3 
                            && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
            Assert.IsTrue(string.Equals("swrlb:replace(?X,?Y,?Z,?Q)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Replace(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Replace(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeReplaceBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#replace\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }


        [TestMethod]
        public void ShouldDeserializeReplaceBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#replace""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Variable IRI=""urn:swrl:var#Z"" /><Variable IRI=""urn:swrl:var#Q"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#replace", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 4);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg1
                            && rlarg1.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2 
                            && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
            Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3 
                            && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
            Assert.IsTrue(string.Equals("swrlb:replace(?X,?Y,?Z,?Q)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#replace\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#replace""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#replace\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldEvaluateReplaceBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Rows.Add("heMMo", "hello", "ll", "MM");
            antecedentResults.Rows.Add("hemMo", "hello", "ll", "MM");
            antecedentResults.Rows.Add("heMMo@EN", "hello@EN", "ll@EN", "MM@EN");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2", "2", "2");
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, "hello", "[a-z]+", "");
            antecedentResults.Rows.Add("hello", DBNull.Value, "hello", "hello");
            antecedentResults.Rows.Add("http://example.org/test/", "ftp://example.org/test/", "ftp", "http");

            SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 4);
            Assert.IsTrue(builtinResults.Rows.Count == 5);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "heMMo"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "ll"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "MM"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "heMMo@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "hello@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "ll@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "MM@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Y"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Z"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Q"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Y"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Z"].ToString(), "[a-z]+"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Q"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?X"].ToString(), "http://example.org/test/"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?Y"].ToString(), "ftp://example.org/test/"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?Z"].ToString(), "ftp"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?Q"].ToString(), "http"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Replace(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?P"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 4);
            Assert.IsTrue(builtinResults2.Rows.Count == 8);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.Replace(
                new SWRLVariableArgument(new RDFVariable("?P")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 4);
            Assert.IsTrue(builtinResults3.Rows.Count == 8);

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
                    IRI = "http://www.w3.org/2003/11/swrlb#replace",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateReplaceBuiltInWithSTRLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Rows.Add("heMMo", "ll", "MM");
            antecedentResults.Rows.Add("hemMo", "ll", "MM");
            antecedentResults.Rows.Add("heMMo@EN", "ll@EN", "MM@EN");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2", "2");
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, "[a-z]+", "");
            antecedentResults.Rows.Add("hello", "hello", "hello");
            antecedentResults.Rows.Add("http://example.org/test/", "ftp", "http");

            SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFPlainLiteral("hello","EN")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 3);
            Assert.IsTrue(builtinResults.Rows.Count == 4);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "heMMo"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "ll"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "MM"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "heMMo@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "ll@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "MM@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Z"].ToString(), "[a-z]+"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Q"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?X"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Z"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Q"].ToString(), "hello"));
        }

        [TestMethod]
        public void ShouldEvaluateReplaceBuiltInWithRPLLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Rows.Add("heMMo", "hello", "ll");
            antecedentResults.Rows.Add("hemMo", "hello", "ll");
            antecedentResults.Rows.Add("heMMo@EN", "hello@EN", "ll@EN");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2", "2");
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, "hello", "[a-z]+");
            antecedentResults.Rows.Add("hello", DBNull.Value, "hello");
            antecedentResults.Rows.Add("http://example.org/test/", "ftp://example.org/test/", "ftp");

            SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLLiteralArgument(new RDFTypedLiteral("MM", RDFModelEnums.RDFDatatypes.XSD_STRING)));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 3);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "heMMo"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "ll"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "heMMo@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "hello@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "ll@EN"));
        }
        #endregion
    }
}