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

                foreach (OWLObjectProperty op in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                         .Select(ax => (OWLObjectProperty)ax.Entity))
                {
                    List<OWLObjectPropertyAssertion> opAsnAxioms = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, op);

                    //Exploit an object property view to compute domains and ranges of the current object property
                    OWLObjectPropertyView opView = new OWLObjectPropertyView(op, ontology);
                    List<OWLClassExpression> opDomains = opView.DomainsAsync().GetAwaiter().GetResult();
                    List<OWLClassExpression> opRanges = opView.RangesAsync().GetAwaiter().GetResult();

                    //Extract inverse object properties of the current object property
                    foreach ((bool, OWLObjectPropertyExpression) iop in GetInverseObjectProperties(ontology, op, invObjProps))
                    {
                        //InverseObjectProperties(OP,IOP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(IOP,IDV2,IDV1)
                        foreach (OWLObjectPropertyAssertion opAsnAxiom in opAsnAxioms)
                        {
                            if (iop.Item1)
                            {
                                OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(iop.Item2, opAsnAxiom.SourceIndividualExpression, opAsnAxiom.TargetIndividualExpression) { IsInference=true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
                            else
                            {
                                OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(iop.Item2, opAsnAxiom.TargetIndividualExpression, opAsnAxiom.SourceIndividualExpression) { IsInference=true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
                        }

                        //InverseObjectProperties(OP,IOP) ^ EquivalentObjectProperties(IOP,IOP2) -> InverseObjectProperties(OP,IOP2)
                        foreach (OWLObjectPropertyExpression equivOfIOP in ontology.GetEquivalentObjectProperties(iop.Item2))
                        {
                            OWLInverseObjectProperties inferenceA = new OWLInverseObjectProperties(op, equivOfIOP) { IsInference=true };
                            inferenceA.GetXML();
                            inferences.Add(new OWLInference(rulename, inferenceA));

                            OWLInverseObjectProperties inferenceB = new OWLInverseObjectProperties(equivOfIOP, op) { IsInference=true };
                            inferenceB.GetXML();
                            inferences.Add(new OWLInference(rulename, inferenceB));
                        }

                        //InverseObjectProperties(OP,IOP) ^ ObjectPropertyDomain(OP,C) -> ObjectPropertyRange(IOP,C)
                        foreach (OWLClassExpression opDomain in opDomains)
                        {
                            if (iop.Item1)
                            {
                                OWLObjectPropertyDomain inference = new OWLObjectPropertyDomain(((OWLObjectInverseOf)iop.Item2).ObjectProperty, opDomain) { IsInference=true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
                            else
                            {
                                OWLObjectPropertyRange inference = new OWLObjectPropertyRange(iop.Item2, opDomain) { IsInference=true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
                        }

                        //InverseObjectProperties(OP,IOP) ^ ObjectPropertyRange(OP,C) -> ObjectPropertyDomain(IOP,C)
                        foreach (OWLClassExpression opRange in opRanges)
                        {
                            if (iop.Item1)
                            {
                                OWLObjectPropertyRange inference = new OWLObjectPropertyRange(((OWLObjectInverseOf)iop.Item2).ObjectProperty, opRange) { IsInference = true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
                            else
                            {
                                OWLObjectPropertyDomain inference = new OWLObjectPropertyDomain(iop.Item2, opRange) { IsInference = true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            }
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
