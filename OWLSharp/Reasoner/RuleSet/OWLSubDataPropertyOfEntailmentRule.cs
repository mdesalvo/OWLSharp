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
    internal static class OWLSubDataPropertyOfEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SubDataPropertyOfEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology, OWLReasonerContext reasonerContext)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            foreach (OWLDataProperty declaredDataProperty in ontology.GetDeclarationAxiomsOfType<OWLDataProperty>()
                                                                     .Select(ax => (OWLDataProperty)ax.Entity))
            {
                //SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                //SubDataPropertyOf(P1,P2) ^ EquivalentDataProperties(P2,P3) -> SubDataPropertyOf(P1,P3)
                List<OWLDataProperty> superDataProperties = ontology.GetSuperDataPropertiesOf(declaredDataProperty);
                foreach (OWLDataProperty superDataProperty in superDataProperties)
                {
                    OWLSubDataPropertyOf inference = new OWLSubDataPropertyOf(declaredDataProperty, superDataProperty) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

                //SubDataPropertyOf(P1,P2) ^ DataPropertyAssertion(P1,I,LIT) -> DataPropertyAssertion(P2,I,LIT)
                foreach (OWLDataPropertyAssertion declaredDataPropertyAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(reasonerContext.DataPropertyAssertions, declaredDataProperty))
                    foreach (OWLDataProperty superDataProperty in superDataProperties)
                    {
                        OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(superDataProperty, declaredDataPropertyAsn.Literal) { IndividualExpression = declaredDataPropertyAsn.IndividualExpression, IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
            }

            return inferences;
        }
    }
}