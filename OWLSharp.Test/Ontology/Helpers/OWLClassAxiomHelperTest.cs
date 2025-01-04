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
            Assert.IsTrue(subClassOf.Count == 1);

            List<OWLEquivalentClasses> equivalentClasses = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();
            Assert.IsTrue(equivalentClasses.Count == 1);

            List<OWLDisjointClasses> disjointClasses = ontology.GetClassAxiomsOfType<OWLDisjointClasses>();
            Assert.IsTrue(disjointClasses.Count == 1);

            List<OWLDisjointUnion> disjointUnion = ontology.GetClassAxiomsOfType<OWLDisjointUnion>();
            Assert.IsTrue(disjointUnion.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetClassAxiomsOfType<OWLSubClassOf>().Count == 0);
        }

        [TestMethod]
        public void ShouldDeclareClassAxiom()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareClassAxiom(new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT)));

            Assert.IsTrue(ontology.ClassAxioms.Count == 1);
            Assert.IsTrue(ontology.CheckHasClassAxiom(new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT))));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareClassAxiom(null as OWLClassAxiom));

            ontology.DeclareClassAxiom(new OWLSubClassOf(
                new OWLClass(RDFVocabulary.FOAF.PERSON),
                new OWLClass(RDFVocabulary.FOAF.AGENT))); //will be discarded, since duplicates are not allowed
            Assert.IsTrue(ontology.ClassAxioms.Count == 1);
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
            Assert.IsTrue(subClassesOfCls1.Count == 3);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls1"))));

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(subClassesOfCls2.Count == 2);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls2"))));

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(subClassesOfCls3.Count == 1);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))));

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(subClassesOfCls4.Count == 0);

            Assert.IsTrue(ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count == 0);
            Assert.IsTrue(ontology.GetSubClassesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSubClassOf(null, new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsFalse(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue((null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(subClassesOfCls1.Count == 5);

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(subClassesOfCls2.Count == 4);

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(subClassesOfCls3.Count == 2);

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(subClassesOfCls4.Count == 0);

            List<OWLClassExpression> subClassesOfCls5 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5")));
            Assert.IsTrue(subClassesOfCls5.Count == 2);

            Assert.IsTrue(ontology.GetSubClassesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(subClassesOfCls1.Count == 5);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls6")), new OWLClass(new RDFResource("ex:Cls1"))));

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(subClassesOfCls2.Count == 2);

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(subClassesOfCls3.Count == 1);

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(subClassesOfCls4.Count == 0);

            Assert.IsTrue(ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count == 0);
            Assert.IsTrue(ontology.GetSubClassesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(subClassesOfCls1.Count == 5);
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls5")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue(ontology.CheckIsSubClassOf(new OWLClass(new RDFResource("ex:Cls6")), new OWLClass(new RDFResource("ex:Cls1"))));

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(subClassesOfCls2.Count == 2);

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(subClassesOfCls3.Count == 1);

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(subClassesOfCls4.Count == 0);

            Assert.IsTrue(ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count == 0);
            Assert.IsTrue(ontology.GetSubClassesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(superClassesOfCls1.Count == 0);

            List<OWLClassExpression> superClassesOfCls2 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(superClassesOfCls2.Count == 1);
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));

            List<OWLClassExpression> superClassesOfCls3 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(superClassesOfCls3.Count == 2);
            Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls3"))));
			Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls3"))));

            List<OWLClassExpression> superClassesOfCls4 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(superClassesOfCls4.Count == 3);
			Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4"))));
			Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls4"))));
			Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls4"))));

            Assert.IsTrue(ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count == 0);
            Assert.IsTrue(ontology.GetSuperClassesOf(null).Count == 0);
            Assert.IsFalse(ontology.CheckIsSuperClassOf(null, new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsFalse(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), null));
            Assert.IsFalse((null as OWLOntology).CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsTrue((null as OWLOntology).GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(superClassesOfCls1.Count == 0);

            List<OWLClassExpression> superClassesOfCls2 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(superClassesOfCls2.Count == 1);

            List<OWLClassExpression> superClassesOfCls3 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(superClassesOfCls3.Count == 2);

            List<OWLClassExpression> superClassesOfCls4 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(superClassesOfCls4.Count == 4);

            List<OWLClassExpression> superClassesOfCls5 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5")));
            Assert.IsTrue(superClassesOfCls5.Count == 2);

            Assert.IsTrue(ontology.GetSuperClassesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(superClassesOfCls1.Count == 0);
            
            List<OWLClassExpression> superClassesOfCls2 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(superClassesOfCls2.Count == 1);

            List<OWLClassExpression> superClassesOfCls3 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(superClassesOfCls3.Count == 2);

            List<OWLClassExpression> superClassesOfCls4 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(superClassesOfCls4.Count == 3);

			List<OWLClassExpression> superClassesOfCls5 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5")));
            Assert.IsTrue(superClassesOfCls5.Count == 1);
			Assert.IsTrue(ontology.CheckIsSuperClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls5"))));
            Assert.IsTrue(ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls7"))).Count == 0);
            Assert.IsTrue(ontology.GetSuperClassesOf(null).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(equivalentClassesOfCls1.Count == 4);
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
            Assert.IsTrue(equivalentClassesOfCls2.Count == 4);

            List<OWLClassExpression> equivalentClassesOfCls3 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(equivalentClassesOfCls3.Count == 4);

            List<OWLClassExpression> equivalentClassesOfCls4 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(equivalentClassesOfCls4.Count == 4);

            Assert.IsTrue(ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls6"))).Count == 0);
            Assert.IsTrue(ontology.GetEquivalentClasses(null).Count == 0);
            Assert.IsFalse(ontology.CheckAreEquivalentClasses(null, new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsFalse(ontology.CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue((null as OWLOntology).GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(disjointClassesOfCls1.Count == 3);
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
            Assert.IsTrue(disjointClassesOfCls2.Count == 3);

            List<OWLClassExpression> disjointClassesOfCls3 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(disjointClassesOfCls3.Count == 2);

            List<OWLClassExpression> disjointOfCls4 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(disjointOfCls4.Count == 1);

            Assert.IsTrue(ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls6"))).Count == 0);
            Assert.IsTrue(ontology.GetDisjointClasses(null).Count == 0);
            Assert.IsFalse(ontology.CheckAreDisjointClasses(null, new OWLClass(new RDFResource("ex:Cls1"))));
            Assert.IsFalse(ontology.CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")), null));
            Assert.IsFalse((null as OWLOntology).CheckAreDisjointClasses(new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2"))));
            Assert.IsTrue((null as OWLOntology).GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls1"))).Count == 0);
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
            Assert.IsTrue(disjointClassesOfCls1.Count == 1);
            
            List<OWLClassExpression> disjointClassesOfCls2 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(disjointClassesOfCls2.Count == 1);

            List<OWLClassExpression> disjointClassesOfCls3 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(disjointClassesOfCls3.Count == 1);

            List<OWLClassExpression> disjointOfCls4 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(disjointOfCls4.Count == 1);
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
            Assert.IsTrue(disjointClassesOfCls1.Count == 1);
            
            List<OWLClassExpression> disjointClassesOfCls2 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls2")));
            Assert.IsTrue(disjointClassesOfCls2.Count == 1);

            List<OWLClassExpression> disjointClassesOfCls3 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls3")));
            Assert.IsTrue(disjointClassesOfCls3.Count == 1);

            List<OWLClassExpression> disjointOfCls4 = ontology.GetDisjointClasses(new OWLClass(new RDFResource("ex:Cls4")));
            Assert.IsTrue(disjointOfCls4.Count == 1);
        }
		#endregion
    }
}