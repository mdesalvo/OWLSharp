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
using System.Threading.Tasks;
using OWLSharp.Ontology;
using OWLSharp.Profiler.Walkers;
using RDFSharp.Model;

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLQLProfile checks an ontology against the OWL 2 QL profile grammar (https://www.w3.org/TR/owl2-profiles/#OWL_2_QL, §3.2).
    /// QL is designed so that conjunctive query answering can be reduced to non-recursive datalog ("FO-rewritability"):
    /// every construct that would force a reasoner to branch or search (union, cardinality, arbitrary nesting of
    /// existentials) is excluded, and the one construct QL DOES keep on both sides of SubClassOf — ObjectSomeValuesFrom
    /// — is deliberately asymmetric in SHAPE, not just in which side admits it: unqualified (∃R.owl:Thing) in
    /// subclass position, qualified with a plain atomic Class (∃R.A) in superclass position. This is the single
    /// most fragile point of the three profiles' grammars (see the owl2_profiles_w3c_spec memory note) — this
    /// implementation was re-verified against a second, independent fetch of §3.2.3/§3.2.5 before being written,
    /// specifically to pin down this qualification asymmetry with confidence rather than trusting the first pass.
    /// </summary>
    internal static class OWLQLProfile
    {
        private const string ClassExpressionRule = nameof(OWLEnums.OWLProfiles.QL) + ".ClassExpression";                 //§3.2.3
        private const string DataRangeRule = nameof(OWLEnums.OWLProfiles.QL) + ".DataRange";                             //§3.2.4
        private const string ClassAxiomTypeRule = nameof(OWLEnums.OWLProfiles.QL) + ".ClassAxiomType";                   //§3.2.5
        private const string ObjectPropertyAxiomTypeRule = nameof(OWLEnums.OWLProfiles.QL) + ".ObjectPropertyAxiomType"; //§3.2.5
        private const string DataPropertyAxiomTypeRule = nameof(OWLEnums.OWLProfiles.QL) + ".DataPropertyAxiomType";     //§3.2.5
        private const string AssertionAxiomTypeRule = nameof(OWLEnums.OWLProfiles.QL) + ".AssertionAxiomType";           //§3.2.5
        private const string DatatypeRule = nameof(OWLEnums.OWLProfiles.QL) + ".Datatype";                               //§3.2.1

        //§3.2.1: the spec extract records QL's datatype allowlist as IDENTICAL to EL's (both share the same
        //"OWL 2 EL and QL datatype map" table in the spec, distinct from RL's — see OWLRLProfile.AllowedDatatypeIRIs
        //for the genuinely different RL list). Reused DIRECTLY from OWLELProfile's field (made `internal`
        //specifically to serve as this shared base) rather than re-declared item-by-item here: since the two
        //sets are asserted by the spec to be exactly equal, re-typing all 19 entries a second time would only
        //create a second place that could silently drift out of sync with the first if either list is ever
        //edited without noticing the other. If a future spec erratum ever actually splits EL's and QL's
        //datatype maps, this is the one line to change (replace the reference with QL's own literal set).
        private static readonly HashSet<string> AllowedDatatypeIRIs = OWLELProfile.AllowedDatatypeIRIs;

        //See OWLProfiler.ExecuteProfileRuleAsync / feedback_genuine_async: Task-returning end-to-end, so the
        //whole dispatch chain from the public CheckProfileAsync entry point down to here is genuinely awaited.
        internal static Task<List<OWLProfileViolation>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLProfileViolation> violations = new List<OWLProfileViolation>();

            //§3.2.3: every top-level class expression slot from SubClassOf/EquivalentClasses/DisjointClasses.
            //Note the position mapping performed inside CheckClassExpression below: QL, unlike RL, has NO
            //third "equivClassExpression" production of its own — EquivalentClasses(C1,C2,...) in QL uses the
            //subClassExpression grammar for every member (per §3.2.5's EquivalentClasses production, which
            //takes "subClassExpression subClassExpression {subClassExpression}"), so the walker's generic
            //EquivalentClass tag is folded back onto the SubClass grammar here, not given its own method the
            //way OWLRLProfile.CheckEquivClassExpression does.
            foreach ((OWLClassAxiom axiom, OWLClassExpression classExpression, OWLEnums.OWLClassExpressionPosition position) in OWLClassAxiomWalker.WalkClassAxioms(ontology))
                CheckClassExpression(axiom, classExpression, position, violations);

            //...plus the class expressions embedded in property domain/range axioms, which OWLClassAxiomWalker
            //does not enumerate (its documented scope is ClassAxioms only). As in OWLRLProfile, the domain/range
            //class of a property axiom plays the SUPERclass role (ObjectPropertyDomain(OP,C) is structurally
            //SubClassOf(∃OP.owl:Thing,C), i.e. C is the consequent).
            //Delegated to the shared OWLPropertyAxiomWalker (see OWLRLProfile for the identical structural
            //"domain/range class always plays the superclass role" rationale, and the walker's own XML-doc for
            //why it stays position-agnostic and lets each profiler apply SuperClass at the call site).
            foreach ((OWLAxiom axiom, OWLClassExpression classExpression) in OWLPropertyAxiomWalker.WalkPropertyDomainRangeClassExpressions(ontology))
                CheckClassExpression(axiom, classExpression, OWLEnums.OWLClassExpressionPosition.SuperClass, violations);

            //§3.2.4 DataRange grammar, for DataPropertyRange(DP,DR), which states its range directly as a DR
            //rather than reaching one through a class expression (identical rationale to OWLELProfile/OWLRLProfile).
            foreach ((OWLAxiom axiom, OWLDataRangeExpression dataRange) in OWLPropertyAxiomWalker.WalkPropertyRangeDataRanges(ontology))
                CheckDataRange(axiom, dataRange, violations);

            //§3.2.5 allowed axiom types, independent of grammar shape.
            CheckClassAxiomTypes(ontology, violations);
            CheckObjectPropertyAxiomTypes(ontology, violations);
            CheckDataPropertyAxiomTypes(ontology, violations);
            CheckAssertionAxiomTypes(ontology, violations);
            CheckKeyAxioms(ontology, violations);

            return Task.FromResult(violations);
        }

        /// <summary>
        /// Dispatches to the sub/superClassExpression grammar for the given position, folding QL's
        /// EquivalentClass tag onto the subClassExpression grammar (see the rationale in ExecuteRuleAsync above:
        /// QL, unlike RL, has no distinct equivClassExpression production of its own).
        /// </summary>
        private static void CheckClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, OWLEnums.OWLClassExpressionPosition position, List<OWLProfileViolation> violations)
        {
            //QL's grammar, unlike RL's, does NOT single out owl:Thing as a forbidden atomic class expression:
            //it only constrains WHICH constructs are admitted and, for ObjectSomeValuesFrom, what shape the
            //filler takes — there is no general "Class other than owl:Thing" clause in §3.2.3. So, unlike
            //OWLRLProfile.CheckClassExpression, there is no owl:Thing pre-check here before dispatching.
            switch (position)
            {
                //Superclass slots (the RHS of SubClassOf, or a property domain/range) get QL's wider,
                //but still position-specific, superClassExpression grammar.
                case OWLEnums.OWLClassExpressionPosition.SuperClass:
                    CheckSuperClassExpression(axiom, classExpression, violations);
                    break;
                default:
                    //Both SubClass and EquivalentClass positions share the exact same subClassExpression grammar
                    //in QL, so both fall through to the same helper — this "default" is not a safety net for an
                    //unhandled case (as it would be in a 3-way RL-style switch), it is a deliberate 2-into-1 merge.
                    CheckSubClassExpression(axiom, classExpression, violations);
                    break;
            }
        }

        /// <summary>
        /// subClassExpression := Class | ObjectSomeValuesFrom(OPE, owl:Thing) [UNqualified — the filler is
        /// literally the constant owl:Thing, not a variable class expression] | DataSomeValuesFrom.
        /// The narrowest of QL's two productions: no ObjectIntersectionOf, no ObjectComplementOf, and the
        /// existential's filler is fixed rather than recursively checked, since it can only ever be owl:Thing.
        /// </summary>
        private static void CheckSubClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, List<OWLProfileViolation> violations)
        {
            switch (classExpression)
            {
                //A plain named class is always admitted here: it is the base case of both QL productions.
                case OWLClass _:
                    break;

                //Per §3.2.3's subObjectSomeValuesFrom production, the filler is not a recursively-checked
                //class expression at all: it MUST be exactly owl:Thing. Anything else — including a filler that
                //would be perfectly fine in EL or RL's subclass position, like another named class — is a
                //violation, because a qualified existential on the antecedent side would make query answering
                //require a join QL is specifically designed to avoid (that's the whole point of the asymmetry
                //with superclass position below, where the filler IS a class).
                case OWLObjectSomeValuesFrom someValuesFrom:
                    if (!IsOwlThing(someValuesFrom.ClassExpression))
                        AddViolation(violations, ClassExpressionRule, axiom,
                            $"ObjectSomeValuesFrom in subclass position must be filled with owl:Thing in the QL profile (unqualified existential only, §3.2.3); found filler of type '{someValuesFrom.ClassExpression.GetType().Name}' instead",
                            "Replace the filler with owl:Thing (an unqualified existential), or move this construct to superclass position where a qualified filler is admitted");
                    break;

                //DataSomeValuesFrom is admitted in BOTH positions (see CheckSuperClassExpression below), with
                //no owl:Thing-style restriction on the data side: its range must satisfy QL's own §3.2.4
                //DataRange grammar (Datatype|DataIntersectionOf), checked via the same CheckDataRange used by
                //DataPropertyRange axioms.
                case OWLDataSomeValuesFrom dataSomeValuesFrom:
                    CheckDataRange(axiom, dataSomeValuesFrom.DataRangeExpression, violations);
                    break;

                //Everything else — including ObjectIntersectionOf and ObjectComplementOf, both superclass-only
                //in QL (the opposite division of labour from RL, where intersection is admitted on BOTH sides) —
                //has no subClassExpression rendering.
                default:
                    AddViolation(violations, ClassExpressionRule, axiom,
                        $"Class expression construct '{classExpression.GetType().Name}' is not admitted in subclass position by the QL profile grammar (§3.2.3)",
                        "Remove or rewrite this construct using only Class, ObjectSomeValuesFrom(OPE,owl:Thing) or DataSomeValuesFrom");
                    break;
            }
        }

        /// <summary>
        /// superClassExpression := Class | ObjectIntersectionOf(superCE+) | ObjectComplementOf(subCE)
        /// [polarity inversion] | ObjectSomeValuesFrom(OPE, Class) [qualified, and — per §3.2.3's literal
        /// "Class" production — with an ATOMIC class filler only, not an arbitrary composite superClassExpression]
        /// | DataSomeValuesFrom.
        /// </summary>
        private static void CheckSuperClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, List<OWLProfileViolation> violations)
        {
            switch (classExpression)
            {
                //A plain named class is unconditionally admitted here: it is the shared base case of both
                //QL productions (sub and super alike), and — unlike RL — QL's grammar never singles out
                //owl:Thing as a forbidden atomic class, so no extra check is needed beyond "it is an OWLClass".
                case OWLClass _:
                    break;

                //Conjunction is admitted here, and ONLY here (contrast with RL, which admits it in every
                //position): every conjunct must itself be a valid superClassExpression.
                case OWLObjectIntersectionOf intersectionOf:
                    foreach (OWLClassExpression member in intersectionOf.ClassExpressions)
                        CheckClassExpression(axiom, member, OWLEnums.OWLClassExpressionPosition.SuperClass, violations);
                    break;

                //ObjectComplementOf is admitted ONLY in superclass position, wrapping an operand that must
                //satisfy the SUBclass grammar instead — the one point in QL's grammar where the walker must
                //invert position while recursing, structurally identical to RL's ObjectComplementOf handling
                //(see OWLRLProfile.CheckSuperClassExpression for the fuller "type-checker variance" analogy).
                case OWLObjectComplementOf complementOf:
                    CheckClassExpression(axiom, complementOf.ClassExpression, OWLProfileWalker.InvertPosition(OWLEnums.OWLClassExpressionPosition.SuperClass), violations);
                    break;

                //This is the crux of QL's grammar and the reason this file underwent an independent re-verification
                //pass before being written: the spec's superObjectSomeValuesFrom production is literally
                //"ObjectSomeValuesFrom(ObjectPropertyExpression Class)" — the filler slot is typed "Class", NOT
                //"superClassExpression". So unlike ObjectAllValuesFrom's filler in RL (which recurses through the
                //full superClassExpression grammar), QL's ∃R.C filler must be a single ATOMIC named class: even
                //an otherwise-perfectly-admitted ObjectIntersectionOf as the filler is out of grammar here.
                case OWLObjectSomeValuesFrom someValuesFrom:
                    if (!(someValuesFrom.ClassExpression is OWLClass))
                        AddViolation(violations, ClassExpressionRule, axiom,
                            $"ObjectSomeValuesFrom in superclass position must be filled with a single atomic Class in the QL profile (qualified existential over a named class only, §3.2.3); found filler of type '{someValuesFrom.ClassExpression.GetType().Name}' instead",
                            "Replace the filler with a single named class (not a composite expression), or move the existential to subclass position with an owl:Thing filler");
                    break;

                //DataSomeValuesFrom is shared verbatim with subclass position (see CheckSubClassExpression):
                //QL's asymmetry is specific to the OBJECT existential, not the data one.
                case OWLDataSomeValuesFrom dataSomeValuesFrom:
                    CheckDataRange(axiom, dataSomeValuesFrom.DataRangeExpression, violations);
                    break;

                //Everything else — including ObjectUnionOf, ObjectOneOf, every cardinality restriction,
                //ObjectAllValuesFrom, ObjectHasValue and DataHasValue, all of which at least one of EL/RL admits
                //somewhere — has no superClassExpression rendering in QL: this is the narrowest of the three
                //profiles' superclass-side grammars.
                default:
                    AddViolation(violations, ClassExpressionRule, axiom,
                        $"Class expression construct '{classExpression.GetType().Name}' is not admitted in superclass position by the QL profile grammar (§3.2.3)",
                        "Remove or rewrite this construct using only Class, ObjectIntersectionOf, ObjectComplementOf, ObjectSomeValuesFrom(OPE,Class) or DataSomeValuesFrom");
                    break;
            }
        }

        /// <summary>
        /// Recursively checks a data range against QL's DataRange grammar (§3.2.4: Datatype|DataIntersectionOf —
        /// the same narrower shape as RL's, excluding DataOneOf which only EL admits), additionally checking
        /// every Datatype leaf against the §3.2.1 datatype allowlist (shared with EL's list, per the spec extract).
        /// </summary>
        private static void CheckDataRange(OWLAxiom axiom, OWLDataRangeExpression dataRange, List<OWLProfileViolation> violations)
        {
            switch (dataRange)
            {
                //A named datatype is admitted as a construct, but the specific IRI must belong to QL's §3.2.1
                //allowlist (the EL/QL shared list — see AllowedDatatypeIRIs above, and contrast with RL's own,
                //genuinely different list in OWLRLProfile).
                case OWLDatatype datatype:
                    if (!AllowedDatatypeIRIs.Contains(datatype.GetIRI().ToString()))
                        AddViolation(violations, DatatypeRule, axiom,
                            $"Datatype '{datatype.GetIRI()}' is not part of the QL profile's allowed datatype map (§3.2.1)",
                            "Use one of the datatypes admitted by the QL profile (e.g. xsd:string, xsd:integer, xsd:dateTime, owl:real...)");
                    break;

                //DataIntersectionOf is admitted, with every member recursively required to be a QL-compliant
                //data range in turn (same inductive pattern used throughout all three profilers).
                case OWLDataIntersectionOf intersectionOf:
                    foreach (OWLDataRangeExpression member in intersectionOf.DataRangeExpressions)
                        CheckDataRange(axiom, member, violations);
                    break;

                //Everything else (DataOneOf — admitted by EL but not QL/RL —, DataUnionOf, DataComplementOf,
                //DatatypeRestriction) falls outside QL's DataRange grammar.
                default:
                    AddViolation(violations, DataRangeRule, axiom,
                        $"Data range construct '{dataRange.GetType().Name}' is not admitted by the QL profile grammar (§3.2.4)",
                        "Remove or rewrite this construct using only Datatype or DataIntersectionOf");
                    break;
            }
        }

        /// <summary>
        /// §3.2.5: QL admits SubClassOf, EquivalentClasses and DisjointClasses (both of the latter built entirely
        /// out of subClassExpression members — see the rationale in ExecuteRuleAsync), excluding only DisjointUnion,
        /// same exclusion and rationale as EL/RL.
        /// </summary>
        private static void CheckClassAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLDisjointUnion disjointUnion in ontology.GetClassAxiomsOfType<OWLDisjointUnion>())
                AddViolation(violations, ClassAxiomTypeRule, disjointUnion,
                    "DisjointUnion is not an axiom type admitted by the QL profile (§3.2.5)",
                    "Replace DisjointUnion(C,(C1,C2,...)) with an equivalent combination of SubClassOf and DisjointClasses axioms");
        }

        /// <summary>
        /// §3.2.5: QL admits SubObjectPropertyOf (WITHOUT property chains — see the dedicated chain check below),
        /// EquivalentObjectProperties, DisjointObjectProperties, InverseObjectProperties, ObjectPropertyDomain,
        /// ObjectPropertyRange, ReflexiveObjectProperty, SymmetricObjectProperty and AsymmetricObjectProperty.
        /// Excluded: FunctionalObjectProperty, InverseFunctionalObjectProperty, TransitiveObjectProperty and
        /// IrreflexiveObjectProperty — functionality/transitivity both break FO-rewritability (a functional or
        /// transitive property can force unbounded chase-style reasoning, exactly what QL trades away).
        /// </summary>
        private static void CheckObjectPropertyAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLObjectPropertyAxiom axiom in ontology.ObjectPropertyAxioms ?? new List<OWLObjectPropertyAxiom>())
            {
                //SubObjectPropertyOf is admitted only in its single-property form: a chain-based one
                //(SubObjectPropertyExpression left null, SubObjectPropertyChain populated instead — see
                //OWLSubObjectPropertyOf's two constructors) is excluded outright by §3.2.5's SubObjectPropertyOf
                //production, which only lists a single ObjectPropertyExpression on each side, never a chain.
                if (axiom is OWLSubObjectPropertyOf subObjectPropertyOf && subObjectPropertyOf.SubObjectPropertyChain != null)
                {
                    AddViolation(violations, ObjectPropertyAxiomTypeRule, axiom,
                        "SubObjectPropertyOf built from a property chain is not admitted by the QL profile (§3.2.5): only a single-property SubObjectPropertyOf is",
                        "Remove the property chain, or restrict it to a single object property expression on the subproperty side");
                    continue;
                }

                bool isAdmitted = axiom is OWLSubObjectPropertyOf
                                   || axiom is OWLEquivalentObjectProperties
                                   || axiom is OWLDisjointObjectProperties
                                   || axiom is OWLInverseObjectProperties
                                   || axiom is OWLObjectPropertyDomain
                                   || axiom is OWLObjectPropertyRange
                                   || axiom is OWLReflexiveObjectProperty
                                   || axiom is OWLSymmetricObjectProperty
                                   || axiom is OWLAsymmetricObjectProperty;

                if (!isAdmitted)
                    AddViolation(violations, ObjectPropertyAxiomTypeRule, axiom,
                        $"Object property axiom type '{axiom.GetType().Name}' is not admitted by the QL profile (§3.2.5)",
                        "Remove this axiom, or restrict the ontology to SubObjectPropertyOf, EquivalentObjectProperties, DisjointObjectProperties, InverseObjectProperties, ObjectPropertyDomain, ObjectPropertyRange, ReflexiveObjectProperty, SymmetricObjectProperty and AsymmetricObjectProperty");
            }
        }

        /// <summary>
        /// §3.2.5: QL admits SubDataPropertyOf, EquivalentDataProperties, DisjointDataProperties, DataPropertyDomain
        /// and DataPropertyRange, excluding only FunctionalDataProperty — for the same FO-rewritability reason
        /// FunctionalObjectProperty is excluded above.
        /// </summary>
        private static void CheckDataPropertyAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLDataPropertyAxiom axiom in ontology.DataPropertyAxioms ?? new List<OWLDataPropertyAxiom>())
            {
                bool isAdmitted = axiom is OWLSubDataPropertyOf
                                   || axiom is OWLEquivalentDataProperties
                                   || axiom is OWLDisjointDataProperties
                                   || axiom is OWLDataPropertyDomain
                                   || axiom is OWLDataPropertyRange;

                if (!isAdmitted)
                    AddViolation(violations, DataPropertyAxiomTypeRule, axiom,
                        $"Data property axiom type '{axiom.GetType().Name}' is not admitted by the QL profile (§3.2.5)",
                        "Remove this axiom, or restrict the ontology to SubDataPropertyOf, EquivalentDataProperties, DisjointDataProperties, DataPropertyDomain and DataPropertyRange");
            }
        }

        /// <summary>
        /// §3.2.5 Assertions: QL admits DifferentIndividuals, ClassAssertion, ObjectPropertyAssertion and
        /// DataPropertyAssertion, excluding SameIndividual and both negative property assertion types outright
        /// as axiom types — SameIndividual would let a query answer conflate two named individuals into one
        /// answer binding, and negative assertions require closed-world reasoning that FO-rewriting cannot
        /// express. ClassAssertion additionally carries a narrower-than-usual restriction (confirmed by the
        /// second verification fetch): the class being asserted must be a plain atomic Class, never a composite
        /// class expression — unlike EL/RL, which impose no such restriction on ClassAssertion at all.
        /// </summary>
        private static void CheckAssertionAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLAssertionAxiom axiom in ontology.AssertionAxioms ?? new List<OWLAssertionAxiom>())
            {
                //ClassAssertion is admitted as an axiom TYPE, but only with an atomic class: checked separately
                //from the generic isAdmitted flag below, since it needs its own, more specific violation message
                //(pointing at the composite-class problem, not at "this axiom type doesn't exist in QL" — the
                //axiom type itself is fine, only its shape here is not).
                if (axiom is OWLClassAssertion classAssertion)
                {
                    if (!(classAssertion.ClassExpression is OWLClass))
                        AddViolation(violations, AssertionAxiomTypeRule, axiom,
                            $"ClassAssertion is only admitted by the QL profile with a plain atomic Class (§3.2.5); found a composite class expression of type '{classAssertion.ClassExpression.GetType().Name}' instead",
                            "Assert membership against a plain named class, not a composite class expression (ObjectIntersectionOf, ObjectSomeValuesFrom, ...)");
                    continue;
                }

                bool isAdmitted = axiom is OWLDifferentIndividuals
                                   || axiom is OWLObjectPropertyAssertion
                                   || axiom is OWLDataPropertyAssertion;

                if (!isAdmitted)
                    AddViolation(violations, AssertionAxiomTypeRule, axiom,
                        $"Assertion axiom type '{axiom.GetType().Name}' is not admitted by the QL profile (§3.2.5)",
                        "Remove this axiom, or restrict the ontology to DifferentIndividuals, ClassAssertion (atomic classes only), ObjectPropertyAssertion and DataPropertyAssertion — SameIndividual and negative property assertions are excluded entirely");
            }
        }

        /// <summary>
        /// §3.2.5 explicitly excludes keys: HasKey axioms (a separate ontology.KeyAxioms list, not part of
        /// AssertionAxioms in this model — see CLAUDE.md's description of OWLOntology's typed axiom lists) have
        /// no rendering in QL at all, regardless of which properties they key on.
        /// </summary>
        private static void CheckKeyAxioms(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLHasKey hasKey in ontology.KeyAxioms ?? new List<OWLHasKey>())
                AddViolation(violations, AssertionAxiomTypeRule, hasKey,
                    "HasKey is not admitted by the QL profile (§3.2.5): key constraints are excluded entirely",
                    "Remove this axiom; QL has no rendering for uniqueness/key constraints");
        }

        //Shared by both ObjectSomeValuesFrom filler checks that treat owl:Thing as a fixed constant rather than
        //a recursively-checked class expression (CheckSubClassExpression's mandatory owl:Thing filler here;
        //contrast with CheckSuperClassExpression's ObjectSomeValuesFrom, which instead requires an atomic
        //non-owl:Thing-specific Class and so does NOT call this helper).
        private static bool IsOwlThing(OWLClassExpression classExpression)
            => classExpression is OWLClass classExpr && classExpr.GetIRI().Equals(RDFVocabulary.OWL.THING);

        //Single choke point for constructing a violation: keeps every call site above a short, readable
        //one-liner, and embeds the offending axiom's own OWL2/XML signature into the description (same
        //traceability convention as OWLELProfile/OWLRLProfile and the Validator's RuleSet classes).
        private static void AddViolation(List<OWLProfileViolation> violations, string ruleName, OWLAxiom offendingAxiom, string description, string suggestion)
            => violations.Add(new OWLProfileViolation(OWLEnums.OWLProfiles.QL, ruleName, $"{description} — offending axiom: '{offendingAxiom.GetXML()}'", suggestion));
    }
}
