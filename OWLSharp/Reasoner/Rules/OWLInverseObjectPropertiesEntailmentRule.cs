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

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLInverseObjectPropertiesEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

            //InverseObjectProperties(OP,IOP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(IOP,IDV2,IDV1)
            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                     	 .Select(ax => (OWLObjectProperty)ax.Expression))
			{
				RDFResource declaredObjectPropertyIRI = declaredObjectProperty.GetIRI();

				//Extract inverse object properties of the current object property
				List<(bool,OWLObjectPropertyExpression)> invsOfDeclaredObjectProperty = GetInverseObjectProperties(ontology, declaredObjectProperty);

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
						inferences.Add(new OWLObjectPropertyAssertion(declaredObjectProperty, opAsnSourceIdvExpr, opAsnTargetIdvExpr) { IsInference=true });
                    }

					//Iterate inverse object properties of the current object property in order to materialize the "swapped-assertion" inference
					foreach ((bool,OWLObjectPropertyExpression) invOfDeclaredObjectProperty in invsOfDeclaredObjectProperty)
                        inferences.Add(new OWLObjectPropertyAssertion(invOfDeclaredObjectProperty.Item2,
                            invOfDeclaredObjectProperty.Item1 ? opAsnSourceIdvExpr : opAsnTargetIdvExpr,
                            invOfDeclaredObjectProperty.Item1 ? opAsnTargetIdvExpr : opAsnSourceIdvExpr) { IsInference=true });
                }
			}

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }

        internal static List<(bool,OWLObjectPropertyExpression)> GetInverseObjectProperties(this OWLOntology ontology, OWLObjectProperty objProp)
        {
            List<(bool,OWLObjectPropertyExpression)> invObjPropExprs = new List<(bool,OWLObjectPropertyExpression)>();
            if (ontology != null && objProp != null)
            {
                RDFResource objPropExprIRI = objProp.GetIRI();
                foreach (OWLInverseObjectProperties invObjProp in ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>())
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