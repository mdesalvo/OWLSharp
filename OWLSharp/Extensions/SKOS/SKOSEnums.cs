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

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSEnums represents a collector for all the enumerations used by the "OWLSharp.Extensions.SKOS" namespace
    /// </summary>
    public static class SKOSEnums
    {
        /// <summary>
        /// Represents an enumeration for the set of built-in SKOS validator rules
        /// </summary>
        public enum SKOSValidatorRules
        {
            /// <summary>
            /// This SKOS rule checks for consistency of skos:hasTopConcept knowledge
            /// </summary>
            TopConcept = 1,
            /// <summary>
            /// This SKOS-XL rule checks for consistency of skosxl:literalForm knowledge
            /// </summary>
            LiteralForm = 2
        };

        /// <summary>
        /// Represents an enumeration for supported types of SKOS concept documentation
        /// </summary>
        public enum SKOSDocumentationTypes
        {
            /// <summary>
            /// skos:changeNote
            /// </summary>
            ChangeNote = 1,
            /// <summary>
            /// skos:definition
            /// </summary>
            Definition = 2,
            /// <summary>
            /// skos:editorialNote
            /// </summary>
            EditorialNote = 3,
            /// <summary>
            /// skos:example
            /// </summary>
            Example = 4,
            /// <summary>
            /// skos:historyNote
            /// </summary>
            HistoryNote = 5,
            /// <summary>
            /// skos:note
            /// </summary>
            Note = 6,
            /// <summary>
            /// skos:scopeNote
            /// </summary>
            ScopeNote = 7
        }
    }
}