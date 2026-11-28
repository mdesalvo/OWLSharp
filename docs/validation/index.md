---
title: Validation at a glance
permalink: /validation/
eyebrow: Validation
tagline: Find what contradicts itself
lede: "The Validator part of OWLSharp runs 30 analysis rules on an ontology and reports the contradictions that make it inconsistent and the pitfalls that make it wrong, each with its severity and a suggestion."
---

## What you find here

The validator lives in the `OWLSharp.Validator` namespace.

<!-- pagecheck: skip -->
```csharp
using OWLSharp.Ontology;
using OWLSharp.Validator;
```

The first chapter runs the validator and reads its report. The second lists the thirty rules. The last one brings back the sloppy import of the RDFSharp manual, `vrd:nero-2024`, to show what OWL2 finds in it, what it does not find and why.

{% include layer-cards.html group="Validation" %}

## Where the ontology comes from

The examples validate [vallerosa.owx]({{ '/assets/data/vallerosa.owx' | relative_url }}), alone and together with new facts that contradict it.
