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

using System.Collections.Generic;

namespace OWLSharp.Reasoner
{
    /// <summary>
    /// OWLReasonerReport is the outcome of applying a reasoner to an ontology, collecting the full list of
    /// inferences materialized across the two-phase (Schema, then Fact) execution of the OWL2 and SWRL rules,
    /// together with observability data about each phase performed under the configured <see cref="OWLReasonerOptions"/>.
    /// </summary>
    public sealed class OWLReasonerReport
    {
        #region Properties
        /// <summary>
        /// The set of inferences discovered by the reasoner across both phases (empty if no new knowledge could be derived)
        /// </summary>
        public List<OWLInference> Inferences { get; internal set; } = new List<OWLInference>();

        /// <summary>
        /// The number of iterations actually performed by the reasoner's FACT-phase fixpoint loop (assertion/individual-level
        /// propagation, plus SWRL). Does NOT include the Schema phase's own rounds, which are reported separately via
        /// <see cref="SchemaClosureRounds"/>: the Schema phase always closes to a real fixpoint by construction and has
        /// no iteration cap, so mixing its round count into this field would blur its well-established "capped iteration" meaning.
        /// </summary>
        public uint IterationsPerformed { get; internal set; }

        /// <summary>
        /// Indicates that the reasoner's FACT phase stopped because its last iteration produced no new inferences (true),
        /// rather than because it exhausted <see cref="OWLReasonerOptions.MaxAllowedIterations"/> with inferences still
        /// pending (false). Does not reflect the Schema phase, which always reaches a real fixpoint by construction.
        /// </summary>
        public bool ReachedFixpoint { get; internal set; }

        /// <summary>
        /// The number of rounds performed by the reasoner's SCHEMA phase (T-Box -> T-Box closure) before it reached its
        /// own internal fixpoint (or, if <see cref="OWLReasonerOptions.EnableIterativeReasoning"/> was disabled, the single
        /// round it was limited to). Zero if no Schema-tier rule was selected on the reasoner's <see cref="OWLReasoner.Rules"/>.
        /// Unlike <see cref="IterationsPerformed"/>, this count is never capped: the Schema tier is acyclic with respect to
        /// the Fact tier (no Reasoner rule derives a T-Box axiom from an A-Box fact), so its closure is guaranteed to terminate.
        /// </summary>
        public uint SchemaClosureRounds { get; internal set; }
        #endregion

        #region Ctors
        internal OWLReasonerReport() { }
        #endregion
    }
}