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
    public class SWRLDataRangeAtomTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLDataRangeAtom()
        {
            SWRLDataRangeAtom atom = new SWRLDataRangeAtom(
                new OWLDataOneOf([
                    new OWLLiteral(new RDFPlainLiteral("hello")),
                    new OWLLiteral(new RDFPlainLiteral("hello", "en-US")),
                    new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)) ]),
                new SWRLVariableArgument(new RDFVariable("?P")));

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(string.Equals(atom.Predicate.ToSWRLString(), "({\"hello\",\"hello\"@EN-US,\"hello\"^^xsd:string})"));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNull(atom.RightArgument);
        }

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLDataRangeAtom()
        {
             SWRLDataRangeAtom atom = new SWRLDataRangeAtom(
                new OWLDataOneOf([
                    new OWLLiteral(new RDFPlainLiteral("hello")),
                    new OWLLiteral(new RDFPlainLiteral("hello", "en-US")),
                    new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)) ]),
                new SWRLVariableArgument(new RDFVariable("?P")));


            Assert.IsTrue(string.Equals("({\"hello\",\"hello\"@EN-US,\"hello\"^^xsd:string})(?P)", atom.ToString()));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLDataRangeAtom()
        {
            SWRLDataRangeAtom atom = new SWRLDataRangeAtom(
                new OWLDataOneOf([
                    new OWLLiteral(new RDFPlainLiteral("hello")),
                    new OWLLiteral(new RDFPlainLiteral("hello", "en-US")),
                    new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)) ]),
                new SWRLVariableArgument(new RDFVariable("?P")));

            Assert.IsTrue(string.Equals(
"""<DataRangeAtom><DataOneOf><Literal>hello</Literal><Literal xml:lang="EN-US">hello</Literal><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#string">hello</Literal></DataOneOf><Variable IRI="urn:swrl:var#P" /></DataRangeAtom>""", OWLSerializer.SerializeObject(atom)));
        }

        [TestMethod]
        public void ShouldGetSWRLDataRangeAtomFromXMLRepresentation()
        {
            SWRLDataRangeAtom atom = OWLSerializer.DeserializeObject<SWRLDataRangeAtom>(
"""<DataRangeAtom><DataOneOf><Literal>hello</Literal><Literal xml:lang="EN-US">hello</Literal><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#string">hello</Literal></DataOneOf><Variable IRI="urn:swrl:var#P" /></DataRangeAtom>""");

            Assert.IsNotNull(atom);
            Assert.IsNotNull(atom.Predicate);
            Assert.IsTrue(string.Equals(atom.Predicate.ToSWRLString(), "({\"hello\",\"hello\"@EN-US,\"hello\"^^xsd:string})"));
            Assert.IsNotNull(atom.LeftArgument);
            Assert.IsTrue(atom.LeftArgument is SWRLVariableArgument leftArgVar && leftArgVar.GetVariable().Equals(new RDFVariable("?P")));
            Assert.IsNull(atom.RightArgument);
        }
        
        [TestMethod]
        public void ShouldEvaluateSWRLDataRangeAtomOnAntecedent()
        {
            SWRLDataRangeAtom atom = new SWRLDataRangeAtom(
                new OWLDataOneOf([
                    new OWLLiteral(new RDFPlainLiteral("hello")),
                    new OWLLiteral(new RDFPlainLiteral("hello", "en-US")),
                    new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)) ]),
                new SWRLVariableArgument(new RDFVariable("?P")));

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
                        new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)))
                ]
            };
            DataTable antecedentResult = atom.EvaluateOnAntecedent(ontology);

            Assert.IsNotNull(antecedentResult);
            Assert.AreEqual(1, antecedentResult.Columns.Count);
            Assert.AreEqual(1, antecedentResult.Rows.Count);
            Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "hello^^http://www.w3.org/2001/XMLSchema#string"));
        }

        [TestMethod]
        public void ShouldEvaluateSWRLDataRangeAtomOnConsequent()
        {
            SWRLDataRangeAtom atom = new SWRLDataRangeAtom(
                new OWLDataOneOf([
                    new OWLLiteral(new RDFPlainLiteral("hello")),
                    new OWLLiteral(new RDFPlainLiteral("hello", "en-US")),
                    new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING)) ]),
                new SWRLVariableArgument(new RDFVariable("?P")));
            List<OWLInference> inferences = atom.EvaluateOnConsequent(null, null); //This kind of atom does not evaluate on consequent

            Assert.IsNotNull(inferences);
            Assert.AreEqual(0, inferences.Count);
        }

        [TestMethod]
        public void ShouldExportSWRLDataRangeAtomToRDFGraph()
        {
            SWRLDataRangeAtom atom = new SWRLDataRangeAtom(
                new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                new SWRLVariableArgument(new RDFVariable("?P")));
            RDFGraph graph = atom.ToRDFGraph(new RDFCollection(RDFModelEnums.RDFItemTypes.Resource));

            Assert.IsNotNull(graph);
            Assert.AreEqual(5, graph.TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.BODY, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.HEAD, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATARANGE_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATAVALUED_PROPERTY_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DIFFERENT_INDIVIDUALS_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.INDIVIDUAL_PROPERTY_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.SAME_INDIVIDUAL_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.CLASS_PREDICATE, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.PROPERTY_PREDICATE, null, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.DATARANGE, null, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.ARGUMENT1, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.ARGUMENT2, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.ARGUMENTS, null, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.BUILTIN_PROP, null, null].TriplesCount);
            Assert.AreEqual(1, graph[new RDFResource("urn:swrl:var#P"), RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE, null].TriplesCount);
            Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.ATOMLIST, null].TriplesCount);
        }
        #endregion
    }
}