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

namespace OWLSharp.DataModel.Test
{
    [TestClass]
    public class OWLDataMaxCardinalityTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataMaxCardinality()
        {
            OWLDataMaxCardinality dataMaxCardinality = new OWLDataMaxCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);

            Assert.IsNotNull(dataMaxCardinality);
            Assert.IsTrue(string.Equals(dataMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMaxCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMaxCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNull(dataMaxCardinality.DataRangeExpression);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataMaxCardinalityBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataMaxCardinality(null, 1));

        [TestMethod]
        public void ShouldCreateDataMaxQualifiedCardinality()
        {
            OWLDataMaxCardinality dataMaxCardinality = new OWLDataMaxCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));

            Assert.IsNotNull(dataMaxCardinality);
            Assert.IsTrue(string.Equals(dataMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMaxCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMaxCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNotNull(dataMaxCardinality.DataRangeExpression);
            Assert.IsTrue(dataMaxCardinality.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataMaxQualifiedCardinalityBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataMaxCardinality(null, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataMaxQualifiedCardinalityBecauseNullDataRange()
            => Assert.ThrowsException<OWLException>(() => new OWLDataMaxCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, null));

        [TestMethod]
        public void ShouldSerializeDataMaxCardinality()
        {
            OWLDataMaxCardinality dataMaxCardinality = new OWLDataMaxCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1);
            string serializedXML = OWLTestSerializer<OWLDataMaxCardinality>.Serialize(dataMaxCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataMaxCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
</DataMaxCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataMaxCardinality()
        {
            OWLDataMaxCardinality dataMaxCardinality = OWLTestSerializer<OWLDataMaxCardinality>.Deserialize(
@"<DataMaxCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
</DataMaxCardinality>");

            Assert.IsNotNull(dataMaxCardinality);
            Assert.IsTrue(string.Equals(dataMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMaxCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMaxCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNull(dataMaxCardinality.DataRangeExpression);
        }

        [TestMethod]
        public void ShouldSerializeDataMaxQualifiedCardinality()
        {
            OWLDataMaxCardinality dataMaxCardinality = new OWLDataMaxCardinality(new OWLDataProperty(RDFVocabulary.DC.DESCRIPTION), 1, new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLTestSerializer<OWLDataMaxCardinality>.Serialize(dataMaxCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataMaxCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataMaxCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataMaxQualifiedCardinality()
        {
            OWLDataMaxCardinality dataMaxCardinality = OWLTestSerializer<OWLDataMaxCardinality>.Deserialize(
@"<DataMaxCardinality cardinality=""1"">
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/description"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataMaxCardinality>");

            Assert.IsNotNull(dataMaxCardinality);
            Assert.IsTrue(string.Equals(dataMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(dataMaxCardinality.DataProperty);
            Assert.IsTrue(string.Equals(dataMaxCardinality.DataProperty.IRI, RDFVocabulary.DC.DESCRIPTION.ToString()));
            Assert.IsNotNull(dataMaxCardinality.DataRangeExpression);
            Assert.IsTrue(dataMaxCardinality.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }
        #endregion
    }
}