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
    internal static class OWLDisjointClassesEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.DisjointClassesEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //EquivalentClasses(C1,C2) ^ DisjointClasses(C2,C3) -> DisjointClasses(C1,C3)
            //SubClassOf(C1,C2) ^ DisjointClasses(C2,C3) -> DisjointClasses(C1,C3)
            foreach (OWLClass declaredClass in ontology.GetDeclarationAxiomsOfType<OWLClass>()
                                                       .Select(ax => (OWLClass)ax.Expression))
            {
                List<OWLClassExpression> disjointClasses = ontology.GetDisjointClasses(declaredClass);
                foreach (OWLClassExpression disjointClass in disjointClasses)
                {
                    OWLDisjointClasses inference = new OWLDisjointClasses(new List<OWLClassExpression>() { declaredClass, disjointClass }) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }    
            }
            
            //DisjointUnion(C1,(C2 C3)) -> DisjointClasses(C2,C3)
            //DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)    
            foreach (OWLDisjointUnion disjointUnion in ontology.GetClassAxiomsOfType<OWLDisjointUnion>())
            {
                OWLDisjointClasses inferenceA = new OWLDisjointClasses(disjointUnion.ClassExpressions) { IsInference=true };
                inferenceA.GetXML();
                inferences.Add(new OWLInference(rulename, inferenceA));

                foreach (OWLClassExpression disjointUnionClassExpression in disjointUnion.ClassExpressions)
                {
                    OWLSubClassOf inferenceB = new OWLSubClassOf(disjointUnionClassExpression, disjointUnion.ClassIRI) { IsInference = true };
                    inferenceB.GetXML();
                    inferences.Add(new OWLInference(rulename, inferenceB));
                }    
            }

            return inferences;
        }
    }
}