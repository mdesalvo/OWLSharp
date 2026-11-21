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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology;

/// <summary>
/// Golden round-trip test: builds a single ontology exercising the same families of constructs as the
/// Functional-Style round-trip test plus a SWRL rule and axioms on inverse properties, writes it to
/// OWL2/Manchester, reads the document back and compares the result axiom by axiom. The axioms on
/// inverse properties are compared with the equivalent form on the named property, which is the way
/// Manchester can express them. Its purpose is to catch integration-level mismatches between the
/// Manchester serializer and parser that the dedicated suites, each covering one side, would not surface.
/// </summary>
[TestClass]
public class OWLManchesterRoundTripTest
{
    #region Utilities
    private static readonly OWLAnnotation KeptAnnotation =
        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), new OWLLiteral(new RDFPlainLiteral("kept")));

    private static OWLClass Class(string localName)
        => new OWLClass(new RDFResource($"http://example.org/pz#{localName}"));

    private static OWLObjectProperty ObjectProperty(string localName)
        => new OWLObjectProperty(new RDFResource($"http://example.org/pz#{localName}"));

    private static OWLDataProperty DataProperty(string localName)
        => new OWLDataProperty(new RDFResource($"http://example.org/pz#{localName}"));

    private static OWLNamedIndividual Individual(string localName)
        => new OWLNamedIndividual(new RDFResource($"http://example.org/pz#{localName}"));

    //Builds the annotated copy of an axiom, so that the original and the expected ontology never share the same instance
    private static T Annotated<T>(T axiom) where T : OWLAxiom
    {
        axiom.Annotate(KeptAnnotation);
        return axiom;
    }

    //Fills the given ontology with the axioms that Manchester represents as they are
    private static void AddRepresentableAxioms(OWLOntology ontology)
    {
        OWLClass pizzaClass = Class("Pizza");
        OWLClass toppingClass = Class("Topping");
        OWLClass margheritaClass = Class("Margherita");
        OWLClass spicyClass = Class("Spicy");
        OWLObjectProperty hasToppingProperty = ObjectProperty("hasTopping");
        OWLObjectProperty isToppingOfProperty = ObjectProperty("isToppingOf");
        OWLObjectProperty hasBaseToppingProperty = ObjectProperty("hasBaseTopping");
        OWLDataProperty hasNameProperty = DataProperty("hasName");
        OWLDataProperty hasCaloriesProperty = DataProperty("hasCalories");
        OWLAnnotationProperty noteProperty = new OWLAnnotationProperty(new RDFResource("http://example.org/pz#note"));
        OWLDatatype positiveIntegerDatatype = new OWLDatatype(new RDFResource("http://example.org/pz#PositiveInteger"));
        OWLNamedIndividual peterIndividual = Individual("Peter");
        OWLNamedIndividual mozzarellaIndividual = Individual("Mozzarella");

        ontology.DeclarationAxioms.AddRange(
        [
            new OWLDeclaration(pizzaClass), new OWLDeclaration(toppingClass), new OWLDeclaration(margheritaClass), new OWLDeclaration(spicyClass),
            new OWLDeclaration(hasToppingProperty), new OWLDeclaration(isToppingOfProperty), new OWLDeclaration(hasBaseToppingProperty),
            new OWLDeclaration(hasNameProperty), new OWLDeclaration(hasCaloriesProperty), new OWLDeclaration(noteProperty),
            new OWLDeclaration(positiveIntegerDatatype), new OWLDeclaration(peterIndividual), new OWLDeclaration(mozzarellaIndividual)
        ]);

        //Composite class expressions and cardinality restrictions with and without qualification
        ontology.ClassAxioms.Add(new OWLSubClassOf(margheritaClass, new OWLObjectSomeValuesFrom(hasToppingProperty, toppingClass)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(toppingClass, new OWLObjectAllValuesFrom(hasToppingProperty, spicyClass)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(spicyClass, new OWLObjectHasValue(hasToppingProperty, mozzarellaIndividual)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLObjectMinCardinality(hasToppingProperty, 1, toppingClass)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLObjectMaxCardinality(hasToppingProperty, 5)));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLObjectUnionOf([ toppingClass, new OWLObjectComplementOf(spicyClass) ])));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLObjectOneOf([ peterIndividual, mozzarellaIndividual ])));
        ontology.ClassAxioms.Add(Annotated(new OWLEquivalentClasses([ margheritaClass, new OWLObjectIntersectionOf([ pizzaClass, new OWLObjectMinCardinality(hasToppingProperty, 1, toppingClass) ]) ])));
        ontology.ClassAxioms.Add(new OWLDisjointClasses([ pizzaClass, toppingClass ]));
        ontology.ClassAxioms.Add(new OWLDisjointUnion(pizzaClass, [ margheritaClass, spicyClass ]));

        //Data ranges with facets and enumerations
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLDataSomeValuesFrom(hasNameProperty,
            new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.STRING), [ new OWLFacetRestriction(new OWLLiteral(new RDFPlainLiteral("1")), RDFVocabulary.XSD.MIN_LENGTH) ]))));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLDataAllValuesFrom(hasCaloriesProperty,
            new OWLDataUnionOf([ new OWLDatatype(RDFVocabulary.XSD.INTEGER), new OWLDatatype(RDFVocabulary.XSD.DECIMAL) ]))));
        ontology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLDataHasValue(hasCaloriesProperty,
            new OWLLiteral(new RDFTypedLiteral("850", RDFModelEnums.RDFDatatypes.XSD_INTEGER)))));
        ontology.DatatypeDefinitionAxioms.Add(new OWLDatatypeDefinition(positiveIntegerDatatype,
            new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.INTEGER),
                [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)), RDFVocabulary.XSD.MIN_INCLUSIVE) ])));

        //Object property frames: chain (second syntactic form of SubObjectPropertyOf) and characteristics
        ontology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(new OWLObjectPropertyChain([ hasToppingProperty, hasBaseToppingProperty ]), hasToppingProperty));
        ontology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(hasBaseToppingProperty, hasToppingProperty));
        ontology.ObjectPropertyAxioms.Add(new OWLFunctionalObjectProperty(hasBaseToppingProperty));
        ontology.ObjectPropertyAxioms.Add(new OWLTransitiveObjectProperty(hasToppingProperty));
        ontology.ObjectPropertyAxioms.Add(new OWLObjectPropertyDomain(hasToppingProperty, pizzaClass));
        ontology.ObjectPropertyAxioms.Add(new OWLObjectPropertyRange(hasToppingProperty, toppingClass));
        ontology.ObjectPropertyAxioms.Add(new OWLInverseObjectProperties(hasToppingProperty, isToppingOfProperty));
        ontology.ObjectPropertyAxioms.Add(new OWLEquivalentObjectProperties([ hasBaseToppingProperty, new OWLObjectInverseOf(isToppingOfProperty) ]));
        ontology.ObjectPropertyAxioms.Add(new OWLDisjointObjectProperties([ hasBaseToppingProperty, isToppingOfProperty ]));

        //Data property frames
        ontology.DataPropertyAxioms.Add(new OWLFunctionalDataProperty(hasNameProperty));
        ontology.DataPropertyAxioms.Add(new OWLDataPropertyDomain(hasNameProperty, pizzaClass));
        ontology.DataPropertyAxioms.Add(new OWLDataPropertyRange(hasCaloriesProperty, new OWLDatatype(RDFVocabulary.XSD.INTEGER)));

        //HasKey with both an object and a data property in its two lists
        ontology.KeyAxioms.Add(new OWLHasKey(pizzaClass, [ hasToppingProperty ]) { DataProperties = [ hasNameProperty ] });

        //Assertions, including negative ones and same/different groups
        ontology.AssertionAxioms.Add(new OWLClassAssertion(margheritaClass, peterIndividual));
        ontology.AssertionAxioms.Add(new OWLObjectPropertyAssertion(hasToppingProperty, peterIndividual, mozzarellaIndividual));
        ontology.AssertionAxioms.Add(new OWLNegativeObjectPropertyAssertion(hasToppingProperty, mozzarellaIndividual, peterIndividual));
        ontology.AssertionAxioms.Add(new OWLDataPropertyAssertion(hasNameProperty, peterIndividual, new OWLLiteral(new RDFPlainLiteral("Peter's pizza"))));
        ontology.AssertionAxioms.Add(new OWLNegativeDataPropertyAssertion(hasNameProperty, peterIndividual, new OWLLiteral(new RDFPlainLiteral("Wrong name"))));
        ontology.AssertionAxioms.Add(new OWLDataPropertyAssertion(hasCaloriesProperty, peterIndividual, new OWLLiteral(new RDFTypedLiteral("-850", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        ontology.AssertionAxioms.Add(new OWLDifferentIndividuals([ peterIndividual, mozzarellaIndividual ]));
        ontology.AssertionAxioms.Add(new OWLSameIndividual([ peterIndividual, Individual("Pete") ]));

        //Annotation axioms, one of which carries a nested meta-annotation
        OWLAnnotationAssertion commentAssertion = new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), pizzaClass.GetIRI(), new OWLLiteral(new RDFPlainLiteral("A dish")));
        commentAssertion.Annotate(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new OWLLiteral(new RDFPlainLiteral("meta"))));
        ontology.AnnotationAxioms.Add(commentAssertion);
        ontology.AnnotationAxioms.Add(new OWLAnnotationPropertyDomain(noteProperty, pizzaClass.GetIRI()));
        ontology.AnnotationAxioms.Add(new OWLAnnotationPropertyRange(noteProperty, toppingClass.GetIRI()));

        //SWRL rule exercising every kind of argument and a built-in
        SWRLAntecedent antecedent = new SWRLAntecedent();
        antecedent.Atoms.Add(new SWRLClassAtom(pizzaClass, new SWRLVariableArgument(new RDFVariable("?x"))));
        antecedent.Atoms.Add(new SWRLObjectPropertyAtom(hasToppingProperty, new SWRLVariableArgument(new RDFVariable("?x")), new SWRLVariableArgument(new RDFVariable("?y"))));
        antecedent.Atoms.Add(new SWRLDataPropertyAtom(hasCaloriesProperty, new SWRLVariableArgument(new RDFVariable("?x")), new SWRLVariableArgument(new RDFVariable("?c"))));
        antecedent.Atoms.Add(new SWRLDifferentIndividualsAtom(new SWRLVariableArgument(new RDFVariable("?x")), new SWRLIndividualArgument(peterIndividual.GetIRI())));
        antecedent.BuiltIns.Add(SWRLBuiltIn.GreaterThan(new SWRLVariableArgument(new RDFVariable("?c")), new SWRLLiteralArgument(new RDFTypedLiteral("500", RDFModelEnums.RDFDatatypes.XSD_INTEGER))));
        SWRLConsequent consequent = new SWRLConsequent();
        consequent.Atoms.Add(new SWRLClassAtom(spicyClass, new SWRLVariableArgument(new RDFVariable("?y"))));
        ontology.Rules.Add(new SWRLRule(new RDFPlainLiteral("heavy-pizza"), new RDFPlainLiteral("The toppings of a heavy pizza are spicy"), antecedent, consequent));
    }

    //Creates an empty ontology with the header that both the original and the expected ontology share
    private static OWLOntology CreateOntology()
    {
        OWLOntology ontology = new OWLOntology(new Uri("http://example.org/pz"), new Uri("http://example.org/pz/1.0"));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("swrlb", "http://www.w3.org/2003/11/swrlb#")));
        ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/imported")));
        ontology.Annotations.Add(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new OWLLiteral(new RDFPlainLiteral("Pizza ontology"))));
        return ontology;
    }

    //Gets the axioms of a registry as sorted XML texts, so that registries can be compared regardless of their order
    private static List<string> SortedXml<T>(List<T> axioms) where T : OWLAxiom
        => axioms.Select(axiom => axiom.GetXML()).OrderBy(xml => xml, StringComparer.Ordinal).ToList();
    #endregion

    #region Tests
    //Verifies that a rich ontology survives the Manchester round-trip, with the axioms on inverse properties in their rewritten form
    [TestMethod]
    public void ShouldRoundTripRichOntologyThroughManchesterSyntax()
    {
        OWLObjectProperty hasToppingProperty = ObjectProperty("hasTopping");
        OWLObjectProperty isToppingOfProperty = ObjectProperty("isToppingOf");
        OWLObjectProperty hasPartProperty = ObjectProperty("hasPart");
        OWLObjectProperty isPartOfProperty = ObjectProperty("isPartOf");
        OWLClass pizzaClass = Class("Pizza");

        OWLOntology originalOntology = CreateOntology();
        AddRepresentableAxioms(originalOntology);
        OWLOntology expectedOntology = CreateOntology();
        AddRepresentableAxioms(expectedOntology);
        foreach (OWLObjectProperty property in new[] { hasPartProperty, isPartOfProperty })
        {
            originalOntology.DeclarationAxioms.Add(new OWLDeclaration(property));
            expectedOntology.DeclarationAxioms.Add(new OWLDeclaration(property));
        }

        //Axioms on "inverse P": each one is added to the original ontology as written and to the expected one in its rewritten form
        originalOntology.ObjectPropertyAxioms.Add(Annotated(new OWLObjectPropertyDomain(new OWLObjectInverseOf(isPartOfProperty), pizzaClass)));
        expectedOntology.ObjectPropertyAxioms.Add(Annotated(new OWLObjectPropertyRange(isPartOfProperty, pizzaClass)));
        originalOntology.ObjectPropertyAxioms.Add(new OWLObjectPropertyRange(new OWLObjectInverseOf(isPartOfProperty), pizzaClass));
        expectedOntology.ObjectPropertyAxioms.Add(new OWLObjectPropertyDomain(isPartOfProperty, pizzaClass));
        originalOntology.ObjectPropertyAxioms.Add(new OWLFunctionalObjectProperty(new OWLObjectInverseOf(hasPartProperty)));
        expectedOntology.ObjectPropertyAxioms.Add(new OWLInverseFunctionalObjectProperty(hasPartProperty));
        originalOntology.ObjectPropertyAxioms.Add(new OWLSymmetricObjectProperty(new OWLObjectInverseOf(hasPartProperty)));
        expectedOntology.ObjectPropertyAxioms.Add(new OWLSymmetricObjectProperty(hasPartProperty));
        originalOntology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(new OWLObjectInverseOf(hasPartProperty), isPartOfProperty));
        expectedOntology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(hasPartProperty, new OWLObjectInverseOf(isPartOfProperty)));
        originalOntology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(
            new OWLObjectPropertyChain([ hasToppingProperty, new OWLObjectInverseOf(isToppingOfProperty) ]), new OWLObjectInverseOf(isPartOfProperty)));
        expectedOntology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(
            new OWLObjectPropertyChain([ isToppingOfProperty, new OWLObjectInverseOf(hasToppingProperty) ]), isPartOfProperty));
        originalOntology.ObjectPropertyAxioms.Add(new OWLInverseObjectProperties(new OWLObjectInverseOf(hasPartProperty), new OWLObjectInverseOf(isPartOfProperty)));
        expectedOntology.ObjectPropertyAxioms.Add(new OWLInverseObjectProperties(hasPartProperty, isPartOfProperty));

        string manchesterDocument = OWLManchesterSerializer.SerializeOntology(originalOntology);
        OWLOntology reparsedOntology = OWLManchesterParser.DeserializeOntology(manchesterDocument);

        Assert.AreEqual(expectedOntology.IRI, reparsedOntology.IRI);
        Assert.AreEqual(expectedOntology.VersionIRI, reparsedOntology.VersionIRI);
        Assert.AreEqual(expectedOntology.Imports.Count, reparsedOntology.Imports.Count);
        CollectionAssert.AreEqual(SortedAnnotationXml(expectedOntology.Annotations), SortedAnnotationXml(reparsedOntology.Annotations));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.DeclarationAxioms), SortedXml(reparsedOntology.DeclarationAxioms));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.DatatypeDefinitionAxioms), SortedXml(reparsedOntology.DatatypeDefinitionAxioms));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.ClassAxioms), SortedXml(reparsedOntology.ClassAxioms));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.ObjectPropertyAxioms), SortedXml(reparsedOntology.ObjectPropertyAxioms));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.DataPropertyAxioms), SortedXml(reparsedOntology.DataPropertyAxioms));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.KeyAxioms), SortedXml(reparsedOntology.KeyAxioms));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.AssertionAxioms), SortedXml(reparsedOntology.AssertionAxioms));
        CollectionAssert.AreEqual(SortedXml(expectedOntology.AnnotationAxioms), SortedXml(reparsedOntology.AnnotationAxioms));
        Assert.HasCount(1, reparsedOntology.Rules);
        Assert.AreEqual(expectedOntology.Rules[0].GetXML(), reparsedOntology.Rules[0].GetXML());

        //Once rewritten the document is stable: writing and reading it again gives the very same document
        string secondDocument = OWLManchesterSerializer.SerializeOntology(reparsedOntology);
        Assert.AreEqual(secondDocument, OWLManchesterSerializer.SerializeOntology(OWLManchesterParser.DeserializeOntology(secondDocument)));
    }
    #endregion

    #region Utilities (annotations)
    //Gets the ontology annotations as sorted XML texts (annotations have no GetXML of their own, so they ride on an axiom)
    private static List<string> SortedAnnotationXml(List<OWLAnnotation> annotations)
        => annotations.Select(annotation => new OWLDeclaration(Class("Probe")) { Annotations = [ annotation ] }.GetXML())
                      .OrderBy(xml => xml, StringComparer.Ordinal).ToList();
    #endregion
}
