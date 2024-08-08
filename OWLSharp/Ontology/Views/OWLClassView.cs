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
	public class OWLClassView
	{
		#region Properties
		public OWLClass Class { get; internal set; }
		internal string ClassIRI { get; set; }

		public OWLOntology Ontology { get; internal set; }
		#endregion

		#region Ctors
		public OWLClassView(OWLClass cls, OWLOntology ont)
		{
			Class = cls ?? throw new OWLException("Cannot create class view because given \"cls\" parameter is null");
			Ontology = ont ?? throw new OWLException("Cannot create class view because given \"ont\" parameter is null");
			ClassIRI = Class.GetIRI().ToString();
		}
		#endregion

		#region Methods
		public Task<List<OWLClassExpression>> SubClassesAsync()
			=> Task.Run(() => Ontology.GetSubClassesOf(Class));

		public Task<List<OWLClassExpression>> SuperClassesAsync()
			=> Task.Run(() => Ontology.GetSuperClassesOf(Class));

		public Task<List<OWLClassExpression>> EquivalentClassesAsync()
			=> Task.Run(() => Ontology.GetEquivalentClasses(Class));

		public Task<List<OWLClassExpression>> DisjointClassesAsync()
			=> Task.Run(() => Ontology.GetDisjointClasses(Class));

		public Task<List<OWLIndividualExpression>> IndividualsAsync()
			=> Task.Run(() => Ontology.GetIndividualsOf(Class));

		public Task<List<OWLNamedIndividual>> NegativeIndividualsAsync()
			=> Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
									  .Select(dax => (OWLNamedIndividual)dax.Expression)
									  .Where(idv => Ontology.CheckIsNegativeIndividualOf(Class, idv))
									  .ToList());

		public Task<List<IOWLEntity>> KeysAsync()
			=> Task.Run(() => Ontology.KeyAxioms.Where(kax => string.Equals(kax.ClassExpression.GetIRI().ToString(), ClassIRI))
												.SelectMany(kax => kax.DataProperties.Cast<IOWLEntity>()
																	.Union(kax.ObjectPropertyExpressions.Cast<IOWLEntity>()))
												.ToList());

		public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
									  .Where(ann => string.Equals(ann.SubjectIRI, ClassIRI)
									  				 && ann.ValueLiteral == null && !string.IsNullOrEmpty(ann.ValueIRI))
									  .ToList());

		public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
									  .Where(ann => string.Equals(ann.SubjectIRI, ClassIRI)
									  				 && ann.ValueLiteral != null && string.IsNullOrEmpty(ann.ValueIRI))
									  .ToList());

		public Task<bool> IsDeprecatedAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
									  .Any(ann => string.Equals(ann.SubjectIRI, ClassIRI)
									  				 && ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
													 && ann.ValueLiteral != null
													 && ann.ValueLiteral.GetLiteral().Equals(RDFTypedLiteral.True)));
		#endregion
	}
}