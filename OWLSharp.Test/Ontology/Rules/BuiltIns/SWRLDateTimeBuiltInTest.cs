/*
   Copyright 2014-2026 Marco De Salvo

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
using OWLSharp;

namespace OWLSharp.Test.Ontology;

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
        Assert.HasCount(8, builtin.Arguments);
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
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.DateTime(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.DateTime(new SWRLVariableArgument(new RDFVariable("?X")), null));
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
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#dateTime"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Variable IRI="urn:swrl:var#Z" /><Variable IRI="urn:swrl:var#Q" /><Variable IRI="urn:swrl:var#T" /><Variable IRI="urn:swrl:var#U" /><Variable IRI="urn:swrl:var#V" /><Variable IRI="urn:swrl:var#W" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#dateTime", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(8, builtin.Arguments);
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
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#dateTime"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#dateTime\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldEvaluateDateTimeBuiltIn()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?U");
        antecedentResults.AddColumn("?V");
        antecedentResults.AddColumn("?W");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "20^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(8, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));

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
        OWLTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(8, builtinResults2.Columns);
        Assert.HasCount(3, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        OWLTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(8, builtinResults3.Columns);
        Assert.HasCount(3, builtinResults3.Rows);

        SWRLBuiltIn builtin4 = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        OWLTable builtinResults4 = builtin4.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults4);
        Assert.HasCount(8, builtinResults4.Columns);
        Assert.HasCount(3, builtinResults4.Rows);

        SWRLBuiltIn builtin5 = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        OWLTable builtinResults5 = builtin5.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults5);
        Assert.HasCount(8, builtinResults5.Columns);
        Assert.HasCount(3, builtinResults5.Rows);

        SWRLBuiltIn builtin6 = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        OWLTable builtinResults6 = builtin6.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults6);
        Assert.HasCount(8, builtinResults6.Columns);
        Assert.HasCount(3, builtinResults6.Rows);

        SWRLBuiltIn builtin7 = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?F")));  //unexisting
        OWLTable builtinResults7 = builtin7.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults7);
        Assert.HasCount(8, builtinResults7.Columns);
        Assert.HasCount(3, builtinResults7.Rows);

        //Test exception on unknown builtIn
        Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#example",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));

        //Test exception on bad-formed builtIn
        Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLBuiltIn
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
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?U");
        antecedentResults.AddColumn("?V");
        antecedentResults.AddColumn("?W");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2012^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLLiteralArgument(new RDFTypedLiteral("2010-05-22T10:30:30Z", RDFModelEnums.RDFDatatypes.XSD_DATETIME)),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(7, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateDateTimeBuiltInWithRightYearLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?U");
        antecedentResults.AddColumn("?V");
        antecedentResults.AddColumn("?W");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2010-08-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFTypedLiteral("2010", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(7, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateDateTimeBuiltInWithRightMonthLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?U");
        antecedentResults.AddColumn("?V");
        antecedentResults.AddColumn("?W");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2010-08-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(7, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateDateTimeBuiltInWithRightDayLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?U");
        antecedentResults.AddColumn("?V");
        antecedentResults.AddColumn("?W");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2010-08-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-16-22T25:10:10Z^^http://www.w3.org/2001/XMLSchema#dateTime", //not valid
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "5^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLLiteralArgument(new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(7, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "5^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateDateTimeBuiltInWithRightHourLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?V");
        antecedentResults.AddColumn("?W");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLLiteralArgument(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(7, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "05^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "05^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateDateTimeBuiltInWithRightMinuteLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?U");
        antecedentResults.AddColumn("?W");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?W")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(7, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "05^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "05^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?W"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateDateTimeBuiltInWithRightSecondLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?U");
        antecedentResults.AddColumn("?V");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "" }); //Will fallback to "UTC"
        antecedentResults.AddRow(new string[] { "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime",
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });
        antecedentResults.AddRow(new string[] { "2010-05-22T10:65:30Z^^http://www.w3.org/2001/XMLSchema#time", //not valid
            "2010^^http://www.w3.org/2001/XMLSchema#int",
            "05^^http://www.w3.org/2001/XMLSchema#int",
            "22^^http://www.w3.org/2001/XMLSchema#int",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC" });

        SWRLBuiltIn builtin = SWRLBuiltIn.DateTime(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?U")),
            new SWRLVariableArgument(new RDFVariable("?V")),
            new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?T")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(7, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2010-05-22T10:30:30Z^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "05^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "2010-05-22T10:30:30^^http://www.w3.org/2001/XMLSchema#dateTime"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2010^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "05^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "22^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?U"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?V"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }
    #endregion
}

