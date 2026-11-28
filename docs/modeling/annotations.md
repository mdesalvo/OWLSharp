---
title: Annotations
permalink: /modeling/annotations/
tagline: What the ontology says to people
lede: Annotations carry labels, comments, sources and notes. They can sit on an entity, on an axiom, on the ontology itself and on another annotation, and the reasoner ignores all of them: they say something about the ontology, not about wines.
---

> In the [Reification](https://mdesalvo.github.io/RDFSharp/model/reification/) chapter of the RDFSharp manual, the sommelier Giulia Neri says that the Vallerosa Reserve goes well with wild boar stew. The cooperative wanted to keep the advice, and who gave it, without printing it in the catalog as a fact. In OWL2 that is what an annotation is for.
{: .story}

## Where annotations go

| On | Built with | Lives in |
|---|---|---|
| an entity, or any IRI | `OWLAnnotationAssertion(property, subject, value)` | `AnnotationAxioms` |
| an axiom | `OWLAnnotation(property, value)` added to the axiom's `Annotations` | the axiom |
| the ontology | `OWLAnnotation(property, value)` passed to `Annotate` | `Annotations` |
| another annotation | `OWLAnnotation(property, value)` set as its `Annotation` | the annotation |

The value of an annotation is a literal, an IRI or an anonymous individual. The property is an `OWLAnnotationProperty`: the nine built into OWL2 need no declaration (`rdfs:label`, `rdfs:comment`, `rdfs:seeAlso`, `rdfs:isDefinedBy`, `owl:deprecated`, `owl:versionInfo`, `owl:priorVersion`, `owl:backwardCompatibleWith`, `owl:incompatibleWith`), any other is declared like an entity.

## Labels

Every entity of the Vallerosa ontology has labels, most of them in English and Italian:

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");
OWLAnnotationProperty label = new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL);
OWLNamedIndividual riserva = new OWLNamedIndividual(new RDFResource(vrd + "riserva-2019"));

foreach (OWLAnnotationAssertion assertion in vallerosa.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                                      .Where(assertion => assertion.SubjectIRI == riserva.GetIRI().ToString()
                                                                          && assertion.AnnotationProperty.GetIRI().Equals(label.GetIRI())))
    Console.WriteLine(assertion.ValueLiteral.GetLiteral());

Console.WriteLine(vallerosa.Annotations.First(annotation => annotation.AnnotationProperty.GetIRI().Equals(label.GetIRI())).ValueLiteral.GetLiteral());
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

The last line is the label of the ontology itself, an annotation of its header. RDFSharp writes language tags in upper case; tags compare without regard to case.

## Advice without assertion

The pairing of Giulia Neri becomes an annotation assertion with a property of the cooperative, `vr:suggestedPairing`. Who said it is an annotation of that assertion:

<div class="pair" markdown="1">

```manchester
AnnotationProperty: vr:suggestedPairing
AnnotationProperty: vr:suggestedBy

Individual: vrd:riserva-2019
    Annotations:
        Annotations: vr:suggestedBy vrd:giulia-neri
        vr:suggestedPairing vrd:wild-boar-stew
```

```csharp
OWLAnnotationProperty suggestedPairing = new OWLAnnotationProperty(new RDFResource(vr + "suggestedPairing"));
OWLAnnotationProperty suggestedBy = new OWLAnnotationProperty(new RDFResource(vr + "suggestedBy"));
RDFResource wildBoarStew = new RDFResource(vrd + "wild-boar-stew");
RDFResource giuliaNeri = new RDFResource(vrd + "giulia-neri");

vallerosa.DeclareEntity(suggestedPairing);
vallerosa.DeclareEntity(suggestedBy);

OWLAnnotationAssertion advice = new OWLAnnotationAssertion(suggestedPairing, riserva.GetIRI(), wildBoarStew);
advice.Annotations.Add(new OWLAnnotation(suggestedBy, giuliaNeri));
vallerosa.AnnotationAxioms.Add(advice);
```

</div>

The advice is in the ontology, with its author, and it is not a fact about the wine. The wines that pair with wild boar stew are still none:

```csharp
OWLObjectProperty pairsWith = new OWLObjectProperty(new RDFResource(vr + "pairsWith"));
OWLNamedIndividual stew = new OWLNamedIndividual(wildBoarStew);

await OWLSharp.Reasoner.OWLReasoner.Default.ApplyToOntologyAsync(vallerosa);
Console.WriteLine(vallerosa.GetIndividualsOf(new OWLObjectHasValue(pairsWith, stew)).Count);
Console.WriteLine(vallerosa.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>().Count(assertion => assertion.AnnotationProperty.GetIRI().Equals(suggestedPairing.GetIRI())));
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

If the cooperative adopts the advice, it states the fact with `vr:pairsWith` and keeps the source as an annotation of that assertion: annotations on axioms say where a fact came from, while the fact counts.

## Annotations on axioms

Any axiom can carry annotations, the facts of the catalog as much as the rules of the vocabulary. Say the cooperative wants to remember when it decided that the colours never meet:

```csharp
OWLDisjointClasses colours = vallerosa.GetClassAxiomsOfType<OWLDisjointClasses>()
                                      .First(axiom => axiom.ClassExpressions.Any(expression => expression.GetIRI().Equals(new RDFResource(vr + "RedWine"))));
colours.Annotations.Add(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
    new OWLLiteral(new RDFPlainLiteral("Agreed by the three wineries in 2026", "en"))));

MemoryStream stream = new MemoryStream();
await new OWLOntology(new Uri("https://vallerosa.example/excerpt")) { ClassAxioms = { colours } }.ToStreamAsync(OWLEnums.OWLFormats.OWL2Functional, stream);
Console.WriteLine(System.Text.Encoding.UTF8.GetString(stream.ToArray()).Split('\n').First(line => line.Contains("DisjointClasses")).Trim());
```

<div class="result" markdown="1">

```
TO BE FILLED BY PAGECHECK
```

</div>

Annotations take part in the identity of an axiom: two `DisjointClasses` with the same classes and different annotations are two axioms.

## Annotation property axioms

Annotation properties have axioms of their own, which organize the annotations without giving them meaning:

| Axiom | Says |
|---|---|
| `OWLSubAnnotationPropertyOf(p, q)` | `p` is a more specific kind of `q`, as a suggested pairing is a kind of comment |
| `OWLAnnotationPropertyDomain(p, iri)` | the subjects of `p` are meant to be of the kind `iri` |
| `OWLAnnotationPropertyRange(p, iri)` | the values of `p` are meant to be of the kind `iri` |

The ontology answers with `GetSubAnnotationPropertiesOf`, `GetSuperAnnotationPropertiesOf` and their `Check...` forms. The reasoner does not use these axioms: in OWL 2 annotations have no meaning for reasoning, which is exactly why an advice can live in them.

## Deprecated terms

The built-in property `owl:deprecated` with the value `true` marks an entity that should no longer be used. The vocabulary keeps it, so that old data still reads, and the [validator]({{ '/validation/' | relative_url }}) warns when the ontology keeps using it (`TermsDeprecationAnalysis`).
