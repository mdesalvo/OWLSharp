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
using OWLSharp.Test;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Test
{
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
            => Assert.ThrowsException<OWLException>(() => new OWLDataExactCardinality(null, 1));

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
            => Assert.ThrowsException<OWLException>(() => new OWLDataExactCardinality(null, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataExactQualifiedCardinalityBecauseNullDataRange()
            => Assert.ThrowsException<OWLException>(() => new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, null));

        [TestMethod]
        public void ShouldSerializeDataExactCardinality()
        {
            OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);
            string serializedXML = OWLTestSerializer<OWLDataExactCardinality>.Serialize(dataExactCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataExactCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
</DataExactCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataExactCardinality()
        {
            OWLDataExactCardinality dataExactCardinality = OWLTestSerializer<OWLDataExactCardinality>.Deserialize(
@"<DataExactCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
</DataExactCardinality>");

            Assert.IsNotNull(dataExactCardinality);
            Assert.IsTrue(string.Equals(dataExactCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataExactCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataExactCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNull(dataExactCardinality.DataRangeExpression);
        }

        [TestMethod]
        public void ShouldSerializeDataExactQualifiedCardinality()
        {
            OWLDataExactCardinality dataExactCardinality = new OWLDataExactCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLTestSerializer<OWLDataExactCardinality>.Serialize(dataExactCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataExactCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataExactCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataExactQualifiedCardinality()
        {
            OWLDataExactCardinality dataExactCardinality = OWLTestSerializer<OWLDataExactCardinality>.Deserialize(
@"<DataExactCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataExactCardinality>");

            Assert.IsNotNull(dataExactCardinality);
            Assert.IsTrue(string.Equals(dataExactCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataExactCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataExactCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNotNull(dataExactCardinality.DataRangeExpression);
            Assert.IsTrue(dataExactCardinality.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }
        #endregion
    }
}