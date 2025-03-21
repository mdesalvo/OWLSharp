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
public class OWLDataComplementOfTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateDataComplementOf()
    {
        OWLDataComplementOf dataComplementOf = new OWLDataComplementOf(new OWLDatatype(RDFVocabulary.XSD.STRING));

        Assert.IsNotNull(dataComplementOf);
        Assert.IsNotNull(dataComplementOf.DataRangeExpression);
        Assert.IsTrue(dataComplementOf.DataRangeExpression is OWLDatatype datatype
                      && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingDataAllValuesFromBecauseNullDataRangeExpression()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataComplementOf(null));

    [TestMethod]
    public void ShouldGetStringRepresentationOfDataComplementOf()
    {
        OWLDataComplementOf dataComplementOf = new OWLDataComplementOf(
            new OWLDataUnionOf([
                new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                new OWLDatatype(RDFVocabulary.XSD.DOUBLE)]));
        string swrlString = dataComplementOf.ToSWRLString();

        Assert.IsTrue(string.Equals(swrlString, "(not(integer or double))"));
    }

    [TestMethod]
    public void ShouldSerializeDataComplementOf()
    {
        OWLDataComplementOf dataComplementOf = new OWLDataComplementOf(new OWLDatatype(RDFVocabulary.XSD.STRING));
        string serializedXML = OWLSerializer.SerializeObject(dataComplementOf);

        Assert.IsTrue(string.Equals(serializedXML,
            """<DataComplementOf><Datatype IRI="http://www.w3.org/2001/XMLSchema#string" /></DataComplementOf>"""));
    }

    [TestMethod]
    public void ShouldDeserializeDataComplementOf()
    {
        OWLDataComplementOf dataComplementOf = OWLSerializer.DeserializeObject<OWLDataComplementOf>(
            """
            <DataComplementOf>
              <Datatype IRI="http://www.w3.org/2001/XMLSchema#string" />
            </DataComplementOf>
            """);

        Assert.IsNotNull(dataComplementOf);
        Assert.IsNotNull(dataComplementOf.DataRangeExpression);
        Assert.IsTrue(dataComplementOf.DataRangeExpression is OWLDatatype datatype
                      && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
    }

    [TestMethod]
    public void ShouldConvertDataComplementOfToGraph()
    {
        OWLDataComplementOf dataComplementOf = new OWLDataComplementOf(new OWLDatatype(RDFVocabulary.XSD.STRING));
        RDFGraph graph = dataComplementOf.ToRDFGraph();

        Assert.IsNotNull(graph);
        Assert.AreEqual(3, graph.TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE, null].TriplesCount);
        Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.COMPLEMENT_OF, RDFVocabulary.XSD.STRING, null].TriplesCount);
        Assert.AreEqual(1, graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
    }
    #endregion
}