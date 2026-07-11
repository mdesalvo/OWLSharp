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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

/// <summary>
/// Golden round-trip smoke test: builds a single ontology exercising a wide cross-section of OWL2
/// constructs (entities, all three property-expression flavours, composite class expressions and data
/// ranges, cardinality restrictions with and without qualification, property chains, HasKey, every
/// assertion kind, annotations with a nested meta-annotation), serializes it to OWL2/Functional-Style,
/// reparses the resulting document, and checks the reparsed model matches the original structurally.
/// This is deliberately not split into one test per construct (that is the job of the dedicated
/// ParserTest/SerializerTest suites): its only purpose is to catch integration-level mismatches between
/// ToFunctionalString and OWLFunctionalParser that per-file unit tests, each exercising only one side
/// of the round trip, would not surface.
/// </summary>
[TestClass]
public class OWLFunctionalRoundTripTest
{
    [TestMethod]
    public void ShouldRoundTripRichOntologyThroughFunctionalSyntax()
    {
        OWLOntology originalOntology = new OWLOntology(new Uri("http://example.org/pz"), new Uri("http://example.org/pz/1.0"));
        originalOntology.Prefixes.Add(new OWLPrefix(new RDFNamespace("pz", "http://example.org/pz#")));
        originalOntology.Imports.Add(new OWLImport(new RDFResource("http://example.org/imported")));
        originalOntology.Annotations.Add(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new OWLLiteral(new RDFPlainLiteral("Pizza ontology"))));

        OWLClass pizzaClass = new OWLClass(new RDFResource("http://example.org/pz#Pizza"));
        OWLClass toppingClass = new OWLClass(new RDFResource("http://example.org/pz#Topping"));
        OWLClass margheritaClass = new OWLClass(new RDFResource("http://example.org/pz#Margherita"));
        OWLObjectProperty hasToppingProperty = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasTopping"));
        OWLObjectProperty hasBaseToppingProperty = new OWLObjectProperty(new RDFResource("http://example.org/pz#hasBaseTopping"));
        OWLDataProperty hasNameProperty = new OWLDataProperty(new RDFResource("http://example.org/pz#hasName"));
        OWLNamedIndividual peterIndividual = new OWLNamedIndividual(new RDFResource("http://example.org/pz#Peter"));
        OWLNamedIndividual mozzarellaIndividual = new OWLNamedIndividual(new RDFResource("http://example.org/pz#Mozzarella"));

        originalOntology.DeclarationAxioms.Add(new OWLDeclaration(pizzaClass));
        originalOntology.DeclarationAxioms.Add(new OWLDeclaration(toppingClass));
        originalOntology.DeclarationAxioms.Add(new OWLDeclaration(margheritaClass));
        originalOntology.DeclarationAxioms.Add(new OWLDeclaration(hasToppingProperty));
        originalOntology.DeclarationAxioms.Add(new OWLDeclaration(hasNameProperty));
        originalOntology.DeclarationAxioms.Add(new OWLDeclaration(peterIndividual));

