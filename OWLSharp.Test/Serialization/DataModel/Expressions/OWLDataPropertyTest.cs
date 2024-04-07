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

using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLDataPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIDataProperty()
        {
            OWLDataProperty dp = new OWLDataProperty(RDFVocabulary.FOAF.AGE);

            Assert.IsNotNull(dp);
            Assert.IsTrue(string.Equals(dp.IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsNull(dp.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLDataProperty(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLDataProperty(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameDataProperty()
        {
            OWLDataProperty dp = new OWLDataProperty(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(dp);
            Assert.IsNull(dp.IRI);
            Assert.IsTrue(string.Equals(dp.AbbreviatedIRI, new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataPropertyBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLDataProperty(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIDataProperty()
        {
            OWLDataProperty dp = new OWLDataProperty(RDFVocabulary.FOAF.AGE);
            string serializedXML = OWLTestSerializer<OWLDataProperty>.Serialize(dp);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIDataProperty()
        {
            OWLDataProperty dp = OWLTestSerializer<OWLDataProperty>.Deserialize(
@"<DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" />");

            Assert.IsNotNull(dp);
            Assert.IsTrue(string.Equals(dp.IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsNull(dp.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameDataProperty()
        {
            OWLDataProperty dp = new OWLDataProperty(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLTestSerializer<OWLDataProperty>.Serialize(dp);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataProperty xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:age"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameDataProperty()
        {
            OWLDataProperty dp = OWLTestSerializer<OWLDataProperty>.Deserialize(
@"<DataProperty xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:age"" />");

            Assert.IsNotNull(dp);
            Assert.IsNull(dp.IRI);
            Assert.IsTrue(string.Equals(dp.AbbreviatedIRI, new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI)));
        }
        #endregion
    }
}