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
public class OWLRLProfileTest
{
    //A test-only stand-in for "some future OWLDataPropertyAxiom subtype not yet wired into RL's grammar".
    //Reachable here only because OWLDataPropertyAxiom's constructor is internal and InternalsVisibleTo grants
    //OWLSharp.Test access to OWLSharp's internals — the same mechanism this whole test project relies on to
    //reach the RuleSet classes. Used ONLY against OWLRLProfile.IsAdmittedDataPropertyAxiomType directly (never
    //through ExecuteRuleAsync's full pipeline): going through AddViolation would call GetXML(), whose
    //XmlSerializer requires every polymorphically-serialized type to be registered via [XmlInclude] on
    //OWLAxiom — a registration this fictitious type deliberately has none of, and rightly so (adding a
    //test-only type to OWLAxiom's real XmlInclude list just to satisfy this one test would pollute production
    //serialization for no benefit). Testing the extracted predicate directly sidesteps that entirely.
    private sealed class FutureDataPropertyAxiom : OWLDataPropertyAxiom;

    #region Tests

    //RL's textbook realm is rule-engine-friendly business data (order fulfillment, e-commerce): scalable
    //reasoning over ABox-heavy data. Fully RL-compliant on purpose.
    //NOTE: the original scaffolding-phase fixture put ObjectSomeValuesFrom on the SUPERclass side of a SubClassOf
    //("PriorityOrder subClassOf ∃placedBy.Customer") — which looked plausible but is NOT RL-compliant, since
    //RL's superClassExpression grammar does not admit ObjectSomeValuesFrom at all (only subClassExpression does).
    //It is fixed here to put the existential on the SUBclass side instead ("∃placedBy.Customer subClassOf Order"),
    //which is both genuinely compliant and a realistic way to phrase the same domain fact. See
    //ShouldFlagObjectSomeValuesFromInSuperClassPositionAsync below for a test that pins down the original mistake.
    private static OWLOntology BuildOrderFulfillmentOntology()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLClass priorityOrder = new OWLClass(new RDFResource("http://shop.org/PriorityOrder"));
        OWLClass customer = new OWLClass(new RDFResource("http://shop.org/Customer"));
        OWLClass product = new OWLClass(new RDFResource("http://shop.org/Product"));
        OWLObjectProperty placedBy = new OWLObjectProperty(new RDFResource("http://shop.org/placedBy"));

