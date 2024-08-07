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

namespace OWLSharp.Validator.Rules
{
    internal static class OWLObjectPropertyDomainAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.ObjectPropertyDomainAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be individuals explicitly incompatible with domain class of object properties within ObjectPropertyAssertion axioms!";

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

			//ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyDomain(OP,C) ^ ClassAssertion(ObjectComplementOf(C),IDV1) -> ERROR
			foreach (OWLObjectPropertyDomain opDomain in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>())
			{
				bool opDomainUsesObjectInverseOf = opDomain.ObjectPropertyExpression is OWLObjectInverseOf;
            	foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, opDomain.ObjectPropertyExpression))
				{
					if (ontology.CheckIsNegativeIndividualOf(opDomain.ClassExpression, opDomainUsesObjectInverseOf ? opAsn.TargetIndividualExpression : opAsn.SourceIndividualExpression))
						issues.Add(new OWLIssue(
							OWLEnums.OWLIssueSeverity.Error, 
							rulename, 
							$"Violated ObjectPropertyDomain axiom with signature: {opDomain.GetXML()}", 
							rulesugg));
				}
			}

            return issues;
        }
    }
}