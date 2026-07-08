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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Profiler;
using RDFSharp.Model;

namespace OWLSharp.Test.Profiler.RuleSet;

[TestClass]
public class OWLQLProfileTest
{
    #region Tests

    //QL's textbook realm is OBDA/data integration (DL-Lite): thin TBoxes over large relational-like ABoxes,
    //answered via query rewriting. Fully QL-compliant on purpose: both ObjectPropertyDomain/Range use plain
    //atomic classes (the only shape admitted in superclass position for a bare Class, no existential involved),
    //and the single SubClassOf uses atomic classes on both sides.
    private static OWLOntology BuildEmployeeDepartmentOntology()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));

        return new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(employee), new OWLDeclaration(manager),
                new OWLDeclaration(department), new OWLDeclaration(worksFor)
            ],
            ClassAxioms = [
                new OWLSubClassOf(manager, employee)
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyDomain(worksFor, employee),
                new OWLObjectPropertyRange(worksFor, department)
            ]
        };
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnRealisticOntologyAsync()
    {
        OWLOntology ontology = BuildEmployeeDepartmentOntology();

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsNotNull(violations);
        //Real compliance assertion (not a stub baseline): every check implemented in OWLQLProfile must agree
        //this deliberately simple, atomic-classes-only fixture is QL-compliant.
        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnEmptyOntologyAsync()
    {
        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(new OWLOntology());

        Assert.IsNotNull(violations);
        Assert.IsEmpty(violations);
    }

    //--- The core qualification asymmetry: ObjectSomeValuesFrom's filler shape depends on position -------------

    [TestMethod]
    public async Task ShouldAllowUnqualifiedObjectSomeValuesFromInSubClassPositionAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLOntology ontology = new OWLOntology
        {
            //"Anyone who works for something is an Employee": the unqualified existential ∃worksFor.owl:Thing
            //is exactly the shape §3.2.3's subObjectSomeValuesFrom production admits on the antecedent side.
            ClassAxioms = [new OWLSubClassOf(new OWLObjectSomeValuesFrom(worksFor, new OWLClass(RDFVocabulary.OWL.THING)), employee)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //The mistake a DL-Lite newcomer (or an EL/RL-trained intuition) would naturally make: qualifying the
    //existential with an actual class, which is perfectly fine in EL/RL's subclass position but NOT in QL's.
    [TestMethod]
    public async Task ShouldFlagQualifiedObjectSomeValuesFromInSubClassPositionAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLOntology ontology = new OWLOntology
        {
            //"Anyone who works for a Department is an Employee": ∃worksFor.Department is qualified, which
            //§3.2.3 only admits in SUPERclass position — see the compliant mirror-image test below.
            ClassAxioms = [new OWLSubClassOf(new OWLObjectSomeValuesFrom(worksFor, department), employee)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ClassExpression"));
        Assert.Contains("must be filled with owl:Thing", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowQualifiedObjectSomeValuesFromInSuperClassPositionAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLOntology ontology = new OWLOntology
        {
            //Mirror image of the violation above, with the SAME axiom shape but flipped sides: "Every Employee
            //works for some Department" (∃worksFor.Department on the CONSEQUENT side) is exactly what §3.2.3's
            //superObjectSomeValuesFrom production admits.
            ClassAxioms = [new OWLSubClassOf(employee, new OWLObjectSomeValuesFrom(worksFor, department))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //owl:Thing is itself a plain atomic Class, so it is NOT excluded as a superclass-position existential
    //filler the way it would be excluded as a plain atomic superClassExpression in RL (QL has no such
    //"Class other than owl:Thing" restriction anywhere in its grammar — see CheckClassExpression's comment).
    [TestMethod]
    public async Task ShouldAllowOwlThingAsObjectSomeValuesFromFillerInSuperClassPositionAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(employee, new OWLObjectSomeValuesFrom(worksFor, new OWLClass(RDFVocabulary.OWL.THING)))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //The second half of the qualification asymmetry: even in superclass position, where a qualified filler IS
    //admitted, the filler must be a single ATOMIC class — §3.2.3's production is literally typed "Class", not
    //"superClassExpression", so a composite filler (even one that is otherwise perfectly admitted in superclass
    //position on its own, like ObjectIntersectionOf) is still a violation here.
    [TestMethod]
    public async Task ShouldFlagCompositeFillerInSuperClassObjectSomeValuesFromAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLClass remoteFriendly = new OWLClass(new RDFResource("http://corp.org/RemoteFriendly"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(employee, new OWLObjectSomeValuesFrom(worksFor, new OWLObjectIntersectionOf([department, remoteFriendly])))
            ]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("single atomic Class", violations[0].Description);
    }

    //--- Position asymmetry for the other constructs ----------------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagObjectIntersectionOfInSubClassPositionAsync()
    {
        OWLClass seniorManager = new OWLClass(new RDFResource("http://corp.org/SeniorManager"));
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass tenured = new OWLClass(new RDFResource("http://corp.org/Tenured"));
        OWLOntology ontology = new OWLOntology
        {
            //Contrast with RL, which admits ObjectIntersectionOf on BOTH sides: QL's subClassExpression grammar
            //has no conjunction production at all — only Class, the unqualified existential, or DataSomeValuesFrom.
            ClassAxioms = [new OWLSubClassOf(new OWLObjectIntersectionOf([manager, tenured]), seniorManager)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectIntersectionOf", violations[0].Description);
        Assert.Contains("subclass position", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowObjectIntersectionOfInSuperClassPositionAsync()
    {
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass approver = new OWLClass(new RDFResource("http://corp.org/Approver"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(manager, new OWLObjectIntersectionOf([employee, approver]))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowObjectComplementOfWrappingCompliantSubClassExpressionAsync()
    {
        OWLClass activeEmployee = new OWLClass(new RDFResource("http://corp.org/ActiveEmployee"));
        OWLClass formerEmployee = new OWLClass(new RDFResource("http://corp.org/FormerEmployee"));
        OWLOntology ontology = new OWLOntology
        {
            //ObjectComplementOf is admitted ONLY in superclass position, same as RL, with its operand checked
            //against the SUBclass grammar via polarity inversion.
            ClassAxioms = [new OWLSubClassOf(activeEmployee, new OWLObjectComplementOf(formerEmployee))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldFlagObjectComplementOfInSubClassPositionAsync()
    {
        OWLClass formerEmployee = new OWLClass(new RDFResource("http://corp.org/FormerEmployee"));
        OWLClass person = new OWLClass(new RDFResource("http://corp.org/Person"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectComplementOf(formerEmployee), person)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectComplementOf", violations[0].Description);
        Assert.Contains("subclass position", violations[0].Description);
    }

    //Polarity-inversion corner case, structurally identical to RL's (see OWLRLProfileTest for the analogous
    //test): ObjectComplementOf's operand must satisfy the SUBclass grammar, so a filler that is only valid in
    //superclass position (a qualified ObjectSomeValuesFrom) must still be flagged once the position flips.
    [TestMethod]
    public async Task ShouldFlagSuperClassOnlyConstructNestedInsideObjectComplementOfAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(employee, new OWLObjectComplementOf(new OWLObjectSomeValuesFrom(worksFor, department)))
            ]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        //After the position flip, the qualified filler is checked against subClassExpression, which only
        //admits owl:Thing as an ObjectSomeValuesFrom filler — hence the same message as the plain
        //"qualified existential in subclass position" violation above.
        Assert.Contains("must be filled with owl:Thing", violations[0].Description);
    }

    //ObjectUnionOf has no rendering in QL's grammar AT ALL — unlike RL, which admits it (subclass-only): QL's
    //FO-rewritability requirement rules out disjunction entirely, on either side.
    [TestMethod]
    public async Task ShouldFlagObjectUnionOfAsync()
    {
        OWLClass contractor = new OWLClass(new RDFResource("http://corp.org/Contractor"));
        OWLClass freelancer = new OWLClass(new RDFResource("http://corp.org/Freelancer"));
        OWLClass consultant = new OWLClass(new RDFResource("http://corp.org/Consultant"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectUnionOf([freelancer, consultant]), contractor)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectUnionOf", violations[0].Description);
    }

    //ObjectOneOf (nominals) is not part of QL's grammar at all — contrast with EL's singleton-only version and
    //RL's any-size version, QL admits neither.
    [TestMethod]
    public async Task ShouldFlagObjectOneOfAsync()
    {
        OWLClass ceoRole = new OWLClass(new RDFResource("http://corp.org/CeoRole"));
        OWLNamedIndividual aliceCeo = new OWLNamedIndividual(new RDFResource("http://corp.org/AliceCeo"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectOneOf([aliceCeo]), ceoRole)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectOneOf", violations[0].Description);
    }

    //ObjectHasValue is admitted by both EL and RL, but has NO rendering anywhere in QL's grammar (neither
    //subClassExpression nor superClassExpression lists it) — a genuine three-way split worth pinning down.
    [TestMethod]
    public async Task ShouldFlagObjectHasValueAsync()
    {
        OWLClass parisOffice = new OWLClass(new RDFResource("http://corp.org/ParisOffice"));
        OWLObjectProperty locatedIn = new OWLObjectProperty(new RDFResource("http://corp.org/locatedIn"));
        OWLNamedIndividual paris = new OWLNamedIndividual(new RDFResource("http://corp.org/Paris"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(parisOffice, new OWLObjectHasValue(locatedIn, paris))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectHasValue", violations[0].Description);
    }

    //Cardinality restrictions have no QL rendering at all (contrast with RL, which admits ObjectMaxCardinality
    //at 0/1): counting would force a reasoner outside non-recursive datalog.
    [TestMethod]
    public async Task ShouldFlagObjectMaxCardinalityAsync()
    {
        OWLClass soleProprietor = new OWLClass(new RDFResource("http://corp.org/SoleProprietor"));
        OWLObjectProperty employs = new OWLObjectProperty(new RDFResource("http://corp.org/employs"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(soleProprietor, new OWLObjectMaxCardinality(employs, 0))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectMaxCardinality", violations[0].Description);
    }

    //--- EquivalentClasses folds onto the subClassExpression grammar, not a distinct equivCE (contrast with RL) ---

    [TestMethod]
    public async Task ShouldAllowAtomicClassesInEquivalentClassesAsync()
    {
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass teamLead = new OWLClass(new RDFResource("http://corp.org/TeamLead"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLEquivalentClasses([manager, teamLead])]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //Because QL folds EquivalentClasses members onto the SAME subClassExpression grammar used by SubClassOf's
    //antecedent side (not a separate, RL-style equivCE), a qualified existential is rejected here for the exact
    //same reason it would be rejected as a plain SubClassOf antecedent — worth confirming explicitly, since it
    //is easy to assume "equivalence" gets its own, possibly more permissive, grammar the way it does in RL.
    [TestMethod]
    public async Task ShouldFlagQualifiedObjectSomeValuesFromInEquivalentClassesAsync()
    {
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLObjectProperty leads = new OWLObjectProperty(new RDFResource("http://corp.org/leads"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLEquivalentClasses([manager, new OWLObjectSomeValuesFrom(leads, department)])]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("must be filled with owl:Thing", violations[0].Description);
    }

    //--- §3.2.4 DataRange grammar (same narrower shape as RL's: no DataOneOf) -----------------------------------

    [TestMethod]
    public async Task ShouldFlagDataOneOfInDataRangeAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLDataProperty hasStatus = new OWLDataProperty(new RDFResource("http://corp.org/hasStatus"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(new OWLDataSomeValuesFrom(hasStatus,
                    new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("active")), new OWLLiteral(new RDFPlainLiteral("onLeave"))])), employee)
            ]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".DataRange"));
        Assert.Contains("DataOneOf", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowDataIntersectionOfAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLDataProperty hasCode = new OWLDataProperty(new RDFResource("http://corp.org/hasCode"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(new OWLDataSomeValuesFrom(hasCode,
                    new OWLDataIntersectionOf([new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.TOKEN)])), employee)
            ]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- §3.2.1 datatype allowlist: shared with EL, deliberately different from RL's -----------------------------

    [TestMethod]
    public async Task ShouldFlagDisallowedDatatypeAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLDataProperty hasSalary = new OWLDataProperty(new RDFResource("http://corp.org/hasSalary"));
        OWLOntology ontology = new OWLOntology
        {
            //Same trap as OWLELProfileTest.ShouldFlagDisallowedDatatypeAsync: xsd:double is admitted by RL but
            //excluded from the EL/QL shared datatype map.
            ClassAxioms = [new OWLSubClassOf(new OWLDataSomeValuesFrom(hasSalary, new OWLDatatype(RDFVocabulary.XSD.DOUBLE)), employee)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".Datatype"));
    }

    [TestMethod]
    public async Task ShouldAllowOwlRealDatatypeAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLDataProperty hasSalary = new OWLDataProperty(new RDFResource("http://corp.org/hasSalary"));
        OWLOntology ontology = new OWLOntology
        {
            //Contrast with OWLRLProfileTest.ShouldFlagOwlRealDatatypeAsync: owl:real is excluded from RL but
            //admitted here, because QL's datatype map is literally EL's (see AllowedDatatypeIRIs' derivation).
            ClassAxioms = [new OWLSubClassOf(new OWLDataSomeValuesFrom(hasSalary, new OWLDatatype(new RDFResource($"{RDFVocabulary.OWL.BASE_URI}real"))), employee)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- §3.2.5 allowed axiom types ---------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagDisjointUnionClassAxiomAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass contractor = new OWLClass(new RDFResource("http://corp.org/Contractor"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLDisjointUnion(employee, [manager, contractor])]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ClassAxiomType"));
    }

    [TestMethod]
    public async Task ShouldFlagSubObjectPropertyOfWithPropertyChainAsync()
    {
        OWLObjectProperty hasGrandManager = new OWLObjectProperty(new RDFResource("http://corp.org/hasGrandManager"));
        OWLObjectProperty hasManager = new OWLObjectProperty(new RDFResource("http://corp.org/hasManager"));
        OWLOntology ontology = new OWLOntology
        {
            //"managed-by . managed-by ⊑ hasGrandManager": property chains are explicitly excluded from QL's
            //SubObjectPropertyOf production (§3.2.5), unlike EL ("EL++") and RL, which both admit them.
            ObjectPropertyAxioms = [new OWLSubObjectPropertyOf(new OWLObjectPropertyChain([hasManager, hasManager]), hasGrandManager)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("property chain", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowSubObjectPropertyOfWithoutChainAsync()
    {
        OWLObjectProperty manages = new OWLObjectProperty(new RDFResource("http://corp.org/manages"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [new OWLSubObjectPropertyOf(manages, worksFor)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldFlagTransitiveObjectPropertyAxiomAsync()
    {
        OWLObjectProperty managerOf = new OWLObjectProperty(new RDFResource("http://corp.org/managerOf"));
        OWLOntology ontology = new OWLOntology
        {
            //Contrast with EL and RL, which both admit TransitiveObjectProperty: QL excludes it, because a
            //transitive property can force unbounded chase-style reasoning that breaks FO-rewritability.
            ObjectPropertyAxioms = [new OWLTransitiveObjectProperty(managerOf)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("OWLTransitiveObjectProperty", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagFunctionalObjectPropertyAxiomAsync()
    {
        OWLObjectProperty hasBiologicalMother = new OWLObjectProperty(new RDFResource("http://corp.org/hasBiologicalMother"));
        OWLOntology ontology = new OWLOntology
        {
            //Same exclusion, same FO-rewritability rationale, as OWLELProfileTest's analogous test — but here
            //it is QL, not EL, forbidding it (RL is the only one of the three that admits it).
            ObjectPropertyAxioms = [new OWLFunctionalObjectProperty(hasBiologicalMother)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("OWLFunctionalObjectProperty", violations[0].Description);
    }

    //Consolidated coverage (one test, several axioms) of every object property characteristic QL DOES admit:
    //Reflexive/Symmetric/Asymmetric/Disjoint/Inverse, the same five RL admits alongside Functional/Transitive/
    //Irreflexive/InverseFunctional (which QL excludes, checked individually above/below).
    [TestMethod]
    public async Task ShouldAllowAdmittedObjectPropertyCharacteristicsAsync()
    {
        OWLObjectProperty manages = new OWLObjectProperty(new RDFResource("http://corp.org/manages"));
        OWLObjectProperty managedBy = new OWLObjectProperty(new RDFResource("http://corp.org/managedBy"));
        OWLObjectPropertyAxiom[] admittedAxioms =
        [
            new OWLReflexiveObjectProperty(manages),
            new OWLSymmetricObjectProperty(manages),
            new OWLAsymmetricObjectProperty(manages),
            new OWLDisjointObjectProperties([manages, managedBy]),
            new OWLInverseObjectProperties(manages, managedBy)
        ];

        foreach (OWLObjectPropertyAxiom axiom in admittedAxioms)
        {
            OWLOntology ontology = new OWLOntology { ObjectPropertyAxioms = [axiom] };

            List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

            Assert.IsEmpty(violations, $"expected no violation for {axiom.GetType().Name}");
        }
    }

    [TestMethod]
    public async Task ShouldFlagIrreflexiveAndInverseFunctionalObjectPropertyAxiomsAsync()
    {
        OWLObjectProperty manages = new OWLObjectProperty(new RDFResource("http://corp.org/manages"));
        (string Label, OWLObjectPropertyAxiom Axiom)[] excludedAxioms =
        [
            ("IrreflexiveObjectProperty", new OWLIrreflexiveObjectProperty(manages)),
            ("InverseFunctionalObjectProperty", new OWLInverseFunctionalObjectProperty(manages))
        ];

        foreach ((string label, OWLObjectPropertyAxiom axiom) in excludedAxioms)
        {
            OWLOntology ontology = new OWLOntology { ObjectPropertyAxioms = [axiom] };

            List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

            Assert.HasCount(1, violations, $"expected exactly one violation for {label}");
            Assert.IsTrue(violations[0].RuleName.EndsWith(".ObjectPropertyAxiomType"), $"unexpected rule name for {label}");
        }
    }

    [TestMethod]
    public async Task ShouldFlagFunctionalDataPropertyAxiomAsync()
    {
        OWLDataProperty hasSsn = new OWLDataProperty(new RDFResource("http://corp.org/hasSsn"));
        OWLOntology ontology = new OWLOntology
        {
            //Contrast with EL, which explicitly admits FunctionalDataProperty (§2.2.5): QL excludes it, unlike
            //EL, for the same FO-rewritability reason as FunctionalObjectProperty above.
            DataPropertyAxioms = [new OWLFunctionalDataProperty(hasSsn)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".DataPropertyAxiomType"));
    }

    [TestMethod]
    public async Task ShouldAllowDisjointDataPropertiesAxiomAsync()
    {
        OWLDataProperty hasFirstName = new OWLDataProperty(new RDFResource("http://corp.org/hasFirstName"));
        OWLDataProperty hasLastName = new OWLDataProperty(new RDFResource("http://corp.org/hasLastName"));
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [new OWLDisjointDataProperties([hasFirstName, hasLastName])]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- §3.2.5 assertions: a whole axiom family EL/RL's implementations never had to check --------------------

    [TestMethod]
    public async Task ShouldFlagSameIndividualAssertionAsync()
    {
        OWLNamedIndividual clarkKent = new OWLNamedIndividual(new RDFResource("http://corp.org/ClarkKent"));
        OWLNamedIndividual superman = new OWLNamedIndividual(new RDFResource("http://corp.org/Superman"));
        OWLOntology ontology = new OWLOntology
        {
            //SameIndividual would let two distinct query bindings collapse into one answer, which is exactly
            //the kind of extra reasoning power FO-rewriting cannot express.
            AssertionAxioms = [new OWLSameIndividual([clarkKent, superman])]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".AssertionAxiomType"));
        Assert.Contains("OWLSameIndividual", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagNegativeObjectPropertyAssertionAsync()
    {
        OWLObjectProperty manages = new OWLObjectProperty(new RDFResource("http://corp.org/manages"));
        OWLNamedIndividual alice = new OWLNamedIndividual(new RDFResource("http://corp.org/Alice"));
        OWLNamedIndividual bob = new OWLNamedIndividual(new RDFResource("http://corp.org/Bob"));
        OWLOntology ontology = new OWLOntology
        {
            //Negative assertions require closed-world reasoning to interpret meaningfully, which is incompatible
            //with QL's open-world, query-rewriting semantics.
            AssertionAxioms = [new OWLNegativeObjectPropertyAssertion(manages, alice, bob)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("OWLNegativeObjectPropertyAssertion", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagNegativeDataPropertyAssertionAsync()
    {
        OWLDataProperty hasSalary = new OWLDataProperty(new RDFResource("http://corp.org/hasSalary"));
        OWLNamedIndividual alice = new OWLNamedIndividual(new RDFResource("http://corp.org/Alice"));
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [new OWLNegativeDataPropertyAssertion(hasSalary, alice, new OWLLiteral(new RDFPlainLiteral("0")))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("OWLNegativeDataPropertyAssertion", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowDifferentIndividualsObjectAndDataPropertyAssertionsAsync()
    {
        OWLObjectProperty manages = new OWLObjectProperty(new RDFResource("http://corp.org/manages"));
        OWLDataProperty hasSalary = new OWLDataProperty(new RDFResource("http://corp.org/hasSalary"));
        OWLNamedIndividual alice = new OWLNamedIndividual(new RDFResource("http://corp.org/Alice"));
        OWLNamedIndividual bob = new OWLNamedIndividual(new RDFResource("http://corp.org/Bob"));
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [
                new OWLDifferentIndividuals([alice, bob]),
                new OWLObjectPropertyAssertion(manages, alice, bob),
                new OWLDataPropertyAssertion(hasSalary, alice, new OWLLiteral(new RDFPlainLiteral("50000")))
            ]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //ClassAssertion is admitted as an axiom TYPE by QL, but — per the second, independent verification fetch
    //of §3.2.5 — only when asserting membership against a plain atomic Class, never a composite expression.
    [TestMethod]
    public async Task ShouldFlagClassAssertionWithCompositeClassExpressionAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLNamedIndividual alice = new OWLNamedIndividual(new RDFResource("http://corp.org/Alice"));
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [new OWLClassAssertion(new OWLObjectSomeValuesFrom(worksFor, employee), alice)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("plain atomic Class", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowClassAssertionWithAtomicClassAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLNamedIndividual alice = new OWLNamedIndividual(new RDFResource("http://corp.org/Alice"));
        OWLOntology ontology = new OWLOntology
        {
            AssertionAxioms = [new OWLClassAssertion(employee, alice)]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //§3.2.5 explicitly excludes keys: a HasKey axiom (ontology.KeyAxioms, a list separate from AssertionAxioms
    //in this model) has no QL rendering regardless of which properties it keys on.
    [TestMethod]
    public async Task ShouldFlagHasKeyAxiomAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLDataProperty hasSsn = new OWLDataProperty(new RDFResource("http://corp.org/hasSsn"));
        OWLOntology ontology = new OWLOntology
        {
            KeyAxioms = [new OWLHasKey(employee, [hasSsn])]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("HasKey", violations[0].Description);
    }

    //--- Standalone coverage for expressions so far only exercised in the OTHER position -----------------------

    //DataSomeValuesFrom is admitted in BOTH sub and super position with the same shape (unlike the object
    //existential's qualification asymmetry) — every other DataSomeValuesFrom test in this file used it in
    //subclass position (e.g. ShouldAllowDataIntersectionOfAsync), so this closes the superclass-position gap.
    [TestMethod]
    public async Task ShouldAllowDataSomeValuesFromInSuperClassPositionAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLDataProperty hasBadgeCode = new OWLDataProperty(new RDFResource("http://corp.org/hasBadgeCode"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(employee, new OWLDataSomeValuesFrom(hasBadgeCode, new OWLDatatype(RDFVocabulary.XSD.STRING)))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- Class/data expressions embedded in property domain/range axioms (the walker's other two branches) -----
    //--- (ObjectPropertyDomain/Range are already exercised by the qualification tests above via SubClassOf; -----
    //--- these two specifically pin down DataPropertyDomain and DataPropertyRange, reached via the SAME shared ---
    //--- OWLPropertyAxiomWalker but never yet exercised for QL specifically.) ------------------------------------

    [TestMethod]
    public async Task ShouldFlagDisallowedClassExpressionInDataPropertyDomainAsync()
    {
        OWLDataProperty hasBadgeCode = new OWLDataProperty(new RDFResource("http://corp.org/hasBadgeCode"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLClass remoteFriendly = new OWLClass(new RDFResource("http://corp.org/RemoteFriendly"));
        OWLOntology ontology = new OWLOntology
        {
            //A qualified ObjectSomeValuesFrom with a composite filler is out of grammar wherever it appears,
            //including as a property's domain class.
            DataPropertyAxioms = [new OWLDataPropertyDomain(hasBadgeCode, new OWLObjectSomeValuesFrom(worksFor, new OWLObjectIntersectionOf([department, remoteFriendly])))]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("single atomic Class", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDisallowedDataRangeInDataPropertyRangeAsync()
    {
        OWLDataProperty hasBadgeCode = new OWLDataProperty(new RDFResource("http://corp.org/hasBadgeCode"));
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLDataPropertyRange(hasBadgeCode, new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("A")), new OWLLiteral(new RDFPlainLiteral("B"))]))
            ]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".DataRange"));
    }

    //--- Cross-cutting corner cases ---------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldReportMultipleIndependentViolationsInSameOntologyAsync()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass contractor = new OWLClass(new RDFResource("http://corp.org/Contractor"));
        OWLObjectProperty managerOf = new OWLObjectProperty(new RDFResource("http://corp.org/managerOf"));
        OWLNamedIndividual clarkKent = new OWLNamedIndividual(new RDFResource("http://corp.org/ClarkKent"));
        OWLNamedIndividual superman = new OWLNamedIndividual(new RDFResource("http://corp.org/Superman"));
        OWLOntology ontology = new OWLOntology
        {
            //A class-expression violation (union has no rendering), a class-axiom-type violation (DisjointUnion),
            //a property-axiom-type violation (Transitive) and an assertion-axiom-type violation (SameIndividual),
            //all unrelated to each other, thrown into the same ontology.
            ClassAxioms = [
                new OWLSubClassOf(new OWLObjectUnionOf([manager, contractor]), employee),
                new OWLDisjointUnion(employee, [manager, contractor])
            ],
            ObjectPropertyAxioms = [new OWLTransitiveObjectProperty(managerOf)],
            AssertionAxioms = [new OWLSameIndividual([clarkKent, superman])]
        };

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(4, violations);
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ClassExpression")));
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ClassAxiomType")));
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ObjectPropertyAxiomType")));
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".AssertionAxiomType")));
    }
    #endregion
}
