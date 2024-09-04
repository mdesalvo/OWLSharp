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
    public class OWLObjectExactCardinalityTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectExactCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);

            Assert.IsNotNull(objectExactCardinality);
            Assert.IsTrue(string.Equals(objectExactCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectExactCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectExactCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(objectExactCardinality.ClassExpression);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectExactCardinalityBecauseNullObjectProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectExactCardinality(null, 1));

        [TestMethod]
        public void ShouldCreateObjectExactQualifiedCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));

            Assert.IsNotNull(objectExactCardinality);
            Assert.IsTrue(string.Equals(objectExactCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectExactCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectExactCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(objectExactCardinality.ClassExpression is OWLClass cls 
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectExactQualifiedCardinalityBecauseNullObjectPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectExactCardinality(null, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectExactQualifiedCardinalityBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, null));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfObjectExactCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);
            string swrlString = objectExactCardinality.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(knows exactly 1)"));
        }

        [TestMethod]
        public void ShouldSerializeObjectExactCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);
            string serializedXML = OWLSerializer.SerializeObject(objectExactCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectExactCardinality cardinality=""1""><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /></ObjectExactCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectExactCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = OWLSerializer.DeserializeObject<OWLObjectExactCardinality>(
@"<ObjectExactCardinality cardinality=""1"">
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
</ObjectExactCardinality>");

            Assert.IsNotNull(objectExactCardinality);
            Assert.IsTrue(string.Equals(objectExactCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectExactCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectExactCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(objectExactCardinality.ClassExpression);
        }

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfObjectExactQualifiedCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));
            string swrlString = objectExactCardinality.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(knows exactly 1 Person)"));
        }

        [TestMethod]
        public void ShouldSerializeObjectExactQualifiedCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));
            string serializedXML = OWLSerializer.SerializeObject(objectExactCardinality);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectExactCardinality cardinality=""1""><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /></ObjectExactCardinality>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectExactQualifiedCardinality()
        {
            OWLObjectExactCardinality objectExactCardinality = OWLSerializer.DeserializeObject<OWLObjectExactCardinality>(
@"<ObjectExactCardinality cardinality=""1"">
  <ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
</ObjectExactCardinality>");

            Assert.IsNotNull(objectExactCardinality);
            Assert.IsTrue(string.Equals(objectExactCardinality.Cardinality, "1"));
            Assert.IsNotNull(objectExactCardinality.ObjectPropertyExpression);
            Assert.IsTrue(objectExactCardinality.ObjectPropertyExpression is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsTrue(objectExactCardinality.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
        }

        [TestMethod]
        public void ShouldConvertObjectExactCardinalityToGraph()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1);
            RDFGraph graph = objectExactCardinality.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 4);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

        [TestMethod]
        public void ShouldConvertObjectExactQualifiedCardinalityToGraph()
        {
            OWLObjectExactCardinality objectExactCardinality = new OWLObjectExactCardinality(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), 1, new OWLClass(RDFVocabulary.FOAF.PERSON));
            RDFGraph graph = objectExactCardinality.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 6);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.QUALIFIED_CARDINALITY, null, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.ON_CLASS, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
            Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }
        #endregion
    }
}