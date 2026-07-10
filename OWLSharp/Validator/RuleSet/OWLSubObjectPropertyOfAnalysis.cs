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
    /// <para>OWLSharp extension: T-Box overlap check (SubObjectPropertyOf vs DisjointObjectProperties), no direct RL/RDF correspondent.
    /// Warning, not Error: the clash only forces the involved properties to be equivalent to owl:bottomObjectProperty, which is a
    /// modeling smell rather than an ontology-level inconsistency</para>
    /// </summary>
    internal static class OWLSubObjectPropertyOfAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.SubObjectPropertyOfAnalysis);
        internal const string rulesugg = "An object property should not be asserted as SubObjectPropertyOf a property it is also stated DisjointObjectProperties with: this forces the sub-property to be equivalent to owl:bottomObjectProperty!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //SubObjectPropertyOf(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> WARNING
            //NOTE: SubObjectPropertyOf(OP1,OP2) combined with the mutual SubObjectPropertyOf(OP2,OP1) or with
            //EquivalentObjectProperties(OP1,OP2) is NOT flagged: both are just redundant (not contradictory) restatements of OP1=OP2,
            //and mutual SubObjectPropertyOf in particular is a common, deliberate idiom for expressing property equivalence
            foreach (OWLSubObjectPropertyOf subObjectPropertyOf in ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>())
                if (ontology.CheckAreDisjointObjectProperties(subObjectPropertyOf.SubObjectPropertyExpression, subObjectPropertyOf.SuperObjectPropertyExpression))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Violated SubObjectPropertyOf axiom with signature: '{subObjectPropertyOf.GetXML()}'",
                        rulesugg));
                }

            return issues;
        }
    }
}