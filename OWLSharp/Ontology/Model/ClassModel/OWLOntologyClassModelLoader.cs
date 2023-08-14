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
using System.Globalization;
using System.Linq;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntologyClassModelLoader is responsible for loading ontology class models from remote sources or alternative representations
    /// </summary>
    internal static class OWLOntologyClassModelLoader
    {
        #region Methods
        /// <summary>
        /// Gets an ontology class model representation of the given graph
        /// </summary>
        internal static void LoadClassModel(this OWLOntology ontology, RDFGraph graph)
        {
            #region Guards
            if (graph == null)
                throw new OWLException("Cannot get ontology class model from RDFGraph because given \"graph\" parameter is null");
            #endregion

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' is going to be parsed as ClassModel...", graph.Context));

            #region Declarations
            HashSet<long> annotationProperties = graph.GetAnnotationPropertyHashes();
            annotationProperties.UnionWith(OWLUtilities.StandardResourceAnnotations);

            //Class
            List<RDFResource> classes = GetClassDeclarations(graph)
                                         .Union(GetDeprecatedClassDeclarations(graph))
                                          .Union(GetRDFSClassDeclarations(graph))
                                           .ToList();
            foreach (RDFResource owlClass in classes)
                ontology.Model.ClassModel.DeclareClass(owlClass, GetClassBehavior(owlClass, graph));

            //Restriction
            foreach (RDFResource owlRestriction in GetRestrictionDeclarations(graph))
                ontology.LoadRestriction(owlRestriction, graph);

            //Enumerate
            foreach (RDFResource owlEnumerate in GetEnumerateDeclarations(graph))
                ontology.LoadEnumerateClass(owlEnumerate, graph);

            //Composite
            List<RDFResource> composites = GetCompositeUnionDeclarations(graph)
                                            .Union(GetCompositeIntersectionDeclarations(graph))
                                             .Union(GetCompositeComplementDeclarations(graph))
                                              .ToList();
            foreach (RDFResource owlComposite in composites)
                ontology.LoadCompositeClass(owlComposite, graph);

            //DisjointUnion [OWL2]
            foreach (RDFResource owlDisjointUnion in GetDisjointUnionDeclarations(graph))
                ontology.LoadDisjointUnionClass(owlDisjointUnion, graph);
            #endregion

            #region Taxonomies
            foreach (RDFResource owlClass in ontology.Model.ClassModel)
                foreach (RDFTriple classAnnotation in graph[owlClass, null, null, null].Where(t => annotationProperties.Contains(t.Predicate.PatternMemberID)))
                {
                    if (classAnnotation.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                        ontology.Model.ClassModel.AnnotateClass(owlClass, (RDFResource)classAnnotation.Predicate, (RDFResource)classAnnotation.Object);
                    else
                        ontology.Model.ClassModel.AnnotateClass(owlClass, (RDFResource)classAnnotation.Predicate, (RDFLiteral)classAnnotation.Object);
                }

            //rdfs:subClassOf
            foreach (RDFTriple subClassRelation in graph[null, RDFVocabulary.RDFS.SUB_CLASS_OF, null, null])
                ontology.Model.ClassModel.DeclareSubClasses((RDFResource)subClassRelation.Subject, (RDFResource)subClassRelation.Object);

            //owl:equivalentClass
            foreach (RDFTriple equivalentClassRelation in graph[null, RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null])
                ontology.Model.ClassModel.DeclareEquivalentClasses((RDFResource)equivalentClassRelation.Subject, (RDFResource)equivalentClassRelation.Object);

            //owl:disjointWith
            foreach (RDFTriple disjointClassRelation in graph[null, RDFVocabulary.OWL.DISJOINT_WITH, null, null])
                ontology.Model.ClassModel.DeclareDisjointClasses((RDFResource)disjointClassRelation.Subject, (RDFResource)disjointClassRelation.Object);

            //owl:hasKey [OWL2]
            foreach (RDFTriple hasKeyRelation in graph[null, RDFVocabulary.OWL.HAS_KEY, null, null])
            {
                List<RDFResource> keyProperties = new List<RDFResource>();
                RDFCollection keyPropertiesCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)hasKeyRelation.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                foreach (RDFPatternMember keyProperty in keyPropertiesCollection)
                    keyProperties.Add((RDFResource)keyProperty);
                ontology.Model.ClassModel.DeclareHasKey((RDFResource)hasKeyRelation.Subject, keyProperties);
            }

            //owl:disjointUnionOf [OWL2]
            foreach (RDFTriple disjointUnion in graph[null, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null]) //OWL2
            {
                List<RDFResource> disjointClasses = new List<RDFResource>();
                RDFCollection disjointClassesCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)disjointUnion.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                foreach (RDFPatternMember disjointClass in disjointClassesCollection)
                    disjointClasses.Add((RDFResource)disjointClass);
                ontology.Model.ClassModel.DeclareDisjointUnionClass((RDFResource)disjointUnion.Subject, disjointClasses);
            }

            //owl:AllDisjointClasses [OWL2]
            foreach (RDFResource allDisjointClasses in GetAllDisjointClassesDeclarations(graph))
                foreach (RDFTriple allDisjointClassesMembers in graph[allDisjointClasses, RDFVocabulary.OWL.MEMBERS, null, null])
                {
                    List<RDFResource> disjointClasses = new List<RDFResource>();
                    RDFCollection disjointClassesCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)allDisjointClassesMembers.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    foreach (RDFPatternMember disjointClass in disjointClassesCollection)
                        disjointClasses.Add((RDFResource)disjointClass);
                    ontology.Model.ClassModel.DeclareAllDisjointClasses(allDisjointClasses, disjointClasses);
                }
            #endregion

            OWLEvents.RaiseInfo(string.Format("Graph '{0}' has been parsed as ClassModel", graph.Context));
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Gets the owl:Class declarations
        /// </summary>
        internal static HashSet<RDFResource> GetClassDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:DeprecatedClass declarations
        /// </summary>
        internal static HashSet<RDFResource> GetDeprecatedClassDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_CLASS, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the rdfs:Class declarations
        /// </summary>
        internal static HashSet<RDFResource> GetRDFSClassDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:Restriction declarations
        /// </summary>
        internal static HashSet<RDFResource> GetRestrictionDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:onProperty declaration of the given owl:Restriction
        /// </summary>
        internal static RDFResource GetRestrictionProperty(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.ON_PROPERTY, null, null]
                  .FirstOrDefault()?.Object as RDFResource;

        /// <summary>
        /// Gets the owl:allValuesFrom declaration of the given owl:Restriction
        /// </summary>
        internal static RDFResource GetRestrictionAllValuesFromClass(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.ALL_VALUES_FROM, null, null]
                  .FirstOrDefault()?.Object as RDFResource;

        /// <summary>
        /// Gets the owl:someValuesFrom declaration of the given owl:Restriction
        /// </summary>
        internal static RDFResource GetRestrictionSomeValuesFromClass(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.SOME_VALUES_FROM, null, null]
                  .FirstOrDefault()?.Object as RDFResource;

        /// <summary>
        /// Gets the owl:hasValue declaration of the given owl:Restriction
        /// </summary>
        internal static RDFPatternMember GetRestrictionHasValue(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.HAS_VALUE, null, null]
                  .FirstOrDefault()?.Object;

        /// <summary>
        /// Gets the owl:hasSelf declaration of the given owl:Restriction [OWL2]
        /// </summary>
        internal static RDFTypedLiteral GetRestrictionHasSelf(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.HAS_SELF, null, null]
                  .FirstOrDefault()?.Object as RDFTypedLiteral;

        /// <summary>
        /// Gets the owl:cardinality declaration of the given owl:Restriction
        /// </summary>
        internal static RDFTypedLiteral GetRestrictionCardinality(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.CARDINALITY, null, null]
                  .FirstOrDefault()?.Object as RDFTypedLiteral;

        /// <summary>
        /// Gets the owl:minCardinality declaration of the given owl:Restriction
        /// </summary>
        internal static RDFTypedLiteral GetRestrictionMinCardinality(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.MIN_CARDINALITY, null, null]
                  .FirstOrDefault()?.Object as RDFTypedLiteral;

        /// <summary>
        /// Gets the owl:maxCardinality declaration of the given owl:Restriction
        /// </summary>
        internal static RDFTypedLiteral GetRestrictionMaxCardinality(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.MAX_CARDINALITY, null, null]
                  .FirstOrDefault()?.Object as RDFTypedLiteral;

        /// <summary>
        /// Gets the owl:onClass declaration of the given owl:Restriction [OWL2]
        /// </summary>
        internal static RDFResource GetRestrictionClass(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.ON_CLASS, null, null]
                  .FirstOrDefault()?.Object as RDFResource;

        /// <summary>
        /// Gets the owl:qualifiedCardinality declaration of the given owl:Restriction [OWL2]
        /// </summary>
        internal static RDFTypedLiteral GetRestrictionQualifiedCardinality(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.QUALIFIED_CARDINALITY, null, null]
                  .FirstOrDefault()?.Object as RDFTypedLiteral;

        /// <summary>
        /// Gets the owl:minQualifiedCardinality declaration of the given owl:Restriction [OWL2]
        /// </summary>
        internal static RDFTypedLiteral GetRestrictionMinQualifiedCardinality(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, null, null]
                  .FirstOrDefault()?.Object as RDFTypedLiteral;

        /// <summary>
        /// Gets the owl:maxQualifiedCardinality declaration of the given owl:Restriction [OWL2]
        /// </summary>
        internal static RDFTypedLiteral GetRestrictionMaxQualifiedCardinality(RDFGraph graph, RDFResource owlRestriction)
            => graph[owlRestriction, RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, null, null]
                  .FirstOrDefault()?.Object as RDFTypedLiteral;

        /// <summary>
        /// Gets the owl:oneOf declarations
        /// </summary>
        internal static HashSet<RDFResource> GetEnumerateDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.OWL.ONE_OF, null, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:unionOf declarations
        /// </summary>
        internal static HashSet<RDFResource> GetCompositeUnionDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.OWL.UNION_OF, null, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:intersectionOf declarations
        /// </summary>
        internal static HashSet<RDFResource> GetCompositeIntersectionDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.OWL.INTERSECTION_OF, null, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:complementOf declarations
        /// </summary>
        internal static HashSet<RDFResource> GetCompositeComplementDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.OWL.COMPLEMENT_OF, null, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:disjointUnionOf declarations [OWL2]
        /// </summary>
        internal static HashSet<RDFResource> GetDisjointUnionDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the owl:AllDisjointClasses declarations [OWL2]
        /// </summary>
        internal static HashSet<RDFResource> GetAllDisjointClassesDeclarations(RDFGraph graph)
            => new HashSet<RDFResource>(graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES, null]
                                           .Select(t => t.Subject)
                                           .OfType<RDFResource>());

        /// <summary>
        /// Gets the behavior of the given class
        /// </summary>
        internal static OWLOntologyClassBehavior GetClassBehavior(RDFResource owlClass, RDFGraph graph)
            => new OWLOntologyClassBehavior()
            {
                Deprecated = graph.ContainsTriple(new RDFTriple(owlClass, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_CLASS))
            };

        /// <summary>
        /// Loads the definition of the given restriction class
        /// </summary>
        internal static void LoadRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFGraph graph)
        {
            //Get mandatory owl:onProperty information
            RDFResource onProperty = GetRestrictionProperty(graph, owlRestriction);
            if (onProperty == null)
                throw new OWLException($"Cannot load Restriction '{owlRestriction}' from graph because it does not have required owl:onProperty information");

            //Try load the given restriction as instance of owl:[all|some]ValuesFrom
            if (TryLoadAllValuesFromRestriction(ontology, owlRestriction, onProperty, graph)
                 || TryLoadSomeValuesFromRestriction(ontology, owlRestriction, onProperty, graph))
                return;

            //Try load the given restriction as instance of owl:hasValue
            if (TryLoadHasValueRestriction(ontology, owlRestriction, onProperty, graph))
                return;

            //Try load the given restriction as instance of owl:hasSelf [OWL2]
            if (TryLoadHasSelfRestriction(ontology, owlRestriction, onProperty, graph))
                return;

            //Try load the given restriction as instance of owl:cardinality
            if (TryLoadCardinalityRestriction(ontology, owlRestriction, onProperty, graph))
                return;

            //Try load the given restriction as instance of owl:[min|max]Cardinality
            if (TryLoadMinMaxCardinalityRestriction(ontology, owlRestriction, onProperty, graph))
                return;

            //Try load the given restriction as instance of owl:qualifiedCardinality [OWL2]
            if (TryLoadQualifiedCardinalityRestriction(ontology, owlRestriction, onProperty, graph))
                return;

            //Try load the given restriction as instance of owl:[min|max]QualifiedCardinality [OWL2]
            TryLoadMinMaxQualifiedCardinalityRestriction(ontology, owlRestriction, onProperty, graph);
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:allValuesFrom restriction
        /// </summary>
        internal static bool TryLoadAllValuesFromRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            RDFResource allValuesFromClass = GetRestrictionAllValuesFromClass(graph, owlRestriction);
            if (allValuesFromClass != null)
            {
                ontology.Model.ClassModel.DeclareAllValuesFromRestriction(owlRestriction, onProperty, allValuesFromClass);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:someValuesFrom restriction
        /// </summary>
        internal static bool TryLoadSomeValuesFromRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            RDFResource someValuesFromClass = GetRestrictionSomeValuesFromClass(graph, owlRestriction);
            if (someValuesFromClass != null)
            {
                ontology.Model.ClassModel.DeclareSomeValuesFromRestriction(owlRestriction, onProperty, someValuesFromClass);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:hasValue restriction
        /// </summary>
        internal static bool TryLoadHasValueRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            RDFPatternMember hasValue = GetRestrictionHasValue(graph, owlRestriction);
            if (hasValue != null)
            {
                if (hasValue is RDFResource)
                    ontology.Model.ClassModel.DeclareHasValueRestriction(owlRestriction, onProperty, (RDFResource)hasValue);
                else
                    ontology.Model.ClassModel.DeclareHasValueRestriction(owlRestriction, onProperty, (RDFLiteral)hasValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:hasSelf restriction [OWL2]
        /// </summary>
        internal static bool TryLoadHasSelfRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            RDFTypedLiteral hasSelf = GetRestrictionHasSelf(graph, owlRestriction);
            if (hasSelf != null)
            {
                if (hasSelf.Equals(RDFTypedLiteral.True))
                {
                    ontology.Model.ClassModel.DeclareHasSelfRestriction(owlRestriction, onProperty, true);
                    return true;
                }
                else if (hasSelf.Equals(RDFTypedLiteral.False))
                {
                    ontology.Model.ClassModel.DeclareHasSelfRestriction(owlRestriction, onProperty, false);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:cardinality restriction
        /// </summary>
        internal static bool TryLoadCardinalityRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            RDFTypedLiteral cardinality = GetRestrictionCardinality(graph, owlRestriction);
            if (cardinality != null && cardinality.HasDecimalDatatype())
            {
                if (uint.TryParse(cardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint cardinalityValue))
                {
                    ontology.Model.ClassModel.DeclareCardinalityRestriction(owlRestriction, onProperty, cardinalityValue);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:[min|max]Cardinality restriction
        /// </summary>
        internal static bool TryLoadMinMaxCardinalityRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            //Try detect owl:minCardinality
            uint minCardinalityValue = 0;
            bool hasMinCardinality = false;            
            RDFTypedLiteral minCardinality = GetRestrictionMinCardinality(graph, owlRestriction);
            if (minCardinality != null && minCardinality.HasDecimalDatatype())
                hasMinCardinality = uint.TryParse(minCardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out minCardinalityValue);

            //Try detect owl:maxCardinality
            uint maxCardinalityValue = 0;
            bool hasMaxCardinality = false;
            RDFTypedLiteral maxCardinality = GetRestrictionMaxCardinality(graph, owlRestriction);
            if (maxCardinality != null && maxCardinality.HasDecimalDatatype())
                hasMaxCardinality = uint.TryParse(maxCardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxCardinalityValue);

            if (hasMinCardinality && !hasMaxCardinality)
            {
                ontology.Model.ClassModel.DeclareMinCardinalityRestriction(owlRestriction, onProperty, minCardinalityValue);
                return true;
            }
            else if (!hasMinCardinality && hasMaxCardinality)
            {
                ontology.Model.ClassModel.DeclareMaxCardinalityRestriction(owlRestriction, onProperty, maxCardinalityValue);
                return true;
            }
            else if (hasMinCardinality && hasMaxCardinality)
            {
                ontology.Model.ClassModel.DeclareMinMaxCardinalityRestriction(owlRestriction, onProperty, minCardinalityValue, maxCardinalityValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:qualifiedCardinality restriction [OWL2]
        /// </summary>
        internal static bool TryLoadQualifiedCardinalityRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            //Get mandatory owl:onClass information
            RDFResource onClass = GetRestrictionClass(graph, owlRestriction);
            if (onClass == null)
                throw new OWLException($"Cannot load qualified Restriction '{owlRestriction}' from graph because it does not have required owl:onClass information");

            RDFTypedLiteral qualifiedCardinality = GetRestrictionQualifiedCardinality(graph, owlRestriction);
            if (qualifiedCardinality != null && qualifiedCardinality.HasDecimalDatatype())
            {
                if (uint.TryParse(qualifiedCardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint qualifiedCardinalityValue))
                {
                    ontology.Model.ClassModel.DeclareQualifiedCardinalityRestriction(owlRestriction, onProperty, qualifiedCardinalityValue, onClass);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to load the given owl:Restriction as instance of owl:[min|max]QualifiedCardinality restriction [OWL2]
        /// </summary>
        internal static bool TryLoadMinMaxQualifiedCardinalityRestriction(this OWLOntology ontology, RDFResource owlRestriction, RDFResource onProperty, RDFGraph graph)
        {
            //Get mandatory owl:onClass information
            RDFResource onClass = GetRestrictionClass(graph, owlRestriction);
            if (onClass == null)
                throw new OWLException($"Cannot load qualified Restriction '{owlRestriction}' from graph because it does not have required owl:onClass information");

            //Try detect owl:minQualifiedCardinality
            uint minQualifiedCardinalityValue = 0;
            bool hasMinQualifiedCardinality = false;
            RDFTypedLiteral minQualifiedCardinality = GetRestrictionMinQualifiedCardinality(graph, owlRestriction);
            if (minQualifiedCardinality != null && minQualifiedCardinality.HasDecimalDatatype())
                hasMinQualifiedCardinality = uint.TryParse(minQualifiedCardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out minQualifiedCardinalityValue);

            //Try detect owl:maxQualifiedCardinality
            uint maxQualifiedCardinalityValue = 0;
            bool hasMaxQualifiedCardinality = false;
            RDFTypedLiteral maxQualifiedCardinality = GetRestrictionMaxQualifiedCardinality(graph, owlRestriction);
            if (maxQualifiedCardinality != null && maxQualifiedCardinality.HasDecimalDatatype())
                hasMaxQualifiedCardinality = uint.TryParse(maxQualifiedCardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxQualifiedCardinalityValue);

            if (hasMinQualifiedCardinality && !hasMaxQualifiedCardinality)
            {
                ontology.Model.ClassModel.DeclareMinQualifiedCardinalityRestriction(owlRestriction, onProperty, minQualifiedCardinalityValue, onClass);
                return true;
            }
            else if (!hasMinQualifiedCardinality && hasMaxQualifiedCardinality)
            {
                ontology.Model.ClassModel.DeclareMaxQualifiedCardinalityRestriction(owlRestriction, onProperty, maxQualifiedCardinalityValue, onClass);
                return true;
            }
            else if (hasMinQualifiedCardinality && hasMaxQualifiedCardinality)
            {
                ontology.Model.ClassModel.DeclareMinMaxQualifiedCardinalityRestriction(owlRestriction, onProperty, minQualifiedCardinalityValue, maxQualifiedCardinalityValue, onClass);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads the definition of the given enumerate class
        /// </summary>
        internal static void LoadEnumerateClass(this OWLOntology ontology, RDFResource owlEnumerate, RDFGraph graph)
        {
            RDFResource oneOfRepresentative = graph[owlEnumerate, RDFVocabulary.OWL.ONE_OF, null, null].FirstOrDefault()?.Object as RDFResource;
            if (oneOfRepresentative != null)
            {
                List<RDFResource> oneOfMembers = new List<RDFResource>();
                RDFCollection oneOfMembersCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, oneOfRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                foreach (RDFPatternMember oneOfMember in oneOfMembersCollection)
                    oneOfMembers.Add((RDFResource)oneOfMember);
                ontology.Model.ClassModel.DeclareEnumerateClass(owlEnumerate, oneOfMembers);
            }
        }

        /// <summary>
        /// Loads the definition of the given composite class
        /// </summary>
        internal static void LoadCompositeClass(this OWLOntology ontology, RDFResource owlComposite, RDFGraph graph)
        {
            #region owl:unionOf
            RDFResource unionRepresentative = graph[owlComposite, RDFVocabulary.OWL.UNION_OF, null, null].FirstOrDefault()?.Object as RDFResource;
            if (unionRepresentative != null)
            {
                List<RDFResource> unionMembers = new List<RDFResource>();
                RDFCollection unionMembersCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, unionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                foreach (RDFPatternMember unionMember in unionMembersCollection)
                    unionMembers.Add((RDFResource)unionMember);
                ontology.Model.ClassModel.DeclareUnionClass(owlComposite, unionMembers);
                return;
            }
            #endregion

            #region owl:intersectionOf
            RDFResource intersectionRepresentative = graph[owlComposite, RDFVocabulary.OWL.INTERSECTION_OF, null, null].FirstOrDefault()?.Object as RDFResource;
            if (intersectionRepresentative != null)
            {
                List<RDFResource> intersectionMembers = new List<RDFResource>();
                RDFCollection intersectionMembersCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, intersectionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                foreach (RDFPatternMember intersectionMember in intersectionMembersCollection)
                    intersectionMembers.Add((RDFResource)intersectionMember);
                ontology.Model.ClassModel.DeclareIntersectionClass(owlComposite, intersectionMembers);
                return;
            }
            #endregion

            #region owl:complementOf
            RDFResource complementClass = graph[owlComposite, RDFVocabulary.OWL.COMPLEMENT_OF, null, null].FirstOrDefault()?.Object as RDFResource;
            if (complementClass != null)
                ontology.Model.ClassModel.DeclareComplementClass(owlComposite, complementClass);
            #endregion
        }

        /// <summary>
        /// Loads the definition of the given disjoint union class [OWL2]
        /// </summary>
        internal static void LoadDisjointUnionClass(this OWLOntology ontology, RDFResource owlDisjointUnion, RDFGraph graph)
        {
            RDFResource disjointUnionRepresentative = graph[owlDisjointUnion, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null].FirstOrDefault()?.Object as RDFResource;
            if (disjointUnionRepresentative != null)
            {
                List<RDFResource> disjointUnionMembers = new List<RDFResource>();
                RDFCollection disjointUnionMembersCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, disjointUnionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                foreach (RDFPatternMember disjointUnionMember in disjointUnionMembersCollection)
                    disjointUnionMembers.Add((RDFResource)disjointUnionMember);
                ontology.Model.ClassModel.DeclareDisjointUnionClass(owlDisjointUnion, disjointUnionMembers);
            }
        }
        #endregion
    }
}