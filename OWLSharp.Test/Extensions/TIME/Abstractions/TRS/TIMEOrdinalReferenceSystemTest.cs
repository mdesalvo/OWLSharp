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

namespace OWLSharp.Test.Extensions.TIME
{
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
            Assert.IsNotNull(TestTRS.THORSOntology);
            Assert.IsTrue(TestTRS.Equals(new RDFResource("ex:Thors")));
            Assert.IsTrue(TestTRS.THORSOntology.Imports.Count == 3);
            Assert.IsTrue(TestTRS.THORSOntology.Prefixes.Count == 5);
            Assert.IsTrue(TestTRS.THORSOntology.DeclarationAxioms.Count == 109);
            Assert.IsTrue(TestTRS.THORSOntology.AssertionAxioms.Count == 63);
            Assert.IsTrue(TestTRS.THORSOntology.ClassAxioms.Count == 62);
            Assert.IsTrue(TestTRS.THORSOntology.DataPropertyAxioms.Count == 25);
            Assert.IsTrue(TestTRS.THORSOntology.ObjectPropertyAxioms.Count == 119);

            //Test copy-ctor
            TIMEOrdinalReferenceSystem newTRS = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            Assert.IsNotNull(newTRS);
            Assert.IsNotNull(newTRS.THORSOntology);
            Assert.IsTrue(newTRS.Equals(new RDFResource("ex:Thors2")));
            Assert.IsTrue(newTRS.THORSOntology.Imports.Count == 3);
            Assert.IsTrue(newTRS.THORSOntology.Prefixes.Count == 5);
            Assert.IsTrue(newTRS.THORSOntology.DeclarationAxioms.Count == 109);
            Assert.IsTrue(newTRS.THORSOntology.AssertionAxioms.Count == 63);
            Assert.IsTrue(newTRS.THORSOntology.ClassAxioms.Count == 62);
            Assert.IsTrue(newTRS.THORSOntology.DataPropertyAxioms.Count == 25);
            Assert.IsTrue(newTRS.THORSOntology.ObjectPropertyAxioms.Count == 119);
            Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), null));
        }

        [TestMethod]
        public void ShouldDeclareEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"), 
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));

            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:era"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM),
                    new OWLNamedIndividual(new RDFResource("ex:era")),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")))));

            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginning"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginningPosition"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                    new OWLNamedIndividual(new RDFResource("ex:era")),
                    new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                    new OWLNamedIndividual(new RDFResource("ex:eraBeginning")),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));

            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEnd"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEndPosition"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                    new OWLNamedIndividual(new RDFResource("ex:era")),
                    new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                    new OWLNamedIndividual(new RDFResource("ex:eraEnd")),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEraBecauseNullEra()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                                                            .DeclareEra(null, new TIMEInstant(new RDFResource("ex:begin")), new TIMEInstant(new RDFResource("ex:end"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEraBecauseNullBegin()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                                                            .DeclareEra(new RDFResource("ex:era"), null, new TIMEInstant(new RDFResource("ex:end"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEraBecauseNullEnd()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                                                            .DeclareEra(new RDFResource("ex:era"), new TIMEInstant(new RDFResource("ex:begin")), null));

        [TestMethod]
        public void ShouldDeclareSubEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));

            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:era"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM),
                    new OWLNamedIndividual(new RDFResource("ex:era")),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginning"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraBeginningPosition"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                    new OWLNamedIndividual(new RDFResource("ex:era")),
                    new OWLNamedIndividual(new RDFResource("ex:eraBeginning")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                    new OWLNamedIndividual(new RDFResource("ex:eraBeginning")),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEnd"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:eraEndPosition"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                    new OWLNamedIndividual(new RDFResource("ex:era")),
                    new OWLNamedIndividual(new RDFResource("ex:eraEnd")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                    new OWLNamedIndividual(new RDFResource("ex:eraEnd")),
                    new OWLNamedIndividual(new RDFResource("ex:era")))));

            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEra"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraBeginning"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraBeginningPosition"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:subEraBeginning")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")),
                    new OWLNamedIndividual(new RDFResource("ex:subEraBeginning")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                    new OWLNamedIndividual(new RDFResource("ex:subEraBeginning")),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraEnd"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:subEraEndPosition"))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:subEraEnd")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")),
                    new OWLNamedIndividual(new RDFResource("ex:subEraEnd")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                    new OWLNamedIndividual(new RDFResource("ex:subEraEnd")),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")))));

            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER),
                    new OWLNamedIndividual(new RDFResource("ex:era")),
                    new OWLNamedIndividual(new RDFResource("ex:subEra")))));

            Assert.ThrowsException<OWLException>(() => thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSubEraBecauseNullEra()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                                                            .DeclareSubEra(null, new RDFResource("ex:subEra")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSubEraBecauseNullSubEra()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                                                            .DeclareSubEra(new RDFResource("ex:era"), null));

        [TestMethod]
        public void ShouldDeclareReferencePoints()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareReferencePoints([
                new TIMEInstant(
                    new RDFResource("ex:massExtinctionEventA"),
                    new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionA"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 111.9)),
                new TIMEInstant(
                    new RDFResource("ex:massExtinctionEventB"),
                    new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionB"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 65.5))
            ]);

            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventA")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventB")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                    new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventA")))));
            Assert.IsTrue(thors.THORSOntology.CheckHasAssertionAxiom(
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT),
                    new OWLNamedIndividual(new RDFResource("ex:Thors2")),
                    new OWLNamedIndividual(new RDFResource("ex:massExtinctionEventB")))));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringReferencePointsBecauseNullReferencePoints()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                .DeclareReferencePoints(null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringReferencePointsBecauseContainingNullReferencePoints()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                .DeclareReferencePoints([new TIMEInstant(new RDFResource("ex:massExtinctionEvent")), null]));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringReferencePointsBecauseLessThan2ReferencePoints()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS)
                .DeclareReferencePoints([new TIMEInstant(new RDFResource("ex:massExtinctionEvent"))]));


        [TestMethod]
        public void ShouldCheckHasEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            
            Assert.IsTrue(thors.CheckHasEra(new RDFResource("ex:era")));
            Assert.IsTrue(thors.CheckHasEra(new RDFResource("ex:subEra")));
            Assert.IsFalse(thors.CheckHasEra(new RDFResource("ex:erazz")));
            Assert.ThrowsException<OWLException>(() => thors.CheckHasEra(null));
        }

        [TestMethod]
        public void ShouldCheckHasEraBoundary()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));

            Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:eraBeginning")));
            Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:eraEnd")));
            Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:subEraBeginning")));
            Assert.IsTrue(thors.CheckHasEraBoundary(new RDFResource("ex:subEraEnd")));
            Assert.IsFalse(thors.CheckHasEraBoundary(new RDFResource("ex:erazz")));
            Assert.ThrowsException<OWLException>(() => thors.CheckHasEraBoundary(null));
        }

        [TestMethod]
        public void ShouldCheckHasReferencePoint()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareReferencePoints([
                new TIMEInstant(
                    new RDFResource("ex:massExtinctionEventA"),
                    new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionA"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 111.9)),
                new TIMEInstant(
                    new RDFResource("ex:massExtinctionEventB"),
                    new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPositionB"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 65.5))
            ]);

            Assert.IsTrue(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEventA")));
            Assert.IsTrue(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEventB")));
            Assert.IsFalse(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEventzz")));
            Assert.ThrowsException<OWLException>(() => thors.CheckHasReferencePoint(null));
        }

        [TestMethod]
        public void ShouldCheckIsSubEraOf()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 184)));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
            thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));

            Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subEra"), new RDFResource("ex:era"), true));
            Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"), true));
            Assert.IsTrue(thors.CheckIsSubEraOf(new RDFResource("ex:subsubEra"), new RDFResource("ex:era"), true));
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
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 184)));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
            thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
            List<RDFResource> subErasOfEraWithReasoning = thors.GetSubErasOf(new RDFResource("ex:era"), true);
            List<RDFResource> subErasOfEraWithoutReasoning = thors.GetSubErasOf(new RDFResource("ex:era"), false);
            List<RDFResource> subErasOfSubEraWithReasoning = thors.GetSubErasOf(new RDFResource("ex:subEra"), true);
            List<RDFResource> subErasOfSubEraWithoutReasoning = thors.GetSubErasOf(new RDFResource("ex:subEra"), false);
            List<RDFResource> subErasOfSubSubEraWithReasoning = thors.GetSubErasOf(new RDFResource("ex:subsubEra"), true);
            List<RDFResource> subErasOfSubSubEraWithoutReasoning = thors.GetSubErasOf(new RDFResource("ex:subsubEra"), false);

            Assert.IsTrue(subErasOfEraWithReasoning.Count == 2);
            Assert.IsTrue(subErasOfEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subEra"))));
            Assert.IsTrue(subErasOfEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subsubEra"))));
            Assert.IsTrue(subErasOfEraWithoutReasoning.Count == 1);
            Assert.IsTrue(subErasOfEraWithoutReasoning.Any(x => x.Equals(new RDFResource("ex:subEra"))));
            Assert.IsTrue(subErasOfSubEraWithReasoning.Count == 1);
            Assert.IsTrue(subErasOfSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subsubEra"))));
            Assert.IsTrue(subErasOfSubEraWithoutReasoning.Count == 1);
            Assert.IsTrue(subErasOfSubEraWithoutReasoning.Any(x => x.Equals(new RDFResource("ex:subsubEra"))));
            Assert.IsTrue(subErasOfSubSubEraWithReasoning.Count == 0);
            Assert.IsTrue(subErasOfSubSubEraWithoutReasoning.Count == 0);
        }

        [TestMethod]
        public void ShouldCheckIsSuperEraOf()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 184)));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
            thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));

            Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:era"), new RDFResource("ex:subEra"), true));
            Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"), true));
            Assert.IsTrue(thors.CheckIsSuperEraOf(new RDFResource("ex:era"), new RDFResource("ex:subsubEra"), true));
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
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 184)));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
            thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
            List<RDFResource> superErasOfEraWithReasoning = thors.GetSuperErasOf(new RDFResource("ex:era"), true);
            List<RDFResource> superErasOfEraWithoutReasoning = thors.GetSuperErasOf(new RDFResource("ex:era"), false);
            List<RDFResource> superErasOfSubEraWithReasoning = thors.GetSuperErasOf(new RDFResource("ex:subEra"), true);
            List<RDFResource> superErasOfSubEraWithoutReasoning = thors.GetSuperErasOf(new RDFResource("ex:subEra"), false);
            List<RDFResource> superErasOfSubSubEraWithReasoning = thors.GetSuperErasOf(new RDFResource("ex:subsubEra"), true);
            List<RDFResource> superErasOfSubSubEraWithoutReasoning = thors.GetSuperErasOf(new RDFResource("ex:subsubEra"), false);

            Assert.IsTrue(superErasOfEraWithReasoning.Count == 0);
            Assert.IsTrue(superErasOfEraWithoutReasoning.Count == 0);
            Assert.IsTrue(superErasOfSubEraWithReasoning.Count == 1);
            Assert.IsTrue(superErasOfSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:era"))));
            Assert.IsTrue(superErasOfSubEraWithoutReasoning.Count == 1);
            Assert.IsTrue(superErasOfSubEraWithoutReasoning.Any(x => x.Equals(new RDFResource("ex:era"))));
            Assert.IsTrue(superErasOfSubSubEraWithReasoning.Count == 2);
            Assert.IsTrue(superErasOfSubSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:subEra"))));
            Assert.IsTrue(superErasOfSubSubEraWithReasoning.Any(x => x.Equals(new RDFResource("ex:era"))));
            Assert.IsTrue(superErasOfSubSubEraWithoutReasoning.Count == 1);
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
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 184)));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
            thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
            (TIMECoordinate, TIMECoordinate) eraCoordinates = thors.GetEraCoordinates(new RDFResource("ex:era"));
            (TIMECoordinate, TIMECoordinate) subEraCoordinates = thors.GetEraCoordinates(new RDFResource("ex:subEra"));
            (TIMECoordinate, TIMECoordinate) subsubEraCoordinates = thors.GetEraCoordinates(new RDFResource("ex:subsubEra"));

            Assert.IsNotNull(eraCoordinates);
            Assert.IsNotNull(eraCoordinates.Item1);
            Assert.IsTrue(eraCoordinates.Item1.Equals(new TIMECoordinate(-185_498_050, 1, 1, 0, 0, 0)));
            Assert.IsNotNull(eraCoordinates.Item2);
            Assert.IsTrue(eraCoordinates.Item2.Equals(new TIMECoordinate(-169_998_050, 1, 1, 0, 0, 0)));
            Assert.IsNotNull(subEraCoordinates);
            Assert.IsNotNull(subEraCoordinates.Item1);
            Assert.IsTrue(subEraCoordinates.Item1.Equals(new TIMECoordinate(-185_498_050, 1, 1, 0, 0, 0)));
            Assert.IsNotNull(subEraCoordinates.Item2);
            Assert.IsTrue(subEraCoordinates.Item2.Equals(new TIMECoordinate(-180_498_050, 1, 1, 0, 0, 0)));
            Assert.IsNotNull(subsubEraCoordinates);
            Assert.IsNotNull(subsubEraCoordinates.Item1);
            Assert.IsTrue(subsubEraCoordinates.Item1.Equals(new TIMECoordinate(-185_498_050, 1, 1, 0, 0, 0)));
            Assert.IsNotNull(subsubEraCoordinates.Item2);
            Assert.IsTrue(subsubEraCoordinates.Item2.Equals(new TIMECoordinate(-183_998_050, 1, 1, 0, 0, 0)));
            Assert.ThrowsException<OWLException>(() => thors.GetEraCoordinates(new RDFResource("ex:unexistingEra")));
            Assert.ThrowsException<OWLException>(() => thors.GetEraCoordinates(null));
        }

        [TestMethod]
        public void ShouldGetEraExtent()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors2"), TestTRS);
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.ChronometricGeologicTime, 184)));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"));
            thors.DeclareSubEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"));
            TIMEExtent eraExtent = thors.GetEraExtent(new RDFResource("ex:era"));
            TIMEExtent subEraExtent = thors.GetEraExtent(new RDFResource("ex:subEra"));
            TIMEExtent subsubEraExtent = thors.GetEraExtent(new RDFResource("ex:subsubEra"));

            Assert.IsNotNull(eraExtent);
            Assert.IsTrue(eraExtent.Days == 5_657_500_000);
            Assert.IsNotNull(subEraExtent);
            Assert.IsTrue(subEraExtent.Days == 1_825_000_000);
            Assert.IsNotNull(subsubEraExtent);
            Assert.IsTrue(subsubEraExtent.Days == 547_500_000);
            Assert.ThrowsException<OWLException>(() => thors.GetEraExtent(new RDFResource("ex:unexistingEra")));
            Assert.ThrowsException<OWLException>(() => thors.GetEraExtent(null));
        }
        #endregion
    }
}