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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLDataPropertyDomainEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			//Temporary working variables
			List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

			//DataPropertyDomain(DP,C) ^ DataPropertyAssertion(DP, I, LIT) -> ClassAssertion(C,I)
            foreach (OWLDataPropertyDomain dataPropertyDomain in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>())
				foreach (OWLDataPropertyAssertion dataPropertyAssertion in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, dataPropertyDomain.DataProperty))
					inferences.Add(new OWLClassAssertion(dataPropertyDomain.ClassExpression) { IndividualExpression=dataPropertyAssertion.IndividualExpression, IsInference=true});

			//Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => clsAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }
    }
}