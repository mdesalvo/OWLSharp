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

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLAnnotationAxiomHelperTest
{
    #region Tests
    [TestMethod]
    public void ShouldGetAnnotationAxioms()
    {
        OWLOntology ontology = new OWLOntology
        {
            AnnotationAxioms = [
                new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new RDFResource("ex:Subj"), new RDFResource("ex:Obj")),
                new OWLAnnotationPropertyDomain(new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT), RDFVocabulary.FOAF.PERSON),
                new OWLAnnotationPropertyRange(new OWLAnnotationProperty(RDFVocabulary.FOAF.AGENT), RDFVocabulary.FOAF.PERSON),
                new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLAnnotationProperty(RDFVocabulary.DC.TITLE))
            ]
        };

        List<OWLAnnotationAssertion> annotationAssertion = ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>();
        Assert.AreEqual(1, annotationAssertion.Count);

        List<OWLAnnotationPropertyDomain> annotationPropertyDomain = ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyDomain>();
        Assert.AreEqual(1, annotationPropertyDomain.Count);

        List<OWLAnnotationPropertyRange> annotationPropertyRange = ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyRange>();
        Assert.AreEqual(1, annotationPropertyRange.Count);

        List<OWLSubAnnotationPropertyOf> subAnnotationProperty = ontology.GetAnnotationAxiomsOfType<OWLSubAnnotationPropertyOf>();
        Assert.AreEqual(1, subAnnotationProperty.Count);

        Assert.AreEqual(0, (null as OWLOntology).GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count);
    }

    [TestMethod]
    public void ShouldGetSubAnnotationPropertiesOf()
    {
        OWLOntology ontology = new OWLOntology
        {
            AnnotationAxioms = [
                new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))),
                new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))),
                new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp3")))
            ]
        };

        List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp1 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")));
        Assert.AreEqual(3, subAnnotationPropertiesOfAnp1.Count);
        Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
        Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
        Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));

        List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp2 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")));
        Assert.AreEqual(2, subAnnotationPropertiesOfAnp2.Count);
        Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));
        Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));

        List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp3 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")));
        Assert.AreEqual(1, subAnnotationPropertiesOfAnp3.Count);
        Assert.IsTrue(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))));

        List<OWLAnnotationProperty> subAnnotationPropertiesOfAnp4 = ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")));
        Assert.AreEqual(0, subAnnotationPropertiesOfAnp4.Count);

        Assert.AreEqual(0, ontology.GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp5"))).Count);
        Assert.AreEqual(0, ontology.GetSubAnnotationPropertiesOf(null).Count);
        Assert.IsFalse(ontology.CheckIsSubAnnotationPropertyOf(null, new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));
        Assert.IsFalse(ontology.CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), null));
        Assert.IsFalse((null as OWLOntology).CheckIsSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
        Assert.AreEqual(0, (null as OWLOntology).GetSubAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1"))).Count);
    }

    [TestMethod]
    public void ShouldGetSuperAnnotationPropertiesOf()
    {
        OWLOntology ontology = new OWLOntology
        {
            AnnotationAxioms = [
                new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))),
                new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))),
                new OWLSubAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")), new OWLAnnotationProperty(new RDFResource("ex:Anp3")))
            ]
        };

        List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp1 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")));
        Assert.AreEqual(0, superAnnotationPropertiesOfAnp1.Count);

        List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp2 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")));
        Assert.AreEqual(1, superAnnotationPropertiesOfAnp2.Count);
        Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")), new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));

        List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp3 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")));
        Assert.AreEqual(2, superAnnotationPropertiesOfAnp3.Count);
        Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))));
        Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp3"))));

        List<OWLAnnotationProperty> superAnnotationPropertiesOfAnp4 = ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp4")));
        Assert.AreEqual(3, superAnnotationPropertiesOfAnp4.Count);
        Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1")), new OWLAnnotationProperty(new RDFResource("ex:Anp4"))));
        Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp4"))));
        Assert.IsTrue(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp3")), new OWLAnnotationProperty(new RDFResource("ex:Anp4"))));

        Assert.AreEqual(0, ontology.GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp5"))).Count);
        Assert.AreEqual(0, ontology.GetSuperAnnotationPropertiesOf(null).Count);
        Assert.IsFalse(ontology.CheckIsSuperAnnotationPropertyOf(null, new OWLAnnotationProperty(new RDFResource("ex:Anp2"))));
        Assert.IsFalse(ontology.CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), null));
        Assert.IsFalse((null as OWLOntology).CheckIsSuperAnnotationPropertyOf(new OWLAnnotationProperty(new RDFResource("ex:Anp2")), new OWLAnnotationProperty(new RDFResource("ex:Anp1"))));
        Assert.AreEqual(0, (null as OWLOntology).GetSuperAnnotationPropertiesOf(new OWLAnnotationProperty(new RDFResource("ex:Anp1"))).Count);
    }

    [TestMethod]
    public void ShouldDeclareAnnotationAxiom()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.DeclareAnnotationAxiom(new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
            RDFVocabulary.FOAF.PERSON,
            new OWLLiteral(new RDFPlainLiteral("Person", "en-US"))));

        Assert.AreEqual(1, ontology.AnnotationAxioms.Count);
        Assert.IsTrue(ontology.CheckHasAnnotationAxiom(new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
            RDFVocabulary.FOAF.PERSON,
            new OWLLiteral(new RDFPlainLiteral("Person", "en-US")))));
        Assert.ThrowsExactly<OWLException>(() => ontology.DeclareAnnotationAxiom<OWLAnnotationAxiom>(null));

        ontology.DeclareAnnotationAxiom(new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
            RDFVocabulary.FOAF.PERSON,
            new OWLLiteral(new RDFPlainLiteral("Person", "en-US")))); //will be discarded, since duplicates are not allowed
        Assert.AreEqual(1, ontology.AnnotationAxioms.Count);
    }
    #endregion
}