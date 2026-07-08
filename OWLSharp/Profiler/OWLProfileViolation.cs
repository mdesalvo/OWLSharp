/*
   Copyright 2014-2026 Marco De Salvo
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

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLProfileViolation is a finding produced by the profiler that identifies an axiom, or a syntactic construct within it,
    /// which does not conform to the grammar of a given OWL2 profile (EL, QL, RL) as defined at https://www.w3.org/TR/owl2-profiles/.
    /// Unlike validator issues, which flag modeling errors or inconsistencies, profile violations are purely syntactic:
    /// they do not imply that the ontology is wrong, only that it falls outside the specific profile's restricted grammar.
    /// </summary>
    public sealed class OWLProfileViolation
    {
        #region Properties
        /// <summary>
        /// The profile against which the violation has been detected
        /// </summary>
        public OWLEnums.OWLProfiles Profile { get; }

        /// <summary>
        /// The name of the profiler rule which evidenced the violation
        /// </summary>
        public string RuleName { get; }

        /// <summary>
        /// The description of the violation
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A potential action to be considered for making the offending construct compliant with the profile
        /// </summary>
        public string Suggestion { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a violation for the given profile and with the given characteristics
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLProfileViolation(OWLEnums.OWLProfiles profile, string ruleName, string description, string suggestion)
        {
            Profile = profile;
            RuleName = ruleName?.Trim() ?? throw new OWLException($"Cannot emit profile violation because given '{nameof(ruleName)}' parameter is null");
            Description = description?.Trim() ?? throw new OWLException($"Cannot emit profile violation because given '{nameof(description)}' parameter is null");
            Suggestion = suggestion?.Trim() ?? throw new OWLException($"Cannot emit profile violation because given '{nameof(suggestion)}' parameter is null");
        }
        #endregion
    }
}