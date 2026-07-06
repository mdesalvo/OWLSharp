# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

OWLSharp is a .NET library built atop [RDFSharp](https://github.com/mdesalvo/RDFSharp) for OWL2 ontology **modeling** (OWL2/XML), **reasoning** (26 RDFS/OWL2 entailment rules + SWRL rules), and **validation** (29 analysis rules for pitfalls/inconsistencies). Extensions for LinkedData ontologies live in the separate `OWLSharp.Extensions` repo.

## Commands

```bash
# Build (targets netstandard2.0 + net8.0 for OWLSharp, net10.0 for tests)
dotnet build -c Release

# Run full test suite (MSTest)
dotnet test -c Release

# Run a single test class
dotnet test --filter "FullyQualifiedName~OWLHasSelfEntailmentTest"

# Run a single test method
dotnet test --filter "FullyQualifiedName~OWLHasSelfEntailmentTest.ShouldEntailHasSelf"

# With coverage (as done in CI)
dotnet test -c Release --collect:"XPlat Code Coverage"
```

CI (`.github/workflows/linux.yml`, `windows.yml`) builds+tests on push to `main` on both Ubuntu and Windows, using .NET 8, then uploads coverage to Codecov.

## Architecture

### Rule dispatch pattern (Reasoner & Validator)

Both `OWLReasoner` (`Reasoner/OWLReasoner.cs`) and `OWLValidator` (`Validator/OWLValidator.cs`) work the same way and adding a rule to either touches **three places**:

1. An enum value in `OWLEnums.OWLReasonerRules` / `OWLEnums.OWLValidatorRules` (`OWLEnums.cs`), with the rule's logic documented in XML-doc as a `Antecedent -> Consequent` style comment.
2. A rule class in `Reasoner/RuleSet/` / `Validator/RuleSet/`: an `internal static class` with a `rulename` constant (`nameof(...)` of the enum value) and a static `ExecuteRule(OWLOntology ontology)` method returning `List<OWLInference>` / `List<OWLIssue>`.
3. A `case` in the big `Parallel.ForEach(Rules, ...)` switch inside `OWLReasoner.ApplyToOntologyAsync` / `OWLValidator.ApplyToOntologyAsync` that calls it and stores the result in the registry dictionary keyed by rule name.

Rules run in parallel and independently; the reasoner deduplicates resulting inferences against axioms already asserted in the ontology (per-axiom-type `HashSet<string>` of `GetXML()`), and — unless `OWLReasonerOptions.EnableIterativeReasoning` is disabled — feeds new inferences back into the ontology and re-runs itself up to `MaxAllowedIterations` times (default 3) until fixpoint or the cap is hit.

SWRL rules (`ontology.Rules`, of type `SWRLRule`) are executed by the reasoner alongside the OWL2 rules above, via `SWRLRule.ApplyToOntologyAsync`, and participate in the same inference registry/deduplication/iteration.

### SWRL engine and the OWLShims layer

`Ontology/Rules/` implements SWRL: `SWRLAntecedent`/`SWRLConsequent` combine `SWRLAtom`s (`Atoms/`) and evaluate `SWRLBuiltIn`s (`BuiltIns/`, one file per built-in predicate, e.g. `swrl:greaterThan`, `swrl:substring`). Antecedent evaluation builds and combines binding tables by calling directly into Mirella (RDFSharp's query engine) internals: `RDFTable`/`RDFTableRow`/`RDFTableColumn`.

Those RDFSharp types are `internal sealed` and only friend-visible to the `OWLSharp` assembly (not `OWLSharp.Test`). `OWLShims.cs` defines composition wrappers — `OWLTable`, `OWLTableRow`, `OWLTableColumn`, `OWLTableRowCollection`, `OWLTableColumnCollection` — that mirror the RDFSharp member names exactly, so production SWRL code and its tests use the same `OWLTable*` API instead of the underlying `RDFTable*`. When touching SWRL antecedent/table-combination code or its tests, use `OWLTable*`, not `RDFTable*` directly; if a new `RDFTable*` member needs to be exposed, add it to the matching shim with the same name/signature.

### Ontology model & serialization

`OWLOntology` (`Ontology/OWLOntology.cs`) holds the T-BOX/A-BOX/R-BOX as typed axiom lists (`DeclarationAxioms`, `ClassAxioms`, `ObjectPropertyAxioms`, `DataPropertyAxioms`, `AssertionAxioms`, `AnnotationAxioms`, `Rules`, ...). Axiom subtypes are wired via `[XmlElement(typeof(...), ElementName="...")]` attributes directly on these list properties — the model classes are serialized as OWL2/XML through `System.Xml.Serialization`, driven by `OWLSerializer.cs` (internal). Query-style access to axioms/expressions goes through extension-style static helpers (`OWLOntologyHelper`, `OWLImportHelper`), e.g. `ontology.GetClassAxiomsOfType<T>()`, `ontology.GetAssertionAxiomsOfType<T>()`.

`Ontology/Expressions/` mirrors the OWL2 class/property/individual/literal/data-range expression hierarchy (e.g. `ObjectHasSelf`, `ObjectHasValue`, restrictions); rule/analysis code pattern-matches on these expression types.

### Test project layout

`OWLSharp.Test` mirrors `OWLSharp`'s folder structure 1:1 (e.g. `Reasoner/RuleSet/OWLHasSelfEntailmentTest.cs` tests `Reasoner/RuleSet/OWLHasSelfEntailment.cs`). It targets `net10.0` only (vs. the library's `netstandard2.0`/`net8.0`) and depends on the `InternalsVisibleTo` grant from `OWLSharp/Properties/AssemblyInfo.cs` to reach internal rule/helper classes directly.
