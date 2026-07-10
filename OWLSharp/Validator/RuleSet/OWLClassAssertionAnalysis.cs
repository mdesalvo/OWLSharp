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
    /// <para>W3C OWL2 RL/RDF: cls-com</para>
    /// </summary>
    internal static class OWLClassAssertionAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.ClassAssertionAnalysis);
        internal const string rulesugg = "There should not be named individuals belonging at the same time to a class expression and its object complement!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();

            //ClassAssertion(CLS,IDV) ^ ClassAssertion(ObjectComplementOf(CLS),IDV) -> ERROR
            //Group all ClassAssertion axioms by individual, so each individual's full set of asserted class expressions
            //can be cross-checked against itself for a complement pair (an individual can be typed with many classes at once)
            foreach (var classAsnMap in clsAsns.GroupBy(clax => clax.IndividualExpression.GetIRI().ToString())
                                               .ToDictionary(grp => grp.Key, grp => grp.Select(g => g.ClassExpression)))
            {
                //Look for a class expression CLS and, among the SAME individual's other class expressions, an ObjectComplementOf(CLS):
                //asserting both means the individual is stated to belong and NOT belong to the same class, which is a contradiction
                if (classAsnMap.Value.Any(outerClassExpr =>
                     classAsnMap.Value.Any(innerClassExpr => !outerClassExpr.GetIRI().Equals(innerClassExpr.GetIRI())
                                                               && outerClassExpr is OWLObjectComplementOf objectComplOf
                                                               && objectComplOf.ClassExpression.GetIRI().Equals(innerClassExpr.GetIRI()))))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated ClassAssertion axioms for named individual with signature: {classAsnMap.Key}",
                        rulesugg));
            }

            return issues;
        }
    }
}