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

using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDeclarationHelper simplifies declaration axiom modeling with a set of facilities
    /// </summary>
    public static class OWLDeclarationHelper
    {
        #region Methods
        /// <summary>
        /// Enlists the given type of declaration axiom from the T-BOX/A-BOX of the given ontology
        /// </summary>
        public static List<OWLDeclaration> GetDeclarationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLExpression, IOWLEntity
            => ontology?.DeclarationAxioms.Where(ax => ax.Entity is T).ToList() ?? new List<OWLDeclaration>();

        /// <summary>
        /// Enlists the given type of entity declaration from the T-BOX/A-BOX of the given ontology
        /// </summary>
        public static List<T> GetDeclaredEntitiesOfType<T>(this OWLOntology ontology) where T : OWLExpression, IOWLEntity
            => ontology?.GetDeclarationAxiomsOfType<T>().ConvertAll(ax => (T)ax.Entity) ?? new List<T>();

        /// <summary>
        /// Checks if the given ontology has the given entity declaration in its T-BOX/A-BOX
        /// </summary>
        public static bool CheckHasEntity<T>(this OWLOntology ontology, T entity) where T : OWLExpression, IOWLEntity
            => GetDeclarationAxiomsOfType<T>(ontology).Any(ax => ax.Entity.GetIRI().Equals(entity?.GetIRI()));

        /// <summary>
        /// Declares the given entity to the T-BOX/A-BOX of the given ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static void DeclareEntity<T>(this OWLOntology ontology, T entityIRI) where T : OWLExpression, IOWLEntity
        {
            #region Guards
            if (entityIRI == null)
                throw new OWLException($"Cannot declare entity because given '{nameof(entityIRI)}' parameter is null");
            #endregion

            if (!CheckHasEntity(ontology, entityIRI))
                ontology?.DeclarationAxioms.Add(new OWLDeclaration { Entity = entityIRI });
        }
        #endregion
    }
}