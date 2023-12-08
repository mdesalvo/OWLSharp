/*
   Copyright 2012-2024 Marco De Salvo

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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEOntologyHelperTest
    {
        #region Tests (Declarer)
        [TestMethod]
        public void ShouldDeclareInstantByDateTime()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"), 
                DateTime.Parse("2023-03-22T10:35:34Z"));
            timeOntology.DeclareTimeInstant(new RDFResource("ex:feat"), timeInstant);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 35);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInst"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
        }

        [TestMethod]
        public void ShouldDeclareInstantByDescriptionFromCoordinate()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"), 
                new TIMEInstantDescription(
                    new RDFResource("ex:timeInstDesc"),
                    new TIMECoordinate(2023d, 3d, 22d, 10d, 35d, 34d,
                        new TIMECoordinateMetadata(
                            new RDFResource("ex:TRS"),
                            new RDFResource("ex:Unit"),
                            RDFVocabulary.TIME.GREG.MARCH,
                            RDFVocabulary.TIME.WEDNESDAY,
                            81))));
            timeOntology.DeclareTimeInstant(new RDFResource("ex:feat"), timeInstant);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInst"), RDFVocabulary.TIME.IN_DATETIME, new RDFResource("ex:timeInstDesc")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInstDesc")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.TEMPORAL_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.YEAR, new RDFTypedLiteral("2023", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.MONTH, new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.DAY, new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.HOUR, new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral("35", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.SECOND, new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.UNIT_TYPE, new RDFResource("ex:Unit")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("ex:TRS")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.MONTH_OF_YEAR, RDFVocabulary.TIME.GREG.MARCH));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral("81", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.DAY_OF_WEEK, RDFVocabulary.TIME.WEDNESDAY));
        }

        [TestMethod]
        public void ShouldDeclareInstantByDescriptionFromDateTime()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"), 
                new TIMEInstantDescription(
                    new RDFResource("ex:timeInstDesc"),
                    DateTime.Parse("2023-03-22T10:35:34Z")));
            timeOntology.DeclareTimeInstant(new RDFResource("ex:feat"), timeInstant);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInst"), RDFVocabulary.TIME.IN_DATETIME, new RDFResource("ex:timeInstDesc")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInstDesc")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.TEMPORAL_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.YEAR, new RDFTypedLiteral("2023", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.MONTH, new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.DAY, new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.HOUR, new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral("35", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.SECOND, new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_SECOND));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.MONTH_OF_YEAR, RDFVocabulary.TIME.GREG.MARCH));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral("81", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstDesc"), RDFVocabulary.TIME.DAY_OF_WEEK, RDFVocabulary.TIME.WEDNESDAY));
        }

        [TestMethod]
        public void ShouldDeclareInstantByNumericPosition()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"),
                new TIMEInstantPosition(new RDFResource("ex:timeInstPos"),
                    TIMEPositionReferenceSystem.UnixTRS,
                    1679477734)); //2023-03-22T09:35:34Z
            timeOntology.DeclareTimeInstant(new RDFResource("ex:feat"), timeInstant);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInst"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:timeInstPos")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInstPos")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.TEMPORAL_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.TIME_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("1679477734", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
        }

        [TestMethod]
        public void ShouldDeclareInstantByNumericPositionWithUncertainty()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEIntervalDuration timeInstantUncertainty = new TIMEIntervalDuration(
                new RDFResource("ex:timeInstPosUnc"), RDFVocabulary.TIME.UNIT_SECOND, 4.04);
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"),
                new TIMEInstantPosition(new RDFResource("ex:timeInstPos"),
                    TIMEPositionReferenceSystem.UnixTRS,
                    1679477734) //2023-03-22T09:35:34Z
                .SetPositionalUncertainty(timeInstantUncertainty)); 
            timeOntology.DeclareTimeInstant(new RDFResource("ex:feat"), timeInstant);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 37);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInst"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:timeInstPos")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInstPos")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.TEMPORAL_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.TIME_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("1679477734", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY, new RDFResource("ex:timeInstPosUnc")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInstPosUnc")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPosUnc"), RDFVocabulary.TIME.TEMPORAL_DURATION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPosUnc"), RDFVocabulary.TIME.DURATION));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstPosUnc"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_SECOND));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeInstPosUnc"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("4.04", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
        }

        [TestMethod]
        public void ShouldDeclareInstantByNominalPosition()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"),
                new TIMEInstantPosition(new RDFResource("ex:timeInstPos"),
                    new RDFResource("http://example.org/geologic/"),
                    new RDFResource("http://example.org/geologic/Archean")));
            timeOntology.DeclareTimeInstant(new RDFResource("ex:feat"), timeInstant);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInst"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInst"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:timeInstPos")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInstPos")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.TEMPORAL_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.TIME_POSITION));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.NOMINAL_POSITION, new RDFResource("http://example.org/geologic/Archean")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInstPos"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("http://example.org/geologic/")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringInstantBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt")
                .DeclareTimeInstant(null, new TIMEInstant(new RDFResource("ex:timeInst"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringInstantBecauseNullInstant()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt")
                .DeclareTimeInstant(new RDFResource("ex:feat"), null));

        [TestMethod]
        public void ShouldDeclareInstantRelation()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInstant timeInstant1 = new TIMEInstant(new RDFResource("ex:timeInst1"), DateTime.Parse("2023-03-24T10:35:34Z"));
            TIMEInstant timeInstant2 = new TIMEInstant(new RDFResource("ex:timeInst2"), DateTime.Parse("2023-03-21T10:35:34Z"));
            TIMEInstant timeInstant3 = new TIMEInstant(new RDFResource("ex:timeInst3"), DateTime.Parse("2023-03-22T10:35:34Z"));
            timeOntology.DeclareTimeInstantRelation(timeInstant1, timeInstant3, TIMEEnums.TIMEInstantRelation.After);
            timeOntology.DeclareTimeInstantRelation(timeInstant2, timeInstant3, TIMEEnums.TIMEInstantRelation.Before);
            
            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst1")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst2")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeInst3")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInst1"), RDFVocabulary.TIME.AFTER, new RDFResource("ex:timeInst3")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeInst2"), RDFVocabulary.TIME.BEFORE, new RDFResource("ex:timeInst3")));
            Assert.ThrowsException<OWLException>(() => timeOntology.DeclareTimeInstantRelation(null, timeInstant3, TIMEEnums.TIMEInstantRelation.After));
            Assert.ThrowsException<OWLException>(() => timeOntology.DeclareTimeInstantRelation(timeInstant1, null, TIMEEnums.TIMEInstantRelation.Before));
        }

        [TestMethod]
        public void ShouldDeclareIntervalByTimeSpan()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                XmlConvert.ToTimeSpan("P1Y2M3DT11H5M7S"));
            timeOntology.DeclareTimeInterval(new RDFResource("ex:feat"), timeInterval);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 35);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral("P428DT11H5M7S", RDFModelEnums.RDFDatatypes.XSD_DURATION)));
        }

        [TestMethod]
        public void ShouldDeclareIntervalByDescriptionFromLength()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEIntervalDescription(new RDFResource("ex:timeIntrDesc"),
                    new TIMEExtent(1, 2, 3, 4, 5, 6, 7, new TIMEExtentMetadata(new RDFResource("ex:TRS")))));
            timeOntology.DeclareTimeInterval(new RDFResource("ex:feat"), timeInterval);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new RDFResource("ex:timeIntrDesc")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntrDesc")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.TEMPORAL_DURATION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.YEARS, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.MONTHS, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.WEEKS, new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.DAYS, new RDFTypedLiteral("4", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.HOURS, new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("ex:TRS")));
        }

        [TestMethod]
        public void ShouldDeclareIntervalByDescriptionFromTimeSpan()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEIntervalDescription(new RDFResource("ex:timeIntrDesc"),
                    new TIMEExtent(XmlConvert.ToTimeSpan("P1Y1M1DT1H1M1S"))));
            timeOntology.DeclareTimeInterval(new RDFResource("ex:feat"), timeInterval);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new RDFResource("ex:timeIntrDesc")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntrDesc")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.TEMPORAL_DURATION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.DAYS, new RDFTypedLiteral("396", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.HOURS, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntrDesc"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
        }

        [TestMethod]
        public void ShouldDeclareIntervalByDuration()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEIntervalDuration(new RDFResource("ex:timeIntrDur"), RDFVocabulary.TIME.UNIT_DAY, 77));
            timeOntology.DeclareTimeInterval(new RDFResource("ex:feat"), timeInterval);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_DURATION, new RDFResource("ex:timeIntrDur")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntrDur")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntrDur"), RDFVocabulary.TIME.TEMPORAL_DURATION));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntrDur"), RDFVocabulary.TIME.DURATION));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeIntrDur"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("77", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntrDur"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_DAY));
        }

        [TestMethod]
        public void ShouldDeclareIntervalByBeginning()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEInstant(new RDFResource("ex:timeBeginning"), DateTime.Parse("2023-03-22T10:35:34Z")), null);
            timeOntology.DeclareTimeInterval(new RDFResource("ex:feat"), timeInterval);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:timeBeginning")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeBeginning")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeBeginning"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeBeginning"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeBeginning"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
        }

        [TestMethod]
        public void ShouldDeclareIntervalByEnd()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                null, new TIMEInstant(new RDFResource("ex:timeEnd"), DateTime.Parse("2023-03-22T10:35:34Z")));
            timeOntology.DeclareTimeInterval(new RDFResource("ex:feat"), timeInterval);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 36);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:timeEnd")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeEnd")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeEnd"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeEnd"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
        }

        [TestMethod]
        public void ShouldDeclareIntervalByBeginningEnd()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEInstant(new RDFResource("ex:timeBeginning"), DateTime.Parse("2023-03-22T10:35:34Z")),
                new TIMEInstant(new RDFResource("ex:timeEnd"), DateTime.Parse("2023-03-25T10:35:34Z")));
            timeOntology.DeclareTimeInterval(new RDFResource("ex:feat"), timeInterval);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 37);
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:feat"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntr")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.PROPER_INTERVAL));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:timeBeginning")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeBeginning")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeBeginning"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeBeginning"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeBeginning"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntr"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:timeEnd")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeEnd")));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeEnd"), RDFVocabulary.TIME.TEMPORAL_ENTITY));
            Assert.IsTrue(timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, new RDFResource("ex:timeEnd"), RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:timeEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("2023-03-25T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIntervalBecauseNullFeature()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt").DeclareTimeInterval(
                null, new TIMEInterval(new RDFResource("ex:timeIntr"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIntervalBecauseNullInterval()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt").DeclareTimeInterval(
                new RDFResource("ex:feat"), null));

        [TestMethod]
        public void ShouldDeclareIntervalRelation()
        {
            OWLOntology timeOntology = new OWLOntology("ex:timeOnt");
            timeOntology.InitializeTIME();
            TIMEInterval timeInterval1 = new TIMEInterval(new RDFResource("ex:timeIntv1"), XmlConvert.ToTimeSpan("P1Y2M3DT11H5M7S"));
            TIMEInterval timeInterval2 = new TIMEInterval(new RDFResource("ex:timeIntv2"), XmlConvert.ToTimeSpan("P1Y2M3DT11H5M7S"));
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.After);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Before);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Contains);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Disjoint);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.During);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Equals);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.FinishedBy);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Finishes);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.HasInside);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.In);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Meets);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.MetBy);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.OverlappedBy);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Overlaps);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.StartedBy);
            timeOntology.DeclareTimeIntervalRelation(timeInterval1, timeInterval2, TIMEEnums.TIMEIntervalRelation.Starts);

            //Test persistence of TIME knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(new Uri("ex:timeOnt")));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 76);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 71);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 35);
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntv1")));
            Assert.IsTrue(timeOntology.Data.CheckHasIndividual(new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.HAS_INSIDE, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_AFTER, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_BEFORE, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_CONTAINS, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_DISJOINT, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_DURING, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_EQUALS, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_FINISHED_BY, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_FINISHES, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_IN, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_MEETS, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_MET_BY, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_OVERLAPS, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_STARTED_BY, new RDFResource("ex:timeIntv2")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:timeIntv1"), RDFVocabulary.TIME.INTERVAL_STARTS, new RDFResource("ex:timeIntv2")));
            Assert.ThrowsException<OWLException>(() => timeOntology.DeclareTimeIntervalRelation(null, timeInterval2, TIMEEnums.TIMEIntervalRelation.After));
            Assert.ThrowsException<OWLException>(() => timeOntology.DeclareTimeIntervalRelation(timeInterval1, null, TIMEEnums.TIMEIntervalRelation.Before));
        }
        #endregion

        #region Tests (Analyzer)
        [TestMethod]
        public void ShouldThrowExceptionOnGettingTemporalExtentOfFeatureBecauseNullURI()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt").GetTemporalExtentOfFeature(null));

        [TestMethod]
        public void ShouldNotGetTemporalExtentOfInstantFeatureBecauseNoData()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNull(timeInstant.Description);
            Assert.IsNull(timeInstant.Position);
        }

        [DataTestMethod]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTimeStamp", "1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP, "1939-09-01T08:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTime", "1939-09-01T09:00:00+01:00", RDFModelEnums.RDFDatatypes.XSD_DATETIME, "1939-09-01T08:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDDate", "1939-09-01", RDFModelEnums.RDFDatatypes.XSD_DATE, "1939-09-01T00:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDgYear", "1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR, "1939-01-01T00:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDgYearMonth", "1939-09", RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH, "1939-09-01T00:00:00Z")]
        public void ShouldGetTemporalExtentOfInstantFeatureByDateTime(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, string expectedTimeValue)
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource(timeProperty), new RDFTypedLiteral(timeValue, timeDataType)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsTrue(timeInstant.DateTime.HasValue && timeInstant.DateTime.Equals(DateTime.Parse(expectedTimeValue).ToUniversalTime()));
            Assert.IsNull(timeInstant.Description);
            Assert.IsNull(timeInstant.Position);
        }

        [DataTestMethod]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTimeStamp", "1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP, "1939-09-01T08:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTime", "1939-09-01T09:00:00+01:00", RDFModelEnums.RDFDatatypes.XSD_DATETIME, "1939-09-01T08:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDDate", "1939-09-01", RDFModelEnums.RDFDatatypes.XSD_DATE, "1939-09-01T00:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDgYear", "1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR, "1939-01-01T00:00:00Z")]
        [DataRow("http://www.w3.org/2006/time#inXSDgYearMonth", "1939-09", RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH, "1939-09-01T00:00:00Z")]
        public void ShouldGetTemporalExtentOfInstantFeatureByDateTimeThroughInferredProperty(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, string expectedTimeValue)
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_TIME));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), new RDFResource("ex:hasTemporalExtent"), new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource(timeProperty), new RDFTypedLiteral(timeValue, timeDataType)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsTrue(timeInstant.DateTime.HasValue && timeInstant.DateTime.Equals(DateTime.Parse(expectedTimeValue).ToUniversalTime()));
            Assert.IsNull(timeInstant.Description);
            Assert.IsNull(timeInstant.Position);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfInstantFeatureByDescription()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_DATETIME, new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DATETIME_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_SECOND));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEAR, new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH, new RDFTypedLiteral("--09", RDFModelEnums.RDFDatatypes.XSD_GMONTH)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY, new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.XSD_GDAY)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOUR, new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECOND, new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH_OF_YEAR, RDFVocabulary.TIME.GREG.SEPTEMBER));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_WEEK, RDFVocabulary.TIME.FRIDAY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral("244", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNotNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Description.Coordinate);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.UnitType.Equals(RDFVocabulary.TIME.UNIT_SECOND));
            Assert.IsTrue(timeInstant.Description.Coordinate.Year.HasValue && timeInstant.Description.Coordinate.Year == 1939);
            Assert.IsTrue(timeInstant.Description.Coordinate.Month.HasValue && timeInstant.Description.Coordinate.Month == 9);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.MonthOfYear.Equals(RDFVocabulary.TIME.GREG.SEPTEMBER));
            Assert.IsTrue(timeInstant.Description.Coordinate.Day.HasValue && timeInstant.Description.Coordinate.Day == 1);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfWeek.Equals(RDFVocabulary.TIME.FRIDAY));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfYear.HasValue && timeInstant.Description.Coordinate.Metadata.DayOfYear == 244);
            Assert.IsTrue(timeInstant.Description.Coordinate.Hour.HasValue && timeInstant.Description.Coordinate.Hour == 8);
            Assert.IsTrue(timeInstant.Description.Coordinate.Minute.HasValue && timeInstant.Description.Coordinate.Minute == 1);
            Assert.IsTrue(timeInstant.Description.Coordinate.Second.HasValue && timeInstant.Description.Coordinate.Second == 1);
            Assert.IsNull(timeInstant.Position);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfInstantFeatureByDescriptionThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:isInDateTime"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:isInDateTime"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.IN_DATETIME));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:isInDateTime"), new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_SECOND));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEAR, new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_LONG)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH, new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_FLOAT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOUR, new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECOND, new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH_OF_YEAR, RDFVocabulary.TIME.GREG.SEPTEMBER));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_WEEK, RDFVocabulary.TIME.FRIDAY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral("244", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNotNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Description.Coordinate);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.UnitType.Equals(RDFVocabulary.TIME.UNIT_SECOND));
            Assert.IsTrue(timeInstant.Description.Coordinate.Year.HasValue && timeInstant.Description.Coordinate.Year == 1939);
            Assert.IsTrue(timeInstant.Description.Coordinate.Month.HasValue && timeInstant.Description.Coordinate.Month == 9);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.MonthOfYear.Equals(RDFVocabulary.TIME.GREG.SEPTEMBER));
            Assert.IsTrue(timeInstant.Description.Coordinate.Day.HasValue && timeInstant.Description.Coordinate.Day == 1);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfWeek.Equals(RDFVocabulary.TIME.FRIDAY));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfYear.HasValue && timeInstant.Description.Coordinate.Metadata.DayOfYear == 244);
            Assert.IsTrue(timeInstant.Description.Coordinate.Hour.HasValue && timeInstant.Description.Coordinate.Hour == 8);
            Assert.IsTrue(timeInstant.Description.Coordinate.Minute.HasValue && timeInstant.Description.Coordinate.Minute == 1);
            Assert.IsTrue(timeInstant.Description.Coordinate.Second.HasValue && timeInstant.Description.Coordinate.Second == 1);
            Assert.IsNull(timeInstant.Position);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfInstantFeatureByGeneralDescription()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthday"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:AbbyBirthdayTemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtent"), RDFVocabulary.TIME.IN_DATETIME, new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("http://dbpedia.org/resource/Hebrew_calendar")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.YEAR, new RDFTypedLiteral("5761", RDFModelEnums.RDFDatatypes.TIME_GENERALYEAR)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.MONTH, new RDFTypedLiteral("--03", RDFModelEnums.RDFDatatypes.TIME_GENERALMONTH)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.MONTH_OF_YEAR, new RDFResource("ex:Sivan")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.DAY, new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.TIME_GENERALDAY)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral("45", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_WEEK, new RDFResource("ex:RoshChodesh")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.HOUR, new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"), RDFVocabulary.TIME.SECOND, new RDFTypedLiteral("1.25", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:AbbyBirthday"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:AbbyBirthdayTemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNotNull(timeInstant.Description);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.TRS.Equals(new RDFResource("http://dbpedia.org/resource/Hebrew_calendar")));
            Assert.IsTrue(timeInstant.Description.Coordinate.Year.HasValue && timeInstant.Description.Coordinate.Year == 5761);
            Assert.IsTrue(timeInstant.Description.Coordinate.Month.HasValue && timeInstant.Description.Coordinate.Month == 3);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.MonthOfYear.Equals(new RDFResource("ex:Sivan")));
            Assert.IsTrue(timeInstant.Description.Coordinate.Day.HasValue && timeInstant.Description.Coordinate.Day == 1);
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfWeek.Equals(new RDFResource("ex:RoshChodesh")));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfYear.HasValue && timeInstant.Description.Coordinate.Metadata.DayOfYear == 45);
            Assert.IsTrue(timeInstant.Description.Coordinate.Hour.HasValue && timeInstant.Description.Coordinate.Hour == 8);
            Assert.IsTrue(timeInstant.Description.Coordinate.Minute.HasValue && timeInstant.Description.Coordinate.Minute == 1);
            Assert.IsTrue(timeInstant.Description.Coordinate.Second.HasValue && timeInstant.Description.Coordinate.Second == 1.25);
            Assert.IsNull(timeInstant.Position);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfInstantFeatureByNumericPosition()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Position);
            Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMEPositionReferenceSystem.UnixTRS));
            Assert.IsTrue(timeInstant.Position.NumericValue == -957315600);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfInstantFeatureByNumericPositionThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:isInTimePosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:isInTimePosition"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.IN_TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:isInTimePosition"), new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Position);
            Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMEPositionReferenceSystem.UnixTRS));
            Assert.IsTrue(timeInstant.Position.NumericValue == -957315600);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfInstantFeatureByNominalPosition()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NOMINAL_POSITION, new RDFResource("ex:September1939")));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Position);
            Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(timeInstant.Position.NominalValue.Equals(new RDFResource("ex:September1939")));
        }

        [TestMethod]
        public void ShouldNotGetTemporalExtentOfIntervalFeatureBecauseNoData()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByTimeSpan()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral("P6Y", RDFModelEnums.RDFDatatypes.XSD_DURATION)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsTrue(timeInterval.TimeSpan.HasValue && timeInterval.TimeSpan.Equals(XmlConvert.ToTimeSpan("P6Y")));
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByTimeSpanThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_TIME));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), new RDFResource("ex:hasTemporalExtent"), new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral("P6Y", RDFModelEnums.RDFDatatypes.XSD_DURATION)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsTrue(timeInterval.TimeSpan.HasValue && timeInterval.TimeSpan.Equals(XmlConvert.ToTimeSpan("P6Y")));
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByDescription()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEARS, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTHS, new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.WEEKS, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAYS, new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOURS, new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral("8.7", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNotNull(timeInterval.Description);
            Assert.IsNotNull(timeInterval.Description.Extent);
            Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(timeInterval.Description.Extent.Years.HasValue && timeInterval.Description.Extent.Years == 6);
            Assert.IsTrue(timeInterval.Description.Extent.Months.HasValue && timeInterval.Description.Extent.Months == 3);
            Assert.IsTrue(timeInterval.Description.Extent.Weeks.HasValue && timeInterval.Description.Extent.Weeks == 2);
            Assert.IsTrue(timeInterval.Description.Extent.Days.HasValue && timeInterval.Description.Extent.Days == 5);
            Assert.IsTrue(timeInterval.Description.Extent.Hours.HasValue && timeInterval.Description.Extent.Hours == 9);
            Assert.IsTrue(timeInterval.Description.Extent.Minutes.HasValue && timeInterval.Description.Extent.Minutes == 7);
            Assert.IsTrue(timeInterval.Description.Extent.Seconds.HasValue && timeInterval.Description.Extent.Seconds == 8.7);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByDescriptionThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasDurationDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasDurationDescription"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:hasDurationDescription"), new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEARS, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTHS, new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.WEEKS, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAYS, new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOURS, new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral("8.7", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNotNull(timeInterval.Description);
            Assert.IsNotNull(timeInterval.Description.Extent);
            Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(timeInterval.Description.Extent.Years.HasValue && timeInterval.Description.Extent.Years == 6);
            Assert.IsTrue(timeInterval.Description.Extent.Months.HasValue && timeInterval.Description.Extent.Months == 3);
            Assert.IsTrue(timeInterval.Description.Extent.Weeks.HasValue && timeInterval.Description.Extent.Weeks == 2);
            Assert.IsTrue(timeInterval.Description.Extent.Days.HasValue && timeInterval.Description.Extent.Days == 5);
            Assert.IsTrue(timeInterval.Description.Extent.Hours.HasValue && timeInterval.Description.Extent.Hours == 9);
            Assert.IsTrue(timeInterval.Description.Extent.Minutes.HasValue && timeInterval.Description.Extent.Minutes == 7);
            Assert.IsTrue(timeInterval.Description.Extent.Seconds.HasValue && timeInterval.Description.Extent.Seconds == 8.7);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByGeneralDescription()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, RDFVocabulary.TIME.UNIT_MARS_SOL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEARS, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTHS, new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.WEEKS, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAYS, new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOURS, new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral("7.14", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral("8", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNotNull(timeInterval.Description);
            Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(RDFVocabulary.TIME.UNIT_MARS_SOL));
            Assert.IsTrue(timeInterval.Description.Extent.Years.HasValue && timeInterval.Description.Extent.Years == 6);
            Assert.IsTrue(timeInterval.Description.Extent.Months.HasValue && timeInterval.Description.Extent.Months == 3);
            Assert.IsTrue(timeInterval.Description.Extent.Weeks.HasValue && timeInterval.Description.Extent.Weeks == 2);
            Assert.IsTrue(timeInterval.Description.Extent.Days.HasValue && timeInterval.Description.Extent.Days == 5);
            Assert.IsTrue(timeInterval.Description.Extent.Hours.HasValue && timeInterval.Description.Extent.Hours == 9);
            Assert.IsTrue(timeInterval.Description.Extent.Minutes.HasValue && timeInterval.Description.Extent.Minutes == 7.14);
            Assert.IsTrue(timeInterval.Description.Extent.Seconds.HasValue && timeInterval.Description.Extent.Seconds == 8);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByDuration()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION, new RDFResource("ex:WorldWarIITemporalExtentDuration")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_YEAR));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNotNull(timeInterval.Duration);
            Assert.IsTrue(timeInterval.Duration.UnitType.Equals(RDFVocabulary.TIME.UNIT_YEAR));
            Assert.IsTrue(timeInterval.Duration.Value == 6);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByDurationThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasDuration"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:hasDuration"), new RDFResource("ex:WorldWarIITemporalExtentDuration")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_YEAR));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("6.01", RDFModelEnums.RDFDatatypes.XSD_FLOAT)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNotNull(timeInterval.Duration);
            Assert.IsTrue(timeInterval.Duration.UnitType.Equals(RDFVocabulary.TIME.UNIT_YEAR));
            Assert.IsTrue(timeInterval.Duration.Value == 6.01);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByBeginningInstant()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIITemporalExtentBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNotNull(timeInterval.Beginning);
            Assert.IsTrue(timeInterval.Beginning.URI.Equals(new Uri("ex:WorldWarIITemporalExtentBeginning")));
            Assert.IsTrue(timeInterval.Beginning.DateTime.HasValue && timeInterval.Beginning.DateTime.Equals(DateTime.Parse("1939-09-01T08:00:00Z").ToUniversalTime()));
            Assert.IsNull(timeInterval.Beginning.Description);
            Assert.IsNull(timeInterval.Beginning.Position);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByBeginningInstantThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:beginsAt"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:beginsAt"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_BEGINNING));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:beginsAt"), new RDFResource("ex:WorldWarIITemporalExtentBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNotNull(timeInterval.Beginning);
            Assert.IsTrue(timeInterval.Beginning.URI.Equals(new Uri("ex:WorldWarIITemporalExtentBeginning")));
            Assert.IsTrue(timeInterval.Beginning.DateTime.HasValue && timeInterval.Beginning.DateTime.Equals(DateTime.Parse("1939-09-01T08:00:00Z").ToUniversalTime()));
            Assert.IsNull(timeInterval.Beginning.Description);
            Assert.IsNull(timeInterval.Beginning.Position);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByEndInstant()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIITemporalExtentEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsTrue(timeInterval.End.URI.Equals(new Uri("ex:WorldWarIITemporalExtentEnd")));
            Assert.IsTrue(timeInterval.End.DateTime.HasValue && timeInterval.End.DateTime.Equals(DateTime.Parse("1945-09-02T08:00:00Z").ToUniversalTime()));
            Assert.IsNull(timeInterval.End.Description);
            Assert.IsNull(timeInterval.End.Position);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByEndInstantThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:endsAt"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:endsAt"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_END));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:endsAt"), new RDFResource("ex:WorldWarIITemporalExtentEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsTrue(timeInterval.End.URI.Equals(new Uri("ex:WorldWarIITemporalExtentEnd")));
            Assert.IsTrue(timeInterval.End.DateTime.HasValue && timeInterval.End.DateTime.Equals(DateTime.Parse("1945-09-02T08:00:00Z").ToUniversalTime()));
            Assert.IsNull(timeInterval.End.Description);
            Assert.IsNull(timeInterval.End.Position);
        }

        [TestMethod]
        public void ShouldGetTemporalExtentOfIntervalFeatureByBeginningEndInstants()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIITemporalExtentBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1939-09-01T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIITemporalExtentEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalExtentOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNotNull(timeInterval.Beginning);
            Assert.IsTrue(timeInterval.Beginning.URI.Equals(new Uri("ex:WorldWarIITemporalExtentBeginning")));
            Assert.IsTrue(timeInterval.Beginning.DateTime.HasValue && timeInterval.Beginning.DateTime.Equals(DateTime.Parse("1939-09-01T08:01:01Z").ToUniversalTime()));
            Assert.IsNull(timeInterval.Beginning.Description);
            Assert.IsNull(timeInterval.Beginning.Position);
            Assert.IsNotNull(timeInterval.End);
            Assert.IsTrue(timeInterval.End.URI.Equals(new Uri("ex:WorldWarIITemporalExtentEnd")));
            Assert.IsTrue(timeInterval.End.DateTime.HasValue && timeInterval.End.DateTime.Equals(DateTime.Parse("1945-09-02T08:01:01Z").ToUniversalTime()));
            Assert.IsNull(timeInterval.End.Description);
            Assert.IsNull(timeInterval.End.Position);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingInstantCoordinateBecauseNullInstantURI()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt").GetInstantCoordinate(null));

        [DataTestMethod]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTimeStamp", "1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP, 1939, 9, 1, 8, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTime", "1939-09-01T09:00:00+01:00", RDFModelEnums.RDFDatatypes.XSD_DATETIME, 1939, 9, 1, 8, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDDate", "1939-09-01", RDFModelEnums.RDFDatatypes.XSD_DATE, 1939, 9, 1, 0, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDgYear", "1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR, 1939, 1, 1, 0, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDgYearMonth", "1939-09", RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH, 1939, 9, 1, 0, 0, 0)]
        public void ShouldGetInstantCoordinateByDateTime(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, 
            double expectedYear, double expectedMonth, double expectedDay, double expectedHour, double expectedMinute, double expectedSecond)
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource(timeProperty), new RDFTypedLiteral(timeValue, timeDataType)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate timeCoordinate = timeOntology.GetInstantCoordinate(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == expectedYear);
            Assert.IsTrue(timeCoordinate.Month == expectedMonth);
            Assert.IsTrue(timeCoordinate.Day == expectedDay);
            Assert.IsTrue(timeCoordinate.Hour == expectedHour);
            Assert.IsTrue(timeCoordinate.Minute == expectedMinute);
            Assert.IsTrue(timeCoordinate.Second == expectedSecond);
        }

        [TestMethod]
        public void ShouldGetInstantCoordinateByDescription()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_DATETIME, new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DATETIME_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_SECOND));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEAR, new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH, new RDFTypedLiteral("--09", RDFModelEnums.RDFDatatypes.XSD_GMONTH)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY, new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.XSD_GDAY)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOUR, new RDFTypedLiteral("07", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral("60", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECOND, new RDFTypedLiteral("61", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH_OF_YEAR, RDFVocabulary.TIME.GREG.SEPTEMBER));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_WEEK, RDFVocabulary.TIME.FRIDAY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral("244", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate timeCoordinate = timeOntology.GetInstantCoordinate(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == 1939);
            Assert.IsTrue(timeCoordinate.Month == 9);
            Assert.IsTrue(timeCoordinate.Day == 1);
            Assert.IsTrue(timeCoordinate.Hour == 8);
            Assert.IsTrue(timeCoordinate.Minute == 1);
            Assert.IsTrue(timeCoordinate.Second == 1);
        }

        [TestMethod]
        public void ShouldGetInstantCoordinateByDescriptionWithNormalization()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_DATETIME, new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DATETIME_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_SECOND));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEAR, new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH, new RDFTypedLiteral("--09", RDFModelEnums.RDFDatatypes.XSD_GMONTH)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY, new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.XSD_GDAY)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOUR, new RDFTypedLiteral("8", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECOND, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTH_OF_YEAR, RDFVocabulary.TIME.GREG.SEPTEMBER));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_WEEK, RDFVocabulary.TIME.FRIDAY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral("244", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECalendarReferenceSystem myCalendarTRS = new TIMECalendarReferenceSystem(
                new RDFResource("ex:MyCalendarTRS"),
                new TIMECalendarReferenceSystemMetrics(100, 100, 50, new uint[] { 20, 20, 12, 18 }));
            TIMEReferenceSystemRegistry.AddTRS(myCalendarTRS);
            TIMECoordinate timeCoordinate = timeOntology.GetInstantCoordinate(new RDFResource("ex:WorldWarIITemporalExtent"), myCalendarTRS);

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == 1941);
            Assert.IsTrue(timeCoordinate.Month == 1);
            Assert.IsTrue(timeCoordinate.Day == 1);
            Assert.IsTrue(timeCoordinate.Hour == 8);
            Assert.IsTrue(timeCoordinate.Minute == 1);
            Assert.IsTrue(timeCoordinate.Second == 1);
        }

        [TestMethod]
        public void ShouldGetInstantCoordinateByNumericPosition()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate timeCoordinate = timeOntology.GetInstantCoordinate(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == 1939);
            Assert.IsTrue(timeCoordinate.Month == 9);
            Assert.IsTrue(timeCoordinate.Day == 1);
            Assert.IsTrue(timeCoordinate.Hour == 8);
            Assert.IsTrue(timeCoordinate.Minute == 1);
            Assert.IsTrue(timeCoordinate.Second == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingInstantCoordinateByNumericPositionBecauseUnregisteredTRS()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("ex:MyUnregisteredTRS")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });

            Assert.ThrowsException<OWLException>(() => timeOntology.GetInstantCoordinate(new RDFResource("ex:WorldWarIITemporalExtent")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingInstantCoordinateByNumericPositionBecauseCalendarTRSDetected()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("ex:MyCalendarTRS")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMEReferenceSystemRegistry.AddTRS(new TIMECalendarReferenceSystem(
                new RDFResource("ex:MyCalendarTRS"), 
                new TIMECalendarReferenceSystemMetrics(100, 100, 50, new uint[] { 20, 20, 12, 18 })));

            Assert.ThrowsException<OWLException>(() => timeOntology.GetInstantCoordinate(new RDFResource("ex:WorldWarIITemporalExtent")));
        }

        [TestMethod]
        public void ShouldGetInstantCoordinateByNominalPosition()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NOMINAL_POSITION, new RDFResource("ex:September1939")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:September1939"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:September1939"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:September1939Beginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:September1939Beginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:September1939Beginning"), RDFVocabulary.TIME.IN_XSD_DATE, new RDFTypedLiteral("1939-09-01Z", RDFModelEnums.RDFDatatypes.XSD_DATE)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate timeCoordinate = timeOntology.GetInstantCoordinate(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == 1939);
            Assert.IsTrue(timeCoordinate.Month == 9);
            Assert.IsTrue(timeCoordinate.Day == 1);
            Assert.IsTrue(timeCoordinate.Hour == 0);
            Assert.IsTrue(timeCoordinate.Minute == 0);
            Assert.IsTrue(timeCoordinate.Second == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingIntervalExtentBecauseNullIntervalURI()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt").GetIntervalExtent(null));

        [TestMethod]
        public void ShouldGetIntervalExtentByTimeSpan()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral("P6Y1M1DT1H1M61S", RDFModelEnums.RDFDatatypes.XSD_DURATION)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMEExtent timeExtent = timeOntology.GetIntervalExtent(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeExtent);
            Assert.IsTrue(timeExtent.Years == 0);
            Assert.IsTrue(timeExtent.Months == 0);
            Assert.IsTrue(timeExtent.Weeks == 0);
            Assert.IsTrue(timeExtent.Days == 2221);
            Assert.IsTrue(timeExtent.Hours == 1);
            Assert.IsTrue(timeExtent.Minutes == 2);
            Assert.IsTrue(timeExtent.Seconds == 1);
        }

        [TestMethod]
        public void ShouldGetIntervalExtentByDescription()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_SECOND));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEARS, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTHS, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.WEEKS, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAYS, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOURS, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral("61", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMEExtent timeExtent = timeOntology.GetIntervalExtent(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeExtent);
            Assert.IsTrue(timeExtent.Years == 0);
            Assert.IsTrue(timeExtent.Months == 0);
            Assert.IsTrue(timeExtent.Weeks == 0);
            Assert.IsTrue(timeExtent.Days == 2228);
            Assert.IsTrue(timeExtent.Hours == 1);
            Assert.IsTrue(timeExtent.Minutes == 2);
            Assert.IsTrue(timeExtent.Seconds == 1);
        }

        [TestMethod]
        public void ShouldGetIntervalExtentByDuration()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION, new RDFResource("ex:WorldWarIITemporalExtentDuration")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_YEAR));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("6.01", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMEExtent timeExtent = timeOntology.GetIntervalExtent(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeExtent);
            Assert.IsTrue(timeExtent.Years == 0);
            Assert.IsTrue(timeExtent.Months == 0);
            Assert.IsTrue(timeExtent.Weeks == 0);
            Assert.IsTrue(timeExtent.Days == 2193);
            Assert.IsTrue(timeExtent.Hours == 15);
            Assert.IsTrue(timeExtent.Minutes == 35);
            Assert.IsTrue(timeExtent.Seconds == 59);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingIntervalExtentByDurationBecauseUnknownUnitType()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION, new RDFResource("ex:WorldWarIITemporalExtentDuration")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.UNIT_TYPE, new RDFResource("ex:UnknownUnit")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("6.01", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });

            Assert.ThrowsException<OWLException>(() => timeOntology.GetIntervalExtent(new RDFResource("ex:WorldWarIITemporalExtent")));
        }

        [TestMethod]
        public void ShouldGetIntervalExtentByBeginningEnd()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIEndPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEndPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEndPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEndPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-767807939", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMEExtent timeExtent = timeOntology.GetIntervalExtent(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeExtent);
            Assert.IsTrue(timeExtent.Years == 0);
            Assert.IsTrue(timeExtent.Months == 0);
            Assert.IsTrue(timeExtent.Weeks == 0);
            Assert.IsTrue(timeExtent.Days == 2191);
            Assert.IsTrue(timeExtent.Hours == 0);
            Assert.IsTrue(timeExtent.Minutes == 0);
            Assert.IsTrue(timeExtent.Seconds == 0);
        }

        [TestMethod]
        public void ShouldNotGetIntervalExtentBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMEExtent timeExtent = timeOntology.GetIntervalExtent(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(timeExtent);
        }

        [TestMethod]
        public void ShouldGetBeginningOfInterval()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(tc);
            Assert.IsTrue(tc.Year == 1939);
            Assert.IsTrue(tc.Month == 9);
            Assert.IsTrue(tc.Day == 1);
            Assert.IsTrue(tc.Hour == 8);
            Assert.IsTrue(tc.Minute == 1);
            Assert.IsTrue(tc.Second == 1);
        }

        [TestMethod]
        public void ShouldNotGetBeginningOfIntervalBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public void ShouldGetBeginningOfIntervalIndirectly()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_STARTS, new RDFResource("ex:WorldWarIITemporalExtentBeginning"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIExtentBeginningInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentBeginningInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentBeginningInstant"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningInstantPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(tc);
            Assert.IsTrue(tc.Year == 1939);
            Assert.IsTrue(tc.Month == 9);
            Assert.IsTrue(tc.Day == 1);
            Assert.IsTrue(tc.Hour == 8);
            Assert.IsTrue(tc.Minute == 1);
            Assert.IsTrue(tc.Second == 1);
        }

        [TestMethod]
        public void ShouldNotGetBeginningOfIntervalIndirectlyBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_STARTS, new RDFResource("ex:WorldWarIITemporalExtentBeginning"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIExtentBeginningInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentBeginningInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentBeginningInstant"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningInstantPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public void ShouldGetBeginningOfIntervalIndirectlyMeeting()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_MET_BY, new RDFResource("ex:PrecedingWorldWarIITemporalExtent"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:PrecedingWorldWarIIExtentEndInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIExtentEndInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIExtentEndInstant"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(tc);
            Assert.IsTrue(tc.Year == 1939);
            Assert.IsTrue(tc.Month == 9);
            Assert.IsTrue(tc.Day == 1);
            Assert.IsTrue(tc.Hour == 8);
            Assert.IsTrue(tc.Minute == 1);
            Assert.IsTrue(tc.Second == 1);
        }

        [TestMethod]
        public void ShouldNotGetBeginningOfIntervalIndirectlyMeetingBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_MET_BY, new RDFResource("ex:PrecedingWorldWarIITemporalExtent"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:PrecedingWorldWarIIExtentEndInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIExtentEndInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIExtentEndInstant"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTRS));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public void ShouldGetEndOfInterval()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(tc);
            Assert.IsTrue(tc.Year == 1945);
            Assert.IsTrue(tc.Month == 9);
            Assert.IsTrue(tc.Day == 2);
            Assert.IsTrue(tc.Hour == 8);
            Assert.IsTrue(tc.Minute == 1);
            Assert.IsTrue(tc.Second == 1);
        }

        [TestMethod]
        public void ShouldNotGetEndOfIntervalBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public void ShouldGetEndOfIntervalIndirectly()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_FINISHES, new RDFResource("ex:WorldWarIITemporalExtentEnd"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIExtentEndInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentEndInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentEndInstant"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));

            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(tc);
            Assert.IsTrue(tc.Year == 1945);
            Assert.IsTrue(tc.Month == 9);
            Assert.IsTrue(tc.Day == 2);
            Assert.IsTrue(tc.Hour == 8);
            Assert.IsTrue(tc.Minute == 1);
            Assert.IsTrue(tc.Second == 1);
        }

        [TestMethod]
        public void ShouldNotGetEndOfIntervalIndirectlyBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_FINISHES, new RDFResource("ex:WorldWarIITemporalExtentEnd"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIExtentEndInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentEndInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));

            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public void ShouldGetEndOfIntervalIndirectlyMeeting()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_MEETS, new RDFResource("ex:FollowingWorldWarIITemporalExtent"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));

            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(tc);
            Assert.IsTrue(tc.Year == 1945);
            Assert.IsTrue(tc.Month == 9);
            Assert.IsTrue(tc.Day == 2);
            Assert.IsTrue(tc.Hour == 8);
            Assert.IsTrue(tc.Minute == 1);
            Assert.IsTrue(tc.Second == 1);
        }

        [TestMethod]
        public void ShouldNotGetEndOfIntervalIndirectlyMeetingBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_MEETS, new RDFResource("ex:FollowingWorldWarIITemporalExtent"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));

            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [DataTestMethod]
        [DataRow(TIMEEnums.TIMEIntervalRelation.After)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Before)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Contains)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Disjoint)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.During)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Equals)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.FinishedBy)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Finishes)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.HasInside)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.In)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Meets)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.MetBy)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.NotDisjoint)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.OverlappedBy)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Overlaps)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.StartedBy)]
        [DataRow(TIMEEnums.TIMEIntervalRelation.Starts)]
        public void ShouldGetRelatedIntervals(TIMEEnums.TIMEIntervalRelation relation)
        {
            RDFResource tiRelation = null;
            switch (relation)
            {
                case TIMEEnums.TIMEIntervalRelation.After:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_AFTER;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Before:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_BEFORE;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Contains:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_CONTAINS;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Disjoint:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_DISJOINT;
                    break;
                case TIMEEnums.TIMEIntervalRelation.During:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_DURING;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Equals:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_EQUALS;
                    break;
                case TIMEEnums.TIMEIntervalRelation.FinishedBy:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_FINISHED_BY;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Finishes:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_FINISHES;
                    break;
                case TIMEEnums.TIMEIntervalRelation.HasInside:
                    tiRelation = RDFVocabulary.TIME.HAS_INSIDE;
                    break;
                case TIMEEnums.TIMEIntervalRelation.In:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_IN;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Meets:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_MEETS;
                    break;
                case TIMEEnums.TIMEIntervalRelation.MetBy:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_MET_BY;
                    break;
                case TIMEEnums.TIMEIntervalRelation.NotDisjoint:
                    tiRelation = RDFVocabulary.TIME.NOT_DISJOINT;
                    break;
                case TIMEEnums.TIMEIntervalRelation.OverlappedBy:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Overlaps:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_OVERLAPS;
                    break;
                case TIMEEnums.TIMEIntervalRelation.StartedBy:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_STARTED_BY;
                    break;
                case TIMEEnums.TIMEIntervalRelation.Starts:
                    tiRelation = RDFVocabulary.TIME.INTERVAL_STARTS;
                    break;
            }

            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:Feature"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:IntervalA")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:IntervalA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:IntervalA"), tiRelation, new RDFResource("ex:IntervalB")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:IntervalB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            OWLOntology timeOntology = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { EnableTIMESupport=true });
            List<RDFResource> relatedIntervals = timeOntology.GetRelatedIntervals(new RDFResource("ex:IntervalA"), relation);

            Assert.IsNotNull(relatedIntervals);
            Assert.IsTrue(relatedIntervals.Single().Equals(new RDFResource("ex:IntervalB")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingRelatedIntervalsBecauseNullURI()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:timeOnt").GetRelatedIntervals(null, TIMEEnums.TIMEIntervalRelation.Meets));

        //E2E Tests

        [TestMethod]
        public void ShouldGetTemporalExtentOfFeatureOccurringInArcheanEra()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Stream resourceStream = Assembly.GetExecutingAssembly()
                                                   .GetManifestResourceStream("OWLSharp.Test.Extensions.TIME.Resources.GeologicTimeExample.ttl"))
            {
                using (StreamReader resourceReader = new StreamReader(resourceStream))
                {
                    using (StreamWriter memoryStreamWriter = new StreamWriter(memoryStream))
                        memoryStreamWriter.Write(resourceReader.ReadToEnd());
                }
            }

            RDFGraph gtsGraph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, new MemoryStream(memoryStream.ToArray()));
            OWLOntology gtsOntology = OWLOntology.FromRDFGraph(gtsGraph, new OWLOntologyLoaderOptions() { EnableTIMESupport = true });
            gtsOntology.Data.DeclareIndividual(new RDFResource("ex:ArcheanFeature"));
            gtsOntology.Data.DeclareObjectAssertion(new RDFResource("ex:ArcheanFeature"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("http://example.org/geologic/Archean"));
            List<TIMEEntity> archeanTimeExtent = gtsOntology.GetTemporalExtentOfFeature(new RDFResource("ex:ArcheanFeature"));

            Assert.IsNotNull(archeanTimeExtent);
            Assert.IsTrue(archeanTimeExtent.Count == 1);
            Assert.IsTrue(archeanTimeExtent.Single() is TIMEInterval);

            TIMEInterval archeanTimeInterval = (TIMEInterval)archeanTimeExtent.Single();

            Assert.IsNull(archeanTimeInterval.TimeSpan);
            Assert.IsNull(archeanTimeInterval.Description);
            Assert.IsNull(archeanTimeInterval.Duration);
            Assert.IsNotNull(archeanTimeInterval.Beginning);
            Assert.IsNull(archeanTimeInterval.Beginning.DateTime);
            Assert.IsNull(archeanTimeInterval.Beginning.Description);
            Assert.IsNotNull(archeanTimeInterval.Beginning.Position);
            Assert.IsTrue(archeanTimeInterval.Beginning.Position.TRS.Equals(TIMEPositionReferenceSystem.GeologicTRS));
            Assert.IsTrue(archeanTimeInterval.Beginning.Position.NumericValue == 4000);
            Assert.IsNotNull(archeanTimeInterval.End);
            Assert.IsNull(archeanTimeInterval.End.DateTime);
            Assert.IsNull(archeanTimeInterval.End.Description);
            Assert.IsNotNull(archeanTimeInterval.End.Position);
            Assert.IsTrue(archeanTimeInterval.End.Position.TRS.Equals(TIMEPositionReferenceSystem.GeologicTRS));
            Assert.IsTrue(archeanTimeInterval.End.Position.NumericValue == 2500);
        }

        [TestMethod]
        public void ShouldGetCoordinatesAndExtentOfArcheanEra()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Stream resourceStream = Assembly.GetExecutingAssembly()
                                                   .GetManifestResourceStream("OWLSharp.Test.Extensions.TIME.Resources.GeologicTimeExample.ttl"))
            {
                using (StreamReader resourceReader = new StreamReader(resourceStream))
                {
                    using (StreamWriter memoryStreamWriter = new StreamWriter(memoryStream))
                        memoryStreamWriter.Write(resourceReader.ReadToEnd());
                }
            }

            RDFGraph gtsGraph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, new MemoryStream(memoryStream.ToArray()));
            OWLOntology gtsOntology = OWLOntology.FromRDFGraph(gtsGraph, new OWLOntologyLoaderOptions() { EnableTIMESupport = true });
            gtsOntology.Data.DeclareIndividual(new RDFResource("ex:ArcheanFeature"));
            gtsOntology.Data.DeclareObjectAssertion(new RDFResource("ex:ArcheanFeature"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("http://example.org/geologic/Archean"));
            
            TIMECoordinate archeanBeginning = gtsOntology.GetBeginningOfInterval(new RDFResource("http://example.org/geologic/Archean"));
            Assert.IsNotNull(archeanBeginning);
            Assert.IsTrue(archeanBeginning.Year == -3_999_998_050);

            TIMECoordinate archeanEnd = gtsOntology.GetEndOfInterval(new RDFResource("http://example.org/geologic/Archean"));
            Assert.IsNotNull(archeanEnd);
            Assert.IsTrue(archeanEnd.Year == -2_499_998_050);

            TIMEExtent archeanExtent = gtsOntology.GetIntervalExtent(new RDFResource("http://example.org/geologic/Archean"));
            Assert.IsNotNull(archeanExtent);
            Assert.IsTrue(archeanExtent.Days == 547_500_000_000);
        }

        [TestMethod]
        public void ShouldAnswerAllenQuestionsAboutArcheanEra()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Stream resourceStream = Assembly.GetExecutingAssembly()
                                                   .GetManifestResourceStream("OWLSharp.Test.Extensions.TIME.Resources.GeologicTimeExample.ttl"))
            {
                using (StreamReader resourceReader = new StreamReader(resourceStream))
                {
                    using (StreamWriter memoryStreamWriter = new StreamWriter(memoryStream))
                        memoryStreamWriter.Write(resourceReader.ReadToEnd());
                }
            }

            RDFGraph gtsGraph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, new MemoryStream(memoryStream.ToArray()));
            OWLOntology gtsOntology = OWLOntology.FromRDFGraph(gtsGraph, new OWLOntologyLoaderOptions() { EnableTIMESupport = true });
            gtsOntology.Data.DeclareIndividual(new RDFResource("ex:ArcheanFeature"));
            gtsOntology.Data.DeclareObjectAssertion(new RDFResource("ex:ArcheanFeature"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("http://example.org/geologic/Archean"));

            Assert.IsTrue(TIMEIntervalHelper.CheckMeets(
                gtsOntology, 
                new RDFResource("http://example.org/geologic/Archean"), 
                new RDFResource("http://example.org/geologic/Proterozoic")));
            Assert.IsTrue(TIMEIntervalHelper.CheckMetBy(
                gtsOntology,
                new RDFResource("http://example.org/geologic/Archean"),
                new RDFResource("http://example.org/geologic/Hadean")));
        }
        #endregion
    }
}