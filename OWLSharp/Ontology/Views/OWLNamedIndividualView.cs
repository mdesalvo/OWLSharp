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
	public class OWLNamedIndividualView
	{
		#region Properties
		public OWLNamedIndividual NamedIndividual { get; internal set; }
		internal string NamedIndividualIRI { get; set; }

		public OWLOntology Ontology { get; internal set; }
		#endregion

		#region Ctors
		public OWLNamedIndividualView(OWLNamedIndividual idv, OWLOntology ont)
		{
			NamedIndividual = idv ?? throw new OWLException("Cannot create named individual view because given \"idv\" parameter is null");
			Ontology = ont ?? throw new OWLException("Cannot create named individual view because given \"ont\" parameter is null");
			NamedIndividualIRI = NamedIndividual.GetIRI().ToString();
		}
		#endregion

		#region Methods
		public Task<List<OWLIndividualExpression>> SameIndividualsAsync()
			=> Task.Run(() => Ontology.GetSameIndividuals(NamedIndividual));

		public Task<List<OWLIndividualExpression>> DifferentIndividualsAsync()
			=> Task.Run(() => Ontology.GetDifferentIndividuals(NamedIndividual));

		public Task<List<OWLClassAssertion>> ClassAssertionsAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLClassAssertion>()
									  .Where(ax => string.Equals(ax.IndividualExpression.GetIRI().ToString(), NamedIndividualIRI))
									  .ToList()); 

		public Task<List<OWLObjectPropertyAssertion>> ObjectAssertionsAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>()
									  .Where(ax => string.Equals(ax.SourceIndividualExpression.GetIRI().ToString(), NamedIndividualIRI))
									  .ToList()); 
		
		public Task<List<OWLDataPropertyAssertion>> DataAssertionsAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
									  .Where(ax => string.Equals(ax.IndividualExpression.GetIRI().ToString(), NamedIndividualIRI))
									  .ToList()); 

		public Task<List<OWLNegativeObjectPropertyAssertion>> NegativeObjectAssertionsAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>()
									  .Where(ax => string.Equals(ax.SourceIndividualExpression.GetIRI().ToString(), NamedIndividualIRI))
									  .ToList()); 
		
		public Task<List<OWLNegativeDataPropertyAssertion>> NegativeDataAssertionsAsync()
			=> Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>()
									  .Where(ax => string.Equals(ax.IndividualExpression.GetIRI().ToString(), NamedIndividualIRI))
									  .ToList()); 

		public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
									  .Where(ann => string.Equals(ann.SubjectIRI, NamedIndividualIRI)
									  				 && ann.ValueLiteral == null && !string.IsNullOrEmpty(ann.ValueIRI))
									  .ToList());

		public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
									  .Where(ann => string.Equals(ann.SubjectIRI, NamedIndividualIRI)
									  				 && ann.ValueLiteral != null && string.IsNullOrEmpty(ann.ValueIRI))
									  .ToList());

		public Task<bool> IsDeprecatedAsync()
			=> Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
									  .Any(ann => string.Equals(ann.SubjectIRI, NamedIndividualIRI)
									  				 && ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
													 && ann.ValueLiteral != null
													 && ann.ValueLiteral.GetLiteral().Equals(RDFTypedLiteral.True)));
		#endregion
	}
}