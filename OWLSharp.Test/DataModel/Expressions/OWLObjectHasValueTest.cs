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
    public class OWLObjectHasValueTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectHasValue()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Bob")));

            Assert.IsNotNull(objectHasValue);
            Assert.IsNotNull(objectHasValue.ObjectPropertyExpression);
            Assert.IsTrue(objectHasValue.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsNotNull(objectHasValue.IndividualExpression);
            Assert.IsTrue(objectHasValue.IndividualExpression is OWLNamedIndividual namedIdv
                            && string.Equals(namedIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldCreateObjectHasValueWithAnonymousIndividual()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLAnonymousIndividual("AnonIdv"));

            Assert.IsNotNull(objectHasValue);
            Assert.IsNotNull(objectHasValue.ObjectPropertyExpression);
            Assert.IsTrue(objectHasValue.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsNotNull(objectHasValue.IndividualExpression);
            Assert.IsTrue(objectHasValue.IndividualExpression is OWLAnonymousIndividual anonIdv
                            && string.Equals(anonIdv.NodeID, "AnonIdv"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectHasValueBecauseNullObjectPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectHasValue(null, new OWLNamedIndividual(new RDFResource("ex:Bob"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectHasValueBecauseNullIndividualExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), null));

        [TestMethod]
        public void ShouldSerializeObjectHasValue()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLTestSerializer<OWLObjectHasValue>.Serialize(objectHasValue);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectHasValue>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <NamedIndividual IRI=""ex:Bob"" />
</ObjectHasValue>"));
        }

        [TestMethod]
        public void ShouldSerializeObjectHasValueWithAnonymousIndividual()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLAnonymousIndividual("AnonIdv"));
            string serializedXML = OWLTestSerializer<OWLObjectHasValue>.Serialize(objectHasValue);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectHasValue>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</ObjectHasValue>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectHasValue()
        {
            OWLObjectHasValue objectHasValue = OWLTestSerializer<OWLObjectHasValue>.Deserialize(
@"<ObjectHasValue>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <NamedIndividual IRI=""ex:Bob"" />
</ObjectHasValue>");

            Assert.IsNotNull(objectHasValue);
            Assert.IsNotNull(objectHasValue.ObjectPropertyExpression);
            Assert.IsTrue(objectHasValue.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsNotNull(objectHasValue.IndividualExpression);
            Assert.IsTrue(objectHasValue.IndividualExpression is OWLNamedIndividual namedIdv
                            && string.Equals(namedIdv.IRI, "ex:Bob"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectHasValueWithAnonymousIndividual()
        {
            OWLObjectHasValue objectHasValue = OWLTestSerializer<OWLObjectHasValue>.Deserialize(
@"<ObjectHasValue>
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</ObjectHasValue>");

            Assert.IsNotNull(objectHasValue);
            Assert.IsNotNull(objectHasValue.ObjectPropertyExpression);
            Assert.IsTrue(objectHasValue.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsNotNull(objectHasValue.IndividualExpression);
            Assert.IsTrue(objectHasValue.IndividualExpression is OWLAnonymousIndividual anonIdv
                            && string.Equals(anonIdv.NodeID, "AnonIdv"));
        }
        #endregion
    }
}