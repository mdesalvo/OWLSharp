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
    /// OWLELProfile checks an ontology against the OWL 2 EL profile grammar (https://www.w3.org/TR/owl2-profiles/#OWL_2_EL, §2.2).
    /// Unlike QL and RL, EL does not distinguish subClassExpression/superClassExpression: its ClassExpression grammar is unique,
    /// so every class expression slot in the ontology is checked with the same predicate regardless of where it occurs.
    /// </summary>
    internal static class OWLELProfile
    {
        private const string ClassExpressionRule = nameof(OWLEnums.OWLProfiles.EL) + ".ClassExpression";                 //§2.2.3
        private const string DataRangeRule = nameof(OWLEnums.OWLProfiles.EL) + ".DataRange";                             //§2.2.4
        private const string ClassAxiomTypeRule = nameof(OWLEnums.OWLProfiles.EL) + ".ClassAxiomType";                   //§2.2.5
        private const string ObjectPropertyAxiomTypeRule = nameof(OWLEnums.OWLProfiles.EL) + ".ObjectPropertyAxiomType"; //§2.2.5
        private const string DataPropertyAxiomTypeRule = nameof(OWLEnums.OWLProfiles.EL) + ".DataPropertyAxiomType";     //§2.2.5
        private const string DatatypeRule = nameof(OWLEnums.OWLProfiles.EL) + ".Datatype";                               //§2.2.1

        //§2.2.1: the exact datatype allowlist transcribed from the spec extract (see owl2_profiles_w3c_spec memory
        //note). Matched against OWLDatatype.GetIRI().ToString(), i.e. the full absolute IRI, not a prefixed name,
        //so it is agnostic to how the ontology chose to abbreviate the datatype in its own serialization.
        internal static readonly HashSet<string> AllowedDatatypeIRIs = new HashSet<string>
        {
            RDFVocabulary.RDF.PLAIN_LITERAL.ToString(),
            RDFVocabulary.RDF.XML_LITERAL.ToString(),
            RDFVocabulary.RDFS.LITERAL.ToString(),
            RDFVocabulary.OWL.REAL.ToString(),
            $"{RDFVocabulary.OWL.BASE_URI}rational",
            RDFVocabulary.XSD.DECIMAL.ToString(),
            RDFVocabulary.XSD.INTEGER.ToString(),
            RDFVocabulary.XSD.NON_NEGATIVE_INTEGER.ToString(),
            RDFVocabulary.XSD.STRING.ToString(),
            RDFVocabulary.XSD.NORMALIZED_STRING.ToString(),
            RDFVocabulary.XSD.TOKEN.ToString(),
            RDFVocabulary.XSD.NAME.ToString(),
            RDFVocabulary.XSD.NCNAME.ToString(),
            RDFVocabulary.XSD.NMTOKEN.ToString(),
            RDFVocabulary.XSD.HEX_BINARY.ToString(),
            RDFVocabulary.XSD.BASE64_BINARY.ToString(),
            RDFVocabulary.XSD.ANY_URI.ToString(),
            RDFVocabulary.XSD.DATETIME.ToString(),
            RDFVocabulary.XSD.DATETIMESTAMP.ToString()
        };

        //See OWLProfiler.ExecuteProfileRuleAsync / feedback_genuine_async: Task-returning end-to-end, not just
        //at the outer CheckProfileAsync boundary. The body below is synchronous CPU-bound work (pure grammar
        //pattern-matching over an already in-memory ontology), so it is wrapped in Task.FromResult at the very end.
        internal static Task<List<OWLProfileViolation>> ExecuteRuleAsync(OWLOntology ontology)
        {
            List<OWLProfileViolation> violations = new List<OWLProfileViolation>();

            //§2.2.3 ClassExpression grammar: every top-level slot inside SubClassOf/EquivalentClasses/DisjointClasses...
            foreach ((OWLClassAxiom axiom, OWLClassExpression classExpression, OWLEnums.OWLClassExpressionPosition _) in OWLClassAxiomWalker.WalkClassAxioms(ontology))
                CheckClassExpression(axiom, classExpression, violations);

            //...plus the class expressions embedded in property domain/range axioms, which are NOT enumerated by
            //OWLClassAxiomWalker (that walker's documented scope is ClassAxioms only) but still carry a class
            //expression that must conform to the same §2.2.3 grammar. Delegated to OWLPropertyAxiomWalker so this
            //iteration is written once and shared by OWLRLProfile/OWLQLProfile instead of being re-hand-rolled here.
            foreach ((OWLAxiom axiom, OWLClassExpression classExpression) in OWLPropertyAxiomWalker.WalkPropertyDomainRangeClassExpressions(ontology))
                CheckClassExpression(axiom, classExpression, violations);

            //§2.2.4 DataRange grammar, for the one axiom family that carries a data range without going through
            //a class expression first (DataPropertyRange(DP,DR) states the range of DP directly as a DR, unlike
            //DataSomeValuesFrom/DataHasValue whose data range is reached via CheckClassExpression's recursion).
            foreach ((OWLAxiom axiom, OWLDataRangeExpression dataRange) in OWLPropertyAxiomWalker.WalkPropertyRangeDataRanges(ontology))
                CheckDataRange(axiom, dataRange, violations);

            //§2.2.5 allowed axiom types, checked independently of grammar shape: an axiom can use only
            //EL-admitted class/data expressions internally (checked above) AND still be of a type EL disallows
            //outright (e.g. DisjointUnion, which has no valid EL rendering regardless of its members' shape).
            CheckClassAxiomTypes(ontology, violations);
            CheckObjectPropertyAxiomTypes(ontology, violations);
            CheckDataPropertyAxiomTypes(ontology, violations);

            return Task.FromResult(violations);
        }

        /// <summary>
        /// Recursively checks a class expression (and, through it, any nested data range) against EL's unique
        /// ClassExpression grammar (§2.2.3: Class|ObjectIntersectionOf|ObjectOneOf[singleton]|ObjectSomeValuesFrom|
        /// ObjectHasValue|ObjectHasSelf|DataSomeValuesFrom|DataHasValue). Constructs outside this list are flagged
        /// and NOT recursed into further: the outer construct is already the violation, so descending into its
        /// children would only produce redundant/misleading nested violations for a subtree that is moot anyway.
        /// </summary>
        private static void CheckClassExpression(OWLAxiom axiom, OWLClassExpression classExpression, List<OWLProfileViolation> violations)
        {
            switch (classExpression)
            {
                //A plain named class is always EL-compliant: it is the base case of the grammar.
                case OWLClass _:
                    break;

                //ObjectIntersectionOf is admitted, but only if every member is itself EL-compliant:
                //EL's polynomial classification relies on conjunction being "safe" to flatten, which requires
                //each conjunct to already be in the fragment.
                case OWLObjectIntersectionOf intersectionOf:
                    foreach (OWLClassExpression member in intersectionOf.ClassExpressions)
                        CheckClassExpression(axiom, member, violations);
                    break;

                //ObjectOneOf is admitted only as a SINGLETON ({a}, not {a,b,...}): a multi-individual enumeration
                //is semantically a disjunction of nominals, which EL excludes for the same reason it excludes
                //ObjectUnionOf (see below).
                case OWLObjectOneOf oneOf:
                    if (oneOf.IndividualExpressions == null || oneOf.IndividualExpressions.Count != 1)
                        AddViolation(violations, ClassExpressionRule, axiom,
                            $"ObjectOneOf with {oneOf.IndividualExpressions?.Count ?? 0} individuals is not admitted by the EL profile: only a singleton ObjectOneOf({{a}}) is (§2.2.3)",
                            "Reduce the enumeration to a single individual, or express membership via a class assertion instead of an enumerated class");
                    break;

                //ObjectSomeValuesFrom (∃R.C) is EL's flagship construct: admitted, but its filler C must recurse
                //through this same check, since EL's ClassExpression grammar is defined inductively (an existential
                //over a non-EL filler is itself outside the fragment).
                case OWLObjectSomeValuesFrom someValuesFrom:
                    CheckClassExpression(axiom, someValuesFrom.ClassExpression, violations);
                    break;

                //ObjectHasValue (∃R.{a}) and ObjectHasSelf (∃R.Self) are admitted as-is: neither carries a nested
                //class expression to recurse into (their operand is an individual / the property itself).
                case OWLObjectHasValue _:
                case OWLObjectHasSelf _:
                    break;

                //DataSomeValuesFrom is admitted, but its data range must satisfy the separate §2.2.4 grammar
                //(a different production, hence a different recursive check: CheckDataRange, not CheckClassExpression).
                case OWLDataSomeValuesFrom dataSomeValuesFrom:
                    CheckDataRange(axiom, dataSomeValuesFrom.DataRangeExpression, violations);
                    break;

                //DataHasValue (∃DP.{lit}) is admitted as-is: its operand is a concrete literal, not a data range,
                //so there is nothing further to recurse into.
                case OWLDataHasValue _:
                    break;

                //Everything else (ObjectUnionOf, ObjectComplementOf, ObjectAllValuesFrom, every cardinality
                //restriction, and any future class expression type not yet accounted for) falls outside EL's
                //grammar: EL deliberately excludes universal quantification, negation/disjunction and counting,
                //because admitting them would break the polynomial-time classification guarantee.
                default:
                    AddViolation(violations, ClassExpressionRule, axiom,
                        $"Class expression construct '{classExpression.GetType().Name}' is not admitted by the EL profile grammar (§2.2.3)",
                        "Remove or rewrite this construct using only Class, ObjectIntersectionOf, singleton ObjectOneOf, ObjectSomeValuesFrom, ObjectHasValue, ObjectHasSelf, DataSomeValuesFrom or DataHasValue");
                    break;
            }
        }

        /// <summary>
        /// Recursively checks a data range against EL's DataRange grammar (§2.2.4: Datatype|DataIntersectionOf|DataOneOf),
        /// additionally checking every Datatype leaf against the §2.2.1 datatype allowlist.
        /// </summary>
        private static void CheckDataRange(OWLAxiom axiom, OWLDataRangeExpression dataRange, List<OWLProfileViolation> violations)
        {
            switch (dataRange)
            {
                //A named datatype is admitted by the DataRange grammar, but the specific datatype IRI must also
                //belong to EL's §2.2.1 allowlist (e.g. xsd:double or xsd:boolean are excluded even though
                //"Datatype" as a construct is fine).
                case OWLDatatype datatype:
                    if (!AllowedDatatypeIRIs.Contains(datatype.GetIRI().ToString()))
                        AddViolation(violations, DatatypeRule, axiom,
                            $"Datatype '{datatype.GetIRI()}' is not part of the EL profile's allowed datatype map (§2.2.1)",
                            "Use one of the datatypes admitted by the EL profile (e.g. xsd:string, xsd:integer, xsd:dateTime, owl:real...)");
                    break;

                //DataIntersectionOf is admitted, but every member must itself be an EL-compliant data range,
                //for the same "inductive grammar" reason as ObjectIntersectionOf above.
                case OWLDataIntersectionOf intersectionOf:
                    foreach (OWLDataRangeExpression member in intersectionOf.DataRangeExpressions)
                        CheckDataRange(axiom, member, violations);
                    break;

                //DataOneOf (an enumeration of literals) is admitted as-is by §2.2.4: unlike ObjectOneOf, EL does
                //not restrict it to a singleton. NOTE: the datatype allowlist is not currently re-checked against
                //each enumerated literal's own datatype here (a scope call, not verified against the spec text --
                //see owl2_profiles_w3c_spec memory note on double-checking fragile QL/EL edge cases before relying on them).
                case OWLDataOneOf _:
                    break;

                //Everything else (DataUnionOf, DataComplementOf, DatatypeRestriction with facets, and any future
                //data range type) falls outside EL's DataRange grammar.
                default:
                    AddViolation(violations, DataRangeRule, axiom,
                        $"Data range construct '{dataRange.GetType().Name}' is not admitted by the EL profile grammar (§2.2.4)",
                        "Remove or rewrite this construct using only Datatype, DataIntersectionOf or DataOneOf");
                    break;
            }
        }

        /// <summary>
        /// §2.2.5: EL admits SubClassOf, EquivalentClasses and DisjointClasses as class axiom types, but explicitly
        /// excludes DisjointUnion (it has no valid EL rendering regardless of how its members are shaped).
        /// </summary>
        private static void CheckClassAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLDisjointUnion disjointUnion in ontology.GetClassAxiomsOfType<OWLDisjointUnion>())
                AddViolation(violations, ClassAxiomTypeRule, disjointUnion,
                    "DisjointUnion is not an axiom type admitted by the EL profile (§2.2.5)",
                    "Replace DisjointUnion(C,(C1,C2,...)) with an equivalent combination of SubClassOf and DisjointClasses axioms");
        }

        /// <summary>
        /// §2.2.5: EL admits EquivalentObjectProperties, SubObjectPropertyOf, ObjectPropertyDomain, ObjectPropertyRange,
        /// ReflexiveObjectProperty and TransitiveObjectProperty. Everything else (DisjointObjectProperties,
        /// InverseObjectProperties, IrreflexiveObjectProperty, Functional/InverseFunctionalObjectProperty,
        /// Symmetric/AsymmetricObjectProperty) is excluded, because admitting them would let a reasoner derive
        /// SameIndividual/DifferentIndividuals facts, breaking EL's tractability guarantee.
        /// </summary>
        private static void CheckObjectPropertyAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLObjectPropertyAxiom axiom in ontology.ObjectPropertyAxioms ?? new List<OWLObjectPropertyAxiom>())
            {
                bool isAdmitted = axiom is OWLEquivalentObjectProperties
                                   || axiom is OWLSubObjectPropertyOf
                                   || axiom is OWLObjectPropertyDomain
                                   || axiom is OWLObjectPropertyRange
                                   || axiom is OWLReflexiveObjectProperty
                                   || axiom is OWLTransitiveObjectProperty;

                if (!isAdmitted)
                    AddViolation(violations, ObjectPropertyAxiomTypeRule, axiom,
                        $"Object property axiom type '{axiom.GetType().Name}' is not admitted by the EL profile (§2.2.5)",
                        "Remove this axiom, or restrict the ontology to EquivalentObjectProperties, SubObjectPropertyOf, ObjectPropertyDomain, ObjectPropertyRange, ReflexiveObjectProperty and TransitiveObjectProperty");
            }
        }

        /// <summary>
        /// §2.2.5: EL admits SubDataPropertyOf, EquivalentDataProperties, DataPropertyDomain, DataPropertyRange
        /// and FunctionalDataProperty. DisjointDataProperties is excluded for the same tractability reason as
        /// the excluded object property characteristics above.
        /// </summary>
        private static void CheckDataPropertyAxiomTypes(OWLOntology ontology, List<OWLProfileViolation> violations)
        {
            foreach (OWLDataPropertyAxiom axiom in ontology.DataPropertyAxioms ?? new List<OWLDataPropertyAxiom>())
            {
                bool isAdmitted = axiom is OWLSubDataPropertyOf
                                   || axiom is OWLEquivalentDataProperties
                                   || axiom is OWLDataPropertyDomain
                                   || axiom is OWLDataPropertyRange
                                   || axiom is OWLFunctionalDataProperty;

                if (!isAdmitted)
                    AddViolation(violations, DataPropertyAxiomTypeRule, axiom,
                        $"Data property axiom type '{axiom.GetType().Name}' is not admitted by the EL profile (§2.2.5)",
                        "Remove this axiom, or restrict the ontology to SubDataPropertyOf, EquivalentDataProperties, DataPropertyDomain, DataPropertyRange and FunctionalDataProperty");
            }
        }

        private static void AddViolation(List<OWLProfileViolation> violations, string ruleName, OWLAxiom offendingAxiom, string description, string suggestion)
            //The offending axiom's own OWL2/XML signature is folded into the description (same convention the
            //Validator's RuleSet classes use, e.g. OWLDisjointUnionAnalysis), so a violation is traceable back
            //to the exact axiom instance without needing a separate "Axiom" property on OWLProfileViolation.
            => violations.Add(new OWLProfileViolation(OWLEnums.OWLProfiles.EL, ruleName, $"{description} — offending axiom: '{offendingAxiom.GetXML()}'", suggestion));
    }
}