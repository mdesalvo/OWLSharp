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
        public void ShouldGetSWRLRepresentationOfObjectUnionOf()
        {
            OWLObjectUnionOf objectUnionOf = new OWLObjectUnionOf([
                new OWLObjectUnionOf(
                    [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]),
                new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)]);
            string swrlString = objectUnionOf.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "((Person or Agent) or Organization)"));
        }

        [TestMethod]
        public void ShouldSerializeObjectUnionOf()
        {
            OWLObjectUnionOf objectUnionOf = new OWLObjectUnionOf(
                 [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]);
            string serializedXML = OWLSerializer.SerializeObject(objectUnionOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectUnionOf><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /></ObjectUnionOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectUnionOf()
        {
            OWLObjectUnionOf objectUnionOf = OWLSerializer.DeserializeObject<OWLObjectUnionOf>(
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

        [TestMethod]
        public void ShouldConvertObjectUnionOfToGraph()
        {
            OWLObjectUnionOf objectUnionOf = new OWLObjectUnionOf(
                [new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)]);
            RDFGraph graph = objectUnionOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 10);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 3);
            Assert.IsTrue(graph[null, RDFVocabulary.OWL.UNION_OF, null, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 2);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.PERSON, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.FIRST, RDFVocabulary.FOAF.AGENT, null].TriplesCount == 1);
            Assert.IsTrue(graph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 2);
        }
        #endregion
    }
}