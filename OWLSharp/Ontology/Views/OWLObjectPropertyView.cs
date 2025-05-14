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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    public sealed class OWLObjectPropertyView
    {
        #region Properties
        public OWLObjectProperty ObjectProperty { get; }
        internal string ObjectPropertyIRI { get; }

        public OWLOntology Ontology { get; }
        #endregion

        #region Ctors
        public OWLObjectPropertyView(OWLObjectProperty obp, OWLOntology ont)
        {
            ObjectProperty = obp ?? throw new OWLException("Cannot create object property view because given \"obp\" parameter is null");
            Ontology = ont ?? throw new OWLException("Cannot create object property view because given \"ont\" parameter is null");
            ObjectPropertyIRI = ObjectProperty.GetIRI().ToString();
        }
        #endregion

        #region Methods
        public Task<List<OWLObjectPropertyExpression>> SubObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetSubObjectPropertiesOf(ObjectProperty));

        public Task<List<OWLObjectPropertyExpression>> SuperObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetSuperObjectPropertiesOf(ObjectProperty));

        public Task<List<OWLObjectPropertyExpression>> EquivalentObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetEquivalentObjectProperties(ObjectProperty));

        public Task<List<OWLObjectPropertyExpression>> DisjointObjectPropertiesAsync()
            => Task.Run(() => Ontology.GetDisjointObjectProperties(ObjectProperty));

        public Task<List<OWLObjectPropertyExpression>> InverseObjectPropertiesAsync()
            => Task.Run(() =>
                {
                     List<OWLInverseObjectProperties> inverseObjProps = Ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();
                    return inverseObjProps.Where(ax => string.Equals(ax.LeftObjectPropertyExpression.GetIRI().ToString(), ObjectPropertyIRI))
                                            .Select(ax => ax.RightObjectPropertyExpression)
                                            .Union(inverseObjProps.Where(ax => string.Equals(ax.RightObjectPropertyExpression.GetIRI().ToString(), ObjectPropertyIRI))
                                                                    .Select(ax => ax.LeftObjectPropertyExpression))
                                            .ToList();
                });

        public Task<List<OWLClassExpression>> DomainsAsync()
            => Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>()
                                      .Where(ax => string.Equals(ax.ObjectPropertyExpression.GetIRI().ToString(), ObjectPropertyIRI))
                                      .Select(ax => ax.ClassExpression)
                                      .ToList());

        public Task<List<OWLClassExpression>> RangesAsync()
            => Task.Run(() => Ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>()
                                      .Where(ax => string.Equals(ax.ObjectPropertyExpression.GetIRI().ToString(), ObjectPropertyIRI))
                                      .Select(ax => ax.ClassExpression)
                                      .ToList());

        public Task<List<OWLObjectPropertyAssertion>> ObjectAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>(), ObjectProperty));

        public Task<List<OWLNegativeObjectPropertyAssertion>> NegativeObjectAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectNegativeObjectAssertionsByOPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>(), ObjectProperty));

        public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, ObjectPropertyIRI)
                                                       && ann.ValueLiteral == null && !string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, ObjectPropertyIRI)
                                                       && ann.ValueLiteral != null && string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        public Task<bool> IsDeprecatedAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Any(ann => string.Equals(ann.SubjectIRI, ObjectPropertyIRI)
                                                    && ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
                                                    && ann.ValueLiteral?.GetLiteral().Equals(RDFTypedLiteral.True) == true));

        public Task<bool> IsFunctionalAsync()
            => Task.Run(() => Ontology.CheckHasFunctionalObjectProperty(ObjectProperty));

        public Task<bool> IsInverseFunctionalAsync()
            => Task.Run(() => Ontology.CheckHasInverseFunctionalObjectProperty(ObjectProperty));

        public Task<bool> IsSymmetricAsync()
            => Task.Run(() => Ontology.CheckHasSymmetricObjectProperty(ObjectProperty));

        public Task<bool> IsAsymmetricAsync()
            => Task.Run(() => Ontology.CheckHasAsymmetricObjectProperty(ObjectProperty));

        public Task<bool> IsReflexiveAsync()
            => Task.Run(() => Ontology.CheckHasReflexiveObjectProperty(ObjectProperty));

        public Task<bool> IsIrreflexiveAsync()
            => Task.Run(() => Ontology.CheckHasIrreflexiveObjectProperty(ObjectProperty));

        public Task<bool> IsTransitiveAsync()
            => Task.Run(() => Ontology.CheckHasTransitiveObjectProperty(ObjectProperty));
        #endregion
    }
}