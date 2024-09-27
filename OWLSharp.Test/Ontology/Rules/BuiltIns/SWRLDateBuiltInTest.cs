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
    public class SWRLDateBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDateBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#date", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 5);
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
            Assert.IsTrue(string.Equals("swrlb:date(?X,?Y,?Z,?Q,?T)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Date(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.Date(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeDateBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#date\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#T\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeDateBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#date""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Variable IRI=""urn:swrl:var#Z"" /><Variable IRI=""urn:swrl:var#Q"" /><Variable IRI=""urn:swrl:var#T"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#date", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 5);
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
            Assert.IsTrue(string.Equals("swrlb:date(?X,?Y,?Z,?Q,?T)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#date\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#T\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#date""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#date\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldEvaluateDateBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date", 
                "2010^^http://www.w3.org/2001/XMLSchema#int", 
                "5^^http://www.w3.org/2001/XMLSchema#int", 
                "22^^http://www.w3.org/2001/XMLSchema#int", 
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-08-22Z^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 5);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T"))); 
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 5);
            Assert.IsTrue(builtinResults2.Rows.Count == 3);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),  
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 5);
            Assert.IsTrue(builtinResults3.Rows.Count == 3);

            SWRLBuiltIn builtin4 = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?T")));
            DataTable builtinResults4 = builtin4.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults4);
            Assert.IsTrue(builtinResults4.Columns.Count == 5);
            Assert.IsTrue(builtinResults4.Rows.Count == 3);

            SWRLBuiltIn builtin5 = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
            DataTable builtinResults5 = builtin5.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults5);
            Assert.IsTrue(builtinResults5.Columns.Count == 5);
            Assert.IsTrue(builtinResults5.Rows.Count == 3);

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
                    IRI = "http://www.w3.org/2003/11/swrlb#date",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateDateBuiltInWithLeftLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2012^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Date(
                new SWRLLiteralArgument(new RDFTypedLiteral("2010-05-22Z", RDFModelEnums.RDFDatatypes.XSD_DATE)),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 4);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateBuiltInWithRightYearLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22^^http://www.w3.org/2001/XMLSchema#date",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-08-22Z^^http://www.w3.org/2001/XMLSchema#date",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFTypedLiteral("2010", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 4);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateBuiltInWithRightMonthLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-08-22Z^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "22^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLLiteralArgument(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 4);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "22^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }

        [TestMethod]
        public void ShouldEvaluateDateBuiltInWithRightDayLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?T");
            antecedentResults.Rows.Add(
                "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-05-22^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                ""); //Will fallback to "UTC"
            antecedentResults.Rows.Add(
                "2010-08-22Z^^http://www.w3.org/2001/XMLSchema#date",
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");
            antecedentResults.Rows.Add(
                "2010-16-22Z^^http://www.w3.org/2001/XMLSchema#date", //not valid
                "2010^^http://www.w3.org/2001/XMLSchema#int",
                "5^^http://www.w3.org/2001/XMLSchema#int",
                "UTC");

            SWRLBuiltIn builtin = SWRLBuiltIn.Date(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLLiteralArgument(new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?T")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 4);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "2010-05-22Z^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?T"].ToString(), "UTC"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "2010-05-22^^http://www.w3.org/2001/XMLSchema#date"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "2010^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "5^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?T"].ToString(), ""));
        }
        #endregion
    }
}