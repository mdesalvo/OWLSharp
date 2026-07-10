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
    internal static class OWLSchemaSubObjectPropertyOfEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaSubObjectPropertyOfEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Cheap short-circuit: skip the O(declaredObjectProperties) scan below entirely when the R-Box has no SubObjectPropertyOf axiom at all
            List<OWLSubObjectPropertyOf> sopofAxms = ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>();
            if (sopofAxms.Count > 0)
            {
                foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                             .Select(ax => (OWLObjectProperty)ax.Entity))
                {
                    //SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                    //SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)
                    List<OWLObjectPropertyExpression> superObjectPropertyExprs = ontology.GetSuperObjectPropertiesOf(declaredObjectProperty);
                    foreach (OWLObjectProperty superObjectProperty in superObjectPropertyExprs.OfType<OWLObjectProperty>())
                    {
                        OWLSubObjectPropertyOf inference = new OWLSubObjectPropertyOf(declaredObjectProperty, superObjectProperty) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                    //SubObjectPropertyOf(OP1,ObjectInverseOf(OP2)) needs its own OfType<> branch: OWLObjectInverseOf is not an
                    //OWLObjectProperty, so it would silently be skipped by the plain-property loop above
                    foreach (OWLObjectInverseOf superObjectInverseOf in superObjectPropertyExprs.OfType<OWLObjectInverseOf>())
                    {
                        OWLSubObjectPropertyOf inference = new OWLSubObjectPropertyOf(declaredObjectProperty, superObjectInverseOf) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }

            return inferences;
        }
    }
}
