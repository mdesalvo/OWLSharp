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
    /// <summary>
    /// TIMEOntologyHelper contains methods for declaring and analyzing extent of temporal entities
    /// </summary>
    public static class TIMEOntologyHelper
    {
        #region Declarer
        /// <summary>
        /// Declares the existence of the given temporal instant and makes it the temporal extent of the given A-BOX feature individual
        /// </summary>
        public static OWLOntology DeclareTimeInstant(this OWLOntology timeOntology, RDFResource featureUri, TIMEInstant timeInstant)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare temporal instant to the ontology because given \"featureUri\" parameter is null");
            if (timeInstant == null)
                throw new OWLException("Cannot declare temporal instant to the ontology because given \"timeInstant\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            timeOntology.Data.DeclareIndividual(featureUri);
            timeOntology.Data.DeclareObjectAssertion(featureUri, RDFVocabulary.TIME.HAS_TIME, timeInstant);
            timeOntology.DeclareTimeInstantInternal(timeInstant);

            return timeOntology;
        }
        internal static OWLOntology DeclareTimeInstantInternal(this OWLOntology timeOntology, TIMEInstant timeInstant)
        {
            //Add knowledge to the A-BOX
            timeOntology.Data.DeclareIndividual(timeInstant);
            timeOntology.Data.DeclareIndividualType(timeInstant, RDFVocabulary.TIME.INSTANT);

            //Add knowledge to the A-BOX (time:inXSDDateTimeStamp)
            if (timeInstant.DateTime.HasValue)
                timeOntology.Data.DeclareDatatypeAssertion(timeInstant, RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new RDFTypedLiteral(XmlConvert.ToString(timeInstant.DateTime.Value.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ssZ"), RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP));

            //Add knowledge to the A-BOX (time:inDateTime)
            if (timeInstant.Description != null)
            {
                timeOntology.Data.DeclareObjectAssertion(timeInstant, RDFVocabulary.TIME.IN_DATETIME, timeInstant.Description);
                timeOntology.Data.DeclareIndividual(timeInstant.Description);
                timeOntology.Data.DeclareIndividualType(timeInstant.Description, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
                timeOntology.Data.DeclareObjectAssertion(timeInstant.Description, RDFVocabulary.TIME.UNIT_TYPE, timeInstant.Description.Coordinate.Metadata.UnitType);
                timeOntology.Data.DeclareObjectAssertion(timeInstant.Description, RDFVocabulary.TIME.HAS_TRS, timeInstant.Description.Coordinate.Metadata.TRS);
                if (timeInstant.Description.Coordinate.Year.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Description, RDFVocabulary.TIME.YEAR, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Year, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInstant.Description.Coordinate.Month.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Description, RDFVocabulary.TIME.MONTH, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Month, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInstant.Description.Coordinate.Day.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Description, RDFVocabulary.TIME.DAY, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Day, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInstant.Description.Coordinate.Hour.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Description, RDFVocabulary.TIME.HOUR, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Hour, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInstant.Description.Coordinate.Minute.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Description, RDFVocabulary.TIME.MINUTE, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Minute, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInstant.Description.Coordinate.Second.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Description, RDFVocabulary.TIME.SECOND, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Description.Coordinate.Second, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInstant.Description.Coordinate.Metadata.MonthOfYear != null)
                    timeOntology.Data.DeclareObjectAssertion(timeInstant.Description, RDFVocabulary.TIME.MONTH_OF_YEAR, timeInstant.Description.Coordinate.Metadata.MonthOfYear);
                if (timeInstant.Description.Coordinate.Metadata.DayOfWeek != null)
                    timeOntology.Data.DeclareObjectAssertion(timeInstant.Description, RDFVocabulary.TIME.DAY_OF_WEEK, timeInstant.Description.Coordinate.Metadata.DayOfWeek);
                if (timeInstant.Description.Coordinate.Metadata.DayOfYear.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Description, RDFVocabulary.TIME.DAY_OF_YEAR, new RDFTypedLiteral($"{timeInstant.Description.Coordinate.Metadata.DayOfYear}", RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER));
            }

            //Add knowledge to the A-BOX (time:inTimePosition)
            if (timeInstant.Position != null)
            {
                timeOntology.Data.DeclareObjectAssertion(timeInstant, RDFVocabulary.TIME.IN_TIME_POSITION, timeInstant.Position);
                timeOntology.Data.DeclareIndividual(timeInstant.Position);
                timeOntology.Data.DeclareIndividualType(timeInstant.Position, RDFVocabulary.TIME.TIME_POSITION);
                timeOntology.Data.DeclareObjectAssertion(timeInstant.Position, RDFVocabulary.TIME.HAS_TRS, timeInstant.Position.TRS);
                if (timeInstant.Position.IsNominal)
                    timeOntology.Data.DeclareObjectAssertion(timeInstant.Position, RDFVocabulary.TIME.NOMINAL_POSITION, timeInstant.Position.NominalValue);
                else
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Position, RDFVocabulary.TIME.NUMERIC_POSITION, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Position.NumericValue, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DOUBLE));
                if (timeInstant.Position.PositionalUncertainty != null)
                {
                    timeOntology.Data.DeclareObjectAssertion(timeInstant.Position, RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY, timeInstant.Position.PositionalUncertainty);
                    timeOntology.Data.DeclareIndividual(timeInstant.Position.PositionalUncertainty);
                    timeOntology.Data.DeclareIndividualType(timeInstant.Position.PositionalUncertainty, RDFVocabulary.TIME.DURATION);
                    timeOntology.Data.DeclareObjectAssertion(timeInstant.Position.PositionalUncertainty, RDFVocabulary.TIME.UNIT_TYPE, timeInstant.Position.PositionalUncertainty.UnitType);
                    timeOntology.Data.DeclareDatatypeAssertion(timeInstant.Position.PositionalUncertainty, RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral($"{Convert.ToString(timeInstant.Position.PositionalUncertainty.Value, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DOUBLE));
                }
            }

            return timeOntology;
        }

        /// <summary>
        /// Declares the existence of the given temporal relation between the given instants
        /// </summary>
        public static OWLOntology DeclareTimeInstantRelation(this OWLOntology timeOntology, TIMEInstant aTimeInstant, TIMEInstant bTimeInstant, TIMEEnums.TIMEInstantRelation timeInstantRelation)
        {
            #region Guards
            if (aTimeInstant == null)
                throw new OWLException("Cannot declare instant relation to the ontology because given \"aTimeInstant\" parameter is null");
            if (bTimeInstant == null)
                throw new OWLException("Cannot declare instant relation to the ontology because given \"bTimeInstant\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            timeOntology.DeclareTimeInstantInternal(aTimeInstant);
            timeOntology.DeclareTimeInstantInternal(bTimeInstant);
            switch (timeInstantRelation)
            {
                case TIMEEnums.TIMEInstantRelation.After:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInstant, RDFVocabulary.TIME.AFTER, bTimeInstant);
                    break;
                case TIMEEnums.TIMEInstantRelation.Before:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInstant, RDFVocabulary.TIME.BEFORE, bTimeInstant);
                    break;
            }

            return timeOntology;
        }

        /// <summary>
        /// Declares the existence of the given temporal interval and makes it the temporal extent of the given A-BOX feature individual
        /// </summary>
        public static OWLOntology DeclareTimeInterval(this OWLOntology timeOntology, RDFResource featureUri, TIMEInterval timeInterval)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot declare temporal interval to the ontology because given \"featureUri\" parameter is null");
            if (timeInterval == null)
                throw new OWLException("Cannot declare temporal interval to the ontology because given \"timeInterval\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            timeOntology.Data.DeclareIndividual(featureUri);
            timeOntology.Data.DeclareObjectAssertion(featureUri, RDFVocabulary.TIME.HAS_TIME, timeInterval);
            timeOntology.DeclareTimeIntervalInternal(timeInterval);

            return timeOntology;
        }
        internal static OWLOntology DeclareTimeIntervalInternal(this OWLOntology timeOntology, TIMEInterval timeInterval)
        {
            //Add knowledge to the A-BOX
            timeOntology.Data.DeclareIndividual(timeInterval);
            timeOntology.Data.DeclareIndividualType(timeInterval, RDFVocabulary.TIME.PROPER_INTERVAL);

            //Add knowledge to the A-BOX (time:hasXSDDuration)
            if (timeInterval.TimeSpan.HasValue)
                timeOntology.Data.DeclareDatatypeAssertion(timeInterval, RDFVocabulary.TIME.HAS_XSD_DURATION, new RDFTypedLiteral($"{XmlConvert.ToString(timeInterval.TimeSpan.Value)}", RDFModelEnums.RDFDatatypes.XSD_DURATION));

            //Add knowledge to the A-BOX (time:hasBeginning)
            if (timeInterval.Beginning != null)
            {
                timeOntology.Data.DeclareObjectAssertion(timeInterval, RDFVocabulary.TIME.HAS_BEGINNING, timeInterval.Beginning);
                timeOntology.DeclareTimeInstantInternal(timeInterval.Beginning);
            }

            //Add knowledge to the A-BOX (time:hasEnd)
            if (timeInterval.End != null)
            {
                timeOntology.Data.DeclareObjectAssertion(timeInterval, RDFVocabulary.TIME.HAS_END, timeInterval.End);
                timeOntology.DeclareTimeInstantInternal(timeInterval.End);
            }

            //Add knowledge to the A-BOX (time:hasDurationDescription)
            if (timeInterval.Description != null)
            {
                timeOntology.Data.DeclareObjectAssertion(timeInterval, RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, timeInterval.Description);
                timeOntology.Data.DeclareIndividual(timeInterval.Description);
                timeOntology.Data.DeclareIndividualType(timeInterval.Description, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
                timeOntology.Data.DeclareObjectAssertion(timeInterval.Description, RDFVocabulary.TIME.HAS_TRS, timeInterval.Description.Extent.Metadata.TRS);
                if (timeInterval.Description.Extent.Years.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Description, RDFVocabulary.TIME.YEARS, new RDFTypedLiteral(Convert.ToString(timeInterval.Description.Extent.Years, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInterval.Description.Extent.Months.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Description, RDFVocabulary.TIME.MONTHS, new RDFTypedLiteral(Convert.ToString(timeInterval.Description.Extent.Months, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInterval.Description.Extent.Weeks.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Description, RDFVocabulary.TIME.WEEKS, new RDFTypedLiteral(Convert.ToString(timeInterval.Description.Extent.Weeks, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInterval.Description.Extent.Days.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Description, RDFVocabulary.TIME.DAYS, new RDFTypedLiteral(Convert.ToString(timeInterval.Description.Extent.Days, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInterval.Description.Extent.Hours.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Description, RDFVocabulary.TIME.HOURS, new RDFTypedLiteral(Convert.ToString(timeInterval.Description.Extent.Hours, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInterval.Description.Extent.Minutes.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Description, RDFVocabulary.TIME.MINUTES, new RDFTypedLiteral(Convert.ToString(timeInterval.Description.Extent.Minutes, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
                if (timeInterval.Description.Extent.Seconds.HasValue)
                    timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Description, RDFVocabulary.TIME.SECONDS, new RDFTypedLiteral(Convert.ToString(timeInterval.Description.Extent.Seconds, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
            }

            //Add knowledge to the A-BOX (time:hasDuration)
            if (timeInterval.Duration != null)
            {
                timeOntology.Data.DeclareObjectAssertion(timeInterval, RDFVocabulary.TIME.HAS_DURATION, timeInterval.Duration);
                timeOntology.Data.DeclareIndividual(timeInterval.Duration);
                timeOntology.Data.DeclareIndividualType(timeInterval.Duration, RDFVocabulary.TIME.DURATION);
                timeOntology.Data.DeclareObjectAssertion(timeInterval.Duration, RDFVocabulary.TIME.UNIT_TYPE, timeInterval.Duration.UnitType);
                timeOntology.Data.DeclareDatatypeAssertion(timeInterval.Duration, RDFVocabulary.TIME.NUMERIC_DURATION, new RDFTypedLiteral($"{Convert.ToString(timeInterval.Duration.Value, CultureInfo.InvariantCulture)}", RDFModelEnums.RDFDatatypes.XSD_DOUBLE));
            }

            return timeOntology;
        }

        /// <summary>
        /// Declares the existence of the given temporal relation between the given intervals
        /// </summary>
        public static OWLOntology DeclareTimeIntervalRelation(this OWLOntology timeOntology, TIMEInterval aTimeInterval, TIMEInterval bTimeInterval, TIMEEnums.TIMEIntervalRelation timeIntervalRelation)
        {
            #region Guards
            if (aTimeInterval == null)
                throw new OWLException("Cannot declare interval relation to the ontology because given \"aTimeInterval\" parameter is null");
            if (bTimeInterval == null)
                throw new OWLException("Cannot declare interval relation to the ontology because given \"bTimeInterval\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX
            timeOntology.DeclareTimeIntervalInternal(aTimeInterval);
            timeOntology.DeclareTimeIntervalInternal(bTimeInterval);
            switch (timeIntervalRelation)
            {
                case TIMEEnums.TIMEIntervalRelation.After:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_AFTER, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Before:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_BEFORE, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Contains:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_CONTAINS, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Disjoint:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_DISJOINT, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.During:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_DURING, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Equals:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_EQUALS, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.FinishedBy:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_FINISHED_BY, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Finishes:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_FINISHES, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.HasInside:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.HAS_INSIDE, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.In:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_IN, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Meets:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_MEETS, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.MetBy:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_MET_BY, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.OverlappedBy:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Overlaps:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_OVERLAPS, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.StartedBy:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_STARTED_BY, bTimeInterval);
                    break;
                case TIMEEnums.TIMEIntervalRelation.Starts:
                    timeOntology.Data.DeclareObjectAssertion(aTimeInterval, RDFVocabulary.TIME.INTERVAL_STARTS, bTimeInterval);
                    break;
            }

            return timeOntology;
        }
        #endregion

        #region Analyzer
        /// <summary>
        /// Gets the temporal extent of the given A-BOX feature individual by analyzing its "time:hasTime" relations
        /// </summary>
        public static List<TIMEEntity> GetTemporalExtentOfFeature(this OWLOntology timeOntology, RDFResource featureUri)
        {
            #region Guards
            if (featureUri == null)
                throw new OWLException("Cannot get temporal extent of feature because given \"featureUri\" parameter is null");
            #endregion

            //We need first to determine assertions having "time:hasTime" compatible predicates for the given feature subject
            List<RDFResource> hasTimeProperties = timeOntology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_TIME)
                                                    .Union(timeOntology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_TIME)).ToList();
            RDFGraph hasTimeAssertions = timeOntology.Data.ABoxGraph[featureUri, RDFVocabulary.TIME.HAS_TIME, null, null];
            foreach (RDFResource hasTimeProperty in hasTimeProperties)
                hasTimeAssertions = hasTimeAssertions.UnionWith(timeOntology.Data.ABoxGraph[featureUri, hasTimeProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the temporal extent in its available aspects
            List<TIMEEntity> temporalExtentOfFeature = new List<TIMEEntity>();
            foreach (RDFTriple hasTimeAssertion in hasTimeAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //Detect if the temporal extent of the feature describes a time instant
                if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)hasTimeAssertion.Object, RDFVocabulary.TIME.INSTANT))
                {
                    //Declare the discovered time instant
                    TIMEInstant timeInstant = new TIMEInstant((RDFResource)hasTimeAssertion.Object);

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfTimeInstant(timeOntology, timeInstant);
                    FillDescriptionOfTimeInstant(timeOntology, timeInstant);
                    FillPositionOfTimeInstant(timeOntology, timeInstant);

                    //Collect the time instant into temporal extent of feature
                    temporalExtentOfFeature.Add(timeInstant);
                }

                //Detect if the temporal extent of the feature describes a time interval
                else if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)hasTimeAssertion.Object, RDFVocabulary.TIME.INTERVAL))
                {
                    //Declare the discovered time interval
                    TIMEInterval timeInterval = new TIMEInterval((RDFResource)hasTimeAssertion.Object);

                    //Analyze ontology to extract knowledge of the time interval
                    FillValueOfTimeInterval(timeOntology, timeInterval);
                    FillDescriptionOfTimeInterval(timeOntology, timeInterval);
                    FillDurationOfTimeInterval(timeOntology, timeInterval);
                    FillBeginningOfTimeInterval(timeOntology, timeInterval);
                    FillEndOfTimeInterval(timeOntology, timeInterval);

                    //Collect the time interval into temporal extent of feature
                    temporalExtentOfFeature.Add(timeInterval);
                }
            }

            return temporalExtentOfFeature;
        }
        internal static void FillValueOfTimeInstant(OWLOntology timeOntology, TIMEInstant timeInstant)
        {
            //time:inXSDDateTimeStamp
            RDFPatternMember inXSDDateTimeStampLiteral = timeOntology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, null, null]
                                                           ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
            if (inXSDDateTimeStampLiteral is RDFTypedLiteral inXSDDateTimeStampTLiteral
                 && inXSDDateTimeStampTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDDateTimeStampTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDDateTime
            RDFPatternMember inXSDDateTimeLiteral = timeOntology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_XSD_DATETIME, null, null]
                                                      ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
            if (inXSDDateTimeLiteral is RDFTypedLiteral inXSDDateTimeTLiteral
                 && inXSDDateTimeTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATETIME))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDDateTimeTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDDate
            RDFPatternMember inXSDDateLiteral = timeOntology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_XSD_DATE, null, null]
                                                  ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
            if (inXSDDateLiteral is RDFTypedLiteral inXSDDateTLiteral
                 && inXSDDateTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATE))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDDateTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDgYearMonth
            RDFPatternMember inXSDGYearMonthLiteral = timeOntology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_XSD_GYEARMONTH, null, null]
                                                        ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
            if (inXSDGYearMonthLiteral is RDFTypedLiteral inXSDGYearMonthTLiteral
                 && inXSDGYearMonthTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH))
            {
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDGYearMonthTLiteral.Value, XmlDateTimeSerializationMode.Utc);
                return;
            }

            //time:inXSDgYear
            RDFPatternMember inXSDGYearLiteral = timeOntology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_XSD_GYEAR, null, null]
                                                   ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
            if (inXSDGYearLiteral is RDFTypedLiteral inXSDGYearTLiteral
                 && inXSDGYearTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GYEAR))
                timeInstant.DateTime = XmlConvert.ToDateTime(inXSDGYearTLiteral.Value, XmlDateTimeSerializationMode.Utc);
        }
        internal static void FillDescriptionOfTimeInstant(OWLOntology timeOntology, TIMEInstant timeInstant)
        {
            #region Utilities
            RDFResource GetTRSOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFPatternMember trs = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.HAS_TRS, null, null]
                                            ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (trs is RDFResource trsResource)
                    return trsResource;

                //Default to Gregorian if this required information is not provided
                return TIMECalendarReferenceSystem.Gregorian;
            }
            RDFResource GetUnitTypeOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFPatternMember unitType = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.UNIT_TYPE, null, null]
                                              ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (unitType is RDFResource unitTypeResource)
                    return unitTypeResource;

                //Default to Second if this required information is not provided
                return RDFVocabulary.TIME.UNIT_SECOND;
            }
            double? GetYearOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFPatternMember yearPM = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.YEAR, null, null]
                                            ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (yearPM is RDFTypedLiteral yearTL)
                {
                    //xsd:gYear
                    if (yearTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_GYEAR))
                    {
                        DateTime yearDT = XmlConvert.ToDateTime(yearTL.Value, XmlDateTimeSerializationMode.Utc);
                        return Convert.ToDouble(yearDT.Year);
                    }

                    //time:generalYear or numeric literals
                    if (yearTL.Datatype.Equals(RDFModelEnums.RDFDatatypes.TIME_GENERALYEAR)
                         || yearTL.HasDecimalDatatype())
                        return Convert.ToDouble(yearTL.Value, CultureInfo.InvariantCulture);
                }
                return null;
            }
            double? GetMonthOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFPatternMember monthPM = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.MONTH, null, null]
                                             ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (monthPM is RDFTypedLiteral monthTL)
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
                RDFPatternMember dayPM = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.DAY, null, null]
                                           ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (dayPM is RDFTypedLiteral dayTL)
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
                RDFPatternMember hourPM = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.HOUR, null, null]
                                            ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (hourPM is RDFTypedLiteral hourTL && hourTL.HasDecimalDatatype())
                    return Convert.ToDouble(hourTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMinuteOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFPatternMember minutePM = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.MINUTE, null, null]
                                              ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (minutePM is RDFTypedLiteral minuteTL && minuteTL.HasDecimalDatatype())
                    return Convert.ToDouble(minuteTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetSecondOfInstantDescription(RDFResource dateTimeDescriptionURI)
            {
                RDFPatternMember secondPM = timeOntology.Data.ABoxGraph[dateTimeDescriptionURI, RDFVocabulary.TIME.SECOND, null, null]
                                              ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (secondPM is RDFTypedLiteral secondTL && secondTL.HasDecimalDatatype())
                    return Convert.ToDouble(secondTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            void FillTextualDecorators(TIMEInstantDescription timeInstantDescription)
            {
                //time:MonthOfYear
                RDFPatternMember monthOfYearPM = timeOntology.Data.ABoxGraph[timeInstantDescription, RDFVocabulary.TIME.MONTH_OF_YEAR, null, null]
                                                   ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                timeInstantDescription.Coordinate.Metadata.MonthOfYear = monthOfYearPM is RDFResource monthOfYearResource ? monthOfYearResource : null;

                //time:DayOfYear
                RDFPatternMember dayOfYearPM = timeOntology.Data.ABoxGraph[timeInstantDescription, RDFVocabulary.TIME.DAY_OF_YEAR, null, null]
                                   ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (dayOfYearPM is RDFTypedLiteral dayOfYearTL && dayOfYearTL.HasDecimalDatatype())
                    timeInstantDescription.Coordinate.Metadata.DayOfYear = Convert.ToUInt32(dayOfYearTL.Value, CultureInfo.InvariantCulture);

                //time:DayOfWeek
                RDFPatternMember dayOfWeek = timeOntology.Data.ABoxGraph[timeInstantDescription, RDFVocabulary.TIME.DAY_OF_WEEK, null, null]
                                               ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                timeInstantDescription.Coordinate.Metadata.DayOfWeek = dayOfWeek is RDFResource dayOfWeekResource ? dayOfWeekResource : null;
            }
            #endregion

            //We need first to determine assertions having "time:inDateTime" compatible predicates for the given time instant
            List<RDFResource> inDateTimeProperties = timeOntology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.IN_DATETIME)
                                                       .Union(timeOntology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.IN_DATETIME)).ToList();
            RDFGraph inDateTimeAssertions = timeOntology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_DATETIME, null, null];
            foreach (RDFResource inDateTimeProperty in inDateTimeProperties)
                inDateTimeAssertions = inDateTimeAssertions.UnionWith(timeOntology.Data.ABoxGraph[timeInstant, inDateTimeProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the description of the time instant in its available aspects
            List<TIMEInstantDescription> descriptionsOfTimeInstant = new List<TIMEInstantDescription>();
            foreach (RDFTriple inDateTimeAssertion in inDateTimeAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:GeneralDateTimeDescription
                if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)inDateTimeAssertion.Object, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION))
                {
                    //Declare the discovered time instant description
                    RDFResource trs = GetTRSOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    RDFResource unitType = GetUnitTypeOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    double? year = GetYearOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    double? month = GetMonthOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    double? day = GetDayOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    double? hour = GetHourOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    double? minute = GetMinuteOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    double? second = GetSecondOfInstantDescription((RDFResource)inDateTimeAssertion.Object);
                    TIMECoordinate timeInstantCoordinate = new TIMECoordinate(
                        year, month, day, hour, minute, second, new TIMECoordinateMetadata(trs, unitType));
                    TIMEInstantDescription timeInstantDescription = new TIMEInstantDescription(
                        (RDFResource)inDateTimeAssertion.Object, timeInstantCoordinate);
                    FillTextualDecorators(timeInstantDescription);

                    descriptionsOfTimeInstant.Add(timeInstantDescription);
                }
            }
            timeInstant.Description = descriptionsOfTimeInstant.FirstOrDefault(); //We currently support one description, but this may evolve in future
        }
        internal static void FillPositionOfTimeInstant(OWLOntology timeOntology, TIMEInstant timeInstant)
        {
            #region Utilities
            RDFResource GetTRSOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember trs = timeOntology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.HAS_TRS, null, null]
                                            ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (trs is RDFResource trsResource)
                    return trsResource;
                return null;
            }
            double? GetNumericValueOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember numericPositionPM = timeOntology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.NUMERIC_POSITION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (numericPositionPM is RDFTypedLiteral numericPositionTL && numericPositionTL.HasDecimalDatatype())
                    return Convert.ToDouble(numericPositionTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            RDFResource GetNominalValueOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember nominalPositionPM = timeOntology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.NOMINAL_POSITION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (nominalPositionPM is RDFResource nominalPositionResource)
                    return nominalPositionResource;
                return null;
            }
            RDFResource GetUncertaintyOfPosition(RDFResource temporalExtentURI)
            {
                RDFPatternMember uncertainty = timeOntology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY, null, null]
                                                ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (uncertainty is RDFResource uncertaintyResource)
                    return uncertaintyResource;
                return null;
            }
            #endregion

            //We need first to determine assertions having "time:inTimePosition" compatible predicates for the given time instant
            List<RDFResource> inTimePositionProperties = timeOntology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.IN_TIME_POSITION)
                                                           .Union(timeOntology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.IN_TIME_POSITION)).ToList();
            RDFGraph inTimePositionAssertions = timeOntology.Data.ABoxGraph[timeInstant, RDFVocabulary.TIME.IN_TIME_POSITION, null, null];
            foreach (RDFResource inTimePositionProperty in inTimePositionProperties)
                inTimePositionAssertions = inTimePositionAssertions.UnionWith(timeOntology.Data.ABoxGraph[timeInstant, inTimePositionProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the position of the time instant in its available aspects
            List<TIMEInstantPosition> positionsOfTimeInstant = new List<TIMEInstantPosition>();
            foreach (RDFTriple inTimePositionAssertion in inTimePositionAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:TemporalPosition
                if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)inTimePositionAssertion.Object, RDFVocabulary.TIME.TEMPORAL_POSITION))
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
                                FillDurationOfTimeInterval(timeOntology, positionalUncertaintyInterval);
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
        internal static void FillValueOfTimeInterval(OWLOntology timeOntology, TIMEInterval timeInterval)
        {
            //time:hasXSDDuration
            RDFPatternMember hasXSDDurationLiteral = timeOntology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_XSD_DURATION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
            if (hasXSDDurationLiteral is RDFTypedLiteral hasXSDDurationTLiteral
                 && hasXSDDurationTLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DURATION))
                timeInterval.TimeSpan = XmlConvert.ToTimeSpan(hasXSDDurationTLiteral.Value);
        }
        internal static void FillDescriptionOfTimeInterval(OWLOntology timeOntology, TIMEInterval timeInterval)
        {
            #region Utilities
            RDFResource GetTRSOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember trs = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.HAS_TRS, null, null]
                                         ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (trs is RDFResource trsResource)
                    return trsResource;

                //Default to Gregorian if this required information is not provided
                return TIMECalendarReferenceSystem.Gregorian;
            }
            double? GetYearsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember yearsPM = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.YEARS, null, null]
                                             ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (yearsPM is RDFTypedLiteral yearsTL && yearsTL.HasDecimalDatatype())
                    return Convert.ToDouble(yearsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMonthsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember monthsPM = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.MONTHS, null, null]
                                              ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (monthsPM is RDFTypedLiteral monthsTL && monthsTL.HasDecimalDatatype())
                    return Convert.ToDouble(monthsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetWeeksOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember weeksPM = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.WEEKS, null, null]
                                             ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (weeksPM is RDFTypedLiteral weeksTL && weeksTL.HasDecimalDatatype())
                    return Convert.ToDouble(weeksTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetDaysOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember daysPM = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.DAYS, null, null]
                                            ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (daysPM is RDFTypedLiteral daysTL && daysTL.HasDecimalDatatype())
                    return Convert.ToDouble(daysTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetHoursOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember hoursPM = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.HOURS, null, null]
                                             ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (hoursPM is RDFTypedLiteral hoursTL && hoursTL.HasDecimalDatatype())
                    return Convert.ToDouble(hoursTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetMinutesOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember minutesPM = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.MINUTES, null, null]
                                               ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (minutesPM is RDFTypedLiteral minutesTL && minutesTL.HasDecimalDatatype())
                    return Convert.ToDouble(minutesTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            double? GetSecondsOfIntervalDescription(RDFResource durationDescriptionURI)
            {
                RDFPatternMember secondsPM = timeOntology.Data.ABoxGraph[durationDescriptionURI, RDFVocabulary.TIME.SECONDS, null, null]
                                               ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (secondsPM is RDFTypedLiteral secondsTL && secondsTL.HasDecimalDatatype())
                    return Convert.ToDouble(secondsTL.Value, CultureInfo.InvariantCulture);
                return null;
            }
            #endregion

            //We need first to determine assertions having "time:hasDurationDescription" compatible predicates for the given time interval
            List<RDFResource> hasDurationDescriptionProperties = timeOntology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION)
                                                                   .Union(timeOntology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION)).ToList();
            RDFGraph hasDurationDescriptionAssertions = timeOntology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, null, null];
            foreach (RDFResource hasDurationDescriptionProperty in hasDurationDescriptionProperties)
                hasDurationDescriptionAssertions = hasDurationDescriptionAssertions.UnionWith(timeOntology.Data.ABoxGraph[timeInterval, hasDurationDescriptionProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the description of the time interval in its available aspects
            List<TIMEIntervalDescription> descriptionsOfTimeInterval = new List<TIMEIntervalDescription>();
            foreach (RDFTriple hasDurationDescriptionAssertion in hasDurationDescriptionAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:GeneralDurationDescription
                if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)hasDurationDescriptionAssertion.Object, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION))
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
        internal static void FillDurationOfTimeInterval(OWLOntology timeOntology, TIMEInterval timeInterval)
        {
            #region Utilities
            RDFResource GetUnitTypeOfDuration(RDFResource temporalExtentURI)
            {
                RDFPatternMember unitType = timeOntology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.UNIT_TYPE, null, null]
                                              ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)?.Object;
                if (unitType is RDFResource unitTypeResource)
                    return unitTypeResource;

                //Default to Second if this required information is not provided
                return RDFVocabulary.TIME.UNIT_SECOND;
            }
            double GetValueOfDuration(RDFResource temporalExtentURI)
            {
                RDFPatternMember numericDurationPM = timeOntology.Data.ABoxGraph[temporalExtentURI, RDFVocabulary.TIME.NUMERIC_DURATION, null, null]
                                                       ?.FirstOrDefault(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL)?.Object;
                if (numericDurationPM is RDFTypedLiteral numericDurationTL && numericDurationTL.HasDecimalDatatype())
                    return Convert.ToDouble(numericDurationTL.Value, CultureInfo.InvariantCulture);
                return 0;
            }
            #endregion

            //We need first to determine assertions having "time:hasDuration" compatible predicates for the given time interval
            List<RDFResource> hasDurationProperties = timeOntology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_DURATION)
                                                        .Union(timeOntology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_DURATION)
                                                          //We need to also support "thors:positionalUncertainty" (even if not related with "time:hasDuration")
                                                          .Union(new List<RDFResource>() { RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY })).ToList();
            RDFGraph hasDurationAssertions = timeOntology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_DURATION, null, null];
            foreach (RDFResource hasDurationProperty in hasDurationProperties)
                hasDurationAssertions = hasDurationAssertions.UnionWith(timeOntology.Data.ABoxGraph[timeInterval, hasDurationProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the duration of the time interval in its available aspects
            List<TIMEIntervalDuration> durationsOfTimeInterval = new List<TIMEIntervalDuration>();
            foreach (RDFTriple hasDurationAssertion in hasDurationAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:Duration
                if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)hasDurationAssertion.Object, RDFVocabulary.TIME.DURATION))
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
        internal static void FillBeginningOfTimeInterval(OWLOntology timeOntology, TIMEInterval timeInterval)
        {
            //We need first to determine assertions having "time:hasBeginning" compatible predicates for the given time interval
            List<RDFResource> hasBeginningProperties = timeOntology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_BEGINNING)
                                                         .Union(timeOntology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_BEGINNING)).ToList();
            RDFGraph hasBeginningAssertions = timeOntology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_BEGINNING, null, null];
            foreach (RDFResource hasBeginningProperty in hasBeginningProperties)
                hasBeginningAssertions = hasBeginningAssertions.UnionWith(timeOntology.Data.ABoxGraph[timeInterval, hasBeginningProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the time instant in its available aspects
            List<TIMEInstant> beginningsOfTimeInterval = new List<TIMEInstant>();
            foreach (RDFTriple hasBeginningAssertion in hasBeginningAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:Instant
                if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)hasBeginningAssertion.Object, RDFVocabulary.TIME.INSTANT))
                {
                    //Declare the discovered time instant
                    TIMEInstant beginningTimeInstant = new TIMEInstant((RDFResource)hasBeginningAssertion.Object);

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfTimeInstant(timeOntology, beginningTimeInstant);
                    FillDescriptionOfTimeInstant(timeOntology, beginningTimeInstant);
                    FillPositionOfTimeInstant(timeOntology, beginningTimeInstant);

                    //Collect the time instant into temporal extent of the time interval
                    beginningsOfTimeInterval.Add(beginningTimeInstant);
                }
            }
            timeInterval.Beginning = beginningsOfTimeInterval.FirstOrDefault(); //We currently support one beginning instant, but this may evolve in future
        }
        internal static void FillEndOfTimeInterval(OWLOntology timeOntology, TIMEInterval timeInterval)
        {
            //We need first to determine assertions having "time:hasEnd" compatible predicates for the given time interval
            List<RDFResource> hasEndProperties = timeOntology.Model.PropertyModel.GetSubPropertiesOf(RDFVocabulary.TIME.HAS_END)
                                                   .Union(timeOntology.Model.PropertyModel.GetEquivalentPropertiesOf(RDFVocabulary.TIME.HAS_END)).ToList();
            RDFGraph hasEndAssertions = timeOntology.Data.ABoxGraph[timeInterval, RDFVocabulary.TIME.HAS_END, null, null];
            foreach (RDFResource hasEndProperty in hasEndProperties)
                hasEndAssertions = hasEndAssertions.UnionWith(timeOntology.Data.ABoxGraph[timeInterval, hasEndProperty, null, null]);

            //Then we need to iterate these assertions in order to reconstruct the time instant in its available aspects
            List<TIMEInstant> endsOfTimeInterval = new List<TIMEInstant>();
            foreach (RDFTriple hasEndAssertion in hasEndAssertions.Where(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
            {
                //time:Instant
                if (timeOntology.Data.CheckIsIndividualOf(timeOntology.Model, (RDFResource)hasEndAssertion.Object, RDFVocabulary.TIME.INSTANT))
                {
                    //Declare the discovered time instant
                    TIMEInstant endTimeInstant = new TIMEInstant((RDFResource)hasEndAssertion.Object);

                    //Analyze ontology to extract knowledge of the time instant
                    FillValueOfTimeInstant(timeOntology, endTimeInstant);
                    FillDescriptionOfTimeInstant(timeOntology, endTimeInstant);
                    FillPositionOfTimeInstant(timeOntology, endTimeInstant);

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
        
        /// <summary>
        /// Gets the temporal coordinate expressing the given instant, normalized according to the metrics of the given calendar TRS (Gregorian, if not provided)
        /// </summary>
        public static TIMECoordinate GetInstantCoordinate(this OWLOntology timeOntology, RDFResource timeInstantURI, TIMECalendarReferenceSystem calendarTRS=null)
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
            FillValueOfTimeInstant(timeOntology, timeInstant);
            FillDescriptionOfTimeInstant(timeOntology, timeInstant);
            FillPositionOfTimeInstant(timeOntology, timeInstant);

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
                    TIMECoordinate nominalPositionCoordinate = GetBeginningOfInterval(timeOntology, timeInstant.Position.NominalValue, calendarTRS);
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

        /// <summary>
        /// Gets the temporal extent expressing the given interval, normalized according to the metrics of the given calendar TRS (Gregorian, if not provided)
        /// </summary>
        public static TIMEExtent GetIntervalExtent(this OWLOntology timeOntology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
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
            FillValueOfTimeInterval(timeOntology, timeInterval);
            FillDescriptionOfTimeInterval(timeOntology, timeInterval);
            FillDurationOfTimeInterval(timeOntology, timeInterval);
            FillBeginningOfTimeInterval(timeOntology, timeInterval);
            FillEndOfTimeInterval(timeOntology, timeInterval);

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
                TIMECoordinate timeIntervalBeginning = GetInstantCoordinate(timeOntology, timeInterval.Beginning, calendarTRS);
                TIMECoordinate timeIntervalEnd = GetInstantCoordinate(timeOntology, timeInterval.End, calendarTRS);
                if (timeIntervalBeginning != null && timeIntervalEnd != null)
                    return TIMEConverter.CalculateExtent(timeIntervalBeginning, timeIntervalEnd, calendarTRS);
            }

            //In absence of a valid time interval encoding we simply cannot proceed
            return null;
        }

        /// <summary>
        /// Gets the temporal coordinate expressing the beginning instant of the given interval, normalized according to the metrics of the given calendar TRS (Gregorian, if not provided)<br/>
        /// (It can leverage Allen interval relations to infer this information even in case this is not directly expressed on the given interval)
        /// </summary>
        public static TIMECoordinate GetBeginningOfInterval(this OWLOntology timeOntology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get beginning of interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            return GetBeginningOfIntervalInternal(timeOntology, timeIntervalURI, calendarTRS, new Dictionary<long, RDFResource>());
        }
        internal static TIMECoordinate GetBeginningOfIntervalInternal(this OWLOntology timeOntology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS, Dictionary<long,RDFResource> visitContext)
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
            FillBeginningOfTimeInterval(timeOntology, timeInterval);

            //Get the coordinate of the time interval's beginning instant
            if (timeInterval.Beginning != null)
                return GetInstantCoordinate(timeOntology, timeInterval.Beginning, calendarTRS);

            #region Allen
            //There's no direct representation of the time interval's beginning instant:
            //we can indirectly obtain it by exploiting specific Allen interval relations
            List<RDFResource> compatibleStartingIntervals = RDFQueryUtilities.RemoveDuplicates(
                GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Equals)
                 .Union(GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Starts))
                  .Union(GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.StartedBy))
                  .ToList());

            //Perform a sequential search of the beginning instant on the set of compatible intervals
            TIMECoordinate compatibleBeginning = null;
            IEnumerator<RDFResource> compatibleStartingIntervalsEnumerator = compatibleStartingIntervals.GetEnumerator();
            while (compatibleBeginning == null && compatibleStartingIntervalsEnumerator.MoveNext())
                compatibleBeginning = GetBeginningOfIntervalInternal(timeOntology, compatibleStartingIntervalsEnumerator.Current, calendarTRS, visitContext);
            if (compatibleBeginning == null)
            {
                #region MetBy
                //There's still no direct representation of the time interval's beginning instant:
                //we can indirectly obtain it by exploiting Allen interval-meeting relations
                List<RDFResource> metByIntervals = RDFQueryUtilities.RemoveDuplicates(
                    GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.MetBy));

                //Perform a sequential search of the start instant on the set of metBy intervals
                IEnumerator<RDFResource> metByIntervalsEnumerator = metByIntervals.GetEnumerator();
                while (compatibleBeginning == null && metByIntervalsEnumerator.MoveNext())
                    compatibleBeginning = GetEndOfIntervalInternal(timeOntology, metByIntervalsEnumerator.Current, calendarTRS, visitContext);
                #endregion
            }
            #endregion

            return compatibleBeginning;
        }

        /// <summary>
        /// Gets the temporal coordinate expressing the end instant of the given interval, normalized according to the metrics of the given calendar TRS (Gregorian, if not provided)<br/>
        /// (It can leverage Allen interval relations to infer this information even in case this is not directly expressed on the given interval)
        /// </summary>
        public static TIMECoordinate GetEndOfInterval(this OWLOntology timeOntology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get end of interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            return GetEndOfIntervalInternal(timeOntology, timeIntervalURI, calendarTRS, new Dictionary<long, RDFResource>());
        }
        internal static TIMECoordinate GetEndOfIntervalInternal(this OWLOntology timeOntology, RDFResource timeIntervalURI, TIMECalendarReferenceSystem calendarTRS, Dictionary<long,RDFResource> visitContext)
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
            FillEndOfTimeInterval(timeOntology, timeInterval);

            //Get the coordinate of the time interval's end instant
            if (timeInterval.End != null)
                return GetInstantCoordinate(timeOntology, timeInterval.End, calendarTRS);

            #region Allen
            //There's no direct representation of the time interval's end instant:
            //we can indirectly obtain it by exploiting Allen interval-ending relations
            List<RDFResource> compatibleEndIntervals = RDFQueryUtilities.RemoveDuplicates(
                GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Equals)
                 .Union(GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Finishes))
                  .Union(GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.FinishedBy))
                  .ToList());

            //Perform a sequential search of the end instant on the set of compatible ending intervals
            TIMECoordinate compatibleEnd = null;
            IEnumerator<RDFResource> compatibleEndIntervalsEnumerator = compatibleEndIntervals.GetEnumerator();
            while (compatibleEnd == null && compatibleEndIntervalsEnumerator.MoveNext())
                compatibleEnd = GetEndOfIntervalInternal(timeOntology, compatibleEndIntervalsEnumerator.Current, calendarTRS, visitContext);
            if (compatibleEnd == null)
            {
                #region Meets
                //There's still no direct representation of the time interval's end instant:
                //we can indirectly obtain it by exploiting Allen interval-meeting relations
                List<RDFResource> meetsIntervals = RDFQueryUtilities.RemoveDuplicates(
                    GetRelatedIntervals(timeOntology, timeIntervalURI, TIMEEnums.TIMEIntervalRelation.Meets));

                //Perform a sequential search of the end instant on the set of meets intervals
                IEnumerator<RDFResource> meetsIntervalsEnumerator = meetsIntervals.GetEnumerator();
                while (compatibleEnd == null && meetsIntervalsEnumerator.MoveNext())
                    compatibleEnd = GetBeginningOfIntervalInternal(timeOntology, meetsIntervalsEnumerator.Current, calendarTRS, visitContext);
                #endregion
            }
            #endregion

            return compatibleEnd;
        }

        /// <summary>
        /// Gets the intervals having the given relation with the given interval 
        /// </summary>
        public static List<RDFResource> GetRelatedIntervals(this OWLOntology timeOntology, RDFResource timeIntervalURI, TIMEEnums.TIMEIntervalRelation timeIntervalRelation)
        {
            #region Guards
            if (timeIntervalURI == null)
                throw new OWLException("Cannot get related intervals because given \"timeIntervalURI\" parameter is null");
            #endregion

            //Fetch intervals linked with the given one through the given relation
            List<RDFResource> relatedIntervals = new List<RDFResource>();
            switch (timeIntervalRelation)
            {
                case TIMEEnums.TIMEIntervalRelation.After:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_AFTER, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Before:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_BEFORE, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Contains:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_CONTAINS, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Disjoint:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_DISJOINT, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.During:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_DURING, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Equals:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_EQUALS, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.FinishedBy:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_FINISHED_BY, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Finishes:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_FINISHES, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.HasInside:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.HAS_INSIDE, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.In:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_IN, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Meets:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_MEETS, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.MetBy:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_MET_BY, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.NotDisjoint:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.NOT_DISJOINT, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.OverlappedBy:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Overlaps:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_OVERLAPS, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.StartedBy:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_STARTED_BY, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
                case TIMEEnums.TIMEIntervalRelation.Starts:
                    relatedIntervals.AddRange(timeOntology.Data.ABoxGraph[timeIntervalURI, RDFVocabulary.TIME.INTERVAL_STARTS, null, null]
                        .Select(t => t.Object)
                        .OfType<RDFResource>());
                    break;
            }
            return relatedIntervals;
        }
        #endregion
    }
}
