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
    public class SWRLAnnotationPropertyAtomTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLAnnotationPropertyAtom()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
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
        public void ShouldCreateSWRLAnnotationPropertyAtomWithLiteralRightArgument()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
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
        public void ShouldThrowExceptionOnCreatingSWRLAnnotationPropertyAtomBecauseNullRightArgument()
            => Assert.ThrowsException<SWRLException>(() => new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                null as SWRLVariableArgument));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLAnnotationPropertyAtomWithLiteralRightArgumentBecauseNullRightArgument()
            => Assert.ThrowsException<SWRLException>(() => new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                null as SWRLLiteralArgument));

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLAnnotationPropertyAtom()
        {
             SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));


            Assert.IsTrue(string.Equals("age(?P,?Q)", atom.ToString()));
        }

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLAnnotationPropertyAtomWithLiteralRightArgument()
        {
             SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));


            Assert.IsTrue(string.Equals("age(?P,\"hello\"@EN-US--RTL)", atom.ToString()));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLAnnotationPropertyAtom()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            Assert.IsTrue(string.Equals(
@"<AnnotationPropertyAtom><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#Q"" /></AnnotationPropertyAtom>", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLAnnotationPropertyAtomWithLiteralRightArgument()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

            Assert.IsTrue(string.Equals(
@"<AnnotationPropertyAtom><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Literal xml:lang=""EN-US--RTL"">hello</Literal></AnnotationPropertyAtom>", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetSWRLAnnotationPropertyAtomFromXMLRepresentation()
        {
            SWRLAnnotationPropertyAtom atom = OWLSerializer.DeserializeObject<SWRLAnnotationPropertyAtom>(
@"<AnnotationPropertyAtom><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#Q"" /></AnnotationPropertyAtom>");

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGE));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNotNull(atom.RightArgument);
            Assert.IsTrue(atom.RightArgument is SWRLVariableArgument rightArgVar && rightArgVar.GetVariable().Equals(new RDFVariable("?Q")));
        }

        [TestMethod]
        public void ShouldGetSWRLAnnotationPropertyAtomFromXMLRepresentationWithLiteralRightArgument()
        {
            SWRLAnnotationPropertyAtom atom = OWLSerializer.DeserializeObject<SWRLAnnotationPropertyAtom>(
@"<AnnotationPropertyAtom><AnnotationProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Literal xml:lang=""EN-US--RTL"">hello</Literal></AnnotationPropertyAtom>");

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGE));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNotNull(atom.RightArgument);
            Assert.IsTrue(atom.RightArgument is SWRLLiteralArgument rightArgVar && rightArgVar.GetLiteral().Equals(new RDFPlainLiteral("hello","en-US--RTL")));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLAnnotationPropertyAtomOnAntecedent()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                        new RDFResource("ex:Mark"),
                        new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)))
                ]
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.AreEqual(2, antecedentResult.Columns.Count);
            Assert.AreEqual(1, antecedentResult.Rows.Count);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?Q"].ToString(), "34^^http://www.w3.org/2001/XMLSchema#positiveInteger"));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLAnnotationPropertyAtomWithLiteralRightArgumentOnAntecedent()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

            OWLOntology ontology = new OWLOntology
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                        new RDFResource("ex:Mark"),
                        new OWLLiteral(new RDFPlainLiteral("hello","en-US--RTL")))
                ]
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.AreEqual(1, antecedentResult.Columns.Count);
            Assert.AreEqual(1, antecedentResult.Rows.Count);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLAnnotationPropertyAtomOnConsequent()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLVariableArgument(new RDFVariable("?Q")));

            DataTable antecedentResult = new DataTable();
            antecedentResult.Columns.Add("?P");
            antecedentResult.Columns.Add("?Q");
            antecedentResult.Rows.Add("ex:Mark", "34^^http://www.w3.org/2001/XMLSchema#positiveInteger");

            List<OWLInference> inferences = atom.EvaluateOnConsequent(antecedentResult, null);

            Assert.IsNotNull(inferences);
            Assert.AreEqual(1, inferences.Count);
            Assert.IsTrue(inferences[0].Axiom is OWLAnnotationAssertion annAsnInf
                            && annAsnInf.IsInference
                            && annAsnInf.AnnotationProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                            && string.Equals(annAsnInf.SubjectIRI, "ex:Mark")
                            && annAsnInf.ValueLiteral.GetLiteral().Equals(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLAnnotationPropertyAtomWithLiteralRightArgumentOnConsequent()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

            DataTable antecedentResult = new DataTable();
            antecedentResult.Columns.Add("?P");
            antecedentResult.Rows.Add("ex:Mark");

            List<OWLInference> inferences = atom.EvaluateOnConsequent(antecedentResult, null);

            Assert.IsNotNull(inferences);
            Assert.AreEqual(1, inferences.Count);
            Assert.IsTrue(inferences[0].Axiom is OWLAnnotationAssertion annAsnInf
                            && annAsnInf.IsInference
                            && annAsnInf.AnnotationProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                            && string.Equals(annAsnInf.SubjectIRI, "ex:Mark")
                            && annAsnInf.ValueLiteral.GetLiteral().Equals(new RDFPlainLiteral("hello", "en-US--RTL")));
        }

        [TestMethod]
        public void ShouldExportSWRLAnnotationPropertyAtomToRDFGraph()
        {
            SWRLAnnotationPropertyAtom atom = new SWRLAnnotationPropertyAtom(
                new OWLAnnotationProperty(RDFVocabulary.FOAF.AGE),
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
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, SWRLAnnotationPropertyAtom.AnnotationPropertyAtomIRI, null].TriplesCount);
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
}