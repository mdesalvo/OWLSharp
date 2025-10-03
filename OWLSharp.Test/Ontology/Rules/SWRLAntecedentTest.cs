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

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology;

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
        Assert.IsEmpty(antecedent.Atoms);
    }

    [TestMethod]
    public void ShouldGetStringRepresentationOfSWRLAntecedent()
    {
        SWRLAntecedent antecedent = new SWRLAntecedent {
            Atoms = [
                new SWRLClassAtom(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new SWRLVariableArgument(new RDFVariable("?P"))),
                new SWRLDataRangeAtom(
                    new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                    new SWRLVariableArgument(new RDFVariable("?X")))
            ],
            BuiltIns = [
                SWRLBuiltIn.Tan(
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLVariableArgument(new RDFVariable("?Y")))
            ]
        };

        Assert.IsTrue(string.Equals("Person(?P) ^ integer(?X) ^ swrlb:tan(?X,?Y)", antecedent.ToString()));
    }

    [TestMethod]
    public void ShouldGetXMLRepresentationOfSWRLAntecedent()
    {
        SWRLAntecedent antecedent = new SWRLAntecedent {
            Atoms = [
                new SWRLClassAtom(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new SWRLVariableArgument(new RDFVariable("?P"))),
                new SWRLDataRangeAtom(
                    new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                    new SWRLVariableArgument(new RDFVariable("?X")))
            ],
            BuiltIns = [
                SWRLBuiltIn.Divide(
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLVariableArgument(new RDFVariable("?Y")),
                    new SWRLLiteralArgument(new RDFTypedLiteral("3.141592", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))),
                SWRLBuiltIn.Tan(
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLVariableArgument(new RDFVariable("?Y")))
            ]
        };

        Assert.IsTrue(string.Equals(
            """<Body><ClassAtom><Class IRI="http://xmlns.com/foaf/0.1/Person" /><Variable IRI="urn:swrl:var#P" /></ClassAtom><DataRangeAtom><Datatype IRI="http://www.w3.org/2001/XMLSchema#integer" /><Variable IRI="urn:swrl:var#X" /></DataRangeAtom><BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#divide"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#double">3.141592</Literal></BuiltInAtom><BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#tan"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom></Body>""", OWLSerializer.SerializeObject(antecedent)));
    }

    [TestMethod]
    public void ShouldGetSWRLAntecedentFromXMLRepresentation()
    {
        SWRLAntecedent antecedent = OWLSerializer.DeserializeObject<SWRLAntecedent>(
            """<Body><ClassAtom><Class IRI="http://xmlns.com/foaf/0.1/Person" /><Variable IRI="urn:swrl:var#P" /></ClassAtom><DataRangeAtom><Datatype IRI="http://www.w3.org/2001/XMLSchema#integer" /><Variable IRI="urn:swrl:var#X" /></DataRangeAtom><BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#divide"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /><Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#double">3.141592</Literal></BuiltInAtom><BuiltInAtom IRI="http://www.w3.org/2003/11/swrlb#tan"><Variable IRI="urn:swrl:var#X" /><Variable IRI="urn:swrl:var#Y" /></BuiltInAtom></Body>""");

        Assert.IsNotNull(antecedent);
        Assert.HasCount(2, antecedent.Atoms);
        Assert.HasCount(2, antecedent.BuiltIns);
        Assert.IsTrue(string.Equals("Person(?P) ^ integer(?X) ^ swrlb:divide(?X,?Y,\"3.141592\"^^xsd:double) ^ swrlb:tan(?X,?Y)", antecedent.ToString()));
    }

    [TestMethod]
    public void ShouldEvaluateSWRLAntecedent()
    {
        OWLOntology ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
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
                    new SWRLAntecedent
                    {
                        Atoms = [
                            new SWRLClassAtom(
                                new OWLClass(RDFVocabulary.FOAF.PERSON),
                                new SWRLVariableArgument(new RDFVariable("?P")))
                        ]
                    },
                    new SWRLConsequent
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
        Assert.HasCount(1, antecedentResult.Columns);
        Assert.HasCount(1, antecedentResult.Rows);
        Assert.IsTrue(string.Equals(antecedentResult.Rows[0]["?P"].ToString(), "ex:Mark"));
    }

    [TestMethod]
    public void ShouldExportAntecedentToRDFGraph()
    {
        SWRLAntecedent antecedent = new SWRLAntecedent
        {
            Atoms = [
                new SWRLClassAtom(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new SWRLVariableArgument(new RDFVariable("?P"))),
                new SWRLDataRangeAtom(
                    new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                    new SWRLVariableArgument(new RDFVariable("?X")))
            ],
            BuiltIns = [
                SWRLBuiltIn.Divide(
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLVariableArgument(new RDFVariable("?Y")),
                    new SWRLLiteralArgument(new RDFTypedLiteral("3.141592", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))),
                SWRLBuiltIn.Tan(
                    new SWRLVariableArgument(new RDFVariable("?X")),
                    new SWRLVariableArgument(new RDFVariable("?Y")))
            ]
        };
        RDFGraph graph = antecedent.ToRDFGraph(new RDFResource("bnode:rule"));

        Assert.IsNotNull(graph);
        Assert.AreEqual(49, graph.TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.BODY, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.HEAD, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DATARANGE_ATOM, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.SWRL.ARGUMENT1, null, null].TriplesCount);
        Assert.AreEqual(0, graph[null, RDFVocabulary.SWRL.ARGUMENT2, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.CLASS_PREDICATE, null, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.SWRL.DATARANGE, null, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.BUILTIN_ATOM, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.SWRL.ARGUMENTS, null, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.SWRL.BUILTIN_PROP, null, null].TriplesCount);
        Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE, null].TriplesCount);
        Assert.AreEqual(4, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.ATOMLIST, null].TriplesCount);
    }
    #endregion
}