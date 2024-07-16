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
    internal static class OWLEquivalentClassesEntailmentRule
    {
        private static readonly string rulename = OWLEnums.OWLReasonerRules.EquivalentClassesEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //EquivalentClasses(C1,C2) ^ EquivalentClasses(C2,C3) -> EquivalentClasses(C1,C3)
            foreach (OWLClass declaredClass in ontology.GetDeclarationAxiomsOfType<OWLClass>()
													   .Select(ax => (OWLClass)ax.Expression))
			{
				List<OWLClassExpression> equivalentClasses = ontology.GetEquivalentClasses(declaredClass);
                foreach (OWLClassExpression equivalentClass in equivalentClasses)
                {
                    OWLEquivalentClasses inference = new OWLEquivalentClasses(new List<OWLClassExpression>() { declaredClass, equivalentClass }) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }   
			}

            return inferences;
        }
    }
}