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
using RDFSharp.Model;

namespace OWLSharp.Serialization.Test
{
    [TestClass]
    public class OWLDataAllValuesFromTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataAllValuesFrom()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = new OWLDataAllValuesFrom(
                [ new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())) ],
                new OWLDatatype(RDFVocabulary.XSD.STRING));

            Assert.IsNotNull(dataAllValuesFrom);
            Assert.IsNotNull(dataAllValuesFrom.DataProperties);
            Assert.IsTrue(dataAllValuesFrom.DataProperties.Single() is OWLDataProperty dataProperty 
                            && string.Equals(dataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataAllValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataAllValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataAllValuesFromBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataAllValuesFrom(
                null, new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataAllValuesFromBecauseZeroDataProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLDataAllValuesFrom(
                [], new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataAllValuesFromBecauseNullDataPropertyFound()
            => Assert.ThrowsException<OWLException>(() => new OWLDataAllValuesFrom(
                [null], new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataAllValuesFromBecauseNullDataRange()
            => Assert.ThrowsException<OWLException>(() => new OWLDataAllValuesFrom(
                [ new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())) ], null));

        [TestMethod]
        public void ShouldSerializeDataAllValuesFrom()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = new OWLDataAllValuesFrom(
                [ new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())) ],
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLTestSerializer<OWLDataAllValuesFrom>.Serialize(dataAllValuesFrom);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataAllValuesFrom>
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataAllValuesFrom>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataAllValuesFrom()
        {
            OWLDataAllValuesFrom dataAllValuesFrom = OWLTestSerializer<OWLDataAllValuesFrom>.Deserialize(
@"<DataAllValuesFrom>
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataAllValuesFrom>");

            Assert.IsNotNull(dataAllValuesFrom);
            Assert.IsNotNull(dataAllValuesFrom.DataProperties);
            Assert.IsTrue(dataAllValuesFrom.DataProperties.Single() is OWLDataProperty dataProperty 
                            && string.Equals(dataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataAllValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataAllValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }
        #endregion
    }
}