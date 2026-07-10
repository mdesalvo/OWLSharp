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
    /// NOTE: GetIndividualsOf() only expands ObjectOneOf membership when the enumeration is reached
    /// as an EquivalentClasses alias of a named class; when an ObjectOneOf is referenced anonymously (e.g. as the filler of an
    /// ObjectSomeValuesFrom/ObjectAllValuesFrom restriction, which is the most common real-world usage), nothing else in the ruleset
    /// materializes its member ClassAssertions, so this dedicated rule is required (unlike scm-uni, an anonymous ObjectOneOf has no
    /// SubClassOf target to piggyback its A-Box materialization on).
    /// </summary>
    internal static class OWLFactObjectOneOfEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.FactObjectOneOfEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            //Collect all ObjectOneOf expressions referenced directly in the T-Box (SubClassOf, EquivalentClasses)
            List<OWLObjectOneOf> referencedOneOfs = new List<OWLObjectOneOf>();
            referencedOneOfs.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectOneOf).Select(ax => (OWLObjectOneOf)ax.SuperClassExpression));
            referencedOneOfs.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLObjectOneOf).Select(ax => (OWLObjectOneOf)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedOneOfs.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectOneOf>());

            //Also collect ObjectOneOf expressions nested one level deep as the filler of a referenced ObjectSomeValuesFrom/ObjectAllValuesFrom
            //(the most common real-world pattern, e.g. "hasDay some {Monday,Tuesday,...,Sunday}")
            List<OWLClassExpression> referencedSuperOrSubClsExprs = scofAxms.Select(ax => ax.SuperClassExpression)
                                                                             .Union(scofAxms.Select(ax => ax.SubClassExpression))
                                                                             .Union(eqClsAxms.SelectMany(ax => ax.ClassExpressions))
                                                                             .ToList();
            foreach (OWLObjectSomeValuesFrom svf in referencedSuperOrSubClsExprs.OfType<OWLObjectSomeValuesFrom>())
                if (svf.ClassExpression is OWLObjectOneOf nestedSvfOneOf)
                    referencedOneOfs.Add(nestedSvfOneOf);
            foreach (OWLObjectAllValuesFrom avf in referencedSuperOrSubClsExprs.OfType<OWLObjectAllValuesFrom>())
                if (avf.ClassExpression is OWLObjectOneOf nestedAvfOneOf)
                    referencedOneOfs.Add(nestedAvfOneOf);

            referencedOneOfs = OWLExpressionHelper.RemoveDuplicates(referencedOneOfs);

            //cls-oo: ObjectOneOf(C,(I1..IN)) [referenced] -> ClassAssertion(C,I1) ^ ... ^ ClassAssertion(C,IN)
            foreach (OWLObjectOneOf oneOf in referencedOneOfs)
                foreach (OWLIndividualExpression member in oneOf.IndividualExpressions)
                {
                    OWLClassAssertion inference = new OWLClassAssertion(oneOf) { IndividualExpression=member, IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

            return inferences;
        }
    }
}
