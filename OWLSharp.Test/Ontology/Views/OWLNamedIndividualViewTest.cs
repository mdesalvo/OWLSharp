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
public class OWLNamedIndividualViewTest
{
    private OWLOntology Ontology { get; set; }
    private OWLNamedIndividualView MarkView { get; set; }
    private OWLNamedIndividualView JohnView { get; set; }
    private OWLNamedIndividualView StivView { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Ontology = new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Human"))),
                new OWLDeclaration(new OWLClass(new RDFResource("ex:Developer"))),
                new OWLDeclaration(new OWLDataProperty(new RDFResource("ex:age"))),
                new OWLDeclaration(new OWLObjectProperty(new RDFResource("ex:friendOf"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Markus"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Stiv")))
            ],
            AnnotationAxioms = [
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new RDFResource("ex:Mark"),
                    new OWLLiteral(new RDFPlainLiteral("This is an individual named Mark"))),
                new OWLAnnotationAssertion(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.SEE_ALSO),
                    new RDFResource("ex:Mark"),
                    new RDFResource("http://wikipedia.org/Mark"))
            ],
            AssertionAxioms = [
                new OWLDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:age")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("34", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))),
                new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLNegativeDataPropertyAssertion(
                    new OWLDataProperty(new RDFResource("ex:age")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLLiteral(new RDFTypedLiteral("33", RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER))),
                new OWLNegativeObjectPropertyAssertion(
                    new OWLObjectProperty(new RDFResource("ex:friendOf")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))),
                new OWLSameIndividual([
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Markus"))]),
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:John"))]),
                new OWLDifferentIndividuals([
                    new OWLNamedIndividual(new RDFResource("ex:Mark")),
                    new OWLNamedIndividual(new RDFResource("ex:Stiv"))]),
                new OWLClassAssertion(
                    new OWLClass(new RDFResource("ex:Developer")),
                    new OWLNamedIndividual(new RDFResource("ex:Mark"))),
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

        MarkView = new OWLNamedIndividualView(new OWLNamedIndividual(new RDFResource("ex:Mark")), Ontology);
        JohnView = new OWLNamedIndividualView(new OWLNamedIndividual(new RDFResource("ex:John")), Ontology);
        StivView = new OWLNamedIndividualView(new OWLNamedIndividual(new RDFResource("ex:Stiv")), Ontology);
    }

    [TestMethod]
    public async Task ShouldGetSameIndividualsAsync()
    {
        Assert.HasCount(1, await MarkView.SameIndividualsAsync());
        Assert.IsEmpty(await JohnView.SameIndividualsAsync());
        Assert.IsEmpty(await StivView.SameIndividualsAsync());
    }

    [TestMethod]
    public async Task ShouldGetDifferentIndividualsAsync()
    {
        Assert.HasCount(2, await MarkView.DifferentIndividualsAsync());
        Assert.HasCount(1, await JohnView.DifferentIndividualsAsync());
        Assert.HasCount(1, await StivView.DifferentIndividualsAsync());
    }

    [TestMethod]
    public async Task ShouldGetClassAssertionsAsync()
    {
        Assert.HasCount(2, await MarkView.ClassAssertionsAsync());
        Assert.HasCount(1, await JohnView.ClassAssertionsAsync());
        Assert.HasCount(1, await StivView.ClassAssertionsAsync());
    }

    [TestMethod]
    public async Task ShouldGetDataAssertionsAsync()
    {
        Assert.HasCount(1, await MarkView.DataAssertionsAsync());
        Assert.IsEmpty(await JohnView.DataAssertionsAsync());
        Assert.IsEmpty(await StivView.DataAssertionsAsync());
    }

    [TestMethod]
    public async Task ShouldGetObjectAssertionsAsync()
    {
        Assert.HasCount(1, await MarkView.ObjectAssertionsAsync());
        Assert.IsEmpty(await JohnView.ObjectAssertionsAsync());
        Assert.HasCount(1, await StivView.ObjectAssertionsAsync());
    }

    [TestMethod]
    public async Task ShouldGetNegativeDataAssertionsAsync()
    {
        Assert.HasCount(1, await MarkView.NegativeDataAssertionsAsync());
        Assert.IsEmpty(await JohnView.NegativeDataAssertionsAsync());
        Assert.IsEmpty(await StivView.NegativeDataAssertionsAsync());
    }

    [TestMethod]
    public async Task ShouldGetNegativeObjectAssertionsAsync()
    {
        Assert.HasCount(1, await MarkView.NegativeObjectAssertionsAsync());
        Assert.IsEmpty(await JohnView.NegativeObjectAssertionsAsync());
        Assert.IsEmpty(await StivView.NegativeObjectAssertionsAsync());
    }

    [TestMethod]
    public async Task ShouldGetDataAnnotationsAsync()
    {
        Assert.HasCount(1, await MarkView.DataAnnotationsAsync());
        Assert.IsEmpty(await JohnView.DataAnnotationsAsync());
        Assert.IsEmpty(await StivView.DataAnnotationsAsync());
    }

    [TestMethod]
    public async Task ShouldGetObjectAnnotationsAsync()
    {
        Assert.HasCount(1, await MarkView.ObjectAnnotationsAsync());
        Assert.IsEmpty(await JohnView.ObjectAnnotationsAsync());
        Assert.IsEmpty(await StivView.ObjectAnnotationsAsync());
    }
}