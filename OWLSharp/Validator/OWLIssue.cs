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

namespace OWLSharp.Validator
{
    /// <summary>
    /// OWLIssue represents a modeling pitfall or inconsistency discovered by a validator on an ontology
    /// </summary>
    public sealed class OWLIssue
    {
        #region Properties
        /// <summary>
        /// The severity of the issue
        /// </summary>
        public OWLEnums.OWLIssueSeverity Severity { get; }
        /// <summary>
        /// The name of the validator rule which evidenced the issue
        /// </summary>
        public string RuleName { get; }
        /// <summary>
        /// The description of the issue
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// A potential action to be considered for resolving the issue
        /// </summary>
        public string Suggestion { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an issue with the given level of severity and the given characteristics
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLIssue(OWLEnums.OWLIssueSeverity severity , string ruleName, string description, string suggestion)
        {
            Severity = severity;
            RuleName = ruleName?.Trim() ?? throw new OWLException($"Cannot emit issue because given '{nameof(ruleName)}' parameter is null");
            Description = description?.Trim() ?? throw new OWLException($"Cannot emit issue because given '{nameof(description)}' parameter is null");
            Suggestion = suggestion?.Trim() ?? throw new OWLException($"Cannot emit issue because given '{nameof(suggestion)}' parameter is null");
        }
        #endregion
    }
}