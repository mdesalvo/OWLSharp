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
using System.Threading.Tasks;
using RDFSharp.Model;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntology represents the formal description of an application domain in terms of T-BOX (Model) and A-BOX (Data)
    /// </summary>
    public class OWLOntology : RDFResource, IDisposable
    {
        #region Properties
        /// <summary>
        /// T-BOX of the application domain formalized by the ontology
        /// </summary>
        public OWLOntologyModel Model { get; internal set; }

        /// <summary>
        /// A-BOX available to the ontology from the application domain
        /// </summary>
        public OWLOntologyData Data { get; internal set; }

        /// <summary>
        /// Knowledge describing ontology itself (annotations)
        /// </summary>
        internal RDFGraph OBoxGraph { get; set; }

        /// <summary>
        /// Flag indicating that the ontology has already been disposed
        /// </summary>
        internal bool Disposed { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an empty ontology having the given URI
        /// </summary>
        public OWLOntology(string ontologyURI) : base(ontologyURI)
        {
            Model = new OWLOntologyModel();
            Data = new OWLOntologyData();
            OBoxGraph = new RDFGraph().SetContext(new Uri(ontologyURI));

            //Add knowledge to the O-BOX
            OBoxGraph.AddTriple(new RDFTriple(new RDFResource(ontologyURI), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
        }

        /// <summary>
        /// Builds an ontology having the given URI and the given T-BOX/A-BOX knowledge
        /// </summary>
        public OWLOntology(string ontologyURI, OWLOntologyModel model, OWLOntologyData data) : this(ontologyURI)
        {
            Model = model ?? new OWLOntologyModel();
            Data = data ?? new OWLOntologyData();
        }

        /// <summary>
        /// Destroys the ontology instance
        /// </summary>
        ~OWLOntology() => Dispose(false);
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

        /// <summary>
        /// Disposes the ontology
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Model.Dispose();
                Data.Dispose();
                OBoxGraph.Dispose();

                Model = null;
                Data = null;
                OBoxGraph = null;
            }

            Disposed = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Annotates the ontology with the given "annotationProperty -> annotationValue"
        /// </summary>
        public OWLOntology Annotate(RDFResource annotationProperty, RDFResource annotationValue)
        {
            #region Guards
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate ontology because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate ontology because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate ontology because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            OBoxGraph.AddTriple(new RDFTriple(this, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Annotates the ontology with the given "annotationProperty -> annotationValue"
        /// </summary>
        public OWLOntology Annotate(RDFResource annotationProperty, RDFLiteral annotationValue)
        {
            #region Guards
            if (annotationProperty == null)
                throw new OWLException("Cannot annotate ontology because given \"annotationProperty\" parameter is null");
            if (annotationProperty.IsBlank)
                throw new OWLException("Cannot annotate ontology because given \"annotationProperty\" parameter is a blank predicate");
            if (annotationValue == null)
                throw new OWLException("Cannot annotate ontology because given \"annotationValue\" parameter is null");
            #endregion

            //Add knowledge to the O-BOX
            OBoxGraph.AddTriple(new RDFTriple(this, annotationProperty, annotationValue));

            return this;
        }

        /// <summary>
        /// Gets a graph representation of the ontology
        /// </summary>
        public RDFGraph ToRDFGraph()
            => Model.ToRDFGraph()
                 .UnionWith(Data.ToRDFGraph())
                    .UnionWith(OBoxGraph);

        /// <summary>
        /// Asynchronously gets a graph representation of the ontology
        /// </summary>
        public Task<RDFGraph> ToRDFGraphAsync()
            => Task.Run(() => ToRDFGraph());

        /// <summary>
        /// Writes the ontology into a file in the given OWL format
        /// </summary>
        public void ToFile(OWLEnums.OWLFormats owlFormat, string filepath)
        {
            #region Guards
            if (string.IsNullOrEmpty(filepath))
                throw new OWLException("Cannot write OWL ontology to file because given \"filepath\" parameter is null or empty.");
            #endregion

            switch (owlFormat)
            {
                case OWLEnums.OWLFormats.OwlXml:
                    OWLXml.Serialize(this, filepath);
                    break;
            }
        }

        /// <summary>
        /// Asynchronously writes the ontology into a file in the given OWL format
        /// </summary>
        public Task ToFileAsync(OWLEnums.OWLFormats owlFormat, string filepath)
            => Task.Run(() => ToFile(owlFormat, filepath));

        /// <summary>
        /// Writes the ontology into a stream in the given OWL format (at the end the stream is closed)
        /// </summary>
        public void ToStream(OWLEnums.OWLFormats owlFormat, Stream outputStream)
        {
            #region Guards
            if (outputStream == null)
                throw new OWLException("Cannot write OWL ontology to stream because given \"outputStream\" parameter is null.");
            #endregion

            switch (owlFormat)
            {
                case OWLEnums.OWLFormats.OwlXml:
                    OWLXml.Serialize(this, outputStream);
                    break;
            }
        }

        /// <summary>
        /// Asynchronously writes the ontology into a stream in the given OWL format (at the end the stream is closed)
        /// </summary>
        public Task ToStreamAsync(OWLEnums.OWLFormats owlFormat, Stream outputStream)
            => Task.Run(() => ToStream(owlFormat, outputStream));

        /// <summary>
        /// Gets an ontology representation from the given graph
        /// </summary>
        public static OWLOntology FromRDFGraph(RDFGraph graph)
            => FromRDFGraph(graph, OWLOntologyLoaderOptions.DefaultOptions);

        /// <summary>
        /// Gets an ontology representation from the given graph (applying the given loader options)
        /// </summary>
        public static OWLOntology FromRDFGraph(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
            => OWLOntologyLoader.FromRDFGraph(graph, loaderOptions);

        /// <summary>
        /// Asynchronously gets an ontology representation from the given graph
        /// </summary>
        public static Task<OWLOntology> FromRDFGraphAsync(RDFGraph graph)
            => Task.Run(() => FromRDFGraph(graph));

        /// <summary>
        /// Asynchronously gets an ontology representation from the given graph (applying the given loader options)
        /// </summary>
        public static Task<OWLOntology> FromRDFGraphAsync(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
            => Task.Run(() => FromRDFGraph(graph, loaderOptions));
        #endregion
    }
}