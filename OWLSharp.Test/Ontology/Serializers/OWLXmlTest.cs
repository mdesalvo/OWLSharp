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
        private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        private const string XmlBaseDefault = "xml:base=\"https://rdfsharp.codeplex.com/\"";
        private const string XmlBaseExample = "xml:base=\"http://example.com/\"";
        private const string XmlNsDefault = "xmlns=\"https://rdfsharp.codeplex.com/\"";
        private const string XmlNsOWL = "xmlns:owl=\"http://www.w3.org/2002/07/owl#\"";
        private const string XmlNsRDF = "xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"";
        private const string XmlNsXSD = "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema#\"";

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
<Ontology ontologyIRI=""https://rdfsharp.codeplex.com/"" xmlns:rdfsharp=""https://rdfsharp.codeplex.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xml:base=""https://rdfsharp.codeplex.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdfsharp"" IRI=""https://rdfsharp.codeplex.com/"" />
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
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
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix IRI=""http://example.com/"" />
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
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
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
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
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
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix name=""rdfs"" IRI=""http://www.w3.org/2000/01/rdf-schema#"" />
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
<Ontology ontologyIRI=""http://example.com/"" xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" xmlns:owl=""http://www.w3.org/2002/07/owl#"" xml:base=""http://example.com/"" xmlns=""http://www.w3.org/2002/07/owl#"">
  <Prefix name=""rdf"" IRI=""http://www.w3.org/1999/02/22-rdf-syntax-ns#"" />
  <Prefix name=""owl"" IRI=""http://www.w3.org/2002/07/owl#"" />
  <Prefix IRI=""http://example.com/"" />
  <Annotation>
    <AnnotationProperty IRI=""http://ann.prop/""></AnnotationProperty>
    <IRI>http://ann.obj/</IRI>
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