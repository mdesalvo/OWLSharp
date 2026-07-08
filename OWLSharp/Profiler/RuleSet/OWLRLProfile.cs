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
    /// OWLRLProfile checks an ontology against the OWL 2 RL profile grammar (https://www.w3.org/TR/owl2-profiles/#OWL_2_RL, §4.2).
    /// RL distinguishes THREE class expression positions: subClassExpression, superClassExpression, and the stricter
    /// equivClassExpression (used only by EquivalentClasses). ObjectComplementOf, admitted only in superclass position,
    /// wraps a subClassExpression, so recursion must invert position (OWLClassExpressionPositionExtensions.Invert).
    /// </summary>
    /// <remarks>
    /// Scaffolding stub: the grammar checks (§4.2.3 sub/super/equivClassExpression, §4.2.4 DataRange,
    /// §4.2.5 allowed axiom types incl. the DisjointUnion exclusion, §4.2.1 allowed datatypes) are filled in
    /// during the RL implementation phase.
    /// </remarks>
    internal static class OWLRLProfile
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLProfiles.RL);

        //See OWLELProfile.ExecuteRuleAsync for why this is Task-returning already, even as a stub.
        internal static Task<List<OWLProfileViolation>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLProfileViolation> violations = new List<OWLProfileViolation>();

            //TODO (RL phase): §4.2.3 subClassExpression grammar (Class≠owl:Thing|ObjectIntersectionOf|ObjectUnionOf|ObjectOneOf|
            //ObjectSomeValuesFrom(subCE|owl:Thing)|ObjectHasValue|DataSomeValuesFrom|DataHasValue) over Walkers.OWLClassAxiomWalker
            //TODO (RL phase): §4.2.3 superClassExpression grammar (Class≠owl:Thing|ObjectIntersectionOf|ObjectComplementOf(subCE)
            //[position inversion]|ObjectAllValuesFrom|ObjectHasValue|ObjectMaxCardinality(0/1)|DataAllValuesFrom|DataHasValue|DataMaxCardinality(0/1))
            //TODO (RL phase): §4.2.3 equivClassExpression grammar (Class≠owl:Thing|ObjectIntersectionOf|ObjectHasValue|DataHasValue)
            //TODO (RL phase): §4.2.4 DataRange grammar (Datatype|DataIntersectionOf, no DataOneOf)
            //TODO (RL phase): §4.2.5 allowed axiom types (no DisjointUnion, no ReflexiveObjectProperty)
            //TODO (RL phase): §4.2.1 allowed datatypes allowlist (no owl:real/owl:rational)

            return Task.FromResult(violations);
        }
    }
}
