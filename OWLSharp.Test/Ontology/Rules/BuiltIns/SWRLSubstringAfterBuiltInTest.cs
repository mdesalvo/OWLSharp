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
using System.Data;

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
        Assert.AreEqual(3, builtin.Arguments.Count);
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
        Assert.AreEqual(3, builtin.Arguments.Count);
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
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Columns.Add("?Z");
        antecedentResults.Rows.Add("too", "tattoo", "at");
        antecedentResults.Rows.Add(".org/", "http://example.org/", "ample");

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(3, builtinResults.Columns.Count);
        Assert.AreEqual(2, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "too"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "at"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), ".org/"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "http://example.org/"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "ample"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
        DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.AreEqual(3, builtinResults2.Columns.Count);
        Assert.AreEqual(2, builtinResults2.Rows.Count);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));
        DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.AreEqual(3, builtinResults3.Columns.Count);
        Assert.AreEqual(2, builtinResults3.Rows.Count);

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
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Z");
        antecedentResults.Rows.Add("too", "at");
        antecedentResults.Rows.Add("o", "to");

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFPlainLiteral("tattoo")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(2, builtinResults.Columns.Count);
        Assert.AreEqual(2, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "too"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "at"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "o"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "to"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithRightArgumentEmptySTR()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Z");
        antecedentResults.Rows.Add("", "attoo");
        antecedentResults.Rows.Add("tattoo", "tat");

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(RDFPlainLiteral.Empty),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(2, builtinResults.Columns.Count);
        Assert.AreEqual(1, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "attoo"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithRightArgumentSEP()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Rows.Add("o", "tattoo");
        antecedentResults.Rows.Add("o.org/test/", "http://tattoo.org/test/");

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("atto", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(2, builtinResults.Columns.Count);
        Assert.AreEqual(2, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "o"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "o.org/test/"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "http://tattoo.org/test/"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringAfterBuiltInWithRightArgumentEmptySEP()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Rows.Add("", "tattoo");
        antecedentResults.Rows.Add("http://example.org", "http://tattoo.org/test");

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringAfter(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.AreEqual(2, builtinResults.Columns.Count);
        Assert.AreEqual(1, builtinResults.Rows.Count);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
    }
    #endregion
}