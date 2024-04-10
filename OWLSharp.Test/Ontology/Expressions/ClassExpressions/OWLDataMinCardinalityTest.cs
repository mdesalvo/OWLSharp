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

namespace OWLSharp.Ontology.Expressions.Test
{
    [TestClass]
    public class OWLDataMinCardinalityTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataMinCardinality()
        {
            OWLDataMinCardinality dataMinCardinality = new OWLDataMinCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);

            Assert.IsNotNull(dataMinCardinality);
            Assert.IsTrue(string.Equals(dataMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMinCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMinCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNull(dataMinCardinality.DataRangeExpression);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataMinCardinalityBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataMinCardinality(null, 1));

        [TestMethod]
        public void ShouldCreateDataMinQualifiedCardinality()
        {
            OWLDataMinCardinality dataMinCardinality = new OWLDataMinCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));

            Assert.IsNotNull(dataMinCardinality);
            Assert.IsTrue(string.Equals(dataMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMinCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMinCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNotNull(dataMinCardinality.DataRangeExpression);
            Assert.IsTrue(dataMinCardinality.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataMinQualifiedCardinalityBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataMinCardinality(null, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataMinQualifiedCardinalityBecauseNullDataRange()
            => Assert.ThrowsException<OWLException>(() => new OWLDataMinCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, null));

        [TestMethod]
        public void ShouldSerializeDataMinCardinality()
        {
            OWLDataMinCardinality dataMinCardinality = new OWLDataMinCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);
            string serializedXML = OWLTestSerializer<OWLDataMinCardinality>.Serialize(dataMinCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataMinCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
</DataMinCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataMinCardinality()
        {
            OWLDataMinCardinality dataMinCardinality = OWLTestSerializer<OWLDataMinCardinality>.Deserialize(
@"<DataMinCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
</DataMinCardinality>");

            Assert.IsNotNull(dataMinCardinality);
            Assert.IsTrue(string.Equals(dataMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMinCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMinCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNull(dataMinCardinality.DataRangeExpression);
        }

        [TestMethod]
        public void ShouldSerializeDataMinQualifiedCardinality()
        {
            OWLDataMinCardinality dataMinCardinality = new OWLDataMinCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLTestSerializer<OWLDataMinCardinality>.Serialize(dataMinCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataMinCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataMinCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataMinQualifiedCardinality()
        {
            OWLDataMinCardinality dataMinCardinality = OWLTestSerializer<OWLDataMinCardinality>.Deserialize(
@"<DataMinCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataMinCardinality>");

            Assert.IsNotNull(dataMinCardinality);
            Assert.IsTrue(string.Equals(dataMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMinCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMinCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNotNull(dataMinCardinality.DataRangeExpression);
            Assert.IsTrue(dataMinCardinality.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }
        #endregion
    }
}