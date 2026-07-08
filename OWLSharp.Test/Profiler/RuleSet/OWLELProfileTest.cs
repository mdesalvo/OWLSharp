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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Profiler;
using RDFSharp.Model;

namespace OWLSharp.Test.Profiler.RuleSet;

[TestClass]
public class OWLELProfileTest
{
    #region Tests

    //EL's textbook realm is biomedical terminology (SNOMED/GO-like): large TBoxes dominated by existential
    //restrictions, which is exactly what EL is designed to classify in polynomial time. This small "infectious
    //disease" TBox is fully EL-compliant on purpose: it is the integration fixture exercising every construct
    //genuinely admitted by the grammar (ObjectSomeValuesFrom, TransitiveObjectProperty) together in one ontology,
    //as opposed to the unit tests below, which each isolate a single disallowed construct.
    private static OWLOntology BuildInfectiousDiseaseOntology()
    {
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLClass infectiousDisease = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass symptom = new OWLClass(new RDFResource("http://biomed.org/Symptom"));
        OWLClass fever = new OWLClass(new RDFResource("http://biomed.org/Fever"));
        OWLObjectProperty hasSymptom = new OWLObjectProperty(new RDFResource("http://biomed.org/hasSymptom"));

        return new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(disease), new OWLDeclaration(infectiousDisease),
                new OWLDeclaration(symptom), new OWLDeclaration(fever),
                new OWLDeclaration(hasSymptom)
            ],
            ClassAxioms = [
                new OWLSubClassOf(fever, symptom),
                new OWLSubClassOf(infectiousDisease, disease),
                //EL's flagship construct: existential restriction admitted in EL's unique ClassExpression grammar
                new OWLSubClassOf(infectiousDisease, new OWLObjectSomeValuesFrom(hasSymptom, fever))
            ],
            ObjectPropertyAxioms = [
                //TransitiveObjectProperty is explicitly admitted by EL's ObjectPropertyAxiom grammar (§2.2.5)
                new OWLTransitiveObjectProperty(hasSymptom)
            ]
        };
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnRealisticOntologyAsync()
    {
        OWLOntology ontology = BuildInfectiousDiseaseOntology();

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.IsNotNull(violations);
        //This is now a real compliance assertion (not a stub baseline): the fixture is genuinely EL-compliant,
        //so the full grammar/axiom-type/datatype checks implemented in OWLELProfile must all agree it is.
        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnEmptyOntologyAsync()
    {
        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(new OWLOntology());

        Assert.IsNotNull(violations);
        Assert.IsEmpty(violations);
    }

    //--- §2.2.3 ClassExpression grammar: disallowed constructs -------------------------------------------------

    //A realistic trap: an ontology engineer models "ChronicDisease" as the union of two known subtypes,
    //not realizing OWL2 EL forbids disjunction/union entirely (it would break polynomial-time classification).
    [TestMethod]
    public async Task ShouldFlagObjectUnionOfInClassExpressionAsync()
    {
        OWLClass chronicDisease = new OWLClass(new RDFResource("http://biomed.org/ChronicDisease"));
        OWLClass diabetes = new OWLClass(new RDFResource("http://biomed.org/Diabetes"));
        OWLClass arthritis = new OWLClass(new RDFResource("http://biomed.org/Arthritis"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectUnionOf([diabetes, arthritis]), chronicDisease)]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.AreEqual(OWLEnums.OWLProfiles.EL, violations[0].Profile);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ClassExpression"));
        Assert.Contains("ObjectUnionOf", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagObjectComplementOfInClassExpressionAsync()
    {
        OWLClass healthy = new OWLClass(new RDFResource("http://biomed.org/Healthy"));
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLOntology ontology = new OWLOntology
        {
            //A textbook DL move ("Healthy = not Disease") that is simply not expressible in EL: negation is excluded.
            ClassAxioms = [new OWLEquivalentClasses([healthy, new OWLObjectComplementOf(disease)])]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectComplementOf", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagObjectAllValuesFromInClassExpressionAsync()
    {
        OWLClass strictVegetarian = new OWLClass(new RDFResource("http://food.org/StrictVegetarian"));
        OWLClass plantDish = new OWLClass(new RDFResource("http://food.org/PlantBasedDish"));
        OWLObjectProperty eats = new OWLObjectProperty(new RDFResource("http://food.org/eats"));
        OWLOntology ontology = new OWLOntology
        {
            //"Only eats plant-based dishes" is universal quantification: EL only admits the existential form.
            ClassAxioms = [new OWLSubClassOf(strictVegetarian, new OWLObjectAllValuesFrom(eats, plantDish))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectAllValuesFrom", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagObjectMinCardinalityInClassExpressionAsync()
    {
        OWLClass parent = new OWLClass(new RDFResource("http://family.org/Parent"));
        OWLObjectProperty hasChild = new OWLObjectProperty(new RDFResource("http://family.org/hasChild"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(parent, new OWLObjectMinCardinality(hasChild, 1))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectMinCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagObjectMaxCardinalityInClassExpressionAsync()
    {
        OWLClass monogamousPerson = new OWLClass(new RDFResource("http://family.org/MonogamousPerson"));
        OWLObjectProperty hasSpouse = new OWLObjectProperty(new RDFResource("http://family.org/hasSpouse"));
        OWLOntology ontology = new OWLOntology
        {
            //Counting constructs are exactly what EL trades away for tractability: even a modest "at most 1" breaks it.
            ClassAxioms = [new OWLSubClassOf(monogamousPerson, new OWLObjectMaxCardinality(hasSpouse, 1))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectMaxCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagObjectExactCardinalityInClassExpressionAsync()
    {
        OWLClass twin = new OWLClass(new RDFResource("http://family.org/Twin"));
        OWLObjectProperty hasSibling = new OWLObjectProperty(new RDFResource("http://family.org/hasSibling"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(twin, new OWLObjectExactCardinality(hasSibling, 1))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectExactCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDataAllValuesFromInClassExpressionAsync()
    {
        OWLClass adult = new OWLClass(new RDFResource("http://people.org/Adult"));
        OWLDataProperty hasAge = new OWLDataProperty(new RDFResource("http://people.org/hasAge"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(adult, new OWLDataAllValuesFrom(hasAge, new OWLDatatype(RDFVocabulary.XSD.INTEGER)))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("DataAllValuesFrom", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDataMinCardinalityInClassExpressionAsync()
    {
        OWLClass wellDocumentedPatient = new OWLClass(new RDFResource("http://biomed.org/WellDocumentedPatient"));
        OWLDataProperty hasLabResult = new OWLDataProperty(new RDFResource("http://biomed.org/hasLabResult"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(wellDocumentedPatient, new OWLDataMinCardinality(hasLabResult, 3))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("DataMinCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDataMaxCardinalityInClassExpressionAsync()
    {
        OWLClass singleEmailPerson = new OWLClass(new RDFResource("http://people.org/SingleEmailPerson"));
        OWLDataProperty hasEmail = new OWLDataProperty(new RDFResource("http://people.org/hasEmail"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(singleEmailPerson, new OWLDataMaxCardinality(hasEmail, 1))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("DataMaxCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDataExactCardinalityInClassExpressionAsync()
    {
        OWLClass person = new OWLClass(new RDFResource("http://people.org/Person"));
        OWLDataProperty hasSsn = new OWLDataProperty(new RDFResource("http://people.org/hasSsn"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(person, new OWLDataExactCardinality(hasSsn, 1))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("DataExactCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowSingletonObjectOneOfAsync()
    {
        OWLClass daysOfWeekend = new OWLClass(new RDFResource("http://calendar.org/JustSunday"));
        OWLNamedIndividual sunday = new OWLNamedIndividual(new RDFResource("http://calendar.org/Sunday"));
        OWLOntology ontology = new OWLOntology
        {
            //{Sunday} is a singleton nominal: EL admits ObjectOneOf ONLY in this shape.
            ClassAxioms = [new OWLEquivalentClasses([daysOfWeekend, new OWLObjectOneOf([sunday])])]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldFlagMultiIndividualObjectOneOfAsync()
    {
        OWLClass weekend = new OWLClass(new RDFResource("http://calendar.org/Weekend"));
        OWLNamedIndividual saturday = new OWLNamedIndividual(new RDFResource("http://calendar.org/Saturday"));
        OWLNamedIndividual sunday = new OWLNamedIndividual(new RDFResource("http://calendar.org/Sunday"));
        OWLOntology ontology = new OWLOntology
        {
            //{Saturday, Sunday} is a 2-element nominal, semantically a disjunction: outside EL, unlike the singleton case above.
            ClassAxioms = [new OWLEquivalentClasses([weekend, new OWLObjectOneOf([saturday, sunday])])]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectOneOf", violations[0].Description);
        Assert.Contains("2 individuals", violations[0].Description);
    }

    //--- Recursion: a disallowed construct nested inside an otherwise-admitted one --------------------------------

    //A subtle, easy-to-miss corner case: ObjectSomeValuesFrom itself is EL's flagship construct and is perfectly
    //fine, but here its filler is an ObjectAllValuesFrom, which is not. A profile checker that only inspects the
    //outermost construct of each axiom (instead of recursing into fillers) would wrongly report this as compliant.
    [TestMethod]
    public async Task ShouldFlagNonELConstructNestedInsideObjectSomeValuesFromFillerAsync()
    {
        OWLClass infectiousDisease = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass fever = new OWLClass(new RDFResource("http://biomed.org/Fever"));
        OWLClass rash = new OWLClass(new RDFResource("http://biomed.org/Rash"));
        OWLObjectProperty hasSymptom = new OWLObjectProperty(new RDFResource("http://biomed.org/hasSymptom"));
        OWLObjectProperty causes = new OWLObjectProperty(new RDFResource("http://biomed.org/causes"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(infectiousDisease,
                    new OWLObjectSomeValuesFrom(hasSymptom, new OWLObjectAllValuesFrom(causes, rash)))
            ]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectAllValuesFrom", violations[0].Description);
    }

    //Symmetric corner case, the "happy path" version: a deep chain of admitted constructs (intersection wrapping
    //an existential wrapping an intersection) must recurse cleanly to zero violations, not just a single level deep.
    [TestMethod]
    public async Task ShouldAllowDeeplyNestedELCompliantClassExpressionAsync()
    {
        OWLClass severeInfectiousDisease = new OWLClass(new RDFResource("http://biomed.org/SevereInfectiousDisease"));
        OWLClass infectiousDisease = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass fever = new OWLClass(new RDFResource("http://biomed.org/Fever"));
        OWLClass highGrade = new OWLClass(new RDFResource("http://biomed.org/HighGrade"));
        OWLObjectProperty hasSymptom = new OWLObjectProperty(new RDFResource("http://biomed.org/hasSymptom"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(severeInfectiousDisease,
                    new OWLObjectIntersectionOf([
                        infectiousDisease,
                        new OWLObjectSomeValuesFrom(hasSymptom, new OWLObjectIntersectionOf([fever, highGrade]))
                    ]))
            ]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- §2.2.4 DataRange grammar --------------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagDataUnionOfInDataRangeAsync()
    {
        OWLClass person = new OWLClass(new RDFResource("http://people.org/Person"));
        OWLDataProperty hasIdCode = new OWLDataProperty(new RDFResource("http://people.org/hasIdCode"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(person, new OWLDataSomeValuesFrom(hasIdCode,
                    new OWLDataUnionOf([new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.INTEGER)])))
            ]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".DataRange"));
        Assert.Contains("DataUnionOf", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDataComplementOfInDataRangeAsync()
    {
        OWLClass minor = new OWLClass(new RDFResource("http://people.org/Minor"));
        OWLDataProperty hasAge = new OWLDataProperty(new RDFResource("http://people.org/hasAge"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(minor, new OWLDataSomeValuesFrom(hasAge,
                    new OWLDataComplementOf(new OWLDatatype(RDFVocabulary.XSD.NON_NEGATIVE_INTEGER))))
            ]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("DataComplementOf", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDatatypeRestrictionInDataRangeAsync()
    {
        OWLClass adult = new OWLClass(new RDFResource("http://people.org/Adult"));
        OWLDataProperty hasAge = new OWLDataProperty(new RDFResource("http://people.org/hasAge"));
        OWLOntology ontology = new OWLOntology
        {
            //A facet-based range ("integer >= 18") is exactly the kind of DL-Lite/DatatypeRestriction machinery
            //that none of the three OWL2 profiles admit, EL included.
            ClassAxioms = [
                new OWLSubClassOf(adult, new OWLDataSomeValuesFrom(hasAge,
                    new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.INTEGER), null)))
            ]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("DatatypeRestriction", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowDataIntersectionOfAndDataOneOfAsync()
    {
        OWLClass person = new OWLClass(new RDFResource("http://people.org/Person"));
        OWLDataProperty hasBloodType = new OWLDataProperty(new RDFResource("http://biomed.org/hasBloodType"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                //DataIntersectionOf and DataOneOf are both explicitly admitted by §2.2.4.
                new OWLSubClassOf(person, new OWLDataSomeValuesFrom(hasBloodType,
                    new OWLDataIntersectionOf([
                        new OWLDatatype(RDFVocabulary.XSD.STRING),
                        new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("A+")), new OWLLiteral(new RDFPlainLiteral("O-"))])
                    ])))
            ]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- §2.2.1 datatype allowlist --------------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagDisallowedDatatypeAsync()
    {
        OWLClass measurement = new OWLClass(new RDFResource("http://biomed.org/Measurement"));
        OWLDataProperty hasValue = new OWLDataProperty(new RDFResource("http://biomed.org/hasValue"));
        OWLOntology ontology = new OWLOntology
        {
            //xsd:double is a real-world trap: it is a perfectly ordinary datatype, just not one of the ones EL allows.
            ClassAxioms = [new OWLSubClassOf(measurement, new OWLDataSomeValuesFrom(hasValue, new OWLDatatype(RDFVocabulary.XSD.DOUBLE)))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".Datatype"));
        Assert.Contains(RDFVocabulary.XSD.DOUBLE.ToString(), violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowAdmittedDatatypeAsync()
    {
        OWLClass measurement = new OWLClass(new RDFResource("http://biomed.org/Measurement"));
        OWLDataProperty hasValue = new OWLDataProperty(new RDFResource("http://biomed.org/hasValue"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(measurement, new OWLDataSomeValuesFrom(hasValue, new OWLDatatype(RDFVocabulary.XSD.INTEGER)))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- §2.2.5 allowed axiom types --------------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagDisjointUnionClassAxiomAsync()
    {
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLClass infectious = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass hereditary = new OWLClass(new RDFResource("http://biomed.org/HereditaryDisease"));
        OWLOntology ontology = new OWLOntology
        {
            //DisjointUnion has no valid EL rendering at all: it is excluded as an axiom TYPE, independently of
            //what its member class expressions look like (both members here are plain atomic classes).
            ClassAxioms = [new OWLDisjointUnion(disease, [infectious, hereditary])]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ClassAxiomType"));
        Assert.Contains("DisjointUnion", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagFunctionalObjectPropertyAxiomAsync()
    {
        OWLObjectProperty hasBiologicalMother = new OWLObjectProperty(new RDFResource("http://family.org/hasBiologicalMother"));
        OWLOntology ontology = new OWLOntology
        {
            //A textbook-correct FunctionalObjectProperty ("everyone has exactly one biological mother") is still
            //excluded from EL: admitting property functionality would let a reasoner derive SameIndividual facts,
            //breaking tractability. Contrast with FunctionalDataProperty, which EL DOES admit (see below).
            ObjectPropertyAxioms = [new OWLFunctionalObjectProperty(hasBiologicalMother)]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ObjectPropertyAxiomType"));
        Assert.Contains("OWLFunctionalObjectProperty", violations[0].Description);
    }

    //One test enumerating every remaining excluded ObjectPropertyAxiom subtype in a single pass, rather than
    //a near-duplicate [TestMethod] per type: still asserts each individually (via the loop body), so a
    //regression in any one of them still fails with a precise message pointing at the offending type.
    [TestMethod]
    public async Task ShouldFlagRemainingExcludedObjectPropertyAxiomTypesAsync()
    {
        OWLObjectProperty knows = new OWLObjectProperty(new RDFResource("http://social.org/knows"));
        OWLObjectProperty follows = new OWLObjectProperty(new RDFResource("http://social.org/follows"));
        (string Label, OWLObjectPropertyAxiom Axiom)[] excludedAxioms =
        [
            ("DisjointObjectProperties", new OWLDisjointObjectProperties([knows, follows])),
            ("InverseObjectProperties", new OWLInverseObjectProperties(knows, follows)),
            ("InverseFunctionalObjectProperty", new OWLInverseFunctionalObjectProperty(knows)),
            ("IrreflexiveObjectProperty", new OWLIrreflexiveObjectProperty(knows)),
            ("SymmetricObjectProperty", new OWLSymmetricObjectProperty(knows)),
            ("AsymmetricObjectProperty", new OWLAsymmetricObjectProperty(follows))
        ];

        foreach ((string label, OWLObjectPropertyAxiom axiom) in excludedAxioms)
        {
            OWLOntology ontology = new OWLOntology { ObjectPropertyAxioms = [axiom] };

            List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

            Assert.HasCount(1, violations, $"expected exactly one violation for {label}");
            Assert.IsTrue(violations[0].RuleName.EndsWith(".ObjectPropertyAxiomType"), $"unexpected rule name for {label}");
        }
    }

    [TestMethod]
    public async Task ShouldFlagDisjointDataPropertiesAxiomAsync()
    {
        OWLDataProperty hasFirstName = new OWLDataProperty(new RDFResource("http://people.org/hasFirstName"));
        OWLDataProperty hasLastName = new OWLDataProperty(new RDFResource("http://people.org/hasLastName"));
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [new OWLDisjointDataProperties([hasFirstName, hasLastName])]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".DataPropertyAxiomType"));
        Assert.Contains("OWLDisjointDataProperties", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowFunctionalDataPropertyAxiomAsync()
    {
        OWLDataProperty hasSsn = new OWLDataProperty(new RDFResource("http://people.org/hasSsn"));
        OWLOntology ontology = new OWLOntology
        {
            //Contrast with ShouldFlagFunctionalObjectPropertyAxiomAsync above: EL admits functionality on DATA
            //properties (§2.2.5 explicitly lists FunctionalDataProperty), just not on OBJECT properties.
            DataPropertyAxioms = [new OWLFunctionalDataProperty(hasSsn)]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- Class/data expressions embedded in property domain/range axioms (not reached by OWLClassAxiomWalker) ----

    [TestMethod]
    public async Task ShouldFlagDisallowedClassExpressionInObjectPropertyDomainAsync()
    {
        OWLObjectProperty treats = new OWLObjectProperty(new RDFResource("http://biomed.org/treats"));
        OWLClass doctor = new OWLClass(new RDFResource("http://biomed.org/Doctor"));
        OWLClass patient = new OWLClass(new RDFResource("http://biomed.org/Patient"));
        OWLOntology ontology = new OWLOntology
        {
            //The domain of "treats" is asserted as "anyone who ONLY treats patients": a universal restriction,
            //which is just as much outside EL here as it would be on the LHS/RHS of a SubClassOf.
            ObjectPropertyAxioms = [new OWLObjectPropertyDomain(treats, new OWLObjectAllValuesFrom(treats, patient))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ClassExpression"));
    }

    [TestMethod]
    public async Task ShouldFlagDisallowedClassExpressionInObjectPropertyRangeAsync()
    {
        OWLObjectProperty treats = new OWLObjectProperty(new RDFResource("http://biomed.org/treats"));
        OWLObjectProperty hasCondition = new OWLObjectProperty(new RDFResource("http://biomed.org/hasCondition"));
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [new OWLObjectPropertyRange(treats, new OWLObjectMinCardinality(hasCondition, 1))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectMinCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDisallowedClassExpressionInDataPropertyDomainAsync()
    {
        OWLDataProperty hasDosage = new OWLDataProperty(new RDFResource("http://biomed.org/hasDosage"));
        OWLObjectProperty prescribes = new OWLObjectProperty(new RDFResource("http://biomed.org/prescribes"));
        OWLClass drug = new OWLClass(new RDFResource("http://biomed.org/Drug"));
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [new OWLDataPropertyDomain(hasDosage, new OWLObjectExactCardinality(prescribes, 1))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectExactCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDisallowedDataRangeInDataPropertyRangeAsync()
    {
        OWLDataProperty hasDosage = new OWLDataProperty(new RDFResource("http://biomed.org/hasDosage"));
        OWLOntology ontology = new OWLOntology
        {
            //DataPropertyRange states the range directly as a data range (not via a class expression), so this
            //exercises the second, independent DataRange entry point in OWLELProfile.ExecuteRuleAsync.
            DataPropertyAxioms = [new OWLDataPropertyRange(hasDosage, new OWLDatatype(RDFVocabulary.XSD.DOUBLE))]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".Datatype"));
    }

    //--- Cross-cutting corner cases ---------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldIncludeOffendingAxiomXmlInViolationDescriptionAsync()
    {
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLClass infectious = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass hereditary = new OWLClass(new RDFResource("http://biomed.org/HereditaryDisease"));
        OWLDisjointUnion disjointUnion = new OWLDisjointUnion(disease, [infectious, hereditary]);
        OWLOntology ontology = new OWLOntology { ClassAxioms = [disjointUnion] };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        //Same traceability convention used by the Validator's RuleSet classes (e.g. OWLDisjointUnionAnalysis):
        //the violation must be traceable back to the exact offending axiom via its own OWL2/XML signature.
        Assert.Contains(disjointUnion.GetXML(), violations[0].Description);
    }

    //A single ontology mixing several unrelated violations must report ALL of them, not just the first one hit:
    //guards against an accidental early-return/short-circuit in ExecuteRuleAsync's sequence of checks.
    [TestMethod]
    public async Task ShouldReportMultipleIndependentViolationsInSameOntologyAsync()
    {
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLClass infectious = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass hereditary = new OWLClass(new RDFResource("http://biomed.org/HereditaryDisease"));
        OWLDataProperty hasFirstName = new OWLDataProperty(new RDFResource("http://people.org/hasFirstName"));
        OWLDataProperty hasLastName = new OWLDataProperty(new RDFResource("http://people.org/hasLastName"));
        OWLOntology ontology = new OWLOntology
        {
            //A class-expression violation, a class-axiom-type violation and a data-property-axiom-type violation,
            //all unrelated to each other, thrown into the same ontology.
            ClassAxioms = [
                new OWLSubClassOf(infectious, new OWLObjectComplementOf(disease)),
                new OWLDisjointUnion(disease, [infectious, hereditary])
            ],
            DataPropertyAxioms = [new OWLDisjointDataProperties([hasFirstName, hasLastName])]
        };

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(3, violations);
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ClassExpression")));
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ClassAxiomType")));
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".DataPropertyAxiomType")));
    }
    #endregion
}
