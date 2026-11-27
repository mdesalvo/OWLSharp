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
public class SWRLUpperCaseBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateUpperCaseBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#upperCase", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:upperCase(?X,?Y)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.UpperCase(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.UpperCase(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeUpperCaseBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#upperCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeUpperCaseBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#upperCase"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#upperCase", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:upperCase(?X,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#upperCase\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#upperCase"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#upperCase\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldDeserializeUpperCaseBuiltInWithLeftNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#upperCase"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#upperCase", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLLiteralArgument tlarg
                      && tlarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:upperCase(\"5\"^^xsd:integer,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#upperCase\"><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeUpperCaseBuiltInWithRightNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#upperCase"><Variable IRI="urn:swrl:var#X" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#upperCase", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument trarg
                      && trarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(string.Equals("swrlb:upperCase(?X,\"5\"^^xsd:integer)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#upperCase\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltIn()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["HELLO", "Hello"]);
        antecedentResults.AddRow(["hello@EN-US", "EN-US"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow([null, "-2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", null]);
        antecedentResults.AddRow([null, null]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello"]);
        antecedentResults.AddRow(["hello", "-2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN"]);
        antecedentResults.AddRow(["hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["HTTP://EXAMPLE.ORG/", "htTp://example.org/"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "Hello"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(2, builtinResults2.Columns);
        Assert.HasCount(0, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")));
        RDFTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(3, builtinResults3.Columns);
        Assert.HasCount(6, builtinResults3.Rows);
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][0] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][1] ?? string.Empty), "Hello"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][2] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][0] ?? string.Empty), "hello@EN-US"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][1] ?? string.Empty), "EN-US"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][2] ?? string.Empty), "EN-US"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][0] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][1] ?? string.Empty), "hello^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][2] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][0] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][1] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][2] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][0] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][1] ?? string.Empty), "hello@EN"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][2] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][0] ?? string.Empty), "HTTP://EXAMPLE.ORG/"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][1] ?? string.Empty), "htTp://example.org/"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][2] ?? string.Empty), "HTTP://EXAMPLE.ORG/"));

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
            IRI = "http://www.w3.org/2003/11/swrlb#upperCase",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInWithLeftString()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["Hello"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2.0^^http://www.w3.org/2001/XMLSchema#float"]);
        antecedentResults.AddRow([null]);
        antecedentResults.AddRow(["heLlo^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["hello"]);
        antecedentResults.AddRow(["hellO@EN"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#upperCase",
            Arguments = [
                new SWRLLiteralArgument(new RDFTypedLiteral("HELLO", RDFModelEnums.RDFDatatypes.XSD_STRING)),
                new SWRLVariableArgument(new RDFVariable("?Y"))
            ]
        };

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(4, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "Hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "heLlo^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?Y"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[3]["?Y"] ?? string.Empty), "hellO@EN"));
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInWithRightString()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2.0^^http://www.w3.org/2001/XMLSchema#float"]);
        antecedentResults.AddRow([null]);
        antecedentResults.AddRow(["HELLO^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["HELLO"]);
        antecedentResults.AddRow(["heLlo"]);
        antecedentResults.AddRow(["HELLO@EN"]);
        antecedentResults.AddRow(["http://example.org/hello"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#upperCase",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFPlainLiteral("Hello"))
            ]
        };

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "HELLO^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "HELLO"));
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInWithIndividualArguments()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?dummy");
        antecedentResults.AddRow(["dummy"]);

        //RDFResource always normalizes the scheme/authority of a Uri to lowercase (standard .NET Uri behavior),
        //so its ToString() is never fully uppercase even when constructed from an uppercase IRI: the left side
        //keeps "http://example.org/HELLO", while ToUpper() on the right always yields "HTTP://EXAMPLE.ORG/HELLO",
        //so they can never match when the resource has an alphabetic scheme
        SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
            new SWRLIndividualArgument(new RDFResource("HTTP://EXAMPLE.ORG/HELLO")),
            new SWRLIndividualArgument(new RDFResource("HTTP://EXAMPLE.ORG/HELLO")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.IsEmpty(builtinResults.Rows);
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInBindingFirstArgument()
    {
        //The first variable has no column: the built-in adds it and fills it with the computed value of the rows it can compute
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["hello"]);
        antecedentResults.AddRow(["HeLLo@EN"]);
        antecedentResults.AddRow(["abc^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["hello world"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow([null]);

        SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["hello", "HELLO"],
            ["HeLLo@EN", "HELLO"],
            ["abc^^http://www.w3.org/2001/XMLSchema#string", "ABC"],
            ["hello world", "HELLO WORLD"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.AreEqual("?X", builtinResults.Columns[1].Name);
        Assert.HasCount(4, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
        //The incoming table keeps its own columns
        Assert.HasCount(1, antecedentResults.Columns);
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInInPredicateMode()
    {
        //The first variable is bound: a row is kept only when its first cell holds the computed value
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["HELLO", "hello"]);
        antecedentResults.AddRow(["zzz", "hello"]);
        antecedentResults.AddRow(["HELLO@EN", "hello"]);
        antecedentResults.AddRow(["HELLO^^http://www.w3.org/2001/XMLSchema#string", "hello"]);
        antecedentResults.AddRow(["HELLO", "HeLLo@EN"]);
        antecedentResults.AddRow(["zzz", "HeLLo@EN"]);
        antecedentResults.AddRow(["ABC", "abc^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["zzz", "abc^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["HELLO WORLD", "hello world"]);
        antecedentResults.AddRow(["zzz", "hello world"]);
        antecedentResults.AddRow([null, "hello"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["HELLO", "hello"],
            ["HELLO^^http://www.w3.org/2001/XMLSchema#string", "hello"],
            ["HELLO", "HeLLo@EN"],
            ["ABC", "abc^^http://www.w3.org/2001/XMLSchema#string"],
            ["HELLO WORLD", "hello world"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(5, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInWithUnexistingNonFirstArgument()
    {
        //A non-first variable with no column is unbound: no row can satisfy the built-in whether the first variable is bound or not
        RDFTable bindingResults = new RDFTable();
        bindingResults.AddColumn("?Y");
        bindingResults.AddRow(["hello"]);
        RDFTable predicateResults = new RDFTable();
        predicateResults.AddColumn("?X");
        predicateResults.AddColumn("?Y");
        predicateResults.AddRow(["HELLO", "hello"]);

        SWRLBuiltIn builtin1 = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F")));
        RDFTable bindingTable1 = builtin1.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(2, bindingTable1.Columns);
        Assert.IsEmpty(bindingTable1.Rows);
        RDFTable predicateTable1 = builtin1.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(2, predicateTable1.Columns);
        Assert.IsEmpty(predicateTable1.Rows);
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInIndependentlyFromCurrentCulture()
    {
        //Turkish casing maps i to a dotted capital and I to a dotless small: the built-in must not follow the process culture
        System.Globalization.CultureInfo originalCulture = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");

            RDFTable antecedentResults = new RDFTable();
            antecedentResults.AddColumn("?Y");
            antecedentResults.AddRow(["i"]);

            SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));

            RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.HasCount(2, builtinResults.Columns);
            Assert.HasCount(1, builtinResults.Rows);
            Assert.AreEqual("i", builtinResults.Rows[0][0]);
            Assert.AreEqual("I", builtinResults.Rows[0][1]);
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = originalCulture;
        }
    }
    #endregion
}

