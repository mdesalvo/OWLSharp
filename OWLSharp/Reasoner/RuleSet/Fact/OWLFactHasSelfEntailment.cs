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
    internal static class OWLFactHasSelfEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FactHasSelfEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            #region Forward: SubClassOf(C,ObjectHasSelf(OP)) ^ ClassAssertion(C,I) -> ObjectPropertyAssertion(OP,I,I)
            foreach (OWLSubClassOf subClassOfObjectHasSelf in scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectHasSelf))
            {
                OWLObjectHasSelf objHasSelf = (OWLObjectHasSelf)subClassOfObjectHasSelf.SuperClassExpression;
                RDFResource subClassExpressionIRI = subClassOfObjectHasSelf.SubClassExpression.GetIRI();
                foreach (OWLClassAssertion classAssertion in clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassExpressionIRI)))
                {
                    if (objHasSelf.ObjectPropertyExpression is OWLObjectInverseOf objInvOfHasSelf)
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objInvOfHasSelf.ObjectProperty, classAssertion.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                    else
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objHasSelf.ObjectPropertyExpression, classAssertion.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }
            #endregion

            #region Reverse: ObjectPropertyAssertion(P,I,I) ^ [ObjectHasSelf(P) referenced] -> ClassAssertion(ObjectHasSelf(P),I)
            //Temporary working variables (scoped to this region: only needed for the reverse direction)
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

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
