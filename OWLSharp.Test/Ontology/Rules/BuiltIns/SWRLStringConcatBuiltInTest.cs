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
public class SWRLStringConcatBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateStringConcatBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.StringConcat(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringConcat", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:stringConcat(?X,?Y)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.StringConcat(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.StringConcat(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeStringConcatBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.StringConcat(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringConcat\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeStringConcatBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#stringConcat"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringConcat", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:stringConcat(?X,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringConcat\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#stringConcat"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringConcat\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldDeserializeStringConcatBuiltInWithLeftNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#stringConcat"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringConcat", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLLiteralArgument tlarg
                      && tlarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:stringConcat(\"5\"^^xsd:integer,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringConcat\"><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeStringConcatBuiltInWithRightNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#stringConcat"><Variable IRI="urn:swrl:var#X" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#stringConcat", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(2, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument trarg
                      && trarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(string.Equals("swrlb:stringConcat(?X,\"5\"^^xsd:integer)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#stringConcat\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldEvaluateStringConcatBuiltIn()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(["hello", "h", "ello"]);
        antecedentResults.AddRow(["hello@EN-US", "EN-US", null]);
        antecedentResults.AddRow(["22", "2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int"
        ]);
        antecedentResults.AddRow(["-2", null, "-2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", null, null]);
        antecedentResults.AddRow(["hello", "hello@EN", null]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "2", "hello^^http://www.w3.org/2001/XMLSchema#string"
        ]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string", null, "hello@EN--ltr"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello", null]);
        antecedentResults.AddRow(["hello", "-2^^http://www.w3.org/2001/XMLSchema#int", null]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN", null]);
        antecedentResults.AddRow(["hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int", null]);
        antecedentResults.AddRow(["http://example.org/test/", "http://example.org/", "test/"]);

        SWRLBuiltIn builtin = SWRLBuiltIn.StringConcat(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(4, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "h"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Z"] ?? string.Empty), "ello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?X"] ?? string.Empty), "hello"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "hello@EN"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Z"] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?X"] ?? string.Empty), "hello^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?Y"] ?? string.Empty), ""));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?Z"] ?? string.Empty), "hello@EN--ltr"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[3]["?X"] ?? string.Empty), "http://example.org/test/"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[3]["?Y"] ?? string.Empty), "http://example.org/"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[3]["?Z"] ?? string.Empty), "test/"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.StringConcat(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
        OWLTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(3, builtinResults2.Columns);
        Assert.IsEmpty(builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.StringConcat(
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")));
        OWLTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(3, builtinResults3.Columns);
        Assert.IsEmpty(builtinResults3.Rows);

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
            IRI = "http://www.w3.org/2003/11/swrlb#stringConcat",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateStringConcatBuiltInWithLeftString()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(["llo"]);
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2.0^^http://www.w3.org/2001/XMLSchema#float"]);
        antecedentResults.AddRow([null]);
        antecedentResults.AddRow(["llo^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["llo@EN"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#stringConcat",
            Arguments = [
                new SWRLLiteralArgument(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)),
                new SWRLLiteralArgument(new RDFTypedLiteral("he", RDFModelEnums.RDFDatatypes.XSD_STRING)),
                new SWRLVariableArgument(new RDFVariable("?Y"))
            ]
        };

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(3, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?Y"] ?? string.Empty), "llo"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[1]["?Y"] ?? string.Empty), "llo^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals((builtinResults.Rows[2]["?Y"] ?? string.Empty), "llo@EN"));
    }

    [TestMethod]
    public void ShouldEvaluateStringConcatBuiltInWithRightString()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddRow(["2^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["7^^http://www.w3.org/2001/XMLSchema#int"]);
        antecedentResults.AddRow(["2.0^^http://www.w3.org/2001/XMLSchema#float"]);
        antecedentResults.AddRow([null]);
        antecedentResults.AddRow(["hello^^http://www.w3.org/2001/XMLSchema#string"]);
        antecedentResults.AddRow(["hello"]);
        antecedentResults.AddRow(["heLlo"]);
        antecedentResults.AddRow(["hello@EN"]);
        antecedentResults.AddRow(["http://example.org/test"]);

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#stringConcat",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLIndividualArgument(new RDFResource("http://example.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("test","en"))
            ]
        };

        OWLTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(1, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals((builtinResults.Rows[0]["?X"] ?? string.Empty), "http://example.org/test"));
    }

    [TestMethod]
    public void ShouldEvaluateStringConcatBuiltInWithMultipleArguments()
    {
        OWLTable antecedentResults = new OWLTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddRow(["The white fox jumps"]);
        antecedentResults.AddRow(["The white fox jumpsover the lazy..."]);
        antecedentResults.AddRow(["The white fox jumps over the lazy..."]);

        SWRLBuiltIn builtin2 = SWRLBuiltIn.StringConcat(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFPlainLiteral("The white fox jumps")),
            new SWRLLiteralArgument(new RDFPlainLiteral(" over","en")),
            new SWRLLiteralArgument(new RDFTypedLiteral(" the lazy...", RDFModelEnums.RDFDatatypes.XSD_STRING))
        );

        OWLTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(1, builtinResults2.Columns);
        Assert.HasCount(1, builtinResults2.Rows);
        Assert.IsTrue(string.Equals((builtinResults2.Rows[0]["?X"] ?? string.Empty), "The white fox jumps over the lazy..."));
    }
    #endregion
}

