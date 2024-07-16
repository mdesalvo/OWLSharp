﻿/*
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

namespace OWLSharp
{
    public static class OWLEnums
    {
        public enum OWLFormats
        {
            OWL2XML = 1
        };

		public enum OWLReasonerRules
		{
            ClassAssertionEntailment = 1,
            DataPropertyDomainEntailment = 2,
            DifferentIndividualsEntailment = 3,
            DisjointClassesEntailment = 4,
            DisjointDataPropertiesEntailment = 5,
            DisjointObjectPropertiesEntailment = 6,
            EquivalentClassesEntailment = 7,
            EquivalentDataPropertiesEntailment = 8,
            EquivalentObjectPropertiesEntailment = 9,
            FunctionalObjectPropertyEntailment = 10,
            HasKeyEntailment = 11,
            HasSelfEntailment = 12,
            HasValueEntailment = 13,
            InverseFunctionalObjectPropertyEntailment = 14,
            InverseObjectPropertiesEntailment = 15,
            ObjectPropertyChainEntailment = 16,
            ObjectPropertyDomainEntailment = 17,
            ObjectPropertyRangeEntailment = 18,
            ReflexiveObjectPropertyEntailment = 19,
            SameIndividualEntailment = 20,
            SubClassOfEntailment = 21,
            SubDataPropertyOfEntailment = 22,
            SubObjectPropertyOfEntailment = 23,
            SymmetricObjectPropertyEntailment = 24,
            TransitiveObjectPropertyEntailment = 25
        }
    }
}