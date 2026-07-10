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
    internal static class OWLFactObjectAllValuesFromEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FactObjectAllValuesFromEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();

            #region cls-avf: ClassAssertion(ObjectAllValuesFrom(P,C),I1) ^ ObjectPropertyAssertion(P,I1,I2) -> ClassAssertion(C,I2)
            //Anonymous class expressions use random blank-node IRIs per instance, so IRI-based lookup across instances is unreliable.
            //We iterate directly over class assertions whose ClassExpression IS an ObjectAllValuesFrom, then apply the property assertion scan.
            //In iterative reasoning mode, avf class assertions derived by FactClassAssertionEntailment (from SubClassOf) will feed this rule in subsequent passes.
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

            return inferences;
        }
    }
}
