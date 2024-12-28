/*
   Copyright 2014-2024 Marco De Salvo

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEHelperTest
    {
        #region Tests (Declarer)
        [TestMethod]
        public void ShouldDeclareInstantByDateTime()
        {
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            timeOntology.DeclareInstantFeature(new RDFResource("ex:feat"), 
                new TIMEInstant(new RDFResource("ex:timeInst"), DateTime.Parse("2023-03-22T10:35:34Z")));

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInst"))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
                new OWLNamedIndividual(new RDFResource("ex:feat")),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.INSTANT),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")),
                new OWLLiteral(new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)))));
        }

        [TestMethod]
        public void ShouldDeclareInstantByDescriptionFromCoordinate()
        {
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            timeOntology.DeclareInstantFeature(new RDFResource("ex:feat"), 
                new TIMEInstant(new RDFResource("ex:timeInst"), 
                    new TIMEInstantDescription(
                        new RDFResource("ex:timeInstDesc"),
                        new TIMECoordinate(2023d, 3d, 22d, 10d, 35d, 34d,
                            new TIMECoordinateMetadata(
                                new RDFResource("ex:TRS"),
                                new RDFResource("ex:Unit"),
                                RDFVocabulary.TIME.GREG.MARCH,
                                RDFVocabulary.TIME.WEDNESDAY,
                                81)))));

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInst"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInstDesc"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:TRS"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:Unit"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(RDFVocabulary.TIME.GREG.MARCH)));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(RDFVocabulary.TIME.WEDNESDAY)));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
                new OWLNamedIndividual(new RDFResource("ex:feat")),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.INSTANT),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.YEAR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("2023", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.MONTH),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.DAY),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.HOUR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("35", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.SECOND),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLNamedIndividual(new RDFResource("ex:Unit")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLNamedIndividual(new RDFResource("ex:TRS")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLNamedIndividual(RDFVocabulary.TIME.GREG.MARCH))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLNamedIndividual(RDFVocabulary.TIME.WEDNESDAY))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("81", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)))));
        }

        [TestMethod]
        public void ShouldDeclareInstantByDescriptionFromDateTime()
        {
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            timeOntology.DeclareInstantFeature(new RDFResource("ex:feat"), 
                new TIMEInstant(new RDFResource("ex:timeInst"), 
                    new TIMEInstantDescription(new RDFResource("ex:timeInstDesc"), DateTime.Parse("2023-03-22T10:35:34Z"))));

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInst"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInstDesc"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(RDFVocabulary.TIME.GREG.MARCH)));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(RDFVocabulary.TIME.WEDNESDAY)));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
                new OWLNamedIndividual(new RDFResource("ex:feat")),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.INSTANT),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.YEAR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("2023", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.MONTH),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.DAY),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("22", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.HOUR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("35", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.SECOND),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLNamedIndividual(RDFVocabulary.TIME.GREG.MARCH))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLNamedIndividual(RDFVocabulary.TIME.WEDNESDAY))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR),
                new OWLNamedIndividual(new RDFResource("ex:timeInstDesc")),
                new OWLLiteral(new RDFTypedLiteral("81", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER)))));
        }

        [TestMethod]
        public void ShouldDeclareInstantByNumericPosition()
        {
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            timeOntology.DeclareInstantFeature(new RDFResource("ex:feat"), 
                new TIMEInstant(new RDFResource("ex:timeInst"),
                    new TIMEInstantPosition(new RDFResource("ex:timeInstPos"), TIMEPositionReferenceSystem.UnixTime, 1679477734))); //2023-03-22T09:35:34Z

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInst"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInstPos"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Unix_time"))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
                new OWLNamedIndividual(new RDFResource("ex:feat")),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.INSTANT),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPos")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPos")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPos")),
                new OWLLiteral(new RDFTypedLiteral("1679477734", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.TRS),
                new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Unix_time")))));
        }

        [TestMethod]
        public void ShouldDeclareInstantByNumericPositionWithUncertainty()
        {
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEIntervalDuration timeInstantUncertainty = new TIMEIntervalDuration(
                new RDFResource("ex:timeInstPosUnc"), RDFVocabulary.TIME.UNIT_SECOND, 4.04);
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"),
                new TIMEInstantPosition(new RDFResource("ex:timeInstPos"),
                    TIMEPositionReferenceSystem.UnixTime,
                    1679477734)); //2023-03-22T09:35:34Z
            timeInstant.Position.PositionalUncertainty = timeInstantUncertainty; 
            timeOntology.DeclareInstantFeature(new RDFResource("ex:feat"), timeInstant);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInst"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInstPos"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInstPosUnc"))));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(TIMEUnit.Second)));
            Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Unix_time"))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
                new OWLNamedIndividual(new RDFResource("ex:feat")),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.INSTANT),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
                new OWLNamedIndividual(new RDFResource("ex:timeInst")),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPos")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPos")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPos")),
                new OWLLiteral(new RDFTypedLiteral("1679477734", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.TRS),
                new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Unix_time")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.TEMPORAL_UNIT),
                new OWLNamedIndividual(TIMEUnit.Second))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.DURATION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPosUnc")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPos")),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPosUnc")))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPosUnc")),
                new OWLNamedIndividual(TIMEUnit.Second))));
            Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
                new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
                new OWLNamedIndividual(new RDFResource("ex:timeInstPosUnc")),
                new OWLLiteral(new RDFTypedLiteral("4.04", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)))));
        }

/*
        [TestMethod]
        public void ShouldDeclareInstantByNominalPosition()
        {
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"),
                new TIMEInstantPosition(new RDFResource("ex:timeInstPos"),
                    new RDFResource("http://example.org/geologic/"),
                    new RDFResource("http://example.org/geologic/Archean")));
            timeOntology.DeclareInstantFeature(new RDFResource("ex:feat"), timeInstant);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            => Assert.ThrowsException<OWLException>(() => new OWLOntology(new Uri("ex:timeOnt"))
                .DeclareInstantFeature(null, new TIMEInstant(new RDFResource("ex:timeInst"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringInstantBecauseNullInstant()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology(new Uri("ex:timeOnt"))
                .DeclareInstantFeature(new RDFResource("ex:feat"), null));

        [TestMethod]
        public void ShouldDeclareIntervalByTimeSpan()
        {
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                XmlConvert.ToTimeSpan("P1Y2M3DT11H5M7S"));
            timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEIntervalDescription(new RDFResource("ex:timeIntrDesc"),
                    new TIMEExtent(1, 2, 3, 4, 5, 6, 7, new TIMEExtentMetadata(new RDFResource("ex:TRS")))));
            timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEIntervalDescription(new RDFResource("ex:timeIntrDesc"),
                    new TIMEExtent(XmlConvert.ToTimeSpan("P1Y1M1DT1H1M1S"))));
            timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEIntervalDuration(new RDFResource("ex:timeIntrDur"), RDFVocabulary.TIME.UNIT_DAY, 77));
            timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEInstant(new RDFResource("ex:timeBeginning"), DateTime.Parse("2023-03-22T10:35:34Z")), null);
            timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                null, new TIMEInstant(new RDFResource("ex:timeEnd"), DateTime.Parse("2023-03-22T10:35:34Z")));
            timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));
            
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntr"),
                new TIMEInstant(new RDFResource("ex:timeBeginning"), DateTime.Parse("2023-03-22T10:35:34Z")),
                new TIMEInstant(new RDFResource("ex:timeEnd"), DateTime.Parse("2023-03-25T10:35:34Z")));
            timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
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
            => Assert.ThrowsException<OWLException>(() => new OWLOntology(new Uri("ex:timeOnt")).DeclareIntervalFeature(
                null, new TIMEInterval(new RDFResource("ex:timeIntr"))));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIntervalBecauseNullInterval()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology(new Uri("ex:timeOnt")).DeclareIntervalFeature(
                new RDFResource("ex:feat"), null));
        #endregion

        #region Tests (Analyzer)
        [TestMethod]
        public void ShouldThrowExceptionOnGettingTemporalExtentOfFeatureBecauseNullURI()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology(new Uri("ex:timeOnt")).GetTemporalRepresentationsOfFeature(null));

        [TestMethod]
        public async Task ShouldNotGetTemporalExtentOfInstantFeatureBecauseNoData()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfInstantFeatureByDateTime(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, string expectedTimeValue)
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource(timeProperty), new RDFTypedLiteral(timeValue, timeDataType)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfInstantFeatureByDateTimeThroughInferredProperty(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, string expectedTimeValue)
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_TIME));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), new RDFResource("ex:hasTemporalExtent"), new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource(timeProperty), new RDFTypedLiteral(timeValue, timeDataType)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfInstantFeatureByDescription()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfInstantFeatureByDescriptionThroughInferredProperty()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfInstantFeatureByGeneralDescription()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:AbbyBirthday"));

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
        public async Task ShouldGetTemporalExtentOfInstantFeatureByNumericPosition()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Position);
            Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMEPositionReferenceSystem.UnixTime));
            Assert.IsTrue(timeInstant.Position.NumericValue == -957315600);
        }

        [TestMethod]
        public async Task ShouldGetTemporalExtentOfInstantFeatureByNumericPositionThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:isInTimePosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:isInTimePosition"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.IN_TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:isInTimePosition"), new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInstant);

            TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Position);
            Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMEPositionReferenceSystem.UnixTime));
            Assert.IsTrue(timeInstant.Position.NumericValue == -957315600);
        }

        [TestMethod]
        public async Task ShouldGetTemporalExtentOfInstantFeatureByNominalPosition()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NOMINAL_POSITION, new RDFResource("ex:September1939")));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldNotGetTemporalExtentOfIntervalFeatureBecauseNoData()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByTimeSpan()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral("P6Y", RDFModelEnums.RDFDatatypes.XSD_DURATION)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByTimeSpanThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasTemporalExtent"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_TIME));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), new RDFResource("ex:hasTemporalExtent"), new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral("P6Y", RDFModelEnums.RDFDatatypes.XSD_DURATION)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByDescription()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByDescriptionThroughInferredProperty()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByGeneralDescription()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new RDFResource("ex:WorldWarIITemporalExtentDescription")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HAS_TRS, TIMEUnit.MarsSol));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.YEARS, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MONTHS, new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.WEEKS, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.DAYS, new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.HOURS, new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral("7.14", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDescription"), RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral("8", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

            Assert.IsNotNull(timeEntities);
            Assert.IsTrue(timeEntities.Count == 1);
            Assert.IsTrue(timeEntities.Single() is TIMEInterval);

            TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalExtent")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNotNull(timeInterval.Description);
            Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(TIMEUnit.MarsSol));
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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByDuration()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION, new RDFResource("ex:WorldWarIITemporalExtentDuration")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_YEAR));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByDurationThroughInferredProperty()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByBeginningInstant()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIITemporalExtentBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByBeginningInstantThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:beginsAt"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:beginsAt"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_BEGINNING));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:beginsAt"), new RDFResource("ex:WorldWarIITemporalExtentBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentBeginning"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByEndInstant()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIITemporalExtentEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByEndInstantThroughInferredProperty()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:endsAt"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:endsAt"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, RDFVocabulary.TIME.HAS_END));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource("ex:endsAt"), new RDFResource("ex:WorldWarIITemporalExtentEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
        public async Task ShouldGetTemporalExtentOfIntervalFeatureByBeginningEndInstants()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            List<TIMEEntity> timeEntities = timeOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:WorldWarII"));

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
            => Assert.ThrowsException<OWLException>(() => new OWLOntology(new Uri("ex:timeOnt")).GetCoordinateOfInstant(null));

        [DataTestMethod]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTimeStamp", "1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP, 1939, 9, 1, 8, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDDateTime", "1939-09-01T09:00:00+01:00", RDFModelEnums.RDFDatatypes.XSD_DATETIME, 1939, 9, 1, 8, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDDate", "1939-09-01", RDFModelEnums.RDFDatatypes.XSD_DATE, 1939, 9, 1, 0, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDgYear", "1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR, 1939, 1, 1, 0, 0, 0)]
        [DataRow("http://www.w3.org/2006/time#inXSDgYearMonth", "1939-09", RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH, 1939, 9, 1, 0, 0, 0)]
        public async Task ShouldGetInstantCoordinateByDateTime(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, 
            double expectedYear, double expectedMonth, double expectedDay, double expectedHour, double expectedMinute, double expectedSecond)
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), new RDFResource(timeProperty), new RDFTypedLiteral(timeValue, timeDataType)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == expectedYear);
            Assert.IsTrue(timeCoordinate.Month == expectedMonth);
            Assert.IsTrue(timeCoordinate.Day == expectedDay);
            Assert.IsTrue(timeCoordinate.Hour == expectedHour);
            Assert.IsTrue(timeCoordinate.Minute == expectedMinute);
            Assert.IsTrue(timeCoordinate.Second == expectedSecond);
        }

        [TestMethod]
        public async Task ShouldGetInstantCoordinateByDescription()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == 1939);
            Assert.IsTrue(timeCoordinate.Month == 9);
            Assert.IsTrue(timeCoordinate.Day == 1);
            Assert.IsTrue(timeCoordinate.Hour == 8);
            Assert.IsTrue(timeCoordinate.Minute == 1);
            Assert.IsTrue(timeCoordinate.Second == 1);
        }

        [TestMethod]
        public async Task ShouldGetInstantCoordinateByDescriptionWithNormalization()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECalendarReferenceSystem myCalendarTRS = new TIMECalendarReferenceSystem(
                new RDFResource("ex:MyCalendarTRS"),
                new TIMECalendarReferenceSystemMetrics(100, 100, 50, [20, 20, 12, 18]));
            TIMEReferenceSystemRegistry.AddTRS(myCalendarTRS);
            TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalExtent"), myCalendarTRS);

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == 1941);
            Assert.IsTrue(timeCoordinate.Month == 1);
            Assert.IsTrue(timeCoordinate.Day == 1);
            Assert.IsTrue(timeCoordinate.Hour == 8);
            Assert.IsTrue(timeCoordinate.Minute == 1);
            Assert.IsTrue(timeCoordinate.Second == 1);
        }

        [TestMethod]
        public async Task ShouldGetInstantCoordinateByNumericPosition()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNotNull(timeCoordinate);
            Assert.IsTrue(timeCoordinate.Year == 1939);
            Assert.IsTrue(timeCoordinate.Month == 9);
            Assert.IsTrue(timeCoordinate.Day == 1);
            Assert.IsTrue(timeCoordinate.Hour == 8);
            Assert.IsTrue(timeCoordinate.Minute == 1);
            Assert.IsTrue(timeCoordinate.Second == 1);
        }

        [TestMethod]
        public async Task ShouldThrowExceptionOnGettingInstantCoordinateByNumericPositionBecauseUnregisteredTRS()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("ex:MyUnregisteredTRS")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);

            Assert.ThrowsException<OWLException>(() => timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalExtent")));
        }

        [TestMethod]
        public async Task ShouldThrowExceptionOnGettingInstantCoordinateByNumericPositionBecauseCalendarTRSDetected()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIITemporalExtentPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.HAS_TRS, new RDFResource("ex:MyCalendarTRS")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMEReferenceSystemRegistry.AddTRS(new TIMECalendarReferenceSystem(
                new RDFResource("ex:MyCalendarTRS"), 
                new TIMECalendarReferenceSystemMetrics(100, 100, 50, [20, 20, 12, 18])));

            Assert.ThrowsException<OWLException>(() => timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalExtent")));
        }

        [TestMethod]
        public async Task ShouldGetInstantCoordinateByNominalPosition()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalExtent"));

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
            => Assert.ThrowsException<OWLException>(() => new OWLOntology(new Uri("ex:timeOnt")).GetExtentOfInterval(null));

        [TestMethod]
        public async Task ShouldGetIntervalExtentByTimeSpan()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral("P6Y1M1DT1H1M61S", RDFModelEnums.RDFDatatypes.XSD_DURATION)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

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
        public async Task ShouldGetIntervalExtentByDescription()
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
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

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
        public async Task ShouldGetIntervalExtentByDuration()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION, new RDFResource("ex:WorldWarIITemporalExtentDuration")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_YEAR));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("6.01", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

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
        public async Task ShouldThrowExceptionOnGettingIntervalExtentByDurationBecauseUnknownUnitType()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_DURATION, new RDFResource("ex:WorldWarIITemporalExtentDuration")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.DURATION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.UNIT_TYPE, new RDFResource("ex:UnknownUnit")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentDuration"), RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral("6.01", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);

            Assert.ThrowsException<OWLException>(() => timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalExtent")));
        }

        [TestMethod]
        public async Task ShouldGetIntervalExtentByBeginningEnd()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIEndPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEndPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEndPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEndPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-767807939", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

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
        public async Task ShouldNotGetIntervalExtentBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(timeExtent);
        }

        [TestMethod]
        public async Task ShouldGetBeginningOfInterval()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
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
        public async Task ShouldNotGetBeginningOfIntervalBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:WorldWarIIBeginning")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginning"), RDFVocabulary.TIME.IN_TIME_POSITION, new RDFResource("ex:WorldWarIIBeginningPosition")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.TIME_POSITION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public async Task ShouldGetBeginningOfIntervalIndirectly()
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
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
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
        public async Task ShouldNotGetBeginningOfIntervalIndirectlyBecauseNoValidEncoding()
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
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIBeginningInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public async Task ShouldGetBeginningOfIntervalIndirectlyMeeting()
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
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
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
        public async Task ShouldNotGetBeginningOfIntervalIndirectlyMeetingBecauseNoValidEncoding()
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
            graph.AddTriple(new RDFTriple(new RDFResource("ex:PrecedingWorldWarIIEndInstantPosition"), RDFVocabulary.TIME.HAS_TRS, TIMEPositionReferenceSystem.UnixTime));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public async Task ShouldGetEndOfInterval()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
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
        public async Task ShouldNotGetEndOfIntervalBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIEnd")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public async Task ShouldGetEndOfIntervalIndirectly()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_FINISHES, new RDFResource("ex:WorldWarIITemporalExtentEnd"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIExtentEndInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentEndInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentEndInstant"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));

            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
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
        public async Task ShouldNotGetEndOfIntervalIndirectlyBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_FINISHES, new RDFResource("ex:WorldWarIITemporalExtentEnd"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtentEnd"), RDFVocabulary.TIME.HAS_END, new RDFResource("ex:WorldWarIIExtentEndInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIIExtentEndInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));

            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        [TestMethod]
        public async Task ShouldGetEndOfIntervalIndirectlyMeeting()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_MEETS, new RDFResource("ex:FollowingWorldWarIITemporalExtent"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant"), RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral("1945-09-02T08:01:01Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)));

            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
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
        public async Task ShouldNotGetEndOfIntervalIndirectlyMeetingBecauseNoValidEncoding()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarII"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("ex:WorldWarIITemporalExtent")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:WorldWarIITemporalExtent"), RDFVocabulary.TIME.INTERVAL_MEETS, new RDFResource("ex:FollowingWorldWarIITemporalExtent"))); //indirect path
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INTERVAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIITemporalExtent"), RDFVocabulary.TIME.HAS_BEGINNING, new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:FollowingWorldWarIIExtentBeginningInstant"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.INSTANT));

            OWLOntology timeOntology = await OWLOntology.FromRDFGraphAsync(graph);
            TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalExtent"));

            Assert.IsNull(tc);
        }

        //E2E Tests

        [TestMethod]
        public async Task ShouldGetTemporalRepresentationsOfFeatureOccurringInArcheanEra()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Stream resourceStream = Assembly.GetExecutingAssembly()
                                                   .GetManifestResourceStream("OWLSharp.Test.Extensions.TIME.GeologicTimeExample.ttl"))
            {
                using (StreamReader resourceReader = new StreamReader(resourceStream))
                {
                    using (StreamWriter memoryStreamWriter = new StreamWriter(memoryStream))
                        memoryStreamWriter.Write(resourceReader.ReadToEnd());
                }
            }

            RDFGraph gtsGraph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, new MemoryStream(memoryStream.ToArray()));
            OWLOntology gtsOntology = await OWLOntology.FromRDFGraphAsync(gtsGraph);
            gtsOntology.Data.DeclareIndividual(new RDFResource("ex:ArcheanFeature"));
            gtsOntology.Data.DeclareObjectAssertion(new RDFResource("ex:ArcheanFeature"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("http://example.org/geologic/Archean"));
            List<TIMEEntity> archeanTimeExtent = gtsOntology.GetTemporalRepresentationsOfFeature(new RDFResource("ex:ArcheanFeature"));

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
            Assert.IsTrue(archeanTimeInterval.Beginning.Position.TRS.Equals(TIMEPositionReferenceSystem.GeologicTime));
            Assert.IsTrue(archeanTimeInterval.Beginning.Position.NumericValue == 4000);
            Assert.IsNotNull(archeanTimeInterval.End);
            Assert.IsNull(archeanTimeInterval.End.DateTime);
            Assert.IsNull(archeanTimeInterval.End.Description);
            Assert.IsNotNull(archeanTimeInterval.End.Position);
            Assert.IsTrue(archeanTimeInterval.End.Position.TRS.Equals(TIMEPositionReferenceSystem.GeologicTime));
            Assert.IsTrue(archeanTimeInterval.End.Position.NumericValue == 2500);
        }

        [TestMethod]
        public async Task ShouldGetCoordinatesAndExtentOfArcheanEra()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Stream resourceStream = Assembly.GetExecutingAssembly()
                                                   .GetManifestResourceStream("OWLSharp.Test.Extensions.TIME.GeologicTimeExample.ttl"))
            {
                using (StreamReader resourceReader = new StreamReader(resourceStream))
                {
                    using (StreamWriter memoryStreamWriter = new StreamWriter(memoryStream))
                        memoryStreamWriter.Write(resourceReader.ReadToEnd());
                }
            }

            RDFGraph gtsGraph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, new MemoryStream(memoryStream.ToArray()));
            OWLOntology gtsOntology = await OWLOntology.FromRDFGraphAsync(gtsGraph, new OWLOntologyLoaderOptions());
            gtsOntology.Data.DeclareIndividual(new RDFResource("ex:ArcheanFeature"));
            gtsOntology.Data.DeclareObjectAssertion(new RDFResource("ex:ArcheanFeature"), RDFVocabulary.TIME.HAS_TIME, new RDFResource("http://example.org/geologic/Archean"));
            
            TIMECoordinate archeanBeginning = gtsOntology.GetBeginningOfInterval(new RDFResource("http://example.org/geologic/Archean"));
            Assert.IsNotNull(archeanBeginning);
            Assert.IsTrue(archeanBeginning.Year == -3_999_998_050);

            TIMECoordinate archeanEnd = gtsOntology.GetEndOfInterval(new RDFResource("http://example.org/geologic/Archean"));
            Assert.IsNotNull(archeanEnd);
            Assert.IsTrue(archeanEnd.Year == -2_499_998_050);

            TIMEExtent archeanExtent = gtsOntology.GetExtentOfInterval(new RDFResource("http://example.org/geologic/Archean"));
            Assert.IsNotNull(archeanExtent);
            Assert.IsTrue(archeanExtent.Days == 547_500_000_000);
        }

        [TestMethod]
        public async Task ShouldAnswerAllenQuestionsAboutArcheanEra()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Stream resourceStream = Assembly.GetExecutingAssembly()
                                                   .GetManifestResourceStream("OWLSharp.Test.Extensions.TIME.GeologicTimeExample.ttl"))
            {
                using (StreamReader resourceReader = new StreamReader(resourceStream))
                {
                    using (StreamWriter memoryStreamWriter = new StreamWriter(memoryStream))
                        memoryStreamWriter.Write(resourceReader.ReadToEnd());
                }
            }

            RDFGraph gtsGraph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, new MemoryStream(memoryStream.ToArray()));
            OWLOntology gtsOntology = await OWLOntology.FromRDFGraphAsync(gtsGraph);
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
        }*/
        #endregion
    }
}