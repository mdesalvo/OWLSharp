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
    internal static class OWLFunctionalObjectPropertyAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.FunctionalObjectPropertyAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be functional object properties linking the same individual to more than one individual within ObjectPropertyAssertion axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
			OWLIndividualExpression swapIdvExpr;
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

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

			//FunctionalObjectProperty(FOP) ^ ObjectPropertyAssertion(FOP,IDV1,IDV2) ^ ObjectPropertyAssertion(FOP,IDV1,IDV3) ^ DifferentIndividuals(IDV2,IDV3) -> ERROR
			foreach (OWLFunctionalObjectProperty fop in ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>())
			{
				List<OWLObjectPropertyAssertion> fopAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, fop.ObjectPropertyExpression);
            	foreach (OWLObjectPropertyAssertion fopAsn in fopAsns)
				{
					#region Calibration (FunctionalObjectProperty)
					//In case the functional object property works under inverse logic, we must swap source/target of the object assertion
					if (fopAsn.ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
					{   
						swapIdvExpr = fopAsn.SourceIndividualExpression;
						fopAsn.SourceIndividualExpression = fopAsn.TargetIndividualExpression;
						fopAsn.TargetIndividualExpression = swapIdvExpr;
						fopAsn.ObjectPropertyExpression = objInvOf.ObjectProperty;
					}
					#endregion
				}
				fopAsns = OWLAxiomHelper.RemoveDuplicates(fopAsns);

				foreach (var fopAsnMap in fopAsns.GroupBy(opex => opex.SourceIndividualExpression.GetIRI().ToString())
												 .Select(grp => 
												 	new 
													{ 
														FopAsnTargets = OWLExpressionHelper.RemoveDuplicates(grp.Select(g => g.TargetIndividualExpression).ToList()),
														FoundDiffFromTargets = grp.Select(g => g.TargetIndividualExpression)
																				  .Any(outerTgtIdv => grp.Select(g => g.TargetIndividualExpression)
																				  						 .Any(innerTgtIdv => !outerTgtIdv.GetIRI().Equals(innerTgtIdv.GetIRI())
																										 						&& ontology.CheckAreDifferentIndividuals(outerTgtIdv, innerTgtIdv)))
													})
												 .Where(grp => grp.FoundDiffFromTargets && grp.FopAsnTargets.Count() > 1))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated FunctionalObjectProperty axiom with signature: {fop.GetXML()}", 
						rulesugg));
			}

            return issues;
        }
    }
}