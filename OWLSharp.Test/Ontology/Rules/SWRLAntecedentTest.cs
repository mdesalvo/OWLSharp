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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Rules;
using OWLSharp.Ontology.Rules.Arguments;
using OWLSharp.Ontology.Rules.Atoms;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLAntecedentTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLAntecedent()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent();

            Assert.IsNotNull(antecedent);
            Assert.IsNotNull(antecedent.Atoms);
            Assert.IsTrue(antecedent.Atoms.Count == 0);
        }

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLAntecedent()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent() { 
                Atoms = [ 
                    new SWRLClassAtom(
                        new OWLClass(RDFVocabulary.FOAF.PERSON), 
                        new SWRLVariableArgument(new RDFVariable("?P"))),
                    new SWRLDataRangeAtom(
                        new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                        new SWRLVariableArgument(new RDFVariable("?X")))
                ] };

            Assert.IsTrue(string.Equals("Person(?P) ^ integer(?X)", antecedent.ToString()));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLAntecedent()
        {
            SWRLAntecedent antecedent = new SWRLAntecedent() { 
                Atoms = [ 
                    new SWRLClassAtom(
                        new OWLClass(RDFVocabulary.FOAF.PERSON), 
                        new SWRLVariableArgument(new RDFVariable("?P"))),
                    new SWRLDataRangeAtom(
                        new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                        new SWRLVariableArgument(new RDFVariable("?X")))
                ] };

            Assert.IsTrue(string.Equals(
@"<Body><ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom><DataRangeAtom><Datatype IRI=""http://www.w3.org/2001/XMLSchema#integer"" /><Variable IRI=""urn:swrl:var#X"" /></DataRangeAtom></Body>", OWLSerializer.SerializeObject(antecedent)));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLAntecedent()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ],
                Rules = [
                    new SWRLRule(
                        new RDFPlainLiteral("SWRL1"),
                        new RDFPlainLiteral("This is a test SWRL rule"),
                        new SWRLAntecedent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                                    new SWRLVariableArgument(new RDFVariable("?P")))
                            ]
                        },
                        new SWRLConsequent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                                    new SWRLVariableArgument(new RDFVariable("?P")))
                            ]
                        })
                ]
            };
            DataTable antecedentResult = ontology.Rules[0].Antecedent.Evaluate(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.IsTrue(antecedentResult.Columns.Count == 1);
            Assert.IsTrue(antecedentResult.Rows.Count == 1);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
        }
        #endregion
    }
}