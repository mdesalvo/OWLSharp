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
    internal static class OWLDisjointDataPropertiesEntailmentRule
    {
        private static readonly string rulename = OWLEnums.OWLReasonerRules.DisjointDataPropertiesEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            foreach (OWLDataProperty declaredDataProperty in ontology.GetDeclarationAxiomsOfType<OWLDataProperty>()
                                                                     .Select(ax => (OWLDataProperty)ax.Expression))
			{
				List<OWLDataProperty> disjointDataProperties = ontology.GetDisjointDataProperties(declaredDataProperty);
                foreach (OWLDataProperty disjointDataProperty in disjointDataProperties)
                {
                    OWLDisjointDataProperties inference = new OWLDisjointDataProperties(new List<OWLDataProperty>() { declaredDataProperty, disjointDataProperty }) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }   
			}
            
            return inferences;
        }
    }
}