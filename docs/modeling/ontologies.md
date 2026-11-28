---
title: Ontologies and formats
permalink: /modeling/ontologies/
tagline: The container of everything, in three syntaxes
lede: An OWLOntology holds the header of an ontology and its axioms, sorted by kind. It writes itself in OWL2/XML, Manchester and Functional syntax, reads itself back from any of them and turns into an RDF graph when RDFSharp needs one.
---

> In the [RDF formats](https://mdesalvo.github.io/RDFSharp/model/formats/) chapter of the RDFSharp manual, Cantina dei Tre Ulivi sends N-Triples and a partner shop asks for RDF/XML. The ontology has the same fate: the cooperative keeps it in OWL2/XML, the winemakers who edit it read Manchester and the university that studies the valley wants Functional syntax.
{: .story}

## The ontology object

`OWLOntology` is one class in the `OWLSharp.Ontology` namespace. It has a header (an IRI, a version IRI, prefixes, imports, annotations) and one list for each kind of axiom:

| Member | Holds |
|---|---|
| `IRI`, `VersionIRI` | the name of the ontology and of its version, as strings |
| `Prefixes` | the short names that the Manchester and Functional writers use for long IRIs |
| `Imports` | the other ontologies this one builds on ([Imports]({{ '/modeling/imports/' | relative_url }})) |
| `Annotations` | labels, comments and other annotations on the ontology itself |
| `DeclarationAxioms` | which IRIs are classes, properties, individuals or datatypes ([Entities and declarations]({{ '/modeling/entities/' | relative_url }})) |
| `ClassAxioms` | subclasses, equivalent and disjoint classes ([Class axioms]({{ '/modeling/class-axioms/' | relative_url }})) |
| `ObjectPropertyAxioms`, `DataPropertyAxioms` | what the properties are like ([Object property axioms]({{ '/modeling/object-properties/' | relative_url }}), [Data properties and datatypes]({{ '/modeling/data-properties/' | relative_url }})) |
| `DatatypeDefinitionAxioms`, `KeyAxioms` | new datatypes and keys ([Data properties and datatypes]({{ '/modeling/data-properties/' | relative_url }})) |
| `AssertionAxioms` | the facts about individuals ([Individuals and assertions]({{ '/modeling/assertions/' | relative_url }})) |
| `AnnotationAxioms` | the annotations on entities ([Annotations]({{ '/modeling/annotations/' | relative_url }})) |
| `Rules` | the SWRL rules ([SWRL rules]({{ '/reasoning/swrl-rules/' | relative_url }})) |

Three constructors build one: `new OWLOntology()` gives an empty ontology with no IRI, `new OWLOntology(iri, versionIri)` gives it a name and `new OWLOntology(other)` copies another one, header and axioms, into new lists. Every new ontology starts with five prefixes, `owl`, `rdfs`, `rdf`, `xsd` and `xml`; `Prefix` adds more and `Annotate` adds an annotation to the header.

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");

OWLOntology ontology = new OWLOntology(new Uri("https://vallerosa.example/ontology"), new Uri("https://vallerosa.example/ontology/2026"));
ontology.Prefix(new OWLPrefix(vr));
ontology.Prefix(new OWLPrefix(vrd));
ontology.Annotate(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
    new OWLLiteral(new RDFPlainLiteral("The Vallerosa ontology", "en"))));

Console.WriteLine(ontology.IRI);
Console.WriteLine(ontology.VersionIRI);
Console.WriteLine(string.Join(" ", ontology.Prefixes.Select(prefix => prefix.Name)));
```

<div class="result" markdown="1">

```
https://vallerosa.example/ontology
https://vallerosa.example/ontology/2026
owl rdfs rdf xsd xml vr vrd
```

</div>

The axioms go in their lists with `Add`, like any list. Here is the Vermentino of the Plain, the wine that the [RDFSharp manual](https://mdesalvo.github.io/RDFSharp/model/triples/) typed in triple by triple: a white wine with an alcohol of 12.5. What each line means is the subject of the next chapters; for now they are what the syntaxes will write.

```csharp
OWLClass whiteWine = new OWLClass(new RDFResource(vr + "WhiteWine"));
OWLDataProperty alcohol = new OWLDataProperty(new RDFResource(vr + "alcohol"));
OWLNamedIndividual vermentino = new OWLNamedIndividual(new RDFResource(vrd + "vermentino-2023"));

ontology.DeclarationAxioms.Add(new OWLDeclaration(whiteWine));
ontology.DeclarationAxioms.Add(new OWLDeclaration(alcohol));
ontology.DeclarationAxioms.Add(new OWLDeclaration(vermentino));
ontology.AssertionAxioms.Add(new OWLClassAssertion(whiteWine, vermentino));
ontology.AssertionAxioms.Add(new OWLDataPropertyAssertion(alcohol, vermentino,
    new OWLLiteral(new RDFTypedLiteral("12.5", RDFModelEnums.RDFDatatypes.XSD_DECIMAL))));
