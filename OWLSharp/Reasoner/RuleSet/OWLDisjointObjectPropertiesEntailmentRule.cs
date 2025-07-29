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
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLDisjointObjectPropertiesEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.DisjointObjectPropertiesEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology, OWLReasonerContext reasonerContext)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                         .Select(ax => (OWLObjectProperty)ax.Expression))
            {
                List<OWLObjectPropertyExpression> disjointObjectPropertyExpressions = ontology.GetDisjointObjectProperties(declaredObjectProperty);
                foreach (OWLObjectProperty disjointObjectProperty in disjointObjectPropertyExpressions.OfType<OWLObjectProperty>())
                {
                    OWLDisjointObjectProperties inferenceA = new OWLDisjointObjectProperties(new List<OWLObjectPropertyExpression> { declaredObjectProperty, disjointObjectProperty }) { IsInference=true };
                    inferenceA.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceA));

                    OWLDisjointObjectProperties inferenceB = new OWLDisjointObjectProperties(new List<OWLObjectPropertyExpression> { disjointObjectProperty, declaredObjectProperty }) { IsInference=true };
                    inferenceB.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceB));
                }
                foreach (OWLObjectInverseOf disjointObjectInverseOf in disjointObjectPropertyExpressions.OfType<OWLObjectInverseOf>())
                {
                    OWLDisjointObjectProperties inferenceA = new OWLDisjointObjectProperties(new List<OWLObjectPropertyExpression> { declaredObjectProperty, disjointObjectInverseOf }) { IsInference=true };
                    inferenceA.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceA));

                    OWLDisjointObjectProperties inferenceB = new OWLDisjointObjectProperties(new List<OWLObjectPropertyExpression> { disjointObjectInverseOf, declaredObjectProperty }) { IsInference=true };
                    inferenceB.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceB));
                }
            }

            return inferences;
        }
    }
}