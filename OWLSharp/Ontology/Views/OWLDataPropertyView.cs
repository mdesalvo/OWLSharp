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
    /// OWLDataPropertyView helps at focusing on the knowledge about a given data property
    /// </summary>
    public sealed class OWLDataPropertyView
    {
        #region Properties
        /// <summary>
        /// Represents the data property on which this view focuses
        /// </summary>
        public OWLDataProperty DataProperty { get; }

        /// <summary>
        /// Represents the ontology on which this view operates
        /// </summary>
        public OWLOntology Ontology { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a view focusing on the given data property and ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDataPropertyView(OWLDataProperty dtp, OWLOntology ont)
        {
            DataProperty = dtp ?? throw new OWLException($"Cannot create data property view because given '{dtp}' parameter is null");
            Ontology = ont ?? throw new OWLException($"Cannot create data property view because given '{ont}' parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Enlists the sub properties of this view's data property
        /// </summary>
        public Task<List<OWLDataProperty>> SubDataPropertiesAsync()
            => Task.Run(() => Ontology.GetSubDataPropertiesOf(DataProperty));

        /// <summary>
        /// Enlists the super properties of this view's data property
        /// </summary>
        public Task<List<OWLDataProperty>> SuperDataPropertiesAsync()
            => Task.Run(() => Ontology.GetSuperDataPropertiesOf(DataProperty));

        /// <summary>
        /// Enlists the equivalent properties of this view's data property
        /// </summary>
        public Task<List<OWLDataProperty>> EquivalentDataPropertiesAsync()
            => Task.Run(() => Ontology.GetEquivalentDataProperties(DataProperty));

        /// <summary>
        /// Enlists the disjoint properties of this view's data property
        /// </summary>
        public Task<List<OWLDataProperty>> DisjointDataPropertiesAsync()
            => Task.Run(() => Ontology.GetDisjointDataProperties(DataProperty));

        /// <summary>
        /// Enlists the domains of this view's data property
        /// </summary>
        public Task<List<OWLClassExpression>> DomainsAsync()
            => Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>()
                                      .Where(ax => ax.DataProperty.GetIRI().Equals(DataProperty.GetIRI()))
                                      .Select(ax => ax.ClassExpression)
                                      .ToList());

        /// <summary>
        /// Enlists the ranges of this view's data property
        /// </summary>
        public Task<List<OWLDataRangeExpression>> RangesAsync()
            => Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>()
                                      .Where(ax => ax.DataProperty.GetIRI().Equals(DataProperty.GetIRI()))
                                      .Select(ax => ax.DataRangeExpression)
                                      .ToList());

        /// <summary>
        /// Enlists the data assertions using this view's data property
        /// </summary>
        public Task<List<OWLDataPropertyAssertion>> DataAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(), DataProperty));

        /// <summary>
        /// Enlists the negative data assertions using this view's data property
        /// </summary>
        public Task<List<OWLNegativeDataPropertyAssertion>> NegativeDataAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectNegativeDataAssertionsByDPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>(), DataProperty));

        /// <summary>
        /// Enlists the object annotations of this view's data property
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, DataProperty.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral == null && !string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        /// <summary>
        /// Enlists the data annotations of this view's data property
        /// </summary>
        public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, DataProperty.GetIRI().ToString(), StringComparison.Ordinal)
                                                      && ann.ValueLiteral != null && string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        /// <summary>
        /// Answers if this view's data property is annotated as a deprecated property
        /// </summary>
        public Task<bool> IsDeprecatedAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Any(ann => string.Equals(ann.SubjectIRI, DataProperty.GetIRI().ToString(), StringComparison.Ordinal)
                                                    && ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
                                                    && ann.ValueLiteral?.GetLiteral().Equals(RDFTypedLiteral.True) == true));

        /// <summary>
        /// Answers if this view's data property is a functional property
        /// </summary>
        public Task<bool> IsFunctionalAsync()
            => Task.Run(() => Ontology.CheckHasFunctionalDataProperty(DataProperty));
        #endregion
    }
}