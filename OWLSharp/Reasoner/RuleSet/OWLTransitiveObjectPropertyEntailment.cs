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
    internal static class OWLTransitiveObjectPropertyEntailment
    {
        private static readonly List<OWLIndividualExpression> EmptyIdvExprList = Enumerable.Empty<OWLIndividualExpression>().ToList();
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.TransitiveObjectPropertyEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            List<OWLTransitiveObjectProperty> transitiveObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>();
            if (transitiveObjProps.Count > 0)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
                HashSet<long> visitContext = new HashSet<long>();

                //TransitiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV3) -> ObjectPropertyAssertion(OP,IDV1,IDV3)
                foreach (OWLTransitiveObjectProperty trnObjProp in transitiveObjProps)
                {
                    OWLObjectProperty trnObjPropInvOfValue = (trnObjProp.ObjectPropertyExpression as OWLObjectInverseOf)?.ObjectProperty;

                    #region Recalibration
                    List <OWLObjectPropertyAssertion> trnObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, trnObjProp.ObjectPropertyExpression);
                    foreach (OWLObjectPropertyAssertion trnObjPropAsn in trnObjPropAsns)
                    {
                        //In case the transitive object property works under inverse logic, we must swap source/target of the object assertion
                        if (trnObjPropInvOfValue != null)
                        {
                            (trnObjPropAsn.SourceIndividualExpression, trnObjPropAsn.TargetIndividualExpression) = (trnObjPropAsn.TargetIndividualExpression, trnObjPropAsn.SourceIndividualExpression);
                            trnObjPropAsn.ObjectPropertyExpression = trnObjPropInvOfValue;
                        }
                    }
                    trnObjPropAsns = OWLAxiomHelper.RemoveDuplicates(trnObjPropAsns);
                    #endregion

                    #region Analysis
                    //Iterate object assertions to find inference opportunities (transitive closure)
                    List<IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion>> trnObjPropAsnGroups = trnObjPropAsns.GroupBy(asn => asn.SourceIndividualExpression).ToList();
                    foreach (IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion> trnObjPropAsnGroup in trnObjPropAsnGroups)
                    {
                        RDFResource trnObjPropAsnGroupKeyIRI = trnObjPropAsnGroup.Key.GetIRI();
                        foreach (OWLIndividualExpression transitiveRelatedIdvExpr in FindTransitiveRelatedIndividuals(trnObjPropAsnGroupKeyIRI, trnObjPropAsnGroups, visitContext))
                        {
                            OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(trnObjPropInvOfValue ?? trnObjProp.ObjectPropertyExpression, trnObjPropAsnGroup.Key, transitiveRelatedIdvExpr) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                        visitContext.Clear();
                    }
                    #endregion
                }
            }

            return inferences;
        }

        private static List<OWLIndividualExpression> FindTransitiveRelatedIndividuals(RDFResource trnObjPropAsnGroupKeyIRI,
            List<IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion>> trnObjPropAsnGroups, HashSet<long> visitContext)
        {
            List<OWLIndividualExpression> transitiveRelatedIdvExprs = new List<OWLIndividualExpression>();

            #region VisitContext
            if (!visitContext.Add(trnObjPropAsnGroupKeyIRI.PatternMemberID))
                return transitiveRelatedIdvExprs;
            #endregion

            //DIRECT
            transitiveRelatedIdvExprs.AddRange(trnObjPropAsnGroups.SingleOrDefault(grp => grp.Key.GetIRI().Equals(trnObjPropAsnGroupKeyIRI))
                                                                 ?.Select(asn => asn.TargetIndividualExpression) ?? EmptyIdvExprList);

            //INDIRECT (TRANSITIVE CLOSURE)
            foreach (OWLIndividualExpression transitiveRelatedIdvExpr in transitiveRelatedIdvExprs.ToList())
                transitiveRelatedIdvExprs.AddRange(FindTransitiveRelatedIndividuals(transitiveRelatedIdvExpr.GetIRI(), trnObjPropAsnGroups, visitContext));

            return transitiveRelatedIdvExprs;
        }
    }
}