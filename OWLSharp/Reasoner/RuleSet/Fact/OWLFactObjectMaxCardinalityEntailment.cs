/*
  Copyright 2014-2026 Marco De Salvo
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
    internal static class OWLFactObjectMaxCardinalityEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FactObjectMaxCardinalityEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();

            #region cls-maxc2: SubClassOf(D,ObjectMaxCardinality(1,P)) ^ ClassAssertion(D,I) ^ ObjectPropertyAssertion(P,I,I1) ^ ObjectPropertyAssertion(P,I,I2) -> SameIndividual(I1,I2)
            foreach (OWLSubClassOf maxCard1SubClassOf in scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectMaxCardinality omc && omc.Cardinality == "1"))
            {
                OWLObjectMaxCardinality maxCard = (OWLObjectMaxCardinality)maxCard1SubClassOf.SuperClassExpression;
                bool isInverse = maxCard.ObjectPropertyExpression is OWLObjectInverseOf;

                List<OWLObjectPropertyAssertion> propAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, maxCard.ObjectPropertyExpression);
                if (propAsns.Count == 0)
                    continue;

                //Find all individuals of the constraining class D
                List<OWLIndividualExpression> domainIndividuals = ontology.GetIndividualsOf(maxCard1SubClassOf.SubClassExpression, clsAsns, false);

                foreach (OWLIndividualExpression domainIdv in domainIndividuals)
                {
                    RDFResource domainIdvIRI = domainIdv.GetIRI();

                    //Collect all fillers of P for this individual
                    List<OWLIndividualExpression> fillers = new List<OWLIndividualExpression>();
                    foreach (OWLObjectPropertyAssertion propAsn in propAsns)
                    {
                        OWLIndividualExpression subjectIdv = isInverse ? propAsn.TargetIndividualExpression : propAsn.SourceIndividualExpression;
                        OWLIndividualExpression fillerIdv  = isInverse ? propAsn.SourceIndividualExpression : propAsn.TargetIndividualExpression;
                        if (subjectIdv.GetIRI().Equals(domainIdvIRI))
                            fillers.Add(fillerIdv);
                    }

                    //MaxCardinality(1) is violated => all fillers must be SameIndividual
                    if (fillers.Count > 1)
                    {
                        OWLSameIndividual inference = new OWLSameIndividual(fillers) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }
            #endregion

            return inferences;
        }
    }
}
