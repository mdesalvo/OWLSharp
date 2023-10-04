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

namespace OWLSharp
{
    /// <summary>
    /// OWLEnums represents a collector for all the enumerations used by the "OWLSharp" namespace
    /// </summary>
    public static class OWLEnums
    {
        /// <summary>
        /// Represents an enumeration for possible categories of ontology validator evidence
        /// </summary>
        public enum OWLValidatorEvidenceCategory
        {
            /// <summary>
            /// Specifications have not been violated: ontology may contain semantic inconsistencies
            /// </summary>
            Warning = 1,
            /// <summary>
            /// Specifications have been violated: ontology will contain semantic inconsistencies
            /// </summary>
            Error = 2
        };

        /// <summary>
        /// Represents an enumeration for the set of built-in standard RDFS/OWL-DL/OWL2 validator rules
        /// </summary>
        public enum OWLValidatorStandardRules
        {
            /// <summary>
            /// This OWL-DL rule checks for vocabulary disjointness of classes, properties and individuals
            /// </summary>
            TermDisjointness = 1,
            /// <summary>
            /// This OWL-DL rule checks for explicit declaration of classes, properties and individuals
            /// </summary>
            TermDeclaration = 2,
            /// <summary>
            /// This OWL-DL rule checks for usage of deprecated classes and properties
            /// </summary>
            TermDeprecation = 3,
            /// <summary>
            /// This RDFS rule checks for consistency of rdfs:domain and rdfs:range knowledge
            /// </summary>
            DomainRange = 4,
            /// <summary>
            /// This OWL-DL rule checks for consistency of owl:inverseOf knowledge
            /// </summary>
            InverseOf = 5,
            /// <summary>
            /// This OWL-DL rule checks for consistency of owl:SymmetricProperty knowledge
            /// </summary>
            SymmetricProperty = 6,
            /// <summary>
            /// This OWL-DL rule checks for consistency of rdf:type knowledge
            /// </summary>
            ClassType = 7,
            /// <summary>
            /// This OWL2 rule checks for consistency of owl:AsymmetricProperty knowledge
            /// </summary>
            AsymmetricProperty = 8,
            /// <summary>
            /// This OWL2 rule checks for consistency of owl:IrreflexiveProperty knowledge
            /// </summary>
            IrreflexiveProperty = 9,
            /// <summary>
            /// This OWL2 rule checks for consistency of owl:propertyDisjointWith knowledge
            /// </summary>
            PropertyDisjoint = 10,
            /// <summary>
            /// This OWL2 rule checks for consistency of owl:hasKey knowledge
            /// </summary>
            ClassKey = 11,
            /// <summary>
            /// This OWL2 rule checks for consistency of owl:propertyChainAxiom knowledge
            /// </summary>
            PropertyChainAxiom = 12,           
            /// <summary>
            /// This OWL2 rule checks for consistency of owl:NegativePropertyAssertion knowledge
            /// </summary>
            NegativeAssertions = 13,
            /// <summary>
            /// This OWL-DL rule checks for consistency of global cardinality constraints
            /// </summary>
            GlobalCardinality = 14,
            /// <summary>
            /// This OWL-DL rule checks for consistency of local cardinality constraints
            /// </summary>
            LocalCardinality = 15,
            /// <summary>
            /// This OWL-DL rule checks for consistency of object and datatype properties
            /// </summary>
            PropertyConsistency = 16,
            /// <summary>
            /// This OWL2 rule checks for consistency of owl:disjointUnionOf knowledge
            /// </summary>
            DisjointUnion = 17
        };

        /// <summary>
        /// Represents an enumeration for possible categories of ontology reasoner evidence
        /// </summary>
        public enum OWLReasonerEvidenceCategory
        {
            /// <summary>
            /// Inference generated on class model knowledge (T-BOX)
            /// </summary>
            ClassModel = 1,
            /// <summary>
            /// Inference generated on property model knowledge (T-BOX)
            /// </summary>
            PropertyModel = 2,
            /// <summary>
            /// Inference generated on data knowledge (A-BOX)
            /// </summary>
            Data = 3
        };

