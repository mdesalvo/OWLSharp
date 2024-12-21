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

using OWLSharp.Ontology;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace OWLSharp.Extensions.TIME
{
    public static class TIMEHelper
    {
        #region Declarer
        public static OWLOntology DeclareTIMEInstantFeature(this OWLOntology ontology, RDFResource featureUri, TIMEInstant timeInstant)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare temporal instant because given \"featureUri\" parameter is null");
            if (timeInstant == null)
                throw new OWLException("Cannot declare temporal instant because given \"timeInstant\" parameter is null");
            #endregion

            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME));
            ontology.DeclareEntity(new OWLNamedIndividual(featureUri));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(timeInstant)));
            ontology.DeclareTIMEInstantFeatureInternal(timeInstant);

            return ontology;
        }
        internal static OWLOntology DeclareTIMEInstantFeatureInternal(this OWLOntology ontology, TIMEInstant timeInstant)
        {
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.INSTANT));
            ontology.DeclareEntity(new OWLNamedIndividual(timeInstant));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.INSTANT),
                new OWLNamedIndividual(timeInstant)));

            //Add knowledge to the A-BOX (time:inXSDDateTimeStamp)
            if (timeInstant.DateTime.HasValue)
            {
                ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP));
                ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP),
                    new OWLNamedIndividual(timeInstant),
                    new OWLLiteral(new RDFTypedLiteral(XmlConvert.ToString(timeInstant.DateTime.Value.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ssZ"), RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))));
            }

            //Add knowledge to the A-BOX (time:inDateTime)
            if (timeInstant.Description != null)
            {
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION));
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.TEMPORAL_UNIT));
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.TRS));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS));
                ontology.DeclareEntity(new OWLNamedIndividual(timeInstant.Description));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME),
                    new OWLNamedIndividual(timeInstant),
                    new OWLNamedIndividual(timeInstant.Description)));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION),
                    new OWLNamedIndividual(timeInstant.Description)));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
                    new OWLNamedIndividual(timeInstant.Description),
                    new OWLNamedIndividual(timeInstant.Description.Coordinate.Metadata.UnitType)));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
                    new OWLNamedIndividual(timeInstant.Description),
                    new OWLNamedIndividual(timeInstant.Description.Coordinate.Metadata.TRS)));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.TEMPORAL_UNIT),
                    new OWLNamedIndividual(timeInstant.Description.Coordinate.Metadata.UnitType)));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.TRS),
                    new OWLNamedIndividual(timeInstant.Description.Coordinate.Metadata.TRS)));

                if (timeInstant.Description.Coordinate.Year.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.YEAR));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.YEAR),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Year, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInstant.Description.Coordinate.Month.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.MONTH));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.MONTH),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Month, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInstant.Description.Coordinate.Day.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.DAY));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.DAY),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Day, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInstant.Description.Coordinate.Hour.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.HOUR));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.HOUR),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Hour, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInstant.Description.Coordinate.Minute.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.MINUTE));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.MINUTE),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Minute, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInstant.Description.Coordinate.Second.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.SECOND));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.SECOND),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Second, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }

                if (timeInstant.Description.Coordinate.Metadata.MonthOfYear != null)
                {
                    ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR));
                    ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLNamedIndividual(timeInstant.Description.Coordinate.Metadata.MonthOfYear)));
                }
                if (timeInstant.Description.Coordinate.Metadata.DayOfWeek != null)
                {
                    ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK));
                    ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLNamedIndividual(timeInstant.Description.Coordinate.Metadata.DayOfWeek)));
                }
                if (timeInstant.Description.Coordinate.Metadata.DayOfYear.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR),
                        new OWLNamedIndividual(timeInstant.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Metadata.DayOfYear, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER))));
                }
            }

            //Add knowledge to the A-BOX (time:inTimePosition)
            if (timeInstant.Position != null)
            {
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.TIME_POSITION));
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.TRS));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS));
                ontology.DeclareEntity(new OWLNamedIndividual(timeInstant.Position));
                ontology.DeclareEntity(new OWLNamedIndividual(timeInstant.Position.TRS));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION),
                    new OWLNamedIndividual(timeInstant),
                    new OWLNamedIndividual(timeInstant.Position)));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.TIME_POSITION),
                    new OWLNamedIndividual(timeInstant.Position)));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.TRS),
                    new OWLNamedIndividual(timeInstant.Position.TRS)));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
                    new OWLNamedIndividual(timeInstant.Position),
                    new OWLNamedIndividual(timeInstant.Position.TRS)));

                if (timeInstant.Position.IsNominal)
                {
                    ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION));
                    ontology.DeclareEntity(new OWLNamedIndividual(timeInstant.Position.NominalValue));
                    ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION),
                        new OWLNamedIndividual(timeInstant.Position),
                        new OWLNamedIndividual(timeInstant.Position.NominalValue)));
                }
                else
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION),
                        new OWLNamedIndividual(timeInstant.Position),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Position.NumericValue, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));
                }

                if (timeInstant.Position.PositionalUncertainty != null)
                {
                    ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.TEMPORAL_UNIT));
                    ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.DURATION));
                    ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE));
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION));
                    ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY));
                    ontology.DeclareEntity(new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty));
                    ontology.DeclareEntity(new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty.UnitType));
                    ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.TIME.TEMPORAL_UNIT),
                        new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty.UnitType)));
                    ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.TIME.DURATION),
                        new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty)));
                    ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY),
                        new OWLNamedIndividual(timeInstant.Position),
                        new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty)));
                    ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                        new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
                        new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty),
                        new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty.UnitType)));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
                        new OWLNamedIndividual(timeInstant.Position.PositionalUncertainty),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInstant.Position.PositionalUncertainty.Value, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));
                }
            }

            return ontology;
        }

        public static OWLOntology DeclareTIMEIntervalFeature(this OWLOntology ontology, RDFResource featureUri, TIMEInterval timeInterval)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare temporal interval because given \"featureUri\" parameter is null");
            if (timeInterval == null)
                throw new OWLException("Cannot declare temporal interval because given \"timeInterval\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME));
            ontology.DeclareEntity(new OWLNamedIndividual(featureUri));
            ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME),
                new OWLNamedIndividual(featureUri),
                new OWLNamedIndividual(timeInterval)));
            ontology.DeclareTIMEIntervalFeatureInternal(timeInterval);

            return ontology;
        }
        internal static OWLOntology DeclareTIMEIntervalFeatureInternal(this OWLOntology ontology, TIMEInterval timeInterval)
        {
            //Add knowledge to the A-BOX
            ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.INTERVAL));
            ontology.DeclareEntity(new OWLNamedIndividual(timeInterval));
            ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.INTERVAL),
                new OWLNamedIndividual(timeInterval)));

            //Add knowledge to the A-BOX (time:hasXSDDuration)
            if (timeInterval.TimeSpan.HasValue)
            {
                ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.HAS_XSD_DURATION));
                ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                    new OWLDataProperty(RDFVocabulary.TIME.HAS_XSD_DURATION),
                    new OWLNamedIndividual(timeInterval),
                    new OWLLiteral(new RDFTypedLiteral($"{XmlConvert.ToString(timeInterval.TimeSpan.Value)}", RDFModelEnums.RDFDatatypes.XSD_DURATION))));
            }

            //Add knowledge to the A-BOX (time:hasBeginning)
            if (timeInterval.Beginning != null)
            {
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING),
                    new OWLNamedIndividual(timeInterval),
                    new OWLNamedIndividual(timeInterval.Beginning)));
                ontology.DeclareTIMEInstantFeatureInternal(timeInterval.Beginning);
            }

            //Add knowledge to the A-BOX (time:hasEnd)
            if (timeInterval.End != null)
            {
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_END));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_END),
                    new OWLNamedIndividual(timeInterval),
                    new OWLNamedIndividual(timeInterval.End)));
                ontology.DeclareTIMEInstantFeatureInternal(timeInterval.End);
            }

            //Add knowledge to the A-BOX (time:hasDurationDescription)
            if (timeInterval.Description != null)
            {
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION));
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.TRS));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS));
                ontology.DeclareEntity(new OWLNamedIndividual(timeInterval.Description));
                ontology.DeclareEntity(new OWLNamedIndividual(timeInterval.Description.Extent.Metadata.TRS));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION),
                    new OWLNamedIndividual(timeInterval.Description)));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION),
                    new OWLNamedIndividual(timeInterval),
                    new OWLNamedIndividual(timeInterval.Description)));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.TRS),
                    new OWLNamedIndividual(timeInterval.Description.Extent.Metadata.TRS)));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS),
                    new OWLNamedIndividual(timeInterval.Description),
                    new OWLNamedIndividual(timeInterval.Description.Extent.Metadata.TRS)));

                if (timeInterval.Description.Extent.Years.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.YEARS));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.YEARS),
                        new OWLNamedIndividual(timeInterval.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Description.Extent.Years, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInterval.Description.Extent.Months.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.MONTHS));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.MONTHS),
                        new OWLNamedIndividual(timeInterval.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Description.Extent.Months, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInterval.Description.Extent.Weeks.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.WEEKS));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.WEEKS),
                        new OWLNamedIndividual(timeInterval.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Description.Extent.Weeks, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInterval.Description.Extent.Days.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.DAYS));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.DAYS),
                        new OWLNamedIndividual(timeInterval.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Description.Extent.Days, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInterval.Description.Extent.Hours.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.HOURS));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.HOURS),
                        new OWLNamedIndividual(timeInterval.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Description.Extent.Hours, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInterval.Description.Extent.Minutes.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.MINUTES));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.MINUTES),
                        new OWLNamedIndividual(timeInterval.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Description.Extent.Minutes, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
                if (timeInterval.Description.Extent.Seconds.HasValue)
                {
                    ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.SECONDS));
                    ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.SECONDS),
                        new OWLNamedIndividual(timeInterval.Description),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Description.Extent.Seconds, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
                }
            }

            //Add knowledge to the A-BOX (time:hasDuration)
            if (timeInterval.Duration != null)
            {
                ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.DURATION));
                ontology.DeclareEntity(new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION));
                ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.TEMPORAL_UNIT));
                ontology.DeclareEntity(new OWLNamedIndividual(timeInterval.Duration));
                ontology.DeclareEntity(new OWLNamedIndividual(timeInterval.Duration.UnitType));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.DURATION),
                    new OWLNamedIndividual(timeInterval.Duration)));
                ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.TEMPORAL_UNIT),
                    new OWLNamedIndividual(timeInterval.Duration.UnitType)));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION),
                    new OWLNamedIndividual(timeInterval),
                    new OWLNamedIndividual(timeInterval.Duration)));
                ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE),
                    new OWLNamedIndividual(timeInterval.Duration),
                    new OWLNamedIndividual(timeInterval.Duration.UnitType)));
                ontology.DeclareAssertionAxiom(new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION),
                        new OWLNamedIndividual(timeInterval.Duration),
                        new OWLLiteral(new RDFTypedLiteral($"{Convert.ToString(timeInterval.Duration.Value, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DOUBLE))));
            }

            return ontology;
        }
        #endregion

        #region Analyzer
        public static List<TIMEEntity> GetTIMEEntityOfFeature(this OWLOntology ontology, RDFResource featureUri)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot get temporal extent of feature because given \"featureUri\" parameter is null");
            #endregion

            //Temporary working variables
            OWLObjectProperty hasTimeOP = new OWLObjectProperty(RDFVocabulary.TIME.HAS_TIME);
            OWLClass timeInstantCLS = new OWLClass(RDFVocabulary.TIME.INSTANT);
            OWLClass timeIntervalCLS = new OWLClass(RDFVocabulary.TIME.INTERVAL);
            List<OWLDataPropertyAssertion> dtPropAsns = OWLAssertionAxiomHelper.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(ontology);
            List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLObjectPropertyAssertion> hasTimeObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasTimeOP);

            //Filter assertions compatible with "time:hasTime" object property
            List<OWLObjectPropertyExpression> hasTimeObjPropExprs = ontology.GetSubObjectPropertiesOf(hasTimeOP)
                                                                      .Union(ontology.GetEquivalentObjectProperties(hasTimeOP)).ToList();
            foreach (OWLObjectPropertyExpression hasTimeObjPropExpr in hasTimeObjPropExprs)
                hasTimeObjPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasTimeObjPropExpr));

            //Iterate these assertions to reconstruct the temporal extent of corresponding temporal entity
            List<TIMEEntity> temporalExtentOfFeature = new List<TIMEEntity>();
            foreach (OWLObjectPropertyAssertion hasTimeObjPropsAsn in hasTimeObjPropAsns)
            {
                //Detect if the temporal extent is a time instant
                if (ontology.CheckIsIndividualOf(timeInstantCLS, hasTimeObjPropsAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time instant
                    TIMEInstant timeInstant = new TIMEInstant(hasTimeObjPropsAsn.TargetIndividualExpression.GetIRI());

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfInstant(ontology, timeInstant, dtPropAsns, objPropAsns);
                    FillDescriptionOfInstant(ontology, timeInstant, dtPropAsns, objPropAsns);
                    FillPositionOfInstant(ontology, timeInstant, dtPropAsns, objPropAsns);

                    //Collect the time instant
                    temporalExtentOfFeature.Add(timeInstant);
                }

                //Detect if the temporal extent is a time interval
                else if (ontology.CheckIsIndividualOf(timeIntervalCLS, hasTimeObjPropsAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time interval
                    TIMEInterval timeInterval = new TIMEInterval(hasTimeObjPropsAsn.TargetIndividualExpression.GetIRI());

                    //Analyze ontology to extract knowledge of the time interval
                    FillValueOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
                    FillDescriptionOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
                    FillDurationOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
                    FillBeginningOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
                    FillEndOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);

                    //Collect the time interval
                    temporalExtentOfFeature.Add(timeInterval);
                }
            }

            return temporalExtentOfFeature;
        }
        internal static void FillValueOfInstant(OWLOntology ontology, TIMEInstant timeInstant, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            //time:inXSDDateTimeStamp
            OWLLiteral inXSDDateTimeStampLiteral = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP))
                                                    .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(timeInstant))?.Literal;
            if (inXSDDateTimeStampLiteral?.GetLiteral() is RDFTypedLiteral inXSDDateTimeStampTLiteral
                 && inXSDDateTimeStampTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDDateTimeStampTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDDateTime
            OWLLiteral inXSDDateTimeLiteral = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATETIME))
                                               .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(timeInstant))?.Literal;
            if (inXSDDateTimeLiteral?.GetLiteral() is RDFTypedLiteral inXSDDateTimeTLiteral
                 && inXSDDateTimeTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATETIME))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDDateTimeTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDDate
            OWLLiteral inXSDDateLiteral = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_DATE))
                                           .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(timeInstant))?.Literal;
            if (inXSDDateLiteral?.GetLiteral() is RDFTypedLiteral inXSDDateTLiteral
                 && inXSDDateTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATE))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDDateTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDgYearMonth
            OWLLiteral inXSDGYearMonthLiteral = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_GYEARMONTH))
                                                 .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(timeInstant))?.Literal;
            if (inXSDGYearMonthLiteral?.GetLiteral() is RDFTypedLiteral inXSDGYearMonthTLiteral
                 && inXSDGYearMonthTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDGYearMonthTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDgYear
            OWLLiteral inXSDGYearLiteral = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.IN_XSD_GYEAR))
                                            .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(timeInstant))?.Literal;
            if (inXSDGYearLiteral?.GetLiteral() is RDFTypedLiteral inXSDGYearTLiteral
                 && inXSDGYearTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GYEAR))
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDGYearTLiteral.Value, XmlDateTimeSerializationMode.Utc);
        }
        internal static void FillDescriptionOfInstant(OWLOntology ontology, TIMEInstant timeInstant, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            #region Utilities
            RDFResource GetTRSOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFResource trs = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS))
                                   .FirstOrDefault(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.TargetIndividualExpression.GetIRI();
                //Default to Gregorian if this required information is not provided
                return  trs ?? TIMECalendarReferenceSystem.Gregorian;
            }
            RDFResource GetUnitTypeOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFResource unitType = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE))
                                        .FirstOrDefault(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.TargetIndividualExpression.GetIRI();
                //Default to Gregorian if this required information is not provided
                return  unitType ?? TIMEUnit.Second;
            }
            double? GetYearOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                OWLLiteral yearPM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.YEAR))
                                     .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.Literal;
                if (yearPM?.GetLiteral() is RDFTypedLiteral yearTL)
                {
                    //xsd:gYear
                    if (yearTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GYEAR))
                    {
                        DateTime yearDT = XmlConvert.ToDateTime(yearTL.Value, XmlDateTimeSerializationMode.Utc);
                        return Convert.ToDouble(yearDT.Year);
                    }

                    //time:generalYear or numeric literals
                    if (yearTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.TIME_GENERALYEAR) || yearTL.HasDecimalDatatype())
                        return Convert.ToDouble(yearTL.Value, CultureInfo.InvariantCulture);
                }
                return null;
            }
            double? GetMonthOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                OWLLiteral monthPM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.MONTH))
                                      .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.Literal;
                if (monthPM?.GetLiteral() is RDFTypedLiteral monthTL)
                {
                    //xsd:gMonth
                    if (monthTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GMONTH))
                    {
                        DateTime monthDT = XmlConvert.ToDateTime(monthTL.Value, XmlDateTimeSerializationMode.Utc);
                        return Convert.ToDouble(monthDT.Month);
                    }

                    //time:generalMonth
                    if (monthTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.TIME_GENERALMONTH))
                        return Convert.ToDouble(monthTL.Value.Replace("--", string.Empty), CultureInfo.InvariantCulture);

                    //numeric literals
                    if (monthTL.HasDecimalDatatype())
                        return Convert.ToDouble(monthTL.Value, CultureInfo.InvariantCulture);
                }
                return null;
            }
            double? GetDayOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                OWLLiteral dayPM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.DAY))
                                    .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.Literal;
                if (dayPM?.GetLiteral() is RDFTypedLiteral dayTL)
                {
                    //xsd:gDay
                    if (dayTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GDAY))
                    {
                        DateTime dayDT = XmlConvert.ToDateTime(dayTL.Value, XmlDateTimeSerializationMode.Utc);
                        return Convert.ToDouble(dayDT.Day);
                    }

                    //time:generalDay
                    if (dayTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.TIME_GENERALDAY))
                        return Convert.ToDouble(dayTL.Value.Replace("---", string.Empty), CultureInfo.InvariantCulture);

                    //numeric literals
                    if (dayTL.HasDecimalDatatype())
                        return Convert.ToDouble(dayTL.Value, CultureInfo.InvariantCulture);
                }
                return null;
            }
            double? GetHourOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                OWLLiteral hourPM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.HOUR))
                                     .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.Literal;
                if (hourPM?.GetLiteral() is RDFTypedLiteral hourTL && hourTL.HasDecimalDatatype())
                    return Convert.ToDouble(hourTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMinuteOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                OWLLiteral minutePM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.MINUTE))
                                       .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.Literal;
                if (minutePM?.GetLiteral() is RDFTypedLiteral minuteTL && minuteTL.HasDecimalDatatype())
                    return Convert.ToDouble(minuteTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetSecondOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                OWLLiteral secondPM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.SECOND))
                                       .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(dateTimeDescriptionURI))?.Literal;;
                if (secondPM?.GetLiteral() is RDFTypedLiteral secondTL && secondTL.HasDecimalDatatype())
                    return Convert.ToDouble(secondTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            void FillTextualDecorators(TIMEInstantDescription timeInstantDescription)
            {
                //time:MonthOfYear
                RDFResource monthOfYearPM = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR))
                                             .FirstOrDefault(asn => asn.SourceIndividualExpression.GetIRI().Equals(timeInstantDescription))?.TargetIndividualExpression.GetIRI();
                timeInstantDescription.Coordinate.Metadata.MonthOfYear = monthOfYearPM;

                //time:DayOfYear
                OWLLiteral dayOfYearPM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.DAY_OF_YEAR))
                                          .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(timeInstantDescription))?.Literal;
                if (dayOfYearPM?.GetLiteral() is RDFTypedLiteral dayOfYearTL && dayOfYearTL.HasDecimalDatatype())
                    timeInstantDescription.Coordinate.Metadata.DayOfYear = Convert.ToUInt32(dayOfYearTL.Value, CultureInfo.InvariantCulture);

                //time:DayOfWeek
                RDFResource dayOfWeek = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK))
                                         .FirstOrDefault(asn => asn.SourceIndividualExpression.GetIRI().Equals(timeInstantDescription))?.TargetIndividualExpression.GetIRI();
                timeInstantDescription.Coordinate.Metadata.DayOfWeek = dayOfWeek;
            }
            #endregion

            //Temporary working variables
            OWLObjectProperty inDateTimeOP = new OWLObjectProperty(RDFVocabulary.TIME.IN_DATETIME);
            OWLClass timeGeneralDateTimeDescriptionCLS = new OWLClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            List<OWLObjectPropertyAssertion> inDateTimeObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, inDateTimeOP);

            //Filter assertions compatible with "time:inDateTime" object property
            List<OWLObjectPropertyExpression> inDateTimeObjPropExprs = ontology.GetSubObjectPropertiesOf(inDateTimeOP)
                                                                        .Union(ontology.GetEquivalentObjectProperties(inDateTimeOP)).ToList();
            foreach (OWLObjectPropertyExpression inDateTimeObjPropExpr in inDateTimeObjPropExprs)
                inDateTimeObjPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, inDateTimeObjPropExpr));

            //Iterate these assertions to reconstruct the temporal extent of corresponding temporal entity
            List<TIMEInstantDescription> descriptionsOfTimeInstant = new List<TIMEInstantDescription>();
            foreach (OWLObjectPropertyAssertion inDateTimeObjPropAsn in inDateTimeObjPropAsns)
            {
                //Detect if the temporal extent is a general datetime description
                if (ontology.CheckIsIndividualOf(timeGeneralDateTimeDescriptionCLS, inDateTimeObjPropAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time instant description
                    RDFResource inDateTimeObjPropAsnTargetIRI = inDateTimeObjPropAsn.TargetIndividualExpression.GetIRI();
                    RDFResource trs = GetTRSOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    RDFResource unitType = GetUnitTypeOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    double? year = GetYearOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    double? month = GetMonthOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    double? day = GetDayOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    double? hour = GetHourOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    double? minute = GetMinuteOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    double? second = GetSecondOfInstantDescription(inDateTimeObjPropAsnTargetIRI);
                    TIMECoordinate timeInstantCoordinate = new TIMECoordinate(
                        year, month, day, hour, minute, second, new TIMECoordinateMetadata(trs, unitType));
                    TIMEInstantDescription timeInstantDescription = new TIMEInstantDescription(
                        inDateTimeObjPropAsnTargetIRI, timeInstantCoordinate);
                    FillTextualDecorators(timeInstantDescription);

                    descriptionsOfTimeInstant.Add(timeInstantDescription);
                }
            }
            timeInstant.Description = descriptionsOfTimeInstant.FirstOrDefault();
        }
        internal static void FillPositionOfInstant(OWLOntology ontology, TIMEInstant timeInstant, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            #region Utilities
            RDFResource GetTRSOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember trs = ontology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.HAS_TRS, null, null]
                                            ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (trs is RDFResource trsResource)
                    return trsResource;
                return null;
            }
            double? GetNumericValueOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember numericPositionPM = ontology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.NUMERIC_POSITION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (numericPositionPM is RDFTypedLiteral numericPositionTL && numericPositionTL.HasDecimalDatatype())
                    return Convert.ToDouble(numericPositionTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            RDFResource GetNominalValueOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember nominalPositionPM = ontology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.NOMINAL_POSITION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (nominalPositionPM is RDFResource nominalPositionResource)
                    return nominalPositionResource;
                return null;
            }
            RDFResource GetUncertaintyOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember uncertainty = ontology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY, null, null]
                                                ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (uncertainty is RDFResource uncertaintyResource)
                    return uncertaintyResource;
                return null;
            }
            #endregion

            //We need first to determine assertions having "time:inTimePosition" compatible predicates for the given time instant
            List<RDFResource> inTimePositionProperties = ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.IN_TIME_POSITION)
                                                           .Union(ontology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.IN_TIME_POSITION)).ToList();
            RDFGraph inTimePositionAssertions = ontology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_TIME_POSITION, null, null];
            foreach (RDFResource inTimePositionProperty in inTimePositionProperties)
                inTimePositionAssertions = inTimePositionAssertions.UnionWith(ontology.Data.ABoxGraph[timeInstant, inTimePositionProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the position of the time instant in its available aspects
            List<TIMEInstantPosition> positionsOfTimeInstant = new List<TIMEInstantPosition>();
            foreach (RDFTriple inTimePositionAssertion in inTimePositionAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:TemporalPosition
                if (ontology.Data.CheckIsIndividualOf(ontology.Model, (RDFResource)inTimePositionAssertion.Object, RDFVocabulary.TIME.TEMPORAL_POSITION))
                {
                    //Declare the discovered time position (numeric VS nominal)
                    RDFResource positionTRS = GetTRSOfPosition((RDFResource)inTimePositionAssertion.Object);
                    if (positionTRS != null)
                    {
                        TIMEInstantPosition timeInstantPosition = null;

                        //time:numericPosition
                        double? numericPositionValue = GetNumericValueOfPosition((RDFResource)inTimePositionAssertion.Object);
                        if (numericPositionValue.HasValue)
                            timeInstantPosition = new TIMEInstantPosition((RDFResource)inTimePositionAssertion.Object, positionTRS, numericPositionValue.Value);

                        //time:nominalPosition
                        else
                        {
                            RDFResource nominalPositionValue = GetNominalValueOfPosition((RDFResource)inTimePositionAssertion.Object);
                            if (nominalPositionValue != null)
                                timeInstantPosition = new TIMEInstantPosition((RDFResource)inTimePositionAssertion.Object, positionTRS, nominalPositionValue);
                        }

                        if (timeInstantPosition != null)
                        {
                            //thors:positionalUncertainty
                            RDFResource positionalUncertainty = GetUncertaintyOfPosition((RDFResource)inTimePositionAssertion.Object);
                            if (positionalUncertainty != null)
                            {
                                TIMEInterval positionalUncertaintyInterval = new TIMEInterval(positionalUncertainty);
                                FillDurationOfInterval(ontology, positionalUncertaintyInterval);
                                if (positionalUncertaintyInterval.Duration != null)
                                    timeInstantPosition.SetPositionalUncertainty(positionalUncertaintyInterval.Duration);
                            }

                            positionsOfTimeInstant.Add(timeInstantPosition);
                        }   
                    }
                }
            }
            timeInstant.Position = positionsOfTimeInstant.FirstOrDefault(); //We currently support one position, but this may evolve in future
        }
        internal static void FillValueOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            //time:hasXSDDuration
            RDFPatternMember hasXSDDurationLiteral = ontology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_XSD_DURATION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
            if (hasXSDDurationLiteral is RDFTypedLiteral hasXSDDurationTLiteral
                 && hasXSDDurationTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DURATION))
                timeInterval.TimeSpan = XmlConvert.ToTimeSpan(hasXSDDurationTLiteral.Value);
        }
        internal static void FillDescriptionOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            #region Utilities
            RDFResource GetTRSOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember trs = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.HAS_TRS, null, null]
                                         ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (trs is RDFResource trsResource)
                    return trsResource;

                //Default to Gregorian if this required information is not provided
                return TIMECalendarReferenceSystem.Gregorian;
            }
            double? GetYearsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember yearsPM = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.YEARS, null, null]
                                             ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (yearsPM is RDFTypedLiteral yearsTL && yearsTL.HasDecimalDatatype())
                    return Convert.ToDouble(yearsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMonthsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember monthsPM = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.MONTHS, null, null]
                                              ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (monthsPM is RDFTypedLiteral monthsTL && monthsTL.HasDecimalDatatype())
                    return Convert.ToDouble(monthsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetWeeksOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember weeksPM = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.WEEKS, null, null]
                                             ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (weeksPM is RDFTypedLiteral weeksTL && weeksTL.HasDecimalDatatype())
                    return Convert.ToDouble(weeksTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetDaysOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember daysPM = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.DAYS, null, null]
                                            ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (daysPM is RDFTypedLiteral daysTL && daysTL.HasDecimalDatatype())
                    return Convert.ToDouble(daysTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetHoursOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember hoursPM = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.HOURS, null, null]
                                             ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (hoursPM is RDFTypedLiteral hoursTL && hoursTL.HasDecimalDatatype())
                    return Convert.ToDouble(hoursTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMinutesOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember minutesPM = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.MINUTES, null, null]
                                               ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (minutesPM is RDFTypedLiteral minutesTL && minutesTL.HasDecimalDatatype())
                    return Convert.ToDouble(minutesTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetSecondsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember secondsPM = ontology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.SECONDS, null, null]
                                               ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (secondsPM is RDFTypedLiteral secondsTL && secondsTL.HasDecimalDatatype())
                    return Convert.ToDouble(secondsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            #endregion

            //We need first to determine assertions having "time:hasDurationDescription" compatible predicates for the given time interval
            List<RDFResource> hasDurationDescriptionProperties = ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION)
                                                                   .Union(ontology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION)).ToList();
            RDFGraph hasDurationDescriptionAssertions = ontology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, null, null];
            foreach (RDFResource hasDurationDescriptionProperty in hasDurationDescriptionProperties)
                hasDurationDescriptionAssertions = hasDurationDescriptionAssertions.UnionWith(ontology.Data.ABoxGraph[timeInterval, hasDurationDescriptionProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the description of the time interval in its available aspects
            List<TIMEIntervalDescription> descriptionsOfTimeInterval = new List<TIMEIntervalDescription>();
            foreach (RDFTriple hasDurationDescriptionAssertion in hasDurationDescriptionAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:GeneralDurationDescription
                if (ontology.Data.CheckIsIndividualOf(ontology.Model, (RDFResource)hasDurationDescriptionAssertion.Object, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION))
                {
                    //Declare the discovered time interval description
                    RDFResource trs = GetTRSOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    double? years = GetYearsOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    double? months = GetMonthsOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    double? weeks = GetWeeksOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    double? days = GetDaysOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    double? hours = GetHoursOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    double? minutes = GetMinutesOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    double? seconds = GetSecondsOfIntervalDescription((RDFResource)hasDurationDescriptionAssertion.Object);
                    TIMEExtent timeIntervalLength = new TIMEExtent(
                        years, months, weeks, days, hours, minutes, seconds, new TIMEExtentMetadata(trs));
                    TIMEIntervalDescription timeIntervalDescription = new TIMEIntervalDescription(
                        (RDFResource)hasDurationDescriptionAssertion.Object, timeIntervalLength);

                    descriptionsOfTimeInterval.Add(timeIntervalDescription);
                }
            }
            timeInterval.Description = descriptionsOfTimeInterval.FirstOrDefault(); //We currently support one description, but this may evolve in future
        }
        internal static void FillDurationOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            #region Utilities
            RDFResource GetUnitTypeOfDuration(RDFResource temporalExtentURI)
            {
                RDFPatternMember unitType = ontology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.UNIT_TYPE, null, null]
                                              ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (unitType is RDFResource unitTypeResource)
                    return unitTypeResource;

                //Default to Second if this required information is not provided
                return RDFVocabulary.TIME.UNIT_SECOND;
            }
            double GetValueOfDuration(RDFResource temporalExtentURI)
            {
                RDFPatternMember numericDurationPM = ontology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.NUMERIC_DURATION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (numericDurationPM is RDFTypedLiteral numericDurationTL && numericDurationTL.HasDecimalDatatype())
                    return Convert.ToDouble(numericDurationTL.Value, CultureInfo.InvariantCulture);
                return 0;
            }
            #endregion

            //We need first to determine assertions having "time:hasDuration" compatible predicates for the given time interval
            List<RDFResource> hasDurationProperties = ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_DURATION)
                                                        .Union(ontology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_DURATION)
                                                          //We need to also support "thors:positionalUncertainty" (even if not related with "time:hasDuration")
                                                          .Union(new List<RDFResource>() { RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY })).ToList();
            RDFGraph hasDurationAssertions = ontology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_DURATION, null, null];
            foreach (RDFResource hasDurationProperty in hasDurationProperties)
                hasDurationAssertions = hasDurationAssertions.UnionWith(ontology.Data.ABoxGraph[timeInterval, hasDurationProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the duration of the time interval in its available aspects
            List<TIMEIntervalDuration> durationsOfTimeInterval = new List<TIMEIntervalDuration>();
            foreach (RDFTriple hasDurationAssertion in hasDurationAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:Duration
                if (ontology.Data.CheckIsIndividualOf(ontology.Model, (RDFResource)hasDurationAssertion.Object, RDFVocabulary.TIME.DURATION))
                {
                    //Declare the discovered time duration
                    TIMEIntervalDuration timeIntervalDuration = new TIMEIntervalDuration((RDFResource)hasDurationAssertion.Object)
                    {
                        UnitType = GetUnitTypeOfDuration((RDFResource)hasDurationAssertion.Object),
                        Value = GetValueOfDuration((RDFResource)hasDurationAssertion.Object)                        
                    };
                    durationsOfTimeInterval.Add(timeIntervalDuration);
                }
            }
            timeInterval.Duration = durationsOfTimeInterval.FirstOrDefault(); //We currently support one duration, but this may evolve in future
        }
        internal static void FillBeginningOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            //We need first to determine assertions having "time:hasBeginning" compatible predicates for the given time interval
            List<RDFResource> hasBeginningProperties = ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_BEGINNING)
                                                         .Union(ontology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_BEGINNING)).ToList();
            RDFGraph hasBeginningAssertions = ontology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_BEGINNING, null, null];
            foreach (RDFResource hasBeginningProperty in hasBeginningProperties)
                hasBeginningAssertions = hasBeginningAssertions.UnionWith(ontology.Data.ABoxGraph[timeInterval, hasBeginningProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the time instant in its available aspects
            List<TIMEInstant> beginningsOfTimeInterval = new List<TIMEInstant>();
            foreach (RDFTriple hasBeginningAssertion in hasBeginningAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:Instant
                if (ontology.Data.CheckIsIndividualOf(ontology.Model, (RDFResource)hasBeginningAssertion.Object, RDFVocabulary.TIME.INSTANT))
                {
                    //Declare the discovered time instant
                    TIMEInstant beginningTimeInstant = new TIMEInstant((RDFResource)hasBeginningAssertion.Object);

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfInstant(ontology, beginningTimeInstant);
                    FillDescriptionOfInstant(ontology, beginningTimeInstant);
                    FillPositionOfInstant(ontology, beginningTimeInstant);

                    //Collect the time instant into temporal extent of the time interval
                    beginningsOfTimeInterval.Add(beginningTimeInstant);
                }
            }
            timeInterval.Beginning = beginningsOfTimeInterval.FirstOrDefault(); //We currently support one beginning instant, but this may evolve in future
        }
        internal static void FillEndOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            //We need first to determine assertions having "time:hasEnd" compatible predicates for the given time interval
            List<RDFResource> hasEndProperties = ontology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_END)
                                                   .Union(ontology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_END)).ToList();
            RDFGraph hasEndAssertions = ontology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_END, null, null];
            foreach (RDFResource hasEndProperty in hasEndProperties)
                hasEndAssertions = hasEndAssertions.UnionWith(ontology.Data.ABoxGraph[timeInterval, hasEndProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the time instant in its available aspects
            List<TIMEInstant> endsOfTimeInterval = new List<TIMEInstant>();
            foreach (RDFTriple hasEndAssertion in hasEndAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:Instant
                if (ontology.Data.CheckIsIndividualOf(ontology.Model, (RDFResource)hasEndAssertion.Object, RDFVocabulary.TIME.INSTANT))
                {
                    //Declare the discovered time instant
                    TIMEInstant endTimeInstant = new TIMEInstant((RDFResource)hasEndAssertion.Object);

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfInstant(ontology, endTimeInstant);
                    FillDescriptionOfInstant(ontology, endTimeInstant);
                    FillPositionOfInstant(ontology, endTimeInstant);

                    //Collect the time instant into temporal extent of the time interval
                    endsOfTimeInterval.Add(endTimeInstant);
                }
            }
            timeInterval.End = endsOfTimeInterval.FirstOrDefault(); //We currently support one end instant, but this may evolve in future
        }
        [ExcludeFromCodeCoverage]
        internal static RDFResource GetMonthOfYear(int? month)
        {
            if (!month.HasValue)
                return null;

            switch (month)
            {
                case  1: return RDFVocabulary.TIME.GREG.JANUARY;
                case  2: return RDFVocabulary.TIME.GREG.FEBRUARY;
                case  3: return RDFVocabulary.TIME.GREG.MARCH;
                case  4: return RDFVocabulary.TIME.GREG.APRIL;
                case  5: return RDFVocabulary.TIME.GREG.MAY;
                case  6: return RDFVocabulary.TIME.GREG.JUNE;
                case  7: return RDFVocabulary.TIME.GREG.JULY;
                case  8: return RDFVocabulary.TIME.GREG.AUGUST;
                case  9: return RDFVocabulary.TIME.GREG.SEPTEMBER;
                case 10: return RDFVocabulary.TIME.GREG.OCTOBER;
                case 11: return RDFVocabulary.TIME.GREG.NOVEMBER;
                case 12: return RDFVocabulary.TIME.GREG.DECEMBER;
                default: return null;
            }
        }
        [ExcludeFromCodeCoverage]
        internal static RDFResource GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:    return RDFVocabulary.TIME.SUNDAY;
                case DayOfWeek.Monday:    return RDFVocabulary.TIME.MONDAY;
                case DayOfWeek.Tuesday:   return RDFVocabulary.TIME.TUESDAY;
                case DayOfWeek.Wednesday: return RDFVocabulary.TIME.WEDNESDAY;
                case DayOfWeek.Thursday:  return RDFVocabulary.TIME.THURSDAY;
                case DayOfWeek.Friday:    return RDFVocabulary.TIME.FRIDAY;
                case DayOfWeek.Saturday:  return RDFVocabulary.TIME.SATURDAY;
                default:                  return null;
            }
        }
        
        public static TIMECoordinate GetCoordinateOfInstant(this OWLOntology ontology, RDFResource timeInstantURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeInstantURI == null)
                throw new OWLException("Cannot get temporal coordinate of instant because given \"timeInstantURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Declare the time instant
            TIMEInstant timeInstant = new TIMEInstant(timeInstantURI);

            //Analyze ontology to extract knowledge of the time instant
            FillValueOfInstant(ontology, timeInstant);
            FillDescriptionOfInstant(ontology, timeInstant);
            FillPositionOfInstant(ontology, timeInstant);

            //The time instant has been encoded in DateTime
            if (timeInstant.DateTime.HasValue)
                return TIMEConverter.NormalizeCoordinate(new TIMECoordinate(timeInstant.DateTime.Value), calendarTRS);

            //The time instant has been encoded in a structured 6-component description
            if (timeInstant.Description != null)
                return TIMEConverter.NormalizeCoordinate(timeInstant.Description.Coordinate, calendarTRS);

            //The time instant has been encoded in a nominal value or a numeric coordinate
            if (timeInstant.Position != null)
            {
                //Nominal position (e.g: Uri of a well-known interval, which is part of an ordinal TRS)
                if (timeInstant.Position.IsNominal)
                {
                    TIMECoordinate nominalPositionCoordinate = GetBeginningOfInterval(ontology, timeInstant.Position.NominalValue, calendarTRS);
                    if (nominalPositionCoordinate != null)
                        return TIMEConverter.NormalizeCoordinate(nominalPositionCoordinate, calendarTRS);
                }

                //Numeric position (e.g: seconds elapsed from 1st January 1970, which describes UnixTime Epoch)
                else
                {
                    #region Guards
                    //We can proceed only if the detected TRS has been correctly registered into the TRS registry
                    if (!TIMEReferenceSystemRegistry.ContainsTRS(timeInstant.Position.TRS))
                        throw new OWLException($"Cannot get coordinate from instant because TRS '{timeInstant.Position.TRS}' is unknown. Please register it as a positional TRS into TIMEReferenceSystemRegistry.");
                    if (TIMEReferenceSystemRegistry.Instance.TRS[timeInstant.Position.TRS.ToString()] is TIMECalendarReferenceSystem)
                        throw new OWLException($"Cannot get coordinate from instant because TRS '{timeInstant.Position.TRS}' is required to be a positional TRS, instead it has been registered as a calendar TRS into TIMEReferenceSystemRegistry.");
                    #endregion

                    TIMEPositionReferenceSystem positionTRS = (TIMEPositionReferenceSystem)TIMEReferenceSystemRegistry.Instance.TRS[timeInstant.Position.TRS.ToString()];
                    return TIMEConverter.NormalizeCoordinate(
                             TIMEConverter.PositionToCoordinate(timeInstant.Position.NumericValue, positionTRS, calendarTRS), calendarTRS);
                }
            }

            //In absence of a valid time instant encoding we simply cannot proceed
            return null;
        }

        public static TIMEExtent GetExtentOfInterval(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get temporal extent of interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Declare the time interval
            TIMEInterval timeInterval = new TIMEInterval(timeIntervalURI);

            //Analyze ontology to extract knowledge of the time interval
            FillValueOfInterval(ontology, timeInterval);
            FillDescriptionOfInterval(ontology, timeInterval);
            FillDurationOfInterval(ontology, timeInterval);
            FillBeginningOfInterval(ontology, timeInterval);
            FillEndOfInterval(ontology, timeInterval);

            //The time interval has been encoded in TimeSpan
            if (timeInterval.TimeSpan.HasValue)
                return TIMEConverter.NormalizeExtent(new TIMEExtent(timeInterval.TimeSpan.Value), calendarTRS);

            //The time interval has been encoded in a structured 7-component description
            if (timeInterval.Description != null)
                return TIMEConverter.NormalizeExtent(timeInterval.Description.Extent, calendarTRS);

            //The time interval has been encoded in a numeric duration extent
            if (timeInterval.Duration != null)
            {
                #region Guards
                //We can proceed only if the detected UnitType has been correctly registered into the UnitType registry
                if (!TIMEUnitTypeRegistry.ContainsUnitType(timeInterval.Duration.UnitType))
                    throw new OWLException($"Cannot get extent from interval because UnitType '{timeInterval.Duration.UnitType}' is unknown. Please register it into TIMEUnitTypeRegistry.");
                #endregion

                TIMEUnit durationUnit = TIMEUnitTypeRegistry.Instance.UnitTypes[timeInterval.Duration.UnitType.ToString()];
                return TIMEConverter.NormalizeExtent(
                        TIMEConverter.DurationToExtent(timeInterval.Duration.Value, durationUnit, calendarTRS), calendarTRS);
            }

            //The time interval has been encoded in Beginning/End time instants
            if (timeInterval.Beginning != null && timeInterval.End != null)
            {
                TIMECoordinate timeIntervalBeginning = GetCoordinateOfInstant(ontology, timeInterval.Beginning, calendarTRS);
                TIMECoordinate timeIntervalEnd = GetCoordinateOfInstant(ontology, timeInterval.End, calendarTRS);
                if (timeIntervalBeginning != null && timeIntervalEnd != null)
                    return TIMEConverter.CalculateExtent(timeIntervalBeginning, timeIntervalEnd, calendarTRS);
            }

            //In absence of a valid time interval encoding we simply cannot proceed
            return null;
        }

        public static TIMECoordinate GetBeginningOfInterval(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get beginning of interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            return GetBeginningOfIntervalInternal(ontology, timeIntervalURI, calendarTRS, new Dictionary<long, RDFResource>());
        }
        internal static TIMECoordinate GetBeginningOfIntervalInternal(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS, Dictionary<long,RDFResource> visitContext)
        {
            #region visitContext
            if (!visitContext.ContainsKey(timeIntervalURI.PatternMemberID))
                visitContext.Add(timeIntervalURI.PatternMemberID, timeIntervalURI);
            else
                return null;
            #endregion

            //Declare the time interval
            TIMEInterval timeInterval = new TIMEInterval(timeIntervalURI);

            //Analyze ontology to extract knowledge of the time interval
            FillBeginningOfInterval(ontology, timeInterval);

            //Get the coordinate of the time interval's beginning instant
            if (timeInterval.Beginning != null)
                return GetCoordinateOfInstant(ontology, timeInterval.Beginning, calendarTRS);

            #region Allen
            //There's no direct representation of the time interval's beginning instant:
            //we can indirectly obtain it by exploiting specific Allen interval relations
            List<RDFResource> compatibleStartingIntervals = RDFQueryUtilities.RemoveDuplicates(
                GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Equals)
                 .Union(GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Starts))
                  .Union(GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.StartedBy))
                  .ToList());

            //Perform a sequential search of the beginning instant on the set of compatible intervals
            TIMECoordinate compatibleBeginning = null;
            IEnumerator<RDFResource> compatibleStartingIntervalsEnumerator = compatibleStartingIntervals.GetEnumerator();
            while (compatibleBeginning == null && compatibleStartingIntervalsEnumerator.MoveNext())
                compatibleBeginning = GetBeginningOfIntervalInternal(ontology, compatibleStartingIntervalsEnumerator.Current, calendarTRS, visitContext);
            if (compatibleBeginning == null)
            {
                #region MetBy
                //There's still no direct representation of the time interval's beginning instant:
                //we can indirectly obtain it by exploiting Allen interval-meeting relations
                List<RDFResource> metByIntervals = RDFQueryUtilities.RemoveDuplicates(
                    GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.MetBy));

                //Perform a sequential search of the start instant on the set of metBy intervals
                IEnumerator<RDFResource> metByIntervalsEnumerator = metByIntervals.GetEnumerator();
                while (compatibleBeginning == null && metByIntervalsEnumerator.MoveNext())
                    compatibleBeginning = GetEndOfIntervalInternal(ontology, metByIntervalsEnumerator.Current, calendarTRS, visitContext);
                #endregion
            }
            #endregion

            return compatibleBeginning;
        }
        public static TIMECoordinate GetEndOfInterval(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get end of interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            return GetEndOfIntervalInternal(ontology, timeIntervalURI, calendarTRS, new Dictionary<long, RDFResource>());
        }
        internal static TIMECoordinate GetEndOfIntervalInternal(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS, Dictionary<long,RDFResource> visitContext)
        {
            #region visitContext
            if (!visitContext.ContainsKey(timeIntervalURI.PatternMemberID))
                visitContext.Add(timeIntervalURI.PatternMemberID, timeIntervalURI);
            else
                return null;
            #endregion

            //Declare the time interval
            TIMEInterval timeInterval = new TIMEInterval(timeIntervalURI);

            //Analyze ontology to extract knowledge of the time interval
            FillEndOfInterval(ontology, timeInterval);

            //Get the coordinate of the time interval's end instant
            if (timeInterval.End != null)
                return GetCoordinateOfInstant(ontology, timeInterval.End, calendarTRS);

            #region Allen
            //There's no direct representation of the time interval's end instant:
            //we can indirectly obtain it by exploiting Allen interval-ending relations
            List<RDFResource> compatibleEndIntervals = RDFQueryUtilities.RemoveDuplicates(
                GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Equals)
                 .Union(GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Finishes))
                  .Union(GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.FinishedBy))
                  .ToList());

            //Perform a sequential search of the end instant on the set of compatible ending intervals
            TIMECoordinate compatibleEnd = null;
            IEnumerator<RDFResource> compatibleEndIntervalsEnumerator = compatibleEndIntervals.GetEnumerator();
            while (compatibleEnd == null && compatibleEndIntervalsEnumerator.MoveNext())
                compatibleEnd = GetEndOfIntervalInternal(ontology, compatibleEndIntervalsEnumerator.Current, calendarTRS, visitContext);
            if (compatibleEnd == null)
            {
                #region Meets
                //There's still no direct representation of the time interval's end instant:
                //we can indirectly obtain it by exploiting Allen interval-meeting relations
                List<RDFResource> meetsIntervals = RDFQueryUtilities.RemoveDuplicates(
                    GetRelatedIntervals(ontology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Meets));

                //Perform a sequential search of the end instant on the set of meets intervals
                IEnumerator<RDFResource> meetsIntervalsEnumerator = meetsIntervals.GetEnumerator();
                while (compatibleEnd == null && meetsIntervalsEnumerator.MoveNext())
                    compatibleEnd = GetBeginningOfIntervalInternal(ontology, meetsIntervalsEnumerator.Current, calendarTRS, visitContext);
                #endregion
            }
            #endregion

            return compatibleEnd;
        }
        #endregion
    }
}
