---
title: Getting started
permalink: /getting-started/
tagline: From an empty project to a first deduction
lede: Three steps to a console program that loads an OWL2 ontology, defines a class and lets the reasoner find its members.
---

## 1. Install

OWLSharp is one NuGet package for `netstandard2.0`, `net8.0` and `net10.0`. It brings [RDFSharp](https://mdesalvo.github.io/RDFSharp/) along, on which it is built.

```shell
dotnet new console -n Vallerosa
cd Vallerosa
dotnet add package OWLSharp
```

## 2. Get the ontology

Download [vallerosa.owx]({{ '/assets/data/vallerosa.owx' | relative_url }}) into the project folder. It is the ontology that every example of this manual uses, written in OWL2/XML: [Meet Vallerosa]({{ '/vallerosa/' | relative_url }}) tells what is inside.

## 3. Let the reasoner answer

The RDFSharp manual opens with a SPARQL query for the red wines under 20 euros. Here the same question becomes a class: an affordable red is a red wine with a price under 20. Nobody says which wines belong to it; the reasoner works it out.

Replace `Program.cs` with this program:

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");

OWLClass redWine = new OWLClass(new RDFResource(vr + "RedWine"));
OWLClass affordableRed = new OWLClass(new RDFResource(vr + "AffordableRed"));
OWLDataProperty price = new OWLDataProperty(new RDFResource(vr + "price"));

OWLDatatypeRestriction underTwenty = new OWLDatatypeRestriction(
    new OWLDatatype(RDFVocabulary.XSD.DECIMAL),
    new List<OWLFacetRestriction> {
        new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("20", RDFModelEnums.RDFDatatypes.XSD_DECIMAL)), RDFVocabulary.XSD.MAX_EXCLUSIVE) });

vallerosa.DeclarationAxioms.Add(new OWLDeclaration(affordableRed));
vallerosa.ClassAxioms.Add(new OWLEquivalentClasses(new List<OWLClassExpression> {
    affordableRed,
    new OWLObjectIntersectionOf(new List<OWLClassExpression> { redWine, new OWLDataSomeValuesFrom(price, underTwenty) }) }));

OWLReasonerReport report = await OWLReasoner.Default.ApplyToOntologyAsync(vallerosa);
foreach (OWLInference inference in report.Inferences)
    if (inference.Axiom is OWLClassAssertion assertion && assertion.ClassExpression.GetIRI().Equals(affordableRed.GetIRI()))
        Console.WriteLine($"{assertion.IndividualExpression.GetIRI()} | {inference.RuleName}");
```

Run it with `dotnet run`:

<div class="result" markdown="1">

```
https://vallerosa.example/id/rosso-2021 | FactClassAssertionEntailment
```

</div>

The Vallerosa Red costs 14.50 euros and is a red wine, so it is an affordable red; the Vallerosa Reserve, at 32.00 euros, is not. The reasoner tells which of its rules made the deduction. The next pages explain each piece: the [ontology]({{ '/modeling/ontologies/' | relative_url }}), the [class expressions]({{ '/modeling/class-expressions/' | relative_url }}) and the [reasoner]({{ '/reasoning/reasoner/' | relative_url }}).
