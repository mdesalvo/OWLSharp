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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLFunctionalDataPropertyEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            Dictionary<string, List<OWLIndividualExpression>> litLookup = new Dictionary<string, List<OWLIndividualExpression>>();

            //FunctionalDataProperty(FDP) ^ DataPropertyAssertion(FDP,IDV1,LIT) ^ DataPropertyAssertion(FDP,IDV2,LIT) -> SameIndividual(IDV1,IDV2)
            foreach (OWLFunctionalDataProperty fdp in ontology.GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>())
            {
                litLookup.Clear();
                foreach (OWLDataPropertyAssertion fdpAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, fdp.DataProperty))
                {
                    string lit = fdpAsn.Literal.GetLiteral().ToString();
                    if (!litLookup.ContainsKey(lit))
                        litLookup.Add(lit, new List<OWLIndividualExpression>());
                    litLookup[lit].Add(fdpAsn.IndividualExpression);
                }
                foreach (List<OWLIndividualExpression> litLookupEntry in litLookup.Values.Where(idvExprs => idvExprs.Count > 1))
                    inferences.Add(new OWLSameIndividual(litLookupEntry) { IsInference=true });
            }

            return inferences;
        }
    }
}