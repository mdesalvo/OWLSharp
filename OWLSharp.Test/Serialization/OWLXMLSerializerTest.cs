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
    public class OWLXMLSerializerTest
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

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithImport()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnSerializingAndDeserializingOntologyWithBadFormedImport()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));

            Assert.ThrowsException<OWLException>(() => ontology.Imports.Add(new OWLImport(null)));
            Assert.ThrowsException<OWLException>(() => ontology.Imports.Add(new OWLImport(new RDFResource())));
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithSubClassOfAxiomHavingIRI()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new RDFResource("http://example.org/Cls1")), 
                new OWLClass(new RDFResource("http://example.org/Cls2"))));

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
        public void ShouldSerializeAndDeserializeOntologyWithSubClassOfAxiomHavingAbbreviatedIRI()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(new RDFNamespace("ex", "http://example.org/classes/")));
            ontology.Imports.Add(new OWLImport(new RDFResource("http://example.org/import/")));
            ontology.Axioms.Add(new OWLSubClassOfAxiom(
                new OWLClass(new XmlQualifiedName("Cls1", "http://example.org/classes/")),
                new OWLClass(new RDFResource("http://example.org/Cls2"))));

            string owxOntology = OWLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
            Assert.IsTrue(ontology2.Imports.Count == 1);
            Assert.IsTrue(ontology2.Axioms.Count == 1);
        }
        #endregion
    }
}