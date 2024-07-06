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
    internal static class OWLHasValueEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			List<OWLSubClassOf> subClassOfAxioms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
			List<OWLClassAssertion> classAssertions = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();

			//SubClassOf(C,ObjectHasValue(OP,I2)) ^ ClassAssertion(C,I1) -> ObjectPropertyAssertion(OP,I1,I2)
			foreach (OWLSubClassOf subClassOfObjectHasValue in subClassOfAxioms.Where(ax => ax.SuperClassExpression is OWLObjectHasValue objHasValue))
			{
				OWLObjectHasValue objHasValue = (OWLObjectHasValue)subClassOfObjectHasValue.SuperClassExpression;
				foreach (OWLClassAssertion classAssertion in classAssertions.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassOfObjectHasValue.SubClassExpression.GetIRI())))
					inferences.Add(objHasValue.ObjectPropertyExpression is OWLObjectInverseOf objInvOfHasValue 
						? new OWLObjectPropertyAssertion(objInvOfHasValue.ObjectProperty, objHasValue.IndividualExpression, classAssertion.IndividualExpression)
						: new OWLObjectPropertyAssertion(objHasValue.ObjectPropertyExpression, classAssertion.IndividualExpression, objHasValue.IndividualExpression));	
			}

            return inferences;
        }
    }
}