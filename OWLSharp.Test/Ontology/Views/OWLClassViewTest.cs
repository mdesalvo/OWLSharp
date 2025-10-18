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
public class OWLClassViewTest
{
    private OWLOntology Ontology { get; set; }
    private OWLClassView LivingEntityView { get; set; }
    private OWLClassView AnimalView { get; set; }
    private OWLClassView CatView { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:LivingEntity"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Animal"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Cat"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:DomesticFeline"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Dog"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:hasName"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:FelixTheCat"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:SnoopyTheDog"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:NotACat")))
            ],
            AnnotationAxioms = [
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("ex:Cat"),
                    new OWLLiteral(new RDFPlainLiteral("This is the class of cats"))),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO),
                    new RDFResource("ex:Cat"),
                    new RDFResource("http://wikipedia.org/Cat")),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                    new RDFResource("ex:Cat"),
                    new OWLLiteral(RDFTypedLiteral.True))
            ],
            AssertionAxioms = [
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Cat")),
                    new OWLNamedIndividual(new RDFResource("ex:FelixTheCat"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Dog")),
                    new OWLNamedIndividual(new RDFResource("ex:SnoopyTheDog"))),
                new OWLClassAssertion(
                    new OWLObjectComplementOf(new OWLClass(new RDFResource("ex:Cat"))),
                    new OWLNamedIndividual(new RDFResource("ex:NotACat"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:hasName")),
                    new OWLNamedIndividual(new RDFResource("ex:FelixTheCat")),
                    new OWLLiteral(new RDFPlainLiteral("Felix", "en-US"))),
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:hasName")),
                    new OWLNamedIndividual(new RDFResource("ex:SnoopyTheDog")),
                    new OWLLiteral(new RDFPlainLiteral("Snoopy", "en-US")))
            ],
            ClassAxioms = [
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Animal")),
                    new OWLClass(new RDFResource("ex:LivingEntity"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Cat")),
                    new OWLClass(new RDFResource("ex:Animal"))),
                new OWLSubClassOf(
                    new OWLClass(new RDFResource("ex:Dog")),
                    new OWLClass(new RDFResource("ex:Animal"))),
                new OWLDisjointClasses([
                    new OWLClass(new RDFResource("ex:Cat")),
                    new OWLClass(new RDFResource("ex:Dog"))]),
                new OWLEquivalentClasses([
                    new OWLClass(new RDFResource("ex:DomesticFeline")),
                    new OWLClass(new RDFResource("ex:Cat"))])
            ],
            KeyAxioms = [
                new OWLHasKey(
                    new OWLClass(new RDFResource("ex:LivingEntity")),
                    [new OWLDataProperty(new RDFResource("ex:hasName"))])
            ]
        };

        LivingEntityView = new OWLClassView(new OWLClass(new RDFResource("ex:LivingEntity")), Ontology);
        AnimalView = new OWLClassView(new OWLClass(new RDFResource("ex:Animal")), Ontology);
        CatView = new OWLClassView(new OWLClass(new RDFResource("ex:Cat")), Ontology);
    }

    [TestMethod]
    public async Task ShouldGetSubClassesAsync()
    {
        Assert.HasCount(4, await LivingEntityView.SubClassesAsync());
        Assert.HasCount(3, await AnimalView.SubClassesAsync());
        Assert.IsEmpty(await CatView.SubClassesAsync());
    }

    [TestMethod]
    public async Task ShouldGetSuperClassesAsync()
    {
        Assert.IsEmpty(await LivingEntityView.SuperClassesAsync());
        Assert.HasCount(1, await AnimalView.SuperClassesAsync());
        Assert.HasCount(2, await CatView.SuperClassesAsync());
    }

    [TestMethod]
    public async Task ShouldGetEquivalentClassesAsync()
    {
        Assert.IsEmpty(await LivingEntityView.EquivalentClassesAsync());
        Assert.IsEmpty(await AnimalView.EquivalentClassesAsync());
        Assert.HasCount(1, await CatView.EquivalentClassesAsync());
    }

    [TestMethod]
    public async Task ShouldGetDisjointClassesAsync()
    {
        Assert.IsEmpty(await LivingEntityView.DisjointClassesAsync());
        Assert.IsEmpty(await AnimalView.DisjointClassesAsync());
        Assert.HasCount(1, await CatView.DisjointClassesAsync());
    }

    [TestMethod]
    public async Task ShouldGetIndividualsAsync()
    {
        Assert.HasCount(2, await LivingEntityView.IndividualsAsync());
        Assert.HasCount(2, await AnimalView.IndividualsAsync());
        Assert.HasCount(1, await CatView.IndividualsAsync());
    }

    [TestMethod]
    public async Task ShouldGetNegativeIndividualsAsync()
    {
        Assert.IsEmpty(await LivingEntityView.NegativeIndividualsAsync());
        Assert.IsEmpty(await AnimalView.NegativeIndividualsAsync());
        Assert.HasCount(1, await CatView.NegativeIndividualsAsync());
    }

    [TestMethod]
    public async Task ShouldGetKeysAsync()
    {
        Assert.HasCount(1, await LivingEntityView.KeysAsync());
        Assert.IsEmpty(await AnimalView.KeysAsync());
        Assert.IsEmpty(await CatView.KeysAsync());
    }

    [TestMethod]
    public async Task ShouldGetDataAnnotationsAsync()
    {
        Assert.IsEmpty(await LivingEntityView.DataAnnotationsAsync());
        Assert.IsEmpty(await AnimalView.DataAnnotationsAsync());
        Assert.HasCount(2, await CatView.DataAnnotationsAsync());
    }

    [TestMethod]
    public async Task ShouldGetObjectAnnotationsAsync()
    {
        Assert.IsEmpty(await LivingEntityView.ObjectAnnotationsAsync());
        Assert.IsEmpty(await AnimalView.ObjectAnnotationsAsync());
        Assert.HasCount(1, await CatView.ObjectAnnotationsAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsDeprecatedAsync()
    {
        Assert.IsFalse(await LivingEntityView.IsDeprecatedAsync());
        Assert.IsFalse(await AnimalView.IsDeprecatedAsync());
        Assert.IsTrue(await CatView.IsDeprecatedAsync());
    }
}