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

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLAxiomHelper simplifies axiom modeling with a set of facilities
    /// </summary>
    internal static class OWLAxiomHelper
    {
        #region Methods
        /// <summary>
        /// Removes the duplicate axioms from the given list
        /// </summary>
        internal static List<T> RemoveDuplicates<T>(List<T> axioms) where T : OWLAxiom
        {
            #region Guards
            if (axioms == null || axioms.Count == 0)
                return new List<T>();
            #endregion

#if NET8_0_OR_GREATER
            HashSet<string> lookup = new HashSet<string>(axioms.Count);
#else
            HashSet<string> lookup = new HashSet<string>();
#endif
            List<T> deduplicatedAxioms = new List<T>(axioms.Count);
            foreach (T axiom in axioms)
            {
                if (lookup.Add(axiom.GetXML()))
                    deduplicatedAxioms.Add(axiom);
            }
            return deduplicatedAxioms;
        }
        #endregion
    }
}