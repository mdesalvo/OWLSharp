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

namespace OWLSharp.Validator.Rules
{
    internal static class OWLDataPropertyDomainAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.DataPropertyDomainAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be individuals explicitly incompatible with domain class of data properties within DataPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

			//DataPropertyAssertion(DP,IDV,LIT) ^ DataPropertyDomain(DP,C) ^ ClassAssertion(ObjectComplementOf(C),IDV) -> ERROR
			foreach (OWLDataPropertyDomain dpDomain in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>())
            	foreach (OWLDataPropertyAssertion dpAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, dpDomain.DataProperty))
				{
					if (ontology.CheckIsNegativeIndividualOf(dpDomain.ClassExpression, dpAsn.IndividualExpression))
						issues.Add(new OWLIssue(
							OWLEnums.OWLIssueSeverity.Error, 
							rulename, 
							$"Violated DataPropertyDomain axiom with signature: {dpDomain.GetXML()}", 
							rulesugg));
				}

            return issues;
        }
    }
}