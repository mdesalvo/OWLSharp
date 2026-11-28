---
title: Data properties and datatypes
permalink: /modeling/data-properties/
tagline: Values, the ranges they live in and the keys they make
lede: Data property axioms say what a relation between an individual and a value is like. Data ranges describe sets of values, from a plain datatype to a decimal between 0 and 20, and a datatype definition gives such a set a name. Keys turn values into identity.
---

> In the [Datatypes and facets](https://mdesalvo.github.io/RDFSharp/model/datatypes/) chapter of the RDFSharp manual, a spreadsheet import brought a wine with an alcohol of 135 and the cooperative taught the library that an alcohol by volume is a decimal from 0 to 20. The Vallerosa ontology keeps that lesson as a datatype of its own, `vr:abv`.
{: .story}

## Data property axioms

| Axiom | Manchester | Says |
|---|---|---|
| `OWLDataPropertyDomain(d, C)` | `Domain: C` | whatever has a `d` value is a `C` |
| `OWLDataPropertyRange(d, R)` | `Range: R` | every `d` value is in the data range `R` |
| `OWLSubDataPropertyOf(d, e)` | `SubPropertyOf: e` | every `d` value is an `e` value |
| `OWLEquivalentDataProperties(list)` | `EquivalentTo:` | the properties have the same values |
| `OWLDisjointDataProperties(list)` | `DisjointWith:` | no individual has the same value for two of them |
| `OWLFunctionalDataProperty(d)` | `Characteristics: Functional` | an individual has at most one `d` value |

They go in `DataPropertyAxioms`. The ontology answers with `GetSubDataPropertiesOf`, `GetSuperDataPropertiesOf`, `GetEquivalentDataProperties`, `GetDisjointDataProperties`, their `Check...` forms and `CheckHasFunctionalDataProperty`. The four data properties of the Vallerosa ontology are functional, each with its range:

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");

foreach (OWLDataPropertyRange range in vallerosa.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>())
    Console.WriteLine($"{range.DataProperty.GetIRI().ToString().Replace(vr.ToString(), "vr:")} functional={vallerosa.CheckHasFunctionalDataProperty(range.DataProperty)} range={range.DataRangeExpression.GetIRI().ToString().Replace(vr.ToString(), "vr:").Replace("http://www.w3.org/2001/XMLSchema#", "xsd:")}");
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

## Data ranges

A data range is a set of values, as a class is a set of individuals. Every data range is an `OWLDataRangeExpression`:

| Data range | Manchester | Values |
|---|---|---|
| `OWLDatatype(iri)` | `xsd:decimal` | the values of a datatype |
| `OWLDatatypeRestriction(datatype, facets)` | `xsd:decimal[>= 0, <= 20]` | the values of a datatype that satisfy every facet |
| `OWLDataOneOf(literals)` | `{"it", "en"}` | exactly the values listed |
| `OWLDataIntersectionOf(list)` | `R and S` | the values in all the ranges |
| `OWLDataUnionOf(list)` | `R or S` | the values in at least one range |
| `OWLDataComplementOf(R)` | `not R` | the values outside `R` |

A facet is an `OWLFacetRestriction(literal, facet)`, where the facet is one of the IRIs of XML Schema that RDFSharp has in `RDFVocabulary.XSD`:

| Facet | Manchester | For |
|---|---|---|
| `MIN_INCLUSIVE`, `MIN_EXCLUSIVE`, `MAX_INCLUSIVE`, `MAX_EXCLUSIVE` | `>=`, `>`, `<=`, `<` | numbers and dates: the bound is a value of the datatype, decimals and negatives included |
| `LENGTH`, `MIN_LENGTH`, `MAX_LENGTH` | `length`, `minLength`, `maxLength` | strings and binary data: a count, a non-negative integer |
| `PATTERN` | `pattern` | strings: a regular expression the whole value must match |

`CheckIsLiteralOf(range, literal)` tells whether a value belongs to a data range. The import of the RDFSharp manual, 135 instead of 13.5, against the alcohol of the cooperative:

```csharp
OWLDatatypeRestriction alcoholByVolume = new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.DECIMAL),
    new List<OWLFacetRestriction> {
        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("0", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MIN_INCLUSIVE),
        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("20", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MAX_INCLUSIVE) });

OWLLiteral right = new OWLLiteral(new RDFTypedLiteral("13.5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));
OWLLiteral wrong = new OWLLiteral(new RDFTypedLiteral("135", RDFModelEnums.RDFDatatypes.XSD_DECIMAL));

Console.WriteLine(vallerosa.CheckIsLiteralOf(alcoholByVolume, right));
Console.WriteLine(vallerosa.CheckIsLiteralOf(alcoholByVolume, wrong));
Console.WriteLine(vallerosa.CheckIsLiteralOf(new OWLDataComplementOf(alcoholByVolume), wrong));
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

## Datatype definitions

A restriction written in every axiom that needs it is a restriction copied by hand. `OWLDatatypeDefinition(datatype, range)` gives it a name: the datatype has exactly the values of the range. The Vallerosa ontology defines `vr:abv` and uses it as the range of `vr:alcohol`:

<div class="pair" markdown="1">

```manchester
Datatype: vr:abv
    EquivalentTo: xsd:decimal[>= "0"^^xsd:decimal, <= "20"^^xsd:decimal]

DataProperty: vr:alcohol
    Characteristics: Functional
    Range: vr:abv
```

```csharp
OWLDatatype abv = new OWLDatatype(new RDFResource(vr + "abv"));

Console.WriteLine(vallerosa.CheckHasDatatypeDefinition(new OWLDatatypeDefinition(abv, alcoholByVolume)));
Console.WriteLine(vallerosa.CheckIsLiteralOf(abv, right));
Console.WriteLine(vallerosa.CheckIsLiteralOf(abv, wrong));
```

</div>

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

The named datatype answers like its definition. A definition can use other defined datatypes; OWL 2 DL forbids cycles among them, and `CheckIsLiteralOf` answers `false` rather than loop on one.

Two things to know about datatypes outside the [OWL 2 datatype map](https://www.w3.org/TR/owl2-syntax/#Datatype_Maps). A datatype of your own, such as `vr:abv`, is fine as long as a definition says what it is. A datatype of XML Schema outside the map, such as `xsd:gYear` or `xsd:date`, is not allowed in OWL 2 DL, which is why the Vallerosa ontology counts its vintages as integers ([From RDF to OWL]({{ '/modeling/from-rdf/' | relative_url }})). The [OWL2 profiles]({{ '/profiling/profiles/' | relative_url }}) are stricter still: none of them admits a datatype restriction, so `vr:abv` is the one axiom that keeps the Vallerosa ontology out of OWL 2 RL.

## Keys

A key says that the individuals of a class with the same values for some properties are the same individual. `OWLHasKey(class, properties)` takes object properties or data properties and goes in `KeyAxioms`; keys apply only to named individuals.

The cooperative registers its wineries with their VAT numbers. An import from the regional register brings Poggio Alto under another name, with the same number:

<div class="pair" markdown="1">

```manchester
Class: vr:Producer
    HasKey: vr:vatNumber
```

```csharp
OWLClass producer = new OWLClass(new RDFResource(vr + "Producer"));
OWLDataProperty vatNumber = new OWLDataProperty(new RDFResource(vr + "vatNumber"));
OWLNamedIndividual poggioAlto = new OWLNamedIndividual(new RDFResource(vrd + "poggio-alto"));
OWLNamedIndividual registered = new OWLNamedIndividual(new RDFResource(vrd + "IT01234560527"));
OWLLiteral number = new OWLLiteral(new RDFTypedLiteral("IT01234560527", RDFModelEnums.RDFDatatypes.XSD_STRING));

vallerosa.DeclareEntity(vatNumber);
vallerosa.DeclareEntity(registered);
vallerosa.KeyAxioms.Add(new OWLHasKey(producer, new List<OWLDataProperty> { vatNumber }));
vallerosa.AssertionAxioms.Add(new OWLDataPropertyAssertion(vatNumber, poggioAlto, number));
vallerosa.AssertionAxioms.Add(new OWLClassAssertion(producer, registered));
vallerosa.AssertionAxioms.Add(new OWLDataPropertyAssertion(vatNumber, registered, number));

OWLSharp.Reasoner.OWLReasonerReport report = await OWLSharp.Reasoner.OWLReasoner.Default.ApplyToOntologyAsync(vallerosa);
foreach (OWLSharp.Reasoner.OWLInference inference in report.Inferences.Where(inference => inference.RuleName == "FactHasKeyEntailment"))
    Console.WriteLine(string.Join(" = ", ((OWLSameIndividual)inference.Axiom).IndividualExpressions.Select(individual => individual.GetIRI().ToString().Replace(vrd.ToString(), "vrd:"))));
```

</div>

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

The reasoner concludes that the two names are the same winery, and from there every fact about one is a fact about the other ([Fact rules]({{ '/reasoning/fact-rules/' | relative_url }})). A functional property does the same from the other side: two values of `vr:producedBy` for one wine make two names of one winery.
