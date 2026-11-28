---
title: Object property axioms
permalink: /modeling/object-properties/
tagline: What a link between individuals implies
lede: Object property axioms say what a relation between individuals is like. A part of a part is a part, a wine has one producer, a winery produces what is produced by it and a wine comes from wherever its producer is. Each of these is one axiom.
---

> In the [Property paths](https://mdesalvo.github.io/RDFSharp/query/property-paths/) chapter of the RDFSharp manual, the cooperative wanted every wine of Tuscany, at the end of "a chain of unknown length": wine, producer, subzone, valley, region. The path `vr:producedBy/vr:locatedIn/vr:partOf*` had to be written in every query that needed it. The ontology says it once.
{: .story}

## The axioms

| Axiom | Manchester | Says |
|---|---|---|
| `OWLObjectPropertyDomain(p, C)` | `Domain: C` | whatever has a `p` link is a `C` |
| `OWLObjectPropertyRange(p, C)` | `Range: C` | whatever a `p` link reaches is a `C` |
| `OWLSubObjectPropertyOf(p, q)` | `SubPropertyOf: q` | every `p` link is a `q` link |
| `OWLSubObjectPropertyOf(chain, q)` | `SubPropertyChain: p1 o p2` | following `p1` then `p2` gives a `q` link |
| `OWLEquivalentObjectProperties(list)` | `EquivalentTo:` | the properties have the same links |
| `OWLDisjointObjectProperties(list)` | `DisjointWith:` | no two individuals are linked by two of them |
| `OWLInverseObjectProperties(p, q)` | `InverseOf: q` | `a p b` exactly when `b q a` |
| `OWLFunctionalObjectProperty(p)` | `Characteristics: Functional` | an individual has at most one `p` link |
| `OWLInverseFunctionalObjectProperty(p)` | `Characteristics: InverseFunctional` | an individual is reached by at most one `p` link |
| `OWLTransitiveObjectProperty(p)` | `Characteristics: Transitive` | `a p b` and `b p c` give `a p c` |
| `OWLSymmetricObjectProperty(p)` | `Characteristics: Symmetric` | `a p b` gives `b p a` |
| `OWLAsymmetricObjectProperty(p)` | `Characteristics: Asymmetric` | `a p b` excludes `b p a` |
| `OWLReflexiveObjectProperty(p)` | `Characteristics: Reflexive` | every individual is linked to itself |
| `OWLIrreflexiveObjectProperty(p)` | `Characteristics: Irreflexive` | no individual is linked to itself |

They go in `ObjectPropertyAxioms`. The characteristics take an object property or its inverse; the other axioms take any object property expression. The ontology answers questions about them with `GetSubObjectPropertiesOf`, `GetSuperObjectPropertiesOf`, `GetEquivalentObjectProperties`, `GetDisjointObjectProperties` and their `Check...` forms, and with one `CheckHas...ObjectProperty` method for each characteristic.

## Characteristics

The Vallerosa ontology gives three of its properties a characteristic or an inverse:

<div class="pair" markdown="1">

```manchester
ObjectProperty: vr:partOf
    Characteristics: Transitive

ObjectProperty: vr:producedBy
    InverseOf: vr:produces
    Characteristics: Functional
```

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");

OWLObjectProperty partOf = new OWLObjectProperty(new RDFResource(vr + "partOf"));
OWLObjectProperty producedBy = new OWLObjectProperty(new RDFResource(vr + "producedBy"));
OWLObjectProperty produces = new OWLObjectProperty(new RDFResource(vr + "produces"));
OWLObjectProperty locatedIn = new OWLObjectProperty(new RDFResource(vr + "locatedIn"));
OWLObjectProperty comesFrom = new OWLObjectProperty(new RDFResource(vr + "comesFrom"));

Console.WriteLine(vallerosa.CheckHasTransitiveObjectProperty(partOf));
Console.WriteLine(vallerosa.CheckHasFunctionalObjectProperty(producedBy));
Console.WriteLine(vallerosa.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>().Count);
```

</div>

<div class="result" markdown="1">

```
True
True
1
```

</div>

What each one buys:

- `vr:partOf` is **transitive**: the catalog says that the Hilltop is part of the valley and the valley part of Tuscany, so the Hilltop is part of Tuscany, and of Italy.
- `vr:producedBy` is **functional**: a wine has one producer. If an import gives a wine a second producer, either the two names are the same winery or the import is wrong; the reasoner deduces the first ([Fact rules]({{ '/reasoning/fact-rules/' | relative_url }})) and the [validator]({{ '/validation/' | relative_url }}) reports the second when the two wineries are known to be different.
- `vr:produces` is the **inverse** of `vr:producedBy`: the catalog writes only `vrd:rosso-2021 vr:producedBy vrd:poggio-alto`, and the ontology knows that Poggio Alto produces the Vallerosa Red.

## Chains of properties

A chain makes one property out of several in a row. `vr:comesFrom` has two:

<div class="pair" markdown="1">

```manchester
ObjectProperty: vr:comesFrom
    SubPropertyChain: vr:producedBy o vr:locatedIn, vr:comesFrom o vr:partOf
    Domain: vr:Wine
    Range: vr:Area
```

```csharp
foreach (OWLSubObjectPropertyOf subProperty in vallerosa.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>())
    if (subProperty.SubObjectPropertyChain != null)
        Console.WriteLine(string.Join(" o ", subProperty.SubObjectPropertyChain.ObjectPropertyExpressions.Select(link => link.GetIRI().ToString().Replace(vr.ToString(), "vr:")))
                          + " -> " + subProperty.SuperObjectPropertyExpression.GetIRI().ToString().Replace(vr.ToString(), "vr:"));
```

</div>

<div class="result" markdown="1">

```
vr:producedBy o vr:locatedIn -> vr:comesFrom
vr:comesFrom o vr:partOf -> vr:comesFrom
```

</div>

The first chain says that a wine comes from the area of its producer. The second, recursive, says that whatever a wine comes from, it also comes from every area containing it. Together they are the property path of the RDFSharp manual, `vr:producedBy/vr:locatedIn/vr:partOf*`, as a property of the ontology.

A chain is built with `OWLObjectPropertyChain`, a list of object property expressions, and goes in the `OWLSubObjectPropertyOf` constructor that takes a chain:

<!-- pagecheck: skip -->
```csharp
vallerosa.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(
    new OWLObjectPropertyChain(new List<OWLObjectPropertyExpression> { producedBy, locatedIn }), comesFrom));
