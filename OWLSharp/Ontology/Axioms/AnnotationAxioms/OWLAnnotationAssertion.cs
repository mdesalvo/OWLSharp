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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLAnnotationAssertion axiom attaches metadata to an ontology element (entity, axiom, or another annotation)
    /// by associating an annotation property with a value.
    /// For example, AnnotationAssertion(rdfs:label :Person "Person"@en) adds the English label "Person" to the class :Person,
    /// allowing enrichment of the ontology with human-readable documentation, provenance information, versioning details,
    /// or other descriptive metadata without affecting logical reasoning.
    /// </summary>
    [XmlRoot("AnnotationAssertion")]
    public sealed class OWLAnnotationAssertion : OWLAnnotationAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the property used for this annotation axiom (e.g: http://www.w3.org/2000/01/rdf-schema#label)
        /// </summary>
        [XmlElement(Order=2)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        /// <summary>
        /// Represents the IRI of the entity to which this annotation axiom is referred (e.g: http://xmlns.com/foaf/0.1/Person)
        /// </summary>
        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string SubjectIRI { get; set; }

        //AnnotationValue (cannot be a self-object, since this would introduce an additional XmlElement)

        /// <summary>
        /// Represents the annotation value to be used in case of IRI (e.g: http://xmlns.com/foaf/0.1/Person)
        /// </summary>
        [XmlElement("IRI", DataType="anyURI", Order=4)]
        public string ValueIRI { get; set; }

        /// <summary>
        /// Represents the annotation value to be used in case of literal (e.g: "Person")
        /// </summary>
        [XmlElement("Literal", Order=5)]
        public OWLLiteral ValueLiteral { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationAssertion() { }

        /// <summary>
        /// Builds an OWLAnnotationAssertion with the given annotation property and owner entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri) : this()
        {
            AnnotationProperty = annotationProperty ?? throw new OWLException($"Cannot create OWLAnnotationAssertion because given '{nameof(annotationProperty)}' parameter is null");
            SubjectIRI = subjectIri?.ToString() ?? throw new OWLException($"Cannot create OWLAnnotationAssertion because given '{nameof(subjectIri)}' parameter is null");
        }

        /// <summary>
        /// Builds an OWLAnnotationAssertion with the given annotation property, owner entity and target entity
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, RDFResource valueIri) : this(annotationProperty, subjectIri)
            => ValueIRI = valueIri?.ToString() ?? throw new OWLException($"Cannot create OWLAnnotationAssertion because given '{nameof(valueIri)}' parameter is null");

        /// <summary>
        /// Builds an OWLAnnotationAssertion with the given annotation property, owner entity and literal value
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAnnotationAssertion(OWLAnnotationProperty annotationProperty, RDFResource subjectIri, OWLLiteral valueLiteral) : this(annotationProperty, subjectIri)
            => ValueLiteral = valueLiteral ?? throw new OWLException($"Cannot create OWLAnnotationAssertion because given '{nameof(valueLiteral)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLAnnotationAssertion to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = AnnotationProperty.ToRDFGraph();

            //Axiom Triple
            RDFTriple axiomTriple = !string.IsNullOrEmpty(ValueIRI)
                ? new RDFTriple(new RDFResource(SubjectIRI), AnnotationProperty.GetIRI(), new RDFResource(ValueIRI))
                : new RDFTriple(new RDFResource(SubjectIRI), AnnotationProperty.GetIRI(), ValueLiteral.GetLiteral());
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}