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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Profiler;

namespace OWLSharp.Test.Profiler;

[TestClass]
public class OWLProfilerTest
{
    #region Tests
    [TestMethod]
    public async Task ShouldThrowExceptionOnCheckingProfileOfNullOntologyAsync()
        //CheckProfileAsync is now genuinely async (it awaits ExecuteProfileRuleAsync internally instead of
        //wrapping synchronous work in Task.FromResult), so its guard-clause exception is captured into the
        //returned Task's fault state rather than thrown synchronously: it must be awaited (ThrowsExactlyAsync)
        //to surface it, same as CheckProfilesAsync below.
        => await Assert.ThrowsExactlyAsync<OWLException>(() => OWLProfiler.CheckProfileAsync(null, OWLEnums.OWLProfiles.EL));

    [TestMethod]
    public async Task ShouldThrowExceptionOnCheckingProfilesOfNullOntologyAsync()
        //CheckProfilesAsync is declared async: its guard-clause exception is captured into the returned Task's
        //fault state rather than thrown synchronously, so it must be awaited (ThrowsExactlyAsync) to surface it.
        => await Assert.ThrowsExactlyAsync<OWLException>(() => OWLProfiler.CheckProfilesAsync(null));

    //One test per profile enum value, instead of a single parametrized test, so that once each RuleSet phase
    //(EL, then RL, then QL) lands with real predicates, its dedicated test starts asserting real violations
    //without touching the other two.
    [TestMethod]
    public async Task ShouldCheckELProfileOfOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology();

        OWLProfileReport report = await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.EL);

        Assert.IsNotNull(report);
        Assert.AreEqual(OWLEnums.OWLProfiles.EL, report.Profile);
    }

    [TestMethod]
    public async Task ShouldCheckQLProfileOfOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology();

        OWLProfileReport report = await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.QL);

        Assert.IsNotNull(report);
        Assert.AreEqual(OWLEnums.OWLProfiles.QL, report.Profile);
    }

    [TestMethod]
    public async Task ShouldCheckRLProfileOfOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology();

        OWLProfileReport report = await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.RL);

        Assert.IsNotNull(report);
        Assert.AreEqual(OWLEnums.OWLProfiles.RL, report.Profile);
    }

    [TestMethod]
    public async Task ShouldCheckAllProfilesOfOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology();

        //CheckProfilesAsync must cover exactly the three enum values, one report each, keyed by profile:
        //this is the "shared walk" entry point discussed in the design (currently one CheckProfileAsync
        //call per profile under the hood; a future optimization may fold the three walks into one pass
        //without changing this contract).
        Dictionary<OWLEnums.OWLProfiles, OWLProfileReport> reports = await ontology.CheckProfilesAsync();

        Assert.IsNotNull(reports);
        Assert.HasCount(3, reports);
        Assert.IsTrue(reports.ContainsKey(OWLEnums.OWLProfiles.EL));
        Assert.IsTrue(reports.ContainsKey(OWLEnums.OWLProfiles.QL));
        Assert.IsTrue(reports.ContainsKey(OWLEnums.OWLProfiles.RL));
    }
    #endregion
}
