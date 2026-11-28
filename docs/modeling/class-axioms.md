---
title: Class axioms
permalink: /modeling/class-axioms/
tagline: Subclasses, definitions and classes that never meet
lede: Four axioms relate classes to each other. A subclass is contained in its superclass, equivalent classes have the same members, disjoint classes share none and a disjoint union splits a class into parts that do not overlap.
---

> In the [Property paths](https://mdesalvo.github.io/RDFSharp/query/property-paths/) chapter of the RDFSharp manual, every query that wanted "every wine, whatever its subclass" walked `rdfs:subClassOf*` by hand. In the ontology the hierarchy is stated once, and so are two things RDF Schema could not say: that a red wine is never a white wine and what makes a wine Tuscan.
{: .story}

## The four axioms

| Axiom | Manchester | Says |
|---|---|---|
| `OWLSubClassOf(sub, super)` | `Class: sub SubClassOf: super` | every individual of `sub` is an individual of `super` |
| `OWLEquivalentClasses(list)` | `EquivalentClasses: A, B` | the expressions have the same individuals |
| `OWLDisjointClasses(list)` | `DisjointClasses: A, B, C` | no individual belongs to two of them |
| `OWLDisjointUnion(C, list)` | `Class: C DisjointUnionOf: A, B` | `C` is the union of the expressions, which are disjoint |

They take class expressions, named or not, and go in `ClassAxioms`. On the ontology, a family of extension methods answers questions about them:

| Method | Answers |
|---|---|
| `GetSubClassesOf(C)`, `GetSuperClassesOf(C)` | the class expressions below or above `C`, following subclasses, equivalences and disjoint unions |
| `GetEquivalentClasses(C)` | the class expressions equivalent to `C`, transitively |
| `GetDisjointClasses(C)` | the class expressions disjoint with `C`, through `DisjointClasses` axioms, equivalences and subclasses |
| `CheckIsSubClassOf(A, B)`, `CheckIsSuperClassOf(A, B)`, `CheckAreEquivalentClasses(A, B)`, `CheckAreDisjointClasses(A, B)` | the same questions as yes or no |
| `GetClassAxiomsOfType<T>()` | the class axioms of one kind |

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");

string Short(OWLExpression expression)
    => expression.GetIRI().ToString().Replace(vr.ToString(), "vr:").Replace(vrd.ToString(), "vrd:");

OWLClass wine = new OWLClass(new RDFResource(vr + "Wine"));
OWLClass redWine = new OWLClass(new RDFResource(vr + "RedWine"));
OWLClass whiteWine = new OWLClass(new RDFResource(vr + "WhiteWine"));
OWLClass sweetWine = new OWLClass(new RDFResource(vr + "SweetWine"));

Console.WriteLine(string.Join(" ", vallerosa.GetSubClassesOf(wine).Select(Short).OrderBy(name => name)));
Console.WriteLine(vallerosa.CheckIsSubClassOf(sweetWine, wine));
```

<div class="result" markdown="1">

```
vr:RedWine vr:RoseWine vr:SweetWine vr:WhiteWine
True
```

</div>

## Subclasses

`SubClassOf` is the one class axiom that RDF Schema has too, as `rdfs:subClassOf`: the lift of the [previous chapter]({{ '/modeling/from-rdf/' | relative_url }}) brought the four colours below `vr:Wine`. With it, the members of a class include those of its subclasses, without any walk:

```csharp
Console.WriteLine(string.Join(" ", vallerosa.GetIndividualsOf(wine).Select(Short).OrderBy(name => name)));
```

<div class="result" markdown="1">

```
vrd:bianco-2023 vrd:riserva-2019 vrd:rosato-2024 vrd:rosso-2021 vrd:vermentino-2023 vrd:vinsanto-2015
```

</div>

The subclass of a `SubClassOf` can be any class expression, not only a named class. Such an axiom, a **general class inclusion**, states a sufficient condition: whatever satisfies the expression belongs to the superclass. It is the second half of the sentence that the RDFSharp manual leaves to OWL2, "a wine above 15 degrees made from dried grapes is a sweet wine". The first half was the expression of the [previous chapter]({{ '/modeling/class-expressions/' | relative_url }}):

<div class="pair" markdown="1">

```manchester
Class: vr:SweetWine

(vr:Wine
    and (vr:alcohol some xsd:decimal[> "15"^^xsd:decimal])
    and (vr:method value vrd:appassimento))
    SubClassOf: vr:SweetWine
```

```csharp
OWLDataProperty alcohol = new OWLDataProperty(new RDFResource(vr + "alcohol"));
OWLObjectProperty method = new OWLObjectProperty(new RDFResource(vr + "method"));
OWLNamedIndividual appassimento = new OWLNamedIndividual(new RDFResource(vrd + "appassimento"));

OWLDatatypeRestriction overFifteen = new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.DECIMAL),
    new List<OWLFacetRestriction> {
        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("15", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MIN_EXCLUSIVE) });
OWLClassExpression strongFromDriedGrapes = new OWLObjectIntersectionOf(new List<OWLClassExpression> {
    wine, new OWLDataSomeValuesFrom(alcohol, overFifteen), new OWLObjectHasValue(method, appassimento) });

