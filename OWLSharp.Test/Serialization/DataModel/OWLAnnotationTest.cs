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
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLAnnotationTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/seeThis"));

            Assert.IsNotNull(annotation);
            Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                            && string.Equals(annProp.IRI, $"{RDFVocabulary.RDFS.COMMENT}")
                            && annProp.AbbreviatedIRI is null);
            Assert.IsTrue(string.Equals(annotation.ValueIRI, "http://example.org/seeThis"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAnnotationBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(null, new RDFResource("http://example.org/seeThis")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAnnotationBecauseNullIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), null as RDFResource));

        [TestMethod]
        public void ShouldSerializeIRIAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/seeThis"));
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<OWLAnnotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>http://example.org/seeThis</IRI>
</OWLAnnotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeIRIAnnotationWithAbbreviatedProperty()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(new XmlQualifiedName("comment", RDFVocabulary.RDFS.BASE_URI)),
                new RDFResource("http://example.org/seeThis"));
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI);
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation, xmlSerializerNamespaces);

            Assert.IsTrue(string.Equals(serializedXML,
@"<OWLAnnotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <IRI>http://example.org/seeThis</IRI>
</OWLAnnotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeIRIAnnotationWithAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/seeThis"))
            {
                Annotation = new OWLAnnotation(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("http://example.org/seeThat"))
            };
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<OWLAnnotation>
  <Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <IRI>http://example.org/seeThat</IRI>
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>http://example.org/seeThis</IRI>
</OWLAnnotation>"));          
        }
        #endregion
    }
}