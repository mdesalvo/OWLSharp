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
using RDFSharp.Model;


namespace OWLSharp.Test.Ontology
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
		
		[TestMethod]
        public void ShouldGetSubAnnotationPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))),
                    new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))),
                    new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))),
                ]
            };

            List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp1 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")));
            Assert.IsTrue(subAnnotationPropertiesOfAnp1.Count == 3);
            Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
            Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
            Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));

            List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp2 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")));
            Assert.IsTrue(subAnnotationPropertiesOfAnp2.Count == 2);
            Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));
            Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));

            List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp3 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")));
            Assert.IsTrue(subAnnotationPropertiesOfAnp3.Count == 1);
            Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))));

            List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp4 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")));
            Assert.IsTrue(subAnnotationPropertiesOfAnp4.Count == 0);

            Assert.IsTrue(ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSubAnnotationPropertiesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSubAnnotationPropertyOf(null, new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));
            Assert.IsFalse(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
            Assert.IsTrue((null as OWLOntology).GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1"))).Count == 0);
        }
		
		[TestMethod]
        public void ShouldGetSuperAnnotationPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                AnnotationAxioms = [
                    new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))),
                    new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))),
                    new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))),
                ]
            };

            List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp1 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")));
            Assert.IsTrue(superAnnotationPropertiesOfAnp1.Count == 0);

            List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp2 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")));
            Assert.IsTrue(superAnnotationPropertiesOfAnp2.Count == 1);
            Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));

            List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp3 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")));
            Assert.IsTrue(superAnnotationPropertiesOfAnp3.Count == 2);
            Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))));
			Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))));

            List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp4 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")));
            Assert.IsTrue(superAnnotationPropertiesOfAnp4.Count == 3);
			Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")), new OWLAnnotationProperty(new RDFResource("ex:Anp4"))));
			Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp4"))));
			Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp4"))));

            Assert.IsTrue(ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSuperAnnotationPropertiesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSuperAnnotationPropertyOf(null, new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));
            Assert.IsFalse(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
            Assert.IsTrue((null as OWLOntology).GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1"))).Count == 0);
        }

        [TestMethod]
        public void ShouldDeclareAnnotationAxiom()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.AddAnnotationAxiom(new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                RDFVocabulary.FOAF.PERSON,
                new OWLLiteral(new RDFPlainLiteral("Person", "en-US"))));

            Assert.IsTrue(ontology.AnnotationAxioms.Count == 1);
            Assert.IsTrue(ontology.CheckHasAnnotationAxiom(new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                RDFVocabulary.FOAF.PERSON,
                new OWLLiteral(new RDFPlainLiteral("Person", "en-US")))));
            Assert.ThrowsException<OWLException>(() => ontology.AddAnnotationAxiom(null as OWLAnnotationAxiom));

            ontology.AddAnnotationAxiom(new OWLAnnotationAssertion(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                RDFVocabulary.FOAF.PERSON,
                new OWLLiteral(new RDFPlainLiteral("Person", "en-US")))); //will be discarded, since duplicates are not allowed
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 1);
        }
        #endregion
	}
}