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
    /// <para>OWLSharp extension: T-Box overlap check (EquivalentClasses vs DisjointClasses); the RL/RDF ruleset assumes consistent T-Box
    /// input rather than flagging redundant/contradictory axiom combinations. Warning, not Error: the clash only forces the involved
    /// classes to be equivalent to owl:Nothing, which is a modeling smell rather than an ontology-level inconsistency</para>
    /// </summary>
    internal static class OWLEquivalentClassesAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.EquivalentClassesAnalysis);
        internal const string rulesugg = "Classes should not be asserted as EquivalentClasses if they are also stated DisjointClasses: this forces them to be equivalent to owl:Nothing!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //EquivalentClasses(CLS1,CLS2) ^ DisjointClasses(CLS1,CLS2) -> WARNING
            //Any/Any scan across the whole n-ary member list (not just adjacent pairs): equivalence is stated for the whole
            //group at once, so a single pairwise DisjointClasses clash anywhere in the set is enough to flag the axiom.
            //NOTE: EquivalentClasses(CLS1,CLS2) combined with SubClassOf(CLS1,CLS2) or SubClassOf(CLS2,CLS1) is NOT flagged: an
            //explicit SubClassOf restating one direction of an already-declared equivalence is redundant, not contradictory
            foreach (OWLEquivalentClasses equivClasses in ontology.GetClassAxiomsOfType<OWLEquivalentClasses>())
                if (equivClasses.ClassExpressions.Any(outerClass =>
                      equivClasses.ClassExpressions.Any(innerClass => !outerClass.GetIRI().Equals(innerClass.GetIRI())
                                                                          && ontology.CheckAreDisjointClasses(outerClass, innerClass))))
                {
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Violated EquivalentClasses axiom with signature: '{equivClasses.GetXML()}'",
                        rulesugg));
                }

            return issues;
        }
    }
}