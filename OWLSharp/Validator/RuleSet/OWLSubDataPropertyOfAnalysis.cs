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

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>OWLSharp extension: T-Box overlap check (SubDataPropertyOf vs DisjointDataProperties), no direct RL/RDF correspondent.
    /// Warning, not Error: the clash only forces the involved properties to be equivalent to owl:bottomDataProperty, which is a
    /// modeling smell rather than an ontology-level inconsistency</para>
    /// </summary>
    internal static class OWLSubDataPropertyOfAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.SubDataPropertyOfAnalysis);
        internal const string rulesugg = "A data property should not be asserted as SubDataPropertyOf a property it is also stated DisjointDataProperties with: this forces the sub-property to be equivalent to owl:bottomDataProperty!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //SubDataPropertyOf(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> WARNING
            //NOTE: SubDataPropertyOf(DP1,DP2) combined with the mutual SubDataPropertyOf(DP2,DP1) or with
            //EquivalentDataProperties(DP1,DP2) is NOT flagged: both are just redundant (not contradictory) restatements of DP1=DP2,
            //and mutual SubDataPropertyOf in particular is a common, deliberate idiom for expressing property equivalence
            foreach (OWLSubDataPropertyOf subDataPropertyOf in ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>())
                if (ontology.CheckAreDisjointDataProperties(subDataPropertyOf.SubDataProperty, subDataPropertyOf.SuperDataProperty))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Violated SubDataPropertyOf axiom with signature: '{subDataPropertyOf.GetXML()}'",
                        rulesugg));
                }

            return issues;
        }
    }
}