/*
   Copyright 2014-2024 Marco De Salvo

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
using OWLSharp.Ontology.Rules;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;

namespace OWLSharp.Test.Ontology.Rules
{
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
            Assert.IsTrue(builtin.Arguments.Count == 3);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument vlarg 
                            && vlarg.GetVariable().Equals(new RDFVariable("?X")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLVariableArgument rlarg 
                            && rlarg.GetVariable().Equals(new RDFVariable("?Y")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLVariableArgument rlarg2 
                            && rlarg2.GetVariable().Equals(new RDFVariable("?Z")));
            Assert.IsTrue(string.Equals("swrlb:substringBefore(?X,?Y,?Z)", builtin.ToString()));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.SubstringBefore(null, new SWRLVariableArgument(new RDFVariable("?Y"))));
            Assert.ThrowsException<OWLException>(() => SWRLBuiltIn.SubstringBefore(new SWRLVariableArgument(new RDFVariable("?X")), null));
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
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#substringBefore""><Variable IRI=""urn:swrl:var#X"" /><Variable IRI=""urn:swrl:var#Y"" /><Variable IRI=""urn:swrl:var#Z"" /></BuiltInAtom>");

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals("http://www.w3.org/2003/11/swrlb#substringBefore", builtin.IRI));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 3);
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
@"<BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#substringBefore""></BuiltInAtom>");

            Assert.IsTrue(string.Equals(string.Empty, emptyBuiltin.ToString()));
            Assert.IsTrue(string.Equals("<BuiltInAtom IRI=\"http://www.w3.org/2003/11/swrlb#substringBefore\" />", OWLSerializer.SerializeObject(emptyBuiltin)));
        }

        [TestMethod]
        public void ShouldEvaluateSubstringBeforeBuiltIn()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Rows.Add("t", "tattoo", "attoo");
            antecedentResults.Rows.Add("http://example.org/", "http://example.org/test", "test");

            SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 3);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
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
            DataTable builtinResults2 = builtin2.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults2);
            Assert.IsTrue(builtinResults2.Columns.Count == 3);
            Assert.IsTrue(builtinResults2.Rows.Count == 2);

            SWRLBuiltIn builtin3 = SWRLBuiltIn.SubstringBefore(
                new SWRLVariableArgument(new RDFVariable("?F")),  //unexisting
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")));
            DataTable builtinResults3 = builtin3.EvaluateOnAntecedent(antecedentResults);
            Assert.IsNotNull(builtinResults3);
            Assert.IsTrue(builtinResults3.Columns.Count == 3);
            Assert.IsTrue(builtinResults3.Rows.Count == 2);

            //Test exception on unknown builtIn
            Assert.ThrowsException<OWLException>(() =>
                new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#example",
                    Arguments = [
                        new SWRLVariableArgument(new RDFVariable("?X")),
                        new SWRLVariableArgument(new RDFVariable("?Y"))
                    ]
                }.EvaluateOnAntecedent(antecedentResults));

            //Test exception on bad-formed builtIn
            Assert.ThrowsException<OWLException>(() => 
                new SWRLBuiltIn()
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
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Rows.Add("t", "attoo");
            antecedentResults.Rows.Add("", "tat");

            SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFPlainLiteral("tattoo")),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 2);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "t"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "attoo"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[1]["?Z"].ToString(), "tat"));
        }

        [TestMethod]
        public void ShouldEvaluateSubstringBeforeBuiltInWithRightArgumentEmptySTR()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Z");
            antecedentResults.Rows.Add("", "attoo");
            antecedentResults.Rows.Add("tattoo", "tat");

            SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLLiteralArgument(new RDFPlainLiteral("")),
                new SWRLVariableArgument(new RDFVariable("?Z")));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Z"].ToString(), "attoo"));
        }

        [TestMethod]
        public void ShouldEvaluateSubstringBeforeBuiltInWithRightArgumentSEP()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("t", "tattoo");
            antecedentResults.Rows.Add("http://example.org", "http://tattoo.org/test");

            SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLLiteralArgument(new RDFTypedLiteral("attoo", RDFModelEnums.RDFDatatypes.XSD_STRING)));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), "t"));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
        }

        [TestMethod]
        public void ShouldEvaluateSubstringBeforeBuiltInWithRightArgumentEmptySEP()
        {
            DataTable antecedentResults = new DataTable();
            antecedentResults.Columns.Add("?X");
            antecedentResults.Columns.Add("?Y");
            antecedentResults.Rows.Add("", "tattoo");
            antecedentResults.Rows.Add("http://example.org", "http://tattoo.org/test");

            SWRLBuiltIn builtin = SWRLBuiltIn.SubstringBefore(
                new SWRLVariableArgument(new RDFVariable("?X")),
                new SWRLVariableArgument(new RDFVariable("?Y")),
                new SWRLLiteralArgument(new RDFTypedLiteral("", RDFModelEnums.RDFDatatypes.XSD_STRING)));

            DataTable builtinResults = builtin.EvaluateOnAntecedent(antecedentResults);

            Assert.IsNotNull(builtinResults);
            Assert.IsTrue(builtinResults.Columns.Count == 2);
            Assert.IsTrue(builtinResults.Rows.Count == 1);
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?X"].ToString(), ""));
            Assert.IsTrue(string.Equals(builtinResults.Rows[0]["?Y"].ToString(), "tattoo"));
        }
        #endregion
    }
}