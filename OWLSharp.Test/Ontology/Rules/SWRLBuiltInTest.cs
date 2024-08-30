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
    public class SWRLBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateAbsBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Abs(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsTrue(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#abs", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:abs(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Abs(null,new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Abs(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeAbsBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Abs(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#abs\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeAbsBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#abs""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsTrue(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#abs", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:abs(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#abs\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateAbsBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value, "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
            antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
            antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.Abs(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "-2^^http://www.w3.org/2001/XMLSchema#int"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Abs(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.Abs(
                new SWRLVariableArgument(new RDFVariable("?Z")), //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateAddBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Add(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")), 
                1.55);

            Assert.IsNotNull(builtin);
            Assert.IsTrue(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#add", builtin.IRI));
            Assert.IsNotNull(builtin.Literal);
            Assert.IsTrue(builtin.Literal.GetLiteral().Equals(new RDFTypedLiteral("1.55", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:add(?X,?Y,\"1.55\"^^xsd:double)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Add(null, new SWRLVariableArgument(new RDFVariable("?Y")), 1.55));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Add(new SWRLVariableArgument(new RDFVariable("?X")), null, 1.55));
        }

        [TestMethod]
        public void ShouldSerializeAddBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Add(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                1.55);

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#add\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#double\">1.55</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeAddBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#add""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#double"">1.55</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsTrue(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#add", builtin.IRI));
            Assert.IsNotNull(builtin.Literal);
            Assert.IsTrue(builtin.Literal.GetLiteral().Equals(new RDFTypedLiteral("1.55", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:add(?X,?Y,\"1.55\"^^xsd:double)", builtin.ToString()));
        }

        [TestMethod]
        public void ShouldEvaluateAddBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "1^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("1^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value, "0^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("1^^http://www.w3.org/2001/XMLSchema#int", DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add("1^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
            antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
            antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.Add(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                1);

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Abs(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.Abs(
                new SWRLVariableArgument(new RDFVariable("?Z")), //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldEvaluateAddBuiltMissingLiteralValue()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "1^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#add""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 0);
        }
        #endregion
    }
}