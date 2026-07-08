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

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLProfileWalker collects logic shared by every specific walker under Profiler/Walkers/ (e.g. OWLClassAxiomWalker
    /// for class axioms, and future walkers over object/data property axioms and data ranges). Each specific walker
    /// is responsible for enumerating the syntactic "slots" of one axiom family; this base class holds the pieces
    /// of logic that do not depend on which axiom family is being walked.
    /// </summary>
    internal static class OWLProfileWalker
    {
        /// <summary>
        /// Inverts a subClassExpression/superClassExpression position (equivClassExpression positions are left unchanged,
        /// since no profile's equivClassExpression grammar admits a polarity-flipping construct like ObjectComplementOf).
        /// </summary>
        /// <remarks>
        /// Used by every walker's recursive descent when it steps into a construct that flips polarity, the way a
        /// type-checker inverts variance when it descends into a contravariant position. In the OWL2 profiles grammars,
        /// the only such construct is ObjectComplementOf: it is admitted only in superclass position (QL, RL), but the
        /// operand it wraps must itself satisfy the subClassExpression grammar, not the superclass one.
        /// </remarks>
        internal static OWLEnums.OWLClassExpressionPosition InvertPosition(OWLEnums.OWLClassExpressionPosition position)
        {
            //Only Sub/Super are meaningful to flip: they are the two "poles" of the QL/RL grammars.
            //Equivalent falls through unchanged because RL's equivClassExpression grammar (the only one
            //using this third position) never admits ObjectComplementOf or any other polarity-flipping construct.
            switch (position)
            {
                case OWLEnums.OWLClassExpressionPosition.SubClass:
                    return OWLEnums.OWLClassExpressionPosition.SuperClass;
                case OWLEnums.OWLClassExpressionPosition.SuperClass:
                    return OWLEnums.OWLClassExpressionPosition.SubClass;
                default:
                    return position;
            }
        }
    }
}
