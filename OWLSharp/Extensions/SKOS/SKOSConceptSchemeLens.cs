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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSConceptSchemeLens represents a magnifying glass on the knowledge available for a SKOS concept within a SKOS concept scheme
    /// </summary>
    public class SKOSConceptSchemeLens
    {
        #region Properties
        /// <summary>
        /// Concept observed by the lens
        /// </summary>
        public RDFResource Concept { get; internal set; }

        /// <summary>
        /// Scheme observed by the lens
        /// </summary>
        public SKOSConceptScheme Scheme { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a concept scheme lens for the given skos:Concept instance on the given skos:ConceptScheme
        /// </summary>
        public SKOSConceptSchemeLens(RDFResource skosConcept, SKOSConceptScheme skosConceptScheme)
        {
            if (skosConcept == null)
                throw new OWLException("Cannot create data lens because given \"skosConcept\" parameter is null");
            if (skosConceptScheme == null)
                throw new OWLException("Cannot create data lens because given \"skosConceptScheme\" parameter is null");

            Concept = skosConcept;
            Scheme = skosConceptScheme;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks if the lens concept is the top concept of the lens scheme
        /// </summary>
        public bool IsTopConcept()
            => Scheme.CheckHasTopConcept(Concept);

        /// <summary>
        /// Asynchronously checks if the lens concept is the top concept of the lens scheme
        /// </summary>
        public Task<bool> IsTopConceptAsync()
            => Task.Run(() => IsTopConcept());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:[broader|broaderTransitive]
        /// </summary>
        public List<RDFResource> BroaderConcepts()
            => Scheme.GetBroaderConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:[broader|broaderTransitive]
        /// </summary>
        public Task<List<RDFResource>> BroaderConceptsAsync()
            => Task.Run(() => BroaderConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:broadMatch
        /// </summary>
        public List<RDFResource> BroadMatchConcepts()
            => Scheme.GetBroadMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:broadMatch
        /// </summary>
        public Task<List<RDFResource>> BroadMatchConceptsAsync()
            => Task.Run(() => BroadMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:[narrower|narrowerTransitive]
        /// </summary>
        public List<RDFResource> NarrowerConcepts()
            => Scheme.GetNarrowerConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:[broader|broaderTransitive]
        /// </summary>
        public Task<List<RDFResource>> NarrowerConceptsAsync()
            => Task.Run(() => NarrowerConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:narrowMatch
        /// </summary>
        public List<RDFResource> NarrowMatchConcepts()
            => Scheme.GetNarrowMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:narrowMatch
        /// </summary>
        public Task<List<RDFResource>> NarrowMatchConceptsAsync()
            => Task.Run(() => NarrowMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:closeMatch
        /// </summary>
        public List<RDFResource> CloseMatchConcepts()
            => Scheme.GetCloseMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:closeMatch
        /// </summary>
        public Task<List<RDFResource>> CloseMatchConceptsAsync()
            => Task.Run(() => CloseMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:exactMatch
        /// </summary>
        public List<RDFResource> ExactMatchConcepts()
            => Scheme.GetExactMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:exactMatch
        /// </summary>
        public Task<List<RDFResource>> ExactMatchConceptsAsync()
            => Task.Run(() => ExactMatchConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:related
        /// </summary>
        public List<RDFResource> RelatedConcepts()
            => Scheme.GetRelatedConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:related
        /// </summary>
        public Task<List<RDFResource>> RelatedConceptsAsync()
            => Task.Run(() => RelatedConcepts());

        /// <summary>
        /// Enlists the concept which are related with the lens concept by skos:relatedMatch
        /// </summary>
        public List<RDFResource> RelatedMatchConcepts()
            => Scheme.GetRelatedMatchConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concept which are related with the lens concept by skos:relatedMatch
        /// </summary>
        public Task<List<RDFResource>> RelatedMatchConceptsAsync()
            => Task.Run(() => RelatedMatchConcepts());

        /// <summary>
        /// Enlists the concepts which are related with the lens concept by skos:mappingRelation
        /// </summary>
        public List<RDFResource> MappingRelatedConcepts()
            => Scheme.GetMappingRelatedConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concepts which are related with the lens concept by skos:mappingRelation
        /// </summary>
        public Task<List<RDFResource>> MappingRelatedConceptsAsync()
            => Task.Run(() => MappingRelatedConcepts());

        /// <summary>
        /// Enlists the concepts which are related with the lens concept by skos:semanticRelation
        /// </summary>
        public List<RDFResource> SemanticRelatedConcepts()
            => Scheme.GetSemanticRelatedConcepts(Concept);

        /// <summary>
        /// Asynchronously enlists the concepts which are related with the lens concept by skos:semanticRelation
        /// </summary>
        public Task<List<RDFResource>> SemanticRelatedConceptsAsync()
            => Task.Run(() => SemanticRelatedConcepts());

        /// <summary>
        /// Enlists the skos:notation attributions of the lens concept
        /// </summary>
        public List<RDFLiteral> Notations()
            => Scheme.GetConceptNotations(Concept);

        /// <summary>
        /// Asynchronously enlists the skos:notation attributions of the lens concept
        /// </summary>
        public Task<List<RDFLiteral>> NotationsAsync()
            => Task.Run(() => Notations());

        /// <summary>
        /// Enlists the label relations which are assigned to the lens concept [SKOS-XL]
        /// </summary>
        public List<(RDFResource, RDFResource)> LabelRelations()
        {
            List<(RDFResource, RDFResource)> result = new List<(RDFResource, RDFResource)>();

            //skosxl:PrefLabel
            foreach (RDFTriple prefLabelTriple in Scheme.Ontology.Data.ABoxGraph[Concept, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, (RDFResource)prefLabelTriple.Object));

            //skosxl:AltLabel
            foreach (RDFTriple altLabelTriple in Scheme.Ontology.Data.ABoxGraph[Concept, RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, (RDFResource)altLabelTriple.Object));

            //skosxl:HiddenLabel
            foreach (RDFTriple hiddenLabelTriple in Scheme.Ontology.Data.ABoxGraph[Concept, RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, null, null])
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
        public List<(RDFResource, RDFLiteral)> LabelAnnotations()
        {
            List<(RDFResource, RDFLiteral)> result = new List<(RDFResource, RDFLiteral)>();

            //skos:PrefLabel
            foreach (RDFTriple prefLabelTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.PREF_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.PREF_LABEL, (RDFLiteral)prefLabelTriple.Object));

            //skos:AltLabel
            foreach (RDFTriple altLabelTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.ALT_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.ALT_LABEL, (RDFLiteral)altLabelTriple.Object));

            //skos:HiddenLabel
            foreach (RDFTriple hiddenLabelTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.HIDDEN_LABEL, null, null])
                result.Add((RDFVocabulary.SKOS.HIDDEN_LABEL, (RDFLiteral)hiddenLabelTriple.Object));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the label annotations which are assigned to the lens concept
        /// </summary>
        public Task<List<(RDFResource, RDFLiteral)>> LabelAnnotationsAsync()
            => Task.Run(() => LabelAnnotations());

        /// <summary>
        /// Enlists the documentation annotations which are assigned to the lens concept
        /// </summary>
        public List<(RDFResource, RDFLiteral)> DocumentationAnnotations()
        {
            List<(RDFResource, RDFLiteral)> result = new List<(RDFResource, RDFLiteral)>();

            //skos:Note
            foreach (RDFTriple noteTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.NOTE, (RDFLiteral)noteTriple.Object));

            //skos:ChangeNote
            foreach (RDFTriple changeNoteTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.CHANGE_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.CHANGE_NOTE, (RDFLiteral)changeNoteTriple.Object));

            //skos:EditorialNote
            foreach (RDFTriple editorialNoteTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.EDITORIAL_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.EDITORIAL_NOTE, (RDFLiteral)editorialNoteTriple.Object));

            //skos:HistoryNote
            foreach (RDFTriple historyNoteTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.HISTORY_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.HISTORY_NOTE, (RDFLiteral)historyNoteTriple.Object));

            //skos:ScopeNote
            foreach (RDFTriple scopeNoteTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.SCOPE_NOTE, null, null])
                result.Add((RDFVocabulary.SKOS.SCOPE_NOTE, (RDFLiteral)scopeNoteTriple.Object));

            //skos:Definition
            foreach (RDFTriple definitionTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.DEFINITION, null, null])
                result.Add((RDFVocabulary.SKOS.DEFINITION, (RDFLiteral)definitionTriple.Object));

            //skos:Example
            foreach (RDFTriple exampleTriple in Scheme.Ontology.Data.OBoxGraph[Concept, RDFVocabulary.SKOS.EXAMPLE, null, null])
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