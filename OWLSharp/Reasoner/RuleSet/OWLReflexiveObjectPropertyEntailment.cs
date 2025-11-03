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
    internal static class OWLReflexiveObjectPropertyEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.ReflexiveObjectPropertyEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            List<OWLReflexiveObjectProperty> reflexiveObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>();
            if (reflexiveObjProps.Count > 0)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

                //ReflexiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(OP,IDV1,IDV1)
                foreach (OWLReflexiveObjectProperty refObjProp in reflexiveObjProps)
                {
                    //Extract object assertions of the current reflexive property
                    foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, refObjProp.ObjectPropertyExpression))
                    {
                        OWLIndividualExpression opAsnSourceIdvExpr = opAsn.SourceIndividualExpression;
                        OWLIndividualExpression opAsnTargetIdvExpr = opAsn.TargetIndividualExpression;

                        //In case the reflexive object property works under inverse logic, we must swap source/target of the object assertion
                        if (refObjProp.ObjectPropertyExpression is OWLObjectInverseOf)
                        {
                            opAsnSourceIdvExpr = opAsn.TargetIndividualExpression;
                            opAsnTargetIdvExpr = opAsn.SourceIndividualExpression;
                        }

                        //Exploit the reflexive object property to emit the "selfswapped-assertion" inference
                        if (refObjProp.ObjectPropertyExpression is OWLObjectInverseOf refObjInvOf)
                        {
                            OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(refObjInvOf.ObjectProperty, opAsnSourceIdvExpr, opAsnSourceIdvExpr) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                        else
                        {
                            OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(refObjProp.ObjectPropertyExpression, opAsnSourceIdvExpr, opAsnSourceIdvExpr) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                    }
                }
            }

            return inferences;
        }
    }
}