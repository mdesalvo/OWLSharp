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
    internal static class OWLClassAssertionEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.ClassAssertionEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology, OWLReasonerContext reasonerContext)
        {
            List<OWLInference> inferences = [];

            //Temporary working variables
            List<OWLEquivalentClasses> equivalentClassesAxioms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();
            List<OWLDisjointClasses> disjointClassesAxioms = ontology.GetClassAxiomsOfType<OWLDisjointClasses>();
            List<OWLDisjointUnion> disjointUnionAxioms = ontology.GetClassAxiomsOfType<OWLDisjointUnion>();
            List<OWLClassExpression> inScopeClsExprs =
            [
                .. ontology.GetDeclarationAxiomsOfType<OWLClass>()
                                                                                                .Select(ax => (OWLClass)ax.Expression),
                .. reasonerContext.ClassAssertions.Select(ax => ax.ClassExpression),
                .. equivalentClassesAxioms.SelectMany(ax => ax.ClassExpressions.Select(cls => cls)),
                .. disjointClassesAxioms.SelectMany(ax => ax.ClassExpressions.Select(cls => cls)),
                .. disjointUnionAxioms.Select(ax => ax.ClassIRI),
                .. disjointUnionAxioms.SelectMany(ax => ax.ClassExpressions.Select(cls => cls)),
            ];
            foreach (OWLClassExpression inScopeClsExpr in OWLExpressionHelper.RemoveDuplicates(inScopeClsExprs))
            {
                inScopeClsExprs.AddRange(ontology.GetSuperClassesOf(inScopeClsExpr));
                inScopeClsExprs.AddRange(ontology.GetEquivalentClasses(inScopeClsExpr));
            }

            //ClassAssertion(C1,I) ^ SubClassOf(C1,C2) -> ClassAssertion(C2,I)
            //ClassAssertion(C1,I) ^ EquivalentClasses(C1,C2) -> ClassAssertion(C2,I)
            foreach (OWLClassExpression inScopeClsExpr in OWLExpressionHelper.RemoveDuplicates(inScopeClsExprs))
                foreach (OWLIndividualExpression idvExprOfInScopeClsExpr in ontology.GetIndividualsOf(inScopeClsExpr, reasonerContext.ClassAssertions, false))
                {
                    OWLClassAssertion inference = new OWLClassAssertion(inScopeClsExpr) { IndividualExpression=idvExprOfInScopeClsExpr, IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

            return inferences;
        }
    }
}