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
		}

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
		}
		
		[TestMethod]
		public void ShouldGetAnnotationPropertyAxioms()
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
		}		
		#endregion
    }
}