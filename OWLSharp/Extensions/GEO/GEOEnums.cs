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

namespace OWLSharp.Extensions.GEO
{
    /// <summary>
    /// GEOEnums represents a collector for all the enumerations used by the "OWLSharp.Extensions.GEO" namespace
    /// </summary>
    public static class GEOEnums
    {
        /// <summary>
        /// Represents an enumeration for the set of built-in Egenhofer spatial relations
        /// </summary>
        public enum GEOEgenhoferRelations
        {
            /// <summary>
            /// geof:ehEquals
            /// </summary>
            Equals = 1,
            /// <summary>
            /// geof:ehDisjoint
            /// </summary>
            Disjoint = 2,
            /// <summary>
            /// geof:ehMeet
            /// </summary>
            Meet = 3,
            /// <summary>
            /// geof:ehOverlap
            /// </summary>
            Overlap = 4,
            /// <summary>
            /// geof:ehCovers
            /// </summary>
            Covers = 5,
            /// <summary>
            /// geof:ehCoveredBy
            /// </summary>
            CoveredBy = 6,
            /// <summary>
            /// geof:ehInside
            /// </summary>
            Inside = 7,
            /// <summary>
            /// geof:ehContains
            /// </summary>
            Contains = 8
        };

        /// <summary>
        /// Represents an enumeration for the set of built-in RCC8 spatial relations
        /// </summary>
        public enum GEORCC8Relations
        {
            /// <summary>
            /// geosf:rcc8dc    -> geof:relate("FFTFFTTTT")
            /// </summary>
            RCC8DC = 1,
            /// <summary>
            /// geosf:rcc8dc    -> geof:relate("FFTFTTTTT")
            /// </summary>
            RCC8EC = 2,
            /// <summary>
            /// geosf:rcc8po    -> geof:relate("TTTTTTTTT")
            /// </summary>
            RCC8PO = 3,
            /// <summary>
            /// geosf:rcc8tppi  -> geof:relate("TTTFTTFFT")
            /// </summary>
            RCC8TPPI = 4,
            /// <summary>
            /// geosf:rcc8tpp   -> geof:relate("TFFTTFTTT")
            /// </summary>
            RCC8TPP = 5,
            /// <summary>
            /// geosf:rcc8ntpp  -> geof:relate("TFFTFFTTT")
            /// </summary>
            RCC8NTPP = 6,
            /// <summary>
            /// geosf:rcc8ntppi -> geof:relate("TTTFFTFFT")
            /// </summary>
            RCC8NTPPI = 7,
            /// <summary>
            /// geosf:rcc8eq    -> geof:relate("TFFFTFFFT")
            /// </summary>
            RCC8EQ = 8
        }
    }
}