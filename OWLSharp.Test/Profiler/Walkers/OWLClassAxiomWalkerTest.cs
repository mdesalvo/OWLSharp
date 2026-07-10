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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Profiler;
using RDFSharp.Model;

namespace OWLSharp.Test.Profiler;

[TestClass]
public class OWLClassAxiomWalkerTest
{
    #region Tests

    //Small but realistic "university" TBox, built so that every branch of WalkClassAxioms is exercised at once:
    //  - SubClassOf with two plain atomic classes on both sides (Professor/Person, Student/Person)
    //  - SubClassOf with a composite class expression on the SuperClass side (Professor sub ∃teaches.Course),
    //    the exact shape a real profile predicate will later have to recurse into
    //  - EquivalentClasses mixing an atomic class and a composite one (Person ≡ Professor ∪ Student)
    //  - DisjointClasses with three members (Professor, Student, Course), to check the walker does not
    //    stop at a pairwise reading of disjointness
    private static OWLOntology BuildUniversityOntology(
        out OWLClass person, out OWLClass professor, out OWLClass student, out OWLClass course,
        out OWLSubClassOf professorSubPerson, out OWLSubClassOf studentSubPerson, out OWLSubClassOf professorTeachesCourse,
        out OWLEquivalentClasses personEquivUnion, out OWLDisjointClasses disjointTriple,
        out OWLObjectSomeValuesFrom teachesCourseRestriction, out OWLObjectUnionOf professorOrStudentUnion)
    {
        person = new OWLClass(new RDFResource("http://university.org/Person"));
        professor = new OWLClass(new RDFResource("http://university.org/Professor"));
        student = new OWLClass(new RDFResource("http://university.org/Student"));
        course = new OWLClass(new RDFResource("http://university.org/Course"));
        OWLObjectProperty teaches = new OWLObjectProperty(new RDFResource("http://university.org/teaches"));

        professorSubPerson = new OWLSubClassOf(professor, person);
        studentSubPerson = new OWLSubClassOf(student, person);

        teachesCourseRestriction = new OWLObjectSomeValuesFrom(teaches, course);
        professorTeachesCourse = new OWLSubClassOf(professor, teachesCourseRestriction);

        professorOrStudentUnion = new OWLObjectUnionOf([professor, student]);
        personEquivUnion = new OWLEquivalentClasses([person, professorOrStudentUnion]);

        disjointTriple = new OWLDisjointClasses([professor, student, course]);

        return new OWLOntology
        {
            ClassAxioms = [professorSubPerson, studentSubPerson, professorTeachesCourse, personEquivUnion, disjointTriple]
        };
    }

