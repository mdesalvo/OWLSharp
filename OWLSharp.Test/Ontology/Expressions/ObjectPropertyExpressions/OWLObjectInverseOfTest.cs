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
    public class OWLObjectInverseOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectInverseOf()
        {
            OWLObjectInverseOf objectInverseOf = new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

            Assert.IsNotNull(objectInverseOf);
            Assert.IsNotNull(objectInverseOf.ObjectProperty);
            Assert.IsTrue(string.Equals(objectInverseOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectInverseOfBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectInverseOf(null));

        [TestMethod]
        public void ShouldSerializeObjectInverseOf()
        {
            OWLObjectInverseOf objectInverseOf = new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLTestSerializer<OWLObjectInverseOf>.Serialize(objectInverseOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectInverseOf><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectInverseOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectInverseOf()
        {
            OWLObjectInverseOf objectInverseOf = OWLTestSerializer<OWLObjectInverseOf>.Deserialize(
@"<ObjectInverseOf>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectInverseOf>");

            Assert.IsNotNull(objectInverseOf);
            Assert.IsNotNull(objectInverseOf.ObjectProperty);
            Assert.IsTrue(string.Equals(objectInverseOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }
        #endregion
    }
}