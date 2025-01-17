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

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLImportHelperTest
    {
        #region Tests
        [TestMethod]
        public async Task ShouldImportOntologyAsync()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
            await ontology.ImportAsync(new Uri(RDFVocabulary.SKOS.DEREFERENCE_URI));

            Assert.IsTrue(ontology.Imports.Count == 1);
            Assert.IsTrue(string.Equals(ontology.Imports.Single().IRI, "http://www.w3.org/2004/02/skos/core"));
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 51);
            Assert.IsTrue(ontology.AnnotationAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.ClassAxioms.Count == 4);
            Assert.IsTrue(ontology.ClassAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.DeclarationAxioms.Count == 32);
            Assert.IsTrue(ontology.DeclarationAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 41);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.AssertionAxioms.Count == 0);
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 0);
            Assert.IsTrue(ontology.DatatypeDefinitionAxioms.Count == 0);
            Assert.IsTrue(ontology.KeyAxioms.Count == 0);
            Assert.IsTrue(ontology.Prefixes.Count == 5);
            Assert.IsTrue(ontology.Rules.Count == 0);

            Assert.IsTrue(OWLImportHelper.OntologyCache.TryGetValue(RDFVocabulary.SKOS.DEREFERENCE_URI, out var cachedSKOSOntology)
                           && cachedSKOSOntology.ExpireTimestamp > DateTime.UtcNow);
        }

        [TestMethod]
        public async Task ShouldResolveImportsAsync()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
            ontology.Imports.Add(new OWLImport(new RDFResource(RDFVocabulary.SKOS.DEREFERENCE_URI)));
            ontology.Imports.Add(new OWLImport(new RDFResource(RDFVocabulary.FOAF.DEREFERENCE_URI)));
            await ontology.ResolveImportsAsync();

            Assert.IsTrue(ontology.Imports.Count == 2);
            Assert.IsTrue(ontology.AnnotationAxioms.Count == 151);
            Assert.IsTrue(ontology.AnnotationAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.ClassAxioms.Count == 21);
            Assert.IsTrue(ontology.ClassAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.DeclarationAxioms.Count == 140);
            Assert.IsTrue(ontology.DeclarationAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.ObjectPropertyAxioms.Count == 106);
            Assert.IsTrue(ontology.ObjectPropertyAxioms.TrueForAll(ax => ax.IsImport));
            Assert.IsTrue(ontology.AssertionAxioms.Count == 0);
            Assert.IsTrue(ontology.DataPropertyAxioms.Count == 29);
            Assert.IsTrue(ontology.DatatypeDefinitionAxioms.Count == 0);
            Assert.IsTrue(ontology.KeyAxioms.Count == 0);
            Assert.IsTrue(ontology.Prefixes.Count == 5);
            Assert.IsTrue(ontology.Rules.Count == 0);

            Assert.IsTrue(OWLImportHelper.OntologyCache.TryGetValue(RDFVocabulary.SKOS.DEREFERENCE_URI, out var cachedSKOSOntology)
                           && cachedSKOSOntology.ExpireTimestamp > DateTime.UtcNow);
            Assert.IsTrue(OWLImportHelper.OntologyCache.TryGetValue(RDFVocabulary.FOAF.DEREFERENCE_URI, out var cachedFOAFOntology)
                           && cachedFOAFOntology.ExpireTimestamp > DateTime.UtcNow);
        }

        [TestMethod]
        public async Task ShouldThrowExceptionOnImportingNullOntologyAsync()
        {
            OWLOntology ontology = null;
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await ontology.ImportAsync(new Uri("ex:ont"), 5, 5));
        }

        [TestMethod]
        public async Task ShouldThrowExceptionOnImportingNullIRIOntologyAsync()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await ontology.ImportAsync(null, 5, 5));
        }

        [TestMethod]
        public async Task ShouldThrowExceptionOnImportingTimeoutOntologyAsync()
        {
            OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
            await Assert.ThrowsExceptionAsync<OWLException>(async () => await ontology.ImportAsync(new Uri("ex:ont"), 5, 5));
        }
        #endregion
    }
}