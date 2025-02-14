/*
   Copyright 2014-2025 Marco De Salvo

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
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
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
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectHasSelf(null));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfObjectHasSelf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string swrlString = objectHasSelf.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(knows Self)"));
        }

        [TestMethod]
        public void ShouldSerializeObjectHasSelf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            string serializedXML = OWLSerializer.SerializeObject(objectHasSelf);

            Assert.IsTrue(string.Equals(serializedXML,
"""<ObjectHasSelf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectHasSelf>"""));
        }

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfObjectHasSelfWithObjectInverseOf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string swrlString = objectHasSelf.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(inverse(knows) Self)"));
        }

        [TestMethod]
        public void ShouldSerializeObjectHasSelfWithObjectInverseOf()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            string serializedXML = OWLSerializer.SerializeObject(objectHasSelf);

            Assert.IsTrue(string.Equals(serializedXML,
"""<ObjectHasSelf><ObjectInverseOf><ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" /></ObjectInverseOf></ObjectHasSelf>"""));
        }

        [TestMethod]
        public void ShouldDeserializeObjectHasSelf()
        {
            OWLObjectHasSelf objectHasSelf = OWLSerializer.DeserializeObject<OWLObjectHasSelf>(
                """
                <ObjectHasSelf>
                  <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                </ObjectHasSelf>
                """);

            Assert.IsNotNull(objectHasSelf);
            Assert.IsNotNull(objectHasSelf.ObjectPropertyExpression);
            Assert.IsTrue(objectHasSelf.ObjectPropertyExpression is OWLObjectProperty pbjProp
                            && string.Equals(pbjProp.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldDeserializeObjectHasSelfWithObjectInverseOf()
        {
            OWLObjectHasSelf objectHasSelf = OWLSerializer.DeserializeObject<OWLObjectHasSelf>(
                """
                <ObjectHasSelf>
                  <ObjectInverseOf>
                    <ObjectProperty IRI="http://xmlns.com/foaf/0.1/knows" />
                  </ObjectInverseOf>
                </ObjectHasSelf>
                """);

            Assert.IsNotNull(objectHasSelf);
            Assert.IsNotNull(objectHasSelf.ObjectPropertyExpression);
            Assert.IsTrue(objectHasSelf.ObjectPropertyExpression is OWLObjectInverseOf objInvOf
                            && string.Equals(objInvOf.ObjectProperty.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
        }

        [TestMethod]
        public void ShouldConvertObjectHasSelfToGraph()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS));
            RDFGraph graph = objectHasSelf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(4, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.HAS_SELF, null, RDFTypedLiteral.True].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        }

        [TestMethod]
        public void ShouldConvertObjectHasSelfWithObjectInverseOfToGraph()
        {
            OWLObjectHasSelf objectHasSelf = new OWLObjectHasSelf(new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)));
            RDFGraph graph = objectHasSelf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(5, graph.TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.ON_PROPERTY, null, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.HAS_SELF, null, RDFTypedLiteral.True].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INVERSE_OF, RDFVocabulary.FOAF.KNOWS, null].TriplesCount);
            Assert.AreEqual(1, graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount);
        }
        #endregion
    }
}