    [TestMethod]
    public void ShouldWalkAllClassExpressionSlotsOfRealisticOntology()
    {
        OWLOntology ontology = BuildUniversityOntology(
            out _, out OWLClass professor, out OWLClass student, out _,
            out OWLSubClassOf professorSubPerson, out OWLSubClassOf studentSubPerson, out OWLSubClassOf professorTeachesCourse,
            out OWLEquivalentClasses personEquivUnion, out OWLDisjointClasses disjointTriple,
            out OWLObjectSomeValuesFrom teachesCourseRestriction, out OWLObjectUnionOf professorOrStudentUnion);

        List<(OWLClassAxiom Axiom, OWLClassExpression ClassExpression, OWLEnums.OWLClassExpressionPosition Position)> slots =
            OWLClassAxiomWalker.WalkClassAxioms(ontology).ToList();

        //3 SubClassOf axioms * 2 slots + 1 EquivalentClasses * 2 members + 1 DisjointClasses * 3 members = 11
        Assert.HasCount(11, slots);

        //Every SubClassOf axiom must contribute exactly one SubClass-tagged and one SuperClass-tagged slot,
        //referencing back to the very same axiom instance (not a copy), so a later profile predicate can
        //attach a violation to the correct axiom.
        foreach (OWLSubClassOf subClassOf in new[] { professorSubPerson, studentSubPerson, professorTeachesCourse })
        {
            Assert.IsTrue(slots.Any(s => ReferenceEquals(s.Axiom, subClassOf) && ReferenceEquals(s.ClassExpression, subClassOf.SubClassExpression)
                                          && s.Position == OWLEnums.OWLClassExpressionPosition.SubClass));
            Assert.IsTrue(slots.Any(s => ReferenceEquals(s.Axiom, subClassOf) && ReferenceEquals(s.ClassExpression, subClassOf.SuperClassExpression)
                                          && s.Position == OWLEnums.OWLClassExpressionPosition.SuperClass));
        }

        //The composite filler of the existential restriction (∃teaches.Course) must surface tagged SuperClass,
        //exactly like a plain atomic class would: the walker must not special-case composite expressions,
        //recursion into them is entirely the profile predicate's job, not the walker's.
        Assert.IsTrue(slots.Any(s => ReferenceEquals(s.ClassExpression, teachesCourseRestriction) && s.Position == OWLEnums.OWLClassExpressionPosition.SuperClass));

        //EquivalentClasses members use the dedicated EquivalentClass position, never SubClass/SuperClass,
        //for both the atomic member (Person) and the composite one (Professor ∪ Student).
        Assert.IsTrue(slots.Any(s => ReferenceEquals(s.Axiom, personEquivUnion) && s.ClassExpression.GetIRI().Equals(new RDFResource("http://university.org/Person"))
                                      && s.Position == OWLEnums.OWLClassExpressionPosition.EquivalentClass));
        Assert.IsTrue(slots.Any(s => ReferenceEquals(s.Axiom, personEquivUnion) && ReferenceEquals(s.ClassExpression, professorOrStudentUnion)
                                      && s.Position == OWLEnums.OWLClassExpressionPosition.EquivalentClass));

        //All three DisjointClasses members are tagged SubClass (there is no dedicated "disjoint" position
        //in the spec grammar), and none of them leaks into SuperClass or EquivalentClass.
        List<(OWLClassAxiom Axiom, OWLClassExpression ClassExpression, OWLEnums.OWLClassExpressionPosition Position)> disjointSlots =
            slots.Where(s => ReferenceEquals(s.Axiom, disjointTriple)).ToList();
        Assert.HasCount(3, disjointSlots);
        Assert.IsTrue(disjointSlots.TrueForAll(s => s.Position == OWLEnums.OWLClassExpressionPosition.SubClass));
        Assert.IsTrue(disjointSlots.Any(s => ReferenceEquals(s.ClassExpression, professor)));
        Assert.IsTrue(disjointSlots.Any(s => ReferenceEquals(s.ClassExpression, student)));
    }

    [TestMethod]
    public void ShouldReturnNoSlotsOnOntologyWithoutClassAxioms()
    {
        OWLOntology ontology = new OWLOntology();

        List<(OWLClassAxiom Axiom, OWLClassExpression ClassExpression, OWLEnums.OWLClassExpressionPosition Position)> slots =
            OWLClassAxiomWalker.WalkClassAxioms(ontology).ToList();

        Assert.IsEmpty(slots);
    }

    //Corner case: an ontology whose ClassAxioms list mixes an unrelated axiom kind (DisjointUnion, which is
    //excluded from every OWL2 profile's grammar and is therefore NOT walked here at all - it is an
    //axiom-type-level exclusion, to be enforced by each profile's own axiom-type check, not by this walker).
    [TestMethod]
    public void ShouldIgnoreClassAxiomTypesOutsideItsScope()
    {
        OWLClass vehicle = new OWLClass(new RDFResource("http://example.org/Vehicle"));
        OWLClass car = new OWLClass(new RDFResource("http://example.org/Car"));
        OWLClass truck = new OWLClass(new RDFResource("http://example.org/Truck"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLDisjointUnion(vehicle, [car, truck])]
        };

        List<(OWLClassAxiom Axiom, OWLClassExpression ClassExpression, OWLEnums.OWLClassExpressionPosition Position)> slots =
            OWLClassAxiomWalker.WalkClassAxioms(ontology).ToList();

        Assert.IsEmpty(slots);
    }
    #endregion
}