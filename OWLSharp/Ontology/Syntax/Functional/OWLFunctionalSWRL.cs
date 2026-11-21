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

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLFunctionalSWRL renders SWRL rules as the "DLSafeRule( ... )" productions that OWL API and Protege exchange in OWL2/Functional-Style documents.
    /// </summary>
    internal static class OWLFunctionalSWRL
    {
        #region Methods
        /// <summary>
        /// Gets the "DLSafeRule( annotations Body( ... ) Head( ... ) )" representation of the given SWRL rule
        /// </summary>
        internal static string ToFunctionalString(SWRLRule rule, OWLFunctionalContext functionalContext)
        {
            //A rule read from OWL/XML may lack its body or its head: a missing side renders as an empty one
            List<string> bodyItems = new List<string>();
            if (rule.Antecedent != null)
            {
                //Atoms come first and built-ins last, since the antecedent keeps them in two separate lists
                bodyItems.AddRange(rule.Antecedent.Atoms.Select(atom => RenderAtom(atom, functionalContext)));
                bodyItems.AddRange(rule.Antecedent.BuiltIns.Select(builtIn => RenderBuiltIn(builtIn, functionalContext)));
            }
            List<string> headItems = rule.Consequent != null
                ? rule.Consequent.Atoms.Select(atom => RenderAtom(atom, functionalContext)).ToList()
                : new List<string>();

            //The rule annotations (label and comment among them) precede the body like every axiom annotation
            return $"DLSafeRule( {functionalContext.RenderAxiomAnnotations(rule.Annotations)}{RenderAtomList("Body", bodyItems)} {RenderAtomList("Head", headItems)} )";
        }

        /// <summary>
        /// Renders the given already-rendered atoms as a "Keyword( atom1 atom2 ... )" block (an empty one stays well-formed)
        /// </summary>
        private static string RenderAtomList(string keyword, List<string> renderedAtoms)
            => renderedAtoms.Count > 0
                ? $"{keyword}( {string.Join(" ", renderedAtoms)} )"
                : $"{keyword}( )";

        /// <summary>
        /// Renders the given atom with the Functional-Style keyword that matches its kind
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private static string RenderAtom(SWRLAtom atom, OWLFunctionalContext functionalContext)
        {
            switch (atom)
            {
                //The predicate is a class expression and the only argument an individual
                case SWRLClassAtom _:
                    return $"ClassAtom( {atom.Predicate.ToFunctionalString(functionalContext)} {RenderArgument(atom.LeftArgument, functionalContext)} )";
                //The predicate is a data range and the only argument a data value
                case SWRLDataRangeAtom _:
                    return $"DataRangeAtom( {atom.Predicate.ToFunctionalString(functionalContext)} {RenderArgument(atom.LeftArgument, functionalContext)} )";
                //The predicate is an object property relating two individuals
                case SWRLObjectPropertyAtom _:
                    return $"ObjectPropertyAtom( {atom.Predicate.ToFunctionalString(functionalContext)} {RenderArgument(atom.LeftArgument, functionalContext)} {RenderArgument(atom.RightArgument, functionalContext)} )";
                //The predicate is a data property relating an individual to a data value
                case SWRLDataPropertyAtom _:
                    return $"DataPropertyAtom( {atom.Predicate.ToFunctionalString(functionalContext)} {RenderArgument(atom.LeftArgument, functionalContext)} {RenderArgument(atom.RightArgument, functionalContext)} )";
                //The predicate is an annotation property: this production is an OWLSharp extension to the OWL API grammar
                case SWRLAnnotationPropertyAtom _:
                    return $"AnnotationPropertyAtom( {atom.Predicate.ToFunctionalString(functionalContext)} {RenderArgument(atom.LeftArgument, functionalContext)} {RenderArgument(atom.RightArgument, functionalContext)} )";
                //The predicate is implied by the keyword (owl:sameAs)
                case SWRLSameIndividualAtom _:
                    return $"SameIndividualAtom( {RenderArgument(atom.LeftArgument, functionalContext)} {RenderArgument(atom.RightArgument, functionalContext)} )";
                //The predicate is implied by the keyword (owl:differentFrom)
                case SWRLDifferentIndividualsAtom _:
                    return $"DifferentIndividualsAtom( {RenderArgument(atom.LeftArgument, functionalContext)} {RenderArgument(atom.RightArgument, functionalContext)} )";
                //The atom hierarchy is closed to this assembly: no other kind can reach this point
                default:
                    throw new OWLException($"Cannot render SWRL atom of type {atom.GetType().Name} to OWL2/Functional-Style syntax");
            }
        }

        /// <summary>
        /// Renders the given built-in as "BuiltInAtom( IRI arg1 ... argN )"
        /// </summary>
        private static string RenderBuiltIn(SWRLBuiltIn builtIn, OWLFunctionalContext functionalContext)
        {
            //The built-in is identified by its IRI alone: the evaluator is resolved from it when the rule runs
            List<string> renderedParts = new List<string> { functionalContext.Abbreviate(new RDFResource(builtIn.IRI)) };
            renderedParts.AddRange((builtIn.Arguments ?? new List<SWRLArgument>()).Select(argument => RenderArgument(argument, functionalContext)));

            return $"BuiltInAtom( {string.Join(" ", renderedParts)} )";
        }

        /// <summary>
        /// Renders the given atom argument as a "Variable( IRI )" wrapper or as a bare individual IRI or literal
        /// </summary>
        /// <exception cref="OWLException"></exception>
        private static string RenderArgument(SWRLArgument argument, OWLFunctionalContext functionalContext)
        {
            switch (argument)
            {
                //A variable is the only argument kind that needs its own keyword wrapper
                case SWRLVariableArgument variableArgument:
                    return $"Variable( {functionalContext.Abbreviate(new RDFResource(variableArgument.IRI))} )";
                //A named individual is a bare IRI like everywhere else in the document
                case SWRLIndividualArgument individualArgument:
                    return functionalContext.Abbreviate(new RDFResource(individualArgument.IRI));
                //A literal is copied field by field into its OWL counterpart, which owns the quoting and escaping rules
                case SWRLLiteralArgument literalArgument:
                    return new OWLLiteral { Value = literalArgument.Value, DatatypeIRI = literalArgument.DatatypeIRI, Language = literalArgument.Language }.ToFunctionalString(functionalContext);
                //A missing argument would produce a document that cannot be read back
                default:
                    throw new OWLException("Cannot render SWRL atom to OWL2/Functional-Style syntax because one of its arguments is missing");
            }
        }
        #endregion
    }
}
