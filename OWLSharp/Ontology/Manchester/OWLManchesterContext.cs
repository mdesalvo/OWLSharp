/*
   Copyright 2014-2026 Marco De Salvo

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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLManchesterContext carries the contextual knowledge (prefixes) needed for translating
    /// ontology expressions and axioms to their OWL2/Manchester representation, along with
    /// the shared rendering utilities (IRI abbreviation, annotation rendering, facet mapping).
    /// </summary>
    public sealed class OWLManchesterContext
    {
        #region Properties
        /// <summary>
        /// The set of prefixes usable for abbreviating IRIs during Manchester rendering
        /// </summary>
        internal List<OWLPrefix> Prefixes { get; }

        /// <summary>
        /// Regex validating that the local part of an abbreviated IRI is a legal Manchester simple name
        /// </summary>
        internal static readonly Regex LocalNameRegex = new Regex(@"^[a-zA-Z0-9_][a-zA-Z0-9_\-.]*$", RegexOptions.Compiled);

        /// <summary>
        /// Bidirectional mapping between OWL2 facet IRIs and their Manchester keywords/symbols
        /// </summary>
        internal static readonly Dictionary<string, string> FacetSymbols = new Dictionary<string, string>
        {
            { RDFVocabulary.XSD.LENGTH.ToString(), "length" },
            { RDFVocabulary.XSD.MIN_LENGTH.ToString(), "minLength" },
            { RDFVocabulary.XSD.MAX_LENGTH.ToString(), "maxLength" },
            { RDFVocabulary.XSD.PATTERN.ToString(), "pattern" },
            { RDFVocabulary.RDF.LANG_RANGE.ToString(), "langRange" },
            { RDFVocabulary.XSD.MIN_INCLUSIVE.ToString(), ">=" },
            { RDFVocabulary.XSD.MIN_EXCLUSIVE.ToString(), ">" },
            { RDFVocabulary.XSD.MAX_INCLUSIVE.ToString(), "<=" },
            { RDFVocabulary.XSD.MAX_EXCLUSIVE.ToString(), "<" }
        };
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a Manchester rendering context on the given set of prefixes
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLManchesterContext(List<OWLPrefix> prefixes)
            => Prefixes = prefixes ?? throw new OWLException($"Cannot create OWLManchesterContext because given '{nameof(prefixes)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Abbreviates the given IRI into a Manchester prefixed name, if a declared prefix matches it;
        /// otherwise emits the full IRI form &lt;iri&gt;
        /// </summary>
        public string Abbreviate(RDFResource iri)
        {
            string iriString = iri?.ToString() ?? string.Empty;

            //Search the declared prefix having the longest namespace matching the given IRI
            OWLPrefix bestPrefix = null;
            foreach (OWLPrefix prefix in Prefixes.Where(pfx => iriString.StartsWith(pfx.IRI, System.StringComparison.Ordinal)))
            {
                if (bestPrefix == null || prefix.IRI.Length > bestPrefix.IRI.Length)
                    bestPrefix = prefix;
            }

            //The remainder must be a legal Manchester simple name, otherwise fallback to full IRI form
            if (bestPrefix != null)
            {
                string localName = iriString.Substring(bestPrefix.IRI.Length);
                if (localName.Length > 0 && LocalNameRegex.IsMatch(localName))
                    return $"{bestPrefix.Name}:{localName}";
            }
            return $"<{iriString}>";
        }

        /// <summary>
        /// Renders the given expression, wrapping it in parentheses unless it is self-delimiting
        /// (entities, enumerations) and so can be safely nested into a composite expression
        /// </summary>
        internal string Nest(OWLExpression expression)
        {
            switch (expression)
            {
                case OWLClass _:
                case OWLDatatype _:
                case OWLObjectProperty _:
                case OWLDataProperty _:
                case OWLObjectOneOf _:
                case OWLDataOneOf _:
                    return expression.ToManchesterString(this);
                default:
                    return $"({expression.ToManchesterString(this)})";
            }
        }

        /// <summary>
        /// Renders the given annotation as "annProp annValue", recursively prepending
        /// its eventual nested annotation as an "Annotations: ..." block
        /// </summary>
        internal string RenderAnnotation(OWLAnnotation annotation)
        {
            StringBuilder sb = new StringBuilder();
            if (annotation.Annotation != null)
                sb.Append($"Annotations: {RenderAnnotation(annotation.Annotation)} ");
            sb.Append(annotation.AnnotationProperty.ToManchesterString(this));
            sb.Append(' ');
            if (annotation.ValueIRI != null)
                sb.Append(Abbreviate(new RDFResource(annotation.ValueIRI)));
            else if (annotation.ValueAbbreviatedIRI != null)
                sb.Append(Abbreviate(new RDFResource(string.Concat(annotation.ValueAbbreviatedIRI.Namespace, annotation.ValueAbbreviatedIRI.Name))));
            else if (annotation.ValueAnonymousIndividual != null)
                sb.Append(annotation.ValueAnonymousIndividual.ToManchesterString(this));
            else if (annotation.ValueLiteral != null)
                sb.Append(annotation.ValueLiteral.ToManchesterString(this));
            return sb.ToString();
        }

        /// <summary>
        /// Renders the annotations of an axiom as the "Annotations: ann1, ann2 " block
        /// prepended to the axiom's Manchester frame item (empty string if there are none)
        /// </summary>
        internal string RenderAxiomAnnotations(List<OWLAnnotation> annotations)
            => annotations?.Count > 0
                ? $"Annotations: {string.Join(", ", annotations.Select(RenderAnnotation))} "
                : string.Empty;
        #endregion
    }

    /// <summary>
    /// OWLManchesterFrameKind enumerates the kinds of Manchester frame an axiom can contribute to
    /// </summary>
    internal enum OWLManchesterFrameKind
    {
        Class = 1,
        ObjectProperty = 2,
        DataProperty = 3,
        AnnotationProperty = 4,
        Datatype = 5,
        Individual = 6,
        /* Frame-less section for n-ary axioms (EquivalentClasses:, SameIndividual:, ...) */
        Misc = 7,
        /* Annotations targeting a declared entity of any kind (resolved by the serializer) */
        EntityAnnotation = 8
    }

    /// <summary>
    /// OWLManchesterFrameItem is the contribution of a single axiom to the Manchester rendering of an ontology:
    /// an item under a section of an entity frame, a frame-less misc section, or a bare frame declaration
    /// </summary>
    internal sealed class OWLManchesterFrameItem
    {
        #region Properties
        /// <summary>
        /// The kind of frame this item belongs to
        /// </summary>
        internal OWLManchesterFrameKind FrameKind { get; set; }

        /// <summary>
        /// The rendered name of the entity owning the frame (null for Misc items)
        /// </summary>
        internal string EntityName { get; set; }

        /// <summary>
        /// The section keyword under which this item goes (e.g: "SubClassOf:"; null for bare declarations)
        /// </summary>
        internal string SectionKeyword { get; set; }

        /// <summary>
        /// The rendered Manchester text of this item (null for bare declarations)
        /// </summary>
        internal string ItemText { get; set; }
        #endregion
    }
}
