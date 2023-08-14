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
using System.Collections.Generic;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// TIMEOntologyLoader is responsible for loading temporal ontologies from remote sources or alternative representations
    /// </summary>
    public static class TIMEOntologyLoader
    {
        #region Methods
        /// <summary>
        /// Prepares the given ontology for OWL-TIME support, making it suitable for temporal analysis
        /// </summary>
        public static void InitializeTIME(this OWLOntology ontology)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot initialize temporal ontology because given \"ontology\" parameter is null");
            #endregion

            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.TIME.BASE_URI));
            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.TIME.THORS.BASE_URI));
            BuildTIMEClassModel(ontology.Model.ClassModel);
            BuildTIMEPropertyModel(ontology.Model.PropertyModel);
            BuildTIMEData(ontology.Data);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Builds a reference temporal class model
        /// </summary>
        internal static OWLOntologyClassModel BuildTIMEClassModel(OWLOntologyClassModel existingClassModel = null)
        {
            OWLOntologyClassModel classModel = existingClassModel ?? new OWLOntologyClassModel();

            //OWL-TIME
            classModel.DeclareClass(RDFVocabulary.TIME.DATETIME_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.DATETIME_INTERVAL);
            classModel.DeclareClass(RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            classModel.DeclareClass(RDFVocabulary.TIME.DURATION);
            classModel.DeclareClass(RDFVocabulary.TIME.DURATION_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DAY);
            classModel.DeclareClass(RDFVocabulary.TIME.GENERAL_MONTH);
            classModel.DeclareClass(RDFVocabulary.TIME.GENERAL_YEAR);
            classModel.DeclareClass(RDFVocabulary.TIME.INSTANT);
            classModel.DeclareClass(RDFVocabulary.TIME.INTERVAL);
            classModel.DeclareClass(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            classModel.DeclareClass(RDFVocabulary.TIME.PROPER_INTERVAL);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_UNIT);
            classModel.DeclareClass(RDFVocabulary.TIME.TIMEZONE_CLASS);
            classModel.DeclareClass(RDFVocabulary.TIME.TIME_POSITION);
            classModel.DeclareClass(RDFVocabulary.TIME.TRS);
            classModel.DeclareHasValueRestriction(new RDFResource("bnode:HasGregorianTRSValue"), 
                RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneTRSValue"),
                RDFVocabulary.TIME.HAS_TRS, 1);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllYearValuesFromGYear"),
                RDFVocabulary.TIME.YEAR, RDFVocabulary.XSD.G_YEAR);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMonthValuesFromGMonth"),
                RDFVocabulary.TIME.MONTH, RDFVocabulary.XSD.G_MONTH);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllDayValuesFromGDay"),
                RDFVocabulary.TIME.DAY, RDFVocabulary.XSD.G_DAY);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNumericDurationValue"),
                RDFVocabulary.TIME.NUMERIC_DURATION, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNumericPositionValue"),
                RDFVocabulary.TIME.NUMERIC_POSITION, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNominalPositionValue"),
                RDFVocabulary.TIME.NOMINAL_POSITION, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneUnitTypeValue"),
                RDFVocabulary.TIME.UNIT_TYPE, 1);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllYearsValuesFromDecimal"),
                RDFVocabulary.TIME.YEARS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMonthsValuesFromDecimal"),
                RDFVocabulary.TIME.MONTHS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllWeeksValuesFromDecimal"),
                RDFVocabulary.TIME.WEEKS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllDaysValuesFromDecimal"),
                RDFVocabulary.TIME.DAYS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllHoursValuesFromDecimal"),
                RDFVocabulary.TIME.HOURS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMinutesValuesFromDecimal"),
                RDFVocabulary.TIME.MINUTES, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllSecondsValuesFromDecimal"),
                RDFVocabulary.TIME.SECONDS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneTimeZoneValue"),
                RDFVocabulary.TIME.TIMEZONE, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneYearValue"),
                RDFVocabulary.TIME.YEAR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthValue"),
                RDFVocabulary.TIME.MONTH, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayValue"),
                RDFVocabulary.TIME.DAY, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneHourValue"),
                RDFVocabulary.TIME.HOUR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMinuteValue"),
                RDFVocabulary.TIME.MINUTE, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneSecondValue"),
                RDFVocabulary.TIME.SECOND, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneWeekValue"),
                RDFVocabulary.TIME.WEEK, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayOfYearValue"),
                RDFVocabulary.TIME.DAY_OF_YEAR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayOfWeekValue"),
                RDFVocabulary.TIME.DAY_OF_WEEK, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthOfYearValue"),
                RDFVocabulary.TIME.MONTH_OF_YEAR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneYearsValue"),
                RDFVocabulary.TIME.YEARS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthsValue"),
                RDFVocabulary.TIME.MONTHS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDaysValue"),
                RDFVocabulary.TIME.DAYS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneHoursValue"),
                RDFVocabulary.TIME.HOURS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMinutesValue"),
                RDFVocabulary.TIME.MINUTES, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneSecondsValue"),
                RDFVocabulary.TIME.SECONDS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneWeeksValue"),
                RDFVocabulary.TIME.WEEKS, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoYearValues"),
                RDFVocabulary.TIME.YEAR, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoMonthValues"),
                RDFVocabulary.TIME.MONTH, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneMonthValues"),
                RDFVocabulary.TIME.MONTH, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoWeekValues"),
                RDFVocabulary.TIME.WEEK, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoDayValues"),
                RDFVocabulary.TIME.DAY, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoHourValues"),
                RDFVocabulary.TIME.HOUR, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoMinuteValues"),
                RDFVocabulary.TIME.MINUTE, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoSecondValues"),
                RDFVocabulary.TIME.SECOND, 0);
            classModel.DeclareHasValueRestriction(new RDFResource("bnode:HasUnitTypeMonthValue"),
                RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_MONTH);
            classModel.DeclareUnionClass(RDFVocabulary.TIME.TEMPORAL_ENTITY, new List<RDFResource>() {
                RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.INTERVAL });
            classModel.DeclareUnionClass(new RDFResource("bnode:HasTRSDomain"), new List<RDFResource>() {
                RDFVocabulary.TIME.TEMPORAL_POSITION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION });
            classModel.DeclareUnionClass(new RDFResource("bnode:HasExactlyOneNumericPositionValueOrNominalPositionValue"), new List<RDFResource>() {
                new RDFResource("bnode:HasExactlyOneNumericPositionValue"), new RDFResource("bnode:HasExactlyOneNominalPositionValue") });
            classModel.DeclareUnionClass(new RDFResource("bnode:GeneralDateTimeDescriptionOrDuration"), new List<RDFResource>() {
                RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, RDFVocabulary.TIME.DURATION });
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasGregorianTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllYearValuesFromGYear"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllMonthValuesFromGMonth"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllDayValuesFromGDay"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_INTERVAL, RDFVocabulary.TIME.PROPER_INTERVAL);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, new RDFResource("bnode:HasExactlyOneNumericDurationValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, new RDFResource("bnode:HasExactlyOneUnitTypeValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasGregorianTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllYearsValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllMonthsValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllWeeksValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllDaysValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllHoursValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllMinutesValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllSecondsValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasExactlyOneTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneYearsValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthsValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDaysValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneHoursValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMinutesValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneSecondsValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneWeeksValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.INTERVAL, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, RDFVocabulary.TIME.DATETIME_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoYearValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasExactlyOneMonthValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoWeekValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoDayValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoHourValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoMinuteValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoSecondValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasUnitTypeMonthValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.PROPER_INTERVAL, RDFVocabulary.TIME.INTERVAL);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TEMPORAL_UNIT, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TIME_POSITION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TIME_POSITION, new RDFResource("bnode:HasExactlyOneNumericPositionValueOrNominalPositionValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TEMPORAL_POSITION, new RDFResource("bnode:HasExactlyOneTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneTimeZoneValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasExactlyOneUnitTypeValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneYearValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneHourValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMinuteValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneSecondValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneWeekValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayOfYearValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayOfWeekValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthOfYearValue"));
            classModel.DeclareDisjointClasses(RDFVocabulary.TIME.PROPER_INTERVAL, RDFVocabulary.TIME.INSTANT);

            //THORS
            classModel.DeclareClass(RDFVocabulary.TIME.THORS.ERA);
            classModel.DeclareClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY);
            classModel.DeclareClass(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneThorsBegin"),
                RDFVocabulary.TIME.THORS.BEGIN, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneThorsEnd"),
                RDFVocabulary.TIME.THORS.END, 1);
            classModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumOneThorsNextEra"),
                RDFVocabulary.TIME.THORS.NEXT_ERA, 1);
            classModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumOneThorsPreviousEra"),
                RDFVocabulary.TIME.THORS.PREVIOUS_ERA, 1);
            classModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumOneThorsComponent"),
                RDFVocabulary.TIME.THORS.COMPONENT, 1);
            classModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumTwoThorsReferencePoints"),
                RDFVocabulary.TIME.THORS.REFERENCE_POINT, 2);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, RDFVocabulary.TIME.TRS);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, new RDFResource("bnode:HasMinimumOneThorsComponent"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, new RDFResource("bnode:HasMinimumTwoThorsReferencePoints"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA, RDFVocabulary.TIME.PROPER_INTERVAL);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA, new RDFResource("bnode:HasExactlyOneThorsBegin"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA, new RDFResource("bnode:HasExactlyOneThorsEnd"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA_BOUNDARY, RDFVocabulary.TIME.INSTANT);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA_BOUNDARY, new RDFResource("bnode:HasMinimumOneThorsNextEra"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA_BOUNDARY, new RDFResource("bnode:HasMinimumOneThorsPreviousEra"));

            return classModel;
        }

        /// <summary>
        /// Builds a reference temporal property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildTIMEPropertyModel(OWLOntologyPropertyModel existingPropertyModel = null)
        {
            OWLOntologyPropertyModel propertyModel = existingPropertyModel ?? new OWLOntologyPropertyModel();

            //OWL-TIME            
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.AFTER, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.BEFORE, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.DAY_OF_WEEK_CLASS });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY_OF_YEAR, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAYS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.DISJOINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.EQUALS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DATETIME_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.DATETIME_INTERVAL, Range = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.DURATION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.DURATION_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_END, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_INSIDE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TEMPORAL_DURATION, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_DURATION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TIME, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TRS, new OWLOntologyObjectPropertyBehavior() { 
                Domain = new RDFResource("bnode:HasTRSDomain"), Range = RDFVocabulary.TIME.TRS, Functional = true });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HAS_XSD_DURATION, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.XSD.DURATION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOUR, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOURS, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.IN_DATETIME, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INSIDE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INTERVAL, Range = RDFVocabulary.TIME.INSTANT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.IN_TEMPORAL_POSITION, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.TIME.TEMPORAL_POSITION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHED_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_IN, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.TIME.TIME_POSITION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_DATE, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.DATE });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_DATETIME, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.DATETIME, Deprecated = true });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.DATETIMESTAMP });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_GYEAR, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.G_YEAR });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_GYEARMONTH, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.G_YEAR_MONTH });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MINUTE, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MINUTES, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MONTH, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MONTHS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TIME_POSITION, Range = RDFVocabulary.TIME.INTERVAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.NOT_DISJOINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.NUMERIC_DURATION, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.DURATION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.NUMERIC_POSITION, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.TIME_POSITION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.SECOND, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.SECONDS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.TIMEZONE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.TIMEZONE_CLASS });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.UNIT_TYPE, new OWLOntologyObjectPropertyBehavior() {
                Domain = new RDFResource("bnode:GeneralDateTimeDescriptionOrDuration"), Range = RDFVocabulary.TIME.TEMPORAL_UNIT });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.WEEK, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.WEEKS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.XSD_DATETIME, new OWLOntologyDatatypePropertyBehavior()  {
                Domain = RDFVocabulary.TIME.DATETIME_INTERVAL, Range = RDFVocabulary.XSD.DATETIME, Deprecated = true });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.YEAR, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.YEARS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.AFTER, RDFVocabulary.TIME.DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.BEFORE, RDFVocabulary.TIME.DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.EQUALS, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_DURATION, RDFVocabulary.TIME.HAS_TEMPORAL_DURATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, RDFVocabulary.TIME.HAS_TEMPORAL_DURATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_INSIDE, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INSIDE, RDFVocabulary.TIME.HAS_INSIDE);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.IN_DATETIME, RDFVocabulary.TIME.IN_TEMPORAL_POSITION);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_AFTER, RDFVocabulary.TIME.AFTER);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_AFTER, RDFVocabulary.TIME.INTERVAL_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_BEFORE, RDFVocabulary.TIME.BEFORE);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_BEFORE, RDFVocabulary.TIME.INTERVAL_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_CONTAINS, RDFVocabulary.TIME.HAS_INSIDE);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_DISJOINT, RDFVocabulary.TIME.DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_EQUALS, RDFVocabulary.TIME.EQUALS);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_FINISHED_BY, RDFVocabulary.TIME.HAS_INSIDE);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_FINISHES, RDFVocabulary.TIME.INTERVAL_IN);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_IN, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_MEETS, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_MET_BY, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_OVERLAPS, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_STARTED_BY, RDFVocabulary.TIME.HAS_INSIDE);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.IN_TIME_POSITION, RDFVocabulary.TIME.IN_TEMPORAL_POSITION);            
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.AFTER, RDFVocabulary.TIME.BEFORE);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_AFTER, RDFVocabulary.TIME.INTERVAL_BEFORE);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_CONTAINS, RDFVocabulary.TIME.INTERVAL_DURING);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_FINISHES, RDFVocabulary.TIME.INTERVAL_FINISHED_BY);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_MEETS, RDFVocabulary.TIME.INTERVAL_MET_BY);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_OVERLAPS, RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_STARTS, RDFVocabulary.TIME.INTERVAL_STARTED_BY);            
            propertyModel.DeclareDisjointProperties(RDFVocabulary.TIME.DISJOINT, RDFVocabulary.TIME.NOT_DISJOINT);
            propertyModel.DeclareDisjointProperties(RDFVocabulary.TIME.HAS_INSIDE, RDFVocabulary.TIME.EQUALS);
            propertyModel.DeclareDisjointProperties(RDFVocabulary.TIME.INTERVAL_EQUALS, RDFVocabulary.TIME.INTERVAL_IN);

            //THORS
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.BEGIN, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Deprecated = true });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, Range = RDFVocabulary.TIME.THORS.ERA });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.END, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Deprecated = true });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.MEMBER, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.ERA, Deprecated = true });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Range = RDFVocabulary.TIME.THORS.ERA });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TIME_POSITION, Range = RDFVocabulary.TIME.DURATION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Range = RDFVocabulary.TIME.THORS.ERA });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, Range = RDFVocabulary.TIME.THORS.ERA_BOUNDARY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM });
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.THORS.BEGIN, RDFVocabulary.TIME.HAS_BEGINNING);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.THORS.END, RDFVocabulary.TIME.HAS_END);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.THORS.COMPONENT, RDFVocabulary.TIME.THORS.SYSTEM);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.THORS.BEGIN, RDFVocabulary.TIME.THORS.NEXT_ERA);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.THORS.END, RDFVocabulary.TIME.THORS.PREVIOUS_ERA);

            return propertyModel;
        }

        /// <summary>
        /// Builds a reference temporal data
        /// </summary>
        internal static OWLOntologyData BuildTIMEData(OWLOntologyData existingData = null)
        {
            OWLOntologyData data = existingData ?? new OWLOntologyData();

            //OWL-TIME
            data.DeclareIndividual(TIMECalendarReferenceSystem.Gregorian);
            data.DeclareIndividual(TIMEPositionReferenceSystem.UnixTRS);
            data.DeclareIndividual(TIMEPositionReferenceSystem.GeologicTRS);
            data.DeclareIndividual(TIMEPositionReferenceSystem.GlobalPositioningSystemTRS);
            data.DeclareIndividual(RDFVocabulary.TIME.MONDAY);
            data.DeclareIndividual(RDFVocabulary.TIME.TUESDAY);
            data.DeclareIndividual(RDFVocabulary.TIME.WEDNESDAY);
            data.DeclareIndividual(RDFVocabulary.TIME.THURSDAY);
            data.DeclareIndividual(RDFVocabulary.TIME.FRIDAY);
            data.DeclareIndividual(RDFVocabulary.TIME.SATURDAY);
            data.DeclareIndividual(RDFVocabulary.TIME.SUNDAY);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.JANUARY);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.FEBRUARY);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.MARCH);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.APRIL);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.MAY);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.JUNE);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.JULY);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.AUGUST);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.SEPTEMBER);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.OCTOBER);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.NOVEMBER);
            data.DeclareIndividual(RDFVocabulary.TIME.GREG.DECEMBER);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_MILLENIUM);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_CENTURY);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_DECADE);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_YEAR);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_MONTH);            
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_WEEK);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_DAY);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_HOUR);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_MINUTE);
            data.DeclareIndividual(RDFVocabulary.TIME.UNIT_SECOND);
            data.DeclareIndividualType(TIMECalendarReferenceSystem.Gregorian, RDFVocabulary.TIME.TRS);
            data.DeclareIndividualType(TIMEPositionReferenceSystem.UnixTRS, RDFVocabulary.TIME.TRS);
            data.DeclareIndividualType(TIMEPositionReferenceSystem.GeologicTRS, RDFVocabulary.TIME.TRS);
            data.DeclareIndividualType(TIMEPositionReferenceSystem.GlobalPositioningSystemTRS, RDFVocabulary.TIME.TRS);
            data.DeclareIndividualType(RDFVocabulary.TIME.MONDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.TUESDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.WEDNESDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.THURSDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.FRIDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.SATURDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.SUNDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.JANUARY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.FEBRUARY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.MARCH, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.APRIL, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.MAY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.JUNE, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.JULY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.AUGUST, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.SEPTEMBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.OCTOBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.NOVEMBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.GREG.DECEMBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_MILLENIUM, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_CENTURY, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_DECADE, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_YEAR, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_MONTH, RDFVocabulary.TIME.TEMPORAL_UNIT);            
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_WEEK, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_DAY, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_HOUR, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_MINUTE, RDFVocabulary.TIME.TEMPORAL_UNIT);
            data.DeclareIndividualType(RDFVocabulary.TIME.UNIT_SECOND, RDFVocabulary.TIME.TEMPORAL_UNIT);

            return data;
        }
        #endregion
    }
}