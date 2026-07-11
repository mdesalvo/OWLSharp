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
    /// OWLFunctionalContext carries the contextual knowledge (prefixes) needed for translating
    /// ontology expressions and axioms to their OWL2/Functional-Style representation, along with
    /// the shared rendering utilities (IRI abbreviation, annotation rendering).
    /// Unlike OWLManchesterContext, this context does not need a "frame aggregation" concept:
    /// the Functional-Style Syntax has no notion of grouping axioms under an entity frame
    /// (e.g: "Class: Foo ... SubClassOf: ..."). Every construct - be it an expression or an axiom -
    /// is a fully self-delimiting "Keyword( arg1 arg2 ... )" token, so axioms can simply be emitted
    /// one after another inside the enclosing Ontology( ... ) block, in any order the serializer picks.
    /// </summary>
    public sealed class OWLFunctionalContext
    {
        #region Properties
        /// <summary>
        /// The set of prefixes usable for abbreviating IRIs during Functional-Style rendering
        /// </summary>
        internal List<OWLPrefix> Prefixes { get; }

        /// <summary>
        /// Regex validating that the local part of an abbreviated IRI is a legal PN_LOCAL production
        /// (SPARQL/Turtle grammar, conservative subset: the full production also allows escaped reserved
        /// characters and a wide range of Unicode code points, which we deliberately do not attempt to
        /// recognize here - an IRI whose local part relies on those exotic characters simply falls back
        /// to the always-legal full IRI form below, so under-recognizing is safe, over-recognizing is not)
        /// </summary>
        internal static readonly Regex LocalNameRegex = new Regex(@"^[a-zA-Z0-9_][a-zA-Z0-9_\-.]*$", RegexOptions.Compiled);
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a Functional-Style rendering context on the given set of prefixes
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLFunctionalContext(List<OWLPrefix> prefixes)
            => Prefixes = prefixes ?? throw new OWLException($"Cannot create OWLFunctionalContext because given '{nameof(prefixes)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Abbreviates the given IRI into a Functional-Style prefixed name (abbreviatedIRI production),
        /// if a declared prefix matches it; otherwise emits the full IRI form &lt;iri&gt;
        /// (fullIRI production, enclosed in angle brackets as mandated by the grammar)
        /// </summary>
        public string Abbreviate(RDFResource iri)
        {
            string iriString = iri?.ToString() ?? string.Empty;

            //Among all the declared prefixes whose namespace is a prefix of the given IRI string,
            //pick the one with the longest namespace: this is the most specific match, and it is
            //the one that minimizes the risk of an accidental, unintended abbreviation collision
            OWLPrefix bestPrefix = null;
            foreach (OWLPrefix prefix in Prefixes.Where(pfx => iriString.StartsWith(pfx.IRI, System.StringComparison.Ordinal)))
            {
                if (bestPrefix == null || prefix.IRI.Length > bestPrefix.IRI.Length)
                    bestPrefix = prefix;
            }

            //The remainder after stripping the namespace must still be a legal PN_LOCAL, otherwise
            //abbreviating would produce a token the Functional-Style lexer could not re-tokenize back
            //into the same IRI on a subsequent parse - in that case we fall back to the full IRI form
            if (bestPrefix != null)
            {
                string localName = iriString.Substring(bestPrefix.IRI.Length);
                if (localName.Length > 0 && LocalNameRegex.IsMatch(localName))
                    return $"{bestPrefix.Name}:{localName}";
            }

            return $"<{iriString}>";
        }

        /// <summary>
        /// Renders the given annotation as "Annotation( annProp annValue )", recursively prepending
        /// its eventual nested annotation (annotationAnnotations production) as further
        /// "Annotation( ... )" arguments preceding the annotation property and value
        /// </summary>
        internal string RenderAnnotation(OWLAnnotation annotation)
        {
            StringBuilder functionalAnnotation = new StringBuilder("Annotation( ");

            //Nested annotations-on-annotations render as further Annotation(...) tokens injected
            //before the property/value pair, mirroring the annotationAnnotations production
            if (annotation.Annotation != null)
                functionalAnnotation.Append(RenderAnnotation(annotation.Annotation)).Append(' ');

            functionalAnnotation.Append(annotation.AnnotationProperty.ToFunctionalString(this));
            functionalAnnotation.Append(' ');

            //AnnotationValue is one of IRI | AnonymousIndividual | Literal: the OWLAnnotation model
            //keeps these as mutually-exclusive optional fields, so exactly one branch below fires
            if (annotation.ValueIRI != null)
                functionalAnnotation.Append(Abbreviate(new RDFResource(annotation.ValueIRI)));
            else if (annotation.ValueAbbreviatedIRI != null)
                functionalAnnotation.Append(Abbreviate(new RDFResource(string.Concat(annotation.ValueAbbreviatedIRI.Namespace, annotation.ValueAbbreviatedIRI.Name))));
            else if (annotation.ValueAnonymousIndividual != null)
                functionalAnnotation.Append(annotation.ValueAnonymousIndividual.ToFunctionalString(this));
            else if (annotation.ValueLiteral != null)
                functionalAnnotation.Append(annotation.ValueLiteral.ToFunctionalString(this));

            functionalAnnotation.Append(" )");
            return functionalAnnotation.ToString();
        }

        /// <summary>
        /// Renders the axiomAnnotations production shared by every axiom: zero or more
        /// "Annotation( ... )" tokens, space-separated and followed by a trailing space so callers
        /// can directly concatenate it in front of the axiom's own arguments (empty string if there
        /// are no annotations, so the concatenation is a no-op and no stray space is introduced)
        /// </summary>
        internal string RenderAxiomAnnotations(List<OWLAnnotation> annotations)
            => annotations?.Count > 0
                ? string.Concat(annotations.Select(annotation => RenderAnnotation(annotation) + " "))
                : string.Empty;
        #endregion
    }
}