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
    public class SWRLClassAtomTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLClassAtom()
        {
            SWRLClassAtom atom = new SWRLClassAtom(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new SWRLVariableArgument(new RDFVariable("?P")));

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGENT));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNull(atom.RightArgument);
        }

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLClassAtom()
        {
             SWRLClassAtom atom = new SWRLClassAtom(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new SWRLVariableArgument(new RDFVariable("?P")));


            Assert.IsTrue(string.Equals("Agent(?P)", atom.ToString()));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLClassAtom()
        {
            SWRLClassAtom atom = new SWRLClassAtom(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new SWRLVariableArgument(new RDFVariable("?P")));

            Assert.IsTrue(string.Equals(
@"<ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom>", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetSWRLClassAtomFromXMLRepresentation()
        {
            SWRLClassAtom atom = OWLSerializer.DeserializeObject<SWRLClassAtom>(
@"<ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom>");

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(atom.Predicate.GetIRI().Equals(RDFVocabulary.FOAF.AGENT));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNull(atom.RightArgument);
        }

        [TestMethod]
        public void ShouldEvaluateSWRLClassAtomOnAntecedent()
        {
            SWRLClassAtom atom = new SWRLClassAtom(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new SWRLVariableArgument(new RDFVariable("?P")));

            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                ClassAxioms = [
                    new OWLSubClassOf(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new OWLClass(RDFVocabulary.FOAF.AGENT))
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.IsTrue(antecedentResult.Columns.Count == 1);
            Assert.IsTrue(antecedentResult.Rows.Count == 1);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLClassAtomOnConsequent()
        {
            SWRLClassAtom atom = new SWRLClassAtom(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                new SWRLVariableArgument(new RDFVariable("?P")));

            DataTable antecedentResult = new DataTable();
            antecedentResult.Columns.Add("?P");
            antecedentResult.Rows.Add("ex:Mark");

            List<OWLInference> inferences = atom.EvaluateOnConsequent(antecedentResult, null);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0].Axiom is OWLClassAssertion clsAsnInf
                            && clsAsnInf.IsInference
                            && clsAsnInf.ClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.AGENT)
                            && clsAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark")));
        }
        #endregion
    }
}