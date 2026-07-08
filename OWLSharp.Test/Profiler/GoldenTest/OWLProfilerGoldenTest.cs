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

namespace OWLSharp.Test.Profiler.GoldenTest;

/// <summary>
/// OWLProfilerGoldenTest is a cross-profile regression anchor: it takes the three "realistic, single-profile"
/// ontologies already used as fixtures in OWLELProfileTest/OWLRLProfileTest/OWLQLProfileTest and checks EACH of
/// them against ALL THREE profiles via CheckProfilesAsync, not just its own "home" profile.
/// </summary>
/// <remarks>
/// Why this exists, and why it is NOT anchored to the W3C spec's own example ontologies: the OWL 2 Profiles
/// document's introductory sections (§2.1 EL, §3.1 QL, §4.1 RL) were checked (via a dedicated fetch) for
/// illustrative sample axioms to use as golden fixtures, and contain none — only prose describing each
/// profile's target use case (SNOMED CT for EL, UML/ER-style conceptual models for QL, RDFS-plus-a-bit for RL),
/// with the actual grammar tables being the only normative content. Downloading and parsing a real external
/// OWL corpus (an actual SNOMED CT fragment, a real DL-Lite/OBDA ontology, an actual RL rule-set ontology) was
/// considered and rejected: this environment's web-fetch tool summarizes HTML through a text model, which is
/// not a reliable way to byte-faithfully reconstruct OWL axioms — a corrupted "golden" fixture would be worse
/// than no golden fixture at all, silently validating the checker against the wrong expected answer.
/// Instead, this file re-purposes the three fixtures already individually verified against the real,
/// re-checked spec grammar (see OWLELProfile/OWLRLProfile/OWLQLProfile's XML-doc and the owl2_profiles_w3c_spec
/// memory note) as a cross-profile classification matrix: each fixture's violation count against the OTHER two
/// profiles is derived by hand, axiom-by-axiom, from the same grammar tables the checkers themselves implement
/// — which is exactly what a golden/reference test should do (an independently-derived expected answer, not a
/// tautological "assert whatever the code currently returns").
/// </remarks>
[TestClass]
public class OWLProfilerGoldenTest
{
    #region Tests

