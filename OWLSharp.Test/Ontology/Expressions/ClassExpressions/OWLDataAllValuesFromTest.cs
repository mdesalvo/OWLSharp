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

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLDataAllValuesFromTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataAllValuesFrom()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = new OWLDataAllValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));

            Assert.IsNotNull(dataAllValuesFrom);
            Assert.IsNotNull(dataAllValuesFrom.DataProperty);
            Assert.IsTrue(string.Equals(dataAllValuesFrom.DataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataAllValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataAllValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataAllValuesFromBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataAllValuesFrom(
                null, new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataAllValuesFromBecauseNullDataRange()
            => Assert.ThrowsException<OWLException>(() => new OWLDataAllValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())), null));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfDataAllValuesFrom()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = new OWLDataAllValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            string swrlString = dataAllValuesFrom.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(creator only string)"));
        }

        [TestMethod]
        public void ShouldSerializeDataAllValuesFrom()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = new OWLDataAllValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLSerializer.SerializeObject(dataAllValuesFrom);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataAllValuesFrom><DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" /><Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" /></DataAllValuesFrom>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataAllValuesFrom()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = OWLSerializer.DeserializeObject<OWLDataAllValuesFrom>(
@"<DataAllValuesFrom>
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataAllValuesFrom>");

            Assert.IsNotNull(dataAllValuesFrom);
            Assert.IsNotNull(dataAllValuesFrom.DataProperty);
            Assert.IsTrue(string.Equals(dataAllValuesFrom.DataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataAllValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataAllValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldConvertDataAllValuesFromToGraph()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = new OWLDataAllValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            RDFGraph graph = dataAllValuesFrom.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(5, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.DC.CREATOR, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ALL_VALUES_FROM, RDFVocabulary.XSD.STRING, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.CREATOR, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
        }
        #endregion
    }
}