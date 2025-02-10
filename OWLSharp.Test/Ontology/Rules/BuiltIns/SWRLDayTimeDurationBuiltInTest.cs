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
    public class SWRLDayTimeDurationBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDayTimeDurationBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")));

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#dayTimeDuration", builtin.IRI));
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
                            && rlarg4.GetVariable().Equals(new RDFVariable("?U")));
            Assert.IsTrue(string.Equals("swrlb:dayTimeDuration(?X,?Y,?Z,?Q,?U)", builtin.ToString()));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.DayTimeDuration(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<SWRLException>(() => SWRLBuiltIn.DayTimeDuration(new SWRLVariableArgument(new RDFVariable("?X")), null));
        }

        [TestMethod]
        public void ShouldSerializeDayTimeDurationBuiltIn()
        {
            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")));

            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#dayTimeDuration\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#U\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
        }

        [TestMethod]
        public void ShouldDeserializeDayTimeDurationBuiltIn()
        {
            SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#dayTimeDuration""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Variable IRI=""urn:swrl:var#Z"" /><Variable IRI=""urn:swrl:var#Q"" /><Variable IRI=""urn:swrl:var#U"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#dayTimeDuration", builtin.IRI));
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
                            && rlarg4.GetVariable().Equals(new RDFVariable("?U")));
            Assert.IsTrue(string.Equals("swrlb:dayTimeDuration(?X,?Y,?Z,?Q,?U)", builtin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#dayTimeDuration\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /><Variable IRI=\"urn:swrl:var#U\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

            //Test string handling for empty builtIns

            SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#dayTimeDuration""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#dayTimeDuration\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldEvaluateDayTimeDurationBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration", 
                "1^^http://www.w3.org/2001/XMLSchema#int", 
                "7^^http://www.w3.org/2001/XMLSchema#int", 
                "6^^http://www.w3.org/2001/XMLSchema#int", 
                "12^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
               "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration", 
                "1^^http://www.w3.org/2001/XMLSchema#int", 
                "7^^http://www.w3.org/2001/XMLSchema#int", 
                "8^^http://www.w3.org/2001/XMLSchema#int", 
                "12^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(5, builtinResults.Columns.Count);
            Assert.AreEqual(1, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "6^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "12^^http://www.w3.org/2001/XMLSchema#int"));

            //Test with unexisting variables

            SWRLBuiltIn builtin2 = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U"))); 
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.AreEqual(5, builtinResults2.Columns.Count);
            Assert.AreEqual(2, builtinResults2.Rows.Count);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),  
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.AreEqual(5, builtinResults3.Columns.Count);
            Assert.AreEqual(2, builtinResults3.Rows.Count);

            SWRLBuiltIn builtin4 = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),  
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?U")));
            DataTable builtinResults4 = builtin4.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults4);
            Assert.AreEqual(5, builtinResults4.Columns.Count);
            Assert.AreEqual(2, builtinResults4.Rows.Count);

            SWRLBuiltIn builtin5 = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),  
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
            DataTable builtinResults5 = builtin5.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults5);
            Assert.AreEqual(5, builtinResults5.Columns.Count);
            Assert.AreEqual(2, builtinResults5.Rows.Count);

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
                    IRI = "http://www.w3.org/2003/11/swrlb#dayTimeDuration",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?V"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));
        }

        [TestMethod]
        public void ShouldEvaluateDayTimeDurationBuiltInWithLeftLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Rows.Add(
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "7^^http://www.w3.org/2001/XMLSchema#int",
                "6^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "7^^http://www.w3.org/2001/XMLSchema#int",
                "9^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLLiteralArgument(new RDFTypedLiteral("P1DT7H6M12S", RDFModelEnums.RDFDatatypes.XSD_DURATION)),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(1, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "6^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "12^^http://www.w3.org/2001/XMLSchema#int"));
        }

        [TestMethod]
        public void ShouldEvaluateDayTimeDurationBuiltInWithRightDayLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "7^^http://www.w3.org/2001/XMLSchema#int",
                "6^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "7^^http://www.w3.org/2001/XMLSchema#int",
                "9^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(RDFTypedLiteral.One),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(1, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "6^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "12^^http://www.w3.org/2001/XMLSchema#int"));
        }

        [TestMethod]
        public void ShouldEvaluateDayTimeBuiltInWithRightHourLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "6^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "9^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLLiteralArgument(new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLVariableArgument(new RDFVariable("?U")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(1, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "6^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "12^^http://www.w3.org/2001/XMLSchema#int"));
        }
        
        [TestMethod]
        public void ShouldEvaluateDayTimeBuiltInWithRightMinuteLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?U");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "7^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "9^^http://www.w3.org/2001/XMLSchema#int",
                "12^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLLiteralArgument(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?U")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(1, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?U"].ToString(), "12^^http://www.w3.org/2001/XMLSchema#int"));
        }

        [TestMethod]
        public void ShouldEvaluateDayTimeBuiltInWithRightSecondLiteral()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Columns.Add("?Q");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "7^^http://www.w3.org/2001/XMLSchema#int",
                "6^^http://www.w3.org/2001/XMLSchema#int");
            antecedentResults.Rows.Add(
                "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration",
                "1^^http://www.w3.org/2001/XMLSchema#int",
                "9^^http://www.w3.org/2001/XMLSchema#int",
                "6^^http://www.w3.org/2001/XMLSchema#int");

            SWRLBuiltIn builtin = SWRLBuiltIn.DayTimeDuration(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")),
                new SWRLVariableArgument(new RDFVariable("?Q")),
                new SWRLLiteralArgument(new RDFTypedLiteral("12", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.AreEqual(4, builtinResults.Columns.Count);
            Assert.AreEqual(1, builtinResults.Rows.Count);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "P1DT7H6M12S^^http://www.w3.org/2001/XMLSchema#duration"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "1^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "7^^http://www.w3.org/2001/XMLSchema#int"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "6^^http://www.w3.org/2001/XMLSchema#int"));
        }
        #endregion
    }
}