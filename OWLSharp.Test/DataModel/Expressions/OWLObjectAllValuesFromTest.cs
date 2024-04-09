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
    public class OWLObjectAllValuesFromTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectAllValuesFrom()
        {
            OWLObjectAllValuesFrom objectAllValuesFrom = new OWLObjectAllValuesFrom(
                new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())),
                new OWLClass(RDFVocabulary.FOAF.PERSON));

            Assert.IsNotNull(objectAllValuesFrom);
            Assert.IsNotNull(objectAllValuesFrom.ObjectPropertyExpression);
            Assert.IsTrue(objectAllValuesFrom.ObjectPropertyExpression is OWLObjectProperty objectProperty 
                            && string.Equals(objectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectAllValuesFrom.ClassExpression);
            Assert.IsTrue(objectAllValuesFrom.ClassExpression is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectAllValuesFromBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectAllValuesFrom(
                null, new OWLClass(RDFVocabulary.FOAF.PERSON)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectAllValuesFromBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectAllValuesFrom(
                new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())), null));

        [TestMethod]
        public void ShouldSerializeObjectAllValuesFrom()
        {
            OWLObjectAllValuesFrom objectAllValuesFrom = new OWLObjectAllValuesFrom(
                new OWLObjectProperty(new RDFResource(RDFVocabulary.FOAF.KNOWS.ToString())),
                new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLTestSerializer<OWLObjectAllValuesFrom>.Serialize(objectAllValuesFrom);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectAllValuesFrom>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectAllValuesFrom>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectAllValuesFrom()
        {
            OWLObjectAllValuesFrom objectAllValuesFrom = OWLTestSerializer<OWLObjectAllValuesFrom>.Deserialize(
@"<ObjectAllValuesFrom>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectAllValuesFrom>");

            Assert.IsNotNull(objectAllValuesFrom);
            Assert.IsNotNull(objectAllValuesFrom.ObjectPropertyExpression);
            Assert.IsTrue(objectAllValuesFrom.ObjectPropertyExpression is OWLObjectProperty objectProperty
                            && string.Equals(objectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNotNull(objectAllValuesFrom.ClassExpression);
            Assert.IsTrue(objectAllValuesFrom.ClassExpression is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }
        #endregion
    }
}