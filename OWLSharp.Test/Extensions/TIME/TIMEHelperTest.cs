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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OWLSharp.Test.Extensions.TIME;

[TestClass]
public class TIMEHelperTest : TIMETestOntology
{
    #region Tests (Declarer)
    [TestMethod]
    public void ShouldDeclareInstantByDateTime()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
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
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
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
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
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
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
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
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEIntervalDuration timeInstantUncertainty = new TIMEIntervalDuration(
            new RDFResource("ex:timeInstPosUnc"), RDFVocabulary.TIME.UNIT_SECOND, 4.04);
        TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"),
            new TIMEInstantPosition(new RDFResource("ex:timeInstPos"),
                TIMEPositionReferenceSystem.UnixTime,
                1679477734))
        {
            Position =
            {
                PositionalUncertainty = timeInstantUncertainty
            }
        }; //2023-03-22T09:35:34Z
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

    [TestMethod]
    public void ShouldDeclareInstantByNominalPosition()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:timeInst"),
            new TIMEInstantPosition(new RDFResource("ex:timeInstPos"),
                new RDFResource("https://en.wikipedia.org/wiki/Geologic_time_scale"),
                new RDFResource("https://en.wikipedia.org/wiki/Geologic_time_scale#Archean")));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:feat"), timeInstant);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInst"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeInstPos"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Geologic_time_scale"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Geologic_time_scale#Archean"))));
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
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:timeInstPos")),
            new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Geologic_time_scale#Archean")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TRS),
            new OWLNamedIndividual(new RDFResource("https://en.wikipedia.org/wiki/Geologic_time_scale")))));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringInstantBecauseNullFeature()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLOntology(new Uri("ex:timeOnt"))
            .DeclareInstantFeature(null, new TIMEInstant(new RDFResource("ex:timeInst"))));

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringInstantBecauseNullInstant()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLOntology(new Uri("ex:timeOnt"))
            .DeclareInstantFeature(new RDFResource("ex:feat"), null));

    [TestMethod]
    public void ShouldDeclareIntervalByTimeSpan()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntv"),
            XmlConvert.ToTimeSpan("P1Y2M3DT11H5M7S"));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntv"))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:feat")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HAS_XSD_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLLiteral(new RDFTypedLiteral("P428DT11H5M7S", RDFModelEnums.RDFDatatypes.XSD_DURATION)))));
    }

    [TestMethod]
    public void ShouldDeclareIntervalByBeginning()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntv"),
            new TIMEInstant(new RDFResource("ex:timeIntvBegin"), DateTime.Parse("2023-03-22T10:35:34Z")), null);
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntv"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin"))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:feat")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin")),
            new OWLLiteral(new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)))));
    }

    [TestMethod]
    public void ShouldDeclareIntervalByEnd()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntv"), null,
            new TIMEInstant(new RDFResource("ex:timeIntvEnd"), DateTime.Parse("2023-03-22T10:35:34Z")));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntv"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd"))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:feat")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd")),
            new OWLLiteral(new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)))));
    }

    [TestMethod]
    public void ShouldDeclareIntervalByBeginningEnd()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntv"),
            new TIMEInstant(new RDFResource("ex:timeIntvBegin"), DateTime.Parse("2023-03-22T10:35:34Z")),
            new TIMEInstant(new RDFResource("ex:timeIntvEnd"), DateTime.Parse("2023-03-25T10:35:34Z")));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntv"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd"))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:feat")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvBegin")),
            new OWLLiteral(new RDFTypedLiteral("2023-03-22T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvEnd")),
            new OWLLiteral(new RDFTypedLiteral("2023-03-25T10:35:34Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP)))));
    }

    [TestMethod]
    public void ShouldDeclareIntervalByDescriptionFromExtent()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntv"),
            new TIMEIntervalDescription(new RDFResource("ex:timeIntvDesc"),
                new TIMEExtent(1, 2, 3, 4, 5, 6, 7, new TIMEExtentMetadata(new RDFResource("ex:TRS")))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntv"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:TRS"))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:feat")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLNamedIndividual(new RDFResource("ex:TRS")))));
            
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEARS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTHS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.WEEKS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAYS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("4", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOURS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTES),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECONDS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
    }

    [TestMethod]
    public void ShouldDeclareIntervalByDescriptionFromTimeSpan()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntv"),
            new TIMEIntervalDescription(new RDFResource("ex:timeIntvDesc"),
                new TIMEExtent(XmlConvert.ToTimeSpan("P1Y1M1DT1H1M1S"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntv"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc"))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:feat")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")))));

        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAYS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("396", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOURS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTES),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECONDS),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDesc")),
            new OWLLiteral(new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)))));
    }

    [TestMethod]
    public void ShouldDeclareIntervalByDuration()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:timeIntv"),
            new TIMEIntervalDuration(new RDFResource("ex:timeIntvDur"), RDFVocabulary.TIME.UNIT_DAY, 77));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:feat"), timeInterval);

        Assert.IsNotNull(timeOntology);
        Assert.IsTrue(timeOntology.IRI.Equals("ex:timeOnt"));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:feat"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntv"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(new RDFResource("ex:timeIntvDur"))));
        Assert.IsTrue(timeOntology.CheckHasEntity(new OWLNamedIndividual(TIMEUnit.Day)));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:feat")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDur")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TEMPORAL_UNIT),
            new OWLNamedIndividual(TIMEUnit.Day))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntv")),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDur")))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDur")),
            new OWLNamedIndividual(TIMEUnit.Day))));
        Assert.IsTrue(timeOntology.CheckHasAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:timeIntvDur")),
            new OWLLiteral(new RDFTypedLiteral("77", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)))));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringIntervalBecauseNullFeature()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLOntology(new Uri("ex:timeOnt")).DeclareIntervalFeature(
            null, new TIMEInterval(new RDFResource("ex:timeIntr"))));

    [TestMethod]
    public void ShouldThrowExceptionOnDeclaringIntervalBecauseNullInterval()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLOntology(new Uri("ex:timeOnt")).DeclareIntervalFeature(
            new RDFResource("ex:feat"), null));
    #endregion

    #region Tests (Analyzer)
    [TestMethod]
    public void ShouldThrowExceptionOnGettingTemporalFeatureBecauseNullURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLOntology(new Uri("ex:timeOnt")).GetTemporalFeature(null));

    [DataTestMethod]
    [DataRow("http://www.w3.org/2006/time#inXSDDateTimeStamp", "1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP, "1939-09-01T08:00:00Z")]
    [DataRow("http://www.w3.org/2006/time#inXSDDateTime", "1939-09-01T09:00:00+01:00", RDFModelEnums.RDFDatatypes.XSD_DATETIME, "1939-09-01T08:00:00Z")]
    [DataRow("http://www.w3.org/2006/time#inXSDDate", "1939-09-01", RDFModelEnums.RDFDatatypes.XSD_DATE, "1939-09-01T00:00:00Z")]
    [DataRow("http://www.w3.org/2006/time#inXSDgYear", "1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR, "1939-01-01T00:00:00Z")]
    [DataRow("http://www.w3.org/2006/time#inXSDgYearMonth", "1939-09", RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH, "1939-09-01T00:00:00Z")]
    public void ShouldGetTemporalInstantFeatureByDateTime(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, string expectedTimeValue)
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(new RDFResource(timeProperty)),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLLiteral(new RDFTypedLiteral(timeValue, timeDataType))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
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
    public void ShouldGetTemporalInstantFeatureByDateTimeThroughInferredProperty(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, string expectedTimeValue)
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(new RDFResource(timeProperty)),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLLiteral(new RDFTypedLiteral(timeValue, timeDataType))));
            
        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsTrue(timeInstant.DateTime.HasValue && timeInstant.DateTime.Equals(DateTime.Parse(expectedTimeValue).ToUniversalTime()));
        Assert.IsNull(timeInstant.Description);
        Assert.IsNull(timeInstant.Position);
    }

    [TestMethod]
    public void ShouldGetTemporalInstantFeatureByDescription()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DATETIME_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMEUnit.Second)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTH),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("--09", RDFModelEnums.RDFDatatypes.XSD_GMONTH))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.XSD_GDAY))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOUR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECOND),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(RDFVocabulary.TIME.GREG.SEPTEMBER)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(RDFVocabulary.TIME.FRIDAY)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("244", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInstant.DateTime.HasValue);
        Assert.IsNotNull(timeInstant.Description);
        Assert.IsNotNull(timeInstant.Description.Coordinate);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.UnitType.Equals(RDFVocabulary.TIME.UNIT_SECOND));
        Assert.IsTrue(timeInstant.Description.Coordinate.Year is 1939);
        Assert.IsTrue(timeInstant.Description.Coordinate.Month is 9);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.MonthOfYear.Equals(RDFVocabulary.TIME.GREG.SEPTEMBER));
        Assert.IsTrue(timeInstant.Description.Coordinate.Day is 1);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfWeek.Equals(RDFVocabulary.TIME.FRIDAY));
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfYear is 244);
        Assert.IsTrue(timeInstant.Description.Coordinate.Hour is 8);
        Assert.IsTrue(timeInstant.Description.Coordinate.Minute is 1);
        Assert.IsTrue(timeInstant.Description.Coordinate.Second is 1);
        Assert.IsNull(timeInstant.Position);
    }

    [TestMethod]
    public void ShouldGetTemporalInstantFeatureByDescriptionThroughInferredProperty()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian));
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DATETIME_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMEUnit.Second)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTH),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("--09", RDFModelEnums.RDFDatatypes.XSD_GMONTH))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.XSD_GDAY))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOUR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECOND),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(RDFVocabulary.TIME.GREG.SEPTEMBER)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(RDFVocabulary.TIME.FRIDAY)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("244", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInstant.DateTime.HasValue);
        Assert.IsNotNull(timeInstant.Description);
        Assert.IsNotNull(timeInstant.Description.Coordinate);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.UnitType.Equals(RDFVocabulary.TIME.UNIT_SECOND));
        Assert.IsTrue(timeInstant.Description.Coordinate.Year is 1939);
        Assert.IsTrue(timeInstant.Description.Coordinate.Month is 9);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.MonthOfYear.Equals(RDFVocabulary.TIME.GREG.SEPTEMBER));
        Assert.IsTrue(timeInstant.Description.Coordinate.Day is 1);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfWeek.Equals(RDFVocabulary.TIME.FRIDAY));
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfYear is 244);
        Assert.IsTrue(timeInstant.Description.Coordinate.Hour is 8);
        Assert.IsTrue(timeInstant.Description.Coordinate.Minute is 1);
        Assert.IsTrue(timeInstant.Description.Coordinate.Second is 1);
        Assert.IsNull(timeInstant.Position);
    }

    [TestMethod]
    public void ShouldGetTemporalInstantFeatureByGeneralDescription()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:AbbyBirthday")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtent")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("http://dbpedia.org/resource/Hebrew_calendar")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtent"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthday")),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtent"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLNamedIndividual(new RDFResource("http://dbpedia.org/resource/Hebrew_calendar"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEAR),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLLiteral(new RDFTypedLiteral("5761", RDFModelEnums.RDFDatatypes.TIME_GENERALYEAR))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTH),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLLiteral(new RDFTypedLiteral("--03", RDFModelEnums.RDFDatatypes.TIME_GENERALMONTH))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLLiteral(new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.TIME_GENERALDAY))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOUR),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLLiteral(new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECOND),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLLiteral(new RDFTypedLiteral("1.25", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLNamedIndividual(new RDFResource("ex:Sivan"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLNamedIndividual(new RDFResource("ex:RoshChodesh"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR),
            new OWLNamedIndividual(new RDFResource("ex:AbbyBirthdayTemporalExtentDescription")),
            new OWLLiteral(new RDFTypedLiteral("45", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:AbbyBirthday"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:AbbyBirthdayTemporalExtent")));
        Assert.IsFalse(timeInstant.DateTime.HasValue);
        Assert.IsNotNull(timeInstant.Description);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.TRS.Equals(new RDFResource("http://dbpedia.org/resource/Hebrew_calendar")));
        Assert.IsTrue(timeInstant.Description.Coordinate.Year is 5761);
        Assert.IsTrue(timeInstant.Description.Coordinate.Month is 3);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.MonthOfYear.Equals(new RDFResource("ex:Sivan")));
        Assert.IsTrue(timeInstant.Description.Coordinate.Day is 1);
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfWeek.Equals(new RDFResource("ex:RoshChodesh")));
        Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfYear is 45);
        Assert.IsTrue(timeInstant.Description.Coordinate.Hour is 8);
        Assert.IsTrue(timeInstant.Description.Coordinate.Minute is 1);
        Assert.IsTrue(timeInstant.Description.Coordinate.Second is 1.25);
        Assert.IsNull(timeInstant.Position);
    }

    [TestMethod]
    public void ShouldGetTemporalInstantFeatureByNumericPosition()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInstant.DateTime.HasValue);
        Assert.IsNull(timeInstant.Description);
        Assert.IsNotNull(timeInstant.Position);
        Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMEPositionReferenceSystem.UnixTime));
        Assert.AreEqual(-957315600, timeInstant.Position.NumericValue);
    }

    [TestMethod]
    public void ShouldGetTemporalInstantFeatureByNumericPositionThroughInferredProperty()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInstant.DateTime.HasValue);
        Assert.IsNull(timeInstant.Description);
        Assert.IsNotNull(timeInstant.Position);
        Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMEPositionReferenceSystem.UnixTime));
        Assert.AreEqual(-957315600, timeInstant.Position.NumericValue);
    }

    [TestMethod]
    public void ShouldGetTemporalInstantFeatureByNominalPosition()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:September1939")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(new RDFResource("ex:September1939"))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInstant);

        TIMEInstant timeInstant = (TIMEInstant)timeEntities.Single();

        Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInstant.DateTime.HasValue);
        Assert.IsNull(timeInstant.Description);
        Assert.IsNotNull(timeInstant.Position);
        Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsTrue(timeInstant.Position.NominalValue.Equals(new RDFResource("ex:September1939")));
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByTimeSpan()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HAS_XSD_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLLiteral(new RDFTypedLiteral("P6Y", RDFModelEnums.RDFDatatypes.XSD_DURATION))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsTrue(timeInterval.TimeSpan.HasValue && timeInterval.TimeSpan.Equals(XmlConvert.ToTimeSpan("P6Y")));
        Assert.IsNull(timeInterval.Description);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByTimeSpanThroughInferredProperty()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HAS_XSD_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLLiteral(new RDFTypedLiteral("P6Y", RDFModelEnums.RDFDatatypes.XSD_DURATION))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsTrue(timeInterval.TimeSpan.HasValue && timeInterval.TimeSpan.Equals(XmlConvert.ToTimeSpan("P6Y")));
        Assert.IsNull(timeInterval.Description);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByDescription()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEARS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTHS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.WEEKS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAYS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOURS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTES),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECONDS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("8.7", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNotNull(timeInterval.Description);
        Assert.IsNotNull(timeInterval.Description.Extent);
        Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsTrue(timeInterval.Description.Extent.Years is 6);
        Assert.IsTrue(timeInterval.Description.Extent.Months is 3);
        Assert.IsTrue(timeInterval.Description.Extent.Weeks is 2);
        Assert.IsTrue(timeInterval.Description.Extent.Days is 5);
        Assert.IsTrue(timeInterval.Description.Extent.Hours is 9);
        Assert.IsTrue(timeInterval.Description.Extent.Minutes is 7);
        Assert.IsTrue(timeInterval.Description.Extent.Seconds is 8.7);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByDescriptionThroughInferredProperty()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEARS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTHS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.WEEKS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAYS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOURS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTES),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECONDS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("8.7", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNotNull(timeInterval.Description);
        Assert.IsNotNull(timeInterval.Description.Extent);
        Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsTrue(timeInterval.Description.Extent.Years is 6);
        Assert.IsTrue(timeInterval.Description.Extent.Months is 3);
        Assert.IsTrue(timeInterval.Description.Extent.Weeks is 2);
        Assert.IsTrue(timeInterval.Description.Extent.Days is 5);
        Assert.IsTrue(timeInterval.Description.Extent.Hours is 9);
        Assert.IsTrue(timeInterval.Description.Extent.Minutes is 7);
        Assert.IsTrue(timeInterval.Description.Extent.Seconds is 8.7);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByGeneralDescription()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEARS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTHS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.WEEKS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAYS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOURS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTES),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECONDS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("8.7", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNotNull(timeInterval.Description);
        Assert.IsNotNull(timeInterval.Description.Extent);
        Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsTrue(timeInterval.Description.Extent.Years is 6);
        Assert.IsTrue(timeInterval.Description.Extent.Months is 3);
        Assert.IsTrue(timeInterval.Description.Extent.Weeks is 2);
        Assert.IsTrue(timeInterval.Description.Extent.Days is 5);
        Assert.IsTrue(timeInterval.Description.Extent.Hours is 9);
        Assert.IsTrue(timeInterval.Description.Extent.Minutes is 7);
        Assert.IsTrue(timeInterval.Description.Extent.Seconds is 8.7);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByDuration()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLNamedIndividual(TIMEUnit.Year)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNull(timeInterval.Description);
        Assert.IsNotNull(timeInterval.Duration);
        Assert.IsTrue(timeInterval.Duration.UnitType.Equals(RDFVocabulary.TIME.UNIT_YEAR));
        Assert.AreEqual(6, timeInterval.Duration.Value);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByDurationThroughInferredProperty()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")));
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLNamedIndividual(TIMEUnit.Year)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNull(timeInterval.Description);
        Assert.IsNotNull(timeInterval.Duration);
        Assert.IsTrue(timeInterval.Duration.UnitType.Equals(RDFVocabulary.TIME.UNIT_YEAR));
        Assert.AreEqual(6, timeInterval.Duration.Value);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByBeginningInstant()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")),
            new OWLLiteral(new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNull(timeInterval.Description);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNotNull(timeInterval.Beginning);
        Assert.IsTrue(timeInterval.Beginning.URI.Equals(new Uri("ex:WorldWarIITemporalDimensionBeginning")));
        Assert.IsTrue(timeInterval.Beginning.DateTime.HasValue && timeInterval.Beginning.DateTime.Equals(DateTime.Parse("1939-09-01T08:00:00Z").ToUniversalTime()));
        Assert.IsNull(timeInterval.Beginning.Description);
        Assert.IsNull(timeInterval.Beginning.Position);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByBeginningInstantThroughInferredProperty()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")));
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")),
            new OWLLiteral(new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNull(timeInterval.Description);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNotNull(timeInterval.Beginning);
        Assert.IsTrue(timeInterval.Beginning.URI.Equals(new Uri("ex:WorldWarIITemporalDimensionBeginning")));
        Assert.IsTrue(timeInterval.Beginning.DateTime.HasValue && timeInterval.Beginning.DateTime.Equals(DateTime.Parse("1939-09-01T08:00:00Z").ToUniversalTime()));
        Assert.IsNull(timeInterval.Beginning.Description);
        Assert.IsNull(timeInterval.Beginning.Position);
        Assert.IsNull(timeInterval.End);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByEndInstant()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")),
            new OWLLiteral(new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNull(timeInterval.Description);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNotNull(timeInterval.End);
        Assert.IsTrue(timeInterval.End.URI.Equals(new Uri("ex:WorldWarIITemporalDimensionEnd")));
        Assert.IsTrue(timeInterval.End.DateTime.HasValue && timeInterval.End.DateTime.Equals(DateTime.Parse("1945-09-02T08:00:00Z").ToUniversalTime()));
        Assert.IsNull(timeInterval.End.Description);
        Assert.IsNull(timeInterval.End.Position);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByEndInstantThroughInferredProperty()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")));
        timeOntology.DeclareEntity(new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")));
        timeOntology.DeclareObjectPropertyAxiom(new OWLSubObjectPropertyOf(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME)));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(new RDFResource("ex:hasTemporalExtent")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")),
            new OWLLiteral(new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));

        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNull(timeInterval.Description);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNull(timeInterval.Beginning);
        Assert.IsNotNull(timeInterval.End);
        Assert.IsTrue(timeInterval.End.URI.Equals(new Uri("ex:WorldWarIITemporalDimensionEnd")));
        Assert.IsTrue(timeInterval.End.DateTime.HasValue && timeInterval.End.DateTime.Equals(DateTime.Parse("1945-09-02T08:00:00Z").ToUniversalTime()));
        Assert.IsNull(timeInterval.End.Description);
        Assert.IsNull(timeInterval.End.Position);
    }

    [TestMethod]
    public void ShouldGetTemporalIntervalFeatureByBeginningEndInstants()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")),
            new OWLLiteral(new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")),
            new OWLLiteral(new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));
            
        List<TIMEEntity> timeEntities = timeOntology.GetTemporalFeature(new RDFResource("ex:WorldWarII"));

        Assert.IsNotNull(timeEntities);
        Assert.AreEqual(1, timeEntities.Count);
        Assert.IsTrue(timeEntities.Single() is TIMEInterval);

        TIMEInterval timeInterval = (TIMEInterval)timeEntities.Single();

        Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:WorldWarIITemporalDimension")));
        Assert.IsFalse(timeInterval.TimeSpan.HasValue);
        Assert.IsNull(timeInterval.Description);
        Assert.IsNull(timeInterval.Duration);
        Assert.IsNotNull(timeInterval.Beginning);
        Assert.IsTrue(timeInterval.Beginning.URI.Equals(new Uri("ex:WorldWarIITemporalDimensionBeginning")));
        Assert.IsTrue(timeInterval.Beginning.DateTime.HasValue && timeInterval.Beginning.DateTime.Equals(DateTime.Parse("1939-09-01T08:00:00Z").ToUniversalTime()));
        Assert.IsNull(timeInterval.Beginning.Description);
        Assert.IsNull(timeInterval.Beginning.Position);
        Assert.IsNotNull(timeInterval.End);
        Assert.IsTrue(timeInterval.End.URI.Equals(new Uri("ex:WorldWarIITemporalDimensionEnd")));
        Assert.IsTrue(timeInterval.End.DateTime.HasValue && timeInterval.End.DateTime.Equals(DateTime.Parse("1945-09-02T08:00:00Z").ToUniversalTime()));
        Assert.IsNull(timeInterval.End.Description);
        Assert.IsNull(timeInterval.End.Position);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnGettingInstantCoordinateBecauseNullInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLOntology(new Uri("ex:timeOnt")).GetCoordinateOfInstant(null));

    [DataTestMethod]
    [DataRow("http://www.w3.org/2006/time#inXSDDateTimeStamp", "1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP, 1939, 9, 1, 8, 0, 0)]
    [DataRow("http://www.w3.org/2006/time#inXSDDateTime", "1939-09-01T09:00:00+01:00", RDFModelEnums.RDFDatatypes.XSD_DATETIME, 1939, 9, 1, 8, 0, 0)]
    [DataRow("http://www.w3.org/2006/time#inXSDDate", "1939-09-01", RDFModelEnums.RDFDatatypes.XSD_DATE, 1939, 9, 1, 0, 0, 0)]
    [DataRow("http://www.w3.org/2006/time#inXSDgYear", "1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR, 1939, 1, 1, 0, 0, 0)]
    [DataRow("http://www.w3.org/2006/time#inXSDgYearMonth", "1939-09", RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH, 1939, 9, 1, 0, 0, 0)]
    public void ShouldGetInstantCoordinateByDateTime(string timeProperty, string timeValue, RDFModelEnums.RDFDatatypes timeDataType, 
        double expectedYear, double expectedMonth, double expectedDay, double expectedHour, double expectedMinute, double expectedSecond)
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(new RDFResource(timeProperty)),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLLiteral(new RDFTypedLiteral(timeValue, timeDataType))));
        TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(expectedYear, timeCoordinate.Year);
        Assert.AreEqual(expectedMonth, timeCoordinate.Month);
        Assert.AreEqual(expectedDay, timeCoordinate.Day);
        Assert.AreEqual(expectedHour, timeCoordinate.Hour);
        Assert.AreEqual(expectedMinute, timeCoordinate.Minute);
        Assert.AreEqual(expectedSecond, timeCoordinate.Second);
    }

    [TestMethod]
    public void ShouldGetInstantCoordinateByDescription()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DATETIME_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMEUnit.Second)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTH),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("--09", RDFModelEnums.RDFDatatypes.XSD_GMONTH))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.XSD_GDAY))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOUR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECOND),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));

        TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(1939, timeCoordinate.Year);
        Assert.AreEqual(9, timeCoordinate.Month);
        Assert.AreEqual(1, timeCoordinate.Day);
        Assert.AreEqual(8, timeCoordinate.Hour);
        Assert.AreEqual(1, timeCoordinate.Minute);
        Assert.AreEqual(1, timeCoordinate.Second);
    }

    [TestMethod]
    public void ShouldGetInstantCoordinateByDescriptionWithNormalization()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DATETIME_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMEUnit.Second)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEAR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("1939", RDFModelEnums.RDFDatatypes.XSD_GYEAR))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTH),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("--09", RDFModelEnums.RDFDatatypes.XSD_GMONTH))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAY),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("---01", RDFModelEnums.RDFDatatypes.XSD_GDAY))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOUR),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("08", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECOND),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("01", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
        TIMECalendarReferenceSystem myCalendarTRS = new TIMECalendarReferenceSystem(
            new RDFResource("ex:MyCalendarTRS"),
            new TIMECalendarReferenceSystemMetrics(100, 100, 50, [20, 20, 12, 18]));
        TIMEReferenceSystemRegistry.AddTRS(myCalendarTRS);

        TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalDimension"), myCalendarTRS);

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(1941, timeCoordinate.Year);
        Assert.AreEqual(1, timeCoordinate.Month);
        Assert.AreEqual(1, timeCoordinate.Day);
        Assert.AreEqual(8, timeCoordinate.Hour);
        Assert.AreEqual(1, timeCoordinate.Minute);
        Assert.AreEqual(1, timeCoordinate.Second);
    }

    [TestMethod]
    public void ShouldGetInstantCoordinateByNumericPosition()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
            
        TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(1939, timeCoordinate.Year);
        Assert.AreEqual(8, timeCoordinate.Month);
        Assert.AreEqual(31, timeCoordinate.Day);
        Assert.AreEqual(23, timeCoordinate.Hour);
        Assert.AreEqual(00, timeCoordinate.Minute);
        Assert.AreEqual(00, timeCoordinate.Second);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnGettingInstantCoordinateByNumericPositionBecauseUnregisteredTRS()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:MyUnregisteredTRS")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        Assert.ThrowsExactly<OWLException>(() => _ = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalDimension")));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnGettingInstantCoordinateByNumericPositionBecauseCalendarTRSDetected()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(new RDFResource("ex:MyCalendarTRS"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957315600", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        TIMEReferenceSystemRegistry.AddTRS(new TIMECalendarReferenceSystem(
            new RDFResource("ex:MyCalendarTRS"), 
            new TIMECalendarReferenceSystemMetrics(100, 100, 50, [20, 20, 12, 18])));

        Assert.ThrowsExactly<OWLException>(() => _ = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalDimension")));
    }

    [TestMethod]
    public void ShouldGetInstantCoordinateByNominalPosition()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:September1939")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:September1939Beginning")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:September1939End")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:September1939"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:September1939Beginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:September1939End"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionPosition")),
            new OWLNamedIndividual(new RDFResource("ex:September1939"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:September1939")),
            new OWLNamedIndividual(new RDFResource("ex:September1939Beginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:September1939")),
            new OWLNamedIndividual(new RDFResource("ex:September1939End"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATE),
            new OWLNamedIndividual(new RDFResource("ex:September1939Beginning")),
            new OWLLiteral(new RDFTypedLiteral("1939-09-01Z", RDFModelEnums.RDFDatatypes.XSD_DATE))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATE),
            new OWLNamedIndividual(new RDFResource("ex:September1939End")),
            new OWLLiteral(new RDFTypedLiteral("1945-09-02Z", RDFModelEnums.RDFDatatypes.XSD_DATE))));

        TIMECoordinate timeCoordinate = timeOntology.GetCoordinateOfInstant(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(1939, timeCoordinate.Year);
        Assert.AreEqual(9, timeCoordinate.Month);
        Assert.AreEqual(1, timeCoordinate.Day);
        Assert.AreEqual(0, timeCoordinate.Hour);
        Assert.AreEqual(0, timeCoordinate.Minute);
        Assert.AreEqual(0, timeCoordinate.Second);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnGettingIntervalExtentBecauseNullIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = new OWLOntology(new Uri("ex:timeOnt")).GetExtentOfInterval(null));

    [TestMethod]
    public void ShouldGetIntervalExtentByTimeSpan()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HAS_XSD_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLLiteral(new RDFTypedLiteral("P6YT1H1M1S", RDFModelEnums.RDFDatatypes.XSD_DURATION))));
        TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeExtent);
        Assert.AreEqual(0, timeExtent.Years);
        Assert.AreEqual(0, timeExtent.Months);
        Assert.AreEqual(0, timeExtent.Weeks);
        Assert.AreEqual(2190, timeExtent.Days);
        Assert.AreEqual(1, timeExtent.Hours);
        Assert.AreEqual(1, timeExtent.Minutes);
        Assert.AreEqual(1, timeExtent.Seconds);
    }

    [TestMethod]
    public void ShouldGetIntervalExtentByDescription()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLNamedIndividual(TIMECalendarReferenceSystem.Gregorian)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.YEARS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MONTHS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("3", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.WEEKS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.DAYS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("5", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.HOURS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("9", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.MINUTES),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("7", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.SECONDS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDescription")),
            new OWLLiteral(new RDFTypedLiteral("61", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));
        TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeExtent);
        Assert.AreEqual(0, timeExtent.Years);
        Assert.AreEqual(0, timeExtent.Months);
        Assert.AreEqual(0, timeExtent.Weeks);
        Assert.AreEqual(2299, timeExtent.Days);
        Assert.AreEqual(9, timeExtent.Hours);
        Assert.AreEqual(8, timeExtent.Minutes);
        Assert.AreEqual(1, timeExtent.Seconds);
    }

    [TestMethod]
    public void ShouldGetIntervalExtentByDuration()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLNamedIndividual(TIMEUnit.Year)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeExtent);
        Assert.AreEqual(0, timeExtent.Years);
        Assert.AreEqual(0, timeExtent.Months);
        Assert.AreEqual(0, timeExtent.Weeks);
        Assert.AreEqual(2190, timeExtent.Days);
        Assert.AreEqual(0, timeExtent.Hours);
        Assert.AreEqual(0, timeExtent.Minutes);
        Assert.AreEqual(0, timeExtent.Seconds);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnGettingIntervalExtentByDurationBecauseUnknownUnitType()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:UnknownUnitType")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLNamedIndividual(new RDFResource("ex:UnknownUnitType"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionDuration")),
            new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        Assert.ThrowsExactly<OWLException>(() => _ = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalDimension")));
    }

    [TestMethod]
    public void ShouldGetIntervalExtentByBeginningEnd()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")),
            new OWLLiteral(new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")),
            new OWLLiteral(new RDFTypedLiteral("1945-09-02T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));
        TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(timeExtent);
        Assert.AreEqual(0, timeExtent.Years);
        Assert.AreEqual(0, timeExtent.Months);
        Assert.AreEqual(0, timeExtent.Weeks);
        Assert.AreEqual(2191, timeExtent.Days);
        Assert.AreEqual(0, timeExtent.Hours);
        Assert.AreEqual(0, timeExtent.Minutes);
        Assert.AreEqual(0, timeExtent.Seconds);
    }

    [TestMethod]
    public void ShouldNotGetIntervalExtentBecauseNoValidEncoding()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")),
            new OWLLiteral(new RDFTypedLiteral("1939-09-01T08:00:00Z", RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));
        TIMEExtent timeExtent = timeOntology.GetExtentOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNull(timeExtent);
    }

    [TestMethod]
    public void ShouldGetBeginningOfInterval()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(tc);
        Assert.AreEqual(1939, tc.Year);
        Assert.AreEqual(9, tc.Month);
        Assert.AreEqual(1, tc.Day);
        Assert.AreEqual(8, tc.Hour);
        Assert.AreEqual(1, tc.Minute);
        Assert.AreEqual(1, tc.Second);
    }

    [TestMethod]
    public void ShouldNotGetBeginningOfIntervalBecauseNoValidEncoding()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginning")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIBeginningPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNull(tc);
    }

    [TestMethod]
    public void ShouldGetBeginningOfIntervalIndirectly()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(tc);
        Assert.AreEqual(1939, tc.Year);
        Assert.AreEqual(9, tc.Month);
        Assert.AreEqual(1, tc.Day);
        Assert.AreEqual(8, tc.Hour);
        Assert.AreEqual(1, tc.Minute);
        Assert.AreEqual(1, tc.Second);
    }

    [TestMethod]
    public void ShouldNotGetBeginningOfIntervalIndirectlyBecauseNoValidEncoding()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionBeginning")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstant")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionBeginningInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNull(tc);
    }
        
    [TestMethod]
    public void ShouldGetBeginningOfIntervalIndirectlyMeeting()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant")),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition")),
            new OWLLiteral(new RDFTypedLiteral("-957283139", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(tc);
        Assert.AreEqual(1939, tc.Year);
        Assert.AreEqual(9, tc.Month);
        Assert.AreEqual(1, tc.Day);
        Assert.AreEqual(8, tc.Hour);
        Assert.AreEqual(1, tc.Minute);
        Assert.AreEqual(1, tc.Second);
    }

    [TestMethod]
    public void ShouldNotGetBeginningOfIntervalIndirectlyMeetingBecauseNoValidEncoding()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstant")),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:PrecedingWorldWarIITemporalDimensionEndInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));

        TIMECoordinate tc = timeOntology.GetBeginningOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNull(tc);
    }

    [TestMethod]
    public void ShouldGetEndOfInterval()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition")),
            new OWLLiteral(new RDFTypedLiteral("-767808000", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(tc);
        Assert.AreEqual(1945, tc.Year);
        Assert.AreEqual(9, tc.Month);
        Assert.AreEqual(2, tc.Day);
        Assert.AreEqual(8, tc.Hour);
        Assert.AreEqual(0, tc.Minute);
        Assert.AreEqual(0, tc.Second);
    }

    [TestMethod]
    public void ShouldNotGetEndOfIntervalBecauseNoValidEncoding()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEnd")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIIEndPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNull(tc);
    }

    [TestMethod]
    public void ShouldGetEndOfIntervalIndirectly()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition")),
            new OWLLiteral(new RDFTypedLiteral("-767808000", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(tc);
        Assert.AreEqual(1945, tc.Year);
        Assert.AreEqual(9, tc.Month);
        Assert.AreEqual(2, tc.Day);
        Assert.AreEqual(8, tc.Hour);
        Assert.AreEqual(0, tc.Minute);
        Assert.AreEqual(0, tc.Second);
    }

    [TestMethod]
    public void ShouldNotGetEndOfIntervalIndirectlyBecauseNoValidEncoding()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimensionEnd")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstant")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDImensionEndInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNull(tc);
    }
        
    [TestMethod]
    public void ShouldGetEndOfIntervalIndirectlyMeeting()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant")),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));
        timeOntology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
            new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition")),
            new OWLLiteral(new RDFTypedLiteral("-767808000", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));

        TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNotNull(tc);
        Assert.AreEqual(1945, tc.Year);
        Assert.AreEqual(9, tc.Month);
        Assert.AreEqual(2, tc.Day);
        Assert.AreEqual(8, tc.Hour);
        Assert.AreEqual(0, tc.Minute);
        Assert.AreEqual(0, tc.Second);
    }

    [TestMethod]
    public void ShouldNotGetEndOfIntervalIndirectlyMeetingBecauseNoValidEncoding()
    {
        OWLOntology timeOntology = new OWLOntology(TestOntology) { IRI = "ex:timeOnt" };
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarII")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition")));
        timeOntology.DeclareEntity(new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INTERVAL),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.INSTANT),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLClassAssertion(
            new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarII")),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS),
            new OWLNamedIndividual(new RDFResource("ex:WorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimension")),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstant")),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition"))));
        timeOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
            new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
            new OWLNamedIndividual(new RDFResource("ex:FollowingWorldWarIITemporalDimensionBeginningInstantPosition")),
            new OWLNamedIndividual(TIMEPositionReferenceSystem.UnixTime)));

        TIMECoordinate tc = timeOntology.GetEndOfInterval(new RDFResource("ex:WorldWarIITemporalDimension"));

        Assert.IsNull(tc);
    }
    #endregion
}