```

## Three syntaxes

OWL2 has one structure and several ways to write it. OWLSharp reads and writes three, named by `OWLEnums.OWLFormats`:

| Value | Syntax | Usual extension | Good for |
|---|---|---|---|
| `OWL2XML` | [OWL2/XML](https://www.w3.org/TR/owl2-xml-serialization/) | `.owx` | exchanging ontologies with XML tools |
| `OWL2Manchester` | [Manchester](https://www.w3.org/TR/owl2-manchester-syntax/) | `.omn` | reading and writing by hand: one frame for each entity |
| `OWL2Functional` | [Functional](https://www.w3.org/TR/owl2-syntax/#Functional-Style_Syntax) | `.ofn` | one line for each axiom, close to the specification |

`ToStreamAsync(format, stream)` writes the ontology on a stream and `ToFileAsync(format, path)` on a file. The stream below is a memory stream turned into text:

```csharp
async Task<string> WriteAsync(OWLEnums.OWLFormats format)
{
    MemoryStream stream = new MemoryStream();
    await ontology.ToStreamAsync(format, stream);
    return System.Text.Encoding.UTF8.GetString(stream.ToArray());
}

Console.WriteLine(await WriteAsync(OWLEnums.OWLFormats.OWL2Manchester));
```

<div class="result" data-label="Manchester" markdown="1">

```
Prefix: owl: <http://www.w3.org/2002/07/owl#>
Prefix: rdfs: <http://www.w3.org/2000/01/rdf-schema#>
Prefix: rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
Prefix: xsd: <http://www.w3.org/2001/XMLSchema#>
Prefix: xml: <http://www.w3.org/XML/1998/namespace>
Prefix: vr: <https://vallerosa.example/vocab#>
Prefix: vrd: <https://vallerosa.example/id/>

Ontology: <https://vallerosa.example/ontology> <https://vallerosa.example/ontology/2026>
Annotations: rdfs:label "The Vallerosa ontology"@EN

DataProperty: vr:alcohol

Class: vr:WhiteWine

Individual: vrd:vermentino-2023
    Types: vr:WhiteWine
    Facts: vr:alcohol "12.5"^^xsd:decimal
