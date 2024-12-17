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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLHasSelfEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.HasSelfEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLSubClassOf> subClassOfAxioms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
			List<OWLClassAssertion> classAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();

			//SubClassOf(C,ObjectHasSelf(OP)) ^ ClassAssertion(C,I) -> ObjectPropertyAssertion(OP,I,I)
			foreach (OWLSubClassOf subClassOfObjectHasSelf in subClassOfAxioms.Where(ax => ax.SuperClassExpression is OWLObjectHasSelf objHasSelf))
			{
				OWLObjectHasSelf objHasSelf = (OWLObjectHasSelf)subClassOfObjectHasSelf.SuperClassExpression;
				RDFResource subClassExpressionIRI = subClassOfObjectHasSelf.SubClassExpression.GetIRI();
				foreach (OWLClassAssertion classAssertion in classAssertions.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassExpressionIRI)))
				{
					if (objHasSelf.ObjectPropertyExpression is OWLObjectInverseOf objInvOfHasSelf)
					{
						OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objInvOfHasSelf.ObjectProperty, classAssertion.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
						inference.GetXML();
						inferences.Add(new OWLInference(rulename, inference));
                    }
					else
					{
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objHasSelf.ObjectPropertyExpression, classAssertion.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
				}
			}

            return inferences;
        }
    }
}