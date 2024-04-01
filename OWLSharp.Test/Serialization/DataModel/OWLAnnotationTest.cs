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
        //IRI
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
@"<Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>http://example.org/seeThis</IRI>
</Annotation>"));          
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
@"<Annotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <IRI>http://example.org/seeThis</IRI>
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeIRIAnnotationWithNestedAnnotations()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/seeThis"))
            {
                Annotation = new OWLAnnotation(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("http://example.org/seeThat"))
                    {
                        Annotation = new OWLAnnotation(
                            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                            new OWLLiteral(new RDFPlainLiteral("annotation!", "en-US")))
                    }
            };
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
      <Literal xml:lang=""EN-US"">annotation!</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <IRI>http://example.org/seeThat</IRI>
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>http://example.org/seeThis</IRI>
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotation()
        {
            OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>http://example.org/seeThis</IRI>
</Annotation>");

            Assert.IsNotNull(annotation);
            Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                            && string.Equals(annProp.IRI, $"{RDFVocabulary.RDFS.COMMENT}")
                            && annProp.AbbreviatedIRI is null);
            Assert.IsTrue(string.Equals(annotation.ValueIRI, "http://example.org/seeThis"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationWithAbbreviatedProperty()
        {
            OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <IRI>http://example.org/seeThis</IRI>
</Annotation>");

            Assert.IsNotNull(annotation);
            Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                            && string.Equals(annProp.AbbreviatedIRI.ToString(), $"http://www.w3.org/2000/01/rdf-schema#:comment")
                            && annProp.IRI is null);
            Assert.IsTrue(string.Equals(annotation.ValueIRI, "http://example.org/seeThis"));
        }

        [TestMethod]
        public void ShouldDeserializeIRIAnnotationWithNestedAnnotations()
        {
            OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
      <Literal xml:lang=""EN-US"">annotation!</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <IRI>http://example.org/seeThat</IRI>
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>http://example.org/seeThis</IRI>
</Annotation>");

          Assert.IsNotNull(annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
          Assert.IsTrue(string.Equals(annotation.ValueIRI, "http://example.org/seeThis"));
          Assert.IsNotNull(annotation.Annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.Annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
          Assert.IsTrue(string.Equals(annotation.Annotation.ValueIRI, "http://example.org/seeThat"));
          Assert.IsNotNull(annotation.Annotation.Annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.Annotation.Annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#label"));
          Assert.IsTrue(string.Equals(((OWLLiteral)annotation.Annotation.Annotation.ValueLiteralExpression).Value, "annotation!"));
          Assert.IsTrue(string.Equals(((OWLLiteral)annotation.Annotation.Annotation.ValueLiteralExpression).Language, "EN-US"));
        }

        //AbbreviatedIRI

        [TestMethod]
        public void ShouldCreateAbbreviatedIRIAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("seeThis","http://example.org/"));

            Assert.IsNotNull(annotation);
            Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                            && string.Equals(annProp.IRI, $"{RDFVocabulary.RDFS.COMMENT}")
                            && annProp.AbbreviatedIRI is null);
            Assert.IsTrue(string.Equals(annotation.ValueAbbreviatedIRI, new XmlQualifiedName("seeThis","http://example.org/")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAnnotationBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(null, new XmlQualifiedName("seeThis","http://example.org/")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAnnotationBecauseNullQualifiedName()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), null as XmlQualifiedName));

        [TestMethod]
        public void ShouldSerializeAbbreviatedIRIAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("seeThis","http://example.org/"));
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AbbreviatedIRI xmlns:q1=""http://example.org/"">q1:seeThis</AbbreviatedIRI>
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeAbbreviatedIRIAnnotationWithAbbreviatedProperty()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(new XmlQualifiedName("comment", RDFVocabulary.RDFS.BASE_URI)),
                new XmlQualifiedName("seeThis","http://example.org/"));
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI);
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation, xmlSerializerNamespaces);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <AbbreviatedIRI xmlns:q1=""http://example.org/"">q1:seeThis</AbbreviatedIRI>
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeAbbreviatedIRIAnnotationWithNestedAnnotations()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("seeThis","http://example.org/"))
            {
                Annotation = new OWLAnnotation(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new XmlQualifiedName("seeThat","http://example.org/"))
                    {
                        Annotation = new OWLAnnotation(
                            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                            new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER)))
                    }
            };
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
      <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#positiveInteger"">25</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <AbbreviatedIRI xmlns:q1=""http://example.org/"">q1:seeThat</AbbreviatedIRI>
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AbbreviatedIRI xmlns:q2=""http://example.org/"">q2:seeThis</AbbreviatedIRI>
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldDeserializeAbbreviatedIRIAnnotation()
        {
          OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AbbreviatedIRI xmlns:q1=""http://example.org/"">q1:seeThis</AbbreviatedIRI>
</Annotation>");

          Assert.IsNotNull(annotation);
          Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                          && string.Equals(annProp.IRI, $"{RDFVocabulary.RDFS.COMMENT}")
                          && annProp.AbbreviatedIRI is null);
          Assert.IsTrue(string.Equals(annotation.ValueAbbreviatedIRI, new XmlQualifiedName("seeThis","http://example.org/")));
        }

        [TestMethod]
        public void ShouldDeserializeAbbreviatedIRIAnnotationWithAbbreviatedProperty()
        {
          OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <AbbreviatedIRI xmlns:q1=""http://example.org/"">q1:seeThis</AbbreviatedIRI>
</Annotation>");

          Assert.IsNotNull(annotation);
          Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                          && string.Equals(annProp.AbbreviatedIRI.ToString(), "http://www.w3.org/2000/01/rdf-schema#:comment")
                          && annProp.IRI is null);
          Assert.IsTrue(string.Equals(annotation.ValueAbbreviatedIRI, new XmlQualifiedName("seeThis","http://example.org/")));
        }

        [TestMethod]
        public void ShouldDeserializeAbbreviatedIRIAnnotationWithNestedAnnotations()
        {
          OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
      <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#positiveInteger"">25</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <AbbreviatedIRI xmlns:q1=""http://example.org/"">q1:seeThat</AbbreviatedIRI>
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AbbreviatedIRI xmlns:q2=""http://example.org/"">q2:seeThis</AbbreviatedIRI>
</Annotation>");

          Assert.IsNotNull(annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
          Assert.IsTrue(string.Equals(annotation.ValueAbbreviatedIRI.ToString(), "http://example.org/:seeThis"));
          Assert.IsNotNull(annotation.Annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.Annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
          Assert.IsTrue(string.Equals(annotation.Annotation.ValueAbbreviatedIRI.ToString(), "http://example.org/:seeThat"));
          Assert.IsNotNull(annotation.Annotation.Annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.Annotation.Annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#label"));
          Assert.IsTrue(string.Equals(((OWLLiteral)annotation.Annotation.Annotation.ValueLiteralExpression).Value, "25"));
          Assert.IsTrue(string.Equals(((OWLLiteral)annotation.Annotation.Annotation.ValueLiteralExpression).DatatypeIRI, "http://www.w3.org/2001/XMLSchema#positiveInteger"));
        }

        //AnonymousIndividual

        [TestMethod]
        public void ShouldCreateAnonymousIndividualAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"));

            Assert.IsNotNull(annotation);
            Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                            && string.Equals(annProp.IRI, $"{RDFVocabulary.RDFS.COMMENT}")
                            && annProp.AbbreviatedIRI is null);
            Assert.IsTrue(string.Equals(annotation.ValueAnonymousIndividual.NodeID, "AnonIdv"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualAnnotationBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(null, new OWLAnonymousIndividual("AnonIdv")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualAnnotationBecauseNullAnonymousIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), null as OWLAnonymousIndividual));

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"));
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualAnnotationWithAbbreviatedProperty()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(new XmlQualifiedName("comment", RDFVocabulary.RDFS.BASE_URI)),
                new OWLAnonymousIndividual("AnonIdv"));
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI);
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation, xmlSerializerNamespaces);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeAnonymousIndividualAnnotationWithNestedAnnotations()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"))
            {
                Annotation = new OWLAnnotation(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new OWLAnonymousIndividual("AnonIdv2"))
                    {
                        Annotation = new OWLAnnotation(
                            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                            new OWLLiteral(new RDFPlainLiteral("annotation!", "en-US")))
                    }
            };
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
      <Literal xml:lang=""EN-US"">annotation!</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <AnonymousIndividual nodeID=""AnonIdv2"" />
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualAnnotation()
        {
          OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</Annotation>");

          Assert.IsNotNull(annotation);
          Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                          && string.Equals(annProp.IRI, $"{RDFVocabulary.RDFS.COMMENT}")
                          && annProp.AbbreviatedIRI is null);
          Assert.IsTrue(string.Equals(annotation.ValueAnonymousIndividual.NodeID, "AnonIdv"));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualAnnotationWithAbbreviatedProperty()
        {
          OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</Annotation>");

          Assert.IsNotNull(annotation);
          Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                          && string.Equals(annProp.AbbreviatedIRI.ToString(), "http://www.w3.org/2000/01/rdf-schema#:comment")
                          && annProp.IRI is null);
          Assert.IsTrue(string.Equals(annotation.ValueAnonymousIndividual.NodeID, "AnonIdv"));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividualAnnotationWithNestedAnnotations()
        {
          OWLAnnotation annotation = OWLTestSerializer<OWLAnnotation>.Deserialize(
@"<Annotation>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
      <Literal xml:lang=""EN-US--ltr"">annotation!</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <AnonymousIndividual nodeID=""AnonIdv2"" />
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</Annotation>");

          Assert.IsNotNull(annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
          Assert.IsTrue(string.Equals(annotation.ValueAnonymousIndividual.NodeID.ToString(), "AnonIdv"));
          Assert.IsNotNull(annotation.Annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.Annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#comment"));
          Assert.IsTrue(string.Equals(annotation.Annotation.ValueAnonymousIndividual.NodeID.ToString(), "AnonIdv2"));
          Assert.IsNotNull(annotation.Annotation.Annotation);
          Assert.IsTrue(string.Equals(((OWLAnnotationProperty)annotation.Annotation.Annotation.AnnotationPropertyExpression).IRI, "http://www.w3.org/2000/01/rdf-schema#label"));
          Assert.IsTrue(string.Equals(((OWLLiteral)annotation.Annotation.Annotation.ValueLiteralExpression).Value, "annotation!"));
          Assert.IsTrue(string.Equals(((OWLLiteral)annotation.Annotation.Annotation.ValueLiteralExpression).Language, "EN-US--ltr"));
        }

        //Literal

        [TestMethod]
        public void ShouldCreateLiteralAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFPlainLiteral("Lit!")));

            Assert.IsNotNull(annotation);
            Assert.IsTrue(annotation.AnnotationPropertyExpression is OWLAnnotationProperty annProp 
                            && string.Equals(annProp.IRI, $"{RDFVocabulary.RDFS.COMMENT}")
                            && annProp.AbbreviatedIRI is null);
            Assert.IsTrue(string.Equals(((OWLLiteral)annotation.ValueLiteralExpression).Value, "Lit!"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingLiteralAnnotationBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(null, new OWLLiteral(new RDFPlainLiteral("Lit!"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingLiteralAnnotationBecauseNullLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), null as OWLLiteral));

        [TestMethod]
        public void ShouldSerializeLiteralAnnotation()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFPlainLiteral("Lit!")));
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <Literal>Lit!</Literal>
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeLiteralAnnotationWithAbbreviatedProperty()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(new XmlQualifiedName("comment", RDFVocabulary.RDFS.BASE_URI)),
                new OWLLiteral(new RDFPlainLiteral("Lit!")));
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(RDFVocabulary.RDFS.PREFIX, RDFVocabulary.RDFS.BASE_URI);
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation, xmlSerializerNamespaces);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"">
  <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
  <Literal>Lit!</Literal>
</Annotation>"));          
        }

        [TestMethod]
        public void ShouldSerializeLiteralAnnotationWithNestedAnnotations()
        {
            OWLAnnotation annotation = new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFPlainLiteral("Lit!")))
            {
                Annotation = new OWLAnnotation(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new OWLLiteral(new RDFPlainLiteral("Lit!2")))
                    {
                        Annotation = new OWLAnnotation(
                            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                            new OWLLiteral(new RDFPlainLiteral("annotation!", "en-US")))
                    }
            };
            string serializedXML = OWLTestSerializer<OWLAnnotation>.Serialize(annotation);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Annotation>
  <Annotation>
    <Annotation>
      <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" />
      <Literal xml:lang=""EN-US"">annotation!</Literal>
    </Annotation>
    <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
    <Literal>Lit!2</Literal>
  </Annotation>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <Literal>Lit!</Literal>
</Annotation>"));          
        }
        #endregion
    }
}