```

</div>

Manchester groups everything said about an entity under a frame that starts with its kind (`Class:`, `DataProperty:`, `Individual:`) and writes its facts and types inside it. Functional syntax writes one axiom per line instead:

```csharp
Console.WriteLine(await WriteAsync(OWLEnums.OWLFormats.OWL2Functional));
```

<div class="result" data-label="Functional" markdown="1">

```
Prefix(owl:=<http://www.w3.org/2002/07/owl#>)
Prefix(rdfs:=<http://www.w3.org/2000/01/rdf-schema#>)
Prefix(rdf:=<http://www.w3.org/1999/02/22-rdf-syntax-ns#>)
Prefix(xsd:=<http://www.w3.org/2001/XMLSchema#>)
Prefix(xml:=<http://www.w3.org/XML/1998/namespace>)
Prefix(vr:=<https://vallerosa.example/vocab#>)
Prefix(vrd:=<https://vallerosa.example/id/>)

Ontology( <https://vallerosa.example/ontology> <https://vallerosa.example/ontology/2026>
    Annotation( rdfs:label "The Vallerosa ontology"@EN )
    Declaration( Class( vr:WhiteWine ) )
    Declaration( DataProperty( vr:alcohol ) )
    Declaration( NamedIndividual( vrd:vermentino-2023 ) )
    ClassAssertion( vr:WhiteWine vrd:vermentino-2023 )
    DataPropertyAssertion( vr:alcohol vrd:vermentino-2023 "12.5"^^xsd:decimal )
)
```

</div>

OWL2/XML is the same structure as XML elements:

```csharp
Console.WriteLine(await WriteAsync(OWLEnums.OWLFormats.OWL2XML));
```

<div class="result" data-label="OWL2/XML" markdown="1">

```
<?xml version="1.0" encoding="utf-8"?>
<Ontology xmlns:owl="http://www.w3.org/2002/07/owl#" xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:xsd="http://www.w3.org/2001/XMLSchema#" xmlns:vr="https://vallerosa.example/vocab#" xmlns:vrd="https://vallerosa.example/id/" ontologyIRI="https://vallerosa.example/ontology" versionIRI="https://vallerosa.example/ontology/2026">
  <Prefix name="owl" IRI="http://www.w3.org/2002/07/owl#" />
  <Prefix name="rdfs" IRI="http://www.w3.org/2000/01/rdf-schema#" />
  <Prefix name="rdf" IRI="http://www.w3.org/1999/02/22-rdf-syntax-ns#" />
  <Prefix name="xsd" IRI="http://www.w3.org/2001/XMLSchema#" />
  <Prefix name="xml" IRI="http://www.w3.org/XML/1998/namespace" />
  <Prefix name="vr" IRI="https://vallerosa.example/vocab#" />
  <Prefix name="vrd" IRI="https://vallerosa.example/id/" />
  <Annotation>
    <AnnotationProperty IRI="http://www.w3.org/2000/01/rdf-schema#label" />
    <Literal xml:lang="EN">The Vallerosa ontology</Literal>
  </Annotation>
  <Declaration>
    <Class IRI="https://vallerosa.example/vocab#WhiteWine" />
  </Declaration>
  <Declaration>
    <DataProperty IRI="https://vallerosa.example/vocab#alcohol" />
  </Declaration>
  <Declaration>
    <NamedIndividual IRI="https://vallerosa.example/id/vermentino-2023" />
  </Declaration>
  <ClassAssertion>
    <Class IRI="https://vallerosa.example/vocab#WhiteWine" />
    <NamedIndividual IRI="https://vallerosa.example/id/vermentino-2023" />
  </ClassAssertion>
  <DataPropertyAssertion>
    <DataProperty IRI="https://vallerosa.example/vocab#alcohol" />
    <NamedIndividual IRI="https://vallerosa.example/id/vermentino-2023" />
    <Literal datatypeIRI="http://www.w3.org/2001/XMLSchema#decimal">12.5</Literal>
  </DataPropertyAssertion>
</Ontology>
```

</div>

Manchester and Functional syntax abbreviate every IRI they can with the prefixes of the ontology: `vr:WhiteWine` is written short because the ontology carries the `vr` prefix; without it the IRI would be written in full. OWL2/XML declares the prefixes too, but writes every IRI in full in its `IRI` attributes.

## Reading back

The reading side mirrors the writing one, as static methods that return a new ontology:

| Method | Reads |
|---|---|
| `FromFileAsync(format, path)` | a file in one of the three syntaxes |
| `FromStreamAsync(format, stream)` | a stream in one of the three syntaxes |
| `FromRDFGraphAsync(graph)` | an RDFSharp graph ([From RDF to OWL]({{ '/modeling/from-rdf/' | relative_url }})) |
| `FromUriAsync(uri, timeout)` | a document on the Web, in any syntax the server declares |

The Vallerosa ontology is shipped in two of them, OWL2/XML and Manchester. Read from either file, it is the same ontology:

```csharp
OWLOntology fromXml = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");
OWLOntology fromManchester = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2Manchester, "vallerosa.omn");

foreach (OWLOntology read in new[] { fromXml, fromManchester })
    Console.WriteLine($"{read.IRI}: {read.DeclarationAxioms.Count} declarations, {read.ClassAxioms.Count} class axioms, {read.AssertionAxioms.Count} assertions, {read.AnnotationAxioms.Count} annotation assertions");
```

<div class="result" markdown="1">

```
https://vallerosa.example/ontology: 51 declarations, 8 class axioms, 77 assertions, 97 annotation assertions
https://vallerosa.example/ontology: 51 declarations, 8 class axioms, 77 assertions, 97 annotation assertions
```

</div>

`FromUriAsync` asks the server for `application/owl+xml`, `text/owl-manchester`, `text/owl-functional`, `application/rdf+xml` or Turtle and reads the answer according to the content type the server declares. When the server declares none, it reads the answer as RDF/XML; when it declares one that is not in that list, the method returns `null`. The default timeout is 20 seconds.

<!-- pagecheck: skip -->
```csharp
OWLOntology remote = await OWLOntology.FromUriAsync(new Uri("https://www.w3.org/ns/shacl.ttl"));
```

## What the writers leave out

Two kinds of axioms carry a mark that the writers look at:

- an axiom that the reasoner deduced has `IsInference` set. `ToStreamAsync`, `ToFileAsync` and `ToRDFGraphAsync` write the inferences unless their last argument, `includeInferences`, is `false`. [Working with inferences]({{ '/reasoning/inferences/' | relative_url }}) shows when that matters.
- an axiom that came from an imported ontology has `IsImport` set. The writers never write it: the file keeps the `Imports` entry instead, so that whoever reads it loads the imported ontology on their own.

## When something goes wrong

The reading methods and `ToStreamAsync` report their failures as an `OWLException`, whose message tells what went wrong: a path that does not exist, a stream that is `null`, a document that does not parse. `ToFileAsync` opens the file before writing, so a path it cannot create fails with the `IOException` of .NET. Manchester syntax has a trap of its own: a frame such as `vr:price some xsd:decimal` does not say whether `vr:price` is an object property or a data property, so the reader refuses it unless the document declares it:

```csharp
string manchester = @"Prefix: vr: <https://vallerosa.example/vocab#>
Prefix: xsd: <http://www.w3.org/2001/XMLSchema#>
Ontology: <https://vallerosa.example/ontology>
Class: vr:Wine
    SubClassOf: vr:price some xsd:decimal";

try
{
    await OWLOntology.FromStreamAsync(OWLEnums.OWLFormats.OWL2Manchester, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(manchester)));
}
catch (OWLException exception)
{
    Console.WriteLine(exception.Message);
}
```

<div class="result" markdown="1">

```
Cannot read ontology from stream because: Cannot parse OWL2/Manchester document: restriction on <https://vallerosa.example/vocab#price> requires it to be declared as object or data property ('vr:price' (PrefixedName) at line 5, column 17)
```

</div>

Adding the line `DataProperty: vr:price` to the document is enough. The files that OWLSharp writes always declare their entities, so they read back without this problem.
