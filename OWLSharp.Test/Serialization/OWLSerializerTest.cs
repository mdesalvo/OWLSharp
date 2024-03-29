/*
   Copyright 2014-2024 Marco De Salvo

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RDFSharp.Model;
using System.Xml;

namespace OWLSharp.Test.Serialization
{
    [TestClass]
    public class OWLSerializerTest
    {
        #region Tests
        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithIRIOnly()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsNull(ontology2.OntologyVersion);
            Assert.IsTrue(ontology2.Prefixes.Count == 5);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithIRIandVersion()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 5);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnSerializingAndDeserializingNullOntology()
        {
            Assert.ThrowsException<OWLException>(() => OWLSerializer.Serialize(null));
            Assert.ThrowsException<OWLException>(() => OWLSerializer.Deserialize(null));
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithPrefix()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Annotations.Add(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/test")
                ));
            ontology.Annotations.Add(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI)
                ));
            ontology.Annotations.Add(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv")
                ));
            ontology.Annotations.Add(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFPlainLiteral("hello"))
                ));
            ontology.Annotations.Add(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFPlainLiteral("hello", "en-US--rtl"))
                ));
            ontology.Annotations.Add(new OWLAnnotation(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLLiteral(new RDFTypedLiteral("75.24", RDFModelEnums.RDFDatatypes.XSD_FLOAT))
                ));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Annotations.Count == 6);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithImport()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/Cls1"),
                new RDFResource("http://example.org/Cls2")
                ) { 
                    AxiomAnnotations = 
                    [ 
                        new OWLAnnotation(new OWLAnnotationProperty(RDFVocabulary.DC.DESCRIPTION), new OWLLiteral(new RDFPlainLiteral("hello!")))
                    ] 
                  });
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI),
                new RDFResource("http://example.org/Cls2")
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new RDFResource("http://example.org/Cls2")
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/Cls1"),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI)
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI)
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI)
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/Cls1"),
                new OWLAnonymousIndividual("AnonIdv")
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI),
                new OWLAnonymousIndividual("AnonIdv")
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new OWLAnonymousIndividual("AnonIdv")
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new RDFResource("http://example.org/Cls1"),
                new OWLLiteral(new RDFPlainLiteral("hello"))
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI),
                new OWLLiteral(new RDFPlainLiteral("hello", "en-US--ltr"))
                ));
            ontology.Axioms.Add(new OWLAnnotationAssertionAxiom(
                new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                new OWLAnonymousIndividual("AnonIdv"),
                new OWLLiteral(RDFTypedLiteral.True)
                ));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 12);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnSerializingAndDeserializingOntologyWithBadFormedImport()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));

            Assert.ThrowsException<OWLException>(() => ontology.Imports.Add(new OWLImport(null)));
            Assert.ThrowsException<OWLException>(() => ontology.Imports.Add(new OWLImport(new RDFResource())));
        }

        //Axioms

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithSubClassOfAxiomHavingIRI()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls1")), 
                new OWLClass(new RDFResource("http://example.org/Cls2"))));
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLClass(new RDFResource("http://example.org/Cls"))));
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLDatatype(new RDFResource("http://example.org/Dtp"))));
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLObjectProperty(new RDFResource("http://example.org/objProp"))));
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLDataProperty(new RDFResource("http://example.org/dtProp"))));
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLAnnotationProperty(new RDFResource("http://example.org/annProp"))));
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLNamedIndividual(new RDFResource("http://example.org/Idv"))));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 7);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithSubClassOfAxiomHavingAbbreviatedIRI()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("ex", "http://example.org/classes/")));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new XmlQualifiedName("Cls1", "http://example.org/classes/")),
                new OWLClass(new RDFResource("http://example.org/Cls2"))));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLClass(new RDFResource("http://example.org/Cls2"))) { IsInference=true, SerializationPriority=12 });
            ontology.Axioms.Add(new OWLDeclarationAxiom(new OWLClass(new RDFResource("http://example.org/Cls3"))));
            ontology.Axioms.Add(new OWLSubObjectPropertyOfAxiom(
                new OWLObjectProperty(new RDFResource("http://example.org/objPropA")),
                new OWLObjectProperty(new RDFResource("http://example.org/objPropB"))));
            ontology.Axioms.Add(new OWLSubObjectPropertyOfAxiom(
                new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://example.org/objPropA"))),
                new OWLObjectProperty(new RDFResource("http://example.org/objPropB"))));
            ontology.Axioms.Add(new OWLSubObjectPropertyOfAxiom(
                new OWLObjectPropertyChain([
                    new OWLObjectProperty(new RDFResource("http://example.org/objPropA")),
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://example.org/objPropA")))]),
                new OWLObjectProperty(new RDFResource("http://example.org/objPropB"))));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 6);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithSubClassOfAxiomHavingObjectIntersectionOfAxiom()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls1")),
                new OWLObjectIntersectionOf(
                [
                    new OWLClass(new RDFResource("http://example.org/Cls2")),
                    new OWLClass(new RDFResource("http://example.org/Cls3"))
                ])));
            ontology.Axioms.Add(new OWLDatatypeDefinitionAxiom(
                new OWLDatatype(new RDFResource("http://example.org/minorAge")),
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.INT),
                    [ new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("0", RDFModelEnums.RDFDatatypes.XSD_INT)), 
                                              new RDFResource("http://www.w3.org/2001/XMLSchema#minInclusive")),
                      new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("18", RDFModelEnums.RDFDatatypes.XSD_INT)),
                                              new RDFResource("http://www.w3.org/2001/XMLSchema#maxInclusive"))
                    ]
                )));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 2);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithEquivalentClassesAxiomHavingIRI()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLEquivalentClassesAxiom(
            [
                new OWLClass(new RDFResource("http://example.org/Cls1")),
                new OWLClass(new RDFResource("http://example.org/Cls2"))
            ]));
            ontology.Axioms.Add(new OWLHasKeyAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls1")),
                [],
                []
            ));
            ontology.Axioms.Add(new OWLHasKeyAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls2")),
                [new OWLObjectProperty(new RDFResource("http://example.org/objProp"))],
                []
            ));
            ontology.Axioms.Add(new OWLHasKeyAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls3")),
                [],
                [new OWLDataProperty(new RDFResource("http://example.org/dtProp"))]
            ));
            ontology.Axioms.Add(new OWLHasKeyAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls4")),
                [new OWLObjectProperty(new RDFResource("http://example.org/objProp"))],
                [new OWLDataProperty(new RDFResource("http://example.org/dtProp"))]
            ));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 5);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithEquivalentClassesAxiomHavingObjectIntersectionOfAxiom()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLEquivalentClassesAxiom(
            [
                new OWLClass(new RDFResource("http://example.org/Cls1")),
                new OWLObjectIntersectionOf(
                [
                    new OWLClass(new RDFResource("http://example.org/Cls2")),
                    new OWLClass(new RDFResource("http://example.org/Cls3"))
                ]),
                new OWLObjectHasValue(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp")),
                    new OWLNamedIndividual(new RDFResource("http://example.org/Idv1"))),
                new OWLObjectHasSelf(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp"))),
                new OWLDataHasValue(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")),
                    new OWLLiteral(new RDFPlainLiteral("hello"))),
                new OWLDataHasValue(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")),
                    new OWLLiteral(new RDFPlainLiteral("hello", "en-us--ltr"))),
                new OWLDataHasValue(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")),
                    new OWLLiteral(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.XSD_STRING))),
            ]));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 1);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithDisjointClassesAxiom()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLDisjointClassesAxiom(
            [
                new OWLClass(new RDFResource("http://example.org/Cls1")),
                new OWLClass(new RDFResource("http://example.org/Cls2")),
                new OWLObjectMinCardinality(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp")), 1),
                new OWLObjectMinCardinality(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp")), new OWLClass(new RDFResource("http://example.org/Cls1")), 0),
                new OWLObjectMaxCardinality(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://example.org/objProp"))), 2),
                new OWLObjectMaxCardinality(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp")), 
                    new OWLObjectHasSelf(new OWLObjectProperty(new RDFResource("http://example.org/objProp"))), 4),
                new OWLObjectExactCardinality(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp")), 1),
                new OWLObjectExactCardinality(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp")), 
                    new OWLClass(new RDFResource("http://example.org/Cls1")), 0),
                new OWLDataMinCardinality(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")), 1),
                new OWLDataMinCardinality(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")), 
                    new OWLDatatype(RDFVocabulary.XSD.BOOLEAN), 0),
                new OWLDataMaxCardinality(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")), 2),
                new OWLDataMaxCardinality(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")), 
                    new OWLDatatype(RDFVocabulary.XSD.BOOLEAN), 4),
                new OWLDataExactCardinality(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")), 1),
                new OWLDataExactCardinality(
                    new OWLDataProperty(new RDFResource("http://example.org/dtProp")), 
                    new OWLDatatype(RDFVocabulary.XSD.BOOLEAN), 2),
            ]));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 1);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithDisjointClassesAxiomHavingClassExpression()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLDisjointClassesAxiom(
            [
                new OWLClass(new RDFResource("http://example.org/Cls1")),
                new OWLObjectUnionOf(
                [
                    new OWLClass(new RDFResource("http://example.org/Cls2")),
                    new OWLClass(new RDFResource("http://example.org/Cls3")),
                    new OWLObjectIntersectionOf(
                    [
                        new OWLClass(new XmlQualifiedName("Agent", RDFVocabulary.FOAF.BASE_URI)),
                        new OWLClass(new XmlQualifiedName("Person", RDFVocabulary.FOAF.BASE_URI)),
                        new OWLClass(new XmlQualifiedName("Organization", RDFVocabulary.FOAF.BASE_URI)),
                    ])
                ])
            ]));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 1);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithDisjointUnionAxiomHavingClass()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLDisjointUnionAxiom(
                new OWLClass(new RDFResource("http://example/DisjUnCls")),
                [
                    new OWLClass(new RDFResource("http://example.org/Cls1")),
                    new OWLClass(new RDFResource("http://example.org/Cls2"))
                ]));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 1);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithEquivalentClassesAxiomHavingObjectSomeValuesFromClassExpression()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLEquivalentClassesAxiom(
            [
                new OWLObjectSomeValuesFrom(
                    new OWLObjectInverseOf(new OWLObjectProperty(new RDFResource("http://example.org/objProp1"))),
                    new OWLClass(new RDFResource("http://example.org/Cls1"))),
                new OWLObjectAllValuesFrom(
                    new OWLObjectProperty(new RDFResource("http://example.org/objProp2")),
                    new OWLClass(new XmlQualifiedName("Agent", RDFVocabulary.FOAF.BASE_URI))),
                new OWLDataSomeValuesFrom(
                    [new OWLDataProperty(new RDFResource("http://example.org/dtProp1"))],
                    new OWLDatatype(RDFVocabulary.XSD.BOOLEAN)),
                new OWLDataSomeValuesFrom(
                    [new OWLDataProperty(new RDFResource("http://example.org/dtProp1"))],
                    new OWLDataComplementOf(new OWLDatatype(RDFVocabulary.XSD.BOOLEAN))),
                new OWLDataAllValuesFrom(
                    [new OWLDataProperty(new RDFResource("http://example.org/dtProp1"))],
                    new OWLDataIntersectionOf([new OWLDatatype(RDFVocabulary.XSD.BOOLEAN),new OWLDatatype(RDFVocabulary.XSD.INT)])),
                new OWLDataAllValuesFrom(
                    [new OWLDataProperty(new RDFResource("http://example.org/dtProp1"))],
                    new OWLDataUnionOf([new OWLDatatype(RDFVocabulary.XSD.BOOLEAN),new OWLDatatype(RDFVocabulary.XSD.INT)])),
                new OWLDataAllValuesFrom(
                    [new OWLDataProperty(new RDFResource("http://example.org/dtProp1"))],
                    new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("hello")),
                                      new OWLLiteral(new RDFPlainLiteral("hello","en-US--rtl")),
                                      new OWLLiteral(new RDFTypedLiteral("34.77", RDFModelEnums.RDFDatatypes.XSD_FLOAT))])),
                new OWLDataSomeValuesFrom(
                    [new OWLDataProperty(new RDFResource("http://example.org/dtProp1")),
                     new OWLDataProperty(new RDFResource("http://example.org/dtProp2"))],
                    new OWLDatatype(RDFVocabulary.XSD.BOOLEAN)),
                new OWLDataSomeValuesFrom(
                    [new OWLDataProperty(new RDFResource("http://example.org/dtProp1"))],
                    new OWLDatatypeRestriction(
                        new OWLDatatype(new XmlQualifiedName("Age", RDFVocabulary.FOAF.BASE_URI)),
                        [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INT)),
                                                 new RDFResource("http://www.w3.org/2001/XMLSchema#minInclusive"))]))
            ]));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 1);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithMultipleAxioms()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLDisjointUnionAxiom(
                new OWLClass(new RDFResource("http://example/DisjUnCls")),
                [
                    new OWLClass(new RDFResource("http://example.org/Cls1")),
                    new OWLObjectUnionOf(
                    [
                        new OWLClass(new RDFResource("http://example.org/Cls2")),
                        new OWLClass(new RDFResource("http://example.org/Cls3")),
                        new OWLObjectComplementOf(new OWLObjectUnionOf(
                        [
                            new OWLClass(new RDFResource("http://example.org/Cls4")),
                            new OWLClass(new RDFResource("http://example.org/Cls5")),
                            new OWLObjectComplementOf(new OWLClass(new RDFResource("http://example.org/Cls6")))
                        ]))
                    ])
                ]));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls1")),
                new OWLClass(new RDFResource("http://example.org/Cls2"))));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls2")),
                new OWLClass(new RDFResource("http://example.org/Cls3"))));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls4")),
                new OWLObjectOneOf(
                [
                    new OWLNamedIndividual(new RDFResource("http://example.org/Idv1")),
                    new OWLNamedIndividual(new XmlQualifiedName("Idv2", RDFVocabulary.FOAF.BASE_URI)),
                    new OWLAnonymousIndividual("AnonIdv")
                ])));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 4);
        }
        #endregion
    }
}