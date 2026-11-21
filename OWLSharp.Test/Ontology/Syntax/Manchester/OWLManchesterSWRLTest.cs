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
/// Exercises the SWRL support of the Manchester syntax: reading of "Rule:" frames in the format
/// written by Protege and the OWL API (every kind of atom and built-in) and writing of SWRL rules
/// </summary>
[TestClass]
public class OWLManchesterSWRLTest
{
    #region Utilities
    //Every test document is prefixed with "pz:" and default prefixes on the pizza namespace plus the swrlb one,
    //so that rules can use both prefixed and simple names
    private const string Prologue =
        "Prefix: pz: <http://example.org/pz#>\nPrefix: : <http://example.org/pz#>\n"
        + "Prefix: swrlb: <http://www.w3.org/2003/11/swrlb#>\nOntology: <http://example.org/pz>\n";

    private static OWLOntology Parse(string body)
        => OWLManchesterParser.DeserializeOntology(Prologue + body);

    private static List<string> CaptureWarnings(Action action)
    {
        List<string> messages = [];
        void Handler(string message) => messages.Add(message);

        OWLEvents.OnWarning += Handler;
        try { action(); }
        finally { OWLEvents.OnWarning -= Handler; }
        return messages;
    }

    private static SWRLVariableArgument Variable(string name)
        => new SWRLVariableArgument(new RDFVariable($"?{name}"));

    private static OWLClass Class(string localName)
        => new OWLClass(new RDFResource($"http://example.org/pz#{localName}"));

    private static OWLObjectProperty ObjectProperty(string localName)
        => new OWLObjectProperty(new RDFResource($"http://example.org/pz#{localName}"));

    private static OWLDataProperty DataProperty(string localName)
        => new OWLDataProperty(new RDFResource($"http://example.org/pz#{localName}"));

