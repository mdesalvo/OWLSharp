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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Validator;

namespace OWLSharp.Test.Validator
{
    [TestClass]
    public class OWLIssueTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIssue()
        {
            OWLIssue issue = new OWLIssue(
                OWLEnums.OWLIssueSeverity.Warning,
                " rulename ",
                " this is a warning ",
                " try solve this way... ");

            Assert.IsNotNull(issue);
            Assert.IsTrue(issue.Severity == OWLEnums.OWLIssueSeverity.Warning);
            Assert.IsTrue(string.Equals(issue.RuleName, "rulename"));
            Assert.IsTrue(string.Equals(issue.Description, "this is a warning"));
            Assert.IsTrue(string.Equals(issue.Suggestion, "try solve this way..."));
        }

        [TestMethod]
        public void ShouldThrowExceptiononCreatingInferenceWithNullRuleName()
            => Assert.ThrowsException<OWLException>(() => new OWLIssue(OWLEnums.OWLIssueSeverity.Warning, null, "this is a warning", "try solve this way..."));

        [TestMethod]
        public void ShouldThrowExceptiononCreatingInferenceWithNullDescription()
            => Assert.ThrowsException<OWLException>(() => new OWLIssue(OWLEnums.OWLIssueSeverity.Warning, "rulename", null, "try solve this way..."));

        [TestMethod]
        public void ShouldThrowExceptiononCreatingInferenceWithNullSuggestion()
            => Assert.ThrowsException<OWLException>(() => new OWLIssue(OWLEnums.OWLIssueSeverity.Warning, "rulename", "this is a warning", null));
        #endregion
    }
}