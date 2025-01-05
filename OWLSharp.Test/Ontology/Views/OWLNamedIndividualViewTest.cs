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

namespace OWLSharp.Test.Ontology
{
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
            Ontology = new OWLOntology() 
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
            Assert.IsTrue((await MarkView.SameIndividualsAsync()).Count == 1);
            Assert.IsTrue((await JohnView.SameIndividualsAsync()).Count == 0);
            Assert.IsTrue((await StivView.SameIndividualsAsync()).Count == 0);
        }

        [TestMethod]
        public async Task ShouldGetDifferentIndividualsAsync()
        {
            Assert.IsTrue((await MarkView.DifferentIndividualsAsync()).Count == 2);
            Assert.IsTrue((await JohnView.DifferentIndividualsAsync()).Count == 1);
            Assert.IsTrue((await StivView.DifferentIndividualsAsync()).Count == 1);
        }

        [TestMethod]
        public async Task ShouldGetClassAssertionsAsync()
        {
            Assert.IsTrue((await MarkView.ClassAssertionsAsync()).Count == 2);
            Assert.IsTrue((await JohnView.ClassAssertionsAsync()).Count == 1);
            Assert.IsTrue((await StivView.ClassAssertionsAsync()).Count == 1);
        }

        [TestMethod]
        public async Task ShouldGetDataAssertionsAsync()
        {
            Assert.IsTrue((await MarkView.DataAssertionsAsync()).Count == 1);
            Assert.IsTrue((await JohnView.DataAssertionsAsync()).Count == 0);
            Assert.IsTrue((await StivView.DataAssertionsAsync()).Count == 0);
        }

        [TestMethod]
        public async Task ShouldGetObjectAssertionsAsync()
        {
            Assert.IsTrue((await MarkView.ObjectAssertionsAsync()).Count == 1);
            Assert.IsTrue((await JohnView.ObjectAssertionsAsync()).Count == 0);
            Assert.IsTrue((await StivView.ObjectAssertionsAsync()).Count == 1);
        }

        [TestMethod]
        public async Task ShouldGetNegativeDataAssertionsAsync()
        {
            Assert.IsTrue((await MarkView.NegativeDataAssertionsAsync()).Count == 1);
            Assert.IsTrue((await JohnView.NegativeDataAssertionsAsync()).Count == 0);
            Assert.IsTrue((await StivView.NegativeDataAssertionsAsync()).Count == 0);
        }

        [TestMethod]
        public async Task ShouldGetNegativeObjectAssertionsAsync()
        {
            Assert.IsTrue((await MarkView.NegativeObjectAssertionsAsync()).Count == 1);
            Assert.IsTrue((await JohnView.NegativeObjectAssertionsAsync()).Count == 0);
            Assert.IsTrue((await StivView.NegativeObjectAssertionsAsync()).Count == 0);
        }

        [TestMethod]
        public async Task ShouldGetDataAnnotationsAsync()
        {
            Assert.IsTrue((await MarkView.DataAnnotationsAsync()).Count == 1);
            Assert.IsTrue((await JohnView.DataAnnotationsAsync()).Count == 0);
            Assert.IsTrue((await StivView.DataAnnotationsAsync()).Count == 0);
        }

        [TestMethod]
        public async Task ShouldGetObjectAnnotationsAsync()
        {
            Assert.IsTrue((await MarkView.ObjectAnnotationsAsync()).Count == 1);
            Assert.IsTrue((await JohnView.ObjectAnnotationsAsync()).Count == 0);
            Assert.IsTrue((await StivView.ObjectAnnotationsAsync()).Count == 0);
        }
    }
}