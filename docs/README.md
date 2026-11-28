# The OWLSharp manual

This folder is the source of https://mdesalvo.github.io/OWLSharp/, built by GitHub Pages with Jekyll from the `main` branch. It is not part of the NuGet package. It shares its look, its building blocks and its running example with the [RDFSharp manual](https://mdesalvo.github.io/RDFSharp/): this one starts where that one ends.

## Preview it locally

```shell
cd docs
docker run --rm -p 4000:4000 -v "$PWD":/srv/jekyll jekyll/jekyll:pages jekyll serve --host 0.0.0.0 --baseurl /OWLSharp --force_polling
```

Then open http://localhost:4000/OWLSharp/. The `jekyll/jekyll:pages` image carries the same Jekyll, kramdown and Rouge that GitHub Pages uses.

## How it is organized

| Path | Holds |
|---|---|
| `_data/nav.yml` | the index: groups, pages and their order. The order is the reading order, which the pager at the foot of every page follows |
| `_layouts/default.html` | the only layout: top bar, index, page header, pager, the link to edit the page |
| `_includes/` | the logo and the cards that list the pages of a group on its overview |
| `assets/css/site.css`, `assets/js/site.js` | the whole look and behaviour, no framework |
| `assets/data/` | the Vallerosa files every example works on |
| `index.md`, `getting-started.md`, `vallerosa.md`, `going-further.md` | the pages outside the four groups |
| `modeling/`, `reasoning/`, `profiling/`, `validation/` | one Markdown file per page of each group |

A new page needs a file and a line in `nav.yml`. Its front matter:

```yaml
---
title: Object property axioms                   # as in nav.yml
permalink: /modeling/object-properties/         # as in nav.yml
tagline: What a link between individuals implies   # one line, shown in italics under the title
lede: Say once that a part of a part is a part: every wine then knows its region.   # one or two sentences
---
```

The eyebrow above the title (the name of the group, such as "Modeling") is computed from `nav.yml`.

## The running example

Every example works on the Vallerosa case, described in `vallerosa.md`. The ontology is `assets/data/vallerosa.owx` (OWL2/XML), with the same content in `vallerosa.omn` (Manchester): both are written by the library from the RDF Schema vocabulary and the catalog of the RDFSharp manual, which `assets/data/` also carries for the chapter that lifts them. Use the IRIs of that page, its prefixes (`vr:` for the vocabulary, `vrd:` for the data), its wines and its numbers. Do not invent new wines, producers or prices; when a chapter needs something new (a defined class, a sommelier, a bad import), give it a `vr:` or `vrd:` IRI and keep it inside that chapter.

**Continuity.** Every chapter picks up an example of the RDFSharp manual and carries it to what only OWL2 can say: the property path that finds the wines of Tuscany becomes a property chain, the SHACL report on `vrd:nero-2024` meets the validator. The story block names the RDFSharp chapter it continues, with a link.

## Building blocks

**The story.** Each chapter opens with one or two sentences on what happens at Vallerosa, in a story block:

```markdown
> The [property paths](https://mdesalvo.github.io/RDFSharp/query/property-paths/) of the RDFSharp manual walk from a wine to Tuscany at every query. The cooperative wants the ontology to know it once.
{: .story}
```

**Pairs.** When an example has a standard form and a C# form, put both in a pair, the standard form first. Readers switch between them with tabs; when the lines are short enough, they appear side by side. The reader's choice of language is remembered across pages. The standard form is Manchester syntax; OWL2/XML and Functional syntax appear where a chapter is about them.

````markdown
<div class="pair" markdown="1">

```manchester
ObjectProperty: vr:partOf
    Characteristics: Transitive
```

```csharp
ontology.ObjectPropertyAxioms.Add(new OWLTransitiveObjectProperty(partOf));
```

</div>
````

Tab labels come from the fence language (`manchester`, `functional`, `xml` as OWL2/XML, `turtle`, `sparql`, `csharp`, `json`, `shell`). For anything else, or to rename them, add `data-labels="RDF/XML,C#"` to the div.

**Results.** What a program prints goes in a result block right after the code that produced it. It must be copied from a real run, never written by hand:

````markdown
<div class="result" markdown="1">

```
https://vallerosa.example/id/rosso-2021
```

</div>
````

Add `data-label="vallerosa.omn"` to name it after a file instead of "Result".

**Checking the pages.** The C# blocks of a page run in order as one program, so a later block uses what an earlier one declared; every result block is compared with what its block prints. Two HTML comments, invisible on the site, steer that check: `<!-- pagecheck: skip -->` on the line before a fence leaves that block out (a signature, a fragment) while a comment opened by `<!-- pagecheck: prelude` holds code run before the first block, for a page that shows only part of what it needs.

**Diagrams.** A fenced `mermaid` block becomes a diagram, in the notation of the RDFSharp manual: individuals and classes are stadiums with the class `res`, literals are boxes with the class `lit`, anonymous individuals are stadiums with the class `blank`, properties are labelled arrows. Write quotes inside labels as `#quot;`. A diagram earns its place when it shows a shape the text cannot, such as the chain that links a wine to Tuscany. One per page at most, usually none.

**Notes.** A blockquote followed by `{: .tip}` or `{: .warn}` becomes a tip or a warning. Keep them rare.

## Writing rules

- English, second person, present tense. Plain words first, the term of art right after: "a defined class, one whose members the reasoner finds by itself".
- The API first: what a member does, its exact semantics and its edge cases, in tables where they help. Vallerosa comes in small doses, in the story and in the examples.
- Never a comma before "and", never a sentence that opens with "And" or "Or".
- C# follows the library: explicit types (`OWLClass redWine = new OWLClass(...)`), descriptive names, entities declared as locals before the axioms that use them, the `Async` methods. No `var`.
- Every claim about OWL2 is checked against the W3C recommendation and against the source of the library.
- Every example must compile and run against the current source of the library, not against older documentation. Every output shown must come from running it.

**Mermaid gotchas.** Never use `graph`, `end` or `subgraph` as a node id. Inside a label, write quotes as `#quot;` and the colon of an IRI scheme as `#58;` (`https#58;//...`): Mermaid otherwise takes an IRI for a Markdown link and refuses it.