        return new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(order), new OWLDeclaration(priorityOrder),
                new OWLDeclaration(customer), new OWLDeclaration(product),
                new OWLDeclaration(placedBy)
            ],
            ClassAxioms = [
                new OWLSubClassOf(priorityOrder, order),
                //RL's subClassExpression grammar admits ObjectSomeValuesFrom with a subCE filler
                new OWLSubClassOf(new OWLObjectSomeValuesFrom(placedBy, customer), order),
                new OWLDisjointClasses([order, customer, product])
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyDomain(placedBy, order),
                new OWLObjectPropertyRange(placedBy, customer),
                //FunctionalObjectProperty is explicitly admitted by RL's ObjectPropertyAxiom grammar (§4.2.5),
                //unlike EL and QL which both disallow it
                new OWLFunctionalObjectProperty(placedBy)
            ]
        };
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnRealisticOntologyAsync()
    {
        OWLOntology ontology = BuildOrderFulfillmentOntology();

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsNotNull(violations);
        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnEmptyOntologyAsync()
    {
        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(new OWLOntology());

        Assert.IsNotNull(violations);
        Assert.IsEmpty(violations);
    }

    //--- Position asymmetry: the heart of RL's grammar ------------------------------------------------------------

    //This is exactly the mistake the original scaffolding-phase fixture made (see BuildOrderFulfillmentOntology's
    //note above): ObjectSomeValuesFrom belongs to subClassExpression only, never to superClassExpression.
    [TestMethod]
    public async Task ShouldFlagObjectSomeValuesFromInSuperClassPositionAsync()
    {
        OWLClass priorityOrder = new OWLClass(new RDFResource("http://shop.org/PriorityOrder"));
        OWLClass customer = new OWLClass(new RDFResource("http://shop.org/Customer"));
        OWLObjectProperty placedBy = new OWLObjectProperty(new RDFResource("http://shop.org/placedBy"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(priorityOrder, new OWLObjectSomeValuesFrom(placedBy, customer))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ClassExpression"));
        Assert.Contains("superclass position", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowObjectUnionOfInSubClassPositionAsync()
    {
        OWLClass vipCustomer = new OWLClass(new RDFResource("http://shop.org/VipCustomer"));
        OWLClass goldCustomer = new OWLClass(new RDFResource("http://shop.org/GoldCustomer"));
        OWLClass platinumCustomer = new OWLClass(new RDFResource("http://shop.org/PlatinumCustomer"));
        OWLOntology ontology = new OWLOntology
        {
            //"A Gold or Platinum customer is a VIP customer": union on the antecedent (sub) side is the
            //distinctive RL asymmetry — it maps onto two separate datalog rules, one per disjunct.
            ClassAxioms = [new OWLSubClassOf(new OWLObjectUnionOf([goldCustomer, platinumCustomer]), vipCustomer)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldFlagObjectUnionOfInSuperClassPositionAsync()
    {
        OWLClass customer = new OWLClass(new RDFResource("http://shop.org/Customer"));
        OWLClass goldCustomer = new OWLClass(new RDFResource("http://shop.org/GoldCustomer"));
        OWLClass platinumCustomer = new OWLClass(new RDFResource("http://shop.org/PlatinumCustomer"));
        OWLOntology ontology = new OWLOntology
        {
            //The mirror image of the compliant case above: union on the CONSEQUENT side is not rule-safe
            //(it would require the rule engine to "guess" which disjunct holds), so it is excluded here.
            ClassAxioms = [new OWLSubClassOf(customer, new OWLObjectUnionOf([goldCustomer, platinumCustomer]))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectUnionOf", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowObjectComplementOfWrappingCompliantSubClassExpressionAsync()
    {
        OWLClass activeOrder = new OWLClass(new RDFResource("http://shop.org/ActiveOrder"));
        OWLClass cancelledOrder = new OWLClass(new RDFResource("http://shop.org/CancelledOrder"));
        OWLOntology ontology = new OWLOntology
        {
            //"ActiveOrder subClassOf not(CancelledOrder)": ObjectComplementOf is admitted ONLY in superclass
            //position, and its operand (CancelledOrder, a plain Class) is a valid subClassExpression.
            ClassAxioms = [new OWLSubClassOf(activeOrder, new OWLObjectComplementOf(cancelledOrder))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldFlagObjectComplementOfInSubClassPositionAsync()
    {
        OWLClass cancelledOrder = new OWLClass(new RDFResource("http://shop.org/CancelledOrder"));
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLOntology ontology = new OWLOntology
        {
            //ObjectComplementOf on the ANTECEDENT side has no rule-safe reading at all: negation-as-failure is
            //not what RL's monotone rule semantics support, on either side, but the grammar only spells out the
            //superclass-side exception — subclass position admits no complement whatsoever.
            ClassAxioms = [new OWLSubClassOf(new OWLObjectComplementOf(cancelledOrder), order)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectComplementOf", violations[0].Description);
        Assert.Contains("subclass position", violations[0].Description);
    }

    //The polarity-inversion corner case: ObjectComplementOf's operand must satisfy subClassExpression, so an
    //operand that is only valid in SUPERclass position (like ObjectAllValuesFrom) must still be flagged, even
    //though it sits underneath a construct that is itself in superclass position.
    [TestMethod]
    public async Task ShouldFlagSuperClassOnlyConstructNestedInsideObjectComplementOfAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLClass exclusivelyDigitalOrder = new OWLClass(new RDFResource("http://shop.org/ExclusivelyDigitalOrder"));
        OWLClass digitalProduct = new OWLClass(new RDFResource("http://shop.org/DigitalProduct"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(order, new OWLObjectComplementOf(new OWLObjectAllValuesFrom(contains, digitalProduct)))
            ]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectAllValuesFrom", violations[0].Description);
        Assert.Contains("subclass position", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowObjectMaxCardinalityZeroOrOneAsync()
    {
        OWLClass singleItemOrder = new OWLClass(new RDFResource("http://shop.org/SingleItemOrder"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(singleItemOrder, new OWLObjectMaxCardinality(contains, 1))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldFlagObjectMaxCardinalityGreaterThanOneAsync()
    {
        OWLClass bulkOrder = new OWLClass(new RDFResource("http://shop.org/BulkOrder"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            //"At most 5 items" is an entirely ordinary OWL2 restriction, just outside RL's "zeroOrOne" production.
            ClassAxioms = [new OWLSubClassOf(bulkOrder, new OWLObjectMaxCardinality(contains, 5))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectMaxCardinality(5)", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagObjectMinCardinalityAsync()
    {
        OWLClass bulkOrder = new OWLClass(new RDFResource("http://shop.org/BulkOrder"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            //ObjectMinCardinality has no RL rendering at all, regardless of the bound value (unlike Max, which
            //is admitted specifically at 0/1): RL's grammar simply never lists it.
            ClassAxioms = [new OWLSubClassOf(bulkOrder, new OWLObjectMinCardinality(contains, 1))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectMinCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagObjectExactCardinalityAsync()
    {
        OWLClass twinPack = new OWLClass(new RDFResource("http://shop.org/TwinPack"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(twinPack, new OWLObjectExactCardinality(contains, 2))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectExactCardinality", violations[0].Description);
    }

    //Contrast with EL, whose ObjectOneOf is restricted to a singleton: RL admits any-size enumerations in
    //subclass position (it has no such restriction in its grammar).
    [TestMethod]
    public async Task ShouldAllowMultiIndividualObjectOneOfInSubClassPositionAsync()
    {
        OWLClass weekendOrder = new OWLClass(new RDFResource("http://shop.org/WeekendPromoOrder"));
        OWLNamedIndividual saturdayPromo = new OWLNamedIndividual(new RDFResource("http://shop.org/SaturdayPromo"));
        OWLNamedIndividual sundayPromo = new OWLNamedIndividual(new RDFResource("http://shop.org/SundayPromo"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectOneOf([saturdayPromo, sundayPromo]), weekendOrder)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- owl:Thing exclusion and its explicit filler exceptions ---------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagOwlThingAsPlainSuperClassExpressionAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(order, new OWLClass(RDFVocabulary.OWL.THING))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("owl:Thing", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowOwlThingAsObjectSomeValuesFromFillerAsync()
    {
        OWLClass orderedProduct = new OWLClass(new RDFResource("http://shop.org/OrderedProduct"));
        OWLObjectProperty orderedBy = new OWLObjectProperty(new RDFResource("http://shop.org/orderedBy"));
        OWLOntology ontology = new OWLOntology
        {
            //"∃orderedBy.owl:Thing" ("has been ordered by someone") is the grammar's explicit exception: owl:Thing
            //is admitted here even though it is excluded as a plain class expression elsewhere.
            ClassAxioms = [new OWLSubClassOf(new OWLObjectSomeValuesFrom(orderedBy, new OWLClass(RDFVocabulary.OWL.THING)), orderedProduct)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowOwlThingAsObjectMaxCardinalityFillerAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(order, new OWLObjectMaxCardinality(contains, 1, new OWLClass(RDFVocabulary.OWL.THING)))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- equivClassExpression: the strictest of the three productions ---------------------------------------------

    [TestMethod]
    public async Task ShouldAllowObjectIntersectionOfInEquivalentClassesAsync()
    {
        OWLClass vipCustomer = new OWLClass(new RDFResource("http://shop.org/VipCustomer"));
        OWLClass customer = new OWLClass(new RDFResource("http://shop.org/Customer"));
        OWLClass goldTier = new OWLClass(new RDFResource("http://shop.org/GoldTier"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLEquivalentClasses([vipCustomer, new OWLObjectIntersectionOf([customer, goldTier])])]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //Even though ObjectSomeValuesFrom is perfectly fine in subclass position, it is NOT admitted as an
    //EquivalentClasses member: equivClassExpression is strictly narrower than subClassExpression.
    [TestMethod]
    public async Task ShouldFlagObjectSomeValuesFromInEquivalentClassesAsync()
    {
        OWLClass loyalCustomer = new OWLClass(new RDFResource("http://shop.org/LoyalCustomer"));
        OWLClass customer = new OWLClass(new RDFResource("http://shop.org/Customer"));
        OWLObjectProperty placedBy = new OWLObjectProperty(new RDFResource("http://shop.org/placedBy"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLEquivalentClasses([loyalCustomer, new OWLObjectSomeValuesFrom(placedBy, customer)])]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("EquivalentClasses member", violations[0].Description);
    }

    //--- §4.2.4 DataRange grammar: narrower than EL's (no DataOneOf) ----------------------------------------------

    [TestMethod]
    public async Task ShouldFlagDataOneOfInDataRangeAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLDataProperty hasStatus = new OWLDataProperty(new RDFResource("http://shop.org/hasStatus"));
        OWLOntology ontology = new OWLOntology
        {
            //DataOneOf is admitted by EL (§2.2.4) but NOT by RL (§4.2.4): a genuine cross-profile difference.
            ClassAxioms = [
                new OWLSubClassOf(new OWLDataSomeValuesFrom(hasStatus,
                    new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("shipped")), new OWLLiteral(new RDFPlainLiteral("delivered"))])), order)
            ]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".DataRange"));
        Assert.Contains("DataOneOf", violations[0].Description);
    }

    //--- §4.2.1 datatype allowlist: a different set from EL's -----------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagOwlRealDatatypeAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLDataProperty hasTotal = new OWLDataProperty(new RDFResource("http://shop.org/hasTotal"));
        OWLOntology ontology = new OWLOntology
        {
            //owl:real is admitted by EL (§2.2.1) but explicitly excluded from RL's datatype map (§4.2.1) —
            //the mirror image of the xsd:double case below.
            ClassAxioms = [new OWLSubClassOf(new OWLDataSomeValuesFrom(hasTotal, new OWLDatatype(new RDFResource($"{RDFVocabulary.OWL.BASE_URI}real"))), order)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".Datatype"));
    }

    [TestMethod]
    public async Task ShouldAllowXsdDoubleDatatypeAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLDataProperty hasTotal = new OWLDataProperty(new RDFResource("http://shop.org/hasTotal"));
        OWLOntology ontology = new OWLOntology
        {
            //Contrast with OWLELProfileTest.ShouldFlagDisallowedDatatypeAsync: xsd:double is excluded from EL
            //but explicitly admitted by RL's datatype map.
            ClassAxioms = [new OWLSubClassOf(new OWLDataSomeValuesFrom(hasTotal, new OWLDatatype(RDFVocabulary.XSD.DOUBLE)), order)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //--- §4.2.5 allowed axiom types -----------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldFlagReflexiveObjectPropertyAxiomAsync()
    {
        OWLObjectProperty relatedTo = new OWLObjectProperty(new RDFResource("http://shop.org/relatedTo"));
        OWLOntology ontology = new OWLOntology
        {
            //The ONLY object property axiom type RL excludes (§4.2.5): reflexivity is not expressible as a
            //rule-safe datalog consequent.
            ObjectPropertyAxioms = [new OWLReflexiveObjectProperty(relatedTo)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ObjectPropertyAxiomType"));
    }

    //Contrast with EL, which excludes almost every one of these: RL admits all of them. One consolidated test
    //(rather than a near-duplicate [TestMethod] per type) exercising every object property characteristic EL
    //forbids but RL allows, confirming RL and EL are near-complementary on this axis.
    [TestMethod]
    public async Task ShouldAllowObjectPropertyCharacteristicsExcludedByELAsync()
    {
        OWLObjectProperty knows = new OWLObjectProperty(new RDFResource("http://social.org/knows"));
        OWLObjectProperty follows = new OWLObjectProperty(new RDFResource("http://social.org/follows"));
        OWLObjectPropertyAxiom[] admittedAxioms =
        [
            new OWLDisjointObjectProperties([knows, follows]),
            new OWLInverseObjectProperties(knows, follows),
            new OWLFunctionalObjectProperty(knows),
            new OWLInverseFunctionalObjectProperty(knows),
            new OWLIrreflexiveObjectProperty(follows),
            new OWLSymmetricObjectProperty(knows),
            new OWLAsymmetricObjectProperty(follows)
        ];

        foreach (OWLObjectPropertyAxiom axiom in admittedAxioms)
        {
            OWLOntology ontology = new OWLOntology { ObjectPropertyAxioms = [axiom] };

            List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

            Assert.IsEmpty(violations, $"expected no violation for {axiom.GetType().Name}");
        }
    }

    [TestMethod]
    public async Task ShouldFlagDisjointUnionClassAxiomAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLClass onlineOrder = new OWLClass(new RDFResource("http://shop.org/OnlineOrder"));
        OWLClass inStoreOrder = new OWLClass(new RDFResource("http://shop.org/InStoreOrder"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLDisjointUnion(order, [onlineOrder, inStoreOrder])]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".ClassAxiomType"));
    }

    //--- Class/data expressions embedded in property domain/range axioms -------------------------------------------

    [TestMethod]
    public async Task ShouldFlagDisallowedClassExpressionInObjectPropertyDomainAsync()
    {
        OWLObjectProperty ships = new OWLObjectProperty(new RDFResource("http://shop.org/ships"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            //ObjectPropertyDomain's class expression plays a SUPERclass role: ObjectSomeValuesFrom there is
            //just as much a violation as it would be on the RHS of a SubClassOf.
            ObjectPropertyAxioms = [new OWLObjectPropertyDomain(ships, new OWLObjectMinCardinality(contains, 1))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectMinCardinality", violations[0].Description);
    }

    //ObjectPropertyDomain/Range are reached via OWLPropertyAxiomWalker.WalkPropertyDomainRangeClassExpressions,
    //DataPropertyDomain via the same walker method, and DataPropertyRange via WalkPropertyRangeDataRanges — the
    //ObjectPropertyDomain case above already covers that walker's first branch; these two cover the remaining ones.
    [TestMethod]
    public async Task ShouldFlagDisallowedClassExpressionInDataPropertyDomainAsync()
    {
        OWLDataProperty hasDiscountCode = new OWLDataProperty(new RDFResource("http://shop.org/hasDiscountCode"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            ObjectPropertyAxioms = [],
            DataPropertyAxioms = [new OWLDataPropertyDomain(hasDiscountCode, new OWLObjectExactCardinality(contains, 2))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("ObjectExactCardinality", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldFlagDisallowedDataRangeInDataPropertyRangeAsync()
    {
        OWLDataProperty hasDiscountCode = new OWLDataProperty(new RDFResource("http://shop.org/hasDiscountCode"));
        OWLOntology ontology = new OWLOntology
        {
            //owl:real is excluded from RL's datatype map (§4.2.1) even though the "Datatype" construct itself is fine.
            DataPropertyAxioms = [new OWLDataPropertyRange(hasDiscountCode, new OWLDatatype(new RDFResource($"{RDFVocabulary.OWL.BASE_URI}real")))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.IsTrue(violations[0].RuleName.EndsWith(".Datatype"));
    }

    //--- Standalone coverage for constructs so far only ever exercised as a nested filler/member --------------------
    //--- (recursion tests confirm a construct is checked correctly WHEN nested, but never exercised it as the -------
    //--- direct, top-level construct of an axiom on its own — closing that gap here). -----------------------------

    [TestMethod]
    public async Task ShouldAllowStandaloneObjectIntersectionOfInSubClassPositionAsync()
    {
        OWLClass giftOrder = new OWLClass(new RDFResource("http://shop.org/GiftOrder"));
        OWLClass onlineOrder = new OWLClass(new RDFResource("http://shop.org/OnlineOrder"));
        OWLClass wrapped = new OWLClass(new RDFResource("http://shop.org/Wrapped"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectIntersectionOf([onlineOrder, wrapped]), giftOrder)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowStandaloneObjectIntersectionOfInSuperClassPositionAsync()
    {
        OWLClass priorityOrder = new OWLClass(new RDFResource("http://shop.org/PriorityOrder"));
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLClass expedited = new OWLClass(new RDFResource("http://shop.org/Expedited"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(priorityOrder, new OWLObjectIntersectionOf([order, expedited]))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowStandaloneObjectHasValueInSubClassPositionAsync()
    {
        OWLClass domesticOrder = new OWLClass(new RDFResource("http://shop.org/DomesticOrder"));
        OWLObjectProperty shipsTo = new OWLObjectProperty(new RDFResource("http://shop.org/shipsTo"));
        OWLNamedIndividual homeCountry = new OWLNamedIndividual(new RDFResource("http://shop.org/HomeCountry"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(new OWLObjectHasValue(shipsTo, homeCountry), domesticOrder)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowStandaloneObjectHasValueInSuperClassPositionAsync()
    {
        OWLClass domesticOrder = new OWLClass(new RDFResource("http://shop.org/DomesticOrder"));
        OWLObjectProperty shipsTo = new OWLObjectProperty(new RDFResource("http://shop.org/shipsTo"));
        OWLNamedIndividual homeCountry = new OWLNamedIndividual(new RDFResource("http://shop.org/HomeCountry"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(domesticOrder, new OWLObjectHasValue(shipsTo, homeCountry))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowStandaloneObjectAllValuesFromInSuperClassPositionAsync()
    {
        OWLClass exclusivelyDigitalOrder = new OWLClass(new RDFResource("http://shop.org/ExclusivelyDigitalOrder"));
        OWLClass digitalProduct = new OWLClass(new RDFResource("http://shop.org/DigitalProduct"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(exclusivelyDigitalOrder, new OWLObjectAllValuesFrom(contains, digitalProduct))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //Exercises the recursion branch where ObjectMaxCardinality carries a qualifying class expression that is
    //NEITHER absent NOR owl:Thing: a genuine atomic subCE filler, which must itself pass CheckSubClassExpression.
    [TestMethod]
    public async Task ShouldAllowObjectMaxCardinalityWithAtomicSubClassFillerAsync()
    {
        OWLClass singleGiftOrder = new OWLClass(new RDFResource("http://shop.org/SingleGiftOrder"));
        OWLClass giftItem = new OWLClass(new RDFResource("http://shop.org/GiftItem"));
        OWLObjectProperty contains = new OWLObjectProperty(new RDFResource("http://shop.org/contains"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(singleGiftOrder, new OWLObjectMaxCardinality(contains, 1, giftItem))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowStandaloneDataAllValuesFromAsync()
    {
        OWLClass domesticOrder = new OWLClass(new RDFResource("http://shop.org/DomesticOrder"));
        OWLDataProperty hasCurrency = new OWLDataProperty(new RDFResource("http://shop.org/hasCurrency"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(domesticOrder, new OWLDataAllValuesFrom(hasCurrency, new OWLDatatype(RDFVocabulary.XSD.STRING)))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldFlagDataMaxCardinalityGreaterThanOneAsync()
    {
        OWLClass bulkOrder = new OWLClass(new RDFResource("http://shop.org/BulkOrder"));
        OWLDataProperty hasNote = new OWLDataProperty(new RDFResource("http://shop.org/hasNote"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(bulkOrder, new OWLDataMaxCardinality(hasNote, 3))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(1, violations);
        Assert.Contains("DataMaxCardinality(3)", violations[0].Description);
    }

    [TestMethod]
    public async Task ShouldAllowDataMaxCardinalityZeroOrOneWithQualifyingRangeAsync()
    {
        OWLClass singleNoteOrder = new OWLClass(new RDFResource("http://shop.org/SingleNoteOrder"));
        OWLDataProperty hasNote = new OWLDataProperty(new RDFResource("http://shop.org/hasNote"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLSubClassOf(singleNoteOrder, new OWLDataMaxCardinality(hasNote, 1, new OWLDatatype(RDFVocabulary.XSD.STRING)))]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowStandaloneObjectHasValueAndDataHasValueInEquivalentClassesAsync()
    {
        OWLClass giftForMom = new OWLClass(new RDFResource("http://shop.org/GiftForMom"));
        OWLObjectProperty recipient = new OWLObjectProperty(new RDFResource("http://shop.org/recipient"));
        OWLNamedIndividual mom = new OWLNamedIndividual(new RDFResource("http://shop.org/Mom"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [new OWLEquivalentClasses([giftForMom, new OWLObjectHasValue(recipient, mom)])]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldAllowStandaloneDataIntersectionOfInDataRangeAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLDataProperty hasTrackingCode = new OWLDataProperty(new RDFResource("http://shop.org/hasTrackingCode"));
        OWLOntology ontology = new OWLOntology
        {
            ClassAxioms = [
                new OWLSubClassOf(new OWLDataSomeValuesFrom(hasTrackingCode,
                    new OWLDataIntersectionOf([new OWLDatatype(RDFVocabulary.XSD.STRING), new OWLDatatype(RDFVocabulary.XSD.TOKEN)])), order)
            ]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //CheckDataPropertyAxiomTypes' loop body (the "isAdmitted" check) never runs unless the ontology actually has
    //at least one DataPropertyAxiom: every other RL test either omits DataPropertyAxioms entirely or (rarely)
    //adds one that violates a DIFFERENT check first. This exercises the loop with axioms that are, today,
    //ALL admitted (see the method's XML-doc: RL's DataPropertyAxiom allowlist currently covers every subtype
    //that exists), so the expected outcome is zero violations — a compliance confirmation of a check that can
    //never currently fire, not a no-op test.
    [TestMethod]
    public async Task ShouldAllowEveryCurrentDataPropertyAxiomTypeAsync()
    {
        OWLDataProperty hasTrackingCode = new OWLDataProperty(new RDFResource("http://shop.org/hasTrackingCode"));
        OWLDataProperty hasCarrierCode = new OWLDataProperty(new RDFResource("http://shop.org/hasCarrierCode"));
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLOntology ontology = new OWLOntology
        {
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(hasTrackingCode, hasCarrierCode),
                new OWLEquivalentDataProperties([hasTrackingCode, hasCarrierCode]),
                new OWLDisjointDataProperties([hasTrackingCode, hasCarrierCode]),
                new OWLDataPropertyDomain(hasTrackingCode, order),
                new OWLDataPropertyRange(hasTrackingCode, new OWLDatatype(RDFVocabulary.XSD.STRING)),
                new OWLFunctionalDataProperty(hasTrackingCode)
            ]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsEmpty(violations);
    }

    //Every REAL OWLDataPropertyAxiom subtype existing today is admitted by RL (confirmed above, via
    //ShouldAllowEveryCurrentDataPropertyAxiomTypeAsync going through the full pipeline): this pins down the
    //FALSE side of the predicate, which no real axiom type can reach, by calling the extracted predicate
    //directly with the test-only FutureDataPropertyAxiom stand-in — confirming an unrecognized subtype
    //correctly defaults to "not admitted" rather than silently passing, without needing GetXML/serialization.
    [TestMethod]
    public void ShouldNotAdmitUnrecognizedDataPropertyAxiomType()
        => Assert.IsFalse(OWLRLProfile.IsAdmittedDataPropertyAxiomType(new FutureDataPropertyAxiom()));

    [TestMethod]
    public void ShouldAdmitEveryCurrentDataPropertyAxiomTypeDirectly()
    {
        OWLDataProperty dp1 = new OWLDataProperty(new RDFResource("http://shop.org/dp1"));
        OWLDataProperty dp2 = new OWLDataProperty(new RDFResource("http://shop.org/dp2"));
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLDataPropertyAxiom[] admittedAxioms =
        [
            new OWLSubDataPropertyOf(dp1, dp2),
            new OWLEquivalentDataProperties([dp1, dp2]),
            new OWLDisjointDataProperties([dp1, dp2]),
            new OWLDataPropertyDomain(dp1, order),
            new OWLDataPropertyRange(dp1, new OWLDatatype(RDFVocabulary.XSD.STRING)),
            new OWLFunctionalDataProperty(dp1)
        ];

        foreach (OWLDataPropertyAxiom axiom in admittedAxioms)
            Assert.IsTrue(OWLRLProfile.IsAdmittedDataPropertyAxiomType(axiom), $"expected {axiom.GetType().Name} to be admitted");
    }

    //--- Cross-cutting corner cases ---------------------------------------------------------------------------

    [TestMethod]
    public async Task ShouldReportMultipleIndependentViolationsInSameOntologyAsync()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLClass onlineOrder = new OWLClass(new RDFResource("http://shop.org/OnlineOrder"));
        OWLClass inStoreOrder = new OWLClass(new RDFResource("http://shop.org/InStoreOrder"));
        OWLObjectProperty relatedTo = new OWLObjectProperty(new RDFResource("http://shop.org/relatedTo"));
        OWLOntology ontology = new OWLOntology
        {
            //A class-expression violation (union in superclass position) and a class-axiom-type violation
            //(DisjointUnion), plus a ReflexiveObjectProperty violation, all unrelated to each other.
            ClassAxioms = [
                new OWLSubClassOf(order, new OWLObjectUnionOf([onlineOrder, inStoreOrder])),
                new OWLDisjointUnion(order, [onlineOrder, inStoreOrder])
            ],
            ObjectPropertyAxioms = [new OWLReflexiveObjectProperty(relatedTo)]
        };

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.HasCount(3, violations);
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ClassExpression")));
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ClassAxiomType")));
        Assert.IsTrue(violations.Any(v => v.RuleName.EndsWith(".ObjectPropertyAxiomType")));
    }
    #endregion
}