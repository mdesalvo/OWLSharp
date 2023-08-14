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

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSLabelHelper represents the SKOS-XL extension of SKOSConceptScheme for describing relations between skos:Concept and skosxl:Label individuals
    /// </summary>
    public static class SKOSLabelHelper
    {
        #region Declarer
        /// <summary>
        /// Checks for the existence of the given skosxl:Label declaration within the concept scheme [SKOS-XL]
        /// </summary>
        public static bool CheckHasLabel(this SKOSConceptScheme conceptScheme, RDFResource skosxlLabel)
        {
            bool labelFound = false;
            if (skosxlLabel != null && conceptScheme != null)
            {
                IEnumerator<RDFResource> labelsEnumerator = conceptScheme.LabelsEnumerator;
                while (!labelFound && labelsEnumerator.MoveNext())
                    labelFound = labelsEnumerator.Current.Equals(skosxlLabel);
            }
            return labelFound;
        }

        /// <summary>
        /// Checks for the existence of the given skosxl:Label having the given skosxl:literalForm within the concept scheme [SKOS-XL]
        /// </summary>
        public static bool CheckHasLabelWithLiteralForm(this SKOSConceptScheme conceptScheme, RDFResource skosxlLabel, RDFLiteral skosxlLiteralFormValue)
            => CheckHasLabel(conceptScheme, skosxlLabel)
                && conceptScheme.Ontology.Data.ABoxGraph[skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, null, skosxlLiteralFormValue].TriplesCount > 0;

        /// <summary>
        /// Declares the given skosxl:Label instance to the concept scheme [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme DeclareLabel(this SKOSConceptScheme conceptScheme, RDFResource skosLabel)
        {
            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot declare skosxl:Label instance to the concept scheme because given \"conceptScheme\" parameter is null");
            if (skosLabel == null)
                throw new OWLException("Cannot declare skosxl:Label instance to the concept scheme because given \"skosLabel\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            conceptScheme.Ontology.Data.DeclareIndividual(skosLabel);
            conceptScheme.Ontology.Data.DeclareIndividualType(skosLabel, RDFVocabulary.SKOS.SKOSXL.LABEL);
            conceptScheme.Ontology.Data.DeclareObjectAssertion(skosLabel, RDFVocabulary.SKOS.IN_SCHEME, conceptScheme);

            return conceptScheme;
        }

        //ANNOTATIONS

        /// <summary>
        /// Annotates the given skosxl:Label with the given "annotationProperty -> annotationValue" [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme AnnotateLabel(this SKOSConceptScheme conceptScheme, RDFResource skosxlLabel, RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot annotate label because given \"conceptScheme\" parameter is null");
            if (skosxlLabel == null)
                throw new OWLException("Cannot annotate label because given \"skosxlLabel\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate label because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            conceptScheme.Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosxlLabel, annotationProperty, annotationValue));

            return conceptScheme;
        }

        /// <summary>
        /// Annotates the given skosxl:Label with the given "annotationProperty -> annotationValue" [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme AnnotateLabel(this SKOSConceptScheme conceptScheme, RDFResource skosxlLabel, RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot annotate label because given \"conceptScheme\" parameter is null");
            if (skosxlLabel == null)
                throw new OWLException("Cannot annotate label because given \"skosxlLabel\" parameter is null");
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate label because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate label because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            conceptScheme.Ontology.Data.OBoxGraph.AddTriple(new RDFTriple(skosxlLabel, annotationProperty, annotationValue));

            return conceptScheme;
        }

        //RELATIONS

        /// <summary>
        /// Declares the existence of the given "PrefLabel(skosConcept,skosxlLabel) ^ LiteralForm(skosxlLabel,preferredLabelValue)" relations to the concept scheme [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme DeclarePreferredLabel(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, RDFResource skosxlLabel, RDFPlainLiteral preferredLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => conceptScheme.CheckPreferredLabelCompatibility(skosConcept, preferredLabelValue);
            #endregion

            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot declare skosxl:prefLabel relation to the concept scheme because given \"conceptScheme\" parameter is null");
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
                conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, skosxlLabel));
                conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, preferredLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("PrefLabel relation between concept '{0}' and label '{1}' with value '{2}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, skosxlLabel, preferredLabelValue));

            return conceptScheme;
        }

        /// <summary>
        /// Declares the existence of the given "AltLabel(skosConcept,skosxlLabel) ^ LiteralForm(skosxlLabel,alternativeLabelValue)" relations to the concept scheme [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme DeclareAlternativeLabel(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, RDFResource skosxlLabel, RDFPlainLiteral alternativeLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => conceptScheme.CheckAlternativeLabelCompatibility(skosConcept, alternativeLabelValue);
            #endregion

            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot declare skosxl:altLabel relation to the concept scheme because given \"conceptScheme\" parameter is null");
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
                conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, skosxlLabel));
                conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, alternativeLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("AltLabel relation between concept '{0}' and label '{1}' with value '{2}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, skosxlLabel, alternativeLabelValue));

            return conceptScheme;
        }

        /// <summary>
        /// Declares the existence of the given "HiddenLabel(skosConcept,skosxlLabel) ^ LiteralForm(skosxlLabel,hiddenLabelValue)" relations to the concept scheme [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme DeclareHiddenLabel(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, RDFResource skosxlLabel, RDFPlainLiteral hiddenLabelValue)
        {
            #region SKOS Integrity Checks
            bool SKOSIntegrityChecks()
                => conceptScheme.CheckHiddenLabelCompatibility(skosConcept, hiddenLabelValue);
            #endregion

            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot declare skosxl:hiddenLabel relation to the concept scheme because given \"conceptScheme\" parameter is null");
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
                conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosConcept, RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, skosxlLabel));
                conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, hiddenLabelValue));
            }
            else
                OWLEvents.RaiseWarning(string.Format("HiddenLabel relation between concept '{0}' and label '{1}' with value '{2}' cannot be declared to the concept scheme because it would violate SKOS integrity", skosConcept, skosxlLabel, hiddenLabelValue));

            return conceptScheme;
        }

        /// <summary>
        /// Declares the existence of the given "LabelRelation(leftLabel,rightLabel)" relation to the concept scheme [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme DeclareRelatedLabels(this SKOSConceptScheme conceptScheme, RDFResource leftLabel, RDFResource rightLabel)
        {
            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot declare skosxl:labelRelation relation to the concept scheme because given \"conceptScheme\" parameter is null");
            if (leftLabel == null)
                throw new OWLException("Cannot declare skosxl:labelRelation relation to the concept scheme because given \"leftLabel\" parameter is null");
            if (rightLabel == null)
                throw new OWLException("Cannot declare skosxl:labelRelation relation to the concept scheme because given \"rightLabel\" parameter is null");
            if (leftLabel.Equals(rightLabel))
                throw new OWLException("Cannot declare skosxl:labelRelation relation to the concept scheme because given \"leftLabel\" parameter refers to the same label as the given \"rightLabel\" parameter");
            #endregion

            //Add knowledge to the A-BOX
            conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(leftLabel, RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, rightLabel));

            //Also add an automatic A-BOX inference exploiting simmetry of skosxl:labelRelation relation
            conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(rightLabel, RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, leftLabel));

            return conceptScheme;
        }

        /// <summary>
        /// Declares the existence of the given "LiteralForm(skosxlLabel,literalFormValue)" relation to the concept scheme [SKOS-XL]
        /// </summary>
        public static SKOSConceptScheme DeclareLiteralFormOfLabel(this SKOSConceptScheme conceptScheme, RDFResource skosxlLabel, RDFLiteral literalFormValue)
        {
            #region Guards
            if (conceptScheme == null)
                throw new OWLException("Cannot declare skosxl:literalForm relation to the concept scheme because given \"conceptScheme\" parameter is null");
            if (skosxlLabel == null)
                throw new OWLException("Cannot declare skosxl:literalForm relation to the concept scheme because given \"skosxlLabel\" parameter is null");
            if (literalFormValue == null)
                throw new OWLException("Cannot declare skosxl:literalForm relation to the concept scheme because given \"literalFormValue\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            conceptScheme.Ontology.Data.ABoxGraph.AddTriple(new RDFTriple(skosxlLabel, RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, literalFormValue));

            return conceptScheme;
        }
        #endregion

        #region Ckecker
        /// <summary>
        /// Checks if the given skos:Concept can be assigned the given [skos|skosxl]:prefLabel attribution without tampering SKOS integrity
        /// </summary>
        internal static bool CheckPreferredLabelCompatibility(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, RDFPlainLiteral preferredLabelValue)
        {
            //Check skos:prefLabel annotation => no occurrences of the given value's language must be found (in order to accept the annotation)
            RDFSelectQuery skosPrefLabelQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(skosConcept, RDFVocabulary.SKOS.PREF_LABEL, new RDFVariable("?PREFLABEL")))
                    .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?PREFLABEL")))
                    .AddFilter(new RDFLangMatchesFilter(new RDFVariable("?PREFLABEL"), preferredLabelValue.Language)));
            RDFSelectQueryResult skosPrefLabelQueryResult = skosPrefLabelQuery.ApplyToGraph(conceptScheme.Ontology.Data.OBoxGraph);
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
                RDFSelectQueryResult skosxlPrefLabelQueryResult = skosxlPrefLabelQuery.ApplyToGraph(conceptScheme.Ontology.Data.ABoxGraph);
                canAddPreferredLabel = skosxlPrefLabelQueryResult.SelectResultsCount == 0;
            }

            //Check pairwise disjointness with skos:hiddenLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            if (canAddPreferredLabel)
                canAddPreferredLabel = conceptScheme.Ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.HIDDEN_LABEL, null, preferredLabelValue].TriplesCount == 0;

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
                RDFSelectQueryResult skosxlHiddenLabelQueryResult = skosxlHiddenLabelQuery.ApplyToGraph(conceptScheme.Ontology.Data.ABoxGraph);
                canAddPreferredLabel = skosxlHiddenLabelQueryResult.SelectResultsCount == 0;
            }

            return canAddPreferredLabel;
        }

        /// <summary>
        /// Checks if the given skos:Concept can be assigned the given [skos|skosxl]:altLabel attribution without tampering SKOS integrity
        /// </summary>
        internal static bool CheckAlternativeLabelCompatibility(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, RDFPlainLiteral alternativeLabelValue)
        {
            //Check pairwise disjointness with skos:prefLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            bool canAddAlternativeLabel = conceptScheme.Ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.PREF_LABEL, null, alternativeLabelValue].TriplesCount == 0;

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
                RDFSelectQueryResult skosxlPrefLabelQueryResult = skosxlPrefLabelQuery.ApplyToGraph(conceptScheme.Ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlPrefLabelQueryResult.SelectResultsCount == 0;
            }

            //Check pairwise disjointness with skos:hiddenLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            if (canAddAlternativeLabel)
                canAddAlternativeLabel = conceptScheme.Ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.HIDDEN_LABEL, null, alternativeLabelValue].TriplesCount == 0;

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
                RDFSelectQueryResult skosxlHiddenLabelQueryResult = skosxlHiddenLabelQuery.ApplyToGraph(conceptScheme.Ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlHiddenLabelQueryResult.SelectResultsCount == 0;
            }

            return canAddAlternativeLabel;
        }

        /// <summary>
        /// Checks if the given skos:Concept can be assigned the given [skos|skosxl]:hiddenLabel attribution without tampering SKOS integrity
        /// </summary>
        internal static bool CheckHiddenLabelCompatibility(this SKOSConceptScheme conceptScheme, RDFResource skosConcept, RDFPlainLiteral hiddenLabelValue)
        {
            //Check pairwise disjointness with skos:prefLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            bool canAddAlternativeLabel = conceptScheme.Ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.PREF_LABEL, null, hiddenLabelValue].TriplesCount == 0;

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
                RDFSelectQueryResult skosxlPrefLabelQueryResult = skosxlPrefLabelQuery.ApplyToGraph(conceptScheme.Ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlPrefLabelQueryResult.SelectResultsCount == 0;
            }

            //Check pairwise disjointness with skos:altLabel annotation => no occurrences of the given value must be found (in order to accept the annotation)
            if (canAddAlternativeLabel)
                canAddAlternativeLabel = conceptScheme.Ontology.Data.OBoxGraph[skosConcept, RDFVocabulary.SKOS.ALT_LABEL, null, hiddenLabelValue].TriplesCount == 0;

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
                RDFSelectQueryResult skosxlAltLabelQueryResult = skosxlAltLabelQuery.ApplyToGraph(conceptScheme.Ontology.Data.ABoxGraph);
                canAddAlternativeLabel = skosxlAltLabelQueryResult.SelectResultsCount == 0;
            }

            return canAddAlternativeLabel;
        }
        #endregion
    }
}