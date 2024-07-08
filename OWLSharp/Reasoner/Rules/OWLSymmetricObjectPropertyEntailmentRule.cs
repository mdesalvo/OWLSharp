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
    internal static class OWLSymmetricObjectPropertyEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            //Temporary working variables
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            
            //SymmetricObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(OP,IDV2,IDV1)
            foreach (OWLSymmetricObjectProperty symObjProp in ontology.GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>())
			{
                //Extract object assertions of the current symmetric property
                foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, symObjProp.ObjectPropertyExpression))
				{
					OWLIndividualExpression opAsnSourceIdvExpr = opAsn.SourceIndividualExpression;
                    OWLIndividualExpression opAsnTargetIdvExpr = opAsn.TargetIndividualExpression;

                    //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (opAsn.ObjectPropertyExpression is OWLObjectInverseOf)
                    {
                        opAsnSourceIdvExpr = opAsn.TargetIndividualExpression;
                        opAsnTargetIdvExpr = opAsn.SourceIndividualExpression;
                    }

                    //Exploit the symmetric object property to emit the "swapped-assertion" inference
                    if (symObjProp.ObjectPropertyExpression is OWLObjectInverseOf symObjInvOf)
                        inferences.Add(new OWLObjectPropertyAssertion(symObjInvOf.ObjectProperty, opAsnTargetIdvExpr, opAsnSourceIdvExpr) { IsInference=true });
                    else
                        inferences.Add(new OWLObjectPropertyAssertion(symObjProp.ObjectPropertyExpression, opAsnTargetIdvExpr, opAsnSourceIdvExpr) { IsInference=true });
                }
			}
            //Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => opAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return inferences;
        }
    }
}