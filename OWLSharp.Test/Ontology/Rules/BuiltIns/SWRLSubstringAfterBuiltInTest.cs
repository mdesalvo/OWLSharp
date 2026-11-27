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
public class SWRLSubstringAfterBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateSubstringAfterBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#substringAfter", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(3, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(string.Equals("swrlb:substringAfter(?X,?Y,?Z)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.SubstringAfter(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.SubstringAfter(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeSubstringAfterBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substringAfter\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeSubstringAfterBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#substringAfter"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Variable IRI="urn:swrl:var#Z" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#substringAfter", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(3, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(string.Equals("swrlb:substringAfter(?X,?Y,?Z)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substringAfter\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#substringAfter"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substringAfter\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltIn()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["too", "tattoo", "at"]);
        antecedentResults.AddRow([".org/", "http://example.org/", "ample"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "too"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "tattoo"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "at"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), ".org/"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "http://example.org/"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "ample"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(3, builtinResults2.Columns);
        Assert.HasCount(0, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));
        RDFTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(4, builtinResults3.Columns);
        Assert.HasCount(2, builtinResults3.Rows);
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][0] ?? string.Empty), "too"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][1] ?? string.Empty), "tattoo"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][2] ?? string.Empty), "at"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][3] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][0] ?? string.Empty), ".org/"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][1] ?? string.Empty), "http://example.org/"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][2] ?? string.Empty), "ample"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][3] ?? string.Empty), ""));

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
            IRI = "http://www.w3.org/2003/11/swrlb#substringAfter",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithRightArgumentSTR()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["too", "at"]);
        antecedentResults.AddRow(["o", "to"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFPlainLiteral("tattoo")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "too"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "at"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "o"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "to"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithRightArgumentEmptySTR()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["", "attoo"]);
        antecedentResults.AddRow(["tattoo", "tat"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(RDFPlainLiteral.Empty),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "attoo"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithRightArgumentSEP()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["o", "tattoo"]);
        antecedentResults.AddRow(["o.org/test/", "http://tattoo.org/test/"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("atto", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "o"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "tattoo"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "o.org/test/"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "http://tattoo.org/test/"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithRightArgumentEmptySEP()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["", "tattoo"]);
        antecedentResults.AddRow(["http://example.org", "http://tattoo.org/test"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "tattoo"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInBindingFirstArgument()
    {
        //The first variable has no column: the built-in adds it and fills it with the computed value of the rows it can compute
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["abracadabra", "bra"]);
        antecedentResults.AddRow(["abracadabra", "x"]);
        antecedentResults.AddRow(["hello@EN", "l"]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", "he"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "x"]);
        antecedentResults.AddRow([null, "bra"]);
        antecedentResults.AddRow(["abracadabra", null]);

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["abracadabra", "bra", "cadabra"],
            ["abracadabra", "x", ""],
            ["hello@EN", "l", "lo"],
            ["hello^^http://www.w3.org/2001/XMLSchema#string", "he", "llo"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.AreEqual("?X", builtinResults.Columns[2].Name);
        Assert.HasCount(4, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
        //The incoming table keeps its own columns
        Assert.HasCount(2, antecedentResults.Columns);
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInInPredicateMode()
    {
        //The first variable is bound: a row is kept only when its first cell holds the computed value
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["cadabra", "abracadabra", "bra"]);
        antecedentResults.AddRow(["zzz", "abracadabra", "bra"]);
        antecedentResults.AddRow(["cadabra@EN", "abracadabra", "bra"]);
        antecedentResults.AddRow(["cadabra^^http://www.w3.org/2001/XMLSchema#string", "abracadabra", "bra"]);
        antecedentResults.AddRow(["lo", "hello@EN", "l"]);
        antecedentResults.AddRow(["zzz", "hello@EN", "l"]);
        antecedentResults.AddRow(["llo", "hello^^http://www.w3.org/2001/XMLSchema#string", "he"]);
        antecedentResults.AddRow(["zzz", "hello^^http://www.w3.org/2001/XMLSchema#string", "he"]);
        antecedentResults.AddRow([null, "abracadabra", "bra"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["cadabra", "abracadabra", "bra"],
            ["cadabra^^http://www.w3.org/2001/XMLSchema#string", "abracadabra", "bra"],
            ["lo", "hello@EN", "l"],
            ["llo", "hello^^http://www.w3.org/2001/XMLSchema#string", "he"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(4, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithUnexistingNonFirstArgument()
    {
        //A non-first variable with no column is unbound: no row can satisfy the built-in whether the first variable is bound or not
        RDFTable bindingResults = new RDFTable();
        bindingResults.AddColumn("?Y");
        bindingResults.AddColumn("?Z");
        bindingResults.AddRow(["abracadabra", "bra"]);
        RDFTable predicateResults = new RDFTable();
        predicateResults.AddColumn("?X");
        predicateResults.AddColumn("?Y");
        predicateResults.AddColumn("?Z");
        predicateResults.AddRow(["cadabra", "abracadabra", "bra"]);

        SWRLBuiltIn builtin1 = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?Z")));
        RDFTable bindingTable1 = builtin1.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(3, bindingTable1.Columns);
        Assert.IsEmpty(bindingTable1.Rows);
        RDFTable predicateTable1 = builtin1.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(3, predicateTable1.Columns);
        Assert.IsEmpty(predicateTable1.Rows);

        SWRLBuiltIn builtin2 = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F")));
        RDFTable bindingTable2 = builtin2.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(3, bindingTable2.Columns);
        Assert.IsEmpty(bindingTable2.Rows);
        RDFTable predicateTable2 = builtin2.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(3, predicateTable2.Columns);
        Assert.IsEmpty(predicateTable2.Rows);
    }
    #endregion
}

