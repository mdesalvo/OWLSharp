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

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLBuiltInTest
    {
        #region Tests
        /*
        //ComparisonFilter

        [TestMethod]
        public void ShouldCreateGreaterThanBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.GreaterThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#greaterThan", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:greaterThan(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.GreaterThan(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.GreaterThan(new SWRLVariableArgument(new RDFVariable("?X")), null as SWRLVariableArgument));
        }

        [TestMethod]
        public void ShouldSerializeGreaterThanBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.GreaterThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#greaterThan\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeGreaterThanBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#greaterThan""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#greaterThan", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:greaterThan(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#greaterThan\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateGreaterThanBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "1^^http://www.w3.org/2001/XMLSchema#int");
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

            SWRLBuiltIn builtin = SWRLBuiltIn.GreaterThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.GreaterThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.GreaterThan(
                new SWRLVariableArgument(new RDFVariable("?Z")), //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateGreaterThanOrEqualBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.GreaterThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#greaterThanOrEqual", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:greaterThanOrEqual(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.GreaterThanOrEqual(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.GreaterThanOrEqual(new SWRLVariableArgument(new RDFVariable("?X")), null as SWRLVariableArgument));
        }

        [TestMethod]
        public void ShouldSerializeGreaterThanOrEqualBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.GreaterThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#greaterThanOrEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeGreaterThanOrEqualBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#greaterThanOrEqual""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#greaterThanOrEqual", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:greaterThanOrEqual(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#greaterThanOrEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateGreaterThanOrEqualBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int");
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

            SWRLBuiltIn builtin = SWRLBuiltIn.GreaterThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), DBNull.Value.ToString()));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), DBNull.Value.ToString()));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.GreaterThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.GreaterThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?Z")), //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateLessThanOrEqualBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThanOrEqual", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lessThanOrEqual(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.LessThanOrEqual(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.LessThanOrEqual(new SWRLVariableArgument(new RDFVariable("?X")), null as SWRLVariableArgument));
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
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThanOrEqual", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lessThanOrEqual(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThanOrEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateLessThanOrEqualBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value, "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
            antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
            antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), DBNull.Value.ToString()));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), DBNull.Value.ToString()));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.LessThanOrEqual(
                new SWRLVariableArgument(new RDFVariable("?Z")), //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateLessThanBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.LessThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThan", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lessThan(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.LessThan(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.LessThan(new SWRLVariableArgument(new RDFVariable("?X")), null as SWRLVariableArgument));
        }

        [TestMethod]
        public void ShouldSerializeLessThanBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.LessThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThan\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeLessThanBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#lessThan""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lessThan", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lessThan(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lessThan\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateLessThanBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "3^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value, "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
            antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
            antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
            antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.LessThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "3^^http://www.w3.org/2001/XMLSchema#int"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.LessThan(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.LessThan(
                new SWRLVariableArgument(new RDFVariable("?Z")), //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateNotEqualBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.NotEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#notEqual", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:notEqual(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Equal(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Equal(new SWRLVariableArgument(new RDFVariable("?X")), null as SWRLVariableArgument));
        }

        [TestMethod]
        public void ShouldSerializeNotEqualBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.NotEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#notEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeNotEqualBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#notEqual""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsTrue(builtin.IsComparisonFilterBuiltIn);
            Assert.IsFalse(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#notEqual", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:notEqual(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#notEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateNotEqualBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int");
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

            SWRLBuiltIn builtin = SWRLBuiltIn.NotEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "-2^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.NotEqual(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.NotEqual(
                new SWRLVariableArgument(new RDFVariable("?Z")), //unexisting
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        //StringFilter

        [TestMethod]
        public void ShouldCreateContainsBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Contains(
                new SWRLVariableArgument(new RDFVariable("?X")),
                "hello");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#contains", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:contains(?X,\"hello\")", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Contains(null, "hello"));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Contains(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeContainsBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Contains(
                new SWRLVariableArgument(new RDFVariable("?X")), "hello");

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#contains\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeContainsBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#contains""><Variable IRI=""urn:swrl:var#X"" /><Literal>hello</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#contains", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:contains(?X,\"hello\")", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#contains\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateContainsBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("FC Internazionale Milano is the best");
            antecedentResults.Rows.Add("inter is the best");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = SWRLBuiltIn.Contains(
                new SWRLVariableArgument(new RDFVariable("?X")), "Inter");

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "FC Internazionale Milano is the best"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Contains(
                new SWRLVariableArgument(new RDFVariable("?Z")), "hello"); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 1);
            Assert.IsTrue(builtinResults2.Rows.Count == 5); //SPARQL evaluates true every row if the column is unknown
        }

        [TestMethod]
        public void ShouldCreateContainsIgnoreCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.ContainsIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?X")),
                "hello");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#containsIgnoreCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:containsIgnoreCase(?X,\"hello\")", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.ContainsIgnoreCase(null, "hello"));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.ContainsIgnoreCase(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeContainsIgnoreCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.ContainsIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?X")), "hello");

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#containsIgnoreCase\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeContainsIgnoreCaseBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#containsIgnoreCase""><Variable IRI=""urn:swrl:var#X"" /><Literal>hello</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#containsIgnoreCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:containsIgnoreCase(?X,\"hello\")", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#containsIgnoreCase\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateContainsIgnoreCaseBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("FC Internazionale Milano is the best");
            antecedentResults.Rows.Add("inter is the best");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = SWRLBuiltIn.ContainsIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?X")), "Inter");

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "FC Internazionale Milano is the best"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "inter is the best"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.ContainsIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?Z")), "hello"); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 1);
            Assert.IsTrue(builtinResults2.Rows.Count == 5); //SPARQL evaluates true every row if the column is unknown
        }

        [TestMethod]
        public void ShouldCreateEndsWithBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.EndsWith(
                new SWRLVariableArgument(new RDFVariable("?X")),
                "hello");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#endsWith", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:endsWith(?X,\"hello\")", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.EndsWith(null, "hello"));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.EndsWith(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeEndsWithBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.EndsWith(
                new SWRLVariableArgument(new RDFVariable("?X")), "hello");

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#endsWith\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeEndsWithBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#endsWith""><Variable IRI=""urn:swrl:var#X"" /><Literal>hello</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#endsWith", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:endsWith(?X,\"hello\")", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#endsWith\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateEndsWithBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("FC Internazionale Milano is the best!");
            antecedentResults.Rows.Add("inter is the best");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = SWRLBuiltIn.EndsWith(
                new SWRLVariableArgument(new RDFVariable("?X")), "best!");

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "FC Internazionale Milano is the best!"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.EndsWith(
                new SWRLVariableArgument(new RDFVariable("?Z")), "hello"); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 1);
            Assert.IsTrue(builtinResults2.Rows.Count == 5); //SPARQL evaluates true every row if the column is unknown
        }

        [TestMethod]
        public void ShouldCreateLowerCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.LowerCase(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lowerCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lowerCase(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.LowerCase(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.LowerCase(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeLowerCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.LowerCase(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lowerCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeLowerCaseBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#lowerCase""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#lowerCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:lowerCase(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#lowerCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateLowerCaseBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("INTER", "intEr");
            antecedentResults.Rows.Add("inter", "INTER");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int",DBNull.Value);
            antecedentResults.Rows.Add("hello@EN", "http://example.org/");

            SWRLBuiltIn builtin = SWRLBuiltIn.LowerCase(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "inter"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "INTER"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.LowerCase(
                new SWRLVariableArgument(new RDFVariable("?Z")), new SWRLVariableArgument(new RDFVariable("?X"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateMatchesBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?X")),
                "hello$");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello$")));
            Assert.IsTrue(string.Equals("swrlb:matches(?X,\"hello$\")", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Matches(null, "hello$"));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Matches(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldCreateMatchesOptionBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?X")),
                "hello$", 
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
            Assert.IsNotNull(builtin.Literal);
            Assert.IsTrue(builtin.Literal.GetLiteral().Equals(new RDFPlainLiteral("ismx")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello$")));
            Assert.IsTrue(string.Equals("swrlb:matches(?X,\"hello$\",\"ismx\")", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Matches(null, "hello$", RegexOptions.IgnoreCase));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Matches(new SWRLVariableArgument(new RDFVariable("?X")), null, RegexOptions.IgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeMatchesBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?X")), "hello$");

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello$</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldSerializeMatchesOptionBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?X")), "hello$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello$</Literal><Literal>ismx</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeMatchesBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#matches""><Variable IRI=""urn:swrl:var#X"" /><Literal>hello$</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello$")));
            Assert.IsTrue(string.Equals("swrlb:matches(?X,\"hello$\")", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello$</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeMatchesOptionBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#matches""><Variable IRI=""urn:swrl:var#X"" /><Literal>hello$</Literal><Literal>ismx</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
            Assert.IsNotNull(builtin.Literal);
            Assert.IsTrue(builtin.Literal.GetLiteral().Equals(new RDFPlainLiteral("ismx")));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello$")));
            Assert.IsTrue(string.Equals("swrlb:matches(?X,\"hello$\",\"ismx\")", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello$</Literal><Literal>ismx</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateMatchesBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("FC Internazionale Milano is the best");
            antecedentResults.Rows.Add("inter is the Best");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?X")), "is the best");

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "FC Internazionale Milano is the best"));
            
            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?Z")), "hello$"); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 1);
            Assert.IsTrue(builtinResults2.Rows.Count == 5); //SPARQL evaluates true every row if the column is unknown
        }

        [TestMethod]
        public void ShouldEvaluateMatchesOptionBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("FC Internazionale Milano is the best");
            antecedentResults.Rows.Add("inter is the Best");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?X")), "is the best", RegexOptions.IgnoreCase);

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 2);

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Matches(
                new SWRLVariableArgument(new RDFVariable("?Z")), "hello$"); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 1);
            Assert.IsTrue(builtinResults2.Rows.Count == 5); //SPARQL evaluates true every row if the column is unknown
        }

        [TestMethod]
        public void ShouldCreateStartsWithBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StartsWith(
                new SWRLVariableArgument(new RDFVariable("?X")),
                "hello");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#startsWith", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:startsWith(?X,\"hello\")", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StartsWith(null, "hello"));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StartsWith(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeStartsWithBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StartsWith(
                new SWRLVariableArgument(new RDFVariable("?X")), "hello");

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#startsWith\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeStartsWithBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#startsWith""><Variable IRI=""urn:swrl:var#X"" /><Literal>hello</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#startsWith", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLLiteralArgument rlarg
                            && rlarg.GetLiteral().Equals(new RDFPlainLiteral("hello")));
            Assert.IsTrue(string.Equals("swrlb:startsWith(?X,\"hello\")", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#startsWith\"><Variable IRI=\"urn:swrl:var#X\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateStartsWithBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Rows.Add("FC Internazionale Milano is the best!");
            antecedentResults.Rows.Add("inter is the best");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(DBNull.Value);
            antecedentResults.Rows.Add("hello@EN");

            SWRLBuiltIn builtin = SWRLBuiltIn.StartsWith(
                new SWRLVariableArgument(new RDFVariable("?X")), "FC Internazionale");

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 1);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "FC Internazionale Milano is the best!"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.StartsWith(
                new SWRLVariableArgument(new RDFVariable("?Z")), "hello"); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 1);
            Assert.IsTrue(builtinResults2.Rows.Count == 5); //SPARQL evaluates true every row if the column is unknown
        }

        [TestMethod]
        public void ShouldCreateStringConcatBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StringConcat(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                "hello");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringConcat", builtin.IRI));
            Assert.IsNotNull(builtin.Literal);
            Assert.IsTrue(string.Equals("hello", builtin.Literal.GetLiteral().ToString()));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StringConcat(null, new SWRLVariableArgument(new RDFVariable("?X")), "hello"));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StringConcat(new SWRLVariableArgument(new RDFVariable("?X")), null, "hello"));
        }

        [TestMethod]
        public void ShouldSerializeStringConcatBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StringConcat(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                "hello");

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringConcat\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeStringConcatBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#stringConcat""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Literal>hello</Literal></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringConcat", builtin.IRI));
            Assert.IsNotNull(builtin.Literal);
            Assert.IsTrue(string.Equals("hello", builtin.Literal.GetLiteral().ToString()));
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:stringConcat(?X,?Y,\"hello\")", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringConcat\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Literal>hello</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateStringConcatBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("My name is John, hello!", "My name is John, ");
            antecedentResults.Rows.Add("My name is John, hello!", DBNull.Value);
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int", "hello!");
            antecedentResults.Rows.Add("hello!",DBNull.Value);
            antecedentResults.Rows.Add(DBNull.Value,DBNull.Value);
            antecedentResults.Rows.Add("hello!","hello!");


            SWRLBuiltIn builtin = SWRLBuiltIn.StringConcat(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                "hello!");

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "My name is John, hello!"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "My name is John, "));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "hello!"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), string.Empty));

            //Test without literal

            SWRLBuiltIn builtin2 = SWRLBuiltIn.StringConcat(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                null);

            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults2.Rows[0]["?X"].ToString(), string.Empty));
            Assert.IsTrue(string.Equals(builtinResults2.Rows[0]["?Y"].ToString(), string.Empty));
            Assert.IsTrue(string.Equals(builtinResults2.Rows[1]["?X"].ToString(), "hello!"));
            Assert.IsTrue(string.Equals(builtinResults2.Rows[1]["?Y"].ToString(), "hello!"));

            //Test with unexisting variables

            SWRLBuiltIn builtin3 = SWRLBuiltIn.StringConcat(
                new SWRLVariableArgument(new RDFVariable("?Z")), new SWRLVariableArgument(new RDFVariable("?X")), "hello"); //unexisting
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 2);
            Assert.IsTrue(builtinResults3.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateStringEqualIgnoreCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StringEqualIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:stringEqualIgnoreCase(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StringEqualIgnoreCase(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StringEqualIgnoreCase(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeStringEqualIgnoreCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StringEqualIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeStringEqualIgnoreCaseBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:stringEqualIgnoreCase(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringEqualIgnoreCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateStringEqualIgnoreCaseBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("INtER", "intEr");
            antecedentResults.Rows.Add("inter", "INTER");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int",DBNull.Value);
            antecedentResults.Rows.Add("hello@EN", "http://example.org/");

            SWRLBuiltIn builtin = SWRLBuiltIn.StringEqualIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "INtER"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "intEr"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "inter"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "INTER"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.StringEqualIgnoreCase(
                new SWRLVariableArgument(new RDFVariable("?Z")), new SWRLVariableArgument(new RDFVariable("?X"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateStringLengthBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StringLength(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringLength", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:stringLength(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StringLength(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.StringLength(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeStringLengthBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.StringLength(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringLength\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeStringLengthBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#stringLength""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringLength", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:stringLength(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringLength\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateStringLengthBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("5^^http://www.w3.org/2001/XMLSchema#integer", "intEr");
            antecedentResults.Rows.Add("11^^http://www.w3.org/2001/XMLSchema#integer", "INTER@en-US");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int",DBNull.Value);
            antecedentResults.Rows.Add("hello@EN", "http://example.org/");

            SWRLBuiltIn builtin = SWRLBuiltIn.StringLength(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#integer"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "intEr"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.StringLength(
                new SWRLVariableArgument(new RDFVariable("?Z")), new SWRLVariableArgument(new RDFVariable("?X"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);
        }

        [TestMethod]
        public void ShouldCreateUpperCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#upperCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:upperCase(?X,?Y)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.UpperCase(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.UpperCase(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeUpperCaseBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#upperCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeUpperCaseBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#upperCase""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsFalse(builtin.IsBooleanBuiltIn);
            Assert.IsFalse(builtin.IsMathBuiltIn);
            Assert.IsFalse(builtin.IsComparisonFilterBuiltIn);
            Assert.IsTrue(builtin.IsStringFilterBuiltIn);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#upperCase", builtin.IRI));
            Assert.IsNull(builtin.Literal);
            Assert.IsNotNull(builtin.LeftArgument);
            Assert.IsTrue(builtin.LeftArgument is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsNotNull(builtin.RightArgument);
            Assert.IsTrue(builtin.RightArgument is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(string.Equals("swrlb:upperCase(?X,?Y)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#upperCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldEvaluateUpperCaseBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("INTER", "intEr");
            antecedentResults.Rows.Add("inter", "INTER");
            antecedentResults.Rows.Add("-2^^http://www.w3.org/2001/XMLSchema#int",DBNull.Value);
            antecedentResults.Rows.Add("hello@EN", "http://example.org/");

            SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
                new SWRLVariableArgument(new RDFVariable("?X")), new SWRLVariableArgument(new RDFVariable("?Y")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "INTER"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "intEr"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.UpperCase(
                new SWRLVariableArgument(new RDFVariable("?Z")), new SWRLVariableArgument(new RDFVariable("?X"))); //unexisting
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 2);
            Assert.IsTrue(builtinResults2.Rows.Count == 0);
        }
        */
        #endregion
    }
}