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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLSubObjectPropertyOfEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.SubObjectPropertyOfEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();
            
            //Temporary working variables
            OWLIndividualExpression swapIdvExpr;
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                          .Select(ax => (OWLObjectProperty)ax.Expression))
            {
                //SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                //SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)
                List<OWLObjectPropertyExpression> superObjectPropertyExprs = ontology.GetSuperObjectPropertiesOf(declaredObjectProperty);
                foreach (OWLObjectProperty superObjectProperty in superObjectPropertyExprs.OfType<OWLObjectProperty>())
                {
                    OWLSubObjectPropertyOf inference = new OWLSubObjectPropertyOf(declaredObjectProperty, superObjectProperty) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }                    
                foreach (OWLObjectInverseOf superObjectInverseOf in superObjectPropertyExprs.OfType<OWLObjectInverseOf>())
                {
                    OWLSubObjectPropertyOf inference = new OWLSubObjectPropertyOf(declaredObjectProperty, superObjectInverseOf) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }   

                //SubObjectPropertyOf(P1,P2) ^ ObjectPropertyAssertion(P1,I1,I2) -> ObjectPropertyAssertion(P2,I1,I2)
                List<OWLObjectPropertyAssertion> declaredObjectPropertyAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, declaredObjectProperty);
                for (int i = 0; i < declaredObjectPropertyAsns.Count; i++)
                    if (declaredObjectPropertyAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {   
                        swapIdvExpr = declaredObjectPropertyAsns[i].SourceIndividualExpression;
                        declaredObjectPropertyAsns[i].SourceIndividualExpression = declaredObjectPropertyAsns[i].TargetIndividualExpression;
                        declaredObjectPropertyAsns[i].TargetIndividualExpression = swapIdvExpr;
                        declaredObjectPropertyAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }
                foreach (OWLObjectPropertyAssertion declaredObjectPropertyAsn in OWLAxiomHelper.RemoveDuplicates(declaredObjectPropertyAsns))
                    foreach (OWLObjectPropertyExpression superObjectPropertyExpr in superObjectPropertyExprs)
                    {
                        if (superObjectPropertyExpr is OWLObjectInverseOf objInvOf)
                        {
                            OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objInvOf.ObjectProperty, declaredObjectPropertyAsn.TargetIndividualExpression, declaredObjectPropertyAsn.SourceIndividualExpression) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                        else
                        {
                            OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(superObjectPropertyExpr, declaredObjectPropertyAsn.SourceIndividualExpression, declaredObjectPropertyAsn.TargetIndividualExpression) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                    }
            }

            return inferences;
        }
    }
}