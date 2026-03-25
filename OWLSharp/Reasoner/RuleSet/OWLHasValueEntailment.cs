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
    internal static class OWLHasValueEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.HasValueEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            //SubClassOf(C,ObjectHasValue(OP,I2)) ^ ClassAssertion(C,I1) -> ObjectPropertyAssertion(OP,I1,I2)
            foreach (OWLSubClassOf subClassOfObjectHasValue in scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectHasValue))
            {
                OWLObjectHasValue objHasValue = (OWLObjectHasValue)subClassOfObjectHasValue.SuperClassExpression;
                RDFResource subClassExpressionIRI = subClassOfObjectHasValue.SubClassExpression.GetIRI();
                foreach (OWLClassAssertion classAssertion in clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassExpressionIRI)))
                {
                    if (objHasValue.ObjectPropertyExpression is OWLObjectInverseOf objInvOfHasValue)
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objInvOfHasValue.ObjectProperty, objHasValue.IndividualExpression, classAssertion.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                    else
                    {
                        OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(objHasValue.ObjectPropertyExpression, classAssertion.IndividualExpression, objHasValue.IndividualExpression) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }

            //SubClassOf(C,DataHasValue(DP,LIT)) ^ ClassAssertion(C,I) -> DataPropertyAssertion(DP,I,LIT)
            foreach (OWLSubClassOf subClassOfDataHasValue in scofAxms.Where(ax => ax.SuperClassExpression is OWLDataHasValue))
            {
                OWLDataHasValue dtHasValue = (OWLDataHasValue)subClassOfDataHasValue.SuperClassExpression;
                RDFResource subClassExpressionIRI = subClassOfDataHasValue.SubClassExpression.GetIRI();
                foreach (OWLClassAssertion classAssertion in clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(subClassExpressionIRI)))
                {
                    OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(dtHasValue.DataProperty, dtHasValue.Literal) { IndividualExpression = classAssertion.IndividualExpression, IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }

            //Collect all ObjectHasValue expressions referenced anywhere in the ontology (SubClassOf, EquivalentClasses)
            List<OWLObjectHasValue> referencedObjHasValues = new List<OWLObjectHasValue>();
            referencedObjHasValues.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectHasValue).Select(ax => (OWLObjectHasValue)ax.SuperClassExpression));
            referencedObjHasValues.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLObjectHasValue).Select(ax => (OWLObjectHasValue)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedObjHasValues.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectHasValue>());

            //ObjectPropertyAssertion(P,I1,V) ^ ObjectHasValue(P,V) [referenced] -> ClassAssertion(ObjectHasValue(P,V),I1)
            //ObjectPropertyAssertion(P,V,I1) ^ ObjectHasValue(inverse(P),V) [referenced] -> ClassAssertion(ObjectHasValue(inverse(P),V),I1)
            foreach (OWLObjectHasValue objHasValue in OWLExpressionHelper.RemoveDuplicates(referencedObjHasValues))
            {
                bool isInverse = objHasValue.ObjectPropertyExpression is OWLObjectInverseOf;
                RDFResource hasValueIRI = objHasValue.IndividualExpression.GetIRI();
                foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, objHasValue.ObjectPropertyExpression))
                {
                    //After CalibrateObjectAssertions: all assertions use direct properties; inverse(P)(A,B) was swapped to P(B,A)
                    //For ObjectHasValue(P,v):          need P(subject,v)  -> target == v's IRI, infer ClassAssertion for source
                    //For ObjectHasValue(inverse(P),v): need P(v,subject)  -> source == v's IRI (post-swap), infer ClassAssertion for target
                    if (isInverse)
                    {
                        if (opAsn.SourceIndividualExpression.GetIRI().Equals(hasValueIRI))
                        {
                            OWLClassAssertion inference = new OWLClassAssertion(objHasValue) { IndividualExpression=opAsn.TargetIndividualExpression, IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                    }
                    else
                    {
                        if (opAsn.TargetIndividualExpression.GetIRI().Equals(hasValueIRI))
                        {
                            OWLClassAssertion inference = new OWLClassAssertion(objHasValue) { IndividualExpression=opAsn.SourceIndividualExpression, IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                        }
                    }
                }
            }

            //Collect all DataHasValue expressions referenced anywhere in the ontology (SubClassOf, EquivalentClasses)
            List<OWLDataHasValue> referencedDataHasValues = new List<OWLDataHasValue>();
            referencedDataHasValues.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLDataHasValue).Select(ax => (OWLDataHasValue)ax.SuperClassExpression));
            referencedDataHasValues.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLDataHasValue).Select(ax => (OWLDataHasValue)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedDataHasValues.AddRange(eqAxm.ClassExpressions.OfType<OWLDataHasValue>());

            //DataPropertyAssertion(DP,I,LIT) ^ DataHasValue(DP,LIT) [referenced] -> ClassAssertion(DataHasValue(DP,LIT),I)
            foreach (OWLDataHasValue dataHasValue in OWLExpressionHelper.RemoveDuplicates(referencedDataHasValues))
            {
                RDFLiteral hasValueLiteral = dataHasValue.Literal.GetLiteral();
                foreach (OWLDataPropertyAssertion dpAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, dataHasValue.DataProperty))
                {
                    if (dpAsn.Literal.GetLiteral().Equals(hasValueLiteral))
                    {
                        OWLClassAssertion inference = new OWLClassAssertion(dataHasValue) { IndividualExpression=dpAsn.IndividualExpression, IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }
            }

            return inferences;
        }
    }
}