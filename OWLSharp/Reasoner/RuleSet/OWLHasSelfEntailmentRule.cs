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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLHasSelfEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.HasSelfEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology, OWLReasonerContext reasonerContext)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //SubClassOf(C,ObjectHasSelf(OP)) ^ ClassAssertion(C,I) -> ObjectPropertyAssertion(OP,I,I)
            foreach (OWLSubClassOf subClassOfObjectHasSelf in ontology.GetClassAxiomsOfType<OWLSubClassOf>()
                                                                      .Where(ax => ax.SuperClassExpression is OWLObjectHasSelf))
            {
                OWLObjectHasSelf objHasSelf = (OWLObjectHasSelf)subClassOfObjectHasSelf.SuperClassExpression;
                RDFResource subClassExpressionIRI = subClassOfObjectHasSelf.SubClassExpression.GetIRI();
                foreach (OWLClassAssertion classAssertion in reasonerContext.ClassAssertions.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassExpressionIRI)))
                {
                    if (objHasSelf.ObjectPropertyExpression is OWLObjectInverseOf objInvOfHasSelf)
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objInvOfHasSelf.ObjectProperty, classAssertion.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                    else
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objHasSelf.ObjectPropertyExpression, classAssertion.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }

            return inferences;
        }
    }
}