```

## What OWL 2 DL forbids

A property defined by a chain, or a transitive one, is **not simple**: its links follow from other links. OWL 2 DL keeps reasoning decidable by [not allowing such properties](https://www.w3.org/TR/owl2-syntax/#Global_Restrictions_on_Axioms_in_OWL_2_DL) in some places: in cardinality restrictions and `Self`, and in functional, inverse functional, irreflexive, asymmetric and disjointness axioms. `vr:comesFrom` cannot be made functional and `vr:partOf` cannot be made irreflexive, however true that might be: the ontology would leave OWL 2 DL. The chains must also be regular: a chain may use the property it defines only as its first or its last link, as the second chain above does.

## What the reasoner makes of them

The axioms add no fact by themselves; the [reasoner]({{ '/reasoning/reasoner/' | relative_url }}) applies them. A preview, on the Vin Santo:

```csharp
OWLNamedIndividual vinsanto = new OWLNamedIndividual(new RDFResource(vrd + "vinsanto-2015"));
OWLNamedIndividual leCrete = new OWLNamedIndividual(new RDFResource(vrd + "le-crete"));

await OWLSharp.Reasoner.OWLReasoner.Default.ApplyToOntologyAsync(vallerosa);

Console.WriteLine("comes from: " + string.Join(" ", vallerosa.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>()
    .Where(assertion => assertion.ObjectPropertyExpression.GetIRI().Equals(comesFrom.GetIRI()) && assertion.SourceIndividualExpression.GetIRI().Equals(vinsanto.GetIRI()))
    .Select(assertion => assertion.TargetIndividualExpression.GetIRI().ToString().Replace(vrd.ToString(), "vrd:")).OrderBy(area => area)));
Console.WriteLine("Le Crete produces: " + string.Join(" ", vallerosa.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>()
    .Where(assertion => assertion.ObjectPropertyExpression.GetIRI().Equals(produces.GetIRI()) && assertion.SourceIndividualExpression.GetIRI().Equals(leCrete.GetIRI()))
    .Select(assertion => assertion.TargetIndividualExpression.GetIRI().ToString().Replace(vrd.ToString(), "vrd:")).OrderBy(wine => wine)));
```

<div class="result" markdown="1">

```
comes from: vrd:colle-nord vrd:italy vrd:tuscany vrd:valle-rosa
Le Crete produces: vrd:bianco-2023 vrd:vinsanto-2015
```

</div>

The Vin Santo comes from the North Hill, the Rose Valley, Tuscany and Italy, four facts that no file states. Tenuta Le Crete produces its two wines, though the catalog only says it the other way round.
