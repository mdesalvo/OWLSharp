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
    internal static class OWLDifferentIndividualsEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.DifferentIndividualsEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            foreach (OWLNamedIndividual declaredNamedIndividual in ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
            															   .Select(ax => (OWLNamedIndividual)ax.Expression))
				foreach (OWLIndividualExpression differentIdvExpr in ontology.GetDifferentIndividuals(declaredNamedIndividual))
				{
                    OWLDifferentIndividuals inferenceA = new OWLDifferentIndividuals(new List<OWLIndividualExpression>() { declaredNamedIndividual, differentIdvExpr }) { IsInference=true };
                    inferenceA.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceA));

                    OWLDifferentIndividuals inferenceB = new OWLDifferentIndividuals(new List<OWLIndividualExpression>() { differentIdvExpr, declaredNamedIndividual }) { IsInference=true };
                    inferenceB.GetXML();
					inferences.Add(new OWLInference(rulename, inferenceB));
				}
            
            return inferences;
        }
    }
}