---
title: Profiling at a glance
permalink: /profiling/
eyebrow: Profiling
tagline: Trade expressivity for speed, knowingly
lede: "OWL2 defines three profiles, EL, QL and RL, that give up part of the language so that reasoning stays fast. The Profiler part of OWLSharp tells whether an ontology stays inside them and, when it does not, which axiom takes it out."
---

## What you find here

The profiler lives in the `OWLSharp.Profiler` namespace.

<!-- pagecheck: skip -->
```csharp
using OWLSharp.Ontology;
using OWLSharp.Profiler;
```

The first chapter explains the three profiles: what each one leaves out and what it is good for. The second runs the profiler on the Vallerosa ontology and reads its violations.

{% include layer-cards.html group="Profiling" %}

## Where the ontology comes from

The examples profile [vallerosa.owx]({{ '/assets/data/vallerosa.owx' | relative_url }}), which stays inside OWL2 RL but for one axiom: the chapters show which one and why.
