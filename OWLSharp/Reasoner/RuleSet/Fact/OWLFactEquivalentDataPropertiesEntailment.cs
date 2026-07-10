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
    internal static class OWLFactEquivalentDataPropertiesEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FactEquivalentDataPropertiesEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            if (ontology.DataPropertyAxioms.Any(clsAxm => clsAxm is OWLEquivalentDataProperties))
            {
                //Temporary working variables
                List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

                foreach (OWLDataProperty declaredDataProperty in ontology.GetDeclarationAxiomsOfType<OWLDataProperty>()
                                                                         .Select(ax => (OWLDataProperty)ax.Entity))
                {
                    List<OWLDataProperty> equivalentDataProperties = ontology.GetEquivalentDataProperties(declaredDataProperty);
                    if (equivalentDataProperties.Count == 0)
                        continue;

                    //EquivalentDataProperties(P1,P2) ^ DataPropertyAssertion(P1,I,LIT) -> DataPropertyAssertion(P2,I,LIT)
                    foreach (OWLDataPropertyAssertion declaredDataPropertyAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, declaredDataProperty))
                        foreach (OWLDataProperty equivalentDataProperty in equivalentDataProperties)
                        {
                            OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(equivalentDataProperty, declaredDataPropertyAsn.Literal) { IndividualExpression=declaredDataPropertyAsn.IndividualExpression, IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                }
            }

            return inferences;
        }
    }
}
