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

namespace OWLSharp.Serialization.Test
{
    [TestClass]
    public class OWLObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIObjectProperty()
        {
            OWLObjectProperty dp = new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS);

            Assert.IsNotNull(dp);
            Assert.IsTrue(string.Equals(dp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(dp.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectProperty(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectProperty(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameObjectProperty()
        {
            OWLObjectProperty dp = new OWLObjectProperty(new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(dp);
            Assert.IsNull(dp.IRI);
            Assert.IsTrue(string.Equals(dp.AbbreviatedIRI, new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectProperty(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIObjectProperty()
        {
            OWLObjectProperty dp = new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS);
            string serializedXML = OWLTestSerializer<OWLObjectProperty>.Serialize(dp);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIObjectProperty()
        {
            OWLObjectProperty dp = OWLTestSerializer<OWLObjectProperty>.Deserialize(
@"<ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />");

            Assert.IsNotNull(dp);
            Assert.IsTrue(string.Equals(dp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(dp.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameObjectProperty()
        {
            OWLObjectProperty dp = new OWLObjectProperty(new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLTestSerializer<OWLObjectProperty>.Serialize(dp);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectProperty xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:knows"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameObjectProperty()
        {
            OWLObjectProperty dp = OWLTestSerializer<OWLObjectProperty>.Deserialize(
@"<ObjectProperty xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:knows"" />");

            Assert.IsNotNull(dp);
            Assert.IsNull(dp.IRI);
            Assert.IsTrue(string.Equals(dp.AbbreviatedIRI, new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI)));
        }
        #endregion
    }
}