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
    internal static class OWLDisjointObjectPropertiesEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            //EquivalentObjectProperties(P1,P2) ^ DisjointObjectProperties(P2,P3) -> DisjointObjectProperties(P1,P3)
            //SubObjectPropertyOf(P1,P2) ^ DisjointObjectProperties(P2,P3) -> DisjointObjectProperties(P1,P3)
            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
            															 .Select(ax => (OWLObjectProperty)ax.Expression))
			{
				List<OWLObjectPropertyExpression> inferredDisjointObjectPropertyExpressions = ontology.GetDisjointObjectProperties(declaredObjectProperty);
                foreach (OWLObjectProperty inferredDisjointObjectProperty in inferredDisjointObjectPropertyExpressions.OfType<OWLObjectProperty>())
                    inferences.Add(new OWLDisjointObjectProperties(new List<OWLObjectPropertyExpression>() { declaredObjectProperty, inferredDisjointObjectProperty }) { IsInference = true });
                foreach (OWLObjectInverseOf inferredDisjointObjectInverseOf in inferredDisjointObjectPropertyExpressions.OfType<OWLObjectInverseOf>())
                    inferences.Add(new OWLDisjointObjectProperties(new List<OWLObjectPropertyExpression>() { declaredObjectProperty, inferredDisjointObjectInverseOf }) { IsInference = true });
			}

            return inferences;
        }
    }
}