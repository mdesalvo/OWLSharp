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
    public class OWLIssue
    {
        #region Properties
		public OWLEnums.OWLIssueSeverity Severity { get; internal set; }
        public string RuleName { get; internal set; }
        public string Description { get; internal set; }
		public string Suggestion { get; internal set; }
        #endregion

        #region Ctors
        public OWLIssue(OWLEnums.OWLIssueSeverity severity , string ruleName, string description, string suggestion)
        {
			Severity = severity;
            RuleName = ruleName?.Trim() ?? throw new OWLException("Cannot emit issue because given \"ruleName\" parameter is null");
            Description = description?.Trim() ?? throw new OWLException("Cannot emit issue because given \"description\" parameter is null");
			Suggestion = suggestion?.Trim() ?? throw new OWLException("Cannot emit issue because given \"suggestion\" parameter is null");
        }
        #endregion
    }
}