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
    public class OWLObjectUnionOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectUnionOf()
        {
            OWLObjectUnionOf objectUnionOf = new OWLObjectUnionOf(
                [ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT) ]);

            Assert.IsNotNull(objectUnionOf);
            Assert.IsNotNull(objectUnionOf.ClassExpressions);
            Assert.IsTrue(objectUnionOf.ClassExpressions.Count == 2);
            Assert.IsTrue(objectUnionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString())));
            Assert.IsTrue(objectUnionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString())));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectUnionOfBecauseNullClassExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectUnionOf(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectUnionOfBecauseLessThan2ClassExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectUnionOf([ new OWLClass(RDFVocabulary.FOAF.PERSON) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectUnionOfBecauseNullClassExpressionFound()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectUnionOf([ new OWLClass(RDFVocabulary.FOAF.PERSON), null ]));

        [TestMethod]
        public void ShouldSerializeObjectUnionOf()
        {
            OWLObjectUnionOf objectUnionOf = new OWLObjectUnionOf(
                 [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]);
            string serializedXML = OWLTestSerializer<OWLObjectUnionOf>.Serialize(objectUnionOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectUnionOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
</ObjectUnionOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectUnionOf()
        {
            OWLObjectUnionOf objectUnionOf = OWLTestSerializer<OWLObjectUnionOf>.Deserialize(
@"<ObjectUnionOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
</ObjectUnionOf>");

            Assert.IsNotNull(objectUnionOf);
            Assert.IsNotNull(objectUnionOf.ClassExpressions);
            Assert.IsTrue(objectUnionOf.ClassExpressions.Count == 2);
            Assert.IsTrue(objectUnionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString())));
            Assert.IsTrue(objectUnionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString())));
        }
        #endregion
    }
}