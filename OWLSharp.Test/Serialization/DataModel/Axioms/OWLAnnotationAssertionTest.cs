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
    public class OWLAnnotationAssertionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIRIIRIAssertion()
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
        public void ShouldThrowExceptionOnCreatingIRIRIAnnotationBecauseNullAnnotationProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                null,
                new RDFResource("ex:Subj"),
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIRIAnnotationBecauseNullSubjectIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                null as RDFResource,
                new RDFResource("ex:Obj")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIRIRIAnnotationBecauseNullValueIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("ex:Subj"),
                null as RDFResource));
        #endregion
    }
}