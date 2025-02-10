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
    public class SWRLDateTimeBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDateTimeBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#dateTime", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.AreEqual(8, builtin.Arguments.Count);
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
            Assert.IsTrue(builtin.Arguments[5] is SWRLVariableArgument rlarg5
                            && rlarg5.GetVariable().Equals(new RDFVariable("?U")));
            Assert.IsTrue(builtin.Arguments[6] is SWRLVariableArgument rlarg6
                            && rlarg6.GetVariable().Equals(new RDFVariable("?V")));
            Assert.IsTrue(builtin.Arguments[7] is SWRLVariableArgument rlarg7
                            && rlarg7.GetVariable().Equals(new RDFVariable("?W")));
            Assert.IsTrue(string.Equals("swrlb:dateTime(?X,?Y,?Z,?Q,?T,?U,?V,?W)", builtin.ToString()));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.DateTime(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.DateTime(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeDateTimeBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#dateTime\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#T\" /><Variable IRI=\"urn:swrl:var#U\" /><Variable IRI=\"urn:swrl:var#V\" /><Variable IRI=\"urn:swrl:var#W\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeDateTimeBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#dateTime""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Variable IRI=""urn:swrl:var#Z"" /><Variable IRI=""urn:swrl:var#Q"" /><Variable IRI=""urn:swrl:var#T"" /><Variable IRI=""urn:swrl:var#U"" /><Variable IRI=""urn:swrl:var#V"" /><Variable IRI=""urn:swrl:var#W"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#dateTime", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.AreEqual(8, builtin.Arguments.Count);
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
            Assert.IsTrue(builtin.Arguments[5] is SWRLVariableArgument rlarg5
                            && rlarg5.GetVariable().Equals(new RDFVariable("?U")));
            Assert.IsTrue(builtin.Arguments[6] is SWRLVariableArgument rlarg6
                            && rlarg6.GetVariable().Equals(new RDFVariable("?V")));
            Assert.IsTrue(builtin.Arguments[7] is SWRLVariableArgument rlarg7
                            && rlarg7.GetVariable().Equals(new RDFVariable("?W")));
            Assert.IsTrue(string.Equals("swrlb:dateTime(?X,?Y,?Z,?Q,?T,?U,?V,?W)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#dateTime\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#T\" /><Variable IRI=\"urn:swrl:var#U\" /><Variable IRI=\"urn:swrl:var#V\" /><Variable IRI=\"urn:swrl:var#W\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#dateTime""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#dateTime\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Columns.Add("?V");
            antecedentResults.Columns.Add("?W");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime", 
                "2010^^http://www.w3.org/2001/XMLSchema#int", 
                "5^^http://www.w3.org/2001/XMLSchema#int", 
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "20^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(8, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T"))); 
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.AreEqual(8, builtinResults2.Columns.Count);
            Assert.AreEqual(3, builtinResults2.Rows.Count);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),  
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.AreEqual(8, builtinResults3.Columns.Count);
            Assert.AreEqual(3, builtinResults3.Rows.Count);

            SWRLBuiltIn builtin4 = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults4 = builtin4.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults4);
            Assert.AreEqual(8, builtinResults4.Columns.Count);
            Assert.AreEqual(3, builtinResults4.Rows.Count);

            SWRLBuiltIn builtin5 = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults5 = builtin5.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults5);
            Assert.AreEqual(8, builtinResults5.Columns.Count);
            Assert.AreEqual(3, builtinResults5.Rows.Count);

            SWRLBuiltIn builtin6 = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults6 = builtin6.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults6);
            Assert.AreEqual(8, builtinResults6.Columns.Count);
            Assert.AreEqual(3, builtinResults6.Rows.Count);

            SWRLBuiltIn builtin7 = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?F")));  //unexisting
            DataTable builtinResults7 = builtin7.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults7);
            Assert.AreEqual(8, builtinResults7.Columns.Count);
            Assert.AreEqual(3, builtinResults7.Rows.Count);

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
                    IRI = "http://www.w3.org/2003/11/swrlb#dateTime",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltInWithLeftLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Columns.Add("?V");
            antecedentResults.Columns.Add("?W");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2012^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLLiteralArgument(new RDFTypedLiteral("2010-05-22T10:30:30Z", RDFModelEnums.RDFDatatypes.XSD_DATETIME)),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(7, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltInWithRightYearLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Columns.Add("?V");
            antecedentResults.Columns.Add("?W");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-08-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFTypedLiteral("2010", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(7, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltInWithRightMonthLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Columns.Add("?V");
            antecedentResults.Columns.Add("?W");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-08-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLLiteralArgument(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(7, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltInWithRightDayLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Columns.Add("?V");
            antecedentResults.Columns.Add("?W");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-08-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-16-22T25:10:10Z^^http://www.w3.org/2001/XMLSchema#dateTime", //not valid
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLLiteralArgument(new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(7, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltInWithRightHourLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?V");
            antecedentResults.Columns.Add("?W");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLLiteralArgument(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(7, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "05^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "05^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }
        
        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltInWithRightMinuteLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Columns.Add("?W");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?W")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(7, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "05^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "05^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?W"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateTimeBuiltInWithRightSecondLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Columns.Add("?V");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "30^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22T10:65:30Z^^http://www.w3.org/2001/XMLSchema#time", //not valid
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "05^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "10^^http://www.w3.org/2001/XMLSchema#int",
                "34^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")),
                new SWRLVariableArgument(new RDFVariable("?V")),
                new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(7, builtinResults.Columns.Count);
            Assert.AreEqual(2, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "05^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "05^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?U"].ToString(), "10^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?V"].ToString(), "30^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }
        #endregion
    }
}