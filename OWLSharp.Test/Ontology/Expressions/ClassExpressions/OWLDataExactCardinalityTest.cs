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
public class OWLDataExactCardinalityTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateDataExactCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);

        Assert.IsNotNull(dataExactCardinality);
        Assert.IsTrue(string.Equals(dataExactCardinality.Cardinality, "1"));
        Assert.IsNotNull(dataExactCardinality.DataProperty);
        Assert.IsTrue(string.Equals(dataExactCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
        Assert.IsNull(dataExactCardinality.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDataExactCardinalityBecauseNullDataProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataExactCardinality(null, 1));

    [TestMethod]
    public void ShouldCreateDataExactQualifiedCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));

        Assert.IsNotNull(dataExactCardinality);
        Assert.IsTrue(string.Equals(dataExactCardinality.Cardinality, "1"));
        Assert.IsNotNull(dataExactCardinality.DataProperty);
        Assert.IsTrue(string.Equals(dataExactCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
        Assert.IsNotNull(dataExactCardinality.DataRangeExpression);
        Assert.IsTrue(dataExactCardinality.DataRangeExpression is OWLDatatype datatype 
                      && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDataExactQualifiedCardinalityBecauseNullDataProperty()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataExactCardinality(null, 1));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDataExactQualifiedCardinalityBecauseNullDataRange()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, null));

    [TestMethod]
    public void ShouldGetSWRLRepresentationOfDataExactCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);
        string swrlString = dataExactCardinality.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "(description exactly 1)"));
    }

    [TestMethod]
    public void ShouldSerializeDataExactCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);
        string serializedXML = OWLSerializer.SerializeObject(dataExactCardinality);

        Assert.IsTrue(string.Equals(serializedXML,
            """<DataExactCardinality cardinality="1"><DataProperty IRI="http://purl.org/dc/elements/1.1/description" /></DataExactCardinality>"""));
    }

    [TestMethod]
    public void ShouldDeserializeDataExactCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = OWLSerializer.DeserializeObject<OWLDataExactCardinality>(
            """
            <DataExactCardinality cardinality="1">
              <DataProperty IRI="http://purl.org/dc/elements/1.1/description" />
            </DataExactCardinality>
            """);

        Assert.IsNotNull(dataExactCardinality);
        Assert.IsTrue(string.Equals(dataExactCardinality.Cardinality, "1"));
        Assert.IsNotNull(dataExactCardinality.DataProperty);
        Assert.IsTrue(string.Equals(dataExactCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
        Assert.IsNull(dataExactCardinality.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldGetSWRLRepresentationOfDataExactQualifiedCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));
        string swrlString = dataExactCardinality.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "(description exactly 1 string)"));
    }

    [TestMethod]
    public void ShouldSerializeDataExactQualifiedCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));
        string serializedXML = OWLSerializer.SerializeObject(dataExactCardinality);

        Assert.IsTrue(string.Equals(serializedXML,
            """<DataExactCardinality cardinality="1"><DataProperty IRI="http://purl.org/dc/elements/1.1/description" /><Datatype IRI="http://www.w3.org/2001/XMLSchema#string" /></DataExactCardinality>"""));
    }

    [TestMethod]
    public void ShouldDeserializeDataExactQualifiedCardinality()
    {
        OWLDataExactCardinality dataExactCardinality = OWLSerializer.DeserializeObject<OWLDataExactCardinality>(
            """
            <DataExactCardinality cardinality="1">
              <DataProperty IRI="http://purl.org/dc/elements/1.1/description" />
              <Datatype IRI="http://www.w3.org/2001/XMLSchema#string" />
            </DataExactCardinality>
            """);

        Assert.IsNotNull(dataExactCardinality);
        Assert.IsTrue(string.Equals(dataExactCardinality.Cardinality, "1"));
        Assert.IsNotNull(dataExactCardinality.DataProperty);
        Assert.IsTrue(string.Equals(dataExactCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
        Assert.IsNotNull(dataExactCardinality.DataRangeExpression);
        Assert.IsTrue(dataExactCardinality.DataRangeExpression is OWLDatatype datatype 
                      && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
    }

    [TestMethod]
    public void ShouldConvertDataExactCardinalityToGraph()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);
        RDFGraph graph = dataExactCardinality.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(4, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.DC.DESCRIPTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.DC.DESCRIPTION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
    }

    [TestMethod]
    public void ShouldConvertDataExactQualifiedCardinalityToGraph()
    {
        OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));
        RDFGraph graph = dataExactCardinality.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(6, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.DC.DESCRIPTION, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_DATARANGE, RDFVocabulary.XSD.STRING, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.QUALIFIED_CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.DC.DESCRIPTION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
    }
    #endregion
}