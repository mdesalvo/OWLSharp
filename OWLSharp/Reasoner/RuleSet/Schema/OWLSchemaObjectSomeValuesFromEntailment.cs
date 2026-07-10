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
    internal static class OWLSchemaObjectSomeValuesFromEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaObjectSomeValuesFromEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            //Collect all ObjectSomeValuesFrom expressions referenced anywhere in the T-Box (SubClassOf, EquivalentClasses)
            List<OWLObjectSomeValuesFrom> referencedSVFs = new List<OWLObjectSomeValuesFrom>();
            referencedSVFs.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectSomeValuesFrom).Select(ax => (OWLObjectSomeValuesFrom)ax.SuperClassExpression));
            referencedSVFs.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLObjectSomeValuesFrom).Select(ax => (OWLObjectSomeValuesFrom)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedSVFs.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectSomeValuesFrom>());
            referencedSVFs = OWLExpressionHelper.RemoveDuplicates(referencedSVFs);

            foreach (OWLObjectSomeValuesFrom svf1 in referencedSVFs)
                foreach (OWLObjectSomeValuesFrom svf2 in referencedSVFs)
                {
                    if (svf1 == svf2)
                        continue;

                    bool samePropertyExpression = svf1.ObjectPropertyExpression.GetIRI().Equals(svf2.ObjectPropertyExpression.GetIRI());

                    //scm-svf1: ObjectSomeValuesFrom(OP,Y1) ^ ObjectSomeValuesFrom(OP,Y2) ^ SubClassOf(Y1,Y2) -> SubClassOf(ObjectSomeValuesFrom(OP,Y1),ObjectSomeValuesFrom(OP,Y2))
                    if (samePropertyExpression)
                    {
                        bool fillerIsSubClass = svf1.ClassExpression.GetIRI().Equals(svf2.ClassExpression.GetIRI())
                                                  || ontology.CheckIsSubClassOf(svf1.ClassExpression, svf2.ClassExpression);
                        if (fillerIsSubClass)
                        {
                            OWLSubClassOf inference = new OWLSubClassOf(svf1, svf2) { IsInference=true };
                            inference.GetXML();
                            inferences.Add(new OWLInference(rulename, inference));
                            continue;
                        }
                    }

                    //scm-svf2: ObjectSomeValuesFrom(OP1,Y) ^ ObjectSomeValuesFrom(OP2,Y) ^ SubObjectPropertyOf(OP1,OP2) -> SubClassOf(ObjectSomeValuesFrom(OP1,Y),ObjectSomeValuesFrom(OP2,Y))
                    bool sameFillerClass = svf1.ClassExpression.GetIRI().Equals(svf2.ClassExpression.GetIRI());
                    if (sameFillerClass && ontology.CheckIsSubObjectPropertyOf(svf1.ObjectPropertyExpression, svf2.ObjectPropertyExpression))
                    {
                        OWLSubClassOf inference = new OWLSubClassOf(svf1, svf2) { IsInference=true };
                        inference.GetXML();
                        inferences.Add(new OWLInference(rulename, inference));
                    }
                }

            return inferences;
        }
    }
}
