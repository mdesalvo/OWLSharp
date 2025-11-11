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

using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLOntology represents a formal, structured representation of knowledge about a domain,
    /// consisting of a set of axioms that define entities (classes, properties, individuals, datatypes)
    /// and their relationships, constraints, and characteristics.
    /// An ontology is identified by an IRI and may include metadata, imports of other ontologies, and declarations.
    /// It can also contain an R-BOX (Rule Box) with SWRL inference rules that extend reasoning capabilities
    /// beyond standard OWL2 semantics, serving as a comprehensive knowledge base that enables automated reasoning,
    /// data integration, and semantic interoperability.
    /// </summary>
    [XmlRoot("Ontology")]
#if NET8_0_OR_GREATER
    public sealed class OWLOntology : IDisposable, IAsyncDisposable
#else
    public sealed class OWLOntology : IDisposable
#endif
    {
        #region Properties
        /// <summary>
        /// The IRI of the ontology (e.g: http://www.w3.org/2000/01/rdf-schema#)
        /// </summary>
        [XmlAttribute("ontologyIRI", DataType="anyURI")]
        public string IRI { get; set; }

        /// <summary>
        /// The version IRI of the ontology, which eventually represents the current version of the artifact
        /// </summary>
        [XmlAttribute("ontologyVersion", DataType="anyURI")]
        public string VersionIRI { get; set; }

        /// <summary>
        /// The set of prefixes which might be used for shortening IRIs of the ontology axioms (e.g: rdf, rdfs)
        /// </summary>
        [XmlElement("Prefix")]
        public List<OWLPrefix> Prefixes { get; internal set; }

        /// <summary>
        /// The set of imported ontologies, from which this ontology is expected to use some terms in its T-BOX or A-BOX
        /// </summary>
        [XmlElement("Import")]
        public List<OWLImport> Imports { get; internal set; }

        /// <summary>
        /// The set of annotations about this ontology (e.g: comment, label, author)
        /// </summary>
        [XmlElement("Annotation")]
        public List<OWLAnnotation> Annotations { get; internal set; }

        //Axioms

        /// <summary>
        /// Axioms stating the formal existence of well-known ontology entities (classes, object properties, ...)
        /// </summary>
        [XmlElement("Declaration")]
        public List<OWLDeclaration> DeclarationAxioms { get; internal set; }

        /// <summary>
        /// Axioms stating any kind of possible relationships between the classes of the ontology domain
        /// </summary>
        [XmlElement(typeof(OWLSubClassOf), ElementName="SubClassOf")]
        [XmlElement(typeof(OWLEquivalentClasses), ElementName="EquivalentClasses")]
        [XmlElement(typeof(OWLDisjointClasses), ElementName="DisjointClasses")]
        [XmlElement(typeof(OWLDisjointUnion), ElementName="DisjointUnion")]
        public List<OWLClassAxiom> ClassAxioms { get; internal set; }

        /// <summary>
        /// Axioms stating any kind of possible relationships between the object properties of the ontology domain
        /// </summary>
        [XmlElement(typeof(OWLSubObjectPropertyOf), ElementName="SubObjectPropertyOf")]
        [XmlElement(typeof(OWLEquivalentObjectProperties), ElementName="EquivalentObjectProperties")]
        [XmlElement(typeof(OWLDisjointObjectProperties), ElementName="DisjointObjectProperties")]
        [XmlElement(typeof(OWLInverseObjectProperties), ElementName="InverseObjectProperties")]
        [XmlElement(typeof(OWLObjectPropertyDomain), ElementName="ObjectPropertyDomain")]
        [XmlElement(typeof(OWLObjectPropertyRange), ElementName="ObjectPropertyRange")]
        [XmlElement(typeof(OWLFunctionalObjectProperty), ElementName="FunctionalObjectProperty")]
        [XmlElement(typeof(OWLInverseFunctionalObjectProperty), ElementName="InverseFunctionalObjectProperty")]
        [XmlElement(typeof(OWLReflexiveObjectProperty), ElementName="ReflexiveObjectProperty")]
        [XmlElement(typeof(OWLIrreflexiveObjectProperty), ElementName="IrreflexiveObjectProperty")]
        [XmlElement(typeof(OWLSymmetricObjectProperty), ElementName="SymmetricObjectProperty")]
        [XmlElement(typeof(OWLAsymmetricObjectProperty), ElementName="AsymmetricObjectProperty")]
        [XmlElement(typeof(OWLTransitiveObjectProperty), ElementName="TransitiveObjectProperty")]
        public List<OWLObjectPropertyAxiom> ObjectPropertyAxioms { get; internal set; }

        /// <summary>
        /// Axioms stating any kind of possible relationships between the data properties of the ontology domain
        /// </summary>
        [XmlElement(typeof(OWLSubDataPropertyOf), ElementName="SubDataPropertyOf")]
        [XmlElement(typeof(OWLEquivalentDataProperties), ElementName="EquivalentDataProperties")]
        [XmlElement(typeof(OWLDisjointDataProperties), ElementName="DisjointDataProperties")]
        [XmlElement(typeof(OWLDataPropertyDomain), ElementName="DataPropertyDomain")]
        [XmlElement(typeof(OWLDataPropertyRange), ElementName="DataPropertyRange")]
        [XmlElement(typeof(OWLFunctionalDataProperty), ElementName="FunctionalDataProperty")]
        public List<OWLDataPropertyAxiom> DataPropertyAxioms { get; internal set; }

        /// <summary>
        /// Axioms stating the formal definition of custom datatypes characterizing the ontology domain
        /// </summary>
        [XmlElement(ElementName="DatatypeDefinition")]
        public List<OWLDatatypeDefinition> DatatypeDefinitionAxioms { get; internal set; }

        /// <summary>
        /// Axioms formalizing the "semantic uniqueness" of the individuals of a given class,
        /// specifying the properties on which their values should be assumed to be unique
        /// </summary>
        [XmlElement(ElementName="HasKey")]
        public List<OWLHasKey> KeyAxioms { get; internal set; }

        /// <summary>
        /// Axioms stating any kind of possible relationships between the individuals of the ontology domain
        /// </summary>
        [XmlElement(typeof(OWLSameIndividual), ElementName="SameIndividual")]
        [XmlElement(typeof(OWLDifferentIndividuals), ElementName="DifferentIndividuals")]
        [XmlElement(typeof(OWLClassAssertion), ElementName="ClassAssertion")]
        [XmlElement(typeof(OWLObjectPropertyAssertion), ElementName="ObjectPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeObjectPropertyAssertion), ElementName="NegativeObjectPropertyAssertion")]
        [XmlElement(typeof(OWLDataPropertyAssertion), ElementName="DataPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeDataPropertyAssertion), ElementName="NegativeDataPropertyAssertion")]
        public List<OWLAssertionAxiom> AssertionAxioms { get; internal set; }

        /// <summary>
        /// Axioms stating any kind of possible relationships between the annotation properties of the ontology domain,
        /// along with expressing annotations describing any kind of declared ontology entity (e.g: comment, label)
        /// </summary>
        [XmlElement(typeof(OWLAnnotationAssertion), ElementName="AnnotationAssertion")]
        [XmlElement(typeof(OWLSubAnnotationPropertyOf), ElementName="SubAnnotationPropertyOf")]
        [XmlElement(typeof(OWLAnnotationPropertyDomain), ElementName="AnnotationPropertyDomain")]
        [XmlElement(typeof(OWLAnnotationPropertyRange), ElementName="AnnotationPropertyRange")]
        public List<OWLAnnotationAxiom> AnnotationAxioms { get; internal set; }

        //Rules

        /// <summary>
        /// DL-safe rules expressed in SWRL, which are available to any ontology reasoner for processing
        /// </summary>
        [XmlElement("DLSafeRule")]
        public List<SWRLRule> Rules { get; internal set; }

        /// <summary>
        /// Flag indicating that the ontology has already been disposed
        /// </summary>
        [XmlIgnore]
        internal bool Disposed { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an empty ontology having just the predefines set of prefixes (rdf, rdfs, xml, xsd, owl)
        /// </summary>
        public OWLOntology()
        {
            Prefixes = new List<OWLPrefix>
            {
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.OWL.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDFS.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XSD.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX))
            };
            Imports = new List<OWLImport>();
            Annotations = new List<OWLAnnotation>();

            //Axioms
            DeclarationAxioms = new List<OWLDeclaration>();
            ClassAxioms = new List<OWLClassAxiom>();
            ObjectPropertyAxioms = new List<OWLObjectPropertyAxiom>();
            DataPropertyAxioms = new List<OWLDataPropertyAxiom>();
            DatatypeDefinitionAxioms = new List<OWLDatatypeDefinition>();
            KeyAxioms = new List<OWLHasKey>();
            AssertionAxioms = new List<OWLAssertionAxiom>();
            AnnotationAxioms = new List<OWLAnnotationAxiom>();

            //Rules
            Rules = new List<SWRLRule>();
        }

        /// <summary>
        /// Builds an ontology having the given IRI and eventual version IRI
        /// </summary>
        public OWLOntology(Uri ontologyIRI, Uri ontologyVersionIRI=null) : this()
        {
            IRI = ontologyIRI?.ToString();
            VersionIRI = ontologyVersionIRI?.ToString();
        }

        /// <summary>
        /// Builds an ontology having the same IRI, version IRI, prefixes, T-BOX, A-BOX and R-BOX of the given one
        /// </summary>
        public OWLOntology(OWLOntology ontology)
        {
            IRI = ontology?.IRI;
            VersionIRI = ontology?.VersionIRI;
            Prefixes = ontology?.Prefixes.ToList() ??
                        new List<OWLPrefix>
                        {
                            new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.OWL.PREFIX)),
                            new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDFS.PREFIX)),
                            new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX)),
                            new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XSD.PREFIX)),
                            new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX))
                        };
            Imports = new List<OWLImport>(ontology?.Imports ?? Enumerable.Empty<OWLImport>());
            Annotations = new List<OWLAnnotation>(ontology?.Annotations ?? Enumerable.Empty<OWLAnnotation>());

            //Axioms
            DeclarationAxioms = new List<OWLDeclaration>(ontology?.DeclarationAxioms ?? Enumerable.Empty<OWLDeclaration>());
            ClassAxioms = new List<OWLClassAxiom>(ontology?.ClassAxioms ?? Enumerable.Empty<OWLClassAxiom>());
            ObjectPropertyAxioms = new List<OWLObjectPropertyAxiom>(ontology?.ObjectPropertyAxioms ?? Enumerable.Empty<OWLObjectPropertyAxiom>());
            DataPropertyAxioms = new List<OWLDataPropertyAxiom>(ontology?.DataPropertyAxioms ?? Enumerable.Empty<OWLDataPropertyAxiom>());
            DatatypeDefinitionAxioms = new List<OWLDatatypeDefinition>(ontology?.DatatypeDefinitionAxioms ?? Enumerable.Empty<OWLDatatypeDefinition>());
            KeyAxioms = new List<OWLHasKey>(ontology?.KeyAxioms ?? Enumerable.Empty<OWLHasKey>());
            AssertionAxioms = new List<OWLAssertionAxiom>(ontology?.AssertionAxioms ?? Enumerable.Empty<OWLAssertionAxiom>());
            AnnotationAxioms = new List<OWLAnnotationAxiom>(ontology?.AnnotationAxioms ?? Enumerable.Empty<OWLAnnotationAxiom>());

            //Rules
            Rules = new List<SWRLRule>(ontology?.Rules ?? Enumerable.Empty<SWRLRule>());
        }

        /// <summary>
        /// Destroys the ontology instance
        /// </summary>
        ~OWLOntology()
            => Dispose(false);
        #endregion

        #region Interfaces
        /// <summary>
        /// Disposes the ontology (IDisposable)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Asynchronously disposes the ontology (IAsyncDisposable)
        /// </summary>
        ValueTask IAsyncDisposable.DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
