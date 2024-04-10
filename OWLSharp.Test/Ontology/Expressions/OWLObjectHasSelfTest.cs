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

namespace OWLSharp.Ontology.Test
{
    [TestClass]
    public class OWLObjectHasSelfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectHasSelf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));

            Assert.IsNotNull(objectHasSelf);
            Assert.IsNotNull(objectHasSelf.ObjectPropertyExpression);
            Assert.IsTrue(objectHasSelf.ObjectPropertyExpression is OWLObjectProperty objProp 
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldCreateObjectHasSelfWithObjectinverseOf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));

            Assert.IsNotNull(objectHasSelf);
            Assert.IsNotNull(objectHasSelf.ObjectPropertyExpression);
            Assert.IsTrue(objectHasSelf.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectHasSelfBecauseNullObjectPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectHasSelf(null));

        [TestMethod]
        public void ShouldSerializeObjectHasSelf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLTestSerializer<OWLObjectHasSelf>.Serialize(objectHasSelf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectHasSelf>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectHasSelf>"));
        }

        [TestMethod]
        public void ShouldSerializeObjectHasSelfWithObjectInverseOfOf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLTestSerializer<OWLObjectHasSelf>.Serialize(objectHasSelf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectHasSelf>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
</ObjectHasSelf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectHasSelf()
        {
            OWLObjectHasSelf objectHasSelf = OWLTestSerializer<OWLObjectHasSelf>.Deserialize(
@"<ObjectHasSelf>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectHasSelf>");

            Assert.IsNotNull(objectHasSelf);
            Assert.IsNotNull(objectHasSelf.ObjectPropertyExpression);
            Assert.IsTrue(objectHasSelf.ObjectPropertyExpression is OWLObjectProperty pbjProp
                            && string.Equals(pbjProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeObjectHasSelfWithObjectInverseOf()
        {
            OWLObjectHasSelf objectHasSelf = OWLTestSerializer<OWLObjectHasSelf>.Deserialize(
@"<ObjectHasSelf>
  <ObjectInverseOf>
    <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  </ObjectInverseOf>
</ObjectHasSelf>");

            Assert.IsNotNull(objectHasSelf);
            Assert.IsNotNull(objectHasSelf.ObjectPropertyExpression);
            Assert.IsTrue(objectHasSelf.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }
        #endregion
    }
}