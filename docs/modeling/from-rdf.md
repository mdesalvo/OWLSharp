---
title: From RDF to OWL
permalink: /modeling/from-rdf/
tagline: Lift a vocabulary into an ontology, and know what stays behind
lede: FromRDFGraphAsync reads an RDF graph as an OWL2 ontology, following the mapping that the W3C defines between the two. An RDF Schema vocabulary goes through it once three things are in place; the rest of this chapter is about those three things.
---

> The last page of the RDFSharp manual promises that this one "starts from the same Vallerosa files and grows the vocabulary into an ontology". The cooperative takes its vocabulary and its catalog as they are, in Turtle, and asks OWLSharp to read them as an ontology.
{: .story}

## Read a graph as an ontology

`OWLOntology.FromRDFGraphAsync(graph)` takes an `RDFGraph` of RDFSharp and returns an `OWLOntology`. It reads the graph as the [OWL 2 Mapping to RDF Graphs](https://www.w3.org/TR/owl2-mapping-to-rdf/) prescribes: `rdf:type owl:Class` becomes a class declaration, `rdfs:subClassOf` a `SubClassOf` axiom, `rdf:type` on an individual a `ClassAssertion` and so on. The way back is `ToRDFGraphAsync`.

The two files of the RDFSharp manual go into one graph, the way that manual merges them:

```csharp
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

RDFGraph vocabulary = await RDFGraph.FromFileAsync(RDFModelEnums.RDFFormats.Turtle, "vallerosa-vocab.ttl");
RDFGraph catalog = await RDFGraph.FromFileAsync(RDFModelEnums.RDFFormats.Turtle, "vallerosa-catalog.ttl");
RDFGraph graph = await vocabulary.UnionWithAsync(catalog);

try
{
    await OWLOntology.FromRDFGraphAsync(graph);
}
catch (OWLException exception)
{
    Console.WriteLine(exception.Message);
}
```

<div class="result" markdown="1">

```
Cannot find an owl:Ontology definition in the given graph. Please ensure that it correctly encodes an OWL ontology.
```

</div>

## 1. The header

An RDF graph is just triples; an ontology is a document with a name. The mapping looks for the node typed `owl:Ontology` and reads the ontology from there, so a graph without one is refused. One triple gives it a name:

```csharp
RDFResource ontologyIRI = new RDFResource("https://vallerosa.example/ontology");
graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));

OWLOntology firstTry = await OWLOntology.FromRDFGraphAsync(graph);
Console.WriteLine($"{firstTry.DeclarationAxioms.Count} declarations, {firstTry.ClassAxioms.Count} class axioms");
Console.WriteLine($"{firstTry.ObjectPropertyAxioms.Count} object property axioms, {firstTry.DataPropertyAxioms.Count} data property axioms");
Console.WriteLine($"{firstTry.AssertionAxioms.OfType<OWLClassAssertion>().Count()} class assertions, {firstTry.AssertionAxioms.OfType<OWLObjectPropertyAssertion>().Count()} object property assertions, {firstTry.AssertionAxioms.OfType<OWLDataPropertyAssertion>().Count()} data property assertions");
Console.WriteLine($"{firstTry.AnnotationAxioms.Count} annotation assertions");
```

<div class="result" markdown="1">

```
35 declarations, 4 class axioms
0 object property axioms, 0 data property axioms
21 class assertions, 0 object property assertions, 0 data property assertions
94 annotation assertions
```

</div>

The classes are there: `rdfs:Class` counts as a class declaration and `rdfs:subClassOf` gives the four subclass axioms of the colours. The individuals are there too, declared because they are members of a class, with their labels. The properties are not.

## 2. What a property links

RDF Schema types every property with `rdf:Property` and says nothing more. OWL2 needs to know whether a property links an individual to another individual (an **object property**, `owl:ObjectProperty`) or to a value (a **data property**, `owl:DatatypeProperty`): the mapping has no rule for a bare `rdf:Property`, so the importer leaves those properties out, together with their domains, ranges and every assertion that uses them, without a warning.

The vocabulary itself knows the answer: a range that is a class means an object property, a range that is an XML Schema type means a data property. The cooperative types the properties it wants in the ontology:

```csharp
RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");

foreach (string objectProperty in new[] { "memberOf", "locatedIn", "partOf", "producedBy", "produces", "grape", "pairsWith" })
    graph.AddTriple(new RDFTriple(new RDFResource(vr + objectProperty), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
foreach (string dataProperty in new[] { "vintage", "alcohol", "price", "organic" })
    graph.AddTriple(new RDFTriple(new RDFResource(vr + dataProperty), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));

OWLOntology ontology = await OWLOntology.FromRDFGraphAsync(graph);
Console.WriteLine($"{ontology.DeclarationAxioms.Count} declarations, {ontology.ClassAxioms.Count} class axioms");
Console.WriteLine($"{ontology.ObjectPropertyAxioms.Count} object property axioms, {ontology.DataPropertyAxioms.Count} data property axioms");
Console.WriteLine($"{ontology.AssertionAxioms.OfType<OWLClassAssertion>().Count()} class assertions, {ontology.AssertionAxioms.OfType<OWLObjectPropertyAssertion>().Count()} object property assertions, {ontology.AssertionAxioms.OfType<OWLDataPropertyAssertion>().Count()} data property assertions");
Console.WriteLine($"{ontology.AssertionAxioms.OfType<OWLSameIndividual>().Count()} same individuals");
```

<div class="result" markdown="1">

```
46 declarations, 4 class axioms
14 object property axioms, 7 data property axioms
21 class assertions, 26 object property assertions, 24 data property assertions
5 same individuals
```

</div>

Seven object properties, four data properties, their domains and ranges, all the facts of the catalog and the five `owl:sameAs` links of the grapes to DBpedia.

The other properties of the vocabulary stay untyped on purpose. `vr:blend` holds an `rdf:List`, `vr:awards` an `rdf:Bag` and `vr:claims` reified statements: RDF structures that OWL2 does not model, so they stay in the RDF graph. `vr:score`, `vr:soldTo`, `vr:soldWine`, `vr:bottles` and `vr:date` belong to the scores of the guides and to the sales, which live in the stores of the RDFSharp manual.

## 3. Datatypes that OWL2 knows

Every data property of the vocabulary has a range. Three of them come through, one does not:

```csharp
foreach (OWLDataPropertyRange range in ontology.DataPropertyAxioms.OfType<OWLDataPropertyRange>())
    Console.WriteLine($"{range.DataProperty.GetIRI()} -> {range.DataRangeExpression.GetIRI()}");
```

<div class="result" markdown="1">

```
https://vallerosa.example/vocab#alcohol -> http://www.w3.org/2001/XMLSchema#decimal
https://vallerosa.example/vocab#price -> http://www.w3.org/2001/XMLSchema#decimal
https://vallerosa.example/vocab#organic -> http://www.w3.org/2001/XMLSchema#boolean
```

</div>

OWL2 works with a fixed list of datatypes, the [OWL 2 datatype map](https://www.w3.org/TR/owl2-syntax/#Datatype_Maps): numbers, strings, booleans, binary data, IRIs and date-times, 34 in all with `rdfs:Literal`. Every ontology knows them without declaring them, so `xsd:decimal` and `xsd:boolean` are read as ranges. `xsd:gYear`, the bare year of `vr:vintage`, is not in the list: an OWL 2 DL ontology [cannot use it](https://www.w3.org/TR/owl2-syntax/#Datatypes) at all, since `xsd:` belongs to the reserved vocabulary. Its range is dropped and the vintages of the catalog keep a datatype that OWL2 reasoners refuse.

The Vallerosa ontology counts its vintages as integers instead. The assertions are rewritten and the range is stated again:

```csharp
OWLDataProperty vintage = new OWLDataProperty(new RDFResource(vr + "vintage"));

foreach (OWLDataPropertyAssertion vintageAssertion in ontology.AssertionAxioms.OfType<OWLDataPropertyAssertion>()
                                                               .Where(assertion => assertion.DataProperty.GetIRI().Equals(vintage.GetIRI())).ToList())
{
    string year = vintageAssertion.Literal.Value.TrimEnd('Z');
    ontology.AssertionAxioms.Remove(vintageAssertion);
    ontology.AssertionAxioms.Add(new OWLDataPropertyAssertion(vintage, (OWLNamedIndividual)vintageAssertion.IndividualExpression,
        new OWLLiteral(new RDFTypedLiteral(year, RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
}
ontology.DataPropertyAxioms.Add(new OWLDataPropertyRange(vintage, new OWLDatatype(RDFVocabulary.XSD.INTEGER)));

Console.WriteLine(ontology.AssertionAxioms.OfType<OWLDataPropertyAssertion>()
                                         .First(assertion => assertion.DataProperty.GetIRI().Equals(vintage.GetIRI())).Literal.GetLiteral());
```

<div class="result" markdown="1">

```
2021^^http://www.w3.org/2001/XMLSchema#integer
```

</div>

The `TrimEnd('Z')` is there because RDFSharp writes every date and time in UTC: the `"2021"^^xsd:gYear` of the catalog reaches the ontology as `2021Z`.

## How the pieces map

| In the RDF graph | In the ontology |
|---|---|
| `x rdf:type owl:Ontology` | the ontology header, required |
| `C rdf:type rdfs:Class` or `owl:Class` | `Declaration(Class(C))` |
| `p rdf:type owl:ObjectProperty`, `owl:DatatypeProperty`, `owl:AnnotationProperty` | the declaration of the property |
| `p rdf:type rdf:Property` and nothing more | nothing: the property and its uses are left out |
| `C1 rdfs:subClassOf C2` | `SubClassOf(C1 C2)` |
| `p rdfs:domain C`, `p rdfs:range R` | `ObjectPropertyDomain`, `DataPropertyRange` and the like, by the kind of `p` |
| a range in the OWL 2 datatype map (`xsd:decimal`, `xsd:boolean`, ...) | a data range, with no declaration needed |
| a range outside the map (`xsd:gYear`, `xsd:date`, ...) | a data range only if the graph types it `rdfs:Datatype` |
| `i rdf:type C`, with `C` a class | `ClassAssertion(C i)` and, for an IRI, `Declaration(NamedIndividual(i))` |
| `i p j`, `i p "v"` | `ObjectPropertyAssertion`, `DataPropertyAssertion`, by the kind of `p` |
| `i owl:sameAs j` | `SameIndividual(i j)`, even when `j` has no type (a DBpedia resource) |
| `x rdfs:label "..."` and the other built-in annotation properties | `AnnotationAssertion`, on any IRI and on the anonymous individuals of the assertions |
| `rdf:List`, `rdf:Bag`, reified statements | nothing: RDF structures with no OWL2 counterpart |

## Back to RDF

`ToRDFGraphAsync()` gives the ontology back as an RDFSharp graph, in the same mapping, ready for the writers, the stores and the SPARQL engine of RDFSharp. The triples it produces are the OWL2 ones: the properties now carry their `owl:` types.

```csharp
RDFGraph lifted = await ontology.ToRDFGraphAsync();
Console.WriteLine(lifted.ContainsTriple(new RDFTriple(new RDFResource(vr + "partOf"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY)));
Console.WriteLine(lifted.ContainsTriple(new RDFTriple(new RDFResource(vr + "partOf"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.PROPERTY)));
```

<div class="result" markdown="1">

```
True
False
```

</div>

What the lift gives is an ontology with the knowledge of the RDFSharp vocabulary and nothing more. The file [vallerosa.owx]({{ '/assets/data/vallerosa.owx' | relative_url }}) is this ontology plus what OWL2 can say and RDF Schema cannot; the next chapters add those pieces one by one.
