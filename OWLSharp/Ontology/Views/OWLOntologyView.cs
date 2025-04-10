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
    public sealed class OWLOntologyView
    {
        #region Properties
        public OWLOntology Ontology { get; }
        #endregion

        #region Ctors
        public OWLOntologyView(OWLOntology ont)
            => Ontology = ont ?? throw new OWLException("Cannot create ontology view because given \"ont\" parameter is null");
        #endregion

        #region Methods

        //Ontology
        public Task<int> AnnotationsCountAsync()
            => Task.FromResult(Ontology.Annotations.Count);

        public Task<int> ImportsCountAsync()
            => Task.FromResult(Ontology.Imports.Count);

        public Task<int> PrefixesCountAsync()
            => Task.FromResult(Ontology.Prefixes.Count);

        //Entities
        public Task<int> ClassCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLClass>().Count);

        public Task<int> AnnotationPropertyCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLAnnotationProperty>().Count);

        public Task<int> DataPropertyCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLDataProperty>().Count);

        public Task<int> ObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>().Count);

        public Task<int> DatatypeCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLDatatype>().Count);

        public Task<int> NamedIndividualCountAsync()
            => Task.FromResult(Ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>().Count);

        //Axioms

        public Task<int> AnnotationAssertionCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count);

        public Task<int> AnnotationPropertyDomainCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyDomain>().Count);

        public Task<int> AnnotationPropertyRangeCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyRange>().Count);

        public Task<int> SubAnnotationPropertyOfCountAsync()
            => Task.FromResult(Ontology.GetAnnotationAxiomsOfType<OWLSubAnnotationPropertyOf>().Count);

        public Task<int> ClassAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count);

        public Task<int> DataPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count);

        public Task<int> DifferentIndividualsCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Count);

        public Task<int> NegativeDataPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>().Count);

        public Task<int> NegativeObjectPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>().Count);

        public Task<int> ObjectPropertyAssertionCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count);

        public Task<int> SameIndividualCountAsync()
            => Task.FromResult(Ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Count);

        public Task<int> DisjointClassesCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLDisjointClasses>().Count);

        public Task<int> DisjointUnionCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLDisjointUnion>().Count);

        public Task<int> EquivalentClassesCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLEquivalentClasses>().Count);

        public Task<int> SubClassOfCountAsync()
            => Task.FromResult(Ontology.GetClassAxiomsOfType<OWLSubClassOf>().Count);

        public Task<int> DataPropertyDomainCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>().Count);

        public Task<int> DataPropertyRangeCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>().Count);

        public Task<int> DisjointDataPropertiesCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>().Count);

        public Task<int> EquivalentDataPropertiesCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>().Count);

        public Task<int> FunctionalDataPropertyCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>().Count);

        public Task<int> SubDataPropertyOfCountAsync()
            => Task.FromResult(Ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Count);

        public Task<int> DatatypeDefinitionCountAsync()
            => Task.FromResult(Ontology.DatatypeDefinitionAxioms.Count);

        public Task<int> DeclarationCountAsync()
            => Task.FromResult(Ontology.DeclarationAxioms.Count);

        public Task<int> KeyCountAsync()
            => Task.FromResult(Ontology.KeyAxioms.Count);

        public Task<int> AsymmetricObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>().Count);

        public Task<int> DisjointObjectPropertiesCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>().Count);

        public Task<int> EquivalentObjectPropertiesCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>().Count);

        public Task<int> FunctionalObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>().Count);

        public Task<int> InverseFunctionalObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>().Count);

        public Task<int> InverseObjectPropertiesCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>().Count);

        public Task<int> IrreflexiveObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>().Count);

        public Task<int> ObjectPropertyDomainCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>().Count);

        public Task<int> ObjectPropertyRangeCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>().Count);

        public Task<int> ReflexiveObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>().Count);

        public Task<int> SubObjectPropertyOfCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Count);

        public Task<int> SymmetricObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>().Count);

        public Task<int> TransitiveObjectPropertyCountAsync()
            => Task.FromResult(Ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>().Count);

        #endregion
    }
}