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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLImportHelper simplifies OWLImport directive modeling with a set of facilities
    /// </summary>
    public static class OWLImportHelper
    {
        #region Properties
        /// <summary>
        /// Singleton cache of imported ontologies
        /// </summary>
        internal static readonly Dictionary<string, (OWLOntology Ontology, DateTime ExpireTimestamp)> OntologyCache
            = new Dictionary<string, (OWLOntology, DateTime)>();

        /// <summary>
        /// Semaphore to control access to the cache of imported ontologies
        /// </summary>
        private static readonly SemaphoreSlim OntologyCacheGuard = new SemaphoreSlim(1);
        #endregion

        #region Methods
        /// <summary>
        /// Tries to import the ontology at the given IRI into the given working ontology.
        /// In case of success, it also caches the retrieved ontology for the specified amount of milliseconds.
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static Task ImportAsync(this OWLOntology ontology, Uri ontologyIRI, int timeoutMilliseconds=20000, int cacheMilliseconds=3600000)
            => ImportAsync(ontology, ontologyIRI, timeoutMilliseconds, cacheMilliseconds, true);
        private static Task ImportAsync(this OWLOntology ontology, Uri ontologyIRI, int timeoutMilliseconds, int cacheMilliseconds, bool shouldCollectImport)
            => Task.Run(async () =>
                {
                    #region Guards
                    if (ontology == null)
                        throw new OWLException($"Cannot import ontology because given '{nameof(ontology)}' parameter is null");
                    if (ontologyIRI == null)
                        throw new OWLException($"Cannot import ontology because given '{nameof(ontologyIRI)}' parameter is null");
                    string ontologyIRIString = ontologyIRI.ToString();
                    #endregion

                    try
                    {
                        #region Cache
                        //Acquire semaphore
                        if (!await OntologyCacheGuard.WaitAsync(timeoutMilliseconds))
                            throw new OWLException($"could not acquire lock on the cache of imported ontologies within timeout period of '{timeoutMilliseconds}' milliseconds");

                        //Access cache
                        if (OntologyCache.ContainsKey(ontologyIRIString) && OntologyCache[ontologyIRIString].ExpireTimestamp < DateTime.UtcNow)
                            OntologyCache.Remove(ontologyIRIString);
                        if (!OntologyCache.ContainsKey(ontologyIRIString))
                        {
                            RDFGraph importGraph = await RDFGraph.FromUriAsync(ontologyIRI, timeoutMilliseconds, true);
                            OWLOntology importOntology = await OWLOntology.FromRDFGraphAsync(importGraph);
                            //Save the ontology into the cache for the given amount of milliseconds
                            OntologyCache.Add(ontologyIRIString, (importOntology, DateTime.UtcNow.AddMilliseconds(cacheMilliseconds)));
                        }
                        OWLOntology importedOntology = OntologyCache[ontologyIRIString].Ontology;
                        #endregion Cache

                        //Imports
                        if (shouldCollectImport)
                            ontology.Imports.Add(new OWLImport(new RDFResource(importedOntology.IRI)));

                        //Prefixes
                        importedOntology.Prefixes.ForEach(importingPrefix =>
                        {
                            if (!ontology.Prefixes.Any(prefix => string.Equals(prefix.Name, importingPrefix.Name, StringComparison.OrdinalIgnoreCase)))
                                ontology.Prefixes.Add(importingPrefix);
                        });

                        //Axioms
                        importedOntology.DeclarationAxioms.ForEach(ax => { ax.IsImport=true; ontology.DeclarationAxioms.Add(ax); });
                        importedOntology.ClassAxioms.ForEach(ax => { ax.IsImport=true; ontology.ClassAxioms.Add(ax); });
                        importedOntology.ObjectPropertyAxioms.ForEach(ax => { ax.IsImport=true; ontology.ObjectPropertyAxioms.Add(ax); });
                        importedOntology.DataPropertyAxioms.ForEach(ax => { ax.IsImport=true; ontology.DataPropertyAxioms.Add(ax); });
                        importedOntology.DatatypeDefinitionAxioms.ForEach(ax => { ax.IsImport=true; ontology.DatatypeDefinitionAxioms.Add(ax); });
                        importedOntology.KeyAxioms.ForEach(ax => { ax.IsImport=true; ontology.KeyAxioms.Add(ax); });
                        importedOntology.AssertionAxioms.ForEach(ax => { ax.IsImport=true; ontology.AssertionAxioms.Add(ax); });
                        importedOntology.AnnotationAxioms.ForEach(ax => { ax.IsImport=true; ontology.AnnotationAxioms.Add(ax); });

                        //Rules
                        importedOntology.Rules.ForEach(rl => { rl.IsImport=true; ontology.Rules.Add(rl); });
                    }
                    catch (Exception ex)
                    {
                        throw new OWLException($"Cannot import ontology from IRI {ontologyIRI} because: {ex.Message}", ex);
                    }
                    finally
                    {
                        //Release semaphore
                        OntologyCacheGuard.Release();
                    }
                });

        /// <summary>
        /// Tries to resolve each OWLImport directive found in the given ontology.
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static Task ResolveImportsAsync(this OWLOntology ontology, int timeoutMilliseconds=20000, int cacheMilliseconds=3600000)
            => Task.Run(async () =>
                {
                    foreach (OWLImport import in ontology?.Imports ?? Enumerable.Empty<OWLImport>())
                        await ImportAsync(ontology, new Uri(import.IRI), timeoutMilliseconds, cacheMilliseconds, false);
                });
        #endregion
    }
}