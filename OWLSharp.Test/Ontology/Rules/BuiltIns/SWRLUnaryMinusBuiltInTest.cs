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
    public class SWRLUnaryMinusBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateUnaryMinusBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:unaryMinus(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.UnaryMinus(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.UnaryMinus(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeUnaryMinusBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeUnaryMinusBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#unaryMinus""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:unaryMinus(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#unaryMinus""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldDeserializeUnaryMinusBuiltInWithLeftNumber()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#unaryMinus""><Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#integer"">5</Literal><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLLiteralArgument tlarg 
                            && tlarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:unaryMinus(\"5\"^^xsd:integer,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeUnaryMinusBuiltInWithRightNumber()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#unaryMinus""><Variable IRI=""urn:swrl:var#X"" /><Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#integer"">5</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 2);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument trarg 
                            && trarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            Assert.IsTrue(string.Equals("swrlb:unaryMinus(?X,\"5\"^^xsd:integer)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateUnaryMinusBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "-2.00^^http://www.w3.org/2001/XMLSchema#double");
            antecedentResults.Rows.Add("-3^^http://www.w3.org/2001/XMLSchema#int", "3^^http://www.w3.org/2001/XMLSchema#positiveInteger");
            antecedentResults.Rows.Add(DBNull.Value, "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
            antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
            antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "http://example.org/tests");

            SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "-2.00^^http://www.w3.org/2001/XMLSchema#double"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "-3^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "3^^http://www.w3.org/2001/XMLSchema#positiveInteger"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.UnaryMinus(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 12);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.UnaryMinus(
                new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 12);

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
                    IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateUnaryMinusBuiltInWithLeftNumber()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#double");
            antecedentResults.Rows.Add("2.73^^http://www.w3.org/2001/XMLSchema#double");
            antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello");
            antecedentResults.Rows.Add("hello@EN");
            antecedentResults.Rows.Add("http://example.org/tests");

            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
                Arguments = [
                    new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                    new SWRLVariableArgument(new RDFVariable("?Y"))
                ]
            };

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "-2^^http://www.w3.org/2001/XMLSchema#double"));
        }

        [TestMethod]
        public void ShouldEvaluateUnaryMinusBuiltInWithRightNumber()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("-2.73^^http://www.w3.org/2001/XMLSchema#double");
            antecedentResults.Rows.Add("3^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2.0^^http://www.w3.org/2001/XMLSchema#float");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello");
            antecedentResults.Rows.Add("hello@EN");
            antecedentResults.Rows.Add("http://example.org/test");

            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
                Arguments = [
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLLiteralArgument(new RDFTypedLiteral("2.73", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
                ]
            };

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "-2.73^^http://www.w3.org/2001/XMLSchema#double"));
        }
        #endregion
    }
}