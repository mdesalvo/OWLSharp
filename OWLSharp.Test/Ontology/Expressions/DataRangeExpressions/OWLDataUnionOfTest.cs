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

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLDataUnionOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataUnionOf()
        {
            OWLDataUnionOf dataUnionOf = new OWLDataUnionOf(
                [ new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.ANY_URI) ]);

            Assert.IsNotNull(dataUnionOf);
            Assert.IsNotNull(dataUnionOf.DataRangeExpressions);
            Assert.AreEqual(2, dataUnionOf.DataRangeExpressions.Count);
            Assert.IsTrue(dataUnionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.STRING.ToString())));
            Assert.IsTrue(dataUnionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.ANY_URI.ToString())));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataUnionOfBecauseNullDataRangeExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDataUnionOf(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataUnionOfBecauseLessThan2DataRangeExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDataUnionOf([ new OWLDatatype(RDFVocabulary.XSD.STRING) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataUnionOfBecauseNullDataRangeExpressionFound()
            => Assert.ThrowsException<OWLException>(() => new OWLDataUnionOf([ new OWLDatatype(RDFVocabulary.XSD.STRING), null ]));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfDataUnionOf()
        {
            OWLDataUnionOf dataUnionOf = new OWLDataUnionOf(
                 [new OWLDatatype(RDFVocabulary.XSD.STRING), 
                  new OWLDatatype(RDFVocabulary.XSD.ANY_URI),
                  new OWLDataOneOf([
                      new OWLLiteral(new RDFPlainLiteral("A")),
                      new OWLLiteral(new RDFPlainLiteral("B"))])]);
            string swrlString = dataUnionOf.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(string or anyURI or ({\"A\",\"B\"}))"));
        }

        [TestMethod]
        public void ShouldSerializeDataUnionOf()
        {
           OWLDataUnionOf dataUnionOf = new OWLDataUnionOf(
                [ new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.ANY_URI) ]);
            string serializedXML = OWLSerializer.SerializeObject(dataUnionOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataUnionOf><Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" /><Datatype IRI=""http://www.w3.org/2001/XMLSchema#anyURI"" /></DataUnionOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataUnionOf()
        {
            OWLDataUnionOf dataUnionOf = OWLSerializer.DeserializeObject<OWLDataUnionOf>(
@"<DataUnionOf>
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#anyURI"" />
</DataUnionOf>");

            Assert.IsNotNull(dataUnionOf);
            Assert.IsNotNull(dataUnionOf.DataRangeExpressions);
            Assert.AreEqual(2, dataUnionOf.DataRangeExpressions.Count);
            Assert.IsTrue(dataUnionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.STRING.ToString())));
            Assert.IsTrue(dataUnionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.ANY_URI.ToString())));
        }

        [TestMethod]
        public void ShouldConvertDataUnionOfToGraph()
        {
            OWLDataUnionOf dataUnionOf = new OWLDataUnionOf(
                [new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.ANY_URI)]);
            RDFGraph graph = dataUnionOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(10, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.UNION_OF, null, null].TriplesCount);
            Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.XSD.STRING, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.XSD.ANY_URI, null].TriplesCount);
            Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount);
            Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount);
        }
        #endregion
    }
}