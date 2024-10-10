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
            Assert.IsNull(builtin.EvaluatorFunction);
        }

        [TestMethod]
        public void ShouldCreateBuiltInWithEvaluator()
        {
            bool Evaluator(DataRow datarow) => true;

            SWRLBuiltIn builtin = new SWRLBuiltIn(
                Evaluator,
                new RDFResource("http://www.w3.org/2003/11/swrl#example"),
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit")));

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
            Assert.IsNotNull(builtin.EvaluatorFunction);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingBuiltInWithEvaluatorBecauseNullEvaluator()
            => Assert.ThrowsException<SWRLException>(() => new SWRLBuiltIn(
                null,
                new RDFResource("http://www.w3.org/2003/11/swrl#example"),
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingBuiltInWithEvaluatorBecauseNullIRI()
        {
            bool Evaluator(DataRow datarow) => true;

            Assert.ThrowsException<SWRLException>(() => new SWRLBuiltIn(
                Evaluator,
                null,
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit"))));
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
            Assert.ThrowsException<SWRLException>(() => builtin.EvaluateOnAntecedent(table));
        }

        [TestMethod]
        public void ShouldEvaluateBuiltInWithEvaluatorOnAntecedent()
        {
            bool Evaluator(DataRow datarow) => string.Equals(datarow["?VAR"].ToString(), "value");

            SWRLBuiltIn builtin = new SWRLBuiltIn(
                Evaluator,
                new RDFResource("http://www.w3.org/2003/11/swrl#exampleRegistered"),
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit")));
            DataTable table = new DataTable();
            table.Columns.Add("?VAR");
            table.Rows.Add("value");
            table.Rows.Add("value2");
            table.Rows.Add("value3");
            SWRLBuiltInRegister.AddBuiltIn(builtin);

            Assert.IsTrue(builtin.EvaluateOnAntecedent(table).Rows.Count == 1);
        }

        [TestMethod]
        public void ShouldExportBuiltinToRDFGraph()
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
            RDFGraph graph = builtin.ToRDFGraph(new RDFCollection(RDFModelEnums.RDFItemTypes.Resource));

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 13);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.BODY, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.HEAD, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATARANGE_ATOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATAVALUED_PROPERTY_ATOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DIFFERENT_INDIVIDUALS_ATOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.INDIVIDUAL_PROPERTY_ATOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.SAME_INDIVIDUAL_ATOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.ARGUMENT1, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.ARGUMENT2, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.ARGUMENTS, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.BUILTIN_PROP, new RDFResource("http://www.w3.org/2003/11/swrl#example"), null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("urn:swrl:var#VAR"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.ATOMLIST, null].TriplesCount == 0);
        }
        #endregion
    }
}