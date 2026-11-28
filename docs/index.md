---
title: OWLSharp
permalink: /
lede: "OWLSharp is a .NET library for OWL2 ontologies: model them, let a reasoner deduce what they imply, check them against the OWL2 profiles and validate them for inconsistencies."
---

<div class="hero" markdown="0">
  <h1>OWL2 ontologies, <em>in plain C#</em></h1>
  <p>OWLSharp gives a .NET application the language of ontologies. Where RDF states facts, OWL2 says what they mean: that a part of a part is a part, that a red wine is never a white wine, that a wine from a Tuscan valley is a Tuscan wine. A reasoner then deduces what nobody wrote down. Every page of this manual shows it on the case of the <a href="https://mdesalvo.github.io/RDFSharp/">RDFSharp manual</a>, the catalog of a wine cooperative, picked up where that manual leaves it.</p>
</div>

<!-- pagecheck: prelude
using OWLSharp;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
-->

## A first taste

The RDFSharp manual opens with a SPARQL query for the red wines under 20 euros. In OWL2 the question becomes a class, defined once in the ontology: whoever asks for its members gets them from the reasoner, with no query to write.

<div class="pair" markdown="1">

```manchester
Class: vr:AffordableRed
    EquivalentTo: vr:RedWine and (vr:price some xsd:decimal[< "20"^^xsd:decimal])
```

```csharp
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
        Console.WriteLine(assertion.IndividualExpression.GetIRI());
```

</div>

<div class="result" markdown="1">

```
https://vallerosa.example/id/rosso-2021
```

</div>

## Four parts, one library

OWLSharp ships as a NuGet package targeting `netstandard2.0`, `net8.0` and `net10.0`, built on RDFSharp. Its four parts work on the same ontology object.

<div class="cards">
  <a class="card" href="{{ '/modeling/' | relative_url }}"><span class="kind">OWLSharp.Ontology</span><h3>Model</h3><p>Classes, properties, individuals and the axioms that relate them. Class expressions, datatypes, annotations, imports. OWL2/XML, Manchester and Functional syntax.</p></a>
  <a class="card" href="{{ '/reasoning/' | relative_url }}"><span class="kind">OWLSharp.Reasoner</span><h3>Deduce</h3><p>Forty-two RDFS and OWL2 entailment rules run to a fixpoint, together with the SWRL rules of the ontology and their built-ins.</p></a>
  <a class="card" href="{{ '/profiling/' | relative_url }}"><span class="kind">OWLSharp.Profiler</span><h3>Profile</h3><p>Check whether an ontology stays inside OWL2 EL, QL or RL, the profiles that trade expressivity for fast reasoning.</p></a>
  <a class="card" href="{{ '/validation/' | relative_url }}"><span class="kind">OWLSharp.Validator</span><h3>Validate</h3><p>Thirty analysis rules find the contradictions and the modeling pitfalls of an ontology, from disjoint classes to deprecated terms.</p></a>
</div>

## The family

<div class="cards">
  <a class="card" href="https://mdesalvo.github.io/RDFSharp/"><span class="kind">RDF</span><h3>RDFSharp</h3><p>RDF graphs and stores, SPARQL queries and updates, SHACL validation. OWLSharp is built on it; its manual starts the Vallerosa case.</p></a>
  <a class="card" href="https://github.com/mdesalvo/OWLSharp.Extensions"><span class="kind">LinkedData</span><h3>OWLSharp.Extensions</h3><p>Helpers for the LinkedData ontologies: SKOS concept schemes, spatial features with GeoSPARQL and temporal ones with OWL-TIME.</p></a>
</div>
