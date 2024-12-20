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

        public enum TIMEInstantRelation
        {
            After = 1,
            Before = 2
        }

        public enum TIMEIntervalRelation
        {
            After = 1,
            Before = 2,
            Contains = 3,
            Disjoint = 4,
            During = 5,
            Equals = 6,
            Finishes = 7,
            FinishedBy = 8,
            HasInside = 9,
            In = 10,
            Meets = 11,
            MetBy = 12,
            NotDisjoint = 13,
            Overlaps = 14,
            OverlappedBy = 15,
            Starts = 16,
            StartedBy = 17
        }
    }
}