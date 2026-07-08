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

namespace OWLSharp.Profiler.Walkers
{
    /// <summary>
    /// OWLPropertyAxiomWalker enumerates the class/data range expressions embedded in property domain/range axioms
    /// (ObjectPropertyDomain, ObjectPropertyRange, DataPropertyDomain, DataPropertyRange) — the one family of
    /// embedded expressions that OWLClassAxiomWalker does NOT cover, since its documented scope is ClassAxioms
    /// only (SubClassOf/EquivalentClasses/DisjointClasses). Before this walker existed, OWLELProfile, OWLRLProfile
    /// and OWLQLProfile each hand-rolled the same three/four `foreach (... GetObjectPropertyAxiomsOfType...)` loops
    /// independently inside their own ExecuteRuleAsync — this factors that duplicated iteration into one place,
    /// the same way OWLClassAxiomWalker already did for SubClassOf/EquivalentClasses/DisjointClasses.
    /// </summary>
    internal static class OWLPropertyAxiomWalker
    {
        /// <summary>
        /// Walks ObjectPropertyDomain, ObjectPropertyRange and DataPropertyDomain, yielding each embedded class
        /// expression alongside its owning axiom. Deliberately does NOT tag a position the way
        /// OWLClassAxiomWalker.WalkClassAxioms does: structurally, a property's domain/range class always plays
        /// the SUPERclass role (ObjectPropertyDomain(OP,C) is equivalent to SubClassOf(ObjectSomeValuesFrom(OP,owl:Thing),C),
        /// i.e. C is the consequent) — but EL has no sub/super distinction to begin with, so baking a position
        /// into this walker would force EL's caller to awkwardly ignore it. Callers that DO need a position
        /// (OWLRLProfile, OWLQLProfile) apply OWLEnums.OWLClassExpressionPosition.SuperClass themselves at the
        /// call site, exactly as they already do for this same structural reasoning.
        /// </summary>
        internal static IEnumerable<(OWLAxiom Axiom, OWLClassExpression ClassExpression)> WalkPropertyDomainRangeClassExpressions(OWLOntology ontology)
        {
            foreach (OWLObjectPropertyDomain domain in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>())
                yield return (domain, domain.ClassExpression);

            foreach (OWLObjectPropertyRange range in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>())
                yield return (range, range.ClassExpression);

            foreach (OWLDataPropertyDomain domain in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>())
                yield return (domain, domain.ClassExpression);
        }

        /// <summary>
        /// Walks DataPropertyRange, yielding each embedded data range expression alongside its owning axiom.
        /// Kept as its own method (rather than folded into the class-expression walk above) because
        /// DataPropertyRange(DP,DR) states its range directly as a DataRangeExpression, with no class expression
        /// step in between — a genuinely different expression family (OWLDataRangeExpression, not
        /// OWLClassExpression), consumed by each profiler's CheckDataRange rather than CheckClassExpression.
        /// </summary>
        internal static IEnumerable<(OWLAxiom Axiom, OWLDataRangeExpression DataRangeExpression)> WalkPropertyRangeDataRanges(OWLOntology ontology)
        {
            foreach (OWLDataPropertyRange range in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>())
                yield return (range, range.DataRangeExpression);
        }
    }
}