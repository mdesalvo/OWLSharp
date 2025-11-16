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

using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLExpressionHelper simplifies expression modeling with a set of facilities
    /// </summary>
    public static class OWLExpressionHelper
    {
        #region Methods
        /// <summary>
        /// Gets an entity of the given type from the given RDF resource
        /// </summary>
        public static T ToEntity<T>(this RDFResource iri) where T : IOWLEntity
        {
            object entity = null;
            switch (typeof(T).Name)
            {
                case nameof(OWLClass):
                    entity = new OWLClass(iri);
                    break;
                case nameof(OWLDatatype):
                    entity = new OWLDatatype(iri);
                    break;
                case nameof(OWLNamedIndividual):
                    entity = new OWLNamedIndividual(iri);
                    break;
                case nameof(OWLAnnotationProperty):
                    entity = new OWLAnnotationProperty(iri);
                    break;
                case nameof(OWLDataProperty):
                    entity = new OWLDataProperty(iri);
                    break;
                case nameof(OWLObjectProperty):
                    entity = new OWLObjectProperty(iri);
                    break;
            }
            return (T)entity;
        }

        /// <summary>
        /// Removes the duplicate expressions from the given list
        /// </summary>
        internal static List<T> RemoveDuplicates<T>(List<T> expressions) where T : OWLExpression
        {
            #region Guards
            if (expressions == null || expressions.Count == 0)
                return new List<T>();
            #endregion

#if NET8_0_OR_GREATER
            HashSet<long> lookup = new HashSet<long>(expressions.Count);
#else
            HashSet<long> lookup = new HashSet<long>();
#endif
            List<T> deduplicatedExpressions = new List<T>(expressions.Count);
            foreach (T expression in expressions)
            {
                if (lookup.Add(expression.GetIRI().PatternMemberID))
                    deduplicatedExpressions.Add(expression);
            }
            return deduplicatedExpressions;
        }
        #endregion
    }
}