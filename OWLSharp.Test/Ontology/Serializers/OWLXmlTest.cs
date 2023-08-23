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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
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
  <Prefix name="""" IRI=""http://example.com/"" />
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
  <Prefix name="""" IRI=""http://example.com/"" />
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
  <Prefix name="""" IRI=""http://example.com/"" />
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
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
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/someValuesFromRestriction"" />
    <ObjectSomeValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectSomeValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
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
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/someValuesFromRestriction"" />
    <DataSomeValuesFrom>
      <DataProperty IRI=""http://example.com/datatypeProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </DataSomeValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
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
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
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
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <DataAllValuesFrom>
      <DataProperty IRI=""http://example.com/datatypeProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </DataAllValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <DataProperty IRI=""http://example.com/datatypeProperty"" />
  </Declaration>
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
            const string expectedFileContent=
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
  <EquivalentClasses>
    <Class abbreviatedIRI=""geo:SpatialThing"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
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
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""bnode://12345"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
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
            const string expectedFileContent=
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
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class abbreviatedIRI=""geo:SpatialThing"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
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
            const string expectedFileContent=
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""xml"" IRI=""http://www.w3.org/XML/1998/namespace"" />
  <Prefix name="""" IRI=""http://example.com/"" />
  <Declaration>
    <Class IRI=""http://example.com/SimpleClass"" />
  </Declaration>
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <ObjectAllValuesFrom>
      <ObjectProperty IRI=""http://example.com/objectProperty"" />
      <Class IRI=""bnode://12345"" />
    </ObjectAllValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <ObjectProperty IRI=""http://example.com/objectProperty"" />
  </Declaration>
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
            const string expectedFileContent=
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
  <EquivalentClasses>
    <Class IRI=""http://example.com/allValuesFromRestriction"" />
    <DataAllValuesFrom>
      <DataProperty abbreviatedIRI=""geo:lat_long"" />
      <Class IRI=""http://example.com/SimpleClass"" />
    </DataAllValuesFrom>
  </EquivalentClasses>
  <Declaration>
    <DataProperty abbreviatedIRI=""geo:lat_long"" />
  </Declaration>
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
        public void ShouldSerializeNamedIndividualDeclarations()
        {
            OWLOntology ontology = new OWLOntology("http://example.com/");
            ontology.Data.DeclareIndividual(new RDFResource("http://example.com/ID1"));
            ontology.Data.DeclareIndividual(RDFVocabulary.GEO.LOCATION);
            OWLXml.Serialize(ontology, Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeNamedIndividualDeclarations.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeNamedIndividualDeclarations.owx")));
            string fileContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"OWLXmlTest_ShouldSerializeNamedIndividualDeclarations.owx"));
            const string expectedFileContent=
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

        [TestCleanup]
        public void Cleanup()
        {
            foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory, "OWLXmlTest_Should*"))
                File.Delete(file);
        }
        #endregion
    }
}