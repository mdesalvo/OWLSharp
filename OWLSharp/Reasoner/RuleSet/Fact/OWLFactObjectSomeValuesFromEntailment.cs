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
    internal static class OWLFactObjectSomeValuesFromEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FactObjectSomeValuesFromEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            #region cls-svf1: ObjectSomeValuesFrom(P,C) ^ ObjectPropertyAssertion(P,I1,I2) ^ ClassAssertion(C,I2) -> ClassAssertion(ObjectSomeValuesFrom(P,C),I1)
            //An ObjectSomeValuesFrom restriction can be materialized only if it is actually referenced somewhere in the T-Box:
            //either as the super-class of a SubClassOf axiom, or as one of the members of an EquivalentClasses axiom
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

            #region cls-svf2: ObjectSomeValuesFrom(P,owl:Thing) ^ ObjectPropertyAssertion(P,I1,I2) -> ClassAssertion(ObjectSomeValuesFrom(P,owl:Thing),I1)
            //Unlike cls-svf1, the filler here is the unqualified owl:Thing: ANY target individual (of ANY type, even untyped) satisfies it,
            //so we skip the GetIndividualsOf(filler) lookup entirely and just require the existence of a property assertion
            foreach (OWLObjectSomeValuesFrom unqualifiedSvf in OWLExpressionHelper.RemoveDuplicates(referencedSVFs)
                                                                                    .Where(svf => svf.ClassExpression is OWLClass svfCls && svfCls.GetIRI().Equals(RDFVocabulary.OWL.THING)))
            {
                bool isInverse = unqualifiedSvf.ObjectPropertyExpression is OWLObjectInverseOf;
                foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, unqualifiedSvf.ObjectPropertyExpression))
                {
                    OWLIndividualExpression subjectIdv = isInverse ? opAsn.TargetIndividualExpression : opAsn.SourceIndividualExpression;

                    OWLClassAssertion inference = new OWLClassAssertion(unqualifiedSvf) { IndividualExpression=subjectIdv, IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }
            #endregion

            return inferences;
        }
    }
}
