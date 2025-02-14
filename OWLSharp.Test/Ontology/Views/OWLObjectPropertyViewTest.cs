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
public class OWLObjectPropertyViewTest
{
    private OWLOntology Ontology { get; set; }
    private OWLObjectPropertyView KnowsView { get; set; }
    private OWLObjectPropertyView FriendOfView { get; set; }
    private OWLObjectPropertyView BestFriendOfView { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Human"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:knows"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:bestFriendOf"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:isFavoriteFriendOf"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:loves"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Valentine"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv")))
            ],
            AnnotationAxioms = [
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("ex:friendOf"),
                    new OWLLiteral(new RDFPlainLiteral("This is the object property for human friendships"))),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO),
                    new RDFResource("ex:friendOf"),
                    new RDFResource("http://wikipedia.org/friendOf")),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.OWL.DEPRECATED),
                    new RDFResource("ex:friendOf"),
                    new OWLLiteral(RDFTypedLiteral.True))
            ],
            ObjectPropertyAxioms = [
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLObjectProperty(new RDFResource("ex:knows"))),
                new OWLSubObjectPropertyOf(
                    new OWLObjectProperty(new RDFResource("ex:bestFriendOf")),
                    new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLEquivalentObjectProperties([
                    new OWLObjectProperty(new RDFResource("ex:bestFriendOf")),
                    new OWLObjectProperty(new RDFResource("ex:isFavoriteFriendOf"))]),
                new OWLDisjointObjectProperties([
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLObjectProperty(new RDFResource("ex:loves"))]),
                new OWLObjectPropertyDomain(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLClass(new RDFResource("ex:Human"))),
                new OWLObjectPropertyRange(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLClass(new RDFResource("ex:Human"))),
                new OWLInverseObjectProperties(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLObjectProperty(new RDFResource("ex:hasFriend"))),
                //Just for test purposes...
                new OWLFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLInverseFunctionalObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLSymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLAsymmetricObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLReflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLIrreflexiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLTransitiveObjectProperty(new OWLObjectProperty(new RDFResource("ex:friendOf")))
            ],
            AssertionAxioms = [
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Valentine"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Human")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Human")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Human")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Human")),
                    new OWLNamedIndividual(new RDFResource("ex:Valentine")))
            ]
        };        
        
        KnowsView = new OWLObjectPropertyView(new OWLObjectProperty(new RDFResource("ex:knows")), Ontology);
        FriendOfView = new OWLObjectPropertyView(new OWLObjectProperty(new RDFResource("ex:friendOf")), Ontology);
        BestFriendOfView = new OWLObjectPropertyView(new OWLObjectProperty(new RDFResource("ex:bestFriendOf")), Ontology);
    }

    [TestMethod]
    public async Task ShouldGetSubObjectPropertiesAsync()
    {
        Assert.AreEqual(3, (await KnowsView.SubObjectPropertiesAsync()).Count);
        Assert.AreEqual(2, (await FriendOfView.SubObjectPropertiesAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.SubObjectPropertiesAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetSuperObjectPropertiesAsync()
    {
        Assert.AreEqual(0, (await KnowsView.SuperObjectPropertiesAsync()).Count);
        Assert.AreEqual(1, (await FriendOfView.SuperObjectPropertiesAsync()).Count);
        Assert.AreEqual(2, (await BestFriendOfView.SuperObjectPropertiesAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetEquivalentObjectPropertiesAsync()
    {
        Assert.AreEqual(0, (await KnowsView.EquivalentObjectPropertiesAsync()).Count);
        Assert.AreEqual(0, (await FriendOfView.EquivalentObjectPropertiesAsync()).Count);
        Assert.AreEqual(1, (await BestFriendOfView.EquivalentObjectPropertiesAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetDisjointObjectPropertiesAsync()
    {
        Assert.AreEqual(0, (await KnowsView.DisjointObjectPropertiesAsync()).Count);
        Assert.AreEqual(1, (await FriendOfView.DisjointObjectPropertiesAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.DisjointObjectPropertiesAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetInverseObjectPropertiesAsync()
    {
        Assert.AreEqual(0, (await KnowsView.InverseObjectPropertiesAsync()).Count);
        Assert.AreEqual(1, (await FriendOfView.InverseObjectPropertiesAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.InverseObjectPropertiesAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetObjectAssertionsAsync()
    {
        Assert.AreEqual(0, (await KnowsView.ObjectAssertionsAsync()).Count);
        Assert.AreEqual(2, (await FriendOfView.ObjectAssertionsAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.ObjectAssertionsAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetNegativeObjectAssertionsAsync()
    {
        Assert.AreEqual(0, (await KnowsView.NegativeObjectAssertionsAsync()).Count);
        Assert.AreEqual(1, (await FriendOfView.NegativeObjectAssertionsAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.NegativeObjectAssertionsAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetDomainsAsync()
    {
        Assert.AreEqual(0, (await KnowsView.DomainsAsync()).Count);
        Assert.AreEqual(1, (await FriendOfView.DomainsAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.DomainsAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetRangesAsync()
    {
        Assert.AreEqual(0, (await KnowsView.RangesAsync()).Count);
        Assert.AreEqual(1, (await FriendOfView.RangesAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.RangesAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetDataAnnotationsAsync()
    {
        Assert.AreEqual(0, (await KnowsView.DataAnnotationsAsync()).Count);
        Assert.AreEqual(2, (await FriendOfView.DataAnnotationsAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.DataAnnotationsAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldGetObjectAnnotationsAsync()
    {
        Assert.AreEqual(0, (await KnowsView.ObjectAnnotationsAsync()).Count);
        Assert.AreEqual(1, (await FriendOfView.ObjectAnnotationsAsync()).Count);
        Assert.AreEqual(0, (await BestFriendOfView.ObjectAnnotationsAsync()).Count);
    }

    [TestMethod]
    public async Task ShouldCheckIsDeprecatedAsync()
    {
        Assert.IsFalse(await KnowsView.IsDeprecatedAsync());
        Assert.IsTrue(await FriendOfView.IsDeprecatedAsync());
        Assert.IsFalse(await BestFriendOfView.IsDeprecatedAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsFunctionalAsync()
    {
        Assert.IsFalse(await KnowsView.IsFunctionalAsync());
        Assert.IsTrue(await FriendOfView.IsFunctionalAsync());
        Assert.IsFalse(await BestFriendOfView.IsFunctionalAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsInverseFunctionalAsync()
    {
        Assert.IsFalse(await KnowsView.IsInverseFunctionalAsync());
        Assert.IsTrue(await FriendOfView.IsInverseFunctionalAsync());
        Assert.IsFalse(await BestFriendOfView.IsInverseFunctionalAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsSymmetricAsync()
    {
        Assert.IsFalse(await KnowsView.IsSymmetricAsync());
        Assert.IsTrue(await FriendOfView.IsSymmetricAsync());
        Assert.IsFalse(await BestFriendOfView.IsSymmetricAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsAsymmetricAsync()
    {
        Assert.IsFalse(await KnowsView.IsAsymmetricAsync());
        Assert.IsTrue(await FriendOfView.IsAsymmetricAsync());
        Assert.IsFalse(await BestFriendOfView.IsAsymmetricAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsReflexiveAsync()
    {
        Assert.IsFalse(await KnowsView.IsReflexiveAsync());
        Assert.IsTrue(await FriendOfView.IsReflexiveAsync());
        Assert.IsFalse(await BestFriendOfView.IsReflexiveAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsIrreflexiveAsync()
    {
        Assert.IsFalse(await KnowsView.IsIrreflexiveAsync());
        Assert.IsTrue(await FriendOfView.IsIrreflexiveAsync());
        Assert.IsFalse(await BestFriendOfView.IsIrreflexiveAsync());
    }

    [TestMethod]
    public async Task ShouldCheckIsTransitiveAsync()
    {
        Assert.IsFalse(await KnowsView.IsTransitiveAsync());
        Assert.IsTrue(await FriendOfView.IsTransitiveAsync());
        Assert.IsFalse(await BestFriendOfView.IsTransitiveAsync());
    }
}