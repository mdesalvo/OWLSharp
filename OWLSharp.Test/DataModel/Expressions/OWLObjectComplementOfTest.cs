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
    public class OWLObjectComplementOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectComplementOf()
        {
            OWLObjectComplementOf objectComplementOf = new OWLObjectComplementOf(new OWLClass(RDFVocabulary.FOAF.KNOWS));

            Assert.IsNotNull(objectComplementOf);
            Assert.IsNotNull(objectComplementOf.ClassExpression);
            Assert.IsTrue(objectComplementOf.ClassExpression is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectComplementOfBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectComplementOf(null));

        [TestMethod]
        public void ShouldSerializeObjectComplementOf()
        {
            OWLObjectComplementOf objectComplementOf = new OWLObjectComplementOf(new OWLClass(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLTestSerializer<OWLObjectComplementOf>.Serialize(objectComplementOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectComplementOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectComplementOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectComplementOf()
        {
            OWLObjectComplementOf objectComplementOf = OWLTestSerializer<OWLObjectComplementOf>.Deserialize(
@"<ObjectComplementOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectComplementOf>");

            Assert.IsNotNull(objectComplementOf);
            Assert.IsNotNull(objectComplementOf.ClassExpression);
            Assert.IsTrue(objectComplementOf.ClassExpression is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }
        #endregion
    }
}