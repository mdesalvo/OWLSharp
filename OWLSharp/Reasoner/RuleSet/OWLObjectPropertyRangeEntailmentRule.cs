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

namespace OWLSharp.Reasoner
{
    internal static class OWLObjectPropertyRangeEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.ObjectPropertyRangeEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            List<OWLObjectPropertyRange> objectPropertyRanges = ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>();
            if (objectPropertyRanges.Count > 0)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

                //ObjectPropertyRange(OP,C) ^ ObjectPropertyAssertion(OP, I1, I2) -> ClassAssertion(C,I2)
                foreach (OWLObjectPropertyRange objectPropertyRange in objectPropertyRanges)
                    foreach (OWLObjectPropertyAssertion objectPropertyAssertion in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, objectPropertyRange.ObjectPropertyExpression))
                    {
                        OWLIndividualExpression opAsnSourceIdvExpr = objectPropertyAssertion.SourceIndividualExpression;
                        OWLIndividualExpression opAsnTargetIdvExpr = objectPropertyAssertion.TargetIndividualExpression;

                        //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                        if (objectPropertyAssertion.ObjectPropertyExpression is OWLObjectInverseOf)
                        {
                            opAsnSourceIdvExpr = objectPropertyAssertion.TargetIndividualExpression;
                            opAsnTargetIdvExpr = objectPropertyAssertion.SourceIndividualExpression;
                        }

                        //In case the object property domain works under inverse logic, we must swap source/target of the object assertion
                        if (objectPropertyRange.ObjectPropertyExpression is OWLObjectInverseOf)
                        {
                            OWLClassAssertion inference = new OWLClassAssertion(objectPropertyRange.ClassExpression) { IndividualExpression=opAsnSourceIdvExpr, IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                        else
                        {
                            OWLClassAssertion inference = new OWLClassAssertion(objectPropertyRange.ClassExpression) { IndividualExpression=opAsnTargetIdvExpr, IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                    }
            }

            return inferences;
        }
    }
}