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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
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
            Assert.AreEqual(2, objectIntersectionOf.ClassExpressions.Count);
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString())));
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString())));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectIntersectionOfBecauseNullClassExpressions()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectIntersectionOf(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectIntersectionOfBecauseLessThan2ClassExpressions()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectIntersectionOf([ new OWLClass(RDFVocabulary.FOAF.PERSON) ]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectIntersectionOfBecauseNullClassExpressionFound()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLObjectIntersectionOf([ new OWLClass(RDFVocabulary.FOAF.PERSON), null ]));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfObjectIntersectionOf()
        {
            OWLObjectIntersectionOf objectIntersectionOf = new OWLObjectIntersectionOf([
                new OWLObjectUnionOf(
                    [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]),
                new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)]);
            string swrlString = objectIntersectionOf.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "((Person or Agent) and Organization)"));
        }

        [TestMethod]
        public void ShouldSerializeObjectIntersectionOf()
        {
            OWLObjectIntersectionOf objectIntersectionOf = new OWLObjectIntersectionOf(
                 [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]);
            string serializedXML = OWLSerializer.SerializeObject(objectIntersectionOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectIntersectionOf><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /></ObjectIntersectionOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectIntersectionOf()
        {
            OWLObjectIntersectionOf objectIntersectionOf = OWLSerializer.DeserializeObject<OWLObjectIntersectionOf>(
@"<ObjectIntersectionOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Person"" />
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
</ObjectIntersectionOf>");

            Assert.IsNotNull(objectIntersectionOf);
            Assert.IsNotNull(objectIntersectionOf.ClassExpressions);
            Assert.AreEqual(2, objectIntersectionOf.ClassExpressions.Count);
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.PERSON.ToString())));
            Assert.IsTrue(objectIntersectionOf.ClassExpressions.Any(cex => cex is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString())));
        }

        [TestMethod]
        public void ShouldConvertObjectIntersectionOfToGraph()
        {
            OWLObjectIntersectionOf objectIntersectionOf = new OWLObjectIntersectionOf(
                [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]);
            RDFGraph graph = objectIntersectionOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(10, graph.TriplesCount);
            Assert.AreEqual(3, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.INTERSECTION_OF, null, null].TriplesCount);
            Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.PERSON, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
            Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount);
        }
        #endregion
    }
}