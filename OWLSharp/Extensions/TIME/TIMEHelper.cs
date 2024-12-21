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
        public static List<TIMEEntity> GetTemporalExtentOfFeature(this OWLOntology ontology, RDFResource featureUri)
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
                    TIMEInstantDescription timeInstantDescription = new TIMEInstantDescription(inDateTimeObjPropAsnTargetIRI, timeInstantCoordinate);
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
                RDFResource trs = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS))
                                   .FirstOrDefault(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(temporalExtentURI))?.TargetIndividualExpression.GetIRI();
                //Default to Gregorian if this required information is not provided
                return  trs ?? TIMECalendarReferenceSystem.Gregorian;
            }
            double? GetNumericValueOfPosition(RDFResource temporalExtentURI)
            {
                OWLLiteral numericPositionPM = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_POSITION))
                                                .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(temporalExtentURI))?.Literal;
                if (numericPositionPM?.GetLiteral() is RDFTypedLiteral numericPositionTL && numericPositionTL.HasDecimalDatatype())
                    return Convert.ToDouble(numericPositionTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            RDFResource GetNominalValueOfPosition(RDFResource temporalExtentURI)
            {
                RDFResource nominalPosition = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION))
                                               .FirstOrDefault(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(temporalExtentURI))?.TargetIndividualExpression.GetIRI();
                return nominalPosition;
            }
            RDFResource GetUncertaintyOfPosition(RDFResource temporalExtentURI)
            {
                RDFResource positionalUncertainty = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY))
                                                     .FirstOrDefault(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(temporalExtentURI))?.TargetIndividualExpression.GetIRI();
                return positionalUncertainty;
            }
            #endregion

            //Temporary working variables
            OWLObjectProperty inTimePositionOP = new OWLObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION);
            OWLClass temporalPositionCLS = new OWLClass(RDFVocabulary.TIME.TEMPORAL_POSITION);
            List<OWLObjectPropertyAssertion> inTimePositionObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, inTimePositionOP);

            //Filter assertions compatible with "time:inTimePosition" object property
            List<OWLObjectPropertyExpression> inTimePositionObjPropExprs = ontology.GetSubObjectPropertiesOf(inTimePositionOP)
                                                                            .Union(ontology.GetEquivalentObjectProperties(inTimePositionOP)).ToList();
            foreach (OWLObjectPropertyExpression inTimePositionObjPropExpr in inTimePositionObjPropExprs)
                inTimePositionObjPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, inTimePositionObjPropExpr));

            //Iterate these assertions to reconstruct the temporal extent of corresponding temporal entity
            List<TIMEInstantPosition> positionsOfTimeInstant = new List<TIMEInstantPosition>();
            foreach (OWLObjectPropertyAssertion inTimePositionObjPropAsn in inTimePositionObjPropAsns)
            {
                //Detect if the temporal extent is a temporal position
                if (ontology.CheckIsIndividualOf(temporalPositionCLS, inTimePositionObjPropAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time position (numeric VS nominal)
                    RDFResource inTimePositionObjPropAsnTargetIRI = inTimePositionObjPropAsn.TargetIndividualExpression.GetIRI();
                    RDFResource positionTRS = GetTRSOfPosition(inTimePositionObjPropAsnTargetIRI);
                    if (positionTRS != null)
                    {
                        TIMEInstantPosition timeInstantPosition = null;

                        //time:numericPosition
                        double? numericPositionValue = GetNumericValueOfPosition(inTimePositionObjPropAsnTargetIRI);
                        if (numericPositionValue.HasValue)
                            timeInstantPosition = new TIMEInstantPosition(inTimePositionObjPropAsnTargetIRI, positionTRS, numericPositionValue.Value);

                        //time:nominalPosition
                        else
                        {
                            RDFResource nominalPositionValue = GetNominalValueOfPosition(inTimePositionObjPropAsnTargetIRI);
                            if (nominalPositionValue != null)
                                timeInstantPosition = new TIMEInstantPosition(inTimePositionObjPropAsnTargetIRI, positionTRS, nominalPositionValue);
                        }

                        if (timeInstantPosition != null)
                        {
                            //thors:positionalUncertainty
                            RDFResource positionalUncertainty = GetUncertaintyOfPosition(inTimePositionObjPropAsnTargetIRI);
                            if (positionalUncertainty != null)
                            {
                                TIMEInterval positionalUncertaintyInterval = new TIMEInterval(positionalUncertainty);
                                FillDurationOfInterval(ontology, positionalUncertaintyInterval, dtPropAsns, objPropAsns);
                                if (positionalUncertaintyInterval.Duration != null)
                                    timeInstantPosition.PositionalUncertainty = positionalUncertaintyInterval.Duration;
                            }

                            positionsOfTimeInstant.Add(timeInstantPosition);
                        }   
                    }
                }
            }
            timeInstant.Position = positionsOfTimeInstant.FirstOrDefault();
        }
        internal static void FillValueOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            //time:hasXSDDuration
            OWLLiteral hasXSDDurationLiteral = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.HAS_XSD_DURATION))
                                                .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(timeInterval)).Literal;
            if (hasXSDDurationLiteral?.GetLiteral() is RDFTypedLiteral hasXSDDurationTLiteral
                 && hasXSDDurationTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DURATION))
                timeInterval.TimeSpan = XmlConvert.ToTimeSpan(hasXSDDurationTLiteral.Value);
        }
        internal static void FillDescriptionOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            #region Utilities
            RDFResource GetTRSOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFResource trs = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.HAS_TRS))
                                   .FirstOrDefault(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(durationDescriptionURI))?.TargetIndividualExpression.GetIRI();
                //Default to Gregorian if this required information is not provided
                return  trs ?? TIMECalendarReferenceSystem.Gregorian;
            }
            double? GetYearsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                OWLLiteral years = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.YEARS))
                                    .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(durationDescriptionURI))?.Literal;
                if (years?.GetLiteral() is RDFTypedLiteral yearsTL && yearsTL.HasDecimalDatatype())
                    return Convert.ToDouble(yearsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMonthsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                OWLLiteral months = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.MONTHS))
                                     .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(durationDescriptionURI))?.Literal;
                if (months?.GetLiteral() is RDFTypedLiteral monthsTL && monthsTL.HasDecimalDatatype())
                    return Convert.ToDouble(monthsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetWeeksOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                OWLLiteral weeks = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.WEEKS))
                                     .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(durationDescriptionURI))?.Literal;
                if (weeks?.GetLiteral() is RDFTypedLiteral weeksTL && weeksTL.HasDecimalDatatype())
                    return Convert.ToDouble(weeksTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetDaysOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                OWLLiteral days = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.DAYS))
                                   .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(durationDescriptionURI))?.Literal;
                if (days?.GetLiteral() is RDFTypedLiteral daysTL && daysTL.HasDecimalDatatype())
                    return Convert.ToDouble(daysTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetHoursOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                OWLLiteral hours = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.HOURS))
                                    .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(durationDescriptionURI))?.Literal;
                if (hours?.GetLiteral() is RDFTypedLiteral hoursTL && hoursTL.HasDecimalDatatype())
                    return Convert.ToDouble(hoursTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMinutesOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                OWLLiteral minutes = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.MINUTES))
                                      .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(durationDescriptionURI))?.Literal;
                if (minutes?.GetLiteral() is RDFTypedLiteral minutesTL && minutesTL.HasDecimalDatatype())
                    return Convert.ToDouble(minutesTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetSecondsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                OWLLiteral seconds = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.SECONDS))
                                      .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(durationDescriptionURI))?.Literal;
                if (seconds?.GetLiteral() is RDFTypedLiteral secondsTL && secondsTL.HasDecimalDatatype())
                    return Convert.ToDouble(secondsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            #endregion

            //Temporary working variables
            OWLObjectProperty hasDurationDescriptionOP = new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION);
            OWLClass timeGeneralDurationDescriptionCLS = new OWLClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            List<OWLObjectPropertyAssertion> hasDurationDescriptionObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasDurationDescriptionOP);

            //Filter assertions compatible with "time:hasDurationDescription" object property
            List<OWLObjectPropertyExpression> hasDurationDescriptionObjPropExprs = ontology.GetSubObjectPropertiesOf(hasDurationDescriptionOP)
                                                                                    .Union(ontology.GetEquivalentObjectProperties(hasDurationDescriptionOP)).ToList();
            foreach (OWLObjectPropertyExpression hasDurationDescriptionObjPropExpr in hasDurationDescriptionObjPropExprs)
                hasDurationDescriptionObjPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasDurationDescriptionObjPropExpr));

            //Iterate these assertions to reconstruct the temporal extent of corresponding temporal entity
            List<TIMEIntervalDescription> descriptionsOfTimeInterval = new List<TIMEIntervalDescription>();
            foreach (OWLObjectPropertyAssertion hasDurationDescriptionObjPropAsn in hasDurationDescriptionObjPropAsns)
            {
                //Detect if the temporal extent is a general duration description
                if (ontology.CheckIsIndividualOf(timeGeneralDurationDescriptionCLS, hasDurationDescriptionObjPropAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time interval description
                    RDFResource hasDurationDescriptionObjPropAsnTargetIRI = hasDurationDescriptionObjPropAsn.TargetIndividualExpression.GetIRI();
                    RDFResource trs = GetTRSOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    double? years = GetYearsOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    double? months = GetMonthsOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    double? weeks = GetWeeksOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    double? days = GetDaysOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    double? hours = GetHoursOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    double? minutes = GetMinutesOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    double? seconds = GetSecondsOfIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI);
                    TIMEExtent timeIntervalLength = new TIMEExtent(
                        years, months, weeks, days, hours, minutes, seconds, new TIMEExtentMetadata(trs));
                    TIMEIntervalDescription timeIntervalDescription = new TIMEIntervalDescription(hasDurationDescriptionObjPropAsnTargetIRI, timeIntervalLength);

                    descriptionsOfTimeInterval.Add(timeIntervalDescription);
                }
            }
            timeInterval.Description = descriptionsOfTimeInterval.FirstOrDefault();
        }
        internal static void FillDurationOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            #region Utilities
            RDFResource GetUnitTypeOfDuration(RDFResource temporalExtentURI)
            {
                RDFResource unitType = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.UNIT_TYPE))
                                        .FirstOrDefault(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(temporalExtentURI))?.TargetIndividualExpression.GetIRI();
                //Default to Gregorian if this required information is not provided
                return  unitType ?? TIMEUnit.Second;
            }
            double GetValueOfDuration(RDFResource temporalExtentURI)
            {
                OWLLiteral numericDuration = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dtPropAsns, new OWLDataProperty(RDFVocabulary.TIME.NUMERIC_DURATION))
                                              .FirstOrDefault(asn => asn.IndividualExpression.GetIRI().Equals(temporalExtentURI))?.Literal;
                if (numericDuration?.GetLiteral() is RDFTypedLiteral numericDurationTL && numericDurationTL.HasDecimalDatatype())
                    return Convert.ToDouble(numericDurationTL.Value, CultureInfo.InvariantCulture);
                return 0;
            }
            #endregion

            //Temporary working variables
            OWLObjectProperty hasDurationOP = new OWLObjectProperty(RDFVocabulary.TIME.HAS_DURATION);
            OWLObjectProperty positionalUncertaintyOP = new OWLObjectProperty(RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY);
            OWLClass durationCLS = new OWLClass(RDFVocabulary.TIME.DURATION);
            List<OWLObjectPropertyAssertion> hasDurationObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasDurationOP)
                                                                       .Union(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, positionalUncertaintyOP)).ToList();

            //Filter assertions compatible with "time:hasDuration" object property
            List<OWLObjectPropertyExpression> hasDurationObjPropExprs = ontology.GetSubObjectPropertiesOf(hasDurationOP)
                                                                         .Union(ontology.GetEquivalentObjectProperties(hasDurationOP)).ToList();
            //Filter assertions compatible with "thors:positionalUncertainty" object property
            List<OWLObjectPropertyExpression> positionalUncertaintyObjPropExprs = ontology.GetSubObjectPropertiesOf(positionalUncertaintyOP)
                                                                                   .Union(ontology.GetEquivalentObjectProperties(positionalUncertaintyOP)).ToList();
            foreach (OWLObjectPropertyExpression hasDurationObjPropExpr in hasDurationObjPropExprs.Union(positionalUncertaintyObjPropExprs))
                hasDurationObjPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasDurationObjPropExpr));

            //Iterate these assertions to reconstruct the temporal extent of corresponding temporal entity
            List<TIMEIntervalDuration> durationsOfTimeInterval = new List<TIMEIntervalDuration>();
            foreach (OWLObjectPropertyAssertion hasDurationObjPropAsn in hasDurationObjPropAsns)
            {
                //Detect if the temporal extent is a temporal duration
                if (ontology.CheckIsIndividualOf(durationCLS, hasDurationObjPropAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time duration
                    RDFResource hasDurationObjPropAsnTargetIRI = hasDurationObjPropAsn.TargetIndividualExpression.GetIRI();
                    TIMEIntervalDuration timeIntervalDuration = new TIMEIntervalDuration(hasDurationObjPropAsnTargetIRI)
                    {
                        UnitType = GetUnitTypeOfDuration(hasDurationObjPropAsnTargetIRI),
                        Value = GetValueOfDuration(hasDurationObjPropAsnTargetIRI)
                    };
                    durationsOfTimeInterval.Add(timeIntervalDuration);
                }
            }
            timeInterval.Duration = durationsOfTimeInterval.FirstOrDefault();
        }
        internal static void FillBeginningOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            //Temporary working variables
            OWLObjectProperty hasBeginningOP = new OWLObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING);
            OWLClass instantCLS = new OWLClass(RDFVocabulary.TIME.INSTANT);
            List<OWLObjectPropertyAssertion> hasBeginningObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasBeginningOP);

            //Filter assertions compatible with "time:hasBeginning" object property
            List<OWLObjectPropertyExpression> hasBeginningObjPropExprs = ontology.GetSubObjectPropertiesOf(hasBeginningOP)
                                                                          .Union(ontology.GetEquivalentObjectProperties(hasBeginningOP)).ToList();
            foreach (OWLObjectPropertyExpression hasBeginningObjPropExpr in hasBeginningObjPropExprs)
                hasBeginningObjPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasBeginningObjPropExpr));

            //Iterate these assertions to reconstruct the temporal extent of corresponding temporal entity
            List<TIMEInstant> beginningsOfTimeInterval = new List<TIMEInstant>();
            foreach (OWLObjectPropertyAssertion hasBeginningObjPropAsn in hasBeginningObjPropAsns)
            {
                //Detect if the temporal extent is a time instant
                if (ontology.CheckIsIndividualOf(instantCLS, hasBeginningObjPropAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time instant
                    TIMEInstant beginningTimeInstant = new TIMEInstant(hasBeginningObjPropAsn.TargetIndividualExpression.GetIRI());

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfInstant(ontology, beginningTimeInstant, dtPropAsns, objPropAsns);
                    FillDescriptionOfInstant(ontology, beginningTimeInstant, dtPropAsns, objPropAsns);
                    FillPositionOfInstant(ontology, beginningTimeInstant, dtPropAsns, objPropAsns);

                    //Collect the time instant
                    beginningsOfTimeInterval.Add(beginningTimeInstant);
                }
            }
            timeInterval.Beginning = beginningsOfTimeInterval.FirstOrDefault();
        }
        internal static void FillEndOfInterval(OWLOntology ontology, TIMEInterval timeInterval, List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns)
        {
            //Temporary working variables
            OWLObjectProperty hasEndOP = new OWLObjectProperty(RDFVocabulary.TIME.HAS_END);
            OWLClass instantCLS = new OWLClass(RDFVocabulary.TIME.INSTANT);
            List<OWLObjectPropertyAssertion> hasEndObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasEndOP);

            //Filter assertions compatible with "time:hasEnd" object property
            List<OWLObjectPropertyExpression> hasEndObjPropExprs = ontology.GetSubObjectPropertiesOf(hasEndOP)
                                                                    .Union(ontology.GetEquivalentObjectProperties(hasEndOP)).ToList();
            foreach (OWLObjectPropertyExpression hasEndObjPropExpr in hasEndObjPropExprs)
                hasEndObjPropAsns.AddRange(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, hasEndObjPropExpr));

            //Iterate these assertions to reconstruct the temporal extent of corresponding temporal entity
            List<TIMEInstant> endsOfTimeInterval = new List<TIMEInstant>();
            foreach (OWLObjectPropertyAssertion hasEndObjPropAsn in hasEndObjPropAsns)
            {
                //Detect if the temporal extent is a time instant
                if (ontology.CheckIsIndividualOf(instantCLS, hasEndObjPropAsn.TargetIndividualExpression))
                {
                    //Declare the discovered time instant
                    TIMEInstant endTimeInstant = new TIMEInstant(hasEndObjPropAsn.TargetIndividualExpression.GetIRI());

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfInstant(ontology, endTimeInstant, dtPropAsns, objPropAsns);
                    FillDescriptionOfInstant(ontology, endTimeInstant, dtPropAsns, objPropAsns);
                    FillPositionOfInstant(ontology, endTimeInstant, dtPropAsns, objPropAsns);

                    //Collect the time instant
                    endsOfTimeInterval.Add(endTimeInstant);
                }
            }
            timeInterval.End = endsOfTimeInterval.FirstOrDefault();
        }

        public static TIMECoordinate GetCoordinateOfTIMEInstant(this OWLOntology ontology, RDFResource timeInstantURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeInstantURI == null)
                throw new OWLException("Cannot get temporal coordinate of instant because given \"timeInstantURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Temporary working variables
            List<OWLDataPropertyAssertion> dtPropAsns = OWLAssertionAxiomHelper.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(ontology);
            List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

            //Declare the time instant
            TIMEInstant timeInstant = new TIMEInstant(timeInstantURI);

            //Analyze ontology to extract knowledge of the time instant
            FillValueOfInstant(ontology, timeInstant, dtPropAsns, objPropAsns);
            FillDescriptionOfInstant(ontology, timeInstant, dtPropAsns, objPropAsns);
            FillPositionOfInstant(ontology, timeInstant, dtPropAsns, objPropAsns);

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
                    TIMECoordinate nominalPositionCoordinate = GetBeginningOfTIMEInterval(ontology, timeInstant.Position.NominalValue, calendarTRS);
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

        public static TIMEExtent GetExtentOfTIMEInterval(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get temporal extent of interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Temporary working variables
            List<OWLDataPropertyAssertion> dtPropAsns = OWLAssertionAxiomHelper.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(ontology);
            List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

            //Declare the time interval
            TIMEInterval timeInterval = new TIMEInterval(timeIntervalURI);

            //Analyze ontology to extract knowledge of the time interval
            FillValueOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
            FillDescriptionOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
            FillDurationOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
            FillBeginningOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);
            FillEndOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);

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
                TIMECoordinate timeIntervalBeginning = GetCoordinateOfTIMEInstant(ontology, timeInterval.Beginning, calendarTRS);
                TIMECoordinate timeIntervalEnd = GetCoordinateOfTIMEInstant(ontology, timeInterval.End, calendarTRS);
                if (timeIntervalBeginning != null && timeIntervalEnd != null)
                    return TIMEConverter.CalculateExtent(timeIntervalBeginning, timeIntervalEnd, calendarTRS);
            }

            //In absence of a valid time interval encoding we simply cannot proceed
            return null;
        }

        public static TIMECoordinate GetBeginningOfTIMEInterval(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get beginning of interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Temporary working variables
            List<OWLDataPropertyAssertion> dtPropAsns = OWLAssertionAxiomHelper.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(ontology);
            List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);

            return GetBeginningOfIntervalInternal(ontology, timeIntervalURI, calendarTRS, dtPropAsns, objPropAsns, new Dictionary<long, RDFResource>());
        }
        internal static TIMECoordinate GetBeginningOfIntervalInternal(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS,
            List<OWLDataPropertyAssertion> dtPropAsns, List<OWLObjectPropertyAssertion> objPropAsns, Dictionary<long,RDFResource> visitContext)
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
            FillBeginningOfInterval(ontology, timeInterval, dtPropAsns, objPropAsns);

            //Get the coordinate of the time interval's beginning instant
            if (timeInterval.Beginning != null)
                return GetCoordinateOfTIMEInstant(ontology, timeInterval.Beginning, calendarTRS);

            #region Allen
            //There's no direct representation of the time interval's beginning instant:
            //we can indirectly obtain it by exploiting specific Allen interval relations
            OWLObjectProperty timeIntervalEqualsOP = new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS);
            OWLObjectProperty timeIntervalStartsOP = new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS);
            OWLObjectProperty timeIntervalStartedByOP = new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY);
            List<RDFResource> compatibleStartingIntervals = RDFQueryUtilities.RemoveDuplicates(
                OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, timeIntervalEqualsOP)
                 .Union(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, timeIntervalStartsOP))
                 .Union(OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, timeIntervalStartedByOP))
                 .Select(objPropAsn => objPropAsn.TargetIndividualExpression.GetIRI()).ToList());

            //Perform a sequential search of the beginning instant on the set of compatible intervals
            TIMECoordinate compatibleBeginning = null;
            IEnumerator<RDFResource> compatibleStartingIntervalsEnumerator = compatibleStartingIntervals.GetEnumerator();
            while (compatibleBeginning == null && compatibleStartingIntervalsEnumerator.MoveNext())
                compatibleBeginning = GetBeginningOfIntervalInternal(ontology, compatibleStartingIntervalsEnumerator.Current, calendarTRS, dtPropAsns, objPropAsns, visitContext);
            if (compatibleBeginning == null)
            {
                #region MetBy
                //There's still no direct representation of the time interval's beginning instant:
                //we can indirectly obtain it by exploiting Allen interval-meeting relations
                OWLObjectProperty timeIntervalMetByOP = new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY);
                List<RDFResource> metByIntervals = RDFQueryUtilities.RemoveDuplicates(
                    OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, timeIntervalMetByOP)
                     .Select(objPropAsn => objPropAsn.TargetIndividualExpression.GetIRI()).ToList());

                //Perform a sequential search of the start instant on the set of metBy intervals
                IEnumerator<RDFResource> metByIntervalsEnumerator = metByIntervals.GetEnumerator();
                while (compatibleBeginning == null && metByIntervalsEnumerator.MoveNext())
                    compatibleBeginning = GetEndOfIntervalInternal(ontology, metByIntervalsEnumerator.Current, calendarTRS, visitContext);
                #endregion
            }
            #endregion

            return compatibleBeginning;
        }
        public static TIMECoordinate GetEndOfTIMEInterval(this OWLOntology ontology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
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
                return GetCoordinateOfTIMEInstant(ontology, timeInterval.End, calendarTRS);

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
        #endregion
    }
}
