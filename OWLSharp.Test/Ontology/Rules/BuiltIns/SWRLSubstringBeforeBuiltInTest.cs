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
public class SWRLSubstringBeforeBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateSubstringBeforeBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#substringBefore", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(3, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(string.Equals("swrlb:substringBefore(?X,?Y,?Z)", builtin.ToString()));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.SubstringBefore(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
        Assert.ThrowsExactly<SWRLException>(() => _ = SWRLBuiltIn.SubstringBefore(new SWRLVariableArgument(new RDFVariable("?X")), null));
    }

    [TestMethod]
    public void ShouldSerializeSubstringBeforeBuiltIn()
    {
        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substringBefore\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));
    }

    [TestMethod]
    public void ShouldDeserializeSubstringBeforeBuiltIn()
    {
        SWRLBuiltIn builtin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#substringBefore"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Variable IRI="urn:swrl:var#Z" /></BuiltInAtom>""");

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#substringBefore", builtin.IRI));
        Assert.IsNotNull(builtin.Arguments);
        Assert.HasCount(3, builtin.Arguments);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg
                      && vlarg.GetVariable().Equals(new RDFVariable("?X")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg
                      && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2
                      && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
        Assert.IsTrue(string.Equals("swrlb:substringBefore(?X,?Y,?Z)", builtin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substringBefore\"><Variable IRI=\"urn:swrl:var#X\" /><Variable IRI=\"urn:swrl:var#Y\" /><Variable IRI=\"urn:swrl:var#Z\" /></BuiltInAtom>", OWLSerializer.SerializeObject(builtin)));

        //Test string handling for empty builtIns

        SWRLBuiltIn emptyBuiltin = OWLSerializer.DeserializeObject<SWRLBuiltIn>(
            """<BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#substringBefore"></BuiltInAtom>""");

        Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
        Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substringBefore\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBeforeBuiltIn()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(new string[] { "t", "tattoo", "attoo" });
        antecedentResults.AddRow(new string[] { "http://example.org/", "http://example.org/test", "test" });

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(3, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "t"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "attoo"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), "http://example.org/"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Y"].ToString(), "http://example.org/test"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "test"));

        //Test with unexisting variables

        SWRLBuiltIn builtin2 = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLVariableArgument(new RDFVariable("?F"))); //unexisting
        RDFTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults2);
        Assert.HasCount(3, builtinResults2.Columns);
        Assert.HasCount(2, builtinResults2.Rows);

        SWRLBuiltIn builtin3 = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")));
        RDFTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
        Assert.IsNotNull(builtinResults3);
        Assert.HasCount(3, builtinResults3.Columns);
        Assert.HasCount(2, builtinResults3.Rows);

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
            IRI = "http://www.w3.org/2003/11/swrlb#substringBefore",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?V"))
            ]
        }.EvaluateOnAntecedent(antecedentResults));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBeforeBuiltInWithRightArgumentSTR()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(new string[] { "t", "attoo" });
        antecedentResults.AddRow(new string[] { "", "tat" });

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(new RDFPlainLiteral("tattoo")),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(2, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "t"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "attoo"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "tat"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBeforeBuiltInWithRightArgumentEmptySTR()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Z");
        antecedentResults.AddRow(new string[] { "", "attoo" });
        antecedentResults.AddRow(new string[] { "tattoo", "tat" });

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLLiteralArgument(RDFPlainLiteral.Empty),
            new SWRLVariableArgument(new RDFVariable("?Z")));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "attoo"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBeforeBuiltInWithRightArgumentSEP()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(new string[] { "t", "tattoo" });
        antecedentResults.AddRow(new string[] { "http://example.org", "http://tattoo.org/test" });

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("attoo", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "t"));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
    }

    [TestMethod]
    public void ShouldEvaluateSubstringBeforeBuiltInWithRightArgumentEmptySEP()
    {
        RDFTable antecedentResults = new RDFTable();
        antecedentResults.AddColumn("?X");
        antecedentResults.AddColumn("?Y");
        antecedentResults.AddRow(new string[] { "", "tattoo" });
        antecedentResults.AddRow(new string[] { "http://example.org", "http://tattoo.org/test" });

        SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
            new SWRLVariableArgument(new RDFVariable("?X")),
            new SWRLVariableArgument(new RDFVariable("?Y")),
            new SWRLLiteralArgument(new RDFTypedLiteral("", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        RDFTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

        Assert.IsNotNull(builtinResults);
        Assert.HasCount(2, builtinResults.Columns);
        Assert.HasCount(1, builtinResults.Rows);
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), ""));
        Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
    }
    #endregion
}