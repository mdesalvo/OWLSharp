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

using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLObjectPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIObjectProperty()
        {
            OWLObjectProperty op = new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS);

            Assert.IsNotNull(op);
            Assert.IsTrue(string.Equals(op.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(op.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectProperty(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectProperty(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameObjectProperty()
        {
            OWLObjectProperty op = new OWLObjectProperty(new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(op);
            Assert.IsNull(op.IRI);
            Assert.IsTrue(string.Equals(op.AbbreviatedIRI, new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectPropertyBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectProperty(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIObjectProperty()
        {
            OWLObjectProperty op = new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS);
            string serializedXML = OWLSerializer.SerializeObject(op);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIObjectProperty()
        {
            OWLObjectProperty op = OWLSerializer.DeserializeObject<OWLObjectProperty>(
@"<ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" />");

            Assert.IsNotNull(op);
            Assert.IsTrue(string.Equals(op.IRI, RDFVocabulary.FOAF.KNOWS.ToString()));
            Assert.IsNull(op.AbbreviatedIRI);
            //Test stabilization of ExpressionIRI
            Assert.IsTrue(op.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            op.GetIRI();
            Assert.IsFalse(op.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            Assert.IsTrue(op.ExpressionIRI.ToString().Equals("http://xmlns.com/foaf/0.1/knows"));
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameObjectProperty()
        {
            OWLObjectProperty op = new OWLObjectProperty(new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLSerializer.SerializeObject(op);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectProperty xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:knows"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameObjectProperty()
        {
            OWLObjectProperty op = OWLSerializer.DeserializeObject<OWLObjectProperty>(
@"<ObjectProperty xmlns:q1=""http://xmlns.com/foaf/0.1/"" abbreviatedIRI=""q1:knows"" />");

            Assert.IsNotNull(op);
            Assert.IsNull(op.IRI);
            Assert.IsTrue(string.Equals(op.AbbreviatedIRI, new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI)));
            //Test stabilization of ExpressionIRI
            Assert.IsTrue(op.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            op.GetIRI();
            Assert.IsFalse(op.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            Assert.IsTrue(op.ExpressionIRI.ToString().Equals("http://xmlns.com/foaf/0.1/knows"));
        }

		[TestMethod]
        public void ShouldConvertIRIObjectPropertyToGraph()
        {
            OWLObjectProperty op = new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS);
			RDFGraph graph = op.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameObjectPropertyToGraph()
        {
            OWLObjectProperty op = new OWLObjectProperty(new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI));
			RDFGraph graph = op.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.FOAF.KNOWS, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertIRIObjectPropertyToResource()
        {
            OWLObjectProperty op = new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS);
			RDFResource representative = op.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.KNOWS));
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameObjectPropertyToResource()
        {
            OWLObjectProperty op = new OWLObjectProperty(new XmlQualifiedName("knows", RDFVocabulary.FOAF.BASE_URI));
			RDFResource representative = op.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.FOAF.KNOWS));
        }
        #endregion
    }
}