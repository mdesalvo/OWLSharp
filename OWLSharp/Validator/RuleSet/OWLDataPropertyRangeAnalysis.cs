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
    /// <para>W3C OWL2 RL/RDF: prp-rng / dt-not-type (consistency-check form for datatype-constrained ranges)</para>
    /// </summary>
    internal static class OWLDataPropertyRangeAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.DataPropertyRangeAnalysis);
        internal const string rulesugg = "There should not be literals incompatible with range expression of data properties within DataPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            List<OWLDataPropertyRange> dpRanges = ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>();
            if (dpRanges.Count > 0)
            {
                //Temporary working variables
                List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

                //DataPropertyAssertion(DP,IDV,LIT) ^ DataPropertyRange(DP,DR) ^ !dt-not-type(LIT,DR) -> ERROR
                //Unlike LiteralDatatypeAnalysis (which only checks a literal's lexical form against ITS OWN declared datatype),
                //this checks the literal against the data range declared for the property it is asserted with (e.g. a facet-restricted
                //range like xsd:integer[>=0]), so a well-formed literal can still violate a stricter property-level range
                foreach (OWLDataPropertyRange dpRange in dpRanges)
                    foreach (OWLDataPropertyAssertion dpAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, dpRange.DataProperty))
                    {
                        if (!ontology.CheckIsLiteralOf(dpRange.DataRangeExpression, dpAsn.Literal))
                            issues.Add(new OWLIssue(
                                OWLEnums.OWLIssueSeverity.Error,
                                rulename,
                                $"Violated DataPropertyRange axiom with signature: {dpRange.GetXML()}",
                                rulesugg));
                    }
            }

            return issues;
        }
    }
}