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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLObjectOneOfTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateObjectOneOf()
    {
        OWLObjectOneOf ObjectOneOf = new OWLObjectOneOf([
            new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLAnonymousIndividual("AnonIdv")]);

        Assert.IsNotNull(ObjectOneOf);
        Assert.IsNotNull(ObjectOneOf.IndividualExpressions);
        Assert.HasCount(2, ObjectOneOf.IndividualExpressions);
        Assert.IsTrue(ObjectOneOf.IndividualExpressions.Any(iex => iex is OWLNamedIndividual namedIdv
                                                                   && string.Equals(namedIdv.IRI, "ex:Bob")));
        Assert.IsTrue(ObjectOneOf.IndividualExpressions.Any(iex => iex is OWLAnonymousIndividual anonIdv
                                                                   && string.Equals(anonIdv.NodeID, "AnonIdv")));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingObjectOneOfBecauseNullIndividualExpressions()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectOneOf(null));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingObjectOneOfBecauseZeroIndividualExpressions()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectOneOf([]));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingObjectOneOfBecauseNullIndividualExpressionFound()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectOneOf([null]));

    [TestMethod]
    public void ShouldGetSWRLRepresentationOfObjectOneOf()
    {
        OWLObjectOneOf objectOneOf = new OWLObjectOneOf([
            new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLAnonymousIndividual("AnonIdv")]);
        string swrlString = objectOneOf.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "({ex:Bob,bnode:AnonIdv})"));
    }

    [TestMethod]
    public void ShouldSerializeObjectOneOf()
    {
        OWLObjectOneOf objectOneOf = new OWLObjectOneOf([
            new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLAnonymousIndividual("AnonIdv")]);
        string serializedXML = OWLSerializer.SerializeObject(objectOneOf);

        Assert.IsTrue(string.Equals(serializedXML,
            """<ObjectOneOf><NamedIndividual IRI="ex:Bob" /><AnonymousIndividual nodeID="AnonIdv" /></ObjectOneOf>"""));
    }

    [TestMethod]
    public void ShouldDeserializeObjectOneOf()
    {
        OWLObjectOneOf objectOneOf = OWLSerializer.DeserializeObject<OWLObjectOneOf>(
            """
            <ObjectOneOf>
              <NamedIndividual IRI="ex:Bob" />
              <AnonymousIndividual nodeID="AnonIdv" />
            </ObjectOneOf>
            """);

        Assert.IsNotNull(objectOneOf);
        Assert.IsNotNull(objectOneOf.IndividualExpressions);
        Assert.HasCount(2, objectOneOf.IndividualExpressions);
        Assert.IsTrue(objectOneOf.IndividualExpressions.Any(iex => iex is OWLNamedIndividual namedIdv
                                                                   && string.Equals(namedIdv.IRI, "ex:Bob")));
        Assert.IsTrue(objectOneOf.IndividualExpressions.Any(iex => iex is OWLAnonymousIndividual anonIdv
                                                                   && string.Equals(anonIdv.NodeID, "AnonIdv")));
    }

    [TestMethod]
    public void ShouldConvertObjectOneOfToGraph()
    {
        OWLObjectOneOf objectOneOf = new OWLObjectOneOf([
            new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLAnonymousIndividual("AnonIdv")]);
        RDFGraph graph = objectOneOf.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(9, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ONE_OF, null, null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:Bob"), null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, new RDFResource("bnode:AnonIdv"), null].TriplesCount);
        Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount);
        Assert.AreEqual(1, graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
    }
    #endregion
}