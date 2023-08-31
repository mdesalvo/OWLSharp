/*
   Copyright 2012-2023 Marco De Salvo

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSConceptScheme represents an OWL ontology specialized in describing relations between skos:Concept individuals
    /// </summary>
    public class SKOSConceptScheme : RDFResource, IEnumerable<RDFResource>
    {
        #region Properties
        /// <summary>
        /// Count of the concepts
        /// </summary>
        public long ConceptsCount
        {
            get
            {
                long count = 0;
                IEnumerator<RDFResource> concepts = ConceptsEnumerator;
                while (concepts.MoveNext())
                    count++;
                return count;
            }
        }

        /// <summary>
        /// Count of the collections
        /// </summary>
        public long CollectionsCount
        {
            get
            {
                long count = 0;
                IEnumerator<RDFResource> collections = CollectionsEnumerator;
                while (collections.MoveNext())
                    count++;
                return count;
            }
        }

        /// <summary>
        /// Count of the ordered collections
        /// </summary>
        public long OrderedCollectionsCount
        {
            get
            {
                long count = 0;
                IEnumerator<RDFResource> orderedCollections = OrderedCollectionsEnumerator;
                while (orderedCollections.MoveNext())
                    count++;
                return count;
            }
        }

        /// <summary>
        /// Count of the labels [SKOS-XL]
        /// </summary>
        public long LabelsCount
        {
            get
            {
                long count = 0;
                IEnumerator<RDFResource> labels = LabelsEnumerator;
                while (labels.MoveNext())
                    count++;
                return count;
            }
        }

        /// <summary>
        /// Gets the enumerator on the concepts for iteration
        /// </summary>
        public IEnumerator<RDFResource> ConceptsEnumerator
            => Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Gets the enumerator on the collections for iteration
        /// </summary>
        public IEnumerator<RDFResource> CollectionsEnumerator
            => Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Gets the enumerator on the ordered collections for iteration
        /// </summary>
        public IEnumerator<RDFResource> OrderedCollectionsEnumerator
            => Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Gets the enumerator on the labels for iteration [SKOS-XL]
        /// </summary>
        public IEnumerator<RDFResource> LabelsEnumerator
            => Ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.SKOSXL.LABEL, null]
                            .Select(t => t.Subject)
                            .OfType<RDFResource>()
                            .GetEnumerator();

        /// <summary>
        /// Knowledge describing the concept scheme
        /// </summary>
        internal OWLOntology Ontology { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a concept scheme with the given URI (internal T-BOX is initialized with SKOS ontology)
        /// </summary>
        public SKOSConceptScheme(string conceptSchemeURI) : base(conceptSchemeURI)
        {
            Ontology = new OWLOntology(conceptSchemeURI);
            Ontology.InitializeSKOS();

            //Declare concept scheme to the data
            Ontology.Data.DeclareIndividual(this);
            Ontology.Data.DeclareIndividualType(this, RDFVocabulary.SKOS.CONCEPT_SCHEME);
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Exposes a typed enumerator on the concepts for iteration
        /// </summary>
        IEnumerator<RDFResource> IEnumerable<RDFResource>.GetEnumerator()
            => ConceptsEnumerator;

        /// <summary>
        /// Exposes an untyped enumerator on the concepts for iteration
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => ConceptsEnumerator;
        #endregion

        #region Methods
        /// <summary>
        /// Declares the given skos:Concept instance to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareConcept(RDFResource skosConcept)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:Concept instance to the concept scheme because given \"skosConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.DeclareIndividual(skosConcept);
            Ontology.Data.DeclareIndividualType(skosConcept, RDFVocabulary.SKOS.CONCEPT);
            Ontology.Data.DeclareObjectAssertion(skosConcept, RDFVocabulary.SKOS.IN_SCHEME, this);

            return this;
        }

        /// <summary>
        /// Declares the given skos:Collection instance to the concept scheme (but not its concepts or subcollections, which must be declared apart)
        /// </summary>
        public SKOSConceptScheme DeclareCollection(RDFResource skosCollection, List<RDFResource> skosConcepts)
        {
            #region Guards
            if (skosCollection == null)
                throw new OWLException("Cannot declare skos:Collection instance to the concept scheme because given \"skosCollection\" parameter is null");
            if (skosConcepts == null)
                throw new OWLException("Cannot declare skos:Collection instance to the concept scheme because given \"skosConcepts\" parameter is null");
            if (skosConcepts.Count == 0)
                throw new OWLException("Cannot declare skos:Collection instance to the concept scheme because given \"skosConcepts\" parameter is an empty list");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.DeclareIndividual(skosCollection);
            Ontology.Data.DeclareIndividualType(skosCollection, RDFVocabulary.SKOS.COLLECTION);
            Ontology.Data.DeclareObjectAssertion(skosCollection, RDFVocabulary.SKOS.IN_SCHEME, this);
            foreach (RDFResource skosConcept in skosConcepts)
                Ontology.Data.DeclareObjectAssertion(skosCollection, RDFVocabulary.SKOS.MEMBER, skosConcept);

            return this;
        }

        /// <summary>
        /// Declares the given skos:OrderedCollection instance to the concept scheme (but not its concepts, which must be declared apart)
        /// </summary>
        public SKOSConceptScheme DeclareOrderedCollection(RDFResource skosOrderedCollection, List<RDFResource> skosConcepts)
        {
            #region Guards
            if (skosOrderedCollection == null)
                throw new OWLException("Cannot declare skos:OrderedCollection instance to the concept scheme because given \"skosOrderedCollection\" parameter is null");
            if (skosConcepts == null)
                throw new OWLException("Cannot declare skos:OrderedCollection instance to the concept scheme because given \"skosConcepts\" parameter is null");
            if (skosConcepts.Count == 0)
                throw new OWLException("Cannot declare skos:OrderedCollection instance to the concept scheme because given \"skosConcepts\" parameter is an empty list");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.DeclareIndividual(skosOrderedCollection);
            Ontology.Data.DeclareIndividualType(skosOrderedCollection, RDFVocabulary.SKOS.ORDERED_COLLECTION);
            Ontology.Data.DeclareObjectAssertion(skosOrderedCollection, RDFVocabulary.SKOS.IN_SCHEME, this);
            RDFCollection rdfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (RDFResource skosConcept in skosConcepts)
                rdfCollection.AddItem(skosConcept);
            Ontology.Data.ABoxGraph.AddCollection(rdfCollection);
            Ontology.Data.DeclareObjectAssertion(skosOrderedCollection, RDFVocabulary.SKOS.MEMBER_LIST, rdfCollection.ReificationSubject);

            return this;
        }

        //ANNOTATIONS

        /// <summary>
        /// Annotates the concept scheme with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme Annotate(RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(this, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the concept scheme with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme Annotate(RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate concept scheme because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(this, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the given skos:Concept with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme AnnotateConcept(RDFResource skosConcept, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
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
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the given skos:Concept with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme AnnotateConcept(RDFResource skosConcept, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
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
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the given skos:Collection with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme AnnotateCollection(RDFResource skosCollection, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
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
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosCollection, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the given skos:Collection with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme AnnotateCollection(RDFResource skosCollection, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
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
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosCollection, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the given skos:OrderedCollection with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme AnnotateOrderedCollection(RDFResource skosOrderedCollection, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
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
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosOrderedCollection, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the given skos:OrderedCollection with the given "annotationProperty -> annotationValue"
        /// </summary>
        public SKOSConceptScheme AnnotateOrderedCollection(RDFResource skosOrderedCollection, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
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
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosOrderedCollection, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "Note(skosConcept,noteValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DocumentConceptWithNote(RDFResource skosConcept, RDFLiteral noteValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:note annotation to the concept scheme because given \"skosConcept\" parameter is null");
            if (noteValue == null)
                throw new OWLException("Cannot declare skos:note annotation to the concept scheme because given \"noteValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.NOTE, noteValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "ChangeNote(skosConcept,changeNoteValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DocumentConceptWithChangeNote(RDFResource skosConcept, RDFLiteral changeNoteValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:changeNote annotation to the concept scheme because given \"skosConcept\" parameter is null");
            if (changeNoteValue == null)
                throw new OWLException("Cannot declare skos:changeNote annotation to the concept scheme because given \"changeNoteValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.CHANGE_NOTE, changeNoteValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "EditorialNote(skosConcept,editorialNoteValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DocumentConceptWithEditorialNote(RDFResource skosConcept, RDFLiteral editorialNoteValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:editorialNote annotation to the concept scheme because given \"skosConcept\" parameter is null");
            if (editorialNoteValue == null)
                throw new OWLException("Cannot declare skos:editorialNote annotation to the concept scheme because given \"editorialNoteValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.EDITORIAL_NOTE, editorialNoteValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "HistoryNote(skosConcept,historyNoteValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DocumentConceptWithHistoryNote(RDFResource skosConcept, RDFLiteral historyNoteValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:historyNote annotation to the concept scheme because given \"skosConcept\" parameter is null");
            if (historyNoteValue == null)
                throw new OWLException("Cannot declare skos:historyNote annotation to the concept scheme because given \"historyNoteValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.HISTORY_NOTE, historyNoteValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "ScopeNote(skosConcept,scopeNoteValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DocumentConceptWithScopeNote(RDFResource skosConcept, RDFLiteral scopeNoteValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:scopeNote annotation to the concept scheme because given \"skosConcept\" parameter is null");
            if (scopeNoteValue == null)
                throw new OWLException("Cannot declare skos:scopeNote annotation to the concept scheme because given \"scopeNoteValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SCOPE_NOTE, scopeNoteValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "Definition(skosConcept,definitionValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DocumentConceptWithDefinition(RDFResource skosConcept, RDFLiteral definitionValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:definition annotation to the concept scheme because given \"skosConcept\" parameter is null");
            if (definitionValue == null)
                throw new OWLException("Cannot declare skos:definition annotation to the concept scheme because given \"definitionValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.DEFINITION, definitionValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "Example(skosConcept,exampleValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DocumentConceptWithExample(RDFResource skosConcept, RDFLiteral exampleValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:example annotation to the concept scheme because given \"skosConcept\" parameter is null");
            if (exampleValue == null)
                throw new OWLException("Cannot declare skos:example annotation to the concept scheme because given \"exampleValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.EXAMPLE, exampleValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "PrefLabel(skosConcept,preferredLabelValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclarePreferredLabel(RDFResource skosConcept, RDFPlainLiteral preferredLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckPreferredLabelCompatibility(skosConcept, preferredLabelValue);
            #endregion

            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:prefLabel relation to the concept scheme because given \"skosConcept\" parameter is null");
            if (preferredLabelValue == null)
                throw new OWLException("Cannot declare skos:prefLabel relation to the concept scheme because given \"preferredLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the O-BOX
                Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.PREF_LABEL, preferredLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("PrefLabel relation between concept '{0}' and value '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, preferredLabelValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "AltLabel(skosConcept,alternativeLabelValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareAlternativeLabel(RDFResource skosConcept, RDFPlainLiteral alternativeLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckAlternativeLabelCompatibility(skosConcept, alternativeLabelValue);
            #endregion

            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:altLabel relation to the concept scheme because given \"skosConcept\" parameter is null");
            if (alternativeLabelValue == null)
                throw new OWLException("Cannot declare skos:altLabel relation to the concept scheme because given \"alternativeLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the O-BOX
                Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.ALT_LABEL, alternativeLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("AltLabel relation between concept '{0}' and value '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, alternativeLabelValue));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "HiddenLabel(skosConcept,hiddenLabelValue)" annotation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareHiddenLabel(RDFResource skosConcept, RDFPlainLiteral hiddenLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckHiddenLabelCompatibility(skosConcept, hiddenLabelValue);
            #endregion

            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:hiddenLabel relation to the concept scheme because given \"skosConcept\" parameter is null");
            if (hiddenLabelValue == null)
                throw new OWLException("Cannot declare skos:hiddenLabel relation to the concept scheme because given \"hiddenLabelValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the O-BOX
                Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.HIDDEN_LABEL, hiddenLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("HiddenLabel relation between concept '{0}' and value '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, hiddenLabelValue));

            return this;
        }

        //RELATIONS

        /// <summary>
        /// Declares the existence of the given "TopConceptOf(skosConcept,skosConceptScheme)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareTopConcept(RDFResource skosConcept)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:topConceptOf relation to the concept scheme because given \"skosConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.TOP_CONCEPT_OF, this));

            //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
            Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(this, RDFVocabulary.SKOS.HAS_TOP_CONCEPT, skosConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "SemanticRelation(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareSemanticRelatedConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:semanticRelation relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:semanticRelation relation to the concept scheme because given \"rightConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.SEMANTIC_RELATION, rightConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "MappingRelation(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareMappingRelatedConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:mappingRelation relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:mappingRelation relation to the concept scheme because given \"rightConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.MAPPING_RELATION, rightConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "Related(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareRelatedConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:related relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:related relation to the concept scheme because given \"rightConcept\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.RELATED, rightConcept));

            //Also add an automatic A-BOX inference exploiting symmetry of skos:related relation
            Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.RELATED, leftConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "Broader(childConcept,motherConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareBroaderConcepts(RDFResource childConcept, RDFResource motherConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckBroaderCompatibility(childConcept, motherConcept);
            #endregion

            #region Guards
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:broader relation to the concept scheme because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:broader relation to the concept scheme because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:broader relation to the concept scheme because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER, motherConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER, childConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("Broader relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", childConcept, motherConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "BroaderTransitive(childConcept,motherConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareBroaderTransitiveConcepts(RDFResource childConcept, RDFResource motherConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckBroaderCompatibility(childConcept, motherConcept);
            #endregion

            #region Guards
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:broaderTransitive relation to the concept scheme because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:broaderTransitive relation to the concept scheme because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:broaderTransitive relation to the concept scheme because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER_TRANSITIVE, motherConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER_TRANSITIVE, childConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("BroaderTransitive relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", childConcept, motherConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "Narrower(motherConcept,childConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareNarrowerConcepts(RDFResource motherConcept, RDFResource childConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckNarrowerCompatibility(motherConcept, childConcept);
            #endregion

            #region Guards
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:narrower relation to the concept scheme because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:narrower relation to the concept scheme because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:narrower relation to the concept scheme because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER, childConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER, motherConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("Narrower relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", motherConcept, childConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "NarrowerTransitive(motherConcept,childConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareNarrowerTransitiveConcepts(RDFResource motherConcept, RDFResource childConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckNarrowerCompatibility(motherConcept, childConcept);
            #endregion

            #region Guards
            if (childConcept == null)
                throw new OWLException("Cannot declare skos:narrowerTransitive relation to the concept scheme because given \"childConcept\" parameter is null");
            if (motherConcept == null)
                throw new OWLException("Cannot declare skos:narrowerTransitive relation to the concept scheme because given \"motherConcept\" parameter is null");
            if (childConcept.Equals(motherConcept))
                throw new OWLException("Cannot declare skos:narrowerTransitive relation to the concept scheme because given \"childConcept\" parameter refers to the same concept as the given \"motherConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(motherConcept, RDFVocabulary.SKOS.NARROWER_TRANSITIVE, childConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(childConcept, RDFVocabulary.SKOS.BROADER_TRANSITIVE, motherConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("NarrowerTransitive relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", motherConcept, childConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "CloseMatch(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareCloseMatchConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckCloseOrExactMatchCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:closeMatch relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:closeMatch relation to the concept scheme because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:closeMatch relation to the concept scheme because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.CLOSE_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting simmetry of skos:closeMatch relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.CLOSE_MATCH, leftConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("CloseMatch relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", leftConcept, rightConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "ExactMatch(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareExactMatchConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckCloseOrExactMatchCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:exactMatch relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:exactMatch relation to the concept scheme because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:exactMatch relation to the concept scheme because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.EXACT_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting simmetry of skos:exactMatch relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.EXACT_MATCH, leftConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("ExactMatch relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", leftConcept, rightConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "BroadMatch(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareBroadMatchConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckBroaderCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:broadMatch relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:broadMatch relation to the concept scheme because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:broadMatch relation to the concept scheme because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.BROAD_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.NARROW_MATCH, leftConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("BroadMatch relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", leftConcept, rightConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "NarrowMatch(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareNarrowMatchConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckNarrowerCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:narrowMatch relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:narrowMatch relation to the concept scheme because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:narrowMatch relation to the concept scheme because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.NARROW_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting owl:inverseOf relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.BROAD_MATCH, leftConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("NarrowMatch relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", leftConcept, rightConcept));

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "RelatedMatch(leftConcept,rightConcept)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareRelatedMatchConcepts(RDFResource leftConcept, RDFResource rightConcept)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => this.CheckRelatedCompatibility(leftConcept, rightConcept);
            #endregion

            #region Guards
            if (leftConcept == null)
                throw new OWLException("Cannot declare skos:relatedMatch relation to the concept scheme because given \"leftConcept\" parameter is null");
            if (rightConcept == null)
                throw new OWLException("Cannot declare skos:relatedMatch relation to the concept scheme because given \"rightConcept\" parameter is null");
            if (leftConcept.Equals(rightConcept))
                throw new OWLException("Cannot declare skos:relatedMatch relation to the concept scheme because given \"leftConcept\" parameter refers to the same concept as the given \"rightConcept\" parameter");
            #endregion

            //Add knowledge to the A-BOX (or raise warning if violations are detected)
            if (SKOSIntegrityChecks())
            {
                //Add knowledge to the A-BOX
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftConcept, RDFVocabulary.SKOS.RELATED_MATCH, rightConcept));

                //Also add an automatic A-BOX inference exploiting simmetry of skos:relatedMatch relation
                Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightConcept, RDFVocabulary.SKOS.RELATED_MATCH, leftConcept));
            }
            else
                OWLEvents.RaiseWarning(string.Format("RelatedMatch relation between concept '{0}' and concept '{1}' cannot be declared to the concept scheme because it would violate SKOS integrity", leftConcept, rightConcept));

            return this;
        }

        /// <summary>
        ///  Declares the existence of the given "Notation(skosConcept,notationValue)" relation to the concept scheme
        /// </summary>
        public SKOSConceptScheme DeclareNotation(RDFResource skosConcept, RDFLiteral notationValue)
        {
            #region Guards
            if (skosConcept == null)
                throw new OWLException("Cannot declare skos:notation relation to the concept scheme because given \"skosConcept\" parameter is null");
            if (notationValue == null)
                throw new OWLException("Cannot declare skos:notation relation to the concept scheme because given \"notationValue\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.NOTATION, notationValue));

            return this;
        }

        //EXPORT

        /// <summary>
        /// Gets a graph representation of the concept scheme
        /// </summary>
        public RDFGraph ToRDFGraph(bool includeInferences=true)
            => Ontology.ToRDFGraph(includeInferences);

        /// <summary>
        /// Asynchronously gets a graph representation of the concept scheme
        /// </summary>
        public Task<RDFGraph> ToRDFGraphAsync(bool includeInferences=true)
            => Task.Run(() => ToRDFGraph(includeInferences));

        //IMPORT

        /// <summary>
        /// Gets a concept scheme representation from the given graph
        /// </summary>
        public static SKOSConceptScheme FromRDFGraph(RDFGraph graph)
            => SKOSConceptSchemeLoader.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableSKOSSupport=true });

        /// <summary>
        /// Asynchronously gets a concept scheme representation from the given graph
        /// </summary>
        public static Task<SKOSConceptScheme> FromRDFGraphAsync(RDFGraph graph)
            => Task.Run(() => FromRDFGraph(graph));
        #endregion
    }
}