﻿/*
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
        #region Tests
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
        }
        
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
		}
		#endregion
    }
}