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
    /// OWLELProfile checks an ontology against the OWL 2 EL profile grammar (https://www.w3.org/TR/owl2-profiles/#OWL_2_EL, §2.2).
    /// Unlike QL and RL, EL does not distinguish subClassExpression/superClassExpression: its ClassExpression grammar is unique.
    /// </summary>
    /// <remarks>
    /// Scaffolding stub: the grammar checks (§2.2.3 ClassExpression, §2.2.4 DataRange, §2.2.5 allowed axiom types,
    /// §2.2.1 allowed datatypes) are filled in during the EL implementation phase.
    /// </remarks>
    internal static class OWLELProfile
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLProfiles.EL);

        //Task-returning (not just List<...>) so the whole dispatch chain from OWLProfiler.CheckProfileAsync
        //down to here is genuinely awaited, not synchronously computed and wrapped in Task.FromResult only
        //at the outermost boundary. Right now the body is trivial and could be Task.FromResult in one line,
        //but the signature is fixed from the start so later phases can freely go `async`/await internal
        //helpers (e.g. a future SPARQL-backed datatype lookup) without another breaking signature change.
        internal static Task<List<OWLProfileViolation>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLProfileViolation> violations = new List<OWLProfileViolation>();

            //TODO (EL phase): §2.2.3 ClassExpression grammar (Class|ObjectIntersectionOf|ObjectOneOf[singleton]|
            //ObjectSomeValuesFrom|ObjectHasValue|ObjectHasSelf|DataSomeValuesFrom|DataHasValue) over Walkers.OWLClassAxiomWalker
            //TODO (EL phase): §2.2.4 DataRange grammar (Datatype|DataIntersectionOf|DataOneOf)
            //TODO (EL phase): §2.2.5 allowed ClassAxiom/ObjectPropertyAxiom/DataPropertyAxiom types
            //TODO (EL phase): §2.2.1 allowed datatypes allowlist

            return Task.FromResult(violations);
        }
    }
}
