---
title: Modeling at a glance
permalink: /modeling/
eyebrow: Modeling
tagline: Say what the data means
lede: "The Ontology part of OWLSharp is where an ontology takes shape: entities and the axioms that relate them, class expressions that describe a class by what its members are, annotations and imports, written in the three syntaxes of OWL2."
---

## What you find here

Everything in this part lives in the `OWLSharp.Ontology` namespace. The IRIs come from RDFSharp, so its `RDFSharp.Model` namespace is always there too.

<!-- pagecheck: skip -->
```csharp
using OWLSharp.Ontology;
using RDFSharp.Model;
```

The chapters follow the growth of the ontology. First the container and its formats, then the lift of the RDF files of the RDFSharp manual into OWL2. Then the vocabulary of OWL2, from the plainest piece to the richest: entities, class expressions, the axioms about classes, object properties and data properties, the facts about individuals. Last, what sits around the axioms: annotations, imports of other ontologies and views that answer questions about one entity.

{% include layer-cards.html group="Modeling" %}

## Where the ontology comes from

The examples load [vallerosa.owx]({{ '/assets/data/vallerosa.owx' | relative_url }}) or build pieces of it by hand. [Meet Vallerosa]({{ '/vallerosa/' | relative_url }}) describes what is inside.
