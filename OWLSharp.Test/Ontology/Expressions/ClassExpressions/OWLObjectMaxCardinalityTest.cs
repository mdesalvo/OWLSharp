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
    public class OWLObjectMaxCardinalityTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectMaxCardinality()
        {
            OWLObjectMaxCardinality objectMaxCardinality = new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);

            Assert.IsNotNull(objectMaxCardinality);
            Assert.IsTrue(string.Equals(objectMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMaxCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMaxCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(objectMaxCardinality.ClassExpression);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectMaxCardinalityBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectMaxCardinality(null, 1));

        [TestMethod]
        public void ShouldCreateObjectMaxQualifiedCardinality()
        {
            OWLObjectMaxCardinality objectMaxCardinality = new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));

            Assert.IsNotNull(objectMaxCardinality);
            Assert.IsTrue(string.Equals(objectMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMaxCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMaxCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(objectMaxCardinality.ClassExpression is OWLClass cls 
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectMaxQualifiedCardinalityBecauseNullObjectPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectMaxCardinality(null, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectMaxQualifiedCardinalityBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, null));

        [TestMethod]
        public void ShouldSerializeObjectMaxCardinality()
        {
            OWLObjectMaxCardinality objectMaxCardinality = new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);
            string serializedXML = OWLTestSerializer<OWLObjectMaxCardinality>.Serialize(objectMaxCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectMaxCardinality cardinality=""1""><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectMaxCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectMaxCardinality()
        {
            OWLObjectMaxCardinality objectMaxCardinality = OWLTestSerializer<OWLObjectMaxCardinality>.Deserialize(
@"<ObjectMaxCardinality cardinality=""1"">
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectMaxCardinality>");

            Assert.IsNotNull(objectMaxCardinality);
            Assert.IsTrue(string.Equals(objectMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMaxCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMaxCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(objectMaxCardinality.ClassExpression);
        }

        [TestMethod]
        public void ShouldSerializeObjectMaxQualifiedCardinality()
        {
            OWLObjectMaxCardinality objectMaxCardinality = new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLTestSerializer<OWLObjectMaxCardinality>.Serialize(objectMaxCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectMaxCardinality cardinality=""1""><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectMaxCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectMaxQualifiedCardinality()
        {
            OWLObjectMaxCardinality objectMaxCardinality = OWLTestSerializer<OWLObjectMaxCardinality>.Deserialize(
@"<ObjectMaxCardinality cardinality=""1"">
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectMaxCardinality>");

            Assert.IsNotNull(objectMaxCardinality);
            Assert.IsTrue(string.Equals(objectMaxCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMaxCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMaxCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(objectMaxCardinality.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldConvertMaxCardinalityToGraph()
        {
            OWLObjectMaxCardinality objectMaxCardinality = new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);
            RDFGraph graph = objectMaxCardinality.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.MAX_CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertMaxQualifiedCardinalityToGraph()
        {
            OWLObjectMaxCardinality objectMaxCardinality = new OWLObjectMaxCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));
            RDFGraph graph = objectMaxCardinality.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 4);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_CLASS, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
        }
        #endregion
    }
}