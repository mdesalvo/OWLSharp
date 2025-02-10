/*
   Copyright 2014-2025 Marco De Salvo

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
using System.Data;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class SWRLTimeBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateTimeBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#time", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.AreEqual(5, builtin.Arguments.Count);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2 
                            && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
            Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3 
                            && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
            Assert.IsTrue(builtin.Arguments[4] is SWRLVariableArgument rlarg4
                            && rlarg4.GetVariable().Equals(new RDFVariable("?T")));
            Assert.IsTrue(string.Equals("swrlb:time(?X,?Y,?Z,?Q,?T)", builtin.ToString()));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.Time(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.Time(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeTimeBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#time\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#T\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeTimeBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#time""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Variable IRI=""urn:swrl:var#Z"" /><Variable IRI=""urn:swrl:var#Q"" /><Variable IRI=""urn:swrl:var#T"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#time", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.AreEqual(5, builtin.Arguments.Count);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                            && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
            Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3
                            && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
            Assert.IsTrue(builtin.Arguments[4] is SWRLVariableArgument rlarg4
                            && rlarg4.GetVariable().Equals(new RDFVariable("?T")));
            Assert.IsTrue(string.Equals("swrlb:time(?X,?Y,?Z,?Q,?T)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#time\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#T\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#time""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#time\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldEvaluateTimeBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", 
                "10^^http://www.w3.org/2001/XMLSchema#int", 
                "30^^http://www.w3.org/2001/XMLSchema#int", 
                "30^^http://www.w3.org/2001/XMLSchema#int", 
                "UTC");
            antecedentResults.Rows.Add(
                "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "10:34:30Z^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(5, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T"))); 
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.AreEqual(5, builtinResults2.Columns.Count);
            Assert.AreEqual(3, builtinResults2.Rows.Count);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),  
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.AreEqual(5, builtinResults3.Columns.Count);
            Assert.AreEqual(3, builtinResults3.Rows.Count);

            SWRLBuiltIn builtin4 = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults4 = builtin4.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults4);
            Assert.AreEqual(5, builtinResults4.Columns.Count);
            Assert.AreEqual(3, builtinResults4.Rows.Count);

            SWRLBuiltIn builtin5 = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
            DataTable builtinResults5 = builtin5.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults5);
            Assert.AreEqual(5, builtinResults5.Columns.Count);
            Assert.AreEqual(3, builtinResults5.Rows.Count);

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
                    IRI = "http://www.w3.org/2003/11/swrlb#time",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateTimeBuiltInWithLeftLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Time(
                new SWRLLiteralArgument(new RDFTypedLiteral("10:30:30Z", RDFModelEnums.RDFDatatypes.XSD_TIME)),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateTimeBuiltInWithRightHourLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateTimeBuiltInWithRightMinuteLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateTimeBuiltInWithRightSecondLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "10:65:30Z^^http://www.w3.org/2001/XMLSchema#time", //not valid
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Time(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }
        #endregion
    }
}