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

namespace OWLSharp.Test.Ontology;

[TestClass]
public class OWLImportHelperTest
{
    #region Tests
    [TestMethod]
    public async Task ShouldImportOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
        await ontology.ImportAsync(new Uri(RDFVocabulary.SKOS.DEREFERENCE_URI));

        Assert.HasCount(1, ontology.Imports);
        Assert.IsTrue(string.Equals(ontology.Imports.Single().IRI, "http://www.w3.org/2004/02/skos/core"));
        Assert.HasCount(51, ontology.AnnotationAxioms);
        Assert.IsTrue(ontology.AnnotationAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(4, ontology.ClassAxioms);
        Assert.IsTrue(ontology.ClassAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(32, ontology.DeclarationAxioms);
        Assert.IsTrue(ontology.DeclarationAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(41, ontology.ObjectPropertyAxioms);
        Assert.IsTrue(ontology.ObjectPropertyAxioms.TrueForAll(ax => ax.IsImport));
        Assert.IsEmpty(ontology.AssertionAxioms);
        Assert.IsEmpty(ontology.DataPropertyAxioms);
        Assert.IsEmpty(ontology.DatatypeDefinitionAxioms);
        Assert.IsEmpty(ontology.KeyAxioms);
        Assert.HasCount(5, ontology.Prefixes);
        Assert.IsEmpty(ontology.Rules);

        Assert.IsTrue(OWLImportHelper.OntologyCache.TryGetValue(RDFVocabulary.SKOS.DEREFERENCE_URI, out var cachedSKOSOntology)
                      && cachedSKOSOntology.ExpireTimestamp > DateTime.UtcNow);
    }

    [TestMethod]
    public async Task ShouldResolveImportsAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
        await ontology.ResolveImportsAsync();

        Assert.IsEmpty(ontology.Imports);

        ontology.Imports.Add(new OWLImport(new RDFResource(RDFVocabulary.SKOS.DEREFERENCE_URI)));
        ontology.Imports.Add(new OWLImport(new RDFResource(RDFVocabulary.TIME.DEREFERENCE_URI)));
        ontology.Imports.Add(new OWLImport(new RDFResource(RDFVocabulary.GEOSPARQL.DEREFERENCE_URI)));
        await ontology.ResolveImportsAsync();

        Assert.HasCount(3, ontology.Imports);
        Assert.HasCount(53, ontology.AnnotationAxioms);
        Assert.IsTrue(ontology.AnnotationAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(65, ontology.ClassAxioms);
        Assert.IsTrue(ontology.ClassAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(200, ontology.DeclarationAxioms);
        Assert.IsTrue(ontology.DeclarationAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(238, ontology.ObjectPropertyAxioms);
        Assert.IsTrue(ontology.ObjectPropertyAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(63, ontology.AssertionAxioms);
        Assert.IsTrue(ontology.AssertionAxioms.TrueForAll(ax => ax.IsImport));
        Assert.HasCount(57, ontology.DataPropertyAxioms);
        Assert.IsEmpty(ontology.DatatypeDefinitionAxioms);
        Assert.IsEmpty(ontology.KeyAxioms);
        Assert.HasCount(5, ontology.Prefixes);
        Assert.IsEmpty(ontology.Rules);

        Assert.IsTrue(OWLImportHelper.OntologyCache.TryGetValue(RDFVocabulary.SKOS.DEREFERENCE_URI, out var cachedSKOSOntology)
                       && cachedSKOSOntology.ExpireTimestamp > DateTime.UtcNow);
        Assert.IsTrue(OWLImportHelper.OntologyCache.TryGetValue(RDFVocabulary.TIME.DEREFERENCE_URI, out var cachedFOAFOntology)
                       && cachedFOAFOntology.ExpireTimestamp > DateTime.UtcNow);
        Assert.IsTrue(OWLImportHelper.OntologyCache.TryGetValue(RDFVocabulary.GEOSPARQL.DEREFERENCE_URI, out var cachedGEOSPARQLOntology)
                       && cachedGEOSPARQLOntology.ExpireTimestamp > DateTime.UtcNow);
    }

    [TestMethod]
    public async Task ShouldThrowExceptionOnImportingNullOntologyAsync()
    {
        await Assert.ThrowsExactlyAsync<OWLException>(() => (null as OWLOntology).ImportAsync(new Uri("ex:ont"), 5, 5));
    }

    [TestMethod]
    public async Task ShouldThrowExceptionOnImportingNullIRIOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
        await Assert.ThrowsExactlyAsync<OWLException>(() => ontology.ImportAsync(null, 5, 5));
    }

    [TestMethod]
    public async Task ShouldThrowExceptionOnImportingTimeoutOntologyAsync()
    {
        OWLOntology ontology = new OWLOntology(new Uri("ex:ont"));
        await Assert.ThrowsExactlyAsync<OWLException>(() => ontology.ImportAsync(new Uri("ex:ont"), 5, 5));
    }
    #endregion
}