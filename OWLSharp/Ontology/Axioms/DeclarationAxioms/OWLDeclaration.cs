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