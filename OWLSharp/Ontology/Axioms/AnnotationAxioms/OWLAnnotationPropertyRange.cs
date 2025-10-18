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
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLAnnotationPropertyRange axiom suggests the intended type of values for an annotation property,
    /// indicating whether the property typically takes IRIs, literals, or other specific value types.
    /// For example, AnnotationPropertyRange(rdfs:label rdfs:Literal) suggests that rdfs:label should have
    /// literal values, though this axiom has no formal semantic constraints and serves purely as documentation
    /// to guide ontology developers in providing appropriate annotation values.
    /// </summary>
    [XmlRoot("AnnotationPropertyRange")]
    public sealed class OWLAnnotationPropertyRange : OWLAnnotationAxiom
    {
        #region Properties
        /// <summary>
        /// Represents the property used for this annotation axiom (e.g: http://www.w3.org/2000/01/rdf-schema#label)
        /// </summary>
        [XmlElement(ElementName="AnnotationProperty", Order=2)]
        public OWLAnnotationProperty AnnotationProperty { get; set; }

        /// <summary>
        /// Represents the IRI of the class/datarange expression being range of the annotation property (e.g: http://www.w3.org/2000/01/rdf-schema#Literal)
        /// </summary>
        [XmlElement("IRI", DataType="anyURI", Order=3)]
        public string IRI { get; set; }

        /// <summary>
        /// Represents the xsd:qualifiedName of the class expression being domain of the annotation property (e.g: rdfs:Literal)
        /// </summary>
        [XmlElement("AbbreviatedIRI", DataType="QName", Order=4)]
        public XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLAnnotationPropertyRange() { }

        /// <summary>
        /// Builds an OWLAnnotationPropertyRange with the given annotation property
        /// </summary>
        /// <exception cref="OWLException"></exception>
        internal OWLAnnotationPropertyRange(OWLAnnotationProperty annotationProperty) : this()
            => AnnotationProperty = annotationProperty ?? throw new OWLException($"Cannot create OWLAnnotationPropertyRange because given '{nameof(annotationProperty)}' parameter is null");

        /// <summary>
        /// Builds an OWLAnnotationPropertyRange with the given annotation property and class/datarange expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAnnotationPropertyRange(OWLAnnotationProperty annotationProperty, RDFResource iri) : this(annotationProperty)
            => IRI = iri?.ToString() ?? throw new OWLException($"Cannot create OWLAnnotationPropertyRange because given '{nameof(iri)}' parameter is null");

        /// <summary>
        /// Builds an OWLAnnotationPropertyRange with the given annotation property and class/datarange expression
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAnnotationPropertyRange(OWLAnnotationProperty annotationProperty, XmlQualifiedName abbreviatedIRI) : this(annotationProperty)
            => AbbreviatedIRI = abbreviatedIRI ?? throw new OWLException($"Cannot create OWLAnnotationPropertyRange because given '{nameof(abbreviatedIRI)}' parameter is null");
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLAnnotationPropertyRange to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = AnnotationProperty.ToRDFGraph();

            //Axiom Triple
            string rangeIRI = IRI;
            if (string.IsNullOrEmpty(rangeIRI))
                rangeIRI = string.Concat(AbbreviatedIRI.Namespace, AbbreviatedIRI.Name);
            RDFTriple axiomTriple = new RDFTriple(AnnotationProperty.GetIRI(), RDFVocabulary.RDFS.RANGE, new RDFResource(rangeIRI));
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}