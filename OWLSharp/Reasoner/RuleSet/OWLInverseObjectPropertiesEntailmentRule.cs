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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLInverseObjectPropertiesEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
			List<OWLInverseObjectProperties> invObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();

            //InverseObjectProperties(OP,IOP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(IOP,IDV2,IDV1)
			//InverseObjectProperties(OP,IOP) ^ EquivalentObjectProperties(IOP,IOP2) -> InverseObjectProperties(OP,IOP2)
            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                     	 .Select(ax => (OWLObjectProperty)ax.Expression))
			{
				//Extract inverse object properties of the current object property
				List<(bool,OWLObjectPropertyExpression)> invsOfDeclaredObjectProperty = GetInverseObjectProperties(ontology, declaredObjectProperty, invObjProps);

				//Extract object assertions of the current object property
				foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, declaredObjectProperty))
				{
					OWLIndividualExpression opAsnSourceIdvExpr = opAsn.SourceIndividualExpression;
                    OWLIndividualExpression opAsnTargetIdvExpr = opAsn.TargetIndividualExpression;

					//In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (opAsn.ObjectPropertyExpression is OWLObjectInverseOf)
                    {
                        opAsnSourceIdvExpr = opAsn.TargetIndividualExpression;
                        opAsnTargetIdvExpr = opAsn.SourceIndividualExpression;

                        //Directly materialize the inference by swapping the object assertion itself (since it uses an anonymous inverse object property)
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(declaredObjectProperty, opAsnSourceIdvExpr, opAsnTargetIdvExpr) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }

					//Iterate inverse object properties of the current object property in order to materialize the "swapped-assertion" inference
					foreach ((bool,OWLObjectPropertyExpression) invOfDeclaredObjectProperty in invsOfDeclaredObjectProperty)
                    {
                        if (invOfDeclaredObjectProperty.Item1)
                        {
                            OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(invOfDeclaredObjectProperty.Item2, opAsnSourceIdvExpr, opAsnTargetIdvExpr) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                        else
                        {
                            OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(invOfDeclaredObjectProperty.Item2, opAsnTargetIdvExpr, opAsnSourceIdvExpr) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                    }
                }

				//Iterate inverse object properties of the current object property in order to materialize the "swapped-assertion" inference
				foreach ((bool,OWLObjectPropertyExpression) invOfDeclaredObjectProperty in invsOfDeclaredObjectProperty)
					foreach (OWLObjectPropertyExpression equivOfInvOfDeclaredObjectProperty in ontology.GetEquivalentObjectProperties(invOfDeclaredObjectProperty.Item2))
					{
                        OWLInverseObjectProperties inferenceA = new OWLInverseObjectProperties(declaredObjectProperty, equivOfInvOfDeclaredObjectProperty) { IsInference=true };
                        inferenceA.GetXML();
                        inferences.Add(new OWLInference(rulename, inferenceA));

                        OWLInverseObjectProperties inferenceB = new OWLInverseObjectProperties(equivOfInvOfDeclaredObjectProperty, declaredObjectProperty) { IsInference=true };
                        inferenceB.GetXML();
                        inferences.Add(new OWLInference(rulename, inferenceB));
					}
			}

            return inferences;
        }

        internal static List<(bool,OWLObjectPropertyExpression)> GetInverseObjectProperties(this OWLOntology ontology, OWLObjectProperty objProp, List<OWLInverseObjectProperties> invObjProps)
        {
            List<(bool,OWLObjectPropertyExpression)> invObjPropExprs = new List<(bool,OWLObjectPropertyExpression)>();
            if (ontology != null && objProp != null)
            {
                RDFResource objPropExprIRI = objProp.GetIRI();
                foreach (OWLInverseObjectProperties invObjProp in invObjProps)
                {
                    //Item1 is a flag to signal the reasoner that the final inference will need to be locally swapped,
                    //since coming from a ObjectInverseOf(ObjectInverseOf) modeling situation
                    if (invObjProp.LeftObjectPropertyExpression.GetIRI().Equals(objPropExprIRI))
                        invObjPropExprs.Add((false, invObjProp.RightObjectPropertyExpression));
                    if (invObjProp.LeftObjectPropertyExpression is OWLObjectInverseOf leftInvOf && leftInvOf.ObjectProperty.GetIRI().Equals(objPropExprIRI))
                        invObjPropExprs.Add((true, invObjProp.RightObjectPropertyExpression));
                    if (invObjProp.RightObjectPropertyExpression.GetIRI().Equals(objPropExprIRI))
                        invObjPropExprs.Add((false, invObjProp.LeftObjectPropertyExpression));
                    if (invObjProp.RightObjectPropertyExpression is OWLObjectInverseOf rightInvOf && rightInvOf.ObjectProperty.GetIRI().Equals(objPropExprIRI))
                        invObjPropExprs.Add((true, invObjProp.LeftObjectPropertyExpression));
                }
            }
            return invObjPropExprs;
        }
    }
}
