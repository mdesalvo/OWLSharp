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
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology;

/// <summary>
/// Exercises OWLFunctionalSerializer.SerializeOntology directly: document structure (prefixes, header,
/// imports, ontology-level annotations, the anonymous-ontology and orphan-versionIRI edge cases, the
/// DLSafeRule rendering of SWRL rules, the warning+skip policy for incomplete annotation assertions). Per-axiom-type "Keyword( axiomAnnotations arg1 arg2 ... )" rendering is
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
    #endregion

    #region Tests (SWRL rules)
    private static SWRLVariableArgument Variable(string name)
        => new SWRLVariableArgument(new RDFVariable($"?{name}"));

    [TestMethod]
    public void ShouldSerializeSWRLRuleWithoutWarnings()
    {
        //Verifies a rule is written as DLSafeRule with its label and comment annotations and raises no warning
        OWLOntology ontology = CreateBaseOntology();
        ontology.Rules.Add(new SWRLRule(
            new RDFPlainLiteral("adult"), new RDFPlainLiteral("a person of age"),
            new SWRLAntecedent { Atoms = [ new SWRLClassAtom(new OWLClass(new RDFResource("http://example.org/pz#Person")), Variable("x")) ] },
            new SWRLConsequent { Atoms = [ new SWRLClassAtom(new OWLClass(new RDFResource("http://example.org/pz#Adult")), Variable("x")) ] }));

        string document = null;
        List<string> warnings = CaptureWarnings(() => document = OWLFunctionalSerializer.SerializeOntology(ontology));

        Assert.IsEmpty(warnings);
        Assert.IsTrue(document.Contains(
            "    DLSafeRule( Annotation( rdfs:label \"adult\" ) Annotation( rdfs:comment \"a person of age\" ) Body( ClassAtom( pz:Person Variable( <urn:swrl:var#X> ) ) ) Head( ClassAtom( pz:Adult Variable( <urn:swrl:var#X> ) ) ) )",
            StringComparison.Ordinal));
        Assert.IsTrue(document.EndsWith(")", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldSerializeSWRLRuleWithEveryAtomKindAndBuiltInWithTypedLiteral()
    {
        //Verifies each atom kind gets its own keyword and the built-in comes after the atoms with its typed literal
        OWLOntology ontology = CreateBaseOntology();
        ontology.Rules.Add(new SWRLRule(
            new RDFPlainLiteral("all"), new RDFPlainLiteral("every atom"),
            new SWRLAntecedent
            {
                Atoms = [
                    new SWRLDataRangeAtom(new OWLDatatype(RDFVocabulary.XSD.INTEGER), Variable("age")),
                    new SWRLObjectPropertyAtom(new OWLObjectProperty(new RDFResource("http://example.org/pz#hasParent")), Variable("x"), Variable("y")),
                    new SWRLDataPropertyAtom(new OWLDataProperty(new RDFResource("http://example.org/pz#hasAge")), Variable("x"), Variable("age")),
                    new SWRLAnnotationPropertyAtom(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), Variable("x"), Variable("n")),
                    new SWRLSameIndividualAtom(Variable("x"), new SWRLIndividualArgument(new RDFResource("http://example.org/pz#Bob")))
                ],
                BuiltIns = [ SWRLBuiltIn.GreaterThan(Variable("age"), new SWRLLiteralArgument(new RDFTypedLiteral("18", RDFModelEnums.RDFDatatypes.XSD_INTEGER))) ]
            },
            new SWRLConsequent { Atoms = [ new SWRLDifferentIndividualsAtom(Variable("x"), Variable("y")) ] }));

        string document = OWLFunctionalSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("DataRangeAtom( xsd:integer Variable( <urn:swrl:var#AGE> ) )", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("ObjectPropertyAtom( pz:hasParent Variable( <urn:swrl:var#X> ) Variable( <urn:swrl:var#Y> ) )", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("DataPropertyAtom( pz:hasAge Variable( <urn:swrl:var#X> ) Variable( <urn:swrl:var#AGE> ) )", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("AnnotationPropertyAtom( rdfs:label Variable( <urn:swrl:var#X> ) Variable( <urn:swrl:var#N> ) )", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("SameIndividualAtom( Variable( <urn:swrl:var#X> ) pz:Bob ) BuiltInAtom( <http://www.w3.org/2003/11/swrlb#greaterThan> Variable( <urn:swrl:var#AGE> ) \"18\"^^xsd:integer ) )", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Head( DifferentIndividualsAtom( Variable( <urn:swrl:var#X> ) Variable( <urn:swrl:var#Y> ) ) )", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldSerializeInverseObjectPropertyAtomAndReadItBackAsSwappedAtom()
    {
        //Verifies an atom whose predicate is an inverse property is written with ObjectInverseOf and reread over the named property
        OWLOntology ontology = CreateBaseOntology();
        ontology.Rules.Add(new SWRLRule
        {
            Antecedent = new SWRLAntecedent
            {
                Atoms = [ new SWRLObjectPropertyAtom
                {
                    Predicate = new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://example.org/pz#hasParent"))),
                    LeftArgument = Variable("x"),
                    RightArgument = Variable("y")
                } ]
            },
            Consequent = new SWRLConsequent()
        });

        string document = OWLFunctionalSerializer.SerializeOntology(ontology);
        OWLOntology reparsedOntology = OWLFunctionalParser.DeserializeOntology(document);

        Assert.IsTrue(document.Contains("ObjectPropertyAtom( ObjectInverseOf( pz:hasParent ) Variable( <urn:swrl:var#X> ) Variable( <urn:swrl:var#Y> ) )", StringComparison.Ordinal));
        SWRLObjectPropertyAtom atom = (SWRLObjectPropertyAtom)reparsedOntology.Rules[0].Antecedent.Atoms[0];
        Assert.IsInstanceOfType<OWLObjectProperty>(atom.Predicate);
        Assert.AreEqual("urn:swrl:var#Y", ((SWRLVariableArgument)atom.LeftArgument).IRI);
        Assert.AreEqual("urn:swrl:var#X", ((SWRLVariableArgument)atom.RightArgument).IRI);
    }

    [TestMethod]
    public void ShouldSerializeSWRLRuleWithoutBodyAndHead()
    {
        //Verifies a rule lacking both sections (as read from incomplete OWL/XML) still gets a readable form
        OWLOntology ontology = CreateBaseOntology();
        ontology.Rules.Add(new SWRLRule());

        string document = OWLFunctionalSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("DLSafeRule( Body( ) Head( ) )", StringComparison.Ordinal));
        Assert.HasCount(1, OWLFunctionalParser.DeserializeOntology(document).Rules);
    }
    #endregion

    #region Tests (incomplete annotation assertions)
    [TestMethod]
    public void ShouldWarnAndSkipAnnotationAssertionWithoutSubject()
    {
        //Verifies an assertion lacking its subject is skipped with a warning instead of failing the serialization
        OWLOntology ontology = CreateBaseOntology();
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion { AnnotationProperty = new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), ValueLiteral = new OWLLiteral(new RDFPlainLiteral("lost")) });

        string document = null;
        List<string> warnings = CaptureWarnings(() => document = OWLFunctionalSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("AnnotationAssertion", StringComparison.Ordinal));
        Assert.IsFalse(document.Contains("AnnotationAssertion(", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipAnnotationAssertionWithoutValue()
    {
        //Verifies an assertion lacking both its IRI and its literal value is skipped with a warning
        OWLOntology ontology = CreateBaseOntology();
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion { AnnotationProperty = new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), SubjectIRI = "http://example.org/pz#Pizza" });

        string document = null;
        List<string> warnings = CaptureWarnings(() => document = OWLFunctionalSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsFalse(document.Contains("AnnotationAssertion(", StringComparison.Ordinal));
    }
    #endregion
}
