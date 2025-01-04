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

namespace OWLSharp.Extensions.SKOS
{
    public static class SKOSEnums
    {
        public enum SKOSValidatorRules
        {
            AlternativeLabelAnalysis = 1,
            HiddenLabelAnalysis = 2,
            PreferredLabelAnalysis = 3,
            NotationAnalysis = 4,
            BroaderConceptAnalysis = 5,
            NarrowerConceptAnalysis = 6,
            CloseOrExactMatchConceptAnalysis = 7,
            RelatedConceptAnalysis = 8,
            LiteralFormAnalysis = 9
        }
    }
}