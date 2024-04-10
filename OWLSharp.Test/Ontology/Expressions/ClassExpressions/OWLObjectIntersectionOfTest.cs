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

namespace OWLSharp.Ontology.Expressions.Test
{
    [TestClass]
    public class OWLObjectIntersectionOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectIntersectionOf()
        {
            OWLObjectIntersectionOf objectIntersectionOf = new OWLObjectIntersectionOf(
                [ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT) ]);

            Assert.IsNotNull(objectIntersectionOf);
            Assert.IsNotNull(objectIntersectionOf.ClassExpressions);
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Count == 2);
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString())));
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString())));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectIntersectionOfBecauseNullClassExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectIntersectionOf(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectIntersectionOfBecauseLessThan2ClassExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectIntersectionOf([ new OWLClass(RDFVocabulary.FOAF.PERSON) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectIntersectionOfBecauseNullClassExpressionFound()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectIntersectionOf([ new OWLClass(RDFVocabulary.FOAF.PERSON), null ]));

        [TestMethod]
        public void ShouldSerializeObjectIntersectionOf()
        {
            OWLObjectIntersectionOf objectIntersectionOf = new OWLObjectIntersectionOf(
                 [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]);
            string serializedXML = OWLTestSerializer<OWLObjectIntersectionOf>.Serialize(objectIntersectionOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectIntersectionOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
</ObjectIntersectionOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectIntersectionOf()
        {
            OWLObjectIntersectionOf objectIntersectionOf = OWLTestSerializer<OWLObjectIntersectionOf>.Deserialize(
@"<ObjectIntersectionOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
</ObjectIntersectionOf>");

            Assert.IsNotNull(objectIntersectionOf);
            Assert.IsNotNull(objectIntersectionOf.ClassExpressions);
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Count == 2);
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString())));
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString())));
        }
        #endregion
    }
}