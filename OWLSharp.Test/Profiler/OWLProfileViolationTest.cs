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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Profiler;

namespace OWLSharp.Test.Profiler;

[TestClass]
public class OWLProfileViolationTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateViolation()
    {
        //Ctor trims surrounding whitespace on the string fields, same contract as OWLIssue: verify it here too.
        OWLProfileViolation violation = new OWLProfileViolation(
            OWLEnums.OWLProfiles.EL,
            " rulename ",
            " this construct is not admitted in EL ",
            " remove it or switch to a less restrictive profile ");

        Assert.IsNotNull(violation);
        Assert.AreEqual(OWLEnums.OWLProfiles.EL, violation.Profile);
        Assert.IsTrue(string.Equals(violation.RuleName, "rulename"));
        Assert.IsTrue(string.Equals(violation.Description, "this construct is not admitted in EL"));
        Assert.IsTrue(string.Equals(violation.Suggestion, "remove it or switch to a less restrictive profile"));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingViolationWithNullRuleName()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLProfileViolation(OWLEnums.OWLProfiles.EL, null, "description", "suggestion"));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingViolationWithNullDescription()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLProfileViolation(OWLEnums.OWLProfiles.EL, "rulename", null, "suggestion"));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingViolationWithNullSuggestion()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLProfileViolation(OWLEnums.OWLProfiles.EL, "rulename", "description", null));
    #endregion
}
