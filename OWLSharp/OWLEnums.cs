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
            /// <summary>
            /// OWL2/XML format (https://www.w3.org/TR/owl2-xml-serialization/)
            /// </summary>
            OWL2XML = 1
        }

        /// <summary>
        /// OWLReasonerRules represents an enumeration for supported RDFS/OWL2 reasoner rules
        /// </summary>
        public enum OWLReasonerRules
        {
            /// <summary>
            /// ClassAssertion(C1,I) ^ SubClassOf(C1,C2) -> ClassAssertion(C2,I)<br/>
            /// ClassAssertion(C1,I) ^ EquivalentClasses(C1,C2) -> ClassAssertion(C2,I)
            /// </summary>
            ClassAssertionEntailment = 1,
            /// <summary>
            /// DataPropertyDomain(DP,C) ^ DataPropertyAssertion(DP, I, LIT) -> ClassAssertion(C,I)
            /// </summary>
            DataPropertyDomainEntailment = 2,
            /// <summary>
            /// AllDifferent(I1,I2,...IN) -> DifferentIndividuals(I1,I2) ^ DifferentIndividuals(I1,IN) ^ ...
            /// </summary>
            DifferentIndividualsEntailment = 3,
            /// <summary>
            /// EquivalentClasses(C1,C2) ^ DisjointClasses(C2,C3) -> DisjointClasses(C1,C3)<br/>
            /// SubClassOf(C1,C2) ^ DisjointClasses(C2,C3) -> DisjointClasses(C1,C3)<br/>
            /// DisjointUnion(C1,(C2 C3)) -> DisjointClasses(C2,C3)<br/>
            /// DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
            /// </summary>
            DisjointClassesEntailment = 4,
            /// <summary>
            /// AllDisjointProperties(DP1,DP2,...DPN) -> DisjointDataProperties(DP1,DP2) ^ DisjointDataProperties(DP1,DPN) ^ ...
            /// </summary>
            DisjointDataPropertiesEntailment = 5,
            /// <summary>
            /// AllDisjointProperties(OP1,OP2,...OPN) -> DisjointObjectProperties(OP1,OP2) ^ DisjointObjectProperties(OP1,OPN) ^ ...
            /// </summary>
            DisjointObjectPropertiesEntailment = 6,
            /// <summary>
            /// EquivalentClasses(C1,C2) ^ EquivalentClasses(C2,C3) -> EquivalentClasses(C1,C3)
            /// </summary>
            EquivalentClassesEntailment = 7,
            /// <summary>
            /// EquivalentDataProperties(P1,P2) ^ EquivalentDataProperties(P2,P3) -> EquivalentDataProperties(P1,P3)<br/>
            /// EquivalentDataProperties(P1,P2) ^ DataPropertyAssertion(P1,I,LIT) -> DataPropertyAssertion(P2,I,LIT)
            /// </summary>
            EquivalentDataPropertiesEntailment = 8,
            /// <summary>
            /// EquivalentObjectProperties(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> EquivalentObjectProperties(P1,P3)<br/>
            /// EquivalentObjectProperties(P1,P2) ^ ObjectPropertyAssertion(P1,I1,I2) -> ObjectPropertyAssertion(P2,I1,I2)
            /// </summary>
            EquivalentObjectPropertiesEntailment = 9,
            /// <summary>
            /// FunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDVX,IDV1) ^ ObjectPropertyAssertion(OP,IDVX,IDV2) -> SameIndividual(IDV1,IDV2)
            /// </summary>
            FunctionalObjectPropertyEntailment = 10,
            /// <summary>
            /// HasKey(C,OP) ^ ClassAssertion(C,I1) ^ ObjectPropertyAssertion(OP,I1,IX) ^ ClassAssertion(C,I2) ^ ObjectPropertyAssertion(OP,I2,IX) -> SameIndividual(I1,I2)<br/>
            /// HasKey(C,DP) ^ ClassAssertion(C,I1) ^ DataPropertyAssertion(DP,I1,LIT)  ^ ClassAssertion(C,I2) ^ DataPropertyAssertion(DP,I2,LIT)  -> SameIndividual(I1,I2)
            /// </summary>
            HasKeyEntailment = 11,
            /// <summary>
            /// SubClassOf(C,ObjectHasSelf(OP)) ^ ClassAssertion(C,I) -> ObjectPropertyAssertion(OP,I,I)
            /// </summary>
            HasSelfEntailment = 12,
            /// <summary>
            /// SubClassOf(C,ObjectHasValue(OP,I2)) ^ ClassAssertion(C,I1) -> ObjectPropertyAssertion(OP,I1,I2)<br/>
            /// SubClassOf(C,DataHasValue(DP,LIT)) ^ ClassAssertion(C,I) -> DataPropertyAssertion(DP,I,LIT)
            /// </summary>
            HasValueEntailment = 13,
            /// <summary>
            /// InverseFunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDVX) ^ ObjectPropertyAssertion(OP,IDV2,IDVX) -> SameIndividual(IDV1,IDV2)
            /// </summary>
            InverseFunctionalObjectPropertyEntailment = 14,
            /// <summary>
            /// InverseObjectProperties(OP,IOP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(IOP,IDV2,IDV1)<br/>
            /// InverseObjectProperties(OP,IOP) ^ EquivalentObjectProperties(IOP,IOP2)  -> InverseObjectProperties(OP,IOP2)<br/>
            /// InverseObjectProperties(OP,IOP) ^ ObjectPropertyDomain(OP,C) -> ObjectPropertyRange(IOP,C)<br/>
            /// InverseObjectProperties(OP,IOP) ^ ObjectPropertyRange(OP,C)  -> ObjectPropertyDomain(IOP,C)
            /// </summary>
            InverseObjectPropertiesEntailment = 15,
            /// <summary>
            /// ObjectPropertyChain(PC,(OP1..OPN)) ^ SubObjectPropertyOf(PC,OP) -> ObjectPropertyAssertion(OP,OP1,OPN)
            /// </summary>
            ObjectPropertyChainEntailment = 16,
            /// <summary>
            /// ObjectPropertyDomain(OP,C) ^ ObjectPropertyAssertion(OP,I1,I2) -> ClassAssertion(C,I1)
            /// </summary>
            ObjectPropertyDomainEntailment = 17,
            /// <summary>
            /// ObjectPropertyRange(OP,C) ^ ObjectPropertyAssertion(OP,I1,I2) -> ClassAssertion(C,I2)
            /// </summary>
            ObjectPropertyRangeEntailment = 18,
            /// <summary>
            /// ReflexiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(OP,IDV1,IDV1)
            /// </summary>
            ReflexiveObjectPropertyEntailment = 19,
            /// <summary>
            /// SameIndividual(I1,I2) ^ SameIndividual(I2,I3) -> SameIndividual(I1,I3)<br/>
            /// SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I1,I3) -> ObjectPropertyAssertion(OP,I2,I3)<br/>
            /// SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I2,I3) -> ObjectPropertyAssertion(OP,I1,I3)
            /// </summary>
            SameIndividualEntailment = 20,
            /// <summary>
            /// SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)<br/>
            /// SubClassOf(C1,C2) ^ EquivalentClasses(C2,C3) -> SubClassOf(C1,C3)<br/>
            /// EquivalentClasses(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)<br/>
            /// DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
            /// </summary>
            SubClassOfEntailment = 21,
            /// <summary>
            /// SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)<br/>
            /// SubDataPropertyOf(P1,P2) ^ EquivalentDataProperties(P2,P3) -> SubDataPropertyOf(P1,P3)<br/>
            /// SubDataPropertyOf(P1,P2) ^ DataPropertyAssertion(P1,I,LIT) -> DataPropertyAssertion(P2,I,LIT)
            /// </summary>
            SubDataPropertyOfEntailment = 22,
            /// <summary>
            /// SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)<br/>
            /// SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)<br/>
            /// SubObjectPropertyOf(P1,P2) ^ ObjectPropertyAssertion(P1,I1,I2) -> ObjectPropertyAssertion(P2,I1,I2)
            /// </summary>
            SubObjectPropertyOfEntailment = 23,
            /// <summary>
            /// SymmetricObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(OP,IDV2,IDV1)
            /// </summary>
            SymmetricObjectPropertyEntailment = 24,
            /// <summary>
            /// TransitiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV3) -> ObjectPropertyAssertion(OP,IDV1,IDV3)
            /// </summary>
            TransitiveObjectPropertyEntailment = 25
        }

        /// <summary>
        /// OWLValidatorRules represents an enumeration for supported RDFS/OWL2 validator rules
        /// </summary>
        public enum OWLValidatorRules
        {
            /// <summary>
            /// AsymmetricObjectProperty(OP) ^ SymmetricObjectProperty(OP) -> ERROR<br/>
            /// AsymmetricObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I2,I1) -> ERROR
            /// </summary>
            AsymmetricObjectPropertyAnalysis = 1,
            /// <summary>
            /// ClassAssertion(C,I) ^ ClassAssertion(ObjectComplementOf(C),I) -> ERROR
            /// </summary>
            ClassAssertionAnalysis = 2,
            /// <summary>
            /// DataPropertyAssertion(DP,I,LIT) ^ DataPropertyDomain(DP,C) ^ ClassAssertion(ObjectComplementOf(C),I) -> ERROR
            /// </summary>
            DataPropertyDomainAnalysis = 3,
            /// <summary>
            /// DataPropertyAssertion(DP,I,LIT) ^ DataPropertyRange(DP,DR) ^ swrl:equal(Datatype(LIT),DataComplementOf(DR)) -> ERROR
            /// </summary>
            DataPropertyRangeAnalysis = 4,
            /// <summary>
            /// DifferentIndividuals(I1,I2) ^ SameIndividual(I2,I1) -> ERROR<br/>
            /// DifferentIndividuals(I,I) -> ERROR
            /// </summary>
            DifferentIndividualsAnalysis = 5,
            /// <summary>
            /// DisjointClasses(C1,C2) ^ SubClassOf(C1,C2) -> ERROR<br/>
            /// DisjointClasses(C1,C2) ^ SubClassOf(C2,C1) -> ERROR<br/>
            /// DisjointClasses(C1,C2) ^ EquivalentClasses(C1,C2) -> ERROR<br/>
            /// DisjointClasses(C1,C2) ^ ClassAssertion(C1,I) ^ ClassAssertion(C2,I) -> ERROR
            /// </summary>
            DisjointClassesAnalysis = 6,
            /// <summary>
            /// DisjointDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP1,DP2) -> ERROR<br/>
            /// DisjointDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP2,DP1) -> ERROR<br/>
            /// DisjointDataProperties(DP1,DP2) ^ EquivalentDataProperties(DP1,DP2) -> ERROR<br/>
            /// DisjointDataProperties(DP1,DP2) ^ DataPropertyAssertion(DP1,I,LIT) ^ DataPropertyAssertion(DP2,I,LIT) -> ERROR
            /// </summary>
            DisjointDataPropertiesAnalysis = 7,
            /// <summary>
            /// DisjointObjectProperties(OP1,OP2) ^ SubDataPropertyOf(OP1,OP2) -> ERROR<br/>
            /// DisjointObjectProperties(OP1,OP2) ^ SubDataPropertyOf(OP2,OP1) -> ERROR<br/>
            /// DisjointObjectProperties(OP1,OP2) ^ EquivalentDataProperties(OP1,OP2) -> ERROR<br/>
            /// DisjointObjectProperties(OP1,OP2) ^ ObjectPropertyAssertion(OP1,I1,I2) ^ ObjectPropertyAssertion(OP2,I1,I2) -> ERROR
            /// </summary>
            DisjointObjectPropertiesAnalysis = 8,
            /// <summary>
            /// DisjointUnion(C,(C1,C2)) ^ ClassAssertion(C1,I) ^ ClassAssertion(C2,I) -> ERROR
            /// </summary>
            DisjointUnionAnalysis = 9,
            /// <summary>
            /// EquivalentClasses(C1,C2) ^ SubClassOf(C1,C2) -> ERROR<br/>
            /// EquivalentClasses(C1,C2) ^ SubClassOf(C2,C1) -> ERROR<br/>
            /// EquivalentClasses(C1,C2) ^ DisjointClasses(C1,C2) -> ERROR
            /// </summary>
            EquivalentClassesAnalysis = 10,
            /// <summary>
            /// EquivalentDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP1,DP2) -> ERROR<br/>
            /// EquivalentDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP2,DP1) -> ERROR<br/>
            /// EquivalentDataProperties(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> ERROR
            /// </summary>
            EquivalentDataPropertiesAnalysis = 11,
            /// <summary>
            /// EquivalentObjectProperties(OP1,OP2) ^ SubObjectPropertyOf(OP1,OP2) -> ERROR<br/>
            /// EquivalentObjectProperties(OP1,OP2) ^ SubObjectPropertyOf(OP2,OP1) -> ERROR<br/>
            /// EquivalentObjectProperties(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> ERROR
            /// </summary>
            EquivalentObjectPropertiesAnalysis = 12,
            /// <summary>
            /// FunctionalDataProperty(DP) ^ DataPropertyAssertion(DP,I,LIT1) ^ DataPropertyAssertion(DP,I,LIT2) -> ERROR
            /// </summary>
            FunctionalDataPropertyAnalysis = 13,
            /// <summary>
            /// FunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I1,I3) ^ DifferentIndividuals(I2,I3) -> ERROR<br/>
            /// FunctionalObjectProperty(OP) ^ TransitiveObjectProperty(OP) -> ERROR<br/>
            /// FunctionalObjectProperty(OP) ^ SubObjectPropertyOf(OP,OP2) ^ TransitiveObjectProperty(OP2) -> ERROR
            /// </summary>
            FunctionalObjectPropertyAnalysis = 14,
            /// <summary>
            /// HasKey(C,OP) ^ ClassAssertion(C,I1) ^ ObjectPropertyAssertion(OP,I1,IX) ^ ClassAssertion(C,I2) ^ ObjectPropertyAssertion(OP,I2,IX) ^ DifferentIndividuals(I1,I2) -> ERROR<br/>
            /// HasKey(C,DP) ^ ClassAssertion(C,I1) ^ DataPropertyAssertion(DP,I1,LIT)  ^ ClassAssertion(C,I2) ^ DataPropertyAssertion(DP,I2,LIT)  ^ DifferentIndividuals(I1,I2) -> ERROR
            /// </summary>
            HasKeyAnalysis = 15,
            /// <summary>
            /// InverseFunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I3,I2) ^ DifferentIndividuals(I1,I3) -> ERROR<br/>
            /// InverseFunctionalObjectProperty(OP) ^ TransitiveObjectProperty(OP) -> ERROR<br/>
            /// InverseFunctionalObjectProperty(OP) ^ SubObjectPropertyOf(OP,OP2) ^ TransitiveObjectProperty(OP2) -> ERROR
            /// </summary>
            InverseFunctionalObjectPropertyAnalysis = 16,
            /// <summary>
            /// IrreflexiveObjectProperty(OP) ^ ReflexiveObjectProperty(OP) -> ERROR<br/>
            /// IrreflexiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I,I) -> ERROR
            /// </summary>
            IrreflexiveObjectPropertyAnalysis = 17,
            /// <summary>
            /// NegativeDataPropertyAssertion(DP,I,LIT) ^ DataPropertyAssertion(DP,I,LIT) -> ERROR
            /// </summary>
            NegativeDataAssertionsAnalysis = 18,
            /// <summary>
            /// NegativeObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I1,I2) -> ERROR
            /// </summary>
            NegativeObjectAssertionsAnalysis = 19,
            /// <summary>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ AsymmetricObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ FunctionalObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ InverseFunctionalObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ IrreflexiveObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP1) -> ERROR
            /// </summary>
            ObjectPropertyChainAnalysis = 20,
            /// <summary>
            /// ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyDomain(OP,C) ^ ClassAssertion(ObjectComplementOf(C),I1) -> ERROR
            /// </summary>
            ObjectPropertyDomainAnalysis = 21,
            /// <summary>
            /// ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyRange(OP,C) ^ ClassAssertion(ObjectComplementOf(C),I2) -> ERROR
            /// </summary>
            ObjectPropertyRangeAnalysis = 22,
            /// <summary>
            /// SubClassOf(C1,C2) ^ SubClassOf(C2,C1) -> ERROR<br/>
            /// SubClassOf(C1,C2) ^ EquivalentClasses(C1,C2) -> ERROR<br/>
            /// SubClassOf(C1,C2) ^ DisjointClasses(C1,C2) -> ERROR<br/>
            /// SubClassOf(C,[Object|Data][Exact|Max]Cardinality(P,N) -> ERROR (ON ASSERTION'S CARDINALITY VIOLATION)
            /// </summary>
            SubClassOfAnalysis = 23,
            /// <summary>
            /// SubDataPropertyOf(DP1,DP2) ^ SubDataPropertyOf(DP2,DP1) -> ERROR<br/>
            /// SubDataPropertyOf(DP1,DP2) ^ EquivalentDataProperties(DP1,DP2) -> ERROR<br/>
            /// SubDataPropertyOf(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> ERROR
            /// </summary>
            SubDataPropertyOfAnalysis = 24,
            /// <summary>
            /// SubObjectPropertyOf(OP1,OP2) ^ SubObjectPropertyOf(OP2,OP1) -> ERROR<br/>
            /// SubObjectPropertyOf(OP1,OP2) ^ EquivalentObjectProperties(OP1,OP2) -> ERROR<br/>
            /// SubObjectPropertyOf(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> ERROR
            /// </summary>
            SubObjectPropertyOfAnalysis = 25,
            /// <summary>
            /// Class(C) ^ AnnotationAssertion(owl:deprecated,C,true) -> WARNING<br/>
            /// Datatype(D) ^ AnnotationAssertion(owl:deprecated,D,true) -> WARNING<br/>
            /// DataProperty(DP) ^ AnnotationAssertion(owl:deprecated,DP,true) -> WARNING<br/>
            /// ObjectProperty(OP) ^ AnnotationAssertion(owl:deprecated,OP,true) -> WARNING<br/>
            /// AnnotationProperty(AP) ^ AnnotationAssertion(owl:deprecated,AP,true) -> WARNING
            /// </summary>
            TermsDeprecationAnalysis = 26,
            /// <summary>
            /// Class(C) ^ Datatype(C) -> WARNING<br/>
            /// Class(C) ^ DataProperty(C) -> WARNING<br/>
            /// Class(C) ^ ObjectProperty(C) -> WARNING<br/>
            /// Class(C) ^ AnnotationProperty(C) -> WARNING<br/>
            /// Class(C) ^ NamedIndividual(C) -> WARNING<br/>
            /// DataProperty(DP) ^ Datatype(DP) -> WARNING<br/>
            /// DataProperty(DP) ^ ObjectProperty(DP) -> WARNING<br/>
            /// DataProperty(DP) ^ AnnotationProperty(DP) -> WARNING<br/>
            /// DataProperty(DP) ^ NamedIndividual(DP) -> WARNING<br/>
            /// ObjectProperty(OP) ^ Datatype(OP) -> WARNING<br/>
            /// ObjectProperty(OP) ^ AnnotationProperty(OP) -> WARNING<br/>
            /// ObjectProperty(DP) ^ NamedIndividual(OP) -> WARNING<br/>
            /// AnnotationProperty(AP) ^ Datatype(AP) -> WARNING<br/>
            /// AnnotationProperty(AP) ^ NamedIndividual(AP) -> WARNING<br/>
            /// Datatype(D) ^ NamedIndividual(D) -> WARNING
            /// </summary>
            TermsDisjointnessAnalysis = 27,
            /// <summary>
            /// Class(C) ^ SubClassOf(owl:Thing,C) -> WARNING<br/>
            /// Class(C) ^ SubClassOf(C, owl:Nothing) -> WARNING<br/>
            /// ClassAssertion(owl:Nothing, I) -> WARNING
            /// </summary>
            ThingNothingAnalysis = 28,
            /// <summary>
            /// ObjectProperty(OP) ^ SubObjectPropertyOf(owl:topObjectProperty,OP) -> WARNING<br/>
            /// ObjectProperty(OP) ^ SubObjectPropertyOf(OP, owl:bottomObjectProperty) -> WARNING<br/>
            /// DataProperty(DP) ^ SubDataPropertyOf(owl:topDataProperty,DP) -> WARNING<br/>
            /// DataProperty(DP) ^ SubDataPropertyOf(DP, owl:bottomDataProperty) -> WARNING
            /// </summary>
            TopBottomAnalysis = 29
        }

        /// <summary>
        /// OWLIssueSeverity represents an enumeration for possible severities of RDFS/OWL2 validator rules
        /// </summary>
        public enum OWLIssueSeverity
        {
            /// <summary>
            /// States that the issue does not represent an error, but should be addressed for the sake of ontology integrity
            /// </summary>
            Warning = 1,
            /// <summary>
            /// States that the issue represents an error, indicating that the ontology integrity has been tampered
            /// </summary>
            Error = 2
        }
    }
}