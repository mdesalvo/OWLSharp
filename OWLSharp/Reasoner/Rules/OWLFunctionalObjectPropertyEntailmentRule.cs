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
    internal static class OWLFunctionalObjectPropertyEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            Dictionary<string, List<OWLIndividualExpression>> idvxLookup = new Dictionary<string, List<OWLIndividualExpression>>();

            //FunctionalObjectProperty(FOP) ^ ObjectPropertyAssertion(FOP,IDVX,IDV1) ^ ObjectPropertyAssertion(FOP,IDVX,IDV2) -> SameIndividual(IDV1,IDV2)
            foreach (OWLFunctionalObjectProperty fop in ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>())
            {
                idvxLookup.Clear();
                foreach (OWLObjectPropertyAssertion fopAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, fop.ObjectPropertyExpression))
                {
                    OWLIndividualExpression fopAsnSourceIdvExpr = fopAsn.SourceIndividualExpression;
                    OWLIndividualExpression fopAsnTargetIdvExpr = fopAsn.TargetIndividualExpression;
                    if (fop.ObjectPropertyExpression is OWLObjectInverseOf || fopAsn.ObjectPropertyExpression is OWLObjectInverseOf)
                    {
                        fopAsnSourceIdvExpr = fopAsn.TargetIndividualExpression;
                        fopAsnTargetIdvExpr = fopAsn.SourceIndividualExpression;
                    }

                    string idvx = fopAsnSourceIdvExpr.GetIRI().ToString();
                    if (!idvxLookup.ContainsKey(idvx))
                        idvxLookup.Add(idvx, new List<OWLIndividualExpression>());
                    idvxLookup[idvx].Add(fopAsnTargetIdvExpr);
                }
                foreach (List<OWLIndividualExpression> idvxLookupEntry in idvxLookup.Values.Where(idvExprs => idvExprs.Count > 1))
                    inferences.Add(new OWLSameIndividual(idvxLookupEntry) { IsInference=true });
            }

            return inferences;
        }
    }
}