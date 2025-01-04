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
using System.Xml;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLAnnotationPropertyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR);

            Assert.IsNotNull(annotation);
            Assert.IsTrue(string.Equals(annotation.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNull(annotation.AbbreviatedIRI);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationPropertyBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationProperty(null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationPropertyBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationProperty(new RDFResource()));

        [TestMethod]
        public void ShouldCreateQualifiedNameAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI));

            Assert.IsNotNull(annotation);
            Assert.IsNull(annotation.IRI);
            Assert.IsTrue(string.Equals(annotation.AbbreviatedIRI, new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationPropertyBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationProperty(null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeIRIAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR);
            string serializedXML = OWLSerializer.SerializeObject(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationProperty()
        {
            OWLAnnotationProperty annotation = OWLSerializer.DeserializeObject<OWLAnnotationProperty>(
@"<AnnotationProperty IRI=""http://purl.org/dc/elements/1.1/creator"" />");

            Assert.IsNotNull(annotation);
            Assert.IsTrue(string.Equals(annotation.IRI, RDFVocabulary.DC.CREATOR.ToString()));
            Assert.IsNull(annotation.AbbreviatedIRI);
            //Test stabilization of ExpressionIRI
            Assert.IsTrue(annotation.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            annotation.GetIRI();
            Assert.IsFalse(annotation.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            Assert.IsTrue(annotation.ExpressionIRI.ToString().Equals("http://purl.org/dc/elements/1.1/creator"));
        }

        [TestMethod]
        public void ShouldSerializeQualifiedNameAnnotationProperty()
        {
            OWLAnnotationProperty annotation = new OWLAnnotationProperty(new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI));
            string serializedXML = OWLSerializer.SerializeObject(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationProperty xmlns:q1=""http://purl.org/dc/elements/1.1/"" abbreviatedIRI=""q1:creator"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeQualifiedNameAnnotationProperty()
        {
            OWLAnnotationProperty annotation = OWLSerializer.DeserializeObject<OWLAnnotationProperty>(
@"<AnnotationProperty xmlns:q1=""http://purl.org/dc/elements/1.1/"" abbreviatedIRI=""q1:creator"" />");

            Assert.IsNotNull(annotation);
            Assert.IsNull(annotation.IRI);
            Assert.IsTrue(string.Equals(annotation.AbbreviatedIRI, new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI)));
            //Test stabilization of ExpressionIRI
            Assert.IsTrue(annotation.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            annotation.GetIRI();
            Assert.IsFalse(annotation.ExpressionIRI.ToString().StartsWith("bnode:ex"));
            Assert.IsTrue(annotation.ExpressionIRI.ToString().Equals("http://purl.org/dc/elements/1.1/creator"));
        }

		[TestMethod]
        public void ShouldConvertIRIAnnotationPropertyToGraph()
        {
            OWLAnnotationProperty ann = new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR);
			RDFGraph graph = ann.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.DC.CREATOR, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameAnnotationPropertyToGraph()
        {
            OWLAnnotationProperty ann = new OWLAnnotationProperty(new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI));
			RDFGraph graph = ann.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 1);
			Assert.IsTrue(graph[RDFVocabulary.DC.CREATOR, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null].TriplesCount == 1);
        }

		[TestMethod]
        public void ShouldConvertIRIAnnotationPropertyToResource()
        {
            OWLAnnotationProperty ann = new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR);
			RDFResource representative = ann.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.DC.CREATOR));
        }

		[TestMethod]
        public void ShouldConvertQualifiedNameAnnotationPropertyToResource()
        {
            OWLAnnotationProperty ann = new OWLAnnotationProperty(new XmlQualifiedName("creator", RDFVocabulary.DC.BASE_URI));
			RDFResource representative = ann.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(RDFVocabulary.DC.CREATOR));
        }
        #endregion
    }
}