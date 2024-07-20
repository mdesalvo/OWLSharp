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
    internal static class OWLAsymmetricObjectPropertyAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.AsymmetricObjectPropertyAnalysis.ToString();

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
			OWLIndividualExpression swapIdvExpr;
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            
            //AsymmetricObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV1) -> ERROR
            foreach (OWLAsymmetricObjectProperty asymObjProp in ontology.GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>())
			{
				OWLObjectProperty asymObjPropInvOfValue = (asymObjProp.ObjectPropertyExpression as OWLObjectInverseOf)?.ObjectProperty;
				string asymObjPropXML = asymObjProp.GetXML();

				#region Calibration
                //Extract (calibrated and deduplicated) object assertions of the current asymmetric property
                List <OWLObjectPropertyAssertion> asymObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, asymObjProp.ObjectPropertyExpression);
                for (int i=0; i<asymObjPropAsns.Count; i++)
                {
                    //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (asymObjPropAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {   
                        swapIdvExpr = asymObjPropAsns[i].SourceIndividualExpression;
                        asymObjPropAsns[i].SourceIndividualExpression = asymObjPropAsns[i].TargetIndividualExpression;
                        asymObjPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                        asymObjPropAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }

                    //In case the asymmetric object property works under inverse logic, we must swap source/target of the object assertion
                    if (asymObjPropInvOfValue != null)
                    {
                        swapIdvExpr = asymObjPropAsns[i].SourceIndividualExpression;
                        asymObjPropAsns[i].SourceIndividualExpression = asymObjPropAsns[i].TargetIndividualExpression;
                        asymObjPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                        asymObjPropAsns[i].ObjectPropertyExpression = asymObjPropInvOfValue;
                    }
                }
                asymObjPropAsns = OWLAxiomHelper.RemoveDuplicates(asymObjPropAsns);
                #endregion

				if (asymObjPropAsns.Any(outerAsn => 
						asymObjPropAsns.Any(innerAsn => innerAsn.SourceIndividualExpression.GetIRI().Equals(outerAsn.TargetIndividualExpression.GetIRI())
														 && innerAsn.TargetIndividualExpression.GetIRI().Equals(outerAsn.SourceIndividualExpression.GetIRI()))))
				{
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violation of AsymmetricObjectProperty constraint with definition '{asymObjPropXML}'", 
						"There should not be object assertions switching subject/object under the same asymmetric object property!"));
				}
			}

            return issues;
        }
    }
}