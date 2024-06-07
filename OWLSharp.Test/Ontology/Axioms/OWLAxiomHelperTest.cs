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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Ontology.Axioms
{
    [TestClass]
    public class OWLAxiomHelperTest
    {
        #region Tests (DeclarationAxioms)
        [TestMethod]
        public void ShouldGetDeclarationAxioms()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.INTEGER)),
                    new OWLDeclaration(new OWLDatatype(RDFVocabulary.XSD.STRING)),
                    new OWLDeclaration(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.AGE)),
                    new OWLDeclaration(new OWLAnnotationProperty(RDFVocabulary.DC.CREATOR)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark")))
                ]
            };

            List<OWLDeclaration> classDeclarations = ontology.GetDeclarationAxiomsOfType<OWLClass>();
            Assert.IsTrue(classDeclarations.Count == 3);

            List<OWLDeclaration> datatypeDeclarations = ontology.GetDeclarationAxiomsOfType<OWLDatatype>();
            Assert.IsTrue(datatypeDeclarations.Count == 2);

            List<OWLDeclaration> objectPropertyDeclarations = ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>();
            Assert.IsTrue(objectPropertyDeclarations.Count == 1);

            List<OWLDeclaration> dataPropertyDeclarations = ontology.GetDeclarationAxiomsOfType<OWLDataProperty>();
            Assert.IsTrue(dataPropertyDeclarations.Count == 1);

            List<OWLDeclaration> annotationPropertyDeclarations = ontology.GetDeclarationAxiomsOfType<OWLAnnotationProperty>();
            Assert.IsTrue(annotationPropertyDeclarations.Count == 1);

            List<OWLDeclaration> individualDeclarations = ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>();
            Assert.IsTrue(individualDeclarations.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetDeclarationAxiomsOfType<OWLClass>().Count == 0);
        }
        #endregion

        #region Tests (ClassAxioms)
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

            Assert.IsTrue(ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count == 0);
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
        public void ShouldGetSubClassesOfDirectOnly()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                ]
            };

            List<OWLClassExpression> subClassesOfCls1 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1")), true);
            Assert.IsTrue(subClassesOfCls1.Count == 1);

            List<OWLClassExpression> subClassesOfCls2 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls2")), true);
            Assert.IsTrue(subClassesOfCls2.Count == 1);

            List<OWLClassExpression> subClassesOfCls3 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls3")), true);
            Assert.IsTrue(subClassesOfCls3.Count == 1);

            List<OWLClassExpression> subClassesOfCls4 = ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls4")), true);
            Assert.IsTrue(subClassesOfCls4.Count == 0);

            Assert.IsTrue(ontology.GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls5")), true).Count == 0);
            Assert.IsTrue(ontology.GetSubClassesOf(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSubClassesOf(new OWLClass(new RDFResource("ex:Cls1")), true).Count == 0);
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

            Assert.IsTrue(ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5"))).Count == 0);
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
        public void ShouldGetSuperClassesOfDirectOnly()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls1"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls3")), new OWLClass(new RDFResource("ex:Cls2"))),
                    new OWLSubClassOf(new OWLClass(new RDFResource("ex:Cls4")), new OWLClass(new RDFResource("ex:Cls3"))),
                ]
            };

            List<OWLClassExpression> superClassesOfCls1 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1")), true);
            Assert.IsTrue(superClassesOfCls1.Count == 0);

            List<OWLClassExpression> superClassesOfCls2 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls2")), true);
            Assert.IsTrue(superClassesOfCls2.Count == 1);

            List<OWLClassExpression> superClassesOfCls3 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls3")), true);
            Assert.IsTrue(superClassesOfCls3.Count == 1);

            List<OWLClassExpression> superClassesOfCls4 = ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls4")), true);
            Assert.IsTrue(superClassesOfCls4.Count == 1);

            Assert.IsTrue(ontology.GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls5")), true).Count == 0);
            Assert.IsTrue(ontology.GetSuperClassesOf(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetSuperClassesOf(new OWLClass(new RDFResource("ex:Cls1")), true).Count == 0);
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
        public void ShouldGetEquivalentClassesDirectOnly()
        {
            OWLOntology ontology = new OWLOntology()
            {
                ClassAxioms = [
                    new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls3")) ]),
					new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cls1")), new OWLClass(new RDFResource("ex:Cls4")) ]),
					new OWLEquivalentClasses([ new OWLClass(new RDFResource("ex:Cls2")), new OWLClass(new RDFResource("ex:Cls5")) ]),
                ]
            };

            List<OWLClassExpression> equivalentClassesOfCls1 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), true);
            Assert.IsTrue(equivalentClassesOfCls1.Count == 3);

            List<OWLClassExpression> equivalentClassesOfCls2 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls2")), true);
            Assert.IsTrue(equivalentClassesOfCls2.Count == 3);

            List<OWLClassExpression> equivalentClassesOfCls3 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls3")), true);
            Assert.IsTrue(equivalentClassesOfCls3.Count == 2);

            List<OWLClassExpression> equivalentClassesOfCls4 = ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls4")), true);
            Assert.IsTrue(equivalentClassesOfCls4.Count == 1);

            Assert.IsTrue(ontology.GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls6")), true).Count == 0);
            Assert.IsTrue(ontology.GetEquivalentClasses(null, true).Count == 0);
            Assert.IsTrue((null as OWLOntology).GetEquivalentClasses(new OWLClass(new RDFResource("ex:Cls1")), true).Count == 0);
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

        #region Tests (DataPropertyAxioms)
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
        #endregion

        #region Tests (ObjectPropertyAxioms)
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
        #endregion

        #region Tests (AnnotationAxioms)
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
        #endregion

        #region Tests (AssertionAxioms)
        [TestMethod]
		public void ShouldGetAssertionAxioms()
		{
			OWLOntology ontology = new OWLOntology()
            {
                AssertionAxioms = [
                    new OWLClassAssertion(new OWLClass(RDFVocabulary.FOAF.AGENT), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
					new OWLDataPropertyAssertion(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLDifferentIndividuals([ new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLNamedIndividual(new RDFResource("ex:Carl")) ]),
					new OWLNegativeDataPropertyAssertion(new OWLDataProperty(RDFVocabulary.FOAF.AGE), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
					new OWLNegativeObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
					new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob"))),
					new OWLSameIndividual([ new OWLNamedIndividual(new RDFResource("ex:Alice")), new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLNamedIndividual(new RDFResource("ex:Carl")) ])
                ]
            };

            List<OWLClassAssertion> classAssertion = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            Assert.IsTrue(classAssertion.Count == 1);

			List<OWLDataPropertyAssertion> dataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            Assert.IsTrue(dataPropertyAssertion.Count == 1);

			List<OWLDifferentIndividuals> differentIndividuals = ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>();
            Assert.IsTrue(differentIndividuals.Count == 1);

			List<OWLNegativeDataPropertyAssertion> negativeDataPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>();
            Assert.IsTrue(negativeDataPropertyAssertion.Count == 1);

			List<OWLNegativeObjectPropertyAssertion> negativeObjectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>();
            Assert.IsTrue(negativeObjectPropertyAssertion.Count == 1);

			List<OWLObjectPropertyAssertion> objectPropertyAssertion = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            Assert.IsTrue(objectPropertyAssertion.Count == 1);

			List<OWLSameIndividual> sameIndividual = ontology.GetAssertionAxiomsOfType<OWLSameIndividual>();
            Assert.IsTrue(sameIndividual.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 0);
        }
		#endregion
    }
}