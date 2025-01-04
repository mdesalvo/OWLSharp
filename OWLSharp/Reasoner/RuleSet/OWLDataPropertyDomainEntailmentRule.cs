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

namespace OWLSharp.Reasoner
{
    internal static class OWLDataPropertyDomainEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.DataPropertyDomainEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

			//Temporary working variables
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

            //DataPropertyDomain(DP,C) ^ DataPropertyAssertion(DP, I, LIT) -> ClassAssertion(C,I)
            foreach (OWLDataPropertyDomain dataPropertyDomain in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>())
                foreach (OWLDataPropertyAssertion dataPropertyAssertion in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, dataPropertyDomain.DataProperty))
                {
                    OWLClassAssertion inference = new OWLClassAssertion(dataPropertyDomain.ClassExpression) { IndividualExpression=dataPropertyAssertion.IndividualExpression, IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

			return inferences;
        }
    }
}