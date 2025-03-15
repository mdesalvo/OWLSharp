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
public class OWLDataPropertyAxiomHelperTest
{
    #region Tests
    [TestMethod]
    public void ShouldGetDataPropertyAxioms()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(RDFVocabulary.DC.DCTERMS.TITLE), new OWLDataProperty(RDFVocabulary.DC.TITLE)),
                new OWLFunctionalDataProperty(new OWLDataProperty(RDFVocabulary.RDFS.LABEL)),
                new OWLEquivalentDataProperties([ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]),
                new OWLDisjointDataProperties([ new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLDataProperty(RDFVocabulary.FOAF.NAME), new OWLDataProperty(RDFVocabulary.FOAF.TITLE) ]),
                new OWLDataPropertyRange(new OWLDataProperty(RDFVocabulary.RDFS.COMMENT), new OWLDatatype(RDFVocabulary.XSD.STRING)),
                new OWLDataPropertyDomain(new OWLDataProperty(RDFVocabulary.RDFS.COMMENT), new OWLClass(RDFVocabulary.FOAF.PERSON))
            ]
        };

        List<OWLSubDataPropertyOf> subDataPropertyOf = ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>();
        Assert.AreEqual(1, subDataPropertyOf.Count);

        List<OWLFunctionalDataProperty> functionalDataProperty = ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>();
        Assert.AreEqual(1, functionalDataProperty.Count);

        List<OWLEquivalentDataProperties> equivalentDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>();
        Assert.AreEqual(1, equivalentDataProperties.Count);

        List<OWLDisjointDataProperties> disjointDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>();
        Assert.AreEqual(1, disjointDataProperties.Count);

        List<OWLDataPropertyRange> dataPropertyRange = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>();
        Assert.AreEqual(1, dataPropertyRange.Count);

        List<OWLDataPropertyDomain> dataPropertyDomain = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>();
        Assert.AreEqual(1, dataPropertyDomain.Count);

        Assert.AreEqual(0, (null as OWLOntology).GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Count);
    }

    [TestMethod]
    public void ShouldDeclareDataPropertyAxiom()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.DeclareDataPropertyAxiom(new OWLSubDataPropertyOf(
            new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
            new OWLDataProperty(RDFVocabulary.FOAF.TITLE)));

        Assert.AreEqual(1, ontology.DataPropertyAxioms.Count);
        Assert.IsTrue(ontology.CheckHasDataPropertyAxiom(new OWLSubDataPropertyOf(
            new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
            new OWLDataProperty(RDFVocabulary.FOAF.TITLE))));
        Assert.ThrowsExactly<OWLException>(() => ontology.DeclareDataPropertyAxiom<OWLDataPropertyAxiom>(null));

        ontology.DeclareDataPropertyAxiom(new OWLSubDataPropertyOf(
            new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
            new OWLDataProperty(RDFVocabulary.FOAF.TITLE))); //will be discarded, since duplicates are not allowed
        Assert.AreEqual(1, ontology.DataPropertyAxioms.Count);
    }

    [TestMethod]
    public void ShouldGetSubDataPropertiesOf()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3")))
            ]
        };

        List<OWLDataProperty> subDataPropertiesOfDtp1 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1")));
        Assert.AreEqual(3, subDataPropertiesOfDtp1.Count);
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));

        List<OWLDataProperty> subDataPropertiesOfDtp2 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.AreEqual(2, subDataPropertiesOfDtp2.Count);
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));

        List<OWLDataProperty> subDataPropertiesOfDtp3 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.AreEqual(1, subDataPropertiesOfDtp3.Count);
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));

        List<OWLDataProperty> subDataPropertiesOfDtp4 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.AreEqual(0, subDataPropertiesOfDtp4.Count);

        Assert.AreEqual(0, ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5"))).Count);
        Assert.AreEqual(0, ontology.GetSubDataPropertiesOf(null).Count);
        Assert.IsFalse(ontology.CheckIsSubDataPropertyOf(null, new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsFalse(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), null));
        Assert.IsFalse((null as OWLOntology).CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.AreEqual(0, (null as OWLOntology).GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count);
    }

    [TestMethod]
    public void ShouldGetSubDataPropertiesOfWithEquivalentDataPropertiesDiscovery()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3"))),
                new OWLEquivalentDataProperties([new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp5"))]),
                new OWLEquivalentDataProperties([new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp6"))])
            ]
        };

        List<OWLDataProperty> subDataPropertiesOfDtp1 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1")));
        Assert.AreEqual(5, subDataPropertiesOfDtp1.Count);

        List<OWLDataProperty> subDataPropertiesOfDtp2 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.AreEqual(4, subDataPropertiesOfDtp2.Count);

        List<OWLDataProperty> subDataPropertiesOfDtp3 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.AreEqual(2, subDataPropertiesOfDtp3.Count);

        List<OWLDataProperty> subDataPropertiesOfDtp4 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.AreEqual(0, subDataPropertiesOfDtp4.Count);

        List<OWLDataProperty> subDataPropertiesOfDtp5 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5")));
        Assert.AreEqual(2, subDataPropertiesOfDtp5.Count);

        Assert.AreEqual(0, ontology.GetSubDataPropertiesOf(null).Count);
        Assert.AreEqual(0, (null as OWLOntology).GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count);
    }

    [TestMethod]
    public void ShouldGetSuperDataPropertiesOf()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3")))
            ]
        };

        List<OWLDataProperty> superDataPropertiesOfDtp1 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1")));
        Assert.AreEqual(0, superDataPropertiesOfDtp1.Count);

        List<OWLDataProperty> superDataPropertiesOfDtp2 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.AreEqual(1, superDataPropertiesOfDtp2.Count);
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));

        List<OWLDataProperty> superDataPropertiesOfDtp3 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.AreEqual(2, superDataPropertiesOfDtp3.Count);
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));

        List<OWLDataProperty> superDataPropertiesOfDtp4 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.AreEqual(3, superDataPropertiesOfDtp4.Count);
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));

        Assert.AreEqual(0, ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5"))).Count);
        Assert.AreEqual(0, ontology.GetSuperDataPropertiesOf(null).Count);
        Assert.IsFalse(ontology.CheckIsSuperDataPropertyOf(null, new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsFalse(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), null));
        Assert.IsFalse((null as OWLOntology).CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.AreEqual(0, (null as OWLOntology).GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count);
    }

    [TestMethod]
    public void ShouldGetSuperDataPropertiesOfWithEquivalentDataPropertiesDiscovery()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3"))),
                new OWLEquivalentDataProperties([new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp5"))]),
                new OWLEquivalentDataProperties([new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp6"))])
            ]
        };

        List<OWLDataProperty> superDataPropertiesOfDtp1 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1")));
        Assert.AreEqual(0, superDataPropertiesOfDtp1.Count);

        List<OWLDataProperty> superDataPropertiesOfDtp2 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.AreEqual(1, superDataPropertiesOfDtp2.Count);

        List<OWLDataProperty> superDataPropertiesOfDtp3 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.AreEqual(2, superDataPropertiesOfDtp3.Count);

        List<OWLDataProperty> superDataPropertiesOfDtp4 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.AreEqual(4, superDataPropertiesOfDtp4.Count);

        List<OWLDataProperty> subDataPropertiesOfDtp5 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5")));
        Assert.AreEqual(2, subDataPropertiesOfDtp5.Count);

        Assert.AreEqual(0, ontology.GetSuperDataPropertiesOf(null).Count);
        Assert.AreEqual(0, (null as OWLOntology).GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count);
    }

    [TestMethod]
    public void ShouldGetEquivalentDataProperties()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLEquivalentDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3")) ]),
                new OWLEquivalentDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4")) ]),
                new OWLEquivalentDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp5")) ])
            ]
        };

        List<OWLDataProperty> equivalentDataPropertiesOfDtp1 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")));
        Assert.AreEqual(4, equivalentDataPropertiesOfDtp1.Count);
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp5"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp5")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp5"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp5")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp5"))));
        Assert.IsTrue(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp5")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));

        List<OWLDataProperty> equivalentDataPropertiesOfDtp2 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.AreEqual(4, equivalentDataPropertiesOfDtp2.Count);

        List<OWLDataProperty> equivalentDataPropertiesOfDtp3 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.AreEqual(4, equivalentDataPropertiesOfDtp3.Count);

        List<OWLDataProperty> equivalentDataPropertiesOfDtp4 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.AreEqual(4, equivalentDataPropertiesOfDtp4.Count);

        Assert.AreEqual(0, ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp6"))).Count);
        Assert.AreEqual(0, ontology.GetEquivalentDataProperties(null).Count);
        Assert.IsFalse(ontology.CheckAreEquivalentDataProperties(null, new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsFalse(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), null));
        Assert.IsFalse((null as OWLOntology).CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.AreEqual(0, (null as OWLOntology).GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count);
    }

    [TestMethod]
    public void ShouldGetDisjointDataProperties()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLDisjointDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3")) ]),
                new OWLDisjointDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4")) ]),
                new OWLDisjointDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp5")) ])
            ]
        };

        List<OWLDataProperty> disjointDataPropertiesOfDtp1 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")));
        Assert.AreEqual(3, disjointDataPropertiesOfDtp1.Count);
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp5"))));
        Assert.IsTrue(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp5")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));

        List<OWLDataProperty> disjointDataPropertiesOfDtp2 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.AreEqual(3, disjointDataPropertiesOfDtp2.Count);

        List<OWLDataProperty> disjointDataPropertiesOfDtp3 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.AreEqual(2, disjointDataPropertiesOfDtp3.Count);

        List<OWLDataProperty> disjointOfDtp4 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.AreEqual(1, disjointOfDtp4.Count);

        Assert.AreEqual(0, ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp6"))).Count);
        Assert.AreEqual(0, ontology.GetDisjointDataProperties(null).Count);
        Assert.IsFalse(ontology.CheckAreDisjointDataProperties(null, new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsFalse(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), null));
        Assert.IsFalse((null as OWLOntology).CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.AreEqual(0, (null as OWLOntology).GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count);
    }

    [TestMethod]
    public void ShouldCheckHasFunctionalDataProperty()
    {
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:FuncDtp"))),
                new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:FuncDtp")), new OWLDataProperty(new RDFResource("ex:Dtp2"))),
                new OWLDisjointDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4")) ])
            ]
        };

        Assert.IsTrue(ontology.CheckHasFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:FuncDtp"))));
        Assert.IsFalse(ontology.CheckHasFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsFalse(ontology.CheckHasFunctionalDataProperty(null));
        Assert.IsFalse((null as OWLOntology).CheckHasFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:FuncDtp"))));
    }
    #endregion
}