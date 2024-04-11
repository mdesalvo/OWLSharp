﻿/*
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
using OWLSharp.Test;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLAnnotationAssertionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateAnnotationIRIAssertion()
        {
            OWLAnnotationIRIAssertion annotationAssertion = new OWLAnnotationIRIAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNotNull(annotationAssertion.ValueIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueIRI, "ex:Obj"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationIRIAssertionBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationIRIAssertion(
                null,
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationIRIAssertionBecauseNullSubjectIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationIRIAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null,
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationIRIAssertionBecauseNullValueIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationIRIAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                null));

        [TestMethod]
        public void ShouldSerializeAnnotationIRIAssertion()
        {
            OWLAnnotationIRIAssertion annotationAssertion = new OWLAnnotationIRIAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj"));
            string serializedXML = OWLTestSerializer<OWLAnnotationIRIAssertion>.Serialize(annotationAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationAssertion>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>ex:Subj</IRI>
  <IRI>ex:Obj</IRI>
</AnnotationAssertion>"));
        }

        [TestMethod]
        public void ShouldDeserializeAnnotationIRIAssertion()
        {
            OWLAnnotationIRIAssertion annotationAssertion = OWLTestSerializer<OWLAnnotationIRIAssertion>.Deserialize(
@"<AnnotationAssertion>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>ex:Subj</IRI>
  <IRI>ex:Obj</IRI>
</AnnotationAssertion>");

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNotNull(annotationAssertion.ValueIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueIRI, "ex:Obj"));
        }

        [TestMethod]
        public void ShouldCreateAnnotationLiteralAssertion()
        {
            OWLAnnotationLiteralAssertion annotationAssertion = new OWLAnnotationLiteralAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new OWLLiteral(new RDFPlainLiteral("hello", "en")));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNotNull(annotationAssertion.ValueLiteral);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Value, "hello"));
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Language, "EN"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationLiteralAssertionBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationLiteralAssertion(
                null,
                new RDFResource("ex:Subj"),
                new OWLLiteral(new RDFPlainLiteral("hello", "en"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationLiteralAssertionBecauseNullSubjectIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationLiteralAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null,
                new OWLLiteral(new RDFPlainLiteral("hello", "en"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnnotationLiteralAssertionBecauseNullValueLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationLiteralAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                null));

        [TestMethod]
        public void ShouldSerializeAnnotationLiteralAssertion()
        {
            OWLAnnotationLiteralAssertion annotationAssertion = new OWLAnnotationLiteralAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new OWLLiteral(new RDFPlainLiteral("hello", "en")));
            string serializedXML = OWLTestSerializer<OWLAnnotationLiteralAssertion>.Serialize(annotationAssertion);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnnotationAssertion>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>ex:Subj</IRI>
  <Literal xml:lang=""EN"">hello</Literal>
</AnnotationAssertion>"));
        }

        [TestMethod]
        public void ShouldDeserializeAnnotationLiteralAssertion()
        {
            OWLAnnotationLiteralAssertion annotationAssertion = OWLTestSerializer<OWLAnnotationLiteralAssertion>.Deserialize(
@"<AnnotationAssertion>
  <AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" />
  <IRI>ex:Subj</IRI>
  <Literal xml:lang=""EN"">hello</Literal>
</AnnotationAssertion>");

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNotNull(annotationAssertion.ValueLiteral);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Value, "hello"));
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Language, "EN"));
        }
        #endregion
    }
}