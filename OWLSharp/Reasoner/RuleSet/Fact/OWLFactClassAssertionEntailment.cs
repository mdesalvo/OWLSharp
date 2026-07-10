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
    internal static class OWLFactClassAssertionEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FactClassAssertionEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLClassAssertion> clsAsns = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
            //Seed the "in scope" set with every class expression that could possibly gain new individuals: declared classes,
            //classes already used in a ClassAssertion, and classes only ever mentioned as operands of EquivalentClasses/DisjointClasses/DisjointUnion
            //(these last three would otherwise never surface, since they don't necessarily have their own declaration or assertion)
            List<OWLClassExpression> inScopeClsExprs =
                ontology.GetDeclarationAxiomsOfType<OWLClass>().Select(ax => (OWLClass)ax.Entity)
                        .Union(clsAsns.Select(ax => ax.ClassExpression))
                        .Union(ontology.GetClassAxiomsOfType<OWLEquivalentClasses>().SelectMany(ax => ax.ClassExpressions.Select(cls => cls)))
                        .Union(ontology.GetClassAxiomsOfType<OWLDisjointClasses>().SelectMany(ax => ax.ClassExpressions.Select(cls => cls)))
                        .Union(ontology.GetClassAxiomsOfType<OWLDisjointUnion>().Select(ax => ax.ClassIRI))
                        .Union(ontology.GetClassAxiomsOfType<OWLDisjointUnion>().SelectMany(ax => ax.ClassExpressions.Select(cls => cls))).ToList();
            //Widen the scope one level along the T-Box hierarchy: for each seed class, also pull in its superclasses and equivalent
            //classes, so the materialization loop below reaches classes that are only reachable transitively (e.g. C1 subClassOf C2,
            //with C2 never directly asserted/declared elsewhere). RemoveDuplicates here only dedupes the iteration source, not inScopeClsExprs itself.
            foreach (OWLClassExpression inScopeClsExpr in OWLExpressionHelper.RemoveDuplicates(inScopeClsExprs))
            {
                inScopeClsExprs.AddRange(ontology.GetSuperClassesOf(inScopeClsExpr));
                inScopeClsExprs.AddRange(ontology.GetEquivalentClasses(inScopeClsExpr));
            }

            //ClassAssertion(C1,I) ^ SubClassOf(C1,C2) -> ClassAssertion(C2,I)
            //ClassAssertion(C1,I) ^ EquivalentClasses(C1,C2) -> ClassAssertion(C2,I)
            //Final dedup: the widening step above can have added the same class multiple times (e.g. reached both as a
            //direct seed and as someone else's superclass), and GetIndividualsOf() would otherwise be run redundantly for it
            foreach (OWLClassExpression inScopeClsExpr in OWLExpressionHelper.RemoveDuplicates(inScopeClsExprs))
                foreach (OWLIndividualExpression idvExprOfInScopeClsExpr in ontology.GetIndividualsOf(inScopeClsExpr, clsAsns, false))
                {
                    OWLClassAssertion inference = new OWLClassAssertion(inScopeClsExpr) { IndividualExpression=idvExprOfInScopeClsExpr, IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

            return inferences;
        }
    }
}