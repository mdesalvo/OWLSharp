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
    internal static class OWLSchemaObjectPropertyEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaObjectPropertyEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                          .Select(ax => (OWLObjectProperty)ax.Entity))
            {
                //scm-op: ObjectProperty(OP) -> SubObjectPropertyOf(OP,OP)
                OWLSubObjectPropertyOf selfSubObjectPropertyOf = new OWLSubObjectPropertyOf(declaredObjectProperty, declaredObjectProperty) { IsInference=true };
                selfSubObjectPropertyOf.GetXML();
                inferences.Add(new OWLInference(rulename, selfSubObjectPropertyOf));

                //scm-op: ObjectProperty(OP) -> EquivalentObjectProperties(OP,OP)
                OWLEquivalentObjectProperties selfEquivalentObjectProperties = new OWLEquivalentObjectProperties(new List<OWLObjectPropertyExpression> { declaredObjectProperty, declaredObjectProperty }) { IsInference=true };
                selfEquivalentObjectProperties.GetXML();
                inferences.Add(new OWLInference(rulename, selfEquivalentObjectProperties));
            }

            return inferences;
        }
    }
}
