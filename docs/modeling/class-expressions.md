---
title: Class expressions
permalink: /modeling/class-expressions/
tagline: Describe a class by what its members are
lede: A class expression describes a set of individuals without naming it, by combining classes and by restricting properties. It is the piece of OWL2 that turns the filters of a query into knowledge the ontology holds.
---

> The RDFSharp manual asked its questions in SPARQL: reds and whites together in [Variables and patterns](https://mdesalvo.github.io/RDFSharp/query/patterns/), the light whites under 13 degrees in [Filters](https://mdesalvo.github.io/RDFSharp/query/filters/), the wines of the two grapes of a tasting night in [Values](https://mdesalvo.github.io/RDFSharp/query/values/). Each of those questions is a set of wines, and OWL2 can describe each set.
{: .story}

## Expressions and their members

Every class expression is an `OWLClassExpression`; a named class, `OWLClass`, is the simplest one. The others are built from classes, properties, individuals and values:

| Expression | Manchester | Members |
|---|---|---|
| `OWLObjectIntersectionOf(list)` | `A and B` | the individuals of all the expressions |
| `OWLObjectUnionOf(list)` | `A or B` | the individuals of at least one expression |
| `OWLObjectComplementOf(A)` | `not A` | the individuals that are not in `A` |
| `OWLObjectOneOf(list)` | `{a, b}` | exactly the individuals listed |
| `OWLObjectSomeValuesFrom(p, A)` | `p some A` | linked by `p` to at least one individual of `A` |
| `OWLObjectAllValuesFrom(p, A)` | `p only A` | linked by `p` to individuals of `A` only |
| `OWLObjectHasValue(p, a)` | `p value a` | linked by `p` to the individual `a` |
| `OWLObjectHasSelf(p)` | `p Self` | linked by `p` to themselves |
| `OWLObjectMinCardinality(p, n, A)` | `p min n A` | linked by `p` to at least `n` individuals (of `A`) |
| `OWLObjectMaxCardinality(p, n, A)` | `p max n A` | linked by `p` to at most `n` individuals (of `A`) |
| `OWLObjectExactCardinality(p, n, A)` | `p exactly n A` | linked by `p` to exactly `n` individuals (of `A`) |
| `OWLDataSomeValuesFrom(d, R)` | `d some R` | with at least one value of `d` in the data range `R` |
| `OWLDataAllValuesFrom(d, R)` | `d only R` | with values of `d` in `R` only |
| `OWLDataHasValue(d, v)` | `d value v` | with the value `v` for `d` |
| `OWLDataMinCardinality`, `OWLDataMaxCardinality`, `OWLDataExactCardinality` | `d min n R`, ... | with at least, at most, exactly `n` values of `d` (in `R`) |

The property `p` of the object restrictions is an object property or its inverse, `OWLObjectInverseOf(p)`, written `inverse p`: `inverse vr:producedBy` links a winery to its wines. The data range `R` of the data restrictions is a datatype or one of the data ranges of [Data properties and datatypes]({{ '/modeling/data-properties/' | relative_url }}). The cardinalities without a class or a data range count every link.

## Asking for the members

`GetIndividualsOf(expression)`, an extension method on the ontology, returns the individuals of a class expression: those asserted to it, those of its subclasses and equivalent classes, and those that satisfy its definition according to the facts of the ontology. The examples of this chapter load the Vallerosa ontology and print the members of each expression in short form:

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");

void PrintMembers(OWLClassExpression expression)
{
    List<string> members = vallerosa.GetIndividualsOf(expression)
                                    .Select(individual => individual.GetIRI().ToString()
                                        .Replace(vrd.ToString(), "vrd:").Replace("http://dbpedia.org/resource/", "dbr:"))
                                    .OrderBy(member => member).ToList();
    Console.WriteLine(members.Count == 0 ? "(none)" : string.Join(" ", members));
}

OWLClass wine = new OWLClass(new RDFResource(vr + "Wine"));
OWLClass redWine = new OWLClass(new RDFResource(vr + "RedWine"));
OWLClass whiteWine = new OWLClass(new RDFResource(vr + "WhiteWine"));
OWLObjectProperty producedBy = new OWLObjectProperty(new RDFResource(vr + "producedBy"));
OWLObjectProperty grape = new OWLObjectProperty(new RDFResource(vr + "grape"));
OWLObjectProperty method = new OWLObjectProperty(new RDFResource(vr + "method"));
OWLDataProperty alcohol = new OWLDataProperty(new RDFResource(vr + "alcohol"));
OWLDataProperty organic = new OWLDataProperty(new RDFResource(vr + "organic"));

PrintMembers(wine);
```

<div class="result" markdown="1">

```
vrd:bianco-2023 vrd:riserva-2019 vrd:rosato-2024 vrd:rosso-2021 vrd:vermentino-2023 vrd:vinsanto-2015
```

</div>

No wine of the file is asserted to be a `vr:Wine`: the six come from the four subclasses of the colours.

## And, or, not

Reds and whites together, the union of the [patterns](https://mdesalvo.github.io/RDFSharp/query/patterns/) chapter:

<div class="pair" markdown="1">

```manchester
vr:RedWine or vr:WhiteWine
```

```csharp
PrintMembers(new OWLObjectUnionOf(new List<OWLClassExpression> { redWine, whiteWine }));
```

</div>

<div class="result" markdown="1">

```
vrd:bianco-2023 vrd:riserva-2019 vrd:rosso-2021 vrd:vermentino-2023
```

</div>

The Vin Santo is not among them: the catalog says it is a sweet wine and nothing says it is a white one.

An intersection keeps the individuals of all its parts. The organic wines, where the RDFSharp manual tested the organic flag in a filter:

<div class="pair" markdown="1">

```manchester
vr:Wine and (vr:organic value "true"^^xsd:boolean)
```

```csharp
OWLLiteral yes = new OWLLiteral(new RDFTypedLiteral("true", RDFModelEnums.RDFDatatypes.XSD_BOOLEAN));
PrintMembers(new OWLObjectIntersectionOf(new List<OWLClassExpression> { wine, new OWLDataHasValue(organic, yes) }));
```

</div>

<div class="result" markdown="1">

```
vrd:bianco-2023 vrd:vermentino-2023
```

</div>

The complement is where OWL2 stops behaving like a query. The wines that are not organic look like an easy question:

```csharp
OWLClass organicWine = new OWLClass(new RDFResource(vr + "OrganicWine"));
PrintMembers(new OWLObjectIntersectionOf(new List<OWLClassExpression> { wine, new OWLObjectComplementOf(organicWine) }));
```

<div class="result" markdown="1">

```
(none)
```

</div>

None, and the reason is worth knowing. OWL2 reasons under the **open world assumption**: what the ontology does not say is unknown, not false. A SPARQL query with `FILTER NOT EXISTS` answers on what is written; OWL2 answers only with what follows from it. For the Vallerosa Red something does follow: `vr:organic` is functional and the Red has the value `false`, so it cannot also have `true` and a complete OWL2 reasoner proves that it is not an organic wine. OWLSharp works with rules, like the reasoners of the OWL 2 RL profile, and rules do not derive that an individual belongs to a complement: `GetIndividualsOf` gives for `not A` only the individuals asserted to it. To ask for the wines that are not organic, ask for what the ontology states, `vr:organic value false`.

## Enumerations

The tasting night of the [Values](https://mdesalvo.github.io/RDFSharp/query/values/) chapter needed the two grapes as a fixed list, because "nothing in the catalog says that these two belong together". An enumeration is that list as a class:

<div class="pair" markdown="1">

```manchester
{vrd:sangiovese, vrd:vermentino}
```

```csharp
OWLObjectOneOf tastingGrapes = new OWLObjectOneOf(new List<OWLIndividualExpression> {
    new OWLNamedIndividual(new RDFResource(vrd + "sangiovese")),
    new OWLNamedIndividual(new RDFResource(vrd + "vermentino")) });
PrintMembers(tastingGrapes);
```

</div>

<div class="result" markdown="1">

```
dbr:Sangiovese dbr:Vermentino vrd:sangiovese vrd:vermentino
```

</div>

Four members for two names: the catalog says that `vrd:sangiovese` is the same individual as `dbr:Sangiovese` of DBpedia, so the enumeration holds it under both names. `GetIndividualsOf` follows `SameIndividual` unless its second argument, `enableSameAsEntailment`, is `false`.

## Object restrictions

A restriction describes individuals by their links. The wines made from a grape of the tasting night:

<div class="pair" markdown="1">

```manchester
vr:grape some {vrd:sangiovese, vrd:vermentino}
```

```csharp
PrintMembers(new OWLObjectSomeValuesFrom(grape, tastingGrapes));
```

</div>

<div class="result" markdown="1">

```
vrd:riserva-2019 vrd:rosato-2024 vrd:rosso-2021 vrd:vermentino-2023
```

</div>

The wines of one winery, and the wineries of the red wines through the inverse of `vr:producedBy`:

<div class="pair" markdown="1">

```manchester
vr:producedBy value vrd:poggio-alto

inverse vr:producedBy some vr:RedWine
```

```csharp
PrintMembers(new OWLObjectHasValue(producedBy, new OWLNamedIndividual(new RDFResource(vrd + "poggio-alto"))));
PrintMembers(new OWLObjectSomeValuesFrom(new OWLObjectInverseOf(producedBy), redWine));
```

</div>

<div class="result" markdown="1">

```
vrd:riserva-2019 vrd:rosso-2021
vrd:poggio-alto
```

</div>

Cardinalities count the links. The blends, wines of two grapes or more, against the wines of exactly one grape:

<div class="pair" markdown="1">

```manchester
vr:grape min 2

vr:grape exactly 1
```

```csharp
PrintMembers(new OWLObjectMinCardinality(grape, 2));
PrintMembers(new OWLObjectExactCardinality(grape, 1));
```

</div>

<div class="result" markdown="1">

```
vrd:bianco-2023 vrd:rosso-2021 vrd:vinsanto-2015
(none)
```

</div>

The open world again: the ontology knows that the Vallerosa Reserve has the grape Sangiovese, not that it has no other grape, so "exactly one" cannot be proved. `OWLObjectMaxCardinality`, `OWLObjectExactCardinality` and `OWLObjectAllValuesFrom`, like their data counterparts, get from `GetIndividualsOf` only the individuals asserted to them; the reasoner uses them the other way round, to deduce facts about the individuals that belong to them ([Fact rules]({{ '/reasoning/fact-rules/' | relative_url }})).

## Data restrictions

A data restriction looks at values. The light whites of the [Filters](https://mdesalvo.github.io/RDFSharp/query/filters/) chapter, white wines with an alcohol under 13, use a datatype restricted by a facet:

<div class="pair" markdown="1">

```manchester
vr:WhiteWine and (vr:alcohol some xsd:decimal[< "13"^^xsd:decimal])
```

```csharp
OWLDatatypeRestriction underThirteen = new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.DECIMAL),
    new List<OWLFacetRestriction> {
        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("13", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MAX_EXCLUSIVE) });
