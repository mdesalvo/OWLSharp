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

            //skos:inScheme
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

        public static OWLOntology DeclareSKOSConcept(this OWLOntology ontology, RDFResource conceptUri,
            RDFPlainLiteral[] labels=null)
        {
            #region Guards
            if (conceptUri == null)
                throw new OWLException("Cannot declare concept because given \"conceptUri\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.CONCEPT));
            ontology.DeclareEntity(new OWLNamedIndividual(conceptUri));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.CONCEPT),
                new OWLNamedIndividual(conceptUri)));

            if (labels?.Count() > 0)
            {
                ontology.DeclareEntity(new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL));

                HashSet<string> langtagLookup = new HashSet<string>();
                foreach (RDFPlainLiteral preferredLabel in labels)
                {
                    //(S14) skos:prefLabel annotation requires uniqueness of language tags foreach rdfs:Resource
                    if (langtagLookup.Contains(preferredLabel.Language))
                        throw new OWLException($"Cannot setup preferred label of concept {conceptUri} because having more than one occurrence of the same language tag is not allowed!");

                    langtagLookup.Add(preferredLabel.Language);
                    ontology.DeclareAnnotationAxiom(new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        conceptUri,
                        new OWLLiteral(preferredLabel)));
                }   
            }

            return ontology;
        }

        public static OWLOntology DeclareSKOSCollection(this OWLOntology ontology, RDFResource collectionUri,
            RDFResource[] concepts, RDFPlainLiteral[] labels=null)
        {
            #region Guards
            if (collectionUri == null)
                throw new OWLException("Cannot declare collection because given \"collectionUri\" parameter is null");
            if (concepts == null)
                throw new OWLException("Cannot declare collection because given \"concepts\" parameter is null");
            if (concepts.Length == 0)
                throw new OWLException("Cannot declare collection because given \"concepts\" parameter must contain at least 1 element");
            #endregion

            ontology.DeclareEntity(new OWLClass(RDFVocabulary.SKOS.COLLECTION));
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.SKOS.MEMBER));
            ontology.DeclareEntity(new OWLNamedIndividual(collectionUri));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.SKOS.COLLECTION),
                new OWLNamedIndividual(collectionUri)));

            foreach (RDFResource concept in concepts)
            {
                ontology.DeclareSKOSConcept(concept);
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.SKOS.MEMBER),
                    new OWLNamedIndividual(collectionUri),
                    new OWLNamedIndividual(concept)));
            }

            if (labels?.Count() > 0)
            {
                ontology.DeclareEntity(new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL));

                HashSet<string> langtagLookup = new HashSet<string>();
                foreach (RDFPlainLiteral preferredLabel in labels)
                {
                    //(S14) skos:prefLabel annotation requires uniqueness of language tags foreach rdfs:Resource
                    if (langtagLookup.Contains(preferredLabel.Language))
                        throw new OWLException($"Cannot setup preferred label of collection {collectionUri} because having more than one occurrence of the same language tag is not allowed!");

                    langtagLookup.Add(preferredLabel.Language);
                    ontology.DeclareAnnotationAxiom(new OWLAnnotationAssertion(
                        new OWLAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL),
                        collectionUri,
                        new OWLLiteral(preferredLabel)));
                }
            }

            return ontology;
        }
        #endregion
    }
}