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
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();
            OWLIndividualExpression swapIdvExpr;
            HashSet<long> visitContext = new HashSet<long>();
            List<OWLIndividualExpression> transitiveRelatedIdvExprs = new List<OWLIndividualExpression>();
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

            //TransitiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV3) -> ObjectPropertyAssertion(OP,IDV1,IDV3)
            foreach (OWLTransitiveObjectProperty trnObjProp in ontology.GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>())
			{
                visitContext.Clear();
                transitiveRelatedIdvExprs.Clear();

                //Extract object assertions of the current transitive property
                List<OWLObjectPropertyAssertion> trnObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, trnObjProp.ObjectPropertyExpression);
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
                    if (trnObjProp.ObjectPropertyExpression is OWLObjectInverseOf trnObjInvOf)
                    {
                        swapIdvExpr = trnObjPropAsns[i].SourceIndividualExpression;
                        trnObjPropAsns[i].SourceIndividualExpression = trnObjPropAsns[i].TargetIndividualExpression;
                        trnObjPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                        trnObjPropAsns[i].ObjectPropertyExpression = trnObjInvOf.ObjectProperty;
                    }
                }

                //Iterate object assertions to find inference opportunities (transitive closure)
                IEnumerable<IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion>> trnObjPropAsnGroups = trnObjPropAsns.GroupBy(asn => asn.SourceIndividualExpression);
                foreach (IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion> trnObjPropAsnGroup in trnObjPropAsnGroups)
				{
                    RDFResource trnObjPropAsnGroupKeyIRI = trnObjPropAsnGroup.Key.GetIRI();

                    #region VisitContext
                    if (!visitContext.Contains(trnObjPropAsnGroupKeyIRI.PatternMemberID))
                        visitContext.Add(trnObjPropAsnGroupKeyIRI.PatternMemberID);
                    #endregion

                    transitiveRelatedIdvExprs.AddRange(FindTransitiveRelatedIndividuals(trnObjPropAsns, trnObjPropAsnGroupKeyIRI, trnObjPropAsnGroups, visitContext));
                    foreach (OWLIndividualExpression transitiveRelatedIdvExpr in transitiveRelatedIdvExprs)
                        inferences.Add(new OWLObjectPropertyAssertion(trnObjProp.ObjectPropertyExpression, trnObjPropAsnGroup.Key, transitiveRelatedIdvExpr));

                    transitiveRelatedIdvExprs.Clear();
                }
			}

            return inferences;
        }

        internal static List<OWLIndividualExpression> FindTransitiveRelatedIndividuals(List<OWLObjectPropertyAssertion> trnObjPropAsns, RDFResource trnObjPropAsnGroupKeyIRI,
            IEnumerable<IGrouping<OWLIndividualExpression, OWLObjectPropertyAssertion>> trnObjPropAsnGroups, HashSet<long> visitContext)
        {
            List<OWLIndividualExpression> transitiveRelatedIdvExprs = new List<OWLIndividualExpression>();

            #region VisitContext
            RDFResource trnObjPropAsnGroupKeyIRI = trnObjPropAsnGroup.Key.GetIRI();
            if (!visitContext.Contains(trnObjPropAsnGroupKeyIRI.PatternMemberID))
                visitContext.Add(trnObjPropAsnGroupKeyIRI.PatternMemberID);
            #endregion

            //DIRECT
            transitiveRelatedIdvExprs.AddRange(trnObjPropAsnGroups.Single(grp => grp.Key.GetIRI().Equals(trnObjPropAsnGroupKeyIRI))
                                                                  .Select(asn => asn.TargetIndividualExpression));

            //INDIRECT
            foreach (OWLIndividualExpression transitiveRelatedIdvExpr in transitiveRelatedIdvExprs.ToList())
                transitiveRelatedIdvExprs.AddRange(FindTransitiveRelatedIndividuals(trnObjPropAsns,  , visitContext));

            return transitiveRelatedIdvExprs;
        }
    }
}