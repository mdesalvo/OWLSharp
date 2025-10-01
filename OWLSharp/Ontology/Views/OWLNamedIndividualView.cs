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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLNamedIndividualView helps at focusing on the knowledge about a given named individual
    /// </summary>
    public sealed class OWLNamedIndividualView
    {
        #region Properties
        /// <summary>
        /// Represents the named individual on which this view focuses
        /// </summary>
        public OWLNamedIndividual NamedIndividual { get; }

        /// <summary>
        /// Represents the ontology on which this view operates
        /// </summary>
        public OWLOntology Ontology { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a view focusing on the given named individual and ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLNamedIndividualView(OWLNamedIndividual idv, OWLOntology ont)
        {
            NamedIndividual = idv ?? throw new OWLException($"Cannot create named individual view because given '{nameof(idv)}' parameter is null");
            Ontology = ont ?? throw new OWLException($"Cannot create named individual view because given '{nameof(ont)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Enlists the same individuals of this view's named individual
        /// </summary>
        public Task<List<OWLIndividualExpression>> SameIndividualsAsync()
            => Task.Run(() => Ontology.GetSameIndividuals(NamedIndividual));

        /// <summary>
        /// Enlists the different individuals of this view's named individual
        /// </summary>
        public Task<List<OWLIndividualExpression>> DifferentIndividualsAsync()
            => Task.Run(() => Ontology.GetDifferentIndividuals(NamedIndividual));

        /// <summary>
        /// Enlists the class assertions of this view's named individual
        /// </summary>
        public Task<List<OWLClassAssertion>> ClassAssertionsAsync()
            => Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLClassAssertion>()
                                      .Where(ax => ax.IndividualExpression.GetIRI().Equals(NamedIndividual.GetIRI()))
                                      .ToList());

        /// <summary>
        /// Enlists the object assertions using this view's named individual
        /// </summary>
        public Task<List<OWLObjectPropertyAssertion>> ObjectAssertionsAsync()
            => Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>()
                                      .Where(ax => ax.SourceIndividualExpression.GetIRI().Equals(NamedIndividual.GetIRI()))
                                      .ToList());

        /// <summary>
        /// Enlists the data assertions using this view's named individual
        /// </summary>
        public Task<List<OWLDataPropertyAssertion>> DataAssertionsAsync()
            => Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
                                      .Where(ax => ax.IndividualExpression.GetIRI().Equals(NamedIndividual.GetIRI()))
                                      .ToList());

        /// <summary>
        /// Enlists the negative object assertions using this view's named individual
        /// </summary>
        public Task<List<OWLNegativeObjectPropertyAssertion>> NegativeObjectAssertionsAsync()
            => Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>()
                                      .Where(ax => ax.SourceIndividualExpression.GetIRI().Equals(NamedIndividual.GetIRI()))
                                      .ToList());

        /// <summary>
        /// Enlists the negative data assertions using this view's named individual
        /// </summary>
        public Task<List<OWLNegativeDataPropertyAssertion>> NegativeDataAssertionsAsync()
            => Task.Run(() => Ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>()
                                      .Where(ax => ax.IndividualExpression.GetIRI().Equals(NamedIndividual.GetIRI()))
                                      .ToList());

        /// <summary>
        /// Enlists the object annotations of this view's named individual
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, NamedIndividual.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral == null
                                                      && !string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        /// <summary>
        /// Enlists the data annotations of this view's named individual
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, NamedIndividual.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral != null
                                                      && string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());
        #endregion
    }
}