/*
   Copyright 2012-2024 Marco De Salvo

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

namespace OWLSharp
{
    /// <summary>
    /// OWLUtilities is a collector of reusable utility methods for ontology management
    /// </summary>
    internal static class OWLUtilities
    {
        #region Properties
        /// <summary>
        /// These classes, under "Strict" check policy, are considered reserved
        /// </summary>
        internal static HashSet<long> ReservedClasses { get; set; }

        /// <summary>
        /// These properties, under "Strict" check policy, are considered reserved
        /// </summary>
        internal static HashSet<long> ReservedProperties { get; set; }

        /// <summary>
        /// These properties are always handled as annotation properties for ontologies
        /// </summary>
        internal static HashSet<long> StandardOntologyAnnotations { get; set; }

        /// <summary>
        /// These properties are always handled as annotation properties for resources
        /// </summary>
        internal static HashSet<long> StandardResourceAnnotations { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Static-ctor to initialize properties
        /// </summary>
        static OWLUtilities()
        {
            ReservedClasses = new HashSet<long>()
            {
                //RDF
                RDFVocabulary.RDF.XML_LITERAL.PatternMemberID,
                RDFVocabulary.RDF.HTML.PatternMemberID,
                RDFVocabulary.RDF.JSON.PatternMemberID,
                RDFVocabulary.RDF.PROPERTY.PatternMemberID,
                RDFVocabulary.RDF.STATEMENT.PatternMemberID,
                RDFVocabulary.RDF.ALT.PatternMemberID,
                RDFVocabulary.RDF.BAG.PatternMemberID,
                RDFVocabulary.RDF.SEQ.PatternMemberID,
                RDFVocabulary.RDF.LIST.PatternMemberID,
                RDFVocabulary.RDF.NIL.PatternMemberID,
                //RDFS
                RDFVocabulary.RDFS.RESOURCE.PatternMemberID,
                RDFVocabulary.RDFS.CLASS.PatternMemberID,
                RDFVocabulary.RDFS.CONTAINER.PatternMemberID,
                RDFVocabulary.RDFS.DATATYPE.PatternMemberID,
                RDFVocabulary.RDFS.LITERAL.PatternMemberID,
                //XSD
                RDFVocabulary.XSD.ANY_URI.PatternMemberID,
                RDFVocabulary.XSD.BASE64_BINARY.PatternMemberID,
                RDFVocabulary.XSD.BOOLEAN.PatternMemberID,
                RDFVocabulary.XSD.BYTE.PatternMemberID,
                RDFVocabulary.XSD.DATE.PatternMemberID,
                RDFVocabulary.XSD.DATETIME.PatternMemberID,
                RDFVocabulary.XSD.DATETIMESTAMP.PatternMemberID,
                RDFVocabulary.XSD.DECIMAL.PatternMemberID,
                RDFVocabulary.XSD.DOUBLE.PatternMemberID,
                RDFVocabulary.XSD.DURATION.PatternMemberID,
                RDFVocabulary.XSD.FLOAT.PatternMemberID,
                RDFVocabulary.XSD.G_DAY.PatternMemberID,
                RDFVocabulary.XSD.G_MONTH.PatternMemberID,
                RDFVocabulary.XSD.G_MONTH_DAY.PatternMemberID,
                RDFVocabulary.XSD.G_YEAR.PatternMemberID,
                RDFVocabulary.XSD.G_YEAR_MONTH.PatternMemberID,
                RDFVocabulary.XSD.HEX_BINARY.PatternMemberID,
                RDFVocabulary.XSD.INT.PatternMemberID,
                RDFVocabulary.XSD.INTEGER.PatternMemberID,
                RDFVocabulary.XSD.LANGUAGE.PatternMemberID,
                RDFVocabulary.XSD.LONG.PatternMemberID,
                RDFVocabulary.XSD.NAME.PatternMemberID,
                RDFVocabulary.XSD.NCNAME.PatternMemberID,
                RDFVocabulary.XSD.NEGATIVE_INTEGER.PatternMemberID,
                RDFVocabulary.XSD.NMTOKEN.PatternMemberID,
                RDFVocabulary.XSD.NON_NEGATIVE_INTEGER.PatternMemberID,
                RDFVocabulary.XSD.NON_POSITIVE_INTEGER.PatternMemberID,
                RDFVocabulary.XSD.NORMALIZED_STRING.PatternMemberID,
                RDFVocabulary.XSD.NOTATION.PatternMemberID,
                RDFVocabulary.XSD.POSITIVE_INTEGER.PatternMemberID,
                RDFVocabulary.XSD.QNAME.PatternMemberID,
                RDFVocabulary.XSD.SHORT.PatternMemberID,
                RDFVocabulary.XSD.STRING.PatternMemberID,
                RDFVocabulary.XSD.TIME.PatternMemberID,
                RDFVocabulary.XSD.TOKEN.PatternMemberID,
                RDFVocabulary.XSD.UNSIGNED_BYTE.PatternMemberID,
                RDFVocabulary.XSD.UNSIGNED_INT.PatternMemberID,
                RDFVocabulary.XSD.UNSIGNED_LONG.PatternMemberID,
                RDFVocabulary.XSD.UNSIGNED_SHORT.PatternMemberID,
                //OWL
                RDFVocabulary.OWL.CLASS.PatternMemberID,
                RDFVocabulary.OWL.DEPRECATED_CLASS.PatternMemberID,
                RDFVocabulary.OWL.RESTRICTION.PatternMemberID,
                RDFVocabulary.OWL.DATA_RANGE.PatternMemberID,
                RDFVocabulary.OWL.INDIVIDUAL.PatternMemberID,
                RDFVocabulary.OWL.NAMED_INDIVIDUAL.PatternMemberID,
                RDFVocabulary.OWL.ONTOLOGY.PatternMemberID,
                RDFVocabulary.OWL.ANNOTATION_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.DATATYPE_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.DEPRECATED_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.OBJECT_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.ONTOLOGY_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.FUNCTIONAL_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.SYMMETRIC_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.TRANSITIVE_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.THING.PatternMemberID,
                RDFVocabulary.OWL.NOTHING.PatternMemberID,
                //OWL2
                RDFVocabulary.OWL.ALL_DISJOINT_CLASSES.PatternMemberID,
                RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES.PatternMemberID,
                RDFVocabulary.OWL.ALL_DIFFERENT.PatternMemberID,
                RDFVocabulary.OWL.ASYMMETRIC_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.REFLEXIVE_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION.PatternMemberID,
                RDFVocabulary.OWL.AXIOM.PatternMemberID
            };

            ReservedProperties = new HashSet<long>()
            {
                //RDF
                RDFVocabulary.RDF.FIRST.PatternMemberID,
                RDFVocabulary.RDF.REST.PatternMemberID,
                RDFVocabulary.RDF.LI.PatternMemberID,
                RDFVocabulary.RDF.SUBJECT.PatternMemberID,
                RDFVocabulary.RDF.PREDICATE.PatternMemberID,
                RDFVocabulary.RDF.OBJECT.PatternMemberID,
                RDFVocabulary.RDF.VALUE.PatternMemberID,
                RDFVocabulary.RDF.TYPE.PatternMemberID,
                //RDFS
                RDFVocabulary.RDFS.SUB_CLASS_OF.PatternMemberID,
                RDFVocabulary.RDFS.SUB_PROPERTY_OF.PatternMemberID,
                RDFVocabulary.RDFS.DOMAIN.PatternMemberID,
                RDFVocabulary.RDFS.RANGE.PatternMemberID,
                RDFVocabulary.RDFS.COMMENT.PatternMemberID,
                RDFVocabulary.RDFS.LABEL.PatternMemberID,
                RDFVocabulary.RDFS.SEE_ALSO.PatternMemberID,
                RDFVocabulary.RDFS.IS_DEFINED_BY.PatternMemberID,
                RDFVocabulary.RDFS.MEMBER.PatternMemberID, 
                //OWL
                RDFVocabulary.OWL.EQUIVALENT_CLASS.PatternMemberID,
                RDFVocabulary.OWL.DISJOINT_WITH.PatternMemberID,
                RDFVocabulary.OWL.EQUIVALENT_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.INVERSE_OF.PatternMemberID,
                RDFVocabulary.OWL.ON_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.ONE_OF.PatternMemberID,
                RDFVocabulary.OWL.UNION_OF.PatternMemberID,
                RDFVocabulary.OWL.INTERSECTION_OF.PatternMemberID,
                RDFVocabulary.OWL.COMPLEMENT_OF.PatternMemberID,
                RDFVocabulary.OWL.ALL_VALUES_FROM.PatternMemberID,
                RDFVocabulary.OWL.SOME_VALUES_FROM.PatternMemberID,
                RDFVocabulary.OWL.HAS_VALUE.PatternMemberID,
                RDFVocabulary.OWL.SAME_AS.PatternMemberID,
                RDFVocabulary.OWL.DIFFERENT_FROM.PatternMemberID,
                RDFVocabulary.OWL.MEMBERS.PatternMemberID,
                RDFVocabulary.OWL.DISTINCT_MEMBERS.PatternMemberID,
                RDFVocabulary.OWL.CARDINALITY.PatternMemberID,
                RDFVocabulary.OWL.MIN_CARDINALITY.PatternMemberID,
                RDFVocabulary.OWL.MAX_CARDINALITY.PatternMemberID,
                RDFVocabulary.OWL.VERSION_INFO.PatternMemberID,
                RDFVocabulary.OWL.VERSION_IRI.PatternMemberID,
                RDFVocabulary.OWL.IMPORTS.PatternMemberID,
                RDFVocabulary.OWL.BACKWARD_COMPATIBLE_WITH.PatternMemberID,
                RDFVocabulary.OWL.INCOMPATIBLE_WITH.PatternMemberID,
                RDFVocabulary.OWL.PRIOR_VERSION.PatternMemberID,
                //OWL2
                RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH.PatternMemberID,
                RDFVocabulary.OWL.DISJOINT_UNION_OF.PatternMemberID,
                RDFVocabulary.OWL.HAS_SELF.PatternMemberID,
                RDFVocabulary.OWL.QUALIFIED_CARDINALITY.PatternMemberID,
                RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY.PatternMemberID,
                RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY.PatternMemberID,
                RDFVocabulary.OWL.ON_CLASS.PatternMemberID,
                RDFVocabulary.OWL.ON_DATARANGE.PatternMemberID,
                RDFVocabulary.OWL.SOURCE_INDIVIDUAL.PatternMemberID,
                RDFVocabulary.OWL.ASSERTION_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.TARGET_INDIVIDUAL.PatternMemberID,
                RDFVocabulary.OWL.TARGET_VALUE.PatternMemberID,
                RDFVocabulary.OWL.HAS_KEY.PatternMemberID,
                RDFVocabulary.OWL.PROPERTY_CHAIN_AXIOM.PatternMemberID,
                RDFVocabulary.OWL.TOP_OBJECT_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.TOP_DATA_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.ANNOTATED_SOURCE.PatternMemberID,
                RDFVocabulary.OWL.ANNOTATION_PROPERTY.PatternMemberID,
                RDFVocabulary.OWL.ANNOTATED_TARGET.PatternMemberID
            };

            StandardResourceAnnotations = new HashSet<long>()
            {
                //RDFS
                { RDFVocabulary.RDFS.COMMENT.PatternMemberID },
                { RDFVocabulary.RDFS.LABEL.PatternMemberID },
                { RDFVocabulary.RDFS.SEE_ALSO.PatternMemberID },
                { RDFVocabulary.RDFS.IS_DEFINED_BY.PatternMemberID },
                //OWL
                { RDFVocabulary.OWL.VERSION_INFO.PatternMemberID },
                { RDFVocabulary.OWL.VERSION_IRI.PatternMemberID },
                //DC+DCTERMS
                { RDFVocabulary.DC.CREATOR.PatternMemberID },
                { RDFVocabulary.DC.CONTRIBUTOR.PatternMemberID },
                { RDFVocabulary.DC.PUBLISHER.PatternMemberID },
                { RDFVocabulary.DC.LANGUAGE.PatternMemberID },
                { RDFVocabulary.DC.DATE.PatternMemberID },
                { RDFVocabulary.DC.DESCRIPTION.PatternMemberID },
                { RDFVocabulary.DC.TITLE.PatternMemberID },
                { RDFVocabulary.DC.RIGHTS.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.CREATOR.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.CONTRIBUTOR.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.PUBLISHER.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.LANGUAGE.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.DATE.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.DESCRIPTION.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.TITLE.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.RIGHTS.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.DATE_ACCEPTED.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.DATE_SUBMITTED.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.DATE_COPYRIGHTED.PatternMemberID },
                { RDFVocabulary.DC.DCTERMS.LICENSE.PatternMemberID },
                //SKOS
                { RDFVocabulary.SKOS.ALT_LABEL.PatternMemberID },
                { RDFVocabulary.SKOS.CHANGE_NOTE.PatternMemberID },
                { RDFVocabulary.SKOS.DEFINITION.PatternMemberID },
                { RDFVocabulary.SKOS.EDITORIAL_NOTE.PatternMemberID },
                { RDFVocabulary.SKOS.EXAMPLE.PatternMemberID },
                { RDFVocabulary.SKOS.HIDDEN_LABEL.PatternMemberID },
                { RDFVocabulary.SKOS.HISTORY_NOTE.PatternMemberID },
                { RDFVocabulary.SKOS.NOTE.PatternMemberID },
                { RDFVocabulary.SKOS.PREF_LABEL.PatternMemberID },
                { RDFVocabulary.SKOS.SCOPE_NOTE.PatternMemberID }
            };

            StandardOntologyAnnotations = new HashSet<long>(StandardResourceAnnotations)
            {
                //OWL
                { RDFVocabulary.OWL.PRIOR_VERSION.PatternMemberID },
                { RDFVocabulary.OWL.BACKWARD_COMPATIBLE_WITH.PatternMemberID },
                { RDFVocabulary.OWL.INCOMPATIBLE_WITH.PatternMemberID },
                { RDFVocabulary.OWL.IMPORTS.PatternMemberID }
            };
        }
        #endregion
    
        #region Extensions
        internal static bool IsInference(this RDFTriple triple)
            => triple.TripleMetadata.HasValue && triple.TripleMetadata.Value == RDFModelEnums.RDFTripleMetadata.IsInference;

        internal static RDFTriple SetInference(this RDFTriple triple)
        { 
            triple.SetMetadata(RDFModelEnums.RDFTripleMetadata.IsInference);
            return triple;
        }

        internal static bool IsImport(this RDFTriple triple)
            => triple.TripleMetadata.HasValue && triple.TripleMetadata.Value == RDFModelEnums.RDFTripleMetadata.IsImport;

        internal static RDFTriple SetImport(this RDFTriple triple)
        { 
            triple.SetMetadata(RDFModelEnums.RDFTripleMetadata.IsImport);
            return triple;
        }
        #endregion
    }
}