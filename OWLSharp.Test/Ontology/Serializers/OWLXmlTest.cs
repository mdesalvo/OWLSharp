/*
   Copyright 2012-2023 Marco De Salvo

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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLXmlTest
    {
        #region Test
        [TestMethod]
        public void ShouldSerializeEmptyOntology()
        {
            OWLOntology ontology = new OWLOntology(RDFNamespaceRegister.DefaultNamespace.ToString());
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyOntology.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyOntology.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyOntology.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""https://rdfsharp.codeplex.com/"" xmlns:rdfsharp=""https://rdfsharp.codeplex.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""https://rdfsharp.codeplex.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdfsharp"" IRI=""https://rdfsharp.codeplex.com/"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""https://rdfsharp.codeplex.com/"" />
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeEmptyNamedOntology()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyNamedOntology.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyNamedOntology.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyNamedOntology.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeEmptyVersionedOntology()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(RDFVocabulary.OWL.VERSION_IRI, new RDFResource("http://example.com/v1/"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyVersionedOntology.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyVersionedOntology.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyVersionedOntology.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" versionIRI=""http://example.com/v1/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeEmptyImportingOntology()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource("http://example.com/v1/"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyImportingOntology.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyImportingOntology.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEmptyImportingOntology.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Import>http://example.com/v1/</Import>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithAbbreviatedPropertyAndAbbreviatedIRIAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(RDFVocabulary.RDFS.SEE_ALSO, RDFVocabulary.RDFS.CLASS);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndAbbreviatedIRIAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndAbbreviatedIRIAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndAbbreviatedIRIAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso""></AnnotationProperty>
    <AbbreviatedIRI>rdfs:Class</AbbreviatedIRI>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithAbbreviatedPropertyAndIRIAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("http://ex.com/"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndIRIAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndIRIAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndIRIAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso""></AnnotationProperty>
    <IRI>http://ex.com/</IRI>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithPropertyAndAbbreviatedIRIAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(new RDFResource("http://ann.prop/"), RDFVocabulary.RDFS.CLASS);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndAbbreviatedIRIAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndAbbreviatedIRIAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndAbbreviatedIRIAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/""></AnnotationProperty>
    <AbbreviatedIRI>rdfs:Class</AbbreviatedIRI>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithPropertyAndIRIAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(new RDFResource("http://ann.prop/"), new RDFResource("http://ann.obj/"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndIRIAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndIRIAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndIRIAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/""></AnnotationProperty>
    <IRI>http://ann.obj/</IRI>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLiteralAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(RDFVocabulary.RDFS.SEE_ALSO, new RDFPlainLiteral("hello!"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLiteralAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLiteralAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLiteralAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso"" />
    <Literal>hello!</Literal>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLanguageLiteralAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(RDFVocabulary.RDFS.SEE_ALSO, new RDFPlainLiteral("hello!","en-US"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLanguageLiteralAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLanguageLiteralAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndPlainLanguageLiteralAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso"" />
    <Literal xml:lang=""EN-US"">hello!</Literal>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithAbbreviatedPropertyAndTypedLiteralAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(RDFVocabulary.RDFS.SEE_ALSO, new RDFTypedLiteral("POINT(45 15)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndTypedLiteralAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndTypedLiteralAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithAbbreviatedPropertyAndTypedLiteralAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:geosparql=""http://www.opengis.net/ont/geosparql#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""geosparql"" IRI=""http://www.opengis.net/ont/geosparql#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso"" />
    <Literal datatypeIRI=""http://www.opengis.net/ont/geosparql#wktLiteral"">POINT(45 15)</Literal>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithPropertyAndPlainLiteralAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(new RDFResource("http://ann.prop/"), new RDFPlainLiteral("hello!"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndPlainLiteralAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndPlainLiteralAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndPlainLiteralAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/"" />
    <Literal>hello!</Literal>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithPropertyAndPlainLanguageLiteralAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(new RDFResource("http://ann.prop/"), new RDFPlainLiteral("hello!","en-US"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndPlainLanguageLiteralAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndPlainLanguageLiteralAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndPlainLanguageLiteralAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/"" />
    <Literal xml:lang=""EN-US"">hello!</Literal>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeOntologyWithPropertyAndTypedLiteralAnnotation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Annotate(new RDFResource("http://ann.prop/"), new RDFTypedLiteral("POINT(45 15)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndTypedLiteralAnnotation.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndTypedLiteralAnnotation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeOntologyWithPropertyAndTypedLiteralAnnotation.owx"));
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geosparql=""http://www.opengis.net/ont/geosparql#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geosparql"" IRI=""http://www.opengis.net/ont/geosparql#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/"" />
    <Literal datatypeIRI=""http://www.opengis.net/ont/geosparql#wktLiteral"">POINT(45 15)</Literal>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory, "OWLXmlTest_Should*"))
                File.Delete(file);
        }
        #endregion
    }
}