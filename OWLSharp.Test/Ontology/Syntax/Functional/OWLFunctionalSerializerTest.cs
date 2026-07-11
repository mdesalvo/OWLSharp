/*
   Copyright 2014-2026 Marco De Salvo

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

/// <summary>
/// Exercises OWLFunctionalSerializer.SerializeOntology directly: document structure (prefixes, header,
/// imports, ontology-level annotations, the anonymous-ontology and orphan-versionIRI edge cases, the
/// SWRL warning+skip policy). Per-axiom-type "Keyword( axiomAnnotations arg1 arg2 ... )" rendering is
/// tested where the responsibility actually lives - on each OWLAxiom subtype itself, in its own
/// OWL&lt;Axiom&gt;Test.cs (e.g. OWLSubClassOfTest.ShouldSerializeToFunctional) - not centralized here.
/// </summary>
[TestClass]
public class OWLFunctionalSerializerTest
{
    #region Utilities
    //OnWarning is a static event: every test that raises warnings must subscribe/unsubscribe around
    //its own call, or captured messages would leak across tests running in the same process
    private static List<string> CaptureWarnings(Action action)
    {
        List<string> messages = [];
        void Handler(string message) => messages.Add(message);

        OWLEvents.OnWarning += Handler;
        try { action(); }
        finally { OWLEvents.OnWarning -= Handler; }
        return messages;
    }

    private static OWLOntology CreateBaseOntology()
    {
        OWLOntology ontology = new OWLOntology(new Uri("http://example.org/pz"));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        return ontology;
    }
    #endregion

    #region Tests (document structure)
    [TestMethod]
    public void ShouldSerializePrefixesHeaderImportsAndAnnotations()
    {
        OWLOntology ontology = new OWLOntology(new Uri("http://example.org/pz"), new Uri("http://example.org/pz/1.0"));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/base")));
        ontology.Annotations.Add(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("a comment"))));

        string document = OWLFunctionalSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("Prefix(pz:=<http://example.org/pz#>)", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Ontology( <http://example.org/pz> <http://example.org/pz/1.0>", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Import( <http://example.org/base> )", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Annotation( rdfs:comment \"a comment\" )", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldSerializeOntologyWithoutIRIAsAnonymousHeader()
    {
        OWLOntology ontology = new OWLOntology();

        string document = OWLFunctionalSerializer.SerializeOntology(ontology);

        //An anonymous ontology must not emit an empty "< >" IRI token: the IRI argument is entirely
        //omitted (ontologyIRI is optional in the Ontology(...) production), not emitted-but-blank
        Assert.IsFalse(document.Contains("< >", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Ontology(\n", StringComparison.Ordinal) || document.Contains("Ontology(\r\n", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldNotEmitOrphanVersionIRIWhenOntologyIRIIsAbsent()
    {
        //A versionIRI is only grammatically legal alongside an ontologyIRI: if the ontology has a
        //VersionIRI but no IRI, the serializer must not emit a dangling versionIRI on its own
        OWLOntology ontology = new OWLOntology { VersionIRI = "http://example.org/pz/1.0" };

        string document = OWLFunctionalSerializer.SerializeOntology(ontology);

        Assert.IsFalse(document.Contains("http://example.org/pz/1.0", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipSWRLRules()
    {
        OWLOntology ontology = CreateBaseOntology();
        ontology.Rules.Add(new SWRLRule(
            new RDFPlainLiteral("test-rule"), new RDFPlainLiteral("a test rule"), new SWRLAntecedent(), new SWRLConsequent()));

        List<string> warnings = CaptureWarnings(() => OWLFunctionalSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("SWRL", StringComparison.Ordinal));
    }
    #endregion
}
