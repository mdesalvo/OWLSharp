/*
   Copyright 2014-2025 Marco De Salvo

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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLDataPropertyViewTest
{
    private OWLOntology Ontology { get; set; }
    private OWLDataPropertyView HasDataView { get; set; }
    private OWLDataPropertyView HasPersonalDataView { get; set; }
    private OWLDataPropertyView HasAgeView { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Human"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasData"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasPersonalData"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasAge"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:isNYearsOld"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasName"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John")))
            ],
            AnnotationAxioms = [
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("ex:hasAge"),
                    new OWLLiteral(new RDFPlainLiteral("This is the data property for human age"))),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO),
                    new RDFResource("ex:hasAge"),
                    new RDFResource("http://wikipedia.org/hasAge")),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                    new RDFResource("ex:hasAge"),
                    new OWLLiteral(RDFTypedLiteral.True))
            ],
            DataPropertyAxioms = [
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLDataProperty(new RDFResource("ex:hasPersonalData"))),
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("ex:hasPersonalData")),
                    new OWLDataProperty(new RDFResource("ex:hasData"))),
                new OWLSubDataPropertyOf(
                    new OWLDataProperty(new RDFResource("ex:hasName")),
                    new OWLDataProperty(new RDFResource("ex:hasPersonalData"))),
                new OWLEquivalentDataProperties([
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLDataProperty(new RDFResource("ex:isNYearsOld"))]),
                new OWLDisjointDataProperties([
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLDataProperty(new RDFResource("ex:hasName"))]),
                new OWLDataPropertyDomain(
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLClass(new RDFResource("ex:Human"))),
                new OWLDataPropertyRange(
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLDatatype(RDFVocabulary.XSD.INTEGER)),
                new OWLFunctionalDataProperty(new OWLDataProperty(new RDFResource("ex:hasAge")))
            ],
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLLiteral(new RDFTypedLiteral("35", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLNegativeDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:hasAge")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("33", RDFModelEnums.RDFDatatypes.XSD_INTEGER))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Human")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Human")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Human")),
                    new OWLNamedIndividual(new RDFResource("ex:John")))
            ]
        };

        HasDataView = new OWLDataPropertyView(new OWLDataProperty(new RDFResource("ex:hasData")), Ontology);
        HasPersonalDataView = new OWLDataPropertyView(new OWLDataProperty(new RDFResource("ex:hasPersonalData")), Ontology);
        HasAgeView = new OWLDataPropertyView(new OWLDataProperty(new RDFResource("ex:hasAge")), Ontology);
    }

    [TestMethod]
    public async Task ShouldGetSubDataPropertiesAsync()
    {
        Assert.HasCount(4, await HasDataView.SubDataPropertiesAsync());
        Assert.HasCount(3, await HasPersonalDataView.SubDataPropertiesAsync());
        Assert.IsEmpty(await HasAgeView.SubDataPropertiesAsync());
    }

    [TestMethod]
    public async Task ShouldGetSuperDataPropertiesAsync()
    {
        Assert.IsEmpty(await HasDataView.SuperDataPropertiesAsync());
        Assert.HasCount(1, await HasPersonalDataView.SuperDataPropertiesAsync());
        Assert.HasCount(2, await HasAgeView.SuperDataPropertiesAsync());
    }

    [TestMethod]
    public async Task ShouldGetEquivalentDataPropertiesAsync()
    {
        Assert.IsEmpty(await HasDataView.EquivalentDataPropertiesAsync());
        Assert.IsEmpty(await HasPersonalDataView.EquivalentDataPropertiesAsync());
        Assert.HasCount(1, await HasAgeView.EquivalentDataPropertiesAsync());
    }

    [TestMethod]
    public async Task ShouldGetDisjointDataPropertiesAsync()
    {
        Assert.IsEmpty(await HasDataView.DisjointDataPropertiesAsync());
        Assert.IsEmpty(await HasPersonalDataView.DisjointDataPropertiesAsync());
        Assert.HasCount(1, await HasAgeView.DisjointDataPropertiesAsync());
    }

    [TestMethod]
    public async Task ShouldGetDataAssertionsAsync()
    {
        Assert.IsEmpty(await HasDataView.DataAssertionsAsync());
        Assert.IsEmpty(await HasPersonalDataView.DataAssertionsAsync());
        Assert.HasCount(2, await HasAgeView.DataAssertionsAsync());
    }

    [TestMethod]
    public async Task ShouldGetNegativeDataAssertionsAsync()
    {
        Assert.IsEmpty(await HasDataView.NegativeDataAssertionsAsync());
        Assert.IsEmpty(await HasPersonalDataView.NegativeDataAssertionsAsync());
        Assert.HasCount(1, await HasAgeView.NegativeDataAssertionsAsync());
    }

    [TestMethod]
    public async Task ShouldGetDomainsAsync()
    {
        Assert.IsEmpty(await HasDataView.DomainsAsync());
        Assert.IsEmpty(await HasPersonalDataView.DomainsAsync());
        Assert.HasCount(1, await HasAgeView.DomainsAsync());
    }

    [TestMethod]
    public async Task ShouldGetRangesAsync()
    {
        Assert.IsEmpty(await HasDataView.RangesAsync());
        Assert.IsEmpty(await HasPersonalDataView.RangesAsync());
        Assert.HasCount(1, await HasAgeView.RangesAsync());
    }

    [TestMethod]
    public async Task ShouldGetDataAnnotationsAsync()
    {
        Assert.IsEmpty(await HasDataView.DataAnnotationsAsync());
        Assert.IsEmpty(await HasPersonalDataView.DataAnnotationsAsync());
        Assert.HasCount(2, await HasAgeView.DataAnnotationsAsync());
    }

    [TestMethod]
    public async Task ShouldGetObjectAnnotationsAsync()
    {
        Assert.IsEmpty(await HasDataView.ObjectAnnotationsAsync());
        Assert.IsEmpty(await HasPersonalDataView.ObjectAnnotationsAsync());
        Assert.HasCount(1, await HasAgeView.ObjectAnnotationsAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsDeprecatedAsync()
    {
        Assert.IsFalse(await HasDataView.IsDeprecatedAsync());
        Assert.IsFalse(await HasPersonalDataView.IsDeprecatedAsync());
        Assert.IsTrue(await HasAgeView.IsDeprecatedAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsFunctionalAsync()
    {
        Assert.IsFalse(await HasDataView.IsFunctionalAsync());
        Assert.IsFalse(await HasPersonalDataView.IsFunctionalAsync());
        Assert.IsTrue(await HasAgeView.IsFunctionalAsync());
    }
}