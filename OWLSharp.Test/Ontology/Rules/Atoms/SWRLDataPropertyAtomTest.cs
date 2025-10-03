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

using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class SWRLDataPropertyAtomTest
{
    #region Methods
    [TestMethod]
    public void ShouldCreateSWRLDataPropertyAtom()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        Assert.IsNotNull(atom);
        Assert.IsNotNull(atom.Predicate);
        Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGE));
        Assert.IsNotNull(atom.LeftArgument);
        Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
        Assert.IsNotNull(atom.RightArgument);
        Assert.IsTrue(atom.RightArgument is SWRLVariableArgument rightArgVar && rightArgVar.GetVariable().Equals(new RDFVariable("?Q")));
    }

    [TestMethod]
    public void ShouldCreateSWRLDataPropertyAtomWithLiteralRightArgument()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

        Assert.IsNotNull(atom);
        Assert.IsNotNull(atom.Predicate);
        Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGE));
        Assert.IsNotNull(atom.LeftArgument);
        Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
        Assert.IsNotNull(atom.RightArgument);
        Assert.IsTrue(atom.RightArgument is SWRLLiteralArgument rightArgVar && rightArgVar.GetLiteral().Equals(new RDFPlainLiteral("hello","en-US--RTL")));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingSWRLDataPropertyAtomBecauseNullRightArgument()
        => Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            null as SWRLVariableArgument));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingSWRLDataPropertyAtomWithLiteralRightArgumentBecauseNullRightArgument()
        => Assert.ThrowsExactly<SWRLException>(() => _ = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            null as SWRLLiteralArgument));

    [TestMethod]
    public void ShouldGetStringRepresentationOfSWRLDataPropertyAtom()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        Assert.IsTrue(string.Equals("age(?P,?Q)", atom.ToString()));
    }

    [TestMethod]
    public void ShouldGetStringRepresentationOfSWRLDataPropertyAtomWithLiteralRightArgument()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

        Assert.IsTrue(string.Equals("age(?P,\"hello\"@EN-US--RTL)", atom.ToString()));
    }

    [TestMethod]
    public void ShouldGetXMLRepresentationOfSWRLDataPropertyAtom()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        Assert.IsTrue(string.Equals(
            """<DataPropertyAtom><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><Variable IRI="urn:swrl:var#P" /><Variable IRI="urn:swrl:var#Q" /></DataPropertyAtom>""", OWLSerializer.SerializeObject(atom)));
    }

    [TestMethod]
    public void ShouldGetXMLRepresentationOfSWRLDataPropertyAtomWithLiteralRightArgument()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

        Assert.IsTrue(string.Equals(
            """<DataPropertyAtom><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><Variable IRI="urn:swrl:var#P" /><Literal xml:lang="EN-US--RTL">hello</Literal></DataPropertyAtom>""", OWLSerializer.SerializeObject(atom)));
    }

    [TestMethod]
    public void ShouldGetSWRLDataPropertyAtomFromXMLRepresentation()
    {
        SWRLDataPropertyAtom atom = OWLSerializer.DeserializeObject<SWRLDataPropertyAtom>(
            """<DataPropertyAtom><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><Variable IRI="urn:swrl:var#P" /><Variable IRI="urn:swrl:var#Q" /></DataPropertyAtom>""");

        Assert.IsNotNull(atom);
        Assert.IsNotNull(atom.Predicate);
        Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGE));
        Assert.IsNotNull(atom.LeftArgument);
        Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
        Assert.IsNotNull(atom.RightArgument);
        Assert.IsTrue(atom.RightArgument is SWRLVariableArgument rightArgVar && rightArgVar.GetVariable().Equals(new RDFVariable("?Q")));
    }

    [TestMethod]
    public void ShouldGetSWRLDataPropertyAtomFromXMLRepresentationWithLiteralRightArgument()
    {
        SWRLDataPropertyAtom atom = OWLSerializer.DeserializeObject<SWRLDataPropertyAtom>(
            """<DataPropertyAtom><DataProperty IRI="http://xmlns.com/foaf/0.1/age" /><Variable IRI="urn:swrl:var#P" /><Literal xml:lang="EN-US--RTL">hello</Literal></DataPropertyAtom>""");

        Assert.IsNotNull(atom);
        Assert.IsNotNull(atom.Predicate);
        Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGE));
        Assert.IsNotNull(atom.LeftArgument);
        Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
        Assert.IsNotNull(atom.RightArgument);
        Assert.IsTrue(atom.RightArgument is SWRLLiteralArgument rightArgVar && rightArgVar.GetLiteral().Equals(new RDFPlainLiteral("hello","en-US--RTL")));
    }

    [TestMethod]
    public void ShouldEvaluateSWRLDataPropertyAtomOnAntecedent()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
            ],
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)))
            ]
        };
        DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

        Assert.IsNotNull(antecedentResult);
        Assert.HasCount(2, antecedentResult.Columns);
        Assert.HasCount(1, antecedentResult.Rows);
        Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
        Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?Q"].ToString(), "34^^http://www.w3.org/2001/XMLSchema#positiveInteger"));
    }

    [TestMethod]
    public void ShouldEvaluateSWRLDataPropertyAtomWithLiteralRightArgumentOnAntecedent()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
            ],
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFPlainLiteral("hello","en-US--RTL")))
            ]
        };
        DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

        Assert.IsNotNull(antecedentResult);
        Assert.HasCount(1, antecedentResult.Columns);
        Assert.HasCount(1, antecedentResult.Rows);
        Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
    }

    [TestMethod]
    public void ShouldEvaluateSWRLDataPropertyAtomOnConsequent()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLVariableArgument(new RDFVariable("?Q")));

        DataTable antecedentResult = new DataTable();
        antecedentResult.Columns.Add("?P");
        antecedentResult.Columns.Add("?Q");
        antecedentResult.Rows.Add("ex:Mark", "34^^http://www.w3.org/2001/XMLSchema#positiveInteger");

        List<OWLInference> inferences = atom.EvaluateOnConsequent(antecedentResult, null);

        Assert.IsNotNull(inferences);
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences[0].Axiom is OWLDataPropertyAssertion { IsInference: true } dpAsnInf
                      && dpAsnInf.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                      && dpAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark"))
                      && dpAsnInf.Literal.GetLiteral().Equals(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)));
    }

    [TestMethod]
    public void ShouldEvaluateSWRLDataPropertyAtomWithLiteralRightArgumentOnConsequent()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

        DataTable antecedentResult = new DataTable();
        antecedentResult.Columns.Add("?P");
        antecedentResult.Rows.Add("ex:Mark");

        List<OWLInference> inferences = atom.EvaluateOnConsequent(antecedentResult, null);

        Assert.IsNotNull(inferences);
        Assert.HasCount(1, inferences);
        Assert.IsTrue(inferences[0].Axiom is OWLDataPropertyAssertion { IsInference: true } dpAsnInf
                      && dpAsnInf.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                      && dpAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark"))
                      && dpAsnInf.Literal.GetLiteral().Equals(new RDFPlainLiteral("hello", "en-US--RTL")));
    }

    [TestMethod]
    public void ShouldExportSWRLDataPropertyAtomToRDFGraph()
    {
        SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
            new OWLDataProperty(RDFVocabulary.FOAF.AGE),
            new SWRLVariableArgument(new RDFVariable("?P")),
            new SWRLLiteralArgument(new RDFPlainLiteral("hello", "en-US")));
        RDFGraph graph = atom.ToRDFGraph(new RDFCollection(RDFModelEnums.RDFItemTypes.Resource));

        Assert.IsNotNull(graph);
        Assert.AreEqual(5, graph.TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.BODY, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.HEAD, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATARANGE_ATOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATAVALUED_PROPERTY_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DIFFERENT_INDIVIDUALS_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.INDIVIDUAL_PROPERTY_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.SAME_INDIVIDUAL_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.CLASS_PREDICATE, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.PROPERTY_PREDICATE, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.DATARANGE, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.ARGUMENT1, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.ARGUMENT2, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.ARGUMENTS, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.BUILTIN_PROP, null, null].TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("urn:swrl:var#P"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.ATOMLIST, null].TriplesCount);
    }
    #endregion
}