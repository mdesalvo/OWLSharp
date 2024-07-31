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
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator.Rules
{
    internal static class OWLDisjointDataPropertiesAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.DisjointDataPropertiesAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be disjoint data properties linking the same individual to the same literal within DataPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

			//Temporary working variables
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
			List<OWLDataPropertyAssertion> disjDtPropAsns = new List<OWLDataPropertyAssertion>();

            //DisjointDataProperties(DP1,DP2) ^ DataPropertyAssertion(DP1,IDV,LIT) ^ DataPropertyAssertion(DP2,IDV,LIT) -> ERROR
			foreach (OWLDisjointDataProperties disjDtProps in ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>())
			{
				disjDtPropAsns.Clear();
				foreach (OWLDataProperty disjDtProp in disjDtProps.DataProperties)
					disjDtPropAsns.AddRange(OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, disjDtProp));

				foreach (var conflictingDisjDtPropAsnsGroup in disjDtPropAsns.GroupBy(dtAsn => new { 
																	Idv = dtAsn.IndividualExpression.GetIRI().ToString(), 
																	Lit = dtAsn.Literal.GetLiteral().ToString() }).Where(g => g.Count() > 1))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated DisjointDataProperties axiom with signature: '{disjDtProps.GetXML()}'", 
						rulesugg));
			}				

            return issues;
        }
    }
}