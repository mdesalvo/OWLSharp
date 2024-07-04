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

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLTransitiveObjectPropertyEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();
            OWLIndividualExpression swapIdvExpr;
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

            //TransitiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV3) -> ObjectPropertyAssertion(OP,IDV1,IDV3)
            foreach (OWLTransitiveObjectProperty trnObjProp in ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>())
			{
                //Extract object assertions of the current transitive property
                List<OWLObjectPropertyAssertion> trnObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, trnObjProp.ObjectPropertyExpression);
                for (int i=0; i<trnObjPropAsns.Count; i++)
                {
                    //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (trnObjPropAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {   
                        swapIdvExpr = trnObjPropAsns[i].SourceIndividualExpression;
                        trnObjPropAsns[i].SourceIndividualExpression = trnObjPropAsns[i].TargetIndividualExpression;
                        trnObjPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                        trnObjPropAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }

                    //In case the transitive object property works under inverse logic, we must swap source/target of the object assertion
                    if (trnObjProp.ObjectPropertyExpression is OWLObjectInverseOf trnObjInvOf)
                    {
                        swapIdvExpr = trnObjPropAsns[i].SourceIndividualExpression;
                        trnObjPropAsns[i].SourceIndividualExpression = trnObjPropAsns[i].TargetIndividualExpression;
                        trnObjPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                        trnObjPropAsns[i].ObjectPropertyExpression = trnObjInvOf.ObjectProperty;
                    }
                }

                //Iterate object assertions to find inference opportunities (transitive closure)
                foreach (IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion> trnObjPropAsn in trnObjPropAsns.GroupBy(asn => asn.SourceIndividualExpression))
				{
					//TODO
                }
			}

            return inferences;
        }
    }
}