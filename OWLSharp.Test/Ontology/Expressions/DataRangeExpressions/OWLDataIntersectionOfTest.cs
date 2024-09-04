/*
   Copyright 2014-2024 Marco De Salvo

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
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Expressions
{
    [TestClass]
    public class OWLDataIntersectionOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataIntersectionOf()
        {
            OWLDataIntersectionOf dataIntersectionOf = new OWLDataIntersectionOf(
                [ new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.ANY_URI) ]);

            Assert.IsNotNull(dataIntersectionOf);
            Assert.IsNotNull(dataIntersectionOf.DataRangeExpressions);
            Assert.IsTrue(dataIntersectionOf.DataRangeExpressions.Count == 2);
            Assert.IsTrue(dataIntersectionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.STRING.ToString())));
            Assert.IsTrue(dataIntersectionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.ANY_URI.ToString())));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataIntersectionOfBecauseNullDataRangeExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDataIntersectionOf(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataIntersectionOfBecauseLessThan2DataRangeExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLDataIntersectionOf([ new OWLDatatype(RDFVocabulary.XSD.STRING) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataIntersectionOfBecauseNullDataRangeExpressionFound()
            => Assert.ThrowsException<OWLException>(() => new OWLDataIntersectionOf([ new OWLDatatype(RDFVocabulary.XSD.STRING), null ]));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfDataIntersectionOf()
        {
            OWLDataIntersectionOf dataIntersectionOf = new OWLDataIntersectionOf(
                 [new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.ANY_URI)]);
            string swrlString = dataIntersectionOf.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(string and anyURI)"));
        }

        [TestMethod]
        public void ShouldSerializeDataIntersectionOf()
        {
           OWLDataIntersectionOf dataIntersectionOf = new OWLDataIntersectionOf(
                [ new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.ANY_URI) ]);
            string serializedXML = OWLSerializer.SerializeObject(dataIntersectionOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataIntersectionOf><Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" /><Datatype IRI=""http://www.w3.org/2001/XMLSchema#anyURI"" /></DataIntersectionOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataIntersectionOf()
        {
            OWLDataIntersectionOf dataIntersectionOf = OWLSerializer.DeserializeObject<OWLDataIntersectionOf>(
@"<DataIntersectionOf>
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#anyURI"" />
</DataIntersectionOf>");

            Assert.IsNotNull(dataIntersectionOf);
            Assert.IsNotNull(dataIntersectionOf.DataRangeExpressions);
            Assert.IsTrue(dataIntersectionOf.DataRangeExpressions.Count == 2);
            Assert.IsTrue(dataIntersectionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.STRING.ToString())));
            Assert.IsTrue(dataIntersectionOf.DataRangeExpressions.Any(dre => dre is OWLDatatype dataType 
                            && string.Equals(dataType.IRI, RDFVocabulary.XSD.ANY_URI.ToString())));
        }

        [TestMethod]
        public void ShouldConvertDataIntersectionOfToGraph()
        {
            OWLDataIntersectionOf dataIntersectionOf = new OWLDataIntersectionOf(
                [new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.ANY_URI)]);
            RDFGraph graph = dataIntersectionOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 10);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.INTERSECTION_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.XSD.STRING, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.XSD.ANY_URI, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 2);
        }
        #endregion
    }
}