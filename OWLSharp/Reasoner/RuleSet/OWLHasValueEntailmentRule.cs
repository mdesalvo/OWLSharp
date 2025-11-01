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
    internal static class OWLHasValueEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.HasValueEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();

            //SubClassOf(C,ObjectHasValue(OP,I2)) ^ ClassAssertion(C,I1) -> ObjectPropertyAssertion(OP,I1,I2)
            foreach (OWLSubClassOf subClassOfObjectHasValue in scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectHasValue))
            {
                OWLObjectHasValue objHasValue = (OWLObjectHasValue)subClassOfObjectHasValue.SuperClassExpression;
                RDFResource subClassExpressionIRI = subClassOfObjectHasValue.SubClassExpression.GetIRI();
                foreach (OWLClassAssertion classAssertion in clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassExpressionIRI)))
                {
                    if (objHasValue.ObjectPropertyExpression is OWLObjectInverseOf objInvOfHasValue)
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objInvOfHasValue.ObjectProperty, objHasValue.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                    else
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objHasValue.ObjectPropertyExpression, classAssertion.IndividualExpression, objHasValue.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }

            //SubClassOf(C,DataHasValue(DP,LIT)) ^ ClassAssertion(C,I) -> DataPropertyAssertion(DP,I,LIT)
            foreach (OWLSubClassOf subClassOfDataHasValue in scofAxms.Where(ax => ax.SuperClassExpression is OWLDataHasValue))
            {
                OWLDataHasValue dtHasValue = (OWLDataHasValue)subClassOfDataHasValue.SuperClassExpression;
                RDFResource subClassExpressionIRI = subClassOfDataHasValue.SubClassExpression.GetIRI();
                foreach (OWLClassAssertion classAssertion in clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassExpressionIRI)))
                {
                    OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(dtHasValue.DataProperty, dtHasValue.Literal) { IndividualExpression = classAssertion.IndividualExpression, IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }

            return inferences;
        }
    }
}