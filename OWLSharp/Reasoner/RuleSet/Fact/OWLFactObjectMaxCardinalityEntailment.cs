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

            //A qualifying class is usually shared by several restrictions: its members are materialized once per class
            Dictionary<string, HashSet<string>> qualifyingMembersCache = new Dictionary<string, HashSet<string>>();

            #region cls-maxc2, cls-maxqc3, cls-maxqc4: SubClassOf(D,ObjectMaxCardinality(1,P[,C])) ^ ClassAssertion(D,I) ^ ObjectPropertyAssertion(P,I,I1) ^ ObjectPropertyAssertion(P,I,I2) [^ ClassAssertion(C,I1) ^ ClassAssertion(C,I2)] -> SameIndividual(I1,I2)
            foreach (OWLSubClassOf scofAxm in scofAxms)
            {
                //An exact cardinality of 1 bounds the fillers from above just like a max cardinality of 1 does
                OWLObjectPropertyExpression maxCardProperty;
                OWLClassExpression maxCardQualifier;
                switch (scofAxm.SuperClassExpression)
                {
                    //ObjectMaxCardinality(1,P[,C]) caps the fillers of P (belonging to C, when qualified) at one
                    case OWLObjectMaxCardinality maxCard when maxCard.Cardinality == "1":
                        maxCardProperty = maxCard.ObjectPropertyExpression;
                        maxCardQualifier = maxCard.ClassExpression;
                        break;
                    //ObjectExactCardinality(1,P[,C]) caps them at one too: its lower bound plays no role in merging fillers
                    case OWLObjectExactCardinality exactCard when exactCard.Cardinality == "1":
                        maxCardProperty = exactCard.ObjectPropertyExpression;
                        maxCardQualifier = exactCard.ClassExpression;
                        break;
                    //Any other superclass expresses no upper bound of one filler, so it entails no SameIndividual
                    default:
                        continue;
                }

                //ObjectInverseOf(P) caps the individuals pointing to the constrained one, not those it points to
                bool isInverse = maxCardProperty is OWLObjectInverseOf;

                //Only the assertions of P can count as fillers of the restriction
                List<OWLObjectPropertyAssertion> propAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, maxCardProperty);
                if (propAsns.Count == 0)
                    continue;

                //An owl:Thing qualifier excludes no filler (cls-maxqc4), so it is handled as the unqualified restriction (cls-maxc2)
                bool isQualified = maxCardQualifier != null
                                    && !(maxCardQualifier is OWLClass qualifierClass && qualifierClass.GetIRI().Equals(RDFVocabulary.OWL.THING));

                //Under a qualified restriction only the members of C are capped (cls-maxqc3): fillers outside C are free to differ
                HashSet<string> qualifyingMembers = null;
                if (isQualified)
                {
                    //A named qualifier is shared across restrictions by its IRI; an anonymous one is unique to its restriction
                    string qualifierKey = maxCardQualifier.GetIRI().ToString();
                    if (!qualifyingMembersCache.TryGetValue(qualifierKey, out qualifyingMembers))
                    {
                        qualifyingMembers = new HashSet<string>(ontology.GetIndividualsOf(maxCardQualifier, clsAsns, false).Select(idv => idv.GetIRI().ToString()));
                        qualifyingMembersCache.Add(qualifierKey, qualifyingMembers);
                    }
                }

                //Find all individuals of the constraining class D
                List<OWLIndividualExpression> domainIndividuals = ontology.GetIndividualsOf(scofAxm.SubClassExpression, clsAsns, false);

                foreach (OWLIndividualExpression domainIdv in domainIndividuals)
                {
                    RDFResource domainIdvIRI = domainIdv.GetIRI();

                    //Collect the distinct fillers of P for this individual: a filler asserted twice is still one filler,
                    //which must never be declared the same as itself
                    Dictionary<string, OWLIndividualExpression> fillers = new Dictionary<string, OWLIndividualExpression>();
                    foreach (OWLObjectPropertyAssertion propAsn in propAsns)
                    {
                        //Under ObjectInverseOf(P) the constrained individual is the target of P and its filler the source
                        OWLIndividualExpression subjectIdv = isInverse ? propAsn.TargetIndividualExpression : propAsn.SourceIndividualExpression;
                        OWLIndividualExpression fillerIdv  = isInverse ? propAsn.SourceIndividualExpression : propAsn.TargetIndividualExpression;
                        if (!subjectIdv.GetIRI().Equals(domainIdvIRI))
                            continue;

                        //A filler outside the qualifying class is not capped by the restriction, so it is not merged
                        string fillerIRI = fillerIdv.GetIRI().ToString();
                        if (isQualified && !qualifyingMembers.Contains(fillerIRI))
                            continue;

                        //The first assertion of a filler represents it: later duplicates add nothing
                        if (!fillers.ContainsKey(fillerIRI))
                            fillers.Add(fillerIRI, fillerIdv);
                    }

                    //More than one capped filler under a maximum of one => all of them denote the same individual
                    if (fillers.Count > 1)
                    {
                        OWLSameIndividual inference = new OWLSameIndividual(fillers.Values.ToList()) { IsInference=true };
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
