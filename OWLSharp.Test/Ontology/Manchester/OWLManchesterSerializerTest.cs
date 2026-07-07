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

namespace OWLSharp.Test.Ontology.Manchester;

/// <summary>
/// Exercises OWLManchesterSerializer.SerializeOntology directly: document structure (prefixes, header,
/// frame grouping/ordering, misc sections), the representable OWL2 constructs it renders, and the
/// warning+skip policy for the OWL2 constructs that OWLSharp supports but Manchester cannot express
/// (the format is lossy by design, like JPEG for images)
/// </summary>
[TestClass]
public class OWLManchesterSerializerTest
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

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("Prefix: pz: <http://example.org/pz#>", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Ontology: <http://example.org/pz> <http://example.org/pz/1.0>", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Import: <http://example.org/base>", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Annotations: rdfs:comment \"a comment\"", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldSerializeOntologyWithoutIRIAsAnonymousHeader()
    {
        OWLOntology ontology = new OWLOntology();

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("Ontology:\n", StringComparison.Ordinal) || document.Contains("Ontology:\r\n", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldOrderFramesByKindRegardlessOfAxiomVisitOrder()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        OWLDataProperty hasCalories = new OWLDataProperty(new RDFResource("http://example.org/pz#hasCalories"));
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        OWLDatatype positiveInt = new OWLDatatype(new RDFResource("http://example.org/pz#PositiveInt"));
        OWLNamedIndividual margherita = new OWLNamedIndividual(new RDFResource("http://example.org/pz#Margherita"));

        //Declared in an order deliberately different from the FrameKeywords emission order
        ontology.DeclarationAxioms.AddRange(
        [ new OWLDeclaration(margherita), new OWLDeclaration(pizza), new OWLDeclaration(hasCalories),
          new OWLDeclaration(hasTopping), new OWLDeclaration(note), new OWLDeclaration(positiveInt) ]);

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        //Emission order is always: Datatype, AnnotationProperty, ObjectProperty, DataProperty, Class, Individual
        int datatypeIdx = document.IndexOf("Datatype: pz:PositiveInt", StringComparison.Ordinal);
        int annPropIdx = document.IndexOf("AnnotationProperty: pz:note", StringComparison.Ordinal);
        int objPropIdx = document.IndexOf("ObjectProperty: pz:hasTopping", StringComparison.Ordinal);
        int dtPropIdx = document.IndexOf("DataProperty: pz:hasCalories", StringComparison.Ordinal);
        int classIdx = document.IndexOf("Class: pz:Pizza", StringComparison.Ordinal);
        int idvIdx = document.IndexOf("Individual: pz:Margherita", StringComparison.Ordinal);

        Assert.IsTrue(datatypeIdx < annPropIdx);
        Assert.IsTrue(annPropIdx < objPropIdx);
        Assert.IsTrue(objPropIdx < dtPropIdx);
        Assert.IsTrue(dtPropIdx < classIdx);
        Assert.IsTrue(classIdx < idvIdx);
    }

    [TestMethod]
    public void ShouldGroupMultipleAxiomsUnderTheSameFrameSection()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLClass topping = new OWLClass(new RDFResource("http://example.org/pz#Topping"));
        OWLClass food = new OWLClass(new RDFResource("http://example.org/pz#Food"));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(pizza));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, topping));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, food));

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("SubClassOf: pz:Topping, pz:Food", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldEmitMiscSectionsAfterEntityFrames()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLClass a = new OWLClass(new RDFResource("http://example.org/pz#A"));
        OWLClass b = new OWLClass(new RDFResource("http://example.org/pz#B"));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(a));
        ontology.ClassAxioms.Add(new OWLDisjointClasses([ a, b ]));

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.IndexOf("Class: pz:A", StringComparison.Ordinal) < document.IndexOf("DisjointClasses:", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldAttachEntityAnnotationToItsDeclaredFrame()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(pizza));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), pizza.GetIRI(), new OWLLiteral(new RDFPlainLiteral("Pizza"))));

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        Assert.IsTrue(document.Contains("Class: pz:Pizza\n    Annotations: rdfs:label \"Pizza\"", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipEntityAnnotationOnUndeclaredSubject()
    {
        OWLOntology ontology = CreateBaseOntology();
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion(
            new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new RDFResource("http://example.org/pz#Ghost"), new OWLLiteral(new RDFPlainLiteral("Ghost"))));

        List<string> warnings = CaptureWarnings(() => OWLManchesterSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("not a declared entity", StringComparison.Ordinal));
    }
    #endregion

    #region Tests (representable constructs)
    [TestMethod]
    public void ShouldSerializeComprehensiveRepresentableOntology()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLClass topping = new OWLClass(new RDFResource("http://example.org/pz#Topping"));
        OWLClass spicy = new OWLClass(new RDFResource("http://example.org/pz#SpicyTopping"));
        OWLClass vegPizza = new OWLClass(new RDFResource("http://example.org/pz#VegetarianPizza"));
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        OWLObjectProperty isToppingOf = new OWLObjectProperty(new RDFResource("http://example.org/pz#isToppingOf"));
        OWLDataProperty hasCalories = new OWLDataProperty(new RDFResource("http://example.org/pz#hasCalories"));
        OWLAnnotationProperty note = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        OWLDatatype positiveInt = new OWLDatatype(new RDFResource("http://example.org/pz#PositiveInt"));
        OWLNamedIndividual margherita = new OWLNamedIndividual(new RDFResource("http://example.org/pz#Margherita"));
        OWLNamedIndividual mozzarella = new OWLNamedIndividual(new RDFResource("http://example.org/pz#Mozzarella"));

        ontology.DeclarationAxioms.AddRange(
        [ new OWLDeclaration(pizza), new OWLDeclaration(topping), new OWLDeclaration(spicy), new OWLDeclaration(vegPizza),
          new OWLDeclaration(hasTopping), new OWLDeclaration(isToppingOf), new OWLDeclaration(hasCalories),
          new OWLDeclaration(note), new OWLDeclaration(positiveInt), new OWLDeclaration(margherita), new OWLDeclaration(mozzarella) ]);

        //Class expressions
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectSomeValuesFrom(hasTopping, topping)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(topping, new OWLObjectAllValuesFrom(hasTopping, spicy)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(spicy, new OWLObjectHasValue(hasTopping, mozzarella)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(vegPizza, new OWLObjectHasSelf(hasTopping)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectMinCardinality(hasTopping, 1)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectMaxCardinality(hasTopping, 5, topping)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectExactCardinality(hasTopping, 2)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectOneOf([ margherita ])));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectIntersectionOf([ topping, spicy ])));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectUnionOf([ topping, spicy ])));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectComplementOf(spicy)));
        ontology.ClassAxioms.Add(new OWLEquivalentClasses([ vegPizza, pizza ]));
        ontology.ClassAxioms.Add(new OWLDisjointClasses([ pizza, topping ]));
        ontology.ClassAxioms.Add(new OWLDisjointUnion(pizza, [ vegPizza, spicy ]));

        //Data ranges
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLDataSomeValuesFrom(hasCalories,
            new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("0", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_INCLUSIVE) ]))));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLDataAllValuesFrom(hasCalories, new OWLDataUnionOf(
        [ new OWLDatatype(RDFVocabulary.XSD.INTEGER), new OWLDatatype(RDFVocabulary.XSD.DECIMAL) ]))));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLDataHasValue(hasCalories,
            new OWLLiteral(new RDFTypedLiteral("850", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLDataMinCardinality(hasCalories, 1)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLDataSomeValuesFrom(hasCalories,
            new OWLDataOneOf([ new OWLLiteral(new RDFPlainLiteral("a")), new OWLLiteral(new RDFPlainLiteral("b")) ]))));

        //ObjectProperty frame sections
        ontology.ObjectPropertyAxioms.Add(new OWLObjectPropertyDomain(hasTopping, pizza));
        ontology.ObjectPropertyAxioms.Add(new OWLObjectPropertyRange(hasTopping, topping));
        ontology.ObjectPropertyAxioms.Add(new OWLFunctionalObjectProperty(hasTopping));
        ontology.ObjectPropertyAxioms.Add(new OWLInverseFunctionalObjectProperty(isToppingOf));
        ontology.ObjectPropertyAxioms.Add(new OWLTransitiveObjectProperty(isToppingOf));
        ontology.ObjectPropertyAxioms.Add(new OWLSymmetricObjectProperty(isToppingOf));
        ontology.ObjectPropertyAxioms.Add(new OWLAsymmetricObjectProperty(hasTopping));
        ontology.ObjectPropertyAxioms.Add(new OWLReflexiveObjectProperty(hasTopping));
        ontology.ObjectPropertyAxioms.Add(new OWLIrreflexiveObjectProperty(isToppingOf));
        ontology.ObjectPropertyAxioms.Add(new OWLInverseObjectProperties(hasTopping, isToppingOf));
        ontology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(new OWLObjectPropertyChain([ hasTopping, hasTopping ]), hasTopping));

        //DataProperty frame sections
        ontology.DataPropertyAxioms.Add(new OWLDataPropertyDomain(hasCalories, pizza));
        ontology.DataPropertyAxioms.Add(new OWLDataPropertyRange(hasCalories, new OWLDatatype(RDFVocabulary.XSD.INTEGER)));
        ontology.DataPropertyAxioms.Add(new OWLFunctionalDataProperty(hasCalories));

        //AnnotationProperty frame sections
        ontology.AnnotationAxioms.Add(new OWLAnnotationPropertyDomain(note, pizza.GetIRI()));
        ontology.AnnotationAxioms.Add(new OWLAnnotationPropertyRange(note, pizza.GetIRI()));

        //Datatype frame
        ontology.DatatypeDefinitionAxioms.Add(new OWLDatatypeDefinition(positiveInt,
            new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("0", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_INCLUSIVE) ])));

        //HasKey
        ontology.KeyAxioms.Add(new OWLHasKey(pizza, [ hasTopping ]) { DataProperties = [ hasCalories ] });

        //Assertions
        ontology.AssertionAxioms.Add(new OWLClassAssertion(pizza, margherita));
        ontology.AssertionAxioms.Add(new OWLObjectPropertyAssertion(hasTopping, margherita, mozzarella));
        ontology.AssertionAxioms.Add(new OWLNegativeObjectPropertyAssertion(hasTopping, mozzarella, margherita));
        ontology.AssertionAxioms.Add(new OWLDataPropertyAssertion(hasCalories, margherita, new OWLLiteral(new RDFTypedLiteral("850", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        ontology.AssertionAxioms.Add(new OWLNegativeDataPropertyAssertion(hasCalories, mozzarella, new OWLLiteral(new RDFTypedLiteral("0", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        ontology.AssertionAxioms.Add(new OWLSameIndividual([ margherita, mozzarella ]));
        ontology.AssertionAxioms.Add(new OWLDifferentIndividuals([ margherita, mozzarella ]));

        string document = OWLManchesterSerializer.SerializeOntology(ontology);

        //Class expressions
        Assert.IsTrue(document.Contains("pz:hasTopping some pz:Topping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:hasTopping only pz:SpicyTopping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:hasTopping value pz:Mozzarella", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:hasTopping Self", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:hasTopping min 1", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:hasTopping max 5 pz:Topping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:hasTopping exactly 2", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("{pz:Margherita}", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:Topping and pz:SpicyTopping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:Topping or pz:SpicyTopping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("not pz:SpicyTopping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("EquivalentClasses: pz:VegetarianPizza, pz:Pizza", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("DisjointClasses: pz:Pizza, pz:Topping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("DisjointUnionOf: pz:VegetarianPizza, pz:SpicyTopping", StringComparison.Ordinal));

        //Data ranges
        Assert.IsTrue(document.Contains("xsd:integer[>= \"0\"^^xsd:integer]", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("xsd:integer or xsd:decimal", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("pz:hasCalories value \"850\"^^xsd:integer", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("{\"a\", \"b\"}", StringComparison.Ordinal));

        //ObjectProperty sections
        Assert.IsTrue(document.Contains("Domain: pz:Pizza", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Range: pz:Topping", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Functional", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("InverseFunctional", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Transitive", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Symmetric", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Asymmetric", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Reflexive", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Irreflexive", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("InverseOf: pz:isToppingOf", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("SubPropertyChain: pz:hasTopping o pz:hasTopping", StringComparison.Ordinal));

        //DataProperty/AnnotationProperty/Datatype sections
        Assert.IsTrue(document.Contains("DataProperty: pz:hasCalories", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("AnnotationProperty: pz:note", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Datatype: pz:PositiveInt", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("HasKey: pz:hasTopping pz:hasCalories", StringComparison.Ordinal));

        //Assertions
        Assert.IsTrue(document.Contains("Types: pz:Pizza", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("Facts: pz:hasTopping pz:Mozzarella", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("not pz:hasTopping pz:Margherita", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("not pz:hasCalories \"0\"^^xsd:integer", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("SameIndividual: pz:Margherita, pz:Mozzarella", StringComparison.Ordinal));
        Assert.IsTrue(document.Contains("DifferentIndividuals: pz:Margherita, pz:Mozzarella", StringComparison.Ordinal));

        //No warnings should be raised: every construct above is representable
        List<string> warnings = CaptureWarnings(() => OWLManchesterSerializer.SerializeOntology(ontology));
        Assert.IsEmpty(warnings);
    }

    [TestMethod]
    public void ShouldNormalizeObjectPropertyAssertionWithInverseIntoSwappedNamedForm()
    {
        //ObjectInverseOf in an assertion is normalized (swap individuals, use the named property)
        //rather than dropped, since a semantically equivalent Manchester form exists
        OWLOntology ontology = CreateBaseOntology();
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        OWLNamedIndividual margherita = new OWLNamedIndividual(new RDFResource("http://example.org/pz#Margherita"));
        OWLNamedIndividual mozzarella = new OWLNamedIndividual(new RDFResource("http://example.org/pz#Mozzarella"));
        ontology.AssertionAxioms.Add(new OWLObjectPropertyAssertion(new OWLObjectInverseOf(hasTopping), margherita, mozzarella));

        List<string> warnings = CaptureWarnings(() =>
        {
            string document = OWLManchesterSerializer.SerializeOntology(ontology);
            Assert.IsTrue(document.Contains("Facts: pz:hasTopping pz:Margherita", StringComparison.Ordinal));
        });

        Assert.IsEmpty(warnings);
    }
    #endregion

    #region Tests (lossy constructs: warning + skip)
    [TestMethod]
    public void ShouldWarnAndSkipGeneralClassInclusionAxiom()
    {
        //A GCI (anonymous class expression on the left of SubClassOf) has no owning Manchester frame
        OWLOntology ontology = CreateBaseOntology();
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        ontology.ClassAxioms.Add(new OWLSubClassOf(new OWLObjectHasSelf(hasTopping), pizza));

        List<string> warnings = CaptureWarnings(() =>
        {
            string document = OWLManchesterSerializer.SerializeOntology(ontology);
            Assert.IsFalse(document.Contains("SubClassOf:", StringComparison.Ordinal));
        });

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLSubClassOf", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipHasKeyOnAnonymousClassExpression()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLClass topping = new OWLClass(new RDFResource("http://example.org/pz#Topping"));
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        ontology.KeyAxioms.Add(new OWLHasKey(new OWLObjectIntersectionOf([ pizza, topping ]), [ hasTopping ]));

        List<string> warnings = CaptureWarnings(() =>
        {
            string document = OWLManchesterSerializer.SerializeOntology(ontology);
            Assert.IsFalse(document.Contains("HasKey:", StringComparison.Ordinal));
        });

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLHasKey", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipCharacteristicOnAnonymousObjectProperty()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        ontology.ObjectPropertyAxioms.Add(new OWLFunctionalObjectProperty(new OWLObjectInverseOf(hasTopping)));

        List<string> warnings = CaptureWarnings(() =>
        {
            string document = OWLManchesterSerializer.SerializeOntology(ontology);
            Assert.IsFalse(document.Contains("Functional", StringComparison.Ordinal));
        });

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLFunctionalObjectProperty", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipDomainAndRangeOnAnonymousObjectProperty()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        ontology.ObjectPropertyAxioms.Add(new OWLObjectPropertyDomain(new OWLObjectInverseOf(hasTopping), pizza));
        ontology.ObjectPropertyAxioms.Add(new OWLObjectPropertyRange(new OWLObjectInverseOf(hasTopping), pizza));

        List<string> warnings = CaptureWarnings(() => OWLManchesterSerializer.SerializeOntology(ontology));

        Assert.HasCount(2, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLObjectPropertyDomain", StringComparison.Ordinal));
        Assert.IsTrue(warnings[1].Contains("OWLObjectPropertyRange", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipInverseObjectPropertiesWhenBothSidesAreAnonymous()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        OWLObjectProperty isToppingOf = new OWLObjectProperty(new RDFResource("http://example.org/pz#isToppingOf"));
        ontology.ObjectPropertyAxioms.Add(new OWLInverseObjectProperties(new OWLObjectInverseOf(hasTopping), new OWLObjectInverseOf(isToppingOf)));

        List<string> warnings = CaptureWarnings(() =>
        {
            string document = OWLManchesterSerializer.SerializeOntology(ontology);
            Assert.IsFalse(document.Contains("InverseOf:", StringComparison.Ordinal));
        });

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLInverseObjectProperties", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipSubObjectPropertyOfWithAnonymousSubProperty()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        ontology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(new OWLObjectInverseOf(hasTopping), hasTopping));

        List<string> warnings = CaptureWarnings(() => OWLManchesterSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLSubObjectPropertyOf", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipSubPropertyChainWithAnonymousSuperProperty()
    {
        OWLOntology ontology = CreateBaseOntology();
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        ontology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(
            new OWLObjectPropertyChain([ hasTopping, hasTopping ]), new OWLObjectInverseOf(hasTopping)));

        List<string> warnings = CaptureWarnings(() => OWLManchesterSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLSubObjectPropertyOf", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipSWRLRules()
    {
        OWLOntology ontology = CreateBaseOntology();
        ontology.Rules.Add(new SWRLRule(
            new RDFPlainLiteral("test-rule"), new RDFPlainLiteral("a test rule"), new SWRLAntecedent(), new SWRLConsequent()));

        List<string> warnings = CaptureWarnings(() => OWLManchesterSerializer.SerializeOntology(ontology));

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("SWRL", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ShouldWarnAndSkipAnnotationAssertionWithoutRepresentableValue()
    {
        //Neither ValueIRI nor ValueLiteral set: reachable only through the internal parameterless
        //constructor (e.g: an anonymous-individual annotation value has no public constructor either,
        //since Manchester/OWL2-XML both lack a way to model it uniformly across the object model)
        OWLOntology ontology = CreateBaseOntology();
        OWLClass pizza = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        ontology.DeclarationAxioms.Add(new OWLDeclaration(pizza));
        ontology.AnnotationAxioms.Add(new OWLAnnotationAssertion
        {
            AnnotationProperty = new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
            SubjectIRI = pizza.GetIRI().ToString()
        });

        List<string> warnings = CaptureWarnings(() =>
        {
            string document = OWLManchesterSerializer.SerializeOntology(ontology);
            Assert.IsFalse(document.Contains("Annotations:", StringComparison.Ordinal));
        });

        Assert.HasCount(1, warnings);
        Assert.IsTrue(warnings[0].Contains("OWLAnnotationAssertion", StringComparison.Ordinal));
    }
    #endregion
}
