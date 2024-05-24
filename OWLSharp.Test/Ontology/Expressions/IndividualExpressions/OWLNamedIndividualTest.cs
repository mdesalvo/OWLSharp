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

using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Expressions
{
    [TestClass]
    public class OWLNamedIndividualTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRINamedIndividual()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);

            Assert.IsNotNull(idv);
            Assert.IsTrue(string.Equals(idv.IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsNull(idv.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLNamedIndividual(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLNamedIndividual(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameNamedIndividual()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(idv);
            Assert.IsNull(idv.IRI);
            Assert.IsTrue(string.Equals(idv.AbbreviatedIRI, new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNamedIndividualBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLNamedIndividual(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRINamedIndividual()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);
            string serializedXML = OWLTestSerializer<OWLNamedIndividual>.Serialize(idv);

            Assert.IsTrue(string.Equals(serializedXML,
@"<NamedIndividual IRI=""http://xmlns.com/foaf/0.1/age"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRINamedIndividual()
        {
            OWLNamedIndividual idv = OWLTestSerializer<OWLNamedIndividual>.Deserialize(
@"<NamedIndividual IRI=""http://xmlns.com/foaf/0.1/age"" />");

            Assert.IsNotNull(idv);
            Assert.IsTrue(string.Equals(idv.IRI, RDFVocabulary.FOAF.AGE.ToString()));
            Assert.IsNull(idv.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameNamedIndividual()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLTestSerializer<OWLNamedIndividual>.Serialize(idv);

            Assert.IsTrue(string.Equals(serializedXML,
@"<NamedIndividual xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:age"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameNamedIndividual()
        {
            OWLNamedIndividual idv = OWLTestSerializer<OWLNamedIndividual>.Deserialize(
@"<NamedIndividual xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:age"" />");

            Assert.IsNotNull(idv);
            Assert.IsNull(idv.IRI);
            Assert.IsTrue(string.Equals(idv.AbbreviatedIRI, new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI)));
        }

		[TestMethod]
        public void ShouldConvertIRINamedIndividualToGraph()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);
			RDFGraph graph = idv.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameNamedIndividualToGraph()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));
			RDFGraph graph = idv.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.AGE, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertIRINamedIndividualToResource()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(RDFVocabulary.FOAF.AGE);
			RDFResource representative = idv.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.AGE));
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameNamedIndividualToResource()
        {
            OWLNamedIndividual idv = new OWLNamedIndividual(new XmlQualifiedName("age", RDFVocabulary.FOAF.BASE_URI));
			RDFResource representative = idv.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.AGE));
        }
        #endregion
    }
}