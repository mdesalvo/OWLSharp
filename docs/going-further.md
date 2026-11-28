---
title: Going further
permalink: /going-further/
tagline: Where the story continues
lede: The Vallerosa catalog is now an ontology that knows where its wines come from, which ones are Tuscan, which facts contradict it. Here is where to go next.
---

## The data underneath: RDFSharp

An OWLSharp ontology is RDF underneath: `ToRDFGraphAsync` turns it into an `RDFGraph` and `FromRDFGraphAsync` reads one back. Everything the [RDFSharp manual](https://mdesalvo.github.io/RDFSharp/) shows applies to that graph: stores that keep the source of every fact, SPARQL queries over the inferences, SHACL shapes for the checks that need a closed world.

## LinkedData ontologies: OWLSharp.Extensions

[OWLSharp.Extensions](https://github.com/mdesalvo/OWLSharp.Extensions) adds helpers for common LinkedData ontologies: concept schemes with [SKOS](https://www.w3.org/TR/skos-primer/), features with a position in space with [GeoSPARQL](http://www.opengis.net/ont/geosparql) and in time with [OWL-TIME](https://www.w3.org/TR/owl-time/).

This manual leaves three things of the cooperative untold on purpose: where its places and its shops are, when its wines are made and sold, how its grapes and its dishes fall into families. The manual of OWLSharp.Extensions picks the Vallerosa case up there, with the same files.

## The standards

OWLSharp implements W3C recommendations; when in doubt about what something means, they are the final word.

| Standard | What it defines |
|---|---|
| [OWL 2 Structural Specification](https://www.w3.org/TR/owl2-syntax/) | entities, expressions, axioms and the Functional syntax |
| [OWL 2 Direct Semantics](https://www.w3.org/TR/owl2-direct-semantics/) | what every construct means |
| [OWL 2 Mapping to RDF Graphs](https://www.w3.org/TR/owl2-mapping-to-rdf/) | how an ontology is written as RDF and read back |
| [OWL 2 XML Serialization](https://www.w3.org/TR/owl2-xml-serialization/) and [Manchester Syntax](https://www.w3.org/TR/owl2-manchester-syntax/) | the other two syntaxes |
| [OWL 2 Profiles](https://www.w3.org/TR/owl2-profiles/) | EL, QL and RL, with the rules of OWL 2 RL |
| [SWRL](https://www.w3.org/submissions/SWRL/) (W3C Member Submission) | rules and built-ins |
| [XPath and XQuery Functions and Operators 3.1](https://www.w3.org/TR/xpath-functions-31/) | what the SWRL built-ins compute |

## Help and contributions

Questions, bugs and ideas are welcome as [GitHub issues](https://github.com/mdesalvo/OWLSharp/issues). Every page of this manual has an *Improve this page* link at its foot: a correction is one pull request away.
