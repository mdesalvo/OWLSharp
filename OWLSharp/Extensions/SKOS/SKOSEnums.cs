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
        /// Represents an enumeration for the set of built-in standard SKOS/SKOS-XL validator rules
        /// </summary>
        public enum SKOSValidatorStandardRules
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
    }
}