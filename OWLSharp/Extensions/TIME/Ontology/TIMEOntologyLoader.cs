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
        /// Prepares the given ontology for OWL-TIME support, making it suitable for temporal modeling and analysis
        /// </summary>
        public static void InitializeTIME(this OWLOntology ontology)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot initialize TIME ontology because given \"ontology\" parameter is null");
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
        internal static OWLOntologyClassModel BuildTIMEClassModel(OWLOntologyClassModel existingClassModel=null)
        {
            OWLOntologyClassModel timeClassModel = new OWLOntologyClassModel();

            #region OWL-TIME T-BOX

            //OWL-TIME
            timeClassModel.DeclareClass(RDFVocabulary.TIME.DATETIME_DESCRIPTION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.DATETIME_INTERVAL);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.DURATION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.DURATION_DESCRIPTION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DAY);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.GENERAL_MONTH);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.GENERAL_YEAR);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.INSTANT);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.INTERVAL);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.PROPER_INTERVAL);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_DURATION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_ENTITY);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_POSITION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.TIMEZONE_CLASS);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.TIME_POSITION);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.TRS);
            timeClassModel.DeclareHasValueRestriction(new RDFResource("bnode:HasGregorianTRSValue"), 
                RDFVocabulary.TIME.HAS_TRS, TIMECalendarReferenceSystem.Gregorian);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneTRSValue"),
                RDFVocabulary.TIME.HAS_TRS, 1);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllYearValuesFromGYear"),
                RDFVocabulary.TIME.YEAR, RDFVocabulary.XSD.G_YEAR);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMonthValuesFromGMonth"),
                RDFVocabulary.TIME.MONTH, RDFVocabulary.XSD.G_MONTH);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllDayValuesFromGDay"),
                RDFVocabulary.TIME.DAY, RDFVocabulary.XSD.G_DAY);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNumericDurationValue"),
                RDFVocabulary.TIME.NUMERIC_DURATION, 1);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNumericPositionValue"),
                RDFVocabulary.TIME.NUMERIC_POSITION, 1);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNominalPositionValue"),
                RDFVocabulary.TIME.NOMINAL_POSITION, 1);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneUnitTypeValue"),
                RDFVocabulary.TIME.UNIT_TYPE, 1);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllYearsValuesFromDecimal"),
                RDFVocabulary.TIME.YEARS, RDFVocabulary.XSD.DECIMAL);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMonthsValuesFromDecimal"),
                RDFVocabulary.TIME.MONTHS, RDFVocabulary.XSD.DECIMAL);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllWeeksValuesFromDecimal"),
                RDFVocabulary.TIME.WEEKS, RDFVocabulary.XSD.DECIMAL);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllDaysValuesFromDecimal"),
                RDFVocabulary.TIME.DAYS, RDFVocabulary.XSD.DECIMAL);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllHoursValuesFromDecimal"),
                RDFVocabulary.TIME.HOURS, RDFVocabulary.XSD.DECIMAL);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMinutesValuesFromDecimal"),
                RDFVocabulary.TIME.MINUTES, RDFVocabulary.XSD.DECIMAL);
            timeClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllSecondsValuesFromDecimal"),
                RDFVocabulary.TIME.SECONDS, RDFVocabulary.XSD.DECIMAL);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneTimeZoneValue"),
                RDFVocabulary.TIME.TIMEZONE, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneYearValue"),
                RDFVocabulary.TIME.YEAR, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthValue"),
                RDFVocabulary.TIME.MONTH, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayValue"),
                RDFVocabulary.TIME.DAY, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneHourValue"),
                RDFVocabulary.TIME.HOUR, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMinuteValue"),
                RDFVocabulary.TIME.MINUTE, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneSecondValue"),
                RDFVocabulary.TIME.SECOND, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneWeekValue"),
                RDFVocabulary.TIME.WEEK, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayOfYearValue"),
                RDFVocabulary.TIME.DAY_OF_YEAR, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayOfWeekValue"),
                RDFVocabulary.TIME.DAY_OF_WEEK, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthOfYearValue"),
                RDFVocabulary.TIME.MONTH_OF_YEAR, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneYearsValue"),
                RDFVocabulary.TIME.YEARS, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthsValue"),
                RDFVocabulary.TIME.MONTHS, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDaysValue"),
                RDFVocabulary.TIME.DAYS, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneHoursValue"),
                RDFVocabulary.TIME.HOURS, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMinutesValue"),
                RDFVocabulary.TIME.MINUTES, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneSecondsValue"),
                RDFVocabulary.TIME.SECONDS, 1);
            timeClassModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneWeeksValue"),
                RDFVocabulary.TIME.WEEKS, 1);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoYearValues"),
                RDFVocabulary.TIME.YEAR, 0);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoMonthValues"),
                RDFVocabulary.TIME.MONTH, 0);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneMonthValues"),
                RDFVocabulary.TIME.MONTH, 1);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoWeekValues"),
                RDFVocabulary.TIME.WEEK, 0);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoDayValues"),
                RDFVocabulary.TIME.DAY, 0);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoHourValues"),
                RDFVocabulary.TIME.HOUR, 0);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoMinuteValues"),
                RDFVocabulary.TIME.MINUTE, 0);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoSecondValues"),
                RDFVocabulary.TIME.SECOND, 0);
            timeClassModel.DeclareHasValueRestriction(new RDFResource("bnode:HasUnitTypeMonthValue"),
                RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_MONTH);
            timeClassModel.DeclareUnionClass(RDFVocabulary.TIME.TEMPORAL_ENTITY, new List<RDFResource>() {
                RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.INTERVAL });
            timeClassModel.DeclareUnionClass(new RDFResource("bnode:HasTRSDomain"), new List<RDFResource>() {
                RDFVocabulary.TIME.TEMPORAL_POSITION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION });
            timeClassModel.DeclareUnionClass(new RDFResource("bnode:HasExactlyOneNumericPositionValueOrNominalPositionValue"), new List<RDFResource>() {
                new RDFResource("bnode:HasExactlyOneNumericPositionValue"), new RDFResource("bnode:HasExactlyOneNominalPositionValue") });
            timeClassModel.DeclareUnionClass(new RDFResource("bnode:GeneralDateTimeDescriptionOrDuration"), new List<RDFResource>() {
                RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, RDFVocabulary.TIME.DURATION });
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasGregorianTRSValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllYearValuesFromGYear"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllMonthValuesFromGMonth"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllDayValuesFromGDay"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_INTERVAL, RDFVocabulary.TIME.PROPER_INTERVAL);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, new RDFResource("bnode:HasExactlyOneNumericDurationValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, new RDFResource("bnode:HasExactlyOneUnitTypeValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasGregorianTRSValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllYearsValuesFromDecimal"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllMonthsValuesFromDecimal"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllWeeksValuesFromDecimal"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllDaysValuesFromDecimal"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllHoursValuesFromDecimal"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllMinutesValuesFromDecimal"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllSecondsValuesFromDecimal"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasExactlyOneTRSValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneYearsValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthsValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDaysValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneHoursValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMinutesValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneSecondsValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneWeeksValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.INTERVAL, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, RDFVocabulary.TIME.DATETIME_DESCRIPTION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoYearValues"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasExactlyOneMonthValues"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoWeekValues"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoDayValues"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoHourValues"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoMinuteValues"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoSecondValues"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasUnitTypeMonthValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.PROPER_INTERVAL, RDFVocabulary.TIME.INTERVAL);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.TEMPORAL_UNIT, RDFVocabulary.TIME.TEMPORAL_DURATION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.TIME_POSITION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.TIME_POSITION, new RDFResource("bnode:HasExactlyOneNumericPositionValueOrNominalPositionValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.TEMPORAL_POSITION, new RDFResource("bnode:HasExactlyOneTRSValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneTimeZoneValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasExactlyOneUnitTypeValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneYearValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneHourValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMinuteValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneSecondValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneWeekValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayOfYearValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayOfWeekValue"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthOfYearValue"));
            timeClassModel.DeclareDisjointClasses(RDFVocabulary.TIME.PROPER_INTERVAL, RDFVocabulary.TIME.INSTANT);

            //THORS
            timeClassModel.DeclareClass(RDFVocabulary.TIME.THORS.ERA);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY);
            timeClassModel.DeclareClass(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneThorsBegin"),
                RDFVocabulary.TIME.THORS.BEGIN, 1);
            timeClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneThorsEnd"),
                RDFVocabulary.TIME.THORS.END, 1);
            timeClassModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumOneThorsNextEra"),
                RDFVocabulary.TIME.THORS.NEXT_ERA, 1);
            timeClassModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumOneThorsPreviousEra"),
                RDFVocabulary.TIME.THORS.PREVIOUS_ERA, 1);
            timeClassModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumOneThorsComponent"),
                RDFVocabulary.TIME.THORS.COMPONENT, 1);
            timeClassModel.DeclareMinCardinalityRestriction(new RDFResource("bnode:HasMinimumTwoThorsReferencePoints"),
                RDFVocabulary.TIME.THORS.REFERENCE_POINT, 2);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, RDFVocabulary.TIME.TRS);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, new RDFResource("bnode:HasMinimumOneThorsComponent"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, new RDFResource("bnode:HasMinimumTwoThorsReferencePoints"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA, RDFVocabulary.TIME.PROPER_INTERVAL);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA, new RDFResource("bnode:HasExactlyOneThorsBegin"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA, new RDFResource("bnode:HasExactlyOneThorsEnd"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA_BOUNDARY, RDFVocabulary.TIME.INSTANT);
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA_BOUNDARY, new RDFResource("bnode:HasMinimumOneThorsNextEra"));
            timeClassModel.DeclareSubClasses(RDFVocabulary.TIME.THORS.ERA_BOUNDARY, new RDFResource("bnode:HasMinimumOneThorsPreviousEra"));

            #endregion

            existingClassModel?.Merge(timeClassModel);
            return existingClassModel ?? timeClassModel;
        }

        /// <summary>
        /// Builds a reference temporal property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildTIMEPropertyModel(OWLOntologyPropertyModel existingPropertyModel=null)
        {
            OWLOntologyPropertyModel timePropertyModel = new OWLOntologyPropertyModel();

            #region OWL-TIME T-BOX

            //OWL-TIME            
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.AFTER, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.BEFORE, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.DAY_OF_WEEK_CLASS });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY_OF_YEAR, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAYS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.DISJOINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.EQUALS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DATETIME_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.DATETIME_INTERVAL, Range = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.DURATION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.DURATION_DESCRIPTION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_END, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_INSIDE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TEMPORAL_DURATION, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_DURATION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TIME, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TRS, new OWLOntologyObjectPropertyBehavior() { 
                Domain = new RDFResource("bnode:HasTRSDomain"), Range = RDFVocabulary.TIME.TRS, Functional = true });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HAS_XSD_DURATION, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.XSD.DURATION });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOUR, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOURS, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.IN_DATETIME, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INSIDE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INTERVAL, Range = RDFVocabulary.TIME.INSTANT });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.IN_TEMPORAL_POSITION, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.TIME.TEMPORAL_POSITION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_AFTER, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_CONTAINS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_DISJOINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_DURING, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHED_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_IN, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_MEETS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_MET_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTED_BY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.PROPER_INTERVAL, Range = RDFVocabulary.TIME.PROPER_INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.IN_TIME_POSITION, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.TIME.TIME_POSITION });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_DATE, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.DATE });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_DATETIME, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.DATETIME, Deprecated = true });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.DATETIMESTAMP });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_GYEAR, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.G_YEAR });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.IN_XSD_GYEARMONTH, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.INSTANT, Range = RDFVocabulary.XSD.G_YEAR_MONTH });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MINUTE, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MINUTES, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MONTH, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.MONTH_OF_YEAR, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.MONTHS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.NOMINAL_POSITION, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TIME_POSITION, Range = RDFVocabulary.TIME.INTERVAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.NOT_DISJOINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.NUMERIC_DURATION, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.DURATION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.NUMERIC_POSITION, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.TIME_POSITION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.SECOND, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.SECONDS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.TIMEZONE, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.TIMEZONE_CLASS });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.UNIT_TYPE, new OWLOntologyObjectPropertyBehavior() {
                Domain = new RDFResource("bnode:GeneralDateTimeDescriptionOrDuration"), Range = RDFVocabulary.TIME.TEMPORAL_UNIT });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.WEEK, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.WEEKS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.XSD_DATETIME, new OWLOntologyDatatypePropertyBehavior()  {
                Domain = RDFVocabulary.TIME.DATETIME_INTERVAL, Range = RDFVocabulary.XSD.DATETIME, Deprecated = true });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.YEAR, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            timePropertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.YEARS, new OWLOntologyDatatypePropertyBehavior() {
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.AFTER, RDFVocabulary.TIME.DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.BEFORE, RDFVocabulary.TIME.DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.EQUALS, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_DURATION, RDFVocabulary.TIME.HAS_TEMPORAL_DURATION);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, RDFVocabulary.TIME.HAS_TEMPORAL_DURATION);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_INSIDE, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INSIDE, RDFVocabulary.TIME.HAS_INSIDE);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.IN_DATETIME, RDFVocabulary.TIME.IN_TEMPORAL_POSITION);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_AFTER, RDFVocabulary.TIME.AFTER);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_AFTER, RDFVocabulary.TIME.INTERVAL_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_BEFORE, RDFVocabulary.TIME.BEFORE);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_BEFORE, RDFVocabulary.TIME.INTERVAL_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_CONTAINS, RDFVocabulary.TIME.HAS_INSIDE);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_DISJOINT, RDFVocabulary.TIME.DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_EQUALS, RDFVocabulary.TIME.EQUALS);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_FINISHED_BY, RDFVocabulary.TIME.HAS_INSIDE);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_FINISHES, RDFVocabulary.TIME.INTERVAL_IN);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_IN, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_MEETS, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_MET_BY, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_OVERLAPS, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.INTERVAL_STARTED_BY, RDFVocabulary.TIME.HAS_INSIDE);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.IN_TIME_POSITION, RDFVocabulary.TIME.IN_TEMPORAL_POSITION);            
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.AFTER, RDFVocabulary.TIME.BEFORE);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_AFTER, RDFVocabulary.TIME.INTERVAL_BEFORE);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_CONTAINS, RDFVocabulary.TIME.INTERVAL_DURING);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_FINISHES, RDFVocabulary.TIME.INTERVAL_FINISHED_BY);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_MEETS, RDFVocabulary.TIME.INTERVAL_MET_BY);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_OVERLAPS, RDFVocabulary.TIME.INTERVAL_OVERLAPPED_BY);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.INTERVAL_STARTS, RDFVocabulary.TIME.INTERVAL_STARTED_BY);            
            timePropertyModel.DeclareDisjointProperties(RDFVocabulary.TIME.DISJOINT, RDFVocabulary.TIME.NOT_DISJOINT);
            timePropertyModel.DeclareDisjointProperties(RDFVocabulary.TIME.HAS_INSIDE, RDFVocabulary.TIME.EQUALS);
            timePropertyModel.DeclareDisjointProperties(RDFVocabulary.TIME.INTERVAL_EQUALS, RDFVocabulary.TIME.INTERVAL_IN);

            //THORS
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.BEGIN, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Deprecated = true });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, Range = RDFVocabulary.TIME.THORS.ERA });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.END, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Deprecated = true });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.MEMBER, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.ERA, Deprecated = true });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Range = RDFVocabulary.TIME.THORS.ERA });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.POSITIONAL_UNCERTAINTY, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.TIME_POSITION, Range = RDFVocabulary.TIME.DURATION });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA_BOUNDARY, Range = RDFVocabulary.TIME.THORS.ERA });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM, Range = RDFVocabulary.TIME.THORS.ERA_BOUNDARY });
            timePropertyModel.DeclareObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM, new OWLOntologyObjectPropertyBehavior() {
                Domain = RDFVocabulary.TIME.THORS.ERA, Range = RDFVocabulary.TIME.THORS.REFERENCE_SYSTEM });
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.THORS.BEGIN, RDFVocabulary.TIME.HAS_BEGINNING);
            timePropertyModel.DeclareSubProperties(RDFVocabulary.TIME.THORS.END, RDFVocabulary.TIME.HAS_END);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.THORS.COMPONENT, RDFVocabulary.TIME.THORS.SYSTEM);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.THORS.BEGIN, RDFVocabulary.TIME.THORS.NEXT_ERA);
            timePropertyModel.DeclareInverseProperties(RDFVocabulary.TIME.THORS.END, RDFVocabulary.TIME.THORS.PREVIOUS_ERA);

            #endregion

            existingPropertyModel?.Merge(timePropertyModel);
            return timePropertyModel;
        }

        /// <summary>
        /// Builds a reference temporal data
        /// </summary>
        internal static OWLOntologyData BuildTIMEData(OWLOntologyData existingData=null)
        {
            OWLOntologyData timeData = new OWLOntologyData();

            #region OWL-TIME A-BOX

            //OWL-TIME
            timeData.DeclareIndividual(TIMECalendarReferenceSystem.Gregorian);
            timeData.DeclareIndividual(TIMEPositionReferenceSystem.UnixTRS);
            timeData.DeclareIndividual(TIMEPositionReferenceSystem.GeologicTRS);
            timeData.DeclareIndividual(TIMEPositionReferenceSystem.GlobalPositioningSystemTRS);
            timeData.DeclareIndividual(RDFVocabulary.TIME.MONDAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.TUESDAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.WEDNESDAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.THURSDAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.FRIDAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.SATURDAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.SUNDAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.JANUARY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.FEBRUARY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.MARCH);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.APRIL);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.MAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.JUNE);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.JULY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.AUGUST);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.SEPTEMBER);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.OCTOBER);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.NOVEMBER);
            timeData.DeclareIndividual(RDFVocabulary.TIME.GREG.DECEMBER);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_MILLENIUM);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_CENTURY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_DECADE);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_YEAR);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_MONTH);            
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_WEEK);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_DAY);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_HOUR);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_MINUTE);
            timeData.DeclareIndividual(RDFVocabulary.TIME.UNIT_SECOND);
            timeData.DeclareIndividualType(TIMECalendarReferenceSystem.Gregorian, RDFVocabulary.TIME.TRS);
            timeData.DeclareIndividualType(TIMEPositionReferenceSystem.UnixTRS, RDFVocabulary.TIME.TRS);
            timeData.DeclareIndividualType(TIMEPositionReferenceSystem.GeologicTRS, RDFVocabulary.TIME.TRS);
            timeData.DeclareIndividualType(TIMEPositionReferenceSystem.GlobalPositioningSystemTRS, RDFVocabulary.TIME.TRS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.MONDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.TUESDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.WEDNESDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.THURSDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.FRIDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.SATURDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.SUNDAY, RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.JANUARY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.FEBRUARY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.MARCH, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.APRIL, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.MAY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.JUNE, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.JULY, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.AUGUST, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.SEPTEMBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.OCTOBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.NOVEMBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.GREG.DECEMBER, RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_MILLENIUM, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_CENTURY, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_DECADE, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_YEAR, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_MONTH, RDFVocabulary.TIME.TEMPORAL_UNIT);            
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_WEEK, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_DAY, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_HOUR, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_MINUTE, RDFVocabulary.TIME.TEMPORAL_UNIT);
            timeData.DeclareIndividualType(RDFVocabulary.TIME.UNIT_SECOND, RDFVocabulary.TIME.TEMPORAL_UNIT);

            #endregion

            existingData?.Merge(timeData);
            return timeData;
        }
        #endregion
    }
}