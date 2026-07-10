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

namespace OWLSharp.Test.Profiler.Walkers;

[TestClass]
public class OWLPropertyAxiomWalkerTest
{
    #region Tests

    //A small realistic "library" TBox exercising all three axiom types WalkPropertyDomainRangeClassExpressions
    //covers at once: ObjectPropertyDomain, ObjectPropertyRange, and DataPropertyDomain — each carrying a plain
    //atomic class, which is enough to verify the walker surfaces the right (axiom, classExpression) pair per
    //axiom without needing composite expressions here (those are exercised by the profilers' own recursive checks).
    private static OWLOntology BuildLibraryOntology(
        out OWLClass book, out OWLClass author, out OWLClass isbnHolder,
        out OWLObjectPropertyDomain writtenByDomain, out OWLObjectPropertyRange writtenByRange,
        out OWLDataPropertyDomain hasIsbnDomain, out OWLDataPropertyRange hasIsbnRange)
    {
        book = new OWLClass(new RDFResource("http://library.org/Book"));
        author = new OWLClass(new RDFResource("http://library.org/Author"));
        isbnHolder = new OWLClass(new RDFResource("http://library.org/IsbnHolder"));
        OWLObjectProperty writtenBy = new OWLObjectProperty(new RDFResource("http://library.org/writtenBy"));
        OWLDataProperty hasIsbn = new OWLDataProperty(new RDFResource("http://library.org/hasIsbn"));

        writtenByDomain = new OWLObjectPropertyDomain(writtenBy, book);
        writtenByRange = new OWLObjectPropertyRange(writtenBy, author);
        hasIsbnDomain = new OWLDataPropertyDomain(hasIsbn, isbnHolder);
        hasIsbnRange = new OWLDataPropertyRange(hasIsbn, new OWLDatatype(RDFVocabulary.XSD.STRING));

        return new OWLOntology
        {
            ObjectPropertyAxioms = [writtenByDomain, writtenByRange],
            DataPropertyAxioms = [hasIsbnDomain, hasIsbnRange]
        };
    }

    [TestMethod]
    public void ShouldWalkClassExpressionsFromDomainRangeAxioms()
    {
        OWLOntology ontology = BuildLibraryOntology(
            out OWLClass book, out OWLClass author, out OWLClass isbnHolder,
            out OWLObjectPropertyDomain writtenByDomain, out OWLObjectPropertyRange writtenByRange,
            out OWLDataPropertyDomain hasIsbnDomain, out _);

        List<(OWLAxiom Axiom, OWLClassExpression ClassExpression)> slots =
            OWLPropertyAxiomWalker.WalkPropertyDomainRangeClassExpressions(ontology).ToList();

        //ObjectPropertyDomain + ObjectPropertyRange + DataPropertyDomain = 3 slots (DataPropertyRange is
        //deliberately NOT walked here: it carries a data range, not a class expression — see the other test below).
        Assert.HasCount(3, slots);
        Assert.IsTrue(slots.Any(s => ReferenceEquals(s.Axiom, writtenByDomain) && ReferenceEquals(s.ClassExpression, book)));
        Assert.IsTrue(slots.Any(s => ReferenceEquals(s.Axiom, writtenByRange) && ReferenceEquals(s.ClassExpression, author)));
        Assert.IsTrue(slots.Any(s => ReferenceEquals(s.Axiom, hasIsbnDomain) && ReferenceEquals(s.ClassExpression, isbnHolder)));
    }

    [TestMethod]
    public void ShouldWalkDataRangesFromDataPropertyRangeAxiomsOnly()
    {
        OWLOntology ontology = BuildLibraryOntology(
            out _, out _, out _,
            out _, out _, out _, out OWLDataPropertyRange hasIsbnRange);

        List<(OWLAxiom Axiom, OWLDataRangeExpression DataRangeExpression)> slots =
            OWLPropertyAxiomWalker.WalkPropertyRangeDataRanges(ontology).ToList();

        //Only DataPropertyRange carries a data range directly; ObjectPropertyDomain/Range and DataPropertyDomain
        //(present in this same ontology) must NOT leak into this walk — it is a strictly separate expression family.
        Assert.HasCount(1, slots);
        Assert.IsTrue(ReferenceEquals(slots[0].Axiom, hasIsbnRange));
        Assert.IsTrue(ReferenceEquals(slots[0].DataRangeExpression, hasIsbnRange.DataRangeExpression));
    }

    [TestMethod]
    public void ShouldReturnNoSlotsOnOntologyWithoutPropertyDomainRangeAxioms()
    {
        OWLOntology ontology = new OWLOntology();

        Assert.IsEmpty(OWLPropertyAxiomWalker.WalkPropertyDomainRangeClassExpressions(ontology).ToList());
        Assert.IsEmpty(OWLPropertyAxiomWalker.WalkPropertyRangeDataRanges(ontology).ToList());
    }

    //Corner case: an ontology whose property axioms are all of a kind this walker does NOT cover (e.g.
    //FunctionalObjectProperty, which carries no domain/range class expression at all) must yield zero slots,
    //not throw or silently misinterpret the axiom.
    [TestMethod]
    public void ShouldIgnorePropertyAxiomTypesOutsideItsScope()
    {
        OWLObjectProperty knows = new OWLObjectProperty(new RDFResource("http://library.org/knows"));
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [new OWLFunctionalObjectProperty(knows)]
        };

        Assert.IsEmpty(OWLPropertyAxiomWalker.WalkPropertyDomainRangeClassExpressions(ontology).ToList());
    }
    #endregion
}
