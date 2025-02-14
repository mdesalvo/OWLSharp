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
public class SWRLMatchesBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateMatchesBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(2, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:matches(?X,?Y)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Matches(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Matches(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldCreateMatchesBuiltInWithFlags()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(3, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2 
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(string.Equals("swrlb:matches(?X,?Y,?Z)", builtin.ToString()));
    }

    [TestMethod]
    public void ShouldSerializeMatchesBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldSerializeMatchesBuiltInWithFlags()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFPlainLiteral("ismx")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Literal>ismx</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }


    [TestMethod]
    public void ShouldDeserializeMatchesBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#matches"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(2, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(string.Equals("swrlb:matches(?X,?Y)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#matches"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }
        
    [TestMethod]
    public void ShouldDeserializeMatchesBuiltInWithFlags()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#matches"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Literal>ismx</Literal></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#matches", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(3, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLLiteralArgument rlarg2 
                      && rlarg2.GetLiteral().Equals(new RDFPlainLiteral("ismx")));
        Assert.IsTrue(string.Equals("swrlb:matches(?X,?Y,\"ismx\")", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#matches\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Literal>ismx</Literal></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldEvaluateMatchesBuiltIn()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Rows.Add("hello", "ello$");
        antecedentResults.Rows.Add("hello", "eLLo$");
        antecedentResults.Rows.Add("hello@EN-US", "EN-US$");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2");
        antecedentResults.Rows.Add(DBNull.Value, "^hello");
        antecedentResults.Rows.Add("hello", DBNull.Value);
        antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
        antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
        antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
        antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("http://example.org/test/", "^http");

        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(2, builtinResults.Columns.Count);
        Assert.AreEqual(4, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "ello$"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Y"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?X"].ToString(), "http://example.org/test/"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Y"].ToString(), "^http"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Z"))); //unexisting
        DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.AreEqual(2, builtinResults2.Columns.Count);
        Assert.AreEqual(14, builtinResults2.Rows.Count);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?Z")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?Y")));
        DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.AreEqual(2, builtinResults3.Columns.Count);
        Assert.AreEqual(14, builtinResults3.Rows.Count);

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
            IRI = "http://www.w3.org/2003/11/swrlb#matches",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateMatchesBuiltInWithLiteralLeftArgument()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Rows.Add("ello$");
        antecedentResults.Rows.Add("eLLo$");
        antecedentResults.Rows.Add("EN-US$");
        antecedentResults.Rows.Add("2");
        antecedentResults.Rows.Add("^hello");
        antecedentResults.Rows.Add(DBNull.Value);
        antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("^hello$");

        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLLiteralArgument(new RDFPlainLiteral("hello")),
            new SWRLVariableArgument(new RDFVariable("?Y")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(1, builtinResults.Columns.Count);
        Assert.AreEqual(5, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "ello$"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "^hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Y"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Y"].ToString(), "hello^^http://www.w3.org/2001/XMLSchema#string"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[4]["?Y"].ToString(), "^hello$"));
    }

    [TestMethod]
    public void ShouldEvaluateMatchesBuiltInWithLiteralRightArgument()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Rows.Add("hello");
        antecedentResults.Rows.Add("eLLo$");
        antecedentResults.Rows.Add("2");
        antecedentResults.Rows.Add("lo");
        antecedentResults.Rows.Add(DBNull.Value);
        antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int");

        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFPlainLiteral("ello$")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(1, builtinResults.Columns.Count);
        Assert.AreEqual(2, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "hello^^http://www.w3.org/2001/XMLSchema#string"));
    }

    [TestMethod]
    public void ShouldEvaluateMatchesBuiltInWithFlags()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Rows.Add("hello", "eLLo$");
        antecedentResults.Rows.Add("hello@EN-US", "EN-US$");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "2");
        antecedentResults.Rows.Add(DBNull.Value, "^hello");
        antecedentResults.Rows.Add("hello", DBNull.Value);
        antecedentResults.Rows.Add(DBNull.Value, DBNull.Value);
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello^^http://www.w3.org/2001/XMLSchema#string");
        antecedentResults.Rows.Add("hello^^http://www.w3.org/2001/XMLSchema#string", "2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello");
        antecedentResults.Rows.Add("hello", "-2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("2^^http://www.w3.org/2001/XMLSchema#int", "hello@EN");
        antecedentResults.Rows.Add("hello@EN", "-2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("HTtp://example.org/test/", "^http");

        SWRLBuiltIn builtin = SWRLBuiltIn.Matches(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFPlainLiteral("ismx")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(2, builtinResults.Columns.Count);
        Assert.AreEqual(4, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "eLLo$"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[2]["?Y"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?X"].ToString(), "HTtp://example.org/test/"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[3]["?Y"].ToString(), "^http"));
    }
    #endregion
}