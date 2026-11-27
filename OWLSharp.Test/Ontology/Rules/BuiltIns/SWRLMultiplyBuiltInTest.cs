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
public class SWRLMultiplyBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateMultiplyBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#multiply", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(3, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument r1larg
                      && r1larg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument r2larg
                      && r2larg.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(string.Equals("swrlb:multiply(?X,?Y,?Z)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Multiply(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Multiply(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeMultiplyBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#multiply\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeMultiplyBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#multiply"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Variable IRI="urn:swrl:var#Z" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#multiply", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(3, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument r1larg
                      && r1larg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument r2larg
                      && r2larg.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(string.Equals("swrlb:multiply(?X,?Y,?Z)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#multiply\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#multiply"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#multiply\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldDeserializeMultiplyBuiltInWithNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#multiply"><Variable IRI="urn:swrl:var#X" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#multiply", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(3, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument r1larg
                      && r1larg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument r2larg
                      && r2larg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:multiply(?X,\"5\"^^xsd:integer,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#multiply\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldEvaluateMultiplyBuiltIn()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["8^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int", "4.0^^http://www.w3.org/2001/XMLSchema#float"
        ]);
        antecedentResults.AddRow(["-2^^http://www.w3.org/2001/XMLSchema#int", "-2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow([null, "-2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", null, "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow([null, null, null]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", "hello^^http://www.w3.org/2001/XMLSchema#string", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello", "hello"]);
        antecedentResults.AddRow(["hello", "-2^^http://www.w3.org/2001/XMLSchema#int", "hello"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN", "hello@EN"]);
        antecedentResults.AddRow(["hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN"]);
        antecedentResults.AddRow(["0^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int", "0.0^^http://www.w3.org/2001/XMLSchema#float"
        ]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "8^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "4.0^^http://www.w3.org/2001/XMLSchema#float"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "0^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), "0.0^^http://www.w3.org/2001/XMLSchema#float"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(3, builtinResults2.Columns);
        Assert.IsEmpty(builtinResults2.Rows);

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
            IRI = "http://www.w3.org/2003/11/swrlb#multiply",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateMultiplyBuiltInWithOnly2Arguments()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2.0^^http://www.w3.org/2001/XMLSchema#float"
        ]);
        antecedentResults.AddRow(["-2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow([null, "2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", null]);
        antecedentResults.AddRow([null, null]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello"]);
        antecedentResults.AddRow(["hello", "-2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN"]);
        antecedentResults.AddRow(["hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2.0^^http://www.w3.org/2001/XMLSchema#float"));
    }

    [TestMethod]
    public void ShouldEvaluateMultiplyBuiltInWithNumbers()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "1.0^^http://www.w3.org/2001/XMLSchema#float"
        ]);
        antecedentResults.AddRow(["-2^^http://www.w3.org/2001/XMLSchema#int", "-2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow([null, "2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", null]);
        antecedentResults.AddRow([null, null]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello"]);
        antecedentResults.AddRow(["hello", "-2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN"]);
        antecedentResults.AddRow(["hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "1.0^^http://www.w3.org/2001/XMLSchema#float"));
    }

    [TestMethod]
    public void ShouldEvaluateMultiplyBuiltInBindingFirstArgument()
    {
        //The first variable has no column: the built-in adds it and fills it with the computed value of the rows it can compute
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["14.50^^http://www.w3.org/2001/XMLSchema#decimal", "1.22^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "1.5^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["1.5^^http://www.w3.org/2001/XMLSchema#double", "2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow([null, "7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["6^^http://www.w3.org/2001/XMLSchema#int", null]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int", "42^^http://www.w3.org/2001/XMLSchema#integer"],
            ["14.50^^http://www.w3.org/2001/XMLSchema#decimal", "1.22^^http://www.w3.org/2001/XMLSchema#decimal", "17.69^^http://www.w3.org/2001/XMLSchema#decimal"],
            ["2^^http://www.w3.org/2001/XMLSchema#int", "1.5^^http://www.w3.org/2001/XMLSchema#decimal", "3.0^^http://www.w3.org/2001/XMLSchema#decimal"],
            ["1.5^^http://www.w3.org/2001/XMLSchema#double", "2^^http://www.w3.org/2001/XMLSchema#int", "3^^http://www.w3.org/2001/XMLSchema#double"]
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
    public void ShouldEvaluateMultiplyBuiltInInPredicateMode()
    {
        //The first variable is bound: a row is kept only when its first cell holds the computed value
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["42^^http://www.w3.org/2001/XMLSchema#integer", "6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["99^^http://www.w3.org/2001/XMLSchema#int", "6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["42.0^^http://www.w3.org/2001/XMLSchema#decimal", "6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["17.69^^http://www.w3.org/2001/XMLSchema#decimal", "14.50^^http://www.w3.org/2001/XMLSchema#decimal", "1.22^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["99^^http://www.w3.org/2001/XMLSchema#int", "14.50^^http://www.w3.org/2001/XMLSchema#decimal", "1.22^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["3.0^^http://www.w3.org/2001/XMLSchema#decimal", "2^^http://www.w3.org/2001/XMLSchema#int", "1.5^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["99^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int", "1.5^^http://www.w3.org/2001/XMLSchema#decimal"]);
        antecedentResults.AddRow(["3^^http://www.w3.org/2001/XMLSchema#double", "1.5^^http://www.w3.org/2001/XMLSchema#double", "2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["99^^http://www.w3.org/2001/XMLSchema#int", "1.5^^http://www.w3.org/2001/XMLSchema#double", "2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow([null, "6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        string[][] expectedRows = [
            ["42^^http://www.w3.org/2001/XMLSchema#integer", "6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"],
            ["42.0^^http://www.w3.org/2001/XMLSchema#decimal", "6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"],
            ["17.69^^http://www.w3.org/2001/XMLSchema#decimal", "14.50^^http://www.w3.org/2001/XMLSchema#decimal", "1.22^^http://www.w3.org/2001/XMLSchema#decimal"],
            ["3.0^^http://www.w3.org/2001/XMLSchema#decimal", "2^^http://www.w3.org/2001/XMLSchema#int", "1.5^^http://www.w3.org/2001/XMLSchema#decimal"],
            ["3^^http://www.w3.org/2001/XMLSchema#double", "1.5^^http://www.w3.org/2001/XMLSchema#double", "2^^http://www.w3.org/2001/XMLSchema#int"]
        ];
        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(5, builtinResults.Rows);
        for (int i = 0; i < expectedRows.Length; i++)
            for (int j = 0; j < expectedRows[i].Length; j++)
                Assert.AreEqual(expectedRows[i][j], builtinResults.Rows[i][j]);
    }

    [TestMethod]
    public void ShouldEvaluateMultiplyBuiltInWithUnexistingNonFirstArgument()
    {
        //A non-first variable with no column is unbound: no row can satisfy the built-in whether the first variable is bound or not
        RDFTable bindingResults = new RDFTable();
        bindingResults.AddColumn("?Y");
        bindingResults.AddColumn("?Z");
        bindingResults.AddRow(["6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"]);
        RDFTable predicateResults = new RDFTable();
        predicateResults.AddColumn("?X");
        predicateResults.AddColumn("?Y");
        predicateResults.AddColumn("?Z");
        predicateResults.AddRow(["42^^http://www.w3.org/2001/XMLSchema#integer", "6^^http://www.w3.org/2001/XMLSchema#int", "7^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin1 = SWRLBuiltIn.Multiply(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F")),
            new SWRLVariableArgument(new RDFVariable("?Z")));
        RDFTable bindingTable1 = builtin1.EvaluateOnAntecedent(bindingResults);
        Assert.HasCount(3, bindingTable1.Columns);
        Assert.IsEmpty(bindingTable1.Rows);
        RDFTable predicateTable1 = builtin1.EvaluateOnAntecedent(predicateResults);
        Assert.HasCount(3, predicateTable1.Columns);
        Assert.IsEmpty(predicateTable1.Rows);

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Multiply(
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

