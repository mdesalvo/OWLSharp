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

namespace OWLSharp.Serialization.Test
{
    [TestClass]
    public class OWLAnnotationAssertionTest
    {
        #region Tests
        //SubjectIRI
        [TestMethod]
        public void ShouldCreateIRIIRIAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNotNull(annotationAssertion.ValueIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueIRI, "ex:Obj"));
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIRIAnnotationAssertionBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                null,
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIRIAnnotationAssertionBecauseNullSubjectIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as RDFResource,
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIRIAnnotationAssertionBecauseNullValueIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                null as RDFResource));

        [TestMethod]
        public void ShouldCreateIRIAbbreviatedIRIAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new XmlQualifiedName("Obj", "http://example.org/"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNotNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueAbbreviatedIRI.ToString(), "http://example.org/:Obj"));
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAbbreviatedIRIAnnotationAssertionBecauseNullValueAbbreviatedIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                null as XmlQualifiedName));

        [TestMethod]
        public void ShouldCreateIRIAnonymousIndividualAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new OWLAnonymousIndividual("AnonIdv"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNotNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueAnonymousIndividual.NodeID, "AnonIdv"));
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIAnonymousIndividualAnnotationAssertionBecauseNullValueAnonymousIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                null as OWLAnonymousIndividual));

        [TestMethod]
        public void ShouldCreateIRILiteralAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                new OWLLiteral(new RDFPlainLiteral("hello","en-US")));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNotNull(annotationAssertion.SubjectIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectIRI, "ex:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNotNull(annotationAssertion.ValueLiteral);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Value, "hello"));
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Language, "EN-US"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRILiteralAnnotationAssertionBecauseNullValueLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                null as OWLLiteral));

        //SubjectAbbreviatedIRI
        [TestMethod]
        public void ShouldCreateAbbreviatedIRIIRIAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                new RDFResource("ex:Obj"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAbbreviatedIRI.ToString(), "http://example.org/:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNotNull(annotationAssertion.ValueIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueIRI, "ex:Obj"));
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIRIAnnotationAssertionBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                null,
                new XmlQualifiedName("Subj", "http://example.org/"),
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIRIAnnotationAssertionBecauseNullSubjectIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as XmlQualifiedName,
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIRIAnnotationAssertionBecauseNullValueIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                null as RDFResource));

        [TestMethod]
        public void ShouldCreateAbbreviatedIRIAbbreviatedIRIAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                new XmlQualifiedName("Obj", "http://example.org/"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAbbreviatedIRI.ToString(), "http://example.org/:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNotNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueAbbreviatedIRI.ToString(), "http://example.org/:Obj"));
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAbbreviatedIRIAnnotationAssertionBecauseNullValueAbbreviatedIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                null as XmlQualifiedName));

        [TestMethod]
        public void ShouldCreateAbbreviatedIRIAnonymousIndividualAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                new OWLAnonymousIndividual("AnonIdv"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAbbreviatedIRI.ToString(), "http://example.org/:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNotNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueAnonymousIndividual.NodeID, "AnonIdv"));
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRIAnonymousIndividualAnnotationAssertionBecauseNullValueAnonymousIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                null as OWLAnonymousIndividual));

        [TestMethod]
        public void ShouldCreateAbbreviatedIRILiteralAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                new OWLLiteral(new RDFPlainLiteral("hello", "en-US")));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAbbreviatedIRI.ToString(), "http://example.org/:Subj"));
            Assert.IsNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNotNull(annotationAssertion.ValueLiteral);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Value, "hello"));
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Language, "EN-US"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAbbreviatedIRILiteralAnnotationAssertionBecauseNullValueLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Subj", "http://example.org/"),
                null as OWLLiteral));

        //SubjectAnonymousIndividual
        [TestMethod]
        public void ShouldCreateAnonymousIndividualIRIAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new RDFResource("ex:Obj"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAnonymousIndividual.NodeID, "AnonIdv"));
            Assert.IsNotNull(annotationAssertion.ValueIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueIRI, "ex:Obj"));
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualRIAnnotationAssertionBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                null,
                new OWLAnonymousIndividual("AnonIdv"),
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualRIAnnotationAssertionBecauseNullSubjectIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as XmlQualifiedName,
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualRIAnnotationAssertionBecauseNullValueIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                null as RDFResource));

        [TestMethod]
        public void ShouldCreateAnonymousIndividualAbbreviatedIRIAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new XmlQualifiedName("Obj", "http://example.org/"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAnonymousIndividual.NodeID, "AnonIdv"));
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNotNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueAbbreviatedIRI.ToString(), "http://example.org/:Obj"));
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualAbbreviatedIRIAnnotationAssertionBecauseNullValueAbbreviatedIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                null as XmlQualifiedName));

        [TestMethod]
        public void ShouldCreateAnonymousIndividualAnonymousIndividualAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new OWLAnonymousIndividual("AnonIdv"));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAnonymousIndividual.NodeID, "AnonIdv"));
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNotNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueAnonymousIndividual.NodeID, "AnonIdv"));
            Assert.IsNull(annotationAssertion.ValueLiteral);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualAnonymousIndividualAnnotationAssertionBecauseNullValueAnonymousIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                null as OWLAnonymousIndividual));

        [TestMethod]
        public void ShouldCreateAnonymousIndividualLiteralAnnotationAssertion()
        {
            OWLAnnotationAssertion annotationAssertion = new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new OWLLiteral(new RDFPlainLiteral("hello", "en-US")));

            Assert.IsNotNull(annotationAssertion);
            Assert.IsNotNull(annotationAssertion.AnnotationProperty);
            Assert.IsTrue(string.Equals(annotationAssertion.AnnotationProperty.IRI, RDFVocabulary.RDFS.COMMENT.ToString()));
            Assert.IsNull(annotationAssertion.SubjectIRI);
            Assert.IsNull(annotationAssertion.SubjectAbbreviatedIRI);
            Assert.IsNotNull(annotationAssertion.SubjectAnonymousIndividual);
            Assert.IsTrue(string.Equals(annotationAssertion.SubjectAnonymousIndividual.NodeID, "AnonIdv"));
            Assert.IsNull(annotationAssertion.ValueIRI);
            Assert.IsNull(annotationAssertion.ValueAbbreviatedIRI);
            Assert.IsNull(annotationAssertion.ValueAnonymousIndividual);
            Assert.IsNotNull(annotationAssertion.ValueLiteral);
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Value, "hello"));
            Assert.IsTrue(string.Equals(annotationAssertion.ValueLiteral.Language, "EN-US"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualLiteralAnnotationAssertionBecauseNullValueLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                null as OWLLiteral));
        #endregion
    }
}