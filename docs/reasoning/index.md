---
title: Reasoning at a glance
permalink: /reasoning/
eyebrow: Reasoning
tagline: Deduce what nobody wrote down
lede: "The Reasoner part of OWLSharp applies the meaning of the axioms to the ontology: 42 RDFS and OWL2 entailment rules and the SWRL rules of the ontology produce the facts that follow from what is written."
---

## What you find here

The reasoner lives in the `OWLSharp.Reasoner` namespace; the SWRL rules it runs belong to the ontology, in `OWLSharp.Ontology`.

<!-- pagecheck: skip -->
```csharp
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
```

The first chapter runs the reasoner and reads its report: which rules ran, how many rounds they took, what they found. The next two list the rules, the Schema ones that close the hierarchy of classes and properties and the Fact ones that move facts from individual to individual. Then SWRL, for the deductions OWL2 cannot express, with the built-ins that compare and compute values. The last chapter is about what to do with the inferences once they are there.

{% include layer-cards.html group="Reasoning" %}

## Where the ontology comes from

Every example reasons on [vallerosa.owx]({{ '/assets/data/vallerosa.owx' | relative_url }}). Its two defined classes, `vr:TuscanWine` and `vr:OrganicWine`, have no member written in the file: finding them is the reasoner's job.
