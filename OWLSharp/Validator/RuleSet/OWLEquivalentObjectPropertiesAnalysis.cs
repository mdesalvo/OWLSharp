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

using OWLSharp.Ontology;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>OWLSharp extension: T-Box overlap check (EquivalentObjectProperties vs DisjointObjectProperties), no direct RL/RDF
    /// correspondent. Warning, not Error: the clash only forces the involved properties to be equivalent to owl:bottomObjectProperty,
    /// which is a modeling smell rather than an ontology-level inconsistency</para>
    /// </summary>
    internal static class OWLEquivalentObjectPropertiesAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.EquivalentObjectPropertiesAnalysis);
        internal const string rulesugg = "Object properties should not be asserted as EquivalentObjectProperties if they are also stated DisjointObjectProperties: this forces them to be equivalent to owl:bottomObjectProperty!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //EquivalentObjectProperties(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> WARNING
            //Any/Any scan across the whole n-ary member list (not just adjacent pairs): equivalence is stated for the whole
            //group at once, so a single pairwise DisjointObjectProperties clash anywhere in the set is enough to flag the axiom.
            //No inverse-property recalibration is needed here (unlike e.g. AsymmetricObjectPropertyAnalysis): this check only compares
            //property IDENTITIES via T-Box relations, it never inspects ObjectPropertyAssertion source/target individuals.
            //NOTE: EquivalentObjectProperties(OP1,OP2) combined with SubObjectPropertyOf(OP1,OP2) or SubObjectPropertyOf(OP2,OP1) is NOT
            //flagged: an explicit SubObjectPropertyOf restating one direction of an already-declared equivalence is redundant, not contradictory
            foreach (OWLEquivalentObjectProperties equivObjectProps in ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>())
                if (equivObjectProps.ObjectPropertyExpressions.Any(outerOPEX =>
                      equivObjectProps.ObjectPropertyExpressions.Any(innerOPEX => !outerOPEX.GetIRI().Equals(innerOPEX.GetIRI())
                                                                                    && ontology.CheckAreDisjointObjectProperties(outerOPEX, innerOPEX))))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Violated EquivalentObjectProperties axiom with signature: '{equivObjectProps.GetXML()}'",
                        rulesugg));
                }

            return issues;
        }
    }
}