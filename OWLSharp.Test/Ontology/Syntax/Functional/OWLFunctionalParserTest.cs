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

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLFunctionalParserTest
{
    #region Utilities
    //Every test document is implicitly prefixed with a "pz:" namespace and a default (empty) prefix,
    //so that both PrefixedName and default-prefix entity references can be exercised. The default
    //rdf/rdfs/xsd/owl prefixes are already seeded by OWLOntology's own constructor (mirrored by the
    //parser's own prefix map seed), so rdfs:/xsd: references never need an explicit Prefix(...) here
    private const string Prologue =
        "Prefix(:=<http://example.org/pz#>)\nPrefix(pz:=<http://example.org/pz#>)\nOntology(<http://example.org/pz>\n";

    private static OWLOntology Parse(string axioms)
        => OWLFunctionalParser.DeserializeOntology(Prologue + axioms + "\n)");
    #endregion

    #region Tests (document structure)
    [TestMethod]
    public void ShouldParseSinglePrefixDeclaration()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Prefix(pz:=<http://example.org/pz#>)\nOntology(<http://example.org/pz>)");

        Assert.IsTrue(ontology.Prefixes.Any(pfx => pfx.Name == "pz" && pfx.IRI == "http://example.org/pz#"));
    }

    [TestMethod]
    public void ShouldParseMultiplePrefixDeclarationsIncludingDefault()
    {
        OWLOntology ontology = Parse("Declaration(Class(:Pizza))\nDeclaration(Class(pz:Topping))");

        Assert.IsTrue(ontology.Prefixes.Any(pfx => pfx.Name == "" && pfx.IRI == "http://example.org/pz#"));
        Assert.IsTrue(ontology.Prefixes.Any(pfx => pfx.Name == "pz" && pfx.IRI == "http://example.org/pz#"));
        Assert.AreEqual("http://example.org/pz#Pizza", ((OWLClass)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Topping", ((OWLClass)ontology.DeclarationAxioms[1].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseOntologyWithIRI()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology("Ontology(<http://example.org/pz>)");

        Assert.AreEqual("http://example.org/pz", ontology.IRI);
        Assert.IsNull(ontology.VersionIRI);
    }

    [TestMethod]
    public void ShouldParseOntologyWithIRIAndVersionIRI()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Ontology(<http://example.org/pz> <http://example.org/pz/1.0>)");

        Assert.AreEqual("http://example.org/pz", ontology.IRI);
        Assert.AreEqual("http://example.org/pz/1.0", ontology.VersionIRI);
    }

    [TestMethod]
    public void ShouldParseAnonymousOntology()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Prefix(pz:=<http://example.org/pz#>)\nOntology(Declaration(Class(pz:A)))");

        Assert.IsNull(ontology.IRI);
        Assert.HasCount(1, ontology.DeclarationAxioms);
    }

    [TestMethod]
    public void ShouldParseSingleImport()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Ontology(<http://example.org/pz>\nImport(<http://example.org/a>)\n)");

        Assert.HasCount(1, ontology.Imports);
        Assert.AreEqual("http://example.org/a", ontology.Imports[0].IRI);
    }

    [TestMethod]
    public void ShouldParseMultipleImports()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Ontology(<http://example.org/pz>\nImport(<http://example.org/a>)\nImport(<http://example.org/b>)\n)");

        Assert.HasCount(2, ontology.Imports);
        Assert.AreEqual("http://example.org/a", ontology.Imports[0].IRI);
        Assert.AreEqual("http://example.org/b", ontology.Imports[1].IRI);
    }

    [TestMethod]
    public void ShouldParseSingleOntologyAnnotation()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Ontology(<http://example.org/pz>\nAnnotation(rdfs:comment \"a\")\n)");

        Assert.HasCount(1, ontology.Annotations);
        Assert.AreEqual("a", ontology.Annotations[0].ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseMultipleOntologyAnnotations()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Ontology(<http://example.org/pz>\nAnnotation(rdfs:comment \"a\")\nAnnotation(rdfs:label \"b\")\n)");

        Assert.HasCount(2, ontology.Annotations);
        Assert.AreEqual("a", ontology.Annotations[0].ValueLiteral.Value);
        Assert.AreEqual("b", ontology.Annotations[1].ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseNestedOntologyAnnotation()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology(
            "Ontology(<http://example.org/pz>\nAnnotation(Annotation(rdfs:label \"meta\") rdfs:comment \"outer\")\n)");

        Assert.HasCount(1, ontology.Annotations);
        Assert.AreEqual("outer", ontology.Annotations[0].ValueLiteral.Value);
        Assert.IsNotNull(ontology.Annotations[0].Annotation);
        Assert.AreEqual("meta", ontology.Annotations[0].Annotation.ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseDocumentWithoutAnyAxiomAsEmptyOntology()
    {
        OWLOntology ontology = OWLFunctionalParser.DeserializeOntology("Ontology(<http://example.org/pz>)");

        Assert.AreEqual("http://example.org/pz", ontology.IRI);
        Assert.IsEmpty(ontology.DeclarationAxioms);
        Assert.IsEmpty(ontology.ClassAxioms);
        Assert.IsEmpty(ontology.AssertionAxioms);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnUnrecognizedAxiomKeyword()
        => Assert.ThrowsExactly<OWLException>(() => Parse("FizzBuzz(pz:A pz:B)"));

    [TestMethod]
    public void ShouldThrowExceptionOnImportAfterFirstAxiom()
        //Per the grammar, "Import" only ever appears in the directlyImportsDocuments block, strictly
        //before any Axiom: once ParseOntology's Import-consuming loop has moved on to ParseAxiom, a
        //further "Import(...)" is looked up as an Axiom keyword instead and rejected, since it is not
        //one of the Axiom production alternatives
        => Assert.ThrowsExactly<OWLException>(() => Parse(
            "Declaration(Class(pz:A))\nImport(<http://example.org/late>)"));

    [TestMethod]
    public void ShouldThrowExceptionOnMissingClosingParenthesis()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalParser.DeserializeOntology(
            "Prefix(pz:=<http://example.org/pz#>)\nOntology(<http://example.org/pz>\nDeclaration(Class(pz:A))"));

    [TestMethod]
    public void ShouldThrowExceptionOnPrematureEndOfDocument()
        => Assert.ThrowsExactly<OWLException>(() => OWLFunctionalParser.DeserializeOntology(
            "Prefix(pz:=<http://example.org/pz#>)\nOntology(<http://example.org/pz>\nDeclaration(Class(pz:A"));
    #endregion

    #region Tests (entities, literals, individuals)
    [TestMethod]
    public void ShouldParseClassDeclaration()
    {
        OWLOntology ontology = Parse("Declaration(Class(pz:Pizza))");

        Assert.HasCount(1, ontology.DeclarationAxioms);
        Assert.IsInstanceOfType<OWLClass>(ontology.DeclarationAxioms[0].Entity);
        Assert.AreEqual("http://example.org/pz#Pizza", ((OWLClass)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseDatatypeDeclaration()
    {
        OWLOntology ontology = Parse("Declaration(Datatype(pz:PositiveInt))");

        Assert.IsInstanceOfType<OWLDatatype>(ontology.DeclarationAxioms[0].Entity);
        Assert.AreEqual("http://example.org/pz#PositiveInt", ((OWLDatatype)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectPropertyDeclaration()
    {
        OWLOntology ontology = Parse("Declaration(ObjectProperty(pz:hasTopping))");

        Assert.IsInstanceOfType<OWLObjectProperty>(ontology.DeclarationAxioms[0].Entity);
        Assert.AreEqual("http://example.org/pz#hasTopping", ((OWLObjectProperty)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseDataPropertyDeclaration()
    {
        OWLOntology ontology = Parse("Declaration(DataProperty(pz:hasCalories))");

        Assert.IsInstanceOfType<OWLDataProperty>(ontology.DeclarationAxioms[0].Entity);
        Assert.AreEqual("http://example.org/pz#hasCalories", ((OWLDataProperty)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseAnnotationPropertyDeclaration()
    {
        OWLOntology ontology = Parse("Declaration(AnnotationProperty(pz:note))");

        Assert.IsInstanceOfType<OWLAnnotationProperty>(ontology.DeclarationAxioms[0].Entity);
        Assert.AreEqual("http://example.org/pz#note", ((OWLAnnotationProperty)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseNamedIndividualDeclaration()
    {
        OWLOntology ontology = Parse("Declaration(NamedIndividual(pz:Peter))");

        Assert.IsInstanceOfType<OWLNamedIndividual>(ontology.DeclarationAxioms[0].Entity);
        Assert.AreEqual("http://example.org/pz#Peter", ((OWLNamedIndividual)ontology.DeclarationAxioms[0].Entity).GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseAnonymousIndividualAsAssertionSubject()
    {
        OWLOntology ontology = Parse("ClassAssertion(pz:Pizza _:anon1)");

        OWLClassAssertion ca = (OWLClassAssertion)ontology.AssertionAxioms[0];
        Assert.IsInstanceOfType<OWLAnonymousIndividual>(ca.IndividualExpression);
        Assert.AreEqual("anon1", ((OWLAnonymousIndividual)ca.IndividualExpression).NodeID);
    }

    [TestMethod]
    public void ShouldParseAnonymousIndividualAsAssertionObject()
    {
        OWLOntology ontology = Parse("ObjectPropertyAssertion(pz:hasTopping pz:Margherita _:anon2)");

        OWLObjectPropertyAssertion opa = (OWLObjectPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.IsInstanceOfType<OWLAnonymousIndividual>(opa.TargetIndividualExpression);
        Assert.AreEqual("anon2", ((OWLAnonymousIndividual)opa.TargetIndividualExpression).NodeID);
    }

    [TestMethod]
    public void ShouldParseTypedLiteral()
    {
        OWLOntology ontology = Parse("DataPropertyAssertion(pz:hasCalories pz:Margherita \"850\"^^xsd:integer)");

        OWLDataPropertyAssertion dpa = (OWLDataPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("850", dpa.Literal.Value);
        Assert.AreEqual("http://www.w3.org/2001/XMLSchema#integer", dpa.Literal.DatatypeIRI);
    }

    [TestMethod]
    public void ShouldParsePlainLiteralWithLanguage()
    {
        OWLOntology ontology = Parse("AnnotationAssertion(rdfs:label pz:Pizza \"Pizza\"@it)");

        OWLAnnotationAssertion ann = (OWLAnnotationAssertion)ontology.AnnotationAxioms[0];
        Assert.AreEqual("Pizza", ann.ValueLiteral.Value);
        Assert.AreEqual("it", ann.ValueLiteral.Language);
    }

    [TestMethod]
    public void ShouldParsePlainLiteralWithoutLanguageOrDatatype()
    {
        OWLOntology ontology = Parse("DataPropertyAssertion(pz:hasName pz:Margherita \"Margherita\")");

        OWLDataPropertyAssertion dpa = (OWLDataPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("Margherita", dpa.Literal.Value);
        Assert.IsNull(dpa.Literal.DatatypeIRI);
        Assert.IsNull(dpa.Literal.Language);
    }
    #endregion

    #region Tests (property expressions, data ranges)
    [TestMethod]
    public void ShouldParseObjectInverseOf()
    {
        OWLOntology ontology = Parse("ObjectPropertyAssertion(ObjectInverseOf(pz:hasTopping) pz:Mozzarella pz:Margherita)");

        OWLObjectPropertyAssertion opa = (OWLObjectPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.IsInstanceOfType<OWLObjectInverseOf>(opa.ObjectPropertyExpression);
        Assert.AreEqual("http://example.org/pz#hasTopping",
            ((OWLObjectInverseOf)opa.ObjectPropertyExpression).ObjectProperty.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseDataIntersectionOf()
    {
        OWLOntology ontology = Parse("DataPropertyRange(pz:hasCode DataIntersectionOf(xsd:string xsd:token))");

        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[0];
        Assert.IsInstanceOfType<OWLDataIntersectionOf>(range.DataRangeExpression);
        Assert.HasCount(2, ((OWLDataIntersectionOf)range.DataRangeExpression).DataRangeExpressions);
    }

    [TestMethod]
    public void ShouldParseDataUnionOf()
    {
        OWLOntology ontology = Parse("DataPropertyRange(pz:hasCode DataUnionOf(xsd:string xsd:integer))");

        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[0];
        Assert.IsInstanceOfType<OWLDataUnionOf>(range.DataRangeExpression);
        Assert.HasCount(2, ((OWLDataUnionOf)range.DataRangeExpression).DataRangeExpressions);
    }

    [TestMethod]
    public void ShouldParseDataComplementOf()
    {
        OWLOntology ontology = Parse("DataPropertyRange(pz:hasCode DataComplementOf(xsd:string))");

        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[0];
        Assert.IsInstanceOfType<OWLDataComplementOf>(range.DataRangeExpression);
        Assert.IsInstanceOfType<OWLDatatype>(((OWLDataComplementOf)range.DataRangeExpression).DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParseDataOneOf()
    {
        OWLOntology ontology = Parse("DataPropertyRange(pz:hasSize DataOneOf(\"S\" \"M\" \"L\"))");

        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[0];
        Assert.IsInstanceOfType<OWLDataOneOf>(range.DataRangeExpression);
        Assert.HasCount(3, ((OWLDataOneOf)range.DataRangeExpression).Literals);
    }

    [TestMethod]
    public void ShouldParseDatatypeRestrictionWithSingleFacet()
    {
        OWLOntology ontology = Parse(
            "DataPropertyRange(pz:hasCalories DatatypeRestriction(xsd:integer xsd:minInclusive \"0\"^^xsd:integer))");

        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[0];
        OWLDatatypeRestriction restriction = (OWLDatatypeRestriction)range.DataRangeExpression;
        Assert.AreEqual("http://www.w3.org/2001/XMLSchema#integer", restriction.Datatype.GetIRI().ToString());
        Assert.HasCount(1, restriction.FacetRestrictions);
        Assert.AreEqual("http://www.w3.org/2001/XMLSchema#minInclusive", restriction.FacetRestrictions[0].FacetIRI);
        Assert.AreEqual("0", restriction.FacetRestrictions[0].Literal.Value);
    }

    [TestMethod]
    public void ShouldParseDatatypeRestrictionWithMultipleFacets()
    {
        OWLOntology ontology = Parse(
            "DataPropertyRange(pz:hasCalories DatatypeRestriction(xsd:integer "
            + "xsd:minInclusive \"0\"^^xsd:integer xsd:maxInclusive \"9999\"^^xsd:integer))");

        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[0];
        OWLDatatypeRestriction restriction = (OWLDatatypeRestriction)range.DataRangeExpression;
        Assert.HasCount(2, restriction.FacetRestrictions);
        Assert.AreEqual("http://www.w3.org/2001/XMLSchema#maxInclusive", restriction.FacetRestrictions[1].FacetIRI);
        Assert.AreEqual("9999", restriction.FacetRestrictions[1].Literal.Value);
    }
    #endregion

    #region Tests (class expressions)
    [TestMethod]
    public void ShouldParseObjectIntersectionOf()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectIntersectionOf(pz:A pz:B))");

        OWLObjectIntersectionOf inter = (OWLObjectIntersectionOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(2, inter.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseObjectUnionOf()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectUnionOf(pz:A pz:B pz:C))");

        OWLObjectUnionOf union = (OWLObjectUnionOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(3, union.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseObjectComplementOf()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectComplementOf(pz:A))");

        Assert.IsInstanceOfType<OWLObjectComplementOf>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseObjectOneOf()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectOneOf(pz:A pz:B))");

        OWLObjectOneOf oneOf = (OWLObjectOneOf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.HasCount(2, oneOf.IndividualExpressions);
    }

    [TestMethod]
    public void ShouldParseObjectSomeValuesFrom()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectSomeValuesFrom(pz:hasTopping pz:Mozzarella))");

        OWLObjectSomeValuesFrom some = (OWLObjectSomeValuesFrom)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("http://example.org/pz#hasTopping", some.ObjectPropertyExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Mozzarella", some.ClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectAllValuesFrom()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectAllValuesFrom(pz:hasTopping pz:Mozzarella))");

        Assert.IsInstanceOfType<OWLObjectAllValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseObjectHasValue()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectHasValue(pz:hasTopping pz:Mozzarella))");

        OWLObjectHasValue hv = (OWLObjectHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("http://example.org/pz#Mozzarella", hv.IndividualExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectHasSelf()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectHasSelf(pz:likes))");

        OWLObjectHasSelf hs = (OWLObjectHasSelf)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("http://example.org/pz#likes", hs.ObjectPropertyExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseUnqualifiedObjectMinCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectMinCardinality(2 pz:hasTopping))");

        OWLObjectMinCardinality min = (OWLObjectMinCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("2", min.Cardinality);
        Assert.IsNull(min.ClassExpression);
    }

    [TestMethod]
    public void ShouldParseQualifiedObjectMinCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectMinCardinality(2 pz:hasTopping pz:Topping))");

        OWLObjectMinCardinality min = (OWLObjectMinCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("2", min.Cardinality);
        Assert.IsNotNull(min.ClassExpression);
        Assert.AreEqual("http://example.org/pz#Topping", min.ClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectMaxCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectMaxCardinality(5 pz:hasTopping))");

        OWLObjectMaxCardinality max = (OWLObjectMaxCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("5", max.Cardinality);
    }

    [TestMethod]
    public void ShouldParseObjectExactCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X ObjectExactCardinality(1 pz:hasTopping pz:Topping))");

        OWLObjectExactCardinality exact = (OWLObjectExactCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("1", exact.Cardinality);
        Assert.IsNotNull(exact.ClassExpression);
    }

    [TestMethod]
    public void ShouldParseDataSomeValuesFrom()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X DataSomeValuesFrom(pz:hasCalories xsd:integer))");

        Assert.IsInstanceOfType<OWLDataSomeValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseDataAllValuesFrom()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X DataAllValuesFrom(pz:hasCalories xsd:integer))");

        Assert.IsInstanceOfType<OWLDataAllValuesFrom>(((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression);
    }

    [TestMethod]
    public void ShouldParseDataHasValue()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X DataHasValue(pz:hasCalories \"300\"^^xsd:integer))");

        OWLDataHasValue hv = (OWLDataHasValue)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("300", hv.Literal.Value);
    }

    [TestMethod]
    public void ShouldParseUnqualifiedDataMinCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X DataMinCardinality(1 pz:hasCalories))");

        OWLDataMinCardinality min = (OWLDataMinCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("1", min.Cardinality);
        Assert.IsNull(min.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParseQualifiedDataMinCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X DataMinCardinality(1 pz:hasCalories xsd:integer))");

        OWLDataMinCardinality min = (OWLDataMinCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("1", min.Cardinality);
        Assert.IsNotNull(min.DataRangeExpression);
        Assert.IsInstanceOfType<OWLDatatype>(min.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParseDataMaxCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X DataMaxCardinality(3 pz:hasCalories xsd:integer))");

        OWLDataMaxCardinality max = (OWLDataMaxCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("3", max.Cardinality);
    }

    [TestMethod]
    public void ShouldParseDataExactCardinality()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:X DataExactCardinality(2 pz:hasCalories))");

        OWLDataExactCardinality exact = (OWLDataExactCardinality)((OWLSubClassOf)ontology.ClassAxioms[0]).SuperClassExpression;
        Assert.AreEqual("2", exact.Cardinality);
        Assert.IsNull(exact.DataRangeExpression);
    }
    #endregion

    #region Tests (class axioms)
    [TestMethod]
    public void ShouldParseSubClassOf()
    {
        OWLOntology ontology = Parse("SubClassOf(pz:Margherita pz:Pizza)");

        OWLSubClassOf sco = (OWLSubClassOf)ontology.ClassAxioms[0];
        Assert.AreEqual("http://example.org/pz#Margherita", sco.SubClassExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Pizza", sco.SuperClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseEquivalentClasses()
    {
        OWLOntology ontology = Parse("EquivalentClasses(pz:A pz:B pz:C)");

        OWLEquivalentClasses eqc = (OWLEquivalentClasses)ontology.ClassAxioms[0];
        Assert.HasCount(3, eqc.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseDisjointClasses()
    {
        OWLOntology ontology = Parse("DisjointClasses(pz:A pz:B)");

        OWLDisjointClasses dcc = (OWLDisjointClasses)ontology.ClassAxioms[0];
        Assert.HasCount(2, dcc.ClassExpressions);
    }

    [TestMethod]
    public void ShouldParseDisjointUnion()
    {
        OWLOntology ontology = Parse("DisjointUnion(pz:Pizza pz:Margherita pz:Marinara)");

        OWLDisjointUnion dju = (OWLDisjointUnion)ontology.ClassAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", dju.ClassIRI.GetIRI().ToString());
        Assert.HasCount(2, dju.ClassExpressions);
    }
    #endregion

    #region Tests (object property axioms)
    [TestMethod]
    public void ShouldParseSubObjectPropertyOfSimpleForm()
    {
        OWLOntology ontology = Parse("SubObjectPropertyOf(pz:hasTopping pz:hasIngredient)");

        OWLSubObjectPropertyOf spo = (OWLSubObjectPropertyOf)ontology.ObjectPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasTopping", spo.SubObjectPropertyExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#hasIngredient", spo.SuperObjectPropertyExpression.GetIRI().ToString());
        Assert.IsNull(spo.SubObjectPropertyChain);
    }

    [TestMethod]
    public void ShouldParseSubObjectPropertyOfChainForm()
    {
        OWLOntology ontology = Parse(
            "SubObjectPropertyOf(ObjectPropertyChain(pz:hasParent pz:hasParent) pz:hasGrandparent)");

        OWLSubObjectPropertyOf spo = (OWLSubObjectPropertyOf)ontology.ObjectPropertyAxioms[0];
        Assert.IsNull(spo.SubObjectPropertyExpression);
        Assert.IsNotNull(spo.SubObjectPropertyChain);
        Assert.HasCount(2, spo.SubObjectPropertyChain.ObjectPropertyExpressions);
        Assert.AreEqual("http://example.org/pz#hasGrandparent", spo.SuperObjectPropertyExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseEquivalentObjectProperties()
    {
        OWLOntology ontology = Parse("EquivalentObjectProperties(pz:hasTopping pz:hasIngredient)");

        OWLEquivalentObjectProperties eqp = (OWLEquivalentObjectProperties)ontology.ObjectPropertyAxioms[0];
        Assert.HasCount(2, eqp.ObjectPropertyExpressions);
    }

    [TestMethod]
    public void ShouldParseDisjointObjectProperties()
    {
        OWLOntology ontology = Parse("DisjointObjectProperties(pz:hasTopping pz:excludes)");

        OWLDisjointObjectProperties dop = (OWLDisjointObjectProperties)ontology.ObjectPropertyAxioms[0];
        Assert.HasCount(2, dop.ObjectPropertyExpressions);
    }

    [TestMethod]
    public void ShouldParseInverseObjectProperties()
    {
        OWLOntology ontology = Parse("InverseObjectProperties(pz:hasTopping pz:isToppingOf)");

        OWLInverseObjectProperties inv = (OWLInverseObjectProperties)ontology.ObjectPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasTopping", inv.LeftObjectPropertyExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#isToppingOf", inv.RightObjectPropertyExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectPropertyDomain()
    {
        OWLOntology ontology = Parse("ObjectPropertyDomain(pz:hasTopping pz:Pizza)");

        OWLObjectPropertyDomain domain = (OWLObjectPropertyDomain)ontology.ObjectPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", domain.ClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseObjectPropertyRange()
    {
        OWLOntology ontology = Parse("ObjectPropertyRange(pz:hasTopping pz:Topping)");

        OWLObjectPropertyRange range = (OWLObjectPropertyRange)ontology.ObjectPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#Topping", range.ClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseFunctionalObjectProperty()
    {
        OWLOntology ontology = Parse("FunctionalObjectProperty(pz:hasTopping)");

        Assert.IsInstanceOfType<OWLFunctionalObjectProperty>(ontology.ObjectPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseInverseFunctionalObjectProperty()
    {
        OWLOntology ontology = Parse("InverseFunctionalObjectProperty(pz:hasTopping)");

        Assert.IsInstanceOfType<OWLInverseFunctionalObjectProperty>(ontology.ObjectPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseReflexiveObjectProperty()
    {
        OWLOntology ontology = Parse("ReflexiveObjectProperty(pz:knows)");

        Assert.IsInstanceOfType<OWLReflexiveObjectProperty>(ontology.ObjectPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseIrreflexiveObjectProperty()
    {
        OWLOntology ontology = Parse("IrreflexiveObjectProperty(pz:marriedTo)");

        Assert.IsInstanceOfType<OWLIrreflexiveObjectProperty>(ontology.ObjectPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseSymmetricObjectProperty()
    {
        OWLOntology ontology = Parse("SymmetricObjectProperty(pz:isSiblingOf)");

        Assert.IsInstanceOfType<OWLSymmetricObjectProperty>(ontology.ObjectPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseAsymmetricObjectProperty()
    {
        OWLOntology ontology = Parse("AsymmetricObjectProperty(pz:hasParent)");

        Assert.IsInstanceOfType<OWLAsymmetricObjectProperty>(ontology.ObjectPropertyAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseTransitiveObjectProperty()
    {
        OWLOntology ontology = Parse("TransitiveObjectProperty(pz:hasAncestor)");

        Assert.IsInstanceOfType<OWLTransitiveObjectProperty>(ontology.ObjectPropertyAxioms[0]);
    }
    #endregion

    #region Tests (data property axioms)
    [TestMethod]
    public void ShouldParseSubDataPropertyOf()
    {
        OWLOntology ontology = Parse("SubDataPropertyOf(pz:hasCalories pz:hasNumericValue)");

        OWLSubDataPropertyOf sdp = (OWLSubDataPropertyOf)ontology.DataPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasCalories", sdp.SubDataProperty.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#hasNumericValue", sdp.SuperDataProperty.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseEquivalentDataProperties()
    {
        OWLOntology ontology = Parse("EquivalentDataProperties(pz:hasCalories pz:hasKcal)");

        OWLEquivalentDataProperties eqd = (OWLEquivalentDataProperties)ontology.DataPropertyAxioms[0];
        Assert.HasCount(2, eqd.DataProperties);
    }

    [TestMethod]
    public void ShouldParseDisjointDataProperties()
    {
        OWLOntology ontology = Parse("DisjointDataProperties(pz:hasCalories pz:hasName)");

        OWLDisjointDataProperties ddp = (OWLDisjointDataProperties)ontology.DataPropertyAxioms[0];
        Assert.HasCount(2, ddp.DataProperties);
    }

    [TestMethod]
    public void ShouldParseDataPropertyDomain()
    {
        OWLOntology ontology = Parse("DataPropertyDomain(pz:hasCalories pz:Pizza)");

        OWLDataPropertyDomain domain = (OWLDataPropertyDomain)ontology.DataPropertyAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", domain.ClassExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseDataPropertyRange()
    {
        OWLOntology ontology = Parse("DataPropertyRange(pz:hasCalories xsd:integer)");

        OWLDataPropertyRange range = (OWLDataPropertyRange)ontology.DataPropertyAxioms[0];
        Assert.IsInstanceOfType<OWLDatatype>(range.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParseFunctionalDataProperty()
    {
        OWLOntology ontology = Parse("FunctionalDataProperty(pz:hasCalories)");

        Assert.IsInstanceOfType<OWLFunctionalDataProperty>(ontology.DataPropertyAxioms[0]);
    }
    #endregion

    #region Tests (datatype definition and keys)
    [TestMethod]
    public void ShouldParseDatatypeDefinition()
    {
        OWLOntology ontology = Parse(
            "DatatypeDefinition(pz:PositiveInt DatatypeRestriction(xsd:integer xsd:minInclusive \"0\"^^xsd:integer))");

        OWLDatatypeDefinition def = ontology.DatatypeDefinitionAxioms[0];
        Assert.AreEqual("http://example.org/pz#PositiveInt", def.Datatype.GetIRI().ToString());
        Assert.IsInstanceOfType<OWLDatatypeRestriction>(def.DataRangeExpression);
    }

    [TestMethod]
    public void ShouldParseHasKeyWithOnlyObjectProperties()
    {
        OWLOntology ontology = Parse("HasKey(pz:Pizza (pz:hasTopping) ())");

        OWLHasKey key = ontology.KeyAxioms[0];
        Assert.HasCount(1, key.ObjectPropertyExpressions);
        Assert.IsEmpty(key.DataProperties);
    }

    [TestMethod]
    public void ShouldParseHasKeyWithOnlyDataProperties()
    {
        OWLOntology ontology = Parse("HasKey(pz:Pizza () (pz:hasCode))");

        OWLHasKey key = ontology.KeyAxioms[0];
        Assert.IsEmpty(key.ObjectPropertyExpressions);
        Assert.HasCount(1, key.DataProperties);
    }

    [TestMethod]
    public void ShouldParseHasKeyWithBothObjectAndDataProperties()
    {
        OWLOntology ontology = Parse("HasKey(pz:Pizza (pz:hasTopping) (pz:hasCode))");

        OWLHasKey key = ontology.KeyAxioms[0];
        Assert.HasCount(1, key.ObjectPropertyExpressions);
        Assert.HasCount(1, key.DataProperties);
        Assert.AreEqual("http://example.org/pz#hasTopping", key.ObjectPropertyExpressions[0].GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#hasCode", key.DataProperties[0].GetIRI().ToString());
    }
    #endregion

    #region Tests (assertions)
    [TestMethod]
    public void ShouldParseSameIndividual()
    {
        OWLOntology ontology = Parse("SameIndividual(pz:A pz:B pz:C)");

        OWLSameIndividual si = (OWLSameIndividual)ontology.AssertionAxioms[0];
        Assert.HasCount(3, si.IndividualExpressions);
    }

    [TestMethod]
    public void ShouldParseDifferentIndividuals()
    {
        OWLOntology ontology = Parse("DifferentIndividuals(pz:A pz:B)");

        OWLDifferentIndividuals di = (OWLDifferentIndividuals)ontology.AssertionAxioms[0];
        Assert.HasCount(2, di.IndividualExpressions);
    }

    [TestMethod]
    public void ShouldParseClassAssertionWithNamedIndividual()
    {
        OWLOntology ontology = Parse("ClassAssertion(pz:Pizza pz:Margherita)");

        OWLClassAssertion ca = (OWLClassAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", ca.ClassExpression.GetIRI().ToString());
        Assert.IsInstanceOfType<OWLNamedIndividual>(ca.IndividualExpression);
        Assert.AreEqual("http://example.org/pz#Margherita", ca.IndividualExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseClassAssertionWithAnonymousIndividual()
    {
        OWLOntology ontology = Parse("ClassAssertion(pz:Pizza _:anon3)");

        OWLClassAssertion ca = (OWLClassAssertion)ontology.AssertionAxioms[0];
        Assert.IsInstanceOfType<OWLAnonymousIndividual>(ca.IndividualExpression);
    }

    [TestMethod]
    public void ShouldParseObjectPropertyAssertion()
    {
        OWLOntology ontology = Parse("ObjectPropertyAssertion(pz:hasTopping pz:Margherita pz:Mozzarella)");

        OWLObjectPropertyAssertion opa = (OWLObjectPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasTopping", opa.ObjectPropertyExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Margherita", opa.SourceIndividualExpression.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Mozzarella", opa.TargetIndividualExpression.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseNegativeObjectPropertyAssertion()
    {
        OWLOntology ontology = Parse("NegativeObjectPropertyAssertion(pz:hasTopping pz:Margherita pz:Anchovy)");

        Assert.IsInstanceOfType<OWLNegativeObjectPropertyAssertion>(ontology.AssertionAxioms[0]);
    }

    [TestMethod]
    public void ShouldParseDataPropertyAssertion()
    {
        OWLOntology ontology = Parse("DataPropertyAssertion(pz:hasCalories pz:Margherita \"850\"^^xsd:integer)");

        OWLDataPropertyAssertion dpa = (OWLDataPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.AreEqual("http://example.org/pz#hasCalories", dpa.DataProperty.GetIRI().ToString());
        Assert.AreEqual("850", dpa.Literal.Value);
    }

    [TestMethod]
    public void ShouldParseNegativeDataPropertyAssertion()
    {
        OWLOntology ontology = Parse("NegativeDataPropertyAssertion(pz:hasCalories pz:Margherita \"0\"^^xsd:integer)");

        Assert.IsInstanceOfType<OWLNegativeDataPropertyAssertion>(ontology.AssertionAxioms[0]);
    }
    #endregion

    #region Tests (annotation axioms)
    [TestMethod]
    public void ShouldParseAnnotationAssertionWithIRIValue()
    {
        OWLOntology ontology = Parse("AnnotationAssertion(rdfs:seeAlso pz:Pizza pz:Topping)");

        OWLAnnotationAssertion ann = (OWLAnnotationAssertion)ontology.AnnotationAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", ann.SubjectIRI);
        Assert.AreEqual("http://example.org/pz#Topping", ann.ValueIRI);
        Assert.IsNull(ann.ValueLiteral);
    }

    [TestMethod]
    public void ShouldParseAnnotationAssertionWithLiteralValue()
    {
        OWLOntology ontology = Parse("AnnotationAssertion(rdfs:label pz:Pizza \"Pizza\")");

        OWLAnnotationAssertion ann = (OWLAnnotationAssertion)ontology.AnnotationAxioms[0];
        Assert.AreEqual("http://example.org/pz#Pizza", ann.SubjectIRI);
        Assert.AreEqual("Pizza", ann.ValueLiteral.Value);
        Assert.IsNull(ann.ValueIRI);
    }

    [TestMethod]
    public void ShouldParseSubAnnotationPropertyOf()
    {
        OWLOntology ontology = Parse("SubAnnotationPropertyOf(pz:note rdfs:comment)");

        OWLSubAnnotationPropertyOf sap = (OWLSubAnnotationPropertyOf)ontology.AnnotationAxioms[0];
        Assert.AreEqual("http://example.org/pz#note", sap.SubAnnotationProperty.GetIRI().ToString());
        Assert.AreEqual("http://www.w3.org/2000/01/rdf-schema#comment", sap.SuperAnnotationProperty.GetIRI().ToString());
    }

    [TestMethod]
    public void ShouldParseAnnotationPropertyDomain()
    {
        OWLOntology ontology = Parse("AnnotationPropertyDomain(pz:note pz:Pizza)");

        OWLAnnotationPropertyDomain domain = (OWLAnnotationPropertyDomain)ontology.AnnotationAxioms[0];
        Assert.AreEqual("http://example.org/pz#note", domain.AnnotationProperty.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Pizza", domain.IRI);
    }

    [TestMethod]
    public void ShouldParseAnnotationPropertyRange()
    {
        OWLOntology ontology = Parse("AnnotationPropertyRange(pz:note pz:Pizza)");

        OWLAnnotationPropertyRange range = (OWLAnnotationPropertyRange)ontology.AnnotationAxioms[0];
        Assert.AreEqual("http://example.org/pz#note", range.AnnotationProperty.GetIRI().ToString());
        Assert.AreEqual("http://example.org/pz#Pizza", range.IRI);
    }
    #endregion

    #region Tests (axiom annotations)
    [TestMethod]
    public void ShouldParseAxiomAnnotationsOnSubClassOf()
    {
        OWLOntology ontology = Parse("SubClassOf(Annotation(rdfs:comment \"why\") pz:Margherita pz:Pizza)");

        OWLSubClassOf sco = (OWLSubClassOf)ontology.ClassAxioms[0];
        Assert.HasCount(1, sco.Annotations);
        Assert.AreEqual("why", sco.Annotations[0].ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseAxiomAnnotationsOnClassAssertion()
    {
        OWLOntology ontology = Parse("ClassAssertion(Annotation(rdfs:comment \"asserted\") pz:Pizza pz:Margherita)");

        OWLClassAssertion ca = (OWLClassAssertion)ontology.AssertionAxioms[0];
        Assert.HasCount(1, ca.Annotations);
        Assert.AreEqual("asserted", ca.Annotations[0].ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseAxiomAnnotationsOnObjectPropertyAssertion()
    {
        OWLOntology ontology = Parse(
            "ObjectPropertyAssertion(Annotation(rdfs:comment \"confirmed\") pz:hasTopping pz:Margherita pz:Mozzarella)");

        OWLObjectPropertyAssertion opa = (OWLObjectPropertyAssertion)ontology.AssertionAxioms[0];
        Assert.HasCount(1, opa.Annotations);
        Assert.AreEqual("confirmed", opa.Annotations[0].ValueLiteral.Value);
    }

    [TestMethod]
    public void ShouldParseMultipleAxiomAnnotationsOnDisjointClasses()
    {
        OWLOntology ontology = Parse(
            "DisjointClasses(Annotation(rdfs:comment \"c1\") Annotation(rdfs:label \"c2\") pz:A pz:B)");

        OWLDisjointClasses dcc = (OWLDisjointClasses)ontology.ClassAxioms[0];
        Assert.HasCount(2, dcc.Annotations);
        Assert.AreEqual("c1", dcc.Annotations[0].ValueLiteral.Value);
        Assert.AreEqual("c2", dcc.Annotations[1].ValueLiteral.Value);
    }
    #endregion
}
