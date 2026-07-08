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
using RDFSharp.Model;

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

    //ExecuteProfileRuleAsync's switch has a `default` branch returning an empty violation list for any
    //OWLEnums.OWLProfiles value outside EL/QL/RL — unreachable through the enum's own declared members, but
    //reachable by a caller casting an out-of-range int (e.g. legacy serialized data, or a future enum member
    //added to the model but not yet wired into the switch). Confirms this degrades gracefully to "no violations
    //reported" rather than throwing an unhandled exception for a value the switch doesn't recognize.
    [TestMethod]
    public async Task ShouldReturnEmptyViolationsForUnrecognizedProfileValueAsync()
    {
        OWLOntology ontology = new OWLOntology();

        OWLProfileReport report = await ontology.CheckProfileAsync((OWLEnums.OWLProfiles)999);

        Assert.IsNotNull(report);
        Assert.IsEmpty(report.Violations);
    }

    //--- Entry-point tests: exercise the public OWLOntology.CheckProfileAsync/CheckProfilesAsync surface with -----
    //--- genuinely non-compliant ontologies, not just the empty-ontology wiring checks above. Everywhere else -----
    //--- in this test project (OWLELProfileTest/OWLRLProfileTest/OWLQLProfileTest), violations are asserted by ----
    //--- calling the internal OWL{EL,RL,QL}Profile.ExecuteRuleAsync directly — which is the right level for -------
    //--- exhaustively enumerating every grammar corner case, but never actually goes through the public ----------
    //--- CheckProfileAsync/CheckProfileAsync->ExecuteProfileRuleAsync dispatch chain an external caller would -----
    //--- use. These tests close that gap: they enter purely from the public extension-method surface on -----------
    //--- OWLOntology, the same way a library consumer would, and confirm a violation detected deep inside one -----
    //--- profile's RuleSet class actually surfaces all the way up through OWLProfileReport.

    //A bare ObjectUnionOf axiom is one of the simplest possible EL violations (§2.2.3 excludes union outright):
    //entering via ontology.CheckProfileAsync(EL) rather than OWLELProfile.ExecuteRuleAsync directly confirms the
    //whole chain (extension method -> ExecuteProfileRuleAsync's switch dispatch -> OWLELProfile -> OWLProfileReport)
    //wires together correctly end-to-end, not just that OWLELProfile's own logic is correct in isolation.
    [TestMethod]
    public async Task ShouldSurfaceELViolationThroughPublicEntryPointAsync()
    {
        OWLClass petMammal = new OWLClass(new RDFResource("http://pets.org/PetMammal"));
        OWLClass dog = new OWLClass(new RDFResource("http://pets.org/Dog"));
        OWLClass cat = new OWLClass(new RDFResource("http://pets.org/Cat"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectUnionOf([dog, cat]), petMammal)]
        };

        OWLProfileReport report = await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.EL);

        Assert.IsFalse(report.IsCompliant);
        Assert.HasCount(1, report.Violations);
        Assert.AreEqual(OWLEnums.OWLProfiles.EL, report.Violations[0].Profile);
        Assert.IsTrue(report.Violations[0].RuleName.EndsWith(".ClassExpression"));
    }

    //ReflexiveObjectProperty is the one and only object property axiom type RL's grammar excludes (§4.2.5) —
    //picked deliberately because it is RL-specific and would NOT be flagged by EL or QL's dispatch branch,
    //so a copy-paste mistake in ExecuteProfileRuleAsync's switch (e.g. routing RL to OWLQLProfile) would be
    //caught by this test asserting the violation's Profile is exactly RL, not just "some" violation.
    [TestMethod]
    public async Task ShouldSurfaceRLViolationThroughPublicEntryPointAsync()
    {
        OWLObjectProperty relatedTo = new OWLObjectProperty(new RDFResource("http://pets.org/relatedTo"));
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [new OWLReflexiveObjectProperty(relatedTo)]
        };

        OWLProfileReport report = await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.RL);

        Assert.IsFalse(report.IsCompliant);
        Assert.HasCount(1, report.Violations);
        Assert.AreEqual(OWLEnums.OWLProfiles.RL, report.Violations[0].Profile);
        Assert.IsTrue(report.Violations[0].RuleName.EndsWith(".ObjectPropertyAxiomType"));
    }

    //TransitiveObjectProperty is admitted by EL and RL but excluded by QL (§3.2.5) — same rationale as above,
    //picked so this test can only pass if the dispatch actually reached OWLQLProfile specifically.
    [TestMethod]
    public async Task ShouldSurfaceQLViolationThroughPublicEntryPointAsync()
    {
        OWLObjectProperty ownedBy = new OWLObjectProperty(new RDFResource("http://pets.org/ownedBy"));
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [new OWLTransitiveObjectProperty(ownedBy)]
        };

        OWLProfileReport report = await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.QL);

        Assert.IsFalse(report.IsCompliant);
        Assert.HasCount(1, report.Violations);
        Assert.AreEqual(OWLEnums.OWLProfiles.QL, report.Violations[0].Profile);
        Assert.IsTrue(report.Violations[0].RuleName.EndsWith(".ObjectPropertyAxiomType"));
    }

    //CheckProfilesAsync's other tests above only ever see an empty (trivially triple-compliant) ontology: this
    //exercises it with an ontology that is GENUINELY non-compliant with two profiles and compliant with the
    //third, confirming the three-report dictionary reflects real, differing per-profile outcomes rather than
    //three copies of the same (always-empty) violation list.
    [TestMethod]
    public async Task ShouldSurfaceMixedComplianceThroughCheckProfilesAsync()
    {
        //FunctionalObjectProperty: excluded by EL (§2.2.5) and QL (§3.2.5) alike — both would let a reasoner
        //derive SameIndividual facts, breaking FO-rewritability/tractability — but explicitly admitted by RL
        //(§4.2.5), which has no trouble with property functionality under its rule-based semantics.
        OWLObjectProperty hasBiologicalMother = new OWLObjectProperty(new RDFResource("http://pets.org/hasBiologicalMother"));
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [new OWLFunctionalObjectProperty(hasBiologicalMother)]
        };

        Dictionary<OWLEnums.OWLProfiles, OWLProfileReport> reports = await ontology.CheckProfilesAsync();

        Assert.IsFalse(reports[OWLEnums.OWLProfiles.EL].IsCompliant);
        Assert.HasCount(1, reports[OWLEnums.OWLProfiles.EL].Violations);

        Assert.IsTrue(reports[OWLEnums.OWLProfiles.RL].IsCompliant);

        Assert.IsFalse(reports[OWLEnums.OWLProfiles.QL].IsCompliant);
        Assert.HasCount(1, reports[OWLEnums.OWLProfiles.QL].Violations);
    }
    #endregion
}