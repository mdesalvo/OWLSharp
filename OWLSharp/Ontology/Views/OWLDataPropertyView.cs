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
    public class OWLDataPropertyView
    {
        #region Properties
        public OWLDataProperty DataProperty { get; internal set; }
        internal string DataPropertyIRI { get; set; }

        public OWLOntology Ontology { get; internal set; }
        #endregion

        #region Ctors
        public OWLDataPropertyView(OWLDataProperty dtp, OWLOntology ont)
        {
            DataProperty = dtp ?? throw new OWLException("Cannot create data property view because given \"dtp\" parameter is null");
            Ontology = ont ?? throw new OWLException("Cannot create data property view because given \"ont\" parameter is null");
            DataPropertyIRI = DataProperty.GetIRI().ToString();
        }
        #endregion

        #region Methods
        public Task<List<OWLDataProperty>> SubDataPropertiesAsync()
            => Task.Run(() => Ontology.GetSubDataPropertiesOf(DataProperty));

        public Task<List<OWLDataProperty>> SuperDataPropertiesAsync()
            => Task.Run(() => Ontology.GetSuperDataPropertiesOf(DataProperty));

        public Task<List<OWLDataProperty>> EquivalentDataPropertiesAsync()
            => Task.Run(() => Ontology.GetEquivalentDataProperties(DataProperty));

        public Task<List<OWLDataProperty>> DisjointDataPropertiesAsync()
            => Task.Run(() => Ontology.GetDisjointDataProperties(DataProperty));

        public Task<List<OWLClassExpression>> DomainsAsync()
            => Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>()
                                      .Where(ax => string.Equals(ax.DataProperty.GetIRI().ToString(), DataPropertyIRI))
                                      .Select(ax => ax.ClassExpression)
                                      .ToList());

        public Task<List<OWLDataRangeExpression>> RangesAsync()
            => Task.Run(() => Ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>()
                                      .Where(ax => string.Equals(ax.DataProperty.GetIRI().ToString(), DataPropertyIRI))
                                      .Select(ax => ax.DataRangeExpression)
                                      .ToList());

        public Task<List<OWLDataPropertyAssertion>> DataAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(), DataProperty));

        public Task<List<OWLNegativeDataPropertyAssertion>> NegativeDataAssertionsAsync()
            => Task.Run(() => OWLAssertionAxiomHelper.SelectNegativeDataAssertionsByDPEX(
                                Ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>(), DataProperty));

        public Task<List<OWLAnnotationAssertion>> ObjectAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, DataPropertyIRI)
                                                       && ann.ValueLiteral == null && !string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        public Task<List<OWLAnnotationAssertion>> DataAnnotationsAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Where(ann => string.Equals(ann.SubjectIRI, DataPropertyIRI)
                                                       && ann.ValueLiteral != null && string.IsNullOrEmpty(ann.ValueIRI))
                                      .ToList());

        public Task<bool> IsDeprecatedAsync()
            => Task.Run(() => Ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                      .Any(ann => string.Equals(ann.SubjectIRI, DataPropertyIRI)
                                                       && ann.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
                                                     && ann.ValueLiteral != null
                                                     && ann.ValueLiteral.GetLiteral().Equals(RDFTypedLiteral.True)));

        public Task<bool> IsFunctionalAsync()
            => Task.Run(() => Ontology.CheckHasFunctionalDataProperty(DataProperty));
        #endregion
    }
}