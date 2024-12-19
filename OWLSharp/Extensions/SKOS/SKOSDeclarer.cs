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
using System.Collections.Generic;

namespace OWLSharp.Extensions.SKOS
{
    public static class SKOSDeclarer
    {
        #region Methods
        public static OWLOntology DeclareSKOSConceptScheme(this OWLOntology ontology, RDFResource conceptScheme,
            RDFResource[] concepts=null)
        {
            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot declare concept scheme because given \"conceptScheme\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME));
            ontology.DeclareEntity(new OWLNamedIndividual(conceptScheme));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT_SCHEME),
                new OWLNamedIndividual(conceptScheme)));

            if (concepts?.Length > 0)
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
                        new OWLNamedIndividual(conceptScheme)));
                }
            }

            return ontology;
        }

        public static OWLOntology DeclareSKOSConcept(this OWLOntology ontology, RDFResource concept,
            RDFPlainLiteral[] labels=null)
        {
            #region Guards
            if (concept == null)
                throw new OWLException("Cannot declare concept because given \"concept\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLNamedIndividual(concept));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(concept)));

            if (labels?.Length > 0)
            {
                ontology.DeclareEntity(new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL));

                HashSet<string> langtagLookup = new HashSet<string>();
                foreach (RDFPlainLiteral preferredLabel in labels)
                {
                    //(S14) skos:prefLabel annotation requires uniqueness of language tags foreach rdfs:Resource
                    if (langtagLookup.Contains(preferredLabel.Language))
                        throw new OWLException($"Cannot setup preferred label of concept '{concept}' because having more than one occurrence of the same language tag is not allowed!");

                    langtagLookup.Add(preferredLabel.Language);
                    ontology.DeclareAnnotationAxiom(new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        concept,
                        new OWLLiteral(preferredLabel)));
                }   
            }

            return ontology;
        }

        public static OWLOntology DeclareSKOSCollection(this OWLOntology ontology, RDFResource collection,
            RDFResource[] concepts, RDFPlainLiteral[] labels=null)
        {
            #region Guards
            if (collection == null)
                throw new OWLException("Cannot declare collection because given \"collection\" parameter is null");
            if (concepts == null)
                throw new OWLException("Cannot declare collection because given \"concepts\" parameter is null");
            if (concepts.Length == 0)
                throw new OWLException("Cannot declare collection because given \"concepts\" parameter must contain at least 1 element");
            #endregion

            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.COLLECTION));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.MEMBER));
            ontology.DeclareEntity(new OWLNamedIndividual(collection));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.COLLECTION),
                new OWLNamedIndividual(collection)));

            foreach (RDFResource concept in concepts)
            {
                ontology.DeclareSKOSConcept(concept);
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.MEMBER),
                    new OWLNamedIndividual(collection),
                    new OWLNamedIndividual(concept)));
            }

            if (labels?.Length > 0)
            {
                ontology.DeclareEntity(new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL));

                HashSet<string> langtagLookup = new HashSet<string>();
                foreach (RDFPlainLiteral preferredLabel in labels)
                {
                    //(S14) skos:prefLabel annotation requires uniqueness of language tags foreach rdfs:Resource
                    if (langtagLookup.Contains(preferredLabel.Language))
                        throw new OWLException($"Cannot setup preferred label of collection '{collection}' because having more than one occurrence of the same language tag is not allowed!");

                    langtagLookup.Add(preferredLabel.Language);
                    ontology.DeclareAnnotationAxiom(new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        collection,
                        new OWLLiteral(preferredLabel)));
                }
            }

            return ontology;
        }
        #endregion
    }
}