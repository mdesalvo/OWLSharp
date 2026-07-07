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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;

namespace OWLSharp.Test.Ontology.Manchester;

[TestClass]
public class OWLManchesterParserTest
{
    #region Utilities
    //Every test document is implicitly prefixed with a "pz:" namespace and a default (empty) prefix,
    //so that both PrefixedName and simple-name entity references can be exercised
    private const string Prologue =
        "Prefix: pz: <http://example.org/pz#>\nPrefix: : <http://example.org/pz#>\nOntology: <http://example.org/pz>\n";

    private static OWLOntology Parse(string body)
        => OWLManchesterParser.DeserializeOntology(Prologue + body);

    //Prefix declarations must precede the "Ontology:" header in the grammar, so tests needing an extra
    //prefix (e.g: rdfs:) cannot simply append a "Prefix:" line after Prologue: they must rebuild the header
    private static OWLOntology ParseWithRdfs(string body)
        => OWLManchesterParser.DeserializeOntology(
            "Prefix: pz: <http://example.org/pz#>\nPrefix: : <http://example.org/pz#>\n"
            + "Prefix: rdfs: <http://www.w3.org/2000/01/rdf-schema#>\nOntology: <http://example.org/pz>\n" + body);
    #endregion

    #region Tests (document header)
    [TestMethod]
    public void ShouldParseOntologyIRIAndVersionIRI()
    {
        OWLOntology ontology = OWLManchesterParser.DeserializeOntology(
            "Ontology: <http://example.org/pz> <http://example.org/pz/1.0>");

        Assert.AreEqual("http://example.org/pz", ontology.IRI);
        Assert.AreEqual("http://example.org/pz/1.0", ontology.VersionIRI);
    }

    [TestMethod]
    public void ShouldParseOntologyWithoutVersionIRI()
    {
        OWLOntology ontology = OWLManchesterParser.DeserializeOntology("Ontology: <http://example.org/pz>");

        Assert.AreEqual("http://example.org/pz", ontology.IRI);
        Assert.IsNull(ontology.VersionIRI);
    }

    [TestMethod]
    public void ShouldParseAnonymousOntology()
    {
        OWLOntology ontology = OWLManchesterParser.DeserializeOntology(
            "Prefix: pz: <http://example.org/pz#>\nOntology:\nClass: pz:A");

        Assert.IsNull(ontology.IRI);
        Assert.HasCount(1, ontology.DeclarationAxioms);
    }

    [TestMethod]
    public void ShouldParseDocumentWithoutOntologyHeader()
    {
        //The "Ontology:" header is optional in the grammar
        OWLOntology ontology = OWLManchesterParser.DeserializeOntology(
            "Prefix: pz: <http://example.org/pz#>\nClass: pz:A");

        Assert.IsNull(ontology.IRI);
        Assert.HasCount(1, ontology.DeclarationAxioms);
    }

    [TestMethod]
    public void ShouldParseCustomPrefixDeclaration()
    {
        OWLOntology ontology = OWLManchesterParser.DeserializeOntology(
            "Prefix: pz: <http://example.org/pz#>\nOntology: <http://example.org/pz>\nClass: pz:A");

        Assert.IsTrue(ontology.Prefixes.Any(pfx => pfx.Name == "pz" && pfx.IRI == "http://example.org/pz#"));
    }

    [TestMethod]
    public void ShouldParseMultipleImports()
    {
        OWLOntology ontology = OWLManchesterParser.DeserializeOntology(
            "Ontology: <http://example.org/pz>\nImport: <http://example.org/a>\nImport: <http://example.org/b>");

        Assert.HasCount(2, ontology.Imports);
        Assert.AreEqual("http://example.org/a", ontology.Imports[0].IRI);
        Assert.AreEqual("http://example.org/b", ontology.Imports[1].IRI);
    }

