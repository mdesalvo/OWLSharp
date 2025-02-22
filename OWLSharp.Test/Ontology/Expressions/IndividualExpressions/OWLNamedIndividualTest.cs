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

using System;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLNamedIndividualTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateIRINamedIndividual()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);

        Assert.IsNotNull(idv);
        Assert.IsTrue(string.Equals(idv.IRI, RDFVocabulary.FOAF.AGE.ToString()));
        Assert.IsNull(idv.AbbreviatedIRI);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualBecauseNullUri()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNamedIndividual(null as RDFResource));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualBecauseBlankUri()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNamedIndividual(new RDFResource()));

    [TestMethod]
    public void ShouldCreateQualifiedNameNamedIndividual()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));

        Assert.IsNotNull(idv);
        Assert.IsNull(idv.IRI);
        Assert.IsTrue(Equals(idv.AbbreviatedIRI, new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI)));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNamedIndividualBecauseNullQualifiedName()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLNamedIndividual(null as XmlQualifiedName));

    [TestMethod]
    public void ShouldSerializeIRINamedIndividual()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);
        string serializedXML = OWLSerializer.SerializeObject(idv);

        Assert.IsTrue(string.Equals(serializedXML,
            """<NamedIndividual IRI="http://xmlns.com/foaf/0.1/age" />"""));
    }

    [TestMethod]
    public void ShouldDeserializeIRINamedIndividual()
    {
        OWLNamedIndividual idv = OWLSerializer.DeserializeObject<OWLNamedIndividual>(
            """<NamedIndividual IRI="ex:Mark" />""");

        Assert.IsNotNull(idv);
        Assert.IsTrue(string.Equals(idv.IRI, "ex:Mark"));
        Assert.IsNull(idv.AbbreviatedIRI);
        //Test stabilization of ExpressionIRI
        Assert.IsTrue(idv.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        idv.GetIRI();
        Assert.IsFalse(idv.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        Assert.IsTrue(idv.ExpressionIRI.ToString().Equals("ex:Mark"));
    }

    [TestMethod]
    public void ShouldSerializeQualifiedNameNamedIndividual()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));
        string serializedXML = OWLSerializer.SerializeObject(idv);

        Assert.IsTrue(string.Equals(serializedXML,
            """<NamedIndividual xmlns:q1="http://xmlns.com/foaf/0.1/" abbreviatedIRI="q1:age" />"""));
    }

    [TestMethod]
    public void ShouldDeserializeQualifiedNameNamedIndividual()
    {
        OWLNamedIndividual idv = OWLSerializer.DeserializeObject<OWLNamedIndividual>(
            """<NamedIndividual xmlns:q1="http://xmlns.com/foaf/0.1/" abbreviatedIRI="q1:age" />""");

        Assert.IsNotNull(idv);
        Assert.IsNull(idv.IRI);
        Assert.IsTrue(Equals(idv.AbbreviatedIRI, new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI)));
        //Test stabilization of ExpressionIRI
        Assert.IsTrue(idv.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        idv.GetIRI();
        Assert.IsFalse(idv.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        Assert.IsTrue(idv.ExpressionIRI.ToString().Equals("http://xmlns.com/foaf/0.1/age"));
    }

    [TestMethod]
    public void ShouldConvertIRINamedIndividualToGraph()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);
        RDFGraph graph = idv.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(1, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertQualifiedNameNamedIndividualToGraph()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));
        RDFGraph graph = idv.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(1, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertIRINamedIndividualToResource()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);
        RDFResource representative = idv.GetIRI();

        Assert.IsNotNull(representative);
        Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.AGE));
    }

    [TestMethod]
    public void ShouldConvertQualifiedNameNamedIndividualToResource()
    {
        OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));
        RDFResource representative = idv.GetIRI();

        Assert.IsNotNull(representative);
        Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.AGE));
    }
    #endregion
}