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
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Data;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class SWRLLessThanOrEqualBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateLessThanOrEqualBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThanOrEqual", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lessThanOrEqual(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.LessThanOrEqual(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.LessThanOrEqual(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeLessThanOrEqualBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThanOrEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeLessThanOrEqualBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#lessThanOrEqual""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThanOrEqual", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lessThanOrEqual(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThanOrEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#lessThanOrEqual""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThanOrEqual\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldDeserializeLessThanOrEqualBuiltInWithLeftNumber()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#lessThanOrEqual""><Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#integer"">5</Literal><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThanOrEqual", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLLiteralArgument tlarg 
                            && tlarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lessThanOrEqual(\"5\"^^xsd:integer,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThanOrEqual\"><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeLessThanOrEqualBuiltInWithRightNumber()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#lessThanOrEqual""><Variable IRI=""urn:swrl:var#X"" /><Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#integer"">5</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThanOrEqual", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument trarg 
                            && trarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            Assert.IsTrue(string.Equals("swrlb:lessThanOrEqual(?X,\"5\"^^xsd:integer)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThanOrEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateLessThanOrEqualBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "5^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int", "-8^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value, "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
            antecedentResults.Rows.Add("hello", "hello");
            antecedentResults.Rows.Add("hello", "hello@EN-US");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
            antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("http://example.org/idv1", "http://example.org/idv2");

            SWRLBuiltIn builtin = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 5);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Y"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?X"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Y"].ToString(), "hello@EN-US"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?X"].ToString(), "http://example.org/idv1"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?Y"].ToString(), "http://example.org/idv2"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 13);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 13);

            //Test exception on unknown builtIn
            Assert.ThrowsException<SWRLException>(() =>
                new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#example",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?X")),
                        new SWRLVariableArgument(new RDFVariable("?Y"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));

            //Test exception on bad-formed builtIn
            Assert.ThrowsException<SWRLException>(() => 
                new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateLessThanOrEqualBuiltInWithLeftResource()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2.0^^http://www.w3.org/2001/XMLSchema#float");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello");
            antecedentResults.Rows.Add("hello@EN");
            antecedentResults.Rows.Add("http://example.org/test2");

            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                Arguments = [
                    new SWRLIndividualArgument(new RDFResource("http://example.org/test1")),
                    new SWRLVariableArgument(new RDFVariable("?Y"))
                ]
            };

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "http://example.org/test2"));
        }

        [TestMethod]
        public void ShouldEvaluateLessThanOrEqualBuiltInWithLeftLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2.0^^http://www.w3.org/2001/XMLSchema#float");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello");
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                Arguments = [
                    new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                    new SWRLVariableArgument(new RDFVariable("?Y"))
                ]
            };

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 3);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Y"].ToString(), "2.0^^http://www.w3.org/2001/XMLSchema#float"));
        }

        [TestMethod]
        public void ShouldEvaluateLessThanOrEqualBuiltInWithRightResource()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2.0^^http://www.w3.org/2001/XMLSchema#float");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello");
            antecedentResults.Rows.Add("hello@EN");
            antecedentResults.Rows.Add("http://example.org/test1");

            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                Arguments = [
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLIndividualArgument(new RDFResource("http://example.org/test2"))
                ]
            };

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 5);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "hello^^http://www.w3.org/2001/XMLSchema#string"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), "hello"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?X"].ToString(), "hello@EN"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?X"].ToString(), "http://example.org/test1"));
        }

        [TestMethod]
        public void ShouldEvaluateLessThanOrEqualBuiltInWithRightLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2.0^^http://www.w3.org/2001/XMLSchema#float");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello");
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#lessThanOrEqual",
                Arguments = [
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER))
                ]
            };

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 3);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "-2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), "2.0^^http://www.w3.org/2001/XMLSchema#float"));
        }
        #endregion
    }
}