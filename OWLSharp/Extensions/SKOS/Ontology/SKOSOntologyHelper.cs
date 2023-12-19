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

using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSOntologyHelper contains methods for declaring and analyzing relations describing SKOS ontologies
    /// </summary>
    public static class SKOSOntologyHelper
    {
        #region Declarer
        /// <summary>
        /// Count of the SKOS concept schemes
        /// </summary>
        public static long GetConceptSchemesCount(this OWLOntology ontology)
        {
            long count = 0;
            IEnumerator<RDFResource> conceptSchemes = ontology.GetConceptSchemesEnumerator();
            while (conceptSchemes.MoveNext())
                count++;
            return count;
        }

        /// <summary>
        /// Count of the SKOS concepts
        /// </summary>
        public static long GetConceptsCount(this OWLOntology ontology)
        {
            long count = 0;
            IEnumerator<RDFResource> concepts = ontology.GetConceptsEnumerator();
            while (concepts.MoveNext())
                count++;
            return count;
        }

        /// <summary>
        /// Count of the SKOS collections
        /// </summary>
        public static long GetCollectionsCount(this OWLOntology ontology)
        {
            long count = 0;
            IEnumerator<RDFResource> collections = ontology.GetCollectionsEnumerator();
            while (collections.MoveNext())
                count++;
            return count;
        }

        /// <summary>
        /// Count of the SKOS ordered collections
        /// </summary>
        public static long GetOrderedCollectionsCount(this OWLOntology ontology)
        {
            long count = 0;
            IEnumerator<RDFResource> orderedCollections = ontology.GetOrderedCollectionsEnumerator();
            while (orderedCollections.MoveNext())
                count++;
            return count;
        }

        /// <summary>
        /// Count of the SKOS-XL labels
        /// </summary>
        public static long GetLabelsCount(this OWLOntology ontology)
        {
            long count = 0;
            IEnumerator<RDFResource> labels = ontology.GetLabelsEnumerator();
            while (labels.MoveNext())
                count++;
            return count;
        }

        /// <summary>
        /// Gets the enumerator on the SKOS concept schemes for iteration
        /// </summary>
        public static IEnumerator<RDFResource> GetConceptSchemesEnumerator(this OWLOntology ontology)
            => ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT_SCHEME, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Gets the enumerator on the SKOS concepts for iteration
        /// </summary>
        public static IEnumerator<RDFResource> GetConceptsEnumerator(this OWLOntology ontology)
            => ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Gets the enumerator on the SKOS collections for iteration
        /// </summary>
        public static IEnumerator<RDFResource> GetCollectionsEnumerator(this OWLOntology ontology)
            => ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Gets the enumerator on the SKOS ordered collections for iteration
        /// </summary>
        public static IEnumerator<RDFResource> GetOrderedCollectionsEnumerator(this OWLOntology ontology)
            => ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Gets the enumerator on the SKOS-XL labels for iteration
        /// </summary>
        public static IEnumerator<RDFResource> GetLabelsEnumerator(this OWLOntology ontology)
            => ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.SKOSXL.LABEL, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Declares the given skos:ConceptScheme instance
        /// </summary>
        public static OWLOntology DeclareConceptScheme(this OWLOntology ontology, RDFResource skosConceptScheme)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:ConceptScheme instance because given \"ontology\" parameter is null");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot declare skos:ConceptScheme instance because given \"skosConceptScheme\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            ontology.Data.DeclareIndividual(skosConceptScheme);
            ontology.Data.DeclareIndividualType(skosConceptScheme, RDFVocabulary.SKOS.CONCEPT_SCHEME);

            return ontology;
        }

        /// <summary>
        /// Declares the given skos:Concept instance
        /// </summary>
        public static OWLOntology DeclareConcept(this OWLOntology ontology, RDFResource skosConcept, RDFResource skosConceptScheme)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:Concept instance because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:Concept instance because given \"skosConcept\" parameter is null");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot declare skos:Concept instance because given \"skosConceptScheme\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            DeclareConceptScheme(ontology, skosConceptScheme);
            ontology.Data.DeclareIndividual(skosConcept);
            ontology.Data.DeclareIndividualType(skosConcept, RDFVocabulary.SKOS.CONCEPT);
            ontology.Data.DeclareObjectAssertion(skosConcept, RDFVocabulary.SKOS.IN_SCHEME, skosConceptScheme);

            return ontology;
        }

        /// <summary>
        /// Declares the given skos:Collection instance (but not its concepts or subcollections, which must be declared apart)
        /// </summary>
        public static OWLOntology DeclareCollection(this OWLOntology ontology, RDFResource skosCollection, List<RDFResource> skosConcepts, RDFResource skosConceptScheme)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:Collection instance because given \"ontology\" parameter is null");
            if (skosCollection == null)
                throw new OWLException("Cannot declare skos:Collection instance because given \"skosCollection\" parameter is null");
            if (skosConcepts == null)
                throw new OWLException("Cannot declare skos:Collection instance because given \"skosConcepts\" parameter is null");
            if (skosConcepts.Count == 0)
                throw new OWLException("Cannot declare skos:Collection instance because given \"skosConcepts\" parameter is an empty list");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot declare skos:Collection instance because given \"skosConceptScheme\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            DeclareConceptScheme(ontology, skosConceptScheme);
            ontology.Data.DeclareIndividual(skosCollection);
            ontology.Data.DeclareIndividualType(skosCollection, RDFVocabulary.SKOS.COLLECTION);
            ontology.Data.DeclareObjectAssertion(skosCollection, RDFVocabulary.SKOS.IN_SCHEME, skosConceptScheme);
            foreach (RDFResource skosConcept in skosConcepts)
                ontology.Data.DeclareObjectAssertion(skosCollection, RDFVocabulary.SKOS.MEMBER, skosConcept);

            return ontology;
        }

        /// <summary>
        /// Declares the given skos:OrderedCollection instance (but not its concepts, which must be declared apart)
        /// </summary>
        public static OWLOntology DeclareOrderedCollection(this OWLOntology ontology, RDFResource skosOrderedCollection, List<RDFResource> skosConcepts, RDFResource skosConceptScheme)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:OrderedCollection instance because given \"ontology\" parameter is null");
            if (skosOrderedCollection == null)
                throw new OWLException("Cannot declare skos:OrderedCollection instance because given \"skosOrderedCollection\" parameter is null");
            if (skosConcepts == null)
                throw new OWLException("Cannot declare skos:OrderedCollection instance because given \"skosConcepts\" parameter is null");
            if (skosConcepts.Count == 0)
                throw new OWLException("Cannot declare skos:OrderedCollection instance because given \"skosConcepts\" parameter is an empty list");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot declare skos:OrderedCollection instance because given \"skosConceptScheme\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            DeclareConceptScheme(ontology, skosConceptScheme);
            ontology.Data.DeclareIndividual(skosOrderedCollection);
            ontology.Data.DeclareIndividualType(skosOrderedCollection, RDFVocabulary.SKOS.ORDERED_COLLECTION);
            ontology.Data.DeclareObjectAssertion(skosOrderedCollection, RDFVocabulary.SKOS.IN_SCHEME, skosConceptScheme);
            RDFCollection rdfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (RDFResource skosConcept in skosConcepts)
                rdfCollection.AddItem(skosConcept);
            ontology.Data.ABoxGraph.AddCollection(rdfCollection);
            ontology.Data.DeclareObjectAssertion(skosOrderedCollection, RDFVocabulary.SKOS.MEMBER_LIST, rdfCollection.ReificationSubject);

            return ontology;
        }

        /// <summary>
        /// Declares the given skosxl:Label instance
        /// </summary>
        public static OWLOntology DeclareLabel(this OWLOntology ontology, RDFResource skosLabel, RDFResource skosConceptScheme)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skosxl:Label instance because given \"ontology\" parameter is null");
            if (skosLabel == null)
                throw new OWLException("Cannot declare skosxl:Label instance because given \"skosLabel\" parameter is null");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot declare skosxl:Label instance because given \"skosConceptScheme\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            DeclareConceptScheme(ontology, skosConceptScheme);
            ontology.Data.DeclareIndividual(skosLabel);
            ontology.Data.DeclareIndividualType(skosLabel, RDFVocabulary.SKOS.SKOSXL.LABEL);
            ontology.Data.DeclareObjectAssertion(skosLabel, RDFVocabulary.SKOS.IN_SCHEME, skosConceptScheme);

            return ontology;
        }

        //ANNOTATIONS

        /// <summary>
        /// Annotates the given concept scheme with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateConceptScheme(this OWLOntology ontology, RDFResource skosConceptScheme, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate concept scheme because given \"ontology\" parameter is null");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot annotate concept scheme because given \"skosConceptScheme\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConceptScheme, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given concept scheme with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateConceptScheme(this OWLOntology ontology, RDFResource skosConceptScheme, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate concept scheme because given \"ontology\" parameter is null");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot annotate concept scheme because given \"skosConceptScheme\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConceptScheme, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given concept with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateConcept(this OWLOntology ontology, RDFResource skosConcept, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate concept because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot annotate concept because given \"skosConcept\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate concept because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate concept because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate concept because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given concept with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateConcept(this OWLOntology ontology, RDFResource skosConcept, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate concept because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot annotate concept because given \"skosConcept\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate concept because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate concept because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate concept because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given collection with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateCollection(this OWLOntology ontology, RDFResource skosCollection, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate collection because given \"ontology\" parameter is null");
            if (skosCollection == null)
                throw new OWLException("Cannot annotate collection because given \"skosCollection\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate collection because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate collection because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate collection because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosCollection, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given collection with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateCollection(this OWLOntology ontology, RDFResource skosCollection, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate collection because given \"ontology\" parameter is null");
            if (skosCollection == null)
                throw new OWLException("Cannot annotate collection because given \"skosCollection\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate collection because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate collection because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate collection because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosCollection, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given ordered collection with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateOrderedCollection(this OWLOntology ontology, RDFResource skosOrderedCollection, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate ordered collection because given \"ontology\" parameter is null");
            if (skosOrderedCollection == null)
                throw new OWLException("Cannot annotate ordered collection because given \"skosOrderedCollection\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate ordered collection because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate ordered collection because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate ordered collection because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosOrderedCollection, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given ordered collection with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateOrderedCollection(this OWLOntology ontology, RDFResource skosOrderedCollection, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate ordered collection because given \"ontology\" parameter is null");
            if (skosOrderedCollection == null)
                throw new OWLException("Cannot annotate ordered collection because given \"skosOrderedCollection\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate ordered collection because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate ordered collection because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate ordered collection because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosOrderedCollection, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given label with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateLabel(this OWLOntology ontology, RDFResource skosLabel, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate label because given \"ontology\" parameter is null");
            if (skosLabel == null)
                throw new OWLException("Cannot annotate label because given \"skosLabel\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate label because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosLabel, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given label with the given generic annotation property
        /// </summary>
        public static OWLOntology AnnotateLabel(this OWLOntology ontology, RDFResource skosLabel, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot annotate label because given \"ontology\" parameter is null");
            if (skosLabel == null)
                throw new OWLException("Cannot annotate label because given \"skosLabel\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate label because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosLabel, annotationProperty, annotationValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given concept with the given documentation property
        /// </summary>
        public static OWLOntology DocumentConcept(this OWLOntology ontology, RDFResource skosConcept, SKOSEnums.SKOSDocumentationTypes skosDocumentationType, RDFLiteral noteValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot document concept because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot document concept because given \"skosConcept\" parameter is null");
            if (noteValue == null)
                throw new OWLException("Cannot document concept because given \"noteValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            switch (skosDocumentationType)
            {
                case SKOSEnums.SKOSDocumentationTypes.ChangeNote:
                    ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.CHANGE_NOTE, noteValue));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.Definition:
                    ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.DEFINITION, noteValue));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.EditorialNote:
                    ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.EDITORIAL_NOTE, noteValue));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.Example:
                    ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.EXAMPLE, noteValue));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.HistoryNote:
                    ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.HISTORY_NOTE, noteValue));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.Note:
                    ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.NOTE, noteValue));
                    break;
                case SKOSEnums.SKOSDocumentationTypes.ScopeNote:
                    ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SCOPE_NOTE, noteValue));
                    break;
            }

            return ontology;
        }

        /// <summary>
        /// Annotates the given concept with the given linguistic annotation (preferred)
        /// </summary>
        public static OWLOntology AnnotateConceptPreferredLabel(this OWLOntology ontology, RDFResource skosConcept, RDFPlainLiteral preferredLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckPreferredLabelCompatibility(skosConcept, preferredLabelValue);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:prefLabel annotation because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:prefLabel annotation because given \"skosConcept\" parameter is null");
            if (preferredLabelValue == null)
                throw new OWLException("Cannot declare skos:prefLabel annotation because given \"preferredLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the O-BOX
                ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.PREF_LABEL, preferredLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("PrefLabel relation between concept '{0}' and value '{1}' cannot be declared because it would violate SKOS integrity", skosConcept, preferredLabelValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given concept with the given linguistic annotation (alternative)
        /// </summary>
        public static OWLOntology AnnotateConceptAlternativeLabel(this OWLOntology ontology, RDFResource skosConcept, RDFPlainLiteral alternativeLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckAlternativeLabelCompatibility(skosConcept, alternativeLabelValue);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:altLabel annotation because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:altLabel annotation because given \"skosConcept\" parameter is null");
            if (alternativeLabelValue == null)
                throw new OWLException("Cannot declare skos:altLabel annotation because given \"alternativeLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the O-BOX
                ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.ALT_LABEL, alternativeLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("AltLabel relation between concept '{0}' and value '{1}' cannot be declared because it would violate SKOS integrity", skosConcept, alternativeLabelValue));

            return ontology;
        }

        /// <summary>
        /// Annotates the given concept with the given linguistic annotation (hidden)
        /// </summary>
        public static OWLOntology AnnotateConceptHiddenLabel(this OWLOntology ontology, RDFResource skosConcept, RDFPlainLiteral hiddenLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckHiddenLabelCompatibility(skosConcept, hiddenLabelValue);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:hiddenLabel annotation because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:hiddenLabel annotation because given \"skosConcept\" parameter is null");
            if (hiddenLabelValue == null)
                throw new OWLException("Cannot declare skos:hiddenLabel annotation because given \"hiddenLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the O-BOX
                ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.HIDDEN_LABEL, hiddenLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("HiddenLabel relation between concept '{0}' and value '{1}' cannot be declared because it would violate SKOS integrity", skosConcept, hiddenLabelValue));

            return ontology;
        }

        //RELATIONS

        /// <summary>
        /// Declares the existence of the given "TopConceptOf(skosConcept,skosConceptScheme)" relation
        /// </summary>
        public static OWLOntology DeclareTopConcept(this OWLOntology ontology, RDFResource skosConcept, RDFResource skosConceptScheme)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:topConceptOf relation because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:topConceptOf relation because given \"skosConcept\" parameter is null");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot declare skos:topConceptOf relation because given \"skosConceptScheme\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            DeclareConcept(ontology, skosConcept, skosConceptScheme);
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.TOP_CONCEPT_OF, skosConceptScheme));

            //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConceptScheme, RDFVocabulary.SKOS.HAS_TOP_CONCEPT, skosConcept).SetInference());

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "SemanticRelation(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareSemanticRelatedConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:semanticRelation relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:semanticRelation relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:semanticRelation relation because given \"rightConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.SEMANTIC_RELATION, rightConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "MappingRelation(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareMappingRelatedConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:mappingRelation relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:mappingRelation relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:mappingRelation relation because given \"rightConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.MAPPING_RELATION, rightConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "Related(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareRelatedConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:related relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:related relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:related relation because given \"rightConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.RELATED, rightConcept));

            //Also add an automatic A-BOX inference exploiting symmetry of skos:related relation
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.RELATED, leftConcept).SetInference());

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "Broader(childConcept,motherConcept)" relation
        /// </summary>
        public static OWLOntology DeclareBroaderConcepts(this OWLOntology ontology, RDFResource childConcept, RDFResource motherConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckBroaderCompatibility(childConcept, motherConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:broader relation because given \"ontology\" parameter is null");
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:broader relation because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:broader relation because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:broader relation because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER, motherConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER, childConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("Broader relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", childConcept, motherConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "BroaderTransitive(childConcept,motherConcept)" relation
        /// </summary>
        public static OWLOntology DeclareBroaderTransitiveConcepts(this OWLOntology ontology, RDFResource childConcept, RDFResource motherConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckBroaderCompatibility(childConcept, motherConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:broaderTransitive relation because given \"ontology\" parameter is null");
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:broaderTransitive relation because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:broaderTransitive relation because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:broaderTransitive relation because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER_TRANSITIVE, motherConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER_TRANSITIVE, childConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("BroaderTransitive relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", childConcept, motherConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "Narrower(motherConcept,childConcept)" relation
        /// </summary>
        public static OWLOntology DeclareNarrowerConcepts(this OWLOntology ontology, RDFResource motherConcept, RDFResource childConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckNarrowerCompatibility(motherConcept, childConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:narrower relation because given \"ontology\" parameter is null");
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:narrower relation because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:narrower relation because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:narrower relation because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER, childConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER, motherConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("Narrower relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", motherConcept, childConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "NarrowerTransitive(motherConcept,childConcept)" relation
        /// </summary>
        public static OWLOntology DeclareNarrowerTransitiveConcepts(this OWLOntology ontology, RDFResource motherConcept, RDFResource childConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckNarrowerCompatibility(motherConcept, childConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:narrowerTransitive relation because given \"ontology\" parameter is null");
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:narrowerTransitive relation because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:narrowerTransitive relation because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:narrowerTransitive relation because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER_TRANSITIVE, childConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER_TRANSITIVE, motherConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("NarrowerTransitive relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", motherConcept, childConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "CloseMatch(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareCloseMatchConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckCloseOrExactMatchCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:closeMatch relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:closeMatch relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:closeMatch relation because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:closeMatch relation because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.CLOSE_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting simmetry of skos:closeMatch relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.CLOSE_MATCH, leftConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("CloseMatch relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", leftConcept, rightConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "ExactMatch(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareExactMatchConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckCloseOrExactMatchCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:exactMatch relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:exactMatch relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:exactMatch relation because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:exactMatch relation because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.EXACT_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting simmetry of skos:exactMatch relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.EXACT_MATCH, leftConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("ExactMatch relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", leftConcept, rightConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "BroadMatch(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareBroadMatchConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckBroaderCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:broadMatch relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:broadMatch relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:broadMatch relation because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:broadMatch relation because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.BROAD_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.NARROW_MATCH, leftConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("BroadMatch relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", leftConcept, rightConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "NarrowMatch(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareNarrowMatchConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckNarrowerCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:narrowMatch relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:narrowMatch relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:narrowMatch relation because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:narrowMatch relation because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.NARROW_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.BROAD_MATCH, leftConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("NarrowMatch relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", leftConcept, rightConcept));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "RelatedMatch(leftConcept,rightConcept)" relation
        /// </summary>
        public static OWLOntology DeclareRelatedMatchConcepts(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckRelatedCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:relatedMatch relation because given \"ontology\" parameter is null");
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:relatedMatch relation because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:relatedMatch relation because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:relatedMatch relation because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.RELATED_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting simmetry of skos:relatedMatch relation
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.RELATED_MATCH, leftConcept).SetInference());
            }
            else
                OWLEvents.RaiseWarning(string.Format("RelatedMatch relation between concept '{0}' and concept '{1}' cannot be declared because it would violate SKOS integrity", leftConcept, rightConcept));

            return ontology;
        }

        /// <summary>
        ///  Declares the existence of the given "Notation(skosConcept,notationValue)" relation
        /// </summary>
        public static OWLOntology DeclareConceptNotation(this OWLOntology ontology, RDFResource skosConcept, RDFLiteral notationValue)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skos:notation relation because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:notation relation because given \"skosConcept\" parameter is null");
            if (notationValue == null)
                throw new OWLException("Cannot declare skos:notation relation because given \"notationValue\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.NOTATION, notationValue));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "PrefLabel(skosConcept,skosxlLabel) ^ LiteralForm(skosxlLabel,preferredLabelValue)" relations
        /// </summary>
        public static OWLOntology DeclareConceptPreferredLabel(this OWLOntology ontology, RDFResource skosConcept, RDFResource skosxlLabel, RDFPlainLiteral preferredLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckPreferredLabelCompatibility(skosConcept, preferredLabelValue);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skosxl:prefLabel relation to the concept scheme because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skosxl:prefLabel relation to the concept scheme because given \"skosConcept\" parameter is null");
            if (skosxlLabel == null)
                throw new OWLException("Cannot declare skosxl:prefLabel relation to the concept scheme because given \"skosxlLabel\" parameter is null");
            if (preferredLabelValue == null)
                throw new OWLException("Cannot declare skosxl:prefLabel relation to the concept scheme because given \"preferredLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, skosxlLabel));
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, preferredLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("PrefLabel relation between concept '{0}' and label '{1}' with value '{2}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, skosxlLabel, preferredLabelValue));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "AltLabel(skosConcept,skosxlLabel) ^ LiteralForm(skosxlLabel,alternativeLabelValue)" relations
        /// </summary>
        public static OWLOntology DeclareConceptAlternativeLabel(this OWLOntology ontology, RDFResource skosConcept, RDFResource skosxlLabel, RDFPlainLiteral alternativeLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckAlternativeLabelCompatibility(skosConcept, alternativeLabelValue);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skosxl:altLabel relation to the concept scheme because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skosxl:altLabel relation to the concept scheme because given \"skosConcept\" parameter is null");
            if (skosxlLabel == null)
                throw new OWLException("Cannot declare skosxl:altLabel relation to the concept scheme because given \"skosxlLabel\" parameter is null");
            if (alternativeLabelValue == null)
                throw new OWLException("Cannot declare skosxl:altLabel relation to the concept scheme because given \"alternativeLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, skosxlLabel));
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, alternativeLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("AltLabel relation between concept '{0}' and label '{1}' with value '{2}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, skosxlLabel, alternativeLabelValue));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "HiddenLabel(skosConcept,skosxlLabel) ^ LiteralForm(skosxlLabel,hiddenLabelValue)" relations
        /// </summary>
        public static OWLOntology DeclareConceptHiddenLabel(this OWLOntology ontology, RDFResource skosConcept, RDFResource skosxlLabel, RDFPlainLiteral hiddenLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => ontology.CheckHiddenLabelCompatibility(skosConcept, hiddenLabelValue);
            #endregion

            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skosxl:hiddenLabel relation to the concept scheme because given \"ontology\" parameter is null");
            if (skosConcept == null)
                throw new OWLException("Cannot declare skosxl:hiddenLabel relation to the concept scheme because given \"skosConcept\" parameter is null");
            if (skosxlLabel == null)
                throw new OWLException("Cannot declare skosxl:hiddenLabel relation to the concept scheme because given \"skosxlLabel\" parameter is null");
            if (hiddenLabelValue == null)
                throw new OWLException("Cannot declare skosxl:hiddenLabel relation to the concept scheme because given \"hiddenLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, skosxlLabel));
                ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, hiddenLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("HiddenLabel relation between concept '{0}' and label '{1}' with value '{2}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, skosxlLabel, hiddenLabelValue));

            return ontology;
        }

        /// <summary>
        /// Declares the existence of the given "LabelRelation(leftLabel,rightLabel)" relation
        /// </summary>
        public static OWLOntology DeclareRelatedLabels(this OWLOntology ontology, RDFResource leftLabel, RDFResource rightLabel)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot declare skosxl:labelRelation relation because given \"ontology\" parameter is null");
            if (leftLabel == null)
                throw new OWLException("Cannot declare skosxl:labelRelation relation because given \"leftLabel\" parameter is null");
            if (rightLabel == null)
                throw new OWLException("Cannot declare skosxl:labelRelation relation because given \"rightLabel\" parameter is null");
            if (leftLabel.Equals(rightLabel))
                throw new OWLException("Cannot declare skosxl:labelRelation relation because given \"leftLabel\" parameter refers to the same label as the given \"rightLabel\" parameter");
            #endregion

            //Add knowledge to the A-BOX
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftLabel, RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, rightLabel));

            //Also add an automatic A-BOX inference exploiting simmetry of skosxl:labelRelation relation
            ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightLabel, RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, leftLabel).SetInference());

            return ontology;
        }
        #endregion

        #region Analyzer
        /// <summary>
        /// Checks for the existence of the given skos:ConceptScheme declaration
        /// </summary>
        public static bool CheckHasConceptScheme(this OWLOntology ontology, RDFResource skosConceptScheme)
        {
            bool conceptSchemeFound = false;
            if (skosConceptScheme != null && ontology != null)
            {
                IEnumerator<RDFResource> conceptSchemesEnumerator = ontology.GetConceptSchemesEnumerator();
                while (!conceptSchemeFound && conceptSchemesEnumerator.MoveNext())
                    conceptSchemeFound = conceptSchemesEnumerator.Current.Equals(skosConceptScheme);
            }
            return conceptSchemeFound;
        }

        /// <summary>
        /// Checks for the existence of the given skos:Concept declaration
        /// </summary>
        public static bool CheckHasConcept(this OWLOntology ontology, RDFResource skosConcept)
        {
            bool conceptFound = false;
            if (skosConcept != null && ontology != null)
            {
                IEnumerator<RDFResource> conceptsEnumerator = ontology.GetConceptsEnumerator();
                while (!conceptFound && conceptsEnumerator.MoveNext())
                    conceptFound = conceptsEnumerator.Current.Equals(skosConcept);
            }
            return conceptFound;
        }

        /// <summary>
        /// Checks for the existence of the given skos:Collection declaration
        /// </summary>
        public static bool CheckHasCollection(this OWLOntology ontology, RDFResource skosCollection)
        {
            bool collectionFound = false;
            if (skosCollection != null && ontology != null)
            {
                IEnumerator<RDFResource> collectionsEnumerator = ontology.GetCollectionsEnumerator();
                while (!collectionFound && collectionsEnumerator.MoveNext())
                    collectionFound = collectionsEnumerator.Current.Equals(skosCollection);
            }
            return collectionFound;
        }

        /// <summary>
        /// Checks for the existence of the given skos:OrderedCollection declaration
        /// </summary>
        public static bool CheckHasOrderedCollection(this OWLOntology ontology, RDFResource skosOrderedCollection)
        {
            bool orderedCollectionFound = false;
            if (skosOrderedCollection != null && ontology != null)
            {
                IEnumerator<RDFResource> orderedCollectionsEnumerator = ontology.GetOrderedCollectionsEnumerator();
                while (!orderedCollectionFound && orderedCollectionsEnumerator.MoveNext())
                    orderedCollectionFound = orderedCollectionsEnumerator.Current.Equals(skosOrderedCollection);
            }
            return orderedCollectionFound;
        }

        /// <summary>
        /// Checks for the existence of the given skosxl:Label declaration
        /// </summary>
        public static bool CheckHasLabel(this OWLOntology ontology, RDFResource skosLabel)
        {
            bool labelFound = false;
            if (skosLabel != null && ontology != null)
            {
                IEnumerator<RDFResource> labelsEnumerator = ontology.GetLabelsEnumerator();
                while (!labelFound && labelsEnumerator.MoveNext())
                    labelFound = labelsEnumerator.Current.Equals(skosLabel);
            }
            return labelFound;
        }

        /// <summary>
        /// Checks for the existence of the given skosxl:Label having the given skosxl:literalForm
        /// </summary>
        public static bool CheckHasLabelWithLiteralForm(this OWLOntology ontology, RDFResource skosxlLabel, RDFLiteral skosxlLiteralFormValue)
            => CheckHasLabel(ontology, skosxlLabel)
                && ontology.Data.ABoxGraph[skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, null, skosxlLiteralFormValue].TriplesCount > 0;

        /// <summary>
        /// Gets the direct and indirect skos:Concept instances which are members of the given skos:Collection
        /// </summary>
        public static List<RDFResource> GetCollectionMembers(this OWLOntology ontology, RDFResource skosCollection)
            => GetCollectionMembers(ontology, skosCollection, new Dictionary<long, RDFResource>());

        /// <summary>
        /// Gets the skos:Concept instances which are members of the given skos:OrderedCollection
        /// </summary>
        public static List<RDFResource> GetOrderedCollectionMembers(this OWLOntology ontology, RDFResource skosOrderedCollection)
        {
            List<RDFResource> orderedCollectionMembers = new List<RDFResource>();

            if (skosOrderedCollection != null && ontology != null)
            {
                foreach (RDFTriple memberListRelation in ontology.Data.ABoxGraph[skosOrderedCollection, RDFVocabulary.SKOS.MEMBER_LIST, null, null])
                {
                    RDFCollection skosOrderedCollectionMembers = RDFModelUtilities.DeserializeCollectionFromGraph(ontology.Data.ABoxGraph, (RDFResource)memberListRelation.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    if (skosOrderedCollectionMembers.ItemsCount > 0)
                        skosOrderedCollectionMembers.Items.ForEach(item => orderedCollectionMembers.Add((RDFResource)item));
                }
            }

            return orderedCollectionMembers;
        }

        /// <summary>
        /// Gets the direct and indirect skos:Concept instances which are members of the given skos:Collection (internal recursive version)
        /// </summary>
        internal static List<RDFResource> GetCollectionMembers(this OWLOntology ontology, RDFResource skosCollection, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> collectionMembers = new List<RDFResource>();

            if (skosCollection != null && ontology != null)
            {
                #region visitContext
                if (!visitContext.ContainsKey(skosCollection.PatternMemberID))
                    visitContext.Add(skosCollection.PatternMemberID, skosCollection);
                else
                    return collectionMembers;
                #endregion

                foreach (RDFResource collectionMember in ontology.Data.ABoxGraph[skosCollection, RDFVocabulary.SKOS.MEMBER, null, null]
                                                           .Select(t => t.Object)
                                                           .OfType<RDFResource>())
                {
                    //skos:Collection
                    if (ontology.Data.ABoxGraph[collectionMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null].TriplesCount > 0)
                        collectionMembers.AddRange(GetCollectionMembers(ontology, collectionMember, visitContext));

                    //skos:OrderedCollection
                    else if (ontology.Data.ABoxGraph[collectionMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION, null].TriplesCount > 0)
                        collectionMembers.AddRange(GetOrderedCollectionMembers(ontology, collectionMember));

                    //skos:Concept
                    else
                        collectionMembers.Add(collectionMember);
                }
            }

            return RDFQueryUtilities.RemoveDuplicates(collectionMembers);
        }

        /// <summary>
        /// Checks for the existence of "HasTopConcept(skosConceptScheme,skosConcept)" relations
        /// </summary>
        public static bool CheckHasTopConcept(this OWLOntology ontology, RDFResource skosConceptScheme, RDFResource skosConcept)
            => skosConceptScheme != null && skosConcept != null && ontology != null && ontology.Data.ABoxGraph[skosConceptScheme, RDFVocabulary.SKOS.HAS_TOP_CONCEPT, skosConcept, null].TriplesCount > 0;

        /// <summary>
        /// Checks for the existence of "Broader(childConcept,motherConcept)" relations
        /// </summary>
        public static bool CheckHasBroaderConcept(this OWLOntology ontology, RDFResource childConcept, RDFResource motherConcept)
            => childConcept != null && motherConcept != null && ontology != null && ontology.GetBroaderConcepts(childConcept).Any(concept => concept.Equals(motherConcept));

        /// <summary>
        /// Analyzes "Broader(skosConceptScheme, X)" relations of the concept scheme to answer the broader concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetBroaderConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> broaderConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:broader concepts
                foreach (RDFTriple broaderRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.BROADER, null, null])
                    broaderConcepts.Add((RDFResource)broaderRelation.Object);

                //Get skos:broaderTransitive concepts
                broaderConcepts.AddRange(ontology.GetBroaderConceptsInternal(skosConcept, new Dictionary<long, RDFResource>()));
            }

            return broaderConcepts;
        }

        /// <summary>
        /// Subsumes the "skos:broaderTransitive" taxonomy to discover direct and indirect broader concepts of the given skos:Concept
        /// </summary>
        internal static List<RDFResource> GetBroaderConceptsInternal(this OWLOntology ontology, RDFResource skosConcept, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> broaderTransitiveConcepts = new List<RDFResource>();

            #region visitContext
            if (!visitContext.ContainsKey(skosConcept.PatternMemberID))
                visitContext.Add(skosConcept.PatternMemberID, skosConcept);
            else
                return broaderTransitiveConcepts;
            #endregion

            #region Discovery
            //Find broader concepts linked to the given one with skos:broaderTransitive relation
            foreach (RDFTriple broaderTransitiveRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.BROADER_TRANSITIVE, null, null])
                broaderTransitiveConcepts.Add((RDFResource)broaderTransitiveRelation.Object);
            #endregion

            // Inference: BROADERTRANSITIVE(A,B) ^ BROADERTRANSITIVE(B,C) -> BROADERTRANSITIVE(A,C)
            foreach (RDFResource broaderTransitiveConcept in broaderTransitiveConcepts.ToList())
                broaderTransitiveConcepts.AddRange(ontology.GetBroaderConceptsInternal(broaderTransitiveConcept, visitContext));

            return broaderTransitiveConcepts;
        }

        /// <summary>
        /// Checks for the existence of "Narrower(motherConcept,childConcept)" relations
        /// </summary>
        public static bool CheckHasNarrowerConcept(this OWLOntology ontology, RDFResource motherConcept, RDFResource childConcept)
            => childConcept != null && motherConcept != null && ontology != null && ontology.GetNarrowerConcepts(motherConcept).Any(concept => concept.Equals(childConcept));

        /// <summary>
        /// Analyzes "Narrower(skosConceptScheme, X)" relations of the concept scheme to answer the narrower concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetNarrowerConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> narrowerConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:narrower concepts
                foreach (RDFTriple narrowerRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NARROWER, null, null])
                    narrowerConcepts.Add((RDFResource)narrowerRelation.Object);

                //Get skos:narrowerTransitive concepts
                narrowerConcepts.AddRange(ontology.GetNarrowerConceptsInternal(skosConcept, new Dictionary<long, RDFResource>()));
            }

            return narrowerConcepts;
        }

        /// <summary>
        /// Subsumes the "skos:narrowerTransitive" taxonomy to discover direct and indirect narrower concepts of the given skos:Concept
        /// </summary>
        internal static List<RDFResource> GetNarrowerConceptsInternal(this OWLOntology ontology, RDFResource skosConcept, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> narrowerTransitiveConcepts = new List<RDFResource>();

            #region visitContext
            if (!visitContext.ContainsKey(skosConcept.PatternMemberID))
                visitContext.Add(skosConcept.PatternMemberID, skosConcept);
            else
                return narrowerTransitiveConcepts;
            #endregion

            #region Discovery
            //Find narrower concepts linked to the given one with skos:narrowerTransitive relation
            foreach (RDFTriple narrowerTransitiveRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NARROWER_TRANSITIVE, null, null])
                narrowerTransitiveConcepts.Add((RDFResource)narrowerTransitiveRelation.Object);
            #endregion

            // Inference: NARROWERTRANSITIVE(A,B) ^ NARROWERTRANSITIVE(B,C) -> NARROWERTRANSITIVE(A,C)
            foreach (RDFResource narrowerTransitiveConcept in narrowerTransitiveConcepts.ToList())
                narrowerTransitiveConcepts.AddRange(ontology.GetNarrowerConceptsInternal(narrowerTransitiveConcept, visitContext));

            return narrowerTransitiveConcepts;
        }

        /// <summary>
        /// Checks for the existence of "Related(leftConcept,rightConcept)" relations
        /// </summary>
        public static bool CheckHasRelatedConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => rightConcept != null && leftConcept != null && ontology != null && ontology.GetRelatedConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "Related(skosConceptScheme, X)" relations of the concept scheme to answer the related concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetRelatedConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> relatedConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:related concepts
                foreach (RDFTriple relatedRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.RELATED, null, null])
                    relatedConcepts.Add((RDFResource)relatedRelation.Object);
            }

            return relatedConcepts;
        }

        /// <summary>
        /// Checks for the existence of "BroadMatch(childConcept,motherConcept)" relations
        /// </summary>
        public static bool CheckHasBroadMatchConcept(this OWLOntology ontology, RDFResource childConcept, RDFResource motherConcept)
            => childConcept != null && motherConcept != null && ontology != null && ontology.GetBroadMatchConcepts(childConcept).Any(concept => concept.Equals(motherConcept));

        /// <summary>
        /// Analyzes "BroadMatch(skosConceptScheme, X)" relations of the concept scheme to answer the broad match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetBroadMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> broadMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:broadMatch concepts
                foreach (RDFTriple broadMatchRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.BROAD_MATCH, null, null])
                    broadMatchConcepts.Add((RDFResource)broadMatchRelation.Object);
            }

            return broadMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "NarrowMatch(motherConcept,childConcept)" relations
        /// </summary>
        public static bool CheckHasNarrowMatchConcept(this OWLOntology ontology, RDFResource motherConcept, RDFResource childConcept)
            => childConcept != null && motherConcept != null && ontology != null && ontology.GetNarrowMatchConcepts(motherConcept).Any(concept => concept.Equals(childConcept));

        /// <summary>
        /// Analyzes "NarrowMatch(skosConceptScheme, X)" relations of the concept scheme to answer the narrow match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetNarrowMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> narrowMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:narrowMatch concepts
                foreach (RDFTriple narrowMatchRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NARROW_MATCH, null, null])
                    narrowMatchConcepts.Add((RDFResource)narrowMatchRelation.Object);
            }

            return narrowMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "RelatedMatch(leftConcept,rightConcept)" relations
        /// </summary>
        public static bool CheckHasRelatedMatchConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => rightConcept != null && leftConcept != null && ontology != null && ontology.GetRelatedMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "RelatedMatch(skosConceptScheme, X)" relations of the concept scheme to answer the related match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetRelatedMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> relatedMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:relatedMatch concepts
                foreach (RDFTriple relatedMatchRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.RELATED_MATCH, null, null])
                    relatedMatchConcepts.Add((RDFResource)relatedMatchRelation.Object);
            }

            return relatedMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "CloseMatch(leftConcept,rightConcept)" relations
        /// </summary>
        public static bool CheckHasCloseMatchConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => rightConcept != null && leftConcept != null && ontology != null && ontology.GetCloseMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "CloseMatch(skosConceptScheme, X)" relations of the concept scheme to answer the close match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetCloseMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> closeMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:closeMatch concepts
                foreach (RDFTriple closeMatchRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.CLOSE_MATCH, null, null])
                    closeMatchConcepts.Add((RDFResource)closeMatchRelation.Object);
            }

            return closeMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "ExactMatch(leftConcept,rightConcept)" relations
        /// </summary>
        public static bool CheckHasExactMatchConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => leftConcept != null && rightConcept != null && ontology != null && ontology.GetExactMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "ExactMatch(skosConceptScheme, X)" relations of the concept scheme to answer the exact match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetExactMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> exactMatchConcepts = new List<RDFResource>();

            if (ontology != null && skosConcept != null)
            {
                exactMatchConcepts.AddRange(ontology.FindExactMatchConcepts(skosConcept, new Dictionary<long, RDFResource>()));

                //We don't want to also enlist the given skos:Concept
                exactMatchConcepts.RemoveAll(concept => concept.Equals(skosConcept));
            }

            return RDFQueryUtilities.RemoveDuplicates(exactMatchConcepts);
        }

        /// <summary>
        /// Finds "ExactMatch(skosConceptScheme, X)" relations to enlist the exact match concepts of the given skos:Concept
        /// </summary>
        internal static List<RDFResource> FindExactMatchConcepts(this OWLOntology ontology, RDFResource skosConcept, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> exactMatchConcepts = new List<RDFResource>();

            #region VisitContext
            if (!visitContext.ContainsKey(skosConcept.PatternMemberID))
                visitContext.Add(skosConcept.PatternMemberID, skosConcept);
            else
                return exactMatchConcepts;
            #endregion

            #region Discovery
            //Find exact match concepts linked to the given one with skos:exactMatch relation
            foreach (RDFTriple exactMatchRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.EXACT_MATCH, null, null])
                exactMatchConcepts.Add((RDFResource)exactMatchRelation.Object);
            #endregion

            // Inference: EXACTMATCH(A,B) ^ EXACTMATCH(B,C) -> EXACTMATCH(A,C)
            foreach (RDFResource exactMatchConcept in exactMatchConcepts.ToList())
                exactMatchConcepts.AddRange(ontology.FindExactMatchConcepts(exactMatchConcept, visitContext));

            return exactMatchConcepts;
        }

        /// <summary>
        /// Analyzes "MappingRelation(skosConceptScheme, X)" relations of the concept scheme to answer the mapping related concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetMappingRelatedConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> mappingRelatedConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:mappingRelation concepts
                foreach (RDFTriple mappingRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.MAPPING_RELATION, null, null])
                    mappingRelatedConcepts.Add((RDFResource)mappingRelation.Object);

                //Get indirectly mapped concepts (rdfs:subPropertyOf skos:mappingRelation)
                foreach (RDFResource subMappingRelation in ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.SKOS.MAPPING_RELATION))
                    mappingRelatedConcepts.AddRange(ontology.Data.ABoxGraph[skosConcept, subMappingRelation, null, null]
                                                      .Select(t => t.Object)
                                                      .OfType<RDFResource>());
            }

            return RDFQueryUtilities.RemoveDuplicates(mappingRelatedConcepts);
        }

        /// <summary>
        /// Analyzes "SemanticRelation(skosConceptScheme, X)" relations of the concept scheme to answer the semantic related concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetSemanticRelatedConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> semanticRelatedConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:semanticRelation concepts
                foreach (RDFTriple semanticRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.SEMANTIC_RELATION, null, null])
                    semanticRelatedConcepts.Add((RDFResource)semanticRelation.Object);

                //Get indirectly semantic concepts (rdfs:subPropertyOf skos:semanticRelation)
                foreach (RDFResource subSemanticRelation in ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.SKOS.SEMANTIC_RELATION))
                    semanticRelatedConcepts.AddRange(ontology.Data.ABoxGraph[skosConcept, subSemanticRelation, null, null]
                                                       .Select(t => t.Object)
                                                       .OfType<RDFResource>());
            }

            return RDFQueryUtilities.RemoveDuplicates(semanticRelatedConcepts);
        }

        /// <summary>
        /// Analyzes "Notation(skosConceptScheme, X)" relations of the concept scheme to answer the notations of the given skos:Concept
        /// </summary>
        public static List<RDFLiteral> GetConceptNotations(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFLiteral> notations = new List<RDFLiteral>();

            if (skosConcept != null && ontology != null)
            {
                //Get skos:notation values
                foreach (RDFTriple notationRelation in ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NOTATION, null, null])
                    notations.Add((RDFLiteral)notationRelation.Object);
            }

            return notations;
        }
        #endregion

        #region Checker
        /// <summary>
        /// Checks if the given leftConcept can be skos:[broader|broaderTransitive|broadMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckBroaderCompatibility(this OWLOntology ontology, RDFResource childConcept, RDFResource motherConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddBroaderRelation = !ontology.CheckHasNarrowerConcept(childConcept, motherConcept);

            //Avoid clash with associative relations
            if (canAddBroaderRelation)
                canAddBroaderRelation = !ontology.CheckHasRelatedConcept(childConcept, motherConcept);

            //Avoid clash with mapping relations
            if (canAddBroaderRelation)
                canAddBroaderRelation = !ontology.CheckHasNarrowMatchConcept(childConcept, motherConcept)
                                          && !ontology.CheckHasCloseMatchConcept(childConcept, motherConcept)
                                            && !ontology.CheckHasExactMatchConcept(childConcept, motherConcept)
                                              && !ontology.CheckHasRelatedMatchConcept(childConcept, motherConcept);

            return canAddBroaderRelation;
        }

        /// <summary>
        /// Checks if the given leftConcept can be skos:[narrower|narrowerTransitive|narrowMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckNarrowerCompatibility(this OWLOntology ontology, RDFResource motherConcept, RDFResource childConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddNarrowerRelation = !ontology.CheckHasBroaderConcept(motherConcept, childConcept);

            //Avoid clash with associative relations
            if (canAddNarrowerRelation)
                canAddNarrowerRelation = !ontology.CheckHasRelatedConcept(motherConcept, childConcept);

            //Avoid clash with mapping relations
            if (canAddNarrowerRelation)
                canAddNarrowerRelation = !ontology.CheckHasBroadMatchConcept(motherConcept, childConcept)
                                           && !ontology.CheckHasCloseMatchConcept(motherConcept, childConcept)
                                             && !ontology.CheckHasExactMatchConcept(motherConcept, childConcept)
                                               && !ontology.CheckHasRelatedMatchConcept(motherConcept, childConcept);

            return canAddNarrowerRelation;
        }

        /// <summary>
        /// Checks if the given leftConcept can be skos:[related|relatedMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckRelatedCompatibility(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddRelatedRelation = !ontology.CheckHasBroaderConcept(leftConcept, rightConcept)
                                           && !ontology.CheckHasNarrowerConcept(leftConcept, rightConcept);

            //Avoid clash with mapping relations
            if (canAddRelatedRelation)
                canAddRelatedRelation = !ontology.CheckHasBroadMatchConcept(leftConcept, rightConcept)
                                          && !ontology.CheckHasNarrowMatchConcept(leftConcept, rightConcept)
                                            && !ontology.CheckHasCloseMatchConcept(leftConcept, rightConcept)
                                              && !ontology.CheckHasExactMatchConcept(leftConcept, rightConcept);

            return canAddRelatedRelation;
        }

        /// <summary>
        /// Checks if the given leftConcept can be skos:[closeMatch|exactMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckCloseOrExactMatchCompatibility(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddCloseOrExactMatchRelation = !ontology.CheckHasBroaderConcept(leftConcept, rightConcept)
                                                     && !ontology.CheckHasNarrowerConcept(leftConcept, rightConcept);

            //Avoid clash with mapping relations
            if (canAddCloseOrExactMatchRelation)
                canAddCloseOrExactMatchRelation = !ontology.CheckHasBroadMatchConcept(leftConcept, rightConcept)
                                                    && !ontology.CheckHasNarrowMatchConcept(leftConcept, rightConcept)
                                                      && !ontology.CheckHasRelatedMatchConcept(leftConcept, rightConcept);

            return canAddCloseOrExactMatchRelation;
        }

        /// <summary>
        /// Checks if the given skos:Concept can be assigned the given [skos|skosxl]:prefLabel attribution without tampering SKOS integrity
        /// </summary>
        internal static bool CheckPreferredLabelCompatibility(this OWLOntology ontology, RDFResource skosConcept, RDFPlainLiteral preferredLabelValue)
        {
            //Check skos:prefLabel annotation => no occurrences of the given value's language must be found (in order to accept the annotation)
            RDFSelectQuery skosPrefLabelQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.PREF_LABEL, new RDFVariable("?PREFLABEL")))
                    .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?PREFLABEL")))
                    .AddFilter(new RDFLangMatchesFilter(new RDFVariable("?PREFLABEL"), preferredLabelValue.Language)));
            RDFSelectQueryResult skosPrefLabelQueryResult = skosPrefLabelQuery.ApplyToGraph(ontology.Data.OBoxGraph);
            bool canAddPreferredLabel = skosPrefLabelQueryResult.SelectResultsCount == 0;

            //Check skosxl:prefLabel relation => no occurrences of the given value's language must be found (in order to accept the relation)
            if (canAddPreferredLabel)
            {
                RDFSelectQuery skosxlPrefLabelQuery = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFVariable("?PREFLABEL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?PREFLABEL"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?PREFLABEL")))
                        .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFLangMatchesFilter(new RDFVariable("?LITERALFORM"), preferredLabelValue.Language)));
                RDFSelectQueryResult skosxlPrefLabelQueryResult = skosxlPrefLabelQuery.ApplyToGraph(ontology.Data.ABoxGraph);
                canAddPreferredLabel = skosxlPrefLabelQueryResult.SelectResultsCount == 0;
            }

            //Check pairwise disjointness with skos:hiddenLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            if (canAddPreferredLabel)
                canAddPreferredLabel = ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.HIDDEN_LABEL, null, preferredLabelValue].TriplesCount == 0;

            //Check pairwise disjointness with skosxl:hiddenLabel relation => no occurrences of the given value must be found (in order to accept the relation)
            if (canAddPreferredLabel)
            {
                RDFSelectQuery skosxlHiddenLabelQuery = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFVariable("?HIDDENLABEL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?HIDDENLABEL"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?HIDDENLABEL")))
                        .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFSameTermFilter(new RDFVariable("?LITERALFORM"), preferredLabelValue)));
                RDFSelectQueryResult skosxlHiddenLabelQueryResult = skosxlHiddenLabelQuery.ApplyToGraph(ontology.Data.ABoxGraph);
                canAddPreferredLabel = skosxlHiddenLabelQueryResult.SelectResultsCount == 0;
            }

            return canAddPreferredLabel;
        }

        /// <summary>
        /// Checks if the given skos:Concept can be assigned the given [skos|skosxl]:altLabel attribution without tampering SKOS integrity
        /// </summary>
        internal static bool CheckAlternativeLabelCompatibility(this OWLOntology ontology, RDFResource skosConcept, RDFPlainLiteral alternativeLabelValue)
        {
            //Check pairwise disjointness with skos:prefLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            bool canAddAlternativeLabel = ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.PREF_LABEL, null, alternativeLabelValue].TriplesCount == 0;

            //Check pairwise disjointness with skosxl:prefLabel relation => no occurrences of the given value must be found (in order to accept the relation)
            if (canAddAlternativeLabel)
            {
                RDFSelectQuery skosxlPrefLabelQuery = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFVariable("?PREFLABEL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?PREFLABEL"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?PREFLABEL")))
                        .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFSameTermFilter(new RDFVariable("?LITERALFORM"), alternativeLabelValue)));
                RDFSelectQueryResult skosxlPrefLabelQueryResult = skosxlPrefLabelQuery.ApplyToGraph(ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlPrefLabelQueryResult.SelectResultsCount == 0;
            }

            //Check pairwise disjointness with skos:hiddenLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            if (canAddAlternativeLabel)
                canAddAlternativeLabel = ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.HIDDEN_LABEL, null, alternativeLabelValue].TriplesCount == 0;

            //Check pairwise disjointness with skosxl:hiddenLabel relation => no occurrences of the given value must be found (in order to accept the relation)
            if (canAddAlternativeLabel)
            {
                RDFSelectQuery skosxlHiddenLabelQuery = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFVariable("?HIDDENLABEL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?HIDDENLABEL"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?HIDDENLABEL")))
                        .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFSameTermFilter(new RDFVariable("?LITERALFORM"), alternativeLabelValue)));
                RDFSelectQueryResult skosxlHiddenLabelQueryResult = skosxlHiddenLabelQuery.ApplyToGraph(ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlHiddenLabelQueryResult.SelectResultsCount == 0;
            }

            return canAddAlternativeLabel;
        }

        /// <summary>
        /// Checks if the given skos:Concept can be assigned the given [skos|skosxl]:hiddenLabel attribution without tampering SKOS integrity
        /// </summary>
        internal static bool CheckHiddenLabelCompatibility(this OWLOntology ontology, RDFResource skosConcept, RDFPlainLiteral hiddenLabelValue)
        {
            //Check pairwise disjointness with skos:prefLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            bool canAddAlternativeLabel = ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.PREF_LABEL, null, hiddenLabelValue].TriplesCount == 0;

            //Check pairwise disjointness with skosxl:prefLabel relation => no occurrences of the given value must be found (in order to accept the relation)
            if (canAddAlternativeLabel)
            {
                RDFSelectQuery skosxlPrefLabelQuery = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFVariable("?PREFLABEL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?PREFLABEL"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?PREFLABEL")))
                        .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFSameTermFilter(new RDFVariable("?LITERALFORM"), hiddenLabelValue)));
                RDFSelectQueryResult skosxlPrefLabelQueryResult = skosxlPrefLabelQuery.ApplyToGraph(ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlPrefLabelQueryResult.SelectResultsCount == 0;
            }

            //Check pairwise disjointness with skos:altLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            if (canAddAlternativeLabel)
                canAddAlternativeLabel = ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.ALT_LABEL, null, hiddenLabelValue].TriplesCount == 0;

            //Check pairwise disjointness with skosxl:altLabel relation => no occurrences of the given value must be found (in order to accept the relation)
            if (canAddAlternativeLabel)
            {
                RDFSelectQuery skosxlAltLabelQuery = new RDFSelectQuery()
                    .AddPatternGroup(new RDFPatternGroup()
                        .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new RDFVariable("?ALTLABEL")))
                        .AddPattern(new RDFPattern(new RDFVariable("?ALTLABEL"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFIsUriFilter(new RDFVariable("?ALTLABEL")))
                        .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?LITERALFORM")))
                        .AddFilter(new RDFSameTermFilter(new RDFVariable("?LITERALFORM"), hiddenLabelValue)));
                RDFSelectQueryResult skosxlAltLabelQueryResult = skosxlAltLabelQuery.ApplyToGraph(ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlAltLabelQueryResult.SelectResultsCount == 0;
            }

            return canAddAlternativeLabel;
        }
        #endregion
    }
}