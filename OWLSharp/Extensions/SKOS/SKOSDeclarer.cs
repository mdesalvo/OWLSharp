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

using OWLSharp.Ontology;
using RDFSharp.Model;
using System.Linq;

namespace OWLSharp.Extensions.SKOS
{
    public static class SKOSDeclarer
    {
        #region Methods
        public static OWLOntology DeclareSKOSConceptScheme(this OWLOntology ontology, RDFResource conceptSchemeUri,
            RDFResource[] concepts=null)
        {
            #region Guards
            if (conceptSchemeUri == null)
                throw new OWLException("Cannot declare concept scheme because given \"conceptSchemeUri\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME));
            ontology.DeclareEntity(new OWLNamedIndividual(conceptSchemeUri));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(conceptSchemeUri)));
            if (concepts?.Count() > 0)
            {
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME));
                foreach (RDFResource concept in concepts)
                {
                    ontology.DeclareEntity(new OWLNamedIndividual(concept));
                    ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                        new OWLNamedIndividual(concept)));
                    ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME),
                        new OWLNamedIndividual(concept),
                        new OWLNamedIndividual(conceptSchemeUri)));
                }
            }

            return ontology;
        }
        #endregion
    }
}