---
title: Individuals and assertions
permalink: /modeling/assertions/
tagline: Facts, negative facts and the names of things
lede: Assertions are the facts of an ontology. They say which class an individual belongs to and what it is linked to, what it is not linked to, and which names stand for the same thing or for different things.
---

> The 2022 vintage of the Rosso reaches the cellar. In the [Insert and delete](https://mdesalvo.github.io/RDFSharp/query/insert-delete/) chapter of the RDFSharp manual it entered the catalog as eight triples of a SPARQL update; here it enters the ontology as assertions. The grapes linked to DBpedia by the [Federations](https://mdesalvo.github.io/RDFSharp/query/federations/) chapter get the meaning that `owl:sameAs` always promised.
{: .story}

## The assertions

| Assertion | Manchester | Says |
|---|---|---|
| `OWLClassAssertion(C, a)` | `Individual: a Types: C` | `a` belongs to the class expression `C` |
| `OWLObjectPropertyAssertion(p, a, b)` | `Individual: a Facts: p b` | `a` is linked to `b` by `p` |
| `OWLDataPropertyAssertion(d, a, v)` | `Individual: a Facts: d v` | `a` has the value `v` for `d` |
| `OWLNegativeObjectPropertyAssertion(p, a, b)` | `Individual: a Facts: not p b` | `a` is not linked to `b` by `p` |
| `OWLNegativeDataPropertyAssertion(d, a, v)` | `Individual: a Facts: not d v` | `a` does not have the value `v` for `d` |
| `OWLSameIndividual(list)` | `SameIndividual: a, b` | the names stand for one individual |
| `OWLDifferentIndividuals(list)` | `DifferentIndividuals: a, b, c` | the names stand for individuals that are all different |

They go in `AssertionAxioms` and take named or anonymous individuals. The ontology answers questions about them:

| Method | Answers |
|---|---|
| `GetIndividualsOf(C)`, `CheckIsIndividualOf(C, a)` | the members of a class expression ([Class expressions]({{ '/modeling/class-expressions/' | relative_url }})) |
| `CheckIsNegativeIndividualOf(C, a)` | whether `a` is asserted to the complement of `C` |
| `GetSameIndividuals(a)`, `CheckIsSameIndividual(a, b)` | the other names of `a`, transitively |
| `GetDifferentIndividuals(a)`, `CheckAreDifferentIndividuals(a, b)` | the individuals declared different from `a` |
| `GetAssertionAxiomsOfType<T>()` | the assertions of one kind |

## A new vintage

The Rosso of 2022 as the RDFSharp manual inserted it, with the vintage counted as an integer:

<div class="pair" markdown="1">

```manchester
Individual: vrd:rosso-2022
    Types: vr:RedWine
    Facts: vr:producedBy vrd:poggio-alto,
           vr:grape vrd:sangiovese, vr:grape vrd:canaiolo,
           vr:vintage 2022, vr:alcohol 13.5, vr:price 15.00,
           vr:organic "false"^^xsd:boolean
```

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");
string Short(OWLIndividualExpression individual)
    => individual.GetIRI().ToString().Replace(vrd.ToString(), "vrd:").Replace("http://dbpedia.org/resource/", "dbr:");

OWLClass redWine = new OWLClass(new RDFResource(vr + "RedWine"));
OWLObjectProperty producedBy = new OWLObjectProperty(new RDFResource(vr + "producedBy"));
OWLObjectProperty grape = new OWLObjectProperty(new RDFResource(vr + "grape"));
OWLDataProperty vintage = new OWLDataProperty(new RDFResource(vr + "vintage"));
OWLDataProperty alcohol = new OWLDataProperty(new RDFResource(vr + "alcohol"));
OWLDataProperty price = new OWLDataProperty(new RDFResource(vr + "price"));
OWLDataProperty organic = new OWLDataProperty(new RDFResource(vr + "organic"));
OWLNamedIndividual rosso2022 = new OWLNamedIndividual(new RDFResource(vrd + "rosso-2022"));
OWLNamedIndividual poggioAlto = new OWLNamedIndividual(new RDFResource(vrd + "poggio-alto"));
OWLNamedIndividual sangiovese = new OWLNamedIndividual(new RDFResource(vrd + "sangiovese"));
OWLNamedIndividual canaiolo = new OWLNamedIndividual(new RDFResource(vrd + "canaiolo"));

vallerosa.DeclareEntity(rosso2022);
vallerosa.AssertionAxioms.Add(new OWLClassAssertion(redWine, rosso2022));
vallerosa.AssertionAxioms.Add(new OWLObjectPropertyAssertion(producedBy, rosso2022, poggioAlto));
vallerosa.AssertionAxioms.Add(new OWLObjectPropertyAssertion(grape, rosso2022, sangiovese));
vallerosa.AssertionAxioms.Add(new OWLObjectPropertyAssertion(grape, rosso2022, canaiolo));
vallerosa.AssertionAxioms.Add(new OWLDataPropertyAssertion(vintage, rosso2022, new OWLLiteral(new RDFTypedLiteral("2022", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
vallerosa.AssertionAxioms.Add(new OWLDataPropertyAssertion(alcohol, rosso2022, new OWLLiteral(new RDFTypedLiteral("13.5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
vallerosa.AssertionAxioms.Add(new OWLDataPropertyAssertion(price, rosso2022, new OWLLiteral(new RDFTypedLiteral("15.00", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
vallerosa.AssertionAxioms.Add(new OWLDataPropertyAssertion(organic, rosso2022, new OWLLiteral(new RDFTypedLiteral("false", RDFModelEnums.RDFDatatypes.XSD_BOOLEAN))));

Console.WriteLine(string.Join(" ", vallerosa.GetIndividualsOf(redWine).Select(Short).OrderBy(name => name)));
```

</div>

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

An object property assertion links two individuals; it is also written the other way round with an inverse property, `OWLObjectInverseOf(producedBy)`, from the winery to the wine. A data property assertion takes an `OWLLiteral`, which wraps an RDFSharp literal: typed, as here, or plain with a language tag.

## Same names, different names

The catalog links each grape to its DBpedia resource with `owl:sameAs`, which the lift reads as `SameIndividual`. Two names, one grape:

```csharp
Console.WriteLine(string.Join(" ", vallerosa.GetSameIndividuals(sangiovese).Select(Short)));
Console.WriteLine(vallerosa.CheckIsSameIndividual(new OWLNamedIndividual(new RDFResource("http://dbpedia.org/resource/Sangiovese")), sangiovese));
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

The other way round is less obvious. OWL2 makes **no unique name assumption**: two different IRIs may well name the same individual, unless the ontology says otherwise. Nothing in the catalog says that Poggio Alto, Le Crete and Tre Ulivi are three wineries. Usually it does not matter; it matters as soon as an axiom could merge them. `vr:producedBy` is functional, so a wine recorded with two producers makes the reasoner conclude that the two names are one winery. The cooperative knows better and says so:

<div class="pair" markdown="1">

```manchester
DifferentIndividuals: vrd:poggio-alto, vrd:le-crete, vrd:tre-ulivi
```

```csharp
OWLNamedIndividual leCrete = new OWLNamedIndividual(new RDFResource(vrd + "le-crete"));
OWLNamedIndividual treUlivi = new OWLNamedIndividual(new RDFResource(vrd + "tre-ulivi"));

Console.WriteLine(vallerosa.CheckAreDifferentIndividuals(poggioAlto, leCrete));
vallerosa.AssertionAxioms.Add(new OWLDifferentIndividuals(new List<OWLIndividualExpression> { poggioAlto, leCrete, treUlivi }));
Console.WriteLine(vallerosa.CheckAreDifferentIndividuals(poggioAlto, leCrete));
```

</div>

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

With the three wineries different, a wine with two producers is no longer a merger: it is a contradiction, which the [validator]({{ '/validation/' | relative_url }}) reports.

## Negative facts

Under the open world, a fact that is not stated is unknown. A **negative assertion** states that a fact does not hold. The RDFSharp manual paired grilled steak, `vrd:grilled-steak`, with the wines of Poggio Alto; the cooperative wants it on record that the Vin Santo does not go with it:

<div class="pair" markdown="1">

```manchester
Individual: vrd:vinsanto-2015
    Facts: not vr:pairsWith vrd:grilled-steak
```

```csharp
OWLObjectProperty pairsWith = new OWLObjectProperty(new RDFResource(vr + "pairsWith"));
OWLNamedIndividual vinsanto = new OWLNamedIndividual(new RDFResource(vrd + "vinsanto-2015"));
OWLNamedIndividual grilledSteak = new OWLNamedIndividual(new RDFResource(vrd + "grilled-steak"));

vallerosa.DeclareEntity(grilledSteak);
vallerosa.AssertionAxioms.Add(new OWLNegativeObjectPropertyAssertion(pairsWith, vinsanto, grilledSteak));
Console.WriteLine(vallerosa.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>().Count);
```

</div>

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

A negative assertion adds no fact for the reasoner to propagate; it forbids one. If an import later pairs the Vin Santo with grilled steak, the ontology contradicts itself and the validator says so (`NegativeObjectAssertionsAnalysis`, in [Analysis rules]({{ '/validation/analysis-rules/' | relative_url }})).
