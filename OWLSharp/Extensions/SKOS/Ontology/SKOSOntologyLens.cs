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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSOntologyLens represents a magnifying glass on the knowledge available for a SKOS concept within an ontology
    /// </summary>
    public class SKOSOntologyLens
    {
        #region Properties
        /// <summary>
        /// Concept observed by the lens
        /// </summary>
        public RDFResource Concept { get; internal set; }

        /// <summary>
        /// Ontology observed by the lens
        /// </summary>
        public OWLOntology Ontology { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a concept scheme lens for the given skos:Concept instance on the given skos:ConceptScheme
        /// </summary>
        public SKOSOntologyLens(RDFResource skosConcept, OWLOntology ontology)
        {
            Concept = skosConcept ?? throw new OWLException("Cannot create SKOS lens because given \"skosConcept\" parameter is null");
            Ontology = ontology ?? throw new OWLException("Cannot create SKOS lens because given \"ontology\" parameter is null");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks if the lens concept is the top concept of the given concept scheme
        /// </summary>
        public bool IsTopConceptOf(RDFResource skosConceptScheme)
            => Ontology.CheckHasTopConcept(skosConceptScheme, Concept);

        /// <summary>
        /// Asynchronously checks if the lens concept is the top concept of the given concept scheme
        /// </summary>
        public Task<bool> IsTopConceptOfAsync(RDFResource skosConceptScheme)
            => Task.Run(() => IsTopConceptOf(skosConceptScheme));

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:[broader|broaderTransitive]
        /// </summary>
        public List<RDFResource> BroaderConcepts()
            => Ontology.GetBroaderConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:[broader|broaderTransitive]
        /// </summary>
        public Task<List<RDFResource>> BroaderConceptsAsync()
            => Task.Run(() => BroaderConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:broadMatch
        /// </summary>
        public List<RDFResource> BroadMatchConcepts()
            => Ontology.GetBroadMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:broadMatch
        /// </summary>
        public Task<List<RDFResource>> BroadMatchConceptsAsync()
            => Task.Run(() => BroadMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:[narrower|narrowerTransitive]
        /// </summary>
        public List<RDFResource> NarrowerConcepts()
            => Ontology.GetNarrowerConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:[broader|broaderTransitive]
        /// </summary>
        public Task<List<RDFResource>> NarrowerConceptsAsync()
            => Task.Run(() => NarrowerConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:narrowMatch
        /// </summary>
        public List<RDFResource> NarrowMatchConcepts()
            => Ontology.GetNarrowMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:narrowMatch
        /// </summary>
        public Task<List<RDFResource>> NarrowMatchConceptsAsync()
            => Task.Run(() => NarrowMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:closeMatch
        /// </summary>
        public List<RDFResource> CloseMatchConcepts()
            => Ontology.GetCloseMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:closeMatch
        /// </summary>
        public Task<List<RDFResource>> CloseMatchConceptsAsync()
            => Task.Run(() => CloseMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:exactMatch
        /// </summary>
        public List<RDFResource> ExactMatchConcepts()
            => Ontology.GetExactMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:exactMatch
        /// </summary>
        public Task<List<RDFResource>> ExactMatchConceptsAsync()
            => Task.Run(() => ExactMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:related
        /// </summary>
        public List<RDFResource> RelatedConcepts()
            => Ontology.GetRelatedConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:related
        /// </summary>
        public Task<List<RDFResource>> RelatedConceptsAsync()
            => Task.Run(() => RelatedConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:relatedMatch
        /// </summary>
        public List<RDFResource> RelatedMatchConcepts()
            => Ontology.GetRelatedMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:relatedMatch
        /// </summary>
        public Task<List<RDFResource>> RelatedMatchConceptsAsync()
            => Task.Run(() => RelatedMatchConcepts());

        /// <summary>
        /// Enlists the concepts which are related with the lens concept by skos:mappingRelation
        /// </summary>
        public List<RDFResource> MappingRelatedConcepts()
            => Ontology.GetMappingRelatedConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concepts which are related with the lens concept by skos:mappingRelation
        /// </summary>
        public Task<List<RDFResource>> MappingRelatedConceptsAsync()
            => Task.Run(() => MappingRelatedConcepts());

        /// <summary>
        /// Enlists the concepts which are related with the lens concept by skos:semanticRelation
        /// </summary>
        public List<RDFResource> SemanticRelatedConcepts()
            => Ontology.GetSemanticRelatedConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concepts which are related with the lens concept by skos:semanticRelation
        /// </summary>
        public Task<List<RDFResource>> SemanticRelatedConceptsAsync()
            => Task.Run(() => SemanticRelatedConcepts());

        /// <summary>
        /// Enlists the skos:notation attributions of the lens concept
        /// </summary>
        public List<RDFTypedLiteral> Notations()
            => Ontology.GetConceptNotations(Concept);

        /// <summary>
        /// Asynchronously enlists the skos:notation attributions of the lens concept
        /// </summary>
        public Task<List<RDFTypedLiteral>> NotationsAsync()
            => Task.Run(() => Notations());

        /// <summary>
        /// Enlists the label relations which are assigned to the lens concept [SKOS-XL]
        /// </summary>
        public List<(RDFResource, RDFResource)> LabelRelations()
        {
            List<(RDFResource, RDFResource)> result = new List<(RDFResource, RDFResource)>();

            //skosxl:PrefLabel
            foreach (RDFTriple prefLabelTriple in Ontology.Data.ABoxGraph[Concept, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, (RDFResource)prefLabelTriple.Object));

            //skosxl:AltLabel
            foreach (RDFTriple altLabelTriple in Ontology.Data.ABoxGraph[Concept, RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, (RDFResource)altLabelTriple.Object));

            //skosxl:HiddenLabel
            foreach (RDFTriple hiddenLabelTriple in Ontology.Data.ABoxGraph[Concept, RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, (RDFResource)hiddenLabelTriple.Object));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the label relations which are assigned to the lens concept [SKOS-XL]
        /// </summary>
        public Task<List<(RDFResource, RDFResource)>> LabelRelationsAsync()
            => Task.Run(() => LabelRelations());

        /// <summary>
        /// Enlists the label annotations which are assigned to the lens concept
        /// </summary>
        public List<(RDFResource, RDFPlainLiteral)> LabelAnnotations()
        {
            List<(RDFResource, RDFPlainLiteral)> result = new List<(RDFResource, RDFPlainLiteral)>();

            //skos:PrefLabel
            foreach (RDFTriple prefLabelTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.PREF_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.PREF_LABEL, (RDFPlainLiteral)prefLabelTriple.Object));

            //skos:AltLabel
            foreach (RDFTriple altLabelTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.ALT_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.ALT_LABEL, (RDFPlainLiteral)altLabelTriple.Object));

            //skos:HiddenLabel
            foreach (RDFTriple hiddenLabelTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.HIDDEN_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.HIDDEN_LABEL, (RDFPlainLiteral)hiddenLabelTriple.Object));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the label annotations which are assigned to the lens concept
        /// </summary>
        public Task<List<(RDFResource, RDFPlainLiteral)>> LabelAnnotationsAsync()
            => Task.Run(() => LabelAnnotations());

        /// <summary>
        /// Enlists the documentation annotations which are assigned to the lens concept
        /// </summary>
        public List<(RDFResource, RDFLiteral)> DocumentationAnnotations()
        {
            List<(RDFResource, RDFLiteral)> result = new List<(RDFResource, RDFLiteral)>();

            //skos:Note
            foreach (RDFTriple noteTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.NOTE, (RDFLiteral)noteTriple.Object));

            //skos:ChangeNote
            foreach (RDFTriple changeNoteTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.CHANGE_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.CHANGE_NOTE, (RDFLiteral)changeNoteTriple.Object));

            //skos:EditorialNote
            foreach (RDFTriple editorialNoteTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.EDITORIAL_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.EDITORIAL_NOTE, (RDFLiteral)editorialNoteTriple.Object));

            //skos:HistoryNote
            foreach (RDFTriple historyNoteTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.HISTORY_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.HISTORY_NOTE, (RDFLiteral)historyNoteTriple.Object));

            //skos:ScopeNote
            foreach (RDFTriple scopeNoteTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.SCOPE_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.SCOPE_NOTE, (RDFLiteral)scopeNoteTriple.Object));

            //skos:Definition
            foreach (RDFTriple definitionTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.DEFINITION, null, null])
                result.Add((RDFVocabulary.SKOS.DEFINITION, (RDFLiteral)definitionTriple.Object));

            //skos:Example
            foreach (RDFTriple exampleTriple in Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.EXAMPLE, null, null])
                result.Add((RDFVocabulary.SKOS.EXAMPLE, (RDFLiteral)exampleTriple.Object));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the documentation annotations which are assigned to the lens concept
        /// </summary>
        public Task<List<(RDFResource, RDFLiteral)>> DocumentationAnnotationsAsync()
            => Task.Run(() => DocumentationAnnotations());
        #endregion
    }
}