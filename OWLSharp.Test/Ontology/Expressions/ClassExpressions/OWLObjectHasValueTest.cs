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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Expressions
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
        public void ShouldGetSWRLRepresentationOfObjectHasValue()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string swrlString = objectHasValue.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(knows value ex:Bob)"));
        }

        [TestMethod]
        public void ShouldSerializeObjectHasValue()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Bob")));
            string serializedXML = OWLSerializer.SerializeObject(objectHasValue);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectHasValue><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><NamedIndividual IRI=""ex:Bob"" /></ObjectHasValue>"));
        }

        [TestMethod]
        public void ShouldSerializeObjectHasValueWithAnonymousIndividual()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLAnonymousIndividual("AnonIdv"));
            string serializedXML = OWLSerializer.SerializeObject(objectHasValue);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectHasValue><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><AnonymousIndividual nodeID=""AnonIdv"" /></ObjectHasValue>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectHasValue()
        {
            OWLObjectHasValue objectHasValue = OWLSerializer.DeserializeObject<OWLObjectHasValue>(
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
            OWLObjectHasValue objectHasValue = OWLSerializer.DeserializeObject<OWLObjectHasValue>(
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

        [TestMethod]
        public void ShouldConvertObjectHasValueToGraph()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Bob")));
            RDFGraph graph = objectHasValue.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 5);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.HAS_VALUE, new RDFResource("ex:Bob"), null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[new RDFResource("ex:Bob"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertObjectHasValueWithAnonymousIndividualToGraph()
        {
            OWLObjectHasValue objectHasValue = new OWLObjectHasValue(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLAnonymousIndividual("AnonIdv"));
            RDFGraph graph = objectHasValue.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 4);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.HAS_VALUE, new RDFResource("bnode:AnonIdv"), null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }
        #endregion
    }
}