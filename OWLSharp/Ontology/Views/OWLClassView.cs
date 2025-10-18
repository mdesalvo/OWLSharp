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
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLClassView helps at focusing on the knowledge about a given class
    /// </summary>
    public sealed class OWLClassView
    {
        #region Properties
        /// <summary>
        /// Represents the class on which this view focuses
        /// </summary>
        public OWLClass Class { get; }

        /// <summary>
        /// Represents the ontology on which this view operates
        /// </summary>
        public OWLOntology Ontology { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a view focusing on the given class and ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLClassView(OWLClass cls, OWLOntology ont)
        {
            Class = cls ?? throw new OWLException($"Cannot create class view because given '{nameof(cls)}' parameter is null");
            Ontology = ont ?? throw new OWLException($"Cannot create class view because given '{nameof(ont)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Enlists the sub classes of this view's class
        /// </summary>
        public Task<List<OWLClassExpression>> SubClassesAsync()
            => Task.Run(() => Ontology.GetSubClassesOf(Class));

        /// <summary>
        /// Enlists the super classes of this view's class
        /// </summary>
        public Task<List<OWLClassExpression>> SuperClassesAsync()
            => Task.Run(() => Ontology.GetSuperClassesOf(Class));

        /// <summary>
        /// Enlists the equivalent classes of this view's class
        /// </summary>
        public Task<List<OWLClassExpression>> EquivalentClassesAsync()
            => Task.Run(() => Ontology.GetEquivalentClasses(Class));

        /// <summary>
        /// Enlists the disjoint classes of this view's class
        /// </summary>
        public Task<List<OWLClassExpression>> DisjointClassesAsync()
            => Task.Run(() => Ontology.GetDisjointClasses(Class));

        /// <summary>
        /// Enlists the individuals of this view's class
        /// </summary>
        public Task<List<OWLIndividualExpression>> IndividualsAsync()
            => Task.Run(() => Ontology.GetIndividualsOf(Class));

        /// <summary>
        /// Enlists the negative individuals of this view's class
        /// </summary>
        public Task<List<OWLNamedIndividual>> NegativeIndividualsAsync()
            => Task.Run(() => Ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
                                      .Select(dax => (OWLNamedIndividual)dax.Entity)
                                      .Where(idv => Ontology.CheckIsNegativeIndividualOf(Class, idv))
                                      .ToList());

        /// <summary>
        /// Enlists the key properties of this view's class
        /// </summary>
        public Task<List<IOWLEntity>> KeysAsync()
            => Task.Run(() => Ontology.KeyAxioms.Where(kax => kax.ClassExpression.GetIRI().Equals(Class.GetIRI()))
                                                .SelectMany(kax => kax.DataProperties.Union(kax.ObjectPropertyExpressions.Cast<IOWLEntity>()))
                                                .ToList());

        /// <summary>
        /// Enlists the object annotations of this view's class
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, Class.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral == null
                                                      && !string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        /// <summary>
        /// Enlists the data annotations of this view's class
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, Class.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral != null
                                                      && string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        /// <summary>
        /// Answers if this view's class is annotated as a deprecated class
        /// </summary>
        public Task<bool> IsDeprecatedAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Any(ann => string.Equals(ann.SubjectIRI, Class.GetIRI().ToString(), StringComparison.Ordinal)
                                                    && ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
                                                    && ann.ValueLiteral != null
                                                    && ann.ValueLiteral.GetLiteral().Equals(RDFTypedLiteral.True)));
        #endregion
    }
}