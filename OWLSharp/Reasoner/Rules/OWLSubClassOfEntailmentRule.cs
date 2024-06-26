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
    internal static class OWLSubClassOfEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
			//SubClassOf(C1,C2) ^ EquivalentClasses(C2,C3) -> SubClassOf(C1,C3)
			//DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
            foreach (OWLClass declaredClass in ontology.GetDeclarationAxiomsOfType<OWLClass>()
													   .Select(ax => (OWLClass)ax.Expression))
			{
				List<OWLClassExpression> inferredSuperClasses = ontology.GetSuperClassesOf(declaredClass);
                foreach (OWLClassExpression inferredSuperClass in inferredSuperClasses)
                    inferences.Add(new OWLSubClassOf(declaredClass, inferredSuperClass) { IsInference = true });
			}

            return inferences;
        }
    }
}