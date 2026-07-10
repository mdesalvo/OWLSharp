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
    /// <para>W3C OWL2 RL/RDF: prp-pdw, prp-adp (A-Box shared-assertion check). The assertions of ALL member properties of the DisjointDataProperties
    /// axiom are flattened into a single list and then grouped by (individual,literal) pair: any group with more than one assertion means at least two
    /// distinct members ?pi/?pj (i != j) of the (possibly n-ary) disjoint set relate the same individual to the same literal, which is exactly the prp-adp
    /// pattern (AllDisjointProperties), so no separate rule was needed for the n-ary case (see ShouldAnalyzeDisjointDataProperties test, which already
    /// exercises a 3-member disjoint list with a non-adjacent clash). The T-Box overlap check against SubDataPropertyOf/EquivalentDataProperties is an
    /// OWLSharp extension (Warning, not Error: the clash only forces the involved properties to be equivalent to owl:bottomDataProperty, which is a
    /// modeling smell rather than an ontology-level inconsistency -- unlike the A-Box check above, which is a genuine one)</para>
    /// </summary>
    internal static class OWLDisjointDataPropertiesAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.DisjointDataPropertiesAnalysis);
        internal const string rulesugg = "There should not be disjoint data properties linking the same individual to the same literal within DataPropertyAssertion axioms!";
        internal const string rulesugg2 = "Data properties should not belong at the same time to DisjointDataProperties and SubDataPropertyOf/EquivalentDataProperties axioms: this forces them to be equivalent to owl:bottomDataProperty!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            List<OWLDisjointDataProperties> disjDtPropsAxms = ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>();
            if (disjDtPropsAxms.Count > 0)
            {
                //Temporary working variables
                List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
                List<OWLDataPropertyAssertion> disjDtPropAsns = new List<OWLDataPropertyAssertion>();

                //DisjointDataProperties(DP1,DP2) ^ DataPropertyAssertion(DP1,IDV,LIT) ^ DataPropertyAssertion(DP2,IDV,LIT) -> ERROR
                foreach (OWLDisjointDataProperties disjDtProps in disjDtPropsAxms)
                {
                    disjDtPropAsns.Clear();
                    //Flatten the assertions of ALL member data properties of this (possibly n-ary) disjoint set into one list,
                    //losing track of which specific property each assertion came from -- irrelevant for this check, since any
                    //two members relating the same (individual,literal) pair violate disjointness regardless of which pair it is
                    foreach (OWLDataProperty disjDtProp in disjDtProps.DataProperties)
                        disjDtPropAsns.AddRange(OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, disjDtProp));

                    //Grouping by (individual,literal) and looking for groups with more than one assertion is what generalizes
                    //the pairwise prp-adp check to the n-ary AllDisjointProperties case without an explicit nested property loop
                    disjDtPropAsns.GroupBy(dtAsn => new {
                                        Idv = dtAsn.IndividualExpression.GetIRI().ToString(),
                                        Lit = dtAsn.Literal.GetLiteral().ToString() })
                                  .Where(g => g.Count() > 1)
                                  .ToList()
                                  .ForEach(dtAsn =>
                                  {
                                      issues.Add(new OWLIssue(
                                          OWLEnums.OWLIssueSeverity.Error,
                                          rulename,
                                          $"Violated DisjointDataProperties axiom with signature: '{disjDtProps.GetXML()}'",
                                          rulesugg));
                                  });

                    //DisjointDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP1,DP2) -> WARNING
                    //DisjointDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP2,DP1) -> WARNING
                    //DisjointDataProperties(DP1,DP2) ^ EquivalentDataProperties(DP1,DP2) -> WARNING
                    if (disjDtProps.DataProperties.Any(outerDP =>
                          disjDtProps.DataProperties.Any(innerDP => !outerDP.GetIRI().Equals(innerDP.GetIRI())
                                                                      && (ontology.CheckIsSubDataPropertyOf(outerDP, innerDP)
                                                                           || ontology.CheckIsSubDataPropertyOf(innerDP, outerDP)
                                                                           || ontology.CheckAreEquivalentDataProperties(outerDP, innerDP)))))
                    {
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Warning,
                            rulename,
                            $"Violated DisjointDataProperties axiom with signature: '{disjDtProps.GetXML()}'",
                            rulesugg2));
                    }
                }
            }

            return issues;
        }
    }
}