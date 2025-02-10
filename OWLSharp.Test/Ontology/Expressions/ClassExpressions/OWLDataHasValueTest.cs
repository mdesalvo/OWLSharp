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
    public class OWLDataHasValueTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataHasValue()
        {
            OWLDataHasValue dataHasValue = new OWLDataHasValue(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("hello","en")));

            Assert.IsNotNull(dataHasValue);
            Assert.IsNotNull(dataHasValue.DataProperty);
            Assert.IsTrue(string.Equals(dataHasValue.DataProperty.IRI, "http://purl.org/dc/elements/1.1/description"));
            Assert.IsNotNull(dataHasValue.Literal);
            Assert.IsTrue(string.Equals(dataHasValue.Literal.Value, "hello"));
            Assert.IsTrue(string.Equals(dataHasValue.Literal.Language, "EN"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataHasValueBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataHasValue(null, new OWLLiteral(new RDFPlainLiteral("hello","en"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataHasValueBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLDataHasValue(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), null));

        [TestMethod]
        public void ShouldgetSWRLRepresentationOfDataHasValue()
        {
            OWLDataHasValue dataHasValue = new OWLDataHasValue(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("hello", "en")));
            string swrlString = dataHasValue.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(description value \"hello\"@EN)"));
        }

        [TestMethod]
        public void ShouldSerializeDataHasValue()
        {
            OWLDataHasValue dataHasValue = new OWLDataHasValue(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("hello","en")));
            string serializedXML = OWLSerializer.SerializeObject(dataHasValue);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataHasValue><DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" /><Literal xml:lang=""EN"">hello</Literal></DataHasValue>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataHasValue()
        {
            OWLDataHasValue dataHasValue = OWLSerializer.DeserializeObject<OWLDataHasValue>(
@"<DataHasValue>
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  <Literal xml:lang=""EN"">hello</Literal>
</DataHasValue>");

            Assert.IsNotNull(dataHasValue);
            Assert.IsNotNull(dataHasValue.DataProperty);
            Assert.IsTrue(string.Equals(dataHasValue.DataProperty.IRI, "http://purl.org/dc/elements/1.1/description"));
            Assert.IsNotNull(dataHasValue.Literal);
            Assert.IsTrue(string.Equals(dataHasValue.Literal.Value, "hello"));
            Assert.IsTrue(string.Equals(dataHasValue.Literal.Language, "EN"));
        }


        [TestMethod]
        public void ShouldConvertDataHasValueToGraph()
        {
            OWLDataHasValue dataHasValue = new OWLDataHasValue(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("hello", "en")));
            RDFGraph graph = dataHasValue.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(4, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.DC.DESCRIPTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.HAS_VALUE, null, new RDFPlainLiteral("hello", "en")].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.DC.DESCRIPTION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount);
        }
        #endregion
    }
}