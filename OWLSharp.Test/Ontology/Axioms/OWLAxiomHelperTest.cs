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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLAxiomHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAnnotate()
        {
            OWLSubClassOf axiom = new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT));
            axiom.Annotate(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFPlainLiteral("foaf:Person isA foaf:Agent"))));
            (null as OWLAxiom).Annotate(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFPlainLiteral("Since the axiom is null, this annotation will be discarded"))));

            Assert.AreEqual(1, axiom.Annotations.Count);
            Assert.ThrowsExactly<OWLException>(() => axiom.Annotate(null));
        }

        [TestMethod]
        public void ShouldRemoveDuplicates()
        {
            List<OWLAxiom> axioms = [
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("ex:Subj"),
                    new RDFResource("ex:Obj")),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.AGENT)),
                new OWLSubClassOf(
                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                    new OWLClass(RDFVocabulary.FOAF.AGENT)),
                new OWLEquivalentClasses(
                    [ new OWLClass(RDFVocabulary.FOAF.AGENT),
                      new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                      new OWLClass(RDFVocabulary.FOAF.PERSON)
                    ]),
                new OWLEquivalentClasses(
                    [ new OWLClass(RDFVocabulary.FOAF.AGENT),
                      new OWLClass(RDFVocabulary.FOAF.ORGANIZATION),
                      new OWLClass(RDFVocabulary.FOAF.PERSON)
                    ]),
                new OWLDatatypeDefinition(
                    new OWLDatatype(new RDFResource("ex:length6to10")),
                    new OWLDatatypeRestriction(
                        new OWLDatatype(RDFVocabulary.XSD.STRING),
                        [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                         new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)])),
                new OWLFunctionalObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLFunctionalObjectProperty(
                    new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("ex:Subj"),
                    new RDFResource("ex:Obj"))
            ];

            Assert.AreEqual(5, OWLAxiomHelper.RemoveDuplicates(axioms).Count);
            Assert.AreEqual(0, OWLAxiomHelper.RemoveDuplicates(new List<OWLAxiom>()).Count);
            Assert.AreEqual(0, OWLAxiomHelper.RemoveDuplicates<OWLAxiom>(null).Count);
        }
        #endregion
    }
}