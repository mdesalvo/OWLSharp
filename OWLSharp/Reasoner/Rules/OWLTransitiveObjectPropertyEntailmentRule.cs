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
    internal static class OWLTransitiveObjectPropertyEntailmentRule
    {
        internal static List<OWLIndividualExpression> EmptyIdvExprList = Enumerable.Empty<OWLIndividualExpression>().ToList();

        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            //Temporary working variables
            OWLIndividualExpression swapIdvExpr;
            HashSet<long> visitContext = new HashSet<long>();
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

            //TransitiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV3) -> ObjectPropertyAssertion(OP,IDV1,IDV3)
            foreach (OWLTransitiveObjectProperty trnObjProp in ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>())
			{
                OWLObjectProperty trnObjPropInvOfValue = (trnObjProp.ObjectPropertyExpression as OWLObjectInverseOf)?.ObjectProperty;

                #region ObjectPropertyAssertion Calibration
                //Extract (calibrated and deduplicated) object assertions of the current transitive property
                List <OWLObjectPropertyAssertion> trnObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, trnObjProp.ObjectPropertyExpression);
                for (int i=0; i<trnObjPropAsns.Count; i++)
                {
                    //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (trnObjPropAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {   
                        swapIdvExpr = trnObjPropAsns[i].SourceIndividualExpression;
                        trnObjPropAsns[i].SourceIndividualExpression = trnObjPropAsns[i].TargetIndividualExpression;
                        trnObjPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                        trnObjPropAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }

                    //In case the transitive object property works under inverse logic, we must swap source/target of the object assertion
                    if (trnObjPropInvOfValue != null)
                    {
                        swapIdvExpr = trnObjPropAsns[i].SourceIndividualExpression;
                        trnObjPropAsns[i].SourceIndividualExpression = trnObjPropAsns[i].TargetIndividualExpression;
                        trnObjPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                        trnObjPropAsns[i].ObjectPropertyExpression = trnObjPropInvOfValue;
                    }
                }
                trnObjPropAsns = OWLAxiomHelper.RemoveDuplicates(trnObjPropAsns);
                #endregion

                #region Transitive Closure Analysis
                //Iterate object assertions to find inference opportunities (transitive closure)
                IEnumerable<IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion>> trnObjPropAsnGroups = trnObjPropAsns.GroupBy(asn => asn.SourceIndividualExpression);
                foreach (IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion> trnObjPropAsnGroup in trnObjPropAsnGroups)
				{
                    foreach (OWLIndividualExpression transitiveRelatedIdvExpr in FindTransitiveRelatedIndividuals(trnObjPropAsnGroup.Key.GetIRI(), trnObjPropAsnGroups, visitContext))
                        inferences.Add(new OWLObjectPropertyAssertion(trnObjPropInvOfValue ?? trnObjProp.ObjectPropertyExpression, trnObjPropAsnGroup.Key, transitiveRelatedIdvExpr) { IsInference=true });
                    visitContext.Clear();
                }
                //Remove inferences already stated in explicit knowledge
                inferences.RemoveAll(inf => trnObjPropAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));
                #endregion
            }

            return inferences;
        }

        internal static List<OWLIndividualExpression> FindTransitiveRelatedIndividuals(RDFResource trnObjPropAsnGroupKeyIRI,
            IEnumerable<IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion>> trnObjPropAsnGroups, HashSet<long> visitContext)
        {
            List<OWLIndividualExpression> transitiveRelatedIdvExprs = new List<OWLIndividualExpression>();

            #region VisitContext
            if (!visitContext.Contains(trnObjPropAsnGroupKeyIRI.PatternMemberID))
                visitContext.Add(trnObjPropAsnGroupKeyIRI.PatternMemberID);
            else
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