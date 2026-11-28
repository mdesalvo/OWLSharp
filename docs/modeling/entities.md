---
title: Entities and declarations
permalink: /modeling/entities/
tagline: Every name has a kind
lede: In RDF an IRI is just a name. In OWL2 every name is an entity of one kind, a class, a property, an individual or a datatype, and a declaration says which. The axioms of the next chapters are built from entities.
---

> In the [Resources, literals and triples](https://mdesalvo.github.io/RDFSharp/model/triples/) chapter of the RDFSharp manual, `vr:WhiteWine`, `vr:alcohol` and `vrd:vermentino-2023` were three `RDFResource` objects, alike. The ontology tells them apart: a class, a data property and an individual.
{: .story}

## The six kinds of entity

| Kind | Class | Names | At Vallerosa |
|---|---|---|---|
| class | `OWLClass` | a set of individuals | `vr:Wine`, `vr:RedWine` |
| object property | `OWLObjectProperty` | a relation between two individuals | `vr:producedBy`, `vr:partOf` |
| data property | `OWLDataProperty` | a relation between an individual and a value | `vr:price`, `vr:vintage` |
| annotation property | `OWLAnnotationProperty` | a relation that carries information for people, not for the reasoner | `rdfs:label`, `rdfs:comment` |
| named individual | `OWLNamedIndividual` | one thing of the domain | `vrd:rosso-2021`, `vrd:tuscany` |
| datatype | `OWLDatatype` | a set of values | `xsd:decimal`, `vr:abv` |

Each class takes the IRI as an `RDFResource` or, abbreviated, as an `XmlQualifiedName` of .NET with the local name and the namespace. Both forms name the same entity: `GetIRI()` gives the full IRI either way. A blank resource is refused, since an entity always has a name.

```csharp
using System.Xml;
using OWLSharp;
using OWLSharp.Ontology;
using RDFSharp.Model;

RDFNamespace vr = new RDFNamespace("vr", "https://vallerosa.example/vocab#");
RDFNamespace vrd = new RDFNamespace("vrd", "https://vallerosa.example/id/");

OWLClass whiteWine = new OWLClass(new RDFResource(vr + "WhiteWine"));
OWLClass sameWhiteWine = new OWLClass(new XmlQualifiedName("WhiteWine", vr.ToString()));
OWLDataProperty alcohol = new OWLDataProperty(new RDFResource(vr + "alcohol"));
OWLNamedIndividual vermentino = new OWLNamedIndividual(new RDFResource(vrd + "vermentino-2023"));

Console.WriteLine(whiteWine.GetIRI());
Console.WriteLine(sameWhiteWine.GetIRI().Equals(whiteWine.GetIRI()));

try
{
    new OWLClass(new RDFResource());
}
catch (OWLException exception)
{
    Console.WriteLine(exception.Message);
}
```

<div class="result" markdown="1">

```
https://vallerosa.example/vocab#WhiteWine
True
Cannot create OWLClass because given 'iri' parameter is a blank resource
```

</div>

The vocabularies of OWL2 have built-in entities that every ontology knows without declaring them: the classes `owl:Thing` (every individual) and `owl:Nothing` (no individual), the properties `owl:topObjectProperty` and `owl:bottomObjectProperty` with their data counterparts, the nine annotation properties such as `rdfs:label`, and the datatypes of the [OWL 2 datatype map](https://www.w3.org/TR/owl2-syntax/#Datatype_Maps) with `rdfs:Literal`. RDFSharp has their IRIs in `RDFVocabulary`:

<!-- pagecheck: skip -->
```csharp
OWLClass thing = new OWLClass(RDFVocabulary.OWL.THING);
OWLAnnotationProperty label = new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL);
OWLDatatype decimalType = new OWLDatatype(RDFVocabulary.XSD.DECIMAL);
```

## Declarations

A declaration is an axiom that says "this IRI is an entity of this kind". `OWLDeclaration` has one constructor for each kind and goes in `DeclarationAxioms`. `DeclareEntity` does the same in one call, and `CheckHasEntity` tells whether an ontology declares an entity:

```csharp
OWLOntology ontology = new OWLOntology(new Uri("https://vallerosa.example/ontology"));
ontology.DeclarationAxioms.Add(new OWLDeclaration(whiteWine));
ontology.DeclareEntity(alcohol);
ontology.DeclareEntity(vermentino);

Console.WriteLine(ontology.CheckHasEntity(alcohol));
Console.WriteLine(ontology.CheckHasEntity(new OWLObjectProperty(new RDFResource(vr + "alcohol"))));
```

<div class="result" markdown="1">

```
True
False
```

</div>

The second answer is `false`: `vr:alcohol` is declared as a data property, so the ontology has no object property with that IRI. Kind and IRI identify an entity together.

`GetDeclaredEntitiesOfType<T>()` lists the entities of one kind. On the Vallerosa ontology:

```csharp
OWLOntology vallerosa = await OWLOntology.FromFileAsync(OWLEnums.OWLFormats.OWL2XML, "vallerosa.owx");

Console.WriteLine($"{vallerosa.GetDeclaredEntitiesOfType<OWLClass>().Count} classes");
Console.WriteLine($"{vallerosa.GetDeclaredEntitiesOfType<OWLObjectProperty>().Count} object properties");
Console.WriteLine($"{vallerosa.GetDeclaredEntitiesOfType<OWLDataProperty>().Count} data properties");
Console.WriteLine($"{vallerosa.GetDeclaredEntitiesOfType<OWLNamedIndividual>().Count} named individuals");
Console.WriteLine($"{vallerosa.GetDeclaredEntitiesOfType<OWLDatatype>().Count} datatype");
```

<div class="result" markdown="1">

```
15 classes
9 object properties
4 data properties
22 named individuals
1 datatype
```

</div>

## When a declaration is needed

OWL2 does not require every entity to be declared: the [Structural Specification](https://www.w3.org/TR/owl2-syntax/#Typing_Constraints_of_OWL_2_DL) lets an IRI be used as an individual, for instance, without a declaration. Declarations matter where the kind cannot be guessed:

- Manchester syntax cannot tell an object property from a data property in `vr:price some xsd:decimal`, so its reader needs the declaration ([Ontologies and formats]({{ '/modeling/ontologies/' | relative_url }})).
- OWL 2 DL does not let one IRI be two kinds of property at once, nor a class and a datatype: declaring `vr:alcohol` both as an object property and as a data property makes the ontology fall out of OWL 2 DL.
- A typo in an IRI makes a new, undeclared entity. In the words of the specification, declarations "can be used to catch obvious errors in ontologies": a tool that expects every entity to be declared notices the one that is not.

The files that OWLSharp writes declare every entity they use, which keeps them readable in every syntax.

## Anonymous individuals

An individual can also have no name: `OWLAnonymousIndividual` stands for "something" that the ontology talks about without naming it, as a blank node does in RDF. It is built with a generated identifier or with one of yours, and it can stand wherever an individual stands, in assertions and annotations:

```csharp
OWLAnonymousIndividual someone = new OWLAnonymousIndividual();
OWLAnonymousIndividual sommelier = new OWLAnonymousIndividual("sommelier");

Console.WriteLine(someone.NodeID.StartsWith("ANON"));
Console.WriteLine(sommelier.GetIRI());
```

<div class="result" markdown="1">

```
True
bnode:sommelier
```

</div>

An anonymous individual is never declared: declarations are for names. Its identifier only matters inside the file that holds it.
