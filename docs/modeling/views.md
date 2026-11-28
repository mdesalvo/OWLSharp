---
title: Views
permalink: /modeling/views/
tagline: Everything the ontology says about one thing
lede: A view gathers in one object the questions you would ask about a class, a property, an individual or the whole ontology, each answered by an asynchronous method.
---

> In the [Query forms](https://mdesalvo.github.io/RDFSharp/query/query-forms/) chapter of the RDFSharp manual, a `DESCRIBE` asked for "everything about one resource": Fattoria Poggio Alto, as triples. A view asks the same of the ontology and answers with axioms, before reasoning and after.
{: .story}

## Five views

| View | Built on | Answers |
|---|---|---|
| `OWLOntologyView(ontology)` | the ontology | how many entities and axioms of each kind it holds (`ClassCountAsync`, `SubClassOfCountAsync`, ...) |
| `OWLClassView(class, ontology)` | a class | `SubClassesAsync`, `SuperClassesAsync`, `EquivalentClassesAsync`, `DisjointClassesAsync`, `IndividualsAsync`, `NegativeIndividualsAsync`, `KeysAsync`, annotations, `IsDeprecatedAsync` |
| `OWLObjectPropertyView(property, ontology)` | an object property | sub, super, equivalent, disjoint and inverse properties, `DomainsAsync`, `RangesAsync`, `ObjectAssertionsAsync`, the characteristics (`IsTransitiveAsync`, `IsFunctionalAsync`, ...) |
| `OWLDataPropertyView(property, ontology)` | a data property | sub, super, equivalent and disjoint properties, domains, ranges, `DataAssertionsAsync`, `IsFunctionalAsync` |
| `OWLNamedIndividualView(individual, ontology)` | a named individual | `ClassAssertionsAsync`, `ObjectAssertionsAsync`, `DataAssertionsAsync`, the negative assertions, `SameIndividualsAsync`, `DifferentIndividualsAsync`, annotations |

Every view answers on the ontology as it is when the method is called: the same view gives more answers after the [reasoner]({{ '/reasoning/reasoner/' | relative_url }}) has added its inferences. The annotations come in two methods, `ObjectAnnotationsAsync` for the annotations whose value is an IRI and `DataAnnotationsAsync` for those whose value is a literal.

## A winery

Fattoria Poggio Alto, as the files describe it:

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");
string Short(RDFResource iri) => iri.ToString().Replace(vr.ToString(), "vr:").Replace(vrd.ToString(), "vrd:");

OWLNamedIndividualView poggioAlto = new OWLNamedIndividualView(new OWLNamedIndividual(new RDFResource(vrd + "poggio-alto")), vallerosa);

async Task DescribeAsync()
{
    foreach (OWLClassAssertion assertion in await poggioAlto.ClassAssertionsAsync())
        Console.WriteLine($"a {Short(assertion.ClassExpression.GetIRI())}");
    foreach (OWLObjectPropertyAssertion assertion in (await poggioAlto.ObjectAssertionsAsync()).OrderBy(assertion => Short(assertion.ObjectPropertyExpression.GetIRI())))
        Console.WriteLine($"{Short(assertion.ObjectPropertyExpression.GetIRI())} {Short(assertion.TargetIndividualExpression.GetIRI())}");
    foreach (OWLAnnotationAssertion annotation in await poggioAlto.DataAnnotationsAsync())
        Console.WriteLine($"label {annotation.ValueLiteral.GetLiteral()}");
}

await DescribeAsync();
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

A member of the cooperative, located on the Hilltop, called Fattoria Poggio Alto: the three facts of the catalog. Its wines are not there, because the catalog states them from the side of the wines. After the reasoner, the same view knows more:

```csharp
await OWLReasoner.Default.ApplyToOntologyAsync(vallerosa);
await DescribeAsync();
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

The inverse of `vr:producedBy` gives the two wines, the domains give the class `vr:Producer` again from the properties it uses and `owl:Thing` comes from the class hierarchy closed by the Schema rules.

## A property and a class

The property that the RDFSharp manual walked by hand and the class it queried for every subclass:

```csharp
OWLObjectPropertyView comesFrom = new OWLObjectPropertyView(new OWLObjectProperty(new RDFResource(vr + "comesFrom")), vallerosa);
Console.WriteLine($"{(await comesFrom.ObjectAssertionsAsync()).Count} comesFrom links, domain {string.Join(" ", (await comesFrom.DomainsAsync()).Select(domain => Short(domain.GetIRI())))}");

OWLClassView wine = new OWLClassView(new OWLClass(new RDFResource(vr + "Wine")), vallerosa);
Console.WriteLine($"{(await wine.IndividualsAsync()).Count} wines, {(await wine.SubClassesAsync()).Count(subclass => subclass.IsClass)} named subclasses, disjoint with {(await wine.DisjointClassesAsync()).Count} classes");
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

## The whole ontology

`OWLOntologyView` counts. On the Vallerosa ontology, now reasoned:

```csharp
OWLOntologyView summary = new OWLOntologyView(vallerosa);
Console.WriteLine($"{await summary.ClassCountAsync()} classes, {await summary.ObjectPropertyCountAsync()} object properties, {await summary.DataPropertyCountAsync()} data properties, {await summary.NamedIndividualCountAsync()} named individuals");
Console.WriteLine($"{await summary.SubClassOfCountAsync()} SubClassOf, {await summary.ClassAssertionCountAsync()} class assertions, {await summary.ObjectPropertyAssertionCountAsync()} object property assertions");
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

The counts include the inferences: a view does not tell an asserted axiom from an inferred one. [Working with inferences]({{ '/reasoning/inferences/' | relative_url }}) shows how to tell them apart.
