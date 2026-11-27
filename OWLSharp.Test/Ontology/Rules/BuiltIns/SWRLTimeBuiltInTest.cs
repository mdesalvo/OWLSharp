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

namespace OWLSharp.Test.Ontology;

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
        Assert.HasCount(5, builtin.Arguments);
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
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Time(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Time(new SWRLVariableArgument(new RDFVariable("?X")), null));
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
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#time"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Variable IRI="urn:swrl:var#Z" /><Variable IRI="urn:swrl:var#Q" /><Variable IRI="urn:swrl:var#T" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#time", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(5, builtin.Arguments);
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
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#time"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#time\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldEvaluateTimeBuiltIn()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow([
            "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);
        antecedentResults.AddRow([
            "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            ""
        ]); //Will fallback to "UTC"
        antecedentResults.AddRow([
            "10:34:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(5, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(5, builtinResults2.Columns);
        Assert.HasCount(0, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(5, builtinResults3.Columns);
        Assert.HasCount(0, builtinResults3.Rows);

        SWRLBuiltIn builtin4 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable builtinResults4 = builtin4.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults4);
        Assert.HasCount(5, builtinResults4.Columns);
        Assert.HasCount(0, builtinResults4.Rows);

        SWRLBuiltIn builtin5 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
        RDFTable builtinResults5 = builtin5.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults5);
        Assert.HasCount(5, builtinResults5.Columns);
        Assert.HasCount(0, builtinResults5.Rows);

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
            IRI = "http://www.w3.org/2003/11/swrlb#time",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateTimeBuiltInWithLeftLiteral()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow([
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);
        antecedentResults.AddRow([
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            ""
        ]); //Will fallback to "UTC"
        antecedentResults.AddRow([
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Time(
            new SWRLLiteralArgument(new RDFTypedLiteral("10:30:30Z", RDFModelEnums.RDFDatatypes.XSD_TIME)),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateTimeBuiltInWithRightHourLiteral()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow([
            "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);
        antecedentResults.AddRow([
            "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            ""
        ]); //Will fallback to "UTC"
        antecedentResults.AddRow([
            "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateTimeBuiltInWithRightMinuteLiteral()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow([
            "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);
        antecedentResults.AddRow([
            "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            ""
        ]); //Will fallback to "UTC"
        antecedentResults.AddRow([
            "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateTimeBuiltInWithRightSecondLiteral()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow([
            "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);
        antecedentResults.AddRow([
            "10:30:30^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "30^^http://www.w3.org/2001/XMLSchema#int",
            ""
        ]); //Will fallback to "UTC"
        antecedentResults.AddRow([
            "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time",
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);
        antecedentResults.AddRow([
            "10:65:30Z^^http://www.w3.org/2001/XMLSchema#time", //not valid
            "10^^http://www.w3.org/2001/XMLSchema#int",
            "34^^http://www.w3.org/2001/XMLSchema#int",
            "UTC"
        ]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLLiteralArgument(new RDFTypedLiteral("30", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
            new SWRLVariableArgument(new RDFVariable("?T")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "10:30:30Z^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?T"] ?? string.Empty), "UTC"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "10:30:30^^http://www.w3.org/2001/XMLSchema#time"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "10^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "30^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?T"] ?? string.Empty), ""));
    }

    [TestMethod]
    public void ShouldEvaluateTimeBuiltInWithUnexistingVariableDroppingEveryRow()
    {
        //An unbound variable never satisfies a built-in: the same row is kept by the complete built-in and dropped once one variable has no column
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(["10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", "10^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "UTC"]);

        SWRLBuiltIn completeBuiltin = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable completeResults = completeBuiltin.EvaluateOnAntecedent(antecedentResults);
        Assert.HasCount(5, completeResults.Columns);
        Assert.HasCount(1, completeResults.Rows);
        Assert.AreEqual("10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", completeResults.Rows[0][0]);
        Assert.AreEqual("10^^http://www.w3.org/2001/XMLSchema#int", completeResults.Rows[0][1]);
        Assert.AreEqual("30^^http://www.w3.org/2001/XMLSchema#int", completeResults.Rows[0][2]);
        Assert.AreEqual("30^^http://www.w3.org/2001/XMLSchema#int", completeResults.Rows[0][3]);
        Assert.AreEqual("UTC", completeResults.Rows[0][4]);

        SWRLBuiltIn builtin1 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable builtinResults1 = builtin1.EvaluateOnAntecedent(antecedentResults);
        Assert.HasCount(5, builtinResults1.Columns);
        Assert.IsEmpty(builtinResults1.Rows);

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.HasCount(5, builtinResults2.Columns);
        Assert.IsEmpty(builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.HasCount(5, builtinResults3.Columns);
        Assert.IsEmpty(builtinResults3.Rows);

        SWRLBuiltIn builtin4 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?T")));
        RDFTable builtinResults4 = builtin4.EvaluateOnAntecedent(antecedentResults);
        Assert.HasCount(5, builtinResults4.Columns);
        Assert.IsEmpty(builtinResults4.Rows);

        SWRLBuiltIn builtin5 = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?F")));
        RDFTable builtinResults5 = builtin5.EvaluateOnAntecedent(antecedentResults);
        Assert.HasCount(5, builtinResults5.Columns);
        Assert.IsEmpty(builtinResults5.Rows);
    }

    [TestMethod]
    public void ShouldEvaluateTimeBuiltInWithNullCellDroppingTheRow()
    {
        //A null cell is an unbound variable: the row holding it is dropped while the complete row is kept
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddColumn("?T");
        antecedentResults.AddRow(["10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", "10^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "UTC"]);
        antecedentResults.AddRow([null, "10^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "UTC"]);
        antecedentResults.AddRow(["10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", null, "30^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "UTC"]);
        antecedentResults.AddRow(["10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", "10^^http://www.w3.org/2001/XMLSchema#int", null, "30^^http://www.w3.org/2001/XMLSchema#int", "UTC"]);
        antecedentResults.AddRow(["10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", "10^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", null, "UTC"]);
        antecedentResults.AddRow(["10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", "10^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", "30^^http://www.w3.org/2001/XMLSchema#int", null]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Time(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")),
            new SWRLVariableArgument(new RDFVariable("?T")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(5, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.AreEqual("10:30:30Z^^http://www.w3.org/2001/XMLSchema#time", builtinResults.Rows[0][0]);
        Assert.AreEqual("10^^http://www.w3.org/2001/XMLSchema#int", builtinResults.Rows[0][1]);
        Assert.AreEqual("30^^http://www.w3.org/2001/XMLSchema#int", builtinResults.Rows[0][2]);
        Assert.AreEqual("30^^http://www.w3.org/2001/XMLSchema#int", builtinResults.Rows[0][3]);
        Assert.AreEqual("UTC", builtinResults.Rows[0][4]);
    }
    #endregion
}

