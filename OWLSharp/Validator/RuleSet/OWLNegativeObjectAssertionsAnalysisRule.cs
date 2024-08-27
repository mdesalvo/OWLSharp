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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator.RuleSet
{
    internal static class OWLNegativeObjectAssertionsAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.NegativeObjectAssertionsAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be object assertions conflicting with negative object assertions!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
			OWLIndividualExpression swapIdvExpr;
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
			List<OWLNegativeObjectPropertyAssertion> nopAsns = ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>();

			#region Calibration (ObjectPropertyAssertion)
			for (int i=0; i<opAsns.Count; i++)
			{
				//In case the object assertion works under inverse logic, we must swap source/target of the object assertion
				if (opAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
				{   
					swapIdvExpr = opAsns[i].SourceIndividualExpression;
					opAsns[i].SourceIndividualExpression = opAsns[i].TargetIndividualExpression;
					opAsns[i].TargetIndividualExpression = swapIdvExpr;
					opAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
				}
			}
			opAsns = OWLAxiomHelper.RemoveDuplicates(opAsns);
			#endregion

			#region Calibration (NegativeObjectPropertyAssertion)
			for (int i=0; i<nopAsns.Count; i++)
			{
				//In case the negative object assertion works under inverse logic, we must swap source/target of the object assertion
				if (nopAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
				{   
					swapIdvExpr = nopAsns[i].SourceIndividualExpression;
					nopAsns[i].SourceIndividualExpression = nopAsns[i].TargetIndividualExpression;
					nopAsns[i].TargetIndividualExpression = swapIdvExpr;
					nopAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
				}
			}
			nopAsns = OWLAxiomHelper.RemoveDuplicates(nopAsns);
			#endregion

			//NegativeObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ERROR
            foreach (OWLNegativeObjectPropertyAssertion nopAsn in nopAsns)
			{
				RDFResource ndpAsnSourceIndividualIRI = nopAsn.SourceIndividualExpression.GetIRI();
				RDFResource ndpAsnTargetIndividualIRI = nopAsn.TargetIndividualExpression.GetIRI();
				foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, nopAsn.ObjectPropertyExpression)
																				  	.Where(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(ndpAsnSourceIndividualIRI)
																				  					 && opAsn.TargetIndividualExpression.GetIRI().Equals(ndpAsnTargetIndividualIRI)))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated NegativeObjectPropertyAssertion axiom with signature: '{nopAsn.GetXML()}'", 
						rulesugg));
			}

            return issues;
        }
    }
}