vallerosa.ClassAxioms.Add(new OWLSubClassOf(strongFromDriedGrapes, sweetWine));
```

</div>

The Vin Santo is already a sweet wine in the catalog. The axiom shows what it is for when a new wine comes in: Tenuta Le Crete adds a straw wine of 2022, made from dried grapes with an alcohol of 15.5, and records it only as a wine.

```csharp
OWLNamedIndividual passito = new OWLNamedIndividual(new RDFResource(vrd + "passito-2022"));
vallerosa.DeclareEntity(passito);
vallerosa.AssertionAxioms.Add(new OWLClassAssertion(wine, passito));
vallerosa.AssertionAxioms.Add(new OWLDataPropertyAssertion(alcohol, passito,
    new OWLLiteral(new RDFTypedLiteral("15.5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
vallerosa.AssertionAxioms.Add(new OWLObjectPropertyAssertion(method, passito, appassimento));

Console.WriteLine(string.Join(" ", vallerosa.GetIndividualsOf(sweetWine).Select(Short).OrderBy(name => name)));
```

<div class="result" markdown="1">

```
vrd:passito-2022 vrd:vinsanto-2015
```

</div>

Nobody wrote that the straw wine is sweet: it is, because it satisfies the condition.

## Equivalent classes

`EquivalentClasses` states that the expressions have the same members. Between a named class and an anonymous expression it is a **definition**: the expression is both a necessary and a sufficient condition. The two defined classes of the Vallerosa ontology are of this kind:

```csharp
OWLClass tuscanWine = new OWLClass(new RDFResource(vr + "TuscanWine"));
OWLClass organicWine = new OWLClass(new RDFResource(vr + "OrganicWine"));

foreach (OWLEquivalentClasses definition in vallerosa.GetClassAxiomsOfType<OWLEquivalentClasses>())
    Console.WriteLine(string.Join(" = ", definition.ClassExpressions.Select(expression => expression.GetType().Name)));

Console.WriteLine("organic: " + string.Join(" ", vallerosa.GetIndividualsOf(organicWine).Select(Short).OrderBy(name => name)));
Console.WriteLine("Tuscan: " + string.Join(" ", vallerosa.GetIndividualsOf(tuscanWine).Select(Short).OrderBy(name => name)));
```

<div class="result" markdown="1">

```
OWLClass = OWLObjectIntersectionOf
OWLClass = OWLObjectIntersectionOf
organic: vrd:bianco-2023 vrd:vermentino-2023
Tuscan:
```

</div>

The organic wines are found at once: their definition asks for a value that the catalog states. The Tuscan wines are none, yet: their definition asks for `vr:comesFrom vrd:tuscany`, a fact that no wine of the file has. It follows from a chain of properties and a transitive one, which only the [reasoner]({{ '/reasoning/reasoner/' | relative_url }}) works out.

## Disjoint classes

`DisjointClasses` says that no individual belongs to two of the classes. RDF Schema had no way to say it: the catalog of the RDFSharp manual could hold a wine both red and white and nothing would object. The Vallerosa ontology states two disjointness axioms, the three colours and the nine top classes of the vocabulary:

<div class="pair" markdown="1">

```manchester
DisjointClasses: vr:RedWine, vr:WhiteWine, vr:RoseWine
```

```csharp
Console.WriteLine(vallerosa.CheckAreDisjointClasses(redWine, whiteWine));
Console.WriteLine(vallerosa.CheckAreDisjointClasses(sweetWine, whiteWine));
Console.WriteLine(string.Join(" ", vallerosa.GetDisjointClasses(wine).Select(Short).OrderBy(name => name)));
```

</div>

<div class="result" markdown="1">

```
True
False
vr:Area vr:Cooperative vr:Dish vr:Grape vr:Person vr:Producer vr:Sale vr:Shop
```

</div>

A sweet wine and a white wine are not disjoint: the Vin Santo is made from white grapes and a sweet white is possible. A wine is disjoint from the eight other top classes: it is never a winery, an area or a grape. Disjointness does not add facts; it forbids some. A wine asserted both red and white makes the ontology **inconsistent**, a contradiction that the [validator]({{ '/validation/' | relative_url }}) reports.

## Disjoint unions

`DisjointUnion` says two things at once: a class is the union of some class expressions and those expressions are pairwise disjoint. The cooperative sells still wines and sparkling wines, and every wine is exactly one of the two:

<div class="pair" markdown="1">

```manchester
Class: vr:Wine
    DisjointUnionOf: vr:StillWine, vr:SparklingWine
```

```csharp
OWLClass stillWine = new OWLClass(new RDFResource(vr + "StillWine"));
OWLClass sparklingWine = new OWLClass(new RDFResource(vr + "SparklingWine"));
vallerosa.DeclareEntity(stillWine);
vallerosa.DeclareEntity(sparklingWine);
vallerosa.ClassAxioms.Add(new OWLDisjointUnion(wine, new List<OWLClassExpression> { stillWine, sparklingWine }));

Console.WriteLine(vallerosa.CheckAreDisjointClasses(stillWine, sparklingWine));
Console.WriteLine(vallerosa.CheckIsSubClassOf(stillWine, wine));
```

</div>

<div class="result" markdown="1">

```
False
True
```

</div>

The answers are not the ones the axiom implies, and the difference tells how the helpers work. `CheckIsSubClassOf` follows disjoint unions, so the still wines are wines. `CheckAreDisjointClasses` reads the `DisjointClasses` axioms only, so it does not see that the parts of the union are disjoint. The reasoner turns the disjoint union into the axioms it implies (`SchemaDisjointClassesEntailment` in [Schema rules]({{ '/reasoning/schema-rules/' | relative_url }})); after it, the first question answers `true` as well.
