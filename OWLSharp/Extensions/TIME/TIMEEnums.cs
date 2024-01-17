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

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// TIMEEnums represents a collector for all the enumerations used by the "OWLSharp.Extensions.TIME" namespace
    /// </summary>
    public static class TIMEEnums
    {
        /// <summary>
        /// Represents an enumeration for the set of built-in time unit types in which TRS can measure time
        /// </summary>
        public enum TIMEUnitType
        {
            /// <summary>
            /// Indicates that TRS measures time in years
            /// </summary>
            Year = 1,
            /// <summary>
            /// Indicates that TRS measures time in months
            /// </summary>
            Month = 2,
            /// <summary>
            /// Indicates that TRS measures time in days
            /// </summary>
            Day = 3,
            /// <summary>
            /// Indicates that TRS measures time in hours
            /// </summary>
            Hour = 4,
            /// <summary>
            /// Indicates that TRS measures time in minutes
            /// </summary>
            Minute = 5,
            /// <summary>
            /// Indicates that TRS measures time in seconds
            /// </summary>
            Second = 6
        }

        /// <summary>
        /// Represents an enumeration for the set of built-in Allen algebra relations which can occur between time instants
        /// </summary>
        public enum TIMEInstantRelation
        {
            /// <summary>
            /// T1 is after T2
            /// </summary>
            After = 1,
            /// <summary>
            /// T1 is before T2
            /// </summary>
            Before = 2
        }

        /// <summary>
        /// Represents an enumeration for the set of built-in Allen algebra relations which can occur between time intervals
        /// </summary>
        public enum TIMEIntervalRelation
        {
            /// <summary>
            /// The beginning of T1 is after the end of T2
            /// </summary>
            After = 1,
            /// <summary>
            /// The end of T1 is before the beginning of T2
            /// </summary>
            Before = 2,
            /// <summary>
            /// The beginning of T1 is before the beginning of T2, and the end of T1 is after the end of T2
            /// </summary>
            Contains = 3,
            /// <summary>
            /// The beginning of T1 is after the end of T2, or the end of T1 is before the beginning of T2<br/>
            /// (the intervals do not overlap in any way, but their ordering relationship is not known)
            /// </summary>
            Disjoint = 4,
            /// <summary>
            /// The beginning of T1 is after the beginning of T2, and the end of T1 is before the end of T2
            /// </summary>
            During = 5,
            /// <summary>
            /// The beginning of T1 is coincident with the beginning of T2, and the end of T1 is coincident with the end of T2
            /// </summary>
            Equals = 6,
            /// <summary>
            /// The beginning of T1 is after the beginning of T2, and the end of T1 is coincident with the end of T2
            /// </summary>
            Finishes = 7,
            /// <summary>
            /// The beginning of T1 is before the beginning of T2, and the end of T1 is coincident with the end of T2
            /// </summary>
            FinishedBy = 8,
            /// <summary>
            /// The beginning of T1 is coincident with or before the beginning of T2, and the end of T1 is concident with or after the end of T2,<br/>
            /// except that end of T1 may not be coincident with the end of T2 if the beginning of T1 is coincident with the beginning of T2
            /// </summary>
            HasInside = 9,
            /// <summary>
            /// The beginning of T1 is after or coincident with the beginning of T2, and the end of T1 is before or coincident with the end of T2<br/>
            /// (the intervals cannot be equal: end of T1 may not be coincident with the end of T2 if the beginning of T1 is coincident with the beginning of T2)
            /// </summary>
            In = 10,
            /// <summary>
            /// The end of T1 is coincident with the beginning of T2
            /// </summary>
            Meets = 11,
            /// <summary>
            /// The beginning of T1 is coincident with the end of T2
            /// </summary>
            MetBy = 12,
            /// <summary>
            /// If a temporal entity T1 is notDisjoint with another temporal entity T2, then the the two entities overlap or coincide in some way, but their ordering relationship is not known.<br/>
            /// This relation is the complement of disjoint and is the union of equals, hasInside, in, meets, metBy, overlaps, overlappedBy
            /// </summary>
            NotDisjoint = 13,
            /// <summary>
            /// The beginning of T1 is before the beginning of T2, the end of T1 is after the beginning of T2, and the end of T1 is before the end of T2
            /// </summary>
            Overlaps = 14,
            /// <summary>
            /// The beginning of T1 is after the beginning of T2, the beginning of T1 is before the end of T2, and the end of T1 is after the end of T2
            /// </summary>
            OverlappedBy = 15,
            /// <summary>
            /// The beginning of T1 is coincident with the beginning of T2, and the end of T1 is before the end of T2
            /// </summary>
            Starts = 16,
            /// <summary>
            /// The beginning of T1 is coincident with the beginning of T2, and the end of T1 is after the end of T2
            /// </summary>
            StartedBy = 17
        }
    
        /// <summary>
        /// Represents an enumeration for the set of built-in OWL-TIME reasoner rules
        /// </summary>
        public enum TIMEReasonerRules
        {
            /// <summary>
            /// Given temporal intervals I1, I2, I3: AFTER(?I1,?I2) ^ EQUALS(?I2,?I3) -> AFTER(?I1,?I3)
            /// </summary>
            TIME_AfterEqualsEntailment = 1,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: AFTER(?I1,?I2) ^ FINISHES(?I2,?I3) -> AFTER(?I1,?I3)
            /// </summary>
            TIME_AfterFinishesEntailment = 2,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: AFTER(?I1,?I2) ^ METBY(?I2,?I3) -> AFTER(?I1,?I3)
            /// </summary>
            TIME_AfterMetByEntailment = 3,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: AFTER(?I1,?I2) ^ AFTER(?I3,?I1) -> AFTER(?I3,?I2)
            /// </summary>
            TIME_AfterTransitiveEntailment = 4,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: BEFORE(?I1,?I2) ^ EQUALS(?I2,?I3) -> BEFORE(?I1,?I3)
            /// </summary>
            TIME_BeforeEqualsEntailment = 5,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: BEFORE(?I1,?I2) ^ MEETS(?I2,?I3) -> BEFORE(?I1,?I3)
            /// </summary>
            TIME_BeforeMeetsEntailment = 6,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: BEFORE(?I1,?I2) ^ STARTS(?I2,?I3) -> BEFORE(?I1,?I3)
            /// </summary>
            TIME_BeforeStartsEntailment = 7,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: BEFORE(?I1,?I2) ^ BEFORE(?I3,?I1) -> BEFORE(?I3,?I2)
            /// </summary>
            TIME_BeforeTransitiveEntailment = 8,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: CONTAINS(?I1,?I2) ^ EQUALS(?I2,?I3) -> CONTAINS(?I1,?I3)
            /// </summary>
            TIME_ContainsEqualsEntailment = 9,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: CONTAINS(?I1,?I2) ^ CONTAINS(?I2,?I3) -> CONTAINS(?I1,?I3)
            /// </summary>
            TIME_ContainsTransitiveEntailment = 10,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: DURING(?I1,?I2) ^ EQUALS(?I2,?I3) -> DURING(?I1,?I3)
            /// </summary>
            TIME_DuringEqualsEntailment = 11,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: DURING(?I1,?I2) ^ DURING(?I2,?I3) -> DURING(?I1,?I3)
            /// </summary>
            TIME_DuringTransitiveEntailment = 12,
            /// <summary>
            /// Given temporal intervals I1, I2: STARTS(?I1,?I2) ^ FINISHES(?I1,?I2) -> EQUALS(?I1,?I2)
            /// </summary>
            TIME_EqualsEntailment = 13,
            /// <summary>
            /// Given temporal intervals I1, I2: STARTEDBY(?I1,?I2) ^ FINISHEDBY(?I1,?I2) -> EQUALS(?I1,?I2)
            /// </summary>
            TIME_EqualsInverseEntailment = 14,
            /// <summary>
            /// Given temporal intervals I1, I2: EQUALS(?I1,?I2) ^ EQUALS(?I2,?I3) -> EQUALS(?I1,?I3)
            /// </summary>
            TIME_EqualsTransitiveEntailment = 15,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: FINISHEDBY(?I1,?I2) ^ EQUALS(?I2,?I3) -> FINISHEDBY(?I1,?I3)
            /// </summary>
            TIME_FinishedByEqualsEntailment = 16,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: FINISHES(?I1,?I2) ^ EQUALS(?I2,?I3) -> FINISHES(?I1,?I3)
            /// </summary>
            TIME_FinishesEqualsEntailment = 17,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: MEETS(?I1,?I2) ^ EQUALS(?I2,?I3) -> MEETS(?I1,?I3)
            /// </summary>
            TIME_MeetsEqualsEntailment = 18,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: MEETS(?I1,?I2) ^ STARTS(?I2,?I3) -> MEETS(?I1,?I3)
            /// </summary>
            TIME_MeetsStartsEntailment = 19,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: METBY(?I1,?I2) ^ EQUALS(?I2,?I3) -> METBY(?I1,?I3)
            /// </summary>
            TIME_MetByEqualsEntailment = 20,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: OVERLAPPEDBY(?I1,?I2) ^ EQUALS(?I2,?I3) -> OVERLAPPEDBY(?I1,?I3)
            /// </summary>
            TIME_OverlappedByEqualsEntailment = 21,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: OVERLAPS(?I1,?I2) ^ EQUALS(?I2,?I3) -> OVERLAPS(?I1,?I3)
            /// </summary>
            TIME_OverlapsEqualsEntailment = 22,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: STARTS(?I1,?I2) ^ EQUALS(?I2,?I3) -> STARTS(?I1,?I3)
            /// </summary>
            TIME_StartsEqualsEntailment = 23,
            /// <summary>
            /// Given temporal intervals I1, I2, I3: STARTEDBY(?I1,?I2) ^ EQUALS(?I2,?I3) -> STARTEDBY(?I1,?I3)
            /// </summary>
            TIME_StartedByEqualsEntailment = 24
        }

        /// <summary>
        /// Represents an enumeration for the set of built-in OWL-TIME validator rules
        /// </summary>
        public enum TIMEValidatorRules
        {
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:after relations between instants
            /// </summary>
            TIME_InstantAfter = 1,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:before relations between instants
            /// </summary>
            TIME_InstantBefore = 2,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalAfter relations between intervals
            /// </summary>
            TIME_IntervalAfter = 3,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalBefore relations between intervals
            /// </summary>
            TIME_IntervalBefore = 4,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalContains relations between intervals
            /// </summary>
            TIME_IntervalContains = 5,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalDisjoint relations between intervals
            /// </summary>
            TIME_IntervalDisjoint = 6,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalDuring relations between intervals
            /// </summary>
            TIME_IntervalDuring = 7,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalEquals relations between intervals
            /// </summary>
            TIME_IntervalEquals = 8,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalFinishes relations between intervals
            /// </summary>
            TIME_IntervalFinishes = 9,
            /// <summary>
            /// This OWL-TIME rule checks for consistency of time:intervalFinishedBy relations between intervals
            /// </summary>
            TIME_IntervalFinishedBy = 10
        }
    }
}