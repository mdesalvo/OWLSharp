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
    internal static class OWLDisjointClassesEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            OWLReasonerReport report = new OWLReasonerReport();

			//EquivalentClasses(C1,C2) ^ DisjointWith(C2,C3) -> DisjointWith(C1,C3)
			//SubClassOf(C1,C2) ^ DisjointWith(C2,C3) -> DisjointWith(C1,C3)
            foreach (OWLClass declaredClass in ontology.GetDeclarationAxiomsOfType<OWLClass>()
													   .Select(ax => (OWLClass)ax.Expression))
			{
				List<OWLClassExpression> inferredDisjointClasses = ontology.GetDisjointClasses(declaredClass);
                foreach (OWLClassExpression inferredDisjointClass in inferredDisjointClasses)
                {
                    OWLReasonerInference inference = new OWLReasonerInference(
                        nameof(OWLDisjointClassesEntailmentRule), 
						new OWLDisjointClasses(new List<OWLClassExpression>() { declaredClass, inferredDisjointClass }) { IsInference=true });

                    report.Inferences.Add(inference);
                }
			}

			foreach (OWLDisjointUnion disjointUnion in ontology.GetClassAxiomsOfType<OWLDisjointUnion>())
			{
				//DisjointUnion(C1,(C2 C3)) -> DisjointClasses(C2,C3)
				OWLReasonerInference inference = new OWLReasonerInference(
					nameof(OWLDisjointClassesEntailmentRule), 
					new OWLDisjointClasses(disjointUnion.ClassExpressions) { IsInference=true });

				report.Inferences.Add(inference);

				//DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1)
				//DisjointUnion(C1,(C2 C3)) -> SubClassOf(C3,C1)
				foreach (OWLClassExpression disjointUnionClassExpression in disjointUnion.ClassExpressions)
				{
					OWLReasonerInference subClassInference = new OWLReasonerInference(
						nameof(OWLDisjointClassesEntailmentRule), 
						new OWLSubClassOf(disjointUnionClassExpression, disjointUnion.ClassIRI) { IsInference=true });

					report.Inferences.Add(inference);
				}
			}

            return report;
        }
    }
}