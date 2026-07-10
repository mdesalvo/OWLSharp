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
    /// <para>OWLSharp extension: T-Box overlap check (EquivalentDataProperties vs DisjointDataProperties), no direct RL/RDF
    /// correspondent. Warning, not Error: the clash only forces the involved properties to be equivalent to owl:bottomDataProperty,
    /// which is a modeling smell rather than an ontology-level inconsistency</para>
    /// </summary>
    internal static class OWLEquivalentDataPropertiesAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.EquivalentDataPropertiesAnalysis);
        internal const string rulesugg = "Data properties should not be asserted as EquivalentDataProperties if they are also stated DisjointDataProperties: this forces them to be equivalent to owl:bottomDataProperty!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //EquivalentDataProperties(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> WARNING
            //Any/Any scan across the whole n-ary member list (not just adjacent pairs): equivalence is stated for the whole
            //group at once, so a single pairwise DisjointDataProperties clash anywhere in the set is enough to flag the axiom.
            //NOTE: EquivalentDataProperties(DP1,DP2) combined with SubDataPropertyOf(DP1,DP2) or SubDataPropertyOf(DP2,DP1) is NOT
            //flagged: an explicit SubDataPropertyOf restating one direction of an already-declared equivalence is redundant, not contradictory
            foreach (OWLEquivalentDataProperties equivDataProps in ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>())
                if (equivDataProps.DataProperties.Any(outerDP =>
                      equivDataProps.DataProperties.Any(innerDP => !outerDP.GetIRI().Equals(innerDP.GetIRI())
                                                                    && ontology.CheckAreDisjointDataProperties(outerDP, innerDP))))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Violated EquivalentDataProperties axiom with signature: '{equivDataProps.GetXML()}'",
                        rulesugg));
                }

            return issues;
        }
    }
}