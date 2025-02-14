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
using OWLSharp.Extensions.TIME;
using OWLSharp.Ontology;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Extensions.TIME;

[TestClass]
public class TIMEOrdinalReferenceSystemTest
{
    internal static TIMEOrdinalReferenceSystem TestTRS { get; set; }

    [TestInitialize]
    public void InitializeAsync()
        => TestTRS ??= new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));

    #region Tests
    [TestMethod]
    public void ShouldTestInitialization()
    {
        //Test initialization
        Assert.IsNotNull(TestTRS);
        Assert.IsNotNull(TestTRS.Ontology);
        Assert.IsTrue(TestTRS.Equals(new RDFResource("ex:Thors")));
        Assert.AreEqual(3, TestTRS.Ontology.Imports.Count);
        Assert.AreEqual(5, TestTRS.Ontology.Prefixes.Count);
        Assert.IsTrue(TestTRS.Ontology.DeclarationAxioms.Count > 100);
        Assert.IsTrue(TestTRS.Ontology.AssertionAxioms.Count > 60);
        Assert.IsTrue(TestTRS.Ontology.ClassAxioms.Count > 60);
        Assert.IsTrue(TestTRS.Ontology.DataPropertyAxioms.Count > 20);
        Assert.IsTrue(TestTRS.Ontology.ObjectPropertyAxioms.Count > 100);

        //Test copy-ctor
        TIMEOrdinalReferenceSystem newTRS = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        Assert.IsNotNull(newTRS);
        Assert.IsNotNull(newTRS.Ontology);
        Assert.IsTrue(newTRS.Equals(new RDFResource("ex:Thors2")));
        Assert.AreEqual(3, newTRS.Ontology.Imports.Count);
        Assert.AreEqual(5, newTRS.Ontology.Prefixes.Count);
        Assert.IsTrue(newTRS.Ontology.DeclarationAxioms.Count > 100);
        Assert.IsTrue(newTRS.Ontology.AssertionAxioms.Count > 60);
        Assert.IsTrue(newTRS.Ontology.ClassAxioms.Count > 60);
        Assert.IsTrue(newTRS.Ontology.DataPropertyAxioms.Count > 20);
        Assert.IsTrue(newTRS.Ontology.ObjectPropertyAxioms.Count > 100);
        Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), null));
    }

    [TestMethod]
    public void ShouldDeclareEra()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"), 
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));

        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:era"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(new RDFResource("ex:era")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                new OWLNamedIndividual(new RDFResource("ex:era")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM),
                new OWLNamedIndividual(new RDFResource("ex:era")),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")))));

        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginning"))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginningPosition"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                new OWLNamedIndividual(new RDFResource("ex:era")),
                new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                new OWLNamedIndividual(new RDFResource("ex:eraBeginning")),
                new OWLNamedIndividual(new RDFResource("ex:era")))));

        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEnd"))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEndPosition"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                new OWLNamedIndividual(new RDFResource("ex:era")),
                new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                new OWLNamedIndividual(new RDFResource("ex:eraEnd")),
                new OWLNamedIndividual(new RDFResource("ex:era")))));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringEraBecauseNullEra()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareEra(null, new TIMEInstant(new RDFResource("ex:begin")), new TIMEInstant(new RDFResource("ex:end"))));

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringEraBecauseNullBegin()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareEra(new RDFResource("ex:era"), null, new TIMEInstant(new RDFResource("ex:end"))));

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringEraBecauseNullEnd()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareEra(new RDFResource("ex:era"), new TIMEInstant(new RDFResource("ex:begin")), null));

    [TestMethod]
    public void ShouldDeclareSubEra()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
        thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));

        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:era"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(new RDFResource("ex:era")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                new OWLNamedIndividual(new RDFResource("ex:era")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM),
                new OWLNamedIndividual(new RDFResource("ex:era")),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginning"))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginningPosition"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                new OWLNamedIndividual(new RDFResource("ex:era")),
                new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                new OWLNamedIndividual(new RDFResource("ex:eraBeginning")),
                new OWLNamedIndividual(new RDFResource("ex:era")))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEnd"))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEndPosition"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                new OWLNamedIndividual(new RDFResource("ex:era")),
                new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                new OWLNamedIndividual(new RDFResource("ex:eraEnd")),
                new OWLNamedIndividual(new RDFResource("ex:era")))));

        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEra"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(new RDFResource("ex:subEra")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                new OWLNamedIndividual(new RDFResource("ex:subEra")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM),
                new OWLNamedIndividual(new RDFResource("ex:subEra")),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraBeginning"))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraBeginningPosition"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:subEraBeginning")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                new OWLNamedIndividual(new RDFResource("ex:subEra")),
                new OWLNamedIndividual(new RDFResource("ex:subEraBeginning")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                new OWLNamedIndividual(new RDFResource("ex:subEraBeginning")),
                new OWLNamedIndividual(new RDFResource("ex:subEra")))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraEnd"))));
        Assert.IsTrue(thors.Ontology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraEndPosition"))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:subEraEnd")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                new OWLNamedIndividual(new RDFResource("ex:subEra")),
                new OWLNamedIndividual(new RDFResource("ex:subEraEnd")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                new OWLNamedIndividual(new RDFResource("ex:subEraEnd")),
                new OWLNamedIndividual(new RDFResource("ex:subEra")))));

        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER),
                new OWLNamedIndividual(new RDFResource("ex:era")),
                new OWLNamedIndividual(new RDFResource("ex:subEra")))));

        Assert.ThrowsExactly<OWLException>(() => _ = thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra")));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringSubEraBecauseNullEra()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareSubEra(null, new RDFResource("ex:subEra")));

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringSubEraBecauseNullSubEra()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareSubEra(new RDFResource("ex:era"), null));

    [TestMethod]
    public void ShouldDeclareReferencePoints()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareReferencePoints([
            new TIMEInstant(
                new RDFResource("ex:massExtinctionEventA"),
                new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionA"), TIMEPositionReferenceSystem.GeologicTime, 111.9)),
            new TIMEInstant(
                new RDFResource("ex:massExtinctionEventB"),
                new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionB"), TIMEPositionReferenceSystem.GeologicTime, 65.5))
        ]);

        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventA")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventB")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventA")))));
        Assert.IsTrue(thors.Ontology.CheckHasAssertionAxiom(
            new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT),
                new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventB")))));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringReferencePointsBecauseNullReferencePoints()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareReferencePoints(null));

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringReferencePointsBecauseContainingNullReferencePoints()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareReferencePoints([new TIMEInstant(new RDFResource("ex:massExtinctionEvent")), null]));

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringReferencePointsBecauseLessThan2ReferencePoints()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
            .DeclareReferencePoints([new TIMEInstant(new RDFResource("ex:massExtinctionEvent"))]));


    [TestMethod]
    public void ShouldCheckHasEra()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
            
        Assert.IsTrue(thors.CheckHasEra(new RDFResource("ex:era")));
        Assert.IsTrue(thors.CheckHasEra(new RDFResource("ex:subEra")));
        Assert.IsFalse(thors.CheckHasEra(new RDFResource("ex:erazz")));
        Assert.ThrowsExactly<OWLException>(() => _ = thors.CheckHasEra(null));
    }

    [TestMethod]
    public void ShouldCheckHasEraBoundary()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));

        Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:eraBeginning")));
        Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:eraEnd")));
        Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:subEraBeginning")));
        Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:subEraEnd")));
        Assert.IsFalse(thors.CheckHasEraBoundary(new RDFResource("ex:erazz")));
        Assert.ThrowsExactly<OWLException>(() => _ = thors.CheckHasEraBoundary(null));
    }

    [TestMethod]
    public void ShouldCheckHasReferencePoint()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareReferencePoints([
            new TIMEInstant(
                new RDFResource("ex:massExtinctionEventA"),
                new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionA"), TIMEPositionReferenceSystem.GeologicTime, 111.9)),
            new TIMEInstant(
                new RDFResource("ex:massExtinctionEventB"),
                new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionB"), TIMEPositionReferenceSystem.GeologicTime, 65.5))
        ]);

        Assert.IsTrue(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEventA")));
        Assert.IsTrue(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEventB")));
        Assert.IsFalse(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEventzz")));
        Assert.ThrowsExactly<OWLException>(() => _ = thors.CheckHasReferencePoint(null));
    }

    [TestMethod]
    public void ShouldCheckIsSubEraOf()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
        thors.DeclareEra(
            new RDFResource("ex:subsubEra"),
            new TIMEInstant(
                new RDFResource("ex:subsubEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subsubEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 184)));
        thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
        thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));

        Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subEra"), new RDFResource("ex:era")));
        Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra")));
        Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subsubEra"), new RDFResource("ex:era")));
        Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subEra"), new RDFResource("ex:era"), false));
        Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"), false));
        Assert.IsFalse(thors.CheckIsSubEraOf(new RDFResource("ex:subsubEra"), new RDFResource("ex:era"), false));
    }

    [TestMethod]
    public void ShouldGetSubErasOf()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
        thors.DeclareEra(
            new RDFResource("ex:subsubEra"),
            new TIMEInstant(
                new RDFResource("ex:subsubEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subsubEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 184)));
        thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
        thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
        List<RDFResource> subErasOfEraWithReasoning = thors.GetSubErasOf(new RDFResource("ex:era"));
        List<RDFResource> subErasOfEraWithoutReasoning = thors.GetSubErasOf(new RDFResource("ex:era"), false);
        List<RDFResource> subErasOfSubEraWithReasoning = thors.GetSubErasOf(new RDFResource("ex:subEra"));
        List<RDFResource> subErasOfSubEraWithoutReasoning = thors.GetSubErasOf(new RDFResource("ex:subEra"), false);
        List<RDFResource> subErasOfSubSubEraWithReasoning = thors.GetSubErasOf(new RDFResource("ex:subsubEra"));
        List<RDFResource> subErasOfSubSubEraWithoutReasoning = thors.GetSubErasOf(new RDFResource("ex:subsubEra"), false);

        Assert.AreEqual(2, subErasOfEraWithReasoning.Count);
        Assert.IsTrue(subErasOfEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subEra"))));
        Assert.IsTrue(subErasOfEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subsubEra"))));
        Assert.AreEqual(1, subErasOfEraWithoutReasoning.Count);
        Assert.IsTrue(subErasOfEraWithoutReasoning.Any(x => x.Equals(new RDFResource("ex:subEra"))));
        Assert.AreEqual(1, subErasOfSubEraWithReasoning.Count);
        Assert.IsTrue(subErasOfSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subsubEra"))));
        Assert.AreEqual(1, subErasOfSubEraWithoutReasoning.Count);
        Assert.IsTrue(subErasOfSubEraWithoutReasoning.Any(x => x.Equals(new RDFResource("ex:subsubEra"))));
        Assert.AreEqual(0, subErasOfSubSubEraWithReasoning.Count);
        Assert.AreEqual(0, subErasOfSubSubEraWithoutReasoning.Count);
    }

    [TestMethod]
    public void ShouldCheckIsSuperEraOf()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
        thors.DeclareEra(
            new RDFResource("ex:subsubEra"),
            new TIMEInstant(
                new RDFResource("ex:subsubEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subsubEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 184)));
        thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
        thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));

        Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:era"), new RDFResource("ex:subEra")));
        Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra")));
        Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:era"), new RDFResource("ex:subsubEra")));
        Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:era"), new RDFResource("ex:subEra"), false));
        Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"), false));
        Assert.IsFalse(thors.CheckIsSuperEraOf(new RDFResource("ex:era"), new RDFResource("ex:subsubEra"), false));
    }

    [TestMethod]
    public void ShouldGetSuperErasOf()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
        thors.DeclareEra(
            new RDFResource("ex:subsubEra"),
            new TIMEInstant(
                new RDFResource("ex:subsubEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subsubEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 184)));
        thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
        thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
        List<RDFResource> superErasOfEraWithReasoning = thors.GetSuperErasOf(new RDFResource("ex:era"));
        List<RDFResource> superErasOfEraWithoutReasoning = thors.GetSuperErasOf(new RDFResource("ex:era"), false);
        List<RDFResource> superErasOfSubEraWithReasoning = thors.GetSuperErasOf(new RDFResource("ex:subEra"));
        List<RDFResource> superErasOfSubEraWithoutReasoning = thors.GetSuperErasOf(new RDFResource("ex:subEra"), false);
        List<RDFResource> superErasOfSubSubEraWithReasoning = thors.GetSuperErasOf(new RDFResource("ex:subsubEra"));
        List<RDFResource> superErasOfSubSubEraWithoutReasoning = thors.GetSuperErasOf(new RDFResource("ex:subsubEra"), false);

        Assert.AreEqual(0, superErasOfEraWithReasoning.Count);
        Assert.AreEqual(0, superErasOfEraWithoutReasoning.Count);
        Assert.AreEqual(1, superErasOfSubEraWithReasoning.Count);
        Assert.IsTrue(superErasOfSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:era"))));
        Assert.AreEqual(1, superErasOfSubEraWithoutReasoning.Count);
        Assert.IsTrue(superErasOfSubEraWithoutReasoning.Any(x => x.Equals(new RDFResource("ex:era"))));
        Assert.AreEqual(2, superErasOfSubSubEraWithReasoning.Count);
        Assert.IsTrue(superErasOfSubSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subEra"))));
        Assert.IsTrue(superErasOfSubSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:era"))));
        Assert.AreEqual(1, superErasOfSubSubEraWithoutReasoning.Count);
        Assert.IsTrue(superErasOfSubSubEraWithoutReasoning.Any(x => x.Equals(new RDFResource("ex:subEra"))));
    }

    [TestMethod]
    public void ShouldGetEraCoordinates()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
        thors.DeclareEra(
            new RDFResource("ex:subsubEra"),
            new TIMEInstant(
                new RDFResource("ex:subsubEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subsubEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 184)));
        thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
        thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
        (TIMECoordinate, TIMECoordinate) eraCoordinates = thors.GetEraCoordinates(new RDFResource("ex:era"));
        (TIMECoordinate, TIMECoordinate) subEraCoordinates = thors.GetEraCoordinates(new RDFResource("ex:subEra"));
        (TIMECoordinate, TIMECoordinate) subsubEraCoordinates = thors.GetEraCoordinates(new RDFResource("ex:subsubEra"));

        Assert.IsNotNull(eraCoordinates.Item1);
        Assert.IsTrue(eraCoordinates.Item1.Equals(new TIMECoordinate(-185_498_050, 1, 1, 0, 0, 0)));
        Assert.IsNotNull(eraCoordinates.Item2);
        Assert.IsTrue(eraCoordinates.Item2.Equals(new TIMECoordinate(-169_998_050, 1, 1, 0, 0, 0)));
        Assert.IsNotNull(subEraCoordinates.Item1);
        Assert.IsTrue(subEraCoordinates.Item1.Equals(new TIMECoordinate(-185_498_050, 1, 1, 0, 0, 0)));
        Assert.IsNotNull(subEraCoordinates.Item2);
        Assert.IsTrue(subEraCoordinates.Item2.Equals(new TIMECoordinate(-180_498_050, 1, 1, 0, 0, 0)));
        Assert.IsNotNull(subsubEraCoordinates.Item1);
        Assert.IsTrue(subsubEraCoordinates.Item1.Equals(new TIMECoordinate(-185_498_050, 1, 1, 0, 0, 0)));
        Assert.IsNotNull(subsubEraCoordinates.Item2);
        Assert.IsTrue(subsubEraCoordinates.Item2.Equals(new TIMECoordinate(-183_998_050, 1, 1, 0, 0, 0)));
        Assert.ThrowsExactly<OWLException>(() => _ = thors.GetEraCoordinates(new RDFResource("ex:unexistingEra")));
        Assert.ThrowsExactly<OWLException>(() => _ = thors.GetEraCoordinates(null));
    }

    [TestMethod]
    public void ShouldGetEraExtent()
    {
        TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
        thors.DeclareEra(
            new RDFResource("ex:era"),
            new TIMEInstant(
                new RDFResource("ex:eraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:eraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 170)));
        thors.DeclareEra(
            new RDFResource("ex:subEra"),
            new TIMEInstant(
                new RDFResource("ex:subEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 180.5)));
        thors.DeclareEra(
            new RDFResource("ex:subsubEra"),
            new TIMEInstant(
                new RDFResource("ex:subsubEraBeginning"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTime, 185.5)),
            new TIMEInstant(
                new RDFResource("ex:subsubEraEnd"),
                new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTime, 184)));
        thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
        thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
        TIMEExtent eraExtent = thors.GetEraExtent(new RDFResource("ex:era"));
        TIMEExtent subEraExtent = thors.GetEraExtent(new RDFResource("ex:subEra"));
        TIMEExtent subsubEraExtent = thors.GetEraExtent(new RDFResource("ex:subsubEra"));

        Assert.IsNotNull(eraExtent);
        Assert.AreEqual(5_657_500_000, eraExtent.Days);
        Assert.IsNotNull(subEraExtent);
        Assert.AreEqual(1_825_000_000, subEraExtent.Days);
        Assert.IsNotNull(subsubEraExtent);
        Assert.AreEqual(547_500_000, subsubEraExtent.Days);
        Assert.ThrowsExactly<OWLException>(() => _ = thors.GetEraExtent(new RDFResource("ex:unexistingEra")));
        Assert.ThrowsExactly<OWLException>(() => _ = thors.GetEraExtent(null));
    }
    #endregion
}