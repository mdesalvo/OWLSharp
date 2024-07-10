/*
  Copyright 2014-2024 Marco De Salvo
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
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLClassAssertionEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			//Temporary working variables
			List<OWLClass> declaredClasses = ontology.GetDeclarationAxiomsOfType<OWLClass>()
													 .Select(ax => (OWLClass)ax.Expression)
													 .ToList();
			List<OWLClassAssertion> classAssertionAxioms = ontology.GetAssertionAxiomsOfType<OWLClassAssertion>();
			List<OWLEquivalentClasses> equivalentClassesAxioms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();
			List<OWLDisjointClasses> disjointClassesAxioms = ontology.GetClassAxiomsOfType<OWLDisjointClasses>();
			List<OWLDisjointUnion> disjointUnionAxioms = ontology.GetClassAxiomsOfType<OWLDisjointUnion>();
			List<OWLClassExpression> inScopeClsExprs = new List<OWLClassExpression>(declaredClasses);
			inScopeClsExprs.AddRange(classAssertionAxioms.Select(ax => ax.ClassExpression));
			inScopeClsExprs.AddRange(equivalentClassesAxioms.SelectMany(ax => ax.ClassExpressions.Select(cls => cls)));
			inScopeClsExprs.AddRange(disjointClassesAxioms.SelectMany(ax => ax.ClassExpressions.Select(cls => cls)));
			inScopeClsExprs.AddRange(disjointUnionAxioms.Select(ax => ax.ClassIRI));
			inScopeClsExprs.AddRange(disjointUnionAxioms.SelectMany(ax => ax.ClassExpressions.Select(cls => cls)));
			foreach (OWLClassExpression inScopeClsExpr in OWLExpressionHelper.RemoveDuplicates(inScopeClsExprs))
			{
				inScopeClsExprs.AddRange(ontology.GetSuperClassesOf(inScopeClsExpr));
				inScopeClsExprs.AddRange(ontology.GetEquivalentClasses(inScopeClsExpr));
			}
			inScopeClsExprs = OWLExpressionHelper.RemoveDuplicates(inScopeClsExprs);

			//ClassAssertion(C1,I) ^ SubClassOf(C1,C2) -> ClassAssertion(C2,I)
			//ClassAssertion(C1,I) ^ EquivalentClasses(C1,C2) -> ClassAssertion(C2,I)
            foreach (OWLClassExpression inScopeClsExpr in inScopeClsExprs)
			    foreach (OWLIndividualExpression idvExprOfInScopeClsExpr in ontology.GetIndividualsOf(inScopeClsExpr))
					inferences.Add(new OWLClassAssertion(inScopeClsExpr) { IndividualExpression=idvExprOfInScopeClsExpr, IsInference=true });

			//Remove inferences already stated in explicit knowledge
			inferences.RemoveAll(inf => classAssertionAxioms.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

			return OWLAxiomHelper.RemoveDuplicates(inferences);
		}
    }
}