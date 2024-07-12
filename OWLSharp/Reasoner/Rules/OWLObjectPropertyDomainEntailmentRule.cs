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
    internal static class OWLObjectPropertyDomainEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			//Temporary working variables
			List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

			//ObjectPropertyDomain(OP,C) ^ ObjectPropertyAssertion(OP, I1, I2) -> ClassAssertion(C,I1)
            foreach (OWLObjectPropertyDomain objectPropertyDomain in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>())
				foreach (OWLObjectPropertyAssertion objectPropertyAssertion in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, objectPropertyDomain.ObjectPropertyExpression))
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
					if (objectPropertyDomain.ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
						inferences.Add(new OWLClassAssertion(objectPropertyDomain.ClassExpression) { IndividualExpression=opAsnTargetIdvExpr, IsInference=true});
					else
						inferences.Add(new OWLClassAssertion(objectPropertyDomain.ClassExpression) { IndividualExpression=opAsnSourceIdvExpr, IsInference=true});
				}

			//Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => clsAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }
    }
}