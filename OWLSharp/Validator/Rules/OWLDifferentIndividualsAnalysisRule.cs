/*
  Copyright 2014-2024 Marco De Salvo
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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator.Rules
{
    internal static class OWLDifferentIndividualsAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.DifferentIndividualsAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be named individuals related at the same time by SameIndividual and DifferentIndividuals axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //DifferentIndividuals(IDV1,IDV2) ^ SameIndividual(IDV2,IDV1) -> ERROR
            foreach (OWLDifferentIndividuals diffIdvsAxiom in ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>())
			{
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
			}

            return issues;
        }
    }
}