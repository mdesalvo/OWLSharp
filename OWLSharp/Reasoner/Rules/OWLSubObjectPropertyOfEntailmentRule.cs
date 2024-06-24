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
    internal static class OWLSubObjectPropertyOfEntailmentRule
    {
        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> report = new List<OWLInference>();

            //SubObjectPropertyOf(OP1,OP2) ^ SubObjectPropertyOf(OP2,OP3) -> SubObjectPropertyOf(OP1,OP3)
            //SubObjectPropertyOf(OP1,OP2) ^ EquivalentObjectProperties(OP2,OP3) -> SubObjectPropertyOf(OP1,OP3)
            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                     	 .Select(ax => (OWLObjectProperty)ax.Expression))
			{
				List<OWLObjectPropertyExpression> inferredSuperObjectPropertyExpressions = ontology.GetSuperObjectPropertiesOf(declaredObjectProperty);
                foreach (OWLObjectProperty inferredSuperObjectProperty in inferredSuperObjectPropertyExpressions.OfType<OWLObjectProperty>())
                {
                    OWLInference inference = new OWLInference(
                        nameof(OWLSubObjectPropertyOfEntailmentRule), 
						new OWLSubObjectPropertyOf(declaredObjectProperty, inferredSuperObjectProperty) { IsInference=true });

                    report.Add(inference);
                }
				foreach (OWLObjectInverseOf inferredSuperObjectInverseOf in inferredSuperObjectPropertyExpressions.OfType<OWLObjectInverseOf>())
                {
                    OWLInference inference = new OWLInference(
                        nameof(OWLSubObjectPropertyOfEntailmentRule), 
						new OWLSubObjectPropertyOf(declaredObjectProperty, inferredSuperObjectInverseOf) { IsInference=true });

                    report.Add(inference);
                }
			}

            return report;
        }
    }
}