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

using System.Collections.Generic;
using System.Linq;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntologyPropertyModelLoader is responsible for loading ontology property models from remote sources or alternative representations
    /// </summary>
    internal static class OWLOntologyPropertyModelLoader
    {
        #region Methods
        /// <summary>
        /// Gets an ontology property model representation of the given graph
        /// </summary>
        internal static void LoadPropertyModel(this OWLOntology ontology, RDFGraph graph)
        {
            #region Guards
            if (graph == null)
                throw new OWLException("Cannot get ontology property model from RDFGraph because given \"graph\" parameter is null");
            #endregion

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' is going to be parsed as PropertyModel...", graph.Context));

            #region Declarations
            HashSet<long> annotationProperties = graph.GetAnnotationPropertyHashes();
            annotationProperties.UnionWith(OWLUtilities.StandardResourceAnnotations);

            //AnnotationProperty
            foreach (RDFResource annotationProperty in GetAnnotationPropertyDeclarations(graph))
                ontology.Model.PropertyModel.DeclareAnnotationProperty(annotationProperty, GetAnnotationPropertyBehavior(annotationProperty, graph));

            //DatatypeProperty
            foreach (RDFResource datatypeProperty in GetDatatypePropertyDeclarations(graph))
                ontology.Model.PropertyModel.DeclareDatatypeProperty(datatypeProperty, GetDatatypePropertyBehavior(datatypeProperty, graph));

            //ObjectProperty (plus its derivate declarations)
            List<RDFResource> objectProperties = GetObjectPropertyDeclarations(graph)
                                                  .Union(GetDeprecatedPropertyDeclarations(graph))
                                                   .Union(GetSymmetricPropertyDeclarations(graph))
                                                    .Union(GetTransitivePropertyDeclarations(graph))
                                                     .Union(GetInverseFunctionalPropertyDeclarations(graph))
                                                      .Union(GetAsymmetricPropertyDeclarations(graph))
                                                       .Union(GetReflexivePropertyDeclarations(graph))
                                                        .Union(GetIrreflexivePropertyDeclarations(graph))
                                                         .ToList();
            foreach (RDFResource objectProperty in RDFQueryUtilities.RemoveDuplicates(objectProperties))
                ontology.Model.PropertyModel.DeclareObjectProperty(objectProperty, GetObjectPropertyBehavior(objectProperty, graph));
            #endregion

            #region Taxonomies
            foreach (RDFResource owlProperty in ontology.Model.PropertyModel)
                foreach (RDFTriple propertyAnnotation in graph[owlProperty, null, null, null].Where(t => annotationProperties.Contains(t.Predicate.PatternMemberID)))
                {
                    if (propertyAnnotation.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                        ontology.Model.PropertyModel.AnnotateProperty(owlProperty, (RDFResource)propertyAnnotation.Predicate, (RDFResource)propertyAnnotation.Object);
                    else
                        ontology.Model.PropertyModel.AnnotateProperty(owlProperty, (RDFResource)propertyAnnotation.Predicate, (RDFLiteral)propertyAnnotation.Object);
                }

            //rdfs:subPropertyOf
            foreach (RDFTriple subPropertyRelation in graph[null, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null])
                ontology.Model.PropertyModel.DeclareSubProperties((RDFResource)subPropertyRelation.Subject, (RDFResource)subPropertyRelation.Object);

            //owl:equivalentProperty
            foreach (RDFTriple equivalentPropertyRelation in graph[null, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null])
                ontology.Model.PropertyModel.DeclareEquivalentProperties((RDFResource)equivalentPropertyRelation.Subject, (RDFResource)equivalentPropertyRelation.Object);

            //owl:propertyDisjointWith [OWL2]
            foreach (RDFTriple disjointPropertyRelation in graph[null, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, null, null])
                ontology.Model.PropertyModel.DeclareDisjointProperties((RDFResource)disjointPropertyRelation.Subject, (RDFResource)disjointPropertyRelation.Object);

            //owl:inverseOf
            foreach (RDFTriple inversePropertyRelation in graph[null, RDFVocabulary.OWL.INVERSE_OF, null, null])
                ontology.Model.PropertyModel.DeclareInverseProperties((RDFResource)inversePropertyRelation.Subject, (RDFResource)inversePropertyRelation.Object);

            //owl:propertyChainAxiom [OWL2]
            foreach (RDFTriple propertyChainAxiom in graph[null, RDFVocabulary.OWL.PROPERTY_CHAIN_AXIOM, null, null])
            {
                List<RDFResource> chainAxiomProperties = new List<RDFResource>();
                RDFCollection chainAxiomPropertiesCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)propertyChainAxiom.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                foreach (RDFPatternMember chainAxiomProperty in chainAxiomPropertiesCollection)
                    chainAxiomProperties.Add((RDFResource)chainAxiomProperty);
                ontology.Model.PropertyModel.DeclarePropertyChainAxiom((RDFResource)propertyChainAxiom.Subject, chainAxiomProperties);
            }

            //owl:AllDisjointProperties [OWL2]
            foreach (RDFResource allDisjointProperties in GetAllDisjointPropertiesDeclarations(graph))
                foreach (RDFTriple allDisjointPropertiesMembers in graph[allDisjointProperties, RDFVocabulary.OWL.MEMBERS, null, null])
                {
                    List<RDFResource> disjointProperties = new List<RDFResource>();
                    RDFCollection disjointPropertiesCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)allDisjointPropertiesMembers.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    foreach (RDFPatternMember disjointProperty in disjointPropertiesCollection)
                        disjointProperties.Add((RDFResource)disjointProperty);
                    ontology.Model.PropertyModel.DeclareAllDisjointProperties(allDisjointProperties, disjointProperties);
                }
            #endregion

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' has been parsed as PropertyModel", graph.Context));
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Gets the annotation behavior of the given property
        /// </summary>
        internal static OWLOntologyAnnotationPropertyBehavior GetAnnotationPropertyBehavior(RDFResource property, RDFGraph graph)
            => new OWLOntologyAnnotationPropertyBehavior()
            {
                Deprecated = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_PROPERTY))
            };

        /// <summary>
        /// Gets the datatype behavior of the given property
        /// </summary>
        internal static OWLOntologyDatatypePropertyBehavior GetDatatypePropertyBehavior(RDFResource property, RDFGraph graph)
            => new OWLOntologyDatatypePropertyBehavior()
            {
                Deprecated = GetAnnotationPropertyBehavior(property, graph).Deprecated,
                Functional = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY)),
                Domain = graph[property, RDFVocabulary.RDFS.DOMAIN, null, null].FirstOrDefault()?.Object as RDFResource,
                Range = graph[property, RDFVocabulary.RDFS.RANGE, null, null].FirstOrDefault()?.Object as RDFResource
            };

        /// <summary>
        /// Gets the object behavior of the given property
        /// </summary>
        internal static OWLOntologyObjectPropertyBehavior GetObjectPropertyBehavior(RDFResource property, RDFGraph graph)
        {
            OWLOntologyDatatypePropertyBehavior datatypePropertyBehavior = GetDatatypePropertyBehavior(property, graph);
            return new OWLOntologyObjectPropertyBehavior()
            {
                Deprecated = datatypePropertyBehavior.Deprecated,
                Functional = datatypePropertyBehavior.Functional,
                Domain = datatypePropertyBehavior.Domain,
                Range = datatypePropertyBehavior.Range,
                Symmetric = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY)),
                Transitive = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY)),
                InverseFunctional = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY)),
                //OWL2
                Asymmetric = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY)),
                Reflexive = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY)),
                Irreflexive = graph.ContainsTriple(new RDFTriple(property, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY)),
            };
        }

        /// <summary>
        /// Gets the owl:ObjectProperty declarations
        /// </summary>
        internal static HashSet<RDFResource> GetObjectPropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:DeprecatedProperty declarations
        /// </summary>
        internal static HashSet<RDFResource> GetDeprecatedPropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:SymmetricProperty declarations
        /// </summary>
        internal static HashSet<RDFResource> GetSymmetricPropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:TransitiveProperty declarations
        /// </summary>
        internal static HashSet<RDFResource> GetTransitivePropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:InverseFunctionalProperty declarations
        /// </summary>
        internal static HashSet<RDFResource> GetInverseFunctionalPropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:AsymmetricProperty declarations [OWL2]
        /// </summary>
        internal static HashSet<RDFResource> GetAsymmetricPropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:ReflexiveProperty declarations [OWL2]
        /// </summary>
        internal static HashSet<RDFResource> GetReflexivePropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:IrreflexiveProperty declarations [OWL2]
        /// </summary>
        internal static HashSet<RDFResource> GetIrreflexivePropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:DatatypeProperty declarations
        /// </summary>
        internal static HashSet<RDFResource> GetDatatypePropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:AnnotationProperty declarations
        /// </summary>
        internal static HashSet<RDFResource> GetAnnotationPropertyDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:AllDisjointProperties declarations [OWL2]
        /// </summary>
        internal static HashSet<RDFResource> GetAllDisjointPropertiesDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());
        #endregion
    }
}