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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLSchemaObjectAllValuesFromEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaObjectAllValuesFromEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            //Collect all ObjectAllValuesFrom expressions referenced anywhere in the T-Box (SubClassOf, EquivalentClasses)
            List<OWLObjectAllValuesFrom> referencedAVFs = new List<OWLObjectAllValuesFrom>();
            referencedAVFs.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectAllValuesFrom).Select(ax => (OWLObjectAllValuesFrom)ax.SuperClassExpression));
            referencedAVFs.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLObjectAllValuesFrom).Select(ax => (OWLObjectAllValuesFrom)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedAVFs.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectAllValuesFrom>());
            referencedAVFs = OWLExpressionHelper.RemoveDuplicates(referencedAVFs);

            foreach (OWLObjectAllValuesFrom avf1 in referencedAVFs)
                foreach (OWLObjectAllValuesFrom avf2 in referencedAVFs)
                {
                    if (avf1 == avf2)
                        continue;

                    bool samePropertyExpression = avf1.ObjectPropertyExpression.GetIRI().Equals(avf2.ObjectPropertyExpression.GetIRI());

                    //scm-avf1: ObjectAllValuesFrom(OP,Y1) ^ ObjectAllValuesFrom(OP,Y2) ^ SubClassOf(Y1,Y2) -> SubClassOf(ObjectAllValuesFrom(OP,Y1),ObjectAllValuesFrom(OP,Y2))
                    //Same polarity as scm-svf1: a stricter filler class yields a stricter (subsumed) restriction, for a fixed property
                    if (samePropertyExpression)
                    {
                        bool fillerIsSubClass = avf1.ClassExpression.GetIRI().Equals(avf2.ClassExpression.GetIRI())
                                                  || ontology.CheckIsSubClassOf(avf1.ClassExpression, avf2.ClassExpression);
                        if (fillerIsSubClass)
                        {
                            OWLSubClassOf inference = new OWLSubClassOf(avf1, avf2) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                            continue;
                        }
                    }

                    //scm-avf2: ObjectAllValuesFrom(OP1,Y) ^ ObjectAllValuesFrom(OP2,Y) ^ SubObjectPropertyOf(OP1,OP2) -> SubClassOf(ObjectAllValuesFrom(OP2,Y),ObjectAllValuesFrom(OP1,Y))
                    //NOTE the inverted polarity with respect to scm-svf2: universal quantification is antitone in the property argument, so
                    //when OP1 is a sub-property of OP2, the restriction built on the WIDER property (OP2) is the one subsumed by the restriction
                    //built on the NARROWER property (OP1) -- i.e. AllValuesFrom(OP2,Y) is SubClassOf AllValuesFrom(OP1,Y), not the other way around
                    bool sameFillerClass = avf1.ClassExpression.GetIRI().Equals(avf2.ClassExpression.GetIRI());
                    if (sameFillerClass && ontology.CheckIsSubObjectPropertyOf(avf1.ObjectPropertyExpression, avf2.ObjectPropertyExpression))
                    {
                        //avf1 is on the narrower (sub) property OP1, avf2 is on the wider (super) property OP2 => SubClassOf(avf2,avf1)
                        OWLSubClassOf inference = new OWLSubClassOf(avf2, avf1) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }

            return inferences;
        }
    }
}
