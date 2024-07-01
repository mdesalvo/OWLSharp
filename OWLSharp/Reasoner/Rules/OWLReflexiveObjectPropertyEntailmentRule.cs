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

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLReflexiveObjectPropertyEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            
            //ReflexiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDVX) -> ObjectPropertyAssertion(OP,IDV1,IDV1)
            foreach (OWLReflexiveObjectProperty refObjProp in ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>())
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

					//In case the object assertion works under inverse logic, we must swap source/target of the object assertion
					if (opAsn.ObjectPropertyExpression is OWLObjectInverseOf)
                        opAsnSourceIdvExpr = opAsnTargetIdvExpr;
                    
                    //Exploit the reflexive object property to emit the "selfswapped-assertion" inference
                    if (refObjProp.ObjectPropertyExpression is OWLObjectInverseOf refObjInvOf)
                        inferences.Add(new OWLObjectPropertyAssertion(refObjInvOf.ObjectProperty, opAsnSourceIdvExpr, opAsnSourceIdvExpr) { IsInference=true });
                    else
                        inferences.Add(new OWLObjectPropertyAssertion(refObjProp.ObjectPropertyExpression, opAsnSourceIdvExpr, opAsnSourceIdvExpr) { IsInference=true });
                }
			}

            return inferences;
        }
    }
}