        /// <summary>
        /// Represents an enumeration for the set of built-in standard RDFS/OWL-DL/OWL2 reasoner rules
        /// </summary>
        public enum OWLReasonerStandardRules
        {
            /// <summary>
            /// This OWL-DL rule targets class model knowledge (T-BOX) to reason over rdfs:subClassOf hierarchy
            /// </summary>
            SubClassTransitivity = 1,
            /// <summary>
            /// This OWL-DL rule targets property model knowledge (T-BOX) to reason over rdfs:subPropertyOf hierarchy
            /// </summary>
            SubPropertyTransitivity = 2,
            /// <summary>
            /// This OWL-DL rule targets class model knowledge (T-BOX) to reason over owl:equivalentClass relations
            /// </summary>
            EquivalentClassTransitivity = 3,
            /// <summary>
            /// This OWL-DL rule targets property model knowledge (T-BOX) to reason over owl:equivalentProperty relations
            /// </summary>
            EquivalentPropertyTransitivity = 4,
            /// <summary>
            /// This OWL-DL rule targets class model knowledge (T-BOX) to reason over owl:disjointWith relations
            /// </summary>
            DisjointClassEntailment = 5,
            /// <summary>
            /// This OWL2 rule targets property model knowledge (T-BOX) to reason over owl:propertyDisjointWith relations
            /// </summary>
            DisjointPropertyEntailment = 6,
            /// <summary>
            /// This RDFS rule targets data knowledge (A-BOX) to infer individual types from rdfs:domain relations
            /// </summary>
            DomainEntailment = 7,
            /// <summary>
            /// This RDFS rule targets data knowledge (A-BOX) to infer individual types from rdfs:range relations
            /// </summary>
            RangeEntailment = 8,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to reason over owl:sameAs relations
            /// </summary>
            SameAsTransitivity = 9,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to reason over owl:differentFrom relations
            /// </summary>
            DifferentFromEntailment = 10,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to reason over rdf:type relations
            /// </summary>
            IndividualTypeEntailment = 11,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to reason over symmetric object assertions
            /// </summary>
            SymmetricPropertyEntailment = 12,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to reason over transitive object assertions
            /// </summary>
            TransitivePropertyEntailment = 13,
            /// <summary>
            /// This OWL2 rule targets data knowledge (A-BOX) to reason over reflexive object assertions
            /// </summary>
            ReflexivePropertyEntailment = 14,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to reason over inverse object assertions
            /// </summary>
            InverseOfEntailment = 15,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to infer assertions from rdfs:subPropertyOf and owl:equivalentProperty hierarchies
            /// </summary>
            PropertyEntailment = 16,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to infer assertions from owl:sameAs hierarchy
            /// </summary>
            SameAsEntailment = 17,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to infer assertions from owl:hasValue restrictions
            /// </summary>
            HasValueEntailment = 18,
            /// <summary>
            /// This OWL2 rule targets data knowledge (A-BOX) to infer assertions from owl:hasSelf restrictions
            /// </summary>
            HasSelfEntailment = 19,
            /// <summary>
            /// This OWL2 rule targets data knowledge (A-BOX) to infer owl:sameAs relations from owl:hasKey relations
            /// </summary>
            HasKeyEntailment = 20,
            /// <summary>
            /// This OWL2 rule targets data knowledge (A-BOX) to infer assertions from owl:propertyChainAxiom relations
            /// </summary>
            PropertyChainEntailment = 21,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to infer owl:sameAs relations from assertions having functional object properties
            /// </summary>
            FunctionalEntailment = 22,
            /// <summary>
            /// This OWL-DL rule targets data knowledge (A-BOX) to infer owl:sameAs relations from assertions having inverse functional properties
            /// </summary>
            InverseFunctionalEntailment = 23
        };

        /// <summary>
        /// Represents an enumeration for supported types of ontology property
        /// </summary>
        public enum OWLPropertyType
        {
            /// <summary>
            /// owl:AnnotationProperty
            /// </summary>
            Annotation = 1,
            /// <summary>
            /// owl:DatatypeProperty
            /// </summary>
            Datatype = 2,
            /// <summary>
            /// owl:ObjectProperty
            /// </summary>
            Object = 3
        }

        /// <summary>
        /// Represents an enumeration for supported OWL ontology serialization formats
        /// </summary>
        public enum OWLFormats
        {
            /// <summary>
            /// XML serialization
            /// </summary>
            OwlXml = 0
        };
    }
}