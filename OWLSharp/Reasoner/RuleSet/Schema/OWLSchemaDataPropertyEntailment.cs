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
    internal static class OWLSchemaDataPropertyEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaDataPropertyEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            foreach (OWLDataProperty declaredDataProperty in ontology.GetDeclarationAxiomsOfType<OWLDataProperty>()
                                                                      .Select(ax => (OWLDataProperty)ax.Entity))
            {
                //scm-dp: DataProperty(DP) -> SubDataPropertyOf(DP,DP)
                OWLSubDataPropertyOf selfSubDataPropertyOf = new OWLSubDataPropertyOf(declaredDataProperty, declaredDataProperty) { IsInference=true };
                selfSubDataPropertyOf.GetXML();
                inferences.Add(new OWLInference(rulename, selfSubDataPropertyOf));

                //scm-dp: DataProperty(DP) -> EquivalentDataProperties(DP,DP)
                OWLEquivalentDataProperties selfEquivalentDataProperties = new OWLEquivalentDataProperties(new List<OWLDataProperty> { declaredDataProperty, declaredDataProperty }) { IsInference=true };
                selfEquivalentDataProperties.GetXML();
                inferences.Add(new OWLInference(rulename, selfEquivalentDataProperties));
            }

            return inferences;
        }
    }
}
