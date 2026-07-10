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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>W3C OWL2 RL/RDF: eq-diff1, eq-diff2, eq-diff3 (SameIndividual+DifferentIndividuals overlap check). The first check below is an Any/Any scan across the
    /// WHOLE n-ary member list of the axiom (not just adjacent pairs), combined with CheckIsSameIndividual's transitive closure over owl:sameAs -- this already
    /// covers eq-diff2/eq-diff3 (a clash between any two members ?zi/?zj, i != j, of an n-ary AllDifferent), so no separate rule was needed for those.
    /// OWLSharp does not distinguish an AllDifferent's owl:members vs owl:distinctMembers RDF-level encoding: both collapse onto OWLDifferentIndividuals.IndividualExpressions
    /// in the structural model. The DifferentIndividuals(I,I) self-difference check is an OWLSharp extension</para>
    /// </summary>
    internal static class OWLDifferentIndividualsAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.DifferentIndividualsAnalysis);
        internal const string rulesugg = "There should not be named individuals related at the same time by SameIndividual and DifferentIndividuals axioms!";
        internal const string rulesugg2 = "There should not be named individuals being DifferentIndividuals of theirselves!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            List<OWLDifferentIndividuals> diffIdvsAxioms = ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>();
            if (diffIdvsAxioms.Count > 0)
            {
                foreach (OWLDifferentIndividuals diffIdvsAxiom in diffIdvsAxioms)
                {
                    //DifferentIndividuals(IDV1,IDV2) ^ SameIndividual(IDV2,IDV1) -> ERROR
                    if (diffIdvsAxiom.IndividualExpressions.Any(outerIdv =>
                            diffIdvsAxiom.IndividualExpressions.Any(innerIdv => !outerIdv.GetIRI().Equals(innerIdv.GetIRI())
                                                                                  && ontology.CheckIsSameIndividual(outerIdv, innerIdv))))
                    {
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error,
                            rulename,
                            $"Violated DifferentIndividuals axiom with signature: '{diffIdvsAxiom.GetXML()}'",
                            rulesugg));
                    }

                    //DifferentIndividuals(IDV1,IDV1) -> ERROR
                    //Nested i/j loop (j starting at i+1) instead of an Any/Any scan like the check above: here we want each
                    //duplicate pair reported only once, whereas the SameIndividual clash above intentionally allows re-scanning
                    //both directions since it stops at the first hit anyway (bool short-circuit via Any)
                    for (int i=0; i<diffIdvsAxiom.IndividualExpressions.Count-1; i++)
                    {
                        RDFResource outerIdvIRI = diffIdvsAxiom.IndividualExpressions[i].GetIRI();
                        for (int j=i+1; j<diffIdvsAxiom.IndividualExpressions.Count; j++)
                        {
                            RDFResource innerIdvIRI = diffIdvsAxiom.IndividualExpressions[j].GetIRI();
                            if (outerIdvIRI.Equals(innerIdvIRI))
                            {
                                issues.Add(new OWLIssue(
                                    OWLEnums.OWLIssueSeverity.Error,
                                    rulename,
                                    $"Violated DifferentIndividuals axiom with signature: '{diffIdvsAxiom.GetXML()}'",
                                    rulesugg2));
                            }
                        }
                    }
                }
            }

            return issues;
        }
    }
}