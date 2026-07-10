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

using System.Collections.Generic;
using OWLSharp.Ontology;

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLClassAxiomWalker enumerates every top-level class expression "slot" found in an ontology's class axioms
    /// (SubClassOf, EquivalentClasses, DisjointClasses), tagged with the syntactic position
    /// (subClassExpression/superClassExpression/equivClassExpression) it occupies per the OWL2 profiles grammars
    /// (https://www.w3.org/TR/owl2-profiles/). It is shared by OWLELProfile, OWLQLProfile and OWLRLProfile, so the
    /// three profilers do not each re-implement axiom iteration: they only supply the per-profile, per-position
    /// predicate that recurses into the class expression tree, inverting position (via OWLProfileWalker.InvertPosition)
    /// on constructs like ObjectComplementOf which wrap a subClassExpression while only being admitted in superclass position.
    /// </summary>
    internal static class OWLClassAxiomWalker
    {
        /// <summary>
        /// Walks SubClassOf, EquivalentClasses and DisjointClasses axioms, yielding each class expression slot
        /// alongside its owning axiom and its syntactic position
        /// </summary>
        internal static IEnumerable<(OWLClassAxiom Axiom, OWLClassExpression ClassExpression, OWLEnums.OWLClassExpressionPosition Position)> WalkClassAxioms(OWLOntology ontology)
        {
            //SubClassOf(sub,super): the two sides play different syntactic roles per the spec grammar,
            //so each is tagged with its own position. A profile's predicate for "SubClass" position
            //may admit different constructs than the one for "SuperClass" (e.g. RL: union only on the
            //sub side, universal/cardinality restrictions only on the super side).
            foreach (OWLSubClassOf subClassOf in ontology.GetClassAxiomsOfType<OWLSubClassOf>())
            {
                yield return (subClassOf, subClassOf.SubClassExpression, OWLEnums.OWLClassExpressionPosition.SubClass);
                yield return (subClassOf, subClassOf.SuperClassExpression, OWLEnums.OWLClassExpressionPosition.SuperClass);
            }

            //EquivalentClasses(C1,C2,...): per the RL grammar this uses the stricter "equivClassExpression"
            //production (narrower than both SubClass and SuperClass), so every member gets that dedicated tag
            //rather than being folded into SubClass/SuperClass.
            foreach (OWLEquivalentClasses equivalentClasses in ontology.GetClassAxiomsOfType<OWLEquivalentClasses>())
                foreach (OWLClassExpression classExpression in equivalentClasses.ClassExpressions)
                    yield return (equivalentClasses, classExpression, OWLEnums.OWLClassExpressionPosition.EquivalentClass);

            //DisjointClasses(C1,C2,...): the spec grammar uses "subClassExpression" for every member
            //in both QL and RL, so all members are tagged SubClass here (there is no separate
            //"disjoint position" in the grammar).
            foreach (OWLDisjointClasses disjointClasses in ontology.GetClassAxiomsOfType<OWLDisjointClasses>())
                foreach (OWLClassExpression classExpression in disjointClasses.ClassExpressions)
                    yield return (disjointClasses, classExpression, OWLEnums.OWLClassExpressionPosition.SubClass);
        }
    }
}