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

namespace OWLSharp
{
    /// <summary>
    /// OWLEnums represents a collector for all the enumerations used by the "OWLSharp" namespace
    /// </summary>
    public static class OWLEnums
    {
        /// <summary>
        /// OWLFormats represents an enumeration for supported OWL2 ontology serialization data formats
        /// </summary>
        public enum OWLFormats
        {
            OWL2XML = 1
        }

        /// <summary>
        /// OWLReasonerRules represents an enumeration for supported RDFS/OWL2 reasoner rules
        /// </summary>
        public enum OWLReasonerRules
        {
            ClassAssertionEntailment = 1,
            DataPropertyDomainEntailment = 2,
            DisjointClassesEntailment = 3,
            DisjointDataPropertiesEntailment = 4,
            DisjointObjectPropertiesEntailment = 5,
            EquivalentClassesEntailment = 6,
            EquivalentDataPropertiesEntailment = 7,
            EquivalentObjectPropertiesEntailment = 8,
            FunctionalObjectPropertyEntailment = 9,
            HasKeyEntailment = 10,
            HasSelfEntailment = 11,
            HasValueEntailment = 12,
            InverseFunctionalObjectPropertyEntailment = 13,
            InverseObjectPropertiesEntailment = 14,
            ObjectPropertyChainEntailment = 15,
            ObjectPropertyDomainEntailment = 16,
            ObjectPropertyRangeEntailment = 17,
            ReflexiveObjectPropertyEntailment = 18,
            SameIndividualEntailment = 19,
            SubClassOfEntailment = 20,
            SubDataPropertyOfEntailment = 21,
            SubObjectPropertyOfEntailment = 22,
            SymmetricObjectPropertyEntailment = 23,
            TransitiveObjectPropertyEntailment = 24
        }

        /// <summary>
        /// OWLValidatorRules represents an enumeration for supported RDFS/OWL2 validator rules
        /// </summary>
        public enum OWLValidatorRules
        {
            AsymmetricObjectPropertyAnalysis = 1,
            ClassAssertionAnalysis = 2,
            DataPropertyDomainAnalysis = 3,
            DataPropertyRangeAnalysis = 4,
            DifferentIndividualsAnalysis = 5,
            DisjointClassesAnalysis = 6,
            DisjointDataPropertiesAnalysis = 7,
            DisjointObjectPropertiesAnalysis = 8,
            DisjointUnionAnalysis = 9,
            EquivalentClassesAnalysis = 10,
            EquivalentDataPropertiesAnalysis = 11,
            EquivalentObjectPropertiesAnalysis = 12,
            FunctionalDataPropertyAnalysis = 13,
            FunctionalObjectPropertyAnalysis = 14,
            HasKeyAnalysis = 15,
            InverseFunctionalObjectPropertyAnalysis = 16,
            IrreflexiveObjectPropertyAnalysis = 17,
            NegativeDataAssertionsAnalysis = 18,
            NegativeObjectAssertionsAnalysis = 19,
            ObjectPropertyChainAnalysis = 20,
            ObjectPropertyDomainAnalysis = 21,
            ObjectPropertyRangeAnalysis = 22,
            SubClassOfAnalysis = 23,
            SubDataPropertyOfAnalysis = 24,
            SubObjectPropertyOfAnalysis = 25,
            TermsDeprecationAnalysis = 26,
            TermsDisjointnessAnalysis = 27,
            ThingNothingAnalysis = 28,
            TopBottomAnalysis = 29
        }

        /// <summary>
        /// OWLIssueSeverity represents an enumeration for possible severities of RDFS/OWL2 validator rules
        /// </summary>
        public enum OWLIssueSeverity
        {
            Warning = 1,
            Error = 2
        }
    }
}