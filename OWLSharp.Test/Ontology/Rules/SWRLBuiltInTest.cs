﻿/*
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
public class SWRLBuiltInTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateBuiltIn()
    {
        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://example.org/testBuiltIn",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit"))
            ]
        };

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals(builtin.IRI, "http://example.org/testBuiltIn"));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(3, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument varArg
                      && string.Equals(varArg.IRI,"urn:swrl:var#VAR")
                      && varArg.GetVariable().Equals(new RDFVariable("?VAR")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLIndividualArgument idvArg
                      && string.Equals(idvArg.IRI, "http://test.org/")
                      && idvArg.GetResource().Equals(new RDFResource("http://test.org/")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLLiteralArgument litArg
                      && litArg.GetLiteral().Equals(new RDFPlainLiteral("lit")));
        Assert.IsTrue(string.Equals(builtin.ToString(), "http://example.org/testBuiltIn(?VAR,http://test.org/,\"lit\")"));
        Assert.IsNull(builtin.EvaluatorFunction);
        Assert.IsTrue(builtin.IsCustom);
    }

    [TestMethod]
    public void ShouldCreateBuiltInWithEvaluator()
    {
        SWRLBuiltIn builtin = new SWRLBuiltIn(
            Evaluator,
            new RDFResource("http://example.org/testBuiltIn"),
            new SWRLVariableArgument(new RDFVariable("?VAR")),
            new SWRLIndividualArgument(new RDFResource("http://test.org/")),
            new SWRLLiteralArgument(new RDFPlainLiteral("lit")));

        Assert.IsNotNull(builtin);
        Assert.IsNotNull(builtin.IRI);
        Assert.IsTrue(string.Equals(builtin.IRI, "http://example.org/testBuiltIn"));
        Assert.IsNotNull(builtin.Arguments);
        Assert.AreEqual(3, builtin.Arguments.Count);
        Assert.IsTrue(builtin.Arguments[0] is SWRLVariableArgument varArg
                      && string.Equals(varArg.IRI,"urn:swrl:var#VAR")
                      && varArg.GetVariable().Equals(new RDFVariable("?VAR")));
        Assert.IsTrue(builtin.Arguments[1] is SWRLIndividualArgument idvArg
                      && string.Equals(idvArg.IRI, "http://test.org/")
                      && idvArg.GetResource().Equals(new RDFResource("http://test.org/")));
        Assert.IsTrue(builtin.Arguments[2] is SWRLLiteralArgument litArg
                      && litArg.GetLiteral().Equals(new RDFPlainLiteral("lit")));
        Assert.IsTrue(string.Equals(builtin.ToString(), "http://example.org/testBuiltIn(?VAR,http://test.org/,\"lit\")"));
        Assert.IsNotNull(builtin.EvaluatorFunction);
        Assert.IsTrue(builtin.IsCustom);
        return;

        bool Evaluator(DataRow datarow) => true;
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingBuiltInWithEvaluatorBecauseNullEvaluator()
        => Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLBuiltIn(
            null,
            new RDFResource("http://example.org/testBuiltIn"),
            new SWRLVariableArgument(new RDFVariable("?VAR")),
            new SWRLIndividualArgument(new RDFResource("http://test.org/")),
            new SWRLLiteralArgument(new RDFPlainLiteral("lit"))));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingBuiltInWithEvaluatorBecauseNullIRI()
    {
        Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLBuiltIn(
            Evaluator,
            null,
            new SWRLVariableArgument(new RDFVariable("?VAR")),
            new SWRLIndividualArgument(new RDFResource("http://test.org/")),
            new SWRLLiteralArgument(new RDFPlainLiteral("lit"))));
        return;

        bool Evaluator(DataRow datarow) => true;
    }

    [TestMethod]
    public void ShouldThrowExceptionOnEvaluatingBuiltInOnAntecedentBecauseUnknownIRI()
    {
        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://example.org/testBuiltIn",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit"))
            ]
        };
        DataTable table = new DataTable();
        table.Columns.Add("?VAR");
        table.Rows.Add("value");
        Assert.ThrowsExactly<SWRLException>(() => _ = builtin.EvaluateOnAntecedent(table));
    }

    [TestMethod]
    public void ShouldEvaluateBuiltInWithEvaluatorOnAntecedent()
    {
        SWRLBuiltIn builtin = new SWRLBuiltIn(
            Evaluator,
            new RDFResource("http://example.org/testBuiltInRegistered"),
            new SWRLVariableArgument(new RDFVariable("?VAR")),
            new SWRLIndividualArgument(new RDFResource("http://test.org/")),
            new SWRLLiteralArgument(new RDFPlainLiteral("lit")));
        DataTable table = new DataTable();
        table.Columns.Add("?VAR");
        table.Rows.Add("value");
        table.Rows.Add("value2");
        table.Rows.Add("value3");
        SWRLBuiltInRegister.AddBuiltIn(builtin);

        Assert.AreEqual(1, builtin.EvaluateOnAntecedent(table).Rows.Count);
        return;

        bool Evaluator(DataRow datarow) => string.Equals(datarow["?VAR"].ToString(), "value");
    }

    [TestMethod]
    public void ShouldExportBuiltinToRDFGraph()
    {
        SWRLBuiltIn builtin = new SWRLBuiltIn
        {
            IRI = "http://example.org/testBuiltIn",
            Arguments = [
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit"))
            ]
        };
        RDFGraph graph = builtin.ToRDFGraph(new RDFCollection(RDFModelEnums.RDFItemTypes.Resource));

        Assert.IsNotNull(graph);
        Assert.AreEqual(13, graph.TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.BODY, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.HEAD, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATARANGE_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATAVALUED_PROPERTY_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DIFFERENT_INDIVIDUALS_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.INDIVIDUAL_PROPERTY_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.SAME_INDIVIDUAL_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.ARGUMENT1, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.ARGUMENT2, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.ARGUMENTS, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.BUILTIN_PROP, new RDFResource("http://example.org/testBuiltIn"), null].TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("urn:swrl:var#VAR"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.ATOMLIST, null].TriplesCount);
    }
    #endregion
}