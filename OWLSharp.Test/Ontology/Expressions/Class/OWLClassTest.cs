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
using OWLSharp.Test;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Test
{
    [TestClass]
    public class OWLClassTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIClass()
        {
            OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);

            Assert.IsNotNull(cls);
            Assert.IsTrue(string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(cls.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLClass(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLClass(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameClass()
        {
            OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(cls);
            Assert.IsNull(cls.IRI);
            Assert.IsTrue(string.Equals(cls.AbbreviatedIRI, new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLClass(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIClass()
        {
            OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
            string serializedXML = OWLTestSerializer<OWLClass>.Serialize(cls);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Class IRI=""http://xmlns.com/foaf/0.1/Person"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIClass()
        {
            OWLClass cls = OWLTestSerializer<OWLClass>.Deserialize(
@"<Class IRI=""http://xmlns.com/foaf/0.1/Person"" />");

            Assert.IsNotNull(cls);
            Assert.IsTrue(string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(cls.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameClass()
        {
            OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLTestSerializer<OWLClass>.Serialize(cls);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Class xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:Person"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameClass()
        {
            OWLClass cls = OWLTestSerializer<OWLClass>.Deserialize(
@"<Class xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:Person"" />");

            Assert.IsNotNull(cls);
            Assert.IsNull(cls.IRI);
            Assert.IsTrue(string.Equals(cls.AbbreviatedIRI, new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
        }
        #endregion
    }
}