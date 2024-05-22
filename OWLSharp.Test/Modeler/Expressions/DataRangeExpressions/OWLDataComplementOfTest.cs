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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Modeler.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Modeler.Expressions
{
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
            => Assert.ThrowsException<OWLException>(() => new OWLDataComplementOf(null));

        [TestMethod]
        public void ShouldSerializeDataComplementOf()
        {
            OWLDataComplementOf dataComplementOf = new OWLDataComplementOf(new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLTestSerializer<OWLDataComplementOf>.Serialize(dataComplementOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataComplementOf><Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" /></DataComplementOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataComplementOf()
        {
            OWLDataComplementOf dataComplementOf = OWLTestSerializer<OWLDataComplementOf>.Deserialize(
@"<DataComplementOf>
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataComplementOf>");

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
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.COMPLEMENT_OF, RDFVocabulary.XSD.STRING, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.XSD.STRING, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE, null].TriplesCount == 1);
        }
        #endregion
    }
}