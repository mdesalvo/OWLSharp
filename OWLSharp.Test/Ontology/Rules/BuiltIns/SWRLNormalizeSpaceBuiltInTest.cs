/*
   Copyright 2014-2025 Marco De Salvo

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

namespace OWLSharp.Test.Ontology;

[TestClass]
public class SWRLNormalizeSpaceBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateNormalizeSpaceBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.NormalizeSpace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#normalizeSpace", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(2, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:normalizeSpace(?X,?Y)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.NormalizeSpace(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.NormalizeSpace(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeNormalizeSpaceBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.NormalizeSpace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#normalizeSpace\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeNormalizeSpaceBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#normalizeSpace"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#normalizeSpace", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(2, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:normalizeSpace(?X,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#normalizeSpace\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#normalizeSpace"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#normalizeSpace\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldDeserializeNormalizeSpaceBuiltInWithLeftNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#normalizeSpace"><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#normalizeSpace", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(2, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLLiteralArgument tlarg
                      && tlarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:normalizeSpace(\"5\"^^xsd:integer,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#normalizeSpace\"><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeNormalizeSpaceBuiltInWithRightNumber()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#normalizeSpace"><Variable IRI="urn:swrl:var#X" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#integer">5</Literal></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#normalizeSpace", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(2, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLLiteralArgument trarg
                      && trarg.GetLiteral().Equals(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
        Assert.IsTrue(string.Equals("swrlb:normalizeSpace(?X,\"5\"^^xsd:integer)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#normalizeSpace\"><Variable IRI=\"urn:swrl:var#X\" /><Literal datatypeIRI=\"http://www.w3.org/2001/XMLSchema#integer\">5</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldEvaluateNormalizeSpaceBuiltIn()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Rows.Add(" hel lo ", " hel   lo ");
        antecedentResults.Rows.Add("hello@EN-US", "EN-US");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add(DBNull.Value, "-2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", DBNull.Value);
        antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
        antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
        antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
        antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");

        SWRLBuiltIn builtin = SWRLBuiltIn.NormalizeSpace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(2, builtinResults.Columns.Count);
        Assert.AreEqual(2, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), " hel lo "));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), " hel   lo "));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), ""));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.NormalizeSpace(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
        DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.AreEqual(2, builtinResults2.Columns.Count);
        Assert.AreEqual(12, builtinResults2.Rows.Count);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.NormalizeSpace(
            new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")));
        DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.AreEqual(2, builtinResults3.Columns.Count);
        Assert.AreEqual(12, builtinResults3.Rows.Count);

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
            IRI = "http://www.w3.org/2003/11/swrlb#normalizeSpace",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateNormalizeSpaceBuiltInWithLeftString()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Rows.Add(" hel    lo  ");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2.0^^http://www.w3.org/2001/XMLSchema#float");
        antecedentResults.Rows.Add(DBNull.Value);
        antecedentResults.Rows.Add(" hel  lo ^^http://www.w3.org/2001/XMLSchema#string");
        antecedentResults.Rows.Add(" hel    lo ");
        antecedentResults.Rows.Add("heLlo");
        antecedentResults.Rows.Add("     hel lo @EN");

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#normalizeSpace",
            Arguments = [
                new SWRLLiteralArgument(new RDFTypedLiteral(" hel lo ", RDFModelEnums.RDFDatatypes.XSD_STRING)),
                new SWRLVariableArgument(new RDFVariable("?Y"))
            ]
        };

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(1, builtinResults.Columns.Count);
        Assert.AreEqual(4, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), " hel    lo  "));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), " hel  lo ^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Y"].ToString(), " hel    lo "));
        Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Y"].ToString(), "     hel lo @EN"));
    }

    [TestMethod]
    public void ShouldEvaluateNormalizeSpaceBuiltInWithRightString()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("7^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2.0^^http://www.w3.org/2001/XMLSchema#float");
        antecedentResults.Rows.Add(DBNull.Value);
        antecedentResults.Rows.Add(" h e l lo ^^http://www.w3.org/2001/XMLSchema#string");
        antecedentResults.Rows.Add(" h e l lo ");
        antecedentResults.Rows.Add(" h e L lo ");
        antecedentResults.Rows.Add(" h e l lo @EN");
        antecedentResults.Rows.Add("http://example.org/hello");

        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://www.w3.org/2003/11/swrlb#normalizeSpace",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFPlainLiteral("       h e l    lo  "))
            ]
        };

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(1, builtinResults.Columns.Count);
        Assert.AreEqual(3, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), " h e l lo ^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), " h e l lo "));
        Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), " h e l lo @EN"));
    }
    #endregion
}