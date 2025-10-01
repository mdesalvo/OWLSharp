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
    /// OWLObjectPropertyView helps at focusing on the knowledge about a given object property
    /// </summary>
    public sealed class OWLObjectPropertyView
    {
        #region Properties
        /// <summary>
        /// Represents the object property on which this view focuses
        /// </summary>
        public OWLObjectProperty ObjectProperty { get; }

        /// <summary>
        /// Represents the ontology on which this view operates
        /// </summary>
        public OWLOntology Ontology { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a view focusing on the given object property and ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLObjectPropertyView(OWLObjectProperty obp, OWLOntology ont)
        {
            ObjectProperty = obp ?? throw new OWLException($"Cannot create object property view because given '{nameof(obp)}' parameter is null");
            Ontology = ont ?? throw new OWLException($"Cannot create object property view because given '{nameof(ont)}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Enlists the sub properties of this view's object property
        /// </summary>
        public Task<List<OWLObjectPropertyExpression>> SubObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetSubObjectPropertiesOf(ObjectProperty));

        /// <summary>
        /// Enlists the super properties of this view's object property
        /// </summary>
        public Task<List<OWLObjectPropertyExpression>> SuperObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetSuperObjectPropertiesOf(ObjectProperty));

        /// <summary>
        /// Enlists the equivalent properties of this view's object property
        /// </summary>
        public Task<List<OWLObjectPropertyExpression>> EquivalentObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetEquivalentObjectProperties(ObjectProperty));

        /// <summary>
        /// Enlists the disjoint properties of this view's object property
        /// </summary>
        public Task<List<OWLObjectPropertyExpression>> DisjointObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetDisjointObjectProperties(ObjectProperty));

        /// <summary>
        /// Enlists the inverse properties of this view's object property
        /// </summary>
        public Task<List<OWLObjectPropertyExpression>> InverseObjectPropertiesAsync()
            => Task.Run(() =>
                {
                    List<OWLInverseObjectProperties> inverseObjProps = Ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();
                    return inverseObjProps.Where(ax => ax.LeftObjectPropertyExpression.GetIRI().Equals(ObjectProperty.GetIRI()))
                                          .Select(ax => ax.RightObjectPropertyExpression)
                                          .Union(inverseObjProps.Where(ax => ax.RightObjectPropertyExpression.GetIRI().Equals(ObjectProperty.GetIRI()))
                                                                .Select(ax => ax.LeftObjectPropertyExpression))
                                          .ToList();
                });

        /// <summary>
        /// Enlists the domains of this view's object property
        /// </summary>
        public Task<List<OWLClassExpression>> DomainsAsync()
            => Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>()
                                      .Where(ax => ax.ObjectPropertyExpression.GetIRI().Equals(ObjectProperty.GetIRI()))
                                      .Select(ax => ax.ClassExpression)
                                      .ToList());

        /// <summary>
        /// Enlists the ranges of this view's object property
        /// </summary>
        public Task<List<OWLClassExpression>> RangesAsync()
            => Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>()
                                      .Where(ax => ax.ObjectPropertyExpression.GetIRI().Equals(ObjectProperty.GetIRI()))
                                      .Select(ax => ax.ClassExpression)
                                      .ToList());

        /// <summary>
        /// Enlists the object assertions using this view's object property
        /// </summary>
        public Task<List<OWLObjectPropertyAssertion>> ObjectAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>(), ObjectProperty));

        /// <summary>
        /// Enlists the negative object assertions using this view's object property
        /// </summary>
        public Task<List<OWLNegativeObjectPropertyAssertion>> NegativeObjectAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectNegativeObjectAssertionsByOPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>(), ObjectProperty));

        /// <summary>
        /// Enlists the object annotations of this view's object property
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, ObjectProperty.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral == null
                                                      && !string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        /// <summary>
        /// Enlists the data annotations of this view's object property
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, ObjectProperty.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral != null
                                                      && string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        /// <summary>
        /// Answers if this view's object property is annotated as a deprecated property
        /// </summary>
        public Task<bool> IsDeprecatedAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Any(ann => string.Equals(ann.SubjectIRI, ObjectProperty.GetIRI().ToString(), StringComparison.Ordinal)
                                                    && ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
                                                    && ann.ValueLiteral != null
                                                    && ann.ValueLiteral.GetLiteral().Equals(RDFTypedLiteral.True)));

        /// <summary>
        /// Answers if this view's object property is a functional property
        /// </summary>
        public Task<bool> IsFunctionalAsync()
            => Task.Run(() => Ontology.CheckHasFunctionalObjectProperty(ObjectProperty));

        /// <summary>
        /// Answers if this view's object property is an inverse functional property
        /// </summary>
        public Task<bool> IsInverseFunctionalAsync()
            => Task.Run(() => Ontology.CheckHasInverseFunctionalObjectProperty(ObjectProperty));

        /// <summary>
        /// Answers if this view's object property is a symmetric property
        /// </summary>
        public Task<bool> IsSymmetricAsync()
            => Task.Run(() => Ontology.CheckHasSymmetricObjectProperty(ObjectProperty));

        /// <summary>
        /// Answers if this view's object property is an asymmetric property
        /// </summary>
        public Task<bool> IsAsymmetricAsync()
            => Task.Run(() => Ontology.CheckHasAsymmetricObjectProperty(ObjectProperty));

        /// <summary>
        /// Answers if this view's object property is a reflexive property
        /// </summary>
        public Task<bool> IsReflexiveAsync()
            => Task.Run(() => Ontology.CheckHasReflexiveObjectProperty(ObjectProperty));

        /// <summary>
        /// Answers if this view's object property is an irreflexive property
        /// </summary>
        public Task<bool> IsIrreflexiveAsync()
            => Task.Run(() => Ontology.CheckHasIrreflexiveObjectProperty(ObjectProperty));

        /// <summary>
        /// Answers if this view's object property is a transitive property
        /// </summary>
        public Task<bool> IsTransitiveAsync()
            => Task.Run(() => Ontology.CheckHasTransitiveObjectProperty(ObjectProperty));
        #endregion
    }
}