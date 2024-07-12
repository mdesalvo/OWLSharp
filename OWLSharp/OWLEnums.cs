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
			SubClassOfEntailment = 1,
			EquivalentClassesEntailment = 2,
			DisjointClassesEntailment = 3,
            SubDataPropertyOfEntailment = 4,
			SubObjectPropertyOfEntailment = 5,
			EquivalentDataPropertiesEntailment = 6,
			EquivalentObjectPropertiesEntailment = 7,
            DisjointDataPropertiesEntailment = 8,
            DisjointObjectPropertiesEntailment = 9,
			SameIndividualEntailment = 10,
			DifferentIndividualsEntailment = 11,
            FunctionalObjectPropertyEntailment = 12,
			InverseFunctionalObjectPropertyEntailment = 13,
			InverseObjectPropertiesEntailment = 14,
            SymmetricObjectPropertyEntailment = 15,
			ReflexiveObjectPropertyEntailment = 16,
            TransitiveObjectPropertyEntailment = 17,
			ObjectPropertyChainEntailment = 18,
			HasValueEntailment = 19,
			HasSelfEntailment = 20,
			ClassAssertionEntailment = 21,
			DataPropertyDomainEntailment = 22,
			ObjectPropertyDomainEntailment = 23,
			ObjectPropertyRangeEntailment = 24
        }
    }
}