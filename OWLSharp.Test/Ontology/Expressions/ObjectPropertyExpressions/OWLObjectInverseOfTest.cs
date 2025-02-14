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
public class OWLObjectInverseOfTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateObjectInverseOf()
    {
        OWLObjectInverseOf objectInverseOf = new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

        Assert.IsNotNull(objectInverseOf);
        Assert.IsNotNull(objectInverseOf.ObjectProperty);
        Assert.IsTrue(string.Equals(objectInverseOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingObjectInverseOfBecauseNullObjectProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectInverseOf(null));

    [TestMethod]
    public void ShouldGetSWRLRepresentationOfObjectInverseOf()
    {
        OWLObjectInverseOf objectInverseOf = new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
        string swrlString = objectInverseOf.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "inverse(knows)"));
    }

    [TestMethod]
    public void ShouldSerializeObjectInverseOf()
    {
        OWLObjectInverseOf objectInverseOf = new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
        string serializedXML = OWLSerializer.SerializeObject(objectInverseOf);

        Assert.IsTrue(string.Equals(serializedXML,
            """<ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf>"""));
    }

    [TestMethod]
    public void ShouldDeserializeObjectInverseOf()
    {
        OWLObjectInverseOf objectInverseOf = OWLSerializer.DeserializeObject<OWLObjectInverseOf>(
            """
            <ObjectInverseOf>
              <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
            </ObjectInverseOf>
            """);

        Assert.IsNotNull(objectInverseOf);
        Assert.IsNotNull(objectInverseOf.ObjectProperty);
        Assert.IsTrue(string.Equals(objectInverseOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
    }

    [TestMethod]
    public void ShouldConvertObjectInverseOfToGraph()
    {
        OWLObjectInverseOf objectInverseOf = new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
        RDFGraph graph = objectInverseOf.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(2, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertObjectInverseOfToResource()
    {
        OWLObjectInverseOf objectInverseOf = new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
        RDFResource representative = objectInverseOf.GetIRI();

        Assert.IsNotNull(representative);
        Assert.IsTrue(representative.IsBlank);
    }
    #endregion
}