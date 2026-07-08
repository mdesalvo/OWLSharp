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
using System.Threading.Tasks;
using OWLSharp.Ontology;

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLQLProfile checks an ontology against the OWL 2 QL profile grammar (https://www.w3.org/TR/owl2-profiles/#OWL_2_QL, §3.2).
    /// QL distinguishes subClassExpression/superClassExpression, but the restriction is qualitative rather than purely
    /// "which constructs are admitted": the same construct (ObjectSomeValuesFrom) takes a different shape depending on
    /// position (unqualified, filler owl:Thing, in subclass position; qualified, filler a named Class, in superclass
    /// position), so predicates must inspect the operand, not just the C# type of the expression.
    /// </summary>
    /// <remarks>
    /// Scaffolding stub: the grammar checks (§3.2.3 sub/superClassExpression, §3.2.4 DataRange, §3.2.5 allowed axiom
    /// types incl. the "no property chains"/"no functional properties" restrictions, §3.2.1 allowed datatypes) are
    /// filled in during the QL implementation phase, last because it is the most delicate of the three (see
    /// owl2_profiles_w3c_spec memory note on verifying the exact qualification rules against the spec text before
    /// finalizing predicates).
    /// </remarks>
    internal static class OWLQLProfile
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLProfiles.QL);

        //See OWLELProfile.ExecuteRuleAsync for why this is Task-returning already, even as a stub.
        internal static Task<List<OWLProfileViolation>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLProfileViolation> violations = new List<OWLProfileViolation>();

            //TODO (QL phase): §3.2.3 subClassExpression grammar (Class|ObjectSomeValuesFrom(OPE,owl:Thing)[unqualified]|DataSomeValuesFrom)
            //over Walkers.OWLClassAxiomWalker
            //TODO (QL phase): §3.2.3 superClassExpression grammar (Class|ObjectIntersectionOf(superCE+)|ObjectComplementOf(subCE)
            //[position inversion]|ObjectSomeValuesFrom(OPE,Class)[qualified]|DataSomeValuesFrom)
            //TODO (QL phase): §3.2.4 DataRange grammar (Datatype|DataIntersectionOf, no DataOneOf)
            //TODO (QL phase): §3.2.5 allowed axiom types (no property chains in SubObjectPropertyOf, no Functional/InverseFunctional/
            //TransitiveObjectProperty, no FunctionalDataProperty, no SameIndividual, no negative property assertions)
            //TODO (QL phase): §3.2.1 allowed datatypes allowlist

            return Task.FromResult(violations);
        }
    }
}
