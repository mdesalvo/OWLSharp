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
using System.Linq;
using System.Threading.Tasks;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Views
{
	public class OWLOntologyView
	{
		#region Properties
		public OWLOntology Ontology { get; internal set; }
		#endregion

		#region Ctors
		public OWLOntologyView(OWLObjectProperty obp, OWLOntology ont)
			=> Ontology = ont ?? throw new OWLException("Cannot create ontology view because given \"ont\" parameter is null");
		#endregion

		#region Methods

		//Ontology
		public Task<int> AnnotationsCountAsync()
			=> Task.Run(() => Ontology.Annotations.Count());

		public Task<int> ImportsCountAsync()
			=> Task.Run(() => Ontology.Imports.Count());

		public Task<int> PrefixesCountAsync()
			=> Task.Run(() => Ontology.Prefixes.Count());

		//Entities
		public Task<int> ClassCountAsync()
			=> Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLClass>().Count());

		public Task<int> AnnotationPropertyCountAsync()
			=> Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLAnnotationProperty>().Count());

		public Task<int> DataPropertyCountAsync()
			=> Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLDataProperty>().Count());

		public Task<int> ObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>().Count());

		public Task<int> DatatypeCountAsync()
			=> Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLDatatype>().Count());

		public Task<int> NamedIndividualCountAsync()
			=> Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>().Count());

		//Axioms

		public Task<int> AnnotationAssertionCountAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count());

		public Task<int> AnnotationPropertyDomainCountAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyDomain>().Count());

		public Task<int> AnnotationPropertyRangeCountAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationPropertyRange>().Count());

		public Task<int> SubAnnotationPropertyOfCountAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLSubAnnotationPropertyOf>().Count());

		public Task<int> ClassAssertionCountAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count());

		public Task<int> DataPropertyAssertionCountAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count());

		public Task<int> DifferentIndividualsCountAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Count());

		public Task<int> NegativeDataPropertyAssertionCountAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>().Count());

		public Task<int> NegativeObjectPropertyAssertionCountAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>().Count());

		public Task<int> ObjectPropertyAssertionCountAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count());

		public Task<int> SameIndividualCountAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Count());

		public Task<int> DisjointClassesCountAsync()
			=> Task.Run(() => Ontology.GetClassAxiomsOfType<OWLDisjointClasses>().Count());

		public Task<int> DisjointUnionCountAsync()
			=> Task.Run(() => Ontology.GetClassAxiomsOfType<OWLDisjointUnion>().Count());

		public Task<int> EquivalentClassesCountAsync()
			=> Task.Run(() => Ontology.GetClassAxiomsOfType<OWLEquivalentClasses>().Count());

		public Task<int> SubClassOfCountAsync()
			=> Task.Run(() => Ontology.GetClassAxiomsOfType<OWLSubClassOf>().Count());

		public Task<int> DataPropertyDomainCountAsync()
			=> Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>().Count());

		public Task<int> DataPropertyRangeCountAsync()
			=> Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>().Count());

		public Task<int> DisjointDataPropertiesCountAsync()
			=> Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>().Count());

		public Task<int> EquivalentDataPropertiesCountAsync()
			=> Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>().Count());

		public Task<int> FunctionalDataPropertyCountAsync()
			=> Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>().Count());

		public Task<int> SubDataPropertyOfCountAsync()
			=> Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Count());

		public Task<int> DatatypeDefinitionCountAsync()
			=> Task.Run(() => Ontology.DatatypeDefinitionAxioms.Count());

		public Task<int> DeclarationCountAsync()
			=> Task.Run(() => Ontology.DeclarationAxioms.Count());

		public Task<int> KeyCountAsync()
			=> Task.Run(() => Ontology.KeyAxioms.Count());

		public Task<int> AsymmetricObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>().Count());

		public Task<int> DisjointObjectPropertiesCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>().Count());

		public Task<int> EquivalentObjectPropertiesCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>().Count());

		public Task<int> FunctionalObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>().Count());

		public Task<int> InverseFunctionalObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>().Count());

		public Task<int> InverseObjectPropertiesCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>().Count());

		public Task<int> IrreflexiveObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>().Count());

		public Task<int> ObjectPropertyDomainCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>().Count());

		public Task<int> ObjectPropertyRangeCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>().Count());

		public Task<int> ReflexiveObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>().Count());

		public Task<int> SubObjectPropertyOfCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Count());

		public Task<int> SymmetricObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>().Count());

		public Task<int> TransitiveObjectPropertyCountAsync()
			=> Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>().Count());

		#endregion
	}
}