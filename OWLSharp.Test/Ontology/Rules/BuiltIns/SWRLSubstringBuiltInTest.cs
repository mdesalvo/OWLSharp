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
public class SWRLSubstringBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateSubstringBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#substring", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(4, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3
                      && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
        Assert.IsTrue(string.Equals("swrlb:substring(?X,?Y,?Z,?Q)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Substring(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.Substring(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeSubstringBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substring\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeSubstringBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#substring"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Variable IRI="urn:swrl:var#Z" /><Variable IRI="urn:swrl:var#Q" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#substring", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(4, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(builtin.Arguments[3] is SWRLVariableArgument rlarg3
                      && rlarg3.GetVariable().Equals(new RDFVariable("?Q")));
        Assert.IsTrue(string.Equals("swrlb:substring(?X,?Y,?Z,?Q)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substring\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /><Variable IRI=\"urn:swrl:var#Q\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#substring"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substring\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBuiltIn()
    {
        // W3C SWRL spec uses 1-based indexing (aligned with XPath fn:substring):
        // substring("hello", 2) = "ello"  (from position 2, i.e. the second character)
        // substring("http://example.org/test", 8) = "example.org/test"  (h=1..e=8)
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Columns.Add("?Z");
        antecedentResults.Rows.Add("ello", "hello", "2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("@EN-US", "hello@EN-US", "4^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("example.org/test", "http://example.org/test", "8^^http://www.w3.org/2001/XMLSchema#int");

        SWRLBuiltIn builtin = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "ello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "example.org/test"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "http://example.org/test"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "8^^http://www.w3.org/2001/XMLSchema#int"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
        DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(3, builtinResults2.Columns);
        Assert.HasCount(3, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));
        DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(3, builtinResults3.Columns);
        Assert.HasCount(3, builtinResults3.Rows);

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
            IRI = "http://www.w3.org/2003/11/swrlb#substring",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBuiltInWithStringRightArgumentSRC()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Z");
        // 1-based: substring("hello", 2) = "ello"; substring("hello", 4) = "o" ≠ "@EN-US"
        antecedentResults.Rows.Add("ello", "2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("@EN-US", "4^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("llo", "2^^http://www.w3.org/2001/XMLSchema#int");

        SWRLBuiltIn builtin = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "ello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBuiltInWithNumericRightArgumentIDX()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        // 1-based literal index 2: substring("hello", 2) = "ello"
        antecedentResults.Rows.Add("ello", "hello");
        antecedentResults.Rows.Add("@EN-US", "hello@EN-US");
        antecedentResults.Rows.Add("org/test", "http://example.org/test");

        SWRLBuiltIn builtin = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "ello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "hello"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBuiltInWithNumericRightArgumentIDXLEN()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        // 1-based literal index 2, len 2: substring("hello",2,2)="el"; substring("http://...",2,2)="tt"
        antecedentResults.Rows.Add("el", "hello");
        antecedentResults.Rows.Add("@EN-US", "hello@EN-US");
        antecedentResults.Rows.Add("tt", "http://example.org/test");

        SWRLBuiltIn builtin = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)),
            new SWRLLiteralArgument(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "el"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "tt"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "http://example.org/test"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBuiltInWithLength()
    {
        DataTable antecedentResults = new DataTable();
        antecedentResults.Columns.Add("?X");
        antecedentResults.Columns.Add("?Y");
        antecedentResults.Columns.Add("?Z");
        antecedentResults.Columns.Add("?Q");
        // 1-based: substring("hello",2,2)="el"; substring("hello@EN-US",4,1)="l"≠"@EN-US";
        // substring("http://example.org/test",16,3)="org"  (h=1..o=16)
        antecedentResults.Rows.Add("el", "hello", "2^^http://www.w3.org/2001/XMLSchema#int", "2^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("@EN-US", "hello@EN-US", "4^^http://www.w3.org/2001/XMLSchema#int", "1^^http://www.w3.org/2001/XMLSchema#int");
        antecedentResults.Rows.Add("org", "http://example.org/test", "16^^http://www.w3.org/2001/XMLSchema#int", "3^^http://www.w3.org/2001/XMLSchema#int");

        SWRLBuiltIn builtin = SWRLBuiltIn.Substring(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(4, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "el"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "hello"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Q"].ToString(), "2^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "org"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "http://example.org/test"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "16^^http://www.w3.org/2001/XMLSchema#int"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Q"].ToString(), "3^^http://www.w3.org/2001/XMLSchema#int"));
    }
    #endregion
}