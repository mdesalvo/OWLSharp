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
using OWLSharp.Ontology.Rules;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLBuiltInTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateBuiltIn()
        {
            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrl#example",
                Arguments = [
                    new SWRLVariableArgument(new RDFVariable("?VAR")),
                    new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                    new SWRLLiteralArgument(new RDFPlainLiteral("lit"))
                ]
            };

            Assert.IsNotNull(builtin);
            Assert.IsNotNull(builtin.IRI);
            Assert.IsTrue(string.Equals(builtin.IRI, "http://www.w3.org/2003/11/swrl#example"));
            Assert.IsNotNull(builtin.Arguments);
            Assert.IsTrue(builtin.Arguments.Count == 3);
            Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument varArg 
                                                    && string.Equals(varArg.IRI,"urn:swrl:var#VAR")
                                                    && varArg.GetVariable().Equals(new RDFVariable("?VAR")));
            Assert.IsTrue(builtin.Arguments[1] is SWRLIndividualArgument idvArg
                                                    && string.Equals(idvArg.IRI, "http://test.org/")
                                                    && idvArg.GetResource().Equals(new RDFResource("http://test.org/")));
            Assert.IsTrue(builtin.Arguments[2] is SWRLLiteralArgument litArg
                                                    && litArg.GetLiteral().Equals(new RDFPlainLiteral("lit")));
            Assert.IsTrue(string.Equals(builtin.ToString(), "swrlb:example(?VAR,http://test.org/,\"lit\")"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnEvaluatingBuiltInOnAntecedentBecauseUnknownIRI()
        {
            SWRLBuiltIn builtin = new SWRLBuiltIn()
            {
                IRI = "http://www.w3.org/2003/11/swrl#example",
                Arguments = [
                    new SWRLVariableArgument(new RDFVariable("?VAR")),
                    new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                    new SWRLLiteralArgument(new RDFPlainLiteral("lit"))
                ]
            };
            DataTable table = new DataTable();
            table.Columns.Add("?VAR");
            table.Rows.Add("value");
            Assert.ThrowsException<OWLException>(() => builtin.EvaluateOnAntecedent(table));
        }
        #endregion
    }
}