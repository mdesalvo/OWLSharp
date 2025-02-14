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
public class OWLClassTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateIRIClass()
    {
        OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);

        Assert.IsNotNull(cls);
        Assert.IsTrue(string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        Assert.IsNull(cls.AbbreviatedIRI);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingClassBecauseNullUri()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLClass(null as RDFResource));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingClassBecauseBlankUri()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLClass(new RDFResource()));

    [TestMethod]
    public void ShouldCreateQualifiedNameClass()
    {
        OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));

        Assert.IsNotNull(cls);
        Assert.IsNull(cls.IRI);
        Assert.IsTrue(Equals(cls.AbbreviatedIRI, new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingClassBecauseNullQualifiedName()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLClass(null as XmlQualifiedName));

    [TestMethod]
    public void ShouldGetSWRLRepresentationOfClass()
    {
        OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
        string swrlString = cls.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "Person"));
    }

    [TestMethod]
    public void ShouldSerializeIRIClass()
    {
        OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
        string serializedXML = OWLSerializer.SerializeObject(cls);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Class IRI="http://xmlns.com/foaf/0.1/Person" />"""));
    }

    [TestMethod]
    public void ShouldDeserializeIRIClass()
    {
        OWLClass cls = OWLSerializer.DeserializeObject<OWLClass>(
            """<Class IRI="http://xmlns.com/foaf/0.1/Person" />""");

        Assert.IsNotNull(cls);
        Assert.IsTrue(string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        Assert.IsNull(cls.AbbreviatedIRI);
        //Test stabilization of ExpressionIRI
        Assert.IsTrue(cls.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        cls.GetIRI();
        Assert.IsFalse(cls.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        Assert.IsTrue(cls.ExpressionIRI.ToString().Equals("http://xmlns.com/foaf/0.1/Person"));
    }

    [TestMethod]
    public void ShouldGetSWRLRepresentationOfQualifiedNameClass()
    {
        OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
        string swrlString = cls.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "Person"));
    }

    [TestMethod]
    public void ShouldSerializeQualifiedNameClass()
    {
        OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
        string serializedXML = OWLSerializer.SerializeObject(cls);

        Assert.IsTrue(string.Equals(serializedXML,
            """<Class xmlns:q1="http://xmlns.com/foaf/0.1/" abbreviatedIRI="q1:Person" />"""));
    }

    [TestMethod]
    public void ShouldDeserializeQualifiedNameClass()
    {
        OWLClass cls = OWLSerializer.DeserializeObject<OWLClass>(
            """<Class xmlns:q1="http://xmlns.com/foaf/0.1/" abbreviatedIRI="q1:Person" />""");

        Assert.IsNotNull(cls);
        Assert.IsNull(cls.IRI);
        Assert.IsTrue(Equals(cls.AbbreviatedIRI, new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
        //Test stabilization of ExpressionIRI
        Assert.IsTrue(cls.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        cls.GetIRI();
        Assert.IsFalse(cls.ExpressionIRI.ToString().StartsWith("bnode:ex", StringComparison.Ordinal));
        Assert.IsTrue(cls.ExpressionIRI.ToString().Equals("http://xmlns.com/foaf/0.1/Person"));
    }

    [TestMethod]
    public void ShouldConvertIRIClassToGraph()
    {
        OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
        RDFGraph graph = cls.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(1, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertQualifiedNameClassToGraph()
    {
        OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
        RDFGraph graph = cls.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(1, graph.TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertIRIClassToResource()
    {
        OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
        RDFResource representative = cls.GetIRI();

        Assert.IsNotNull(representative);
        Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.PERSON));
    }

    [TestMethod]
    public void ShouldConvertQualifiedNameClassToResource()
    {
        OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
        RDFResource representative = cls.GetIRI();

        Assert.IsNotNull(representative);
        Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.PERSON));
    }
    #endregion
}