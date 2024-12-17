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
    public class OWLDataPropertyAxiomHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldGetDataPropertyAxioms()
		{
			OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(subDataPropertyOf.Count == 1);

            List<OWLFunctionalDataProperty> functionalDataProperty = ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>();
            Assert.IsTrue(functionalDataProperty.Count == 1);

            List<OWLEquivalentDataProperties> equivalentDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>();
            Assert.IsTrue(equivalentDataProperties.Count == 1);

            List<OWLDisjointDataProperties> disjointDataProperties = ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>();
            Assert.IsTrue(disjointDataProperties.Count == 1);

			List<OWLDataPropertyRange> dataPropertyRange = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>();
            Assert.IsTrue(dataPropertyRange.Count == 1);

			List<OWLDataPropertyDomain> dataPropertyDomain = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>();
            Assert.IsTrue(dataPropertyDomain.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Count == 0);
        }

        [TestMethod]
        public void ShouldDeclareDataPropertyAxiom()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareDataPropertyAxiom(new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
                new OWLDataProperty(RDFVocabulary.FOAF.TITLE)));

            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
            Assert.IsTrue(ontology.CheckHasDataPropertyAxiom(new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
                new OWLDataProperty(RDFVocabulary.FOAF.TITLE))));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareDataPropertyAxiom(null as OWLDataPropertyAxiom));

            ontology.DeclareDataPropertyAxiom(new OWLSubDataPropertyOf(
                new OWLDataProperty(RDFVocabulary.FOAF.SHA1),
                new OWLDataProperty(RDFVocabulary.FOAF.TITLE))); //will be discarded, since duplicates are not allowed
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 1);
        }

        [TestMethod]
        public void ShouldGetSubDataPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DataPropertyAxioms = [
                    new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))),
                    new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))),
                    new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3"))),
                ]
            };

            List<OWLDataProperty> subDataPropertiesOfDtp1 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1")));
            Assert.IsTrue(subDataPropertiesOfDtp1.Count == 3);
            Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
            Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
            Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));

            List<OWLDataProperty> subDataPropertiesOfDtp2 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
            Assert.IsTrue(subDataPropertiesOfDtp2.Count == 2);
            Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
            Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));

            List<OWLDataProperty> subDataPropertiesOfDtp3 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
            Assert.IsTrue(subDataPropertiesOfDtp3.Count == 1);
            Assert.IsTrue(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));

            List<OWLDataProperty> subDataPropertiesOfDtp4 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
            Assert.IsTrue(subDataPropertiesOfDtp4.Count == 0);

            Assert.IsTrue(ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSubDataPropertiesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSubDataPropertyOf(null, new OWLDataProperty(new RDFResource("ex:Dtp2"))));
            Assert.IsFalse(ontology.CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
            Assert.IsTrue((null as OWLOntology).GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldGetSubDataPropertiesOfWithEquivalentDataPropertiesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(subDataPropertiesOfDtp1.Count == 5);

            List<OWLDataProperty> subDataPropertiesOfDtp2 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
            Assert.IsTrue(subDataPropertiesOfDtp2.Count == 4);

            List<OWLDataProperty> subDataPropertiesOfDtp3 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
            Assert.IsTrue(subDataPropertiesOfDtp3.Count == 2);

            List<OWLDataProperty> subDataPropertiesOfDtp4 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
            Assert.IsTrue(subDataPropertiesOfDtp4.Count == 0);

            List<OWLDataProperty> subDataPropertiesOfDtp5 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5")));
            Assert.IsTrue(subDataPropertiesOfDtp5.Count == 2);

            Assert.IsTrue(ontology.GetSubDataPropertiesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count == 0);
        }
		
		[TestMethod]
        public void ShouldGetSuperDataPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DataPropertyAxioms = [
                    new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))),
                    new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp2"))),
                    new OWLSubDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp4")), new OWLDataProperty(new RDFResource("ex:Dtp3"))),
                ]
            };

            List<OWLDataProperty> superDataPropertiesOfDtp1 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1")));
            Assert.IsTrue(superDataPropertiesOfDtp1.Count == 0);

            List<OWLDataProperty> superDataPropertiesOfDtp2 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
            Assert.IsTrue(superDataPropertiesOfDtp2.Count == 1);
            Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));

            List<OWLDataProperty> superDataPropertiesOfDtp3 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
            Assert.IsTrue(superDataPropertiesOfDtp3.Count == 2);
            Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));
			Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3"))));

            List<OWLDataProperty> superDataPropertiesOfDtp4 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
            Assert.IsTrue(superDataPropertiesOfDtp4.Count == 3);
			Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
			Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));
			Assert.IsTrue(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp3")), new OWLDataProperty(new RDFResource("ex:Dtp4"))));

            Assert.IsTrue(ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSuperDataPropertiesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSuperDataPropertyOf(null, new OWLDataProperty(new RDFResource("ex:Dtp2"))));
            Assert.IsFalse(ontology.CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSuperDataPropertyOf(new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp1"))));
            Assert.IsTrue((null as OWLOntology).GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldGetSuperDataPropertiesOfWithEquivalentDataPropertiesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
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
            Assert.IsTrue(superDataPropertiesOfDtp1.Count == 0);

            List<OWLDataProperty> superDataPropertiesOfDtp2 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp2")));
            Assert.IsTrue(superDataPropertiesOfDtp2.Count == 1);

            List<OWLDataProperty> superDataPropertiesOfDtp3 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp3")));
            Assert.IsTrue(superDataPropertiesOfDtp3.Count == 2);

            List<OWLDataProperty> superDataPropertiesOfDtp4 = ontology.GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp4")));
            Assert.IsTrue(superDataPropertiesOfDtp4.Count == 4);

            List<OWLDataProperty> subDataPropertiesOfDtp5 = ontology.GetSubDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp5")));
            Assert.IsTrue(subDataPropertiesOfDtp5.Count == 2);

            Assert.IsTrue(ontology.GetSuperDataPropertiesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSuperDataPropertiesOf(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count == 0);
        }
		
		[TestMethod]
        public void ShouldGetEquivalentDataProperties()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DataPropertyAxioms = [
                    new OWLEquivalentDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3")) ]),
					new OWLEquivalentDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4")) ]),
					new OWLEquivalentDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp5")) ]),
                ]
            };

            List<OWLDataProperty> equivalentDataPropertiesOfDtp1 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")));
            Assert.IsTrue(equivalentDataPropertiesOfDtp1.Count == 4);
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
            Assert.IsTrue(equivalentDataPropertiesOfDtp2.Count == 4);

            List<OWLDataProperty> equivalentDataPropertiesOfDtp3 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")));
            Assert.IsTrue(equivalentDataPropertiesOfDtp3.Count == 4);

            List<OWLDataProperty> equivalentDataPropertiesOfDtp4 = ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")));
            Assert.IsTrue(equivalentDataPropertiesOfDtp4.Count == 4);

            Assert.IsTrue(ontology.GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp6"))).Count == 0);
            Assert.IsTrue(ontology.GetEquivalentDataProperties(null).Count == 0);
            Assert.IsFalse(ontology.CheckAreEquivalentDataProperties(null, new OWLDataProperty(new RDFResource("ex:Dtp1"))));
            Assert.IsFalse(ontology.CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
            Assert.IsTrue((null as OWLOntology).GetEquivalentDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count == 0);
        }

        [TestMethod]
        public void ShouldGetDisjointDataProperties()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DataPropertyAxioms = [
                    new OWLDisjointDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp3")) ]),
                    new OWLDisjointDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp4")) ]),
                    new OWLDisjointDataProperties([ new OWLDataProperty(new RDFResource("ex:Dtp2")), new OWLDataProperty(new RDFResource("ex:Dtp5")) ]),
                ]
            };

            List<OWLDataProperty> disjointDataPropertiesOfDtp1 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")));
            Assert.IsTrue(disjointDataPropertiesOfDtp1.Count == 3);
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
            Assert.IsTrue(disjointDataPropertiesOfDtp2.Count == 3);

            List<OWLDataProperty> disjointDataPropertiesOfDtp3 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp3")));
            Assert.IsTrue(disjointDataPropertiesOfDtp3.Count == 2);

            List<OWLDataProperty> disjointOfDtp4 = ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp4")));
            Assert.IsTrue(disjointOfDtp4.Count == 1);

            Assert.IsTrue(ontology.GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp6"))).Count == 0);
            Assert.IsTrue(ontology.GetDisjointDataProperties(null).Count == 0);
            Assert.IsFalse(ontology.CheckAreDisjointDataProperties(null, new OWLDataProperty(new RDFResource("ex:Dtp1"))));
            Assert.IsFalse(ontology.CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1")), new OWLDataProperty(new RDFResource("ex:Dtp2"))));
            Assert.IsTrue((null as OWLOntology).GetDisjointDataProperties(new OWLDataProperty(new RDFResource("ex:Dtp1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldCheckHasFunctionalDataProperty()
        {
            OWLOntology ontology = new OWLOntology()
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
}