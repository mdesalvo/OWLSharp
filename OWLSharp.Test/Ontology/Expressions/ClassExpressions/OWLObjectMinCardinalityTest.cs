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
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Expressions
{
    [TestClass]
    public class OWLObjectMinCardinalityTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectMinCardinality()
        {
            OWLObjectMinCardinality objectMinCardinality = new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);

            Assert.IsNotNull(objectMinCardinality);
            Assert.IsTrue(string.Equals(objectMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMinCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMinCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(objectMinCardinality.ClassExpression);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectMinCardinalityBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectMinCardinality(null, 1));

        [TestMethod]
        public void ShouldCreateObjectMinQualifiedCardinality()
        {
            OWLObjectMinCardinality objectMinCardinality = new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));

            Assert.IsNotNull(objectMinCardinality);
            Assert.IsTrue(string.Equals(objectMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMinCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMinCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(objectMinCardinality.ClassExpression is OWLClass cls 
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectMinQualifiedCardinalityBecauseNullObjectPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectMinCardinality(null, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectMinQualifiedCardinalityBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, null));

        [TestMethod]
        public void ShouldSerializeObjectMinCardinality()
        {
            OWLObjectMinCardinality objectMinCardinality = new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);
            string serializedXML = OWLTestSerializer<OWLObjectMinCardinality>.Serialize(objectMinCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectMinCardinality cardinality=""1""><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectMinCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectMinCardinality()
        {
            OWLObjectMinCardinality objectMinCardinality = OWLTestSerializer<OWLObjectMinCardinality>.Deserialize(
@"<ObjectMinCardinality cardinality=""1"">
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectMinCardinality>");

            Assert.IsNotNull(objectMinCardinality);
            Assert.IsTrue(string.Equals(objectMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMinCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMinCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(objectMinCardinality.ClassExpression);
        }

        [TestMethod]
        public void ShouldSerializeObjectMinQualifiedCardinality()
        {
            OWLObjectMinCardinality objectMinCardinality = new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLTestSerializer<OWLObjectMinCardinality>.Serialize(objectMinCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectMinCardinality cardinality=""1""><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectMinCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectMinQualifiedCardinality()
        {
            OWLObjectMinCardinality objectMinCardinality = OWLTestSerializer<OWLObjectMinCardinality>.Deserialize(
@"<ObjectMinCardinality cardinality=""1"">
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectMinCardinality>");

            Assert.IsNotNull(objectMinCardinality);
            Assert.IsTrue(string.Equals(objectMinCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectMinCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectMinCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(objectMinCardinality.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldConvertMinCardinalityToGraph()
        {
            OWLObjectMinCardinality objectMinCardinality = new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);
            RDFGraph graph = objectMinCardinality.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 4);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.MIN_CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertMinQualifiedCardinalityToGraph()
        {
            OWLObjectMinCardinality objectMinCardinality = new OWLObjectMinCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));
            RDFGraph graph = objectMinCardinality.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 6);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_CLASS, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }
        #endregion
    }
}