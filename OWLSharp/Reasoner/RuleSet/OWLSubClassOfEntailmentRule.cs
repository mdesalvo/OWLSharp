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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLSubClassOfEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SubClassOfEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            foreach (OWLClass declaredClass in ontology.GetDeclarationAxiomsOfType<OWLClass>()
                                                       .Select(ax => (OWLClass)ax.Expression))
            {
                //SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
                //SubClassOf(C1,C2) ^ EquivalentClasses(C2,C3) -> SubClassOf(C1,C3)
                //EquivalentClasses(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
                //DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
                foreach (OWLClassExpression superClass in ontology.GetSuperClassesOf(declaredClass))
                {
                    OWLSubClassOf inference = new OWLSubClassOf(declaredClass, superClass) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));

                    //SubClassOf(C1,ObjectIntersectionOf(C2,C3)) -> SubClassOf(C1,C2) ^ SubClassOf(C1,C3)
                    if (superClass is OWLObjectIntersectionOf objIntOf)
                        foreach (OWLClassExpression objIntOfCls in objIntOf.ClassExpressions)
                        {
                            OWLSubClassOf inferenceObjIntOf = new OWLSubClassOf(declaredClass, objIntOfCls) { IsInference=true };
                            inferenceObjIntOf.GetXML();
                            inferences.Add(new OWLInference(rulename, inferenceObjIntOf));
                        }
                }
            }

            return inferences;
        }
    }
}