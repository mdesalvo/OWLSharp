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

using System.Linq;
using System.Text;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLFunctionalSerializer orchestrates the OWL2/Functional-Style rendering of an ontology.
    /// Unlike OWLManchesterSerializer, there is no frame-collection phase: every axiom already renders
    /// itself to a complete, self-delimiting "Keyword( ... )" token via ToFunctionalString, so this
    /// class only has to emit the prefix declarations, the Ontology(...) header, and then every axiom
    /// token in a fixed, readable order (declarations first, so every entity is introduced before it is
    /// used - not required by the grammar, which allows any order, but easier to read by a human).
    /// </summary>
    internal static class OWLFunctionalSerializer
    {
        #region Methods
        /// <summary>
        /// Serializes the given ontology to its OWL2/Functional-Style document representation
        /// </summary>
        internal static string SerializeOntology(OWLOntology ontology)
        {
            OWLFunctionalContext functionalContext = new OWLFunctionalContext(ontology.Prefixes);
            StringBuilder functionalDocument = new StringBuilder();

            //Prefix declarations always precede the Ontology(...) block, per the ontologyDocument production
            foreach (OWLPrefix prefix in ontology.Prefixes)
                functionalDocument.AppendLine($"Prefix({prefix.Name}:=<{prefix.IRI}>)");
            functionalDocument.AppendLine();

            //Ontology declaration
            functionalDocument.Append("Ontology(");
            if (!string.IsNullOrEmpty(ontology.IRI))
                functionalDocument.Append($" <{ontology.IRI}>");
            //A version IRI is only meaningful (and only grammatically legal) alongside an ontology IRI
            if (!string.IsNullOrEmpty(ontology.IRI) && !string.IsNullOrEmpty(ontology.VersionIRI))
                functionalDocument.Append($" <{ontology.VersionIRI}>");
            functionalDocument.AppendLine();

            foreach (OWLImport import in ontology.Imports)
                functionalDocument.AppendLine($"    Import( <{import.IRI}> )");

            foreach (string renderedOntologyAnnotation in ontology.Annotations.Select(functionalContext.RenderAnnotation))
                functionalDocument.AppendLine($"    {renderedOntologyAnnotation}");

            //Every axiom family in turn: declarations first (so a human reader meets each entity's kind
            //before seeing it used), then the remaining families in the same order OWLManchesterSerializer
            //already uses, for consistency between the two syntaxes' emitted document structure
            AppendAxioms(functionalDocument, ontology.DeclarationAxioms, functionalContext);
            AppendAxioms(functionalDocument, ontology.DatatypeDefinitionAxioms, functionalContext);
            AppendAxioms(functionalDocument, ontology.ClassAxioms, functionalContext);
            AppendAxioms(functionalDocument, ontology.ObjectPropertyAxioms, functionalContext);
            AppendAxioms(functionalDocument, ontology.DataPropertyAxioms, functionalContext);
            AppendAxioms(functionalDocument, ontology.KeyAxioms, functionalContext);
            AppendAxioms(functionalDocument, ontology.AssertionAxioms, functionalContext);
            AppendAxioms(functionalDocument, ontology.AnnotationAxioms, functionalContext);

            //Unlike every other axiom family, SWRL rules have no Functional-Style production at all
            //(they belong to the separate SWRL specification, not to the OWL2 Structural Specification),
            //so they are skipped with the same non-fatal warning OWLManchesterSerializer already raises
            foreach (SWRLRule swrlRule in ontology.Rules)
                OWLEvents.RaiseWarning($"SWRL rule '{swrlRule}' is not representable in OWL2/Functional-Style syntax and has been skipped");

            functionalDocument.Append(')');
            return functionalDocument.ToString();
        }

        /// <summary>
        /// Renders and appends every axiom of the given registry, skipping (with a non-fatal warning)
        /// any axiom whose ToFunctionalString override is not yet available for its concrete type
        /// (defensive fallback: every shipped OWLAxiom subclass does override it, so this should never
        /// actually trigger, but it keeps a single malformed axiom from failing the whole serialization)
        /// </summary>
        private static void AppendAxioms<T>(StringBuilder functionalDocument, System.Collections.Generic.List<T> axioms, OWLFunctionalContext functionalContext) where T : OWLAxiom
        {
            foreach (T axiom in axioms)
            {
                string renderedAxiom = axiom.ToFunctionalString(functionalContext);
                if (renderedAxiom == null)
                {
                    OWLEvents.RaiseWarning($"Axiom of type {axiom.GetType().Name} is not representable in OWL2/Functional-Style syntax and has been skipped ({axiom.GetXML()})");
                    continue;
                }
                functionalDocument.AppendLine($"    {renderedAxiom}");
            }
        }
        #endregion
    }
}