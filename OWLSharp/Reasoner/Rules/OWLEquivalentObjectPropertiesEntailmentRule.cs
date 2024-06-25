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
    internal static class OWLEquivalentObjectPropertiesEntailmentRule
    {
        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> report = new List<OWLInference>();

            //EquivalentObjectProperties(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> EquivalentObjectProperties(P1,P3)
            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
            															 .Select(ax => (OWLObjectProperty)ax.Expression))
			{
				List<OWLObjectPropertyExpression> inferredEquivalentObjectPropertyExpressions = ontology.GetEquivalentObjectProperties(declaredObjectProperty);
                foreach (OWLObjectProperty inferredEquivalentObjectProperty in inferredEquivalentObjectPropertyExpressions.OfType<OWLObjectProperty>())
                {
                    OWLInference inference = new OWLInference(
                        nameof(OWLEquivalentObjectPropertiesEntailmentRule), 
						new OWLEquivalentObjectProperties(new List<OWLObjectPropertyExpression>() { declaredObjectProperty, inferredEquivalentObjectProperty }) { IsInference=true });

                    report.Add(inference);
                }
				foreach (OWLObjectInverseOf inferredEquivalentObjectInverseOf in inferredEquivalentObjectPropertyExpressions.OfType<OWLObjectInverseOf>())
                {
                    OWLInference inference = new OWLInference(
                        nameof(OWLEquivalentObjectPropertiesEntailmentRule), 
						new OWLEquivalentObjectProperties(new List<OWLObjectPropertyExpression>() { declaredObjectProperty, inferredEquivalentObjectInverseOf }) { IsInference=true });

                    report.Add(inference);
                }
			}

            return report;
        }
    }
}