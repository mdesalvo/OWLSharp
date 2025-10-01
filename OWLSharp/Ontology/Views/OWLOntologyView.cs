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

using System.Threading.Tasks;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLOntologyView helps at focusing on the knowledge about a given ontology
    /// </summary>
    public sealed class OWLOntologyView
    {
        #region Properties
        /// <summary>
        /// Represents the ontology on which this view operates
        /// </summary>
        public OWLOntology Ontology { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a view focusing on the given ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLOntologyView(OWLOntology ont)
            => Ontology = ont ?? throw new OWLException($"Cannot create ontology view because given '{nameof(ont)}' parameter is null");
        #endregion

        #region Methods (Ontology)
        /// <summary>
        /// Counts the annotations of this view's ontology
        /// </summary>
        public Task<int> AnnotationsCountAsync()
            => Task.FromResult(Ontology.Annotations.Count);

        /// <summary>
        /// Counts the import directives of this view's ontology
        /// </summary>
        public Task<int> ImportsCountAsync()
            => Task.FromResult(Ontology.Imports.Count);

        /// <summary>
        /// Counts the prefix directives of this view's ontology
        /// </summary>
        public Task<int> PrefixesCountAsync()
            => Task.FromResult(Ontology.Prefixes.Count);
        #endregion

        #region Methods (Entities)
        /// <summary>
        /// Counts the declared classes of this view's ontology
        /// </summary>
        public Task<int> ClassCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLClass>().Count);

        /// <summary>
        /// Counts the declared annotation properties of this view's ontology
        /// </summary>
        public Task<int> AnnotationPropertyCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLAnnotationProperty>().Count);

        /// <summary>
        /// Counts the declared data properties of this view's ontology
        /// </summary>
        public Task<int> DataPropertyCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLDataProperty>().Count);

        /// <summary>
        /// Counts the declared object properties of this view's ontology
        /// </summary>
        public Task<int> ObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>().Count);

        /// <summary>
        /// Counts the declared datatypes of this view's ontology
        /// </summary>
        public Task<int> DatatypeCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLDatatype>().Count);

        /// <summary>
        /// Counts the declared named individuals of this view's ontology
        /// </summary>
        public Task<int> NamedIndividualCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>().Count);
        #endregion

        #region Methods (Axioms)
        /// <summary>
        /// Counts the OWLAnnotationAssertion axioms of this view's ontology
        /// </summary>
        public Task<int> AnnotationAssertionCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count);

        /// <summary>
        /// Counts the OWLAnnotationPropertyDomain axioms of this view's ontology
        /// </summary>
        public Task<int> AnnotationPropertyDomainCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyDomain>().Count);

        /// <summary>
        /// Counts the OWLAnnotationPropertyRange axioms of this view's ontology
        /// </summary>
        public Task<int> AnnotationPropertyRangeCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyRange>().Count);

        /// <summary>
        /// Counts the OWLSubAnnotationPropertyOf axioms of this view's ontology
        /// </summary>
        public Task<int> SubAnnotationPropertyOfCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLSubAnnotationPropertyOf>().Count);

        /// <summary>
        /// Counts the OWLClassAssertion axioms of this view's ontology
        /// </summary>
        public Task<int> ClassAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count);

        /// <summary>
        /// Counts the OWLDataPropertyAssertion axioms of this view's ontology
        /// </summary>
        public Task<int> DataPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count);

        /// <summary>
        /// Counts the OWLDifferentIndividuals axioms of this view's ontology
        /// </summary>
        public Task<int> DifferentIndividualsCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Count);

        /// <summary>
        /// Counts the OWLNegativeDataPropertyAssertion axioms of this view's ontology
        /// </summary>
        public Task<int> NegativeDataPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>().Count);

        /// <summary>
        /// Counts the OWLNegativeObjectPropertyAssertion axioms of this view's ontology
        /// </summary>
        public Task<int> NegativeObjectPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>().Count);

        /// <summary>
        /// Counts the OWLObjectPropertyAssertion axioms of this view's ontology
        /// </summary>
        public Task<int> ObjectPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count);

        /// <summary>
        /// Counts the OWLSameIndividual axioms of this view's ontology
        /// </summary>
        public Task<int> SameIndividualCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Count);

        /// <summary>
        /// Counts the OWLDisjointClasses axioms of this view's ontology
        /// </summary>
        public Task<int> DisjointClassesCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLDisjointClasses>().Count);

        /// <summary>
        /// Counts the OWLDisjointUnion axioms of this view's ontology
        /// </summary>
        public Task<int> DisjointUnionCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLDisjointUnion>().Count);

        /// <summary>
        /// Counts the OWLEquivalentClasses axioms of this view's ontology
        /// </summary>
        public Task<int> EquivalentClassesCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLEquivalentClasses>().Count);

        /// <summary>
        /// Counts the OWLSubClassOf axioms of this view's ontology
        /// </summary>
        public Task<int> SubClassOfCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLSubClassOf>().Count);

        /// <summary>
        /// Counts the OWLDataPropertyDomain axioms of this view's ontology
        /// </summary>
        public Task<int> DataPropertyDomainCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>().Count);

        /// <summary>
        /// Counts the OWLDataPropertyRange axioms of this view's ontology
        /// </summary>
        public Task<int> DataPropertyRangeCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>().Count);

        /// <summary>
        /// Counts the OWLDisjointDataProperties axioms of this view's ontology
        /// </summary>
        public Task<int> DisjointDataPropertiesCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>().Count);

        /// <summary>
        /// Counts the OWLEquivalentDataProperties axioms of this view's ontology
        /// </summary>
        public Task<int> EquivalentDataPropertiesCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>().Count);

        /// <summary>
        /// Counts the OWLFunctionalDataProperty axioms of this view's ontology
        /// </summary>
        public Task<int> FunctionalDataPropertyCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>().Count);

        /// <summary>
        /// Counts the OWLSubDataPropertyOf axioms of this view's ontology
        /// </summary>
        public Task<int> SubDataPropertyOfCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Count);

        /// <summary>
        /// Counts the OWLDatatypeDefinition axioms of this view's ontology
        /// </summary>
        public Task<int> DatatypeDefinitionCountAsync()
            => Task.FromResult(Ontology.DatatypeDefinitionAxioms.Count);

        /// <summary>
        /// Counts the OWLDeclaration axioms of this view's ontology
        /// </summary>
        public Task<int> DeclarationCountAsync()
            => Task.FromResult(Ontology.DeclarationAxioms.Count);

        /// <summary>
        /// Counts the OWLHasKey axioms of this view's ontology
        /// </summary>
        public Task<int> KeyCountAsync()
            => Task.FromResult(Ontology.KeyAxioms.Count);

        /// <summary>
        /// Counts the OWLAsymmetricObjectProperty axioms of this view's ontology
        /// </summary>
        public Task<int> AsymmetricObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>().Count);

        /// <summary>
        /// Counts the OWLDisjointObjectProperties axioms of this view's ontology
        /// </summary>
        public Task<int> DisjointObjectPropertiesCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>().Count);

        /// <summary>
        /// Counts the OWLEquivalentObjectProperties axioms of this view's ontology
        /// </summary>
        public Task<int> EquivalentObjectPropertiesCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>().Count);

        /// <summary>
        /// Counts the OWLFunctionalObjectProperty axioms of this view's ontology
        /// </summary>
        public Task<int> FunctionalObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>().Count);

        /// <summary>
        /// Counts the OWLInverseFunctionalObjectProperty axioms of this view's ontology
        /// </summary>
        public Task<int> InverseFunctionalObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>().Count);

        /// <summary>
        /// Counts the OWLInverseObjectProperties axioms of this view's ontology
        /// </summary>
        public Task<int> InverseObjectPropertiesCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>().Count);

        /// <summary>
        /// Counts the OWLIrreflexiveObjectProperty axioms of this view's ontology
        /// </summary>
        public Task<int> IrreflexiveObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>().Count);

        /// <summary>
        /// Counts the OWLObjectPropertyDomain axioms of this view's ontology
        /// </summary>
        public Task<int> ObjectPropertyDomainCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>().Count);

        /// <summary>
        /// Counts the OWLObjectPropertyRange axioms of this view's ontology
        /// </summary>
        public Task<int> ObjectPropertyRangeCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>().Count);

        /// <summary>
        /// Counts the OWLReflexiveObjectProperty axioms of this view's ontology
        /// </summary>
        public Task<int> ReflexiveObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>().Count);

        /// <summary>
        /// Counts the OWLSubObjectPropertyOf axioms of this view's ontology
        /// </summary>
        public Task<int> SubObjectPropertyOfCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Count);

        /// <summary>
        /// Counts the OWLSymmetricObjectProperty axioms of this view's ontology
        /// </summary>
        public Task<int> SymmetricObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>().Count);

        /// <summary>
        /// Counts the OWLTransitiveObjectProperty axioms of this view's ontology
        /// </summary>
        public Task<int> TransitiveObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>().Count);
        #endregion
    }
}