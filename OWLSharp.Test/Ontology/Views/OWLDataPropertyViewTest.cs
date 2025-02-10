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
    public class OWLDataPropertyViewTest
    {
        private OWLOntology Ontology { get; set; }
        private OWLDataPropertyView HasDataView { get; set; }
        private OWLDataPropertyView HasPersonalDataView { get; set; }
        private OWLDataPropertyView HasAgeView { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Ontology = new OWLOntology() 
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
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:John"))),
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
                        new OWLLiteral(RDFTypedLiteral.True)),
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
            Assert.AreEqual(4, (await HasDataView.SubDataPropertiesAsync()).Count);
            Assert.AreEqual(3, (await HasPersonalDataView.SubDataPropertiesAsync()).Count);
            Assert.AreEqual(0, (await HasAgeView.SubDataPropertiesAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetSuperDataPropertiesAsync()
        {
            Assert.AreEqual(0, (await HasDataView.SuperDataPropertiesAsync()).Count);
            Assert.AreEqual(1, (await HasPersonalDataView.SuperDataPropertiesAsync()).Count);
            Assert.AreEqual(2, (await HasAgeView.SuperDataPropertiesAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetEquivalentDataPropertiesAsync()
        {
            Assert.AreEqual(0, (await HasDataView.EquivalentDataPropertiesAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.EquivalentDataPropertiesAsync()).Count);
            Assert.AreEqual(1, (await HasAgeView.EquivalentDataPropertiesAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetDisjointDataPropertiesAsync()
        {
            Assert.AreEqual(0, (await HasDataView.DisjointDataPropertiesAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.DisjointDataPropertiesAsync()).Count);
            Assert.AreEqual(1, (await HasAgeView.DisjointDataPropertiesAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetDataAssertionsAsync()
        {
            Assert.AreEqual(0, (await HasDataView.DataAssertionsAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.DataAssertionsAsync()).Count);
            Assert.AreEqual(2, (await HasAgeView.DataAssertionsAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetNegativeDataAssertionsAsync()
        {
            Assert.AreEqual(0, (await HasDataView.NegativeDataAssertionsAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.NegativeDataAssertionsAsync()).Count);
            Assert.AreEqual(1, (await HasAgeView.NegativeDataAssertionsAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetDomainsAsync()
        {
            Assert.AreEqual(0, (await HasDataView.DomainsAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.DomainsAsync()).Count);
            Assert.AreEqual(1, (await HasAgeView.DomainsAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetRangesAsync()
        {
            Assert.AreEqual(0, (await HasDataView.RangesAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.RangesAsync()).Count);
            Assert.AreEqual(1, (await HasAgeView.RangesAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetDataAnnotationsAsync()
        {
            Assert.AreEqual(0, (await HasDataView.DataAnnotationsAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.DataAnnotationsAsync()).Count);
            Assert.AreEqual(2, (await HasAgeView.DataAnnotationsAsync()).Count);
        }

        [TestMethod]
        public async Task ShouldGetObjectAnnotationsAsync()
        {
            Assert.AreEqual(0, (await HasDataView.ObjectAnnotationsAsync()).Count);
            Assert.AreEqual(0, (await HasPersonalDataView.ObjectAnnotationsAsync()).Count);
            Assert.AreEqual(1, (await HasAgeView.ObjectAnnotationsAsync()).Count);
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
}