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
using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLInverseObjectPropertiesEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.InverseObjectPropertiesEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            List<OWLInverseObjectProperties> invObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();
            if (invObjProps.Count > 0)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

                foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                             .Select(ax => (OWLObjectProperty)ax.Entity))
                {
                    //Extract inverse object properties of the current object property
                    foreach ((bool, OWLObjectPropertyExpression) invOfDeclaredObjectProperty in GetInverseObjectProperties(ontology, declaredObjectProperty, invObjProps))
                    {
                        //InverseObjectProperties(OP,IOP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(IOP,IDV2,IDV1)
                        foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, declaredObjectProperty))
                        {
                            if (invOfDeclaredObjectProperty.Item1)
                            {
                                OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(invOfDeclaredObjectProperty.Item2, opAsn.SourceIndividualExpression, opAsn.TargetIndividualExpression) { IsInference = true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
                            else
                            {
                                OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(invOfDeclaredObjectProperty.Item2, opAsn.TargetIndividualExpression, opAsn.SourceIndividualExpression) { IsInference = true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
                        }

                        //InverseObjectProperties(OP,IOP) ^ EquivalentObjectProperties(IOP,IOP2) -> InverseObjectProperties(OP,IOP2)
                        foreach (OWLObjectPropertyExpression equivOfInvOfDeclaredObjectProperty in ontology.GetEquivalentObjectProperties(invOfDeclaredObjectProperty.Item2))
                        {
                            OWLInverseObjectProperties inferenceA = new OWLInverseObjectProperties(declaredObjectProperty, equivOfInvOfDeclaredObjectProperty) { IsInference = true };
                            inferenceA.GetXML();
                            inferences.Add(new OWLInference(rulename, inferenceA));

                            OWLInverseObjectProperties inferenceB = new OWLInverseObjectProperties(equivOfInvOfDeclaredObjectProperty, declaredObjectProperty) { IsInference = true };
                            inferenceB.GetXML();
                            inferences.Add(new OWLInference(rulename, inferenceB));
                        }
                    }
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
