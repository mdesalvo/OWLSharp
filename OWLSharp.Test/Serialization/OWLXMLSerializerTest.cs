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

            string owxOntology = OWLXMLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLXMLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsNull(ontology2.OntologyVersion);
            Assert.IsTrue(ontology2.Prefixes.Count == 5);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithIRIandVersion()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));

            string owxOntology = OWLXMLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLXMLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 5);
        }

        [TestMethod]
        public void ShouldSerializeAndDeserializeOntologyWithPrefix()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            ontology.Prefixes.Add(new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX)));

            string owxOntology = OWLXMLSerializer.Serialize(ontology);

            OWLOntology ontology2 = OWLXMLSerializer.Deserialize(owxOntology);

            Assert.IsNotNull(ontology2);
            Assert.IsTrue(string.Equals(ontology2.OntologyIRI, "http://example.org/"));
            Assert.IsTrue(string.Equals(ontology2.OntologyVersion, "http://example.org/v1"));
            Assert.IsTrue(ontology2.Prefixes.Count == 6);
        }
        #endregion
    }
}