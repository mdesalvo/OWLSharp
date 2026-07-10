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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Profiler;

namespace OWLSharp.Test.Profiler;

[TestClass]
public class OWLProfileReportTest
{
    #region Tests
    [TestMethod]
    public async Task ShouldReportCompliantOnEmptyOntologyAsync()
    {
        //An ontology with no axioms cannot violate any profile's grammar: exercised through the public
        //CheckProfileAsync entry point (the ctor of OWLProfileReport itself is internal, by design,
        //because reports are only meant to be produced by the profiler, not hand-built by callers).
        OWLOntology ontology = new OWLOntology();

        OWLProfileReport report = await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.EL);

        Assert.IsNotNull(report);
        Assert.AreEqual(OWLEnums.OWLProfiles.EL, report.Profile);
        Assert.IsEmpty(report.Violations);
        Assert.IsTrue(report.IsCompliant);
    }

    //Ctor and Violations setter are internal (only OWLProfiler is meant to build reports), but the test
    //assembly reaches them via InternalsVisibleTo: this lets IsCompliant be unit-tested in isolation,
    //without depending on any RuleSet class actually detecting a real violation yet.
    [TestMethod]
    public void ShouldNotBeCompliantWhenViolationsArePresent()
    {
        OWLProfileReport report = new OWLProfileReport(OWLEnums.OWLProfiles.RL)
        {
            Violations = [
                new OWLProfileViolation(OWLEnums.OWLProfiles.RL, "rulename", "description", "suggestion")
            ]
        };

        Assert.IsFalse(report.IsCompliant);
        Assert.HasCount(1, report.Violations);
    }

    [TestMethod]
    public void ShouldAccumulateMultipleViolationsFromDifferentRules()
    {
        OWLProfileReport report = new OWLProfileReport(OWLEnums.OWLProfiles.QL)
        {
            Violations = [
                new OWLProfileViolation(OWLEnums.OWLProfiles.QL, "rule1", "description1", "suggestion1"),
                new OWLProfileViolation(OWLEnums.OWLProfiles.QL, "rule2", "description2", "suggestion2")
            ]
        };

        Assert.IsFalse(report.IsCompliant);
        Assert.HasCount(2, report.Violations);
    }

    //Corner case: an explicitly-assigned-but-empty list must behave exactly like the default one
    //(IsCompliant is a Count check, not a null check), so re-assigning Violations to List.Empty
    //should not accidentally look non-compliant.
    [TestMethod]
    public void ShouldBeCompliantWhenViolationsAreExplicitlyEmptied()
    {
        OWLProfileReport report = new OWLProfileReport(OWLEnums.OWLProfiles.EL)
        {
            Violations = []
        };

        Assert.IsTrue(report.IsCompliant);
    }
    #endregion
}