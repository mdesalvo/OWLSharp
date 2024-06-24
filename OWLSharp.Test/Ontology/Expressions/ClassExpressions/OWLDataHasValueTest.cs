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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Expressions
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
        public void ShouldSerializeDataHasValue()
        {
            OWLDataHasValue dataHasValue = new OWLDataHasValue(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("hello","en")));
            string serializedXML = OWLSerializer.Serialize(dataHasValue);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataHasValue><DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" /><Literal xml:lang=""EN"">hello</Literal></DataHasValue>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataHasValue()
        {
            OWLDataHasValue dataHasValue = OWLSerializer.Deserialize<OWLDataHasValue>(
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
            Assert.IsTrue(graph.TriplesCount == 4);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.DC.DESCRIPTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.HAS_VALUE, null, new RDFPlainLiteral("hello", "en")].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.DC.DESCRIPTION, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null].TriplesCount == 1);
        }
        #endregion
    }
}