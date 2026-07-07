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
using System;
using System.Data;
using OWLSharp;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class SWRLNotEqualBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateNotEqualBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.NotEqual(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#notEqual", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:notEqual(?X,?Y)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.NotEqual(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeNotEqualBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.NotEqual(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#notEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeNotEqualBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#notEqual"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#notEqual", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:notEqual(?X,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#notEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#notEqual"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#notEqual\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldDeserializeNotEqualBuiltInWithLeftNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#notEqual"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#notEqual", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLLiteralArgument tlarg
                      && tlarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:notEqual(\"5\"^^xsd:integer,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#notEqual\"><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeNotEqualBuiltInWithRightNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#notEqual"><Variable IRI="urn:swrl:var#X" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#notEqual", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument trarg
                      && trarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(string.Equals("swrlb:notEqual(?X,\"5\"^^xsd:integer)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#notEqual\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldEvaluateNotEqualBuiltIn()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2.0^^http://www.w3.org/2001/XMLSchema#float"
        ]);
        antecedentResults.AddRow(["-2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow([null, "-2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow([null, null]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello"]);
        antecedentResults.AddRow(["hello", "hello"]);
        antecedentResults.AddRow(["hello", "hello@EN-US"]);
        antecedentResults.AddRow(["http://example.org/idv1", "http://example.org/idv2"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.NotEqual(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(3, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "hello@EN-US"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?X"] ?? string.Empty), "http://example.org/idv1"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?Y"] ?? string.Empty), "http://example.org/idv2"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.NotEqual(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
        OWLTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(2, builtinResults2.Columns);
        Assert.HasCount(9, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.NotEqual(
            new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")));
        OWLTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(2, builtinResults3.Columns);
        Assert.HasCount(9, builtinResults3.Rows);

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
            IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateNotEqualBuiltInWithLeftResource()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["http://example.org/test"]);
        antecedentResults.AddRow(["http://example.org/test2"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
            Arguments = [
                new SWRLIndividualArgument(new RDFResource("http://example.org/test")),
                new SWRLVariableArgument(new RDFVariable("?Y"))
            ]
        };

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "http://example.org/test2"));
    }

    [TestMethod]
    public void ShouldEvaluateNotEqualBuiltInWithLeftLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["-2^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
            Arguments = [
                new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)),
                new SWRLVariableArgument(new RDFVariable("?Y"))
            ]
        };

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#int"));
    }

    [TestMethod]
    public void ShouldEvaluateNotEqualBuiltInWithRightResource()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddRow(["http://example.org/test1"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLIndividualArgument(new RDFResource("http://example.org/test"))
            ]
        };

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "http://example.org/test1"));
    }

    [TestMethod]
    public void ShouldEvaluateNotEqualBuiltInWithRightLiteral()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddRow(["-2^^http://www.w3.org/2001/XMLSchema#int"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#notEqual",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER))
            ]
        };

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "-2^^http://www.w3.org/2001/XMLSchema#int"));
    }
    #endregion
}

