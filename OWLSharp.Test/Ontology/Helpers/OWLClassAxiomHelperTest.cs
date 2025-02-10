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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLClassAxiomHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetClassAxioms()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLEquivalentClasses([ new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ]),
                    new OWLDisjointClasses([ new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION), new OWLClass(RDFVocabulary.FOAF.PERSON) ]),
                    new OWLDisjointUnion(new OWLClass(RDFVocabulary.FOAF.AGENT), [ new OWLClass(RDFVocabulary.FOAF.PERSON), new OWLClass(RDFVocabulary.FOAF.ORGANIZATION) ])
                ]
            };

            List<OWLSubClassOf> subClassOf = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            Assert.AreEqual(1, subClassOf.Count);

            List<OWLEquivalentClasses> equivalentClasses = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();
            Assert.AreEqual(1, equivalentClasses.Count);

            List<OWLDisjointClasses> disjointClasses = ontology.GetClassAxiomsOfType<OWLDisjointClasses>();
            Assert.AreEqual(1, disjointClasses.Count);

            List<OWLDisjointUnion> disjointUnion = ontology.GetClassAxiomsOfType<OWLDisjointUnion>();
            Assert.AreEqual(1, disjointUnion.Count);

            Assert.AreEqual(0, (null as OWLOntology).GetClassAxiomsOfType<OWLSubClassOf>().Count);
        }

        [TestMethod]
        public void ShouldDeclareClassAxiom()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareClassAxiom(new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT)));

            Assert.AreEqual(1, ontology.ClassAxioms.Count);
            Assert.IsTrue(ontology.CheckHasClassAxiom(new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT))));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareClassAxiom(null as OWLClassAxiom));

            ontology.DeclareClassAxiom(new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT))); //will be discarded, since duplicates are not allowed
            Assert.AreEqual(1, ontology.ClassAxioms.Count);
        }

        [TestMethod]
        public void ShouldGetSubClassesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                ]
            };

            List<OWLClassExpression> subClassesOfCls1 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(3, subClassesOfCls1.Count);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls1"))));

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(2, subClassesOfCls2.Count);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls2"))));

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(1, subClassesOfCls3.Count);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))));

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(0, subClassesOfCls4.Count);

            Assert.AreEqual(0, ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count);
            Assert.AreEqual(0, ontology.GetSubClassesOf(null).Count);
            Assert.IsFalse(ontology.CheckIsSubClassOf(null, new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsFalse(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.AreEqual(0, (null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSubClassesOfWithEquivalentClassesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                    new OWLEquivalentClasses([new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls5"))]),
                    new OWLEquivalentClasses([new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls6"))])
                ]
            };

            List<OWLClassExpression> subClassesOfCls1 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(5, subClassesOfCls1.Count);

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(4, subClassesOfCls2.Count);

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(2, subClassesOfCls3.Count);

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(0, subClassesOfCls4.Count);

            List<OWLClassExpression> subClassesOfCls5 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5")));
            Assert.AreEqual(2, subClassesOfCls5.Count);

            Assert.AreEqual(0, ontology.GetSubClassesOf(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSubClassesOfWithEquivalentObjectUnionOfDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                    new OWLEquivalentClasses([new OWLClass(new RDFResource("ex:Cls1")), 
                        new OWLObjectUnionOf([new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls6"))]) ])
                ]
            };

            List<OWLClassExpression> subClassesOfCls1 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(5, subClassesOfCls1.Count);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls6")), new OWLClass(new RDFResource("ex:Cls1"))));

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(2, subClassesOfCls2.Count);

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(1, subClassesOfCls3.Count);

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(0, subClassesOfCls4.Count);

            Assert.AreEqual(0, ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count);
            Assert.AreEqual(0, ontology.GetSubClassesOf(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSubClassesOfWithDisjointUnionOfDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                    new OWLDisjointUnion(new OWLClass(new RDFResource("ex:Cls1")),
                        [ new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls6")) ])
                ]
            };

            List<OWLClassExpression> subClassesOfCls1 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(5, subClassesOfCls1.Count);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls6")), new OWLClass(new RDFResource("ex:Cls1"))));

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(2, subClassesOfCls2.Count);

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(1, subClassesOfCls3.Count);

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(0, subClassesOfCls4.Count);

            Assert.AreEqual(0, ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count);
            Assert.AreEqual(0, ontology.GetSubClassesOf(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSuperClassesOf()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                ]
            };

            List<OWLClassExpression> superClassesOfCls1 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(0, superClassesOfCls1.Count);

            List<OWLClassExpression> superClassesOfCls2 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(1, superClassesOfCls2.Count);
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));

            List<OWLClassExpression> superClassesOfCls3 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(2, superClassesOfCls3.Count);
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls3"))));
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls3"))));

            List<OWLClassExpression> superClassesOfCls4 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(3, superClassesOfCls4.Count);
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4"))));
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls4"))));
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls4"))));

            Assert.AreEqual(0, ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count);
            Assert.AreEqual(0, ontology.GetSuperClassesOf(null).Count);
            Assert.IsFalse(ontology.CheckIsSuperClassOf(null, new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsFalse(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.AreEqual(0, (null as OWLOntology).GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSuperClassesOfWithEquivalentClassesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                    new OWLEquivalentClasses([new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls5"))]),
                    new OWLEquivalentClasses([new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls6"))])
                ]
            };

            List<OWLClassExpression> superClassesOfCls1 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(0, superClassesOfCls1.Count);

            List<OWLClassExpression> superClassesOfCls2 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(1, superClassesOfCls2.Count);

            List<OWLClassExpression> superClassesOfCls3 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(2, superClassesOfCls3.Count);

            List<OWLClassExpression> superClassesOfCls4 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(4, superClassesOfCls4.Count);

            List<OWLClassExpression> superClassesOfCls5 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5")));
            Assert.AreEqual(2, superClassesOfCls5.Count);

            Assert.AreEqual(0, ontology.GetSuperClassesOf(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetSuperClassesOfWithDisjointUnionOfDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                    new OWLDisjointUnion(new OWLClass(new RDFResource("ex:Cls4")),
                        [ new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls6")) ])
                ]
            };

            List<OWLClassExpression> superClassesOfCls1 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(0, superClassesOfCls1.Count);
            
            List<OWLClassExpression> superClassesOfCls2 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(1, superClassesOfCls2.Count);

            List<OWLClassExpression> superClassesOfCls3 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(2, superClassesOfCls3.Count);

            List<OWLClassExpression> superClassesOfCls4 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(3, superClassesOfCls4.Count);

            List<OWLClassExpression> superClassesOfCls5 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5")));
            Assert.AreEqual(1, superClassesOfCls5.Count);
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls5"))));
            Assert.AreEqual(0, ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls7"))).Count);
            Assert.AreEqual(0, ontology.GetSuperClassesOf(null).Count);
            Assert.AreEqual(0, (null as OWLOntology).GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetEquivalentClasses()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls3")) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4")) ]),
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls5")) ]),
                ]
            };

            List<OWLClassExpression> equivalentClassesOfCls1 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(4, equivalentClassesOfCls1.Count);
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls4"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls5"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls5"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls4"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls5"))));
            Assert.IsTrue(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls1"))));

            List<OWLClassExpression> equivalentClassesOfCls2 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(4, equivalentClassesOfCls2.Count);

            List<OWLClassExpression> equivalentClassesOfCls3 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(4, equivalentClassesOfCls3.Count);

            List<OWLClassExpression> equivalentClassesOfCls4 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(4, equivalentClassesOfCls4.Count);

            Assert.AreEqual(0, ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls6"))).Count);
            Assert.AreEqual(0, ontology.GetEquivalentClasses(null).Count);
            Assert.IsFalse(ontology.CheckAreEquivalentClasses(null, new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsFalse(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.AreEqual(0, (null as OWLOntology).GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }

        [TestMethod]
        public void ShouldGetDisjointClasses()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls3")) ]),
                    new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4")) ]),
                    new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls5")) ]),
                ]
            };

            List<OWLClassExpression> disjointClassesOfCls1 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(3, disjointClassesOfCls1.Count);
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls3"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls3"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls5"))));
            Assert.IsTrue(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls2"))));

            List<OWLClassExpression> disjointClassesOfCls2 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(3, disjointClassesOfCls2.Count);

            List<OWLClassExpression> disjointClassesOfCls3 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(2, disjointClassesOfCls3.Count);

            List<OWLClassExpression> disjointOfCls4 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(1, disjointOfCls4.Count);

            Assert.AreEqual(0, ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls6"))).Count);
            Assert.AreEqual(0, ontology.GetDisjointClasses(null).Count);
            Assert.IsFalse(ontology.CheckAreDisjointClasses(null, new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsFalse(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.AreEqual(0, (null as OWLOntology).GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls1"))).Count);
        }
        
        [TestMethod]
        public void ShouldGetDisjointClassesWithEquivalentClassesDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))]),
                    new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls3")) ]),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls1"))),
                ]
            };

            List<OWLClassExpression> disjointClassesOfCls1 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(1, disjointClassesOfCls1.Count);
            
            List<OWLClassExpression> disjointClassesOfCls2 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(1, disjointClassesOfCls2.Count);

            List<OWLClassExpression> disjointClassesOfCls3 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(1, disjointClassesOfCls3.Count);

            List<OWLClassExpression> disjointOfCls4 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(1, disjointOfCls4.Count);
        }

        [TestMethod]
        public void ShouldGetDisjointClassesWithSubClassOfDiscovery()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLDisjointClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4")) ])
                ]
            };

            List<OWLClassExpression> disjointClassesOfCls1 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")));
            Assert.AreEqual(1, disjointClassesOfCls1.Count);
            
            List<OWLClassExpression> disjointClassesOfCls2 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.AreEqual(1, disjointClassesOfCls2.Count);

            List<OWLClassExpression> disjointClassesOfCls3 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.AreEqual(1, disjointClassesOfCls3.Count);

            List<OWLClassExpression> disjointOfCls4 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.AreEqual(1, disjointOfCls4.Count);
        }
        #endregion
    }
}