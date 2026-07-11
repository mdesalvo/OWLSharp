/*
   Copyright 2014-2026 Marco De Salvo

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
            OWL2XML = 1,
            /// <summary>
            /// OWL2/Manchester format (https://www.w3.org/TR/owl2-manchester-syntax/)
            /// </summary>
            OWL2Manchester = 2,
            /// <summary>
            /// OWL2/Functional-Style format (https://www.w3.org/TR/owl2-syntax/#Functional-Style_Syntax)
            /// </summary>
            OWL2Functional = 3
        }

        /// <summary>
        /// OWLReasonerRules represents an enumeration for supported RDFS/OWL2 reasoner rules, organized in two tiers:
        /// <b>Schema</b> (T-Box -> T-Box, no individuals involved) and <b>Fact</b> (propagation at the assertion/individual level).
        /// </summary>
        public enum OWLReasonerRules
        {
            /// <summary>
            /// Class(C) [declared] -> SubClassOf(C,C) ^ EquivalentClasses(C,C) ^ SubClassOf(C,owl:Thing) ^ SubClassOf(owl:Nothing,C)
            /// <para>W3C OWL2 RL/RDF: scm-cls</para><para>Tier: Schema</para>
            /// </summary>
            SchemaClassEntailment = 1,
            /// <summary>
            /// EquivalentClasses(C1,C2) ^ DisjointClasses(C2,C3) -> DisjointClasses(C1,C3)<br/>
            /// SubClassOf(C1,C2) ^ DisjointClasses(C2,C3) -> DisjointClasses(C1,C3)<br/>
            /// DisjointUnion(C1,(C2 C3)) -> DisjointClasses(C2,C3)<br/>
            /// DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
            /// <para>OWLSharp extension: T-Box propagation of class disjointness via SubClassOf/EquivalentClasses/DisjointUnion (not part of the RL/RDF entailment closure)</para><para>Tier: Schema</para>
            /// </summary>
            SchemaDisjointClassesEntailment = 2,
            /// <summary>
            /// AllDisjointProperties(DP1,DP2,...DPN) -> DisjointDataProperties(DP1,DP2) ^ DisjointDataProperties(DP1,DPN) ^ ...
            /// <para>OWLSharp extension: expands n-ary AllDisjointProperties into pairwise DisjointDataProperties (feeds prp-adp downstream, but is not itself a table rule)</para><para>Tier: Schema</para>
            /// </summary>
            SchemaDisjointDataPropertiesEntailment = 3,
            /// <summary>
            /// AllDisjointProperties(OP1,OP2,...OPN) -> DisjointObjectProperties(OP1,OP2) ^ DisjointObjectProperties(OP1,OPN) ^ ...
            /// <para>OWLSharp extension: expands n-ary AllDisjointProperties into pairwise DisjointObjectProperties (feeds prp-adp downstream, but is not itself a table rule)</para><para>Tier: Schema</para>
            /// </summary>
            SchemaDisjointObjectPropertiesEntailment = 4,
            /// <summary>
            /// EquivalentClasses(C1,C2) ^ EquivalentClasses(C2,C3) -> EquivalentClasses(C1,C3)
            /// <para>W3C OWL2 RL/RDF: scm-eqc1, scm-eqc2</para><para>Tier: Schema</para>
            /// </summary>
            SchemaEquivalentClassesEntailment = 5,
            /// <summary>
            /// EquivalentDataProperties(P1,P2) ^ EquivalentDataProperties(P2,P3) -> EquivalentDataProperties(P1,P3)
            /// <para>OWLSharp extension: direct materialization of EquivalentDataProperties transitivity (derivable via scm-eqp1+scm-spo+scm-eqp2 composition, not itself a table rule)</para><para>Tier: Schema</para>
            /// </summary>
            SchemaEquivalentDataPropertiesEntailment = 6,
            /// <summary>
            /// EquivalentObjectProperties(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> EquivalentObjectProperties(P1,P3)
            /// <para>OWLSharp extension: direct materialization of EquivalentObjectProperties transitivity (derivable via scm-eqp1+scm-spo+scm-eqp2 composition, not itself a table rule)</para><para>Tier: Schema</para>
            /// </summary>
            SchemaEquivalentObjectPropertiesEntailment = 7,
            /// <summary>
            /// ObjectHasValue(OP1,I) [referenced] ^ ObjectHasValue(OP2,I) [referenced] ^ SubObjectPropertyOf(OP1,OP2) -> SubClassOf(ObjectHasValue(OP1,I),ObjectHasValue(OP2,I))<br/>
            /// DataHasValue(DP1,LIT) [referenced] ^ DataHasValue(DP2,LIT) [referenced] ^ SubDataPropertyOf(DP1,DP2) -> SubClassOf(DataHasValue(DP1,LIT),DataHasValue(DP2,LIT))
            /// <para>W3C OWL2 RL/RDF: scm-hv</para><para>Tier: Schema</para>
            /// </summary>
            SchemaHasValueEntailment = 8,
            /// <summary>
            /// ObjectAllValuesFrom(OP,Y1) [referenced] ^ ObjectAllValuesFrom(OP,Y2) [referenced] ^ SubClassOf(Y1,Y2) -> SubClassOf(ObjectAllValuesFrom(OP,Y1),ObjectAllValuesFrom(OP,Y2))<br/>
            /// ObjectAllValuesFrom(OP1,Y) [referenced] ^ ObjectAllValuesFrom(OP2,Y) [referenced] ^ SubObjectPropertyOf(OP1,OP2) -> SubClassOf(ObjectAllValuesFrom(OP2,Y),ObjectAllValuesFrom(OP1,Y))
            /// <para>W3C OWL2 RL/RDF: scm-avf1, scm-avf2. NOTE the inverted polarity of scm-avf2 with respect to scm-svf2: the SubClassOf direction flips
            /// because universal quantification is antitone in the property argument (a stricter/subsuming property yields a MORE permissive AllValuesFrom restriction)</para><para>Tier: Schema</para>
            /// </summary>
            SchemaObjectAllValuesFromEntailment = 9,
            /// <summary>
            /// ObjectProperty(OP) [declared] -> SubObjectPropertyOf(OP,OP) ^ EquivalentObjectProperties(OP,OP)
            /// <para>W3C OWL2 RL/RDF: scm-op</para><para>Tier: Schema</para>
            /// </summary>
            SchemaObjectPropertyEntailment = 10,
            /// <summary>
            /// ObjectSomeValuesFrom(OP,Y1) [referenced] ^ ObjectSomeValuesFrom(OP,Y2) [referenced] ^ SubClassOf(Y1,Y2) -> SubClassOf(ObjectSomeValuesFrom(OP,Y1),ObjectSomeValuesFrom(OP,Y2))<br/>
            /// ObjectSomeValuesFrom(OP1,Y) [referenced] ^ ObjectSomeValuesFrom(OP2,Y) [referenced] ^ SubObjectPropertyOf(OP1,OP2) -> SubClassOf(ObjectSomeValuesFrom(OP1,Y),ObjectSomeValuesFrom(OP2,Y))
            /// <para>W3C OWL2 RL/RDF: scm-svf1, scm-svf2</para><para>Tier: Schema</para>
            /// </summary>
            SchemaObjectSomeValuesFromEntailment = 11,
            /// <summary>
            /// ObjectUnionOf(C,(C1..CN)) [referenced] -> SubClassOf(C1,C) ^ ... ^ SubClassOf(CN,C)
            /// <para>W3C OWL2 RL/RDF: scm-uni. Combined iteratively with FactClassAssertionEntailment (cax-sco), this also re-derives the A-Box cls-uni
            /// entailment (ClassAssertion(Ci,I) -> ClassAssertion(ObjectUnionOf(...),I)) for ANY referenced union, including anonymous ones not
            /// named via EquivalentClasses -- see remarks on FactObjectOneOfEntailment for why cls-oo instead needs its own dedicated A-Box rule</para><para>Tier: Schema</para>
            /// </summary>
            SchemaObjectUnionOfEntailment = 12,
            /// <summary>
            /// ObjectPropertyDomain(OP,C1) ^ SubClassOf(C1,C2) -> ObjectPropertyDomain(OP,C2)<br/>
            /// ObjectPropertyDomain(OP2,C) ^ SubObjectPropertyOf(OP1,OP2) -> ObjectPropertyDomain(OP1,C)<br/>
            /// DataPropertyDomain(DP,C1) ^ SubClassOf(C1,C2) -> DataPropertyDomain(DP,C2)<br/>
            /// DataPropertyDomain(DP2,C) ^ SubDataPropertyOf(DP1,DP2) -> DataPropertyDomain(DP1,C)
            /// <para>W3C OWL2 RL/RDF: scm-dom1, scm-dom2 (both the ObjectProperty and DataProperty variants, since the table's ?p is generic over property kind)</para><para>Tier: Schema</para>
            /// </summary>
            SchemaPropertyDomainEntailment = 13,
            /// <summary>
            /// ObjectPropertyRange(OP,C1) ^ SubClassOf(C1,C2) -> ObjectPropertyRange(OP,C2)<br/>
            /// ObjectPropertyRange(OP2,C) ^ SubObjectPropertyOf(OP1,OP2) -> ObjectPropertyRange(OP1,C)<br/>
            /// DataPropertyRange(DP2,DR) ^ SubDataPropertyOf(DP1,DP2) -> DataPropertyRange(DP1,DR)
            /// <para>W3C OWL2 RL/RDF: scm-rng1, scm-rng2. The DataPropertyRange side only implements the scm-rng2 (subPropertyOf) branch, since scm-rng1's
            /// SubClassOf premise does not apply to datatype ranges (DataRangeExpression, not OWLClassExpression) in the OWLSharp model</para><para>Tier: Schema</para>
            /// </summary>
            SchemaPropertyRangeEntailment = 14,
            /// <summary>
            /// DataProperty(DP) [declared] -> SubDataPropertyOf(DP,DP) ^ EquivalentDataProperties(DP,DP)
            /// <para>W3C OWL2 RL/RDF: scm-dp</para><para>Tier: Schema</para>
            /// </summary>
            SchemaDataPropertyEntailment = 15,
            /// <summary>
            /// SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)<br/>
            /// SubClassOf(C1,C2) ^ EquivalentClasses(C2,C3) -> SubClassOf(C1,C3)<br/>
            /// EquivalentClasses(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)<br/>
            /// DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
            /// <para>W3C OWL2 RL/RDF: scm-sco, scm-eqc1, scm-eqc2, scm-int</para><para>Tier: Schema</para>
            /// </summary>
            SchemaSubClassOfEntailment = 16,
            /// <summary>
            /// SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)<br/>
            /// SubDataPropertyOf(P1,P2) ^ EquivalentDataProperties(P2,P3) -> SubDataPropertyOf(P1,P3)
            /// <para>W3C OWL2 RL/RDF: scm-spo</para><para>Tier: Schema</para>
            /// </summary>
            SchemaSubDataPropertyOfEntailment = 17,
            /// <summary>
            /// SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)<br/>
            /// SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)
            /// <para>W3C OWL2 RL/RDF: scm-spo</para><para>Tier: Schema</para>
            /// </summary>
            SchemaSubObjectPropertyOfEntailment = 18,
            /// <summary>
            /// ClassAssertion(C1,I) ^ SubClassOf(C1,C2) -> ClassAssertion(C2,I)<br/>
            /// ClassAssertion(C1,I) ^ EquivalentClasses(C1,C2) -> ClassAssertion(C2,I)
            /// <para>W3C OWL2 RL/RDF: cax-sco, cax-eqc1, cax-eqc2</para><para>Tier: Fact</para>
            /// </summary>
            FactClassAssertionEntailment = 19,
            /// <summary>
            /// DataPropertyDomain(DP,C) ^ DataPropertyAssertion(DP, I, LIT) -> ClassAssertion(C,I)
            /// <para>W3C OWL2 RL/RDF: prp-dom</para><para>Tier: Fact</para>
            /// </summary>
            FactDataPropertyDomainEntailment = 20,
            /// <summary>
            /// AllDifferent(I1,I2,...IN) -> DifferentIndividuals(I1,I2) ^ DifferentIndividuals(I1,IN) ^ ...
            /// <para>OWLSharp extension: expands n-ary AllDifferent into pairwise DifferentIndividuals (feeds eq-diff1/eq-diff2/eq-diff3 downstream, but is not itself a table rule)</para><para>Tier: Fact</para>
            /// </summary>
            FactDifferentIndividualsEntailment = 21,
            /// <summary>
            /// EquivalentDataProperties(P1,P2) ^ DataPropertyAssertion(P1,I,LIT) -> DataPropertyAssertion(P2,I,LIT)
            /// <para>W3C OWL2 RL/RDF: prp-eqp1, prp-eqp2</para><para>Tier: Fact</para>
            /// </summary>
            FactEquivalentDataPropertiesEntailment = 22,
            /// <summary>
            /// EquivalentObjectProperties(P1,P2) ^ ObjectPropertyAssertion(P1,I1,I2) -> ObjectPropertyAssertion(P2,I1,I2)
            /// <para>W3C OWL2 RL/RDF: prp-eqp1, prp-eqp2</para><para>Tier: Fact</para>
            /// </summary>
            FactEquivalentObjectPropertiesEntailment = 23,
            /// <summary>
            /// FunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDVX,IDV1) ^ ObjectPropertyAssertion(OP,IDVX,IDV2) -> SameIndividual(IDV1,IDV2)
            /// <para>W3C OWL2 RL/RDF: prp-fp</para><para>Tier: Fact</para>
            /// </summary>
            FactFunctionalObjectPropertyEntailment = 24,
            /// <summary>
            /// HasKey(C,OP) ^ ClassAssertion(C,I1) ^ ObjectPropertyAssertion(OP,I1,IX) ^ ClassAssertion(C,I2) ^ ObjectPropertyAssertion(OP,I2,IX) -> SameIndividual(I1,I2)<br/>
            /// HasKey(C,DP) ^ ClassAssertion(C,I1) ^ DataPropertyAssertion(DP,I1,LIT)  ^ ClassAssertion(C,I2) ^ DataPropertyAssertion(DP,I2,LIT)  -> SameIndividual(I1,I2)
            /// <para>W3C OWL2 RL/RDF: prp-key</para><para>Tier: Fact</para>
            /// </summary>
            FactHasKeyEntailment = 25,
            /// <summary>
            /// SubClassOf(C,ObjectHasSelf(OP)) ^ ClassAssertion(C,I) -> ObjectPropertyAssertion(OP,I,I)<br/>
            /// ObjectPropertyAssertion(OP,I,I) ^ [ObjectHasSelf(OP) referenced] -> ClassAssertion(ObjectHasSelf(OP),I)
            /// <para>OWLSharp extension: ObjectHasSelf is excluded from the OWL2 RL class-expression grammar, so both the forward (SubClassOf-driven) and
            /// reverse (reflexive-assertion-driven) ObjectHasSelf entailment go beyond the RL/RDF ruleset</para><para>Tier: Fact</para>
            /// </summary>
            FactHasSelfEntailment = 26,
            /// <summary>
            /// SubClassOf(C,ObjectHasValue(OP,I2)) ^ ClassAssertion(C,I1) -> ObjectPropertyAssertion(OP,I1,I2)<br/>
            /// SubClassOf(C,DataHasValue(DP,LIT)) ^ ClassAssertion(C,I) -> DataPropertyAssertion(DP,I,LIT)<br/>
            /// ObjectPropertyAssertion(OP,I1,V) ^ ObjectHasValue(OP,V) [referenced] -> ClassAssertion(ObjectHasValue(OP,V),I1)<br/>
            /// DataPropertyAssertion(DP,I,LIT) ^ DataHasValue(DP,LIT) [referenced] -> ClassAssertion(DataHasValue(DP,LIT),I)
            /// <para>W3C OWL2 RL/RDF: cls-hv1, cls-hv2</para><para>Tier: Fact</para>
            /// </summary>
            FactHasValueEntailment = 27,
            /// <summary>
            /// InverseFunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDVX) ^ ObjectPropertyAssertion(OP,IDV2,IDVX) -> SameIndividual(IDV1,IDV2)
            /// <para>W3C OWL2 RL/RDF: prp-ifp</para><para>Tier: Fact</para>
            /// </summary>
            FactInverseFunctionalObjectPropertyEntailment = 28,
            /// <summary>
            /// InverseObjectProperties(OP,IOP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(IOP,IDV2,IDV1)<br/>
            /// InverseObjectProperties(OP,IOP) ^ EquivalentObjectProperties(IOP,IOP2)  -> InverseObjectProperties(OP,IOP2)<br/>
            /// InverseObjectProperties(OP,IOP) ^ ObjectPropertyDomain(OP,C) -> ObjectPropertyRange(IOP,C)<br/>
            /// InverseObjectProperties(OP,IOP) ^ ObjectPropertyRange(OP,C)  -> ObjectPropertyDomain(IOP,C)
            /// <para>W3C OWL2 RL/RDF: prp-inv1, prp-inv2 (assertion propagation). Domain/range propagation across inverse properties is an OWLSharp extension beyond the table</para><para>Tier: Fact</para>
            /// </summary>
            FactInverseObjectPropertiesEntailment = 29,
            /// <summary>
            /// ObjectAllValuesFrom(OP,C) [class assertion] ^ ObjectPropertyAssertion(OP,I1,I2) -> ClassAssertion(C,I2)
            /// <para>W3C OWL2 RL/RDF: cls-avf</para><para>Tier: Fact</para>
            /// </summary>
            FactObjectAllValuesFromEntailment = 30,
            /// <summary>
            /// SubClassOf(D,ObjectMaxCardinality(1,OP)) ^ ClassAssertion(D,I) ^ ObjectPropertyAssertion(OP,I,I1) ^ ObjectPropertyAssertion(OP,I,I2) -> SameIndividual(I1,I2)
            /// <para>W3C OWL2 RL/RDF: cls-maxc2</para><para>Tier: Fact</para>
            /// </summary>
            FactObjectMaxCardinalityEntailment = 31,
            /// <summary>
            /// ObjectOneOf(C,(I1..IN)) [referenced] -> ClassAssertion(C,I1) ^ ... ^ ClassAssertion(C,IN)
            /// <para>W3C OWL2 RL/RDF: cls-oo. Needed as a dedicated rule because GetIndividualsOf() only expands ObjectOneOf/ObjectUnionOf/ObjectIntersectionOf
            /// membership when the composite expression is reached via an EquivalentClasses alias of a named class (not when referenced anonymously,
            /// e.g. as the filler of a restriction) -- unlike scm-uni's SubClassOf materialization, an anonymous ObjectOneOf has no SubClassOf target
            /// to piggyback on, so its ClassAssertion materialization must be produced directly here</para><para>Tier: Fact</para>
            /// </summary>
            FactObjectOneOfEntailment = 32,
            /// <summary>
            /// ObjectPropertyChain(PC,(OP1..OPN)) ^ SubObjectPropertyOf(PC,OP) -> ObjectPropertyAssertion(OP,OP1,OPN)
            /// <para>W3C OWL2 RL/RDF: prp-spo2</para><para>Tier: Fact</para>
            /// </summary>
            FactObjectPropertyChainEntailment = 33,
            /// <summary>
            /// ObjectPropertyDomain(OP,C) ^ ObjectPropertyAssertion(OP,I1,I2) -> ClassAssertion(C,I1)
            /// <para>W3C OWL2 RL/RDF: prp-dom</para><para>Tier: Fact</para>
            /// </summary>
            FactObjectPropertyDomainEntailment = 34,
            /// <summary>
            /// ObjectPropertyRange(OP,C) ^ ObjectPropertyAssertion(OP,I1,I2) -> ClassAssertion(C,I2)
            /// <para>W3C OWL2 RL/RDF: prp-rng</para><para>Tier: Fact</para>
            /// </summary>
            FactObjectPropertyRangeEntailment = 35,
            /// <summary>
            /// ObjectSomeValuesFrom(OP,C) ^ ObjectPropertyAssertion(OP,I1,I2) ^ ClassAssertion(C,I2) -> ClassAssertion(ObjectSomeValuesFrom(OP,C),I1)<br/>
            /// ObjectSomeValuesFrom(OP,owl:Thing) ^ ObjectPropertyAssertion(OP,I1,I2) -> ClassAssertion(ObjectSomeValuesFrom(OP,owl:Thing),I1)
            /// <para>W3C OWL2 RL/RDF: cls-svf1, cls-svf2</para><para>Tier: Fact</para>
            /// </summary>
            FactObjectSomeValuesFromEntailment = 36,
            /// <summary>
            /// ReflexiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(OP,IDV1,IDV1)
            /// <para>OWLSharp extension: ReflexiveObjectProperty assertion-materialization is not part of the RL/RDF entailment table (reflexivity is used there only for consistency checking, not production)</para><para>Tier: Fact</para>
            /// </summary>
            FactReflexiveObjectPropertyEntailment = 37,
            /// <summary>
            /// SameIndividual(I1,I2) ^ SameIndividual(I2,I3) -> SameIndividual(I1,I3)<br/>
            /// SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I1,I3) -> ObjectPropertyAssertion(OP,I2,I3)<br/>
            /// SameIndividual(I1,I2) ^ ObjectPropertyAssertion(OP,I2,I3) -> ObjectPropertyAssertion(OP,I1,I3)<br/>
            /// SameIndividual(I1,I2) ^ ClassAssertion(C,I1) -> ClassAssertion(C,I2)
            /// <para>W3C OWL2 RL/RDF: eq-sym, eq-trans, eq-rep-s, eq-rep-o</para><para>Tier: Fact</para>
            /// </summary>
            FactSameIndividualEntailment = 38,
            /// <summary>
            /// SubDataPropertyOf(P1,P2) ^ DataPropertyAssertion(P1,I,LIT) -> DataPropertyAssertion(P2,I,LIT)
            /// <para>W3C OWL2 RL/RDF: prp-spo1</para><para>Tier: Fact</para>
            /// </summary>
            FactSubDataPropertyOfEntailment = 39,
            /// <summary>
            /// SubObjectPropertyOf(P1,P2) ^ ObjectPropertyAssertion(P1,I1,I2) -> ObjectPropertyAssertion(P2,I1,I2)
            /// <para>W3C OWL2 RL/RDF: prp-spo1</para><para>Tier: Fact</para>
            /// </summary>
            FactSubObjectPropertyOfEntailment = 40,
            /// <summary>
            /// SymmetricObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(OP,IDV2,IDV1)
            /// <para>W3C OWL2 RL/RDF: prp-symp</para><para>Tier: Fact</para>
            /// </summary>
            FactSymmetricObjectPropertyEntailment = 41,
            /// <summary>
            /// TransitiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) ^ ObjectPropertyAssertion(OP,IDV2,IDV3) -> ObjectPropertyAssertion(OP,IDV1,IDV3)
            /// <para>W3C OWL2 RL/RDF: prp-trp</para><para>Tier: Fact</para>
            /// </summary>
            FactTransitiveObjectPropertyEntailment = 42
        }

        /// <summary>
        /// OWLValidatorRules represents an enumeration for supported RDFS/OWL2 validator rules
        /// </summary>
        public enum OWLValidatorRules
        {
            /// <summary>
            /// AsymmetricObjectProperty(OP) ^ SymmetricObjectProperty(OP) -> ERROR<br/>
            /// AsymmetricObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I2,I1) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-asyp (assertion-pair violation check). The AsymmetricObjectProperty+SymmetricObjectProperty overlap check is an OWLSharp extension</para>
            /// </summary>
            AsymmetricObjectPropertyAnalysis = 1,
            /// <summary>
            /// ClassAssertion(C,I) ^ ClassAssertion(ObjectComplementOf(C),I) -> ERROR
            /// <para>W3C OWL2 RL/RDF: cls-com</para>
            /// </summary>
            ClassAssertionAnalysis = 2,
            /// <summary>
            /// DataPropertyAssertion(DP,I,LIT) ^ DataPropertyDomain(DP,C) ^ ClassAssertion(ObjectComplementOf(C),I) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-dom (consistency-check form: flags when the domain inference would contradict an explicit negative ClassAssertion)</para>
            /// </summary>
            DataPropertyDomainAnalysis = 3,
            /// <summary>
            /// DataPropertyAssertion(DP,I,LIT) ^ DataPropertyRange(DP,DR) ^ swrl:equal(Datatype(LIT),DataComplementOf(DR)) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-rng (consistency-check form for datatype-constrained ranges). Datatype value-space well-formedness of the literal itself (dt-not-type) is covered separately by LiteralDatatypeAnalysis</para>
            /// </summary>
            DataPropertyRangeAnalysis = 4,
            /// <summary>
            /// DifferentIndividuals(I1,...,IN) ^ SameIndividual(IX,IY) for any X != Y among the N members (direct or transitive) -> ERROR<br/>
            /// DifferentIndividuals(I,...,I) with a repeated member -> ERROR
            /// <para>W3C OWL2 RL/RDF: eq-diff1, eq-diff2, eq-diff3 (SameIndividual+DifferentIndividuals overlap check; the pairwise scan already covers every combination
            /// within the n-ary AllDifferent member list, which is the eq-diff2/eq-diff3 case -- OWLSharp does not distinguish an AllDifferent's owl:members
            /// vs owl:distinctMembers encoding, both collapse onto OWLDifferentIndividuals.IndividualExpressions). The DifferentIndividuals(I,I) self-difference check is an OWLSharp extension</para>
            /// </summary>
            DifferentIndividualsAnalysis = 5,
            /// <summary>
            /// DisjointClasses(C1,C2) ^ SubClassOf(C1,C2) -> WARNING<br/>
            /// DisjointClasses(C1,C2) ^ SubClassOf(C2,C1) -> WARNING<br/>
            /// DisjointClasses(C1,C2) ^ EquivalentClasses(C1,C2) -> WARNING<br/>
            /// DisjointClasses(C1,C2) ^ ClassAssertion(C1,I) ^ ClassAssertion(C2,I) -> ERROR
            /// <para>W3C OWL2 RL/RDF: cax-dw (A-Box shared-ClassAssertion check, Error: a genuine ontology inconsistency). The T-Box overlap check
            /// against SubClassOf/EquivalentClasses is an OWLSharp extension (Warning: it only forces the involved classes to be equivalent to
            /// owl:Nothing, a modeling smell rather than an ontology-level inconsistency by itself)</para>
            /// </summary>
            DisjointClassesAnalysis = 6,
            /// <summary>
            /// DisjointDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP1,DP2) -> WARNING<br/>
            /// DisjointDataProperties(DP1,DP2) ^ SubDataPropertyOf(DP2,DP1) -> WARNING<br/>
            /// DisjointDataProperties(DP1,DP2) ^ EquivalentDataProperties(DP1,DP2) -> WARNING<br/>
            /// AllDisjointDataProperties(DP1,...,DPN) ^ DataPropertyAssertion(DPi,I,LIT) ^ DataPropertyAssertion(DPj,I,LIT), i != j -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-pdw, prp-adp (A-Box shared-assertion check; the flattened-then-grouped scan already covers any two distinct members of the
            /// n-ary AllDisjointProperties list sharing the same (individual,literal) pair, which is the prp-adp case; Error, a genuine ontology inconsistency).
            /// The T-Box overlap check against SubDataPropertyOf/EquivalentDataProperties is an OWLSharp extension (Warning: it only forces the involved
            /// properties to be equivalent to owl:bottomDataProperty, a modeling smell rather than an ontology-level inconsistency by itself)</para>
            /// </summary>
            DisjointDataPropertiesAnalysis = 7,
            /// <summary>
            /// DisjointObjectProperties(OP1,OP2) ^ SubObjectPropertyOf(OP1,OP2) -> WARNING<br/>
            /// DisjointObjectProperties(OP1,OP2) ^ SubObjectPropertyOf(OP2,OP1) -> WARNING<br/>
            /// DisjointObjectProperties(OP1,OP2) ^ EquivalentObjectProperties(OP1,OP2) -> WARNING<br/>
            /// AllDisjointObjectProperties(OP1,...,OPN) ^ ObjectPropertyAssertion(OPi,I1,I2) ^ ObjectPropertyAssertion(OPj,I1,I2), i != j -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-pdw, prp-adp (A-Box shared-assertion check; the flattened-then-grouped scan already covers any two distinct members of the
            /// n-ary AllDisjointProperties list sharing the same (source,target) pair, which is the prp-adp case; Error, a genuine ontology inconsistency).
            /// The T-Box overlap check against SubObjectPropertyOf/EquivalentObjectProperties is an OWLSharp extension (Warning: it only forces the involved
            /// properties to be equivalent to owl:bottomObjectProperty, a modeling smell rather than an ontology-level inconsistency by itself)</para>
            /// </summary>
            DisjointObjectPropertiesAnalysis = 8,
            /// <summary>
            /// DisjointUnion(C,(C1,C2)) ^ ClassAssertion(C1,I) ^ ClassAssertion(C2,I) -> ERROR
            /// <para>W3C OWL2 RL/RDF: cax-adc</para>
            /// </summary>
            DisjointUnionAnalysis = 9,
            /// <summary>
            /// EquivalentClasses(C1,C2) ^ DisjointClasses(C1,C2) -> WARNING
            /// <para>OWLSharp extension: T-Box overlap check (EquivalentClasses vs DisjointClasses); the RL/RDF ruleset assumes consistent T-Box input
            /// rather than flagging redundant/contradictory axiom combinations. Warning, not Error: the clash only forces the involved classes to be
            /// equivalent to owl:Nothing, a modeling smell rather than an ontology-level inconsistency by itself. NOTE: EquivalentClasses combined with
            /// SubClassOf in either direction on the same pair is NOT flagged at all (removed, not merely downgraded): it is redundant, not contradictory
            /// -- explicitly restating one direction of an already-declared equivalence via SubClassOf is a common, deliberate modeling idiom</para>
            /// </summary>
            EquivalentClassesAnalysis = 10,
            /// <summary>
            /// EquivalentDataProperties(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> WARNING
            /// <para>OWLSharp extension: T-Box overlap check (EquivalentDataProperties vs DisjointDataProperties), no direct RL/RDF correspondent.
            /// Warning, not Error: the clash only forces the involved properties to be equivalent to owl:bottomDataProperty, a modeling smell rather
            /// than an ontology-level inconsistency by itself. NOTE: EquivalentDataProperties combined with SubDataPropertyOf in either direction on
            /// the same pair is NOT flagged at all (removed, not merely downgraded): it is redundant, not contradictory</para>
            /// </summary>
            EquivalentDataPropertiesAnalysis = 11,
            /// <summary>
            /// EquivalentObjectProperties(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> WARNING
            /// <para>OWLSharp extension: T-Box overlap check (EquivalentObjectProperties vs DisjointObjectProperties), no direct RL/RDF correspondent.
            /// Warning, not Error: the clash only forces the involved properties to be equivalent to owl:bottomObjectProperty, a modeling smell rather
            /// than an ontology-level inconsistency by itself. NOTE: EquivalentObjectProperties combined with SubObjectPropertyOf in either direction on
            /// the same pair is NOT flagged at all (removed, not merely downgraded): it is redundant, not contradictory</para>
            /// </summary>
            EquivalentObjectPropertiesAnalysis = 12,
            /// <summary>
            /// FunctionalDataProperty(DP) ^ DataPropertyAssertion(DP,I,LIT1) ^ DataPropertyAssertion(DP,I,LIT2) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-fp (consistency-check form for data properties: literals cannot be merged via sameAs, so a functional data property linking one individual to two distinct literals is a direct violation)</para>
            /// </summary>
            FunctionalDataPropertyAnalysis = 13,
            /// <summary>
            /// FunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I1,I3) ^ DifferentIndividuals(I2,I3) -> ERROR<br/>
            /// FunctionalObjectProperty(OP) ^ TransitiveObjectProperty(OP) -> ERROR<br/>
            /// FunctionalObjectProperty(OP) ^ SubObjectPropertyOf(OP,OP2) ^ TransitiveObjectProperty(OP2) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-fp (consistency-check form: flags when the sameAs inference would contradict an explicit DifferentIndividuals). The FunctionalObjectProperty+TransitiveObjectProperty restriction check is an OWLSharp extension (OWL2 DL simple-role restriction, unrelated to RL/RDF)</para>
            /// </summary>
            FunctionalObjectPropertyAnalysis = 14,
            /// <summary>
            /// HasKey(C,OP) ^ ClassAssertion(C,I1) ^ ObjectPropertyAssertion(OP,I1,IX) ^ ClassAssertion(C,I2) ^ ObjectPropertyAssertion(OP,I2,IX) ^ DifferentIndividuals(I1,I2) -> ERROR<br/>
            /// HasKey(C,DP) ^ ClassAssertion(C,I1) ^ DataPropertyAssertion(DP,I1,LIT)  ^ ClassAssertion(C,I2) ^ DataPropertyAssertion(DP,I2,LIT)  ^ DifferentIndividuals(I1,I2) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-key (consistency-check form: flags when the sameAs inference would contradict an explicit DifferentIndividuals)</para>
            /// </summary>
            HasKeyAnalysis = 15,
            /// <summary>
            /// InverseFunctionalObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I3,I2) ^ DifferentIndividuals(I1,I3) -> ERROR<br/>
            /// InverseFunctionalObjectProperty(OP) ^ TransitiveObjectProperty(OP) -> ERROR<br/>
            /// InverseFunctionalObjectProperty(OP) ^ SubObjectPropertyOf(OP,OP2) ^ TransitiveObjectProperty(OP2) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-ifp (consistency-check form: flags when the sameAs inference would contradict an explicit DifferentIndividuals). The InverseFunctionalObjectProperty+TransitiveObjectProperty restriction check is an OWLSharp extension (OWL2 DL simple-role restriction, unrelated to RL/RDF)</para>
            /// </summary>
            InverseFunctionalObjectPropertyAnalysis = 16,
            /// <summary>
            /// IrreflexiveObjectProperty(OP) ^ ReflexiveObjectProperty(OP) -> ERROR<br/>
            /// IrreflexiveObjectProperty(OP) ^ ObjectPropertyAssertion(OP,I,I) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-irp (reflexive-assertion violation check). The Irreflexive+Reflexive overlap check is an OWLSharp extension</para>
            /// </summary>
            IrreflexiveObjectPropertyAnalysis = 17,
            /// <summary>
            /// DataPropertyAssertion(DP,I,LIT) ^ dt-not-type(LIT) -> ERROR<br/>
            /// NegativeDataPropertyAssertion(DP,I,LIT) ^ dt-not-type(LIT) -> ERROR
            /// <para>W3C OWL2 RL/RDF: dt-not-type (typed literal value-space compatibility check, scoped to A-Box DataPropertyAssertion/NegativeDataPropertyAssertion literals)</para>
            /// </summary>
            LiteralDatatypeAnalysis = 18,
            /// <summary>
            /// NegativeDataPropertyAssertion(DP,I,LIT) ^ DataPropertyAssertion(DP,I,LIT) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-npa2</para>
            /// </summary>
            NegativeDataAssertionsAnalysis = 19,
            /// <summary>
            /// NegativeObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyAssertion(OP,I1,I2) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-npa1</para>
            /// </summary>
            NegativeObjectAssertionsAnalysis = 20,
            /// <summary>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ AsymmetricObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ FunctionalObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ InverseFunctionalObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP) ^ IrreflexiveObjectProperty(OP) -> ERROR<br/>
            /// ObjectPropertyChain(PC,(OP1,OP2)) ^ SubObjectPropertyOf(PC,OP1) -> ERROR
            /// <para>OWLSharp extension: enforces OWL2 DL property-chain regularity / simple-role restrictions (chain superproperty cannot be asymmetric/functional/inverse-functional/irreflexive; chain cannot be self-referential), which is outside the RL/RDF entailment table</para>
            /// </summary>
            ObjectPropertyChainAnalysis = 21,
            /// <summary>
            /// ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyDomain(OP,C) ^ ClassAssertion(ObjectComplementOf(C),I1) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-dom (consistency-check form: flags when the domain inference would contradict an explicit negative ClassAssertion)</para>
            /// </summary>
            ObjectPropertyDomainAnalysis = 22,
            /// <summary>
            /// ObjectPropertyAssertion(OP,I1,I2) ^ ObjectPropertyRange(OP,C) ^ ClassAssertion(ObjectComplementOf(C),I2) -> ERROR
            /// <para>W3C OWL2 RL/RDF: prp-rng (consistency-check form: flags when the range inference would contradict an explicit negative ClassAssertion)</para>
            /// </summary>
            ObjectPropertyRangeAnalysis = 23,
            /// <summary>
            /// SubClassOf(C1,C2) ^ DisjointClasses(C1,C2) -> WARNING<br/>
            /// SubClassOf(C,[Object|Data][Exact|Max]Cardinality(P,N) -> ERROR (ON ASSERTION'S CARDINALITY VIOLATION)
            /// <para>W3C OWL2 RL/RDF: cls-maxc1, cls-maxc2, cls-maxqc1, cls-maxqc2, cls-maxqc3, cls-maxqc4 (cardinality-violation checks, qualified and
            /// unqualified; Error, a genuine A-Box violation). The SubClassOf vs DisjointClasses overlap check is an OWLSharp extension (Warning: it only
            /// forces the subclass to be equivalent to owl:Nothing, a modeling smell rather than an ontology-level inconsistency by itself). NOTE:
            /// SubClassOf combined with the mutual SubClassOf in the other direction, or with EquivalentClasses on the same pair, is NOT flagged at all
            /// (removed, not merely downgraded): both are just redundant restatements of class equivalence, and mutual SubClassOf in particular is a
            /// common, deliberate idiom for expressing it without an explicit EquivalentClasses axiom</para>
            /// </summary>
            SubClassOfAnalysis = 24,
            /// <summary>
            /// SubDataPropertyOf(DP1,DP2) ^ DisjointDataProperties(DP1,DP2) -> WARNING
            /// <para>OWLSharp extension: T-Box overlap check (SubDataPropertyOf vs DisjointDataProperties), no direct RL/RDF correspondent. Warning, not
            /// Error: the clash only forces the sub-property to be equivalent to owl:bottomDataProperty, a modeling smell rather than an ontology-level
            /// inconsistency by itself. NOTE: SubDataPropertyOf combined with the mutual SubDataPropertyOf in the other direction, or with
            /// EquivalentDataProperties on the same pair, is NOT flagged at all (removed, not merely downgraded): both are just redundant restatements
            /// of property equivalence</para>
            /// </summary>
            SubDataPropertyOfAnalysis = 25,
            /// <summary>
            /// SubObjectPropertyOf(OP1,OP2) ^ DisjointObjectProperties(OP1,OP2) -> WARNING
            /// <para>OWLSharp extension: T-Box overlap check (SubObjectPropertyOf vs DisjointObjectProperties), no direct RL/RDF correspondent. Warning,
            /// not Error: the clash only forces the sub-property to be equivalent to owl:bottomObjectProperty, a modeling smell rather than an
            /// ontology-level inconsistency by itself. NOTE: SubObjectPropertyOf combined with the mutual SubObjectPropertyOf in the other direction, or
            /// with EquivalentObjectProperties on the same pair, is NOT flagged at all (removed, not merely downgraded): both are just redundant
            /// restatements of property equivalence</para>
            /// </summary>
            SubObjectPropertyOfAnalysis = 26,
            /// <summary>
            /// Class(C) ^ AnnotationAssertion(owl:deprecated,C,true) -> WARNING<br/>
            /// Datatype(D) ^ AnnotationAssertion(owl:deprecated,D,true) -> WARNING<br/>
            /// DataProperty(DP) ^ AnnotationAssertion(owl:deprecated,DP,true) -> WARNING<br/>
            /// ObjectProperty(OP) ^ AnnotationAssertion(owl:deprecated,OP,true) -> WARNING<br/>
            /// AnnotationProperty(AP) ^ AnnotationAssertion(owl:deprecated,AP,true) -> WARNING
            /// <para>OWLSharp extension: annotation-level pragma (owl:deprecated) with no formal RL/RDF semantics</para>
            /// </summary>
            TermsDeprecationAnalysis = 27,
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
            /// <para>OWLSharp extension: stylistic discouragement of punning across entity kinds, with no formal RL/RDF semantics</para>
            /// </summary>
            TermsDisjointnessAnalysis = 28,
            /// <summary>
            /// Class(C) ^ SubClassOf(owl:Thing,C) -> WARNING<br/>
            /// Class(C) ^ SubClassOf(C, owl:Nothing) -> WARNING<br/>
            /// ClassAssertion(owl:Nothing, I) -> WARNING
            /// <para>W3C OWL2 RL/RDF: cls-thing, cls-nothing1 (owl:Thing/owl:Nothing root/bottom-position checks), cls-nothing2 (individuals in owl:Nothing check)</para>
            /// </summary>
            ThingNothingAnalysis = 29,
            /// <summary>
            /// ObjectProperty(OP) ^ SubObjectPropertyOf(owl:topObjectProperty,OP) -> WARNING<br/>
            /// ObjectProperty(OP) ^ SubObjectPropertyOf(OP, owl:bottomObjectProperty) -> WARNING<br/>
            /// DataProperty(DP) ^ SubDataPropertyOf(owl:topDataProperty,DP) -> WARNING<br/>
            /// DataProperty(DP) ^ SubDataPropertyOf(DP, owl:bottomDataProperty) -> WARNING<br/>
            /// ObjectPropertyAssertion(owl:bottomObjectProperty,I1,I2) -> ERROR<br/>
            /// DataPropertyAssertion(owl:bottomDataProperty,I,LIT) -> ERROR
            /// <para>OWLSharp extension: no direct W3C RL/RDF correspondent for owl:top/bottomObjectProperty and owl:top/bottomDataProperty root/bottom-position checks
            /// (analogous in spirit to cls-thing/cls-nothing1, but the property-level axiomatic triples are not part of the given RL/RDF table)</para>
            /// </summary>
            TopBottomAnalysis = 30
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

        /// <summary>
        /// OWLProfiles represents an enumeration for the syntactic OWL2 profiles defined at https://www.w3.org/TR/owl2-profiles/
        /// </summary>
        public enum OWLProfiles
        {
            /// <summary>
            /// OWL 2 EL profile (https://www.w3.org/TR/owl2-profiles/#OWL_2_EL), tailored for ontologies with large TBoxes
            /// dominated by existential quantification, guaranteeing polynomial-time reasoning
            /// </summary>
            EL = 1,
            /// <summary>
            /// OWL 2 QL profile (https://www.w3.org/TR/owl2-profiles/#OWL_2_QL), tailored for ontologies with large ABoxes
            /// where query answering is the most important reasoning task, enabling conjunctive query answering via query rewriting
            /// </summary>
            QL = 2,
            /// <summary>
            /// OWL 2 RL profile (https://www.w3.org/TR/owl2-profiles/#OWL_2_RL), tailored for applications requiring scalable
            /// reasoning without sacrificing too much expressive power, implementable using rule-based reasoning engines
            /// </summary>
            RL = 3
        }

        /// <summary>
        /// OWLClassExpressionPosition represents the syntactic slot occupied by a class expression within an axiom,
        /// as distinguished by the OWL2 QL and RL grammars (subClassExpression, superClassExpression, equivClassExpression
        /// at https://www.w3.org/TR/owl2-profiles/). OWL2 EL does not distinguish positions (its ClassExpression grammar
        /// is unique), so EL checks ignore this dimension.
        /// </summary>
        internal enum OWLClassExpressionPosition
        {
            /// <summary>
            /// The class expression occupies a "subClassExpression" slot (e.g: the LHS of SubClassOf, or a DisjointClasses member)
            /// </summary>
            SubClass = 1,
            /// <summary>
            /// The class expression occupies a "superClassExpression" slot (e.g: the RHS of SubClassOf, or a property's domain/range)
            /// </summary>
            SuperClass = 2,
            /// <summary>
            /// The class expression occupies an "equivClassExpression" slot (e.g: an EquivalentClasses member in OWL2 RL),
            /// which is stricter than both subClassExpression and superClassExpression and is not reducible to either
            /// </summary>
            EquivalentClass = 3
        }
    }
}