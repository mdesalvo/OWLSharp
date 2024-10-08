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
    public class OWLDeclarationHelperTest
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
			List<OWLClass> declaredClasses = ontology.GetDeclaredEntitiesOfType<OWLClass>();
			Assert.IsTrue(declaredClasses.Count == 3);
			Assert.IsTrue(ontology.CheckHasEntity<OWLClass>(new OWLClass(RDFVocabulary.FOAF.AGENT)));
			Assert.IsFalse(ontology.CheckHasEntity<OWLClass>(new OWLClass(RDFVocabulary.FOAF.DOCUMENT)));
			Assert.IsFalse(ontology.CheckHasEntity<OWLClass>(null));
			Assert.IsFalse((null as OWLOntology).CheckHasEntity<OWLClass>(new OWLClass(RDFVocabulary.FOAF.AGENT)));

            List<OWLDeclaration> datatypeDeclarations = ontology.GetDeclarationAxiomsOfType<OWLDatatype>();
            Assert.IsTrue(datatypeDeclarations.Count == 2);
			List<OWLDatatype> declaredDatatypes = ontology.GetDeclaredEntitiesOfType<OWLDatatype>();
			Assert.IsTrue(declaredDatatypes.Count == 2);

            List<OWLDeclaration> objectPropertyDeclarations = ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>();
            Assert.IsTrue(objectPropertyDeclarations.Count == 1);
			List<OWLObjectProperty> declaredObjectProperties = ontology.GetDeclaredEntitiesOfType<OWLObjectProperty>();
			Assert.IsTrue(declaredObjectProperties.Count == 1);

            List<OWLDeclaration> dataPropertyDeclarations = ontology.GetDeclarationAxiomsOfType<OWLDataProperty>();
            Assert.IsTrue(dataPropertyDeclarations.Count == 1);
			List<OWLDataProperty> declaredDataProperties = ontology.GetDeclaredEntitiesOfType<OWLDataProperty>();
			Assert.IsTrue(declaredDataProperties.Count == 1);

            List<OWLDeclaration> annotationPropertyDeclarations = ontology.GetDeclarationAxiomsOfType<OWLAnnotationProperty>();
            Assert.IsTrue(annotationPropertyDeclarations.Count == 1);
			List<OWLAnnotationProperty> declaredAnnotationProperties = ontology.GetDeclaredEntitiesOfType<OWLAnnotationProperty>();
			Assert.IsTrue(declaredAnnotationProperties.Count == 1);

            List<OWLDeclaration> individualDeclarations = ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>();
            Assert.IsTrue(individualDeclarations.Count == 1);
			List<OWLNamedIndividual> declaredIndividuals = ontology.GetDeclaredEntitiesOfType<OWLNamedIndividual>();
			Assert.IsTrue(declaredIndividuals.Count == 1);

            Assert.IsTrue((null as OWLOntology).GetDeclarationAxiomsOfType<OWLClass>().Count == 0);
			Assert.IsTrue((null as OWLOntology).GetDeclaredEntitiesOfType<OWLClass>().Count == 0);
        }
		#endregion
	}
}