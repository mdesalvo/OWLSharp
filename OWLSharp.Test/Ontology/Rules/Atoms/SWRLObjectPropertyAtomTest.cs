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

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class SWRLObjectPropertyAtomTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLObjectPropertyAtom()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNotNull(atom.RightArgument);
            Assert.IsTrue(atom.RightArgument is SWRLVariableArgument rightArgVar && rightArgVar.GetVariable().Equals(new RDFVariable("?Q")));
        }

        [TestMethod]
        public void ShouldCreateSWRLObjectPropertyAtomWithIndividualRightArgument()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLIndividualArgument(new RDFResource("ex:Mark")));

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNotNull(atom.RightArgument);
            Assert.IsTrue(atom.RightArgument is SWRLIndividualArgument rightArgIdv && rightArgIdv.GetResource().Equals(new RDFResource("ex:Mark")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLObjectPropertyAtomBecauseNullRightArgument()
            => Assert.ThrowsException<SWRLException>(() => new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                null as SWRLVariableArgument));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLObjectPropertyAtomWithIndividualRightArgumentBecauseNullRightArgument()
            => Assert.ThrowsException<SWRLException>(() => new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                null as SWRLIndividualArgument));

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLObjectPropertyAtom()
        {
             SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));


            Assert.IsTrue(string.Equals("knows(?P,?Q)", atom.ToString()));
        }

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLObjectPropertyAtomWithIndividualRightArgument()
        {
             SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLIndividualArgument(new RDFResource("ex:Mark")));


            Assert.IsTrue(string.Equals("knows(?P,ex:Mark)", atom.ToString()));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLObjectPropertyAtom()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            Assert.IsTrue(string.Equals(
@"<ObjectPropertyAtom><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#Q"" /></ObjectPropertyAtom>", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLObjectPropertyAtomWithIndividualRightArgument()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLIndividualArgument(new RDFResource("ex:Mark")));

            Assert.IsTrue(string.Equals(
@"<ObjectPropertyAtom><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Variable IRI=""urn:swrl:var#P"" /><NamedIndividual IRI=""ex:Mark"" /></ObjectPropertyAtom>", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetSWRLObjectPropertyAtomFromXMLRepresentation()
        {
            SWRLObjectPropertyAtom atom = OWLSerializer.DeserializeObject<SWRLObjectPropertyAtom>(
@"<ObjectPropertyAtom><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#Q"" /></ObjectPropertyAtom>");

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNotNull(atom.RightArgument);
            Assert.IsTrue(atom.RightArgument is SWRLVariableArgument rightArgVar && rightArgVar.GetVariable().Equals(new RDFVariable("?Q")));
        }

        [TestMethod]
        public void ShouldGetSWRLObjectPropertyAtomFromXMLRepresentationWithIndividualRightArgument()
        {
            SWRLObjectPropertyAtom atom = OWLSerializer.DeserializeObject<SWRLObjectPropertyAtom>(
@"<ObjectPropertyAtom><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Variable IRI=""urn:swrl:var#P"" /><NamedIndividual IRI=""ex:Mark"" /></ObjectPropertyAtom>");

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNotNull(atom.RightArgument);
            Assert.IsTrue(atom.RightArgument is SWRLIndividualArgument rightArgIdv && rightArgIdv.GetResource().Equals(new RDFResource("ex:Mark")));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLObjectPropertyAtomOnAntecedent()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John")))
                ]
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.AreEqual(2, antecedentResult.Columns.Count);
            Assert.AreEqual(1, antecedentResult.Rows.Count);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?Q"].ToString(), "ex:John"));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLObjectPropertyAtomWithIndividualRightArgumentOnAntecedent()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLIndividualArgument(new RDFResource("ex:John")));

            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John")))
                ]
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.AreEqual(1, antecedentResult.Columns.Count);
            Assert.AreEqual(1, antecedentResult.Rows.Count);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLObjectPropertyAtomWithObjectAssertionsCalibrationOnAntecedent()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
                ],
                AssertionAxioms = [
                    new OWLObjectPropertyAssertion(
                        new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLNamedIndividual(new RDFResource("ex:John")))
                ]
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.AreEqual(2, antecedentResult.Columns.Count);
            Assert.AreEqual(1, antecedentResult.Rows.Count);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:John"));
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?Q"].ToString(), "ex:Mark"));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLObjectPropertyAtomOnConsequent()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            DataTable antecedentResult = new DataTable();
            antecedentResult.Columns.Add("?P");
            antecedentResult.Columns.Add("?Q");
            antecedentResult.Rows.Add("ex:Mark", "ex:John");

            List<OWLInference> inferences = atom.EvaluateOnConsequent(antecedentResult, null);

            Assert.IsNotNull(inferences);
            Assert.AreEqual(1, inferences.Count);
            Assert.IsTrue(inferences[0].Axiom is OWLObjectPropertyAssertion opAsnInf
                            && opAsnInf.IsInference
                            && opAsnInf.ObjectPropertyExpression.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && opAsnInf.SourceIndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark"))
                            && opAsnInf.TargetIndividualExpression.GetIRI().Equals(new RDFResource("ex:John")));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLObjectPropertyAtomWithIndividualRightArgumentOnConsequent()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLIndividualArgument(new RDFResource("ex:John")));

            DataTable antecedentResult = new DataTable();
            antecedentResult.Columns.Add("?P");
            antecedentResult.Rows.Add("ex:Mark");

            List<OWLInference> inferences = atom.EvaluateOnConsequent(antecedentResult, null);

            Assert.IsNotNull(inferences);
            Assert.AreEqual(1, inferences.Count);
            Assert.IsTrue(inferences[0].Axiom is OWLObjectPropertyAssertion opAsnInf
                            && opAsnInf.IsInference
                            && opAsnInf.ObjectPropertyExpression.GetIRI().Equals(RDFVocabulary.FOAF.KNOWS)
                            && opAsnInf.SourceIndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark"))
                            && opAsnInf.TargetIndividualExpression.GetIRI().Equals(new RDFResource("ex:John")));
        }

        [TestMethod]
        public void ShouldExportSWRLSameIndividualAtomToRDFGraph()
        {
            SWRLObjectPropertyAtom atom = new SWRLObjectPropertyAtom(
                new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLIndividualArgument(new RDFResource("ex:John")));
            RDFGraph graph = atom.ToRDFGraph(new RDFCollection(RDFModelEnums.RDFItemTypes.Resource));

            Assert.IsNotNull(graph);
            Assert.AreEqual(5, graph.TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.BODY, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.HEAD, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATARANGE_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATAVALUED_PROPERTY_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DIFFERENT_INDIVIDUALS_ATOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.INDIVIDUAL_PROPERTY_ATOM, null].TriplesCount);
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
}