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

namespace OWLSharp.Reasoner.RuleSet
{
    internal static class OWLSameIndividualEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.SameIndividualEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            #region Utilities
            List<OWLObjectPropertyAssertion> CalibrateObjectAssertions(List<OWLObjectPropertyAssertion> objectPropertyAssertions)
            {
                OWLIndividualExpression swapIdvExpr;
                for (int i = 0; i < objectPropertyAssertions.Count; i++)
                    if (objectPropertyAssertions[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {
                        swapIdvExpr = objectPropertyAssertions[i].SourceIndividualExpression;
                        objectPropertyAssertions[i].SourceIndividualExpression = objectPropertyAssertions[i].TargetIndividualExpression;
                        objectPropertyAssertions[i].TargetIndividualExpression = swapIdvExpr;
                        objectPropertyAssertions[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }
                return OWLAxiomHelper.RemoveDuplicates(objectPropertyAssertions);
            }
            #endregion

            List<OWLInference> inferences = new List<OWLInference>();
            if (ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Count == 0)
                return inferences;

            //Temporary working variables (general)
            List<OWLObjectPropertyAssertion> opAsns = CalibrateObjectAssertions(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>());
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

            foreach (OWLNamedIndividual declaredIdv in ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
			                                                   .Select(ax => (OWLNamedIndividual)ax.Expression))
			{
                List<OWLIndividualExpression> sameIdvs = ontology.GetSameIndividuals(declaredIdv);
                if (sameIdvs.Count == 0)
                    continue;

                //Temporary working variables (declared individual)
                RDFResource declaredIdvIRI = declaredIdv.GetIRI();
                List<OWLObjectPropertyAssertion> declaredIdvSrcOpAsns = opAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(declaredIdvIRI)).ToList();
                List<OWLObjectPropertyAssertion> declaredIdvTgtOpAsns = opAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(declaredIdvIRI)).ToList();
                List<OWLDataPropertyAssertion> declaredIdvDpAsns = dpAsns.Where(asn => asn.IndividualExpression.GetIRI().Equals(declaredIdvIRI)).ToList();

                foreach (OWLIndividualExpression sameIdv in sameIdvs)
                {
                    //Temporary working variables (same individual)
                    RDFResource sameIdvIRI = sameIdv.GetIRI();
                    List<OWLObjectPropertyAssertion> sameIdvSrcOpAsns = opAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(sameIdvIRI)).ToList();
                    List<OWLObjectPropertyAssertion> sameIdvTgtOpAsns = opAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(sameIdvIRI)).ToList();
                    List<OWLDataPropertyAssertion> sameIdvDpAsns = dpAsns.Where(asn => asn.IndividualExpression.GetIRI().Equals(sameIdvIRI)).ToList();

                    /* SAMEAS TRANSITIVITY */
                    //SameIndividual(I1,I2) ^ SameIndividual(I2,I3) -> SameIndividual(I1,I3)
                    OWLSameIndividual inferenceA = new OWLSameIndividual(new List<OWLIndividualExpression>() { declaredIdv, sameIdv }) { IsInference=true };
                    inferenceA.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceA));
                    OWLSameIndividual inferenceB = new OWLSameIndividual(new List<OWLIndividualExpression>() { sameIdv, declaredIdv }) { IsInference=true };
                    inferenceB.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceB));

                    /* SAMEAS ENTAILMENTS */
                    //SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I1,I3) -> ObjectPropertyAssertion(OP,I2,I3)
                    //SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I2,I3) -> ObjectPropertyAssertion(OP,I1,I3)
                    declaredIdvSrcOpAsns.ForEach(idvOpAsn => 
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, sameIdv, idvOpAsn.TargetIndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    });
                    sameIdvSrcOpAsns.ForEach(idvOpAsn =>
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, declaredIdv, idvOpAsn.TargetIndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    });
                    //SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I3,I1) -> ObjectPropertyAssertion(OP,I3,I2)
                    //SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I3,I2) -> ObjectPropertyAssertion(OP,I3,I1)
                    declaredIdvTgtOpAsns.ForEach(idvOpAsn =>
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, idvOpAsn.SourceIndividualExpression, sameIdv) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    });
                    sameIdvTgtOpAsns.ForEach(idvOpAsn =>
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, idvOpAsn.SourceIndividualExpression, declaredIdv) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    });
                    //SameIndividual(I1,I2) ^ DataPropertyAssertion(DP,I1,LIT) -> DataPropertyAssertion(DP,I2,LIT)
                    //SameIndividual(I1,I2) ^ DataPropertyAssertion(DP,I2,LIT) -> DataPropertyAssertion(DP,I1,LIT)
                    if (sameIdv is OWLNamedIndividual sameNamedIndividual)
                        declaredIdvDpAsns.ForEach(idvDpAsn =>
                        {
                            OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(idvDpAsn.DataProperty, sameNamedIndividual, idvDpAsn.Literal) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        });
                    else if (sameIdv is OWLAnonymousIndividual sameAnonIndividual)
                        declaredIdvDpAsns.ForEach(idvDpAsn =>
                        {
                            OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(idvDpAsn.DataProperty, sameAnonIndividual, idvDpAsn.Literal) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        });
                    sameIdvDpAsns.ForEach(idvDpAsn =>
                    {
                        OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(idvDpAsn.DataProperty, declaredIdv, idvDpAsn.Literal) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    });
                }
            }

            return inferences;
        }
    }
}
