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
    internal static class OWLHasKeyEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            //Temporary working variables
			List<OWLSameIndividual> sameIdvs = ontology.GetAssertionAxiomsOfType<OWLSameIndividual>();
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

			//HasKey(C, OP) ^ ObjectPropertyAssertion(OP, I1, IX) ^ ObjectPropertyAssertion(OP, I2, IX) -> SameIndividual(I1,I2)
			//HasKey(C, DP) ^ DataPropertyAssertion(DP, I1, LIT) ^ DataPropertyAssertion(DP, I2, LIT) -> SameIndividual(I1,I2)
            foreach (OWLHasKey hasKeyAxiom in ontology.KeyAxioms)
			{
				//TODO
				
			}

			//Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => sameIdvs.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }
    }
}