#endif

        /// <summary>
        /// Disposes the ontology
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Prefixes?.Clear();
                Prefixes = null;
                Imports?.Clear();
                Imports = null;
                Annotations?.Clear();
                Annotations = null;
                DeclarationAxioms?.Clear();
                DeclarationAxioms = null;
                ClassAxioms?.Clear();
                ClassAxioms = null;
                ObjectPropertyAxioms?.Clear();
                ObjectPropertyAxioms = null;
                DataPropertyAxioms?.Clear();
                DataPropertyAxioms = null;
                DatatypeDefinitionAxioms?.Clear();
                DatatypeDefinitionAxioms = null;
                KeyAxioms?.Clear();
                KeyAxioms = null;
                AssertionAxioms?.Clear();
                AssertionAxioms = null;
                AnnotationAxioms?.Clear();
                AnnotationAxioms = null;
                Rules?.Clear();
                Rules = null;
            }

            Disposed = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given annotation to the set of this ontology's annotations
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public void Annotate(OWLAnnotation annotation)
            => Annotations.Add(annotation ?? throw new OWLException($"Cannot annotate ontology because given '{nameof(annotation)}' parameter is null"));

        /// <summary>
        /// Adds the given prefix to the set of this ontology's prefixes
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public void Prefix(OWLPrefix prefix)
            => Prefixes.Add(prefix ?? throw new OWLException($"Cannot prefix ontology because given '{nameof(prefix)}' parameter is null"));

        /// <summary>
        /// Exports this ontology to an equivalent RDFGraph object (includes inferred axioms by default)
        /// </summary>
        public Task<RDFGraph> ToRDFGraphAsync(bool includeInferences=true)
            => ToRDFGraphAsync(includeInferences, false);
        internal Task<RDFGraph> ToRDFGraphAsync(bool includeInferences, bool includeImports)
            => Task.Run(() =>
                {
                    RDFGraph graph = new RDFGraph();

                    //IRI
                    RDFResource ontologyIRI = new RDFResource();
                    if (!string.IsNullOrWhiteSpace(IRI))
                        ontologyIRI = new RDFResource(IRI);
                    graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));

                    //VersionIRI
                    if (!string.IsNullOrWhiteSpace(VersionIRI))
                        graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.OWL.VERSION_IRI, new RDFResource(VersionIRI)));

                    //Imports
                    foreach (OWLImport import in Imports)
                        graph.AddTriple(new RDFTriple(ontologyIRI, RDFVocabulary.OWL.IMPORTS, new RDFResource(import.IRI)));

                    //Annotations
                    foreach (OWLAnnotation annotation in Annotations)
                        graph = graph.UnionWith(annotation.ToRDFGraphInternal(ontologyIRI));

                    //Axioms (consider inferred/imported axioms only if specified)
                    foreach (OWLDeclaration declarationAxiom in DeclarationAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(declarationAxiom.ToRDFGraph());
                    foreach (OWLClassAxiom classAxiom in ClassAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(classAxiom.ToRDFGraph());
                    foreach (OWLObjectPropertyAxiom objectPropertyAxiom in ObjectPropertyAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(objectPropertyAxiom.ToRDFGraph());
                    foreach (OWLDataPropertyAxiom dataPropertyAxiom in DataPropertyAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(dataPropertyAxiom.ToRDFGraph());
                    foreach (OWLDatatypeDefinition datatypeDefinitionAxiom in DatatypeDefinitionAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(datatypeDefinitionAxiom.ToRDFGraph());
                    foreach (OWLHasKey keyAxiom in KeyAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(keyAxiom.ToRDFGraph());
                    foreach (OWLAssertionAxiom assertionAxiom in AssertionAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(assertionAxiom.ToRDFGraph());
                    foreach (OWLAnnotationAxiom annotationAxiom in AnnotationAxioms.Where(ax => (includeImports || !ax.IsImport) && (includeInferences || !ax.IsInference)))
                        graph = graph.UnionWith(annotationAxiom.ToRDFGraph());

                    //Rules (consider imported axioms only if specified)
                    foreach (SWRLRule rule in Rules.Where(rl => includeImports || !rl.IsImport))
                        graph = graph.UnionWith(rule.ToRDFGraph());

                    //IRI => Context
                    if (!ontologyIRI.IsBlank)
                        graph.SetContext(ontologyIRI.URI);

                    return graph;
                });

        /// <summary>
        /// Exports this ontology to a file serialized in the given format (includes inferred axioms by default)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public Task ToFileAsync(OWLEnums.OWLFormats owlFormat, string outputFile, bool includeInferences=true)
            => string.IsNullOrWhiteSpace(outputFile)
                ? throw new OWLException($"Cannot write ontology to file because given '{nameof(outputFile)}' parameter is null or empty")
                : ToStreamAsync(owlFormat, new FileStream(outputFile, FileMode.Create), includeInferences);

        /// <summary>
        /// Exports this ontology to a stream serialized in the given format (includes inferred axioms by default)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public Task ToStreamAsync(OWLEnums.OWLFormats owlFormat, Stream outputStream, bool includeInferences=true)
            => Task.Run(() =>
                {
                    if (outputStream == null)
                        throw new OWLException($"Cannot write ontology to stream because given '{nameof(outputStream)}' parameter is null");

                    #region Exclude Imports/Inferences
                    OWLOntology exportOntology = new OWLOntology(this);

                    //Axioms
                    exportOntology.DeclarationAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));
                    exportOntology.ClassAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));
                    exportOntology.ObjectPropertyAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));
                    exportOntology.DataPropertyAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));
                    exportOntology.DatatypeDefinitionAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));
                    exportOntology.KeyAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));
                    exportOntology.AssertionAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));
                    exportOntology.AnnotationAxioms.RemoveAll(ax => ax.IsImport || (!includeInferences && ax.IsInference));

                    //Rules
                    exportOntology.Rules.RemoveAll(rl => rl.IsImport);
                    #endregion

                    try
                    {
                        switch (owlFormat)
                        {
                            case OWLEnums.OWLFormats.OWL2XML:
                                using (StreamWriter streamWriter = new StreamWriter(outputStream, RDFModelUtilities.UTF8_NoBOM))
                                    streamWriter.Write(OWLSerializer.SerializeOntology(exportOntology));
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new OWLException($"Cannot write ontology to stream because: {ex.Message}", ex);
                    }
                });

        /// <summary>
        /// Imports the given RDFGraph object into an equivalent OWLOntology object.<br/><br/>
        /// Consider that translating from RDF expressivity to OWL2 expressivity is lossy by definition,
        /// so that unrepresentable semantic artifacts might be lost (e.g: lists, collections)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static Task<OWLOntology> FromRDFGraphAsync(RDFGraph graph)
            => OWLOntologyHelper.FromRDFGraphAsync(graph);

        /// <summary>
        /// Imports the given file serialized in the given format into an OWLOntology object
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static Task<OWLOntology> FromFileAsync(OWLEnums.OWLFormats owlFormat, string inputFile)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new OWLException($"Cannot read ontology from file because given '{nameof(inputFile)}' parameter is null or empty");
            if (!File.Exists(inputFile))
                throw new OWLException($"Cannot read ontology from file because given '{nameof(inputFile)}' parameter ({inputFile}) does not indicate an existing file");

            return FromStreamAsync(owlFormat, new FileStream(inputFile, FileMode.Open));
        }

        /// <summary>
        /// Imports the given stream serialized in the given format into an OWLOntology object
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static Task<OWLOntology> FromStreamAsync(OWLEnums.OWLFormats owlFormat, Stream inputStream)
            => Task.Run(() =>
                {
                    if (inputStream == null)
                        throw new OWLException($"Cannot read ontology from stream because given '{nameof(inputStream)}' parameter is null");

                    try
                    {
                        switch (owlFormat)
                        {
                            case OWLEnums.OWLFormats.OWL2XML:
                            {
                                using (StreamReader streamReader = new StreamReader(inputStream, RDFModelUtilities.UTF8_NoBOM))
                                    return OWLSerializer.DeserializeOntology(streamReader.ReadToEnd());
                            }
                            default: throw new NotSupportedException($"{owlFormat} format is not supported");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new OWLException($"Cannot read ontology from stream because: {ex.Message}", ex);
                    }
                });

        /// <summary>
        /// Imports the given IRI into an OWLOntology object (aborting with failure in case of timeout)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static async Task<OWLOntology> FromUriAsync(Uri uri, int timeoutMilliseconds=20000)
        {
            if (uri == null)
                throw new OWLException($"Cannot read ontology from Uri because given '{nameof(uri)}' parameter is null or empty");
            if (!uri.IsAbsoluteUri)
                throw new OWLException($"Cannot read ontology from Uri because given '{nameof(uri)}' parameter does not represent an absolute Uri.");

            try
            {
                //Grab eventual dereference Uri
                Uri remappedUri = RDFModelUtilities.RemapUriForDereference(uri);

                HttpWebRequest webRequest = WebRequest.CreateHttp(remappedUri);
                webRequest.MaximumAutomaticRedirections = 3;
                webRequest.AllowAutoRedirect = true;
                webRequest.Timeout = timeoutMilliseconds;
                webRequest.Accept = "application/owl+xml,application/rdf+xml,text/turtle,application/turtle,application/x-turtle,application/n-triples";

                WebResponse webResponse = await webRequest.GetResponseAsync();
                if (webRequest.HaveResponse)
                {
                    //Cascade detection of ContentType from response
                    string responseContentType = webResponse.ContentType;
                    if (string.IsNullOrWhiteSpace(responseContentType))
                    {
                        responseContentType = webResponse.Headers["ContentType"];
                        if (string.IsNullOrWhiteSpace(responseContentType))
                            responseContentType = "application/rdf+xml"; //Fallback to RDF/XML
                    }

                    //OWL2/XML
                    if (responseContentType.Contains("application/owl+xml"))
                        return await FromStreamAsync(OWLEnums.OWLFormats.OWL2XML, webResponse.GetResponseStream());

                    //RDF/XML
                    if (responseContentType.Contains("application/rdf+xml"))
                        return await FromRDFGraphAsync(await RDFGraph.FromStreamAsync(RDFModelEnums.RDFFormats.RdfXml, webResponse.GetResponseStream(), true));

                    //TURTLE
                    if (responseContentType.Contains("text/turtle")
                        || responseContentType.Contains("application/turtle")
                        || responseContentType.Contains("application/x-turtle"))
                        return await FromRDFGraphAsync(await RDFGraph.FromStreamAsync(RDFModelEnums.RDFFormats.Turtle, webResponse.GetResponseStream(), true));

                    //N-TRIPLES
                    if (responseContentType.Contains("application/n-triples"))
                        return await FromRDFGraphAsync(await RDFGraph.FromStreamAsync(RDFModelEnums.RDFFormats.NTriples, webResponse.GetResponseStream(), true));
                }
            }
            catch (Exception ex)
            {
                throw new OWLException($"Cannot read OWL2 ontology from Uri {uri} because: " + ex.Message);
            }

            return await Task.FromResult<OWLOntology>(null);
        }
        #endregion
    }
}