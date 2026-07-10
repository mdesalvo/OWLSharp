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
    /// <summary>
    /// NOTE: this is the T-Box counterpart of SchemaSubClassOfEntailment's ObjectIntersectionOf branch
    /// (scm-int: SubClassOf(C1,ObjectIntersectionOf(C2,C3)) -> SubClassOf(C1,C2) ^ SubClassOf(C1,C3)) -- scm-uni is the "opposite"
    /// direction for unions (each disjunct is a SubClassOf of the union, rather than the union decomposing into conjuncts).
    /// Combined iteratively with FactClassAssertionEntailment (cax-sco), the SubClassOf materialized here also re-derives the A-Box
    /// cls-uni entailment for ANY referenced union (including anonymous ones), so cls-uni does not need its own dedicated rule.
    /// </summary>
    internal static class OWLSchemaObjectUnionOfEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaObjectUnionOfEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            //Collect all ObjectUnionOf expressions referenced anywhere in the T-Box (SubClassOf, EquivalentClasses)
            List<OWLObjectUnionOf> referencedUnions = new List<OWLObjectUnionOf>();
            referencedUnions.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectUnionOf).Select(ax => (OWLObjectUnionOf)ax.SuperClassExpression));
            referencedUnions.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLObjectUnionOf).Select(ax => (OWLObjectUnionOf)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedUnions.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectUnionOf>());
            referencedUnions = OWLExpressionHelper.RemoveDuplicates(referencedUnions);

            //scm-uni: ObjectUnionOf(C,(C1..CN)) [referenced] -> SubClassOf(C1,C) ^ ... ^ SubClassOf(CN,C)
            foreach (OWLObjectUnionOf union in referencedUnions)
                foreach (OWLClassExpression unionMember in union.ClassExpressions)
                {
                    OWLSubClassOf inference = new OWLSubClassOf(unionMember, union) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

            return inferences;
        }
    }
}
