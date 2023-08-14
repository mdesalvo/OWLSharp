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
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSConceptSchemeHelper contains methods for analyzing relations describing SKOS concept schemes
    /// </summary>
    public static class SKOSConceptSchemeHelper
    {
        #region Declarer
        /// <summary>
        /// Checks for the existence of the given skos:Concept declaration within the concept scheme
        /// </summary>
        public static bool CheckHasConcept(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            bool conceptFound = false;
            if (skosConcept != null && conceptScheme != null)
            {
                IEnumerator<RDFResource> conceptsEnumerator = conceptScheme.ConceptsEnumerator;
                while (!conceptFound && conceptsEnumerator.MoveNext())
                    conceptFound = conceptsEnumerator.Current.Equals(skosConcept);
            }
            return conceptFound;
        }

        /// <summary>
        /// Checks for the existence of the given skos:Collection declaration within the concept scheme
        /// </summary>
        public static bool CheckHasCollection(this SKOSConceptScheme conceptScheme, RDFResource skosCollection)
        {
            bool collectionFound = false;
            if (skosCollection != null && conceptScheme != null)
            {
                IEnumerator<RDFResource> collectionsEnumerator = conceptScheme.CollectionsEnumerator;
                while (!collectionFound && collectionsEnumerator.MoveNext())
                    collectionFound = collectionsEnumerator.Current.Equals(skosCollection);
            }
            return collectionFound;
        }

        /// <summary>
        /// Gets the direct and indirect skos:Concept instances which are members of the given skos:Collection within the concept scheme
        /// </summary>
        public static List<RDFResource> GetCollectionMembers(this SKOSConceptScheme conceptScheme, RDFResource skosCollection)
            => GetCollectionMembers(conceptScheme, skosCollection, new Dictionary<long, RDFResource>());

        /// <summary>
        /// Gets the direct and indirect skos:Concept instances which are members of the given skos:Collection within the concept scheme (internal recursive version)
        /// </summary>
        internal static List<RDFResource> GetCollectionMembers(this SKOSConceptScheme conceptScheme, RDFResource skosCollection, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> collectionMembers = new List<RDFResource>();

            if (skosCollection != null && conceptScheme != null)
            {
                #region visitContext
                if (!visitContext.ContainsKey(skosCollection.PatternMemberID))
                    visitContext.Add(skosCollection.PatternMemberID, skosCollection);
                else
                    return collectionMembers;
                #endregion

                foreach (RDFResource collectionMember in conceptScheme.Ontology.Data.ABoxGraph[skosCollection, RDFVocabulary.SKOS.MEMBER, null, null]
                                                           .Select(t => t.Object)
                                                           .OfType<RDFResource>())
                {
                    //skos:Collection
                    if (conceptScheme.Ontology.Data.ABoxGraph[collectionMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null].TriplesCount > 0)
                        collectionMembers.AddRange(GetCollectionMembers(conceptScheme, collectionMember, visitContext));

                    //skos:OrderedCollection
                    else if (conceptScheme.Ontology.Data.ABoxGraph[collectionMember, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION, null].TriplesCount > 0)
                        collectionMembers.AddRange(GetOrderedCollectionMembers(conceptScheme, collectionMember));

                    //skos:Concept
                    else
                        collectionMembers.Add(collectionMember);
                }
            }

            return RDFQueryUtilities.RemoveDuplicates(collectionMembers);
        }

        /// <summary>
        /// Checks for the existence of the given skos:OrderedCollection declaration within the concept scheme
        /// </summary>
        public static bool CheckHasOrderedCollection(this SKOSConceptScheme conceptScheme, RDFResource skosOrderedCollection)
        {
            bool orderedCollectionFound = false;
            if (skosOrderedCollection != null && conceptScheme != null)
            {
                IEnumerator<RDFResource> orderedCollectionsEnumerator = conceptScheme.OrderedCollectionsEnumerator;
                while (!orderedCollectionFound && orderedCollectionsEnumerator.MoveNext())
                    orderedCollectionFound = orderedCollectionsEnumerator.Current.Equals(skosOrderedCollection);
            }
            return orderedCollectionFound;
        }

        /// <summary>
        /// Gets the skos:Concept instances which are members of the given skos:OrderedCollection within the concept scheme
        /// </summary>
        public static List<RDFResource> GetOrderedCollectionMembers(this SKOSConceptScheme conceptScheme, RDFResource skosOrderedCollection)
        {
            List<RDFResource> orderedCollectionMembers = new List<RDFResource>();

            if (skosOrderedCollection != null && conceptScheme != null)
            {
                foreach (RDFTriple memberListRelation in conceptScheme.Ontology.Data.ABoxGraph[skosOrderedCollection, RDFVocabulary.SKOS.MEMBER_LIST, null, null])
                {
                    RDFCollection skosOrderedCollectionMembers = RDFModelUtilities.DeserializeCollectionFromGraph(conceptScheme.Ontology.Data.ABoxGraph, (RDFResource)memberListRelation.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    if (skosOrderedCollectionMembers.ItemsCount > 0)
                        skosOrderedCollectionMembers.Items.ForEach(item => orderedCollectionMembers.Add((RDFResource)item));
                }
            }

            return orderedCollectionMembers;
        }
        #endregion

        #region Analyzer
        /// <summary>
        /// Checks for the existence of "HasTopConcept(skosConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasTopConcept(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
            => skosConcept != null && conceptScheme != null && conceptScheme.Ontology.Data.ABoxGraph[conceptScheme, RDFVocabulary.SKOS.HAS_TOP_CONCEPT, skosConcept, null].TriplesCount > 0;

        /// <summary>
        /// Checks for the existence of "Broader(childConcept,motherConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasBroaderConcept(this SKOSConceptScheme conceptScheme, RDFResource childConcept, RDFResource motherConcept)
            => childConcept != null && motherConcept != null && conceptScheme != null && conceptScheme.GetBroaderConcepts(childConcept).Any(concept => concept.Equals(motherConcept));

        /// <summary>
        /// Analyzes "Broader(skosConcept, X)" relations of the concept scheme to answer the broader concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetBroaderConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> broaderConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:broader concepts
                foreach (RDFTriple broaderRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.BROADER, null, null])
                    broaderConcepts.Add((RDFResource)broaderRelation.Object);

                //Get skos:broaderTransitive concepts
                broaderConcepts.AddRange(conceptScheme.GetBroaderConceptsInternal(skosConcept, new Dictionary<long, RDFResource>()));
            }

            return broaderConcepts;
        }

        /// <summary>
        /// Subsumes the "skos:broaderTransitive" taxonomy to discover direct and indirect broader concepts of the given skos:Concept
        /// </summary>
        internal static List<RDFResource> GetBroaderConceptsInternal(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, Dictionary<long, RDFResource> visitContext)
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
            foreach (RDFTriple broaderTransitiveRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.BROADER_TRANSITIVE, null, null])
                broaderTransitiveConcepts.Add((RDFResource)broaderTransitiveRelation.Object);
            #endregion

            // Inference: BROADERTRANSITIVE(A,B) ^ BROADERTRANSITIVE(B,C) -> BROADERTRANSITIVE(A,C)
            foreach (RDFResource broaderTransitiveConcept in broaderTransitiveConcepts.ToList())
                broaderTransitiveConcepts.AddRange(conceptScheme.GetBroaderConceptsInternal(broaderTransitiveConcept, visitContext));

            return broaderTransitiveConcepts;
        }

        /// <summary>
        /// Checks for the existence of "Narrower(motherConcept,childConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasNarrowerConcept(this SKOSConceptScheme conceptScheme, RDFResource motherConcept, RDFResource childConcept)
            => childConcept != null && motherConcept != null && conceptScheme != null && conceptScheme.GetNarrowerConcepts(motherConcept).Any(concept => concept.Equals(childConcept));

        /// <summary>
        /// Analyzes "Narrower(skosConcept, X)" relations of the concept scheme to answer the narrower concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetNarrowerConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> narrowerConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:narrower concepts
                foreach (RDFTriple narrowerRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NARROWER, null, null])
                    narrowerConcepts.Add((RDFResource)narrowerRelation.Object);

                //Get skos:narrowerTransitive concepts
                narrowerConcepts.AddRange(conceptScheme.GetNarrowerConceptsInternal(skosConcept, new Dictionary<long, RDFResource>()));
            }

            return narrowerConcepts;
        }

        /// <summary>
        /// Subsumes the "skos:narrowerTransitive" taxonomy to discover direct and indirect narrower concepts of the given skos:Concept
        /// </summary>
        internal static List<RDFResource> GetNarrowerConceptsInternal(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, Dictionary<long, RDFResource> visitContext)
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
            foreach (RDFTriple narrowerTransitiveRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NARROWER_TRANSITIVE, null, null])
                narrowerTransitiveConcepts.Add((RDFResource)narrowerTransitiveRelation.Object);
            #endregion

            // Inference: NARROWERTRANSITIVE(A,B) ^ NARROWERTRANSITIVE(B,C) -> NARROWERTRANSITIVE(A,C)
            foreach (RDFResource narrowerTransitiveConcept in narrowerTransitiveConcepts.ToList())
                narrowerTransitiveConcepts.AddRange(conceptScheme.GetNarrowerConceptsInternal(narrowerTransitiveConcept, visitContext));

            return narrowerTransitiveConcepts;
        }

        /// <summary>
        /// Checks for the existence of "Related(leftConcept,rightConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasRelatedConcept(this SKOSConceptScheme conceptScheme, RDFResource leftConcept, RDFResource rightConcept)
            => rightConcept != null && leftConcept != null && conceptScheme != null && conceptScheme.GetRelatedConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "Related(skosConcept, X)" relations of the concept scheme to answer the related concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetRelatedConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> relatedConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:related concepts
                foreach (RDFTriple relatedRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.RELATED, null, null])
                    relatedConcepts.Add((RDFResource)relatedRelation.Object);
            }

            return relatedConcepts;
        }

        /// <summary>
        /// Checks for the existence of "BroadMatch(childConcept,motherConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasBroadMatchConcept(this SKOSConceptScheme conceptScheme, RDFResource childConcept, RDFResource motherConcept)
            => childConcept != null && motherConcept != null && conceptScheme != null && conceptScheme.GetBroadMatchConcepts(childConcept).Any(concept => concept.Equals(motherConcept));

        /// <summary>
        /// Analyzes "BroadMatch(skosConcept, X)" relations of the concept scheme to answer the broad match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetBroadMatchConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> broadMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:broadMatch concepts
                foreach (RDFTriple broadMatchRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.BROAD_MATCH, null, null])
                    broadMatchConcepts.Add((RDFResource)broadMatchRelation.Object);
            }

            return broadMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "NarrowMatch(motherConcept,childConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasNarrowMatchConcept(this SKOSConceptScheme conceptScheme, RDFResource motherConcept, RDFResource childConcept)
            => childConcept != null && motherConcept != null && conceptScheme != null && conceptScheme.GetNarrowMatchConcepts(motherConcept).Any(concept => concept.Equals(childConcept));

        /// <summary>
        /// Analyzes "NarrowMatch(skosConcept, X)" relations of the concept scheme to answer the narrow match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetNarrowMatchConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> narrowMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:narrowMatch concepts
                foreach (RDFTriple narrowMatchRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NARROW_MATCH, null, null])
                    narrowMatchConcepts.Add((RDFResource)narrowMatchRelation.Object);
            }

            return narrowMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "RelatedMatch(leftConcept,rightConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasRelatedMatchConcept(this SKOSConceptScheme conceptScheme, RDFResource leftConcept, RDFResource rightConcept)
            => rightConcept != null && leftConcept != null && conceptScheme != null && conceptScheme.GetRelatedMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "RelatedMatch(skosConcept, X)" relations of the concept scheme to answer the related match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetRelatedMatchConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> relatedMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:relatedMatch concepts
                foreach (RDFTriple relatedMatchRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.RELATED_MATCH, null, null])
                    relatedMatchConcepts.Add((RDFResource)relatedMatchRelation.Object);
            }

            return relatedMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "CloseMatch(leftConcept,rightConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasCloseMatchConcept(this SKOSConceptScheme conceptScheme, RDFResource leftConcept, RDFResource rightConcept)
            => rightConcept != null && leftConcept != null && conceptScheme != null && conceptScheme.GetCloseMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "CloseMatch(skosConcept, X)" relations of the concept scheme to answer the close match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetCloseMatchConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> closeMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:closeMatch concepts
                foreach (RDFTriple closeMatchRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.CLOSE_MATCH, null, null])
                    closeMatchConcepts.Add((RDFResource)closeMatchRelation.Object);
            }

            return closeMatchConcepts;
        }

        /// <summary>
        /// Checks for the existence of "ExactMatch(leftConcept,rightConcept)" relations within the concept scheme
        /// </summary>
        public static bool CheckHasExactMatchConcept(this SKOSConceptScheme conceptScheme, RDFResource leftConcept, RDFResource rightConcept)
            => leftConcept != null && rightConcept != null && conceptScheme != null && conceptScheme.GetExactMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));

        /// <summary>
        /// Analyzes "ExactMatch(skosConcept, X)" relations of the concept scheme to answer the exact match concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetExactMatchConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> exactMatchConcepts = new List<RDFResource>();

            if (conceptScheme != null && skosConcept != null)
            {
                exactMatchConcepts.AddRange(conceptScheme.FindExactMatchConcepts(skosConcept, new Dictionary<long, RDFResource>()));

                //We don't want to also enlist the given skos:Concept
                exactMatchConcepts.RemoveAll(concept => concept.Equals(skosConcept));
            }

            return RDFQueryUtilities.RemoveDuplicates(exactMatchConcepts);
        }

        /// <summary>
        /// Finds "ExactMatch(skosConcept, X)" relations to enlist the exact match concepts of the given skos:Concept
        /// </summary>
        internal static List<RDFResource> FindExactMatchConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, Dictionary<long, RDFResource> visitContext)
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
            foreach (RDFTriple exactMatchRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.EXACT_MATCH, null, null])
                exactMatchConcepts.Add((RDFResource)exactMatchRelation.Object);
            #endregion

            // Inference: EXACTMATCH(A,B) ^ EXACTMATCH(B,C) -> EXACTMATCH(A,C)
            foreach (RDFResource exactMatchConcept in exactMatchConcepts.ToList())
                exactMatchConcepts.AddRange(conceptScheme.FindExactMatchConcepts(exactMatchConcept, visitContext));

            return exactMatchConcepts;
        }

        /// <summary>
        /// Analyzes "MappingRelation(skosConcept, X)" relations of the concept scheme to answer the mapping related concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetMappingRelatedConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> mappingRelatedConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:mappingRelation concepts
                foreach (RDFTriple mappingRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.MAPPING_RELATION, null, null])
                    mappingRelatedConcepts.Add((RDFResource)mappingRelation.Object);

                //Get indirectly mapped concepts (rdfs:subPropertyOf skos:mappingRelation)
                foreach (RDFResource subMappingRelation in conceptScheme.Ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.SKOS.MAPPING_RELATION))
                    mappingRelatedConcepts.AddRange(conceptScheme.Ontology.Data.ABoxGraph[skosConcept, subMappingRelation, null, null]
                                                      .Select(t => t.Object)
                                                      .OfType<RDFResource>());
            }

            return RDFQueryUtilities.RemoveDuplicates(mappingRelatedConcepts);
        }

        /// <summary>
        /// Analyzes "SemanticRelation(skosConcept, X)" relations of the concept scheme to answer the semantic related concepts of the given skos:Concept
        /// </summary>
        public static List<RDFResource> GetSemanticRelatedConcepts(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFResource> semanticRelatedConcepts = new List<RDFResource>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:semanticRelation concepts
                foreach (RDFTriple semanticRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.SEMANTIC_RELATION, null, null])
                    semanticRelatedConcepts.Add((RDFResource)semanticRelation.Object);

                //Get indirectly semantic concepts (rdfs:subPropertyOf skos:semanticRelation)
                foreach (RDFResource subSemanticRelation in conceptScheme.Ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.SKOS.SEMANTIC_RELATION))
                    semanticRelatedConcepts.AddRange(conceptScheme.Ontology.Data.ABoxGraph[skosConcept, subSemanticRelation, null, null]
                                                       .Select(t => t.Object)
                                                       .OfType<RDFResource>());
            }

            return RDFQueryUtilities.RemoveDuplicates(semanticRelatedConcepts);
        }

        /// <summary>
        /// Analyzes "Notation(skosConcept, X)" relations of the concept scheme to answer the notations of the given skos:Concept
        /// </summary>
        public static List<RDFLiteral> GetConceptNotations(this SKOSConceptScheme conceptScheme, RDFResource skosConcept)
        {
            List<RDFLiteral> notations = new List<RDFLiteral>();

            if (skosConcept != null && conceptScheme != null)
            {
                //Get skos:notation values
                foreach (RDFTriple notationRelation in conceptScheme.Ontology.Data.ABoxGraph[skosConcept, RDFVocabulary.SKOS.NOTATION, null, null])
                    notations.Add((RDFLiteral)notationRelation.Object);
            }

            return notations;
        }
        #endregion

        #region Checker
        /// <summary>
        /// Checks if the given leftConcept can be skos:[broader|broaderTransitive|broadMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckBroaderCompatibility(this SKOSConceptScheme conceptScheme, RDFResource childConcept, RDFResource motherConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddBroaderRelation = !conceptScheme.CheckHasNarrowerConcept(childConcept, motherConcept);

            //Avoid clash with associative relations
            if (canAddBroaderRelation)
                canAddBroaderRelation = !conceptScheme.CheckHasRelatedConcept(childConcept, motherConcept);

            //Avoid clash with mapping relations
            if (canAddBroaderRelation)
                canAddBroaderRelation = !conceptScheme.CheckHasNarrowMatchConcept(childConcept, motherConcept)
                                          && !conceptScheme.CheckHasCloseMatchConcept(childConcept, motherConcept)
                                            && !conceptScheme.CheckHasExactMatchConcept(childConcept, motherConcept)
                                              && !conceptScheme.CheckHasRelatedMatchConcept(childConcept, motherConcept);

            return canAddBroaderRelation;
        }

        /// <summary>
        /// Checks if the given leftConcept can be skos:[narrower|narrowerTransitive|narrowMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckNarrowerCompatibility(this SKOSConceptScheme conceptScheme, RDFResource motherConcept, RDFResource childConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddNarrowerRelation = !conceptScheme.CheckHasBroaderConcept(motherConcept, childConcept);

            //Avoid clash with associative relations
            if (canAddNarrowerRelation)
                canAddNarrowerRelation = !conceptScheme.CheckHasRelatedConcept(motherConcept, childConcept);

            //Avoid clash with mapping relations
            if (canAddNarrowerRelation)
                canAddNarrowerRelation = !conceptScheme.CheckHasBroadMatchConcept(motherConcept, childConcept)
                                           && !conceptScheme.CheckHasCloseMatchConcept(motherConcept, childConcept)
                                             && !conceptScheme.CheckHasExactMatchConcept(motherConcept, childConcept)
                                               && !conceptScheme.CheckHasRelatedMatchConcept(motherConcept, childConcept);

            return canAddNarrowerRelation;
        }

        /// <summary>
        /// Checks if the given leftConcept can be skos:[related|relatedMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckRelatedCompatibility(this SKOSConceptScheme conceptScheme, RDFResource leftConcept, RDFResource rightConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddRelatedRelation = !conceptScheme.CheckHasBroaderConcept(leftConcept, rightConcept)
                                           && !conceptScheme.CheckHasNarrowerConcept(leftConcept, rightConcept);

            //Avoid clash with mapping relations
            if (canAddRelatedRelation)
                canAddRelatedRelation = !conceptScheme.CheckHasBroadMatchConcept(leftConcept, rightConcept)
                                          && !conceptScheme.CheckHasNarrowMatchConcept(leftConcept, rightConcept)
                                            && !conceptScheme.CheckHasCloseMatchConcept(leftConcept, rightConcept)
                                              && !conceptScheme.CheckHasExactMatchConcept(leftConcept, rightConcept);

            return canAddRelatedRelation;
        }

        /// <summary>
        /// Checks if the given leftConcept can be skos:[closeMatch|exactMatch] than the given rightConcept without tampering SKOS integrity
        /// </summary>
        internal static bool CheckCloseOrExactMatchCompatibility(this SKOSConceptScheme conceptScheme, RDFResource leftConcept, RDFResource rightConcept)
        {
            //Avoid clash with hierarchical relations
            bool canAddCloseOrExactMatchRelation = !conceptScheme.CheckHasBroaderConcept(leftConcept, rightConcept)
                                                     && !conceptScheme.CheckHasNarrowerConcept(leftConcept, rightConcept);

            //Avoid clash with mapping relations
            if (canAddCloseOrExactMatchRelation)
                canAddCloseOrExactMatchRelation = !conceptScheme.CheckHasBroadMatchConcept(leftConcept, rightConcept)
                                                    && !conceptScheme.CheckHasNarrowMatchConcept(leftConcept, rightConcept)
                                                      && !conceptScheme.CheckHasRelatedMatchConcept(leftConcept, rightConcept);

            return canAddCloseOrExactMatchRelation;
        }
        #endregion
    }
}