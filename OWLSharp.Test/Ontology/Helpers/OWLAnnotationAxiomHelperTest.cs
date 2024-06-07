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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using RDFSharp.Model;


namespace OWLSharp.Test.Ontology.Helpers
{
    [TestClass]
    public class OWLAnnotationAxiomHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldGetAnnotationAxioms()
		{
			OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:Subj"), new RDFResource("ex:Obj")),
					new OWLAnnotationPropertyDomain(new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT), RDFVocabulary.FOAF.PERSON),
					new OWLAnnotationPropertyRange(new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT), RDFVocabulary.FOAF.PERSON),
					new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLAnnotationProperty(RDFVocabulary.DC.TITLE))
                ]
            };

            List<OWLAnnotationAssertion> annotationAssertion = ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>();
            Assert.IsTrue(annotationAssertion.Count == 1);

			List<OWLAnnotationPropertyDomain> annotationPropertyDomain = ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyDomain>();
            Assert.IsTrue(annotationPropertyDomain.Count == 1);

			List<OWLAnnotationPropertyRange> annotationPropertyRange = ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyRange>();
            Assert.IsTrue(annotationPropertyRange.Count == 1);

			List<OWLSubAnnotationPropertyOf> subAnnotationProperty = ontology.GetAnnotationAxiomsOfType<OWLSubAnnotationPropertyOf>();
            Assert.IsTrue(subAnnotationProperty.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count == 0);
        }
		#endregion
	}
}