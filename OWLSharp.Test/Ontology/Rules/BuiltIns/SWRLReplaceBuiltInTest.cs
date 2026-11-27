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
public class SWRLReplaceBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateReplaceBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#replace", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(4, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg1
                      && rlarg1.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3
                      && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
        Assert.IsTrue(string.Equals("swrlb:replace(?X,?Y,?Z,?Q)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Replace(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Replace(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeReplaceBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#replace\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeReplaceBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#replace"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Variable IRI="urn:swrl:var#Z" /><Variable IRI="urn:swrl:var#Q" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#replace", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(4, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg1
                      && rlarg1.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3
                      && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
        Assert.IsTrue(string.Equals("swrlb:replace(?X,?Y,?Z,?Q)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#replace\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#replace"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#replace\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldEvaluateReplaceBuiltIn()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddRow(["heMMo", "hello", "ll", "MM"]);
        antecedentResults.AddRow(["hemMo", "hello", "ll", "MM"]);
        antecedentResults.AddRow(["heMMo@EN", "hello@EN", "ll@EN", "MM@EN"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2", "2", "2"]);
        antecedentResults.AddRow([null, null, null, null]);
        antecedentResults.AddRow([null, "hello", "[a-z]+", ""]);
        antecedentResults.AddRow(["hello", null, "hello", "hello"]);
        antecedentResults.AddRow(["http://example.org/test/", "ftp://example.org/test/", "ftp", "http"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "ll"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "MM"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "http://example.org/test/"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "ftp://example.org/test/"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "ftp"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "http"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?P"))); //unexisting
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(4, builtinResults2.Columns);
        Assert.HasCount(0, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?P")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));
        RDFTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(5, builtinResults3.Columns);
        Assert.HasCount(6, builtinResults3.Rows);
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][0] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][1] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][2] ?? string.Empty), "ll"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][3] ?? string.Empty), "MM"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][4] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][0] ?? string.Empty), "hemMo"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][1] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][2] ?? string.Empty), "ll"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][3] ?? string.Empty), "MM"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][4] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][0] ?? string.Empty), "heMMo@EN"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][1] ?? string.Empty), "hello@EN"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][2] ?? string.Empty), "ll@EN"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][3] ?? string.Empty), "MM@EN"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][4] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][0] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][1] ?? string.Empty), "2"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][2] ?? string.Empty), "2"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][3] ?? string.Empty), "2"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][4] ?? string.Empty), "2"));
        Assert.IsNull(builtinResults3.Rows[4][0]);
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][1] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][2] ?? string.Empty), "[a-z]+"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][3] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][4] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][0] ?? string.Empty), "http://example.org/test/"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][1] ?? string.Empty), "ftp://example.org/test/"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][2] ?? string.Empty), "ftp"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][3] ?? string.Empty), "http"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][4] ?? string.Empty), "http://example.org/test/"));

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
            IRI = "http://www.w3.org/2003/11/swrlb#replace",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateReplaceBuiltInWithSTRLiteral()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddRow(["heMMo", "ll", "MM"]);
        antecedentResults.AddRow(["hemMo", "ll", "MM"]);
        antecedentResults.AddRow(["heMMo@EN", "ll@EN", "MM@EN"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2", "2"]);
        antecedentResults.AddRow([null, null, null]);
        antecedentResults.AddRow([null, "[a-z]+", ""]);
        antecedentResults.AddRow(["hello", "hello", "hello"]);
        antecedentResults.AddRow(["http://example.org/test/", "ftp", "http"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello","EN")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "ll"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "MM"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Q"] ?? string.Empty), "hello"));
    }

    [TestMethod]
    public void ShouldEvaluateReplaceBuiltInWithRGXLiteral()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddRow(["heMMo", "hello", "MM"]);
        antecedentResults.AddRow(["hemMo", "hello", "MM"]);
        antecedentResults.AddRow(["heMMo@EN", "hello@EN", "MM@EN"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2", "2"]);
        antecedentResults.AddRow([null, null, null]);
        antecedentResults.AddRow([null, "hello", ""]);
        antecedentResults.AddRow(["hello", null, "hello"]);
        antecedentResults.AddRow(["http://example.org/test/", "ftp://example.org/test/", "http"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFPlainLiteral("ll")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Q"] ?? string.Empty), "MM"));
    }

    [TestMethod]
    public void ShouldEvaluateReplaceBuiltInWithRPLLiteral()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["heMMo", "hello", "ll"]);
        antecedentResults.AddRow(["hemMo", "hello", "ll"]);
        antecedentResults.AddRow(["heMMo@EN", "hello@EN", "ll@EN"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2", "2"]);
        antecedentResults.AddRow([null, null, null]);
        antecedentResults.AddRow([null, "hello", "[a-z]+"]);
        antecedentResults.AddRow(["hello", null, "hello"]);
        antecedentResults.AddRow(["http://example.org/test/", "ftp://example.org/test/", "ftp"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLLiteralArgument(new RDFTypedLiteral("MM", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "heMMo"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "ll"));
    }

    [TestMethod]
    public void ShouldEvaluateReplaceBuiltInBindingFirstArgument()
    {
        //The first variable has no column: the built-in adds it and fills it with the computed value of the rows it can compute
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddRow(["hello", "l+", "L"]);
        antecedentResults.AddRow(["abracadabra", "a.*?a", "*"]);
        antecedentResults.AddRow(["hello@EN", "l", "L"]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", "l", "L"]);
        antecedentResults.AddRow(["hello", "2^^http://www.w3.org/2001/XMLSchema#int", "L"]);
        antecedentResults.AddRow(["hello", "l", "2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow([null, "l+", "L"]);
        antecedentResults.AddRow(["hello", null, "L"]);
        antecedentResults.AddRow(["hello", "l+", null]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["hello", "l+", "L", "heLo"],
            ["abracadabra", "a.*?a", "*", "*c*bra"],
            ["hello@EN", "l", "L", "heLLo"],
            ["hello^^http://www.w3.org/2001/XMLSchema#string", "l", "L", "heLLo"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.AreEqual("?X", builtinResults.Columns[3].Name);
        Assert.HasCount(4, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
        //The incoming table keeps its own columns
        Assert.HasCount(3, antecedentResults.Columns);
    }

    [TestMethod]
    public void ShouldEvaluateReplaceBuiltInInPredicateMode()
    {
        //The first variable is bound: a row is kept only when its first cell holds the computed value
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddColumn("?Q");
        antecedentResults.AddRow(["heLo", "hello", "l+", "L"]);
        antecedentResults.AddRow(["zzz", "hello", "l+", "L"]);
        antecedentResults.AddRow(["heLo@EN", "hello", "l+", "L"]);
        antecedentResults.AddRow(["heLo^^http://www.w3.org/2001/XMLSchema#string", "hello", "l+", "L"]);
        antecedentResults.AddRow(["*c*bra", "abracadabra", "a.*?a", "*"]);
        antecedentResults.AddRow(["zzz", "abracadabra", "a.*?a", "*"]);
        antecedentResults.AddRow(["heLLo", "hello@EN", "l", "L"]);
        antecedentResults.AddRow(["zzz", "hello@EN", "l", "L"]);
        antecedentResults.AddRow(["heLLo", "hello^^http://www.w3.org/2001/XMLSchema#string", "l", "L"]);
        antecedentResults.AddRow(["zzz", "hello^^http://www.w3.org/2001/XMLSchema#string", "l", "L"]);
        antecedentResults.AddRow([null, "hello", "l+", "L"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["heLo", "hello", "l+", "L"],
            ["heLo^^http://www.w3.org/2001/XMLSchema#string", "hello", "l+", "L"],
            ["*c*bra", "abracadabra", "a.*?a", "*"],
            ["heLLo", "hello@EN", "l", "L"],
            ["heLLo", "hello^^http://www.w3.org/2001/XMLSchema#string", "l", "L"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.HasCount(5, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
    }

    [TestMethod]
    public void ShouldEvaluateReplaceBuiltInWithUnexistingNonFirstArgument()
    {
        //A non-first variable with no column is unbound: no row can satisfy the built-in whether the first variable is bound or not
        RDFTable bindingResults = new RDFTable();
        bindingResults.AddColumn("?Y");
        bindingResults.AddColumn("?Z");
        bindingResults.AddColumn("?Q");
        bindingResults.AddRow(["hello", "l+", "L"]);
        RDFTable predicateResults = new RDFTable();
        predicateResults.AddColumn("?X");
        predicateResults.AddColumn("?Y");
        predicateResults.AddColumn("?Z");
        predicateResults.AddColumn("?Q");
        predicateResults.AddRow(["heLo", "hello", "l+", "L"]);

        SWRLBuiltIn builtin1 = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));
        RDFTable bindingTable1 = builtin1.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(4, bindingTable1.Columns);
        Assert.IsEmpty(bindingTable1.Rows);
        RDFTable predicateTable1 = builtin1.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(4, predicateTable1.Columns);
        Assert.IsEmpty(predicateTable1.Rows);

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?Q")));
        RDFTable bindingTable2 = builtin2.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(4, bindingTable2.Columns);
        Assert.IsEmpty(bindingTable2.Rows);
        RDFTable predicateTable2 = builtin2.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(4, predicateTable2.Columns);
        Assert.IsEmpty(predicateTable2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.Replace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?F")));
        RDFTable bindingTable3 = builtin3.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(4, bindingTable3.Columns);
        Assert.IsEmpty(bindingTable3.Rows);
        RDFTable predicateTable3 = builtin3.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(4, predicateTable3.Columns);
        Assert.IsEmpty(predicateTable3.Rows);
    }
    #endregion
}

