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
        OWLTable antecedentResults = new OWLTable();
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

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "Hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), ""));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
        OWLTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(2, builtinResults2.Columns);
        Assert.HasCount(13, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.UpperCase(
            new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")));
        OWLTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(2, builtinResults3.Columns);
        Assert.HasCount(13, builtinResults3.Rows);

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
        OWLTable antecedentResults = new OWLTable();
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

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

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
        OWLTable antecedentResults = new OWLTable();
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

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(3, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "HELLO^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "HELLO"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?X"] ?? string.Empty), "HELLO@EN"));
    }

    [TestMethod]
    public void ShouldEvaluateUpperCaseBuiltInWithIndividualArguments()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?dummy");
        antecedentResults.AddRow(["dummy"]);

        //RDFResource always normalizes the scheme/authority of a Uri to lowercase (standard .NET Uri behavior),
        //so its ToString() is never fully uppercase even when constructed from an uppercase IRI: the left side
        //keeps "http://example.org/HELLO", while ToUpper() on the right always yields "HTTP://EXAMPLE.ORG/HELLO",
        //so they can never match when the resource has an alphabetic scheme
        SWRLBuiltIn builtin = SWRLBuiltIn.UpperCase(
            new SWRLIndividualArgument(new RDFResource("HTTP://EXAMPLE.ORG/HELLO")),
            new SWRLIndividualArgument(new RDFResource("HTTP://EXAMPLE.ORG/HELLO")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.IsEmpty(builtinResults.Rows);
    }
    #endregion
}

