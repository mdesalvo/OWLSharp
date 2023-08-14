/*
   Copyright 2012-2023 Marco De Salvo

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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEOrdinalReferenceSystemTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateTHORS()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));

            //Test initialization of TRS
            Assert.IsNotNull(thors);
            Assert.IsNotNull(thors.Ontology);
            Assert.IsTrue(thors.Equals(new RDFResource("ex:Thors")));

            //Test initialization of TIME+THORS knowledge
            Assert.IsTrue(thors.Ontology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(thors.Ontology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(thors.Ontology.Data.IndividualsCount == 34);
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:Thors")));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:Thors"), RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:Thors"), RDFVocabulary.TIME.TRS));
        }

        [TestMethod]
        public void ShouldDeclareEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"), 
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));

            //Test evolution of TIME+THORS knowledge
            Assert.IsTrue(thors.Ontology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(thors.Ontology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(thors.Ontology.Data.IndividualsCount == 39);
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:era")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:eraBeginning")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:eraBeginningPosition")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:eraEnd")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:eraEndPosition")));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:era"), RDFVocabulary.TIME.THORS.ERA));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:era"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:era"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraBeginning"), RDFVocabulary.TIME.THORS.ERA_BOUNDARY));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraBeginning"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraBeginningPosition"), RDFVocabulary.TIME.TIME_POSITION));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraBeginningPosition"), RDFVocabulary.TIME.TEMPORAL_POSITION));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraEnd"), RDFVocabulary.TIME.THORS.ERA_BOUNDARY));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraEnd"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraEndPosition"), RDFVocabulary.TIME.TIME_POSITION));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:eraEndPosition"), RDFVocabulary.TIME.TEMPORAL_POSITION));
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:Thors"), RDFVocabulary.TIME.THORS.COMPONENT, new RDFResource("ex:era")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:era"), RDFVocabulary.TIME.THORS.SYSTEM, new RDFResource("ex:Thors")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:era"), RDFVocabulary.TIME.THORS.BEGIN, new RDFResource("ex:eraBeginning")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:eraBeginning"), RDFVocabulary.TIME.THORS.NEXT_ERA, new RDFResource("ex:era")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:era"), RDFVocabulary.TIME.THORS.END, new RDFResource("ex:eraEnd")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:eraEnd"), RDFVocabulary.TIME.THORS.PREVIOUS_ERA, new RDFResource("ex:era")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEraBecauseNullEra()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"))
                .DeclareEra(null, new TIMEInstant(new RDFResource("ex:begin")), new TIMEInstant(new RDFResource("ex:begin"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEraBecauseNullBegin()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"))
                .DeclareEra(new RDFResource("ex:era"), null, new TIMEInstant(new RDFResource("ex:begin"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringEraBecauseNullEnd()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"))
                .DeclareEra(new RDFResource("ex:era"), new TIMEInstant(new RDFResource("ex:begin")), null));

        [TestMethod]
        public void ShouldDeclareSubEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"));

            //Test evolution of TIME+THORS knowledge
            Assert.IsTrue(thors.Ontology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(thors.Ontology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(thors.Ontology.Data.IndividualsCount == 44);
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:era"), RDFVocabulary.TIME.THORS.MEMBER, new RDFResource("ex:subEra")));
            Assert.ThrowsException<OWLException>(() => thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:era")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSubEraBecauseNullEra()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"))
                .DeclareSubEra(null, new RDFResource("ex:subEra")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSubEraBecauseNullSubEra()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"))
                .DeclareSubEra(new RDFResource("ex:era"), null));

        [TestMethod]
        public void ShouldDeclareReferencePoint()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareReferencePoint(
                new TIMEInstant(
                    new RDFResource("ex:massExtinctionEvent"),
                    new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPosition"), TIMEPositionReferenceSystem.GeologicTRS, 65.5)));

            //Test evolution of TIME+THORS knowledge
            Assert.IsTrue(thors.Ontology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(thors.Ontology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(thors.Ontology.Data.IndividualsCount == 36);
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:massExtinctionEvent")));
            Assert.IsTrue(thors.Ontology.Data.CheckHasIndividual(new RDFResource("ex:massExtinctionEventPosition")));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:massExtinctionEvent"), RDFVocabulary.TIME.THORS.ERA_BOUNDARY));
            Assert.IsTrue(thors.Ontology.Data.CheckIsIndividualOf(thors.Ontology.Model, new RDFResource("ex:massExtinctionEvent"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(thors.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:Thors"), RDFVocabulary.TIME.THORS.REFERENCE_POINT, new RDFResource("ex:massExtinctionEvent")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringreferencePointBecauseNullReferencePoint()
            => Assert.ThrowsException<OWLException>(() => new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"))
                .DeclareReferencePoint(null));

        //Analyzer

        [TestMethod]
        public void ShouldCheckHasEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            
            Assert.IsTrue(thors.CheckHasEra(new RDFResource("ex:era")));
            Assert.IsTrue(thors.CheckHasEra(new RDFResource("ex:subEra")));
            Assert.IsFalse(thors.CheckHasEra(new RDFResource("ex:erazz")));
            Assert.ThrowsException<OWLException>(() => thors.CheckHasEra(null));
        }

        [TestMethod]
        public void ShouldCheckHasEraBoundary()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));

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
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareReferencePoint(
                new TIMEInstant(
                    new RDFResource("ex:massExtinctionEvent"),
                    new TIMEInstantPosition(new RDFResource("ex:massExtinctionEventPosition"), TIMEPositionReferenceSystem.GeologicTRS, 65.5)));

            Assert.IsTrue(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEvent")));
            Assert.IsFalse(thors.CheckHasReferencePoint(new RDFResource("ex:massExtinctionEventzz")));
            Assert.ThrowsException<OWLException>(() => thors.CheckHasReferencePoint(null));
        }

        [TestMethod]
        public void ShouldCheckHasSubEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 184)));
            thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"));

            Assert.IsTrue(thors.CheckHasSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"), true));
            Assert.IsTrue(thors.CheckHasSubEra(new RDFResource("ex:era"), new RDFResource("ex:subsubEra"), true));
            Assert.IsTrue(thors.CheckHasSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"), true));
            Assert.IsTrue(thors.CheckHasSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"), false));
            Assert.IsFalse(thors.CheckHasSubEra(new RDFResource("ex:era"), new RDFResource("ex:subsubEra"), false));
            Assert.IsTrue(thors.CheckHasSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"), false));
        }

        [TestMethod]
        public void ShouldGetSubEras()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 184)));
            thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"));
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
        public void ShouldCheckHasSuperEra()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 184)));
            thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"));

            Assert.IsTrue(thors.CheckHasSuperEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"), true));
            Assert.IsTrue(thors.CheckHasSuperEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:era"), true));
            Assert.IsTrue(thors.CheckHasSuperEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"), true));
            Assert.IsTrue(thors.CheckHasSuperEra(new RDFResource("ex:subEra"), new RDFResource("ex:era"), false));
            Assert.IsFalse(thors.CheckHasSuperEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:era"), false));
            Assert.IsTrue(thors.CheckHasSuperEra(new RDFResource("ex:subsubEra"), new RDFResource("ex:subEra"), false));
        }

        [TestMethod]
        public void ShouldGetSuperEras()
        {
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 184)));
            thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"));
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
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 184)));
            thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"));
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
            TIMEOrdinalReferenceSystem thors = new TIMEOrdinalReferenceSystem(new RDFResource("ex:Thors"));
            thors.DeclareEra(
                new RDFResource("ex:era"),
                new TIMEInstant(
                    new RDFResource("ex:eraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:eraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:eraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:eraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 170)));
            thors.DeclareEra(
                new RDFResource("ex:subEra"),
                new TIMEInstant(
                    new RDFResource("ex:subEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 180.5)));
            thors.DeclareEra(
                new RDFResource("ex:subsubEra"),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraBeginning"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraBeginningPosition"), TIMEPositionReferenceSystem.GeologicTRS, 185.5)),
                new TIMEInstant(
                    new RDFResource("ex:subsubEraEnd"),
                    new TIMEInstantPosition(new RDFResource("ex:subsubEraEndPosition"), TIMEPositionReferenceSystem.GeologicTRS, 184)));
            thors.DeclareSubEra(new RDFResource("ex:era"), new RDFResource("ex:subEra"));
            thors.DeclareSubEra(new RDFResource("ex:subEra"), new RDFResource("ex:subsubEra"));
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
