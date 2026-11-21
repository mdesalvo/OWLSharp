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
using System;
using System.Collections.Generic;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLManchesterSWRL renders SWRL rules as the "Rule:" frames written by Protege and the OWL API
    /// and tells which IRIs denote SWRL built-ins, shared by the Manchester serializer and parser
    /// </summary>
    internal static class OWLManchesterSWRL
    {
        #region Statics
        /// <summary>
        /// The IRI namespace under which SWRL variables are stored in the object model
        /// </summary>
        internal const string VariableIRIPrefix = "urn:swrl:var#";

        /// <summary>
        /// The IRI namespace of the SWRL built-ins that OWLSharp offers on top of the standard swrlb ones
        /// </summary>
        private const string ExtensionBuiltInIRIPrefix = "https://github.com/mdesalvo/OWLSharp#";
        #endregion

        #region Methods
        /// <summary>
        /// Tells if the given IRI denotes a SWRL built-in (standard swrlb one, OWLSharp extension or custom registered one)
        /// </summary>
        internal static bool IsBuiltInIRI(string iri)
            => iri.StartsWith(RDFVocabulary.SWRL.SWRLB.BASE_URI, StringComparison.Ordinal)
                || iri.StartsWith(ExtensionBuiltInIRIPrefix, StringComparison.Ordinal)
                || SWRLBuiltInRegister.GetBuiltIn(iri) != null;

        /// <summary>
        /// Renders the given rule as the content of a "Rule:" frame (null if the rule is not representable in Manchester syntax)
        /// </summary>
        internal static string RenderRule(SWRLRule rule, OWLManchesterContext manchesterContext)
        {
            //A rule without body or head has nothing to be written on either side of the arrow
            if (rule.Antecedent == null || rule.Consequent == null)
                return null;

            //Body atoms come first and body built-ins last, since built-ins only filter the bindings that atoms produce
            List<string> bodyItems = new List<string>();
            foreach (SWRLAtom atom in rule.Antecedent.Atoms)
            {
                string renderedAtom = RenderAtom(atom, manchesterContext);
                //A single atom that Manchester cannot express makes the whole rule unrepresentable
                if (renderedAtom == null)
                    return null;
                bodyItems.Add(renderedAtom);
            }
            foreach (SWRLBuiltIn builtIn in rule.Antecedent.BuiltIns)
            {
                string renderedBuiltIn = RenderBuiltIn(builtIn, manchesterContext);
                if (renderedBuiltIn == null)
                    return null;
                bodyItems.Add(renderedBuiltIn);
            }

            //Head holds only atoms, since built-ins are not allowed as consequents
            List<string> headItems = new List<string>();
            foreach (SWRLAtom atom in rule.Consequent.Atoms)
            {
                string renderedAtom = RenderAtom(atom, manchesterContext);
                if (renderedAtom == null)
                    return null;
                headItems.Add(renderedAtom);
            }

            //The grammar requires at least one item on each side of the arrow, otherwise the rule would not read back
            if (bodyItems.Count == 0 || headItems.Count == 0)
                return null;

            return $"{manchesterContext.RenderAxiomAnnotations(rule.Annotations)}{string.Join(", ", bodyItems)} -> {string.Join(", ", headItems)}";
        }

        /// <summary>
        /// Renders the given atom as "predicate(arguments)" (null if its predicate or arguments are not representable in Manchester syntax)
        /// </summary>
        private static string RenderAtom(SWRLAtom atom, OWLManchesterContext manchesterContext)
        {
            string predicateText;
            switch (atom)
            {
                //Manchester atoms are keyed by an entity IRI: complex class expressions and data ranges
                //have no atom syntax, so only named classes and datatypes match these cases
                case SWRLClassAtom _ when atom.Predicate is OWLClass:
                case SWRLDataRangeAtom _ when atom.Predicate is OWLDatatype:
                case SWRLObjectPropertyAtom _ when atom.Predicate is OWLObjectProperty:
                case SWRLDataPropertyAtom _ when atom.Predicate is OWLDataProperty:
                case SWRLAnnotationPropertyAtom _ when atom.Predicate is OWLAnnotationProperty:
                    predicateText = manchesterContext.Abbreviate(atom.Predicate.GetIRI());
                    break;

                //Equality atoms have dedicated keywords, since their predicate is no entity
                case SWRLSameIndividualAtom _:
                    predicateText = "sameAs";
                    break;
                case SWRLDifferentIndividualsAtom _:
                    predicateText = "differentFrom";
                    break;

                //Any other atom shape has no Manchester counterpart
                default:
                    return null;
            }

            //Unary atoms (class and data range) carry only the left argument
            List<SWRLArgument> arguments = new List<SWRLArgument> { atom.LeftArgument };
            if (atom.RightArgument != null)
                arguments.Add(atom.RightArgument);

            return RenderPredicateWithArguments(predicateText, arguments, manchesterContext);
        }

        /// <summary>
        /// Renders the given built-in as "builtinIRI(arguments)" (null if one of its arguments is not representable in Manchester syntax)
        /// </summary>
        private static string RenderBuiltIn(SWRLBuiltIn builtIn, OWLManchesterContext manchesterContext)
            => string.IsNullOrEmpty(builtIn.IRI) || builtIn.Arguments == null
                ? null
                : RenderPredicateWithArguments(manchesterContext.Abbreviate(new RDFResource(builtIn.IRI)), builtIn.Arguments, manchesterContext);

        /// <summary>
        /// Renders the given predicate text followed by its comma-separated arguments in parentheses (null if an argument is not representable)
        /// </summary>
        private static string RenderPredicateWithArguments(string predicateText, List<SWRLArgument> arguments, OWLManchesterContext manchesterContext)
        {
            List<string> argumentTexts = new List<string>();
            foreach (SWRLArgument argument in arguments)
            {
                string argumentText = RenderArgument(argument, manchesterContext);
                //A single argument that Manchester cannot express makes the whole atom unrepresentable
                if (argumentText == null)
                    return null;
                argumentTexts.Add(argumentText);
            }
            return $"{predicateText}({string.Join(", ", argumentTexts)})";
        }

        /// <summary>
        /// Renders the given atom argument as a variable ("?name"), an individual IRI or a literal (null if it is not representable)
        /// </summary>
        private static string RenderArgument(SWRLArgument argument, OWLManchesterContext manchesterContext)
        {
            switch (argument)
            {
                //Variables are written by name only, since the namespace under which the model stores them is internal
                case SWRLVariableArgument variableArgument:
                    string variableName = variableArgument.IRI?.Replace(VariableIRIPrefix, string.Empty);
                    return IsLexableVariableName(variableName) ? $"?{variableName}" : null;

                //Named individuals are written like any other entity reference
                case SWRLIndividualArgument individualArgument:
                    return individualArgument.IRI == null ? null : manchesterContext.Abbreviate(new RDFResource(individualArgument.IRI));

                //Literals reuse the very same renderer that the rest of the Manchester syntax relies on
                case SWRLLiteralArgument literalArgument:
                    return new OWLLiteral { Value = literalArgument.Value, DatatypeIRI = literalArgument.DatatypeIRI, Language = literalArgument.Language }
                            .ToManchesterString(manchesterContext);

                //Unknown argument kinds have no Manchester counterpart
                default:
                    return null;
            }
        }

        /// <summary>
        /// Tells if the given variable name is made only of characters that the Manchester lexer reads back as part of a variable
        /// </summary>
        private static bool IsLexableVariableName(string variableName)
        {
            //A variable without a name would be lexed as an error
            if (string.IsNullOrEmpty(variableName))
                return false;

            foreach (char variableChar in variableName)
                if (!(char.IsLetterOrDigit(variableChar) || variableChar == '_' || variableChar == '-' || variableChar == '.'))
                    return false;
            return true;
        }
        #endregion
    }
}
