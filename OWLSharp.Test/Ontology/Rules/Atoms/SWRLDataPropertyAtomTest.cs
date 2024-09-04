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

using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Rules;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology.Rules
{
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
            => Assert.ThrowsException<OWLException>(() => new SWRLDataPropertyAtom(
                new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                null as SWRLVariableArgument));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLDataPropertyAtomWithLiteralRightArgumentBecauseNullRightArgument()
            => Assert.ThrowsException<OWLException>(() => new SWRLDataPropertyAtom(
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
@"<DataPropertyAtom><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#Q"" /></DataPropertyAtom>", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLDataPropertyAtomWithLiteralRightArgument()
        {
            SWRLDataPropertyAtom atom = new SWRLDataPropertyAtom(
                new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                new SWRLVariableArgument(new RDFVariable("?P")),
                new SWRLLiteralArgument(new RDFPlainLiteral("hello","en-US--RTL")));

            Assert.IsTrue(string.Equals(
@"<DataPropertyAtom><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Literal xml:lang=""EN-US--RTL"">hello</Literal></DataPropertyAtom>", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetSWRLDataPropertyAtomFromXMLRepresentation()
        {
            SWRLDataPropertyAtom atom = OWLSerializer.DeserializeObject<SWRLDataPropertyAtom>(
@"<DataPropertyAtom><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#Q"" /></DataPropertyAtom>");

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
@"<DataPropertyAtom><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /><Variable IRI=""urn:swrl:var#P"" /><Literal xml:lang=""EN-US--RTL"">hello</Literal></DataPropertyAtom>");

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

            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)))
                ],
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.IsTrue(antecedentResult.Columns.Count == 2);
            Assert.IsTrue(antecedentResult.Rows.Count == 1);
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

            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                AssertionAxioms = [
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.AGE),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("hello","en-US--RTL")))
                ],
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.IsTrue(antecedentResult.Columns.Count == 1);
            Assert.IsTrue(antecedentResult.Rows.Count == 1);
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
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0].Axiom is OWLDataPropertyAssertion dpAsnInf
                            && dpAsnInf.IsInference
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
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0].Axiom is OWLDataPropertyAssertion dpAsnInf
                            && dpAsnInf.IsInference
                            && dpAsnInf.DataProperty.GetIRI().Equals(RDFVocabulary.FOAF.AGE)
                            && dpAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark"))
                            && dpAsnInf.Literal.GetLiteral().Equals(new RDFPlainLiteral("hello", "en-US--RTL")));
        }
        #endregion
    }
}