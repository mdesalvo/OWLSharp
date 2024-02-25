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
using System.Data;
using System.Linq;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntologyDataLoader is responsible for loading ontology data from remote sources or alternative representations
    /// </summary>
    internal static class OWLOntologyDataLoader
    {
        #region Methods
        /// <summary>
        /// Gets an ontology data representation of the given graph
        /// </summary>
        internal static void LoadData(this OWLOntology ontology, RDFGraph graph)
        {
            #region Guards
            if (graph == null)
                throw new OWLException("Cannot get ontology data from RDFGraph because given \"graph\" parameter is null");
            #endregion

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' is going to be parsed as Data...", graph.Context));

            #region Declarations
            HashSet<long> annotationProperties = graph.GetAnnotationPropertyHashes();
            annotationProperties.UnionWith(OWLUtilities.StandardResourceAnnotations);

            //Individuals (rdf:type owl:[Named]Individual)
            foreach (RDFResource owlNamedIndividual in GetNamedIndividualDeclarations(graph))
                ontology.Data.DeclareIndividual(owlNamedIndividual);
            foreach (RDFResource owlIndividual in GetIndividualDeclarations(graph))
                ontology.Data.DeclareIndividual(owlIndividual);

            //Individuals (rdf:type owl:Class)
            foreach (RDFResource owlClass in ontology.Model.ClassModel)
                foreach (RDFTriple type in graph[null, RDFVocabulary.RDF.TYPE, owlClass, null])
                {
                    ontology.Data.DeclareIndividual((RDFResource)type.Subject);
                    ontology.Data.DeclareIndividualType((RDFResource)type.Subject, owlClass);
                }
            #endregion

            #region Taxonomies
            foreach (RDFResource owlIndividual in ontology.Data)
                foreach (RDFTriple individualAnnotation in graph[owlIndividual, null, null, null].Where(t => annotationProperties.Contains(t.Predicate.PatternMemberID)))
                {
                    if (individualAnnotation.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                        ontology.Data.AnnotateIndividual(owlIndividual, (RDFResource)individualAnnotation.Predicate, (RDFResource)individualAnnotation.Object);
                    else
                        ontology.Data.AnnotateIndividual(owlIndividual, (RDFResource)individualAnnotation.Predicate, (RDFLiteral)individualAnnotation.Object);
                }

            //owl:sameAs
            foreach (RDFTriple sameAsRelation in graph[null, RDFVocabulary.OWL.SAME_AS, null, null])
                ontology.Data.DeclareSameIndividuals((RDFResource)sameAsRelation.Subject, (RDFResource)sameAsRelation.Object);

            //owl:differentFrom
            foreach (RDFTriple differentFromRelation in graph[null, RDFVocabulary.OWL.DIFFERENT_FROM, null, null])
                ontology.Data.DeclareDifferentIndividuals((RDFResource)differentFromRelation.Subject, (RDFResource)differentFromRelation.Object);

            //owl:NegativePropertyAssertion [OWL2]
            foreach (RDFTriple negativeObjectAssertion in GetNegativeObjectAssertions(graph))
                ontology.Data.DeclareNegativeObjectAssertion((RDFResource)negativeObjectAssertion.Subject, (RDFResource)negativeObjectAssertion.Predicate, (RDFResource)negativeObjectAssertion.Object);
            foreach (RDFTriple negativeDatatypeAssertion in GetNegativeDatatypeAssertions(graph))
                ontology.Data.DeclareNegativeDatatypeAssertion((RDFResource)negativeDatatypeAssertion.Subject, (RDFResource)negativeDatatypeAssertion.Predicate, (RDFLiteral)negativeDatatypeAssertion.Object);

            //owl:[Object|Datatype]PropertyAssertion
            foreach (RDFTriple objectAssertion in GetObjectAssertions(ontology, graph))
                ontology.Data.DeclareObjectAssertion((RDFResource)objectAssertion.Subject, (RDFResource)objectAssertion.Predicate, (RDFResource)objectAssertion.Object);
            foreach (RDFTriple datatypeAssertion in GetDatatypeAssertions(ontology, graph))
                ontology.Data.DeclareDatatypeAssertion((RDFResource)datatypeAssertion.Subject, (RDFResource)datatypeAssertion.Predicate, (RDFLiteral)datatypeAssertion.Object);

            //owl:AllDifferent [OWL2]
            foreach (RDFResource allDifferent in GetAllDifferentDeclarations(graph))
                foreach (RDFTriple allDifferentMembers in graph[allDifferent, RDFVocabulary.OWL.DISTINCT_MEMBERS, null, null])
                {
                    List<RDFResource> differentIndividuals = new List<RDFResource>();
                    RDFCollection differentIndividualsCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)allDifferentMembers.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    foreach (RDFPatternMember differentIndividual in differentIndividualsCollection)
                        differentIndividuals.Add((RDFResource)differentIndividual);
                    ontology.Data.DeclareAllDifferentIndividuals(allDifferent, differentIndividuals);
                }
            #endregion

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' has been parsed as Data", graph.Context));
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Gets the owl:NamedIndividual declarations
        /// </summary>
        internal static HashSet<RDFResource> GetNamedIndividualDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:Individual declarations
        /// </summary>
        internal static HashSet<RDFResource> GetIndividualDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INDIVIDUAL, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:AllDifferent declarations [OWL2]
        /// </summary>
        internal static HashSet<RDFResource> GetAllDifferentDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the negative object assertions of the given graph (same algorythm of DataLens)
        /// </summary>
        internal static RDFGraph GetNegativeObjectAssertions(RDFGraph graph)
        {
            Dictionary<string, long> hashContext = new Dictionary<string, long>();

            //Perform a SPARQL query to fetch all negative object assertions
            RDFSelectQuery negativeObjectAssertionQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION))
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFVariable("?NASN_SUBJECT")))
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFVariable("?NASN_PROPERTY")))
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFVariable("?NASN_TARGET")))
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?NASN_SUBJECT")))
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?NASN_PROPERTY")))
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?NASN_TARGET"))))
                .AddProjectionVariable(new RDFVariable("?NASN_SUBJECT"))
                .AddProjectionVariable(new RDFVariable("?NASN_PROPERTY"))
                .AddProjectionVariable(new RDFVariable("?NASN_TARGET"));
            RDFSelectQueryResult negativeObjectAssertionQueryResult = negativeObjectAssertionQuery.ApplyToGraph(graph);

            //Merge them into the results graph
            RDFGraph negativeObjectAssertionsGraph = new RDFGraph();
            foreach (DataRow negativeObjectAssertion in negativeObjectAssertionQueryResult.SelectResults.Rows)
                negativeObjectAssertionsGraph.AddTriple(new RDFTriple(new RDFResource(negativeObjectAssertion["?NASN_SUBJECT"].ToString(), hashContext), new RDFResource(negativeObjectAssertion["?NASN_PROPERTY"].ToString(), hashContext), new RDFResource(negativeObjectAssertion["?NASN_TARGET"].ToString(), hashContext)));

            return negativeObjectAssertionsGraph;
        }

        /// <summary>
        /// Gets the negative data assertions of the given graph (same algorythm of DataLens)
        /// </summary>
        internal static RDFGraph GetNegativeDatatypeAssertions(RDFGraph graph)
        {
            Dictionary<string, long> hashContext = new Dictionary<string, long>();

            //Perform a SPARQL query to fetch all negative datatype assertions
            RDFSelectQuery negativeDatatypeAssertionQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION))
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFVariable("?NASN_SUBJECT")))
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFVariable("?NASN_PROPERTY")))
                    .AddPattern(new RDFPattern(new RDFVariable("?NASN_REPRESENTATIVE"), RDFVocabulary.OWL.TARGET_VALUE, new RDFVariable("?NASN_TARGET")))
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?NASN_SUBJECT")))
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?NASN_PROPERTY")))
                    .AddFilter(new RDFIsLiteralFilter(new RDFVariable("?NASN_TARGET"))))
                .AddProjectionVariable(new RDFVariable("?NASN_SUBJECT"))
                .AddProjectionVariable(new RDFVariable("?NASN_PROPERTY"))
                .AddProjectionVariable(new RDFVariable("?NASN_TARGET"));
            RDFSelectQueryResult negativeDatatypeAssertionQueryResult = negativeDatatypeAssertionQuery.ApplyToGraph(graph);

            //Merge them into the results graph
            RDFGraph negativeDatatypeAssertionsGraph = new RDFGraph();
            foreach (DataRow negativeDatatypeAssertion in negativeDatatypeAssertionQueryResult.SelectResults.Rows)
                negativeDatatypeAssertionsGraph.AddTriple(new RDFTriple(new RDFResource(negativeDatatypeAssertion["?NASN_SUBJECT"].ToString(), hashContext), new RDFResource(negativeDatatypeAssertion["?NASN_PROPERTY"].ToString(), hashContext), (RDFLiteral)RDFQueryUtilities.ParseRDFPatternMember(negativeDatatypeAssertion["?NASN_TARGET"].ToString())));

            return negativeDatatypeAssertionsGraph;
        }

        /// <summary>
        /// Gets the object relations of the given owl:Individual
        /// </summary>
        internal static RDFGraph GetObjectAssertions(OWLOntology ontology, RDFGraph graph)
        {
            #region Filters
            bool IsObjectAssertion(RDFTriple triple)
                => triple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO
                     && !triple.Subject.Equals(ontology)
                       && !ontology.Model.ClassModel.CheckHasClass((RDFResource)triple.Subject)
                         && !ontology.Model.PropertyModel.CheckHasProperty((RDFResource)triple.Subject);
            #endregion

            RDFGraph objectAssertions = new RDFGraph();

            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
            { 
                foreach (RDFTriple objectAssertion in graph[null, objectPropertiesEnumerator.Current, null, null].Where(asn => IsObjectAssertion(asn)))
                    objectAssertions.AddTriple(objectAssertion);
            }

            return objectAssertions;
        }

        /// <summary>
        /// Gets the data relations of the given owl:Individual
        /// </summary>
        internal static RDFGraph GetDatatypeAssertions(OWLOntology ontology, RDFGraph graph)
        {
            #region Filters
            bool IsDatatypeAssertion(RDFTriple triple)
                => triple.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL
                     && !triple.Subject.Equals(ontology)
                       && !ontology.Model.ClassModel.CheckHasClass((RDFResource)triple.Subject)
                         && !ontology.Model.PropertyModel.CheckHasProperty((RDFResource)triple.Subject);
            #endregion

            RDFGraph datatypeAssertions = new RDFGraph();

            IEnumerator<RDFResource> datatypePropertiesEnumerator = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypePropertiesEnumerator.MoveNext())
            {
                foreach (RDFTriple datatypeAssertion in graph[null, datatypePropertiesEnumerator.Current, null, null].Where(asn => IsDatatypeAssertion(asn)))
                    datatypeAssertions.AddTriple(datatypeAssertion);
            }   

            return datatypeAssertions;
        }
        #endregion
    }
}