    //Mirrors OWLELProfileTest.BuildInfectiousDiseaseOntology exactly (kept as a private copy here, rather than
    //shared, since MSTest test classes cannot see each other's private helpers and a public shared fixture
    //builder would be overkill for three small ontologies used in exactly two places each).
    private static OWLOntology BuildInfectiousDiseaseOntology()
    {
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLClass infectiousDisease = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass symptom = new OWLClass(new RDFResource("http://biomed.org/Symptom"));
        OWLClass fever = new OWLClass(new RDFResource("http://biomed.org/Fever"));
        OWLObjectProperty hasSymptom = new OWLObjectProperty(new RDFResource("http://biomed.org/hasSymptom"));

        return new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(fever, symptom),
                new OWLSubClassOf(infectiousDisease, disease),
                new OWLSubClassOf(infectiousDisease, new OWLObjectSomeValuesFrom(hasSymptom, fever))
            ],
            ObjectPropertyAxioms = [
                new OWLTransitiveObjectProperty(hasSymptom)
            ]
        };
    }

    //Mirrors OWLRLProfileTest.BuildOrderFulfillmentOntology (the FIXED version, existential on the subclass side).
    private static OWLOntology BuildOrderFulfillmentOntology()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLClass priorityOrder = new OWLClass(new RDFResource("http://shop.org/PriorityOrder"));
        OWLClass customer = new OWLClass(new RDFResource("http://shop.org/Customer"));
        OWLClass product = new OWLClass(new RDFResource("http://shop.org/Product"));
        OWLObjectProperty placedBy = new OWLObjectProperty(new RDFResource("http://shop.org/placedBy"));

        return new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(priorityOrder, order),
                new OWLSubClassOf(new OWLObjectSomeValuesFrom(placedBy, customer), order),
                new OWLDisjointClasses([order, customer, product])
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyDomain(placedBy, order),
                new OWLObjectPropertyRange(placedBy, customer),
                new OWLFunctionalObjectProperty(placedBy)
            ]
        };
    }

    //Mirrors OWLQLProfileTest.BuildEmployeeDepartmentOntology.
    private static OWLOntology BuildEmployeeDepartmentOntology()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));

        return new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(manager, employee)
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyDomain(worksFor, employee),
                new OWLObjectPropertyRange(worksFor, department)
            ]
        };
    }

    //The EL fixture's only "interesting" (non-atomic) constructs are an ObjectSomeValuesFrom in superclass
    //position (SubClassOf(InfectiousDisease, ∃hasSymptom.Fever)) and a TransitiveObjectProperty axiom.
    //Expected against each profile, derived by hand from the grammar tables (not from running the code):
    //  - EL: 0 violations (both constructs are squarely inside EL's own grammar — that is the fixture's purpose).
    //  - RL: 1 violation. RL's superClassExpression grammar (§4.2.3) does NOT list ObjectSomeValuesFrom at all
    //    (only subClassExpression does) — so ∃hasSymptom.Fever sitting on the CONSEQUENT side of a SubClassOf
    //    is out of grammar for RL, independent of the TransitiveObjectProperty axiom (which RL freely admits).
    //  - QL: 1 violation. The existential is FINE for QL here — §3.2.3's superObjectSomeValuesFrom production
    //    admits exactly "ObjectSomeValuesFrom(OPE, Class)" with an atomic filler, and Fever is atomic — but
    //    TransitiveObjectProperty is one of the axiom types QL's §3.2.5 explicitly excludes (transitivity breaks
    //    FO-rewritability), so that single axiom is QL's one violation.
    [TestMethod]
    public async Task ShouldClassifyInfectiousDiseaseOntologyAcrossAllProfilesAsync()
    {
        OWLOntology ontology = BuildInfectiousDiseaseOntology();

        Dictionary<OWLEnums.OWLProfiles, OWLProfileReport> reports = await ontology.CheckProfilesAsync();

        Assert.IsTrue(reports[OWLEnums.OWLProfiles.EL].IsCompliant);

        Assert.IsFalse(reports[OWLEnums.OWLProfiles.RL].IsCompliant);
        Assert.HasCount(1, reports[OWLEnums.OWLProfiles.RL].Violations);
        Assert.Contains("superclass position", reports[OWLEnums.OWLProfiles.RL].Violations[0].Description);

        Assert.IsFalse(reports[OWLEnums.OWLProfiles.QL].IsCompliant);
        Assert.HasCount(1, reports[OWLEnums.OWLProfiles.QL].Violations);
        Assert.Contains("OWLTransitiveObjectProperty", reports[OWLEnums.OWLProfiles.QL].Violations[0].Description);
    }

    //The RL fixture's "interesting" constructs are an ObjectSomeValuesFrom in SUBclass position
    //(SubClassOf(∃placedBy.Customer, Order)) and a FunctionalObjectProperty axiom. Expected, again derived
    //by hand from the grammar tables:
    //  - RL: 0 violations (both constructs are squarely inside RL's own grammar — the fixture's purpose).
    //  - EL: 1 violation. EL has no sub/superclass position distinction at all, so the existential is fine
    //    regardless of which side of SubClassOf it sits on — but FunctionalObjectProperty is one of the object
    //    property characteristics EL's §2.2.5 excludes (it would let a reasoner derive SameIndividual facts,
    //    breaking EL's tractability guarantee), so that is EL's one violation.
    //  - QL: 2 violations. The existential is a QL violation here — §3.2.3's subObjectSomeValuesFrom production
    //    only admits an UNqualified filler (owl:Thing), and Customer is a qualified (atomic-but-not-owl:Thing)
    //    filler, which only §3.2.3's SUPERclass production would admit. On top of that, FunctionalObjectProperty
    //    is excluded from QL for the same FO-rewritability reason it is excluded from EL.
    [TestMethod]
    public async Task ShouldClassifyOrderFulfillmentOntologyAcrossAllProfilesAsync()
    {
        OWLOntology ontology = BuildOrderFulfillmentOntology();

        Dictionary<OWLEnums.OWLProfiles, OWLProfileReport> reports = await ontology.CheckProfilesAsync();

        Assert.IsTrue(reports[OWLEnums.OWLProfiles.RL].IsCompliant);

        Assert.IsFalse(reports[OWLEnums.OWLProfiles.EL].IsCompliant);
        Assert.HasCount(1, reports[OWLEnums.OWLProfiles.EL].Violations);
        Assert.Contains("OWLFunctionalObjectProperty", reports[OWLEnums.OWLProfiles.EL].Violations[0].Description);

        Assert.IsFalse(reports[OWLEnums.OWLProfiles.QL].IsCompliant);
        Assert.HasCount(2, reports[OWLEnums.OWLProfiles.QL].Violations);
    }

    //The QL fixture deliberately uses ONLY the subset every profile agrees on (a plain atomic SubClassOf plus
    //ObjectPropertyDomain/Range on atomic classes): no existentials, no property characteristics beyond the
    //bare minimum, nothing profile-specific at all. Expected: compliant with EL, RL AND QL simultaneously —
    //a positive example of the (non-empty) three-way overlap between the profiles, not just of QL's own grammar.
    [TestMethod]
    public async Task ShouldClassifyEmployeeDepartmentOntologyAsCompliantWithEveryProfileAsync()
    {
        OWLOntology ontology = BuildEmployeeDepartmentOntology();

        Dictionary<OWLEnums.OWLProfiles, OWLProfileReport> reports = await ontology.CheckProfilesAsync();

        Assert.IsTrue(reports[OWLEnums.OWLProfiles.EL].IsCompliant);
        Assert.IsTrue(reports[OWLEnums.OWLProfiles.RL].IsCompliant);
        Assert.IsTrue(reports[OWLEnums.OWLProfiles.QL].IsCompliant);
    }
    #endregion
}