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
public class SWRLUnaryMinusBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateUnaryMinusBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:unaryMinus(?X,?Y)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.UnaryMinus(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.UnaryMinus(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeUnaryMinusBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeUnaryMinusBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#unaryMinus"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:unaryMinus(?X,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#unaryMinus"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldDeserializeUnaryMinusBuiltInWithLeftNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#unaryMinus"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLLiteralArgument tlarg
                      && tlarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:unaryMinus(\"5\"^^xsd:integer,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeUnaryMinusBuiltInWithRightNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#unaryMinus"><Variable IRI="urn:swrl:var#X" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#unaryMinus", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument trarg
                      && trarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(string.Equals("swrlb:unaryMinus(?X,\"5\"^^xsd:integer)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#unaryMinus\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldEvaluateUnaryMinusBuiltIn()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "-2.00^^http://www.w3.org/2001/XMLSchema#double"
        ]);
        antecedentResults.AddRow(["-3^^http://www.w3.org/2001/XMLSchema#int", "3^^http://www.w3.org/2001/XMLSchema#positiveInteger"
        ]);
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
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "http://example.org/tests"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "-2.00^^http://www.w3.org/2001/XMLSchema#double"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "-3^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "3^^http://www.w3.org/2001/XMLSchema#positiveInteger"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(2, builtinResults2.Columns);
        Assert.HasCount(0, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")));
        RDFTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(3, builtinResults3.Columns);
        Assert.HasCount(6, builtinResults3.Rows);
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][0] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][1] ?? string.Empty), "-2.00^^http://www.w3.org/2001/XMLSchema#double"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[0][2] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#double"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][0] ?? string.Empty), "-3^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][1] ?? string.Empty), "3^^http://www.w3.org/2001/XMLSchema#positiveInteger"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[1][2] ?? string.Empty), "-3^^http://www.w3.org/2001/XMLSchema#integer"));
        Assert.IsNull(builtinResults3.Rows[2][0]);
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][1] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[2][2] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#integer"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][0] ?? string.Empty), "hello^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][1] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[3][2] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#integer"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][0] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][1] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[4][2] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#integer"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][0] ?? string.Empty), "hello@EN"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][1] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults3.Rows[5][2] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#integer"));

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
            IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateUnaryMinusBuiltInWithLeftNumber()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["-2^^http://www.w3.org/2001/XMLSchema#double"]);
        antecedentResults.AddRow(["2.73^^http://www.w3.org/2001/XMLSchema#double"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow([null]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["hello"]);
        antecedentResults.AddRow(["hello@EN"]);
        antecedentResults.AddRow(["http://example.org/tests"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
            Arguments = [
                new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Y"))
            ]
        };

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#double"));
    }

    [TestMethod]
    public void ShouldEvaluateUnaryMinusBuiltInWithRightNumber()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddRow(["-2.73^^http://www.w3.org/2001/XMLSchema#double"]);
        antecedentResults.AddRow(["3^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2.0^^http://www.w3.org/2001/XMLSchema#float"]);
        antecedentResults.AddRow([null]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["hello"]);
        antecedentResults.AddRow(["hello@EN"]);
        antecedentResults.AddRow(["http://example.org/test"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#unaryMinus",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFTypedLiteral("2.73", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))
            ]
        };

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "-2.73^^http://www.w3.org/2001/XMLSchema#double"));
    }

    [TestMethod]
    public void ShouldEvaluateUnaryMinusBuiltInBindingFirstArgument()
    {
        //The first variable has no column: the built-in adds it and fills it with the computed value of the rows it can compute
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["5^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["-5^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["1.5^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["2.5^^http://www.w3.org/2001/XMLSchema#double"]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow([null]);

        SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["5^^http://www.w3.org/2001/XMLSchema#int", "-5^^http://www.w3.org/2001/XMLSchema#integer"],
            ["-5^^http://www.w3.org/2001/XMLSchema#int", "5^^http://www.w3.org/2001/XMLSchema#integer"],
            ["1.5^^http://www.w3.org/2001/XMLSchema#decimal", "-1.5^^http://www.w3.org/2001/XMLSchema#decimal"],
            ["2.5^^http://www.w3.org/2001/XMLSchema#double", "-2.5^^http://www.w3.org/2001/XMLSchema#double"]
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
    public void ShouldEvaluateUnaryMinusBuiltInInPredicateMode()
    {
        //The first variable is bound: a row is kept only when its first cell holds the computed value
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["-5^^http://www.w3.org/2001/XMLSchema#integer", "5^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int", "5^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["-5.0^^http://www.w3.org/2001/XMLSchema#decimal", "5^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["5^^http://www.w3.org/2001/XMLSchema#integer", "-5^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int", "-5^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["-1.5^^http://www.w3.org/2001/XMLSchema#decimal", "1.5^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int", "1.5^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["-2.5^^http://www.w3.org/2001/XMLSchema#double", "2.5^^http://www.w3.org/2001/XMLSchema#double"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int", "2.5^^http://www.w3.org/2001/XMLSchema#double"]);
        antecedentResults.AddRow([null, "5^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["-5^^http://www.w3.org/2001/XMLSchema#integer", "5^^http://www.w3.org/2001/XMLSchema#int"],
            ["-5.0^^http://www.w3.org/2001/XMLSchema#decimal", "5^^http://www.w3.org/2001/XMLSchema#int"],
            ["5^^http://www.w3.org/2001/XMLSchema#integer", "-5^^http://www.w3.org/2001/XMLSchema#int"],
            ["-1.5^^http://www.w3.org/2001/XMLSchema#decimal", "1.5^^http://www.w3.org/2001/XMLSchema#decimal"],
            ["-2.5^^http://www.w3.org/2001/XMLSchema#double", "2.5^^http://www.w3.org/2001/XMLSchema#double"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(5, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
    }

    [TestMethod]
    public void ShouldEvaluateUnaryMinusBuiltInWithUnexistingNonFirstArgument()
    {
        //A non-first variable with no column is unbound: no row can satisfy the built-in whether the first variable is bound or not
        RDFTable bindingResults = new RDFTable();
        bindingResults.AddColumn("?Y");
        bindingResults.AddRow(["5^^http://www.w3.org/2001/XMLSchema#int"]);
        RDFTable predicateResults = new RDFTable();
        predicateResults.AddColumn("?X");
        predicateResults.AddColumn("?Y");
        predicateResults.AddRow(["-5^^http://www.w3.org/2001/XMLSchema#integer", "5^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin1 = SWRLBuiltIn.UnaryMinus(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F")));
        RDFTable bindingTable1 = builtin1.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(2, bindingTable1.Columns);
        Assert.IsEmpty(bindingTable1.Rows);
        RDFTable predicateTable1 = builtin1.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(2, predicateTable1.Columns);
        Assert.IsEmpty(predicateTable1.Rows);
    }
    #endregion
}

