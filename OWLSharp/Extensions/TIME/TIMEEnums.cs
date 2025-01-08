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

namespace OWLSharp.Extensions.TIME
{
    public static class TIMEEnums
    {
        public enum TIMEUnitType
        {
            Year = 1,
            Month = 2,
            Day = 3,
            Hour = 4,
            Minute = 5,
            Second = 6
        }

        public enum TIMEReasonerRules
        {
            AfterEqualsEntailment = 1,
            AfterFinishesEntailment = 2,
            AfterMetByEntailment = 3,
            AfterTransitiveEntailment = 4,
            BeforeEqualsEntailment = 5,
            BeforeMeetsEntailment = 6,
            BeforeStartsEntailment = 7,
            BeforeTransitiveEntailment = 8,
            ContainsEqualsEntailment = 9,
            ContainsTransitiveEntailment = 10,
            DuringEqualsEntailment = 11,
            DuringTransitiveEntailment = 12,
            EqualsEntailment = 13,
            EqualsInverseEntailment = 14,
            EqualsTransitiveEntailment = 15,
            FinishedByEqualsEntailment = 16,
            FinishesEqualsEntailment = 17,
            MeetsEqualsEntailment = 18,
            MeetsStartsEntailment = 19,
            MetByEqualsEntailment = 20,
            OverlappedByEqualsEntailment = 21,
            OverlapsEqualsEntailment = 22,
            StartsEqualsEntailment = 23,
            StartedByEqualsEntailment = 24
        }

        public enum TIMEValidatorRules
        {
            InstantAfterAnalysis = 1,
            InstantBeforeAnalysis = 2,
            IntervalAfterAnalysis = 3,
            IntervalBeforeAnalysis = 4,
            IntervalContainsAnalysis = 5,
            IntervalDisjointAnalysis = 6,
            IntervalDuringAnalysis = 7,
            IntervalEqualsAnalysis = 8,
            IntervalFinishesAnalysis = 9,
            IntervalFinishedByAnalysis = 10,
            IntervalHasInsideAnalysis = 11,
            IntervalInAnalysis = 12,
            IntervalMeetsAnalysis = 13,
            IntervalMetByAnalysis = 14,
            IntervalNotDisjointAnalysis = 15,
            IntervalOverlapsAnalysis = 16,
            IntervalOverlappedByAnalysis = 17,
            IntervalStartsAnalysis = 18,
            IntervalStartedByAnalysis = 19
        }
    }
}