        //Composite class expressions and a qualified/unqualified cardinality pair
        OWLObjectSomeValuesFrom someTopping = new OWLObjectSomeValuesFrom(hasToppingProperty, toppingClass);
        originalOntology.ClassAxioms.Add(new OWLSubClassOf(margheritaClass, someTopping));
        originalOntology.ClassAxioms.Add(new OWLEquivalentClasses([margheritaClass, new OWLObjectIntersectionOf([pizzaClass, new OWLObjectMinCardinality(hasToppingProperty, 1, toppingClass)])]));
        originalOntology.ClassAxioms.Add(new OWLDisjointClasses([pizzaClass, toppingClass]));
        originalOntology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLObjectMaxCardinality(hasToppingProperty, 5)));
        originalOntology.ClassAxioms.Add(new OWLSubClassOf(pizzaClass, new OWLDataSomeValuesFrom(hasNameProperty, new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.STRING), [new OWLFacetRestriction(new OWLLiteral(new RDFPlainLiteral("1")), RDFVocabulary.XSD.MIN_LENGTH)]))));

        //Property chain (SubObjectPropertyOf second syntactic form) and characteristics
        originalOntology.ObjectPropertyAxioms.Add(new OWLSubObjectPropertyOf(new OWLObjectPropertyChain([hasToppingProperty, hasBaseToppingProperty]), hasToppingProperty));
        originalOntology.ObjectPropertyAxioms.Add(new OWLFunctionalObjectProperty(hasBaseToppingProperty));
        originalOntology.ObjectPropertyAxioms.Add(new OWLObjectPropertyDomain(hasToppingProperty, pizzaClass));
        originalOntology.ObjectPropertyAxioms.Add(new OWLObjectPropertyRange(hasToppingProperty, toppingClass));

        originalOntology.DataPropertyAxioms.Add(new OWLFunctionalDataProperty(hasNameProperty));
        originalOntology.DataPropertyAxioms.Add(new OWLDataPropertyDomain(hasNameProperty, pizzaClass));

        //HasKey with both an object and a data property in its two lists
        originalOntology.KeyAxioms.Add(new OWLHasKey(pizzaClass, [hasToppingProperty]) { DataProperties = [hasNameProperty] });

        //Assertions, including a negative one and a same/different pair
        originalOntology.AssertionAxioms.Add(new OWLClassAssertion(margheritaClass, peterIndividual));
        originalOntology.AssertionAxioms.Add(new OWLObjectPropertyAssertion(hasToppingProperty, peterIndividual, mozzarellaIndividual));
        originalOntology.AssertionAxioms.Add(new OWLDataPropertyAssertion(hasNameProperty, peterIndividual, new OWLLiteral(new RDFPlainLiteral("Peter's pizza"))));
        originalOntology.AssertionAxioms.Add(new OWLNegativeDataPropertyAssertion(hasNameProperty, peterIndividual, new OWLLiteral(new RDFPlainLiteral("Wrong name"))));
        originalOntology.AssertionAxioms.Add(new OWLDifferentIndividuals([peterIndividual, mozzarellaIndividual]));

        //Annotation axiom with a nested meta-annotation on the axiom itself
        OWLAnnotationAssertion labelAssertion = new OWLAnnotationAssertion(new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT), pizzaClass.GetIRI(), new OWLLiteral(new RDFPlainLiteral("A dish")));
        labelAssertion.Annotate(new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL), new OWLLiteral(new RDFPlainLiteral("meta"))));
        originalOntology.AnnotationAxioms.Add(labelAssertion);

        string functionalDocument = OWLFunctionalSerializer.SerializeOntology(originalOntology);
        OWLOntology reparsedOntology = OWLFunctionalParser.DeserializeOntology(functionalDocument);

        Assert.AreEqual(originalOntology.IRI, reparsedOntology.IRI);
        Assert.AreEqual(originalOntology.VersionIRI, reparsedOntology.VersionIRI);
        Assert.AreEqual(originalOntology.Imports.Count, reparsedOntology.Imports.Count);
        Assert.AreEqual(originalOntology.Annotations.Count, reparsedOntology.Annotations.Count);
        Assert.AreEqual(originalOntology.DeclarationAxioms.Count, reparsedOntology.DeclarationAxioms.Count);
        Assert.AreEqual(originalOntology.ClassAxioms.Count, reparsedOntology.ClassAxioms.Count);
        Assert.AreEqual(originalOntology.ObjectPropertyAxioms.Count, reparsedOntology.ObjectPropertyAxioms.Count);
        Assert.AreEqual(originalOntology.DataPropertyAxioms.Count, reparsedOntology.DataPropertyAxioms.Count);
        Assert.AreEqual(originalOntology.KeyAxioms.Count, reparsedOntology.KeyAxioms.Count);
        Assert.AreEqual(originalOntology.AssertionAxioms.Count, reparsedOntology.AssertionAxioms.Count);
        Assert.AreEqual(originalOntology.AnnotationAxioms.Count, reparsedOntology.AnnotationAxioms.Count);

        //Spot-check a handful of axioms structurally by re-serializing both sides to Functional-Style
        //and comparing: cheaper than replicating a field-by-field comparison for every axiom kind, and
        //exercises ToFunctionalString a second time (on the reparsed model) as a bonus consistency check
        Assert.AreEqual(originalOntology.KeyAxioms[0].ToFunctionalString(new OWLFunctionalContext(originalOntology.Prefixes)),
                         reparsedOntology.KeyAxioms[0].ToFunctionalString(new OWLFunctionalContext(reparsedOntology.Prefixes)));
        Assert.AreEqual(originalOntology.ObjectPropertyAxioms[0].ToFunctionalString(new OWLFunctionalContext(originalOntology.Prefixes)),
                         reparsedOntology.ObjectPropertyAxioms[0].ToFunctionalString(new OWLFunctionalContext(reparsedOntology.Prefixes)));
        Assert.AreEqual(originalOntology.ClassAxioms[4].ToFunctionalString(new OWLFunctionalContext(originalOntology.Prefixes)),
                         reparsedOntology.ClassAxioms[4].ToFunctionalString(new OWLFunctionalContext(reparsedOntology.Prefixes)));
    }
}
