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
    public class OWLObjectPropertyAxiomHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetObjectPropertyAxioms()
        {
            OWLOntology ontology = new OWLOntology
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
            Assert.AreEqual(1, subObjectPropertyOf.Count);

            List<OWLTransitiveObjectProperty> transitiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>();
            Assert.AreEqual(1, transitiveObjectProperty.Count);

            List<OWLSymmetricObjectProperty> symmetricObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>();
            Assert.AreEqual(1, symmetricObjectProperty.Count);

            List<OWLReflexiveObjectProperty> reflexiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>();
            Assert.AreEqual(1, reflexiveObjectProperty.Count);

            List<OWLObjectPropertyRange> objectPropertyRange = ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>();
            Assert.AreEqual(1, objectPropertyRange.Count);

            List<OWLObjectPropertyDomain> objectPropertyDomain = ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>();
            Assert.AreEqual(1, objectPropertyDomain.Count);

            List<OWLIrreflexiveObjectProperty> irreflexiveObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>();
            Assert.AreEqual(1, irreflexiveObjectProperty.Count);

            List<OWLInverseObjectProperties> inverseObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();
            Assert.AreEqual(1, inverseObjectProperty.Count);

            List<OWLInverseFunctionalObjectProperty> inverseFunctionalObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>();
            Assert.AreEqual(1, inverseFunctionalObjectProperty.Count);

            List<OWLFunctionalObjectProperty> functionalObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>();
            Assert.AreEqual(1, functionalObjectProperty.Count);

            List<OWLEquivalentObjectProperties> equivalentObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>();
            Assert.AreEqual(1, equivalentObjectProperty.Count);

            List<OWLDisjointObjectProperties> disjointObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>();
            Assert.AreEqual(1, disjointObjectProperty.Count);

            List<OWLAsymmetricObjectProperty> asymmetricObjectProperty = ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>();
            Assert.AreEqual(1, asymmetricObjectProperty.Count);

            Assert.AreEqual(0, (null as OWLOntology).GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Count);
        }

        [TestMethod]
        public void ShouldDeclareObjectPropertyAxiom()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
                new OWLObjectProperty(RDFVocabulary.FOAF.SHA1),
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.TITLE))));

            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
            Assert.IsTrue(ontology.CheckHasObjectPropertyAxiom(new OWLSubObjectPropertyOf(
                new OWLObjectProperty(RDFVocabulary.FOAF.SHA1),
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.TITLE)))));
            Assert.ThrowsExactly<OWLException>(() => ontology.DeclareObjectPropertyAxiom<OWLObjectPropertyAxiom>(null));

            ontology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
                new OWLObjectProperty(RDFVocabulary.FOAF.SHA1),
                new OWLObjectInverseOf(new OWLObjectProperty(RDFVocabulary.FOAF.TITLE)))); //will be discarded, since duplicates are not allowed
            Assert.AreEqual(1, ontology.ObjectPropertyAxioms.Count);
        }

        [TestMethod]
        public void ShouldGetSubObjectPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3")))
                ]
            };

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp1 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.AreEqual(3, subObjectPropertiesOfObp1.Count);
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp2 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.AreEqual(2, subObjectPropertiesOfObp2.Count);
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp3 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.AreEqual(1, subObjectPropertiesOfObp3.Count);
            Assert.IsTrue(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp4 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.AreEqual(0, subObjectPropertiesOfObp4.Count);

            Assert.AreEqual(0, ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5"))).Count);
            Assert.AreEqual(0, ontology.GetSubObjectPropertiesOf(null).Count);
            Assert.IsFalse(ontology.CheckIsSubObjectPropertyOf(null, new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.AreEqual(0, (null as OWLOntology).GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSubObjectPropertiesOfWithEquivalentObjectPropertiesDiscovery()
        {
            OWLOntology ontology = new OWLOntology
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
            Assert.AreEqual(5, subObjectPropertiesOfObp1.Count);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp2 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.AreEqual(4, subObjectPropertiesOfObp2.Count);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp3 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.AreEqual(2, subObjectPropertiesOfObp3.Count);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp4 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.AreEqual(0, subObjectPropertiesOfObp4.Count);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp5 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5")));
            Assert.AreEqual(2, subObjectPropertiesOfObp5.Count);

            Assert.AreEqual(0, ontology.GetSubObjectPropertiesOf(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count);
        }
        
        [TestMethod]
        public void ShouldGetSuperObjectPropertiesOf()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp4")), new OWLObjectProperty(new RDFResource("ex:Obp3")))
                ]
            };

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp1 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.AreEqual(0, superObjectPropertiesOfObp1.Count);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp2 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.AreEqual(1, superObjectPropertiesOfObp2.Count);
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp3 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.AreEqual(2, superObjectPropertiesOfObp3.Count);
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3"))));

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp4 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.AreEqual(3, superObjectPropertiesOfObp4.Count);
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));
            Assert.IsTrue(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp3")), new OWLObjectProperty(new RDFResource("ex:Obp4"))));

            Assert.AreEqual(0, ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5"))).Count);
            Assert.AreEqual(0, ontology.GetSuperObjectPropertiesOf(null).Count);
            Assert.IsFalse(ontology.CheckIsSuperObjectPropertyOf(null, new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSuperObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.AreEqual(0, (null as OWLOntology).GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSuperObjectPropertiesOfWithEquivalentObjectPropertiesDiscovery()
        {
            OWLOntology ontology = new OWLOntology
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
            Assert.AreEqual(0, superObjectPropertiesOfObp1.Count);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp2 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp2")));
            Assert.AreEqual(1, superObjectPropertiesOfObp2.Count);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp3 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.AreEqual(2, superObjectPropertiesOfObp3.Count);

            List<OWLObjectPropertyExpression> superObjectPropertiesOfObp4 = ontology.GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.AreEqual(4, superObjectPropertiesOfObp4.Count);

            List<OWLObjectPropertyExpression> subObjectPropertiesOfObp5 = ontology.GetSubObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp5")));
            Assert.AreEqual(2, subObjectPropertiesOfObp5.Count);

            Assert.AreEqual(0, ontology.GetSuperObjectPropertiesOf(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetSuperObjectPropertiesOf(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count);
        }
        
        [TestMethod]
        public void ShouldGetEquivalentObjectProperties()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3")) ]),
                    new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ]),
                    new OWLEquivalentObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp5")) ])
                ]
            };

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp1 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.AreEqual(4, equivalentObjectPropertiesOfObp1.Count);
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
            Assert.AreEqual(4, equivalentObjectPropertiesOfObp2.Count);

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp3 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.AreEqual(4, equivalentObjectPropertiesOfObp3.Count);

            List<OWLObjectPropertyExpression> equivalentObjectPropertiesOfObp4 = ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.AreEqual(4, equivalentObjectPropertiesOfObp4.Count);

            Assert.AreEqual(0, ontology.GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp6"))).Count);
            Assert.AreEqual(0, ontology.GetEquivalentObjectProperties(null).Count);
            Assert.IsFalse(ontology.CheckAreEquivalentObjectProperties(null, new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsFalse(ontology.CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.AreEqual(0, (null as OWLOntology).GetEquivalentObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count);
        }

        [TestMethod]
        public void ShouldGetDisjointObjectProperties()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp3")) ]),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ]),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp2")), new OWLObjectProperty(new RDFResource("ex:Obp5")) ])
                ]
            };

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp1 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")));
            Assert.AreEqual(3, disjointObjectPropertiesOfObp1.Count);
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
            Assert.AreEqual(3, disjointObjectPropertiesOfObp2.Count);

            List<OWLObjectPropertyExpression> disjointObjectPropertiesOfObp3 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp3")));
            Assert.AreEqual(2, disjointObjectPropertiesOfObp3.Count);

            List<OWLObjectPropertyExpression> disjointOfObp4 = ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp4")));
            Assert.AreEqual(1, disjointOfObp4.Count);

            Assert.AreEqual(0, ontology.GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp6"))).Count);
            Assert.AreEqual(0, ontology.GetDisjointObjectProperties(null).Count);
            Assert.IsFalse(ontology.CheckAreDisjointObjectProperties(null, new OWLObjectProperty(new RDFResource("ex:Obp1"))));
            Assert.IsFalse(ontology.CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.AreEqual(0, (null as OWLOntology).GetDisjointObjectProperties(new OWLObjectProperty(new RDFResource("ex:Obp1"))).Count);
        }

        [TestMethod]
        public void ShouldCheckHasFunctionalObjectProperty()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:FuncObp"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:FuncObp")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            Assert.IsTrue(ontology.CheckHasFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:FuncObp"))));
            Assert.IsFalse(ontology.CheckHasFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckHasFunctionalObjectProperty(null));
            Assert.IsFalse((null as OWLOntology).CheckHasFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:FuncObp"))));
        }

        [TestMethod]
        public void ShouldCheckHasInverseFunctionalObjectProperty()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:InvFuncObp"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:InvFuncObp")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            Assert.IsTrue(ontology.CheckHasInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:InvFuncObp"))));
            Assert.IsFalse(ontology.CheckHasInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckHasInverseFunctionalObjectProperty(null));
            Assert.IsFalse((null as OWLOntology).CheckHasInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:InvFuncObp"))));
        }

        [TestMethod]
        public void ShouldCheckHasSymmetricObjectProperty()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:SymObp"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:SymObp")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            Assert.IsTrue(ontology.CheckHasSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:SymObp"))));
            Assert.IsFalse(ontology.CheckHasSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckHasSymmetricObjectProperty(null));
            Assert.IsFalse((null as OWLOntology).CheckHasSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:SymObp"))));
        }

        [TestMethod]
        public void ShouldCheckHasAsymmetricObjectProperty()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:AsymObp"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:AsymObp")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            Assert.IsTrue(ontology.CheckHasAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:AsymObp"))));
            Assert.IsFalse(ontology.CheckHasAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckHasAsymmetricObjectProperty(null));
            Assert.IsFalse((null as OWLOntology).CheckHasAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:AsymObp"))));
        }

        [TestMethod]
        public void ShouldCheckHasReflexiveObjectProperty()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:ReflObp"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:ReflObp")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            Assert.IsTrue(ontology.CheckHasReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:ReflObp"))));
            Assert.IsFalse(ontology.CheckHasReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckHasReflexiveObjectProperty(null));
            Assert.IsFalse((null as OWLOntology).CheckHasReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:ReflObp"))));
        }

        [TestMethod]
        public void ShouldCheckHasIrreflexiveObjectProperty()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:IrreflObp"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:IrreflObp")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            Assert.IsTrue(ontology.CheckHasIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:IrreflObp"))));
            Assert.IsFalse(ontology.CheckHasIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckHasIrreflexiveObjectProperty(null));
            Assert.IsFalse((null as OWLOntology).CheckHasIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:IrreflObp"))));
        }

        [TestMethod]
        public void ShouldCheckHasTransitiveObjectProperty()
        {
            OWLOntology ontology = new OWLOntology
            {
                ObjectPropertyAxioms = [
                    new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:TransObp"))),
                    new OWLSubObjectPropertyOf(new OWLObjectProperty(new RDFResource("ex:TransObp")), new OWLObjectProperty(new RDFResource("ex:Obp2"))),
                    new OWLDisjointObjectProperties([ new OWLObjectProperty(new RDFResource("ex:Obp1")), new OWLObjectProperty(new RDFResource("ex:Obp4")) ])
                ]
            };

            Assert.IsTrue(ontology.CheckHasTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:TransObp"))));
            Assert.IsFalse(ontology.CheckHasTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:Obp2"))));
            Assert.IsFalse(ontology.CheckHasTransitiveObjectProperty(null));
            Assert.IsFalse((null as OWLOntology).CheckHasTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:TransObp"))));
        }
        #endregion
    }
}