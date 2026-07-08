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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OWLSharp.Ontology;
using OWLSharp.Profiler.Walkers;
using RDFSharp.Model;

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLRLProfile checks an ontology against the OWL 2 RL profile grammar (https://www.w3.org/TR/owl2-profiles/#OWL_2_RL, §4.2).
    /// Unlike EL, RL distinguishes THREE class expression positions (subClassExpression, superClassExpression, and the
    /// stricter equivClassExpression used only by EquivalentClasses), and ObjectComplementOf/ObjectAllValuesFrom/
    /// ObjectMaxCardinality require the checker to recurse with position awareness — including inverting position
    /// on ObjectComplementOf, which is admitted only in superclass position but wraps a subClassExpression operand.
    /// </summary>
    internal static class OWLRLProfile
    {
        private const string ClassExpressionRule = nameof(OWLEnums.OWLProfiles.RL) + ".ClassExpression";                 //§4.2.3
        private const string DataRangeRule = nameof(OWLEnums.OWLProfiles.RL) + ".DataRange";                             //§4.2.4
        private const string ClassAxiomTypeRule = nameof(OWLEnums.OWLProfiles.RL) + ".ClassAxiomType";                   //§4.2.5
        private const string ObjectPropertyAxiomTypeRule = nameof(OWLEnums.OWLProfiles.RL) + ".ObjectPropertyAxiomType"; //§4.2.5
        private const string DataPropertyAxiomTypeRule = nameof(OWLEnums.OWLProfiles.RL) + ".DataPropertyAxiomType";     //§4.2.5
        private const string DatatypeRule = nameof(OWLEnums.OWLProfiles.RL) + ".Datatype";                               //§4.2.1

        //§4.2.1: RL's datatype allowlist DERIVES from EL's shared base (OWLELProfile.AllowedDatatypeIRIs) rather
        //than being re-declared from scratch: the two sets actually share ~17 of EL's 19 entries verbatim (every
        //XSD/RDF/RDFS datatype EL admits), so spelling out that shared core a second time here would just be a
        //second place for it to silently drift out of sync with EL's. Only the genuine DELTA is written out below:
        //  - Except(...): the two entries EL admits that RL does NOT (owl:real, owl:rational — RL has no
        //    equivalent of OWL2's abstract numeric datatypes, only concrete XSD ones);
        //  - Concat(...): the entries RL admits that EL does NOT (the full signed/unsigned XSD integer family,
        //    plus xsd:float/xsd:double/xsd:boolean/xsd:language — RL's rule-based semantics have no trouble with
        //    these concrete numeric/textual types, unlike EL's tractability-driven restriction to a narrower set).
        //If a future spec erratum changes the ~17-entry shared core, only OWLELProfile.AllowedDatatypeIRIs needs
        //editing; if it changes RL's own delta, only the two collections below need editing.
        private static readonly HashSet<string> AllowedDatatypeIRIs = new HashSet<string>(
            OWLELProfile.AllowedDatatypeIRIs
                //Both values reused from OWLELProfile (RDFVocabulary.OWL.REAL directly, and the manually-built
                //OwlRationalDatatypeIRI — see its declaration in OWLELProfile for why owl:rational needs one),
                //rather than each being re-derived here, so this Except(...) can never silently target a
                //differently-constructed-but-equal-looking RDFResource than the one actually inside EL's set.
                .Except(new[] {
                    RDFVocabulary.OWL.REAL.ToString(),
                    OWLELProfile.OwlRationalDatatypeIRI
                })
                .Concat(new[] {
                    RDFVocabulary.XSD.NON_POSITIVE_INTEGER.ToString(),
                    RDFVocabulary.XSD.POSITIVE_INTEGER.ToString(),
                    RDFVocabulary.XSD.NEGATIVE_INTEGER.ToString(),
                    RDFVocabulary.XSD.LONG.ToString(),
                    RDFVocabulary.XSD.INT.ToString(),
                    RDFVocabulary.XSD.SHORT.ToString(),
                    RDFVocabulary.XSD.BYTE.ToString(),
                    RDFVocabulary.XSD.UNSIGNED_LONG.ToString(),
                    RDFVocabulary.XSD.UNSIGNED_INT.ToString(),
                    RDFVocabulary.XSD.UNSIGNED_SHORT.ToString(),
                    RDFVocabulary.XSD.UNSIGNED_BYTE.ToString(),
                    RDFVocabulary.XSD.FLOAT.ToString(),
                    RDFVocabulary.XSD.DOUBLE.ToString(),
                    RDFVocabulary.XSD.LANGUAGE.ToString(),
                    RDFVocabulary.XSD.BOOLEAN.ToString()
                }));

        //See OWLProfiler.ExecuteProfileRuleAsync / feedback_genuine_async: Task-returning end-to-end.
        internal static Task<List<OWLProfileViolation>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLProfileViolation> violations = new List<OWLProfileViolation>();

            //§4.2.3: every top-level class expression slot from SubClassOf/EquivalentClasses/DisjointClasses,
            //already tagged with the correct grammar (Sub/Super/Equiv) by the shared walker — RL is the one
            //profile where that tag actually changes which constructs are admitted.
            foreach ((OWLClassAxiom axiom, OWLClassExpression classExpression, OWLEnums.OWLClassExpressionPosition position) in OWLClassAxiomWalker.WalkClassAxioms(ontology))
                CheckClassExpression(axiom, classExpression, position, violations);

            //...plus the class expressions embedded in property domain/range axioms (not enumerated by
            //OWLClassAxiomWalker, whose scope is ClassAxioms only). Structurally, ObjectPropertyDomain(OP,C) is
            //equivalent to SubClassOf(ObjectSomeValuesFrom(OP,owl:Thing),C) and ObjectPropertyRange(OP,C) to
            //SubClassOf(ObjectSomeValuesFrom(ObjectInverseOf(OP),owl:Thing),C): in both cases C plays the
            //SUPERclass role, never the subclass one.
            foreach (OWLObjectPropertyDomain domain in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>())
                CheckClassExpression(domain, domain.ClassExpression, OWLEnums.OWLClassExpressionPosition.SuperClass, violations);
            foreach (OWLObjectPropertyRange range in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>())
                CheckClassExpression(range, range.ClassExpression, OWLEnums.OWLClassExpressionPosition.SuperClass, violations);
            foreach (OWLDataPropertyDomain domain in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>())
                CheckClassExpression(domain, domain.ClassExpression, OWLEnums.OWLClassExpressionPosition.SuperClass, violations);

            //§4.2.4 DataRange grammar, for DataPropertyRange(DP,DR), which states its range directly as a DR
            //rather than reaching one through a class expression (see OWLELProfile for the identical rationale).
            foreach (OWLDataPropertyRange range in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>())
                CheckDataRange(range, range.DataRangeExpression, violations);

            //§4.2.5 allowed axiom types, independent of grammar shape.
            CheckClassAxiomTypes(ontology, violations);
            CheckObjectPropertyAxiomTypes(ontology, violations);
            CheckDataPropertyAxiomTypes(ontology, violations);

            return Task.FromResult(violations);
        }

        /// <summary>
        /// Recursively checks a class expression against RL's position-dependent grammar (§4.2.3). The three
        /// productions (subClassExpression/superClassExpression/equivClassExpression) share some constructs
        /// (ObjectIntersectionOf, ObjectHasValue, DataHasValue, atomic Class) but otherwise diverge: e.g. union
        /// is sub-only, universal quantification and cardinality are super-only, and equiv is the strictest of all.
        /// </summary>
        private static void CheckClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, OWLEnums.OWLClassExpressionPosition position, List<OWLProfileViolation> violations)
        {
            //Every production explicitly excludes owl:Thing as the atomic class ("Class other than owl:Thing"):
            //an ontology that writes SubClassOf(C, owl:Thing) or similar directly outside a cardinality/existential
            //filler position falls outside RL's grammar, even though owl:Thing is a perfectly normal OWL2 class.
            if (classExpression is OWLClass classExpr && classExpr.GetIRI().Equals(RDFVocabulary.OWL.THING))
            {
                AddViolation(violations, ClassExpressionRule, axiom,
                    $"owl:Thing is not admitted as a plain class expression in {position} position by the RL profile (§4.2.3)",
                    "Remove the explicit owl:Thing reference, or restrict it to the positions where RL admits it (e.g. as an ObjectSomeValuesFrom/ObjectMaxCardinality filler)");
                return;
            }

            //Dispatch to one of three independent grammars, one per production: this is what makes RL different
            //from EL (whose ClassExpression grammar is a single method, no dispatch needed) and from QL (whose
            //grammar only has Sub/Super, no Equiv). Each helper below owns its own switch over the concrete
            //OWLClassExpression subtype, so adding a construct to one production never risks silently affecting
            //what is admitted in another. "default" here means EquivalentClass, the only remaining enum value
            //besides SubClass/SuperClass (OWLEnums.OWLClassExpressionPosition has exactly three values).
            switch (position)
            {
                case OWLEnums.OWLClassExpressionPosition.SubClass:
                    CheckSubClassExpression(axiom, classExpression, violations);
                    break;
                case OWLEnums.OWLClassExpressionPosition.SuperClass:
                    CheckSuperClassExpression(axiom, classExpression, violations);
                    break;
                default:
                    CheckEquivClassExpression(axiom, classExpression, violations);
                    break;
            }
        }

        /// <summary>
        /// subClassExpression := Class(≠owl:Thing) | ObjectIntersectionOf | ObjectUnionOf | ObjectOneOf |
        /// ObjectSomeValuesFrom(OPE, subCE|owl:Thing) | ObjectHasValue | DataSomeValuesFrom | DataHasValue.
        /// </summary>
        private static void CheckSubClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, List<OWLProfileViolation> violations)
        {
            switch (classExpression)
            {
                case OWLClass _:
                    break; //owl:Thing already excluded by the caller; every other atomic class is fine here.

                case OWLObjectIntersectionOf intersectionOf:
                    foreach (OWLClassExpression member in intersectionOf.ClassExpressions)
                        CheckClassExpression(axiom, member, OWLEnums.OWLClassExpressionPosition.SubClass, violations);
                    break;

                //ObjectUnionOf is admitted ONLY in subclass position: this is the single most distinctive
                //asymmetry of RL's grammar, the opposite of what a DL-trained intuition would expect (union is
                //usually thought of as a "wide" superclass-side construct, but RL's rule-based semantics need it
                //on the antecedent side of the corresponding datalog rule instead).
                case OWLObjectUnionOf unionOf:
                    foreach (OWLClassExpression member in unionOf.ClassExpressions)
                        CheckClassExpression(axiom, member, OWLEnums.OWLClassExpressionPosition.SubClass, violations);
                    break;

                //Unlike EL, RL's ObjectOneOf is NOT restricted to a singleton: any nominal enumeration is admitted
                //in subclass position (individuals only, nothing to recurse into).
                case OWLObjectOneOf _:
                    break;

                //ObjectSomeValuesFrom's filler must be EITHER owl:Thing (an explicit grammar exception, checked
                //here rather than via the general "Class≠owl:Thing" rule above, which would otherwise reject it)
                //OR a full subClassExpression.
                case OWLObjectSomeValuesFrom someValuesFrom:
                    if (!IsOwlThing(someValuesFrom.ClassExpression))
                        CheckClassExpression(axiom, someValuesFrom.ClassExpression, OWLEnums.OWLClassExpressionPosition.SubClass, violations);
                    break;

                case OWLObjectHasValue _:
                case OWLDataHasValue _:
                    break; //Admitted as-is: neither carries a nested class/data range expression.

                case OWLDataSomeValuesFrom dataSomeValuesFrom:
                    CheckDataRange(axiom, dataSomeValuesFrom.DataRangeExpression, violations);
                    break;

                //Everything else (ObjectComplementOf, ObjectAllValuesFrom, any cardinality restriction,
                //DataAllValuesFrom) belongs to superClassExpression only, or to no RL production at all.
                default:
                    AddViolation(violations, ClassExpressionRule, axiom,
                        $"Class expression construct '{classExpression.GetType().Name}' is not admitted in subclass position by the RL profile grammar (§4.2.3)",
                        "Remove or rewrite this construct using only Class, ObjectIntersectionOf, ObjectUnionOf, ObjectOneOf, ObjectSomeValuesFrom, ObjectHasValue, DataSomeValuesFrom or DataHasValue");
                    break;
            }
        }

        /// <summary>
        /// superClassExpression := Class(≠owl:Thing) | ObjectIntersectionOf | ObjectComplementOf(subCE) |
        /// ObjectAllValuesFrom | ObjectHasValue | ObjectMaxCardinality(0|1) | DataAllValuesFrom | DataHasValue |
        /// DataMaxCardinality(0|1).
        /// </summary>
        private static void CheckSuperClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, List<OWLProfileViolation> violations)
        {
            switch (classExpression)
            {
                //owl:Thing has already been rejected by the caller (CheckClassExpression), so any other atomic
                //class reaching this branch is a plain, unconditionally admitted superClassExpression.
                case OWLClass _:
                    break;

                //Conjunction is admitted here exactly as it is in subclass position: every conjunct must itself
                //be a valid superClassExpression (intersection does not change which grammar its members obey).
                case OWLObjectIntersectionOf intersectionOf:
                    foreach (OWLClassExpression member in intersectionOf.ClassExpressions)
                        CheckClassExpression(axiom, member, OWLEnums.OWLClassExpressionPosition.SuperClass, violations);
                    break;

                //ObjectComplementOf is admitted ONLY in superclass position, but its operand must satisfy the
                //SUBclass grammar: this is the one point in RL's grammar where the walker must invert position
                //while recursing, exactly like a type-checker flips variance descending into a contravariant slot.
                case OWLObjectComplementOf complementOf:
                    CheckClassExpression(axiom, complementOf.ClassExpression, OWLProfileWalker.InvertPosition(OWLEnums.OWLClassExpressionPosition.SuperClass), violations);
                    break;

                //ObjectAllValuesFrom (∀R.C) is the superclass-side counterpart of ObjectSomeValuesFrom (which
                //belongs to subclass position instead): its filler C stays in SUPERclass position on recursion
                //(unlike ObjectComplementOf above, there is no polarity flip here — universal quantification does
                //not invert the grammar the way negation does).
                case OWLObjectAllValuesFrom allValuesFrom:
                    CheckClassExpression(axiom, allValuesFrom.ClassExpression, OWLEnums.OWLClassExpressionPosition.SuperClass, violations);
                    break;

                //ObjectHasValue (∃R.{a}) and DataHasValue (∃DP.{lit}) are admitted as-is in every position that
                //lists them (sub, super and equiv alike): neither carries a nested class/data range expression
                //to recurse into, since their operand is a concrete individual/literal, not a sub-expression.
                case OWLObjectHasValue _:
                case OWLDataHasValue _:
                    break;

                //ObjectMaxCardinality is admitted only with cardinality 0 or 1 (the grammar's "zeroOrOne"
                //production): "at most 5" is a perfectly normal OWL2 restriction that is simply outside RL.
                //If a qualifying class expression is present, its filler must be owl:Thing or a subCE (same
                //exception pattern as ObjectSomeValuesFrom's filler in subclass position).
                case OWLObjectMaxCardinality maxCardinality:
                    if (!IsZeroOrOne(maxCardinality.Cardinality))
                        AddViolation(violations, ClassExpressionRule, axiom,
                            $"ObjectMaxCardinality({maxCardinality.Cardinality}) is not admitted by the RL profile: only cardinality 0 or 1 is (§4.2.3)",
                            "Reduce the cardinality bound to 0 or 1, or express the restriction differently");
                    else if (maxCardinality.ClassExpression != null && !IsOwlThing(maxCardinality.ClassExpression))
                        CheckClassExpression(axiom, maxCardinality.ClassExpression, OWLEnums.OWLClassExpressionPosition.SubClass, violations);
                    break;

                //DataAllValuesFrom is the data-range counterpart of ObjectAllValuesFrom: admitted, with its
                //range checked against RL's own §4.2.4 DataRange grammar (CheckDataRange), a different
                //production from the ClassExpression one, hence a different recursive method entirely.
                case OWLDataAllValuesFrom dataAllValuesFrom:
                    CheckDataRange(axiom, dataAllValuesFrom.DataRangeExpression, violations);
                    break;

                //DataMaxCardinality mirrors ObjectMaxCardinality above, but on data properties: same "zeroOrOne"
                //cardinality restriction, and the same "check the qualifying range too, when present" logic —
                //except a data range has no owl:Thing-style universal exception, so no IsOwlThing check is needed
                //here, just CheckDataRange directly on whatever range was supplied.
                case OWLDataMaxCardinality dataMaxCardinality:
                    if (!IsZeroOrOne(dataMaxCardinality.Cardinality))
                        AddViolation(violations, ClassExpressionRule, axiom,
                            $"DataMaxCardinality({dataMaxCardinality.Cardinality}) is not admitted by the RL profile: only cardinality 0 or 1 is (§4.2.3)",
                            "Reduce the cardinality bound to 0 or 1, or express the restriction differently");
                    else if (dataMaxCardinality.DataRangeExpression != null)
                        CheckDataRange(axiom, dataMaxCardinality.DataRangeExpression, violations);
                    break;

                //Everything else (ObjectUnionOf, ObjectOneOf, ObjectSomeValuesFrom, ObjectMinCardinality,
                //ObjectExactCardinality, DataSomeValuesFrom, DataMinCardinality, DataExactCardinality) belongs
                //to subClassExpression only, or to no RL production at all.
                default:
                    AddViolation(violations, ClassExpressionRule, axiom,
                        $"Class expression construct '{classExpression.GetType().Name}' is not admitted in superclass position by the RL profile grammar (§4.2.3)",
                        "Remove or rewrite this construct using only Class, ObjectIntersectionOf, ObjectComplementOf, ObjectAllValuesFrom, ObjectHasValue, ObjectMaxCardinality(0/1), DataAllValuesFrom, DataHasValue or DataMaxCardinality(0/1)");
                    break;
            }
        }

        /// <summary>
        /// equivClassExpression := Class(≠owl:Thing) | ObjectIntersectionOf | ObjectHasValue | DataHasValue.
        /// The strictest of the three productions: notably it admits NEITHER union (unlike subCE) NOR universal
        /// quantification/cardinality (unlike superCE) — EquivalentClasses in RL can only equate atomic classes
        /// and conjunctions of atomic-ish constructs, never existentials or restrictions.
        /// </summary>
        private static void CheckEquivClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, List<OWLProfileViolation> violations)
        {
            switch (classExpression)
            {
                //owl:Thing already rejected by the caller; any other atomic class is a valid equivClassExpression.
                case OWLClass _:
                    break;

                //The ONLY composite construct equivClassExpression admits: every conjunct must recurse in the
                //SAME EquivalentClass position (not Sub or Super), keeping the strict grammar closed under nesting.
                case OWLObjectIntersectionOf intersectionOf:
                    foreach (OWLClassExpression member in intersectionOf.ClassExpressions)
                        CheckClassExpression(axiom, member, OWLEnums.OWLClassExpressionPosition.EquivalentClass, violations);
                    break;

                //Admitted as-is, same rationale as in CheckSuperClassExpression above: no nested expression to recurse into.
                case OWLObjectHasValue _:
                case OWLDataHasValue _:
                    break;

                //Everything else — crucially including ObjectSomeValuesFrom and ObjectUnionOf, both perfectly
                //fine in SUBclass position — has no equivClassExpression rendering at all: this production is
                //strictly narrower than either of the other two, not a subset relationship in either direction.
                default:
                    AddViolation(violations, ClassExpressionRule, axiom,
                        $"Class expression construct '{classExpression.GetType().Name}' is not admitted in an EquivalentClasses member by the RL profile grammar (§4.2.3)",
                        "Restrict EquivalentClasses members to Class, ObjectIntersectionOf, ObjectHasValue or DataHasValue");
                    break;
            }
        }

        /// <summary>
        /// Recursively checks a data range against RL's DataRange grammar (§4.2.4: Datatype|DataIntersectionOf —
        /// notably narrower than EL's, which also admits DataOneOf), additionally checking every Datatype leaf
        /// against the §4.2.1 datatype allowlist (a different set from EL's, see AllowedDatatypeIRIs above).
        /// </summary>
        private static void CheckDataRange(OWLAxiom axiom, OWLDataRangeExpression dataRange, List<OWLProfileViolation> violations)
        {
            switch (dataRange)
            {
                //A named datatype is admitted by the DataRange grammar as a construct, but the specific IRI must
                //still belong to RL's §4.2.1 allowlist: e.g. owl:real is a perfectly ordinary data range shape,
                //just not one of the IRIs RL happens to allow (RL's list differs from EL's — see AllowedDatatypeIRIs).
                case OWLDatatype datatype:
                    if (!AllowedDatatypeIRIs.Contains(datatype.GetIRI().ToString()))
                        AddViolation(violations, DatatypeRule, axiom,
                            $"Datatype '{datatype.GetIRI()}' is not part of the RL profile's allowed datatype map (§4.2.1)",
                            "Use one of the datatypes admitted by the RL profile (e.g. xsd:string, xsd:integer, xsd:boolean, xsd:double...)");
                    break;

                //DataIntersectionOf is admitted, with every member recursively required to be an RL-compliant
                //data range in turn (same "inductive grammar" pattern as ObjectIntersectionOf for class expressions).
                case OWLDataIntersectionOf intersectionOf:
                    foreach (OWLDataRangeExpression member in intersectionOf.DataRangeExpressions)
                        CheckDataRange(axiom, member, violations);
                    break;

                //Unlike EL, RL's DataRange grammar does NOT admit DataOneOf: this falls into the default case
                //below along with DataUnionOf, DataComplementOf and DatatypeRestriction.
                default:
                    AddViolation(violations, DataRangeRule, axiom,
                        $"Data range construct '{dataRange.GetType().Name}' is not admitted by the RL profile grammar (§4.2.4)",
                        "Remove or rewrite this construct using only Datatype or DataIntersectionOf");
                    break;
            }
        }

        /// <summary>
        /// §4.2.5: RL admits SubClassOf, EquivalentClasses and DisjointClasses, excluding only DisjointUnion
        /// (same exclusion, same rationale, as EL's — see OWLELProfile.CheckClassAxiomTypes).
        /// </summary>
        private static void CheckClassAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLDisjointUnion disjointUnion in ontology.GetClassAxiomsOfType<OWLDisjointUnion>())
                AddViolation(violations, ClassAxiomTypeRule, disjointUnion,
                    "DisjointUnion is not an axiom type admitted by the RL profile (§4.2.5)",
                    "Replace DisjointUnion(C,(C1,C2,...)) with an equivalent combination of SubClassOf and DisjointClasses axioms");
        }

        /// <summary>
        /// §4.2.5: RL admits SubObjectPropertyOf, EquivalentObjectProperties, DisjointObjectProperties,
        /// InverseObjectProperties, ObjectPropertyDomain, ObjectPropertyRange, Functional/InverseFunctionalObjectProperty,
        /// IrreflexiveObjectProperty, Symmetric/AsymmetricObjectProperty and TransitiveObjectProperty — i.e. every
        /// object property characteristic EXCEPT ReflexiveObjectProperty, which is the only one RL excludes
        /// (reflexivity cannot be expressed as a rule-safe datalog consequent). Contrast with EL, which excludes
        /// almost everything on this list except Reflexive/Transitive: RL and EL are near-complementary here.
        /// </summary>
        private static void CheckObjectPropertyAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLReflexiveObjectProperty reflexive in ontology.GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>())
                AddViolation(violations, ObjectPropertyAxiomTypeRule, reflexive,
                    "ReflexiveObjectProperty is not admitted by the RL profile (§4.2.5): it is the only object property axiom type RL excludes",
                    "Remove this axiom, or model reflexivity via explicit ObjectPropertyAssertion(OP,I,I) assertions instead");
        }

        /// <summary>
        /// §4.2.5: RL admits SubDataPropertyOf, EquivalentDataProperties, DisjointDataProperties, DataPropertyDomain,
        /// DataPropertyRange and FunctionalDataProperty — which is the complete set of OWLDataPropertyAxiom subtypes
        /// that exist today, so this check cannot currently fire. It exists as a forward guard: if a new
        /// OWLDataPropertyAxiom subtype is ever added to the model without being added to RL's grammar too,
        /// it will default to "excluded" here rather than silently passing as compliant.
        /// </summary>
        private static void CheckDataPropertyAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLDataPropertyAxiom axiom in ontology.DataPropertyAxioms ?? new List<OWLDataPropertyAxiom>())
            {
                bool isAdmitted = axiom is OWLSubDataPropertyOf
                                   || axiom is OWLEquivalentDataProperties
                                   || axiom is OWLDisjointDataProperties
                                   || axiom is OWLDataPropertyDomain
                                   || axiom is OWLDataPropertyRange
                                   || axiom is OWLFunctionalDataProperty;

                if (!isAdmitted)
                    AddViolation(violations, DataPropertyAxiomTypeRule, axiom,
                        $"Data property axiom type '{axiom.GetType().Name}' is not admitted by the RL profile (§4.2.5)",
                        "Remove this axiom, or restrict the ontology to SubDataPropertyOf, EquivalentDataProperties, DisjointDataProperties, DataPropertyDomain, DataPropertyRange and FunctionalDataProperty");
            }
        }

        //Shared by every "the filler must be owl:Thing OR a subCE" check above (ObjectSomeValuesFrom in
        //CheckSubClassExpression, ObjectMaxCardinality's optional qualifying class in CheckSuperClassExpression):
        //factored out so both call sites test the exact same condition instead of duplicating the IRI comparison,
        //and so a future third call site (if the grammar ever grows another such exception) has one place to reuse.
        private static bool IsOwlThing(OWLClassExpression classExpression)
            => classExpression is OWLClass classExpr && classExpr.GetIRI().Equals(RDFVocabulary.OWL.THING);

        //The "zeroOrOne" grammar production (§4.2.3): Cardinality is stored as a string (mirroring the OWL2/XML
        //serialization, which encodes xsd:nonNegativeInteger as text), so it is compared against "0"/"1" as
        //strings rather than parsed to an integer — cheaper, and cardinalities are always non-negative by
        //construction (OWLObjectMaxCardinality's ctor only accepts a uint), so no other textual form can occur.
        private static bool IsZeroOrOne(string cardinality)
            => cardinality == "0" || cardinality == "1";

        //Single choke point for constructing a violation, so every call site stays a one-liner and the
        //"embed the offending axiom's OWL2/XML signature into the description" convention (see OWLELProfile's
        //AddViolation for the full rationale — same convention the Validator's RuleSet classes use) only needs
        //to be implemented once per profiler, not repeated at every AddViolation call above.
        private static void AddViolation(List<OWLProfileViolation> violations, string ruleName, OWLAxiom offendingAxiom, string description, string suggestion)
            => violations.Add(new OWLProfileViolation(OWLEnums.OWLProfiles.RL, ruleName, $"{description} — offending axiom: '{offendingAxiom.GetXML()}'", suggestion));
    }
}