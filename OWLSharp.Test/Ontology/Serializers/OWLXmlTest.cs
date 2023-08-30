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
using System.Collections.Generic;
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""https://rdfsharp.codeplex.com/"" xmlns:rdfsharp=""https://rdfsharp.codeplex.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""https://rdfsharp.codeplex.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdfsharp"" IRI=""https://rdfsharp.codeplex.com/"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""https://rdfsharp.codeplex.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" versionIRI=""http://example.com/v1/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:geosparql=""http://www.opengis.net/ont/geosparql#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""geosparql"" IRI=""http://www.opengis.net/ont/geosparql#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geosparql=""http://www.opengis.net/ont/geosparql#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geosparql"" IRI=""http://www.opengis.net/ont/geosparql#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/"" />
    <Literal datatypeIRI=""http://www.opengis.net/ont/geosparql#wktLiteral"">POINT(45 15)</Literal>
  </Annotation>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeSimpleClassDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/C1"));
            ontology.Model.ClassModel.DeclareClass(RDFVocabulary.GEO.SPATIAL_THING);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSimpleClassDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSimpleClassDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSimpleClassDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/C1"" />
  </Declaration>
  <Declaration>
    <Class abbreviatedIRI=""geo:SpatialThing"" />
  </Declaration>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectFunctionalPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"), new OWLOntologyObjectPropertyBehavior() { Functional = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyObjectPropertyBehavior() { Functional = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectFunctionalPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectFunctionalPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectFunctionalPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <FunctionalObjectProperty>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </FunctionalObjectProperty>
  <FunctionalObjectProperty>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </FunctionalObjectProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectInverseFunctionalPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"), new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyObjectPropertyBehavior() { InverseFunctional = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectInverseFunctionalPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectInverseFunctionalPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectInverseFunctionalPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <InverseFunctionalObjectProperty>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </InverseFunctionalObjectProperty>
  <InverseFunctionalObjectProperty>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </InverseFunctionalObjectProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectSymmetricPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"), new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectSymmetricPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectSymmetricPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectSymmetricPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <SymmetricObjectProperty>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </SymmetricObjectProperty>
  <SymmetricObjectProperty>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </SymmetricObjectProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectAsymmetricPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"), new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyObjectPropertyBehavior() { Asymmetric = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectAsymmetricPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectAsymmetricPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectAsymmetricPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <AsymmetricObjectProperty>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </AsymmetricObjectProperty>
  <AsymmetricObjectProperty>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </AsymmetricObjectProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectTransitivePropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"), new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectTransitivePropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectTransitivePropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectTransitivePropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <TransitiveObjectProperty>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </TransitiveObjectProperty>
  <TransitiveObjectProperty>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </TransitiveObjectProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectReflexivePropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"), new OWLOntologyObjectPropertyBehavior() { Reflexive = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyObjectPropertyBehavior() { Reflexive = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectReflexivePropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectReflexivePropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectReflexivePropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <ReflexiveObjectProperty>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </ReflexiveObjectProperty>
  <ReflexiveObjectProperty>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </ReflexiveObjectProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectIrreflexivePropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/OP1"), new OWLOntologyObjectPropertyBehavior() { Irreflexive = true });
            ontology.Model.PropertyModel.DeclareObjectProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyObjectPropertyBehavior() { Irreflexive = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectIrreflexivePropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectIrreflexivePropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectIrreflexivePropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <IrreflexiveObjectProperty>
    <ObjectProperty IRI=""http://example.com/OP1"" />
  </IrreflexiveObjectProperty>
  <IrreflexiveObjectProperty>
    <ObjectProperty abbreviatedIRI=""geo:location"" />
  </IrreflexiveObjectProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/DP1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEO.LOCATION);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/DP1"" />
  </Declaration>
  <Declaration>
    <DataProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataFunctionalPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/DP1"), new OWLOntologyDatatypePropertyBehavior() { Functional = true });
            ontology.Model.PropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEO.LOCATION, new OWLOntologyDatatypePropertyBehavior() { Functional = true });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataFunctionalPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataFunctionalPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataFunctionalPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/DP1"" />
  </Declaration>
  <Declaration>
    <DataProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
  <FunctionalDataProperty>
    <DataProperty IRI=""http://example.com/DP1"" />
  </FunctionalDataProperty>
  <FunctionalDataProperty>
    <DataProperty abbreviatedIRI=""geo:location"" />
  </FunctionalDataProperty>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeAnnotationPropertyDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("http://example.com/AP1"));
            ontology.Model.PropertyModel.DeclareAnnotationProperty(RDFVocabulary.GEO.LOCATION);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAnnotationPropertyDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAnnotationPropertyDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAnnotationPropertyDeclarations.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <AnnotationProperty IRI=""http://example.com/AP1"" />
  </Declaration>
  <Declaration>
    <AnnotationProperty abbreviatedIRI=""geo:location"" />
  </Declaration>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeNamedIndividualDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Data.DeclareIndividual(new RDFResource("http://example.com/ID1"));
            ontology.Data.DeclareIndividual(RDFVocabulary.GEO.LOCATION);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeNamedIndividualDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeNamedIndividualDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeNamedIndividualDeclarations.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <NamedIndividual IRI=""http://example.com/ID1"" />
  </Declaration>
  <Declaration>
    <NamedIndividual abbreviatedIRI=""geo:location"" />
  </Declaration>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        //ClassModel

        [TestMethod]
        public void ShouldSerializeSomeValuesFromObjectRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareSomeValuesFromRestriction(new RDFResource("http://example.com/someValuesFromRestriction"), 
              new RDFResource("http://example.com/objectProperty"), new RDFResource("http://example.com/SimpleClass"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSomeValuesFromObjectRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSomeValuesFromObjectRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSomeValuesFromObjectRestriction.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/someValuesFromRestriction"" />
    <ObjectSomeValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectSomeValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeSomeValuesFromDatatypeRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/datatypeProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareSomeValuesFromRestriction(new RDFResource("http://example.com/someValuesFromRestriction"), 
              new RDFResource("http://example.com/datatypeProperty"), new RDFResource("http://example.com/SimpleClass"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSomeValuesFromDatatypeRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSomeValuesFromDatatypeRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSomeValuesFromDatatypeRestriction.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/someValuesFromRestriction"" />
    <DataSomeValuesFrom>
      <DataProperty IRI=""http://example.com/datatypeProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </DataSomeValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeAllValuesFromObjectRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("http://example.com/allValuesFromRestriction"), 
              new RDFResource("http://example.com/objectProperty"), new RDFResource("http://example.com/SimpleClass"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllValuesFromObjectRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllValuesFromObjectRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllValuesFromObjectRestriction.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeAllValuesFromDatatypeRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/datatypeProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("http://example.com/allValuesFromRestriction"), 
              new RDFResource("http://example.com/datatypeProperty"), new RDFResource("http://example.com/SimpleClass"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllValuesFromDatatypeRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllValuesFromDatatypeRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllValuesFromDatatypeRestriction.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <DataAllValuesFrom>
      <DataProperty IRI=""http://example.com/datatypeProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </DataAllValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeValuesFromRestrictionWithAbbreviatedName()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(RDFVocabulary.GEO.SPATIAL_THING, 
              new RDFResource("http://example.com/objectProperty"), new RDFResource("http://example.com/SimpleClass"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedName.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedName.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedName.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class abbreviatedIRI=""geo:SpatialThing"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeValuesFromRestrictionWithBlankName()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:12345"), 
              new RDFResource("http://example.com/objectProperty"), new RDFResource("http://example.com/SimpleClass"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithBlankName.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithBlankName.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithBlankName.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""bnode://12345"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeValuesFromRestrictionWithAbbreviatedClass()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("http://example.com/allValuesFromRestriction"), 
              new RDFResource("http://example.com/objectProperty"), RDFVocabulary.GEO.SPATIAL_THING);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedClass.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedClass.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedClass.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class abbreviatedIRI=""geo:SpatialThing"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeValuesFromRestrictionWithBlankClass()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("http://example.com/allValuesFromRestriction"), 
              new RDFResource("http://example.com/objectProperty"), new RDFResource("bnode:12345"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithBlankClass.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithBlankClass.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithBlankClass.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""bnode://12345"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeValuesFromRestrictionWithAbbreviatedProperty()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(RDFVocabulary.GEO.LAT_LONG);
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("http://example.com/allValuesFromRestriction"), 
              RDFVocabulary.GEO.LAT_LONG, new RDFResource("http://example.com/SimpleClass"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedProperty.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedProperty.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeValuesFromRestrictionWithAbbreviatedProperty.owx"));
            string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <Declaration>
    <DataProperty abbreviatedIRI=""geo:lat_long"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <DataAllValuesFrom>
      <DataProperty abbreviatedIRI=""geo:lat_long"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </DataAllValuesFrom>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnSerializingValuesFromRestrictionBecauseUndeclaredProperty()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/SimpleClass"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("http://example.com/allValuesFromRestriction"), 
              new RDFResource("http://example.com/undeclaredProperty"), new RDFResource("http://example.com/SimpleClass"));
            Assert.ThrowsException<OWLException>(() => 
              OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldThrowExceptionOnSerializingValuesFromRestrictionBecauseUndeclaredProperty.owx")));
        }

        [TestMethod]
        public void ShouldSerializeHasValueObjectRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("http://example.com/hasValueRestriction"),
              new RDFResource("http://example.com/objectProperty"), new RDFResource("http://example.com/individual"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueObjectRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueObjectRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueObjectRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/hasValueRestriction"" />
    <ObjectHasValue>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <NamedIndividual IRI=""http://example.com/individual"" />
    </ObjectHasValue>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeHasValueDatatypePlainLiteralRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/datatypeProperty"));
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("http://example.com/hasValueRestriction"),
              new RDFResource("http://example.com/datatypeProperty"), new RDFPlainLiteral("value!"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypePlainLiteralRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypePlainLiteralRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypePlainLiteralRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/hasValueRestriction"" />
    <DataHasValue>
      <DataProperty IRI=""http://example.com/datatypeProperty"" />
      <Literal>value!</Literal>
    </DataHasValue>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeHasValueDatatypePlainLanguagedLiteralRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/datatypeProperty"));
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("http://example.com/hasValueRestriction"),
              new RDFResource("http://example.com/datatypeProperty"), new RDFPlainLiteral("value!", "en-US"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypePlainLanguagedLiteralRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypePlainLanguagedLiteralRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypePlainLanguagedLiteralRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/hasValueRestriction"" />
    <DataHasValue>
      <DataProperty IRI=""http://example.com/datatypeProperty"" />
      <Literal xml:lang=""EN-US"">value!</Literal>
    </DataHasValue>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeHasValueDatatypeTypedLiteralRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/datatypeProperty"));
            ontology.Model.ClassModel.DeclareHasValueRestriction(new RDFResource("http://example.com/hasValueRestriction"),
              new RDFResource("http://example.com/datatypeProperty"), new RDFTypedLiteral("value!", RDFModelEnums.RDFDatatypes.XSD_STRING));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypeTypedLiteralRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypeTypedLiteralRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasValueDatatypeTypedLiteralRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/hasValueRestriction"" />
    <DataHasValue>
      <DataProperty IRI=""http://example.com/datatypeProperty"" />
      <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#string"">value!</Literal>
    </DataHasValue>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeHasSelfObjectRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareHasSelfRestriction(new RDFResource("http://example.com/hasSelfRestriction"),
              new RDFResource("http://example.com/objectProperty"), true);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasSelfObjectRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasSelfObjectRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasSelfObjectRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/hasSelfRestriction"" />
    <ObjectHasSelf>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
    </ObjectHasSelf>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectMinCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareMinCardinalityRestriction(new RDFResource("http://example.com/mincRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/mincRestriction"" />
    <ObjectMinCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
    </ObjectMinCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectMinQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Class1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareMinQualifiedCardinalityRestriction(new RDFResource("http://example.com/minqcRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1, new RDFResource("http://example.com/Class1"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Class1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/minqcRestriction"" />
    <ObjectMinCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/Class1"" />
    </ObjectMinCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataMinCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareMinCardinalityRestriction(new RDFResource("http://example.com/mincRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/mincRestriction"" />
    <DataMinCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
    </DataMinCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataMinQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareMinQualifiedCardinalityRestriction(new RDFResource("http://example.com/minqcRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1, RDFVocabulary.XSD.INTEGER);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/minqcRestriction"" />
    <DataMinCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
      <Class abbreviatedIRI=""xsd:integer"" />
    </DataMinCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectMaxCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareMaxCardinalityRestriction(new RDFResource("http://example.com/maxcRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMaxCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMaxCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMaxCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/maxcRestriction"" />
    <ObjectMaxCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
    </ObjectMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectMaxQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Class1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("http://example.com/maxqcRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1, new RDFResource("http://example.com/Class1"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMaxQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMaxQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMaxQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Class1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/maxqcRestriction"" />
    <ObjectMaxCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/Class1"" />
    </ObjectMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataMaxCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareMaxCardinalityRestriction(new RDFResource("http://example.com/maxcRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMaxCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMaxCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMaxCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/maxcRestriction"" />
    <DataMaxCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
    </DataMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataMaxQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareMaxQualifiedCardinalityRestriction(new RDFResource("http://example.com/maxqcRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1, RDFVocabulary.XSD.INTEGER);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMaxQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMaxQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMaxQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/maxqcRestriction"" />
    <DataMaxCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
      <Class abbreviatedIRI=""xsd:integer"" />
    </DataMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectExactCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("http://example.com/excRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectExactCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectExactCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectExactCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/excRestriction"" />
    <ObjectExactCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
    </ObjectExactCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectExactQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Class1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareQualifiedCardinalityRestriction(new RDFResource("http://example.com/exqcRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1, new RDFResource("http://example.com/Class1"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectExactQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectExactQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectExactQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Class1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/exqcRestriction"" />
    <ObjectExactCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/Class1"" />
    </ObjectExactCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataExactCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareCardinalityRestriction(new RDFResource("http://example.com/excRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataExactCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataExactCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataExactCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/excRestriction"" />
    <DataExactCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
    </DataExactCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataExactQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareQualifiedCardinalityRestriction(new RDFResource("http://example.com/exqcRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1, RDFVocabulary.XSD.INTEGER);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataExactQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataExactQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataExactQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/exqcRestriction"" />
    <DataExactCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
      <Class abbreviatedIRI=""xsd:integer"" />
    </DataExactCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectMinMaxCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareMinMaxCardinalityRestriction(new RDFResource("http://example.com/minmaxcRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1, 2);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinMaxCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinMaxCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinMaxCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/minmaxcRestriction"" />
    <ObjectMinCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
    </ObjectMinCardinality>
    <ObjectMaxCardinality cardinality=""2"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
    </ObjectMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectMinMaxQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Class1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("http://example.com/minmaxqcRestriction"),
              new RDFResource("http://example.com/objectProperty"), 1, 2, new RDFResource("http://example.com/Class1"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinMaxQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinMaxQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectMinMaxQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Class1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/minmaxqcRestriction"" />
    <ObjectMinCardinality cardinality=""1"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/Class1"" />
    </ObjectMinCardinality>
    <ObjectMaxCardinality cardinality=""2"">
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/Class1"" />
    </ObjectMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataMinMaxCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareMinMaxCardinalityRestriction(new RDFResource("http://example.com/minmaxcRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1, 2);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinMaxCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinMaxCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinMaxCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/minmaxcRestriction"" />
    <DataMinCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
    </DataMinCardinality>
    <DataMaxCardinality cardinality=""2"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
    </DataMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDataMinMaxQualifiedCardinalityRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dataProperty"));
            ontology.Model.ClassModel.DeclareMinMaxQualifiedCardinalityRestriction(new RDFResource("http://example.com/minmaxqcRestriction"),
              new RDFResource("http://example.com/dataProperty"), 1, 2, RDFVocabulary.XSD.INTEGER);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinMaxQualifiedCardinalityRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinMaxQualifiedCardinalityRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDataMinMaxQualifiedCardinalityRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dataProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/minmaxqcRestriction"" />
    <DataMinCardinality cardinality=""1"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
      <Class abbreviatedIRI=""xsd:integer"" />
    </DataMinCardinality>
    <DataMaxCardinality cardinality=""2"">
      <DataProperty IRI=""http://example.com/dataProperty"" />
      <Class abbreviatedIRI=""xsd:integer"" />
    </DataMaxCardinality>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectEnumerate()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objectProperty"));
            ontology.Model.ClassModel.DeclareEnumerateClass(new RDFResource("http://example.com/enumClass"),
              new List<RDFResource>() { new RDFResource("http://example.com/Idv1"), new RDFResource("http://example.com/Idv2") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectEnumerate.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectEnumerate.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectEnumerate.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/enumClass"" />
    <ObjectOneOf>
      <NamedIndividual IRI=""http://example.com/Idv1"" />
      <NamedIndividual IRI=""http://example.com/Idv2"" />
    </ObjectOneOf>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeLiteralEnumerate()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/datatypeProperty"));
            ontology.Model.ClassModel.DeclareEnumerateClass(new RDFResource("http://example.com/enumClass"),
              new List<RDFLiteral>() { new RDFPlainLiteral("hello","en-US"), new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INT) });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeLiteralEnumerate.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeLiteralEnumerate.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeLiteralEnumerate.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/enumClass"" />
    <DataOneOf>
      <Literal xml:lang=""EN-US"">hello</Literal>
      <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">25</Literal>
    </DataOneOf>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectUnionOf()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareUnionClass(new RDFResource("http://example.com/UnionClass"),
              new List<RDFResource>() { new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectUnionOf.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectUnionOf.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectUnionOf.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <EquivalentClasses>
    <Class IRI=""http://example.com/UnionClass"" />
    <ObjectUnionOf>
      <Class IRI=""http://example.com/Cls1"" />
      <Class IRI=""http://example.com/Cls2"" />
    </ObjectUnionOf>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectIntersectionOf()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareIntersectionClass(new RDFResource("http://example.com/IntersectionClass"),
              new List<RDFResource>() { new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectIntersectionOf.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectIntersectionOf.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectIntersectionOf.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <EquivalentClasses>
    <Class IRI=""http://example.com/IntersectionClass"" />
    <ObjectIntersectionOf>
      <Class IRI=""http://example.com/Cls1"" />
      <Class IRI=""http://example.com/Cls2"" />
    </ObjectIntersectionOf>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectComplementOf()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareComplementClass(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectComplementOf.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectComplementOf.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectComplementOf.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <EquivalentClasses>
    <Class IRI=""http://example.com/Cls1"" />
    <ObjectComplementOf>
      <Class IRI=""http://example.com/Cls2"" />
    </ObjectComplementOf>
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeSubClassRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls3"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubClassRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubClassRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubClassRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls1"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls2"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls3"" />
  </Declaration>
  <SubClassOf>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls2"" />
  </SubClassOf>
  <SubClassOf>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls3"" />
  </SubClassOf>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeSubClassRelationHavingRestriction()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareAllValuesFromRestriction(new RDFResource("http://example.com/AVFRestr"),
              new RDFResource("http://example.com/objProp"), new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/AVFRestr"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubClassRelationHavingRestriction.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubClassRelationHavingRestriction.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubClassRelationHavingRestriction.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls1"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls2"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/AVFRestr"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objProp"" />
      <Class IRI=""http://example.com/Cls2"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
  <SubClassOf>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/AVFRestr"" />
  </SubClassOf>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeEquivalentClassRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls3"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareEquivalentClasses(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentClassRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentClassRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentClassRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls1"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls2"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls3"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls2"" />
  </EquivalentClasses>
  <EquivalentClasses>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls3"" />
  </EquivalentClasses>
  <EquivalentClasses>
    <Class IRI=""http://example.com/Cls2"" />
    <Class IRI=""http://example.com/Cls1"" />
  </EquivalentClasses>
  <EquivalentClasses>
    <Class IRI=""http://example.com/Cls3"" />
    <Class IRI=""http://example.com/Cls1"" />
  </EquivalentClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDisjointClassRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls3"));
            ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareDisjointClasses(new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointClassRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointClassRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointClassRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls1"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls2"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls3"" />
  </Declaration>
  <DisjointClasses>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls2"" />
  </DisjointClasses>
  <DisjointClasses>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls3"" />
  </DisjointClasses>
  <DisjointClasses>
    <Class IRI=""http://example.com/Cls2"" />
    <Class IRI=""http://example.com/Cls1"" />
  </DisjointClasses>
  <DisjointClasses>
    <Class IRI=""http://example.com/Cls3"" />
    <Class IRI=""http://example.com/Cls1"" />
  </DisjointClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDisjointUnionClassRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareDisjointUnionClass(new RDFResource("http://example.com/DisjointUnionClass"),
              new List<RDFResource>() { new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointUnionClassRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointUnionClassRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointUnionClassRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls1"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls2"" />
  </Declaration>
  <DisjointUnion>
    <Class IRI=""http://example.com/DisjointUnionClass"" />
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls2"" />
  </DisjointUnion>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeAllDisjointClassesRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls1"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls2"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls3"));
            ontology.Model.ClassModel.DeclareAllDisjointClasses(new RDFResource("http://example.com/AllDisjointClasses"),
              new List<RDFResource>() { new RDFResource("http://example.com/Cls1"), new RDFResource("http://example.com/Cls2"), new RDFResource("http://example.com/Cls3") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointClassesRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointClassesRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointClassesRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls1"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls2"" />
  </Declaration>
  <Declaration>
    <Class IRI=""http://example.com/Cls3"" />
  </Declaration>
  <DisjointClasses>
    <Class IRI=""http://example.com/Cls1"" />
    <Class IRI=""http://example.com/Cls2"" />
    <Class IRI=""http://example.com/Cls3"" />
  </DisjointClasses>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeHasKeyObjectRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp2"));
            ontology.Model.ClassModel.DeclareHasKey(new RDFResource("http://example.com/Cls"),
              new List<RDFResource>() { new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp2") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasKeyObjectRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasKeyObjectRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasKeyObjectRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </Declaration>
  <HasKey>
    <Class IRI=""http://example.com/Cls"" />
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </HasKey>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeHasKeyDataRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp2"));
            ontology.Model.ClassModel.DeclareHasKey(new RDFResource("http://example.com/Cls"),
              new List<RDFResource>() { new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp2") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasKeyDataRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasKeyDataRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeHasKeyDataRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </Declaration>
  <HasKey>
    <Class IRI=""http://example.com/Cls"" />
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </HasKey>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeClassAnnotations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("http://example.com/Cls"), 
                new OWLOntologyClassBehavior() { Deprecated = true });
            ontology.Model.ClassModel.AnnotateClass(new RDFResource("http://example.com/Cls"), 
              new RDFResource("http://example.com/annProp"), new RDFResource("http://example.com/"));
            ontology.Model.ClassModel.AnnotateClass(new RDFResource("http://example.com/Cls"), 
              RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("http://example.com/ClsAbout")); //TODO: non viene riconosciuta in automatico l'annotazione rdfs:seeAlso
            ontology.Model.ClassModel.AnnotateClass(new RDFResource("http://example.com/Cls"), 
              RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("this is a class", "en"));  //TODO: non viene riconosciuta in automatico l'annotazione rdfs:comment
            ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("http://example.com/annProp"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeClassAnnotations.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeClassAnnotations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeClassAnnotations.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/Cls"" />
  </Declaration>
  <Declaration>
    <AnnotationProperty IRI=""http://example.com/annProp"" />
  </Declaration>
  <AnnotationAssertion>
    <AnnotationProperty IRI=""http://example.com/annProp"" />
    <IRI>http://example.com/Cls</IRI>
    <IRI>http://example.com/</IRI>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso"" />
    <IRI>http://example.com/Cls</IRI>
    <IRI>http://example.com/ClsAbout</IRI>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
    <IRI>http://example.com/Cls</IRI>
    <Literal xml:lang=""EN"">this is a class</Literal>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""owl:deprecated"" />
    <IRI>http://example.com/Cls</IRI>
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#boolean"">true</Literal>
  </AnnotationAssertion>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        //PropertyModel

        [TestMethod]
        public void ShouldSerializeSubObjectPropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp3"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubObjectPropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubObjectPropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubObjectPropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </Declaration>
  <SubObjectPropertyOf>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </SubObjectPropertyOf>
  <SubObjectPropertyOf>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </SubObjectPropertyOf>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeSubDatatypePropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp2"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp3"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp2"));
            ontology.Model.PropertyModel.DeclareSubProperties(new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubDatatypePropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubDatatypePropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeSubDatatypePropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </Declaration>
  <SubDataPropertyOf>
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </SubDataPropertyOf>
  <SubDataPropertyOf>
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </SubDataPropertyOf>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeEquivalentObjectPropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp3"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentObjectPropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentObjectPropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentObjectPropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </Declaration>
  <EquivalentObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </EquivalentObjectProperties>
  <EquivalentObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </EquivalentObjectProperties>
  <EquivalentObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </EquivalentObjectProperties>
  <EquivalentObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </EquivalentObjectProperties>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeEquivalentDatatypePropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp2"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp3"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp2"));
            ontology.Model.PropertyModel.DeclareEquivalentProperties(new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentDatatypePropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentDatatypePropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeEquivalentDatatypePropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </Declaration>
  <EquivalentDataProperties>
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </EquivalentDataProperties>
  <EquivalentDataProperties>
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </EquivalentDataProperties>
  <EquivalentDataProperties>
    <DataProperty IRI=""http://example.com/dtProp2"" />
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </EquivalentDataProperties>
  <EquivalentDataProperties>
    <DataProperty IRI=""http://example.com/dtProp3"" />
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </EquivalentDataProperties>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDisjointObjectPropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp3"));
            ontology.Model.PropertyModel.DeclareDisjointProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareDisjointProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointObjectPropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointObjectPropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointObjectPropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </Declaration>
  <DisjointObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </DisjointObjectProperties>
  <DisjointObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </DisjointObjectProperties>
  <DisjointObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </DisjointObjectProperties>
  <DisjointObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </DisjointObjectProperties>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDisjointDatatypePropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp2"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp3"));
            ontology.Model.PropertyModel.DeclareDisjointProperties(new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp2"));
            ontology.Model.PropertyModel.DeclareDisjointProperties(new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointDatatypePropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointDatatypePropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDisjointDatatypePropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </Declaration>
  <DisjointDataProperties>
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </DisjointDataProperties>
  <DisjointDataProperties>
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </DisjointDataProperties>
  <DisjointDataProperties>
    <DataProperty IRI=""http://example.com/dtProp2"" />
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </DisjointDataProperties>
  <DisjointDataProperties>
    <DataProperty IRI=""http://example.com/dtProp3"" />
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </DisjointDataProperties>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeAllDisjointObjectPropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp3"));
            ontology.Model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("http://example.com/AllDisjointProperties"),
                new List<RDFResource>() { new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp2"), new RDFResource("http://example.com/objProp3") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointObjectPropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointObjectPropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointObjectPropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </Declaration>
  <DisjointObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp2"" />
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </DisjointObjectProperties>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeAllDisjointDataPropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp1"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp2"));
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp3"));
            ontology.Model.PropertyModel.DeclareAllDisjointProperties(new RDFResource("http://example.com/AllDisjointProperties"),
                new List<RDFResource>() { new RDFResource("http://example.com/dtProp1"), new RDFResource("http://example.com/dtProp2"), new RDFResource("http://example.com/dtProp3") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointDataPropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointDataPropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeAllDisjointDataPropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp1"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp2"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </Declaration>
  <DisjointDataProperties>
    <DataProperty IRI=""http://example.com/dtProp1"" />
    <DataProperty IRI=""http://example.com/dtProp2"" />
    <DataProperty IRI=""http://example.com/dtProp3"" />
  </DisjointDataProperties>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeInversePropertyRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp3"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareInverseProperties(new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp3"));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeInversePropertyRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeInversePropertyRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeInversePropertyRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </Declaration>
  <InverseObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </InverseObjectProperties>
  <InverseObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </InverseObjectProperties>
  <InverseObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </InverseObjectProperties>
  <InverseObjectProperties>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </InverseObjectProperties>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializePropertyChainRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp1"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp2"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp3"));
            ontology.Model.PropertyModel.DeclarePropertyChainAxiom(new RDFResource("http://example.com/PropertyChain"),
                new List<RDFResource>() { new RDFResource("http://example.com/objProp1"), new RDFResource("http://example.com/objProp2"), new RDFResource("http://example.com/objProp3") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializePropertyChainRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializePropertyChainRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializePropertyChainRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp1"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp2"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp3"" />
  </Declaration>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/PropertyChain"" />
  </Declaration>
  <SubObjectPropertyOf>
    <ObjectPropertyChain>
      <ObjectProperty IRI=""http://example.com/objProp1"" />
      <ObjectProperty IRI=""http://example.com/objProp2"" />
      <ObjectProperty IRI=""http://example.com/objProp3"" />
    </ObjectPropertyChain>
    <ObjectProperty IRI=""http://example.com/PropertyChain"" />
  </SubObjectPropertyOf>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectDomainRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp"), new OWLOntologyObjectPropertyBehavior() {
              Domain = new RDFResource("http://example.com/ClsDomain") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectDomainRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectDomainRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectDomainRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp"" />
  </Declaration>
  <ObjectPropertyDomain>
    <ObjectProperty IRI=""http://example.com/objProp"" />
    <Class IRI=""http://example.com/ClsDomain"" />
  </ObjectPropertyDomain>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDatatypeDomainRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp"), new OWLOntologyDatatypePropertyBehavior() {
              Domain = new RDFResource("http://example.com/ClsDomain") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDatatypeDomainRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDatatypeDomainRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDatatypeDomainRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp"" />
  </Declaration>
  <DataPropertyDomain>
    <DataProperty IRI=""http://example.com/dtProp"" />
    <Class IRI=""http://example.com/ClsDomain"" />
  </DataPropertyDomain>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeObjectRangeRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp"), new OWLOntologyObjectPropertyBehavior() {
              Range = new RDFResource("http://example.com/ClsRange") });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectRangeRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectRangeRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeObjectRangeRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp"" />
  </Declaration>
  <ObjectPropertyRange>
    <ObjectProperty IRI=""http://example.com/objProp"" />
    <Class IRI=""http://example.com/ClsRange"" />
  </ObjectPropertyRange>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializeDatatypeRangeRelation()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp"), new OWLOntologyDatatypePropertyBehavior() {
              Range = RDFVocabulary.XSD.INTEGER });
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDatatypeRangeRelation.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDatatypeRangeRelation.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeDatatypeRangeRelation.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp"" />
  </Declaration>
  <DataPropertyRange>
    <DataProperty IRI=""http://example.com/dtProp"" />
    <Class abbreviatedIRI=""xsd:integer"" />
  </DataPropertyRange>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        [TestMethod]
        public void ShouldSerializePropertyAnnotations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("http://example.com/objProp"), 
                new OWLOntologyObjectPropertyBehavior() { Deprecated = true });
            ontology.Model.PropertyModel.DeclareDatatypeProperty(new RDFResource("http://example.com/dtProp"));
            ontology.Model.PropertyModel.DeclareAnnotationProperty(new RDFResource("http://example.com/annProp"), new OWLOntologyAnnotationPropertyBehavior() {
               Deprecated = true });
            ontology.Model.PropertyModel.AnnotateProperty(new RDFResource("http://example.com/objProp"), 
              new RDFResource("http://example.com/annProp"), new RDFResource("http://example.com/"));
            ontology.Model.PropertyModel.AnnotateProperty(new RDFResource("http://example.com/dtProp"), 
              RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("http://example.com/DtPropAbout")); //TODO: non viene riconosciuta in automatico l'annotazione rdfs:seeAlso
            ontology.Model.PropertyModel.AnnotateProperty(new RDFResource("http://example.com/dtProp"), 
              RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("this is a property", "en"));  //TODO: non viene riconosciuta in automatico l'annotazione rdfs:comment
            ontology.Model.PropertyModel.AnnotateProperty(new RDFResource("http://example.com/annProp"),
              RDFVocabulary.SKOS.CHANGE_NOTE, new RDFTypedLiteral("http://changenote/", RDFModelEnums.RDFDatatypes.XSD_ANYURI));
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializePropertyAnnotations.owx"));
            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializePropertyAnnotations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializePropertyAnnotations.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:skos=""http://www.w3.org/2004/02/skos/core#"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""skos"" IRI=""http://www.w3.org/2004/02/skos/core#"" />
  <Prefix name=""xsd"" IRI=""http://www.w3.org/2001/XMLSchema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objProp"" />
  </Declaration>
  <Declaration>
    <DataProperty IRI=""http://example.com/dtProp"" />
  </Declaration>
  <Declaration>
    <AnnotationProperty IRI=""http://example.com/annProp"" />
  </Declaration>
  <AnnotationAssertion>
    <AnnotationProperty IRI=""http://example.com/annProp"" />
    <IRI>http://example.com/objProp</IRI>
    <IRI>http://example.com/</IRI>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""owl:deprecated"" />
    <IRI>http://example.com/objProp</IRI>
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#boolean"">true</Literal>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""rdfs:seeAlso"" />
    <IRI>http://example.com/dtProp</IRI>
    <IRI>http://example.com/DtPropAbout</IRI>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""rdfs:comment"" />
    <IRI>http://example.com/dtProp</IRI>
    <Literal xml:lang=""EN"">this is a property</Literal>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""skos:changeNote"" />
    <IRI>http://example.com/annProp</IRI>
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#anyURI"">http://changenote/</Literal>
  </AnnotationAssertion>
  <AnnotationAssertion>
    <AnnotationProperty abbreviatedIRI=""owl:deprecated"" />
    <IRI>http://example.com/annProp</IRI>
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#boolean"">true</Literal>
  </AnnotationAssertion>
</Ontology>";
            Assert.IsTrue(fileContent.Equals(expectedFileContent));
        }

        //Data

        [TestMethod]
        public void ShouldSerializeClassAssertions()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Data.DeclareIndividual(new RDFResource("http://example.com/ID1"));
            ontology.Data.DeclareIndividual(RDFVocabulary.GEO.LOCATION);
            ontology.Data.DeclareIndividualType(new RDFResource("http://example.com/ID1"), new RDFResource("http://example.com/Cls1"));
            ontology.Data.DeclareIndividualType(RDFVocabulary.GEO.LOCATION, RDFVocabulary.GEO.SPATIAL_THING);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeClassAssertions.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeClassAssertions.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeClassAssertions.owx"));
            string expectedFileContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:geo=""http://www.w3.org/2003/01/geo/wgs84_pos#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""geo"" IRI=""http://www.w3.org/2003/01/geo/wgs84_pos#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <NamedIndividual IRI=""http://example.com/ID1"" />
  </Declaration>
  <Declaration>
    <NamedIndividual abbreviatedIRI=""geo:location"" />
  </Declaration>
  <ClassAssertion>
    <Class IRI=""http://example.com/Cls1"" />
    <NamedIndividual IRI=""http://example.com/ID1"" />
  </ClassAssertion>
  <ClassAssertion>
    <Class abbreviatedIRI=""geo:SpatialThing"" />
    <NamedIndividual abbreviatedIRI=""geo:location"" />
  </ClassAssertion>
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