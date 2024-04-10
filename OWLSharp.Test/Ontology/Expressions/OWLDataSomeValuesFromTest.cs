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
using OWLSharp.Test;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Test
{
    [TestClass]
    public class OWLDataSomeValuesFromTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataSomeValuesFrom()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = new OWLDataSomeValuesFrom(
                [ new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())) ],
                new OWLDatatype(RDFVocabulary.XSD.STRING));

            Assert.IsNotNull(dataSomeValuesFrom);
            Assert.IsNotNull(dataSomeValuesFrom.DataProperties);
            Assert.IsTrue(dataSomeValuesFrom.DataProperties.Single() is OWLDataProperty dataProperty 
                            && string.Equals(dataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataSomeValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataSomeValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataSomeValuesFromBecauseNullDataProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLDataSomeValuesFrom(
                null, new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataSomeValuesFromBecauseZeroDataProperties()
            => Assert.ThrowsException<OWLException>(() => new OWLDataSomeValuesFrom(
                [], new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataSomeValuesFromBecauseNullDataPropertyFound()
            => Assert.ThrowsException<OWLException>(() => new OWLDataSomeValuesFrom(
                [null], new OWLDatatype(RDFVocabulary.XSD.STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataSomeValuesFromBecauseNullDataRange()
            => Assert.ThrowsException<OWLException>(() => new OWLDataSomeValuesFrom(
                [ new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())) ], null));

        [TestMethod]
        public void ShouldSerializeDataSomeValuesFrom()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = new OWLDataSomeValuesFrom(
                [ new OWLDataProperty(new RDFResource(RDFVocabulary.DC.CREATOR.ToString())) ],
                new OWLDatatype(RDFVocabulary.XSD.STRING));
            string serializedXML = OWLTestSerializer<OWLDataSomeValuesFrom>.Serialize(dataSomeValuesFrom);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataSomeValuesFrom>
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataSomeValuesFrom>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataSomeValuesFrom()
        {
            OWLDataSomeValuesFrom dataSomeValuesFrom = OWLTestSerializer<OWLDataSomeValuesFrom>.Deserialize(
@"<DataSomeValuesFrom>
  <DataProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
</DataSomeValuesFrom>");

            Assert.IsNotNull(dataSomeValuesFrom);
            Assert.IsNotNull(dataSomeValuesFrom.DataProperties);
            Assert.IsTrue(dataSomeValuesFrom.DataProperties.Single() is OWLDataProperty dataProperty 
                            && string.Equals(dataProperty.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNotNull(dataSomeValuesFrom.DataRangeExpression);
            Assert.IsTrue(dataSomeValuesFrom.DataRangeExpression is OWLDatatype datatype 
                            && string.Equals(datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
        }
        #endregion
    }
}