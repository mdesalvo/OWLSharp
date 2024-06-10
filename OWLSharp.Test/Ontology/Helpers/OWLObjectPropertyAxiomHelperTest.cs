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
    public class OWLObjectPropertyAxiomHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldGetObjectPropertyAxioms()
		{
			OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectPropertyChain([ new OWLObjectProperty(new RDFResource("ex:hasFather")), new OWLObjectProperty(new RDFResource("ex:hasBrother")) ]), new OWLObjectProperty(new RDFResource("ex:hasUncle"))),
					new OWLTransitiveObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLSymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLReflexiveObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLObjectPropertyRange(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLClass(RDFVocabulary.FOAF.PERSON)),
					new OWLObjectPropertyDomain(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLClass(RDFVocabulary.FOAF.PERSON)),
					new OWLIrreflexiveObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLInverseObjectProperties(new OWLObjectProperty(new RDFResource("ex:hasWife")), new OWLObjectProperty(new RDFResource("ex:isWifeOf"))),
					new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLFunctionalObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
					new OWLEquivalentObjectProperties([ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER) ]),
					new OWLDisjointObjectProperties([ new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.MEMBER), new OWLObjectProperty(RDFVocabulary.FOAF.ACCOUNT) ]),
					new OWLAsymmetricObjectProperty(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS))
                ]
            };

            List<OWLSubObjectPropertyOf> subObjectPropertyOf = ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>();
            Assert.IsTrue(subObjectPropertyOf.Count == 1);

			List<OWLTransitiveObjectProperty> transitiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>();
            Assert.IsTrue(transitiveObjectProperty.Count == 1);

			List<OWLSymmetricObjectProperty> symmetricObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>();
            Assert.IsTrue(symmetricObjectProperty.Count == 1);

			List<OWLReflexiveObjectProperty> reflexiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>();
            Assert.IsTrue(reflexiveObjectProperty.Count == 1);

			List<OWLObjectPropertyRange> objectPropertyRange = ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>();
            Assert.IsTrue(objectPropertyRange.Count == 1);

			List<OWLObjectPropertyDomain> objectPropertyDomain = ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>();
            Assert.IsTrue(objectPropertyDomain.Count == 1);

			List<OWLIrreflexiveObjectProperty> irreflexiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>();
            Assert.IsTrue(irreflexiveObjectProperty.Count == 1);

			List<OWLInverseObjectProperties> inverseObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();
            Assert.IsTrue(inverseObjectProperty.Count == 1);

			List<OWLInverseFunctionalObjectProperty> inverseFunctionalObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>();
            Assert.IsTrue(inverseFunctionalObjectProperty.Count == 1);

			List<OWLFunctionalObjectProperty> functionalObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>();
            Assert.IsTrue(functionalObjectProperty.Count == 1);

			List<OWLEquivalentObjectProperties> equivalentObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>();
            Assert.IsTrue(equivalentObjectProperty.Count == 1);

			List<OWLDisjointObjectProperties> disjointObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>();
            Assert.IsTrue(disjointObjectProperty.Count == 1);

			List<OWLAsymmetricObjectProperty> asymmetricObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>();
            Assert.IsTrue(asymmetricObjectProperty.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Count == 0);
        }
		
		[TestMethod]
        public void ShouldGetSubObjectPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))),
                ]
            };

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp1 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(subObjectPropertiesOfObp1.Count == 3);
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp2 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(subObjectPropertiesOfObp2.Count == 2);
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp3 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(subObjectPropertiesOfObp3.Count == 1);
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp4 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(subObjectPropertiesOfObp4.Count == 0);

            Assert.IsTrue(ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSubObjectPropertiesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSubObjectPropertyOf(null, new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue((null as OWLOntology).GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldGetSubObjectPropertiesOfWithEquivalentObjectPropertiesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))),
					new OWLEquivalentObjectProperties([new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp5"))]),
					new OWLEquivalentObjectProperties([new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp6"))])
                ]
            };

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp1 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(subObjectPropertiesOfObp1.Count == 5);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp2 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(subObjectPropertiesOfObp2.Count == 4);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp3 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(subObjectPropertiesOfObp3.Count == 2);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp4 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(subObjectPropertiesOfObp4.Count == 0);

            Assert.IsTrue(ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSubObjectPropertiesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldGetSubObjectPropertiesOfDirectOnly()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))),
                ]
            };

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp1 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), true);
            Assert.IsTrue(subObjectPropertiesOfObp1.Count == 1);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp2 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), true);
            Assert.IsTrue(subObjectPropertiesOfObp2.Count == 1);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp3 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), true);
            Assert.IsTrue(subObjectPropertiesOfObp3.Count == 1);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp4 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), true);
            Assert.IsTrue(subObjectPropertiesOfObp4.Count == 0);

            Assert.IsTrue(ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5")), true).Count == 0);
            Assert.IsTrue(ontology.GetSubObjectPropertiesOf(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), true).Count == 0);
        }
		
		[TestMethod]
        public void ShouldGetSuperObjectPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))),
                ]
            };

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp1 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(superObjectPropertiesOfObp1.Count == 0);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp2 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(superObjectPropertiesOfObp2.Count == 1);
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp3 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(superObjectPropertiesOfObp3.Count == 2);
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));
			Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp4 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(superObjectPropertiesOfObp4.Count == 3);
			Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
			Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
			Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));

            Assert.IsTrue(ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSuperObjectPropertiesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSuperObjectPropertyOf(null, new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue((null as OWLOntology).GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldGetSuperObjectPropertiesOfWithEquivalentObjectPropertiesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))),
					new OWLEquivalentObjectProperties([new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp5"))]),
					new OWLEquivalentObjectProperties([new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp6"))])
                ]
            };

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp1 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(superObjectPropertiesOfObp1.Count == 0);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp2 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(superObjectPropertiesOfObp2.Count == 1);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp3 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(superObjectPropertiesOfObp3.Count == 2);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp4 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(superObjectPropertiesOfObp4.Count == 4);

            Assert.IsTrue(ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5"))).Count == 0);
            Assert.IsTrue(ontology.GetSuperObjectPropertiesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count == 0);
        }

        [TestMethod]
        public void ShouldGetSuperObjectPropertiesOfDirectOnly()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))),
                ]
            };

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp1 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), true);
            Assert.IsTrue(superObjectPropertiesOfObp1.Count == 0);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp2 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), true);
            Assert.IsTrue(superObjectPropertiesOfObp2.Count == 1);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp3 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), true);
            Assert.IsTrue(superObjectPropertiesOfObp3.Count == 1);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp4 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), true);
            Assert.IsTrue(superObjectPropertiesOfObp4.Count == 1);

            Assert.IsTrue(ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5")), true).Count == 0);
            Assert.IsTrue(ontology.GetSuperObjectPropertiesOf(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), true).Count == 0);
        }
		
		[TestMethod]
        public void ShouldGetEquivalentObjectProperties()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3")) ]),
					new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ]),
					new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp5")) ]),
                ]
            };

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp1 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(equivalentObjectPropertiesOfObp1.Count == 4);
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp5"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp5")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp5"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp5")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp5"))));
            Assert.IsTrue(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp5")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp2 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(equivalentObjectPropertiesOfObp2.Count == 4);

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp3 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(equivalentObjectPropertiesOfObp3.Count == 4);

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp4 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(equivalentObjectPropertiesOfObp4.Count == 4);

            Assert.IsTrue(ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp6"))).Count == 0);
            Assert.IsTrue(ontology.GetEquivalentObjectProperties(null).Count == 0);
            Assert.IsFalse(ontology.CheckAreEquivalentObjectProperties(null, new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsFalse(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue((null as OWLOntology).GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count == 0);
        }

		[TestMethod]
        public void ShouldGetEquivalentObjectPropertiesDirectOnly()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3")) ]),
					new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ]),
					new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp5")) ]),
                ]
            };

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp1 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), true);
            Assert.IsTrue(equivalentObjectPropertiesOfObp1.Count == 3);

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp2 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")), true);
            Assert.IsTrue(equivalentObjectPropertiesOfObp2.Count == 3);

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp3 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")), true);
            Assert.IsTrue(equivalentObjectPropertiesOfObp3.Count == 2);

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp4 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")), true);
            Assert.IsTrue(equivalentObjectPropertiesOfObp4.Count == 1);

            Assert.IsTrue(ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp6")), true).Count == 0);
            Assert.IsTrue(ontology.GetEquivalentObjectProperties(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), true).Count == 0);
        }

        [TestMethod]
        public void ShouldGetDisjointObjectProperties()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3")) ]),
            new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ]),
            new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp5")) ]),
        ]
            };

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp1 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(disjointObjectPropertiesOfObp1.Count == 3);
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp5"))));
            Assert.IsTrue(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp5")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp2 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(disjointObjectPropertiesOfObp2.Count == 3);

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp3 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(disjointObjectPropertiesOfObp3.Count == 2);

            List<OWLObjectPropertyExpression> disjointOfObp4 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(disjointOfObp4.Count == 1);

            Assert.IsTrue(ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp6"))).Count == 0);
            Assert.IsTrue(ontology.GetDisjointObjectProperties(null).Count == 0);
            Assert.IsFalse(ontology.CheckAreDisjointObjectProperties(null, new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsFalse(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue((null as OWLOntology).GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count == 0);
        }

        [TestMethod]
        public void ShouldGetDisjointObjectPropertiesWithEquivalentObjectPropertiesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))]),
            new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3")) ]),
            new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
        ]
            };

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp1 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(disjointObjectPropertiesOfObp1.Count == 1);

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp2 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(disjointObjectPropertiesOfObp2.Count == 1);

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp3 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(disjointObjectPropertiesOfObp3.Count == 1);

            List<OWLObjectPropertyExpression> disjointOfObp4 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(disjointOfObp4.Count == 1);
        }

        [TestMethod]
        public void ShouldGetDisjointObjectPropertiesWithSubObjectPropertyOfDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
            new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
            new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp1 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.IsTrue(disjointObjectPropertiesOfObp1.Count == 1);

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp2 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.IsTrue(disjointObjectPropertiesOfObp2.Count == 1);

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp3 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.IsTrue(disjointObjectPropertiesOfObp3.Count == 1);

            List<OWLObjectPropertyExpression> disjointOfObp4 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.IsTrue(disjointOfObp4.Count == 1);
        }
        #endregion
    }
}