    [TestMethod]
    public void ShouldParseOntologyAnnotations()
    {
        OWLOntology ontology = ParseWithRdfs("Annotations: rdfs:comment \"a\", rdfs:comment \"b\"");

        Assert.HasCount(2, ontology.Annotations);
        Assert.AreEqual("a", ontology.Annotations[0].ValueLiteral.Value);
        Assert.AreEqual("b", ontology.Annotations[1].ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseNestedOntologyAnnotation()
    {
        OWLOntology ontology = ParseWithRdfs("Annotations: Annotations: rdfs:label \"meta\" rdfs:comment \"outer\"");

        Assert.HasCount(1, ontology.Annotations);
        Assert.AreEqual("outer", ontology.Annotations[0].ValueLiteral.Value);
        Assert.IsNotNull(ontology.Annotations[0].Annotation);
        Assert.AreEqual("meta", ontology.Annotations[0].Annotation.ValueLiteral.Value);
    }
    #endregion

    #region Tests (Class frame)
    [TestMethod]
    public void ShouldParseClassDeclaration()
    {
        OWLOntology ontology = Parse("Class: pz:Pizza");

        Assert.HasCount(1, ontology.DeclarationAxioms);
        Assert.IsInstanceOfType<OWLClass>(ontology.DeclarationAxioms[0].Entity);
        Assert.AreEqual("http://example.org/pz#Pizza", ((OWLClass)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseClassDeclarationWithSimpleName()
    {
        //A bare name resolves against the default (empty) prefix
        OWLOntology ontology = Parse("Class: Pizza");

        Assert.AreEqual("http://example.org/pz#Pizza", ((OWLClass)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseClassDeclarationWithFullIRI()
    {
        OWLOntology ontology = Parse("Class: <http://other.org/Pizza>");

        Assert.AreEqual("http://other.org/Pizza", ((OWLClass)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseSubClassOfSection()
    {
        OWLOntology ontology = Parse("Class: pz:Margherita\n    SubClassOf: pz:Pizza");

        Assert.HasCount(1, ontology.ClassAxioms);
        OWLSubClassOf sco = (OWLSubClassOf)ontology.ClassAxioms[0];
        Assert.AreEqual("http://example.org/pz#Margherita", sco.SubClassExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Pizza", sco.SuperClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseSubClassOfWithMultipleCommaSeparatedItems()
    {
        OWLOntology ontology = Parse("Class: pz:Margherita\n    SubClassOf: pz:Pizza, pz:Food");

        Assert.HasCount(2, ontology.ClassAxioms);
    }

    [TestMethod]
    public void ShouldParseSubClassOfWithAnnotation()
    {
        OWLOntology ontology = ParseWithRdfs(
            "Class: pz:Margherita\n    SubClassOf: Annotations: rdfs:comment \"why\" pz:Pizza");

        OWLSubClassOf sco = (OWLSubClassOf)ontology.ClassAxioms[0];
        Assert.HasCount(1, sco.Annotations);
        Assert.AreEqual("why", sco.Annotations[0].ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseEquivalentToInClassFrame()
    {
        OWLOntology ontology = Parse("Class: pz:A\n    EquivalentTo: pz:B");

        OWLEquivalentClasses eqc = (OWLEquivalentClasses)ontology.ClassAxioms[0];
        Assert.HasCount(2, eqc.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseDisjointWithInClassFrame()
    {
        OWLOntology ontology = Parse("Class: pz:A\n    DisjointWith: pz:B");

        OWLDisjointClasses dcc = (OWLDisjointClasses)ontology.ClassAxioms[0];
        Assert.HasCount(2, dcc.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseDisjointUnionOf()
    {
        OWLOntology ontology = Parse("Class: pz:Pizza\n    DisjointUnionOf: pz:Margherita, pz:Marinara");

        OWLDisjointUnion dju = (OWLDisjointUnion)ontology.ClassAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", dju.ClassIRI.GetIRI().ToString());
        Assert.HasCount(2, dju.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseHasKeyWithObjectAndDataProperties()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nDataProperty: pz:hasCode\n"
            + "Class: pz:Pizza\n    HasKey: pz:hasTopping pz:hasCode");

        OWLHasKey key = ontology.KeyAxioms[0];
        Assert.HasCount(1, key.ObjectPropertyExpressions);
        Assert.HasCount(1, key.DataProperties);
    }

    [TestMethod]
    public void ShouldParseHasKeyWithInverseObjectProperty()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:Pizza\n    HasKey: inverse pz:hasTopping");

        OWLHasKey key = ontology.KeyAxioms[0];
        Assert.HasCount(1, key.ObjectPropertyExpressions);
        Assert.IsInstanceOfType<OWLObjectInverseOf>(key.ObjectPropertyExpressions[0]);
    }

    [TestMethod]
    public void ShouldParseClassEntityAnnotations()
    {
        OWLOntology ontology = ParseWithRdfs(
            "Class: pz:Pizza\n    Annotations: rdfs:label \"Pizza\"");

        Assert.HasCount(1, ontology.AnnotationAxioms);
        OWLAnnotationAssertion ann = (OWLAnnotationAssertion)ontology.AnnotationAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", ann.SubjectIRI);
        Assert.AreEqual("Pizza", ann.ValueLiteral.Value);
    }
    #endregion

    #region Tests (ObjectProperty frame)
    [TestMethod]
    public void ShouldParseObjectPropertyDomainAndRange()
    {
        OWLOntology ontology = Parse(
            "Class: pz:Pizza\nClass: pz:Topping\n"
            + "ObjectProperty: pz:hasTopping\n    Domain: pz:Pizza\n    Range: pz:Topping");

        OWLObjectPropertyDomain domain = (OWLObjectPropertyDomain)ontology.ObjectPropertyAxioms[0];
        OWLObjectPropertyRange range = (OWLObjectPropertyRange)ontology.ObjectPropertyAxioms[1];
        Assert.AreEqual("http://example.org/pz#Pizza", domain.ClassExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Topping", range.ClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    [DataRow("Functional", typeof(OWLFunctionalObjectProperty))]
    [DataRow("InverseFunctional", typeof(OWLInverseFunctionalObjectProperty))]
    [DataRow("Reflexive", typeof(OWLReflexiveObjectProperty))]
    [DataRow("Irreflexive", typeof(OWLIrreflexiveObjectProperty))]
    [DataRow("Symmetric", typeof(OWLSymmetricObjectProperty))]
    [DataRow("Asymmetric", typeof(OWLAsymmetricObjectProperty))]
    [DataRow("Transitive", typeof(OWLTransitiveObjectProperty))]
    public void ShouldParseEachObjectPropertyCharacteristic(string characteristic, System.Type expectedType)
    {
        OWLOntology ontology = Parse($"ObjectProperty: pz:hasTopping\n    Characteristics: {characteristic}");

        Assert.HasCount(1, ontology.ObjectPropertyAxioms);
        Assert.IsInstanceOfType(ontology.ObjectPropertyAxioms[0], expectedType);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnknownObjectPropertyCharacteristic()
        => Assert.ThrowsExactly<OWLException>(() => Parse("ObjectProperty: pz:hasTopping\n    Characteristics: Idempotent"));

    [TestMethod]
    public void ShouldParseObjectSubPropertyOf()
    {
        OWLOntology ontology = Parse("ObjectProperty: pz:hasTopping\n    SubPropertyOf: pz:hasIngredient");

        OWLSubObjectPropertyOf spo = (OWLSubObjectPropertyOf)ontology.ObjectPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasTopping", spo.SubObjectPropertyExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#hasIngredient", spo.SuperObjectPropertyExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectPropertyEquivalentToAndDisjointWith()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\n    EquivalentTo: pz:hasIngredient\n    DisjointWith: pz:excludes");

        Assert.IsInstanceOfType<OWLEquivalentObjectProperties>(ontology.ObjectPropertyAxioms[0]);
        Assert.IsInstanceOfType<OWLDisjointObjectProperties>(ontology.ObjectPropertyAxioms[1]);
    }

    [TestMethod]
    public void ShouldParseObjectPropertyInverseOf()
    {
        OWLOntology ontology = Parse("ObjectProperty: pz:hasTopping\n    InverseOf: pz:isToppingOf");

        OWLInverseObjectProperties inv = (OWLInverseObjectProperties)ontology.ObjectPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasTopping", inv.LeftObjectPropertyExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#isToppingOf", inv.RightObjectPropertyExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectPropertyInverseOfWithInverseKeyword()
    {
        OWLOntology ontology = Parse("ObjectProperty: pz:hasTopping\n    InverseOf: inverse pz:isToppingOf");

        OWLInverseObjectProperties inv = (OWLInverseObjectProperties)ontology.ObjectPropertyAxioms[0];
        Assert.IsInstanceOfType<OWLObjectInverseOf>(inv.RightObjectPropertyExpression);
    }

    [TestMethod]
    public void ShouldParseSubPropertyChainOfTwoProperties()
    {
        OWLOntology ontology = Parse("ObjectProperty: pz:hasGrandparent\n    SubPropertyChain: pz:hasParent o pz:hasParent");

        OWLSubObjectPropertyOf spo = (OWLSubObjectPropertyOf)ontology.ObjectPropertyAxioms[0];
        Assert.IsNotNull(spo.SubObjectPropertyChain);
        Assert.HasCount(2, spo.SubObjectPropertyChain.ObjectPropertyExpressions);
        Assert.AreEqual("http://example.org/pz#hasGrandparent", spo.SuperObjectPropertyExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseSubPropertyChainOfThreeProperties()
    {
        OWLOntology ontology = Parse("ObjectProperty: pz:p\n    SubPropertyChain: pz:a o pz:b o pz:c");

        OWLSubObjectPropertyOf spo = (OWLSubObjectPropertyOf)ontology.ObjectPropertyAxioms[0];
        Assert.HasCount(3, spo.SubObjectPropertyChain.ObjectPropertyExpressions);
    }
    #endregion

    #region Tests (DataProperty frame)
    [TestMethod]
    public void ShouldParseDataPropertyDomainAndRange()
    {
        OWLOntology ontology = Parse(
            "Class: pz:Pizza\nDataProperty: pz:hasCalories\n    Domain: pz:Pizza\n    Range: xsd:integer");

        OWLDataPropertyDomain domain = (OWLDataPropertyDomain)ontology.DataPropertyAxioms[0];
        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[1];
        Assert.AreEqual("http://example.org/pz#Pizza", domain.ClassExpression.GetIRI().ToString());
        Assert.IsInstanceOfType<OWLDatatype>(range.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParseDataPropertyFunctionalCharacteristic()
    {
        OWLOntology ontology = Parse("DataProperty: pz:hasCalories\n    Characteristics: Functional");

        Assert.IsInstanceOfType<OWLFunctionalDataProperty>(ontology.DataPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnknownDataPropertyCharacteristic()
        => Assert.ThrowsExactly<OWLException>(() => Parse("DataProperty: pz:hasCalories\n    Characteristics: Transitive"));

    [TestMethod]
    public void ShouldParseDataSubPropertyOfEquivalentToDisjointWith()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:hasCalories\n    SubPropertyOf: pz:hasNumericValue\n"
            + "    EquivalentTo: pz:hasKcal\n    DisjointWith: pz:hasName");

        Assert.IsInstanceOfType<OWLSubDataPropertyOf>(ontology.DataPropertyAxioms[0]);
        Assert.IsInstanceOfType<OWLEquivalentDataProperties>(ontology.DataPropertyAxioms[1]);
        Assert.IsInstanceOfType<OWLDisjointDataProperties>(ontology.DataPropertyAxioms[2]);
    }
    #endregion

    #region Tests (AnnotationProperty frame)
    [TestMethod]
    public void ShouldParseAnnotationPropertyDomainRangeAndSubPropertyOf()
    {
        OWLOntology ontology = Parse(
            "Class: pz:Pizza\n"
            + "AnnotationProperty: pz:note\n    Domain: pz:Pizza\n    Range: pz:Pizza\n    SubPropertyOf: pz:comment");

        Assert.IsInstanceOfType<OWLAnnotationPropertyDomain>(ontology.AnnotationAxioms[0]);
        Assert.IsInstanceOfType<OWLAnnotationPropertyRange>(ontology.AnnotationAxioms[1]);
        Assert.IsInstanceOfType<OWLSubAnnotationPropertyOf>(ontology.AnnotationAxioms[2]);
    }
    #endregion

    #region Tests (Datatype frame)
    [TestMethod]
    public void ShouldParseDatatypeEquivalentTo()
    {
        OWLOntology ontology = Parse(
            "Datatype: pz:PositiveInt\n    EquivalentTo: xsd:integer[>= \"0\"^^xsd:integer]");

        OWLDatatypeDefinition def = (OWLDatatypeDefinition)ontology.DatatypeDefinitionAxioms[0];
        Assert.AreEqual("http://example.org/pz#PositiveInt", def.Datatype.GetIRI().ToString());
        Assert.IsInstanceOfType<OWLDatatypeRestriction>(def.DataRangeExpression);
    }
    #endregion

    #region Tests (Individual frame)
    [TestMethod]
    public void ShouldParseIndividualTypesSection()
    {
        OWLOntology ontology = Parse("Class: pz:Pizza\nIndividual: pz:Margherita\n    Types: pz:Pizza");

        OWLClassAssertion ca = (OWLClassAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", ca.ClassExpression.GetIRI().ToString());
        Assert.IsInstanceOfType<OWLNamedIndividual>(ca.IndividualExpression);
    }

    [TestMethod]
    public void ShouldParseIndividualTypesWithComplexExpression()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:Topping\n"
            + "Individual: pz:Margherita\n    Types: pz:hasTopping some pz:Topping");

        OWLClassAssertion ca = (OWLClassAssertion)ontology.AssertionAxioms[0];
        Assert.IsInstanceOfType<OWLObjectSomeValuesFrom>(ca.ClassExpression);
    }

    [TestMethod]
    public void ShouldParseAnonymousIndividualFrame()
    {
        OWLOntology ontology = Parse("Class: pz:Pizza\nIndividual: _:anon1\n    Types: pz:Pizza");

        //Anonymous individuals do not generate a Declaration axiom
        Assert.HasCount(1, ontology.DeclarationAxioms);
        OWLClassAssertion ca = (OWLClassAssertion)ontology.AssertionAxioms[0];
        Assert.IsInstanceOfType<OWLAnonymousIndividual>(ca.IndividualExpression);
        Assert.AreEqual("anon1", ((OWLAnonymousIndividual)ca.IndividualExpression).NodeID);
    }

    [TestMethod]
    public void ShouldParsePositiveObjectPropertyFact()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nIndividual: pz:Margherita\n    Facts: pz:hasTopping pz:Mozzarella");

        OWLObjectPropertyAssertion opa = (OWLObjectPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasTopping", opa.ObjectPropertyExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Margherita", opa.SourceIndividualExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Mozzarella", opa.TargetIndividualExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseNegativeObjectPropertyFact()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nIndividual: pz:Margherita\n    Facts: not pz:hasTopping pz:Anchovy");

        Assert.IsInstanceOfType<OWLNegativeObjectPropertyAssertion>(ontology.AssertionAxioms[0]);
    }

    [TestMethod]
    public void ShouldParsePositiveDataPropertyFact()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:hasCalories\nIndividual: pz:Margherita\n    Facts: pz:hasCalories 850");

        OWLDataPropertyAssertion dpa = (OWLDataPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("850", dpa.Literal.Value);
    }

    [TestMethod]
    public void ShouldParseNegativeDataPropertyFact()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:hasCalories\nIndividual: pz:Margherita\n    Facts: not pz:hasCalories 0");

        Assert.IsInstanceOfType<OWLNegativeDataPropertyAssertion>(ontology.AssertionAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseMultipleFactsCommaSeparated()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nDataProperty: pz:hasCalories\n"
            + "Individual: pz:Margherita\n    Facts: pz:hasTopping pz:Mozzarella, pz:hasCalories 850");

        Assert.HasCount(2, ontology.AssertionAxioms);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnFactWithUndeclaredProperty()
        => Assert.ThrowsExactly<OWLException>(() => Parse("Individual: pz:Margherita\n    Facts: pz:hasTopping pz:Mozzarella"));

    [TestMethod]
    public void ShouldParseSameAsAndDifferentFromSections()
    {
        OWLOntology ontology = Parse(
            "Individual: pz:Margherita\n    SameAs: pz:MargheritaPizza\n    DifferentFrom: pz:Marinara");

        Assert.IsInstanceOfType<OWLSameIndividual>(ontology.AssertionAxioms[0]);
        Assert.IsInstanceOfType<OWLDifferentIndividuals>(ontology.AssertionAxioms[1]);
    }

    [TestMethod]
    public void ShouldParseIndividualEntityAnnotations()
    {
        OWLOntology ontology = ParseWithRdfs(
            "Individual: pz:Margherita\n    Annotations: rdfs:label \"Margherita\"");

        Assert.HasCount(1, ontology.AnnotationAxioms);
    }
    #endregion

    #region Tests (Misc sections)
    [TestMethod]
    public void ShouldParseEquivalentClassesMiscSection()
    {
        OWLOntology ontology = Parse("EquivalentClasses: pz:A, pz:B, pz:C");

        OWLEquivalentClasses eqc = (OWLEquivalentClasses)ontology.ClassAxioms[0];
        Assert.HasCount(3, eqc.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseDisjointClassesMiscSection()
    {
        OWLOntology ontology = Parse("DisjointClasses: pz:A, pz:B");

        Assert.IsInstanceOfType<OWLDisjointClasses>(ontology.ClassAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseEquivalentPropertiesAsObjectVariant()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nObjectProperty: pz:hasIngredient\n"
            + "EquivalentProperties: pz:hasTopping, pz:hasIngredient");

        Assert.IsInstanceOfType<OWLEquivalentObjectProperties>(ontology.ObjectPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseDisjointPropertiesAsDataVariant()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:hasCalories\nDataProperty: pz:hasName\n"
            + "DisjointProperties: pz:hasCalories, pz:hasName");

        Assert.IsInstanceOfType<OWLDisjointDataProperties>(ontology.DataPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseEquivalentPropertiesWithInverseMember()
    {
        //An "inverse" member forces the object-property variant even without a symbol table hit
        OWLOntology ontology = Parse("EquivalentProperties: inverse pz:hasTopping, pz:isToppingOf");

        OWLEquivalentObjectProperties eqp = (OWLEquivalentObjectProperties)ontology.ObjectPropertyAxioms[0];
        Assert.IsInstanceOfType<OWLObjectInverseOf>(eqp.ObjectPropertyExpressions[0]);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnAmbiguousPropertiesMiscSection()
        //Neither member is declared, so object/data cannot be disambiguated
        => Assert.ThrowsExactly<OWLException>(() => Parse("EquivalentProperties: pz:p1, pz:p2"));

    [TestMethod]
    public void ShouldParseSameIndividualMiscSection()
    {
        OWLOntology ontology = Parse("SameIndividual: pz:A, pz:B, pz:C");

        OWLSameIndividual si = (OWLSameIndividual)ontology.AssertionAxioms[0];
        Assert.HasCount(3, si.IndividualExpressions);
    }

    [TestMethod]
    public void ShouldParseDifferentIndividualsMiscSection()
    {
        OWLOntology ontology = Parse("DifferentIndividuals: pz:A, pz:B");

        Assert.IsInstanceOfType<OWLDifferentIndividuals>(ontology.AssertionAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseMiscSectionWithAnnotationBlock()
    {
        OWLOntology ontology = ParseWithRdfs(
            "DisjointClasses: Annotations: rdfs:comment \"why\" pz:A, pz:B");

        Assert.HasCount(1, ontology.ClassAxioms[0].Annotations);
    }
    #endregion

    #region Tests (class expressions)
    [TestMethod]
    public void ShouldParseIntersectionWithAnd()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: pz:A and pz:B");

        OWLObjectIntersectionOf inter = (OWLObjectIntersectionOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(2, inter.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseIntersectionWithThatSynonym()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: pz:A that pz:B");

        Assert.IsInstanceOfType<OWLObjectIntersectionOf>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseUnionWithOr()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: pz:A or pz:B or pz:C");

        OWLObjectUnionOf union = (OWLObjectUnionOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(3, union.ClassExpressions);
    }

    [TestMethod]
    public void ShouldRespectOrOverAndPrecedence()
    {
        //"A and B or C" must parse as "(A and B) or C" (or binds looser than and)
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: pz:A and pz:B or pz:C");

        OWLObjectUnionOf union = (OWLObjectUnionOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(2, union.ClassExpressions);
        Assert.IsInstanceOfType<OWLObjectIntersectionOf>(union.ClassExpressions[0]);
    }

    [TestMethod]
    public void ShouldParseParenthesizedDescription()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: (pz:A or pz:B) and pz:C");

        OWLObjectIntersectionOf inter = (OWLObjectIntersectionOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.IsInstanceOfType<OWLObjectUnionOf>(inter.ClassExpressions[0]);
    }

    [TestMethod]
    public void ShouldParseNegation()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: not pz:A");

        Assert.IsInstanceOfType<OWLObjectComplementOf>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseDoubleNegation()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: not not pz:A");

        OWLObjectComplementOf outer = (OWLObjectComplementOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.IsInstanceOfType<OWLObjectComplementOf>(outer.ClassExpression);
    }

    [TestMethod]
    public void ShouldParseIndividualEnumeration()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: {pz:A, pz:B}");

        OWLObjectOneOf oneOf = (OWLObjectOneOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(2, oneOf.IndividualExpressions);
    }

    [TestMethod]
    public void ShouldParseSingletonIndividualEnumeration()
    {
        OWLOntology ontology = Parse("Class: pz:X\n    SubClassOf: {pz:A}");

        OWLObjectOneOf oneOf = (OWLObjectOneOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(1, oneOf.IndividualExpressions);
    }

    [TestMethod]
    public void ShouldParseObjectSomeValuesFrom()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:X\n    SubClassOf: pz:hasTopping some pz:Mozzarella");

        Assert.IsInstanceOfType<OWLObjectSomeValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseObjectAllValuesFrom()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:X\n    SubClassOf: pz:hasTopping only pz:Mozzarella");

        Assert.IsInstanceOfType<OWLObjectAllValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseObjectHasValue()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:X\n    SubClassOf: pz:hasTopping value pz:Mozzarella");

        OWLObjectHasValue hv = (OWLObjectHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("http://example.org/pz#Mozzarella", hv.IndividualExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectHasSelf()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:likes\nClass: pz:X\n    SubClassOf: pz:likes Self");

        Assert.IsInstanceOfType<OWLObjectHasSelf>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    [DataRow("min")]
    [DataRow("max")]
    [DataRow("exactly")]
    public void ShouldParseUnqualifiedObjectCardinality(string keyword)
    {
        OWLOntology ontology = Parse(
            $"ObjectProperty: pz:hasTopping\nClass: pz:X\n    SubClassOf: pz:hasTopping {keyword} 2");

        OWLClassExpression restriction = ((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        string cardinality = restriction switch
        {
            OWLObjectMinCardinality c => c.Cardinality,
            OWLObjectMaxCardinality c => c.Cardinality,
            OWLObjectExactCardinality c => c.Cardinality,
            _ => null
        };
        Assert.AreEqual("2", cardinality);
    }

    [TestMethod]
    public void ShouldParseQualifiedObjectCardinality()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:Topping\nClass: pz:X\n    SubClassOf: pz:hasTopping max 3 pz:Topping");

        OWLObjectMaxCardinality max = (OWLObjectMaxCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("3", max.Cardinality);
        Assert.IsNotNull(max.ClassExpression);
        Assert.AreEqual("http://example.org/pz#Topping", max.ClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseInverseObjectPropertyRestriction()
    {
        OWLOntology ontology = Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:X\n    SubClassOf: inverse pz:hasTopping some pz:Pizza");

        OWLObjectSomeValuesFrom some = (OWLObjectSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.IsInstanceOfType<OWLObjectInverseOf>(some.ObjectPropertyExpression);
    }

    [TestMethod]
    public void ShouldParseDataSomeValuesFrom()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:hasCalories\nClass: pz:X\n    SubClassOf: pz:hasCalories some xsd:integer");

        Assert.IsInstanceOfType<OWLDataSomeValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseDataHasValue()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:hasCalories\nClass: pz:X\n    SubClassOf: pz:hasCalories value 300");

        OWLDataHasValue hv = (OWLDataHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("300", hv.Literal.Value);
    }

    [TestMethod]
    public void ShouldThrowExceptionWhenRestrictionAppliedToUndeclaredProperty()
        => Assert.ThrowsExactly<OWLException>(() => Parse("Class: pz:X\n    SubClassOf: pz:hasTopping some pz:Y"));
    #endregion

    #region Tests (data ranges and literals)
    [TestMethod]
    public void ShouldParseDataUnionAndIntersection()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p some (xsd:integer and xsd:string or xsd:boolean)");

        OWLDataSomeValuesFrom some = (OWLDataSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        OWLDataUnionOf union = (OWLDataUnionOf)some.DataRangeExpression;
        Assert.HasCount(2, union.DataRangeExpressions);
        Assert.IsInstanceOfType<OWLDataIntersectionOf>(union.DataRangeExpressions[0]);
    }

    [TestMethod]
    public void ShouldParseDataComplementOf()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p some not xsd:integer");

        OWLDataSomeValuesFrom some = (OWLDataSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.IsInstanceOfType<OWLDataComplementOf>(some.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParseDataOneOfLiteralEnumeration()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p some {\"a\", \"b\", \"c\"}");

        OWLDataSomeValuesFrom some = (OWLDataSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        OWLDataOneOf oneOf = (OWLDataOneOf)some.DataRangeExpression;
        Assert.HasCount(3, oneOf.Literals);
    }

    [TestMethod]
    public void ShouldParseDatatypeRestrictionWithSymbolFacet()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p some xsd:integer[>= \"0\"^^xsd:integer, < \"18\"^^xsd:integer]");

        OWLDataSomeValuesFrom some = (OWLDataSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        OWLDatatypeRestriction restriction = (OWLDatatypeRestriction)some.DataRangeExpression;
        Assert.HasCount(2, restriction.FacetRestrictions);
        Assert.AreEqual(RDFSharp.Model.RDFVocabulary.XSD.MIN_INCLUSIVE.ToString(), restriction.FacetRestrictions[0].FacetIRI);
        Assert.AreEqual(RDFSharp.Model.RDFVocabulary.XSD.MAX_EXCLUSIVE.ToString(), restriction.FacetRestrictions[1].FacetIRI);
    }

    [TestMethod]
    public void ShouldParseDatatypeRestrictionWithExplicitFacetIRI()
    {
        //Fallback form emitted by the serializer for facets without a Manchester symbol
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n"
            + "    SubClassOf: pz:p some xsd:string[<http://www.w3.org/2001/XMLSchema#totalDigits> \"5\"^^xsd:integer]");

        OWLDataSomeValuesFrom some = (OWLDataSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        OWLDatatypeRestriction restriction = (OWLDatatypeRestriction)some.DataRangeExpression;
        Assert.AreEqual("http://www.w3.org/2001/XMLSchema#totalDigits", restriction.FacetRestrictions[0].FacetIRI);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnknownFacetKeyword()
        => Assert.ThrowsExactly<OWLException>(() => Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p some xsd:integer[bogus \"1\"]"));

    [TestMethod]
    [DataRow("integer")]
    [DataRow("decimal")]
    [DataRow("float")]
    [DataRow("string")]
    public void ShouldParseBuiltInDatatypeShortcuts(string shortcut)
    {
        OWLOntology ontology = Parse(
            $"DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p some {shortcut}");

        Assert.IsInstanceOfType<OWLDatatype>(((OWLDataSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression).DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParsePlainQuotedLiteral()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p value \"hello\"");

        OWLDataHasValue hv = (OWLDataHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("hello", hv.Literal.Value);
        Assert.IsNull(hv.Literal.DatatypeIRI);
        Assert.IsNull(hv.Literal.Language);
    }

    [TestMethod]
    public void ShouldParseLanguageTaggedLiteral()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p value \"Pizza\"@it");

        OWLDataHasValue hv = (OWLDataHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("it", hv.Literal.Language);
    }

    [TestMethod]
    public void ShouldParseTypedLiteral()
    {
        OWLOntology ontology = Parse(
            "DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p value \"42\"^^xsd:integer");

        OWLDataHasValue hv = (OWLDataHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual(RDFSharp.Model.RDFVocabulary.XSD.INTEGER.ToString(), hv.Literal.DatatypeIRI);
    }

    [TestMethod]
    [DataRow("42", "http://www.w3.org/2001/XMLSchema#integer")]
    [DataRow("3.14", "http://www.w3.org/2001/XMLSchema#decimal")]
    [DataRow("1.5e3", "http://www.w3.org/2001/XMLSchema#float")]
    [DataRow("true", "http://www.w3.org/2001/XMLSchema#boolean")]
    [DataRow("false", "http://www.w3.org/2001/XMLSchema#boolean")]
    public void ShouldParseBareLiteralsWithImplicitDatatype(string literal, string expectedDatatypeIRI)
    {
        OWLOntology ontology = Parse($"DataProperty: pz:p\nClass: pz:X\n    SubClassOf: pz:p value {literal}");

        OWLDataHasValue hv = (OWLDataHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual(expectedDatatypeIRI, hv.Literal.DatatypeIRI);
    }
    #endregion

    #region Tests (punning and symbol table)
    [TestMethod]
    public void ShouldGivePropertyKindPrecedenceOnPunning()
    {
        //The same IRI declared as both a Class and an ObjectProperty: the symbol table must remember
        //the property kind, since only property kinds drive the type-dependent grammar decisions
        OWLOntology ontology = Parse(
            "Class: pz:Ambiguous\nObjectProperty: pz:Ambiguous\nClass: pz:X\n    SubClassOf: pz:Ambiguous some pz:X");

        Assert.IsInstanceOfType<OWLObjectSomeValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldResolveForwardReferenceToPropertyFrameDeclaredLaterInDocument()
    {
        //The whole point of the two-pass strategy: the Class frame using "pz:hasTopping some pz:X" is parsed
        //(pass 2) before the ObjectProperty frame that declares pz:hasTopping is reached in the document,
        //yet pass 1 has already scanned every frame header up front, so the symbol table already knows its kind
        OWLOntology ontology = Parse(
            "Class: pz:X\n    SubClassOf: pz:hasTopping some pz:X\nObjectProperty: pz:hasTopping");

        Assert.IsInstanceOfType<OWLObjectSomeValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }
    #endregion

    #region Tests (errors)
    [TestMethod]
    public void ShouldThrowExceptionOnUndeclaredPrefix()
        => Assert.ThrowsExactly<OWLException>(() => Parse("Class: undeclared:Pizza"));

    [TestMethod]
    public void ShouldThrowExceptionOnUnexpectedTopLevelToken()
        => Assert.ThrowsExactly<OWLException>(() => Parse("42"));

    [TestMethod]
    public void ShouldThrowExceptionOnMalformedCardinality()
        => Assert.ThrowsExactly<OWLException>(() => Parse(
            "ObjectProperty: pz:hasTopping\nClass: pz:X\n    SubClassOf: pz:hasTopping min two"));
    #endregion

    #region Tests (round-trip via serializer)
    [TestMethod]
    public void ShouldRoundTripThroughSerializerAndParser()
    {
        OWLOntology original = new OWLOntology(new System.Uri("http://example.org/pz"));
        original.Prefixes.Add(new OWLPrefix(new RDFSharp.Model.RDFNamespace("pz", "http://example.org/pz#")));

        OWLClass pizza = new OWLClass(new RDFSharp.Model.RDFResource("http://example.org/pz#Pizza"));
        OWLClass topping = new OWLClass(new RDFSharp.Model.RDFResource("http://example.org/pz#Topping"));
        OWLObjectProperty hasTopping = new OWLObjectProperty(new RDFSharp.Model.RDFResource("http://example.org/pz#hasTopping"));
        OWLNamedIndividual margherita = new OWLNamedIndividual(new RDFSharp.Model.RDFResource("http://example.org/pz#Margherita"));

        original.DeclarationAxioms.Add(new OWLDeclaration(pizza));
        original.DeclarationAxioms.Add(new OWLDeclaration(topping));
        original.DeclarationAxioms.Add(new OWLDeclaration(hasTopping));
        original.DeclarationAxioms.Add(new OWLDeclaration(margherita));
        original.ClassAxioms.Add(new OWLSubClassOf(pizza, new OWLObjectSomeValuesFrom(hasTopping, topping)));
        original.ObjectPropertyAxioms.Add(new OWLObjectPropertyDomain(hasTopping, pizza));
        original.AssertionAxioms.Add(new OWLClassAssertion(pizza, margherita));

        string manchesterDocument = OWLManchesterSerializer.SerializeOntology(original);
        OWLOntology reparsed = OWLManchesterParser.DeserializeOntology(manchesterDocument);

        Assert.AreEqual(original.DeclarationAxioms.Count, reparsed.DeclarationAxioms.Count);
        Assert.AreEqual(original.ClassAxioms.Count, reparsed.ClassAxioms.Count);
        Assert.AreEqual(original.ObjectPropertyAxioms.Count, reparsed.ObjectPropertyAxioms.Count);
        Assert.AreEqual(original.AssertionAxioms.Count, reparsed.AssertionAxioms.Count);

        System.Collections.Generic.IEnumerable<string> Xml<T>(System.Collections.Generic.List<T> axioms) where T : OWLAxiom
            => axioms.Select(ax => ax.GetXML()).OrderBy(x => x);

        CollectionAssert.AreEqual(Xml(original.DeclarationAxioms).ToList(), Xml(reparsed.DeclarationAxioms).ToList());
        CollectionAssert.AreEqual(Xml(original.ClassAxioms).ToList(), Xml(reparsed.ClassAxioms).ToList());
        CollectionAssert.AreEqual(Xml(original.ObjectPropertyAxioms).ToList(), Xml(reparsed.ObjectPropertyAxioms).ToList());
        CollectionAssert.AreEqual(Xml(original.AssertionAxioms).ToList(), Xml(reparsed.AssertionAxioms).ToList());

        //Serializing the reparsed ontology again must produce byte-identical Manchester output (idempotence)
        string manchesterDocument2 = OWLManchesterSerializer.SerializeOntology(reparsed);
        Assert.AreEqual(manchesterDocument, manchesterDocument2);
    }
    #endregion
}