    //Builds an ontology declaring the entities that the writing tests use, so that rules read back with the same atom kinds
    private static OWLOntology CreateOntologyWithEntities()
    {
        OWLOntology ontology = new OWLOntology(new Uri("http://example.org/pz"));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("swrlb", "http://www.w3.org/2003/11/swrlb#")));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(Class("Pizza")));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(Class("Expensive")));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(ObjectProperty("hasTopping")));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(DataProperty("price")));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(new OWLNamedIndividual(new RDFResource("http://example.org/pz#Margherita"))));
        return ontology;
    }
    #endregion

    #region Tests (reading atoms)
    //Verifies that a class atom is read for a declared class
    [TestMethod]
    public void ShouldReadClassAtomOnDeclaredClass()
    {
        OWLOntology ontology = Parse("Class: :Pizza\nRule:\n    :Pizza(?x) -> :Pizza(?x)");

        Assert.HasCount(1, ontology.Rules);
        SWRLClassAtom atom = (SWRLClassAtom)ontology.Rules[0].Antecedent.Atoms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", ((OWLClass)atom.Predicate).GetIRI().ToString());
        Assert.AreEqual("urn:swrl:var#x", ((SWRLVariableArgument)atom.LeftArgument).IRI);
        Assert.IsNull(atom.RightArgument);
        Assert.IsInstanceOfType<SWRLClassAtom>(ontology.Rules[0].Consequent.Atoms[0]);
    }

    //Verifies that an undeclared IRI with one argument is read as a class atom
    [TestMethod]
    public void ShouldReadClassAtomOnUndeclaredIRI()
    {
        OWLOntology ontology = Parse("Rule:\n    pz:Pizza(?x) -> pz:Expensive(?x)");

        Assert.IsInstanceOfType<SWRLClassAtom>(ontology.Rules[0].Antecedent.Atoms[0]);
        Assert.IsInstanceOfType<OWLClass>(ontology.Rules[0].Antecedent.Atoms[0].Predicate);
    }

    //Verifies that a data range atom is read for a declared datatype
    [TestMethod]
    public void ShouldReadDataRangeAtomOnDeclaredDatatype()
    {
        OWLOntology ontology = Parse("Datatype: :Price\nRule:\n    :Price(?p) -> :Price(?p)");

        SWRLDataRangeAtom atom = (SWRLDataRangeAtom)ontology.Rules[0].Antecedent.Atoms[0];
        Assert.IsInstanceOfType<OWLDatatype>(atom.Predicate);
        Assert.AreEqual("urn:swrl:var#p", ((SWRLVariableArgument)atom.LeftArgument).IRI);
    }

    //Verifies that an object property atom is read for a declared object property
    [TestMethod]
    public void ShouldReadObjectPropertyAtomOnDeclaredProperty()
    {
        OWLOntology ontology = Parse("ObjectProperty: :hasTopping\nRule:\n    :hasTopping(?x, ?y) -> :hasTopping(?x, ?y)");

        SWRLObjectPropertyAtom atom = (SWRLObjectPropertyAtom)ontology.Rules[0].Antecedent.Atoms[0];
        Assert.IsInstanceOfType<OWLObjectProperty>(atom.Predicate);
        Assert.AreEqual("urn:swrl:var#x", ((SWRLVariableArgument)atom.LeftArgument).IRI);
        Assert.AreEqual("urn:swrl:var#y", ((SWRLVariableArgument)atom.RightArgument).IRI);
    }

    //Verifies that an undeclared IRI with a variable or an individual as second argument is read as an object property atom
    [TestMethod]
    public void ShouldReadObjectPropertyAtomOnUndeclaredIRIWithVariableOrIndividual()
    {
        OWLOntology ontology = Parse("Rule:\n    :hasTopping(?x, ?y), :hasTopping(?x, :Mozzarella) -> :hasTopping(?x, ?y)");

        Assert.IsInstanceOfType<SWRLObjectPropertyAtom>(ontology.Rules[0].Antecedent.Atoms[0]);
        SWRLObjectPropertyAtom individualAtom = (SWRLObjectPropertyAtom)ontology.Rules[0].Antecedent.Atoms[1];
        Assert.AreEqual("http://example.org/pz#Mozzarella", ((SWRLIndividualArgument)individualAtom.RightArgument).IRI);
    }

    //Verifies that a data property atom is read for a declared data property
    [TestMethod]
    public void ShouldReadDataPropertyAtomOnDeclaredProperty()
    {
        OWLOntology ontology = Parse("DataProperty: :price\nRule:\n    :price(?x, ?p) -> :price(?x, ?p)");

        SWRLDataPropertyAtom atom = (SWRLDataPropertyAtom)ontology.Rules[0].Antecedent.Atoms[0];
        Assert.IsInstanceOfType<OWLDataProperty>(atom.Predicate);
    }

    //Verifies that an undeclared IRI with a literal as second argument is read as a data property atom
    [TestMethod]
    public void ShouldReadDataPropertyAtomOnUndeclaredIRIWithLiteral()
    {
        OWLOntology ontology = Parse("Rule:\n    :price(?x, 10) -> :price(?x, \"cheap\"@en)");

        SWRLDataPropertyAtom bodyAtom = (SWRLDataPropertyAtom)ontology.Rules[0].Antecedent.Atoms[0];
        SWRLLiteralArgument bodyLiteral = (SWRLLiteralArgument)bodyAtom.RightArgument;
        Assert.AreEqual("10", bodyLiteral.Value);
        Assert.AreEqual(RDFVocabulary.XSD.INTEGER.ToString(), bodyLiteral.DatatypeIRI);
        SWRLLiteralArgument headLiteral = (SWRLLiteralArgument)((SWRLDataPropertyAtom)ontology.Rules[0].Consequent.Atoms[0]).RightArgument;
        Assert.AreEqual("en", headLiteral.Language);
    }

    //Verifies that an annotation property atom is read for a declared annotation property
    [TestMethod]
    public void ShouldReadAnnotationPropertyAtomOnDeclaredProperty()
    {
        OWLOntology ontology = Parse("AnnotationProperty: :note\nRule:\n    :note(?x, ?y) -> :note(?x, ?y)");

        Assert.IsInstanceOfType<SWRLAnnotationPropertyAtom>(ontology.Rules[0].Antecedent.Atoms[0]);
        Assert.IsInstanceOfType<OWLAnnotationProperty>(ontology.Rules[0].Antecedent.Atoms[0].Predicate);
    }

    //Verifies that sameAs and differentFrom are read as equality atoms with the predicate that the public constructors assign
    [TestMethod]
    public void ShouldReadSameAsAndDifferentFromAtoms()
    {
        OWLOntology ontology = Parse("Rule:\n    sameAs(?x, ?y), differentFrom(?x, :Margherita) -> sameAs(?y, ?x)");

        SWRLSameIndividualAtom sameAtom = (SWRLSameIndividualAtom)ontology.Rules[0].Antecedent.Atoms[0];
        SWRLDifferentIndividualsAtom differentAtom = (SWRLDifferentIndividualsAtom)ontology.Rules[0].Antecedent.Atoms[1];
        Assert.AreEqual(RDFVocabulary.OWL.SAME_AS, sameAtom.Predicate.GetIRI());
        Assert.AreEqual(RDFVocabulary.OWL.DIFFERENT_FROM, differentAtom.Predicate.GetIRI());
        Assert.AreEqual("http://example.org/pz#Margherita", ((SWRLIndividualArgument)differentAtom.RightArgument).IRI);
        Assert.IsInstanceOfType<SWRLSameIndividualAtom>(ontology.Rules[0].Consequent.Atoms[0]);
    }

    //Verifies that an equality atom needs exactly two arguments
    [TestMethod]
    public void ShouldThrowExceptionOnEqualityAtomWithOneArgument()
        => Assert.ThrowsExactly<OWLException>(() => Parse("Rule:\n    sameAs(?x) -> :Pizza(?x)"));

    //Verifies that an atom with more than two arguments is rejected unless it is a built-in
    [TestMethod]
    public void ShouldThrowExceptionOnAtomWithThreeArguments()
        => Assert.ThrowsExactly<OWLException>(() => Parse("Rule:\n    :Pizza(?x, ?y, ?z) -> :Pizza(?x)"));

    //Verifies that a declared property cannot be used with a single argument
    [TestMethod]
    public void ShouldThrowExceptionOnDeclaredPropertyWithOneArgument()
        => Assert.ThrowsExactly<OWLException>(() => Parse("ObjectProperty: :hasTopping\nRule:\n    :hasTopping(?x) -> :Pizza(?x)"));

    //Verifies that a declared class cannot be used with two arguments
    [TestMethod]
    public void ShouldThrowExceptionOnDeclaredClassWithTwoArguments()
        => Assert.ThrowsExactly<OWLException>(() => Parse("Class: :Pizza\nRule:\n    :Pizza(?x, ?y) -> :Pizza(?x)"));
    #endregion

    #region Tests (reading built-ins and rules)
    //Verifies that a swrlb built-in goes to the built-ins of the body and that an untyped number becomes an integer literal
    [TestMethod]
    public void ShouldReadBuiltInWithUntypedNumericLiteral()
    {
        OWLOntology ontology = Parse("Rule:\n    :price(?x, ?p), swrlb:greaterThan(?p, 10) -> :Expensive(?x)");

        Assert.HasCount(1, ontology.Rules[0].Antecedent.Atoms);
        Assert.HasCount(1, ontology.Rules[0].Antecedent.BuiltIns);
        SWRLBuiltIn builtIn = ontology.Rules[0].Antecedent.BuiltIns[0];
        Assert.AreEqual("http://www.w3.org/2003/11/swrlb#greaterThan", builtIn.IRI);
        Assert.HasCount(2, builtIn.Arguments);
        Assert.AreEqual("urn:swrl:var#p", ((SWRLVariableArgument)builtIn.Arguments[0]).IRI);
        SWRLLiteralArgument literalArgument = (SWRLLiteralArgument)builtIn.Arguments[1];
        Assert.AreEqual("10", literalArgument.Value);
        Assert.AreEqual(RDFVocabulary.XSD.INTEGER.ToString(), literalArgument.DatatypeIRI);
    }

    //Verifies that a built-in written with a full IRI is told apart from entities by its namespace
    [TestMethod]
    public void ShouldReadBuiltInWrittenAsFullIRI()
    {
        OWLOntology ontology = Parse("Rule:\n    :price(?x, ?p), <http://www.w3.org/2003/11/swrlb#lessThan>(?p, 5.5) -> :Expensive(?x)");

        Assert.HasCount(1, ontology.Rules[0].Antecedent.BuiltIns);
        Assert.AreEqual(RDFVocabulary.XSD.DECIMAL.ToString(), ((SWRLLiteralArgument)ontology.Rules[0].Antecedent.BuiltIns[0].Arguments[1]).DatatypeIRI);
    }

    //Verifies that a built-in in the head of a rule is rejected
    [TestMethod]
    public void ShouldThrowExceptionOnBuiltInInHead()
        => Assert.ThrowsExactly<OWLException>(() => Parse("Rule:\n    :price(?x, ?p) -> swrlb:greaterThan(?p, 10)"));

    //Verifies that the annotations of a rule are read before its atoms
    [TestMethod]
    public void ShouldReadRuleWithAnnotations()
    {
        OWLOntology ontology = Parse("Rule:\n    Annotations: rdfs:label \"expensive\", rdfs:comment \"a pricey pizza\"\n    :Pizza(?x) -> :Expensive(?x)");

        Assert.HasCount(1, ontology.Rules);
        Assert.HasCount(2, ontology.Rules[0].Annotations);
        Assert.AreEqual("expensive", ontology.Rules[0].Annotations[0].ValueLiteral.Value);
        Assert.AreEqual("a pricey pizza", ontology.Rules[0].Annotations[1].ValueLiteral.Value);
        Assert.HasCount(1, ontology.Rules[0].Antecedent.Atoms);
    }

    //Verifies that a negative number in a fact is not broken by the rule arrow and that negative numbers work inside rules too
    [TestMethod]
    public void ShouldKeepNegativeNumbersWithRuleArrow()
    {
        OWLOntology ontology = Parse(
            "DataProperty: :temperature\n"
            + "Individual: :Oven\n    Facts: :temperature -5\n"
            + "Rule:\n    :temperature(?x, -5) -> :Pizza(?x)");

        Assert.HasCount(1, ontology.AssertionAxioms);
        Assert.AreEqual("-5", ((OWLDataPropertyAssertion)ontology.AssertionAxioms[0]).Literal.Value);
        Assert.HasCount(1, ontology.Rules);
        Assert.AreEqual("-5", ((SWRLLiteralArgument)((SWRLDataPropertyAtom)ontology.Rules[0].Antecedent.Atoms[0]).RightArgument).Value);
    }

    //Verifies that a document written the way Protege does is read completely
    [TestMethod]
    public void ShouldReadProtegeStyleDocument()
    {
        const string document =
            "Prefix: : <http://example.org/pz#>\n"
            + "Prefix: swrlb: <http://www.w3.org/2003/11/swrlb#>\n"
            + "Prefix: xsd: <http://www.w3.org/2001/XMLSchema#>\n\n"
            + "Ontology: <http://example.org/pz>\n\n"
            + "Class: :Expensive\n\n"
            + "Class: :Pizza\n\n"
            + "ObjectProperty: :hasTopping\n\n"
            + "DataProperty: :price\n\n"
            + "Rule: \n"
            + "    :Pizza(?x), :hasTopping(?x, ?y), :price(?x, ?p), swrlb:greaterThan(?p, 10) -> :Expensive(?x)\n";

        OWLOntology ontology = OWLManchesterParser.DeserializeOntology(document);

        Assert.HasCount(4, ontology.DeclarationAxioms);
        Assert.HasCount(1, ontology.Rules);
        SWRLRule rule = ontology.Rules[0];
        Assert.HasCount(3, rule.Antecedent.Atoms);
        Assert.IsInstanceOfType<SWRLClassAtom>(rule.Antecedent.Atoms[0]);
        Assert.IsInstanceOfType<SWRLObjectPropertyAtom>(rule.Antecedent.Atoms[1]);
        Assert.IsInstanceOfType<SWRLDataPropertyAtom>(rule.Antecedent.Atoms[2]);
        Assert.HasCount(1, rule.Antecedent.BuiltIns);
        Assert.HasCount(1, rule.Consequent.Atoms);
        Assert.IsInstanceOfType<SWRLClassAtom>(rule.Consequent.Atoms[0]);
    }

    //Verifies that several rules and frames can follow each other in the same document
    [TestMethod]
    public void ShouldReadRulesInterleavedWithFrames()
    {
        OWLOntology ontology = Parse("Rule:\n    :Pizza(?x) -> :Expensive(?x)\nClass: :Pizza\nRule:\n    :Expensive(?x) -> :Pizza(?x)\n");

        Assert.HasCount(2, ontology.Rules);
        Assert.HasCount(1, ontology.DeclarationAxioms);
    }

    //Verifies that the first atom of a rule is not taken for the header of an entity frame
    [TestMethod]
    public void ShouldNotDeclareEntityFromRuleFrame()
    {
        OWLOntology ontology = Parse("Rule:\n    :Pizza(?x) -> :Expensive(?x)");

        Assert.HasCount(0, ontology.DeclarationAxioms);
    }
    #endregion

    #region Tests (writing)
    //Verifies that a rule built with the public constructor is written and read back with its label and comment
    [TestMethod]
    public void ShouldRoundTripRuleBuiltWithPublicConstructor()
    {
        OWLOntology ontology = CreateOntologyWithEntities();
        SWRLAntecedent antecedent = new SWRLAntecedent();
        antecedent.Atoms.Add(new SWRLClassAtom(Class("Pizza"), Variable("x")));
        antecedent.Atoms.Add(new SWRLObjectPropertyAtom(ObjectProperty("hasTopping"), Variable("x"), Variable("y")));
        antecedent.Atoms.Add(new SWRLDataPropertyAtom(DataProperty("price"), Variable("x"), Variable("p")));
        antecedent.BuiltIns.Add(SWRLBuiltIn.GreaterThan(Variable("p"), new SWRLLiteralArgument(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        SWRLConsequent consequent = new SWRLConsequent();
        consequent.Atoms.Add(new SWRLClassAtom(Class("Expensive"), Variable("x")));
        SWRLRule rule = new SWRLRule(new RDFPlainLiteral("expensive-pizza"), new RDFPlainLiteral("Marks the pizzas that cost more than 10"), antecedent, consequent);
        ontology.Rules.Add(rule);

        string document = string.Empty;
        List<string> warnings = CaptureWarnings(() => document = OWLManchesterSerializer.SerializeOntology(ontology));
        OWLOntology reparsedOntology = OWLManchesterParser.DeserializeOntology(document);

        Assert.HasCount(0, warnings);
        Assert.IsTrue(document.Contains("Rule:", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:Pizza(?X), pz:hasTopping(?X, ?Y), pz:price(?X, ?P), swrlb:greaterThan(?P, \"10\"^^xsd:integer) -> pz:Expensive(?X)", StringComparison.Ordinal));
        Assert.HasCount(1, reparsedOntology.Rules);
        Assert.HasCount(2, reparsedOntology.Rules[0].Annotations);
        Assert.AreEqual("expensive-pizza", reparsedOntology.Rules[0].Annotations[0].ValueLiteral.Value);
        Assert.AreEqual("Marks the pizzas that cost more than 10", reparsedOntology.Rules[0].Annotations[1].ValueLiteral.Value);
        Assert.AreEqual(rule.GetXML(), reparsedOntology.Rules[0].GetXML());
    }

    //Verifies that equality atoms and individual or literal arguments are written and read back unchanged
    [TestMethod]
    public void ShouldRoundTripEqualityAtomsAndConstantArguments()
    {
        OWLOntology ontology = CreateOntologyWithEntities();
        SWRLAntecedent antecedent = new SWRLAntecedent();
        antecedent.Atoms.Add(new SWRLSameIndividualAtom(Variable("x"), Variable("y")));
        antecedent.Atoms.Add(new SWRLDifferentIndividualsAtom(Variable("x"), new SWRLIndividualArgument(new RDFResource("http://example.org/pz#Margherita"))));
        antecedent.Atoms.Add(new SWRLDataPropertyAtom { Predicate = DataProperty("price"), LeftArgument = Variable("x"), RightArgument = new SWRLLiteralArgument(new RDFPlainLiteral("cheap \"deal\"", "en")) });
        SWRLConsequent consequent = new SWRLConsequent();
        consequent.Atoms.Add(new SWRLClassAtom(Class("Pizza"), Variable("y")));
        SWRLRule rule = new SWRLRule(new RDFPlainLiteral("r"), new RDFPlainLiteral("d"), antecedent, consequent);
        ontology.Rules.Add(rule);

        string document = OWLManchesterSerializer.SerializeOntology(ontology);
        OWLOntology reparsedOntology = OWLManchesterParser.DeserializeOntology(document);

        Assert.IsTrue(document.Contains("sameAs(?X, ?Y), differentFrom(?X, pz:Margherita)", StringComparison.Ordinal));
        Assert.HasCount(1, reparsedOntology.Rules);
        Assert.AreEqual(rule.GetXML(), reparsedOntology.Rules[0].GetXML());
    }

    //Verifies that variables are written the way the object model names them (RDFSharp upper-cases them) and that a rule follows the frames and the misc sections
    [TestMethod]
    public void ShouldWriteRuleAfterFramesAndMiscSections()
    {
        OWLOntology ontology = CreateOntologyWithEntities();
        ontology.ClassAxioms.Add(new OWLEquivalentClasses([Class("Pizza"), Class("Expensive")]));
        SWRLAntecedent antecedent = new SWRLAntecedent();
        antecedent.Atoms.Add(new SWRLClassAtom(Class("Pizza"), Variable("Pizza_X")));
        SWRLConsequent consequent = new SWRLConsequent();
        consequent.Atoms.Add(new SWRLClassAtom(Class("Expensive"), Variable("Pizza_X")));
        ontology.Rules.Add(new SWRLRule(new RDFPlainLiteral("r"), new RDFPlainLiteral("d"), antecedent, consequent));

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("pz:Pizza(?PIZZA_X) -> pz:Expensive(?PIZZA_X)", StringComparison.Ordinal));
        Assert.IsTrue(document.IndexOf("EquivalentClasses:", StringComparison.Ordinal) < document.IndexOf("Rule:", StringComparison.Ordinal));
        Assert.IsTrue(document.IndexOf("Individual:", StringComparison.Ordinal) < document.IndexOf("Rule:", StringComparison.Ordinal));
    }

    //Verifies that a rule with a complex class expression in a class atom is skipped with a warning that names the other syntaxes
    [TestMethod]
    public void ShouldWarnAndSkipRuleWithComplexClassAtom()
    {
        OWLOntology ontology = CreateOntologyWithEntities();
        SWRLAntecedent antecedent = new SWRLAntecedent();
        antecedent.Atoms.Add(new SWRLClassAtom(new OWLObjectSomeValuesFrom(ObjectProperty("hasTopping"), Class("Pizza")), Variable("x")));
        SWRLConsequent consequent = new SWRLConsequent();
        consequent.Atoms.Add(new SWRLClassAtom(Class("Expensive"), Variable("x")));
        ontology.Rules.Add(new SWRLRule(new RDFPlainLiteral("r"), new RDFPlainLiteral("d"), antecedent, consequent));

        string document = string.Empty;
        List<string> warnings = CaptureWarnings(() => document = OWLManchesterSerializer.SerializeOntology(ontology));
        OWLOntology reparsedOntology = OWLManchesterParser.DeserializeOntology(document);

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("SWRL", StringComparison.Ordinal));
        Assert.IsTrue(warnings[0].Contains("Functional", StringComparison.Ordinal));
        Assert.IsTrue(warnings[0].Contains("OWL2/XML", StringComparison.Ordinal));
        Assert.IsFalse(document.Contains("Rule:", StringComparison.Ordinal));
        Assert.HasCount(0, reparsedOntology.Rules);
    }

    //Verifies that a rule with a complex data range in a data range atom is skipped with a warning
    [TestMethod]
    public void ShouldWarnAndSkipRuleWithComplexDataRangeAtom()
    {
        OWLOntology ontology = CreateOntologyWithEntities();
        SWRLAntecedent antecedent = new SWRLAntecedent();
        antecedent.Atoms.Add(new SWRLDataRangeAtom(
            new OWLDataUnionOf([ new OWLDatatype(RDFVocabulary.XSD.INTEGER), new OWLDatatype(RDFVocabulary.XSD.STRING) ]), Variable("p")));
        SWRLConsequent consequent = new SWRLConsequent();
        consequent.Atoms.Add(new SWRLClassAtom(Class("Expensive"), Variable("x")));
        ontology.Rules.Add(new SWRLRule(new RDFPlainLiteral("r"), new RDFPlainLiteral("d"), antecedent, consequent));

        string document = string.Empty;
        List<string> warnings = CaptureWarnings(() => document = OWLManchesterSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsFalse(document.Contains("Rule:", StringComparison.Ordinal));
    }

    //Verifies that a rule with a named datatype in a data range atom is written and read back
    [TestMethod]
    public void ShouldRoundTripRuleWithNamedDataRangeAtom()
    {
        OWLOntology ontology = CreateOntologyWithEntities();
        SWRLAntecedent antecedent = new SWRLAntecedent();
        antecedent.Atoms.Add(new SWRLDataPropertyAtom(DataProperty("price"), Variable("x"), Variable("p")));
        antecedent.Atoms.Add(new SWRLDataRangeAtom(new OWLDatatype(RDFVocabulary.XSD.INTEGER), Variable("p")));
        SWRLConsequent consequent = new SWRLConsequent();
        consequent.Atoms.Add(new SWRLClassAtom(Class("Expensive"), Variable("x")));
        SWRLRule rule = new SWRLRule(new RDFPlainLiteral("r"), new RDFPlainLiteral("d"), antecedent, consequent);
        ontology.Rules.Add(rule);

        OWLOntology reparsedOntology = OWLManchesterParser.DeserializeOntology(OWLManchesterSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, reparsedOntology.Rules);
        Assert.AreEqual(rule.GetXML(), reparsedOntology.Rules[0].GetXML());
    }
    #endregion
}
