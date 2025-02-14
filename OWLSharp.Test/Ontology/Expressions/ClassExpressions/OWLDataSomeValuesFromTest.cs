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
    public class OWLDataSomeValuesFromTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataSomeValuesFrom()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = new OWLDataSomeValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));

            Assert.IsNotNull(dataSomeValuesFrom);
            Assert.IsNotNull(dataSomeValuesFrom.DataProperty);
            Assert.IsTrue(string.Equals(dataSomeValuesFrom.DataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataSomeValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataSomeValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataSomeValuesFromBecauseNullDataProperty()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataSomeValuesFrom(
                null, new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataSomeValuesFromBecauseNullDataRange()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLDataSomeValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())), null));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfDataSomeValuesFrom()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = new OWLDataSomeValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            string swrlString = dataSomeValuesFrom.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(creator some string)"));
        }

        [TestMethod]
        public void ShouldSerializeDataSomeValuesFrom()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = new OWLDataSomeValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLSerializer.SerializeObject(dataSomeValuesFrom);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataSomeValuesFrom><DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" /><Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" /></DataSomeValuesFrom>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataSomeValuesFrom()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = OWLSerializer.DeserializeObject<OWLDataSomeValuesFrom>(
@"<DataSomeValuesFrom>
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataSomeValuesFrom>");

            Assert.IsNotNull(dataSomeValuesFrom);
            Assert.IsNotNull(dataSomeValuesFrom.DataProperty);
            Assert.IsTrue(string.Equals(dataSomeValuesFrom.DataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataSomeValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataSomeValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldConvertDataSomeValuesFromToGraph()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = new OWLDataSomeValuesFrom(
                new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())),
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            RDFGraph graph = dataSomeValuesFrom.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(5, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.DC.CREATOR, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.SOME_VALUES_FROM, RDFVocabulary.XSD.STRING, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.CREATOR, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
        }
        #endregion
    }
}