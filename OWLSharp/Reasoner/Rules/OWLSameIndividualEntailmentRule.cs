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
    internal static class OWLSameIndividualEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            List<OWLSameIndividual> sameIdvs = ontology.GetAssertionAxiomsOfType<OWLSameIndividual>();
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            foreach (OWLNamedIndividual declaredIdv in ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
			                                                   .Select(ax => (OWLNamedIndividual)ax.Expression))
			{
                RDFResource declaredIdvIRI = declaredIdv.GetIRI();
                List<OWLObjectPropertyAssertion> declaredIdvSrcOpAsns = opAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(declaredIdvIRI)).ToList();
                List<OWLObjectPropertyAssertion> declaredIdvTgtOpAsns = opAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(declaredIdvIRI)).ToList();
                List<OWLDataPropertyAssertion> declaredIdvDpAsns = dpAsns.Where(asn => asn.IndividualExpression.GetIRI().Equals(declaredIdvIRI)).ToList();

                foreach (OWLIndividualExpression sameIdv in ontology.GetSameIndividuals(declaredIdv))
                {
                    RDFResource sameIdvIRI = sameIdv.GetIRI();
                    List<OWLObjectPropertyAssertion> sameIdvSrcOpAsns = opAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(sameIdvIRI)).ToList();
                    List<OWLObjectPropertyAssertion> sameIdvTgtOpAsns = opAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(sameIdvIRI)).ToList();
                    List<OWLDataPropertyAssertion> sameIdvDpAsns = dpAsns.Where(asn => asn.IndividualExpression.GetIRI().Equals(sameIdvIRI)).ToList();

                    /* TRANSITIVITY */
                    //SameIndividual(I1,I2) ^ SameIndividual(I2,I3) -> SameIndividual(I1,I3)
                    inferences.Add(new OWLSameIndividual(new List<OWLIndividualExpression>() { declaredIdv, sameIdv }) { IsInference=true });
                    inferences.Add(new OWLSameIndividual(new List<OWLIndividualExpression>() { sameIdv, declaredIdv }) { IsInference = true });

                    /* ENTAILMENTS */
                    //SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I1,I3) -> ObjectPropertyAssertion(OP,I2,I3)
                    declaredIdvSrcOpAsns.ForEach(idvOpAsn => inferences.Add(new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, sameIdv, idvOpAsn.TargetIndividualExpression) { IsInference=true }));
                    sameIdvSrcOpAsns.ForEach(idvOpAsn => inferences.Add(new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, declaredIdv, idvOpAsn.TargetIndividualExpression) { IsInference=true }));
                    //SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I3,I1) -> ObjectPropertyAssertion(OP,I3,I2)
                    declaredIdvTgtOpAsns.ForEach(idvOpAsn => inferences.Add(new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, idvOpAsn.SourceIndividualExpression, sameIdv) { IsInference=true }));
                    sameIdvTgtOpAsns.ForEach(idvOpAsn => inferences.Add(new OWLObjectPropertyAssertion(idvOpAsn.ObjectPropertyExpression, idvOpAsn.SourceIndividualExpression, declaredIdv) { IsInference=true }));
                    //SameIndividual(I1,I2) ^ DataPropertyAssertion(DP,I1,LIT) -> DataPropertyAssertion(DP,I2,LIT)
                    if (sameIdv is OWLNamedIndividual sameNamedIndividual)
                        declaredIdvDpAsns.ForEach(idvDpAsn => inferences.Add(new OWLDataPropertyAssertion(idvDpAsn.DataProperty, sameNamedIndividual, idvDpAsn.Literal) { IsInference=true }));
                    else if (sameIdv is OWLAnonymousIndividual sameAnonIndividual)
                        declaredIdvDpAsns.ForEach(idvDpAsn => inferences.Add(new OWLDataPropertyAssertion(idvDpAsn.DataProperty, sameAnonIndividual, idvDpAsn.Literal) { IsInference=true }));
                    sameIdvDpAsns.ForEach(idvDpAsn => inferences.Add(new OWLDataPropertyAssertion(idvDpAsn.DataProperty, declaredIdv, idvDpAsn.Literal) { IsInference = true }));
                }
            }
            //Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => sameIdvs.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));
            inferences.RemoveAll(inf => opAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));
            inferences.RemoveAll(inf => dpAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }
    }
}