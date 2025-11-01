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
        Assert.HasCount(1, subDataPropertyOf);

        List<OWLFunctionalDataProperty> functionalDataProperty = ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>();
        Assert.HasCount(1, functionalDataProperty);

        List<OWLEquivalentDataProperties> equivalentDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>();
        Assert.HasCount(1, equivalentDataProperties);

        List<OWLDisjointDataProperties> disjointDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>();
        Assert.HasCount(1, disjointDataProperties);

        List<OWLDataPropertyRange> dataPropertyRange = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>();
        Assert.HasCount(1, dataPropertyRange);

        List<OWLDataPropertyDomain> dataPropertyDomain = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>();
        Assert.HasCount(1, dataPropertyDomain);

        Assert.IsEmpty((null as OWLOntology).GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>());
    }

    [TestMethod]
    public void ShouldDeclareDataPropertyAxiom()
    {
        OWLOntology ontology = new OWLOntology();
        ontology.DeclareDataPropertyAxiom(new OWLSubDataPropertyOf(
            new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
            new OWLDataProperty(RDFVocabulary.FOAF.TITLE)));

        Assert.HasCount(1, ontology.DataPropertyAxioms);
        Assert.IsTrue(ontology.CheckHasDataPropertyAxiom(new OWLSubDataPropertyOf(
            new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
            new OWLDataProperty(RDFVocabulary.FOAF.TITLE))));
        Assert.ThrowsExactly<OWLException>(() => ontology.DeclareDataPropertyAxiom<OWLDataPropertyAxiom>(null));

        ontology.DeclareDataPropertyAxiom(new OWLSubDataPropertyOf(
            new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
            new OWLDataProperty(RDFVocabulary.FOAF.TITLE))); //will be discarded, since duplicates are not allowed
        Assert.HasCount(1, ontology.DataPropertyAxioms);
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
        Assert.HasCount(3, subDataPropertiesOfDtp1);
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));

        List<OWLDataProperty> subDataPropertiesOfDtp2 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.HasCount(2, subDataPropertiesOfDtp2);
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));

        List<OWLDataProperty> subDataPropertiesOfDtp3 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.HasCount(1, subDataPropertiesOfDtp3);
        Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));

        List<OWLDataProperty> subDataPropertiesOfDtp4 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.IsEmpty(subDataPropertiesOfDtp4);

        Assert.IsEmpty(ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5"))));
        Assert.IsEmpty(ontology.GetSubDataPropertiesOf(null));
        Assert.IsFalse(ontology.CheckIsSubDataPropertyOf(null, new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsFalse(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), null));
        Assert.IsFalse((null as OWLOntology).CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsEmpty((null as OWLOntology).GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))));
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
        Assert.HasCount(5, subDataPropertiesOfDtp1);

        List<OWLDataProperty> subDataPropertiesOfDtp2 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.HasCount(4, subDataPropertiesOfDtp2);

        List<OWLDataProperty> subDataPropertiesOfDtp3 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.HasCount(2, subDataPropertiesOfDtp3);

        List<OWLDataProperty> subDataPropertiesOfDtp4 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.IsEmpty(subDataPropertiesOfDtp4);

        List<OWLDataProperty> subDataPropertiesOfDtp5 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5")));
        Assert.HasCount(2, subDataPropertiesOfDtp5);

        Assert.IsEmpty(ontology.GetSubDataPropertiesOf(null));
        Assert.IsEmpty((null as OWLOntology).GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))));
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
        Assert.IsEmpty(superDataPropertiesOfDtp1);

        List<OWLDataProperty> superDataPropertiesOfDtp2 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.HasCount(1, superDataPropertiesOfDtp2);
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));

        List<OWLDataProperty> superDataPropertiesOfDtp3 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.HasCount(2, superDataPropertiesOfDtp3);
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));

        List<OWLDataProperty> superDataPropertiesOfDtp4 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.HasCount(3, superDataPropertiesOfDtp4);
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
        Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));

        Assert.IsEmpty(ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5"))));
        Assert.IsEmpty(ontology.GetSuperDataPropertiesOf(null));
        Assert.IsFalse(ontology.CheckIsSuperDataPropertyOf(null, new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsFalse(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), null));
        Assert.IsFalse((null as OWLOntology).CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsEmpty((null as OWLOntology).GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))));
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
        Assert.IsEmpty(superDataPropertiesOfDtp1);

        List<OWLDataProperty> superDataPropertiesOfDtp2 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
        Assert.HasCount(1, superDataPropertiesOfDtp2);

        List<OWLDataProperty> superDataPropertiesOfDtp3 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.HasCount(2, superDataPropertiesOfDtp3);

        List<OWLDataProperty> superDataPropertiesOfDtp4 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.HasCount(4, superDataPropertiesOfDtp4);

        List<OWLDataProperty> subDataPropertiesOfDtp5 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5")));
        Assert.HasCount(2, subDataPropertiesOfDtp5);

        Assert.IsEmpty(ontology.GetSuperDataPropertiesOf(null));
        Assert.IsEmpty((null as OWLOntology).GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))));
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
        Assert.HasCount(4, equivalentDataPropertiesOfDtp1);
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
        Assert.HasCount(4, equivalentDataPropertiesOfDtp2);

        List<OWLDataProperty> equivalentDataPropertiesOfDtp3 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.HasCount(4, equivalentDataPropertiesOfDtp3);

        List<OWLDataProperty> equivalentDataPropertiesOfDtp4 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.HasCount(4, equivalentDataPropertiesOfDtp4);

        Assert.IsEmpty(ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp6"))));
        Assert.IsEmpty(ontology.GetEquivalentDataProperties(null));
        Assert.IsFalse(ontology.CheckAreEquivalentDataProperties(null, new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsFalse(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), null));
        Assert.IsFalse((null as OWLOntology).CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsEmpty((null as OWLOntology).GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1"))));
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
        Assert.HasCount(3, disjointDataPropertiesOfDtp1);
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
        Assert.HasCount(3, disjointDataPropertiesOfDtp2);

        List<OWLDataProperty> disjointDataPropertiesOfDtp3 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")));
        Assert.HasCount(2, disjointDataPropertiesOfDtp3);

        List<OWLDataProperty> disjointOfDtp4 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")));
        Assert.HasCount(1, disjointOfDtp4);

        Assert.IsEmpty(ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp6"))));
        Assert.IsEmpty(ontology.GetDisjointDataProperties(null));
        Assert.IsFalse(ontology.CheckAreDisjointDataProperties(null, new OWLDataProperty(new RDFResource("ex:Dtp1"))));
        Assert.IsFalse(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), null));
        Assert.IsFalse((null as OWLOntology).CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
        Assert.IsEmpty((null as OWLOntology).GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1"))));
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