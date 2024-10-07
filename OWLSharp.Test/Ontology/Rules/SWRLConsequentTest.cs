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
    public class SWRLConsequentTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLConsequent()
        {
            SWRLConsequent consequent = new SWRLConsequent();

            Assert.IsNotNull(consequent);
            Assert.IsNotNull(consequent.Atoms);
            Assert.IsTrue(consequent.Atoms.Count == 0);
        }

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLConsequent()
        {
            SWRLConsequent consequent = new SWRLConsequent() { 
                Atoms = [ 
                    new SWRLClassAtom(
                        new OWLClass(RDFVocabulary.FOAF.PERSON), 
                        new SWRLVariableArgument(new RDFVariable("?P"))),
                    new SWRLDataRangeAtom(
                        new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                        new SWRLVariableArgument(new RDFVariable("?X")))
                ] };

            Assert.IsTrue(string.Equals("Person(?P) ^ integer(?X)", consequent.ToString()));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLConsequent()
        {
            SWRLConsequent consequent = new SWRLConsequent() { 
                Atoms = [ 
                    new SWRLClassAtom(
                        new OWLClass(RDFVocabulary.FOAF.PERSON), 
                        new SWRLVariableArgument(new RDFVariable("?P"))),
                    new SWRLDataRangeAtom(
                        new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                        new SWRLVariableArgument(new RDFVariable("?X")))
                ] };

            Assert.IsTrue(string.Equals(
@"<Head><ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom><DataRangeAtom><Datatype IRI=""http://www.w3.org/2001/XMLSchema#integer"" /><Variable IRI=""urn:swrl:var#X"" /></DataRangeAtom></Head>", OWLSerializer.SerializeObject(consequent)));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLConsequent()
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
            List<OWLInference> consequentResult = ontology.Rules[0].Consequent.Evaluate(antecedentResult, ontology);

            Assert.IsNotNull(consequentResult);
            Assert.IsTrue(consequentResult.Count == 1);
            Assert.IsTrue(consequentResult[0].Axiom is OWLClassAssertion clsAsnInf
                            && clsAsnInf.ClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.AGENT)
                            && clsAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark")));
        }


        [TestMethod]
        public void ShouldExportConsequentToRDFGraph()
        {
            SWRLConsequent consequent = new SWRLConsequent()
            {
                Atoms = [
                    new SWRLClassAtom(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new SWRLVariableArgument(new RDFVariable("?P"))),
                    new SWRLDataRangeAtom(
                        new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                        new SWRLVariableArgument(new RDFVariable("?X")))
                ]
            };
            RDFGraph graph = consequent.ToRDFGraph(new RDFResource("bnode:rule"));

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 19);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.BODY, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.HEAD, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATARANGE_ATOM, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.ARGUMENT1, null, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.ARGUMENT2, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.CLASS_PREDICATE, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.DATARANGE, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.ARGUMENTS, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.SWRL.BUILTIN_PROP, null, null].TriplesCount == 0);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.ATOMLIST, null].TriplesCount == 2);
        }
        #endregion
    }
}