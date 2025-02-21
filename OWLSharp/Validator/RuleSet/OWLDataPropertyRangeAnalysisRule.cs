/*
  Copyright 2014-2025 Marco De Salvo
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
    internal static class OWLDataPropertyRangeAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.DataPropertyRangeAnalysis.ToString();
        internal const string rulesugg = "There should not be literals incompatible with range expression of data properties within DataPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, Dictionary<string, object> validatorCache)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

            foreach (OWLDataPropertyRange dpRange in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>())
                foreach (OWLDataPropertyAssertion dpAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, dpRange.DataProperty))
                {
                    if (!ontology.CheckIsLiteralOf(dpRange.DataRangeExpression, dpAsn.Literal))
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error, 
                            rulename, 
                            $"Violated DataPropertyRange axiom with signature: {dpRange.GetXML()}", 
                            rulesugg));
                }

            return issues;
        }
    }
}