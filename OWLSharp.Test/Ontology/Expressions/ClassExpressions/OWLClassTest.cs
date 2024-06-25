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
using OWLSharp.Ontology;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Expressions
{
    [TestClass]
    public class OWLClassTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIClass()
        {
            OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);

            Assert.IsNotNull(cls);
            Assert.IsTrue(string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(cls.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLClass(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLClass(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameClass()
        {
            OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(cls);
            Assert.IsNull(cls.IRI);
            Assert.IsTrue(string.Equals(cls.AbbreviatedIRI, new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingClassBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLClass(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIClass()
        {
            OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
            string serializedXML = OWLSerializer.SerializeObject(cls);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Class IRI=""http://xmlns.com/foaf/0.1/Person"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIClass()
        {
            OWLClass cls = OWLSerializer.DeserializeObject<OWLClass>(
@"<Class IRI=""http://xmlns.com/foaf/0.1/Person"" />");

            Assert.IsNotNull(cls);
            Assert.IsTrue(string.Equals(cls.IRI, RDFVocabulary.FOAF.PERSON.ToString()));
            Assert.IsNull(cls.AbbreviatedIRI);
            //Test stabilization of ExpressionIRI
            Assert.IsTrue(cls.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            cls.GetIRI();
            Assert.IsFalse(cls.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            Assert.IsTrue(cls.ExpressionIRI.ToString().Equals("http://xmlns.com/foaf/0.1/Person"));
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameClass()
        {
            OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLSerializer.SerializeObject(cls);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Class xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:Person"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameClass()
        {
            OWLClass cls = OWLSerializer.DeserializeObject<OWLClass>(
@"<Class xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:Person"" />");

            Assert.IsNotNull(cls);
            Assert.IsNull(cls.IRI);
            Assert.IsTrue(string.Equals(cls.AbbreviatedIRI, new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)));
            //Test stabilization of ExpressionIRI
            Assert.IsTrue(cls.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            cls.GetIRI();
            Assert.IsFalse(cls.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            Assert.IsTrue(cls.ExpressionIRI.ToString().Equals("http://xmlns.com/foaf/0.1/Person"));
        }

		[TestMethod]
        public void ShouldConvertIRIClassToGraph()
        {
            OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
			RDFGraph graph = cls.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameClassToGraph()
        {
            OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
			RDFGraph graph = cls.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.PERSON, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertIRIClassToResource()
        {
            OWLClass cls = new OWLClass(RDFVocabulary.FOAF.PERSON);
			RDFResource representative = cls.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.PERSON));
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameClassToResource()
        {
            OWLClass cls = new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI));
			RDFResource representative = cls.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.PERSON));
        }
        #endregion
    }
}