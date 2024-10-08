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

using System.Collections.Generic;
using System.Linq;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Axioms;

namespace OWLSharp.Ontology.Helpers
{
	public static class OWLDeclarationHelper
	{
		#region Methods
		public static List<OWLDeclaration> GetDeclarationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLExpression, IOWLEntity
            => ontology?.DeclarationAxioms.Where(ax => ax.Expression is T).ToList() ?? new List<OWLDeclaration>();

		public static List<T> GetDeclaredEntitiesOfType<T>(this OWLOntology ontology) where T : OWLExpression, IOWLEntity
            => ontology?.GetDeclarationAxiomsOfType<T>().Select(ax => (T)ax.Expression).ToList() ?? new List<T>();

		public static bool CheckHasEntity<T>(this OWLOntology ontology, T entity) where T : OWLExpression, IOWLEntity
            => GetDeclarationAxiomsOfType<T>(ontology).Any(ax => ax.Expression.GetIRI().Equals(entity?.GetIRI()));
		#endregion
	}
}