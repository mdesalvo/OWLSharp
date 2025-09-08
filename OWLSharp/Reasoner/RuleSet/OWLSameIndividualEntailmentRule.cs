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
    internal static class OWLSameIndividualEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SameIndividualEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology, OWLReasonerContext reasonerContext)
        {
            List<OWLInference> inferences = [];
            if (ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Count == 0)
                return inferences;

            foreach (OWLNamedIndividual declaredIdv in ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
                                                               .Select(ax => (OWLNamedIndividual)ax.Expression))
            {
                List<OWLIndividualExpression> sameIdvs = ontology.GetSameIndividuals(declaredIdv);
                if (sameIdvs.Count == 0)
                    continue;

                //Temporary working variables (declared individual)
                RDFResource declaredIdvIRI = declaredIdv.GetIRI();
                List<OWLObjectPropertyAssertion> declaredIdvSrcOpAsns = [.. reasonerContext.ObjectPropertyAssertions.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(declaredIdvIRI))];
                List<OWLObjectPropertyAssertion> declaredIdvTgtOpAsns = [.. reasonerContext.ObjectPropertyAssertions.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(declaredIdvIRI))];
                List<OWLDataPropertyAssertion> declaredIdvDpAsns = [.. reasonerContext.DataPropertyAssertions.Where(asn => asn.IndividualExpression.GetIRI().Equals(declaredIdvIRI))];

                foreach (OWLIndividualExpression sameIdv in sameIdvs)
                {
                    //Temporary working variables (same individual)
                    RDFResource sameIdvIRI = sameIdv.GetIRI();
                    List<OWLObjectPropertyAssertion> sameIdvSrcOpAsns = [.. reasonerContext.ObjectPropertyAssertions.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(sameIdvIRI))];
                    List<OWLObjectPropertyAssertion> sameIdvTgtOpAsns = [.. reasonerContext.ObjectPropertyAssertions.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(sameIdvIRI))];
                    List<OWLDataPropertyAssertion> sameIdvDpAsns = [.. reasonerContext.DataPropertyAssertions.Where(asn => asn.IndividualExpression.GetIRI().Equals(sameIdvIRI))];

                    /* SAMEAS TRANSITIVITY */
                    //SameIndividual(I1,I2) ^ SameIndividual(I2,I3) -> SameIndividual(I1,I3)
                    OWLSameIndividual inferenceA = new OWLSameIndividual([declaredIdv, sameIdv]) { IsInference=true };
                    inferenceA.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceA));
                    OWLSameIndividual inferenceB = new OWLSameIndividual([sameIdv, declaredIdv]) { IsInference=true };
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
                    switch (sameIdv)
                    {
                        //SameIndividual(I1,I2) ^ DataPropertyAssertion(DP,I1,LIT) -> DataPropertyAssertion(DP,I2,LIT)
                        //SameIndividual(I1,I2) ^ DataPropertyAssertion(DP,I2,LIT) -> DataPropertyAssertion(DP,I1,LIT)
                        case OWLNamedIndividual sameNamedIndividual:
                            declaredIdvDpAsns.ForEach(idvDpAsn =>
                            {
                                OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(idvDpAsn.DataProperty, sameNamedIndividual, idvDpAsn.Literal) { IsInference=true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            });
                            break;
                        case OWLAnonymousIndividual sameAnonIndividual:
                            declaredIdvDpAsns.ForEach(idvDpAsn =>
                            {
                                OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(idvDpAsn.DataProperty, sameAnonIndividual, idvDpAsn.Literal) { IsInference=true };
                                inference.GetXML();
                                inferences.Add(new OWLInference(rulename, inference));
                            });
                            break;
                    }
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