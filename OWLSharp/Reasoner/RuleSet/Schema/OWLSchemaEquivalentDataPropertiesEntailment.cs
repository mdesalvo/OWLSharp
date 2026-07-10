/*
  Copyright 2014-2026 Marco De Salvo
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
    internal static class OWLSchemaEquivalentDataPropertiesEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaEquivalentDataPropertiesEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Cheap short-circuit: skip the O(declaredDataProperties) scan below entirely when the R-Box has no EquivalentDataProperties axiom at all
            if (ontology.DataPropertyAxioms.Any(clsAxm => clsAxm is OWLEquivalentDataProperties))
            {
                foreach (OWLDataProperty declaredDataProperty in ontology.GetDeclarationAxiomsOfType<OWLDataProperty>()
                                                                         .Select(ax => (OWLDataProperty)ax.Entity))
                {
                    //EquivalentDataProperties(P1,P2) ^ EquivalentDataProperties(P2,P3) -> EquivalentDataProperties(P1,P3)
                    List<OWLDataProperty> equivalentDataProperties = ontology.GetEquivalentDataProperties(declaredDataProperty);
                    foreach (OWLDataProperty equivalentDataProperty in equivalentDataProperties)
                    {
                        OWLEquivalentDataProperties inference = new OWLEquivalentDataProperties(new List<OWLDataProperty> { declaredDataProperty, equivalentDataProperty }) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }

            return inferences;
        }
    }
}
