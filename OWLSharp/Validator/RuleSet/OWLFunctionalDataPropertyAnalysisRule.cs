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
using System.Linq;

namespace OWLSharp.Validator
{
    internal static class OWLFunctionalDataPropertyAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.FunctionalDataPropertyAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be functional data properties linking the same individual to more than one literal within DataPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

			//FunctionalDataProperty(FDP) ^ DataPropertyAssertion(FDP,IDV,LIT1) ^ DataPropertyAssertion(FDP,IDV,LIT2) -> ERROR
			foreach (OWLFunctionalDataProperty fdp in ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>())
            	foreach (var dpAsnMap in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, fdp.DataProperty)
																.GroupBy(dpax => dpax.IndividualExpression.GetIRI().ToString())
																.ToDictionary(grp => grp.Key, grp => grp.Select(g => g.Literal))
																.Where(dict => OWLExpressionHelper.RemoveDuplicates(dict.Value.ToList()).Count() > 1))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated FunctionalDataProperty axiom with signature: {fdp.GetXML()}", 
						rulesugg));

            return issues;
        }
    }
}