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
    internal static class OWLInverseFunctionalObjectPropertyEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.InverseFunctionalObjectPropertyEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            Dictionary<string, List<OWLIndividualExpression>> idvxLookup = new Dictionary<string, List<OWLIndividualExpression>>();

            //InverseFunctionalObjectProperty(IFOP) ^ ObjectPropertyAssertion(IFOP,IDV1,IDVX) ^ ObjectPropertyAssertion(FOP,IDV2,IDVX) -> SameIndividual(IDV1,IDV2)
            foreach (OWLInverseFunctionalObjectProperty ifop in ontology.GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>())
            {
                idvxLookup.Clear();
                foreach (OWLObjectPropertyAssertion ifopAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, ifop.ObjectPropertyExpression))
                {
                    OWLIndividualExpression fopAsnSourceIdvExpr = ifopAsn.SourceIndividualExpression;
                    OWLIndividualExpression fopAsnTargetIdvExpr = ifopAsn.TargetIndividualExpression;

                    //In case the inverse functional object property works under inverse logic, we must swap source/target of the object assertion
                    if (ifop.ObjectPropertyExpression is OWLObjectInverseOf)
                    {
                        fopAsnSourceIdvExpr = ifopAsn.TargetIndividualExpression;
                        fopAsnTargetIdvExpr = ifopAsn.SourceIndividualExpression;
                    }

                    //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (ifopAsn.ObjectPropertyExpression is OWLObjectInverseOf)
                        (fopAsnSourceIdvExpr, fopAsnTargetIdvExpr) = (fopAsnTargetIdvExpr, fopAsnSourceIdvExpr);

                    string idvx = fopAsnTargetIdvExpr.GetIRI().ToString();
                    if (!idvxLookup.ContainsKey(idvx))
                        idvxLookup.Add(idvx, new List<OWLIndividualExpression>());
                    idvxLookup[idvx].Add(fopAsnSourceIdvExpr);
                }
                foreach (List<OWLIndividualExpression> idvxLookupEntry in idvxLookup.Values.Where(idvExprs => idvExprs.Count > 1))
                {
                    OWLSameIndividual inference = new OWLSameIndividual(idvxLookupEntry) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }

            return inferences;
        }
    }
}