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
    public class OWLObjectComplementOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectComplementOf()
        {
            OWLObjectComplementOf objectComplementOf = new OWLObjectComplementOf(new OWLClass(RDFVocabulary.FOAF.AGENT));

            Assert.IsNotNull(objectComplementOf);
            Assert.IsNotNull(objectComplementOf.ClassExpression);
            Assert.IsTrue(objectComplementOf.ClassExpression is OWLClass owlClass 
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectComplementOfBecauseNullClassExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectComplementOf(null));

        [TestMethod]
        public void ShouldGetSWRLRepresentationOfObjectComplementOf()
        {
            OWLObjectComplementOf objectComplementOf = new OWLObjectComplementOf(
                new OWLObjectIntersectionOf([
                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                    new OWLObjectComplementOf(
                        new OWLObjectUnionOf([
                            new OWLClass(RDFVocabulary.FOAF.AGENT),
                            new OWLClass(RDFVocabulary.FOAF.PERSON)]))]));
            string swrlString = objectComplementOf.ToSWRLString();

            Assert.IsTrue(string.Equals(swrlString, "(not(Agent and (not(Agent or Person))))"));
        }

        [TestMethod]
        public void ShouldSerializeObjectComplementOf()
        {
            OWLObjectComplementOf objectComplementOf = new OWLObjectComplementOf(new OWLClass(RDFVocabulary.FOAF.AGENT));
            string serializedXML = OWLSerializer.SerializeObject(objectComplementOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectComplementOf><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /></ObjectComplementOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectComplementOf()
        {
            OWLObjectComplementOf objectComplementOf = OWLSerializer.DeserializeObject<OWLObjectComplementOf>(
@"<ObjectComplementOf>
  <Class IRI=""http://xmlns.com/foaf/0.1/Agent"" />
</ObjectComplementOf>");

            Assert.IsNotNull(objectComplementOf);
            Assert.IsNotNull(objectComplementOf.ClassExpression);
            Assert.IsTrue(objectComplementOf.ClassExpression is OWLClass owlClass
                            && string.Equals(owlClass.IRI, RDFVocabulary.FOAF.AGENT.ToString()));
        }

        [TestMethod]
        public void ShouldConvertObjectComplementOfToGraph()
        {
            OWLObjectComplementOf objectComplementOf = new OWLObjectComplementOf(new OWLClass(RDFVocabulary.FOAF.AGENT)); 
            RDFGraph graph = objectComplementOf.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.AreEqual(3, graph.TriplesCount);
            Assert.AreEqual(2, graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount);
            Assert.AreEqual(1, graph[null, RDFVocabulary.OWL.COMPLEMENT_OF, RDFVocabulary.FOAF.AGENT, null].TriplesCount);
        }
        #endregion
    }
}