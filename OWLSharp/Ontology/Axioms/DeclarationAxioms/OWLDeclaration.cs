/*
   Copyright 2014-2026 Marco De Salvo

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
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDeclaration axiom explicitly introduces an entity (class, property, individual, or datatype)
    /// into the ontology's signature by stating its type and IRI. For example, Declaration(Class(:Person))
    /// declares that :Person is a class in the ontology, ensuring the entity is recognized as part of the
    /// ontology's vocabulary even before any additional axioms define its characteristics, and helping
    /// to distinguish entities from unknown or mistyped references.
    /// </summary>
    [XmlRoot("Declaration")]
    public sealed class OWLDeclaration : OWLAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the ontology entity to be declared (can be a class, a property, a named individual or a datatype)
        /// </summary>
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=2)]
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=2)]
        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty", Order=2)]
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        public OWLExpression Entity { get; set; }
        #endregion

        #region Ctors
        internal OWLDeclaration() { }

        /// <summary>
        /// Builds an OWLDeclaration with the given class entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDeclaration(OWLClass classIRI) : this()
            => Entity = classIRI ?? throw new OWLException($"Cannot create OWLDeclaration because given '{nameof(classIRI)}' parameter is null");

        /// <summary>
        /// Builds an OWLDeclaration with the given datatype entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDeclaration(OWLDatatype datatypeIRI) : this()
            => Entity = datatypeIRI ?? throw new OWLException($"Cannot create OWLDeclaration because given '{nameof(datatypeIRI)}' parameter is null");

        /// <summary>
        /// Builds an OWLDeclaration with the given object property entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDeclaration(OWLObjectProperty objectPropertyIRI) : this()
            => Entity = objectPropertyIRI ?? throw new OWLException($"Cannot create OWLDeclaration because given '{nameof(objectPropertyIRI)}' parameter is null");

        /// <summary>
        /// Builds an OWLDeclaration with the given data property entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDeclaration(OWLDataProperty dataPropertyIRI) : this()
            => Entity = dataPropertyIRI ?? throw new OWLException($"Cannot create OWLDeclaration because given '{nameof(dataPropertyIRI)}' parameter is null");

        /// <summary>
        /// Builds an OWLDeclaration with the given annotation property entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDeclaration(OWLAnnotationProperty annotationPropertyIRI) : this()
            => Entity = annotationPropertyIRI ?? throw new OWLException($"Cannot create OWLDeclaration because given '{nameof(annotationPropertyIRI)}' parameter is null");

        /// <summary>
        /// Builds an OWLDeclaration with the given named individual entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDeclaration(OWLNamedIndividual namedIndividualIRI) : this()
            => Entity = namedIndividualIRI ?? throw new OWLException($"Cannot create OWLDeclaration because given '{nameof(namedIndividualIRI)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Gets the contribution of this OWLDeclaration to the OWL2/Manchester rendering of the ontology:
        /// the bare frame of the declared entity (Class:, ObjectProperty:, ...)
        /// </summary>
        internal override OWLManchesterFrameItem ToManchesterFrameItem(OWLManchesterContext manchesterContext)
        {
            OWLManchesterFrameKind? frameKind = null;
            switch (Entity)
            {
                case OWLClass _: frameKind = OWLManchesterFrameKind.Class; break;
                case OWLDatatype _: frameKind = OWLManchesterFrameKind.Datatype; break;
                case OWLObjectProperty _: frameKind = OWLManchesterFrameKind.ObjectProperty; break;
                case OWLDataProperty _: frameKind = OWLManchesterFrameKind.DataProperty; break;
                case OWLAnnotationProperty _: frameKind = OWLManchesterFrameKind.AnnotationProperty; break;
                case OWLNamedIndividual _: frameKind = OWLManchesterFrameKind.Individual; break;
            }
            return frameKind.HasValue
                ? new OWLManchesterFrameItem {
                    FrameKind = frameKind.Value,
                    EntityName = Entity.ToManchesterString(manchesterContext) }
                : null;
        }

        /// <summary>
        /// Gets the OWL2/Functional-Style representation of this Declaration axiom
        /// </summary>
        internal override string ToFunctionalString(OWLFunctionalContext functionalContext)
        {
            //The Entity production requires the declared entity to be wrapped in its own type keyword
            //(Class/Datatype/ObjectProperty/DataProperty/AnnotationProperty/NamedIndividual), unlike every
            //other axiom argument, which just delegates to the plain expression rendering
            switch (Entity)
            {
                case OWLClass cls: return $"Declaration( {functionalContext.RenderAxiomAnnotations(Annotations)}Class( {cls.ToFunctionalString(functionalContext)} ) )";
                case OWLDatatype datatype: return $"Declaration( {functionalContext.RenderAxiomAnnotations(Annotations)}Datatype( {datatype.ToFunctionalString(functionalContext)} ) )";
                case OWLObjectProperty objectProperty: return $"Declaration( {functionalContext.RenderAxiomAnnotations(Annotations)}ObjectProperty( {objectProperty.ToFunctionalString(functionalContext)} ) )";
                case OWLDataProperty dataProperty: return $"Declaration( {functionalContext.RenderAxiomAnnotations(Annotations)}DataProperty( {dataProperty.ToFunctionalString(functionalContext)} ) )";
                case OWLAnnotationProperty annotationProperty: return $"Declaration( {functionalContext.RenderAxiomAnnotations(Annotations)}AnnotationProperty( {annotationProperty.ToFunctionalString(functionalContext)} ) )";
                case OWLNamedIndividual namedIndividual: return $"Declaration( {functionalContext.RenderAxiomAnnotations(Annotations)}NamedIndividual( {namedIndividual.ToFunctionalString(functionalContext)} ) )";
                //Defensive fallback: should never happen given the XmlElement-constrained Entity types, but
                //guarantees this method never throws in case of unexpected runtime types
                default: return $"Declaration( {functionalContext.RenderAxiomAnnotations(Annotations)}{Entity.ToFunctionalString(functionalContext)} )";
            }
        }

        /// <summary>
        /// Exports this OWLDeclaration to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = Entity.ToRDFGraph();

            //Axiom Triple
            RDFTriple axiomTriple = graph.SelectTriples(p: RDFVocabulary.RDF.TYPE).FirstOrDefault();

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}