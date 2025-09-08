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
    internal static class OWLFunctionalObjectPropertyEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FunctionalObjectPropertyEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology, OWLReasonerContext reasonerContext)
        {
            List<OWLInference> inferences = [];

            //Temporary working variables
            Dictionary<string, List<OWLIndividualExpression>> idvxLookup = [];

            //FunctionalObjectProperty(FOP) ^ ObjectPropertyAssertion(FOP,IDVX,IDV1) ^ ObjectPropertyAssertion(FOP,IDVX,IDV2) -> SameIndividual(IDV1,IDV2)
            foreach (OWLFunctionalObjectProperty fop in ontology.GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>())
            {
                idvxLookup.Clear();
                foreach (OWLObjectPropertyAssertion fopAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(reasonerContext.ObjectPropertyAssertions, fop.ObjectPropertyExpression))
                {
                    OWLIndividualExpression fopAsnSourceIdvExpr = fopAsn.SourceIndividualExpression;
                    OWLIndividualExpression fopAsnTargetIdvExpr = fopAsn.TargetIndividualExpression;

                    //In case the functional object property works under inverse logic, we must swap source/target of the object assertion
                    if (fop.ObjectPropertyExpression is OWLObjectInverseOf)
                    {
                        fopAsnSourceIdvExpr = fopAsn.TargetIndividualExpression;
                        fopAsnTargetIdvExpr = fopAsn.SourceIndividualExpression;
                    }

                    //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (fopAsn.ObjectPropertyExpression is OWLObjectInverseOf)
                        (fopAsnSourceIdvExpr, fopAsnTargetIdvExpr) = (fopAsnTargetIdvExpr, fopAsnSourceIdvExpr);

                    string idvx = fopAsnSourceIdvExpr.GetIRI().ToString();
                    if (!idvxLookup.ContainsKey(idvx))
                        idvxLookup.Add(idvx, []);
                    idvxLookup[idvx].Add(fopAsnTargetIdvExpr);
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