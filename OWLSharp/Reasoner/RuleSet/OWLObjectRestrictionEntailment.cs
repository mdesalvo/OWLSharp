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
    internal static class OWLObjectRestrictionEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.ObjectRestrictionEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            #region cls-svf1: ObjectSomeValuesFrom(P,C) ^ ObjectPropertyAssertion(P,I1,I2) ^ ClassAssertion(C,I2) -> ClassAssertion(ObjectSomeValuesFrom(P,C),I1)
            List<OWLObjectSomeValuesFrom> referencedSVFs = new List<OWLObjectSomeValuesFrom>();
            referencedSVFs.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectSomeValuesFrom).Select(ax => (OWLObjectSomeValuesFrom)ax.SuperClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedSVFs.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectSomeValuesFrom>());

            foreach (OWLObjectSomeValuesFrom svf in OWLExpressionHelper.RemoveDuplicates(referencedSVFs))
            {
                //Find all individuals of the filler class C
                List<OWLIndividualExpression> fillerIndividuals = ontology.GetIndividualsOf(svf.ClassExpression, clsAsns, false);
                if (fillerIndividuals.Count == 0)
                    continue;

                bool isInverse = svf.ObjectPropertyExpression is OWLObjectInverseOf;
                HashSet<long> fillerIRISet = new HashSet<long>(fillerIndividuals.Select(idv => idv.GetIRI().PatternMemberID));

                //After CalibrateObjectAssertions: inverse(P)(A,B) swapped to P(B,A)
                //For ObjectSomeValuesFrom(P,C):          filler=target, subject=source
                //For ObjectSomeValuesFrom(inverse(P),C): filler=source, subject=target (post-swap)
                foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, svf.ObjectPropertyExpression))
                {
                    OWLIndividualExpression subjectIdv = isInverse ? opAsn.TargetIndividualExpression : opAsn.SourceIndividualExpression;
                    OWLIndividualExpression fillerIdv  = isInverse ? opAsn.SourceIndividualExpression : opAsn.TargetIndividualExpression;

                    if (fillerIRISet.Contains(fillerIdv.GetIRI().PatternMemberID))
                    {
                        OWLClassAssertion inference = new OWLClassAssertion(svf) { IndividualExpression=subjectIdv, IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }
            #endregion

            #region cls-avf: ClassAssertion(ObjectAllValuesFrom(P,C),I1) ^ ObjectPropertyAssertion(P,I1,I2) -> ClassAssertion(C,I2)
            //Anonymous class expressions use random blank-node IRIs per instance, so IRI-based lookup across instances is unreliable.
            //We iterate directly over class assertions whose ClassExpression IS an ObjectAllValuesFrom, then apply the property assertion scan.
            //In iterative reasoning mode, avf class assertions derived by ClassAssertionEntailment (from SubClassOf) will feed this rule in subsequent passes.
            foreach (OWLClassAssertion avfClsAsn in clsAsns.Where(asn => asn.ClassExpression is OWLObjectAllValuesFrom))
            {
                OWLObjectAllValuesFrom avf = (OWLObjectAllValuesFrom)avfClsAsn.ClassExpression;
                bool isInverse = avf.ObjectPropertyExpression is OWLObjectInverseOf;
                RDFResource avfIdvIRI = avfClsAsn.IndividualExpression.GetIRI();

                foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, avf.ObjectPropertyExpression))
                {
                    OWLIndividualExpression subjectIdv = isInverse ? opAsn.TargetIndividualExpression : opAsn.SourceIndividualExpression;
                    OWLIndividualExpression fillerIdv  = isInverse ? opAsn.SourceIndividualExpression : opAsn.TargetIndividualExpression;

                    if (subjectIdv.GetIRI().Equals(avfIdvIRI))
                    {
                        OWLClassAssertion inference = new OWLClassAssertion(avf.ClassExpression) { IndividualExpression=fillerIdv, IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }
            #endregion

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

            #region ObjectHasSelf reverse: ObjectPropertyAssertion(P,I,I) ^ [ObjectHasSelf(P) referenced] -> ClassAssertion(ObjectHasSelf(P),I)
            List<OWLObjectHasSelf> referencedHasSelf = new List<OWLObjectHasSelf>();
            referencedHasSelf.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectHasSelf).Select(ax => (OWLObjectHasSelf)ax.SuperClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedHasSelf.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectHasSelf>());

            foreach (OWLObjectHasSelf hasSelf in OWLExpressionHelper.RemoveDuplicates(referencedHasSelf))
            {
                bool isInverse = hasSelf.ObjectPropertyExpression is OWLObjectInverseOf;
                foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, hasSelf.ObjectPropertyExpression))
                {
                    //inverse(P)(x,x) after calibration becomes P(x,x): reflexive check is identical for both directions
                    OWLIndividualExpression subjectIdv = isInverse ? opAsn.TargetIndividualExpression : opAsn.SourceIndividualExpression;
                    OWLIndividualExpression fillerIdv  = isInverse ? opAsn.SourceIndividualExpression : opAsn.TargetIndividualExpression;

                    if (subjectIdv.GetIRI().Equals(fillerIdv.GetIRI()))
                    {
                        OWLClassAssertion inference = new OWLClassAssertion(hasSelf) { IndividualExpression=subjectIdv, IsInference=true };
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