PrintMembers(new OWLObjectIntersectionOf(new List<OWLClassExpression> { whiteWine, new OWLDataSomeValuesFrom(alcohol, underThirteen) }));
```

</div>

<div class="result" markdown="1">

```
vrd:bianco-2023 vrd:vermentino-2023
```

</div>

## A promise of the RDFSharp manual

The last page of the RDFSharp manual says that RDF Schema cannot express "that a wine above 15 degrees made from dried grapes is a sweet wine". The first half of that sentence is a class expression: wines, with an alcohol above 15, made with the `vrd:appassimento` method.

<div class="pair" markdown="1">

```manchester
vr:Wine
    and (vr:alcohol some xsd:decimal[> "15"^^xsd:decimal])
    and (vr:method value vrd:appassimento)
```

```csharp
OWLDatatypeRestriction overFifteen = new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.DECIMAL),
    new List<OWLFacetRestriction> {
        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("15", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MIN_EXCLUSIVE) });
OWLClassExpression strongFromDriedGrapes = new OWLObjectIntersectionOf(new List<OWLClassExpression> {
    wine,
    new OWLDataSomeValuesFrom(alcohol, overFifteen),
    new OWLObjectHasValue(method, new OWLNamedIndividual(new RDFResource(vrd + "appassimento"))) });
PrintMembers(strongFromDriedGrapes);
```

</div>

<div class="result" markdown="1">

```
vrd:vinsanto-2015
```

</div>

The second half, "is a sweet wine", is an axiom: [Class axioms]({{ '/modeling/class-axioms/' | relative_url }}) makes this expression a subclass of `vr:SweetWine`.

## What GetIndividualsOf answers

| Expression | Answered from |
|---|---|
| a class | its assertions, its subclasses, its equivalent classes |
| `and`, `or`, `{...}` | their parts, evaluated one by one |
| `some`, `value`, `Self`, `min` (object and data) | the property assertions of the ontology |
| `not`, `only`, `max`, `exactly` (object and data) | the class assertions made on the expression only: proving them needs what the ontology does not say, which the rules of OWLSharp never assume |

The members of an expression are computed on the facts that the ontology holds when it is asked. The [reasoner]({{ '/reasoning/reasoner/' | relative_url }}) adds the facts that follow from the axioms, after which the same question can have more answers.
