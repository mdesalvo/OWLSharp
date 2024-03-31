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
    public class OWLAnnotationPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR);

            Assert.IsNotNull(annotation);
            Assert.IsTrue(string.Equals(annotation.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNull(annotation.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationPropertyBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationProperty(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationPropertyBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationProperty(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI));

            Assert.IsNotNull(annotation);
            Assert.IsNull(annotation.IRI);
            Assert.IsTrue(string.Equals(annotation.AbbreviatedIRI, new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationPropertyBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationProperty(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR);
            string serializedXML = OWLTestSerializer<OWLAnnotationProperty>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<OWLAnnotationProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />"));
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI));
            string serializedXML = OWLTestSerializer<OWLAnnotationProperty>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<OWLAnnotationProperty xmlns:q1=""http://purl.org/dc/elements/1.1/"" abbreviatedIRI=""q1:creator"" />"));
        }
        #endregion
    }
}