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
using System.Diagnostics.CodeAnalysis;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSReasoner gives SKOS reasoning capabilities to standard reasoners
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SKOSReasoner
    {
        #region Methods
        /// <summary>
        /// Applies the SKOS reasoner on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLReasoner reasoner, OWLOntology ontology, Dictionary<string, OWLReasonerReport> inferenceRegistry)
        {
            //TODO
        }
        #endregion
    }
}