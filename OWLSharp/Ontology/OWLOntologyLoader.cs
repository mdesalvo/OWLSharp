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

using System.Collections.Generic;
using System.Linq;
using RDFSharp.Model;
using OWLSharp.Extensions.GEO;
using OWLSharp.Extensions.SKOS;
using OWLSharp.Extensions.TIME;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntologyLoader is responsible for loading ontologies from remote sources or alternative representations
    /// </summary>
    internal static class OWLOntologyLoader
    {
        #region Methods
        /// <summary>
        /// Gets an ontology representation of the given graph, eventually supporting extension ontologies (GEO, TIME, SKOS)
        /// </summary>
        internal static OWLOntology FromRDFGraph(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
        {
            #region Guards
            if (graph == null)
                throw new OWLException("Cannot get ontology from RDFGraph because given \"graph\" parameter is null");
            if (loaderOptions == null)
                loaderOptions = OWLOntologyLoaderOptions.DefaultOptions;
            #endregion

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' is going to be parsed as Ontology...", graph.Context));
            
            //Ontology creation
            LoadOntology(graph, out OWLOntology ontology);

            #region Extensions
            //GeoSPARQL
            if (loaderOptions.EnableGEOSupport 
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEOSPARQL.BASE_URI), null].TriplesCount > 0
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEOSPARQL.GEOF.BASE_URI), null].TriplesCount > 0
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEOSPARQL.SF.BASE_URI), null].TriplesCount > 0
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEO.BASE_URI), null].TriplesCount > 0)
                ontology.InitializeGEO();

            //OWL-TIME
            if (loaderOptions.EnableTIMESupport
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.TIME.BASE_URI), null].TriplesCount > 0
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.TIME.GREG.BASE_URI), null].TriplesCount > 0
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.TIME.THORS.BASE_URI), null].TriplesCount > 0)
                ontology.InitializeTIME();

            //SKOS
            if (loaderOptions.EnableSKOSSupport
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.SKOS.BASE_URI), null].TriplesCount > 0
                 || graph[ontology, RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.SKOS.SKOSXL.BASE_URI), null].TriplesCount > 0)
            {
                ontology.InitializeSKOS();

                #region PostProcessing
                //Extend skos:Collection individuals to A-BOX
                foreach (RDFTriple typeCollection in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null])
                    foreach (RDFTriple memberRelation in graph[(RDFResource)typeCollection.Subject, RDFVocabulary.SKOS.MEMBER, null, null])
                        ontology.Data.DeclareObjectAssertion((RDFResource)typeCollection.Subject, RDFVocabulary.SKOS.MEMBER, (RDFResource)memberRelation.Object);
                //Extend skos:OrderedCollection individuals to A-BOX
                foreach (RDFTriple typeOrderedCollection in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION, null])
                    foreach (RDFTriple memberListRelation in graph[(RDFResource)typeOrderedCollection.Subject, RDFVocabulary.SKOS.MEMBER_LIST, null, null])
                    {
                        RDFCollection skosOrderedCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)memberListRelation.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                        if (skosOrderedCollection.ItemsCount > 0)
                        {
                            ontology.Data.ABoxGraph.AddCollection(skosOrderedCollection);
                            ontology.Data.DeclareObjectAssertion((RDFResource)typeOrderedCollection.Subject, RDFVocabulary.SKOS.MEMBER_LIST, skosOrderedCollection.ReificationSubject);
                        }
                    }
                #endregion
            }
            #endregion

            //Ontology loading
            ontology.LoadModel(graph);
            ontology.LoadData(graph);

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' has been parsed as Ontology", graph.Context));

            return ontology;
        }

        /// <summary>
        /// Gets the hashes of owl:AnnotationProperty declarations
        /// </summary>
        internal static HashSet<long> GetAnnotationPropertyHashes(this RDFGraph graph)
            => new HashSet<long>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null]
                                    .Select(t => t.Subject.PatternMemberID));
        #endregion

        #region Utilities
        /// <summary>
        /// Parses the ontology URI and annotations
        /// </summary>
        private static void LoadOntology(RDFGraph graph, out OWLOntology ontology)
        {
            //Load ontology URI
            ontology = new OWLOntology(graph.Context.ToString());
            if (!graph.ContainsTriple(new RDFTriple(ontology, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY)))
            {
                RDFTriple ontologyDeclaration = graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].FirstOrDefault();
                if (ontologyDeclaration != null)
                    ontology = new OWLOntology(ontologyDeclaration.Subject.ToString());
            }

            //Load ontology annotations
            HashSet<long> annotationProperties = GetAnnotationPropertyHashes(graph);
            annotationProperties.UnionWith(OWLUtilities.StandardOntologyAnnotations);

            RDFGraph ontologyGraph = graph[ontology, null, null, null];
            foreach (RDFTriple ontologyAnnotation in ontologyGraph.Where(t => annotationProperties.Contains(t.Predicate.PatternMemberID)))
            {
                if (ontologyAnnotation.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                    ontology.Annotate((RDFResource)ontologyAnnotation.Predicate, (RDFResource)ontologyAnnotation.Object);
                else
                    ontology.Annotate((RDFResource)ontologyAnnotation.Predicate, (RDFLiteral)ontologyAnnotation.Object);
            }
        }
        #endregion
    }

    /// <summary>
    /// OWLOntologyLoaderOptions represents a configuration for customizing specific aspects of ontology loading
    /// </summary>
    public class OWLOntologyLoaderOptions
    {
        #region Properties
        internal static OWLOntologyLoaderOptions DefaultOptions => new OWLOntologyLoaderOptions();

        /// <summary>
        /// Tells the ontology loader to explicitly inject GeoSPARQL T-BOX during the loading process<br/>
        /// [Default: False]
        /// </summary>
        public bool EnableGEOSupport { get; set; } = false;

        /// <summary>
        /// Tells the ontology loader to explicitly inject OWL-TIME T-BOX and A-BOX during the loading process<br/>
        /// [Default: False]
        /// </summary>
        public bool EnableTIMESupport { get; set; } = false;

        /// <summary>
        /// Tells the ontology loader to explicitly inject SKOS T-BOX and A-BOX during the loading process<br/>
        /// [Default: False]
        /// </summary>
        public bool EnableSKOSSupport { get; set; } = false;
        #endregion
    }
}