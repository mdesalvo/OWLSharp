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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLObjectSomeValuesFromTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateObjectSomeValuesFrom()
    {
        OWLObjectSomeValuesFrom objectSomeValuesFrom = new OWLObjectSomeValuesFrom(
            new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())),
            new OWLClass(RDFVocabulary.FOAF.PERSON));

        Assert.IsNotNull(objectSomeValuesFrom);
        Assert.IsNotNull(objectSomeValuesFrom.ObjectPropertyExpression);
        Assert.IsTrue(objectSomeValuesFrom.ObjectPropertyExpression is OWLObjectProperty objectProperty
                      && string.Equals(objectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        Assert.IsNotNull(objectSomeValuesFrom.ClassExpression);
        Assert.IsTrue(objectSomeValuesFrom.ClassExpression is OWLClass owlClass
                      && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingObjectSomeValuesFromBecauseNullObjectProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectSomeValuesFrom(
            null, new OWLClass(RDFVocabulary.FOAF.PERSON)));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingObjectSomeValuesFromBecauseNullClassExpression()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectSomeValuesFrom(
            new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())), null));

    [TestMethod]
    public void ShouldGetSWRLRepresentationOfObjectSomeValuesFrom()
    {
        OWLObjectSomeValuesFrom objectSomeValuesFrom = new OWLObjectSomeValuesFrom(
            new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())),
            new OWLClass(RDFVocabulary.FOAF.PERSON));
        string swrlString = objectSomeValuesFrom.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "(knows some Person)"));
    }

    [TestMethod]
    public void ShouldSerializeObjectSomeValuesFrom()
    {
        OWLObjectSomeValuesFrom objectSomeValuesFrom = new OWLObjectSomeValuesFrom(
            new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())),
            new OWLClass(RDFVocabulary.FOAF.PERSON));
        string serializedXML = OWLSerializer.SerializeObject(objectSomeValuesFrom);

        Assert.IsTrue(string.Equals(serializedXML,
            """<ObjectSomeValuesFrom><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /><Class IRI="http://xmlns.com/foaf/0.1/Person" /></ObjectSomeValuesFrom>"""));
    }

    [TestMethod]
    public void ShouldDeserializeObjectSomeValuesFrom()
    {
        OWLObjectSomeValuesFrom objectSomeValuesFrom = OWLSerializer.DeserializeObject<OWLObjectSomeValuesFrom>(
            """
            <ObjectSomeValuesFrom>
              <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
              <Class IRI="http://xmlns.com/foaf/0.1/Person" />
            </ObjectSomeValuesFrom>
            """);

        Assert.IsNotNull(objectSomeValuesFrom);
        Assert.IsNotNull(objectSomeValuesFrom.ObjectPropertyExpression);
        Assert.IsTrue(objectSomeValuesFrom.ObjectPropertyExpression is OWLObjectProperty objectProperty
                      && string.Equals(objectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        Assert.IsNotNull(objectSomeValuesFrom.ClassExpression);
        Assert.IsTrue(objectSomeValuesFrom.ClassExpression is OWLClass owlClass
                      && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
    }

    [TestMethod]
    public void ShouldConvertObjectSomeValuesFromToGraph()
    {
        OWLObjectSomeValuesFrom objectSomeValuesFrom = new OWLObjectSomeValuesFrom(
            new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())),
            new OWLClass(RDFVocabulary.FOAF.PERSON));
        RDFGraph graph = objectSomeValuesFrom.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(5, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOME_VALUES_FROM, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
    }
    #endregion
}